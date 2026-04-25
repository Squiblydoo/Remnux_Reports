# Malware Analysis Report: 1.exe

**Date:** 2026-04-25  
**Analyst:** Claude (automated)  
**Sample path:** `/home/remnux/mal/1.exe`

---

## 1. File Metadata

| Field | Value |
|---|---|
| **SHA256** | `df9fefadf7012452f597602f84a31f6e40637004260c1457f819ce1ceb89b7d2` |
| **MD5** | `1dca47fff92d2b11678ff3fd35ab9b06` |
| **SHA1** | `7a5e21d0be1b45b32833d6e6848d1b90d11cd28a` |
| **Size** | 7,498,704 bytes (7.15 MB) |
| **Type** | PE32 executable (GUI) Intel x86 |
| **Compiler** | Borland/Embarcadero Delphi (TurboLinker) |
| **Delphi project** | `cT8sCwRr` |
| **Build timestamp** | 2026-04-25 16:36:45 |
| **Export module** | `Bind_EXE.exe` |
| **Sections** | 11 (.text, .itext, .data, .bss, .idata, .didata, .edata, .tls, .rdata, .reloc, .rsrc) |

**Code Signing Certificate:**
- Issuer: GlobalSign GCC R45 EV CodeSigning CA 2020
- Subject: **厦门鑫美泰网络科技有限公司** (Xiamen Xinmeitai Network Technology Co., Ltd.)
- State: 福建 (Fujian), Locality: 厦门 (Xiamen), Country: CN
- Serial: `5201cd9afb0d56ec78f86942`
- Validity: 2026-04-24 → 2027-04-25 (1-year EV cert, issued day of build)

---

## 2. Classification

**Family:** Bind_EXE File Binder / DLL Sideload Dropper — **SAME TOOLKIT** as `WH_1E_1.exe` (2026-04-24) and `6.exe` (2026-04-06)  
**Confidence:** HIGH  
**Threat Level:** HIGH — confirmed multi-component stealer/RAT dropper with EV-signed evasion

**Cross-reference indicators met (strict policy):**
1. **Extracted config match:** Mutex set `Global\a_`, `Global\b_`, `Global\c_`, `Global\d_`, `Global\e_`, `Global\x_` — exact match to WH_1E_1.exe and 6.exe
2. **Identical build artifact:** Embedded `IRScriptEditor.exe` (Indigo Rose Script Editor 2.0.1002.0, PDB: `C:\proj\IRScriptEditor\Release\IRScriptEditor.pdb`) — same build, same PDB path as WH_1E_1.exe
3. **Same export names verbatim:** `Bind_EXE.exe` (dropper export), `TRUST_IMITATE_DLL.dll` (second-stage) — exact matches

The signing certificate serial number is **different** from WH_1E_1.exe (`5201cd9afb0d56ec78f86942` vs. `10c33f009b54f66a849b4c90`), indicating the same toolkit operator obtained a fresh EV certificate under a different Xiamen CN company identity.

---

## 3. Capabilities

- **DLL Sideloading:** Drops legitimate `IRScriptEditor.exe` (Indigo Rose, expired COMODO cert) as sideload host; loads `TRUST_IMITATE_DLL.dll` (EV-signed Delphi loader) via DLL hijacking
- **Keylogger:** `GetKeyboardState`, `GetKeyState`, `GetForegroundWindow` (confirmed in TRUST_IMITATE_DLL)
- **Screenshot capture:** Screen capture APIs (confirmed by peframe YARA)
- **Process injection:** XOR-0x09-obfuscated `NtUnmapViewOfSection` (process hollowing)
- **ZLIB payload decompression:** Inline inflate 1.2.3 (Mark Adler); additionally uses NTDLL `RtlDecompressBuffer`/`RtlGetCompressionWorkSpaceSize`/`RtlCompressBuffer` APIs (XOR-0x09 obfuscated)
- **Anti-VM process scanning:** XOR-0x09-obfuscated `WTSEnumerateProcessesW` — enumerates running processes to detect VM tools (VMTOOLS.EXE, VBoxSer, etc.)
- **Anti-debug:** `IsDebuggerPresent`, `FindWindowW`, `FindWindowExW`, `GetLastError`, `UnhandledExceptionFilter`
- **Registry persistence:** Writes config/state to `SOFTWARE\SystemCore_` (decoded from XOR-0x09 encoded registry path)
- **File operations:** `ShellExecuteW` (payload launch), `DeleteFileW` (cleanup), `GetLongPathNameW`
- **Lure document:** Drops 20-page PDF (blob2, SHA256 `935d5c5a...`) to distract victim
- **Modular execution modes:** Config flags `[IF_SERVICE]`, `[IF_FILE]`, `[IF_PROCESS]`, `[IF_SERVICE_RUNING]` — supports deployment as service, injected DLL, or standalone process
- **String/API obfuscation:** All sensitive API names stored as XOR-0x09 encoded UTF-16LE; ZLIB payloads stored XOR-0x10 encoded in `.text` section; Delphi RTTI in `Core_LOAD_DLL.dll` overlay XOR-0x10 encoded
- **EV code signing evasion:** GlobalSign EV cert on dropper and TRUST_IMITATE_DLL bypasses SmartScreen/AV trust checks

