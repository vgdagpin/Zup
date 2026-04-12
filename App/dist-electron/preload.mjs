"use strict";
const electron = require("electron");
electron.contextBridge.exposeInMainWorld("zupAPI", {
  // Tasks
  getTasks: () => electron.ipcRenderer.invoke("tasks:getAll"),
  getRunningIds: () => electron.ipcRenderer.invoke("tasks:getRunningIds"),
  startTask: (args) => electron.ipcRenderer.invoke("tasks:start", args),
  stopTask: (taskId, endOn) => electron.ipcRenderer.invoke("tasks:stop", taskId, endOn),
  deleteTask: (taskId) => electron.ipcRenderer.invoke("tasks:delete", taskId),
  updateTask: (taskId, args) => electron.ipcRenderer.invoke("tasks:update", taskId, args),
  resumeTask: (taskId) => electron.ipcRenderer.invoke("tasks:resume", taskId),
  resetTask: (taskId) => electron.ipcRenderer.invoke("tasks:reset", taskId),
  trimTasks: (days) => electron.ipcRenderer.invoke("tasks:trim", days),
  queryTasks: (from, to, search) => electron.ipcRenderer.invoke("tasks:query", from, to, search),
  // Notes
  getNotes: (taskId) => electron.ipcRenderer.invoke("notes:getForTask", taskId),
  saveNote: (taskId, noteId, notes, rtf) => electron.ipcRenderer.invoke("notes:save", taskId, noteId, notes, rtf),
  deleteNote: (noteId) => electron.ipcRenderer.invoke("notes:delete", noteId),
  // Tags
  getTags: () => electron.ipcRenderer.invoke("tags:getAll"),
  updateTag: (tagId, name, description) => electron.ipcRenderer.invoke("tags:update", tagId, name, description),
  deleteTag: (tagId) => electron.ipcRenderer.invoke("tags:delete", tagId),
  // Settings
  getSettings: () => electron.ipcRenderer.invoke("settings:getAll"),
  setSetting: (key, value) => electron.ipcRenderer.invoke("settings:set", key, value),
  changeDbPath: (newPath) => electron.ipcRenderer.invoke("settings:changeDbPath", newPath),
  // DB
  backupDb: () => electron.ipcRenderer.invoke("db:backup"),
  getDbPath: () => electron.ipcRenderer.invoke("db:getCurrentPath"),
  // Utility
  getWeekData: (year) => electron.ipcRenderer.invoke("utility:getWeekData", year),
  // Export
  exportTimesheets: (tasks, format, folder, ext, dateStr) => electron.ipcRenderer.invoke("export:generate", tasks, format, folder, ext, dateStr),
  // Window management
  showNewEntry: () => electron.ipcRenderer.invoke("window:showNewEntry"),
  showUpdateEntry: (taskId) => electron.ipcRenderer.invoke("window:showUpdateEntry", taskId),
  showViewList: () => electron.ipcRenderer.invoke("window:showViewList"),
  showSettings: () => electron.ipcRenderer.invoke("window:showSettings"),
  showTagEditor: (tag) => electron.ipcRenderer.invoke("window:showTagEditor", tag),
  savePosition: (x, y) => electron.ipcRenderer.invoke("window:savePosition", x, y),
  moveWindow: (x, y) => electron.ipcRenderer.invoke("window:move", x, y),
  createFloatingButton: (taskId, taskName, startedOn) => electron.ipcRenderer.invoke("window:createFloatingButton", taskId, taskName, startedOn),
  // Shell
  openPath: (p) => electron.ipcRenderer.invoke("shell:openPath", p),
  // Events (renderer subscribes to main-process push events)
  on: (channel, fn) => {
    electron.ipcRenderer.on(channel, (_e, ...args) => fn(...args));
  },
  off: (channel, fn) => {
    electron.ipcRenderer.removeListener(channel, fn);
  }
});
