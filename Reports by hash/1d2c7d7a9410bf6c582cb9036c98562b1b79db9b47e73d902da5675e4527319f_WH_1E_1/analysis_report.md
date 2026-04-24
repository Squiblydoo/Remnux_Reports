# Malware Analysis Report: WH_1E_1.exe
**Analyst:** REMnux Automated Analysis  
**Date:** 2026-04-24  
**Sample:** `WH_1E_1.exe`

---

## 1. File Metadata

| Field | Value |
|---|---|
| **Filename** | WH_1E_1.exe |
| **SHA256** | `1d2c7d7a9410bf6c582cb9036c98562b1b79db9b47e73d902da5675e4527319f` |
| **MD5** | `b06f00b1d9422e94a2b007b857f084a5` |
| **SHA1** | `614be44191be9bdb04790337937f6d8114d2a362` |
| **Size** | 3,660,912 bytes (3.49 MB) |
| **Type** | PE32 executable (GUI) Intel 80386, 11 sections |
| **Architecture** | x86 |
| **Compiler** | Borland Delphi (TurboLinker) |
| **Delphi Project** | `7QuCNSwF` |
| **Export Module** | `Bind_EXE.exe` |
| **Certificate Issuer** | GlobalSign GCC R45 EV CodeSigning CA 2020 |
| **Certificate Subject** | 威海市明骏信息科技有限公司 (Weihai Mingjun Information Technology Co., Ltd.) |
| **Cert Location** | Weihai, Shandong, CN |
| **Cert Serial** | `10c33f009b54f66a849b4c90` |
| **Cert Validity** | 2026-04-08 → 2027-04-09 (1-year EV) |
| **VersionInfo** | All fields blank (FileVersion 1.0.0.0) |
| **Build Timestamp** | 2026-04-24 05:17:52 |

---

## 2. Classification

**Family:** Bind_EXE Dropper Toolkit  
**Confidence:** HIGH  
**Role:** Multi-payload dropper with DLL sideloading delivery  

This sample is a **configurable file binder/dropper** produced by a custom Delphi toolkit branded "Bind_EXE". It bundles 6 ZLIB-compressed payload blobs in the `.text` section, uses an XOR-0x09-encoded UTF-16LE API obfuscation scheme, performs anti-VM and anti-debug checks, and delivers a DLL sideloading attack chain using the legitimate `IRScriptEditor.exe` as a host process.

**Cross-reference to 6.exe (analyzed 2026-04-06):** Confirmed identical Bind_EXE toolkit via four independent matching indicators: identical export module names (`Bind_EXE.exe`, `Bind_DLL.dll`, `TRUST_IMITATE_DLL.dll`, `IRScriptEditor.exe` sideload host), identical FBAE hidden-MZ signature for DLLs, identical mutex config pattern (`Global\[a-ex]_`), and same EV certificate serial (`10c33f009b54f66a849b4c90`) shared between the outer dropper and the inner `TRUST_IMITATE_DLL.dll` payload. The threat actor now uses a new signing entity (Weihai Mingjun vs. prior Xiamen Jiaming certificate).

---

## 3. Capabilities

