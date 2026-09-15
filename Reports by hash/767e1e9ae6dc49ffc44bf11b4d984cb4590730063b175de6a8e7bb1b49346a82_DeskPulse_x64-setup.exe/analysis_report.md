# DeskPulse_x64-setup.exe — Malware Analysis Report

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | `DeskPulse_x64-setup.exe` |
| SHA256 | `767e1e9ae6dc49ffc44bf11b4d984cb4590730063b175de6a8e7bb1b49346a82` |
| MD5 | `3ae3d9e11eed3b13e1b6796207dfd518` |
| SHA1 | `5a5768a23945f853569c89cdcc730cdb6771ff11` |
| Size | 105,498,680 bytes (~100.6 MB) |
| Type | PE32, Nullsoft Installer (NSIS) self-extracting archive, 5 sections + 105.4 MB overlay |
| Signer | **AirTiki ApS** (Denmark, EV) |
| Cert issuer | Sectigo Public Code Signing CA EV R36 |
| Cert serial | `0531fc09129ab7e3f36f0dfc92333ede` |
| Cert validity | 2026-07-31 → 2027-07-31 |
| Signature status | **Valid**, RFC3161-timestamped (DigiCert, 2026-09-03 11:55:12 GMT) |
| PE VersionInfo | ProductName/FileDescription: `DeskPulseIO`, Version `0.1.0` |
| Build tool | NSIS 3.08 wrapping a Tauri v1 (`tauri-runtime-0.14.6`) Rust/WebView2 application |

**Extracted components** (via Binary Refinery `xtnsis`):

