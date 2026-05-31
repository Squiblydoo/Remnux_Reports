# Malware Analysis Report: letsvpn-latest.exe

**Date:** 2026-05-31  
**Analyst:** REMnux Automated Workflow  
**Sample:** `letsvpn-latest.exe`

---

## 1. File Metadata

| Field | Value |
|---|---|
| **Filename** | letsvpn-latest.exe |
| **SHA256** | `124e8f7ca958fd8cb2a3baf91681513f93f73d9cfa4efea6f4a1f165d8cbc8d9` |
| **MD5** | `3d8f35e54a3dd41286738c8c2a9823bb` |
| **SHA1** | `d8fe5c317d695733ec312c432cc39e861e46bf4b` |
| **Size** | 23,015,744 bytes (22 MB) |
| **Type** | PE32 executable (GUI) Intel 80386, 6 sections |
| **Compiler** | MSVC 2022 (v17.14.2 pre) |
| **Timestamp** | 2025-12-11 01:39:28 |
| **Signing cert** | Sectigo EV — **Weihai Mingjun Information Technology Co., Ltd.** (Shandong, CN) |
| **Cert serial** | `009cf337c12efc4445ecafcb35d02d64be` |
| **Cert validity** | 2025-11-27 → 2026-11-27 |
| **VersionInfo** | LetsVPN Setup EXE v3.16.4.0 / Letsgo Network Incorporated |

**Section layout:**

| Section | Size | Entropy | Notes |
|---|---|---|---|
| .text | 91 KB | 122 | Dropper installer logic |
| .rdata | 30 KB | 102 | Read-only data |
| .data | 2.5 KB | 102 | — |
| .fptable | 512 B | 0 | — |
| **.rsrc** | **22.8 MB** | **200 (max)** | Houses all 4 embedded PE payloads |
| .reloc | 5 KB | 110 | — |

---

## 2. Classification

**Verdict:** Trojanized LetsVPN Installer — multi-component surveillance spyware  
**Confidence:** High

The outer binary presents itself as a legitimate LetsVPN 3.16.4.0 setup executable, signed with a valid Sectigo EV certificate issued to "Weihai Mingjun Information Technology Co., Ltd." (Shandong, China). It contains four embedded PE files in its 22.8 MB `.rsrc` section:

1. **FFWallpaper/Crash.exe** — MFC-based screen exfil agent with C2 comms  
2. **qr.dll** — Continuous screen-QR-scanning stealer (ZXing-powered)  
3. **libcurl.dll** — Keylogger + shellcode loader masquerading as real libcurl  
4. **Legitimate LetsVPN NSIS installer** — Used as the lure/cover

KesaKode online results: all embedded components scored below the 20% attribution threshold for any known family (CryptBot 2.18%, Bqtlock 26% code overlap). This campaign does not reliably map to a published family; analysis is on its own merits.

---

## 3. Capabilities

### Outer Dropper (letsvpn-latest.exe)
- Extracts four embedded PEs from `.rsrc` section, drops to disk
- Runs the legitimate LetsVPN installer to establish cover
- Creates two Windows Scheduled Tasks via **Task Scheduler COM API** (ITaskService → ILogonTrigger → IExecAction):
  - `MicrosoftEdgeUpdate_TenioDL` — runs FFWallpaper.exe (Crash.exe), triggers on user logon
  - `MicrosoftEdgeUpdate_QR` — runs `rundll32 "%s\qr.dll",StartQR`, triggers on user logon
- Uses `GetUserNameW` to scope the logon trigger to the current user account
- Anti-debug: `IsDebuggerPresent`, `IsProcessorFeaturePresent`, `RaiseException`

