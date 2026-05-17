# AppClient.exe — Go x64 SYSTEM-to-User Shellcode Injector (Service)
**Analysis Date**: 2026-05-17  
**Analyst**: REMnux/Claude  

---

## 1. File Metadata

| Field | Value |
|---|---|
| **Filename** | AppClient.exe |
| **SHA256** | `f0f5447c44aa3737ce822b017ee8c374c41ecec291d42c9aba34d7038dd4c715` |
| **SHA1** | `a1757f4a423df26f7cd31a61ec676f6d075f24bd` |
| **MD5** | `4c7c48eb4abc8f63a924697130aa5cda` |
| **Size** | 2,029,056 bytes (1.94 MB) |
| **File Type** | PE32+ executable (GUI) x86-64, for MS Windows, 8 sections |
| **Compiler** | Go (Golang) — confirmed by YARA, Go runtime strings, source paths |
| **Timestamp** | 1970-01-01 (zeroed — deliberate) |
| **PE Checksum** | Not set (zeroed) |
| **Code signing** | None |
| **Subsystem** | GUI (disguised — no window APIs imported) |
| **Imphash** | `ebc247a77b4d4a804b261f97a1fd075c` |
| **Imports** | kernel32.dll only (47 functions); all other APIs resolved via Go LazyProc at runtime |
| **Go source paths** | Standard library only; custom package: `main` |
| **Service name** | `AppClientService` (install path: `C:\ProgramData\AppClient\AppClient.exe`) |

---

## 2. Classification

| Field | Assessment |
|---|---|
| **Family** | Novel / unclassified — no KesaKode family match |
| **Role** | Stage-2 payload: SYSTEM service → user-token impersonation → shellcode injection |
| **Confidence** | **High** |
| **Threat level** | Critical — persistent SYSTEM service with process injection |

**Reasoning**: AppClient.exe is a Windows service that runs as SYSTEM. On execution it steals the logged-in user's session token (WTSQueryUserToken), duplicates it, spawns or finds `taskhostw.exe` under the user context, then injects 504 KB of XOR-decrypted x64 shellcode (`security_audit_20260514.log`) into that process via shared-section injection (NtCreateSection/NtMapViewOfSection pattern). The binary is designed to be delivered and run by its parent dropper WEB-OK_Updater.exe — see cross-reference note in §8.

---

## 3. Capabilities

- **Windows service**: Registers via `golang.org/x/sys/windows/svc.Run`; installs as `AppClientService`, auto-start
- **SYSTEM-to-user token impersonation**: `WTSQueryUserToken` to get current user session token → `DuplicateTokenEx` → `AllocateAndInitializeSid` + `CheckTokenMembership` to check admin group membership → `FreeSid`
- **User environment construction**: `CreateEnvironmentBlock` + `GetUserProfileDirectoryW` to build target user's process environment
- **Shellcode payload decryption**: Reads `security_audit_20260514.log` from the EXE's own directory; XOR-decrypts with 32-byte repeating key (see §5)
- **Process targeting**: Polls for `taskhostw.exe` up to 60 times (2-second intervals, max ~2 minutes); falls back to spawning a new `taskhostw.exe` via `CreateProcessAsUserW`/`CreateProcessW` with `EXTENDED_STARTUPINFO_PRESENT` flag
- **Section-based shellcode injection** (NT native API pattern):
  - `NtCreateSection` — create shared anonymous memory section
  - `NtMapViewOfSection` — map into both injector and target process
  - `memmove` — copy decrypted shellcode into mapped view
  - `GetThreadContext` / `SetThreadContext` — hijack target thread instruction pointer
  - `ResumeThread` — resume execution in injected context
  - `VirtualProtect` — mark region executable (`PAGE_EXECUTE_READWRITE = 0x40`)
  - `FlushInstructionCache` — flush CPU cache after write
- **Lure/fallback**: `ShellExecuteW` call (shell32.dll)
- **Process enumeration**: YARA fingerprint `EnumerateProcesses`; peframe confirms `WTSEnumerateProcessesW` pattern
- **String obfuscation**: All API names, DLL names, file paths, and target process name XOR-encoded with key `0x55` and built on the stack at runtime; 32 dynamic string construction sites, 88 XOR-in-loop instances
- **Anti-analysis**: Zeroed timestamp, no PE checksum, no certificate; service exits with code 2 when not invoked by SCM; Go runtime obfuscates control flow

---

## 4. Attack Chain

