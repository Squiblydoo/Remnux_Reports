# SDK_Driver.exe — Malware Analysis Report

## 1. File Metadata

| Field | Value |
|---|---|
| Filename (as received) | `SDK_Driver.exe` |
| SHA256 | `9c0a88ea53c4e0324157542385a1d342101feb51cf7b8cf76e9441376f1f522a` |
| SHA1 | `36852534338ae1d12fee8567c96636bbe1fe6d38` |
| MD5 | `c31217109ba50059d7c081a7e832d0cf` |
| Imphash | `88016fcdef7f227c62171d0afad9aae4` |
| File type | PE32 executable (GUI), Intel 80386, 11 sections |
| Size | 82,406,088 bytes (82.4 MB) |
| PE timestamp | 2026-05-15 08:20:12 |
| Compiler / builder | Delphi (Inno Setup `SetupLdr` stub), linked with TurboLinker |
| Packer/installer | Inno Setup 6.7.0 (`InnoInstaller` YARA match, reliability 90) |
| VersionInfo::ProductName | `Franz` |
| VersionInfo::FileDescription | `Franz Setup` |
| VersionInfo::CompanyName | `Stefan Malzner` (real upstream author of the legitimate Franz messenger) |
| VersionInfo::Comments | "This installation was built with Inno Setup." |
| Delphi::ProjectName | `SetupLdr` |
| Exports::Module name | `SetupLdr.e32` |

### Code-signing certificate

| Field | Value |
|---|---|
| Subject | ELH Palkehituse OÜ (Valgjärve, Põlvamaa, EE) |
| Issuer | Microsoft ID Verified CS EOC CA 04 |
| Serial | `330001385332ba26bc619362ab000000013853` |
| Validity | **2026-05-20 → 2026-05-23 (3 days)** |
| Hash/Crypt algo | SHA256 / RSA |

A 3-day "Microsoft ID Verified" throwaway certificate issued to a small Estonian entity, applied to a binary whose VersionInfo impersonates the legitimate Franz Electron app and its real developer — this mismatch (legitimate branding + disposable cert + 82MB overlay) is itself a strong indicator of trojanization, independent of any code analysis below.

## 2. Classification

**Confirmed: CryptoVista actor toolkit — trojanized Franz Electron messenger installer with RCE-capable backdoor + Telegram-phishing beacon.**

Confidence: **Confirmed** (not merely code-overlap), based on the following cross-reference criteria being met against prior analyses in memory:

- **Identical build artifact**: PDB path `D:\Coding\Is\issrc-build\Components\ChaCha20.pas` (UTF-16, score 185, 4 occurrences) is byte-for-byte identical to the build artifact documented in `UtilifySetup.exe`, `UltraPlusSetup.msi`, `MegaToolSetup.exe`, `SmartUtilSetup.exe`, and `ms_x64_update.exe` — all previously attributed to the "CryptoVista" actor.
- **Matching C2 / network IOC**: the trojanized `index.js` beacons to `web-telegram.ug/api/` — the **exact same domain** documented as the phishing/C2 domain for `UtilifySetup.exe` (2026-05-21). ANY.RUN dynamic execution of this sample independently confirmed a live DNS request to `web-telegram.ug`.
- Behavioral pattern (victimUUID/campaignKey beacon body, `eval(task.e)` RCE, `task.files` base64 file-drop+exec, `setLoginItemSettings({openAtLogin:true})` persistence, JS anti-debug/console-suppression framework with self-integrity checksum) is structurally identical to the MegaToolSetup/SmartUtilSetup/UltraPlusSetup CryptoVista samples, differing only in lure (Franz instead of Boostnote/Grape/draw.io) and campaign key.

KesaKode: offline verdict empty; online lookup also returned an empty verdict (no family match) — expected, since KesaKode's binary code-similarity engine only inspects the native `SetupLdr` stub (generic Inno Setup loader code), not the JavaScript payload where all malicious logic resides. This is a non-diagnostic result, not evidence against attribution.

## 3. Capabilities

- **Silent self-install**: the Inno Setup `InitializeSetup()` Pascal script (decompiled from `embedded/script.ps`) extracts the bundled payload and copies it directly to `%LOCALAPPDATA%\Programs\Franz`, then execs `Franz.exe` — all before any wizard UI is shown, and returns success unconditionally. No user interaction, consent screen, or install-location choice is presented.
- **Legitimate lure app**: ships a fully functional, unmodified Franz v5.10.0 Electron client (real `package.json`, `app.js`, UI code, node_modules) — the app "works" normally, hiding the backdoor.
- **Persistence**: `app.setLoginItemSettings({openAtLogin:true, openAsHidden:false, ...})` registers the app to auto-launch at every login.
- **Victim fingerprinting / beacon identity**:
  - Victim UUID: read from (or generated and written to) `%APPDATA%\setup.txt` (random base36 string) if not already present.
  - Campaign key: read from `<install_dir>\readme.txt`, split on `-`. Embedded file `readme.txt.txt` (renamed on disk to avoid confusion with the real Franz `readme.txt`) contains campaign key **`Maloi-FileEv`**.
