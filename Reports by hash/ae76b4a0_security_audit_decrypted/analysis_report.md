# security_audit_20260514.log (Decrypted) — ValleyRAT Stage 3+4 Loader Chain
**Analysis Date**: 2026-05-17  
**Analyst**: REMnux/Claude  

---

## 1. File Metadata

### Outer: Shellcode Loader (Stage 3)

| Field | Value |
|---|---|
| **Filename** | `security_audit_20260514.log` (encrypted); `security_audit_decrypted.bin` (decrypted) |
| **SHA256 (decrypted)** | `ae76b4a0e3fc7edbefb1cce56ac51ca9ee1d26cc29dee20de37767b4eb29895d` |
| **Size** | 504,241 bytes |
| **File Type** | Raw x64 shellcode — no MZ header, position-independent |
| **First bytes** | `48 83 ec 28 e8 43 1c 00 00` = `sub rsp,28h; call +0x1c43; add rsp,28h; ret; [int3 padding]` |
| **Decryption key** | 32-byte repeating XOR: `ab cd ef 12 34 56 78 90 de ad be ef ca fe ba be 12 34 56 78 9a bc de f0 0f 1e 2d 3c 4b 5a 69 78` (from AppClient.exe) |
| **Embedded PE offset** | Raw offset `0x1db1` (7601 bytes into shellcode); XOR-encoded with key `0x16` |

### Inner: Embedded PE DLL — "RexRat5" (Stage 4)

