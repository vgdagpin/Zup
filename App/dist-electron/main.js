import { app, BrowserWindow, ipcMain, shell, screen, Tray, Menu, nativeTheme, nativeImage, globalShortcut } from 'electron';
import { fileURLToPath } from 'node:url';
import path from 'node:path';
import Database from 'better-sqlite3';
import fs from 'node:fs';
import { EventEmitter } from 'node:events';
import { randomFillSync, randomUUID } from 'node:crypto';
let db = null;
let dbFilePath = '';
function getDefaultDbPath() {
	const docs = app.getPath('documents');
	const dir = path.join(docs, 'Zup');
	if (!fs.existsSync(dir)) fs.mkdirSync(dir, { recursive: true });
	return path.join(dir, 'Zup.db');
}
function getDb() {
	if (!db) throw new Error('Database not initialized. Call initDb() first.');
	return db;
}
function getCurrentDbPath() {
	return dbFilePath;
}
function initDb(filePath) {
	const p = filePath ?? getDefaultDbPath();
	dbFilePath = p;
	const dir = path.dirname(p);
	if (!fs.existsSync(dir)) fs.mkdirSync(dir, { recursive: true });
	db = new Database(p);
	db.pragma('journal_mode = WAL');
	db.pragma('foreign_keys = ON');
	bootstrapSchema(db);
	runMigrations(db);
	return db;
}
function reopenDb(filePath) {
	if (db) {
		db.close();
		db = null;
	}
	initDb(filePath);
}
function backupDb() {
	if (!dbFilePath || !fs.existsSync(dbFilePath)) return null;
	const dir = path.join(path.dirname(dbFilePath), 'Backup');
	if (!fs.existsSync(dir)) fs.mkdirSync(dir, { recursive: true });
	const stamp = /* @__PURE__ */ new Date().toISOString().replace(/[-:T]/g, '').slice(0, 14);
	const newName = `${path.basename(dbFilePath, '.db')}-${stamp}-bak.db`;
	const backupPath = path.join(dir, newName);
	if (!fs.existsSync(backupPath)) {
		fs.copyFileSync(dbFilePath, backupPath);
	}
	return dir;
}
function bootstrapSchema(d) {
	d.exec(`
    CREATE TABLE IF NOT EXISTS tbl_MigrationHistory (
      MigrationId TEXT PRIMARY KEY,
      AppliedAt   TEXT NOT NULL
    );

    CREATE TABLE IF NOT EXISTS tbl_Setting (
      Name     TEXT PRIMARY KEY,
      Value    TEXT,
      DataType TEXT
    );

    CREATE TABLE IF NOT EXISTS tbl_Tag (
      ID          TEXT PRIMARY KEY,
      Name        TEXT NOT NULL,
      Description TEXT
    );

    CREATE TABLE IF NOT EXISTS tbl_TaskEntry (
      ID        TEXT PRIMARY KEY,
      Task      TEXT NOT NULL,
      CreatedOn TEXT NOT NULL,
      StartedOn TEXT,
      EndedOn   TEXT,
      Reminder  TEXT,
      Rank      INTEGER
    );

    CREATE TABLE IF NOT EXISTS tbl_TaskEntryNote (
      ID        TEXT PRIMARY KEY,
      TaskID    TEXT NOT NULL,
      Notes     TEXT NOT NULL,
      RTF       TEXT,
      CreatedOn TEXT NOT NULL,
      UpdatedOn TEXT,
      FOREIGN KEY (TaskID) REFERENCES tbl_TaskEntry(ID)
    );

    CREATE TABLE IF NOT EXISTS tbl_TaskEntryTag (
      TaskID    TEXT NOT NULL,
      TagID     TEXT NOT NULL,
      CreatedOn TEXT NOT NULL,
      PRIMARY KEY (TaskID, TagID),
      FOREIGN KEY (TaskID) REFERENCES tbl_TaskEntry(ID),
      FOREIGN KEY (TagID)  REFERENCES tbl_Tag(ID)
    );
  `);
}
function runMigrations(d) {
	const applied = new Set(
		d
			.prepare('SELECT MigrationId FROM tbl_MigrationHistory')
			.all()
			.map((r) => r.MigrationId),
	);
	const migrations = [
		// Add future schema changes here
		// { id: '20250001_example', sql: 'ALTER TABLE ...' }
	];
	for (const m of migrations) {
		if (!applied.has(m.id)) {
			d.exec(m.sql);
			d.prepare('INSERT INTO tbl_MigrationHistory (MigrationId, AppliedAt) VALUES (?, ?)').run(
				m.id,
				/* @__PURE__ */ new Date().toISOString(),
			);
		}
	}
}
const DEFAULTS = {
	ShowQueuedTasks: 'false',
	ShowRankedTasks: 'false',
	ShowClosedTasks: 'false',
	EntryListOpacity: '1',
	NumDaysOfDataToLoad: '30',
	ItemsToShow: '5',
	AutoOpenUpdateWindow: 'false',
	UsePillTimer: 'true',
	DayStart: '07:00',
	DayEnd: '18:00',
	DayEndNextDay: 'false',
	TimesheetsFolder: '',
	ExportRowFormat: '~StartedOnTicks~^~Task~^~Comments~^~Tag[Name=Bill%].Description~^~Duration~^False^False',
	ExportFileExtension: 'csv',
	TrimDaysToKeep: '90',
	FormLocationX: '0',
	FormLocationY: '0',
};
const userDataFile = () => path.join(app.getPath('userData'), 'zup-config.json');
function readUserData() {
	try {
		const f = userDataFile();
		if (fs.existsSync(f)) return JSON.parse(fs.readFileSync(f, 'utf-8'));
	} catch {}
	return {};
}
function writeUserData(data) {
	fs.writeFileSync(userDataFile(), JSON.stringify(data, null, 2));
}
function getSetting(name) {
	const row = getDb().prepare('SELECT Value FROM tbl_Setting WHERE Name = ?').get(name);
	return (row == null ? void 0 : row.Value) ?? null;
}
function setSetting(name, value) {
	const existing = getDb().prepare('SELECT Name FROM tbl_Setting WHERE Name = ?').get(name);
	if (existing) {
		getDb().prepare('UPDATE tbl_Setting SET Value = ? WHERE Name = ?').run(value, name);
	} else {
		getDb().prepare('INSERT INTO tbl_Setting (Name, Value, DataType) VALUES (?, ?, ?)').run(name, value, 'string');
	}
}
function get(name) {
	return getSetting(name) ?? DEFAULTS[name] ?? '';
}
function set(name, value) {
	setSetting(name, value);
}
const settingHelper = {
	get ShowQueuedTasks() {
		return get('ShowQueuedTasks') === 'true';
	},
	set ShowQueuedTasks(v) {
		set('ShowQueuedTasks', String(v));
	},
	get ShowRankedTasks() {
		return get('ShowRankedTasks') === 'true';
	},
	set ShowRankedTasks(v) {
		set('ShowRankedTasks', String(v));
	},
	get ShowClosedTasks() {
		return get('ShowClosedTasks') === 'true';
	},
	set ShowClosedTasks(v) {
		set('ShowClosedTasks', String(v));
	},
	get EntryListOpacity() {
		return parseFloat(get('EntryListOpacity'));
	},
	set EntryListOpacity(v) {
		set('EntryListOpacity', String(v));
	},
	get NumDaysOfDataToLoad() {
		return parseInt(get('NumDaysOfDataToLoad'));
	},
	set NumDaysOfDataToLoad(v) {
		set('NumDaysOfDataToLoad', String(v));
	},
	get ItemsToShow() {
		return parseInt(get('ItemsToShow'));
	},
	set ItemsToShow(v) {
		set('ItemsToShow', String(v));
	},
	get AutoOpenUpdateWindow() {
		return get('AutoOpenUpdateWindow') === 'true';
	},
	set AutoOpenUpdateWindow(v) {
		set('AutoOpenUpdateWindow', String(v));
	},
	get UsePillTimer() {
		return get('UsePillTimer') === 'true';
	},
	set UsePillTimer(v) {
		set('UsePillTimer', String(v));
	},
	get DayStart() {
		return get('DayStart');
	},
	set DayStart(v) {
		set('DayStart', v);
	},
	get DayEnd() {
		return get('DayEnd');
	},
	set DayEnd(v) {
		set('DayEnd', v);
	},
	get DayEndNextDay() {
		return get('DayEndNextDay') === 'true';
	},
	set DayEndNextDay(v) {
		set('DayEndNextDay', String(v));
	},
	get TimesheetsFolder() {
		return get('TimesheetsFolder');
	},
	set TimesheetsFolder(v) {
		set('TimesheetsFolder', v);
	},
	get ExportRowFormat() {
		return get('ExportRowFormat');
	},
	set ExportRowFormat(v) {
		set('ExportRowFormat', v);
	},
	get ExportFileExtension() {
		return get('ExportFileExtension');
	},
	set ExportFileExtension(v) {
		set('ExportFileExtension', v);
	},
	get TrimDaysToKeep() {
		return parseInt(get('TrimDaysToKeep'));
	},
	set TrimDaysToKeep(v) {
		set('TrimDaysToKeep', String(v));
	},
	get FormLocationX() {
		return parseInt(get('FormLocationX'));
	},
	set FormLocationX(v) {
		set('FormLocationX', String(v));
	},
	get FormLocationY() {
		return parseInt(get('FormLocationY'));
	},
	set FormLocationY(v) {
		set('FormLocationY', String(v));
	},
	get DbPath() {
		const ud = readUserData();
		if (ud.DbPath) return ud.DbPath;
		return getDefaultDbPath();
	},
	set DbPath(v) {
		const ud = readUserData();
		ud.DbPath = v;
		writeUserData(ud);
	},
};
const byteToHex = [];
for (let i = 0; i < 256; ++i) {
	byteToHex.push((i + 256).toString(16).slice(1));
}
function unsafeStringify(arr, offset = 0) {
	return (
		byteToHex[arr[offset + 0]] +
		byteToHex[arr[offset + 1]] +
		byteToHex[arr[offset + 2]] +
		byteToHex[arr[offset + 3]] +
		'-' +
		byteToHex[arr[offset + 4]] +
		byteToHex[arr[offset + 5]] +
		'-' +
		byteToHex[arr[offset + 6]] +
		byteToHex[arr[offset + 7]] +
		'-' +
		byteToHex[arr[offset + 8]] +
		byteToHex[arr[offset + 9]] +
		'-' +
		byteToHex[arr[offset + 10]] +
		byteToHex[arr[offset + 11]] +
		byteToHex[arr[offset + 12]] +
		byteToHex[arr[offset + 13]] +
		byteToHex[arr[offset + 14]] +
		byteToHex[arr[offset + 15]]
	).toLowerCase();
}
const rnds8Pool = new Uint8Array(256);
let poolPtr = rnds8Pool.length;
function rng() {
	if (poolPtr > rnds8Pool.length - 16) {
		randomFillSync(rnds8Pool);
		poolPtr = 0;
	}
	return rnds8Pool.slice(poolPtr, (poolPtr += 16));
}
const native = { randomUUID };
function _v4(options, buf, offset) {
	var _a;
	options = options || {};
	const rnds = options.random ?? ((_a = options.rng) == null ? void 0 : _a.call(options)) ?? rng();
	if (rnds.length < 16) {
		throw new Error('Random bytes length must be >= 16');
	}
	rnds[6] = (rnds[6] & 15) | 64;
	rnds[8] = (rnds[8] & 63) | 128;
	return unsafeStringify(rnds);
}
function v4(options, buf, offset) {
	if (native.randomUUID && true && !options) {
		return native.randomUUID();
	}
	return _v4(options);
}
const taskEvents = new EventEmitter();
const tasks = /* @__PURE__ */ new Map();
const runningIDs = /* @__PURE__ */ new Set();
function broadcast(channel, payload) {
	BrowserWindow.getAllWindows().forEach((w) => {
		if (!w.isDestroyed()) w.webContents.send(channel, payload);
	});
}
function loadTasks() {
	tasks.clear();
	runningIDs.clear();
	const db2 = getDb();
	const minDate = /* @__PURE__ */ new Date();
	minDate.setDate(minDate.getDate() - settingHelper.NumDaysOfDataToLoad);
	const minIso = minDate.toISOString();
	const rows = db2
		.prepare(
			`
    SELECT * FROM tbl_TaskEntry
    WHERE CreatedOn >= ? OR StartedOn IS NULL
  `,
		)
		.all(minIso);
	for (const r of rows) {
		const t = rowToTask(r);
		tasks.set(t.id, t);
	}
}
function rowToTask(r) {
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
function loadTagsForTask(taskId) {
	const db2 = getDb();
	const rows = db2
		.prepare(
			`
    SELECT t.Name FROM tbl_Tag t
    JOIN tbl_TaskEntryTag tet ON tet.TagID = t.ID
    WHERE tet.TaskID = ?
    ORDER BY tet.CreatedOn DESC
  `,
		)
		.all(taskId);
	return rows.map((r) => r.Name);
}
function getAllTasks() {
	return Array.from(tasks.values());
}
function getRunningIds() {
	return Array.from(runningIDs);
}
function startTask(text, startNow, stopOtherTasks, hideParent, bringNotes, bringTags, getTags, parentEntryId) {
	const db2 = getDb();
	backupDb();
	const now = /* @__PURE__ */ new Date().toISOString();
	const newId = v4();
	db2.prepare(
		`
    INSERT INTO tbl_TaskEntry (ID, Task, CreatedOn, StartedOn)
    VALUES (?, ?, ?, ?)
  `,
	).run(newId, text, now, startNow ? now : null);
	const parentEntry = parentEntryId ? db2.prepare('SELECT * FROM tbl_TaskEntry WHERE ID = ?').get(parentEntryId) : null;
	if (parentEntry) {
		if (bringNotes) {
			const notes = db2.prepare('SELECT * FROM tbl_TaskEntryNote WHERE TaskID = ?').all(parentEntry.ID);
			for (const note of notes) {
				db2.prepare(
					`
          INSERT INTO tbl_TaskEntryNote (ID, TaskID, Notes, RTF, CreatedOn, UpdatedOn)
          VALUES (?, ?, ?, ?, ?, ?)
        `,
				).run(v4(), newId, note.Notes, note.RTF, note.CreatedOn, note.UpdatedOn);
			}
		}
		if (bringTags) {
			const tags = db2.prepare('SELECT * FROM tbl_TaskEntryTag WHERE TaskID = ?').all(parentEntry.ID);
			for (const tag of tags) {
				db2.prepare(
					`
          INSERT OR IGNORE INTO tbl_TaskEntryTag (TaskID, TagID, CreatedOn) VALUES (?, ?, ?)
        `,
				).run(newId, tag.TagID, tag.CreatedOn);
			}
		}
		if (hideParent) {
			db2.prepare('DELETE FROM tbl_TaskEntry WHERE ID = ?').run(parentEntry.ID);
			tasks.delete(parentEntry.ID);
		}
	}
	if (getTags && !parentEntry) {
		const minDate = /* @__PURE__ */ new Date();
		minDate.setDate(minDate.getDate() - settingHelper.NumDaysOfDataToLoad);
		const recentTagIds = db2
			.prepare(
				`
      SELECT DISTINCT tet.TagID FROM tbl_TaskEntry te
      JOIN tbl_TaskEntryTag tet ON tet.TaskID = te.ID
      WHERE te.Task = ? AND te.CreatedOn >= ?
      ORDER BY tet.CreatedOn DESC
    `,
			)
			.all(text, minDate.toISOString());
		for (const { TagID } of recentTagIds) {
			db2.prepare('INSERT OR IGNORE INTO tbl_TaskEntryTag (TaskID, TagID, CreatedOn) VALUES (?, ?, ?)').run(
				newId,
				TagID,
				now,
			);
		}
	}
	const newTask = {
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
function stopTask(taskId, endOn) {
	const task = tasks.get(taskId);
	if (!task) return null;
	task.endedOn = endOn ?? /* @__PURE__ */ new Date().toISOString();
	task.isRunning = false;
	runningIDs.delete(taskId);
	getDb().prepare('UPDATE tbl_TaskEntry SET EndedOn = ? WHERE ID = ?').run(task.endedOn, taskId);
	broadcast('task:stopped', task);
	taskEvents.emit('task:stopped', task);
	return task;
}
function deleteTask(taskId) {
	getDb().prepare('DELETE FROM tbl_TaskEntry WHERE ID = ?').run(taskId);
	const task = tasks.get(taskId);
	tasks.delete(taskId);
	runningIDs.delete(taskId);
	if (task) broadcast('task:deleted', task);
}
function updateTask(taskId, args) {
	const db2 = getDb();
	const existing = tasks.get(taskId);
	if (!existing) return null;
	db2.prepare(
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
function resumeTask(taskId) {
	const task = tasks.get(taskId);
	if (!task) return null;
	const now = /* @__PURE__ */ new Date().toISOString();
	task.startedOn = now;
	task.isRunning = true;
	runningIDs.add(taskId);
	getDb().prepare('UPDATE tbl_TaskEntry SET StartedOn = ? WHERE ID = ?').run(now, taskId);
	broadcast('task:started', task);
	taskEvents.emit('task:started', task);
	return task;
}
function resetTask(taskId) {
	const task = tasks.get(taskId);
	if (!task) return null;
	const now = /* @__PURE__ */ new Date().toISOString();
	task.startedOn = now;
	getDb().prepare('UPDATE tbl_TaskEntry SET StartedOn = ? WHERE ID = ?').run(now, taskId);
	broadcast('task:updated', task);
	return task;
}
function trimTasks(daysToKeep) {
	backupDb();
	const db2 = getDb();
	const keepDate = /* @__PURE__ */ new Date();
	keepDate.setDate(keepDate.getDate() - daysToKeep);
	const iso = keepDate.toISOString();
	db2.prepare(
		`
    DELETE FROM tbl_TaskEntryNote WHERE TaskID IN (
      SELECT ID FROM tbl_TaskEntry WHERE StartedOn < ?
    )
  `,
	).run(iso);
	const result = db2.prepare('DELETE FROM tbl_TaskEntry WHERE StartedOn < ?').run(iso);
	db2.exec('VACUUM');
	const count = result.changes;
	loadTasks();
	return count;
}
function saveTags(taskId, tags) {
	const db2 = getDb();
	const existingTags = db2
		.prepare(
			`
    SELECT t.ID, t.Name FROM tbl_Tag t
    JOIN tbl_TaskEntryTag tet ON tet.TagID = t.ID
    WHERE tet.TaskID = ?
  `,
		)
		.all(taskId);
	for (const e of existingTags) {
		if (!tags.includes(e.Name)) {
			db2.prepare('DELETE FROM tbl_TaskEntryTag WHERE TaskID = ? AND TagID = ?').run(taskId, e.ID);
		}
	}
	const allTags = db2
		.prepare('SELECT ID, Name FROM tbl_Tag WHERE Name IN (' + tags.map(() => '?').join(',') + ')')
		.all(...tags);
	const tagMap = new Map(allTags.map((t) => [t.Name, t.ID]));
	for (const name of [...new Set(tags)]) {
		if (existingTags.some((e) => e.Name === name)) continue;
		let tagId = tagMap.get(name);
		if (!tagId) {
			tagId = v4();
			db2.prepare('INSERT INTO tbl_Tag (ID, Name) VALUES (?, ?)').run(tagId, name);
		}
		db2.prepare('INSERT OR IGNORE INTO tbl_TaskEntryTag (TaskID, TagID, CreatedOn) VALUES (?, ?, ?)').run(
			taskId,
			tagId,
			/* @__PURE__ */ new Date().toISOString(),
		);
	}
}
function getNotes(taskId) {
	const rows = getDb().prepare('SELECT * FROM tbl_TaskEntryNote WHERE TaskID = ? ORDER BY CreatedOn ASC').all(taskId);
	return rows.map((r) => ({
		id: r.ID,
		notes: r.Notes,
		rtf: r.RTF,
		createdOn: r.CreatedOn,
		updatedOn: r.UpdatedOn,
		summary: r.Notes.slice(0, 60).replace(/\s+/g, ' '),
	}));
}
function saveNote(taskId, noteId, notes, rtf) {
	const db2 = getDb();
	if (noteId) {
		if (!notes.trim()) {
			db2.prepare('DELETE FROM tbl_TaskEntryNote WHERE ID = ?').run(noteId);
			return null;
		}
		db2.prepare('UPDATE tbl_TaskEntryNote SET Notes = ?, RTF = ?, UpdatedOn = ? WHERE ID = ?').run(
			notes,
			rtf,
			/* @__PURE__ */ new Date().toISOString(),
			noteId,
		);
		return { id: noteId };
	} else {
		if (!notes.trim()) return null;
		const id = v4();
		db2.prepare('INSERT INTO tbl_TaskEntryNote (ID, TaskID, Notes, RTF, CreatedOn) VALUES (?, ?, ?, ?, ?)').run(
			id,
			taskId,
			notes,
			rtf,
			/* @__PURE__ */ new Date().toISOString(),
		);
		return { id };
	}
}
function deleteNote(noteId) {
	getDb().prepare('DELETE FROM tbl_TaskEntryNote WHERE ID = ?').run(noteId);
}
function getAllTags() {
	return getDb()
		.prepare('SELECT * FROM tbl_Tag ORDER BY Name')
		.all()
		.map((r) => ({ id: r.ID, name: r.Name, description: r.Description }));
}
function updateTag(tagId, name, description) {
	getDb().prepare('UPDATE tbl_Tag SET Name = ?, Description = ? WHERE ID = ?').run(name, description, tagId);
}
function deleteTag(tagId) {
	const inUse = getDb().prepare('SELECT 1 FROM tbl_TaskEntryTag WHERE TagID = ? LIMIT 1').get(tagId);
	if (inUse) return false;
	getDb().prepare('DELETE FROM tbl_Tag WHERE ID = ?').run(tagId);
	return true;
}
function queryTasks(from, to, search) {
	const db2 = getDb();
	const runIds = Array.from(runningIDs);
	let rows;
	if (search && search.trim()) {
		const q = `%${search.toLowerCase()}%`;
		rows = db2.prepare(`SELECT * FROM tbl_TaskEntry WHERE lower(Task) LIKE ? ORDER BY StartedOn DESC`).all(q);
	} else {
		rows = db2
			.prepare(
				`
      SELECT * FROM tbl_TaskEntry
      WHERE (StartedOn >= ? AND StartedOn <= ?) OR StartedOn IS NULL
      ORDER BY StartedOn DESC
    `,
			)
			.all(from, to);
	}
	return rows.map((r) => {
		const tagRows = db2
			.prepare(
				`
      SELECT t.Name FROM tbl_Tag t JOIN tbl_TaskEntryTag tet ON tet.TagID = t.ID
      WHERE tet.TaskID = ? ORDER BY tet.CreatedOn DESC
    `,
			)
			.all(r.ID);
		let duration = null;
		let durationString = null;
		if (r.StartedOn && r.EndedOn) {
			duration = (new Date(r.EndedOn).getTime() - new Date(r.StartedOn).getTime()) / 1e3;
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
function getWeekNumber(date) {
	const d = new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()));
	d.setUTCDate(d.getUTCDate() + 4 - (d.getUTCDay() || 7));
	const yearStart = new Date(Date.UTC(d.getUTCFullYear(), 0, 1));
	return Math.ceil(((d.getTime() - yearStart.getTime()) / 864e5 + 1) / 7);
}
function getWeekData(year) {
	const data = /* @__PURE__ */ new Map();
	const from = new Date(year, 0, 1);
	const to = new Date(year, 11, 31);
	for (let d = new Date(from); d <= to; d.setDate(d.getDate() + 1)) {
		const wn = getWeekNumber(d);
		if (data.has(wn)) continue;
		const day = d.getDay();
		const offset = day === 0 ? 0 : -day;
		const weekStart = new Date(d);
		weekStart.setDate(d.getDate() + offset);
		const weekEnd = new Date(weekStart);
		weekEnd.setDate(weekStart.getDate() + 6);
		weekEnd.setHours(23, 59, 59, 999);
		const fmt = (dt) =>
			`${String(dt.getMonth() + 1).padStart(2, '0')}/${String(dt.getDate()).padStart(2, '0')}/${String(dt.getFullYear()).slice(2)}`;
		data.set(wn, {
			weekNumber: wn,
			start: weekStart.toISOString(),
			end: weekEnd.toISOString(),
			label: `${fmt(weekStart)} - ${fmt(weekEnd)}`,
		});
	}
	return Array.from(data.values()).sort((a, b) => a.weekNumber - b.weekNumber);
}
function parseTagKey(raw) {
	if (!raw) return null;
	const tk = { key: raw };
	const match = raw.match(/^(\w+)(?:\[(\d+)\](?:\.(\w+))?|\[(\w+)=([^\]]+)\](?:\.(\w+))?)?$/);
	if (match) {
		tk.key = match[1];
		tk.index = match[2] != null ? parseInt(match[2]) : null;
		tk.propertyName = match[3] ?? match[6] ?? null;
		tk.indexPropertyName = match[4] ?? null;
		tk.indexPropertyValue = match[5] ?? null;
	}
	return tk;
}
function getTagsFromFormat(input) {
	return [...input.matchAll(/~([^~]+)~/g)].map((m) => m[1]);
}
const registry = /* @__PURE__ */ new Map();
function registerTag(name, resolver) {
	if (!registry.has(name)) registry.set(name, resolver);
}
function runTag(rawTag, task) {
	const tagKey = parseTagKey(rawTag);
	if (!tagKey) return null;
	const resolver = registry.get(tagKey.key);
	if (!resolver) return null;
	return resolver(tagKey, task);
}
function registerIpcHandlers() {
	ipcMain.handle('tasks:getAll', () => getAllTasks());
	ipcMain.handle('tasks:getRunningIds', () => getRunningIds());
	ipcMain.handle('tasks:start', (_e, args) =>
		startTask(
			args.entry,
			args.startNow,
			args.stopOtherTask,
			args.hideParent,
			args.bringNotes,
			args.bringTags,
			args.getTags,
			args.parentEntryId,
		),
	);
	ipcMain.handle('tasks:stop', (_e, taskId, endOn) => stopTask(taskId, endOn));
	ipcMain.handle('tasks:delete', (_e, taskId) => deleteTask(taskId));
	ipcMain.handle('tasks:update', (_e, taskId, args) => updateTask(taskId, args));
	ipcMain.handle('tasks:resume', (_e, taskId) => resumeTask(taskId));
	ipcMain.handle('tasks:reset', (_e, taskId) => resetTask(taskId));
	ipcMain.handle('tasks:trim', (_e, daysToKeep) => trimTasks(daysToKeep));
	ipcMain.handle('tasks:query', (_e, from, to, search) => queryTasks(from, to, search));
	ipcMain.handle('notes:getForTask', (_e, taskId) => getNotes(taskId));
	ipcMain.handle('notes:save', (_e, taskId, noteId, notes, rtf) => saveNote(taskId, noteId, notes, rtf));
	ipcMain.handle('notes:delete', (_e, noteId) => deleteNote(noteId));
	ipcMain.handle('tags:getAll', () => getAllTags());
	ipcMain.handle('tags:update', (_e, tagId, name, description) => updateTag(tagId, name, description));
	ipcMain.handle('tags:delete', (_e, tagId) => deleteTag(tagId));
	ipcMain.handle('settings:getAll', () => ({
		showQueuedTasks: settingHelper.ShowQueuedTasks,
		showRankedTasks: settingHelper.ShowRankedTasks,
		showClosedTasks: settingHelper.ShowClosedTasks,
		entryListOpacity: settingHelper.EntryListOpacity,
		numDaysOfDataToLoad: settingHelper.NumDaysOfDataToLoad,
		itemsToShow: settingHelper.ItemsToShow,
		autoOpenUpdateWindow: settingHelper.AutoOpenUpdateWindow,
		usePillTimer: settingHelper.UsePillTimer,
		dayStart: settingHelper.DayStart,
		dayEnd: settingHelper.DayEnd,
		dayEndNextDay: settingHelper.DayEndNextDay,
		timesheetsFolder: settingHelper.TimesheetsFolder,
		exportRowFormat: settingHelper.ExportRowFormat,
		exportFileExtension: settingHelper.ExportFileExtension,
		trimDaysToKeep: settingHelper.TrimDaysToKeep,
		formLocationX: settingHelper.FormLocationX,
		formLocationY: settingHelper.FormLocationY,
		dbPath: settingHelper.DbPath,
	}));
	ipcMain.handle('settings:set', (_e, key, value) => {
		const s = settingHelper;
		const map = {
			showQueuedTasks: 'ShowQueuedTasks',
			showRankedTasks: 'ShowRankedTasks',
			showClosedTasks: 'ShowClosedTasks',
			entryListOpacity: 'EntryListOpacity',
			numDaysOfDataToLoad: 'NumDaysOfDataToLoad',
			itemsToShow: 'ItemsToShow',
			autoOpenUpdateWindow: 'AutoOpenUpdateWindow',
			usePillTimer: 'UsePillTimer',
			dayStart: 'DayStart',
			dayEnd: 'DayEnd',
			dayEndNextDay: 'DayEndNextDay',
			timesheetsFolder: 'TimesheetsFolder',
			exportRowFormat: 'ExportRowFormat',
			exportFileExtension: 'ExportFileExtension',
			trimDaysToKeep: 'TrimDaysToKeep',
			formLocationX: 'FormLocationX',
			formLocationY: 'FormLocationY',
			dbPath: 'DbPath',
		};
		const prop = map[key];
		if (prop) s[prop] = value;
		return true;
	});
	ipcMain.handle('settings:changeDbPath', (_e, newPath) => {
		settingHelper.DbPath = newPath;
		reopenDb(newPath);
		loadTasks();
		return true;
	});
	ipcMain.handle('db:backup', () => backupDb());
	ipcMain.handle('db:getCurrentPath', () => getCurrentDbPath());
	ipcMain.handle('utility:getWeekData', (_e, year) => getWeekData(year));
	ipcMain.handle('export:generate', (_e, tasks2, format, folder, ext, dateStr) => {
		registerTag('StartedOnTicks', (_k, t) => (t.startedOn ? String(new Date(t.startedOn).getTime()) : ''));
		registerTag('Task', (_k, t) => String(t.task ?? '').slice(0, 200));
		registerTag('Duration', (_k, t) => String(t.durationString ?? ''));
		registerTag('TimesheetDate', (_k, _t) => dateStr);
		registerTag('Comments', (_k, t) => {
			const notes = getNotes(t.id);
			return notes.map((n) => n.notes).join('; ');
		});
		registerTag('Tags', (_k, t) => (t.tags ?? []).join(', '));
		registerTag('Tag', (tagKey, t) => {
			const tagData = t.tags ?? [];
			if (Array.isArray(tagData) && tagData.length > 0 && typeof tagData[0] === 'string') {
				return extractTagValue(tagKey, t.id, tagKey.indexPropertyValue);
			}
			return '';
		});
		const tagKeys = getTagsFromFormat(format);
		const lines = tasks2.map((task) => {
			const taskObj = task;
			return tagKeys.map((tk) => runTag(tk, taskObj) ?? '').join('^');
		});
		const content = lines.join('\n');
		const fileName = `Timesheet-${dateStr.replace(/\//g, '-')}.${ext}`;
		const filePath = path.join(folder, fileName);
		fs.writeFileSync(filePath, content, 'utf-8');
		return filePath;
	});
	ipcMain.handle('shell:openPath', (_e, p) => shell.openPath(p));
}
function extractTagValue(tagKey, taskId, filterValue) {
	if (!tagKey) return '';
	const db2 = getDb();
	const tags = db2
		.prepare(
			`
    SELECT t.Name, t.Description FROM tbl_Tag t
    JOIN tbl_TaskEntryTag tet ON tet.TagID = t.ID
    WHERE tet.TaskID = ?
  `,
		)
		.all(taskId);
	if (filterValue == null ? void 0 : filterValue.endsWith('%')) {
		const prefix = filterValue.slice(0, -1);
		const found = tags.find((t) => t.Name.startsWith(prefix));
		if (found) return tagKey.propertyName === 'Description' ? (found.Description ?? '') : found.Name;
	} else if (filterValue) {
		const found = tags.find((t) => t.Name === filterValue);
		if (found) return tagKey.propertyName === 'Description' ? (found.Description ?? '') : found.Name;
	}
	return '';
}
const __dirname$2 = path.dirname(fileURLToPath(import.meta.url));
const VITE_DEV_SERVER_URL = process.env['VITE_DEV_SERVER_URL'];
const RENDERER_DIST = path.join(process.env.APP_ROOT ?? '', 'dist');
function pageUrl(page) {
	if (VITE_DEV_SERVER_URL) return `${VITE_DEV_SERVER_URL}${page}.html`;
	return path.join(RENDERER_DIST, `${page}.html`);
}
function loadPage(win, page) {
	if (VITE_DEV_SERVER_URL) {
		win.loadURL(pageUrl(page));
	} else {
		win.loadFile(path.join(RENDERER_DIST, `${page}.html`));
	}
}
const preloadPath = path.join(__dirname$2, 'preload.mjs');
let entryListWin = null;
let newEntryWin = null;
let updateEntryWin = null;
let viewListWin = null;
let settingsWin = null;
let tagEditorWin = null;
const floatingButtonWins = [];
let tray = null;
function isAlive(w) {
	return w != null && !w.isDestroyed();
}
function getEntryListWindow() {
	if (!isAlive(entryListWin)) {
		entryListWin = new BrowserWindow({
			width: 320,
			height: 400,
			frame: false,
			resizable: false,
			alwaysOnTop: true,
			skipTaskbar: true,
			transparent: true,
			webPreferences: { preload: preloadPath, contextIsolation: true },
		});
		loadPage(entryListWin, 'entry-list');
		entryListWin.on('closed', () => {
			entryListWin = null;
		});
	}
	return entryListWin;
}
function showNewEntry(suggestions) {
	if (!isAlive(newEntryWin)) {
		newEntryWin = new BrowserWindow({
			width: 420,
			height: 300,
			frame: false,
			resizable: false,
			skipTaskbar: true,
			webPreferences: { preload: preloadPath, contextIsolation: true },
		});
		loadPage(newEntryWin, 'new-entry');
		newEntryWin.on('closed', () => {
			newEntryWin = null;
		});
	}
	newEntryWin.webContents.once('did-finish-load', () => {
		newEntryWin == null ? void 0 : newEntryWin.webContents.send('new-entry:suggestions', suggestions);
	});
	newEntryWin.show();
	newEntryWin.focus();
}
function showUpdateEntry(taskId) {
	if (!isAlive(updateEntryWin)) {
		updateEntryWin = new BrowserWindow({
			width: 640,
			height: 580,
			webPreferences: { preload: preloadPath, contextIsolation: true },
		});
		loadPage(updateEntryWin, 'update-entry');
		updateEntryWin.on('closed', () => {
			updateEntryWin = null;
		});
	}
	updateEntryWin.webContents.once('did-finish-load', () => {
		updateEntryWin == null ? void 0 : updateEntryWin.webContents.send('update-entry:load', taskId);
	});
	if (updateEntryWin.webContents.isLoading());
	else {
		updateEntryWin.webContents.send('update-entry:load', taskId);
	}
	updateEntryWin.show();
	updateEntryWin.focus();
}
function showViewList() {
	if (!isAlive(viewListWin)) {
		viewListWin = new BrowserWindow({
			width: 960,
			height: 680,
			webPreferences: { preload: preloadPath, contextIsolation: true },
		});
		loadPage(viewListWin, 'view-list');
		viewListWin.on('closed', () => {
			viewListWin = null;
		});
	}
	viewListWin.show();
	viewListWin.focus();
}
function showSettings() {
	if (!isAlive(settingsWin)) {
		settingsWin = new BrowserWindow({
			width: 520,
			height: 600,
			resizable: false,
			webPreferences: { preload: preloadPath, contextIsolation: true },
		});
		loadPage(settingsWin, 'settings');
		settingsWin.on('closed', () => {
			settingsWin = null;
		});
	}
	settingsWin.show();
	settingsWin.focus();
}
function showTagEditor(selectTag) {
	if (!isAlive(tagEditorWin)) {
		tagEditorWin = new BrowserWindow({
			width: 480,
			height: 440,
			webPreferences: { preload: preloadPath, contextIsolation: true },
		});
		loadPage(tagEditorWin, 'tag-editor');
		tagEditorWin.on('closed', () => {
			tagEditorWin = null;
		});
	}
	tagEditorWin.show();
	tagEditorWin.focus();
	if (selectTag) {
		tagEditorWin.webContents.once('did-finish-load', () => {
			tagEditorWin == null ? void 0 : tagEditorWin.webContents.send('tag-editor:select', selectTag);
		});
		if (!tagEditorWin.webContents.isLoading()) {
			tagEditorWin.webContents.send('tag-editor:select', selectTag);
		}
	}
}
function createFloatingButton(taskId, taskName, startedOn, x, y) {
	const win = new BrowserWindow({
		width: 228,
		height: 52,
		x,
		y,
		frame: false,
		transparent: true,
		alwaysOnTop: true,
		skipTaskbar: true,
		resizable: false,
		webPreferences: { preload: preloadPath, contextIsolation: true },
	});
	loadPage(win, 'floating-button');
	win.webContents.once('did-finish-load', () => {
		win.webContents.send('floating-button:init', { taskId, taskName, startedOn });
	});
	win.on('closed', () => {
		const idx = floatingButtonWins.indexOf(win);
		if (idx !== -1) floatingButtonWins.splice(idx, 1);
	});
	floatingButtonWins.push(win);
	win.show();
	return win;
}
function getFloatingButtons() {
	return floatingButtonWins.filter((w) => !w.isDestroyed());
}
function getTrayIcon() {
	const pub = process.env.VITE_PUBLIC ?? '';
	const iconFile = nativeTheme.shouldUseDarkColors ? 'tray-icon-white.png' : 'tray-icon.png';
	const iconPath = path.join(pub, iconFile);
	const img = nativeImage.createFromPath(iconPath);
	return img.isEmpty() ? nativeImage.createFromPath(path.join(pub, 'tray-icon.png')) : img;
}
function setupTray(onNewEntry, onViewAll, onSettings, onTagEditor) {
	tray = new Tray(getTrayIcon());
	tray.setToolTip('Zup');
	const menu = Menu.buildFromTemplate([
		{ label: 'New Entry (Shift+Alt+J)', click: onNewEntry },
		{ type: 'separator' },
		{ label: 'View All (Shift+Alt+P)', click: onViewAll },
		{ label: 'Settings', click: onSettings },
		{ label: 'Tag Editor', click: onTagEditor },
		{ type: 'separator' },
		{ label: 'Exit', click: () => app.quit() },
	]);
	tray.setContextMenu(menu);
	tray.on('double-click', onViewAll);
}
function updateTrayIcon(queueCount) {
	if (!tray) return;
	tray.setImage(getTrayIcon());
	const tip = queueCount > 0 ? `Zup (${queueCount} queued)` : 'Zup';
	tray.setToolTip(tip);
}
function moveEntryListToCenter() {
	if (!isAlive(entryListWin)) return;
	const { width, height } = screen.getPrimaryDisplay().workAreaSize;
	entryListWin.setPosition(
		Math.floor(width / 2 - entryListWin.getBounds().width / 2),
		Math.floor(height / 2 - entryListWin.getBounds().height / 2),
	);
	entryListWin.show();
	entryListWin.focus();
}
const __dirname$1 = path.dirname(fileURLToPath(import.meta.url));
process.env.APP_ROOT = path.join(__dirname$1, '..');
process.env.VITE_PUBLIC = VITE_DEV_SERVER_URL ? path.join(process.env.APP_ROOT, 'public') : RENDERER_DIST;
app.on('window-all-closed', (e) => e.preventDefault());
app.whenReady().then(() => {
	initDb(settingHelper.DbPath);
	loadTasks();
	registerIpcHandlers();
	ipcMain.handle('window:showNewEntry', () => {
		const suggestions = getAllTasks()
			.filter((t) => t.endedOn != null)
			.map((t) => t.task)
			.filter((v, i, a) => a.indexOf(v) === i)
			.slice(0, 20);
		showNewEntry(suggestions);
	});
	ipcMain.handle('window:showUpdateEntry', (_e, taskId) => showUpdateEntry(taskId));
	ipcMain.handle('window:showViewList', () => showViewList());
	ipcMain.handle('window:showSettings', () => showSettings());
	ipcMain.handle('window:showTagEditor', (_e, tag) => showTagEditor(tag));
	ipcMain.handle('window:moveEntryListToCenter', () => moveEntryListToCenter());
	ipcMain.handle('window:savePosition', (_e, x2, y2) => {
		settingHelper.FormLocationX = x2;
		settingHelper.FormLocationY = y2;
	});
	ipcMain.handle('window:move', (event, x2, y2) => {
		const win = BrowserWindow.fromWebContents(event.sender);
		if (win && !win.isDestroyed()) win.setPosition(Math.round(x2), Math.round(y2));
	});
	ipcMain.handle('window:createFloatingButton', (_e, taskId, taskName, startedOn) => {
		const buttons = getFloatingButtons();
		const { width: width2, height: height2 } = screen.getPrimaryDisplay().workAreaSize;
		const x2 = settingHelper.FormLocationX || width2 - 240;
		const y2 = (settingHelper.FormLocationY || height2 - 60) + buttons.length * 50;
		createFloatingButton(taskId, taskName, startedOn, x2, y2);
	});
	const entryList = getEntryListWindow();
	const { width, height } = screen.getPrimaryDisplay().workAreaSize;
	const x = settingHelper.FormLocationX || width - 325;
	const y = settingHelper.FormLocationY || height - 420;
	entryList.setPosition(x, y);
	entryList.show();
	entryList.on('moved', () => {
		const [wx, wy] = entryList.getPosition();
		settingHelper.FormLocationX = wx;
		settingHelper.FormLocationY = wy;
	});
	setupTray(
		() => triggerNewEntry(),
		() => showViewList(),
		() => showSettings(),
		() => showTagEditor(),
	);
	updateTrayIcon(getAllTasks().filter((t) => t.startedOn == null).length);
	taskEvents.on('task:started', (task) => {
		if (!settingHelper.UsePillTimer) return;
		const btns = getFloatingButtons();
		const { width: w, height: h } = screen.getPrimaryDisplay().workAreaSize;
		const bx = settingHelper.FormLocationX || w - 240;
		const by = (settingHelper.FormLocationY || h - 60) + btns.length * 50;
		createFloatingButton(task.id, task.task, task.startedOn, bx, by);
		updateTrayIcon(getAllTasks().filter((t) => t.startedOn == null).length);
	});
	taskEvents.on('task:stopped', () => {
		updateTrayIcon(getAllTasks().filter((t) => t.startedOn == null).length);
	});
	globalShortcut.register('Shift+Alt+J', () => triggerNewEntry());
	globalShortcut.register('Shift+Alt+K', () => triggerUpdateCurrent());
	globalShortcut.register('Shift+Alt+L', () => triggerToggleLast());
	globalShortcut.register('Shift+Alt+P', () => showViewList());
	app.on('will-quit', () => globalShortcut.unregisterAll());
});
function triggerNewEntry() {
	checkNewWeekBackup();
	const suggestions = getAllTasks()
		.filter((t) => t.endedOn != null)
		.map((t) => t.task)
		.filter((v, i, a) => a.indexOf(v) === i)
		.slice(0, 20);
	showNewEntry(suggestions);
}
function triggerUpdateCurrent() {
	checkNewWeekBackup();
	const running = getAllTasks().find((t) => t.isRunning);
	if (running) showUpdateEntry(running.id);
}
function triggerToggleLast() {
	checkNewWeekBackup();
	const tasks2 = getAllTasks();
	const running = tasks2.find((t) => t.isRunning);
	if (running) {
		stopTask(running.id);
	} else {
		const last = tasks2
			.filter((t) => t.endedOn != null)
			.sort((a, b) => new Date(b.endedOn).getTime() - new Date(a.endedOn).getTime())[0];
		if (last) startTask(last.task, true, true, false, false, false, false);
	}
}
function checkNewWeekBackup() {
	const tasks2 = getAllTasks();
	if (!tasks2.length) return;
	const dates = tasks2.flatMap((t) => [t.createdOn, t.startedOn, t.endedOn].filter(Boolean));
	const lastDate = new Date(Math.max(...dates.map((d) => new Date(d).getTime())));
	if (getWeekNumber(lastDate) < getWeekNumber(/* @__PURE__ */ new Date())) backupDb();
}