### Component 1: FFWallpaper.exe (dropped as Crash.exe)
- MFC GUI application disguised as "CrashReporter"
- Exfiltrates captured data to `https://graphbizhi.hfnuola[.]com/index.php`
- Uploads log files to `https://bizhi.hfnuola[.]com/pc/api/ClientLogReport`
- Beacon URL pattern: `%s?cfg=15&uid=0&input_name=dumpfile&sign=b8e46a113a3fdb8302b03180c9fb50e7`
- Mutex: `Global\AC51D120-3DD0-42CC-A4BB-205C43B451D6`
- **Imports `libcurl.dll` for HTTP comms** → sideloads the malicious keylogger DLL by design
- Signed by expired DigiCert cert (合肥诺拉网络科技有限公司 / Hefei Nora Network Technology, 2021–2024)
- PDB: `\WallPaper\windows\FFWallpaper\bin\Release\CrashReporter.pdb`

### Component 2: qr.dll (StartQR export)
- **Continuous screen QR-code scanner** running in a dedicated thread
- Every 500 ms: captures entire virtual desktop (all monitors) via `GetDC(0)` + `BitBlt (CAPTUREBLT)`
- Processes each bitmap through **ZXing-cpp** QR/barcode decoder
- When a new QR code is found (FNV-1b hash not in seen list, max 20 entries):
  - Saves screenshot as JPEG: `C:\Users\Public\<subdir>\_%Y%m%d-%H%M%S.jpg`
  - Writes hash to `C:\Users\Public\qr_seen.txt`
- Targets: cryptocurrency wallet QR codes, mobile payment QRs, authenticator QRs displayed on screen
- Exfil path: JPEGs uploaded by FFWallpaper to `hfnuola.com`
- PE32+ x64, compiled with MinGW/GCC
- KesaKode: 26% Bqtlock code overlap (toolkit-level, not attribution)
- Signed by the same Weihai Mingjun Sectigo EV cert as the outer dropper

### Component 3: libcurl.dll (keylogger + shellcode loader)
- **DLL sideloaded by FFWallpaper.exe** — placed alongside it in the same directory
- Exports fake libcurl API surface: `curl_easy_init`, `curl_easy_cleanup`, `curl_easy_perform`, `curl_easy_setopt`, `curl_formadd`, `curl_formfree`, `curl_global_init`, `curl_global_cleanup` + `DebugCreate`
- **Keylogger**: installs keyboard hook, writes to `C:\Users\Public\keylog\keylog.txt`
  - Key format: `[ENTER]`, `[BACK]`, `[SHIFT]`, `[CTRL]`, `[WIN]`, `[TAB]`, `[ESC]`, `[SPACE]`, `[VK=%u]`
  - Each session separated by `\r\n\r\n### ` marker
  - Monitors foreground window (GetForegroundWindow) to tag active application
- **Anti-debug (sophisticated PEB patching)**:
  - Scans ntdll.dll's `.text` section for specific byte pattern, overwrites it
  - Clears `NtGlobalFlag` bit in PEB TIB block (removes heap debug markers)
  - Sets return value of `LdrFastFailInLoaderCallout` to 2 (disables loader fail)
  - Modifies `LdrGetDllFullName` return path (hides own DLL path)
- **Shellcode loader via byte-reversal + APC injection**:
  - Reads own DLL file from disk (`GetModuleFileNameA`)
  - Reverses the entire byte array in memory
  - Allocates RWX memory (`VirtualAlloc(0x3000, 0x40)`)
  - Copies reversed bytes to RWX region
  - Queues APC to current thread (`QueueUserAPC`)
  - Triggers via `NtTestAlert()` — delivers shellcode execution
- Signed by the same Weihai Mingjun Sectigo EV cert

### Component 4: Legitimate LetsVPN Installer (cover)
- SHA256: `64eb14b7127d7c08cc6f691818ad041665c2820ad706855af34c52947e70f991`
- 16.5 MB NSIS installer, v3.16.4.0
- Signed by LetsGo Network Incorporated (GlobalSign, serial `39fc16a868afc14f526f1351`)
- Run first to install working LetsVPN software and deflect victim suspicion

---

