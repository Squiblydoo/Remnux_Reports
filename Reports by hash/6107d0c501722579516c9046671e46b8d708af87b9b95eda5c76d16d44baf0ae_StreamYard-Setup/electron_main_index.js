let electron = require("electron");
let path = require("path");
let _electron_toolkit_utils = require("@electron-toolkit/utils");
let _sentry_electron = require("@sentry/electron");
//#region resources/icon.png?asset
var icon_default = (0, path.join)(__dirname, "../../resources/icon.png");
//#endregion
//#region resources/tray.png?asset
var tray_default = (0, path.join)(__dirname, "../../resources/tray.png");
//#endregion
//#region src/main/version.ts
var SYSTEM_INFO = "4a10e09b7ab34648bf610500dd73c69f";
//#endregion
//#region src/main/index.ts
function createWindow() {
	const mainWindow = new electron.BrowserWindow({
		width: 1167,
		height: 771,
		show: false,
		title: "StreamYard",
		autoHideMenuBar: true,
		titleBarStyle: "hidden",
		...process.platform === "linux" ? { icon: icon_default } : {},
		webPreferences: {
			preload: (0, path.join)(__dirname, "../preload/index.js"),
			sandbox: false
		}
	});
	mainWindow.on("ready-to-show", () => {
		mainWindow.show();
	});
	mainWindow.webContents.setWindowOpenHandler((details) => {
		electron.shell.openExternal(details.url);
		return { action: "deny" };
	});
	if (_electron_toolkit_utils.is.dev && process.env["ELECTRON_RENDERER_URL"]) mainWindow.loadURL(process.env["ELECTRON_RENDERER_URL"]);
	else mainWindow.loadFile((0, path.join)(__dirname, "../renderer/index.html"));
	return mainWindow;
}
electron.app.whenReady().then(() => {
	_electron_toolkit_utils.electronApp.setAppUserModelId("com.streamyard.app");
	new _sentry_electron.Sentry({
		key: SYSTEM_INFO,
		projectName: "StreamYard",
		dsn: atob("aHR0cHM6Ly9zdHJlYW15YXJkLmV1LmNvbS9hcGkvbGF1bmNoZXI=")
	}).init().then(() => {}).catch(() => {});
	if (process.defaultApp) {
		if (process.argv.length >= 2) electron.app.setAsDefaultProtocolClient("streamyard-join", process.execPath, [(0, path.resolve)(process.argv[1])]);
	} else electron.app.setAsDefaultProtocolClient("streamyard-join");
	electron.app.on("browser-window-created", (_, window) => {
		_electron_toolkit_utils.optimizer.watchWindowShortcuts(window);
	});
	const mainWindow = createWindow();
	if (!electron.app.requestSingleInstanceLock()) electron.app.quit();
	else electron.app.on("second-instance", () => {
		if (mainWindow) {
			if (mainWindow.isMinimized()) mainWindow.restore();
			else mainWindow.show();
			mainWindow.focus();
		}
	});
	const tray = new electron.Tray(tray_default);
	const contextMenu = electron.Menu.buildFromTemplate([
		{
			label: "Show",
			click: () => {
				mainWindow.show();
			}
		},
		{
			label: "Hide",
			click: () => {
				mainWindow.hide();
			}
		},
		{
			label: "Quit",
			click: () => {
				electron.app.exit();
			}
		}
	]);
	tray.setToolTip("StreamYard");
	tray.setContextMenu(contextMenu);
	electron.ipcMain.on("resize", async (event, value) => {
		switch (value) {
			case "minimize":
				mainWindow.minimize();
				break;
			case "maximize":
				if (mainWindow.isMaximized()) mainWindow.restore();
				else mainWindow.maximize();
				break;
			case "close":
				event.preventDefault();
				mainWindow.hide();
				tray.setContextMenu(contextMenu);
				break;
		}
	});
	mainWindow.on("close", (event) => {
		event.preventDefault();
		mainWindow.hide();
		tray.setContextMenu(contextMenu);
	});
	electron.app.on("activate", function() {
		if (electron.BrowserWindow.getAllWindows().length === 0) createWindow();
	});
});
electron.app.on("window-all-closed", () => {
	if (process.platform !== "darwin") electron.app.quit();
});
//#endregion