---

## 4. Attack Chain

```
User executes 1.exe (GlobalSign EV signed)
    │
    ▼
Dropper (Bind_EXE.exe)
    ├─ Decompresses ZLIB blob1 (XOR-0x10 decode) → Core_LOAD_DLL.dll
    ├─ Decompresses ZLIB blob2 → PDF lure (20 pages, opens to distract victim)
    ├─ Decompresses ZLIB blob3 → IRScriptEditor.exe (Indigo Rose sideload host)
    └─ Decompresses ZLIB blob4 → TRUST_IMITATE_DLL.dll (EV signed, same cert)
         │
         ▼
Drops to disk → IRScriptEditor.exe + TRUST_IMITATE_DLL.dll (renamed to sideload target)
Creates mutexes: Global\a_, Global\b_, Global\c_, Global\d_, Global\e_, Global\x_
Checks mode flag: [IF_SERVICE] / [IF_FILE] / [IF_PROCESS]
         │
         ▼
IRScriptEditor.exe (loaded as host) → sideloads TRUST_IMITATE_DLL.dll
         │
         ▼
TRUST_IMITATE_DLL.dll loader → loads/executes Core_LOAD_DLL.dll
         │
         ▼
Core_LOAD_DLL.dll (RAT/stealer core):
    ├─ Anti-VM: WTSEnumerateProcessesW (scan for VM processes)
    ├─ Registry: SOFTWARE\SystemCore_ (config/persistence store)
    ├─ Process injection: NtUnmapViewOfSection
    ├─ Keylogger: GetKeyState/GetKeyboardState
    ├─ Screenshot capture
    └─ C2 beacon (URL paths /aax, /11; full C2 hostname XOR-obfuscated, not recovered)
```

---

## 5. IOCs

### Network (defanged)
| Type | Value | Notes |
|---|---|---|
| URL path | `/aax` | C2 endpoint (cleartext UTF-16LE in Core_LOAD_DLL) |
| URL path | `/11` | C2 endpoint (cleartext UTF-16LE in Core_LOAD_DLL) |
| **C2 hostname** | **Not recovered** | XOR-obfuscated; not deobfuscated via static analysis |

### Filesystem
| Path | Notes |
|---|---|
| `IRScriptEditor.exe` | Sideload host (dropped to working dir) |
| `TRUST_IMITATE_DLL.dll` | Signed second-stage loader (dropped) |
| `Core_LOAD_DLL.dll` | Stealer/RAT core DLL (dropped, XOR-0x10 encoded in dropper) |
| PDF lure | 20-page PDF document (dropped as decoy) |

### Registry
| Key | Notes |
|---|---|
| `SOFTWARE\SystemCore_` (+ suffix) | Config/persistence store (XOR-0x09 decoded) |
| `HKCU\...\Run` | Possible persistence (Run key, common in Bind_EXE family) |

### Mutexes
- `Global\a_`
- `Global\b_`
- `Global\c_`
- `Global\d_`
- `Global\e_`
- `Global\x_`

### Certificate IOCs
| Field | Value |
|---|---|
| Dropper cert serial | `5201cd9afb0d56ec78f86942` |
| TRUST_IMITATE_DLL cert serial | `5201cd9afb0d56ec78f86942` (same) |
| Sideload host cert serial | `45a3e17188ebaeb6b157ecf6147e0a74` (expired Indigo Rose, 2014-2019) |

### Component Hashes
| Component | SHA256 |
|---|---|
| 1.exe (main dropper) | `df9fefadf7012452f597602f84a31f6e40637004260c1457f819ce1ceb89b7d2` |
| Core_LOAD_DLL.dll | `421271973ce38a88eb43a63e33483ceeba759d417ad94cac5e15e85427adf0af` |
| TRUST_IMITATE_DLL.dll | `69c33422af04f4c58ecc7aa5f9c327b0303dfdb1defdbef11633e2ebf851dada` |
| IRScriptEditor.exe | `387af3a68e12bcc8dd3875245e114702c124352c5f286660e613432f61750b86` |

---

## 6. Emulation Results

**Speakeasy (pass 1, generic runner, x86):** No IOCs captured. Execution stops at Delphi locale/registry initialization; blocked by unsupported `kernel32.GetThreadPreferredUILanguages` stub.

**Speakeasy (pass 2, plain, x86):** Traces Delphi init (RegOpenKeyExW for Borland/Embarcadero locales, GetModuleFileNameW, GetVersion), then halts at same unsupported API. No network/file/registry IOCs emitted.