| Field | Value |
|---|---|
| **Filename** | `embedded_pe_decoded.bin` (extracted) |
| **SHA256** | `df134ffc0f4f76ced4a3ec6e026718d41d52e29bc91a1e394bc13bb734220b54` |
| **SHA1** | `6e884040bdec148eea95a3dda6ec91e6aa0439ed` |
| **MD5** | `119fbd88ca1ed3b7a85de5a28262f2af` |
| **Size** | 496,640 bytes |
| **File Type** | PE32+ executable (DLL) (GUI) x86-64, for MS Windows, 7 sections |
| **Compiler** | MSVC 2022 (linker: `MSVC_2022_linker`) |
| **Build timestamp** | 2026-05-05 01:44:20 UTC |
| **PDB path** | `C:\Users\Administrator\Desktop\RexRat5   0418\` |
| **ImageBase** | `0x180000000` (standard 64-bit DLL) |
| **EntryRVA** | `0x27e90` |
| **DLL Exports** | `Test` (offset 0x180006550), `load` (0x180006610), `run` (0x180006770), `run` (duplicate) |
| **Imphash** | `41141da83eb6dc530d48f302aa61f772` |
| **Code signing** | None |
| **PE checksum** | Not set (zeroed) |
| **Section `.fptable`** | Non-standard section name — custom function pointer table |

---

## 2. Classification

| Field | Assessment |
|---|---|
| **Family** | **ValleyRAT / PNGPlugLoader** variant — developer name "RexRat5" |
| **KesaKode** | PNGPlugLoader (34%), ValleyRAT (30%), GhostRAT (19%), YoungLotus (19%) |
| **Role** | Stage 3 (shellcode reflective loader) + Stage 4 (ValleyRAT-family DLL implant) |
| **Confidence** | **High** |
| **Threat Level** | Critical — full-featured RAT with keylogging, screenshot, injection, privilege escalation |

**Reasoning**: The `security_audit_20260514.log` file is the XOR-encrypted delivery container for a two-stage payload. After decryption by AppClient.exe, it is a **reflective PE loader shellcode**: a 7.6 KB prefix that resolves `LdrLoadDll`/`LdrGetProcedureAddress`, allocates memory, and copies the embedded PE (XOR-encoded at offset 0x1db1 with key 0x16) into a mapped region with `VirtualAlloc(0x180000000)`. The embedded PE is a 64-bit DLL internally named "RexRat5" with capabilities strongly matching the **ValleyRAT** family (Silver Fox APT), including plugin loading architecture (`PNGPlugLoader`), Chinese AV enumeration, COM-based UAC bypass, system fingerprinting, keylogging, screenshot, and process injection. PDB path `RexRat5   0418` indicates a versioned build from April 2026, compiled on May 5, 2026 — 10 days before the campaign delivery date.

---

## 3. Capabilities

### Stage 3 — Shellcode Reflective Loader
- **API resolution via NT native**: `LdrLoadDll("kernel32.dll")` then `LdrGetProcedureAddress` for: `VirtualAlloc`, `VirtualProtect`, `FlushInstructionCache`, `GetNativeSystemInfo`, `Sleep`, `RtlAddFunctionTable`, `LoadLibraryA` — avoids using `GetProcAddress` directly (lower visibility)
- **Architecture check**: `GetNativeSystemInfo` — verifies target system architecture before mapping
- **PE mapping**: `VirtualAlloc(0x180000000, SizeOfImage=0x81000, MEM_COMMIT|RESERVE, RW)` then fallback `VirtualAlloc(NULL, ...)` if preferred base unavailable
- **Exception handling setup**: `RtlAddFunctionTable` for x64 SEH support in the loaded PE
- **XOR-decode embedded PE**: byte-wise XOR with key `0x16` at shellcode offset 0x1db1, length 496,640 bytes
- **Entry point call**: invokes embedded DLL exports (`load`, `run`) after loading

### Stage 4 — RexRat5 DLL (ValleyRAT Family)

**System Fingerprinting (beaconing)**
- `gethostname` + `gethostbyname` + `inet_ntoa` → collects all local IP addresses (all adapters)
- `GetCurrentHwProfileW` → hardware profile GUID (unique machine identifier)
- COM `IPropertyBag.Read("FriendlyName")` via `CoCreateInstance` → network adapter enumeration
- `GetLocaleInfoW(LOCALE_SYSTEM_DEFAULT, LOCALE_SENGLANGUAGE)` → language/locale
- `GetSystemDirectoryW` → system path
- `GetNativeSystemInfo` / `RtlGetVersion` → OS version
- `K32GetProcessImageFileNameW` → hosting process path
- `GetForegroundWindow` + `GetWindowTextW` → active window title
- `GetTickCount` + `localtime` → system uptime and current time
- `GetCurrentHwProfileW` → hardware profile GUID
- All collected into a ~5285-byte beacon structure

**Keylogger**
- `GetKeyState` — keyboard state polling
- Special key name strings: `[enter]`, `[home]`, `[end]`, `[tab]`, `[esc]` (confirmed by YARA `SpecialKeyNames`)

**Screenshot Capture**
- `GdiplusStartup` + `gdiplus.dll` (22 imported functions) — GDI+ screen capture

**Process Injection**
- `VirtualProtect`, `WriteProcessMemory`, `ReadProcessMemory`, `CreateRemoteThread` — all resolved via `GetProcAddress` at runtime
- `CreateProcessA`, `CreateProcessW` — spawn processes for injection

**Privilege Escalation**
- `OpenProcessToken` + `AdjustTokenPrivileges` + `LookupPrivilegeValueA/W` — SeDebugPrivilege acquisition
- Token integrity level check (`GetTokenInformation(TokenIntegrityLevel)`) — determines privilege context

**UAC Bypass**
- COM elevation moniker: `Elevation:Administrator!new:%s` with CLSID `{3E5FC7F9-9A51-4367-B063-A120244FBEC7}`
- Used to instantiate elevated COM object without UAC prompt

**Windows Defender Evasion**
- `powershell -c "Add-MpPreference -ExclusionPath '%s' -Force"` — adds install path to Defender exclusion list

**AV/Security Product Enumeration** (process list, likely for kill/evasion)
- Chinese AV: 360Safe.exe, 360tray.exe, 360Tray.exe, 360sd.exe, BaiduSd.exe, QQPCRTP.exe, QQPCTray.exe, kxetray.exe
- Communication apps (possibly C2 interference check): QQ.exe, WeChat.exe, WXWork.exe
- International AV: avpui.exe, avp.exe (Kaspersky), NOD32, DR.WEB, F-Secure, QuickHeal, Outpost, BitDefender, msmpeng.exe/MsMpEng.exe (Windows Defender), RavMonD.exe, KvMonXP.exe, V3Svc.exe, SPIDer.exe, AYAgent.aye, remupd.exe, acs.exe, cfp.exe, TMBMSRV.exe, rtvscan.exe, ashDisp.exe

**Registry Access**
- `RegOpenKeyExW`, `RegQueryValueExW`, `RegQueryInfoKeyW`, `RegEnumKeyExW`, `RegCreateKeyExW` (HKCU + HKLM, multiple paths — encrypted at runtime)

**PEB Manipulation / Anti-Debug**
- `change peb success` string — PEB LDR modification (likely for name masquerading)
- `IsDebuggerPresent`, `UnhandledExceptionFilter`, `RaiseException`, `OutputDebugStringW`
- `register seh success` — structured exception handler registration
- PEB walking for module location without API calls

**Persistence**
- `CreateScheduledTask` YARA hit
- `SHGetFolderPathW` — likely targeting startup/AppData folders

**Crypto**
- MD5, RIPEMD160, SHA1 constants — likely used for configuration hashing or data integrity
- 259 XOR loops — string/config decryption throughout

**Obfuscation**
- All sensitive strings (registry paths, C2 config, DLL names, API names) constructed dynamically via vectorized AVX2 XOR decryption at runtime
- 77 dynamically constructed strings, 15 stack array initializations, 9 dynamic DLL name constructions
- `ImportByHash` (×2) for some API resolution

---

## 4. Attack Chain

```
[WEB-OK_Updater.exe] — Stage 1 OKLink lure dropper
  │
  └─ Downloads security_audit_20260514.log from Tencent COS HK
     (wwwc86fasgasg-1330789127.cos.ap-hongkong.myqcloud.com)
  └─ Creates AppClientService (AppClient.exe)

