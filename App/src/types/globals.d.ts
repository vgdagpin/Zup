import type { ZupTask, ZupTag, ZupNote, ZupSettings, NewEntryArgs, UpdateEntryArgs, WeekData } from '../types/index.js';

declare global {
	interface Window {
		zupAPI: {
			// Tasks
			getTasks: () => Promise<ZupTask[]>;
			getRunningIds: () => Promise<string[]>;
			startTask: (args: NewEntryArgs) => Promise<ZupTask>;
			stopTask: (taskId: string, endOn?: string) => Promise<ZupTask | null>;
			deleteTask: (taskId: string) => Promise<void>;
			updateTask: (taskId: string, args: UpdateEntryArgs) => Promise<ZupTask | null>;
			resumeTask: (taskId: string) => Promise<ZupTask | null>;
			resetTask: (taskId: string) => Promise<ZupTask | null>;
			trimTasks: (days: number) => Promise<number>;
			queryTasks: (from: string, to: string, search?: string) => Promise<ZupTask[]>;

			// Notes
			getNotes: (taskId: string) => Promise<ZupNote[]>;
			saveNote: (taskId: string, noteId: string | null, notes: string, rtf: string) => Promise<{ id: string } | null>;
			deleteNote: (noteId: string) => Promise<void>;

			// Tags
			getTags: () => Promise<ZupTag[]>;
			updateTag: (tagId: string, name: string, description: string) => Promise<void>;
			deleteTag: (tagId: string) => Promise<boolean>;

			// Settings
			getSettings: () => Promise<ZupSettings>;
			setSetting: (key: string, value: unknown) => Promise<boolean>;
			changeDbPath: (newPath: string) => Promise<boolean>;

			// DB
			backupDb: () => Promise<string | null>;
			getDbPath: () => Promise<string>;

			// Utility
			getWeekData: (year: number) => Promise<WeekData[]>;

			// Export
			exportTimesheets: (tasks: ZupTask[], format: string, folder: string, ext: string, dateStr: string) => Promise<string>;

			// Window management
			showNewEntry: () => Promise<void>;
			showUpdateEntry: (taskId: string) => Promise<void>;
			showViewList: () => Promise<void>;
			showSettings: () => Promise<void>;
			showTagEditor: (tag?: string) => Promise<void>;
			moveToCenter: () => Promise<void>;
			savePosition: (x: number, y: number) => Promise<void>;
moveWindow: (x: number, y: number) => Promise<void>;
			createFloatingButton: (taskId: string, taskName: string, startedOn: string | null) => Promise<void>;

			// Shell
			openPath: (p: string) => Promise<void>;

			// Events
			on: (channel: string, fn: (...args: unknown[]) => void) => void;
			off: (channel: string, fn: (...args: unknown[]) => void) => void;
		};
	}
}

export {};

