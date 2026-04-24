# Copilot Instructions for Zup

**Zup** is a Windows Forms desktop task tracker (tray app) with a floating pill-timer widget, global hotkeys, and SQLite persistence. Target framework: `net10.0-windows7.0`.

## Build & Run

All commands run from `App/` directory:

```bash
dotnet build                    # Debug build
dotnet build -c Release         # Release build
dotnet run --project Zup        # Run the application
```

**There are no automated tests.** Manual verification by running the app is the only test path.

### After completing a task

After finishing any code change, run a Release publish:

```bash
dotnet publish App/Zup/Zup.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish/
```

### EF Core migrations

```bash
dotnet ef migrations add <Name> --project Zup   # Create migration
dotnet ef database update --project Zup         # Apply migrations
```

### Release

Releases are published via the manual `release.yml` workflow. Version is read from `AssemblyVersion` in `Zup.csproj`. The workflow publishes a self-contained single-file `win-x64` binary.

## Architecture

### Dependency Injection (`Program.cs`)

- **Transient**: all Forms (`frmMain`, `frmFloatingButton`, `frmEntryList`, `frmSetting`, `frmViewList`, `frmNewEntry`, `frmUpdateEntry`, `frmTagEditor`)
- **Singleton**: `SettingHelper`, `TaskCollection`, `ZupDbContext`

Forms are resolved lazily in `frmMain` via cached backing fields with disposed-checks (e.g., `if (frmView == null || frmView.IsDisposed)`). Never instantiate forms with `new` — always get them from the DI container via `serviceProvider.GetRequiredService<T>()`.

### Central state: `TaskCollection`

`TaskCollection` is the single source of truth for all in-memory task state. **All task mutations must go through its methods**:

| Method | What it does |
|--------|-------------|
| `Start(...)` | Creates a new `tbl_TaskEntry`, calls `BackupDb()`, fires `OnTaskStarted` |
| `Stop(sender, id)` | Sets `EndedOn`, fires `OnTaskStopped` |
| `Resume(sender, id)` | Restarts a stopped task without creating a new entry |
| `Delete(sender, id)` | Hard-deletes via `ExecuteDelete()`, fires `OnTaskDeleted` |
| `Update(sender, id, task)` | Updates fields + tags, fires `OnTaskUpdated` |
| `Reset(sender, id)` | Resets `StartedOn` to now, fires `OnTaskUpdated` |

Forms subscribe to the four events (`OnTaskStarted`, `OnTaskStopped`, `OnTaskDeleted`, `OnTaskUpdated`) to refresh their UI — they do **not** poll or re-query directly.

### Data flow

```
User action → Form → TaskCollection method → EF Core (ZupDbContext) → SQLite
                                           ↓
                              TaskCollection fires event
                                           ↓
                              Subscribed forms refresh UI
```

### Domain model

- **`ITask`** — interface; `GetTaskStatus()` extension method derives status from field combinations:
  - `IsRunning = true` → `Running`
  - `Rank != null` → `Ranked`
  - `StartedOn == null` → `Queued`
  - `StartedOn != null && EndedOn == null` → `Unclosed`
  - `StartedOn != null && EndedOn != null` → `Closed`
- **`ZupTask`** — in-memory model implementing `ITask`; adds computed properties (`Duration`, `DurationString`, `Tags`).
- **`tbl_TaskEntry`** — EF Core entity (database row).

`ZupTask` wraps `tbl_TaskEntry` — when loading from DB, construct a `ZupTask` from the entity fields; don't pass entities directly to the UI.

### Database

- SQLite file at `%USERPROFILE%\Documents\Zup\Zup.db` (path stored in `Properties.Settings.Default.DbPath`)
- 5 tables: `tbl_TaskEntry`, `tbl_TaskEntryNote`, `tbl_TaskEntryTag`, `tbl_Tag`, `tbl_Setting`
- Migrations history table: `tbl_MigrationHistory`
- `ZupDbContext.BackupDb()` copies the `.db` file to a `Backup/` subfolder — **call it before any destructive operation** (already called in `Start`, `TrimDb`, and new-week detection)

### Settings

`SettingHelper` stores all user preferences in `tbl_Setting` as key-value strings. Default values come from `Properties.Settings.Default` (the `.settings` file). `DbPath` is the exception — it's stored only in `Properties.Settings.Default`, not in the SQLite table.

### UI forms

- **`frmMain`** — invisible host; owns the system tray icon, global hotkeys via P/Invoke, and coordinates all other forms. Hotkeys: `Shift+Alt+J` (new entry), `Shift+Alt+K` (update running task), `Shift+Alt+L` (toggle last task), `Shift+Alt+P` (view all).
- **`frmFloatingButton`** — the draggable pill widget; custom GDI+ rendering (`CreateButtonBitmap`), high-DPI aware with `BITMAP_SCALE = 4` supersampling.
- **`frmEntryList`** — popup showing active/previous/queued tasks.
- **`frmViewList`** — full task browser with filtering.
- **`frmUpdateEntry`** — task edit dialog with RTF notes and tag management.

### Custom controls (`CustomControls/`)

- **`EachEntry`** — renders individual task rows in list views.
- **`TokenBox/`** — tag autocomplete input (`Token`, `AutoCompleteTextBox`, `TokenBox`).

## Key Conventions

- **Form retrieval in `frmMain`**: each child form has a private lazy property (e.g., `m_FormEntryList`) that re-creates it if disposed. Follow this pattern when adding forms.
- **Settings persistence**: add new settings as properties on `SettingHelper` backed by `GetSetting`/`SetSetting` with a default from `Properties.Settings.Default`. Call `SettingHelper.Save()` (which calls `dbContext.SaveChanges()`) to persist.
- **Tray icon badge**: `frmMain.SetIcon(queueCount)` regenerates the tray icon with a GDI+ count overlay. Call it after any operation that changes the queued task count.
- **Tag management**: tags are always managed through `TaskCollection.SaveTags()` (called inside `Update`). Create `tbl_Tag` rows on demand if a tag name doesn't exist yet.
- **Position saving**: form moves are debounced through `tmrSaveSetting` (1-shot timer) before writing `FormLocationX`/`FormLocationY` to `SettingHelper`.
- **DB table naming**: all database tables use the `tbl_` prefix.
- **Entity configs**: EF Core model configuration is applied via `modelBuilder.ApplyConfigurationsFromAssembly(...)` — add new entity configs as `IEntityTypeConfiguration<T>` classes in the same assembly.
