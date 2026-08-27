let electron = require("electron");
let _electron_toolkit_preload = require("@electron-toolkit/preload");
//#region src/preload/index.ts
var api = { resize: (how) => electron.ipcRenderer.send("resize", how) };
if (process.contextIsolated) try {
	electron.contextBridge.exposeInMainWorld("electron", _electron_toolkit_preload.electronAPI);
	electron.contextBridge.exposeInMainWorld("api", api);
} catch (error) {
	console.error(error);
}
else {
	window.electron = _electron_toolkit_preload.electronAPI;
	window.api = api;
}
//#endregion
