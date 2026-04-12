import { v4 as uuidv4 } from 'uuid';
import { EventEmitter } from 'node:events';
import { getDb, backupDb } from './database.js';
import { settingHelper } from './settingHelper.js';
import type { ZupTask, UpdateEntryArgs } from '../../src/types/index.js';
import { BrowserWindow } from 'electron';

export const taskEvents = new EventEmitter();

// ------- Row types for better-sqlite3 results -------
interface TaskRow {
	ID: string;
	Task: string;
	CreatedOn: string;
	StartedOn: string | null;
	EndedOn: string | null;
	Reminder: string | null;
	Rank: number | null;
}
interface TagRow {
	TagID: string;
	Name: string;
	CreatedOn: string;
}
interface NoteRow {
	ID: string;
	TaskID: string;
	Notes: string;
	RTF: string | null;
	CreatedOn: string;
	UpdatedOn: string | null;
}

// ------- In-memory state -------
const tasks = new Map<string, ZupTask>();
const runningIDs = new Set<string>();

// ------- IPC push helpers -------
function broadcast(channel: string, payload?: unknown): void {
	BrowserWindow.getAllWindows().forEach((w) => {
		if (!w.isDestroyed()) w.webContents.send(channel, payload);
	});
}

// ------- Initialise from DB -------
export function loadTasks(): void {
	tasks.clear();
	runningIDs.clear();

	const db = getDb();
	const minDate = new Date();
	minDate.setDate(minDate.getDate() - settingHelper.NumDaysOfDataToLoad);
	const minIso = minDate.toISOString();

	const rows = db
		.prepare(
			`
    SELECT * FROM tbl_TaskEntry
    WHERE CreatedOn >= ? OR StartedOn IS NULL
  `,
		)
		.all(minIso) as TaskRow[];

	for (const r of rows) {
		const t = rowToTask(r);
		tasks.set(t.id, t);
	}
}

function rowToTask(r: TaskRow): ZupTask {
	return {
		id: r.ID,
		task: r.Task,
		createdOn: r.CreatedOn,
		startedOn: r.StartedOn,
		endedOn: r.EndedOn,
		reminder: r.Reminder,
		rank: r.Rank,
		isRunning: false,
		tags: [],
	};
}

function loadTagsForTask(taskId: string): string[] {
	const db = getDb();
	const rows = db
		.prepare(
			`
    SELECT t.Name FROM tbl_Tag t
    JOIN tbl_TaskEntryTag tet ON tet.TagID = t.ID
    WHERE tet.TaskID = ?
    ORDER BY tet.CreatedOn DESC
  `,
		)
		.all(taskId) as { Name: string }[];
	return rows.map((r) => r.Name);
}

export function getAllTasks(): ZupTask[] {
	return Array.from(tasks.values());
}

export function getRunningIds(): string[] {
	return Array.from(runningIDs);
}

export function findTask(id: string): ZupTask | undefined {
	return tasks.get(id);
}

// ------- Mutations -------

