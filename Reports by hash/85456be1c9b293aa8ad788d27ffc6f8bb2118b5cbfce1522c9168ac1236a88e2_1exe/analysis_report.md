# Malware Analysis Report: 1.exe (Bind_EXE Dropper v5)

**Date**: 2026-05-14  
**Analyst**: REMnux/Claude  
**Sample path**: `/home/remnux/mal/1.exe`

---

## 1. File Metadata

| Field | Value |
|-------|-------|
| **Filename** | 1.exe |
| **SHA256** | `85456be1c9b293aa8ad788d27ffc6f8bb2118b5cbfce1522c9168ac1236a88e2` |
| **SHA1** | `1f363d96b8d98ad8026e263386c86e9ba89a4ccd` |
| **MD5** | `8e80425c0902ce95f3a574244aed7a5b` |
| **File type** | PE32 x86 GUI executable, 11 sections |
| **Size** | 8,721,920 bytes (8.32 MB) |
| **Build timestamp** | 2026-05-14 10:05:20 (same-day build) |
| **Compiler** | Delphi / TurboLinker |
| **Delphi project name** | `a8UsbvkT` (random-looking name) |
| **Export module name** | `Bind_EXE.exe` |
| **VersionInfo** | 1.0.0.0 |
| **imphash** | `dddd95789c6d117404c7c3c56ab141f3` |

### Code-Signing Certificate

| Field | Value |
|-------|-------|
| **Issuer** | Sectigo Public Code Signing CA EV R36 |
| **Subject** | Gansu Shishida Information Technology Co., Ltd. |
| **Country/Province** | CN / Gansu Sheng |
| **Validity** | 2026-04-27 → 2027-04-27 (1-year EV cert) |
| **Serial** | `578b8a96c9a5126336695edf73fc3f51` |

---

## 2. Classification

**Family**: Bind_EXE Dropper (v5 / new iteration)  
**Confidence**: **High**  
**Type**: Multi-stage Delphi dropper — compresses and delivers a DLL stealer, a sideload host, and a signed second-stage loader.

This sample is a new iteration of the Bind_EXE toolkit, a Delphi-compiled dropper family that has appeared consistently in this workspace (prior samples `1.exe` SHA256:`df9fefad...`, `WH_1E_1.exe` SHA256:`1d2c7d7a...`, `6.exe`). This version uses a fresh Sectigo EV signing certificate from a different Chinese company and an updated XOR key (`0x0A` with byte-reversal), but the internal structure, payload names, API obfuscation scheme, and C2 path identifiers are identical to prior versions.

---

## 3. Capabilities

- **DLL sideloading** — drops `IRScriptEditor.exe` (Indigo Rose Script Editor) as a benign-looking host, then plants `TRUST_IMITATE_DLL.dll` alongside it for sideload execution
- **Process injection** — uses `NtUnmapViewOfSection` (Process Hollowing / UnmapViewOfSection injection)
- **Process enumeration / anti-VM** — `WTSEnumerateProcessesW` to enumerate running processes; terminates in VM environments
- **Privilege escalation** — acquires `SeDebugPrivilege` and `SeShutdownPrivilege` via `AdjustTokenPrivileges`
- **Credential theft** — calls `CryptUnprotectData` (crypt32.dll) to decrypt DPAPI-protected credentials
- **Service enumeration** — `OpenSCManagerW` + `EnumServicesStatusExW`
- **Anti-debugging** — `GetTickCount`-based timing checks; LOCK/UNLOCK anti-disassembly gadgets in TRUST_IMITATE_DLL.dll
- **Evasion via valid EV cert** — Sectigo Extended Validation code-signing certificate bypasses SmartScreen
- **ZLIB decompression** — uses `RtlDecompressBuffer` / `RtlDecompressBufferEx` via ntdll.dll for payload extraction
- **Mutex-based single-instance guard** — `CreateMutexW` / `OpenMutexW` under `Global\` prefix
- **Registry persistence** — `SOFTWARE\SystemCore_` registry key (exact path constructed at runtime)
- **File operations** — `ShellExecuteW`, `DeleteFileW`, `MoveFileW`, `CreateFileW`, `ReadFile`, `WriteFile`
- **C2 communication** — HTTP via paths `/aax`, `/11`, `/5`; hostname runtime-resolved from encrypted configuration
- **API name obfuscation** — all imported API names stored XOR-0x09 encoded as UTF-16LE (same scheme as prior Bind_EXE samples)

---

## 4. Attack Chain

```
1.exe (signed dropper)
├── Decompresses 4 ZLIB blobs from .text section
├── Blob 1: Core_LOAD_DLL.dll  ← 4-byte header + XOR-0x10 encoded, stealer DLL
├── Blob 2: PDF lure            ← Decoy document displayed to victim
├── Blob 3: IRScriptEditor.exe  ← Legitimate Indigo Rose MFC app (sideload host)
└── Blob 4: TRUST_IMITATE_DLL.dll ← Delphi loader signed with SAME EV cert as dropper