## 4. Attack Chain

```
letsvpn-latest.exe (victim downloads, runs)
│
├── Extract + run legitimate LetsVPN installer (cover / lure)
│
├── Drop malicious components to disk:
│   ├── Crash.exe   (FFWallpaper exfil agent, 3.96 MB)
│   ├── qr.dll      (QR scanner + screen capture, 2.05 MB)
│   └── libcurl.dll (keylogger + shellcode loader, 113 KB)
│
└── Create logon-triggered scheduled tasks:
    ├── MicrosoftEdgeUpdate_TenioDL → Crash.exe
    │   └── Crash.exe loads libcurl.dll (DLL sideload)
    │       ├── Keylogger active → C:\Users\Public\keylog\keylog.txt
    │       ├── Anti-debug PEB patches apply
    │       └── Shellcode (byte-reversed self) executed via APC+NtTestAlert
    │           └── (shellcode payload unknown; requires key for extraction)
    └── MicrosoftEdgeUpdate_QR → rundll32 qr.dll,StartQR
        └── QR scanner thread starts
            ├── Capture screen every 500 ms
            ├── Scan bitmap with ZXing QR decoder
            └── New QR code found → save JPEG to C:\Users\Public\<subdir>\
```

On subsequent user logons, both tasks fire automatically. The victim sees a working LetsVPN client with no obvious signs of compromise.

---

## 5. IOCs

### Network
| Type | Value | Source |
|---|---|---|
| Domain | `graphbizhi.hfnuola[.]com` | FFWallpaper C2 (main) |
| Domain | `bizhi.hfnuola[.]com` | FFWallpaper C2 (log upload) |
| URL | `https://graphbizhi.hfnuola[.]com/index.php` | FFWallpaper C2 endpoint |
| URL | `https://bizhi.hfnuola[.]com/pc/api/ClientLogReport` | Log/data exfil endpoint |
| String | `b8e46a113a3fdb8302b03180c9fb50e7` | Hardcoded sign token in beacon URL |