| File | SHA256 | Notes |
|---|---|---|
| `DeskPulseIO.exe` | `f43ee43d5c9b9afa6dcc205a40f7bc4c8d157745f826949bc6d179870a61dd41` | Main app, PE32+ x64, PDB `DeskPulse.pdb`, built 2026-09-03, **unsigned** |
| `setup.exe` (nested) | `82d097592b381688d2775d0288b0ee88aed9322a6bee9400277271031d2305d2` | 77 MB, itself an NSIS installer, **unsigned** |
| `uninstall.exe` | `9a204323f41cd10206f1d5341fb2785b2b153f1dd3290b62d1b47fedf6ee0fa2` | Standard NSIS-generated uninstaller stub |
| `VC_redist.x64.exe` | `5d9999036f2b3a930f83b7fe3e2186b12e79ae7c007d538f52e3582e986a37c3` | Genuine Microsoft VC++ 2015-2022 x64 redistributable |
| `DeskPulse.exe` (deep-nested Electron binary, inside `setup.exe`'s `app-64.7z`) | `5cc5a6f64d86b090359d47043a6e11c162790f7eb7b00f184239f91fbb409ba3` | 176 MB Electron main executable, **unsigned** |

## 2. Classification

**Verdict: No malicious functionality identified. Low-confidence, non-transparent bundling anomaly — not classified as malware.**

- **KesaKode (online, `DeskPulseIO.exe`)**: `JloRAT` 18.29%, `Splinter` 16.51%, `Manjusaka` 0.11%, `RustyStealer` 0.06%. All **below the 20% threshold** → discarded per policy; no family attribution.
- **KesaKode (online, outer installer)**: lookup produced no output (105 MB installer likely exceeds the service's practical processing size) — inconclusive, not used.
- **ANY.RUN (`DeskPulseIO.exe`)**: score **0/100, "No threats detected"**, zero behavioral tags. See §7.
- No wallet-address regex, clipboard-hijack, keylogging exfiltration, or destructive file-encryption logic found in source review of the bundled Electron app.
- The YARA `ValuableFileExtensions` (ransomware-associated extension list) and `ElevatePrivileges` hits are standard NSIS/UAC-elevation boilerplate artifacts (`ExecShell "runas"` used to relaunch the installer elevated) — not corroborated by any file-enumeration/encryption code path found during review.

**The one genuine anomaly**, documented in full under §4/§8: the signed 100 MB installer silently carries a second, completely unrelated 77 MB application (a different product, different version scheme, different install GUID) that is never invoked anywhere in the visible NSIS install script. This is unusual, non-transparent packaging, but investigation found no evidence it is a malicious delivery mechanism — see reasoning below.

## 3. Capabilities

**`DeskPulse_x64-setup.exe` (outer installer, standard Tauri-bundler-generated NSIS template):**
- Standard NSIS install flow: checks for existing install, offers uninstall/reinstall, writes `Software\DeskPulse\DeskPulseIO` and standard `...\Uninstall\DeskPulseIO` registry keys, creates Start Menu/desktop shortcuts.
- Downloads and silently installs the official Microsoft WebView2 Evergreen bootstrapper (`https://go.microsoft.com/fwlink/p/?LinkId=2124703`) if not already present — legitimate, standard Tauri-app behavior.
- Extracts `DeskPulseIO.exe`, `VC_redist.x64.exe`, and `setup.exe` to `$INSTDIR`; only `DeskPulseIO.exe` is ever executed by the script (via `ExecShell "runas"` after install).

**`DeskPulseIO.exe` (the actual product):**
- Minimal, near-boilerplate Tauri v1 desktop app shell (WebView2-hosted). No app-specific frontend assets were recoverable in plaintext (Tauri compiles/compresses them into the binary).
- Exposes a small set of custom Tauri IPC commands to its frontend, including the Rust example `greet` command (unmodified from the Tauri template) and a system-diagnostics command that:
  - Reads `HKLM\HARDWARE\DESCRIPTION\System\BIOS` (`SystemManufacturer`, `SystemProductName`, `BIOSVendor`) and checks for `vmware`/`virtualbox`/`qemu`/`xen`/`bochs`/`parallels`/`kvm` substrings, and checks free disk space via `GetDiskFreeSpaceExW`.
  - This result is returned synchronously as an IPC response to the webview frontend (traced via decompilation of `sub_1400ce530` → caller `sub_140129c30`, the Tauri command dispatcher). **No code path was found that uses this result to gate execution of `setup.exe`, make a network call, or alter installer behavior** — it functions as an on-demand system-info query, not a silent PPI/gating fingerprint-and-phone-home routine (contrast with the unrelated `NSSUpdater.exe` PPI-loader pattern, which this superficially resembles but does not match on any concrete indicator).
- No literal reference to `setup.exe`, `\setup.exe`, or any file/process-execution string targeting the bundled installer was found anywhere in `DeskPulseIO.exe`'s strings or decompiled code.

**`setup.exe` (nested, unexecuted 77 MB installer) → `DeskPulse.exe` (Electron app):**
- A separate, legitimately-structured, MIT-licensed open-source Electron application, `package.json` self-identifies: name `deskpulse`, productName `DeskPulse`, version `2.0.0`, description "Open-Source Crypto Price Widget for Windows 11", homepage `https://deskpulse.io`, repository `https://github.com/DeskPulse/DeskPulse`.
- Full `main.js` source reviewed: single-instance lock, tray icon, hotkey toggle, `electron-updater` auto-update, settings store, market-data polling via `MarketService`. IPC handlers are narrowly scoped (settings, market data, window control, external-link opening) — no arbitrary command execution, no filesystem enumeration/encryption, no clipboard access.
- Live network calls are to `https://api.coingecko.com/api/v3` (price data) and `https://api.alternative.me/fng/?limit=1&format=json` (Fear & Greed Index) — both legitimate public crypto-data APIs.

## 4. Attack Chain

No attack chain was established — no malicious execution trigger was found.

Installed layout: `DeskPulse_x64-setup.exe` (signed, run by user) → extracts `DeskPulseIO.exe` + `VC_redist.x64.exe` + `setup.exe` to `$INSTDIR` → launches only `DeskPulseIO.exe`. `setup.exe` (the "DeskPulse 2.0.0" crypto-widget installer) sits inert in `$INSTDIR` unless a user manually runs it — no code path in either the NSIS script or the compiled `DeskPulseIO.exe` was found to invoke it automatically.

Given identical branding thread (`DeskPulse` name, `deskpulse.io` domain referenced in both the outer installer's registry key prefix and the inner app's `package.json` homepage), the most likely explanation is that the same developer's Tauri build config (`tauri.conf.json` → `bundle.resources`) accidentally or intentionally bundled their other product's installer as a static resource file — a packaging oddity rather than a demonstrated malicious dropper mechanism. This could not be fully confirmed without developer/vendor context, and is flagged as a residual gap (§8).

## 5. IOCs

**Network** (all legitimate/expected — no C2 indicators found):
- `api[.]coingecko[.]com` — CoinGecko public price API (used by bundled Electron app)
- `api[.]alternative[.]me` — Fear & Greed Index public API (used by bundled Electron app)
- `go[.]microsoft[.]com/fwlink/p/?LinkId=2124703` — official Microsoft WebView2 bootstrapper

**Filesystem:**
- `%ProgramFiles%\DeskPulseIO\DeskPulseIO.exe`, `VC_redist.x64.exe`, `setup.exe`, `uninstall.exe`
- `%TEMP%\MicrosoftEdgeWebview2Setup.exe` (transient, deleted after use)

**Registry:**
- `HKCU\Software\DeskPulse\DeskPulseIO`
- `HK{LM,CU}\Software\Microsoft\Windows\CurrentVersion\Uninstall\DeskPulseIO`
- `HK{LM,CU}\Software\Microsoft\Windows\CurrentVersion\Uninstall\6792201d-4f97-56a6-8ce3-c24e741cac66` (written only if the nested `DeskPulse 2.0.0` installer is separately, manually run)

**Certificates:**
- Signer: AirTiki ApS (DK) — Serial `0531fc09129ab7e3f36f0dfc92333ede`, issued by Sectigo Public Code Signing CA EV R36

**Hashes:** see table in §1.

## 6. Emulation Results

- **Speakeasy** (`DeskPulseIO.exe`, amd64): emulation halted almost immediately during Rust CRT/TLS initialization (`LoadLibraryExW` for `api-ms-win-core-synch`/`api-ms-win-core-fibers`, `FlsAlloc`/`FlsSetValue`) — only 5 low-value API calls captured, no network/file/registry/mutex IOCs recovered. This is an expected limitation for modern Rust binaries with complex fiber-local-storage-based runtime init; speakeasy could not progress into application logic.
- **capa**: timed out after 180s on `DeskPulseIO.exe` — large Rust binaries with thousands of monomorphized functions routinely exceed capa's practical analysis time; no results obtained.
- No further emulation passes (angr/custom hooks) were pursued given the ANY.RUN sandbox result (§7) provided direct dynamic coverage instead.

## 7. Sandbox Results

- **Outer installer** (`DeskPulse_x64-setup.exe`, 105 MB): submission rejected — **"exceeds the limit of 100 Mb"** (ANY.RUN Ally-tier upload limit).
- **`DeskPulseIO.exe`** (10 MB, the actual executed payload) submitted instead:
  - **Verdict: score 0/100, "No threats detected"**
  - **Tags: none**
  - Public report: https://app.any.run/tasks/9e3a6cd6-6d1c-4e82-a240-abf5b8d81e89

peframe's heuristic behavior tags on `DeskPulseIO.exe` (`vmdetect`, `antidbg`, `network tcp/dns`, `screenshot`, `keylogger`, `win registry`) are consistent with — and explained by — the embedded WebView2/Chromium loader runtime (confirmed present via strings: `EBWebView\x64\EmbeddedBrowserWebView.dll`, `WEBVIEW2_RELEASE_CHANNEL_PREFERENCE`, TLS/certificate-validation code) and the app's own `globalShortcut` hotkey registration (Electron widget) rather than genuine malicious capability. This is corroborated by the clean ANY.RUN verdict and by source-level review of the bundled Electron app finding no matching malicious logic.

## 8. Analyst Notes

- **Primary residual gap**: why does the signed installer carry an entire unrelated, unexecuted 77 MB application? Two hypotheses, neither confirmed:
  1. **Benign packaging mistake / cross-bundling** — both products share the "DeskPulse" brand and (per the inner app's `package.json`) the `deskpulse.io` domain; a shared Tauri build pipeline could have included the sister product's installer as a static `resources` entry without a corresponding launch action being wired up yet (e.g., an in-development "install our other app" upsell feature).
  2. **Intentional non-transparent bundling** for later/conditional use not observed in this static+one-sandbox-run analysis (e.g., a UI button inside `DeskPulseIO.exe`'s own webview frontend that we could not extract in plaintext, since Tauri compiles/compresses frontend assets into the binary).
  - Recommended follow-up: run `DeskPulseIO.exe` interactively (manual VM, not just automated sandbox) and click through its UI to check for any "install"/"get widget" affordance that shells out to `setup.exe`; alternatively extract the compiled frontend asset blob directly from the binary rather than relying on runtime webview rendering.
- KesaKode's low-confidence `JloRAT`/`Splinter` hits are noise-level and were discarded per the ≥80%/20-79%/<20% policy — not corroborated by capability, C2, or behavioral evidence.
- The nested `setup.exe` → `DeskPulse.exe` (Electron, "crypto price widget") was reviewed at the source level (`main.js`, `market.js`, `preload.js`, `settings.js`, `alerts.js`) with no malicious functionality found; it is treated as a legitimate, if oddly delivered, third-party open-source application.
- No sample in memory shares an exact matching indicator (certificate serial, C2, config value, build artifact, or payload hash) with this sample — cross-reference policy is not met by any tracked family or standalone entry, including the superficially similar `NightScreenShot`/`NSSUpdater.exe` PPI-gating-loader case (different cert, different fingerprint routine, no shared C2/config/build-artifact/hash). This sample is analyzed and reported entirely on its own merits.
