# Zup – Copilot Instructions

Zup is a Windows system-tray task/time-tracker built with **Electron + React + TypeScript + Vite** and a local **SQLite** database (via `better-sqlite3`). All code lives under `App/`.

## Commands (run from `App/`)

```bash
npm run dev          # Start Vite dev server + Electron in watch mode
npm run build        # tsc + vite build (output: dist/ and dist-electron/)
npm run dist         # build + package portable Windows exe via electron-builder
npm run lint         # ESLint (zero warnings allowed)
npm run typecheck    # tsc type-check only (no emit)
npm run format       # Prettier write
npm run rebuild      # Rebuild native better-sqlite3 against current Electron ABI
```

There is no test suite.

## Architecture

### Multi-page Electron app
Each Electron window is a **separate Vite entry point** (`*.html` at root + `src/pages/<name>/index.tsx`). Pages are standalone React apps (each calls `createRoot`); there is no shared router or global store.

| Window | Entry | Purpose |
|---|---|---|
| `entry-list` | always-on-top, frameless, transparent | Floating task list widget |
| `floating-button` | always-on-top, transparent, 220×44 | Per-running-task pill timer |
| `new-entry` | frameless popup | Create a task |
| `update-entry` | normal window | Edit task + notes + tags |
| `view-list` | normal window | Query/export historical tasks |
| `settings` | normal window | App settings |
| `tag-editor` | normal window | Manage tags |

### IPC communication
- **Renderer → Main**: `window.zupAPI.*` — all calls go through `electron/preload.ts` via `contextBridge`. Never call `ipcRenderer` directly.
- **Main → Renderer**: `BrowserWindow.getAllWindows().forEach(w => w.webContents.send(channel, payload))` broadcast. Pages subscribe with `window.zupAPI.on('task:started', cb)`.
- **Window data passing**: done by sending IPC events after `did-finish-load` (e.g. `new-entry:suggestions`, `update-entry:load`, `floating-button:init`).

### Main-process services (`electron/services/`)
| File | Responsibility |
|---|---|
| `database.ts` | Open/init SQLite db, bootstrap schema, run migrations |
| `taskCollection.ts` | In-memory `Map<string, ZupTask>` cache over SQLite; all task CRUD + note/tag operations |
| `settingHelper.ts` | Typed property accessors; reads/writes `tbl_Setting`. Exception: `DbPath` is stored in `userData/zup-config.json` |
| `tagHelper.ts` | Parses and runs the `~token~` export format |
| `ipcHandlers.ts` | Registers all `ipcMain.handle` handlers |
| `windowManager.ts` | Creates and manages all `BrowserWindow` instances (singleton per window type) |

### In-memory state
`taskCollection.ts` keeps tasks in a `Map<string, ZupTask>` and a `Set<string>` of running IDs. Always mutate through the exported functions — never touch the map directly. After a DB path change, call `loadTasks()` to repopulate.

## Key Conventions

### Database
- Tables are prefixed `tbl_`, column names are **PascalCase** in SQL.
- TS types (e.g. `ZupTask`) use **camelCase**; mapping happens in `rowToTask()`.
- All datetimes are stored as ISO 8601 strings.
- IDs are UUIDs (`uuid` package).
- Schema migrations: add entries to the `migrations` array in `database.ts`; each entry needs a unique `id` like `'20250001_description'` and is tracked in `tbl_MigrationHistory`.

### Settings
- Most settings live in `tbl_Setting` (key/value text pairs with a `DataType` column).
- `DbPath` is special — stored in `<userData>/zup-config.json` so it survives DB swaps.
- When adding a new setting, add it to the `DEFAULTS` map in `settingHelper.ts` and expose a typed getter/setter.

### Export format
Uses a `~token~`-delimited format: `~StartedOnTicks~^~Task~^~Comments~^~Tag[Name=Bill%].Description~^~Duration~`. Token syntax parsed by `tagHelper.ts`:
- `~Tag[Name=Bill%].Description~` — filter tag by prefix `Bill`, return `Description`
- `~Tag[Name=Exact].Description~` — exact match

### Real-time UI updates
Pages do **not** poll. They call `load()` on mount, then listen for push events (`task:started`, `task:stopped`, `task:updated`, `task:deleted`) and re-fetch on each event. Always clean up listeners in the `useEffect` return.

### Native module
`better-sqlite3` is listed in Vite's `rollupOptions.external` and is never bundled. After upgrading Electron, run `npm run rebuild`.

### App lifetime
The app **never quits** when all windows close (`app.on('window-all-closed', e => e.preventDefault())`). Exit is only via the tray menu "Exit" item or `app.quit()`.

### Backup
`backupDb()` is called automatically on new-week detection and on every `startTask()`. Backups land in `Documents/Zup/Backup/`.

### Code style
Prettier config (enforced): tabs (width 4), single quotes, print width 130, LF line endings. ESLint: zero warnings.

### Adding a new IPC channel
1. Add `ipcMain.handle('ns:action', handler)` in `ipcHandlers.ts`.
2. Expose it in `electron/preload.ts` under `window.zupAPI`.
3. Add the TypeScript signature to `src/types/globals.d.ts`.

### Adding a new window
1. Create `<name>.html` at `App/` root (copy an existing one).
2. Create `src/pages/<name>/index.tsx` with a `createRoot` call.
3. Add `<name>` to the `pages` array in `vite.config.ts`.
4. Add a `BrowserWindow` factory in `windowManager.ts`.
5. Add `ipcMain.handle('window:show<Name>', ...)` in `main.ts`.
6. Expose `show<Name>` in `preload.ts`.