- **C2 beacon**: every 180 seconds, HTTP POST (`Content-Type: application/json`) to `web-telegram.ug/api/` with body `[victimUUID, COMPUTERNAME, USERNAME, campaignKeyPart1, campaignKeyPart2]`, over plain `https.request` with `rejectUnauthorized: false` (TLS validation disabled).
- **Remote code execution**: if the beacon response contains a `task.e` field, the string is passed directly to `eval()` in the Electron main process — full arbitrary JS/Node RCE with `require()` access (filesystem, child_process, network).
- **Remote file-drop + execute**: if the response contains `task.files` (a map of relative path → base64 content), each file is written under `%TEMP%\<timestamp>\`; if any dropped filename ends in `.exe`, it is executed via `child_process.exec()`.
- **Anti-analysis (renderer/main JS)**: `Function.prototype.toString` tamper-detection, `debugger;`-timing checks, regex `toString` override detection, `console` object neutering, self-integrity via a djb2-style checksum (`NI()` must equal `2838916400`) re-verified every 3 seconds — if tampered, disables the anti-debug loop and busy-loops the event loop (effective DoS/hang as an anti-tamper response).
- **String/identifier obfuscation**: all sensitive strings (C2 host, file names, Electron API names) are stored as numeric char-code arrays and reassembled at runtime (`qs()` helper), evading static string scanning.

## 4. Attack Chain

1. Victim downloads/runs `SDK_Driver.exe`, an Inno Setup installer signed with a disposable 3-day cert, presented as/named after "SDK Driver" but internally branded as Franz.
2. `InitializeSetup()` silently extracts payload to `%LOCALAPPDATA%\Programs\Franz` and launches `Franz.exe` — no visible install wizard.
3. Trojanized Electron main process (`index.js`) sets `openAtLogin` persistence, establishes/reads victim UUID (`%APPDATA%\setup.txt`) and campaign key (`readme.txt` = `Maloi-FileEv`), and enters a 180-second beacon loop to `web-telegram.ug/api/`.
4. Operator responds to the beacon with either:
   - `{"task":{"e": "<js code>"}}` → arbitrary code execution in the Electron main process, or
   - `{"task":{"files": {...}}}` → drops and executes an arbitrary binary from the response.
5. Victim continues using a fully functional Franz messenger, unaware of the backdoor running alongside it.

## 5. IOCs

**Network**
- C2 / beacon: `web-telegram[.]ug/api/` (HTTP POST, JSON body) — confirmed both via static extraction and live ANY.RUN DNS request
- TLS validation disabled on the beacon channel (`rejectUnauthorized: false`)

**Filesystem**
- `%LOCALAPPDATA%\Programs\Franz\` — silent install target
- `%LOCALAPPDATA%\Programs\Franz\readme.txt` — campaign key file, content: `Maloi-FileEv`
- `%APPDATA%\setup.txt` — victim UUID (random base36)
- `%TEMP%\<timestamp>\` — staging directory for operator-pushed files
- `error.log` (relative to app cwd) — RCE exception log

**Registry / Persistence**
- Electron `openAtLogin` login-item registration (Run-key equivalent) for `Franz.exe`

**Certificates**
- Serial `330001385332ba26bc619362ab000000013853` — "ELH Palkehituse OÜ", 3-day Microsoft ID Verified cert (2026-05-20 → 2026-05-23)

**Campaign identifiers**
- Campaign key: `Maloi-FileEv`

## 6. Emulation Results

Speakeasy/angr emulation was **not applicable** to this sample: the outer native binary is a stock Inno Setup `SetupLdr` stub containing no malicious logic of its own (confirmed via Pascal script decompilation of `embedded/script.ps` — it only copies files and execs `Franz.exe`). All backdoor logic executes as JavaScript inside the Node/Electron runtime of the bundled `Franz.exe`, which is outside the scope of native-code emulation. Full behavioral recovery was instead achieved through direct static extraction and manual deobfuscation of `resources/app.asar!/index.js` (char-code array deobfuscation), which yielded complete, unambiguous C2/config/logic details — a more reliable result than emulation would have provided for this file type.

capa refused analysis (installer-file limitation, exit code 14, as expected for Inno Setup). peframe completed with a partial internal crash while parsing the signature block, but returned useful metadata (import table, anti-debug/mutex API usage, packer/crypto YARA hits) prior to the crash. floss/stringsifter were not run against the 82MB container (payload strings were already fully recovered from the extracted JS).

## 7. Sandbox Results (ANY.RUN)

- **Verdict**: Malicious activity (score 100/100, threat level 2)
- **Tags**: `inno`, `installer`, `delphi`, `phishing`, `telegram`, `auto-reg`
- **Behavioral flags**: `known_threat`, `autostart`, `bad_module_certificate`, `process_dump`
- **DNS**: `web-telegram.ug` (confirms static C2 finding)
- **HTTP/HTTPS**: only Microsoft CRL/OCSP and `login.live.com` WAM/telemetry endpoints observed beyond the C2 DNS lookup — consistent with the beacon loop firing but the sandbox not receiving an operator-controlled `task` response (no `eval`/file-drop activity captured in this run window)
- **Public report**: https://app.any.run/tasks/d8c244f5-80f4-4151-a59a-f35258ca7d44

## 8. Analyst Notes

- The bundled Franz Electron application itself is unmodified/legitimate apart from the trojanized `index.js`; `app.js` (renderer bootstrap) was verified clean.
- No operator response to the beacon was observed in this sandbox run, so the concrete payload delivered via `task.e`/`task.files` in an active campaign is unknown — only the delivery mechanism was recovered.
- `login.live.com` (WAM/RST2.srf) contacts observed in the sandbox are standard Windows telemetry/credential-provider noise triggered by the OS itself, not sample-specific behavior — not attributed to this sample's C2.
- Recommended follow-up: block/sinkhole `web-telegram.ug`; hunt for `%APPDATA%\setup.txt` + `Programs\Franz\readme.txt` containing non-standard campaign-key strings as a detection signature; consider YARA/behavioral rule on the `qs()` char-code-array deobfuscation pattern + the djb2 self-integrity checksum constant `2838916400`, which appears reusable across CryptoVista builds.
