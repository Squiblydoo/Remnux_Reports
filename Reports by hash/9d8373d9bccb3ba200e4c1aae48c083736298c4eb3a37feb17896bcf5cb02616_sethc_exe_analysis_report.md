# Malware Analysis Report: sethc.exe

**Date**: 2026-04-03
**Analyst**: Claude Code (claude-sonnet-4-6)
**Sample**: sethc.exe

---

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | sethc.exe |
| SHA256 | `9d8373d9bccb3ba200e4c1aae48c083736298c4eb3a37feb17896bcf5cb02616` |
| SHA1 | `1bec61036e97dd9386ab1f0f9aca42945df9442f` |
| MD5 | `de5f970a05abd0f03f99b6d0802aadb2` |
| Size | 168,160 bytes (164 KB) |
| Type | PE32+ executable (GUI) x86-64, Windows |
| Sections | 7 (.text, .rdata, .data, .pdata, .didat, .rsrc, .reloc) |
| Overlay | 11,488 bytes (Authenticode PKCS7 signature) |
| Entropy (overall) | 92/127 |
| .text entropy | 127/127 (maximum — high suspicion of packed/encrypted code) |

### Signing Certificate
| Field | Value |
|---|---|
| Subject | **Xiamen Zhiqing Information Technology Co., Ltd.** |
| Issuer | Sectigo Public Code Signing CA EV R36 (Sectigo Limited, GB) |
| Validity | 2026-03-04 → 2027-03-04 (1 year EV cert) |
| Serial | `7a37178f179e98a61707912ad0deb4f9` |
| Algorithm | SHA256 / RSA |

### VersionInfo (claimed — FALSIFIED)
- CompanyName: `Microsoft Corporation`
- FileDescription: `Accessibility shortcut keys`
- FileVersion: `10.0.19041.4355 (WinBuild.160101.0800)`
- OriginalFilename: `sethc.exe`
- ProductName: `Microsoft® Windows® Operating System`

> ⚠️ **Critical discrepancy**: VersionInfo claims Microsoft Corporation, but the signing certificate belongs to a Chinese company (Xiamen, Fujian province). The real Microsoft `sethc.exe` is signed by Microsoft Corporation.

### Build Artifacts
- PDB path: `sethc.pdb` (minimal/stripped)
- Debug timestamp: `2006-06-17 14:39:08` (falsified — predates Windows 10)
- Delay imports: `wininet.dll` (network library — absent from legitimate sethc.exe)

---

## 2. Classification

| | |
|---|---|
| **Classification** | **HIGH CONFIDENCE MALICIOUS** |
| **Type** | Trojanized Windows System Binary (Accessibility Sticky Keys backdoor) |
| **Family** | Shuyal (malcat kesakode; low-confidence match, treat as informative) |
| **Technique** | T1546.008 — Event Triggered Execution: Accessibility Features |

### Reasoning

The binary contains the **complete, functional Windows 10 sethc.exe** (Sticky Keys / Accessibility Shortcut Keys handler) as a host, with a malicious C2 communication stub injected into the CRT startup sequence. Evidence for trojanization:

1. **Identity forgery**: VersionInfo claims Microsoft Corporation; EV cert belongs to a Chinese technology company. The real Windows sethc.exe is signed by Microsoft. This is deliberate identity deception.
2. **Injected network stub** (`sub_14000eff0`): The CRT startup function (`sub_14000efe0`) calls a covert network routine *before* running the legitimate WinMain. This routine:
   - Uses PEB walking (GS→TEB→PEB→Ldr→InLoadOrderModuleList traversal) to covertly load `wininet.dll` without importing it in the PE import table
   - Applies a counter-based XOR decryption (each byte XORed with a decrementing value starting at `0xFF`) to inline-encrypted C2 data
   - Makes network connections using a spoofed User-Agent
3. **Spoofed User-Agent**: `"Windows 7 10.0; Win64; x64"` — present in the `.text` section near the wininet loading code
4. **Maximum .text entropy (127/127)**: Consistent with encrypted data embedded inline in the code section

### What is NOT malicious (legitimate sethc.exe behavior)
The following capa detections are **legitimate accessibility features** — not malicious:
- **T1056.001 Keylogging**: `GetKeyState()` polling + `SendInput()` = Sticky Keys synthesizes Win+U to open Ease of Access Center (standard behavior)
- **T1547.001 Run key persistence**: Managing OSK/Narrator/Magnifier startup `.lnk` files in Shell Folders (normal AT management)
- **T1134.004 Parent PID Spoofing**: Launching `ATBroker.exe` with Explorer as parent via `PROC_THREAD_ATTRIBUTE_PARENT_PROCESS` (standard accessibility design)
- **T1012 Query Registry**: Extensive accessibility registry queries (legitimate)

---

## 3. Capabilities

### Malicious (injected stub)
- **Covert DLL loading**: Loads `wininet.dll` via PEB module list traversal (LDR_DATA_TABLE_ENTRY walk at `sub_14000eff0`) — avoids import table detection
- **Inline string decryption**: Counter-based XOR decryption (key bytes: 0xFF, 0xFE, 0xFD, ...) applied to 102-byte inline-encrypted buffer containing C2 target
- **HTTP C2 communication**: Uses decrypted wininet.dll functions to make outbound HTTP request with User-Agent `"Windows 7 10.0; Win64; x64"` before legitimate sethc execution
- **System fingerprinting**: `GetUserNameExW()` → `CompareStringOrdinal()` against `DefaultAccountSAMName` from `SOFTWARE\Microsoft\Windows\CurrentVersion\OOBE` (`sub_14000e6c0`); `GetVersionExW()`, `GetSystemInfo()`
- **Identity deception**: Falsified VersionInfo claiming Microsoft Corporation to avoid manual inspection

