# Malware Analysis Report: SteamBoosterSetup.exe

**Analyst:** REMnux / Claude  
**Date:** 2026-06-28  
**Sample:** SteamBoosterSetup.exe

---

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | SteamBoosterSetup.exe |
| SHA256 | `09c4f8262c74179f6f9bf776866f8bc1873ed8ce6b5e258c03d30ea344ad7678` |
| MD5 | `cbeee5807322b86932dcd5c839d8f6bf` |
| SHA1 | `3ca16328f9efafbf8ea29529120450be1aa4f8ac` |
| Size | 4,104,640 bytes (~4.0 MB) |
| Type | PE32+ executable (GUI) x86-64, for MS Windows, 6 sections |
| Compiler | MSVC 2022 / VS 17.14.2 Pre 1.0 (x64-windows-static build) |
| Build Date | 2026-06-24 16:37:58 (PGO / ExDllCharacteristics debug record) |

### Signing

| Field | Value |
|---|---|
| Issuer | Microsoft ID Verified CS EOC CA 03 |
| Subject | **CYBERMID LIMITED** |
| Org Details | CYBERMID LIMITED / State=New York / Locality=New York / Country=US |
| Serial | `33000245024c9591befe9583f9000000024502` |
| Validity | **2026-06-23 to 2026-06-26** (3-day short-lived cert) |
| Algorithm | SHA256 / RSA |

The 3-day Microsoft ID Verified certificate issued to "CYBERMID LIMITED" (NY) is a classic sign-in abuse pattern — actors obtain short-lived certs for malware distribution windows. Certificate was issued 1 day before build timestamp.

### VersionInfo (lure metadata)

| Field | Value |
|---|---|
| Comments | SteamBooster by SteamBalance — Steam wallet top-up helper |
| CompanyName | SteamBalance |
| FileDescription | SteamBooster by SteamBalance |
| ProductName | SteamBooster by SteamBalance |
| ProductVersion | 0.0.23 |
| OriginalFilename | steambooster.exe |

### Build Artifacts

- **PDB path leak:** `D:/Projects/steambooster/booster-injector/injector/build/ReleaseProd/vcpkg_installed/x64-windows-static/include\wil/resource.h`
- **Assembly identity:** `<assemblyIdentity type="win32" name="steambooster" version="0.0.1.0"/>`
- **C++ project namespace:** `sb::` (SteamBooster); internal subdirectories: `cdp/`, `injection/`, `update/`, `ipc/`, `installer/`, `startup/`, `configs/`, `core/`

---

## 2. Classification

| Field | Value |
|---|---|
| **Family** | Novel — no known malware family (see below) |
| **Confidence** | High (detailed structural/behavioral analysis) |
| **Type** | Steam Browser CDP Injector + Remote Plugin Loader |
| **Lure** | Steam wallet top-up helper / "SteamBooster" |

**KesaKode online:** SunBurst 0.73%, SideCopy 0.36% — both below 20% threshold; discarded as noise. This is an **unattributed novel family** not matching any known malware corpus. The codebase is an entirely custom C++ application with no borrowed shellcode or known toolkit components.

The `sb::` namespace and the `booster-injector` project path suggest a purpose-built, professionally developed tool. The embedded JSON schema validation, IPC bus, plugin quota enforcement, and structured logging (spdlog) indicate deliberate software engineering, not a script-kiddie tool.

---

## 3. Capabilities

### Core Mechanism: Steam CDP Browser Injection

- **Discovers Steam's embedded Chromium debug port** by probing `http://127.0.0.1:{}/json/list` and `http://127.0.0.1:{}/json/version` via class `sb::cdp::CdpDiscovery`
- **Parses CDP target list** (JSON array), selecting:
  - Tab with `type=="page"` + URL containing `Steam` + query param `createflags=274` → **Steam main browser window**
  - Tab with `type=="page"` + URL `SharedJSContext` containing `steamloopback.host` → **Steam shared JavaScript context**
- **Establishes WebSocket connections** to both targets via `sb::cdp::Connection` (using `webSocketDebuggerUrl` extracted from CDP response)
- **Injects JavaScript framework** via CDP `Runtime.evaluate` / `Page.addScriptToEvaluateOnNewDocument`

### JavaScript Injection Framework

Injects a multi-layer framework into Steam's browser contexts:

| Component | Description |
|---|---|
| `window.sb` | Root namespace for the injection framework |
| `globalThis.__SB_EW_API__` | API bridge exposed to plugin code |
| `window.__sb_relay_teardown` | Teardown / cleanup function |
| `__sb_ew_chrome_css` | CSS `<style>` element injected into Steam's DOM |
| `__sb_native` / `__sb_natH+` | Native bridge marker strings |
| `__SB_EW_API__` | Test hook (disabled in prod: `__SB_EW_TEST__`) |

CSS class manipulation (body class state machine):
- `booster-ew-solo` — applied when exactly one Steam browser tab is open; triggers full-page UI replacement
- `booster-ew-active-tabshown` — applied when multiple tabs are open
- CSS rules from `__CSS_RULES_PLACEHOLDER__` replaced at runtime with manifest-delivered rules (used to hide/show Steam UI elements)

Lifecycle rollback script injected on detach/teardown:
```javascript
(function(){
  try { window.__sb_relay_teardown && window.__sb_relay_teardown(); } catch(_){}
  try { window.sb && window.sb.lifecycle && window.sb.lifecycle.rollbackAll && window.sb.lifecycle.rollbackAll(); } catch(_){}
})()
```

### Remote Manifest + Plugin System

Via `sb::update::ManifestLoader::DownloadAndAssemble`:

1. Downloads `https://cdn.steambalance[.]cc/booster/manifest.json` (polling interval configurable, `--manifest-poll-interval`)
2. Manifest is a JSON document with schema:
   - `approvedPlugins[]` — list of JS plugin entries (each with `id`, `apiVersion`, `contextKinds`, `grantedCapabilities`)
   - `requiredPlugins[]` — plugins that must be present
   - `manifestHints[]` — dynamic CSS selectors injected into Layer 5
3. Each plugin is downloaded as a bundle (max 4 MB per plugin), stored locally in encrypted form
4. Plugins are evaluated in their target contexts (`main`, `shared`, `tabbedBrowser`)
5. Manifest version is tracked and logged; updates trigger hot-swap of the framework

**Special plugin: `[InjectStorePage]`** — a named plugin targeting Steam Store pages (different injection flow from the main `[Injector]`), likely the primary payload delivery surface for financial fraud.

### Persistence

- **Autostart registry key:** `HKCU\Software\Microsoft\Windows\CurrentVersion\Run` — adds `SteamBooster` entry
- **Uninstall entry:** `HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\steambooster`
- **Desktop shortcut:** `SteamBooster by SteamBalance.lnk`
- **Start menu shortcut:** `SteamBooster.lnk`
- **App user model ID:** `AppUserModelId\SteamBalance.SteamBooster` (taskbar pinning)
- **Tray icon:** `NIM_ADD` on startup; tray menu triggers (un)install/restart/exit

### Self-Update

- Downloads updated binary to `steambooster.exe.download`
- Renames running binary to `steambooster.exe.old`
- Replaces `steambooster.exe` with download
- Launches new version, old version self-deletes via: `cmd.exe /S /C "timeout /T 2 /NOBREAK > nul & del /Q /F "{}""` and directory cleanup via `cmd.exe /S /C "for ... do (rd /s /q)"`

### Other Capabilities

| Capability | Evidence |
|---|---|
| TOR support | `.onion` strings, `TorUsage` YARA, `Not resolving .onion address (RFC 7686)` (libcurl built-in) |
| Process enumeration | `EnumerateProcesses` YARA; monitoring for `steamwebhelper.exe` |
| HTTP form POST | `multipart/form-data`, `Content-Type: application/x-www-form-urlencoded` |
| HTTP client | libcurl 8.x embedded statically |
| TLS | mbedTLS (PolarSSL) embedded statically |
| JSON parsing | nlohmann/json v3.12.0 embedded |
| IPC bus | `sb::ipc::IpcRouter`; `bus.publish` payloads (max 16 KB); encrypted with ChaCha20 |
| ChaCha20 | `expand 32-byte k` constant (confirmed); used for plugin bundle storage and IPC |
| Steam registry | `sb::ipc::ops::DefaultOpenSteamKey`; reads `Software\Valve\Steam` |
| Singleton check | Named event `Local\steamboost..hutdown-request`; `steambooster.singleton.` |
| Delayed batch commands | `DelayBatch` YARA; `timeout /T 2 /NOBREAK > nul` pattern |