[AppClient.exe] — Stage 2 SYSTEM service injector
  │
  ├─ os.ReadFile("security_audit_20260514.log")
  ├─ XOR-decrypt with 32-byte key → security_audit_decrypted.bin (504 KB x64 shellcode)
  ├─ WTSQueryUserToken → DuplicateTokenEx → find/spawn taskhostw.exe
  ├─ NtCreateSection + NtMapViewOfSection → shared memory
  ├─ GetThreadContext/SetThreadContext RIP=shellcode → ResumeThread
  └─ Shellcode executes in taskhostw.exe context (user privilege, hidden process)

[security_audit_decrypted.bin] — Stage 3 Reflective PE Loader (shellcode)
  │
  ├─ LdrLoadDll("kernel32.dll") → LdrGetProcedureAddress × 7 APIs
  ├─ GetNativeSystemInfo → architecture/CPU check
  ├─ VirtualAlloc(0x180000000, 0x81000) → map PE at preferred base (fallback: NULL)
  ├─ XOR-decode embedded PE (offset 0x1db1, key 0x16, 496640 bytes)
  ├─ Copy sections → apply relocations → resolve imports via LoadLibraryA/GetProcAddress
  ├─ RtlAddFunctionTable → setup x64 SEH
  └─ Call embedded DLL export "run"

[embedded_pe_decoded.bin] — Stage 4 RexRat5 DLL (ValleyRAT family)
  │
  ├─ PEB modification → masquerade hosting process
  ├─ Token integrity check (OpenProcessToken/GetTokenInformation)
  ├─ UAC bypass (COM elevation moniker) if not already admin
  ├─ Defender exclusion (PowerShell Add-MpPreference)
  ├─ System fingerprinting (hostname, IPs, MAC, HWID, locale, uptime, OS, window title)
  ├─ AV process enumeration (360, Baidu, Kaspersky, Defender, etc.)
  ├─ Beacon to C2 with ~5285-byte fingerprint packet (C2 address runtime-decrypted, not recovered)
  ├─ Receive commands: keylog, screenshot, inject, exec, download/upload
  └─ Persistence via scheduled task / startup folder
```

---

## 5. IOCs

### Network
All delivery-phase network IOCs belong to parent dropper (WEB-OK_Updater.exe). The C2 address for the ValleyRAT implant is runtime-decrypted and was not recovered in this analysis.

| Type | IOC | Notes |
|---|---|---|
| Domain | `wwwc86fasgasg-1330789127[.]cos[.]ap-hongkong[.]myqcloud[.]com` | Tencent COS HK — delivery server (from parent dropper) |
| URL | `hxxps://wwwc86fasgasg-1330789127[.]cos[.]ap-hongkong[.]myqcloud[.]com/security_audit_20260514[.]log` | Payload download URL |

