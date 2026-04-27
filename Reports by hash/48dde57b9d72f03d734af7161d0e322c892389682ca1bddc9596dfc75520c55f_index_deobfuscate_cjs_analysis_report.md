# Analysis Report: index_deobfuscate.cjs

## 1. File Metadata

| Field        | Value |
|--------------|-------|
| **Filename** | index_deobfuscate.cjs |
| **SHA256**   | `48dde57b9d72f03d734af7161d0e322c892389682ca1bddc9596dfc75520c55f` |
| **SHA1**     | `e57879c3a8f38046e0b06709d3764d0c7d76d59b` |
| **MD5**      | `4e7584648f28122ecd1fece7a9cb0832` |
| **File Type**| JavaScript source, UTF-8 (CommonJS module) |
| **Size**     | 108,447 bytes (108 KB) |
| **Signing**  | None (plain text source) |
| **Build artifact** | Electron main process (`packages/main/...`); preload at `packages/preload/dist/index.cjs` |

**Deobfuscation status:** Partially deobfuscated — the filename itself signals this pass. Most function-level identifiers have been renamed to readable names; however, hundreds of `_0x*`-prefixed variable references remain in-place (unreachable dead code and scattered locals), indicating an automated tool (e.g., `js-deobfuscator` or `synchrony`) was run before submission. Some of these dead-code fragments (`_0x3e6993 = true; _0x18567b = _0x1dc45c;`) are unreachable after early-return statements — artifacts of the original obfuscator's control-flow flattening that were not fully cleaned.

---

## 2. Classification

- **Family:** ModsHub / FiveMods Electron Cheat Client (PUA/Grayware + credential-adjacent telemetry)
- **Confidence:** **HIGH**
- **Reasoning:** Exact matching C2 endpoints (`https://fivemods[.]app/api`, `https://s0.fivemods[.]app/app`) and Discord OAuth port 4311 confirm this is the Electron main-process source code from the same ModsHub campaign previously distributed by `ModsHubSetup.exe` (SHA256: `0c934d4d...`). The file is the deobfuscated `index.cjs` bundle that ships inside the NSIS installer's Electron `app.asar`.

---

## 3. Capabilities

### Core Application
- **Two-window Electron UI**: a 1280×720 main window (`renderer/dist/index.html`) and a 320×330 loader splash (`renderer/dist/load.html`); both hide devTools (`devTools: false`), block new windows, and disable node integration in the renderer.
- **FiveM DLC injection**: Downloads `.dat` payload files and extracts them into `<gamePath>\update\x64\dlcpacks\patchday10ng\temp\`, modifying GTA V / FiveM's RPF archive layer.
- **Mods Engine execution**: Downloads `Mods Engine.exe` from a server-configured CDN (`serverConfig["mods_engine.server"]`) and executes it via `child_process.execFile`; version-checks against `serverConfig["mods_engine.module_version"]`.
- **Optimization downloads**: Downloads optimization packs from `serverConfig["optimization.server"]`.
- **Subscription gating**: All DLC injection is gated on `subscription_days.fivemods_plus > 0` OR `subscription_days.fivemods_cars > 0` from the server.

### Telemetry / Hardware Fingerprinting
- **`Oa()` — hardware fingerprint collection**: Runs `arp -a` to enumerate local ARP table (IP + MAC pairs). Collects: OS username, hostname, OS version, total RAM (rounded to 2 GB), CPU manufacturer+brand, motherboard model, system UUID, GPU model(s) via `systeminformation`. Derives a deterministic victim UUID from hardware attributes via `uuid-by-string`. Posts encoded payload to `POST /user/info`.
- **Log obfuscation (`F0()`)**: All URLs and endpoint paths written to `electron-log` are first encoded: string-reverse → UTF-8 byte array → reverse → base64 → reverse. Makes forensic log analysis significantly harder.

### Authentication
- **Discord OAuth (port 4311)**: On "discord-log-in" IPC event, opens a local HTTP server on port 4311, then launches the browser to `<api>/authorize`. Intercepts the redirect with `?token=<token>` query parameter, stores token in registry at `HKCU\SOFTWARE\ModsHub\token`, then verifies at `/authorize/check`.
- **Token written to disk**: Auth token is written verbatim to `<gamePath>\update\x64\dlcpacks\patchday10ng\temp\code.xml` during mod-client activation — a plaintext credential artifact on disk.

### Persistence
- **Login-at-startup**: `g.app.setLoginItemSettings({openAtLogin: true, openAsHidden: true, name: "ModsHub", args: ["--autostart"]})` — registers as a hidden login item.
- **Daily scheduled task (18:00)**: Uses PowerShell `Register-ScheduledTask` (with schtasks fallback) to create task `"SyncData"` running `wscript "<sync_script_path>" "<gamePath>"` daily at a time derived from subscription expiry; falls back to exactly 18:00:00.

### Privilege Escalation
- **`Qx()` — admin restart**: Runs `powershell "Start-Process '<execPath>' -Verb RunAs"` to restart itself with administrator privileges.
- **Admin check**: Runs `powershell -Command "([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)"`.

### Anti-forensics / Evasion
- **`utimes` timestomping (`wx.utimes`)**: Recursively walks the game DLC directory and sets `btime`, `atime`, `mtime` to `Date.now()` on every file and directory after modification.
- **`--task` silent mode**: When launched with `--task`, executes the mod-disable flow silently and exits without showing a window.
- **Kills Mods Engine on launch** (`ka()`): Force-kills any running `Mods Engine.exe` process (via `fkill`, with `force: true` + `tree: true`) before re-downloading/running a fresh copy.

### Registry Operations
- **Reads**: `HKCU\SOFTWARE\FiveMods` (legacy migration source), `HKCU\SOFTWARE\ModsHub` (primary config), `HKLM\SOFTWARE\WOW6432Node\Rockstar Games\Grand Theft Auto V`, `HKLM\SOFTWARE\WOW6432Node\Rockstar Games\GTAV`, `HKCU\SOFTWARE\CitizenFX\FiveM`, `HKCU\SOFTWARE\RAGE-MP`
- **Writes**: `HKCU\SOFTWARE\ModsHub\*` (token, gamePath, fivePath, installed, downloadList, optimization, language, scale, etc.); migrates and then deletes `HKCU\SOFTWARE\FiveMods` via `reg delete`.

### Custom Protocol / Networking
- Registers `fivemods://` as default protocol client.
- HTTP header fingerprint on all API calls: `FM-ORIGIN: app`, `FM-TOKEN: <token>`, `FM-UUID: <hardware_uuid>`, `FM-BETA`, `FM-BENEFITS`, `FM-APP-VERSION`, `FM-GAME-GENERATION`.
- Custom response header check: `fm-status: error` → abort download + show error modal; `fm-status: banned` → abort silently.