export function startTask(
	text: string,
	startNow: boolean,
	stopOtherTasks: boolean,
	hideParent: boolean,
	bringNotes: boolean,
	bringTags: boolean,
	getTags: boolean,
	parentEntryId?: string | null,
): ZupTask {
	const db = getDb();
	backupDb();

	const now = new Date().toISOString();
	const newId = uuidv4();

	db.prepare(
		`
    INSERT INTO tbl_TaskEntry (ID, Task, CreatedOn, StartedOn)
    VALUES (?, ?, ?, ?)
  `,
	).run(newId, text, now, startNow ? now : null);

	const parentEntry = parentEntryId
		? (db.prepare('SELECT * FROM tbl_TaskEntry WHERE ID = ?').get(parentEntryId) as TaskRow | undefined)
		: null;

	if (parentEntry) {
		if (bringNotes) {
			const notes = db.prepare('SELECT * FROM tbl_TaskEntryNote WHERE TaskID = ?').all(parentEntry.ID) as NoteRow[];
			for (const note of notes) {
				db.prepare(
					`
          INSERT INTO tbl_TaskEntryNote (ID, TaskID, Notes, RTF, CreatedOn, UpdatedOn)
          VALUES (?, ?, ?, ?, ?, ?)
        `,
				).run(uuidv4(), newId, note.Notes, note.RTF, note.CreatedOn, note.UpdatedOn);
			}
		}

		if (bringTags) {
			const tags = db.prepare('SELECT * FROM tbl_TaskEntryTag WHERE TaskID = ?').all(parentEntry.ID) as TagRow[];
			for (const tag of tags) {
				db.prepare(
					`
          INSERT OR IGNORE INTO tbl_TaskEntryTag (TaskID, TagID, CreatedOn) VALUES (?, ?, ?)
        `,
				).run(newId, tag.TagID, tag.CreatedOn);
			}
		}

		if (hideParent) {
			db.prepare('DELETE FROM tbl_TaskEntry WHERE ID = ?').run(parentEntry.ID);
			tasks.delete(parentEntry.ID);
		}
	}

	// Auto-assign previous tags for recurring task names
	if (getTags && !parentEntry) {
		const minDate = new Date();
		minDate.setDate(minDate.getDate() - settingHelper.NumDaysOfDataToLoad);
		const recentTagIds = db
			.prepare(
				`
      SELECT DISTINCT tet.TagID FROM tbl_TaskEntry te
      JOIN tbl_TaskEntryTag tet ON tet.TaskID = te.ID
      WHERE te.Task = ? AND te.CreatedOn >= ?
      ORDER BY tet.CreatedOn DESC
    `,
			)
			.all(text, minDate.toISOString()) as { TagID: string }[];

		for (const { TagID } of recentTagIds) {
			db.prepare('INSERT OR IGNORE INTO tbl_TaskEntryTag (TaskID, TagID, CreatedOn) VALUES (?, ?, ?)').run(
				newId,
				TagID,
				now,
			);
		}
	}

	const newTask: ZupTask = {
		id: newId,
		task: text,
		createdOn: now,
		startedOn: startNow ? now : null,
		endedOn: null,
		reminder: null,
		rank: null,
		isRunning: startNow,
		tags: loadTagsForTask(newId),
	};

	tasks.set(newId, newTask);
	if (startNow) runningIDs.add(newId);

	if (startNow && stopOtherTasks) {
		for (const rid of Array.from(runningIDs)) {
			if (rid !== newId) stopTask(rid);
		}
	}

	broadcast('task:started', newTask);
	if (startNow) taskEvents.emit('task:started', newTask);
	return newTask;
}

export function stopTask(taskId: string, endOn?: string): ZupTask | null {
	const task = tasks.get(taskId);
	if (!task) return null;

	task.endedOn = endOn ?? new Date().toISOString();
	task.isRunning = false;
	runningIDs.delete(taskId);

	getDb().prepare('UPDATE tbl_TaskEntry SET EndedOn = ? WHERE ID = ?').run(task.endedOn, taskId);

	broadcast('task:stopped', task);
	taskEvents.emit('task:stopped', task);
	return task;
}

export function deleteTask(taskId: string): void {
	getDb().prepare('DELETE FROM tbl_TaskEntry WHERE ID = ?').run(taskId);
	const task = tasks.get(taskId);
	tasks.delete(taskId);
	runningIDs.delete(taskId);
	if (task) broadcast('task:deleted', task);
}

export function updateTask(taskId: string, args: UpdateEntryArgs): ZupTask | null {
	const db = getDb();
	const existing = tasks.get(taskId);
	if (!existing) return null;

	db.prepare(
		`
    UPDATE tbl_TaskEntry SET Task = ?, StartedOn = ?, EndedOn = ?, Rank = ? WHERE ID = ?
  `,
	).run(args.task, args.startedOn, args.endedOn, args.rank, taskId);

	saveTags(taskId, args.tags);

	existing.task = args.task;
	existing.startedOn = args.startedOn;
	existing.endedOn = args.endedOn;
	existing.rank = args.rank;
	existing.tags = args.tags;

	broadcast('task:updated', existing);
	return existing;
}