---

## 4. Attack Chain

```
1. USER installs SteamBoosterSetup.exe
   └─ Signed with CYBERMID LIMITED 3-day cert
   └─ Lure: "Steam wallet top-up helper"

2. INSTALL (sb::installer::install_orchestrator)
   ├─ Writes HKCU\...\Run → autostart
   ├─ Creates Start Menu / Desktop shortcuts
   ├─ Registers AppUserModelId (taskbar)
   └─ Adds Uninstall entry

3. RUNTIME (sb::core::LifecycleManager — state machine)
   ├─ Singleton check (prevents duplicate instances)
   ├─ Tray icon shown
   ├─ Waits for steam.exe to launch (monitors process list)
   │
   └─ Steam detected →

4. CDP ATTACH (sb::cdp::CdpDiscovery)
   ├─ Probe ports: http://127.0.0.1:{}/json/list
   ├─ Find target: createflags=274 (main browser)
   ├─ Find target: SharedJSContext (steamloopback.host)
   └─ WebSocket connect to both via webSocketDebuggerUrl

5. MANIFEST FETCH (sb::update::ManifestLoader)
   ├─ GET https://cdn.steambalance[.]cc/booster/manifest.json
   ├─ Validate schema (approvedPlugins, requiredPlugins)
   ├─ Download plugin bundles (encrypted, ChaCha20)
   └─ Cache locally

6. INJECTION (sb::injection::Injector::AttachAndInject)
   ├─ Inject framework JS: window.sb, __SB_EW_API__, __sb_ew_chrome_css CSS
   ├─ Evaluate plugins in context (main / shared / tabbedBrowser)
   ├─ [InjectStorePage] plugin activates on Steam Store navigation
   └─ Apply CSS manifest hints (hide/show Steam UI elements)

7. EXFIL / EVENTS
   └─ POST https://steambalance[.]cc/api/booster/events

8. SELF-UPDATE (background)
   ├─ Poll manifest for new version
   ├─ Download to steambooster.exe.download
   └─ Replace running binary + self-delete old via cmd.exe
```

**Primary fraud hypothesis:** The `[InjectStorePage]` plugin targets Steam Store / Steam Wallet top-up pages. With DOM control over Steam's embedded browser, the malware can silently alter payment amounts, redirect wallet funds, replace checkout details, or harvest Steam Guard codes. The attacker controls the JS payload remotely (no static payload to detect) via the manifest CDN.

---

## 5. IOCs

### Network (defanged)

| Type | Value | Note |
|---|---|---|
| Domain | `steambalance[.]cc` | Main C2 domain |
| URL | `https://cdn.steambalance[.]cc/booster/manifest.json` | Manifest polling endpoint |
| URL | `https://steambalance[.]cc/api/booster/events` | Event exfil endpoint |
| URL | `http://127.0.0.1:{}/json/list` | Local Steam CDP discovery |
| URL | `http://127.0.0.1:{}/json/version` | Local Steam CDP version |

### Filesystem

| Path | Note |
|---|---|
| `steambooster.log` | Activity log |
| `steambooster.exe.old` | Previous version on self-update |
| `steambooster.exe.download` | Staged update download |
| `booster-write-probe-{}.tmp` | Temp file probe (writable dir check) |
| `last-good.json` | Cached last-good manifest |
| `app-icon.ico` | Tray icon |

### Registry

| Key | Note |
|---|---|
| `HKCU\Software\Microsoft\Windows\CurrentVersion\Run` | Autostart |
| `HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\steambooster` | Uninstall entry |
| `HKCU\Software\SteamBalance\SteamBooster` | Config storage |
| `HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\steambooster` | Installed app record |
| `AppUserModelId\SteamBalance.SteamBooster` | Taskbar AUMID |
| `HKCU\Software\Valve\Steam` | Steam install path lookup |

### Certificate

| Field | Value |
|---|---|
| Subject | CYBERMID LIMITED |
| Serial | `33000245024c9591befe9583f9000000024502` |
| Issuer | Microsoft ID Verified CS EOC CA 03 |
| Validity | 2026-06-23 → 2026-06-26 |

### Named Objects / Mutexes

- `Local\steambooster-shutdown-request` (named event, pattern from strings)
- `steambooster.singleton` (singleton lock)