### Filesystem

| Path | Purpose |
|---|---|
| `C:\ProgramData\AppClient\security_audit_20260514.log` | Encrypted shellcode container (XOR-decrypted by AppClient.exe) |
| `C:\Windows\System32\taskhostw.exe` | Injection host (running Stage 3+4 shellcode) |

### Build Artifacts

| Artifact | Value |
|---|---|
| PDB path | `C:\Users\Administrator\Desktop\RexRat5   0418\` |
| Build date | 2026-05-05 01:44:20 UTC |

### Cryptographic

| Item | Value |
|---|---|
| Shellcode SHA256 | `ae76b4a0e3fc7edbefb1cce56ac51ca9ee1d26cc29dee20de37767b4eb29895d` |
| Embedded PE SHA256 | `df134ffc0f4f76ced4a3ec6e026718d41d52e29bc91a1e394bc13bb734220b54` |
| Embedded PE offset | Raw shellcode offset `0x1db1` (7601 bytes) |
| Embedded PE XOR key | `0x16` (single-byte, repeating) |
| Loader-to-embedded XOR key | `0x16` (byte at offset 0x1db1 XORs to 0x4d = 'M' / MZ) |

---

## 6. Emulation Results

### Shellcode — Speakeasy (`-r` raw mode)
- **Result**: Partial emulation — resolved 8 APIs successfully, attempted `VirtualAlloc(0x180000000)`, then `VirtualAlloc(NULL)` fallback; speakeasy returned null for both, causing an `invalid_write` at `mov byte ptr [rdx+rcx], al` (0x1ca1) — copy loop failed because destination = NULL
- **APIs traced**: `LdrLoadDll("kernel32.dll")`, `LdrGetProcedureAddress` for `VirtualAlloc`, `VirtualProtect`, `FlushInstructionCache`, `GetNativeSystemInfo`, `Sleep`, `RtlAddFunctionTable`, `LoadLibraryA`; `GetNativeSystemInfo`; `VirtualAlloc` × 2
- **IOCs obtained**: None — emulation stopped before PE loading

### Embedded PE DLL — Generic runner
- **Result**: 0 IOCs — runner invoked DllMain only; `.fptable` section VirtualProtect calls (RW→R→RW→R) observed
- **Speakeasy plain**:
  - `export.load`: resolved `GetProcAddress` for `VirtualProtect`, `WriteProcessMemory`, `ReadProcessMemory`, `CreateRemoteThread`, `CreateProcessA/W`, `OpenProcessToken`, `AdjustTokenPrivileges`, `LookupPrivilegeValueA/W`; then integrity level check failed (`GetSidSubAuthorityCount(NULL)`) → `invalid_read`
  - `export.run` (×2): `GetModuleFileNameW` → `C:\Windows\system32\svchost.exe`; `RtlGetVersion`; `OpenProcessToken`; integrity level check → `invalid_read`
  - **Key finding**: DLL checks hosting process name, OS version, and token integrity level before activating — conditional activation gate

### Any.Run — Shellcode
- **Task ID**: `15634fb8-69d2-4e40-9961-da4e96386902`
- **Score**: 0/100 — No threats detected
- **Reason**: Raw shellcode submitted without loader context; speakeasy-equivalent limitation applies
- **URL**: https://app.any.run/tasks/15634fb8-69d2-4e40-9961-da4e96386902

---

## 7. Sandbox Results

### Embedded PE DLL — ANY.RUN

| Field | Value |
|---|---|
| **Task ID** | `2e2d2250-d13a-4311-b380-5972974934ac` |
| **Score** | **0/100 — No threats detected** |
| **Tags** | None |
| **URL** | https://app.any.run/tasks/2e2d2250-d13a-4311-b380-5972974934ac |
| **IOCs** | Windows telemetry / OCSP noise only — no malicious C2 contacts |
| **Evasion reason** | DLL activation gate: checks process name, OS version, token integrity level; without the shellcode loader invoking exports with correct context, the DLL exits cleanly |

---

## 8. Analyst Notes

### Attribution / Family Identification
The PDB path `C:\Users\Administrator\Desktop\RexRat5   0418\` provides strong internal attribution to a "RexRat" toolkit versioned at "5". The "0418" subdirectory likely indicates an April 2026 development branch. KesaKode signature matching gives **ValleyRAT** (30%) and **PNGPlugLoader** (34%) as the closest known families. ValleyRAT is a sophisticated RAT attributed to the **Silver Fox** APT group (Chinese threat actor), documented targeting Chinese-speaking financial and crypto sector victims — consistent with the OKLink crypto lure used by WEB-OK_Updater.exe. PNGPlugLoader is the ValleyRAT plugin-loading mechanism that delivers additional capability modules as PNG-encoded payloads.

The combination of:
- Extensive Chinese AV product enumeration (360, Baidu, Tencent)
- Chinese messaging app monitoring (QQ.exe, WeChat.exe, WXWork.exe)  
- OKLink blockchain lure (Stage 1)
- Tencent COS HK delivery infrastructure

...strongly suggests **Chinese-speaking victim targeting**, consistent with published ValleyRAT/Silver Fox campaigns.

### Residual Gaps

1. **C2 address not recovered**
   All C2 config and network destination strings are decrypted at runtime via AVX2-vectorized XOR using keys embedded in `.data`. Recovery requires either dynamic execution in a fully controlled environment (not achievable with speakeasy given the integrity-level gate) or manual disassembly of the string decryption routines (sub_18001acf0 and similar). Recommend: full debugger session under SCM invocation of AppClientService in a sandboxed VM.

2. **Plugin payloads unknown**
   PNGPlugLoader architecture implies additional DLL plugins are downloaded post-C2 contact and loaded as PNG-encoded payloads. These secondary plugins carry specific capabilities (additional stealers, lateral movement, etc.) and were not accessible without a live C2.

3. **`HELLOKITTY` string purpose**
   String at runtime-constructed `.data` offset (not present in raw file) — likely a mutex name, campaign marker, or internal identifier. Not verified.

4. **`|0:db|0:lk|0:hs|...|6202:zb|0.1:bb|` protocol**
   This pipe-delimited key:value string is constructed at runtime — appears to be a C2 protocol configuration or session state string. Field abbreviations (db, lk, hs, zb, bb) may encode session parameters. Not decoded without live execution.

5. **`AhJ+ChJ+ChJ+ChJ+CQ*FEQ*FEQ*FEQ*FE` encoded string**
   Non-standard encoding (contains `*` not valid base64) — custom base64-like or XOR-encoded string in `.rdata`. Possible embedded configuration value or key.

6. **Scheduled task persistence path**
   YARA `CreateScheduledTask` hit confirmed but exact task name/path requires live execution. `SHGetFolderPathW` likely targets `%APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup\` or similar.

### Cross-reference: AppClient.exe (SHA256: `f0f5447c...`)
Direct cross-reference is established by exact matching on:
- **Shellcode decryption key** in AppClient.exe's `runInjection` function matches the 32-byte XOR key used to produce `security_audit_decrypted.bin`
- **Matching file path** `C:\ProgramData\AppClient\security_audit_20260514.log` documented verbatim in AppClient.exe's API resolution
- **Download URL** matches: `wwwc86fasgasg-1330789127.cos.ap-hongkong.myqcloud.com/security_audit_20260514.log` documented in WEB-OK_Updater.exe analysis

This file is **Stage 3+4** of the WEB-OK_Updater.exe campaign chain.

---

*Report written: 2026-05-17*  
*Shellcode: `/home/remnux/mal/output/security_audit_decrypted.bin`*  
*Extracted PE: `/home/remnux/mal/output/embedded_pe_decoded.bin`*  
*Shellcode speakeasy: `/home/remnux/mal/output/shellcode_speakeasy.json`*  
*Embedded PE speakeasy: `/home/remnux/mal/output/embedded_pe_speakeasy.json`*  
*Embedded PE ANY.RUN: `/home/remnux/mal/output/embedded_pe_anyrun_analysis.json`*