- **Anti-VM detection:** Enumerates processes via `WTSEnumerateProcessesW`; kills execution if `VMTOOLS.EXE`, `VMACTHLP.exe`, or `VBoxService` are running
- **Anti-debug:** GetTickCount timing delay check (capa-confirmed); `NtQueryInformationProcess` (in Bind_DLL.dll)
- **Dynamic API resolution:** All Windows API names stored as XOR-0x09-encoded UTF-16LE; resolved at runtime by parsing PE export tables — no visible imports for malicious functions
- **Encrypted payload storage:** 6 ZLIB blobs embedded in `.text` section; some payloads XOR-0x10-encoded inside ZLIB stream; DLLs use custom FBAE hidden-MZ signature instead of "MZ"
- **RC4 encryption:** capa-detected RC4 PRGA (likely for additional payload decryption layer beyond ZLIB+XOR)
- **Config-driven conditional execution:** Reads config from blob1 (267KB sparse binary); config blocks tagged `SSSSCfg`/`HHHHCfg`; runtime conditions: `[IF_PROCESS]`, `[IF_FILE]`, `[IF_MUTEX]`, `[IF_SERVICE]`, `[IF_SERVICE_RUNING]` (misspelled)
- **DLL sideloading:** Drops legitimate `IRScriptEditor.exe` (Indigo Rose Script Editor 2.0.1002.0) alongside malicious `TRUST_IMITATE_DLL.dll`; IRScriptEditor.exe loads the DLL via normal search-order hijacking
- **Mutex-based single-instance:** Checks `Global\a_`, `Global\b_`, `Global\c_`, `Global\d_`, `Global\e_`, `Global\x_`
- **Registry access:** `RegOpenKeyExW`, `RegQueryValueExW`, `RegCreateKeyExW` (persistence)
- **System fingerprinting:** `GetSystemInfo`, `GetVersionExW`, `GlobalMemoryStatus`, `GetDiskFreeSpaceW`, `GetSystemDefaultLCID`, `IsWow64Process`, `GetNativeSystemInfo`
- **Process enumeration:** `WTSEnumerateProcessesW`, `GetProcessImageFileNameW`, `WTSFreeMemory`
- **Network enumeration:** `GetExtendedTcpTable`, `GetExtendedUdpTable` (all active TCP/UDP connections)
- **Filesystem enumeration:** `FindNextFileW`, `SHGetSpecialFolderPathW` (AppData, Temp, Desktop, etc.), `PathFileExistsW`, `.lnk` file access
- **WMI access:** COM automation via `WbemScripting.SWbemLocator` (process/system queries)
- **Privilege inspection:** `OpenProcessToken`, `GetTokenInformation`
- **File mapping / potential injection:** `CreateFileMappingW`
- **PDF lure:** Drops a 77KB PDF document to distract victim during installation
- **Sandbox evasion:** ANY.RUN scored 0 ("No threats detected") — anti-VM + GetTickCount timing check effective against automated sandboxes

---

## 4. Attack Chain

```
WH_1E_1.exe (outer dropper, Bind_EXE, EV-signed)
│
├─ [Check] Anti-VM: WTSEnumerateProcessesW → exit if VMTOOLS/VMACTHLP/VBoxService found
├─ [Check] Anti-debug: GetTickCount timing
├─ [Read] Config blob (ZLIB blob1, 267KB) → parse SSSSCfg/HHHHCfg conditional rules
│
├─ [Decompress] ZLIB blob2 → Bind_DLL.dll (FBAE→MZ DLL, 295KB)
├─ [Decompress] ZLIB blob3 → multi-file container:
│   ├─ XOR-0x10 secondary dropper PE (279KB Delphi)
│   ├─ FBAE DLL (102KB, raw)
│   └─ Delphi MZP PE (1.4MB)
├─ [Decompress] ZLIB blob4 → PDF lure (77KB) + FBAE DLL (363KB)
├─ [Decompress] ZLIB blob5 → IRScriptEditor.exe (2.67MB, signed sideload host)
├─ [Decompress] ZLIB blob6 → TRUST_IMITATE_DLL.dll (460KB, EV-signed Delphi dropper)
│
├─ [Drop] IRScriptEditor.exe (legitimate, expired COMODO cert) to working directory
├─ [Drop] TRUST_IMITATE_DLL.dll (malicious, same GlobalSign cert) to same directory
├─ [Show] PDF lure to victim
├─ [Execute] IRScriptEditor.exe
│   └─ [Sideload] TRUST_IMITATE_DLL.dll (DLL search-order hijack)
│       └─ [Load] Bind_DLL.dll (stealer/RAT component)
│           ├─ Process enumeration (WMI + WTS)
│           ├─ Network connection snapshot (TCP + UDP tables)
│           ├─ Filesystem enumeration + .lnk file access
│           ├─ Special folder enumeration (AppData, Temp, Desktop)
│           ├─ Registry persistence (RegCreateKeyExW)
│           ├─ Privilege/token inspection
│           └─ C2 beacon (URL obfuscated, not recovered statically)
```

