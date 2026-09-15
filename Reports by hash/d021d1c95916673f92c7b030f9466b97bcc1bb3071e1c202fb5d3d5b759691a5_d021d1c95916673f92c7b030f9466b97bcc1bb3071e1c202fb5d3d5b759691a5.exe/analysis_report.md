# Malware Analysis Report: NightScreenShot Setup (d021d1c9...)

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | `d021d1c95916673f92c7b030f9466b97bcc1bb3071e1c202fb5d3d5b759691a5.exe` |
| SHA256 | `d021d1c95916673f92c7b030f9466b97bcc1bb3071e1c202fb5d3d5b759691a5` |
| SHA1 | `cc60c2ec688faebcff5060e7be65fa07c28d2771` |
| MD5 | `0b2c08a303815a33b3287135f30a06ec` |
| File type | PE32 (Inno Setup `SetupLdr.e32`), 11 sections, 30,601,816 bytes |
| Signer | "Retargeting Labs LLC" — Sectigo Public Code Signing CA EV R36, serial `00ce5bae81d856c6d80813cd807606b340`, valid 2026-06-02 → 2027-08-31 |
| Product metadata | ProductName "NightScreenShot" v2.2.1.0, "NightScreenShot inc." |
| Overlay | 29,747,800-byte InnoSetup archive (entropy 226/255 — LZMA-compressed) |

### Embedded payloads (extracted via Binary Refinery `xtinno`)

| File | SHA256 | Type | Notes |
|---|---|---|---|
| `{app}\NightScreenShot.exe` | `52d3668e2bff0d72c4be6645fb48ca5fce0abbda517b4bfad48193cd44444b62` | PE32+ x64 | Legitimate-appearing screenshot/annotation/OCR/screen-recording tool. PDB: `C:\NSS\NightScreenShot-2.2.1\x64\Release\NightScreenShot.pdb` (built 2026-08-19) |
| `C:\ProgramData\NightScreenShot\NSSUpdater.exe` | `06514535057c6911220cfbd1b2084cd0aeb940bdb4d90f8c717a57095de1bc14` | PE32+ x64, MinGW/g++, symbols intact | **Malicious component** — fingerprinting/gating loader (see below) |
| Bundled Tesseract OCR runtime (~25 DLLs + `tesseract.exe`) | n/a | — | Legitimate open-source OCR engine, used by the real app's OCR feature |

## 2. Classification

**Verdict: Malicious — Pay-Per-Install (PPI) style fingerprinting/gating loader bundled with a genuine, functional screenshot utility as a lure/carrier.**
**Confidence: High** (based on decompiled source logic, not signature/YARA guesswork).

- KesaKode online lookups were run against all three PE components (root installer, `NightScreenShot.exe`, `NSSUpdater.exe`). All scores were **below 20%** (offline hits for QuasarRAT/PulsarRAT/DoubleNLoader/MorpheusLoader topped out at 2.18% online) — per policy this is noise and **is discarded**; no family attribution is made from KesaKode.
- No cross-reference to previously tracked families/samples in memory meets the strict bar (the `ChaCha20.pas` build-path string differs by drive letter from the CryptoVista fingerprint, and the structural fingerprint — Electron app + `readme.txt`/`setup.txt` beacon — is entirely absent here; this installer instead drops a native C++ fingerprinting binary with no Electron component). This sample is analyzed independently.
- Attribution is instead based on **decompiled source-level behavior** of `NSSUpdater.exe`, which retains full debug symbols (`RunCycle`, `CollectFingerprint`, `FingerprintToJson`, `HttpDo`, `InstallPlugin`, `CollectVmDrivers`, `CollectSandboxDlls`) confirming a textbook PPI/loader "gating" architecture: profile the machine → phone home → conditionally deploy a second-stage "plugin" only to victims that pass anti-VM/anti-analyst checks.

## 3. Capabilities

### Installer stage (root PE)
- Inno Setup 6.7.0 installer, silently deploys the genuine "NightScreenShot" screenshot/annotation/OCR app to `C:\Program Files\NightScreenShot\`.
- Drops `NSSUpdater.exe` + `libwinpthread-1.dll` to `C:\ProgramData\NightScreenShot\` (world-writable location).
- Sets `HKCU\...\Run\NightScreenShot` = `"{app}\NightScreenShot.exe"` (standard app autostart).
- **Registers a scheduled task named `NSSUpdater`** via a hidden PowerShell command, triggered `AtLogOn`, with **`RunLevel Highest`** (elevated execution every logon).
- Runs `NSSUpdater.exe` once immediately post-install (`NoWait`).
- Uninstaller properly tears down the task/registry/mutex — cosmetic legitimacy.

### `NSSUpdater.exe` (the malicious gating loader)
- **Anti-analysis / anti-VM**: checks registry keys and driver files for VirtualBox, VMware, Xen, Parallels; checks for Sandboxie (`sbiedll.dll`), Cuckoo (`cuckoomon.dll`), WPE Pro (`wpespy.dll`), Comodo sandbox (`cmdvrt64.dll`), Wine; detects unmoving mouse cursor / lack of user activity (human-interaction check); enumerates running processes and installed software.
- **Fingerprinting** (`CollectFingerprint`/`FingerprintToJson`): CPU cores, RAM, disk size, GPU/PCI vendor IDs, MAC addresses, monitor count, battery presence, MachineGuid, installed software list, default browser, browser history existence (Chrome/Edge/Firefox/Brave/Opera), recent-files/prefetch counts, mouse-movement behavior — serialized to JSON.
- **C2 "gating" check-in**: POSTs the fingerprint JSON to `https://nightscreenshot[.]com/api/check` (User-Agent `NSSUpdater/2.0 (Windows; NightScreenShot)`); server replies with `install_plugin` (bool) + `verdict` string.
  - If `install_plugin == false` → logs `"status":"blocked","verdict":"<reason>"` and exits — no further payload activity (classic sandbox/researcher evasion).
  - If `install_plugin == true` → calls `InstallPlugin()`: downloads `plugin_url` to `%TEMP%\NightScreenShot\plugin_installer.exe`, kills any running `NightScreenShot.exe`, silently executes it (`/VERYSILENT /AUTOUPDATE`), and records `plugin_version.txt`.