### Inherited from legitimate sethc.exe (not malicious)
- Accessibility shortcut key handling (Sticky Keys, Filter Keys, Toggle Keys, Mouse Keys, High Contrast)
- Ease of Access Center launch (Win+U)
- ATBroker.exe process management (Narrator, Magnifier, OSK)
- Registry persistence for accessibility tool autostart
- WTS session management, DUI70/OLEACC UI framework

---

## 4. Attack Chain

```
[Victim System]
    │
    ├─ Attacker replaces C:\Windows\System32\sethc.exe
    │  (requires SYSTEM/TrustedInstaller; likely via prior compromise)
    │
    ├─ User locks screen / reaches login screen
    │
    └─ Shift × 5 → Windows triggers sethc.exe (SYSTEM context)
           │
           ├─ EntryPoint → SecurityCookieInit → jmp sub_14000efe0 (CRT startup)
           │
           ├─ sub_14000efe0 calls sub_14000eff0 [MALICIOUS STUB]
           │       │
           │       ├─ PEB walk → locate loaded modules list
           │       ├─ Vtable call → LoadLibrary("wininet.dll") [via image base offset]
           │       ├─ XOR-decrypt 102-byte inline buffer → C2 URL/params
           │       └─ wininet HTTP request to C2 (User-Agent: "Windows 7 10.0; Win64; x64")
           │
           └─ CRT startup continues → WinMain (sub_140001ccc) → legitimate sethc.exe
                   (accessibility UI appears normally — victim unaware)
```

**Why this is dangerous**: sethc.exe runs at the Windows logon screen (before any user logs in) under the WINLOGON process context with elevated privileges. The malicious stub phones home on every Sticky Keys trigger — including from a locked workstation — providing persistent SYSTEM-level C2 access.

---

## 5. IOCs

### Network
| Type | Value | Notes |
|---|---|---|
| User-Agent | `Windows 7 10.0; Win64; x64` | HTTP C2 beacon User-Agent (EA 0xE4CB in binary) |
| Protocol | HTTP via `wininet.dll` | Loaded covertly via PEB walk |
| C2 target | **Encrypted** — requires dynamic analysis | 102-byte XOR-encrypted buffer at image+0x194E0 area |

### File System
| Type | Value |
|---|---|
| Target path | `C:\Windows\System32\sethc.exe` (replacement target) |
| PDB | `sethc.pdb` |

### Code / Certificate
| Type | Value |
|---|---|
| Cert subject | `Xiamen Zhiqing Information Technology Co., Ltd.` |
| Cert serial | `7a37178f179e98a61707912ad0deb4f9` |
| Cert issuer | `Sectigo Public Code Signing CA EV R36` |
| Cert validity | `2026-03-04` → `2027-03-04` |

### Hashes
| Algorithm | Hash |
|---|---|
| SHA256 | `9d8373d9bccb3ba200e4c1aae48c083736298c4eb3a37feb17896bcf5cb02616` |
| SHA1 | `1bec61036e97dd9386ab1f0f9aca42945df9442f` |
| MD5 | `de5f970a05abd0f03f99b6d0802aadb2` |

### MITRE ATT&CK
| Technique | ID | Notes |
|---|---|---|
| Event Triggered Execution: Accessibility Features | T1546.008 | sethc.exe replacement |
| Obfuscated Files or Information | T1027 | XOR-encrypted inline C2 data |
| Shared Modules / Runtime Linking | T1129 | PEB walk to load wininet.dll |
| System Owner/User Discovery | T1033 | GetUserNameExW, DefaultAccountSAMName |
| System Information Discovery | T1082 | GetVersionExW, GetSystemInfo |
| Masquerading | T1036 | Falsified VersionInfo claiming Microsoft |

---

## 6. Analyst Notes

### Gaps / Requiring Dynamic Analysis
1. **C2 URL**: The 102-byte payload at `sethc.exe_base + 0x194E0` is XOR-encrypted with a counter key (starting 0xFF, decrementing per byte). Dumping the in-memory decrypted string requires runtime extraction (debugger breakpoint at `sub_14000eff0` after the XOR loop).
2. **Wininet function used**: `sub_14000f17d` called with decrypted buffer + wininet handle — likely `InternetOpenA/InternetConnectA/HttpOpenRequestA` or similar. Need runtime analysis to identify which wininet functions are resolved via the vtable calls (offsets 0x11A20, 0x11A88, 0x11D78 from image base).
3. **Full C2 capability**: Whether this is a simple beacon vs. full backdoor (download/execute) not determinable statically. The stub appears to make a single outbound request; may receive commands.
4. **Dropper/installer**: How sethc.exe was initially replaced is unknown. Requires incident response investigation of the affected system.

### Alternative Hypotheses Considered
- **"Legitimate Chinese port of sethc.exe"**: Rejected. No plausible reason a Chinese company's EV cert would sign a Windows system binary with falsified Microsoft VersionInfo and a covert wininet HTTP call.
- **"capa detections are false positives"**: Most capa findings (keylog, persistence, PID spoof) are indeed legitimate sethc.exe behaviors. Only the PEB-walk + wininet network stub is confirmed malicious.
- **"Shuyal family attribution"**: Malcat kesakode confidence = 0. Treat as weak signal only. Attribution to Shuyal or any specific threat actor requires additional cross-referencing.

### Detection Opportunities
- Monitor for writes to `C:\Windows\System32\sethc.exe` (high fidelity IOC)
- Alert on any process signed by `Xiamen Zhiqing Information Technology Co., Ltd.` executing from System32
- Alert on `wininet.dll` loaded by processes that don't normally use it (sethc, utilman, osk, narrator)
- Hunt for Authenticode certificates with serial `7a37178f179e98a61707912ad0deb4f9`
