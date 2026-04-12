import Database from 'better-sqlite3';
import path from 'node:path';
import fs from 'node:fs';
import { app } from 'electron';

let db: Database.Database | null = null;
let dbFilePath: string = '';

export function getDefaultDbPath(): string {
	// Match WinForms: %USERPROFILE%\Documents\Zup\Zup.db
	const docs = app.getPath('documents');
	const dir = path.join(docs, 'Zup');
	if (!fs.existsSync(dir)) fs.mkdirSync(dir, { recursive: true });
	return path.join(dir, 'Zup.db');
}

export function getDb(): Database.Database {
	if (!db) throw new Error('Database not initialized. Call initDb() first.');
	return db;
}

export function getCurrentDbPath(): string {
	return dbFilePath;
}

export function initDb(filePath?: string): Database.Database {
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

export function reopenDb(filePath: string): void {
	if (db) {
		db.close();
		db = null;
	}
	initDb(filePath);
}

export function backupDb(): string | null {
	if (!dbFilePath || !fs.existsSync(dbFilePath)) return null;

	const dir = path.join(path.dirname(dbFilePath), 'Backup');
	if (!fs.existsSync(dir)) fs.mkdirSync(dir, { recursive: true });

	const stamp = new Date().toISOString().replace(/[-:T]/g, '').slice(0, 14);
	const newName = `${path.basename(dbFilePath, '.db')}-${stamp}-bak.db`;
	const backupPath = path.join(dir, newName);

	if (!fs.existsSync(backupPath)) {
		fs.copyFileSync(dbFilePath, backupPath);
	}

	return dir;
}

function bootstrapSchema(d: Database.Database): void {
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

interface MigrationRow {
	MigrationId: string;
}

function runMigrations(d: Database.Database): void {
	const applied = new Set(
		(d.prepare('SELECT MigrationId FROM tbl_MigrationHistory').all() as MigrationRow[]).map((r) => r.MigrationId),
	);

	const migrations: Array<{ id: string; sql: string }> = [
		// Add future schema changes here
		// { id: '20250001_example', sql: 'ALTER TABLE ...' }
	];

	for (const m of migrations) {
		if (!applied.has(m.id)) {
			d.exec(m.sql);
			d.prepare('INSERT INTO tbl_MigrationHistory (MigrationId, AppliedAt) VALUES (?, ?)').run(
				m.id,
				new Date().toISOString(),
			);
		}
	}
}