---

## 5. Embedded Payloads

| Blob | Compressed | Decompressed | Content | SHA256 (raw) |
|---|---|---|---|---|
| blob1 | 3,737 B | 267,152 B | Config binary (sparse, possibly XOR-0x10) | `e16774be...` |
| blob2 | 128,629 B | 295,424 B | **Bind_DLL.dll** (FBAE hidden-MZ DLL, stealer) | `d29421f5...` |
| blob3 | 733,532 B | 1,682,444 B | Multi-file container (XOR-0x10 PE + FBAE DLL + MZP PE) | `6ce190a2...` |
| blob4 | 357,104 B | 440,206 B | **PDF lure** (77KB) + FBAE DLL (363KB) | `78b84f97...` |
| blob5 | 1,241,331 B | 2,674,416 B | **IRScriptEditor.exe** (sideload host, legitimate) | `f7895b99...` |
| blob6 | 201,865 B | 460,912 B | **TRUST_IMITATE_DLL.dll** (EV-signed dropper) | `7494a673...` |

**Key extracted payload hashes:**

| File | SHA256 |
|---|---|
| Bind_DLL.dll (blob2, MZ-fixed) | `95dfd066add41616e62f9feb5894d44803b23a4f781d86a60ba0c179bfc4f254` |
| IRScriptEditor.exe (blob5) | `e6b4d9bab8d123b0bf248dada3013d5de3a033f94f2b5144268125157036d9db` |
| TRUST_IMITATE_DLL.dll (blob6) | `734a83fa8950e4e8191683f1427721a11b3d2bf11c0c460f12a83f8aa15f1b62` |
| PDF lure | `5953463740adfd3dcdf2e3e5e7f2d5dd3e856b24122392d3a0c0b0e33d2fd689` |
| blob3 XOR-0x10 PE | `9248ce08af12b2d7e1abd14d0ef82e54b324df4d3b125492e309ab952099ca8e` |
| blob3 MZP EXE | `51fab79fa1f1823770452a4e9d4c2a57472c6ea8e8e6fc8425af6046afe802cc` |

---

## 6. IOCs

### Network
*C2 URL not statically recovered* — URL is XOR-obfuscated inside Bind_DLL.dll (known from 6.exe to use WinHTTP; same toolkit). No C2 DNS/IP observed in speakeasy emulation or ANY.RUN sandbox (evasion successful).

### Signing Infrastructure
| Cert Serial | Subject | Issuer | Valid |
|---|---|---|---|
| `10c33f009b54f66a849b4c90` | 威海市明骏信息科技有限公司 | GlobalSign GCC R45 EV CodeSigning CA 2020 | 2026-04-08→2027-04-09 |

### Filesystem (dropped payloads)
- `IRScriptEditor.exe` — legitimate sideload host
- `TRUST_IMITATE_DLL.dll` — malicious DLL (sideloaded)
- `Bind_DLL.dll` — stealer/RAT DLL (FBAE hidden-MZ)
- PDF lure document
- Special folders targeted: `%APPDATA%`, `%TEMP%`, `%DESKTOP%` (via `SHGetSpecialFolderPathW`)

### Mutexes (config-driven, runtime-created)
- `Global\a_`
- `Global\b_`
- `Global\c_`
- `Global\d_`
- `Global\e_`
- `Global\x_`

### Registry
- `HKCU\Software\Embarcadero\Locales` (Delphi locale init — benign)
- `HKCU\Software\Borland\Locales` (Delphi locale init — benign)
- Additional registry keys written by `Bind_DLL.dll` via `RegCreateKeyExW` (runtime, keys not recovered statically)

### Anti-VM Process Names (kills execution if found)
- `VMTOOLS.EXE` (VMware Tools)
- `VMACTHLP.exe` (VMware Activation Helper)
- `VBoxService` (VirtualBox Guest Service)

---

## 7. Emulation Results

