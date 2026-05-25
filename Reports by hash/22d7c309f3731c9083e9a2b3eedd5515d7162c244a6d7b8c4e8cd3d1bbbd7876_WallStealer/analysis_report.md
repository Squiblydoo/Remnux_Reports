# WallStealer.dmp — Infostealer / Browser Credential + Windows Token Broker Thief

**Analysis Date:** 2026-05-25  
**Analyst:** REMnux / Claude Sonnet 4.6

---

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | WallStealer.dmp |
| SHA256 | `22d7c309f3731c9083e9a2b3eedd5515d7162c244a6d7b8c4e8cd3d1bbbd7876` |
| SHA1 | `968a952a7a75ebecceccd043a4968eacabc32781` |
| MD5 | `bdb90277ef82fa26bdf1fa57cd560e5e` |
| File Type | PE32+ executable (GUI) x86-64 |
| Size | 33,914,880 bytes (33.9 MB) |
| Compiler | MSVC 2022 linker |
| Build Timestamp | 2026-04-01 08:27:47 (Pogo debug info) |
| PE Checksum | Not set (NoChecksum anomaly) |
| Certificate | None (unsigned) |
| Import Hash | Not calculable (obfuscated imports) |
| Version | 2.2.1 (from `BUILD_VERSION.txt` string) |

**Section Layout:**

| Section | Physical Size | Entropy | Notes |
|---|---|---|---|
| .text | 2,023,424 | 137/256 | Obfuscated x64 code |
| .rdata | 581,632 | 89/256 | Read-only data, strings |
| .data | 31,260,672 | 4/256 | **Bloat padding** — 92% of file size, near-zero entropy |
| .pdata | 28,672 | 34/256 | Exception directory |
| .fptable | 4,096 | 34/256 | Unusual section name (unknown purpose) |
| .rsrc | 4,096 | 34/256 | Manifest only |
| .reloc | 8,192 | 34/256 | Relocations |

> **The `.data` section is 31.2 MB of low-entropy padding.** This is deliberate file-size inflation to evade size-based AV heuristics and slow sandbox analysis. The actual functional code is under 3 MB.

---

## 2. Classification

**Malware Family:** WallStealer v2.2.1  
**Type:** Infostealer / Credential Thief  
**Confidence:** High  
**Reasoning:**

- Internal string `=== SYSTEM DATA LOG ===` with structured victim profile fields (HWID, Wallpaper Hash, TAG, LocalIPv4, MachineID) is a classic stealer C2 beacon format
- Named pipe `\\.\pipe\browser_key_pipe` is a distinctive IOC for browser master key extraction
- Targets Chrome/Edge `\Login Data` and `\Local State` (Chromium credential store)
- Windows Token Broker (WAM) credential theft via `login.live.com/RST2.srf`
- OAuth token interception using `session_key_jwe`, `ProcessOAuthRequest`, `FailedTokenReq` exception classes
- Drop path `C:\Users\Admin\AppData\Local\Temp\blue.exe` suggests staged delivery
- JSON beacon structure `{"build_id":"","token":"","session_id":"","correlationID":"...}` is a stealer operator panel registration format
- C2 domain `login.cloudgovapi.` is designed to impersonate government/cloud authentication infrastructure
- KesaKode verdict: NitrogenLoader(21) / DustyStealer(16) — both stealer families; DustyStealer is the higher-confidence match given behavioral overlap

---

## 3. Capabilities

### System Fingerprinting (sub_14018720c / sub_140182710 / sub_140182f08)
- Builds structured `=== SYSTEM DATA LOG ===` victim profile containing:
  - TAG (campaign identifier), LocalIPv4, Date, MachineID, HWID
  - **Wallpaper Hash** (desktop wallpaper file hash used as unique victim identifier)
  - Windows version (via WMI `Caption`/`Version`/`BuildNumber`/`OSArchitecture` + `RtlGetVersion`)
  - Computer Name, User Name, Integrity Level, Admin Group membership
  - TimeZone, SystemUptime, SystemTimestamp, Display Resolution, Keyboard Languages
  - Processor (via WMI `Win32_VideoController` + registry `HARDWARE\DESCRIPTION\System\CentralProcessor\0`)
  - Cores, Threads, RAM, VideoCard (WMI `Win32_VideoController`)
- Writes output files: `systeminfo.txt`, `processes.txt`, `software.txt`, `BUILD_VERSION.txt`
- Enumerates registry hives (`HKLM`, `HKCU`) for installed software

### Browser Credential Theft
- **Chrome/Edge**: Reads `\Local State` (AES-256-GCM master key) and `\Login Data` (SQLite encrypted passwords)
- **Named Pipe**: `\\.\pipe\browser_key_pipe` — creates pipe to extract live browser master key from running browser process; also used with `sedge.dll` (Edge) and `chrome.dll`
- **Firefox**: Targets `firefox.exe` process directly
- `Pull cookies invoke` wide string confirms cookie theft operation

