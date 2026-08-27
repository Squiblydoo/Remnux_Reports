# Analysis Report: UAPSignedBinary_neopass-2.0.9.msix

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | UAPSignedBinary_neopass-2.0.9.msix |
| SHA256 | `f6e7d3a1ca42b79f1cf4cc932d6906574b77ef6f0adf04ed4697661837e734db` |
| SHA1 | `96d2df24537a9e1f346549ea04e08a7922c26ac7` |
| MD5 | `c8452970c4fc62a1c0e342d03ef5754c` |
| File type | ZIP archive (MSIX/APPX package) |
| Size | 131,052,869 bytes (~125 MB) |
| Package identity | `NeoPass.NeoPass-NeoBrowserSolver`, x64, version 2.0.9.0 |
| Display name | "NeoPass - NeoBrowser Solver" |
| Publisher (leaf cert CN) | `DE668580-C247-42CA-82E7-63B4E47F5AF6` |

### Signing chain (AppxSignature.p7x)
The package is signed through Microsoft's **Store/Partner Center submission pipeline**, not a self-signed or third-party EV cert:

```
Leaf:   CN=DE668580-C247-42CA-82E7-63B4E47F5AF6
        issued by: Microsoft Marketplace CA G 024
        serial: 33:00:55:2c:18:45:b5:5b:47:c1:7c:d4:8c:00:01:00:55:2c:18
        validity: 2026-08-14 → 2026-08-17 (3-day, single-submission cert)
Intermediate: Microsoft MarketPlace PCA 2011 (issued by Microsoft Root CA 2011)
```
This 3-day-validity leaf certificate pattern is characteristic of the automated signing service Microsoft applies to every package ingested through the Store submission/Partner Center pipeline — it confirms the package was processed by that pipeline, not that the app is necessarily still listed/available in the Store today.

### Build artifact
PDB path recovered from `process_hider.node`:
```
C:\Users\Praveen\Documents\GitHub\neopass-neobrowser-solver\native\build\Release\process_hider.pdb
```
This reveals the project's GitHub repo name (`neopass-neobrowser-solver`) and developer username (`Praveen`).

### Package contents
Standard Electron/Chromium runtime (`d3dcompiler_47.dll`, `dxcompiler.dll`, `ffmpeg.dll`, `icudtl.dat`, `libEGL.dll`, `libGLESv2.dll`, `v8_context_snapshot.bin`, etc.) plus:
- `app/neopass.exe` — 210 MB unpacked, stock renamed Electron.exe (confirmed via `Electron`/`Chromium` version strings; not custom-compiled, so not deep-analyzed further — no custom logic resides here)
- `app/resources/app.asar` — the actual application (Node.js/JS), 1.9 MB unpacked
- `app/resources/native/hook_dll.dll` — custom C++ DLL, injected into a target process
- `app/resources/native/process_hider.node` — custom Node native addon (N-API), hides the app's own process
- `app.asar.unpacked/node_modules/@lofcz/thirdeye` — third-party OCR/vision npm package (`thirdeye.dll`)
- `app.asar.unpacked/node_modules/screenshot-desktop` — legitimate npm screen-capture module
- `app.asar.unpacked/node_modules/koffi` — legitimate FFI bridge npm module (no build tools required)

## 2. Classification

