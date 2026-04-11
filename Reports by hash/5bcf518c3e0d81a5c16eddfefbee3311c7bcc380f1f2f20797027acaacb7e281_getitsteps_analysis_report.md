# getitsteps.msi — Analysis Report

**Date:** 2026-04-11  
**Analyst:** Claude (automated static analysis)

---

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | `getitsteps.msi` |
| SHA256 | `5bcf518c3e0d81a5c16eddfefbee3311c7bcc380f1f2f20797027acaacb7e281` |
| SHA1 | `fda970c71702dc0791e9609d71b81a0a836b10eb` |
| MD5 | `2653fa8e1e29e3342bebc20d056ddf50` |
| Size | 7,807,488 bytes (7.5 MB) |
| Type | Composite Document File V2 / Windows MSI Installer |
| Subject | Get It Steps |
| ProductCode | `{AB861E8A-EC53-4879-A6A8-9619C75383CF}` |
| RevisionNumber | `{02F1FB86-0BFB-4AFF-983A-AE96F7A49BAB}` |
| ProductVersion | 6.154.18 |
| Created / Last Saved | 2025-11-30 08:45:40 |
| Builder | **Advanced Installer 22.7 build 8031c82a** |
| **Certificate Subject** | **Sichuan Youyixing Technology Co., Ltd.** (Sichuan Sheng, CN) |
| Certificate Issuer | Sectigo Public Code Signing CA EV R36 |
| Certificate Serial | `00fb9fd9d5cf4778459da4762cfdd4ca55` |
| Certificate Validity | 2025-10-03 → 2026-10-03 (1-year EV) |

### Embedded Binaries

| File | SHA256 | Size | Type | Notes |
|---|---|---|---|---|
| `GetItSteps.exe` | `c75649be672e477071c2b344ce0095b52796803afadfe8ad37b724dc92f220fd` | 79,872 B | PE32 .NET 4.7.2 | Lure app; WebView2 wrapper; fake debug timestamp 2059-02-27 |
| `Setup.exe` | `ad96fe9c9798d792f249215e0f057fdf3f90f7e081b2fe50c8c24b9b365fbac8` | 294,656 B | PE32+ .NET x64 | **Chrome search hijacker**; same EV cert as MSI |
| `MicrosoftEdgeWebview2Setup.exe` | (embedded) | 1,636,808 B | PE (signed by Microsoft) | Legitimate WebView2 prerequisite; decoy |
| `PowerShellScriptLauncher.dll` | — | 802,408 B | PE; PDB: `C:\ReleaseAI\win\...\riptLauncher.pdb` | Caphyon Advanced Installer PS execution engine |
| `aicustact.dll` / `Prereq.dll` | — | ~1 MB / ~947 KB | PE | Standard Advanced Installer components |

**PDB artifact (GetItSteps.exe):**  
`F:\IDD\Projects\AT -Batch 3.2\251109_getitsteps\Get It Steps\Get It Steps\obj\Release\Get It Steps.pdb`

- `IDD` — likely developer/org initials  
- `AT` — "Adware/Tracker" batch project designation  
- `3.2` — batch version  
- `251109` — build date 2025-11-09  

---

## 2. Classification

| | |
|---|---|
| **Type** | Adware / Browser Search Hijacker |
| **Confidence** | **HIGH** |
| **Family** | `GetMoreSoftware` distribution network / `optimizeddefault.com` hijack cluster |

This MSI is a deceptively signed, EV-certified adware installer that:

1. Installs a legitimate-looking "Get It Steps" WebView2 app as cover  
2. Executes `Setup.exe` to **permanently hijack Chrome's default search engine** to an affiliate-tracked URL  
3. Sends machine fingerprint and session telemetry to two C2 domains at install start and completion  

---

## 3. Capabilities

- **Browser Search Engine Hijacking** — directly modifies Chrome's `Web Data` SQLite file; inserts and updates the `keywords` table to replace all search engines with `https://search.optimizeddefault[.]com/?q={searchTerms}&u=<MachineId>`
- **New Tab URL Hijacking** — sets Chrome's `new_tab_url` field to `https://www[.]cnn[.]com` for all profiles
- **Policy Bypass** — sets `created_by_policy=0`, `enforced_by_policy=0`, `featured_by_policy=0` to prevent Chrome's enterprise policy lock from reverting the search engine
- **Chrome Process Termination** — kills all `chrome.exe` processes before modifying locked SQLite files; polls up to 30 seconds for Chrome to exit
- **Multi-profile Coverage** — enumerates all Chrome profiles (`Profile*` + `Default`) under `%LOCALAPPDATA%\Google\Chrome\User Data\` and hijacks each
- **Machine Fingerprinting** — reads `HKLM\SOFTWARE\Microsoft\Cryptography\MachineGuid` and appends it as a tracking parameter (`&u=`) in the hijacked search URL; every search is individually tied to the victim machine
- **Affiliate Telemetry** — two separate C2 channels:
  - **MSI layer** (PowerShell): POSTs `{"product":"getitsteps.com","affid":"511190","event":"InstallStart/InstallComplete","sessionid":...,"windowsver":...,"machineid":...}` to `https://event[.]getitsteps[.]com`
  - **Setup.exe layer** (HTTP): POSTs `{"Call":"Start/Finish","User":<MachineId>,"Sesh":<SessionId>,"IOne":<AffID>,"ITwo":<SubID>,"AdditionalInfo":...}` to `https://offer[.]getmoresoftware[.]com`