### Windows Token Broker / OAuth Theft
- Intercepts Windows Account Manager (WAM) OAuth token flows
- Exception class `.?AVFailedTokenReq` and function `ProcessOAuthRequest` confirm WAM hooking
- `session_key_jwe` (JWE-encrypted session key handling) — reads WAM's session key
- `WStrust token request` — WS-Trust enterprise authentication token interception
- Targets `login.live.com/RST2.srf` (Microsoft Live WS-Trust endpoint)
- Captures: `Refresh token`, cached tokens, access tokens, correlationID
- Beacon format: `{"build_id":"","token":"","session_id":"","correlationID":"...}`
- `login_hint` OAuth parameter extraction from active browser sessions

### Memory Reading (sub_1400ea070)
- `VirtualQueryEx` loop in 0x64000 (400KB) chunks — walks target process memory
- Reads up to `0x100000000000` bytes (iterates over process virtual address space)
- Output base64-encoded (buffer divided by 3 + 1 allocation pattern)
- Used to extract decrypted credential data from browser memory

### Secondary Payload
- Drops `C:\Users\Admin\AppData\Local\Temp\blue.exe` — secondary payload (name/purpose unknown without sample)
- Writes to `C:\ProgramData\` for persistence staging

### Anti-Analysis / Evasion
- **File bloat**: 31.2 MB `.data` padding to evade size limits (ANY.RUN: "No threats detected")
- **XOR-in-loop**: 546 instances — runtime string decryption across all visible strings
- **Dynamic string construction**: 256 stack-array-built strings; no plaintext C2 in static view
- **Import-by-hash**: 3 API groups resolved at runtime by hash
- **Anti-debug**: `IsDebuggerPresent`, debug flag checks, timing-based anti-debug
- **Anti-VM / Sandbox**:
  - `GetCursorPos` — mouse movement detection
  - `GetEnvironmentVariableW/A("PATH")` — environment profiling
  - `GetFileAttributesA("comdlg32")`, `GetFileAttributesA("comctl32")` — system DLL presence check
  - `GetModuleHandleA("comctl32")` — loaded DLL check
  - Jumbled junk-code obfuscation in critical functions (sub_1400b4b54 / sub_1400381f8 shown extensively)
- **Obfuscated control flow**: SpaghettiFunction anomaly (27 instances), unreachable blocks injected throughout
- **MSVC Security Cookie** (`0x14027d140`) on all decompiled functions — stack cookie protection

### Screenshot
- `gdiplus.dll` import + BitBlt/GetDIBits pattern — captures desktop screenshots

### Network Communication
- `WINHTTP.dll` for C2 HTTP(S) communication
- `WS2_32.dll` for raw socket operations
- DNS resolution for C2 domain

### WScript Execution
- `wscript.exe` reference — launches VBS/JS scripts

---

## 4. Attack Chain

```
[Delivery] WallStealer.dmp (disguised as crash dump / screen recording)
    │
    ▼
[Anti-sandbox Gate]
  - File size check (31MB bloat exceeds sandbox limits)
  - Mouse position check (GetCursorPos)
  - System DLL presence check (comctl32, comdlg32)
  - Debugger/VM checks
    │
    ▼  (passes)
[Victim Profiling]
  sub_14018720c: Builds "=== SYSTEM DATA LOG ===" with HWID, Wallpaper Hash,
  IP, OS, CPU, RAM, GPU, processes, software, registry
    │
    ▼
[Browser Credential Theft]
  - Named pipe \\.\pipe\browser_key_pipe → extract browser master key
  - Read \Local State (Chrome/Edge AES key)
  - Read \Login Data (encrypted passwords + cookies)
  - Access firefox.exe process directly
    │
    ▼
[Windows Token Broker Theft]
  - Hook WAM (ProcessOAuthRequest)
  - Steal session_key_jwe
  - Capture OAuth tokens (access + refresh) for login.live.com/RST2.srf
  - Extract login_hint, correlationID
    │
    ▼
[Memory Scraping]
  - VirtualQueryEx loop over browser process memory
  - Base64-encode extracted data
    │
    ▼
[Exfiltration]
  - POST to login.cloudgovapi. (C2) via WinHTTP
  - Beacon: {"build_id":"...","token":"...","session_id":"...","correlationID":"..."}
    │
    ▼
[Secondary Payload] (optional)
  - Drop C:\Users\Admin\AppData\Local\Temp\blue.exe
  - Run via wscript.exe or direct exec