**Category: Academic-integrity-evasion / exam-cheating utility (PUA/grayware), not a traditional stealer/backdoor.**
**Confidence: High** (based on direct static/behavioral evidence within the app's own code — not a signature match).

KesaKode online lookup (authoritative per policy) returned **no matches** for either custom native component (`hook_dll.dll`, `process_hider.node`) — score effectively 0%, i.e. below the 20% "discard" threshold. This is expected: the tooling is custom/bespoke to this product, not built from a known malware-family codebase. No family attribution is claimed.

**Reasoning**, drawn directly from the decompiled/deobfuscated Electron main-process bundle (`out/main/index.js`) and native module source (`native/README.md`, `nt_structs.h`) recovered from the app.asar:

- The app markets itself as "an on-screen command overlay for NeoBrowser workflows" but its actual code:
  1. Kills a companion process named **"Neo Browser.exe"** via `taskkill /f /im "Neo Browser.exe"`.
  2. Injects `hook_dll.dll` into that Neo Browser process via `injectIntoNeoBrowser()`, logging `"[ProcessHider] Injected hook DLL into Neo Browser"`.
  3. Loads `process_hider.node`, whose own README states verbatim: *"This native Node.js addon hides the Electron process from Task Manager and other process monitoring tools on Windows."* It re-arms this hiding on a 10-second interval (`setInterval(gg, 1e4)`).
  4. Uses `SYSTEM_PROCESS_INFORMATION`/`NtQuerySystemInformation` structures (confirmed both statically in `nt_structs.h` and dynamically — emulation resolved `NtQuerySystemInformation`, `Process32FirstW`, `Process32NextW` in `hook_dll.dll`), the standard technique for unlinking a process from Task-Manager/API-based process enumeration.
  5. Captures screenshots (`screenshot-desktop` + `@lofcz/thirdeye` OCR/vision) and uploads them to a cloud backend (`https://api.neopass.tech`) with a Bearer-token-authenticated Axios client (auto-refreshing JWT via `/api/refresh-token`).
  6. The backend returns a JSON payload containing a `questions` array and a `code` field, which the app renders in a frameless always-on-top **overlay window** (`loadFile(...,{hash:"overlay"})`) — i.e., it auto-answers on-screen questions from "Neo Browser" and displays the answer/solution as an overlay.

This is a textbook design for a paid exam/quiz-cheating assistant that specifically targets a secure/lockdown testing browser ("Neo Browser") — hiding itself from that browser's own anti-cheat/process-monitoring checks while feeding it AI-generated answers.

## 3. Capabilities

- Self-concealment: hides its own process from Task Manager / `NtQuerySystemInformation`-based enumeration via a MinHook-based API hook (re-applied every 10s)
- Process injection: DLL injection + thread-execution-hijacking (capa: `T1055.001`, `T1055.003`, `T1620` — Reflective Code Loading) of `hook_dll.dll` into `Neo Browser.exe`
- Kills the target "Neo Browser.exe" process before re-launching it under hook
- Screen capture (via `screenshot-desktop`) and OCR/vision analysis (via `@lofcz/thirdeye`, using `koffi` FFI)
- Cloud API exfiltration of captured screenshots to `api.neopass.tech`, with JWT bearer-token auth and refresh-token flow
- Renders an on-screen overlay window with AI-derived answers/"solution code" returned from the backend
- Anti-debug: `hook_dll.dll` contains software-breakpoint detection checks (capa MBC: `B0001.025`)
- Runs with `runFullTrust` capability (declared in AppxManifest.xml) — full desktop access despite MSIX packaging

## 4. Attack Chain

1. User installs the MSIX (sideloaded or via Store-adjacent distribution) — `runFullTrust` capability grants full desktop access.
2. `app\neopass.exe` (Electron) launches; main process loads `process_hider.node` and immediately hides its own PID from Task Manager / process-enumeration APIs, re-arming every 10s.
3. Main process force-kills any running `Neo Browser.exe`, then injects `hook_dll.dll` into it once relaunched (`injectIntoNeoBrowser`).
4. On user command (or automatically), the app screenshots the desktop, runs OCR/vision (`thirdeye`) on it, and POSTs the result to `api.neopass.tech` with a Bearer token.
5. Backend returns `{questions:[...], code:...}`; app opens a frameless overlay window rendering the solved answer/code on top of the exam/quiz UI.

## 5. IOCs

**Network**
- `api[.]neopass[.]tech` — backend API (screenshot submission, answer retrieval, token refresh)
- `neopass[.]tech` — product/marketing domain
- `hxxps[://]api[.]neopass[.]tech/api/refresh-token` — JWT refresh endpoint

**Filesystem**
- `app\neopass.exe` (Electron host, package entry point)
- `app\resources\native\hook_dll.dll`
- `app\resources\native\process_hider.node`
- `app\resources\app.asar`
- Target process name: `Neo Browser.exe`
- Build path (PDB): `C:\Users\Praveen\Documents\GitHub\neopass-neobrowser-solver\native\build\Release\process_hider.pdb`

**Identity / signing**
- Package identity: `NeoPass.NeoPass-NeoBrowserSolver`
- Publisher: `CN=DE668580-C247-42CA-82E7-63B4E47F5AF6`
- Leaf cert serial: `33:00:55:2c:18:45:b5:5b:47:c1:7c:d4:8c:00:01:00:55:2c:18` (Microsoft Marketplace CA G 024)
- AUMID: `com.neopass.app`

**Hashes**
- MSIX package: `f6e7d3a1ca42b79f1cf4cc932d6906574b77ef6f0adf04ed4697661837e734db`
- `hook_dll.dll`: `584ea6a12aa96fa1f02cd2a44869375360867016fd9e34ab6422bd18c52c0f58`
- `process_hider.node`: `27592029cbf4445b971e2dfa32e67b1e7678b966b6342c8b0fd78965a3471c8b`

**Mutexes / registry**: none recovered (not observed in static or emulated behavior).

## 6. Emulation Results

Static analysis (capa) on the two custom native components:

| Module | ATT&CK techniques | Notable capabilities |
|---|---|---|
| `hook_dll.dll` | Discovery (T1083, T1057, T1082), Shared Modules (T1129) | anti-debug (software breakpoint checks), RWX memory allocation, thread suspend/resume/terminate, extensive runtime API linking (11 matches) |
| `process_hider.node` | Process Injection: DLL Injection (T1055.001), Thread Execution Hijacking (T1055.003), Reflective Code Loading (T1620); Discovery (T1083, T1057, T1518, T1082) | inject DLL, inject thread, enumerate/terminate processes, contains PDB path |

Speakeasy emulation (generic runner, x64 DLL mode, 60s timeout) on both modules ran cleanly to the point of dynamic API resolution:
- `hook_dll.dll` resolved `NtQuerySystemInformation`, `Process32FirstW`, `Process32NextW` via `GetProcAddress` — confirms it targets the exact process-enumeration surface (`SystemProcessInformation` class, per the recovered `nt_structs.h`) needed to unlink itself from process listings.
- `process_hider.node` resolved `napi_create_function`, confirming it's a standard Node-API addon (exports `injectIntoNeoBrowser` per static strings).

Both emulations terminated early (before reaching the actual hook/injection logic) — this is expected for early-stage generic emulation of NAPI addons, since they require a live V8/Node runtime to invoke their exported functions; the confirming evidence for the hook/hide behavior came from static decompilation of the caller (`out/main/index.js`) and the module's own README, not from deep emulation. `neopass.exe` itself (stock Electron, 210 MB unpacked) was not emulated — Electron/Chromium binaries are not meaningfully emulatable and contain no custom logic (verified via version strings).

## 7. Sandbox Results

**ANY.RUN: Skipped by analyst decision.** The Ally-tier submission would be public, and this sample identifies a live, currently-operating commercial product (`neopass.tech`) and a real developer's build path/username (`Praveen`) — not confirmed traditional malware. Per workspace policy on ambiguous/identifying samples, the user was asked before any public submission and chose to skip it. All findings above are derived from local static analysis, asar/JS decompilation, and offline emulation only.

## 8. Analyst Notes

- This is **not** attributed to any known malware family; it is a purpose-built commercial tool ("NeoPass") whose stated purpose (on-screen command overlay) is a euphemism for its actual function: defeating the process-integrity checks of a specific proctoring/lockdown browser ("Neo Browser") and auto-answering its on-screen content via a cloud AI backend.
- The Microsoft Marketplace-chain signature indicates this package passed through Microsoft's Store submission signing pipeline at some point; this does not necessarily mean it is currently live/listed in the Store, and does not constitute an endorsement of the app's behavior — Store submission signing is largely automated ingestion, not a security guarantee.
- "Neo Browser" itself was not present in this package and was not analyzed; it is inferred to be a separate secure/lockdown testing browser that this tool specifically targets by process name.
- Residual gaps: the exact content/schema sent to `api.neopass.tech` (screenshot payload format, license/subscription gating) was not captured dynamically since network emulation/sandboxing was not performed for this sample. The `@lofcz/thirdeye` and `koffi` components are legitimate open-source npm packages used as building blocks, not independently malicious.
- Recommended follow-up: if this tool is used in an environment where "Neo Browser" is deployed for supervised testing/exams, treat any host with `process_hider.node` loaded or a hidden process matching `neopass.exe`'s image base as a confirmed integrity violation. Network egress to `api.neopass.tech` from an exam-proctored host is a strong detection signal.