### Files
| Path | Description |
|---|---|
| `C:\Users\Public\keylog\keylog.txt` | Keylog output file |
| `C:\Users\Public\keylog\` | Keylog directory |
| `C:\Users\Public\qr_seen.txt` | Seen QR code hash log |
| `C:\Users\Public\<subdir>\_%Y%m%d-%H%M%S.jpg` | Captured QR code screenshots |
| `%drop_path%\Crash.exe` | Dropped FFWallpaper agent |
| `%drop_path%\qr.dll` | Dropped QR scanner |
| `%drop_path%\libcurl.dll` | Dropped keylogger DLL |

### Registry / Scheduled Tasks
| Name | Description |
|---|---|
| `MicrosoftEdgeUpdate_TenioDL` | Logon-triggered task → runs Crash.exe |
| `MicrosoftEdgeUpdate_QR` | Logon-triggered task → rundll32 qr.dll,StartQR |

### Mutexes
| Value | Component |
|---|---|
| `Global\AC51D120-3DD0-42CC-A4BB-205C43B451D6` | FFWallpaper.exe |

### Hashes — Embedded Components
| SHA256 | Component | Size |
|---|---|---|
| `1943498aa47591a0091612e0dffa400a9a05b7754ddf860c8d9190ca4d5ef406` | FFWallpaper.exe (Crash.exe) | 3.96 MB |
| `64eb14b7127d7c08cc6f691818ad041665c2820ad706855af34c52947e70f991` | Legitimate LetsVPN NSIS installer | 16.5 MB |
| `32c49ba3ca279a42cec4724267ac58fd165eec7e2786df3cda3c3c73816510d0` | libcurl.dll (keylogger + loader) | 113 KB |
| `7852e800743de092a28a2c49c3e10f26e94224090f4a13855c03bc5a429d4f7b` | qr.dll (QR scanner + screen capture) | 2.05 MB |

### Certificates (malicious / suspicious)
| Serial | Subject | Validity | Notes |
|---|---|---|---|
| `009cf337c12efc4445ecafcb35d02d64be` | Weihai Mingjun Information Technology Co., Ltd. (CN) | 2025-11-27→2026-11-27 | Signs outer dropper, libcurl.dll, qr.dll |
| `0f17484c75ad2c2ee59d193f33f22083` | 合肥诺拉网络科技有限公司 (Hefei Nora, CN) | 2021-06-18→2024-06-21 (EXPIRED) | Signs FFWallpaper.exe |

---

## 6. Emulation Results

**Speakeasy (outer dropper, x86):** No IOCs captured. The dropper relies on extracting resources to disk before executing payloads — speakeasy cannot emulate RCDATA extraction or scheduled task creation.

**Speakeasy (libcurl.dll, x86 DLL):** Emulation terminated at `DLL_PROCESS_ATTACH` due to unsupported `FlsGetValue2` API stub. Exports emulated but produced no observable IOCs — the keylogger requires real hook installation (SetWindowsHookEx) and the shellcode loader requires real file I/O.

---

## 7. Sandbox Results

**ANY.RUN Score:** 0/100 — "No threats detected"  
**Tags:** None  
**Evasion:** The dropper does not execute malicious payloads in the first run without the presence of an installed target directory and prior drop of the component files. Sandbox evasion is likely time-delayed / environment-dependent.  
**Public URL:** https://app.any.run/tasks/5cf0ff20-fd73-4bd7-bf76-4ff74ec2293d

---

## 8. Analyst Notes

1. **Certificate abuse**: The Weihai Mingjun Sectigo EV certificate (`009cf337c12efc4445ecafcb35d02d64be`) is used to sign three of the four components in this package. This is either a fraudulently obtained Chinese EV certificate or one belonging to a complicit/compromised company. The same cert being used for the outer dropper, a keylogger DLL, and a QR stealer strongly supports intentional fraud — recommend reporting to Sectigo for revocation.

2. **Shellcode in libcurl.dll**: The byte-reversal + APC+NtTestAlert shellcode execution pattern in libcurl.dll could not be fully analyzed — the shellcode payload is the DLL itself read from disk and reversed. Without executing the full chain in a live sandbox, the final shellcode stage remains unknown. This warrants dynamic analysis in a controlled VM.

3. **QR code theft focus**: The primary unusual capability is `qr.dll`'s continuous 500 ms screen capture and ZXing QR decoding loop. This strongly suggests targeting of:
   - Cryptocurrency exchange QR codes (wallet addresses, payment QRs)
   - WeChat/Alipay mobile payment QRs displayed on PC screens
   - Time-based OTP QR code seeding displayed during MFA setup
   - Banking or corporate authentication QRs

4. **PDB path geography**: The FFWallpaper PDB path `\WallPaper\windows\FFWallpaper\bin\Release\CrashReporter.pdb` and the C2 domain naming using `bizhi` (壁纸, Chinese for "wallpaper") confirm a Chinese-origin actor. The infrastructure domain `hfnuola[.]com` ("HF" likely Hefei; "Nuola" likely a company name) points to Anhui Province, consistent with the Hefei Nora signing certificate.

5. **Drop path unknown**: The exact directory where Crash.exe, qr.dll, and libcurl.dll are dropped was not recovered from static analysis. Likely candidates: `%APPDATA%\<random>\`, `%ProgramData%\`, or a subdirectory named to mimic a legitimate application.

6. **Recommended follow-up**:
   - Acquire a live VM, run the sample, and observe what directory the components are dropped to
   - Monitor for network connections to `hfnuola.com` — if the domain is active, a honeypot registration of a victim GUID could yield C2 traffic
   - Execute libcurl.dll in a proper Windows environment to recover the byte-reversed shellcode payload
   - Pivot on the Weihai Mingjun certificate serial in threat intel platforms for related samples
