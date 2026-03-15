# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Zup** is a Windows Forms desktop task tracker with a floating widget, global keyboard shortcuts, and SQLite persistence. Target framework: `net10.0-windows7.0`.

## Build & Run Commands

All commands run from `App/` directory:

```bash
dotnet build                   # Debug build
dotnet build -c Release        # Release build
dotnet run --project Zup       # Run the application
dotnet ef migrations add <Name> --project Zup  # Add EF migration
dotnet ef database update --project Zup        # Apply migrations
```

There are no automated tests in this project.

## Architecture

### Dependency Injection (`Program.cs`)
All forms are registered as **Transient** services. `SettingHelper` and `TaskCollection` are **Singletons**. `ZupDbContext` is registered as a Singleton pointing to the SQLite database at `%USERPROFILE%\Documents\Zup\Zup.db`.

### Core Business Logic
- **`TaskCollection.cs`** — Central task manager. All task state mutations (Add, Start, Stop, Delete, Update, Resume, Reset) go through here. Fires events (`OnTaskStarted`, `OnTaskStopped`, `OnTaskDeleted`, `OnTaskUpdated`) that forms subscribe to for UI updates.
- **`SettingHelper.cs`** — Singleton for all user preferences (opacity, window position, day start/end times, data path). Settings are persisted in the SQLite database (`tbl_Setting`).
- **`ZupDbContext.cs`** — EF Core context with automatic backup before migrations. 5 tables: `tbl_TaskEntry`, `tbl_TaskEntryNote`, `tbl_TaskEntryTag`, `tbl_Tag`, `tbl_Setting`.

### UI Forms
- **`frmMain`** — Invisible host window; registers global hotkeys via P/Invoke (`Shift+Alt+J/K/L`).
- **`frmFloatingButton`** — The visible floating widget on the screen corner. Custom GDI+ rendering, high-DPI aware, shows running task duration.
- **`frmEntryList`** — Popup list showing active, previous, and queued tasks.
- **`frmViewList`** — Full task browser with filtering.
- **`frmUpdateEntry`** — Task edit dialog with RTF notes and tag management.

### Data Flow
User action → Form → `TaskCollection` method → EF Core → SQLite → `TaskCollection` fires event → subscribed forms refresh UI.

### Custom Controls (`CustomControls/`)
- **`EachEntry`** — Renders individual tasks in list views.
- **`TokenBox/`** — Tag autocomplete input (Token, AutoCompleteTextBox, TokenBox).

### Domain Model
`ZupTask` (in-memory model with computed properties like `Duration`, `DurationString`, `Tags`) wraps `tbl_TaskEntry` (database entity). `TaskStatus` enum: `Ongoing`, `Queued`, `Ranked`, `Closed`, `Unclosed`, `Running`.