- **Operator bypass**: a local file `%TEMP%\NightScreenShot\override.key` is checked for an exact 21-byte hardcoded magic string — **`GSJ65DI92HJD8HHD2H783`** — decoded from three XOR/compare constants in the binary. If present and matching, `InstallPlugin()` is invoked unconditionally, bypassing the C2 gating check entirely (an operator/tester backdoor into the loader).
- **Self-update channel**: separately checks `https://nightscreenshot[.]com/poc/api/version` for app updates, downloads `update_installer.exe`, kills `NightScreenShot.exe`, and silently re-installs (`/VERYSILENT /CLOSEAPPLICATIONS /RESTARTAPPLICATIONS /AUTOUPDATE`).
- **Telemetry**: sends PostHog analytics events (`$create_alias`, `Update_Download_Start`, `Install_complete`, `Gated Installer Status`, `Install_Complete_Search_Changed`) to `https://eu.i.posthog.com/capture/` using a hardcoded project key (`phc_pzTHKoP2h2MhyJZE2zX3qVJwVxR28wJtfFQzZnJZRCCP`) — used by the operator to track install-funnel conversion (a hallmark of commercial PPI/loader-as-a-service operations).
- **Persistence loop**: runs at every user logon via the `NSSUpdater` scheduled task (elevated), re-checking for updates/plugin gating each time.

### `NightScreenShot.exe` (the lure/carrier app)
- Fully-featured, apparently functional Win32 screenshot, annotation, screen-recording, and OCR (bundled Tesseract) application — genuinely usable software, not a decoy stub.
- Only notable outbound call is a lightweight tracking ping to `https://nightscreenshot[.]com/tnx/?mid=` — consistent with ordinary product telemetry, not identified as malicious on its own.

## 4. Attack Chain

1. Victim downloads/runs the signed `NightScreenShot Setup` installer (Sectigo EV-signed, so no SmartScreen friction).
2. Inno Setup silently installs the real, working screenshot app to Program Files.
3. Installer drops `NSSUpdater.exe` to `ProgramData` and registers an elevated, logon-triggered scheduled task for it, then launches it immediately.
4. `NSSUpdater.exe` fingerprints the host (hardware, software, browsers, anti-VM checks, user-activity check) and POSTs the profile to `nightscreenshot[.]com/api/check`.
5. Server-side gating decides, per-victim, whether to serve a `plugin_url`. Sandboxes/VMs/researchers are expected to receive `install_plugin:false` and see no further activity ("blocked").
6. On a positive verdict, `NSSUpdater.exe` downloads and silently executes an arbitrary `plugin_installer.exe` from attacker-controlled infrastructure — this is the true, dynamically-selected second-stage payload, never present in the static file and unrecoverable without server cooperation or the operator override key.
7. The task re-fires at every logon (`RunLevel Highest`), giving the operator a persistent re-entry point to push new plugins/updates over time.

## 5. IOCs

### Network (defanged)
- `nightscreenshot[.]com` — installer publisher domain **and** gating C2 (dual-use)
  - `hxxps[://]nightscreenshot[.]com/api/check` — fingerprint POST / gating decision endpoint
  - `hxxps[://]nightscreenshot[.]com/poc/api/version` — self-update check endpoint
  - `hxxps[://]nightscreenshot[.]com/tnx/?mid=` — lure-app tracking pixel
- `hxxps[://]eu[.]i[.]posthog[.]com/capture/` — third-party analytics (PostHog SaaS), used by operator to track infection funnel; PostHog project key `phc_pzTHKoP2h2MhyJZE2zX3qVJwVxR28wJtfFQzZnJZRCCP`