---

## 4. Attack Chain

```
1. ModsHubSetup.exe (NSIS installer)
   └─> Extracts Electron app including index.cjs (obfuscated)

2. App startup
   ├─ lx(): fetch /config → populate serverConfig (CDN hosts, version hashes)
   ├─ ix(): GET /authorize/check → validate stored token
   ├─ sx(): GET /user/profile → load subscription_days, beta, benefits
   └─ Oa(): ARP + sysinfo harvest → POST /user/info (F0-encoded)

3. User logs in
   ├─ Ra(): Listen on :4311 → open browser to <api>/authorize
   └─ Token captured, stored in HKCU\SOFTWARE\ModsHub\token
      └─ code.xml written to GTA V patchday10ng\temp\

4. Subscription active
   ├─ Download DLC .dat files from line server (token-authenticated)
   ├─ Extract to <gamePath>\dlcpacks\patchday10ng\temp\ (timestamp-stomped)
   ├─ Wx(): Register daily schtask "SyncData" → wscript sync_script.vbs
   └─ Kx(): setLoginItemSettings openAtLogin=true

5. Task / background mode (--task / --autostart)
   ├─ Y0(): Check if GTA5.exe / FiveM.exe running
   ├─ Oe(): Extract DLC on game close
   └─ Z("/user/client?mode=disable&method=task") → server marks client offline
```

---

## 5. IOCs

### Network (defanged)

| Type | Value | Purpose |
|------|-------|---------|
| Domain | `fivemods[.]app` | Primary API host / update host |
| Domain | `fivemods[.]fun` | Extra host (failover) |
| Domain | `fivemods[.]io` | Backup host / homepage |
| URL | `https://fivemods[.]app/api` | API base (hardcoded default) |
| URL | `https://s0[.]fivemods[.]app/app` | Electron auto-updater feed |
| URL | `https://dev[.]s0[.]fivemods[.]io/app` | Dev update feed |
| URL | `https://fivemods[.]io/app-reserve` | Backup host discovery endpoint |
| URL | `https://fivemods[.]io/` | Homepage (OAuth redirect landing) |
| URL | `https://cdn[.]discordapp[.]com` | Discord CDN (profile avatars) |
| URL | `https://discord[.]com/` | Discord sign-up redirect |
| Port | `4311` (localhost) | Discord OAuth callback |
| API endpoint | `GET /config` | Server configuration fetch |
| API endpoint | `GET /authorize/check` | Token validation |
| API endpoint | `GET /user/profile` | Subscription data |
| API endpoint | `POST /user/info` | Hardware fingerprint exfiltration |
| API endpoint | `GET /user/client?mode=enable/disable` | Client status beacon |
| API endpoint | `GET /line`, `/line/update`, `/line/delete`, `/line/active` | Download queue management |
| Dynamic | `https://<serverConfig["mods_engine.server"]>/?type=engine` | Mods Engine download |
| Dynamic | `https://<serverConfig["mods_engine.server"]>/?type=engine_module` | Mods Engine module |
| Dynamic | `https://<serverConfig["optimization.server"]>/?type=optimization` | Optimization pack download |