export function resumeTask(taskId: string): ZupTask | null {
	const task = tasks.get(taskId);
	if (!task) return null;

	const now = new Date().toISOString();
	task.startedOn = now;
	task.isRunning = true;
	runningIDs.add(taskId);

	getDb().prepare('UPDATE tbl_TaskEntry SET StartedOn = ? WHERE ID = ?').run(now, taskId);

	broadcast('task:started', task);
	taskEvents.emit('task:started', task);
	return task;
}

export function resetTask(taskId: string): ZupTask | null {
	const task = tasks.get(taskId);
	if (!task) return null;

	const now = new Date().toISOString();
	task.startedOn = now;
	getDb().prepare('UPDATE tbl_TaskEntry SET StartedOn = ? WHERE ID = ?').run(now, taskId);

	broadcast('task:updated', task);
	return task;
}

export function trimTasks(daysToKeep: number): number {
	backupDb();
	const db = getDb();
	const keepDate = new Date();
	keepDate.setDate(keepDate.getDate() - daysToKeep);
	const iso = keepDate.toISOString();

	db.prepare(
		`
    DELETE FROM tbl_TaskEntryNote WHERE TaskID IN (
      SELECT ID FROM tbl_TaskEntry WHERE StartedOn < ?
    )
  `,
	).run(iso);

	const result = db.prepare('DELETE FROM tbl_TaskEntry WHERE StartedOn < ?').run(iso);
	db.exec('VACUUM');

	const count = result.changes;
	loadTasks();
	return count;
}

function saveTags(taskId: string, tags: string[]): void {
	const db = getDb();

	const existingTags = db
		.prepare(
			`
    SELECT t.ID, t.Name FROM tbl_Tag t
    JOIN tbl_TaskEntryTag tet ON tet.TagID = t.ID
    WHERE tet.TaskID = ?
  `,
		)
		.all(taskId) as { ID: string; Name: string }[];

	// Remove tags no longer present
	for (const e of existingTags) {
		if (!tags.includes(e.Name)) {
			db.prepare('DELETE FROM tbl_TaskEntryTag WHERE TaskID = ? AND TagID = ?').run(taskId, e.ID);
		}
	}

	// Add new tags
	const allTags = db
		.prepare('SELECT ID, Name FROM tbl_Tag WHERE Name IN (' + tags.map(() => '?').join(',') + ')')
		.all(...tags) as { ID: string; Name: string }[];
	const tagMap = new Map(allTags.map((t) => [t.Name, t.ID]));

	for (const name of [...new Set(tags)]) {
		if (existingTags.some((e) => e.Name === name)) continue;

		let tagId = tagMap.get(name);
		if (!tagId) {
			tagId = uuidv4();
			db.prepare('INSERT INTO tbl_Tag (ID, Name) VALUES (?, ?)').run(tagId, name);
		}
		db.prepare('INSERT OR IGNORE INTO tbl_TaskEntryTag (TaskID, TagID, CreatedOn) VALUES (?, ?, ?)').run(
			taskId,
			tagId,
			new Date().toISOString(),
		);
	}
}

// ------- Notes -------

export function getNotes(
	taskId: string,
): { id: string; notes: string; rtf: string | null; createdOn: string; updatedOn: string | null; summary: string }[] {
	const rows = getDb()
		.prepare('SELECT * FROM tbl_TaskEntryNote WHERE TaskID = ? ORDER BY CreatedOn ASC')
		.all(taskId) as NoteRow[];
	return rows.map((r) => ({
		id: r.ID,
		notes: r.Notes,
		rtf: r.RTF,
		createdOn: r.CreatedOn,
		updatedOn: r.UpdatedOn,
		summary: r.Notes.slice(0, 60).replace(/\s+/g, ' '),
	}));
}