**Speakeasy (generic runner):** Emulation halted immediately — only VirtualAlloc called (1.3MB alloc); the Delphi RTL UI language setup hit an unsupported API (`GetThreadPreferredUILanguages`), aborting emulation before payload execution logic was reached. No IOCs captured.

**Speakeasy (plain, x86):** Traced through Delphi initialization: `GetModuleHandleW`, `SetThreadLocale`, `InitializeCriticalSection`, `GetVersion`, `GetProcAddress` for `GetThreadPreferredUILanguages`/`SetThreadPreferredUILanguages`/`GetThreadUILanguage`, then RegOpenKeyExW×6 (Delphi locale registry paths), `GetUserDefaultUILanguage`, `IsValidLocale`, then stopped on unsupported API. Anti-VM logic not reached within emulated trace.

---

## 8. Sandbox Results

**ANY.RUN (public, Ally tier):**  
- **Task ID:** `b36879d2-4744-4c3f-8551-436242eeffcc`  
- **Score:** 0/100  
- **Verdict:** No threats detected  
- **Tags:** (none)  
- **URL:** https://app.any.run/tasks/b36879d2-4744-4c3f-8551-436242eeffcc  
- **Assessment:** Anti-VM (VMTOOLS/VMACTHLP/VBoxService check) and GetTickCount anti-debug are highly effective against automated sandboxes. All 16 observed network events were Windows OS certificate validation (CRL/OCSP to microsoft.com / digicert.com) — no C2 contact recorded.

---

## 9. String Obfuscation Decoding

All Windows API names and DLL names are stored as XOR-0x09-encoded UTF-16LE strings. Decoded API calls (outer dropper + TRUST_IMITATE_DLL.dll):

**System/Fingerprinting:** `CloseHandle`, `GetSystemInfo`, `GetVersionExW`, `GlobalMemoryStatus`, `GetDiskFreeSpaceW`, `GetSystemDefaultLCID`, `GetNativeSystemInfo`, `IsWow64Process`

**Process Enumeration:** `WTSEnumerateProcessesW`, `WTSFreeMemory`, `GetModuleHandleExA`, `GetCurrentProcess`, `GetModuleFileNameW`

**Resource/Payload Loading:** `FindResourceW`, `SizeofResource`, `LockResource`, `CreateFileMappingW`

**Registry:** `RegOpenKeyExW`, `RegOpenKeyW`, `RegQueryValueExW`, `RegCloseKey`

**Security/Privilege:** `OpenSCManagerW` (TRUST_IMITATE_DLL.dll)

**Memory:** `HeapAlloc`, `HeapFree`, `IsBadReadPtr`

**DLL Names (encoded):** `kernel32.DLL`, `oleaut32.dll`, `ole32.dll`

Additional decoded API calls in **Bind_DLL.dll:**

| API | Purpose |
|---|---|
| `SHGetSpecialFolderPathW` | Special folder path discovery |
| `GetExtendedTcpTable` | Enumerate TCP connections |
| `GetExtendedUdpTable` | Enumerate UDP connections |
| `NtQueryInformationProcess` | Anti-debug / process info |
| `WbemScripting.SWbemLocator` | WMI queries via COM |
| `GetTokenInformation` | Token privilege inspection |
| `OpenProcessToken` | Process token access |
| `FindNextFileW` | Filesystem enumeration |
| `RegCreateKeyExW` | Registry key creation (persistence) |
| `GetProcessImageFileNameW` | Process image name enumeration |
| `SHGetSpecialFolderPathW` | AppData / Desktop / Temp paths |

---

## 10. MITRE ATT&CK