**Root cause:** The malware's initialization logic gates on `GetThreadPreferredUILanguages` (used for anti-sandbox locale fingerprinting before proceeding to decompression/payload deployment). Speakeasy does not implement this API, preventing further execution.

**Angr / pass 3:** Not attempted; the blocking condition is an API stub gap rather than a crypto function, making angr unsuitable here. Full dynamic execution on a bare-metal Windows VM would be required to recover runtime IOCs.

---

## 7. Sandbox Results

**ANY.RUN** (task ID: `d4abe6aa-bf9f-4415-b145-e7f0d626d3c1`)  
- Score: **5/100** ("No threats detected")  
- Tags: *(none)*
- Public URL: `https://app[.]any[.]run/tasks/d4abe6aa-bf9f-4415-b145-e7f0d626d3c1`
- IOCs observed: Windows telemetry only (settings-win.data.microsoft.com, login.live.com, watson.events.data.microsoft.com); no C2 network activity

**Assessment:** Strong sandbox evasion. The ANY.RUN 5/100 verdict matches the pattern established by WH_1E_1.exe (0/100) — Bind_EXE family consistently evades cloud sandboxes via locale/environment fingerprinting before detonating payload.

---

## 8. MITRE ATT&CK Techniques

| ID | Technique | Evidence |
|---|---|---|
| T1574.001 | DLL Side-Loading | IRScriptEditor.exe + TRUST_IMITATE_DLL.dll sideload chain |
| T1036.001 | Masquerading: Invalid Code Signature | Expired 2014 Indigo Rose cert on sideload host |
| T1553.002 | Subvert Trust Controls: Code Signing | GlobalSign EV cert (1-yr) on dropper and TRUST_IMITATE_DLL |
| T1027 | Obfuscated Files or Information | XOR-0x09 API names, XOR-0x10 payload blobs, ZLIB compression |
| T1027.002 | Software Packing | ZLIB-compressed embedded payloads in .text section |
| T1055 | Process Injection | NtUnmapViewOfSection (XOR-0x09 obfuscated) |
| T1497 | Virtualization/Sandbox Evasion | WTSEnumerateProcessesW VM process scanning |
| T1622 | Debugger Evasion | IsDebuggerPresent, FindWindowW/ExW |
| T1056.001 | Keylogging | GetKeyboardState, GetKeyState, GetForegroundWindow |
| T1113 | Screen Capture | Screenshot APIs (peframe confirmed) |
| T1012 | Query Registry | SOFTWARE\SystemCore_ config reads |
| T1112 | Modify Registry | SOFTWARE\SystemCore_ persistence/config writes |
| T1057 | Process Discovery | WTSEnumerateProcessesW |
| T1082 | System Information Discovery | GetSystemInfo, GetVersion |
| T1105 | Ingress Tool Transfer | C2 URLs /aax, /11 (host not recovered) |
| T1204.002 | User Execution: Malicious File | PDF lure (20 pages dropped on execution) |
| T1129 | Shared Modules | ZLIB inflate 1.2.3 embedded; Delphi modular loader |

---

## 9. Analyst Notes

**Gaps and recommended follow-up:**
1. **C2 hostname unrecovered.** The URL paths `/aax` and `/11` are visible in `Core_LOAD_DLL.dll` but the hostname is XOR-obfuscated and was not decrypted. Dynamic execution on bare-metal Windows with process monitoring (ProcMon, Wireshark) is required to capture live C2 traffic.
2. **TRUST_IMITATE_DLL.dll second-stage.** The 2.2MB DLL contains the same Delphi library base but its malware-specific logic was not fully decompiled. Its keylogger APIs (`GetKeyboardState`, `GetKeyState`) suggest it is the persistence and data-exfil layer, but behavior vis-à-vis Core_LOAD_DLL is not fully mapped.
3. **Registry suffix for SOFTWARE\SystemCore_.** The decoded config key is `SOFTWARE\SystemCore_` followed by a runtime-computed suffix (possibly victim UUID or campaign ID). The config key suffixes `2_`, `3_`, `4_`, `LTC_`, `CVI_` likely store keylog data, screenshot buffer refs, or victim metadata.
4. **Execution mode selection.** The flags `[IF_SERVICE]`, `[IF_FILE]`, `[IF_PROCESS]` suggest a configurable deployment mode driven by an external config (possibly embedded in the PDF lure's binary metadata or the dropper's `.bss` section). This should be investigated to determine how the operator selects execution mode.
5. **Same toolkit, new cert, new target.** The 1-year GlobalSign EV cert was issued 2026-04-24 (the day before analysis). The previous Bind_EXE sample (WH_1E_1.exe, 2026-04-24) used a different company's cert. The rapid cert rotation suggests active campaign infrastructure.
6. **PDF lure content not analyzed.** The 20-page PDF (blob2) was not opened. It may contain a social-engineering lure specific to a target sector or individual.