export function saveNote(taskId: string, noteId: string | null, notes: string, rtf: string): { id: string } | null {
	const db = getDb();
	if (noteId) {
		if (!notes.trim()) {
			db.prepare('DELETE FROM tbl_TaskEntryNote WHERE ID = ?').run(noteId);
			return null;
		}
		db.prepare('UPDATE tbl_TaskEntryNote SET Notes = ?, RTF = ?, UpdatedOn = ? WHERE ID = ?').run(
			notes,
			rtf,
			new Date().toISOString(),
			noteId,
		);
		return { id: noteId };
	} else {
		if (!notes.trim()) return null;
		const id = uuidv4();
		db.prepare('INSERT INTO tbl_TaskEntryNote (ID, TaskID, Notes, RTF, CreatedOn) VALUES (?, ?, ?, ?, ?)').run(
			id,
			taskId,
			notes,
			rtf,
			new Date().toISOString(),
		);
		return { id };
	}
}

export function deleteNote(noteId: string): void {
	getDb().prepare('DELETE FROM tbl_TaskEntryNote WHERE ID = ?').run(noteId);
}

// ------- Tags -------

export function getAllTags(): { id: string; name: string; description: string | null }[] {
	return (
		getDb().prepare('SELECT * FROM tbl_Tag ORDER BY Name').all() as { ID: string; Name: string; Description: string | null }[]
	).map((r) => ({ id: r.ID, name: r.Name, description: r.Description }));
}

export function updateTag(tagId: string, name: string, description: string): void {
	getDb().prepare('UPDATE tbl_Tag SET Name = ?, Description = ? WHERE ID = ?').run(name, description, tagId);
}

export function deleteTag(tagId: string): boolean {
	const inUse = getDb().prepare('SELECT 1 FROM tbl_TaskEntryTag WHERE TagID = ? LIMIT 1').get(tagId);
	if (inUse) return false;
	getDb().prepare('DELETE FROM tbl_Tag WHERE ID = ?').run(tagId);
	return true;
}

// ------- ViewList queries -------

export function queryTasks(from: string, to: string, search?: string): ZupTask[] {
	const db = getDb();
	const runIds = Array.from(runningIDs);

	let rows: TaskRow[];
	if (search && search.trim()) {
		const q = `%${search.toLowerCase()}%`;
		rows = db.prepare(`SELECT * FROM tbl_TaskEntry WHERE lower(Task) LIKE ? ORDER BY StartedOn DESC`).all(q) as TaskRow[];
	} else {
		rows = db
			.prepare(
				`
      SELECT * FROM tbl_TaskEntry
      WHERE (StartedOn >= ? AND StartedOn <= ?) OR StartedOn IS NULL
      ORDER BY StartedOn DESC
    `,
			)
			.all(from, to) as TaskRow[];
	}

	return rows.map((r) => {
		const tagRows = db
			.prepare(
				`
      SELECT t.Name FROM tbl_Tag t JOIN tbl_TaskEntryTag tet ON tet.TagID = t.ID
      WHERE tet.TaskID = ? ORDER BY tet.CreatedOn DESC
    `,
			)
			.all(r.ID) as { Name: string }[];

		let duration: number | null = null;
		let durationString: string | null = null;
		if (r.StartedOn && r.EndedOn) {
			duration = (new Date(r.EndedOn).getTime() - new Date(r.StartedOn).getTime()) / 1000;
			const h = Math.floor(duration / 3600);
			const m = Math.floor((duration % 3600) / 60);
			const s = Math.floor(duration % 60);
			durationString = `${String(h).padStart(2, '0')}:${String(m).padStart(2, '0')}:${String(s).padStart(2, '0')}`;
		}

		return {
			id: r.ID,
			task: r.Task,
			createdOn: r.CreatedOn,
			startedOn: r.StartedOn,
			endedOn: r.EndedOn,
			reminder: r.Reminder,
			rank: r.Rank,
			isRunning: runIds.includes(r.ID),
			tags: tagRows.map((t) => t.Name),
			duration,
			durationString,
		};
	});
}