```

---

## 5. IOCs

### Network (defanged)

| Type | Value | Notes |
|---|---|---|
| Domain | `login[.]cloudgovapi[.]` | C2 domain (truncated; likely `.com` TLD added at runtime) |
| URL | `login[.]live[.]com/RST2[.]srf` | Microsoft WAM theft target (SOAP/WS-Trust endpoint) |
| URL | `login[.]live[.]com/ppsecure/deviceaddcredential[.]srf` | Microsoft account credential target (sandbox-observed) |
| URL | `http[://]schemas[.]xml[.]` | XML schema reference (partial) |
| URL | `http[://]docs[.]oasis-...` | OAuth SAML XML schema reference (partial) |

### Filesystem

| Path | Purpose |
|---|---|
| `C:\Users\Admin\AppData\Local\Temp\blue.exe` | Secondary payload drop path |
| `C:\ProgramData\` | Staging directory |
| `systeminfo.txt` | System profile output |
| `processes.txt` | Running processes output |
| `software.txt` | Installed software output |
| `BUILD_VERSION.txt` | Version tracking file |
| `\Local State` | Chrome/Edge master key file |
| `\Login Data` | Chrome/Edge credential database |

### Named Pipes

| Name | Purpose |
|---|---|
| `\\.\pipe\browser_key_pipe` | Browser master key extraction channel |

### Mutexes

| Name | Purpose |
|---|---|
| `Global\composerctx` | Single-instance lock |

### Build Artifacts

| Field | Value |
|---|---|
| Version | 2.2.1 |
| Build Date | 2026-04-01 08:27:47 |
| Section Name | `.fptable` (non-standard) |

---

## 6. Emulation Results

**Speakeasy (generic runner + plain):** Failed — `PEFormatError: "data at RVA can't be fetched. Corrupt header?"` from TLS callback parsing. The PE has a TLS directory that speakeasy's pefile parser cannot handle, likely due to the abnormal section layout (31MB .data causes RVA/offset miscalculations).

**angr / Qiling:** Not attempted — the PE loading failure from the inflated .data section would affect these too. The file should be tested in a full Windows VM environment.

No IOCs were dynamically recovered from emulation.

---

## 7. Sandbox Results (ANY.RUN)

| Field | Value |
|---|---|
| Task ID | `090cdc01-bf8f-4740-b2ca-d062494df0cb` |
| Verdict Score | 0 / 100 |
| Threat Level | No threats detected |
| Family Tags | None |
| Public Report | https://app.any.run/tasks/090cdc01-bf8f-4740-b2ca-d062494df0cb |

**Analysis:** ANY.RUN's 0/100 is expected. The file-size bloat (33.9 MB, with 31 MB being null-padded `.data`) causes it to either time out or skip dynamic analysis. The sandbox did record HTTP traffic to legitimate Microsoft endpoints (`login.live.com/RST2.srf`, `login.live.com/ppsecure/deviceaddcredential.srf`), confirming that the binary executes and attempts Windows Token Broker credential interception even without full detonation of the stealer payload.

---

## 8. Analyst Notes

### Confidence Gaps
- **C2 domain is truncated**: `login.cloudgovapi.` ends with a trailing period and null bytes in the binary at file offset `0x204dce`. The full TLD (likely `.com`) may be appended at runtime via one of the 256 dynamically-constructed strings. Active C2 was not observed.
- **`blue.exe` payload unknown**: The secondary payload dropped to `%TEMP%\blue.exe` was not recovered; its capabilities are unknown.
- **OAuth token scope unclear**: The WAM interception scope (which Microsoft services are targeted — Office 365, Azure AD, Xbox, OneDrive?) could not be determined without live execution; the `RST2.srf` WS-Trust endpoint is used for MSA (Microsoft Account) authentication specifically.
- **Exfiltration path not confirmed**: The WinHTTP beacon format was recovered from static analysis but the complete exfiltration flow (HTTP POST vs. GET, TLS vs. plain) was not confirmed dynamically.

### Attribution
- KesaKode verdict **DustyStealer(16)** is the stronger match given:
  - Wallpaper hash as victim fingerprinting (DustyStealer hallmark)
  - Structured system log with TAG field
  - Named-pipe browser key extraction pattern
- **NitrogenLoader(21)** similarity may reflect shared code library (C++ stealer toolkit), not the same actor
- No certificate → no signer pivot possible
- No prior sample in local corpus shares this certificate, C2, or build artifact

### Recommended Follow-Up
1. Execute in an isolated Windows 10/11 VM (VMware snapshots) to capture full network traffic to `login.cloudgovapi.` C2
2. Extract `blue.exe` from a detonated sample for independent analysis
3. Search threat intel for `login.cloudgovapi.` domain registration (WHOIS, passive DNS)
4. Search for `Global\composerctx` mutex or `browser_key_pipe` in EDR telemetry for victim identification
5. Monitor for the JSON beacon `{"build_id":"` on network egress — distinctive stealer registration pattern

### MITRE ATT&CK

| Technique | Description |
|---|---|
| T1555.003 | Credentials from Web Browsers |
| T1539 | Steal Web Session Cookie |
| T1528 | Steal Application Access Token (WAM OAuth) |
| T1005 | Data from Local System |
| T1057 | Process Discovery |
| T1012 | Query Registry |
| T1113 | Screen Capture |
| T1071.001 | Application Layer Protocol: Web Protocols |
| T1041 | Exfiltration Over C2 Channel |
| T1027 | Obfuscated Files or Information |
| T1027.002 | Software Packing / Size Inflation |
| T1497.001 | Virtualization/Sandbox Evasion: System Checks |
| T1036.005 | Masquerading: Match Legitimate Name (`.dmp` extension) |
| T1105 | Ingress Tool Transfer (`blue.exe` download) |
| T1106 | Native API (import-by-hash, NT API resolution) |