```
[WEB-OK_Updater.exe — parent dropper]
  │
  ├─ Extracts CIAQAF RAR → C:\ProgramData\AppClient\AcfunLive.exe
  ├─ Downloads AppClient.exe from Tencent COS (HK)          ← this sample
  ├─ Downloads security_audit_20260514.log from same bucket  ← shellcode payload
  └─ sc create AppClientService ... → sc start AppClientService
                        │
[AppClient.exe runs as SYSTEM service]
  │
  ├─ WTSQueryUserToken(active_session) → user_token
  ├─ DuplicateTokenEx(user_token) → dup_token
  ├─ AllocateAndInitializeSid + CheckTokenMembership → check admin
  ├─ CreateEnvironmentBlock + GetUserProfileDirectoryW → user_env
  │
  ├─ os.ReadFile("security_audit_20260514.log")
  ├─ XOR-decrypt with 32-byte key → 504 KB x64 shellcode
  │
  ├─ [Find taskhostw.exe — poll 60x × 2s]
  │       YES: OpenProcess(taskhostw.exe)
  │       NO:  CreateProcessAsUserW(dup_token, taskhostw.exe, SUSPENDED)
  │
  ├─ NtCreateSection → shared section
  ├─ NtMapViewOfSection(self) + NtMapViewOfSection(target) → shared memory
  ├─ memmove(shellcode → shared view)
  ├─ VirtualProtect(PAGE_EXECUTE_READWRITE)
  ├─ GetThreadContext(target_thread)
  ├─ SetThreadContext(target_thread, RIP=shellcode_va)
  ├─ ResumeThread(target_thread)
  └─ FlushInstructionCache → shellcode executes in taskhostw.exe

[security_audit_20260514.log — 504 KB x64 shellcode]
  └─ Executes in taskhostw.exe context (unknown payload — see §8)
```

---

## 5. IOCs

### Network
All network IOCs belong to the **parent dropper** (WEB-OK_Updater.exe). AppClient.exe itself has no hardcoded C2; the shellcode payload (`security_audit_20260514.log`, decrypted) carries the actual C2 — that was not recoverable in this analysis.

| Type | IOC | Notes |
|---|---|---|
| Domain | `wwwc86fasgasg-1330789127[.]cos[.]ap-hongkong[.]myqcloud[.]com` | Tencent COS HK — source of this sample + shellcode |
| URL | `hxxps://wwwc86fasgasg-1330789127[.]cos[.]ap-hongkong[.]myqcloud[.]com/AppClient[.]exe` | This sample's download URL |
| URL | `hxxps://wwwc86fasgasg-1330789127[.]cos[.]ap-hongkong[.]myqcloud[.]com/security_audit_20260514[.]log` | Shellcode payload download URL |

### Filesystem

| Path | Purpose |
|---|---|
| `C:\ProgramData\AppClient\AppClient.exe` | Service install location |
| `C:\ProgramData\AppClient\security_audit_20260514.log` | Encrypted shellcode payload (504 KB) |
| `C:\Windows\System32\taskhostw.exe` | Injection target (existing or spawned) |

### Registry / Service

| Key/Value | Data |
|---|---|
| Service name | `AppClientService` |
| Service path | `C:\ProgramData\AppClient\AppClient.exe` |
| Start type | `auto` (survives reboots) |

### Cryptographic

| Item | Value |
|---|---|
| **Payload XOR key (hex)** | `abcdef1234567890deadbeefcafebabe1234567890abcdef0f1e2d3c4b5a6978` |
| **XOR key length** | 32 bytes (repeating) |
| **Key pattern** | Note: contains embedded constants (`deadbeef`, `cafebabe`) — likely developer fingerprint |
| **String obfuscation key** | `0x55` (single-byte XOR, all stack-built API/path strings) |

### Decrypted Payload Hashes

| Field | Value |
|---|---|
| **File** | `security_audit_decrypted.bin` (504,241 bytes) |
| **SHA256** | `ae76b4a0e3fc7edbefb1cce56ac51ca9ee1d26cc29dee20de37767b4eb29895d` |
| **Format** | Raw x64 shellcode (no MZ header; first bytes `48 83 ec 28 e8 43 1c 00 00` = `sub rsp,28h; call +0x1c48`) |

---

## 6. Emulation Results

### Speakeasy (pass 2)
- **Result**: `ExitProcess(2)` almost immediately
- **Reason**: Go service binary checks for SCM invocation; without the service control manager calling it, exits with error code 2 (`ERROR_FILE_NOT_FOUND` in SCM context). No IOCs obtained.
- **Note**: Even if SCM invocation were emulated, the shellcode injection requires `security_audit_20260514.log` in the same directory — which speakeasy's filesystem stub would not satisfy.

