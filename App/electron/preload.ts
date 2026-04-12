import { ipcRenderer, contextBridge } from 'electron';
import type { NewEntryArgs, UpdateEntryArgs, ZupTask } from '../src/types/index.js';

contextBridge.exposeInMainWorld('zupAPI', {
	// Tasks
	getTasks: () => ipcRenderer.invoke('tasks:getAll'),
	getRunningIds: () => ipcRenderer.invoke('tasks:getRunningIds'),
	startTask: (args: NewEntryArgs) => ipcRenderer.invoke('tasks:start', args),
	stopTask: (taskId: string, endOn?: string) => ipcRenderer.invoke('tasks:stop', taskId, endOn),
	deleteTask: (taskId: string) => ipcRenderer.invoke('tasks:delete', taskId),
	updateTask: (taskId: string, args: UpdateEntryArgs) => ipcRenderer.invoke('tasks:update', taskId, args),
	resumeTask: (taskId: string) => ipcRenderer.invoke('tasks:resume', taskId),
	resetTask: (taskId: string) => ipcRenderer.invoke('tasks:reset', taskId),
	trimTasks: (days: number) => ipcRenderer.invoke('tasks:trim', days),
	queryTasks: (from: string, to: string, search?: string) => ipcRenderer.invoke('tasks:query', from, to, search),

	// Notes
	getNotes: (taskId: string) => ipcRenderer.invoke('notes:getForTask', taskId),
	saveNote: (taskId: string, noteId: string | null, notes: string, rtf: string) =>
		ipcRenderer.invoke('notes:save', taskId, noteId, notes, rtf),
	deleteNote: (noteId: string) => ipcRenderer.invoke('notes:delete', noteId),

	// Tags
	getTags: () => ipcRenderer.invoke('tags:getAll'),
	updateTag: (tagId: string, name: string, description: string) => ipcRenderer.invoke('tags:update', tagId, name, description),
	deleteTag: (tagId: string) => ipcRenderer.invoke('tags:delete', tagId),

	// Settings
	getSettings: () => ipcRenderer.invoke('settings:getAll'),
	setSetting: (key: string, value: unknown) => ipcRenderer.invoke('settings:set', key, value),
	changeDbPath: (newPath: string) => ipcRenderer.invoke('settings:changeDbPath', newPath),

	// DB
	backupDb: () => ipcRenderer.invoke('db:backup'),
	getDbPath: () => ipcRenderer.invoke('db:getCurrentPath'),

	// Utility
	getWeekData: (year: number) => ipcRenderer.invoke('utility:getWeekData', year),

	// Export
	exportTimesheets: (tasks: ZupTask[], format: string, folder: string, ext: string, dateStr: string) =>
		ipcRenderer.invoke('export:generate', tasks, format, folder, ext, dateStr),

	// Window management
	showNewEntry: () => ipcRenderer.invoke('window:showNewEntry'),
	showUpdateEntry: (taskId: string) => ipcRenderer.invoke('window:showUpdateEntry', taskId),
	showViewList: () => ipcRenderer.invoke('window:showViewList'),
	showSettings: () => ipcRenderer.invoke('window:showSettings'),
	showTagEditor: (tag?: string) => ipcRenderer.invoke('window:showTagEditor', tag),
	savePosition: (x: number, y: number) => ipcRenderer.invoke('window:savePosition', x, y),
	moveWindow: (x: number, y: number) => ipcRenderer.invoke('window:move', x, y),
	createFloatingButton: (taskId: string, taskName: string, startedOn: string | null) =>
		ipcRenderer.invoke('window:createFloatingButton', taskId, taskName, startedOn),

	// Shell
	openPath: (p: string) => ipcRenderer.invoke('shell:openPath', p),

	// Events (renderer subscribes to main-process push events)
	on: (channel: string, fn: (...args: unknown[]) => void) => {
		ipcRenderer.on(channel, (_e, ...args) => fn(...args));
	},
	off: (channel: string, fn: (...args: unknown[]) => void) => {
		ipcRenderer.removeListener(channel, fn as Parameters<typeof ipcRenderer.removeListener>[1]);
	},
});