Execution flow:
  1. Dropper decompresses all blobs into temp/working directory
  2. Opens PDF lure to distract victim
  3. Drops IRScriptEditor.exe + TRUST_IMITATE_DLL.dll side-by-side
  4. Executes IRScriptEditor.exe → sideloads TRUST_IMITATE_DLL.dll
  5. TRUST_IMITATE_DLL.dll loads/injects Core_LOAD_DLL.dll
  6. Core_LOAD_DLL.dll: privilege escalation → anti-VM → C2 registration
                       → credential theft (CryptUnprotectData)
                       → process injection / payload execution
```

### XOR decryption scheme

| Layer | Encoding | Target |
|-------|----------|--------|
| Blob 1 (Core_LOAD_DLL.dll) | 4-byte header + XOR-0x10 | PE payload |
| Blobs 2–4 (PDF, IRScript, TRUST) | Raw ZLIB, no XOR | Decompressed as-is |
| API names in Core_LOAD_DLL.dll | XOR-0x09 UTF-16LE | Import resolution |
| Main dropper API obfuscation | XOR-0x0A + byte-reversal (`sub_ab1fc8`) | String/API name transform |

---

## 5. Extracted Payloads

| Blob | Name | Compressed | Decompressed | SHA256 |
|------|------|------------|--------------|--------|
| 1 | `Core_LOAD_DLL.dll` | 736,646 B | 1,699,848 B | `4923391e869f3c4fc7eaadf223663225896867b64594c3678cffdc6068895e7d` |
| 2 | PDF lure | 357,103 B | 440,206 B | `4b6c70cb7349a36818972b40aad1ef14c72f7ad61a602bd37f31544fe322b017` |
| 3 | `IRScriptEditor.exe` | 1,241,331 B | 2,674,416 B | `c3cf4754cc2d17a876175f8011be7c874c86ed73c0f902d9e12216bae73e60b7` |
| 4 | `TRUST_IMITATE_DLL.dll` | 1,640,292 B | 3,449,344 B | `ed9d14377f89c918d250d5b8324ce85231ae92bc41dec334f6e331cbf50d92c7` |

**Core_LOAD_DLL.dll** — build timestamp: 2026-05-14 07:39:33 (fresh build, same day as dropper)  
**TRUST_IMITATE_DLL.dll** — signed with **same Sectigo EV cert** as the outer dropper (serial `578b8a96c9a5126336695edf73fc3f51`)

---

## 6. IOCs

### Network (defanged)

| Type | IOC | Source | Notes |
|------|-----|--------|-------|
| URL path | `/aax` | Core_LOAD_DLL.dll | C2 registration/beacon path |
| URL path | `/11` | Core_LOAD_DLL.dll | C2 path |
| URL path | `/5` | Core_LOAD_DLL.dll | C2 path |
| Hostname | **Not recovered** | Core_LOAD_DLL.dll | Runtime-constructed from encrypted config |

### Registry

| Key | Notes |
|-----|-------|
| `SOFTWARE\SystemCore_` | Persistence / config key (suffix constructed at runtime) |

### Mutexes

| Mutex | Source |
|-------|--------|
| `Global\<runtime-derived>` | Main dropper (`Global\` prefix hardcoded) |
| `Mutex_Binds` | Core_LOAD_DLL.dll / TRUST_IMITATE_DLL.dll |
| `Mutex_Insert` | Core_LOAD_DLL.dll / TRUST_IMITATE_DLL.dll |
| `Mutex_Installing` | Core_LOAD_DLL.dll / TRUST_IMITATE_DLL.dll |

### Privileges Acquired

- `SeDebugPrivilege`
- `SeShutdownPrivilege`

### File/Process Indicators

| Indicator | Type | Notes |
|-----------|------|-------|
| `Core_LOAD_DLL.dll` | Dropped DLL | Stealer/loader component |
| `IRScriptEditor.exe` | Dropped executable | Indigo Rose Script Editor (sideload host) |
| `TRUST_IMITATE_DLL.dll` | Dropped DLL | Signed second-stage loader |
| `[IF_SERVICE_RUNING]`, `[IF_FILE]`, `[IF_SERVICE]`, `[IF_PROCESS]` | String constants | Operating-mode flags in Core_LOAD_DLL.dll |
| `TRRT_SelfDLL`, `TRRT_File` | String constants | Task type identifiers in Core_LOAD_DLL.dll |

---

## 7. Emulation Results

### Speakeasy (pass 1 — generic runner, x86)
- **VirtualAlloc** at `0x50000`, size `0x13fff0` (1.31 MB), `PAGE_READWRITE` — payload decompression buffer
- Stopped at `kernel32.GetThreadPreferredUILanguages` (API not implemented in speakeasy)
- No network IOCs captured; Delphi runtime initialization blocks emulation before dropper logic executes

### Speakeasy (pass 2 — plain, x86)
- Same VirtualAlloc, Delphi locale registry checks, stopped at same `GetThreadPreferredUILanguages`
- No additional IOCs

### Core_LOAD_DLL.dll (DLL mode, speakeasy)
- Same VirtualAlloc (`0x50000`, 1.3 MB, PAGE_READWRITE)
- No network IOCs — runtime guard prevents reaching network code during emulation

---

## 8. Sandbox Results (ANY.RUN)

| Field | Value |
|-------|-------|
| **Task ID** | `67db0479-14f9-43e9-8a6e-bb25262989b7` |
| **Score** | 0 / 100 |
| **Verdict** | No threats detected |
| **Family tags** | (none) |
| **Public URL** | https://app.any.run/tasks/67db0479-14f9-43e9-8a6e-bb25262989b7 |

The binary shows strong behavioral evasion — ANY.RUN found no malicious activity, consistent with the toolkit's `WTSEnumerateProcessesW` anti-VM check and same-day build timing.

---

## 9. Cross-Reference to Prior Bind_EXE Samples

This sample meets the cross-reference bar on **five independent indicators**:

| Indicator | This sample | Prior sample | Match |
|-----------|-------------|--------------|-------|
| **Export name** `Core_LOAD_DLL.dll` | ✓ (blob1 PE export) | `1.exe` SHA256:`df9fefad...` | Identical build artifact |
| **Export name** `TRUST_IMITATE_DLL.dll` | ✓ (blob4 PE export) | `1.exe` SHA256:`df9fefad...` | Identical build artifact |
| **Sideload host** `IRScriptEditor.exe` | ✓ (blob3, Indigo Rose) | `WH_1E_1.exe` SHA256:`1d2c7d7a...` | Identical build artifact |
| **Registry key** `SOFTWARE\SystemCore_` | ✓ (XOR-0x09 decoded) | `1.exe` SHA256:`df9fefad...` | Identical config value |
| **C2 paths** `/aax` + `/11` | ✓ (UTF-16LE in Core DLL) | `1.exe` SHA256:`df9fefad...` | Matching network IOCs |
| **Mutex names** `Mutex_Binds/Insert/Installing` | ✓ | `1.exe` / `WH_1E_1.exe` | Identical extracted config |

**Delta from prior versions:**
- **Certificate rotated**: Sectigo EV under Gansu Shishida IT (CN) — new entity vs. prior GlobalSign EV under Xiamen/Weihai entities
- **XOR key updated**: Primary decryption routine changed to XOR-0x0A + byte-reversal (`sub_ab1fc8`) vs. XOR-0x10 in prior `1.exe`
- **New C2 path**: `/5` added alongside `/aax` and `/11`
- **Blob count**: 4 ZLIB blobs (same as prior `1.exe` df9fefad, vs 6 blobs in `WH_1E_1.exe`)

---

## 10. Analyst Notes

1. **C2 hostname unrecovered**: The C2 hostname is absent from all static layers of Core_LOAD_DLL.dll. It is either injected from TRUST_IMITATE_DLL.dll at runtime, stored in an encrypted config constructed dynamically, or fetched from a staged C2 endpoint. Dynamic analysis in a real Windows environment (x64dbg + Wireshark) is the recommended next step.

2. **TRUST_IMITATE_DLL.dll cert abuse**: The signed second-stage shares the dropper's EV certificate. This means two binaries dropped to disk both carry valid EV signatures from the same organization, maximizing trust bypass even if the outer dropper is quarantined.

3. **Fresh same-day build**: Both the dropper (10:05 UTC) and Core_LOAD_DLL.dll (07:39 UTC) have build timestamps matching the analysis date (2026-05-14). This suggests active development and operational deployment, not old re-used infrastructure.

4. **KesaKode low-confidence matches**: Malcat reported Mispadu, OzoneRAT, Scote, TefoSteal at confidence=1 for all payloads. These are almost certainly false positives from the Delphi RTL and lure code, not family attribution. The Bind_EXE toolkit is not yet in KesaKode's catalog.

5. **Recommended follow-up**:
   - Execute in isolated Windows VM with network interception to recover C2 hostname from runtime strings
   - Submit Core_LOAD_DLL.dll and TRUST_IMITATE_DLL.dll individually to ANY.RUN for DLL-specific behavioral analysis
   - Monitor certificate serial `578b8a96c9a5126336695edf73fc3f51` on Sectigo CT logs for additional signed samples
   - Search VirusTotal for the `Software\SystemCore_` registry key and `Mutex_Binds` mutex name as hunt pivots

---

## MITRE ATT&CK Mapping

| Technique | ID | Notes |
|-----------|----|-------|
| Masquerading | T1036 | Renamed Bind_EXE export, fake version info |
| Code Signing | T1553.002 | Valid Sectigo EV certificate |
| DLL Side-Loading | T1574.001 | IRScriptEditor.exe + TRUST_IMITATE_DLL.dll |
| Obfuscated Files or Information | T1027 | ZLIB compression + XOR encoding |
| Software Packing | T1027.002 | Multi-layer ZLIB + XOR payload packing |
| Process Injection | T1055 | NtUnmapViewOfSection (Process Hollowing) |
| Token Impersonation/Theft | T1055 (T1134) | SeDebugPrivilege via AdjustTokenPrivileges |
| Process Discovery | T1057 | WTSEnumerateProcessesW (also anti-VM) |
| System Service Discovery | T1007 | EnumServicesStatusExW |
| Credentials from Password Stores | T1555 | CryptUnprotectData (DPAPI) |
| Modify Registry | T1112 | SOFTWARE\SystemCore_ registry key |
| Application Window Discovery | T1010 | Delphi UI lure displayed to victim |
| Virtualization/Sandbox Evasion | T1497 / T1622 | WTSEnumerateProcessesW VM process check |
| Web Protocols | T1071.001 | HTTP C2 via /aax, /11, /5 paths |
| User Execution | T1204.002 | Victim must run dropper |