### Filesystem

| Path | Purpose |
|------|---------|
| `%APPDATA%\CitizenFX\gta5_settings.xml` | FiveM settings path (read) |
| `%LOCALAPPDATA%\FiveM\` | FiveM default install path |
| `<gamePath>\update\x64\dlcpacks\patchday10ng\temp\` | DLC injection staging directory |
| `<gamePath>\update\x64\dlcpacks\patchday10ng\process\` | DLC processing directory |
| `<gamePath>\update\x64\dlcpacks\patchday10ng\temp\code.xml` | Auth token persistence on disk |
| `<gamePath>\update\x64\dlcpacks\patchday10ng\v.engine` | Mods Engine version tracking |
| `<userData>\mods_engine\Mods Engine.exe` | Downloaded and executed secondary payload |
| `<userData>\temp.dat` | Temporary download staging file |

### Registry

| Key | Value | Purpose |
|-----|-------|---------|
| `HKCU\SOFTWARE\ModsHub` | All values | Primary config store |
| `HKCU\SOFTWARE\ModsHub\token` | Discord OAuth token | Authentication |
| `HKCU\SOFTWARE\ModsHub\gamePath` | GTA V install path | Game path |
| `HKCU\SOFTWARE\ModsHub\installed` | JSON object | Installed mod tracking |
| `HKCU\SOFTWARE\ModsHub\downloadList` | JSON array | Pending downloads |
| `HKCU\SOFTWARE\FiveMods` | (migrated/deleted) | Legacy key, removed after migration |

### Scheduled Tasks

| Name | Trigger | Action |
|------|---------|--------|
| `SyncData` | Daily at subscription-expiry time (default 18:00) | `wscript "<sync_script>" "<gamePath>"` |
| `"Updater Task FM"` (task name from `taskSchedulerDeactivatorTaskName`) | Daily at 18:00 | `<app_execPath> --task` |

### Login Items

| Name | Args | Behavior |
|------|------|----------|
| `ModsHub` | `["--autostart"]` | Opens at login, hidden |

---

## 6. Emulation Results

**Not applicable** — this is a JavaScript source file (CommonJS module) requiring the Electron runtime. PE emulation (speakeasy, Qiling) does not apply. No dynamic execution was attempted.

The F0() log-encoding function was manually reversed: it takes an input string, reverses it, converts each UTF-8 byte to a decimal string separated by spaces, reverses that, base64-encodes the result, then reverses the base64. This produces a stable encoding that hides endpoint paths from casual log review.

---

## 7. Sandbox Results

**Skipped** — ANY.RUN requires a PE executable; this is a JS source file that requires the full Electron runtime to execute. The deobfuscated source provides equivalent static visibility.

---

## 8. Cross-Reference

This file shares the following exact network IOCs with the previously analyzed **ModsHubSetup.exe** (`0c934d4d...`):
- C2: `https://fivemods[.]app/api` (identical)
- Update feed: `https://s0[.]fivemods[.]app/app` (identical)
- OAuth port: `4311` (identical)

These confirm this is the main-process source code from that sample's Electron bundle, made available here in a partially deobfuscated form.

---

## 9. Analyst Notes

1. **Deobfuscation completeness**: The automated deobfuscation pass renamed most meaningful identifiers but left `_0x*` residues as dead code and in error-message string literals. No remaining obfuscation blocks meaningful static analysis.

2. **Token-on-disk concern**: Writing the Discord OAuth token to `patchday10ng\temp\code.xml` in the game directory is an unusual persistence pattern. Any process with read access to that directory could harvest the session token.

3. **`serverConfig` as a configuration C2**: The server returns a JSON config object from `/config` that controls which CDN servers are used for all subsequent downloads. This makes the download infrastructure fully operator-controlled and rotatable without updating the client binary.

4. **Timestomping scope**: The `utimes` call walks all files recursively in the DLC staging directories — this is an active anti-forensics measure to reset MAC times after injection.

5. **Mods Engine.exe**: This secondary PE (downloaded at runtime from `serverConfig["mods_engine.server"]`) is not present in the static sample and represents an unanalyzed code execution surface. It is launched without arguments and the app exits on its `exit` event.

6. **Partial code fragments**: Lines 882–884 (setMinimumSize/setSize/center with `_0x*` identifiers) and lines 264–265 (`_0x3e6993 = true; _0x18567b = _0x1dc45c;`) are unreachable dead code from the original obfuscator's flattening logic; they have no runtime effect in the deobfuscated file.

7. **YARA `CloudFileHosting`**: Triggered by `https://s0.fivemods.app/app` — the Electron auto-update feed URL, which follows a file-hosting CDN pattern (`s0.` subdomain serving application binaries).
