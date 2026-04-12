import { app } from 'electron';
import path from 'node:path';
import fs from 'node:fs';
import { getDb, getDefaultDbPath } from './database.js';

// All defaults mirror Properties.Settings.Default in the WinForms app
const DEFAULTS: Record<string, string> = {
	NumDaysOfDataToLoad: '30',
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

// DbPath is stored in electron userData (app-level), not in tbl_Setting
const userDataFile = () => path.join(app.getPath('userData'), 'zup-config.json');

function readUserData(): Record<string, string> {
	try {
		const f = userDataFile();
		if (fs.existsSync(f)) return JSON.parse(fs.readFileSync(f, 'utf-8'));
	} catch {
		/* ignore */
	}
	return {};
}

function writeUserData(data: Record<string, string>): void {
	fs.writeFileSync(userDataFile(), JSON.stringify(data, null, 2));
}

function getSetting(name: string): string | null {
	const row = getDb().prepare('SELECT Value FROM tbl_Setting WHERE Name = ?').get(name) as { Value: string } | undefined;
	return row?.Value ?? null;
}

function setSetting(name: string, value: string): void {
	const existing = getDb().prepare('SELECT Name FROM tbl_Setting WHERE Name = ?').get(name);
	if (existing) {
		getDb().prepare('UPDATE tbl_Setting SET Value = ? WHERE Name = ?').run(value, name);
	} else {
		getDb().prepare('INSERT INTO tbl_Setting (Name, Value, DataType) VALUES (?, ?, ?)').run(name, value, 'string');
	}
}

function get(name: string): string {
	return getSetting(name) ?? DEFAULTS[name] ?? '';
}

function set(name: string, value: string): void {
	setSetting(name, value);
}

export const settingHelper = {
	get NumDaysOfDataToLoad() {
		return parseInt(get('NumDaysOfDataToLoad'));
	},
	set NumDaysOfDataToLoad(v) {
		set('NumDaysOfDataToLoad', String(v));
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

	get DbPath(): string {
		const ud = readUserData();
		if (ud.DbPath) return ud.DbPath;
		return getDefaultDbPath();
	},
	set DbPath(v: string) {
		const ud = readUserData();
		ud.DbPath = v;
		writeUserData(ud);
	},
};
