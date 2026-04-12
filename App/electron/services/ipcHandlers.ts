import { ipcMain, shell } from 'electron';
import fs from 'node:fs';
import path from 'node:path';
import * as tc from './taskCollection.js';
import { settingHelper } from './settingHelper.js';
import { backupDb, reopenDb, getCurrentDbPath, getDb } from './database.js';
import { getAllTags, updateTag, deleteTag, getNotes, saveNote, deleteNote } from './taskCollection.js';
import { getWeekData } from './utility.js';
import { getTagsFromFormat, registerTag, runTag, parseTagKey } from './tagHelper.js';
import type { NewEntryArgs, UpdateEntryArgs, ZupTask } from '../../src/types/index.js';

export function registerIpcHandlers(): void {
	// ---- Tasks ----
	ipcMain.handle('tasks:getAll', () => tc.getAllTasks());
	ipcMain.handle('tasks:getRunningIds', () => tc.getRunningIds());

	ipcMain.handle('tasks:start', (_e, args: NewEntryArgs) =>
		tc.startTask(
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

	ipcMain.handle('tasks:stop', (_e, taskId: string, endOn?: string) => tc.stopTask(taskId, endOn));
	ipcMain.handle('tasks:delete', (_e, taskId: string) => tc.deleteTask(taskId));
	ipcMain.handle('tasks:update', (_e, taskId: string, args: UpdateEntryArgs) => tc.updateTask(taskId, args));
	ipcMain.handle('tasks:resume', (_e, taskId: string) => tc.resumeTask(taskId));
	ipcMain.handle('tasks:reset', (_e, taskId: string) => tc.resetTask(taskId));
	ipcMain.handle('tasks:trim', (_e, daysToKeep: number) => tc.trimTasks(daysToKeep));

	ipcMain.handle('tasks:query', (_e, from: string, to: string, search?: string) => tc.queryTasks(from, to, search));

	// ---- Notes ----
	ipcMain.handle('notes:getForTask', (_e, taskId: string) => getNotes(taskId));
	ipcMain.handle('notes:save', (_e, taskId: string, noteId: string | null, notes: string, rtf: string) =>
		saveNote(taskId, noteId, notes, rtf),
	);
	ipcMain.handle('notes:delete', (_e, noteId: string) => deleteNote(noteId));

	// ---- Tags ----
	ipcMain.handle('tags:getAll', () => getAllTags());
	ipcMain.handle('tags:update', (_e, tagId: string, name: string, description: string) => updateTag(tagId, name, description));
	ipcMain.handle('tags:delete', (_e, tagId: string) => deleteTag(tagId));

	// ---- Settings ----
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

	ipcMain.handle('settings:set', (_e, key: string, value: unknown) => {
		const s = settingHelper as Record<string, unknown>;
		// Map camelCase key to SettingHelper property
		const map: Record<string, string> = {
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
		if (prop) (s as Record<string, unknown>)[prop] = value;
		return true;
	});

	ipcMain.handle('settings:changeDbPath', (_e, newPath: string) => {
		settingHelper.DbPath = newPath;
		reopenDb(newPath);
		tc.loadTasks();
		return true;
	});

	// ---- DB backup ----
	ipcMain.handle('db:backup', () => backupDb());
	ipcMain.handle('db:getCurrentPath', () => getCurrentDbPath());

	// ---- Week data (for ViewList) ----
	ipcMain.handle('utility:getWeekData', (_e, year: number) => getWeekData(year));

	// ---- Export ----
	ipcMain.handle('export:generate', (_e, tasks: ZupTask[], format: string, folder: string, ext: string, dateStr: string) => {
		// Register tag resolvers (idempotent)
		registerTag('StartedOnTicks', (_k, t) => (t.startedOn ? String(new Date(t.startedOn as string).getTime()) : ''));
		registerTag('Task', (_k, t) => String(t.task ?? '').slice(0, 200));
		registerTag('Duration', (_k, t) => String(t.durationString ?? ''));
		registerTag('TimesheetDate', (_k, _t) => dateStr);
		registerTag('Comments', (_k, t) => {
			const notes = getNotes(t.id as string);
			return notes.map((n) => n.notes).join('; ');
		});
		registerTag('Tags', (_k, t) => ((t.tags as string[]) ?? []).join(', '));
		registerTag('Tag', (tagKey, t) => {
			const tagData = (t.tags as { name: string; description: string | null }[] | string[]) ?? [];
			if (Array.isArray(tagData) && tagData.length > 0 && typeof tagData[0] === 'string') {
				return extractTagValue(tagKey, t.id as string, tagKey.indexPropertyValue);
			}
			return '';
		});

		const tagKeys = getTagsFromFormat(format);
		const lines = tasks.map((task) => {
			const taskObj = task as unknown as Record<string, unknown>;
			return tagKeys.map((tk) => runTag(tk, taskObj) ?? '').join('^');
		});

		const content = lines.join('\n');
		const fileName = `Timesheet-${dateStr.replace(/\//g, '-')}.${ext}`;
		const filePath = path.join(folder, fileName);
		fs.writeFileSync(filePath, content, 'utf-8');
		return filePath;
	});

	ipcMain.handle('shell:openPath', (_e, p: string) => shell.openPath(p));
}

function extractTagValue(tagKey: ReturnType<typeof parseTagKey>, taskId: string, filterValue?: string | null): string {
	if (!tagKey) return '';
	const db = getDb();
	const tags = db
		.prepare(
			`
    SELECT t.Name, t.Description FROM tbl_Tag t
    JOIN tbl_TaskEntryTag tet ON tet.TagID = t.ID
    WHERE tet.TaskID = ?
  `,
		)
		.all(taskId) as { Name: string; Description: string | null }[];

	if (filterValue?.endsWith('%')) {
		const prefix = filterValue.slice(0, -1);
		const found = tags.find((t) => t.Name.startsWith(prefix));
		if (found) return tagKey.propertyName === 'Description' ? (found.Description ?? '') : found.Name;
	} else if (filterValue) {
		const found = tags.find((t) => t.Name === filterValue);
		if (found) return tagKey.propertyName === 'Description' ? (found.Description ?? '') : found.Name;
	}

	return '';
}