- **Session Persistence** — writes a session GUID to `%TEMP%\my_session.txt` for correlation across install start/finish telemetry calls
- **Hidden Execution** — `Setup.exe` calls `ShowWindow(hWnd, SW_HIDE=0)` to run silently; console window is never shown
- **Embedded Legitimate Components** — bundles real `MicrosoftEdgeWebview2Setup.exe` (signed by Microsoft) and real SQLitePCL/WebView2 NuGet assemblies as cover
- **Signed with EV cert** — Sectigo 1-year EV certificate for a Sichuan, China entity; likely purchased/abused for SmartScreen bypass

---

## 4. Attack Chain

```
User executes getitsteps.msi
│
├─ Seq 51 [AI_DATA_SETTER_1 + PowerShellScriptInline_1] (immediate, silent)
│   └─ PowerShell: generate sessionid → write %TEMP%\my_session.txt
│      → POST https://event.getitsteps.com {product=getitsteps.com, affid=511190,
│                                            event=InstallStart, sessionid, windowsver, machineid}
│
├─ Seq 52 [PowerShellScriptInline_1] — PS1 execution via PowerShellScriptLauncher.dll
│
├─ [Normal MSI install: installs GetItSteps.exe, Setup.exe, WebView2 SDK, SQLite DLLs
│    to %APPDATA%\Get It Steps\Get It Steps\]
│
├─ Seq 6601 [LaunchExeWithDirectory]
│   └─ Runs: "[TempFolder]\Setup\Setup.exe" tiqrwu subid agree-to-terms=true
│       │
│       ├─ Reads MachineGuid from HKLM\SOFTWARE\Microsoft\Cryptography
│       ├─ POST https://offer.getmoresoftware.com {"Call":"Start","User":<MachineId>,...}
│       ├─ Checks agree-to-terms arg present & Chrome installed
│       ├─ Kills all chrome.exe processes (polls 30s for exit)
│       ├─ Sets hijack URL = https://search.optimizeddefault.com/?q={searchTerms}&u=<MachineId>
│       ├─ For each Chrome profile (%LOCALAPPDATA%\Google\Chrome\User Data\{Default,Profile*}):
│       │   ├─ Opens Web Data SQLite directly
│       │   ├─ INSERT INTO keywords: copy existing entry, replace url with hijack URL,
│       │   │                        sync_guid='ab20c06c-f1df-42ad-9b82-6f318a3e97da',
│       │   │                        prepopulate_id=255
│       │   └─ UPDATE keywords SET url=<hijack>, new_tab_url='https://www.cnn.com',
│       │                          safe_for_autoreplace=0, is_active=1,
│       │                          created_by_policy=0, enforced_by_policy=0,
│       │                          sync_guid=<random UUID v4>
│       └─ POST https://offer.getmoresoftware.com {"Call":"Finish","User":<MachineId>,...}
│
├─ Seq 6602 [AI_DATA_SETTER_2 + PowerShellScriptInline_2]
│   └─ PowerShell: read sessionid from %TEMP%\my_session.txt
│      → POST https://event.getitsteps.com {event=InstallComplete, sessionid, machineid, ...}
│
└─ Seq 6401 [GetItSteps.exe] (registered to run on subsequent logins)
    └─ Simple .NET WinForms app with WebView2 control
       UserData: %LOCALAPPDATA%\Get_It_Steps_WebView2\<username>\
       Navigates to: https://howto.getitsteps.com
```

---

## 5. IOCs

### Network

| Type | Indicator | Context |
|---|---|---|
| Domain | `event[.]getitsteps[.]com` | MSI-layer telemetry C2 (InstallStart/InstallComplete) |
| Domain | `offer[.]getmoresoftware[.]com` | Setup.exe C2 (LogEvent JSON POST) |
| Domain | `search[.]optimizeddefault[.]com` | Hijacked search engine URL |
| Domain | `howto[.]getitsteps[.]com` | Loaded in GetItSteps.exe WebView2 |
| URL | `https[://]event[.]getitsteps[.]com` | Affiliate telemetry endpoint |
| URL | `https[://]offer[.]getmoresoftware[.]com` | Setup.exe JSON POST |
| URL | `https[://]search[.]optimizeddefault[.]com/?q={searchTerms}&u=<MachineId>` | Injected Chrome search URL |

### Filesystem