### Filesystem
- `C:\Program Files\NightScreenShot\NightScreenShot.exe` (lure app)
- `C:\ProgramData\NightScreenShot\NSSUpdater.exe` (malicious loader)
- `C:\ProgramData\NightScreenShot\libwinpthread-1.dll`
- `%TEMP%\NightScreenShot\plugin_installer.exe` (dynamically downloaded second stage — not present in this sample)
- `%TEMP%\NightScreenShot\plugin_version.txt`
- `%TEMP%\NightScreenShot\override.key` (operator bypass file — content: `GSJ65DI92HJD8HHD2H783`)
- `%TEMP%\NightScreenShot\version.txt`, `debug.key`, `update_installer.exe`
- `NSSUpdater_log.txt`

### Registry
- `HKCU\Software\Microsoft\Windows\CurrentVersion\Run\NightScreenShot` = `"{app}\NightScreenShot.exe"`
- `HKCU\Software\NightScreenShot\Launched` (DWORD state flag)
- Uninstall key `{8F3A1C2E-5B7D-4E9A-9C1F-2A6B8D4E7F10}_is1`

### Scheduled Task
- Task name: `NSSUpdater` — Action: `C:\ProgramData\NightScreenShot\NSSUpdater.exe` — Trigger: At Logon — `RunLevel: Highest`

### Certificate
- Subject: "Retargeting Labs LLC"; Issuer: Sectigo Public Code Signing CA EV R36; Serial `00ce5bae81d856c6d80813cd807606b340`

## 6. Emulation Results

- **Speakeasy (generic runner, x64)** on `NSSUpdater.exe`: completed without crashing but produced **zero IOCs** — the binary is a modern MinGW/g++ C++17 build using `WinHTTP` + CRT internals that speakeasy's Windows API model does not fully cover; execution likely stalled before reaching `HttpDo`/`RunCycle`.
- **Static decompilation substituted successfully**: because the binary retains full debug symbols (unstripped MinGW build), malcat's decompiler combined with the object's symbol table (recovered via `objdump -t` address mapping) fully reconstructed the gating logic without needing dynamic execution. No angr/custom-hook pass was required.
- No packer or anti-disassembly obfuscation was present in `NSSUpdater.exe` — anti-detection is purely fingerprinting/behavioral (VM/sandbox checks + server-side gating), not code-level evasion.

## 7. Sandbox Results (ANY.RUN)

- **Verdict score: 80/100 — "Suspicious activity"**
- **Tags**: `delphi`, `inno`, `installer`, `auto-reg`, `evasion`
- **Public report**: https://app.any.run/tasks/c83b516e-e6bf-4162-80fb-7c6dd2a9f346
- **Observed traffic**: `nightscreenshot[.]com` (DNS + `/tnx/?mid=<uuid>` tracking pixel + `/poc/api/version` self-update check), plus PostHog analytics (`eu.i.posthog[.]com`, `eu-assets.i.posthog[.]com`) and unrelated Windows/Edge/Bing telemetry noise.
- **Notable absence**: the gating endpoint `/api/check` — the fingerprint-POST/plugin-gating call identified via static decompilation — **does not appear** in the sandbox's captured traffic, even though the self-update check to `/poc/api/version` did fire. This is strong behavioral corroboration of the anti-VM logic found in `CollectVmDrivers`/`CollectSandboxDlls`: the ANY.RUN environment likely tripped a VM/sandbox check that caused `NSSUpdater.exe` to skip the `CollectFingerprint` → `/api/check` gating path entirely, exactly as the loader is designed to do against analysis environments. The `evasion` tag assigned by ANY.RUN aligns with this.
- No plugin download or second-stage payload execution was observed, consistent with the gating either blocking or never being reached in this sandboxed run.

## 8. Analyst Notes

- The actual second-stage "plugin" payload is **not recoverable from this sample** — it is served dynamically only to victims that pass the server-side gate, a deliberate anti-research design. Submitting from a residential/non-datacenter IP with human-like interaction (mouse movement) would be required to trigger a live `install_plugin:true` response, and even then the response is likely time/IP/geo-gated by the operator.
- The recovered override key (`GSJ65DI92HJD8HHD2H783`) could plausibly be used by researchers to force `InstallPlugin()` execution in a controlled sandbox (by dropping `%TEMP%\NightScreenShot\override.key` with that exact content before running `NSSUpdater.exe`) — **however this would only work if `plugin_url`/`plugin_version.txt` values are also legitimately present from a prior `/api/check` response**, since `InstallPlugin` is called with the manifest's `plugin_url`. This is worth a live follow-up test if further investigation of the second-stage payload is desired.
- The dual identity of `nightscreenshot.com` (legitimate-looking product site + gating C2) and the presence of a real, working product suggests this is either (a) a legitimate small ISV whose install pipeline has been compromised/monetized via a PPI SDK, or (b) a purpose-built PPI "seeder" app using a genuinely functional tool as bait to maximize install legitimacy and evade reputation-based defenses. The valid EV code-signing certificate (issued to "Retargeting Labs LLC", a name itself suggestive of ad-tech/PPI industry) supports the latter reading.
- Recommended follow-up: passive DNS / WHOIS history on `nightscreenshot.com`; monitor for the domain rotating to new `plugin_url` values; flag `override.key`/`debug.key` file drops in `%TEMP%\NightScreenShot\` as a detection opportunity for defenders (their mere presence indicates operator/tester bypass activity, unusual on a genuine end-user install).