| Technique | ID | Notes |
|---|---|---|
| Obfuscated Files or Information | T1027 | XOR-0x09 UTF-16LE API name encoding |
| Obfuscated Files or Information: Software Packing | T1027.002 | ZLIB + XOR-0x10 payload storage |
| Masquerading: Invalid Code Signature | T1036.001 | EV cert on dropper and payload for legitimacy |
| Subvert Trust Controls: Code Signing | T1553.002 | GlobalSign EV 1-year cert |
| Hijack Execution Flow: DLL Search Order Hijacking | T1574.001 | TRUST_IMITATE_DLL.dll sideloaded by IRScriptEditor.exe |
| System Information Discovery | T1082 | GetSystemInfo, GlobalMemoryStatus, IsWow64Process |
| Virtualization/Sandbox Evasion | T1497 | WTS process scan for VMTOOLS/VMACTHLP/VBoxService |
| Debugger Evasion | T1622 | GetTickCount timing; NtQueryInformationProcess |
| Query Registry | T1012 | RegOpenKeyExW, RegQueryValueExW |
| Modify Registry | T1112 | RegCreateKeyExW (persistence) |
| File and Directory Discovery | T1083 | FindNextFileW, .lnk file targeting |
| Process Discovery | T1057 | WTSEnumerateProcessesW, GetProcessImageFileNameW |
| System Network Connections Discovery | T1049 | GetExtendedTcpTable, GetExtendedUdpTable |
| WMI | T1047 | WbemScripting.SWbemLocator via COM |
| Command and Scripting Interpreter | T1059 | WMI automation |
| Ingress Tool Transfer | T1105 | Drops IRScriptEditor.exe, DLLs, PDF lure |
| User Execution: Malicious File | T1204.002 | PDF lure (77KB) presented to victim |
| Shared Modules | T1129 | Runtime dynamic API loading |

---

## 11. Analyst Notes

**Residual gaps:**
- **C2 infrastructure not recovered**: The C2 URL(s) used by `Bind_DLL.dll` are XOR-obfuscated. Decryption requires dynamic execution on a real Windows host with anti-VM bypassed. The identical toolkit in 6.exe used a WinHTTP C2 that was also XOR-obfuscated.
- **Blob3 internal structure**: A multi-file container with 3 embedded files — one XOR-0x10 secondary dropper (279KB Delphi PE), one FBAE DLL (102KB, rough extraction), and one large Delphi MZP executable (1.4MB, SHA256: `51fab79f...`). The large MZP PE's identity is not fully determined — likely another lure application or another stage of the dropper.
- **Blob4 PDF lure content**: The PDF uses compressed content streams (PDF 1.5 binary objects); actual displayed content not extracted without full PDF parsing. The embedded FBAE DLL (363KB) appended after the PDF stream is another Bind toolkit payload.
- **Config blob (blob1)**: The 267KB decompressed binary contains sparse data (mostly 0x10-byte fill) with config values starting at offset 8. The structure resembles XOR-0x10-encoded Delphi string records. Config values observed include what appears to be a 8-char campaign/instance ID (`D1EABD32` after XOR-0x10 decoding at offset 8-22).
- **`$*@@@*$@@@$ *@@*..@*- $@($ *)(* $)`**: Unresolved literal string present in both outer dropper and TRUST_IMITATE_DLL.dll. Purpose unknown — likely an internal status/logging format or debug artifact.

**Alternative hypotheses:**
- The TRUST_IMITATE_DLL.dll payload (blob6) is signed with the same EV cert as the outer dropper; both may have been compiled in the same build session on 2026-04-24, suggesting the threat actor has direct access to the signing infrastructure (not stolen cert).
- The Weihai Mingjun signing entity represents a new shell company established for this campaign after the prior Xiamen Jiaming entity was presumably revoked or expired.

**Recommended follow-up:**
1. Execute sample on isolated physical machine with network capture to recover C2 URL from Bind_DLL.dll at runtime
2. Full PDF parsing of the lure document to identify social engineering theme
3. Search threat intel for SHA256 `734a83fa...` (TRUST_IMITATE_DLL.dll) and `95dfd066...` (Bind_DLL.dll) to determine if previously observed
4. Monitor for cert serial `10c33f009b54f66a849b4c90` revocation status — report to GlobalSign CA as fraudulent EV cert
5. Analyze blob3's 1.4MB `MZP` executable (SHA256: `51fab79f...`) to identify its role in the chain