### Generic runner (pass 1)
- **Result**: 0 IOCs — same exit-early behavior.

### Angr (pass 3)
- **Not attempted**: Decryption routine (`runInjection` XOR loop) was successfully decoded by static analysis; angr would provide no additional value here since the key was fully recovered.

### Manual decryption (pass — `AppClient_decrypt_log.py`)
- **Result**: ✅ Full 504 KB shellcode recovered, saved as `security_audit_decrypted.bin`
- **First 9 bytes**: `48 83 ec 28 e8 43 1c 00 00` — valid x64 prologue (`sub rsp,28h; call 0x1c48`), confirming successful decryption

---

## 7. Sandbox Results

| Field | Value |
|---|---|
| **Sandbox** | ANY.RUN |
| **Task ID** | `943ceb20-5d4a-45e3-b008-302381a765a1` |
| **Score** | **0/100 — No threats detected** |
| **Tags** | None |
| **Public URL** | https://app.any.run/tasks/943ceb20-5d4a-45e3-b008-302381a765a1 |
| **Evasion reason** | Service binary exits immediately without SCM invocation; no `security_audit_20260514.log` on sandbox filesystem → injection never executes |
| **Network IOCs** | Only Windows telemetry / OCSP noise — no malicious C2 |

**Assessment**: The 0/100 result is consistent with designed evasion. The sample is fully benign-looking to behavioral sandboxes that cannot replicate the service + payload-file execution environment.

---

## 8. Analyst Notes

### Cross-reference: WEB-OK_Updater.exe (SHA256: `caef6fe8...`)
This sample meets the **direct IOC matching bar** for cross-referencing:
- **Matching download URL** (exact): `wwwc86fasgasg-1330789127.cos.ap-hongkong.myqcloud.com` is documented in the WEB-OK_Updater analysis as the C2 bucket that downloads *this exact file* by name
- **Matching install path**: `C:\ProgramData\AppClient\` and service name `AppClientService` appear verbatim in the WEB-OK_Updater attack chain

AppClient.exe is **Stage 2** of the WEB-OK_Updater campaign (OKLink crypto lure / AcfunLive DLL sideload chain). WEB-OK_Updater delivers AppClient.exe and its payload `.log` file; AppClient.exe provides privilege escalation (SYSTEM → user) and stealthy execution via section injection into a living system process.

### Residual gaps

1. **Shellcode payload (`security_audit_decrypted.bin`) — not analyzed**
   The 504 KB x64 shellcode is the actual RAT/stealer payload — it carries all C2, exfiltration, and persistence logic. It has no MZ header (position-independent or header-stripped PE). Recommended: analyze with malcat as raw x64 code (`analyse_file` on `security_audit_decrypted.bin`), then run speakeasy with `--shellcode`/raw mode, or emulate via `qltool`.

2. **Exact injection subtype**
   The use of `GetThreadContext`/`SetThreadContext`/`ResumeThread` (alongside NtCreateSection/NtMapViewOfSection) is consistent with **Early Bird APC injection** or **thread context hijacking** into a suspended process — but the exact variant depends on whether the target was freshly spawned (suspended) or an existing live thread. Both branches exist in `runInjection`.

3. **Admin check purpose**
   `AllocateAndInitializeSid` (S-1-5-32-544 = Administrators) + `CheckTokenMembership` suggests the shellcode may behave differently based on whether the user is an admin — possibly a higher-privilege injection path or UAC bypass is conditionally triggered.

4. **`NtCreateSection` vs. `VirtualAllocEx` — detection note**
   Section-based injection bypasses many EDR hooks on `VirtualAllocEx`+`WriteProcessMemory`+`CreateRemoteThread`. Detection should look for `NtCreateSection` with `SEC_COMMIT` + cross-process `NtMapViewOfSection` pairs, especially in Go service processes.

5. **`services.exe` string**
   A hardcoded `services.exe` string appears in `.rdata` — possibly for process parent spoofing or service enumeration cross-check.

---

*Report written: 2026-05-17*  
*Decryption script: `/home/remnux/mal/output/AppClient_decrypt_log.py`*  
*Decrypted payload: `/home/remnux/mal/output/security_audit_decrypted.bin`*  
*Speakeasy JSON: `/home/remnux/mal/output/AppClient_speakeasy.json`*  
*ANY.RUN analysis JSON: `/home/remnux/mal/output/AppClient_anyrun_analysis.json`*