### Build Artifact

- PDB: `D:/Projects/steambooster/booster-injector/injector/build/ReleaseProd/...`

---

## 6. Emulation Results

**Speakeasy (generic runner, amd64, 60s timeout):** 0 IOCs captured.

Expected behavior: the malware's core logic (CDP injection) is gated on Steam being present at a specific localhost port. Without Steam running in the sandbox, the main worker (`LifecycleManager`) enters the `NoSteam` state and waits. Network connections to `cdn.steambalance.cc` are not reached without Steam having launched first.

ANY.RUN sandbox observed DNS resolution to `cdn.steambalance.cc` and `steambalance.cc`, confirming the C2 domains are contacted on execution, but no connection payloads were captured (sandbox lacks a running Steam client).

---

## 7. Sandbox Results

**ANY.RUN:** 100/100 — Malicious activity  
**Tags:** `auto-reg`, `loader`  
**Public report:** https://app.any.run/tasks/ebe2434d-64cf-4801-82f8-b58108aa5aec

DNS lookups observed:
- `cdn.steambalance.cc` (reputation: 0 — newly registered)
- `steambalance.cc` (reputation: 0 — newly registered)

No HTTP/HTTPS connections captured (Steam not present). No dropped files or process injections observed (the malware injects into Steam's existing Chromium process via WebSocket, not via process injection APIs).

---

## 8. Analyst Notes

### What makes this unusual

1. **Novel Steam-specific attack surface:** Rather than injecting into `chrome.exe` or hooking browser DLLs, this malware specifically targets Steam's *built-in* Chromium debug port — a feature most Steam users don't know exists. Steam enables the CDP debug server (`--remote-debugging-port`) internally for Steam Overlay and SharedJSContext purposes.

2. **Fileless payload:** The JS plugin payloads are never written to disk as plaintext. They are downloaded encrypted (ChaCha20), stored as encrypted bundles, and only decrypted in-memory at injection time. Static AV sees only the C++ loader and an encrypted blob.

3. **Remote manifest update:** Operators can push entirely new payloads without touching the installed binary. The `manifest_version` field and `[Injector] ApplyManifest: swapping framework + N plugin(s)` log string confirm hot-swapping is supported.

4. **[InjectStorePage] plugin:** A named plugin with a dedicated injection path for Steam Store pages is the clearest signal of intent. Steam Store pages handle wallet top-ups, item purchases, and gift card redemption — high-value fraud targets.

5. **`booster-ew-solo` full-page replacement mode:** When the user has exactly one Steam browser tab open (solo mode), the malware adds `booster-ew-solo` to `document.body.classList`. The injected CSS then uses `display: none !important` to hide specific Steam Store elements (filled from manifest `manifestHints` selectors), effectively replacing the legitimate Steam UI with attacker-controlled content.

6. **CYBERMID LIMITED (NY) cert:** Consistent with the `UpdaterSetup.exe` (Pulse Browser PUP, also NY) pattern of obtaining Microsoft ID Verified certs for short-duration malware campaigns. Operators appear to have an established process for acquiring these certs quickly.

### Residual gaps

- **Actual plugin JS content:** Not recoverable without running the sample with Steam present and capturing the manifest + plugin bundle decryption. The JS payload is the critical missing piece to confirm specific fraud mechanics.
- **steambalance.cc infrastructure:** The domain was not accessible at analysis time; registrar, hosting AS, and registration date unknown. Pivoting on the cert serial may reveal linked infrastructure.
- **`[InjectStorePage]` plugin context:** Exact Steam Store URL patterns targeted are in the manifest, not the binary. Unknown without live network access.
- **TOR usage:** `.onion` support is present via embedded libcurl. Whether TOR is used for C2 fallback or for operator access to the `steambalance.cc` backend is unclear.

### Detection recommendations

- Hunt for `createflags=274` in any parent process that is not `steam.exe`
- Alert on `http://127.0.0.1:*/json/list` HTTP connections from non-browser processes
- YARA on string `__sb_ew_chrome_css` or `__sb_relay_teardown`
- YARA on string `createflags=274` + `steamloopback.host` (CDP target selection logic)
- Block `steambalance.cc` and `cdn.steambalance.cc` at DNS/proxy
- Registry hunt: `HKCU\Software\SteamBalance\SteamBooster`
