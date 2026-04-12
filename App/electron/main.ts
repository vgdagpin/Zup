import { app, BrowserWindow, globalShortcut, ipcMain, screen } from 'electron';
import { fileURLToPath } from 'node:url';
import path from 'node:path';
import { initDb, backupDb } from './services/database.js';
import { settingHelper } from './services/settingHelper.js';
import { loadTasks, getAllTasks, startTask, stopTask, taskEvents } from './services/taskCollection.js';
import { registerIpcHandlers } from './services/ipcHandlers.js';
import { getWeekNumber } from './services/utility.js';
import {
	getEntryListWindow,
	showNewEntry,
	showUpdateEntry,
	showViewList,
	showSettings,
	showTagEditor,
	createFloatingButton,
	getFloatingButtons,
	setupTray,
	updateTrayIcon,
	moveEntryListToCenter,
	RENDERER_DIST,
	VITE_DEV_SERVER_URL,
} from './windows/windowManager.js';

const __dirname = path.dirname(fileURLToPath(import.meta.url));

process.env.APP_ROOT = path.join(__dirname, '..');
process.env.VITE_PUBLIC = VITE_DEV_SERVER_URL ? path.join(process.env.APP_ROOT, 'public') : RENDERER_DIST;

// Stay alive in system tray when all windows close
app.on('window-all-closed', (e: Event) => e.preventDefault());

app.whenReady().then(() => {
	initDb(settingHelper.DbPath);
	loadTasks();
	registerIpcHandlers();

	// Window management IPC
	ipcMain.handle('window:showNewEntry', () => {
		const suggestions = getAllTasks()
			.filter((t) => t.endedOn != null)
			.map((t) => t.task)
			.filter((v, i, a) => a.indexOf(v) === i)
			.slice(0, 20);
		showNewEntry(suggestions);
	});

	ipcMain.handle('window:showUpdateEntry', (_e, taskId: string) => showUpdateEntry(taskId));
	ipcMain.handle('window:showViewList', () => showViewList());
	ipcMain.handle('window:showSettings', () => showSettings());
	ipcMain.handle('window:showTagEditor', (_e, tag?: string) => showTagEditor(tag));
	ipcMain.handle('window:moveEntryListToCenter', () => moveEntryListToCenter());

	ipcMain.handle('window:savePosition', (_e, x: number, y: number) => {
		settingHelper.FormLocationX = x;
		settingHelper.FormLocationY = y;
	});

	// Move the calling window to a new position (used by JS drag in floating windows)
	ipcMain.handle('window:move', (event, x: number, y: number) => {
		const win = BrowserWindow.fromWebContents(event.sender);
		if (win && !win.isDestroyed()) win.setPosition(Math.round(x), Math.round(y));
	});

	ipcMain.handle('window:createFloatingButton', (_e, taskId: string, taskName: string, startedOn: string | null) => {
		const buttons = getFloatingButtons();
		const { width, height } = screen.getPrimaryDisplay().workAreaSize;
		const x = settingHelper.FormLocationX || width - 240;
		const y = (settingHelper.FormLocationY || height - 60) + buttons.length * 50;
		createFloatingButton(taskId, taskName, startedOn, x, y);
	});

	// Show entry list on startup
	const entryList = getEntryListWindow();
	const { width, height } = screen.getPrimaryDisplay().workAreaSize;
	const x = settingHelper.FormLocationX || width - 325;
	const y = settingHelper.FormLocationY || height - 420;
	entryList.setPosition(x, y);
	entryList.show();

	// Save position when dragged via native -webkit-app-region drag
	entryList.on('moved', () => {
		const [wx, wy] = entryList.getPosition();
		settingHelper.FormLocationX = wx;
		settingHelper.FormLocationY = wy;
	});

	// System tray
	setupTray(
		() => triggerNewEntry(),
		() => showViewList(),
		() => showSettings(),
		() => showTagEditor(),
	);
	updateTrayIcon(getAllTasks().filter((t) => t.startedOn == null).length);

	// Create/destroy floating buttons as tasks start/stop
	taskEvents.on('task:started', (task: { id: string; task: string; startedOn: string | null; isRunning: boolean }) => {
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

	// Global shortcuts
	globalShortcut.register('Shift+Alt+J', () => triggerNewEntry());
	globalShortcut.register('Shift+Alt+K', () => triggerUpdateCurrent());
	globalShortcut.register('Shift+Alt+L', () => triggerToggleLast());
	globalShortcut.register('Shift+Alt+P', () => showViewList());

	app.on('will-quit', () => globalShortcut.unregisterAll());
});

function triggerNewEntry(): void {
	checkNewWeekBackup();
	const suggestions = getAllTasks()
		.filter((t) => t.endedOn != null)
		.map((t) => t.task)
		.filter((v, i, a) => a.indexOf(v) === i)
		.slice(0, 20);
	showNewEntry(suggestions);
}

function triggerUpdateCurrent(): void {
	checkNewWeekBackup();
	const running = getAllTasks().find((t) => t.isRunning);
	if (running) showUpdateEntry(running.id);
}

function triggerToggleLast(): void {
	checkNewWeekBackup();
	const tasks = getAllTasks();
	const running = tasks.find((t) => t.isRunning);
	if (running) {
		stopTask(running.id);
	} else {
		const last = tasks
			.filter((t) => t.endedOn != null)
			.sort((a, b) => new Date(b.endedOn!).getTime() - new Date(a.endedOn!).getTime())[0];
		if (last) startTask(last.task, true, true, false, false, false, false);
	}
}

function checkNewWeekBackup(): void {
	const tasks = getAllTasks();
	if (!tasks.length) return;
	const dates = tasks.flatMap((t) => [t.createdOn, t.startedOn, t.endedOn].filter(Boolean) as string[]);
	const lastDate = new Date(Math.max(...dates.map((d) => new Date(d).getTime())));
	if (getWeekNumber(lastDate) < getWeekNumber(new Date())) backupDb();
}
