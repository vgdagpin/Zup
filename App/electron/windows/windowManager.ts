import { BrowserWindow, nativeImage, Tray, Menu, app, nativeTheme } from 'electron';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const __dirname = path.dirname(fileURLToPath(import.meta.url));

export const VITE_DEV_SERVER_URL = process.env['VITE_DEV_SERVER_URL'];
export const RENDERER_DIST = path.join(process.env.APP_ROOT ?? '', 'dist');

function pageUrl(page: string): string {
	if (VITE_DEV_SERVER_URL) return `${VITE_DEV_SERVER_URL}${page}.html`;
	return path.join(RENDERER_DIST, `${page}.html`);
}

function loadPage(win: BrowserWindow, page: string): void {
	if (VITE_DEV_SERVER_URL) {
		win.loadURL(pageUrl(page));
	} else {
		win.loadFile(path.join(RENDERER_DIST, `${page}.html`));
	}
}

const preloadPath = path.join(__dirname, 'preload.mjs');

// ---- Window registry ----
let newEntryWin: BrowserWindow | null = null;
let updateEntryWin: BrowserWindow | null = null;
let viewListWin: BrowserWindow | null = null;
let settingsWin: BrowserWindow | null = null;
let tagEditorWin: BrowserWindow | null = null;
const floatingButtonWins: BrowserWindow[] = [];

export let tray: Tray | null = null;

function isAlive(w: BrowserWindow | null): w is BrowserWindow {
	return w != null && !w.isDestroyed();
}

// ---- New Entry ----
export function showNewEntry(suggestions: string[]): void {
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
	// Send suggestions once page is ready
	newEntryWin.webContents.once('did-finish-load', () => {
		newEntryWin?.webContents.send('new-entry:suggestions', suggestions);
	});
	newEntryWin.show();
	newEntryWin.focus();
}

export function closeNewEntry(): void {
	if (isAlive(newEntryWin)) newEntryWin.hide();
}

// ---- Update Entry ----
export function showUpdateEntry(taskId: string): void {
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
		updateEntryWin?.webContents.send('update-entry:load', taskId);
	});
	if (updateEntryWin.webContents.isLoading()) {
		// will fire did-finish-load
	} else {
		updateEntryWin.webContents.send('update-entry:load', taskId);
	}
	updateEntryWin.show();
	updateEntryWin.focus();
}

// ---- View List ----
export function showViewList(): void {
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

// ---- Settings ----
export function showSettings(): void {
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

// ---- Tag Editor ----
export function showTagEditor(selectTag?: string): void {
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
			tagEditorWin?.webContents.send('tag-editor:select', selectTag);
		});
		if (!tagEditorWin.webContents.isLoading()) {
			tagEditorWin.webContents.send('tag-editor:select', selectTag);
		}
	}
}

// ---- Floating Button ----
export function createFloatingButton(
	taskId: string,
	taskName: string,
	startedOn: string | null,
	x: number,
	y: number,
): BrowserWindow {
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

export function getFloatingButtons(): BrowserWindow[] {
	return floatingButtonWins.filter((w) => !w.isDestroyed());
}

// ---- System Tray ----
function getTrayIcon(): Electron.NativeImage {
	const pub = process.env.VITE_PUBLIC ?? '';
	// Use white icon on dark backgrounds, black on light backgrounds
	const iconFile = nativeTheme.shouldUseDarkColors ? 'tray-icon-white.png' : 'tray-icon.png';
	const iconPath = path.join(pub, iconFile);
	const img = nativeImage.createFromPath(iconPath);
	return img.isEmpty() ? nativeImage.createFromPath(path.join(pub, 'tray-icon.png')) : img;
}

export function setupTray(onNewEntry: () => void, onViewAll: () => void, onSettings: () => void, onTagEditor: () => void): void {
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

export function updateTrayIcon(queueCount: number): void {
	if (!tray) return;
	tray.setImage(getTrayIcon());
	const tip = queueCount > 0 ? `Zup (${queueCount} queued)` : 'Zup';
	tray.setToolTip(tip);
}