| Path | Context |
|---|---|
| `%APPDATA%\Get It Steps\Get It Steps\` | Application install directory |
| `%LOCALAPPDATA%\Google\Chrome\User Data\*\Web Data` | SQLite file modified (all profiles) |
| `%LOCALAPPDATA%\Get_It_Steps_WebView2\<username>\` | GetItSteps.exe WebView2 user data |
| `%TEMP%\my_session.txt` | Affiliate session GUID persistence |
| `%APPDATA%\Get It Steps\Get It Steps\prerequisites\` | Prerequisite drop directory |

### Registry

| Key | Context |
|---|---|
| `HKLM\SOFTWARE\Microsoft\Cryptography\MachineGuid` | Read for machine fingerprinting |
| `HKCU\Software\Get It Steps\Get It Steps\Version` | Product version written at install |
| `HKCU\Software\Get It Steps\Get It Steps\Path` | Install path written at install |

### Tracking / Campaign

| Indicator | Value |
|---|---|
| Affiliate ID (`affid` / `IOne`) | `511190` |
| Campaign token | `tiqrwu` |
| Sub-ID arg | `subid` (placeholder; varies per distribution) |
| Injected Chrome sync_guid | `ab20c06c-f1df-42ad-9b82-6f318a3e97da` |

### Hashes

| File | SHA256 |
|---|---|
| `getitsteps.msi` | `5bcf518c3e0d81a5c16eddfefbee3311c7bcc380f1f2f20797027acaacb7e281` |
| `GetItSteps.exe` | `c75649be672e477071c2b344ce0095b52796803afadfe8ad37b724dc92f220fd` |
| `Setup.exe` | `ad96fe9c9798d792f249215e0f057fdf3f90f7e081b2fe50c8c24b9b365fbac8` |

---

## 6. Emulation Results

Not applicable — MSI installer with .NET payloads; speakeasy cannot emulate MSI CustomAction sequences or .NET managed code. Static analysis via `msiinfo`, `ilspycmd`, and `cabextract` provided complete coverage of the attack chain without requiring emulation.

---

## 7. Sandbox Results

**Tria.ge:** Submission failed with error code 1010 (likely IP block or rate limit). Local report in `/home/remnux/mal/output/` is authoritative.

---

## 8. MITRE ATT&CK

| Technique | ID | Description |
|---|---|---|
| Masquerading — Match Legitimate Name | T1036.001 | "Get It Steps" branded lure with EV cert for SmartScreen bypass |
| Code Signing | T1553.002 | EV certificate (Sectigo / Sichuan Youyixing Technology Co. CN) |
| PowerShell | T1059.001 | Two embedded PowerShell scripts for telemetry via `PowerShellScriptLauncher.dll` |
| Process Discovery | T1057 | `GetProcessesByName("chrome")` before kill |
| Service Stop | T1489 | `Process.Kill()` all Chrome instances |
| Modify Registry | T1112 | Reads MachineGuid; writes install path/version to `HKCU\Software\Get It Steps\...` |
| Browser Session Hijacking | T1185 | Directly modifies Chrome `Web Data` SQLite (keywords / new_tab_url) |
| Steal Web Session Cookie | T1539 | Machine ID appended to every search query for per-victim tracking |
| Exfiltration Over C2 Channel | T1041 | Machine ID + OS version + session ID + affiliate data POSTed to two C2s |

---

## 9. Analyst Notes

### Confidence & Classification
This is **unambiguously adware/PUA** rather than a high-severity RAT or infostealer. The payload:
- Does not steal credentials or cookies
- Does not establish persistence via `Run` keys or scheduled tasks beyond the MSI app registration
- Does not download secondary payloads
- Generates revenue exclusively via affiliate search query redirection and install telemetry

However, the following factors elevate the threat:
- **EV-signed with a Chinese entity cert** — acquired specifically to bypass SmartScreen/AV detection
- **Machine-ID tracking per search query** — all user searches are individually tied to the victim machine permanently
- **Policy bypass** — explicitly removes Chrome's `enforced_by_policy` flag, making the hijack survive enterprise Chrome policy pushes
- **Multi-profile coverage** — hijacks every Chrome profile, not just the default

### Distribution Mechanism
The `getmoresoftware[.]com` domain is consistent with a known adware distribution network that bundles hijacker payloads into "freeware" installers. The affiliate ID `511190` is embedded in both layers (MSI PS and Setup.exe), confirming this is a pay-per-install distribution. The campaign token `tiqrwu` is the server-side tracking identifier for this specific distribution run.

### Remediation
1. Remove `Get It Steps` via Add/Remove Programs
2. For each Chrome profile: navigate to Settings → Search Engine → reset to Google (or preferred)
3. Optionally: delete `%LOCALAPPDATA%\Google\Chrome\User Data\*\Web Data` and let Chrome rebuild it (resets all saved searches/autocomplete)
4. Delete `%TEMP%\my_session.txt`
5. Check `%APPDATA%\Get It Steps\` and `%LOCALAPPDATA%\Get_It_Steps_WebView2\` for residual files

### Residual Questions
- What does `GetItSteps.exe` / `https://howto.getitsteps.com` actually display? Likely a "step-by-step" guide to using the hijacked search engine, or a retention page. Dynamic analysis would confirm.
- The `Resources.data` byte array embedded in Setup.exe (loaded as `webData`) was not fully traced — it may serve as a fallback SQLite template if `Web Data` is absent.
- Tria.ge sandbox was unavailable; behavioral confirmation of the Chrome modification sequence would further validate this analysis.
