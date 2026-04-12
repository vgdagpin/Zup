import { app as H, BrowserWindow as F, ipcMain as d, shell as yt, screen as V, Tray as kt, Menu as It, nativeTheme as Rt, nativeImage as ot, globalShortcut as v } from "electron";
import { fileURLToPath as pt } from "node:url";
import m from "node:path";
import bt from "better-sqlite3";
import y from "node:fs";
import { EventEmitter as Nt } from "node:events";
import { randomFillSync as Lt, randomUUID as Ft } from "node:crypto";
let b = null, U = "";
function gt() {
  const t = H.getPath("documents"), e = m.join(t, "Zup");
  return y.existsSync(e) || y.mkdirSync(e, { recursive: !0 }), m.join(e, "Zup.db");
}
function p() {
  if (!b) throw new Error("Database not initialized. Call initDb() first.");
  return b;
}
function _t() {
  return U;
}
function Dt(t) {
  const e = t ?? gt();
  U = e;
  const n = m.dirname(e);
  return y.existsSync(n) || y.mkdirSync(n, { recursive: !0 }), b = new bt(e), b.pragma("journal_mode = WAL"), b.pragma("foreign_keys = ON"), At(b), Pt(b), b;
}
function Ct(t) {
  b && (b.close(), b = null), Dt(t);
}
function Q() {
  if (!U || !y.existsSync(U)) return null;
  const t = m.join(m.dirname(U), "Backup");
  y.existsSync(t) || y.mkdirSync(t, { recursive: !0 });
  const e = (/* @__PURE__ */ new Date()).toISOString().replace(/[-:T]/g, "").slice(0, 14), n = `${m.basename(U, ".db")}-${e}-bak.db`, a = m.join(t, n);
  return y.existsSync(a) || y.copyFileSync(U, a), t;
}
function At(t) {
  t.exec(`
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
function Pt(t) {
  const e = new Set(
    t.prepare("SELECT MigrationId FROM tbl_MigrationHistory").all().map((a) => a.MigrationId)
  ), n = [
    // Add future schema changes here
    // { id: '20250001_example', sql: 'ALTER TABLE ...' }
  ];
  for (const a of n)
    e.has(a.id) || (t.exec(a.sql), t.prepare("INSERT INTO tbl_MigrationHistory (MigrationId, AppliedAt) VALUES (?, ?)").run(
      a.id,
      (/* @__PURE__ */ new Date()).toISOString()
    ));
}
const Ut = {
  ShowQueuedTasks: "false",
  ShowRankedTasks: "false",
  ShowClosedTasks: "false",
  EntryListOpacity: "1",
  NumDaysOfDataToLoad: "30",
  ItemsToShow: "5",
  AutoOpenUpdateWindow: "false",
  UsePillTimer: "true",
  DayStart: "07:00",
  DayEnd: "18:00",
  DayEndNextDay: "false",
  TimesheetsFolder: "",
  ExportRowFormat: "~StartedOnTicks~^~Task~^~Comments~^~Tag[Name=Bill%].Description~^~Duration~^False^False",
  ExportFileExtension: "csv",
  TrimDaysToKeep: "90",
  FormLocationX: "0",
  FormLocationY: "0"
}, mt = () => m.join(H.getPath("userData"), "zup-config.json");
function st() {
  try {
    const t = mt();
    if (y.existsSync(t)) return JSON.parse(y.readFileSync(t, "utf-8"));
  } catch {
  }
  return {};
}
function xt(t) {
  y.writeFileSync(mt(), JSON.stringify(t, null, 2));
}
function Mt(t) {
  const e = p().prepare("SELECT Value FROM tbl_Setting WHERE Name = ?").get(t);
  return (e == null ? void 0 : e.Value) ?? null;
}
function Wt(t, e) {
  p().prepare("SELECT Name FROM tbl_Setting WHERE Name = ?").get(t) ? p().prepare("UPDATE tbl_Setting SET Value = ? WHERE Name = ?").run(e, t) : p().prepare("INSERT INTO tbl_Setting (Name, Value, DataType) VALUES (?, ?, ?)").run(t, e, "string");
}
function f(t) {
  return Mt(t) ?? Ut[t] ?? "";
}
function S(t, e) {
  Wt(t, e);
}
const l = {
  get ShowQueuedTasks() {
    return f("ShowQueuedTasks") === "true";
  },
  set ShowQueuedTasks(t) {
    S("ShowQueuedTasks", String(t));
  },
  get ShowRankedTasks() {
    return f("ShowRankedTasks") === "true";
  },
  set ShowRankedTasks(t) {
    S("ShowRankedTasks", String(t));
  },
  get ShowClosedTasks() {
    return f("ShowClosedTasks") === "true";
  },
  set ShowClosedTasks(t) {
    S("ShowClosedTasks", String(t));
  },
  get EntryListOpacity() {
    return parseFloat(f("EntryListOpacity"));
  },
  set EntryListOpacity(t) {
    S("EntryListOpacity", String(t));
  },
  get NumDaysOfDataToLoad() {
    return parseInt(f("NumDaysOfDataToLoad"));
  },
  set NumDaysOfDataToLoad(t) {
    S("NumDaysOfDataToLoad", String(t));
  },
  get ItemsToShow() {
    return parseInt(f("ItemsToShow"));
  },
  set ItemsToShow(t) {
    S("ItemsToShow", String(t));
  },
  get AutoOpenUpdateWindow() {
    return f("AutoOpenUpdateWindow") === "true";
  },
  set AutoOpenUpdateWindow(t) {
    S("AutoOpenUpdateWindow", String(t));
  },
  get UsePillTimer() {
    return f("UsePillTimer") === "true";
  },
  set UsePillTimer(t) {
    S("UsePillTimer", String(t));
  },
  get DayStart() {
    return f("DayStart");
  },
  set DayStart(t) {
    S("DayStart", t);
  },
  get DayEnd() {
    return f("DayEnd");
  },
  set DayEnd(t) {
    S("DayEnd", t);
  },
  get DayEndNextDay() {
    return f("DayEndNextDay") === "true";
  },
  set DayEndNextDay(t) {
    S("DayEndNextDay", String(t));
  },
  get TimesheetsFolder() {
    return f("TimesheetsFolder");
  },
  set TimesheetsFolder(t) {
    S("TimesheetsFolder", t);
  },
  get ExportRowFormat() {
    return f("ExportRowFormat");
  },
  set ExportRowFormat(t) {
    S("ExportRowFormat", t);
  },
  get ExportFileExtension() {
    return f("ExportFileExtension");
  },
  set ExportFileExtension(t) {
    S("ExportFileExtension", t);
  },
  get TrimDaysToKeep() {
    return parseInt(f("TrimDaysToKeep"));
  },
  set TrimDaysToKeep(t) {
    S("TrimDaysToKeep", String(t));
  },
  get FormLocationX() {
    return parseInt(f("FormLocationX"));
  },
  set FormLocationX(t) {
    S("FormLocationX", String(t));
  },
  get FormLocationY() {
    return parseInt(f("FormLocationY"));
  },
  set FormLocationY(t) {
    S("FormLocationY", String(t));
  },
  get DbPath() {
    const t = st();
    return t.DbPath ? t.DbPath : gt();
  },
  set DbPath(t) {
    const e = st();
    e.DbPath = t, xt(e);
  }
}, h = [];
for (let t = 0; t < 256; ++t)
  h.push((t + 256).toString(16).slice(1));
function Xt(t, e = 0) {
  return (h[t[e + 0]] + h[t[e + 1]] + h[t[e + 2]] + h[t[e + 3]] + "-" + h[t[e + 4]] + h[t[e + 5]] + "-" + h[t[e + 6]] + h[t[e + 7]] + "-" + h[t[e + 8]] + h[t[e + 9]] + "-" + h[t[e + 10]] + h[t[e + 11]] + h[t[e + 12]] + h[t[e + 13]] + h[t[e + 14]] + h[t[e + 15]]).toLowerCase();
}
const $ = new Uint8Array(256);
let j = $.length;
function Ht() {
  return j > $.length - 16 && (Lt($), j = 0), $.slice(j, j += 16);
}
const it = { randomUUID: Ft };
function Yt(t, e, n) {
  var s;
  t = t || {};
  const a = t.random ?? ((s = t.rng) == null ? void 0 : s.call(t)) ?? Ht();
  if (a.length < 16)
    throw new Error("Random bytes length must be >= 16");
  return a[6] = a[6] & 15 | 64, a[8] = a[8] & 63 | 128, Xt(a);
}
function z(t, e, n) {
  return it.randomUUID && !t ? it.randomUUID() : Yt(t);
}
const B = new Nt(), N = /* @__PURE__ */ new Map(), _ = /* @__PURE__ */ new Set();
function Y(t, e) {
  F.getAllWindows().forEach((n) => {
    n.isDestroyed() || n.webContents.send(t, e);
  });
}
function et() {
  N.clear(), _.clear();
  const t = p(), e = /* @__PURE__ */ new Date();
  e.setDate(e.getDate() - l.NumDaysOfDataToLoad);
  const n = e.toISOString(), a = t.prepare(
    `
    SELECT * FROM tbl_TaskEntry
    WHERE CreatedOn >= ? OR StartedOn IS NULL
  `
  ).all(n);
  for (const s of a) {
    const o = vt(s);
    N.set(o.id, o);
  }
}
function vt(t) {
  return {
    id: t.ID,
    task: t.Task,
    createdOn: t.CreatedOn,
    startedOn: t.StartedOn,
    endedOn: t.EndedOn,
    reminder: t.Reminder,
    rank: t.Rank,
    isRunning: !1,
    tags: []
  };
}
function Bt(t) {
  return p().prepare(
    `
    SELECT t.Name FROM tbl_Tag t
    JOIN tbl_TaskEntryTag tet ON tet.TagID = t.ID
    WHERE tet.TaskID = ?
    ORDER BY tet.CreatedOn DESC
  `
  ).all(t).map((a) => a.Name);
}
function L() {
  return Array.from(N.values());
}
function jt() {
  return Array.from(_);
}
function ft(t, e, n, a, s, o, r, c) {
  const i = p();
  Q();
  const g = (/* @__PURE__ */ new Date()).toISOString(), E = z();
  i.prepare(
    `
    INSERT INTO tbl_TaskEntry (ID, Task, CreatedOn, StartedOn)
    VALUES (?, ?, ?, ?)
  `
  ).run(E, t, g, e ? g : null);
  const T = c ? i.prepare("SELECT * FROM tbl_TaskEntry WHERE ID = ?").get(c) : null;
  if (T) {
    if (s) {
      const D = i.prepare("SELECT * FROM tbl_TaskEntryNote WHERE TaskID = ?").all(T.ID);
      for (const I of D)
        i.prepare(
          `
          INSERT INTO tbl_TaskEntryNote (ID, TaskID, Notes, RTF, CreatedOn, UpdatedOn)
          VALUES (?, ?, ?, ?, ?, ?)
        `
        ).run(z(), E, I.Notes, I.RTF, I.CreatedOn, I.UpdatedOn);
    }
    if (o) {
      const D = i.prepare("SELECT * FROM tbl_TaskEntryTag WHERE TaskID = ?").all(T.ID);
      for (const I of D)
        i.prepare(
          `
          INSERT OR IGNORE INTO tbl_TaskEntryTag (TaskID, TagID, CreatedOn) VALUES (?, ?, ?)
        `
        ).run(E, I.TagID, I.CreatedOn);
    }
    a && (i.prepare("DELETE FROM tbl_TaskEntry WHERE ID = ?").run(T.ID), N.delete(T.ID));
  }
  if (r && !T) {
    const D = /* @__PURE__ */ new Date();
    D.setDate(D.getDate() - l.NumDaysOfDataToLoad);
    const I = i.prepare(
      `
      SELECT DISTINCT tet.TagID FROM tbl_TaskEntry te
      JOIN tbl_TaskEntryTag tet ON tet.TaskID = te.ID
      WHERE te.Task = ? AND te.CreatedOn >= ?
      ORDER BY tet.CreatedOn DESC
    `
    ).all(t, D.toISOString());
    for (const { TagID: wt } of I)
      i.prepare("INSERT OR IGNORE INTO tbl_TaskEntryTag (TaskID, TagID, CreatedOn) VALUES (?, ?, ?)").run(
        E,
        wt,
        g
      );
  }
  const u = {
    id: E,
    task: t,
    createdOn: g,
    startedOn: e ? g : null,
    endedOn: null,
    reminder: null,
    rank: null,
    isRunning: e,
    tags: Bt(E)
  };
  if (N.set(E, u), e && _.add(E), e && n)
    for (const D of Array.from(_))
      D !== E && nt(D);
  return Y("task:started", u), e && B.emit("task:started", u), u;
}
function nt(t, e) {
  const n = N.get(t);
  return n ? (n.endedOn = e ?? (/* @__PURE__ */ new Date()).toISOString(), n.isRunning = !1, _.delete(t), p().prepare("UPDATE tbl_TaskEntry SET EndedOn = ? WHERE ID = ?").run(n.endedOn, t), Y("task:stopped", n), B.emit("task:stopped", n), n) : null;
}
function Vt(t) {
  p().prepare("DELETE FROM tbl_TaskEntry WHERE ID = ?").run(t);
  const e = N.get(t);
  N.delete(t), _.delete(t), e && Y("task:deleted", e);
}
function $t(t, e) {
  const n = p(), a = N.get(t);
  return a ? (n.prepare(
    `
    UPDATE tbl_TaskEntry SET Task = ?, StartedOn = ?, EndedOn = ?, Rank = ? WHERE ID = ?
  `
  ).run(e.task, e.startedOn, e.endedOn, e.rank, t), Qt(t, e.tags), a.task = e.task, a.startedOn = e.startedOn, a.endedOn = e.endedOn, a.rank = e.rank, a.tags = e.tags, Y("task:updated", a), a) : null;
}
function Kt(t) {
  const e = N.get(t);
  if (!e) return null;
  const n = (/* @__PURE__ */ new Date()).toISOString();
  return e.startedOn = n, e.isRunning = !0, _.add(t), p().prepare("UPDATE tbl_TaskEntry SET StartedOn = ? WHERE ID = ?").run(n, t), Y("task:started", e), B.emit("task:started", e), e;
}
function zt(t) {
  const e = N.get(t);
  if (!e) return null;
  const n = (/* @__PURE__ */ new Date()).toISOString();
  return e.startedOn = n, p().prepare("UPDATE tbl_TaskEntry SET StartedOn = ? WHERE ID = ?").run(n, t), Y("task:updated", e), e;
}
function Jt(t) {
  Q();
  const e = p(), n = /* @__PURE__ */ new Date();
  n.setDate(n.getDate() - t);
  const a = n.toISOString();
  e.prepare(
    `
    DELETE FROM tbl_TaskEntryNote WHERE TaskID IN (
      SELECT ID FROM tbl_TaskEntry WHERE StartedOn < ?
    )
  `
  ).run(a);
  const s = e.prepare("DELETE FROM tbl_TaskEntry WHERE StartedOn < ?").run(a);
  e.exec("VACUUM");
  const o = s.changes;
  return et(), o;
}
function Qt(t, e) {
  const n = p(), a = n.prepare(
    `
    SELECT t.ID, t.Name FROM tbl_Tag t
    JOIN tbl_TaskEntryTag tet ON tet.TagID = t.ID
    WHERE tet.TaskID = ?
  `
  ).all(t);
  for (const r of a)
    e.includes(r.Name) || n.prepare("DELETE FROM tbl_TaskEntryTag WHERE TaskID = ? AND TagID = ?").run(t, r.ID);
  const s = n.prepare("SELECT ID, Name FROM tbl_Tag WHERE Name IN (" + e.map(() => "?").join(",") + ")").all(...e), o = new Map(s.map((r) => [r.Name, r.ID]));
  for (const r of [...new Set(e)]) {
    if (a.some((i) => i.Name === r)) continue;
    let c = o.get(r);
    c || (c = z(), n.prepare("INSERT INTO tbl_Tag (ID, Name) VALUES (?, ?)").run(c, r)), n.prepare("INSERT OR IGNORE INTO tbl_TaskEntryTag (TaskID, TagID, CreatedOn) VALUES (?, ?, ?)").run(
      t,
      c,
      (/* @__PURE__ */ new Date()).toISOString()
    );
  }
}
function lt(t) {
  return p().prepare("SELECT * FROM tbl_TaskEntryNote WHERE TaskID = ? ORDER BY CreatedOn ASC").all(t).map((n) => ({
    id: n.ID,
    notes: n.Notes,
    rtf: n.RTF,
    createdOn: n.CreatedOn,
    updatedOn: n.UpdatedOn,
    summary: n.Notes.slice(0, 60).replace(/\s+/g, " ")
  }));
}
function Gt(t, e, n, a) {
  const s = p();
  if (e)
    return n.trim() ? (s.prepare("UPDATE tbl_TaskEntryNote SET Notes = ?, RTF = ?, UpdatedOn = ? WHERE ID = ?").run(
      n,
      a,
      (/* @__PURE__ */ new Date()).toISOString(),
      e
    ), { id: e }) : (s.prepare("DELETE FROM tbl_TaskEntryNote WHERE ID = ?").run(e), null);
  {
    if (!n.trim()) return null;
    const o = z();
    return s.prepare("INSERT INTO tbl_TaskEntryNote (ID, TaskID, Notes, RTF, CreatedOn) VALUES (?, ?, ?, ?, ?)").run(
      o,
      t,
      n,
      a,
      (/* @__PURE__ */ new Date()).toISOString()
    ), { id: o };
  }
}
function qt(t) {
  p().prepare("DELETE FROM tbl_TaskEntryNote WHERE ID = ?").run(t);
}
function Zt() {
  return p().prepare("SELECT * FROM tbl_Tag ORDER BY Name").all().map((t) => ({ id: t.ID, name: t.Name, description: t.Description }));
}
function te(t, e, n) {
  p().prepare("UPDATE tbl_Tag SET Name = ?, Description = ? WHERE ID = ?").run(e, n, t);
}
function ee(t) {
  return p().prepare("SELECT 1 FROM tbl_TaskEntryTag WHERE TagID = ? LIMIT 1").get(t) ? !1 : (p().prepare("DELETE FROM tbl_Tag WHERE ID = ?").run(t), !0);
}
function ne(t, e, n) {
  const a = p(), s = Array.from(_);
  let o;
  if (n && n.trim()) {
    const r = `%${n.toLowerCase()}%`;
    o = a.prepare("SELECT * FROM tbl_TaskEntry WHERE lower(Task) LIKE ? ORDER BY StartedOn DESC").all(r);
  } else
    o = a.prepare(
      `
      SELECT * FROM tbl_TaskEntry
      WHERE (StartedOn >= ? AND StartedOn <= ?) OR StartedOn IS NULL
      ORDER BY StartedOn DESC
    `
    ).all(t, e);
  return o.map((r) => {
    const c = a.prepare(
      `
      SELECT t.Name FROM tbl_Tag t JOIN tbl_TaskEntryTag tet ON tet.TagID = t.ID
      WHERE tet.TaskID = ? ORDER BY tet.CreatedOn DESC
    `
    ).all(r.ID);
    let i = null, g = null;
    if (r.StartedOn && r.EndedOn) {
      i = (new Date(r.EndedOn).getTime() - new Date(r.StartedOn).getTime()) / 1e3;
      const E = Math.floor(i / 3600), T = Math.floor(i % 3600 / 60), u = Math.floor(i % 60);
      g = `${String(E).padStart(2, "0")}:${String(T).padStart(2, "0")}:${String(u).padStart(2, "0")}`;
    }
    return {
      id: r.ID,
      task: r.Task,
      createdOn: r.CreatedOn,
      startedOn: r.StartedOn,
      endedOn: r.EndedOn,
      reminder: r.Reminder,
      rank: r.Rank,
      isRunning: s.includes(r.ID),
      tags: c.map((E) => E.Name),
      duration: i,
      durationString: g
    };
  });
}
function Z(t) {
  const e = new Date(Date.UTC(t.getFullYear(), t.getMonth(), t.getDate()));
  e.setUTCDate(e.getUTCDate() + 4 - (e.getUTCDay() || 7));
  const n = new Date(Date.UTC(e.getUTCFullYear(), 0, 1));
  return Math.ceil(((e.getTime() - n.getTime()) / 864e5 + 1) / 7);
}
function ae(t) {
  const e = /* @__PURE__ */ new Map(), n = new Date(t, 0, 1), a = new Date(t, 11, 31);
  for (let s = new Date(n); s <= a; s.setDate(s.getDate() + 1)) {
    const o = Z(s);
    if (e.has(o)) continue;
    const r = s.getDay(), c = r === 0 ? 0 : -r, i = new Date(s);
    i.setDate(s.getDate() + c);
    const g = new Date(i);
    g.setDate(i.getDate() + 6), g.setHours(23, 59, 59, 999);
    const E = (T) => `${String(T.getMonth() + 1).padStart(2, "0")}/${String(T.getDate()).padStart(2, "0")}/${String(T.getFullYear()).slice(2)}`;
    e.set(o, {
      weekNumber: o,
      start: i.toISOString(),
      end: g.toISOString(),
      label: `${E(i)} - ${E(g)}`
    });
  }
  return Array.from(e.values()).sort((s, o) => s.weekNumber - o.weekNumber);
}
function re(t) {
  if (!t) return null;
  const e = { key: t }, n = t.match(/^(\w+)(?:\[(\d+)\](?:\.(\w+))?|\[(\w+)=([^\]]+)\](?:\.(\w+))?)?$/);
  return n && (e.key = n[1], e.index = n[2] != null ? parseInt(n[2]) : null, e.propertyName = n[3] ?? n[6] ?? null, e.indexPropertyName = n[4] ?? null, e.indexPropertyValue = n[5] ?? null), e;
}
function oe(t) {
  return [...t.matchAll(/~([^~]+)~/g)].map((e) => e[1]);
}
const tt = /* @__PURE__ */ new Map();
function C(t, e) {
  tt.has(t) || tt.set(t, e);
}
function se(t, e) {
  const n = re(t);
  if (!n) return null;
  const a = tt.get(n.key);
  return a ? a(n, e) : null;
}
function ie() {
  d.handle("tasks:getAll", () => L()), d.handle("tasks:getRunningIds", () => jt()), d.handle(
    "tasks:start",
    (t, e) => ft(
      e.entry,
      e.startNow,
      e.stopOtherTask,
      e.hideParent,
      e.bringNotes,
      e.bringTags,
      e.getTags,
      e.parentEntryId
    )
  ), d.handle("tasks:stop", (t, e, n) => nt(e, n)), d.handle("tasks:delete", (t, e) => Vt(e)), d.handle("tasks:update", (t, e, n) => $t(e, n)), d.handle("tasks:resume", (t, e) => Kt(e)), d.handle("tasks:reset", (t, e) => zt(e)), d.handle("tasks:trim", (t, e) => Jt(e)), d.handle("tasks:query", (t, e, n, a) => ne(e, n, a)), d.handle("notes:getForTask", (t, e) => lt(e)), d.handle(
    "notes:save",
    (t, e, n, a, s) => Gt(e, n, a, s)
  ), d.handle("notes:delete", (t, e) => qt(e)), d.handle("tags:getAll", () => Zt()), d.handle("tags:update", (t, e, n, a) => te(e, n, a)), d.handle("tags:delete", (t, e) => ee(e)), d.handle("settings:getAll", () => ({
    showQueuedTasks: l.ShowQueuedTasks,
    showRankedTasks: l.ShowRankedTasks,
    showClosedTasks: l.ShowClosedTasks,
    entryListOpacity: l.EntryListOpacity,
    numDaysOfDataToLoad: l.NumDaysOfDataToLoad,
    itemsToShow: l.ItemsToShow,
    autoOpenUpdateWindow: l.AutoOpenUpdateWindow,
    usePillTimer: l.UsePillTimer,
    dayStart: l.DayStart,
    dayEnd: l.DayEnd,
    dayEndNextDay: l.DayEndNextDay,
    timesheetsFolder: l.TimesheetsFolder,
    exportRowFormat: l.ExportRowFormat,
    exportFileExtension: l.ExportFileExtension,
    trimDaysToKeep: l.TrimDaysToKeep,
    formLocationX: l.FormLocationX,
    formLocationY: l.FormLocationY,
    dbPath: l.DbPath
  })), d.handle("settings:set", (t, e, n) => {
    const a = l, o = {
      showQueuedTasks: "ShowQueuedTasks",
      showRankedTasks: "ShowRankedTasks",
      showClosedTasks: "ShowClosedTasks",
      entryListOpacity: "EntryListOpacity",
      numDaysOfDataToLoad: "NumDaysOfDataToLoad",
      itemsToShow: "ItemsToShow",
      autoOpenUpdateWindow: "AutoOpenUpdateWindow",
      usePillTimer: "UsePillTimer",
      dayStart: "DayStart",
      dayEnd: "DayEnd",
      dayEndNextDay: "DayEndNextDay",
      timesheetsFolder: "TimesheetsFolder",
      exportRowFormat: "ExportRowFormat",
      exportFileExtension: "ExportFileExtension",
      trimDaysToKeep: "TrimDaysToKeep",
      formLocationX: "FormLocationX",
      formLocationY: "FormLocationY",
      dbPath: "DbPath"
    }[e];
    return o && (a[o] = n), !0;
  }), d.handle("settings:changeDbPath", (t, e) => (l.DbPath = e, Ct(e), et(), !0)), d.handle("db:backup", () => Q()), d.handle("db:getCurrentPath", () => _t()), d.handle("utility:getWeekData", (t, e) => ae(e)), d.handle("export:generate", (t, e, n, a, s, o) => {
    C("StartedOnTicks", (T, u) => u.startedOn ? String(new Date(u.startedOn).getTime()) : ""), C("Task", (T, u) => String(u.task ?? "").slice(0, 200)), C("Duration", (T, u) => String(u.durationString ?? "")), C("TimesheetDate", (T, u) => o), C("Comments", (T, u) => lt(u.id).map((I) => I.notes).join("; ")), C("Tags", (T, u) => (u.tags ?? []).join(", ")), C("Tag", (T, u) => {
      const D = u.tags ?? [];
      return Array.isArray(D) && D.length > 0 && typeof D[0] == "string" ? le(T, u.id, T.indexPropertyValue) : "";
    });
    const r = oe(n), i = e.map((T) => {
      const u = T;
      return r.map((D) => se(D, u) ?? "").join("^");
    }).join(`
`), g = `Timesheet-${o.replace(/\//g, "-")}.${s}`, E = m.join(a, g);
    return y.writeFileSync(E, i, "utf-8"), E;
  }), d.handle("shell:openPath", (t, e) => yt.openPath(e));
}
function le(t, e, n) {
  if (!t) return "";
  const s = p().prepare(
    `
    SELECT t.Name, t.Description FROM tbl_Tag t
    JOIN tbl_TaskEntryTag tet ON tet.TagID = t.ID
    WHERE tet.TaskID = ?
  `
  ).all(e);
  if (n != null && n.endsWith("%")) {
    const o = n.slice(0, -1), r = s.find((c) => c.Name.startsWith(o));
    if (r) return t.propertyName === "Description" ? r.Description ?? "" : r.Name;
  } else if (n) {
    const o = s.find((r) => r.Name === n);
    if (o) return t.propertyName === "Description" ? o.Description ?? "" : o.Name;
  }
  return "";
}
const de = m.dirname(pt(import.meta.url)), J = process.env.VITE_DEV_SERVER_URL, at = m.join(process.env.APP_ROOT ?? "", "dist");
function Te(t) {
  return J ? `${J}${t}.html` : m.join(at, `${t}.html`);
}
function M(t, e) {
  J ? t.loadURL(Te(e)) : t.loadFile(m.join(at, `${e}.html`));
}
const W = m.join(de, "preload.mjs");
let k = null, R = null, O = null, A = null, P = null, w = null;
const K = [];
let x = null;
function X(t) {
  return t != null && !t.isDestroyed();
}
function ue() {
  return X(k) || (k = new F({
    width: 320,
    height: 400,
    frame: !1,
    resizable: !1,
    alwaysOnTop: !0,
    skipTaskbar: !0,
    transparent: !0,
    webPreferences: { preload: W, contextIsolation: !0 }
  }), M(k, "entry-list"), k.on("closed", () => {
    k = null;
  })), k;
}
function St(t) {
  X(R) || (R = new F({
    width: 420,
    height: 300,
    frame: !1,
    resizable: !1,
    skipTaskbar: !0,
    webPreferences: { preload: W, contextIsolation: !0 }
  }), M(R, "new-entry"), R.on("closed", () => {
    R = null;
  })), R.webContents.once("did-finish-load", () => {
    R == null || R.webContents.send("new-entry:suggestions", t);
  }), R.show(), R.focus();
}
function ht(t) {
  X(O) || (O = new F({
    width: 640,
    height: 580,
    webPreferences: { preload: W, contextIsolation: !0 }
  }), M(O, "update-entry"), O.on("closed", () => {
    O = null;
  })), O.webContents.once("did-finish-load", () => {
    O == null || O.webContents.send("update-entry:load", t);
  }), O.webContents.isLoading() || O.webContents.send("update-entry:load", t), O.show(), O.focus();
}
function G() {
  X(A) || (A = new F({
    width: 960,
    height: 680,
    webPreferences: { preload: W, contextIsolation: !0 }
  }), M(A, "view-list"), A.on("closed", () => {
    A = null;
  })), A.show(), A.focus();
}
function dt() {
  X(P) || (P = new F({
    width: 520,
    height: 600,
    resizable: !1,
    webPreferences: { preload: W, contextIsolation: !0 }
  }), M(P, "settings"), P.on("closed", () => {
    P = null;
  })), P.show(), P.focus();
}
function Tt(t) {
  X(w) || (w = new F({
    width: 480,
    height: 440,
    webPreferences: { preload: W, contextIsolation: !0 }
  }), M(w, "tag-editor"), w.on("closed", () => {
    w = null;
  })), w.show(), w.focus(), t && (w.webContents.once("did-finish-load", () => {
    w == null || w.webContents.send("tag-editor:select", t);
  }), w.webContents.isLoading() || w.webContents.send("tag-editor:select", t));
}
function ut(t, e, n, a, s) {
  const o = new F({
    width: 220,
    height: 44,
    x: a,
    y: s,
    frame: !1,
    transparent: !0,
    alwaysOnTop: !0,
    skipTaskbar: !0,
    resizable: !1,
    webPreferences: { preload: W, contextIsolation: !0 }
  });
  return M(o, "floating-button"), o.webContents.once("did-finish-load", () => {
    o.webContents.send("floating-button:init", { taskId: t, taskName: e, startedOn: n });
  }), o.on("closed", () => {
    const r = K.indexOf(o);
    r !== -1 && K.splice(r, 1);
  }), K.push(o), o.show(), o;
}
function ct() {
  return K.filter((t) => !t.isDestroyed());
}
function Ot() {
  const t = process.env.VITE_PUBLIC ?? "", e = Rt.shouldUseDarkColors ? "tray-icon-white.png" : "tray-icon.png", n = m.join(t, e), a = ot.createFromPath(n);
  return a.isEmpty() ? ot.createFromPath(m.join(t, "tray-icon.png")) : a;
}
function ce(t, e, n, a) {
  x = new kt(Ot()), x.setToolTip("Zup");
  const s = It.buildFromTemplate([
    { label: "New Entry (Shift+Alt+J)", click: t },
    { type: "separator" },
    { label: "View All (Shift+Alt+P)", click: e },
    { label: "Settings", click: n },
    { label: "Tag Editor", click: a },
    { type: "separator" },
    { label: "Exit", click: () => H.quit() }
  ]);
  x.setContextMenu(s), x.on("double-click", e);
}
function q(t) {
  if (!x) return;
  x.setImage(Ot());
  const e = t > 0 ? `Zup (${t} queued)` : "Zup";
  x.setToolTip(e);
}
function Ee() {
  if (!X(k)) return;
  const { width: t, height: e } = V.getPrimaryDisplay().workAreaSize;
  k.setPosition(
    Math.floor(t / 2 - k.getBounds().width / 2),
    Math.floor(e / 2 - k.getBounds().height / 2)
  ), k.show(), k.focus();
}
const pe = m.dirname(pt(import.meta.url));
process.env.APP_ROOT = m.join(pe, "..");
process.env.VITE_PUBLIC = J ? m.join(process.env.APP_ROOT, "public") : at;
H.on("window-all-closed", (t) => t.preventDefault());
H.whenReady().then(() => {
  Dt(l.DbPath), et(), ie(), d.handle("window:showNewEntry", () => {
    const o = L().filter((r) => r.endedOn != null).map((r) => r.task).filter((r, c, i) => i.indexOf(r) === c).slice(0, 20);
    St(o);
  }), d.handle("window:showUpdateEntry", (o, r) => ht(r)), d.handle("window:showViewList", () => G()), d.handle("window:showSettings", () => dt()), d.handle("window:showTagEditor", (o, r) => Tt(r)), d.handle("window:moveEntryListToCenter", () => Ee()), d.handle("window:savePosition", (o, r, c) => {
    l.FormLocationX = r, l.FormLocationY = c;
  }), d.handle("window:move", (o, r, c) => {
    const i = F.fromWebContents(o.sender);
    i && !i.isDestroyed() && i.setPosition(Math.round(r), Math.round(c));
  }), d.handle("window:createFloatingButton", (o, r, c, i) => {
    const g = ct(), { width: E, height: T } = V.getPrimaryDisplay().workAreaSize, u = l.FormLocationX || E - 240, D = (l.FormLocationY || T - 60) + g.length * 50;
    ut(r, c, i, u, D);
  });
  const t = ue(), { width: e, height: n } = V.getPrimaryDisplay().workAreaSize, a = l.FormLocationX || e - 325, s = l.FormLocationY || n - 420;
  t.setPosition(a, s), t.show(), t.on("moved", () => {
    const [o, r] = t.getPosition();
    l.FormLocationX = o, l.FormLocationY = r;
  }), ce(
    () => Et(),
    () => G(),
    () => dt(),
    () => Tt()
  ), q(L().filter((o) => o.startedOn == null).length), B.on("task:started", (o) => {
    if (!l.UsePillTimer) return;
    const r = ct(), { width: c, height: i } = V.getPrimaryDisplay().workAreaSize, g = l.FormLocationX || c - 240, E = (l.FormLocationY || i - 60) + r.length * 50;
    ut(o.id, o.task, o.startedOn, g, E), q(L().filter((T) => T.startedOn == null).length);
  }), B.on("task:stopped", () => {
    q(L().filter((o) => o.startedOn == null).length);
  }), v.register("Shift+Alt+J", () => Et()), v.register("Shift+Alt+K", () => ge()), v.register("Shift+Alt+L", () => De()), v.register("Shift+Alt+P", () => G()), H.on("will-quit", () => v.unregisterAll());
});
function Et() {
  rt();
  const t = L().filter((e) => e.endedOn != null).map((e) => e.task).filter((e, n, a) => a.indexOf(e) === n).slice(0, 20);
  St(t);
}
function ge() {
  rt();
  const t = L().find((e) => e.isRunning);
  t && ht(t.id);
}
function De() {
  rt();
  const t = L(), e = t.find((n) => n.isRunning);
  if (e)
    nt(e.id);
  else {
    const n = t.filter((a) => a.endedOn != null).sort((a, s) => new Date(s.endedOn).getTime() - new Date(a.endedOn).getTime())[0];
    n && ft(n.task, !0, !0, !1, !1, !1, !1);
  }
}
function rt() {
  const t = L();
  if (!t.length) return;
  const e = t.flatMap((a) => [a.createdOn, a.startedOn, a.endedOn].filter(Boolean)), n = new Date(Math.max(...e.map((a) => new Date(a).getTime())));
  Z(n) < Z(/* @__PURE__ */ new Date()) && Q();
}
