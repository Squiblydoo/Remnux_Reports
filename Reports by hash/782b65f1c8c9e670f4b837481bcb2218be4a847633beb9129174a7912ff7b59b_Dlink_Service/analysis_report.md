# Malware Analysis Report — Dlink_Service.exe

**Date:** 2026-05-18  
**Analyst:** Claude (automated)  
**SHA256:** `782b65f1c8c9e670f4b837481bcb2218be4a847633beb9129174a7912ff7b59b`

---

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | `Dlink_Service.exe` |
| SHA256 | `782b65f1c8c9e670f4b837481bcb2218be4a847633beb9129174a7912ff7b59b` |
| SHA1 | `518762260b44bc67ec35be29498ceb10717256fb` |
| MD5 | `8f4d03106857a10c8ef74d84c6413de0` |
| Size | 32,568 bytes (~32 KB) |
| File Type | PE32+ executable (GUI) x86-64, Windows, 6 sections |
| Imphash | `5de8aa0ac23773411f0adc9162860dcd` |
| Build Timestamp | 2026-05-09 08:56:57 UTC |
| Compiler | MSVC 2022 / VS 17.14.2 pre 1.0 (Rich header) |
| Overlay | 12,078 bytes at file offset 0x9688 (PKCS7 cert chain — not payload) |

**Code Signing Certificate:**
- Issuer: GlobalSign GCC R45 EV CodeSigning CA 2020 (GlobalSign nv-sa, BE)
- Subject: 上海得物信息集团有限公司 (Shanghai Dewu Information Group Co., Ltd.)
- Serial: `5dafc1b1129f22c8a47bfa29`
- Validity: 2025-06-12 → 2026-06-13
- Algorithm: RSA / SHA-256

**PDB Path (debug artifact):**
```
C:\Users\30710\Desktop\Mal-dev\ClassicalInjection\x64\Release\ClassicalInjection.pdb
```
The directory name `Mal-dev` and project name `ClassicalInjection` explicitly identify this as a malware development artifact.

---

## 2. Classification

| Field | Value |
|---|---|
| Family | Custom Windows Service Dropper + APC Self-Injector |
| Confidence | **High** |
| Threat Level | Malicious |
| Stage | Stage 1 dropper / loader |

**Reasoning:** The binary installs as a Windows service (`DlinkSvc`), performs an extensive anti-sandbox/anti-analysis check chain, downloads an encrypted payload from a C2 server via WinHTTP, decrypts it with AES-256-CBC, and executes it in the current process via NT API-based APC self-injection. The PDB path unambiguously identifies it as a malware development project. The service name and filename both masquerade as D-Link network hardware software.

---

## 3. Capabilities

### Anti-Analysis / Anti-Sandbox (sub_140001f60)
All checks must pass before the downloader runs:

1. **Filename check**: `lstrcmpiW(GetModuleFileNameW(), "Dlink_Service.exe")` — binary must be named exactly `Dlink_Service.exe`
2. **Anti-debug**: `IsDebuggerPresent()`
3. **Processor count**: `GetSystemInfo()` → requires `dwNumberOfProcessors > 1`
4. **Memory check**: `GlobalMemoryStatusEx()` → requires available physical memory `> 0xFFFFFFFF` (>4 GB)
5. **Uptime check**: `GetTickCount64() > 599999` (machine must have been running >10 minutes)
6. **Process count**: `CreateToolhelp32Snapshot` → requires >29 running processes
7. **Username/hostname blocklist**: `GetUserNameW` + `GetComputerNameW`, then `wcsstr` (case-insensitive) against 14 sandbox indicator strings:

   ```
   sandbox, malware, virus, cuckoo, sample, analyst, vmuser,
   vboxuser, happytime, andy, currentuser, wilbert, phil, ronnit, klone
   ```

### Download (sub_140001290)
- WinHTTP download from C2 URL (assembled from 18 wide string fragments split in `.rdata`)
- User-Agent: `Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36`
- Saves to `C:\Windows\Temp\dlink.temp`
- Deletes the file after reading it into memory

### Decrypt (sub_140001000)
- Algorithm: AES-256-CBC via `BCryptOpenAlgorithmProvider` / `BCryptGenerateSymmetricKey` / `BCryptDecrypt`
- Key (32 bytes, XOR-obfuscated in `.rdata`): **`000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F`**
- IV (16 bytes, XOR-obfuscated in `.rdata`): **`000102030405060708090A0B0C0D0E0F`**
- Both key and IV are NIST standard AES test vectors — strongly suggests this is a template/builder default or development artifact. Production deployments likely use different keys.

### Inject (sub_140001520) — APC Self-Injection
Classic NT-API self-injection sequence (all functions resolved dynamically via `GetModuleHandleW("NTDLL.DLL")` + `GetProcAddress`):

1. `NtAllocateVirtualMemory` — allocate RW memory in current process
2. `NtWriteVirtualMemory` — write decrypted payload
3. `NtProtectVirtualMemory` — change protection to RX (`0x20`)
4. `NtQueueApcThread` — queue APC pointing to payload on current thread handle
5. `SleepEx(INFINITE, TRUE)` — trigger alertable wait so APC executes

### Windows Service (sub_140002320 / sub_140002450)
- Service name: `DlinkSvc`
- Service control handler: `sub_140002250` (responds only to `SERVICE_CONTROL_STOP = 1`)
- Service type: `0x10` (WIN32_OWN_PROCESS), `dwCurrentState: 4` (RUNNING)
- Entrypoint calls `StartServiceCtrlDispatcherW("DlinkSvc", sub_140002320)` — must be invoked by the Service Control Manager; runs as standalone EXE under SCM

### C2 URL Obfuscation
The C2 URL is split into 18 small Unicode wide-string fragments stored in `.rdata` at 4–12 byte intervals (VA `0x14000457c` through `0x1400045ec`) and assembled at runtime via string concatenation, defeating simple string-search detection.

---

## 4. Attack Chain

```
[Installer / Dropper unknown] → registers DlinkSvc service
         ↓
DlinkSvc service starts (Dlink_Service.exe)
         ↓
sub_140001f60 — 7-layer sandbox/analysis check:
  ✗ → service exits cleanly (no artifacts)
  ✓ → proceed
         ↓
sub_140001290 — WinHTTP GET http://43.133.164.200:8083/dlink.so
  → saves encrypted blob to C:\Windows\Temp\dlink.temp
         ↓
sub_140001000 — AES-256-CBC decrypt with key 00..1F / IV 00..0F
  → produces raw shellcode or PE payload in heap
         ↓
DeleteFileW(C:\Windows\Temp\dlink.temp)
         ↓
NT API self-APC injection:
  NtAllocateVirtualMemory → NtWriteVirtualMemory → NtProtectVirtualMemory
  → NtQueueApcThread → SleepEx(INFINITE, TRUE)
  → payload executes in DlinkSvc process context
```

---

## 5. IOCs

### Network (Defanged)

| Type | Value | Notes |
|---|---|---|
| URL | `hxxp://43[.]133[.]164[.]200:8083/dlink[.]so` | C2 payload download |
| IP | `43[.]133[.]164[.]200` | Tencent Cloud (AS132203), CN |
| Port | `8083` | Non-standard HTTP port |
| User-Agent | `Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36` | WinHTTP UA |

### Filesystem

| Path | Purpose |
|---|---|
| `C:\Windows\Temp\dlink.temp` | Encrypted payload staging file (deleted post-inject) |
| `C:\Windows\system32\Dlink_Service.exe` | Service executable install path (observed in emulation) |

### Registry / Service

| Artifact | Value |
|---|---|
| Service Name | `DlinkSvc` |
| Service Type | WIN32_OWN_PROCESS |
| Service Start Type | (set by upstream installer, not in this binary) |

### Cryptographic Artifacts

| Artifact | Value |
|---|---|
| AES-256 Key | `000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F` |
| AES IV | `000102030405060708090A0B0C0D0E0F` |
| Code-Sign Cert Serial | `5dafc1b1129f22c8a47bfa29` |

### Build Artifacts

| Artifact | Value |
|---|---|
| PDB Path | `C:\Users\30710\Desktop\Mal-dev\ClassicalInjection\x64\Release\ClassicalInjection.pdb` |
| Developer ID | `30710` (Windows username) |

---

## 6. Emulation Results

**Speakeasy (pass 1 — generic runner):** No IOCs extracted. Execution stalled at `GetSystemInfo` (processor count check returned 1, requirement is >1).

**Speakeasy (pass 2 — direct):** Confirmed execution flow:
- `StartServiceCtrlDispatcherW("DlinkSvc", 0x140002320)` → registered
- `RegisterServiceCtrlHandlerW("DlinkSvc")` → registered
- `lstrcmpiW("Dlink_Service.exe", "Dlink_Service.exe")` → 0 (pass)
- `IsDebuggerPresent()` → 0 (pass)
- `GetSystemInfo()` → emulation stopped (1 processor < required 2)

All network, file, registry, mutex IOCs recovered via FLOSS static deobfuscation rather than emulation.

---

## 7. Sandbox Results

**ANY.RUN:** Task ID `bc06431b-f87b-48e7-94f5-7ee92a5a6c07`  
**Public report:** `https://app.any.run/tasks/bc06431b-f87b-48e7-94f5-7ee92a5a6c07`  
**Score:** 0/100 — "No threats detected"  
**Family tags:** (none)

**Behavioral IOCs from ANY.RUN:** None (no C2 traffic observed). All network events were Windows OS telemetry and OCSP requests.

**Explanation:** The binary's 7-layer anti-sandbox check blocked all malicious behavior in the ANY.RUN VM. The check for `>29 running processes`, `>4 GB available RAM`, and `>10 minutes uptime` all fail on a freshly spawned sandbox. The service also requires SCM registration to execute the payload path.

---

## 8. MITRE ATT&CK

| Technique | ID | Evidence |
|---|---|---|
| Masquerading — Match Legitimate Name | T1036.001 | `DlinkSvc` / `Dlink_Service.exe` impersonating D-Link software |
| Subvert Trust Controls — Code Signing | T1553.002 | EV GlobalSign cert for 上海得物信息集团有限公司 |
| Virtualization/Sandbox Evasion — System Checks | T1497.001 | 7-layer check: filename, debugger, CPU, RAM, uptime, PID count, username |
| Obfuscated Files or Information | T1027 | C2 URL fragmented across 18 rdata slots; AES key XOR-obfuscated |
| Shared Modules | T1129 | Dynamic NTDLL resolution at runtime |
| Download Tool Transfer | T1105 | WinHTTP C2 download |
| Create or Modify System Process — Windows Service | T1543.003 | `DlinkSvc` service registration |
| Process Injection — APC Injection | T1055.004 | NtQueueApcThread self-APC injection |
| Encrypted Channel | T1573 | AES-256-CBC payload encryption |
| Indicator Removal — File Deletion | T1070.004 | `DeleteFileW(C:\Windows\Temp\dlink.temp)` after injection |

---

## 9. Analyst Notes

1. **Key observation — developer placeholder keys**: The AES-256 key (`00 01 02..1F`) and IV (`00 01..0F`) are NIST standard test vectors. This strongly suggests either:
   - A builder/template where per-deployment keys are injected at build time (this sample is a base template), or
   - Early development/test build that escaped into the wild.
   The key may differ in production deployments.

2. **Payload naming — `dlink.so`**: The downloaded payload is named `dlink.so` (a Linux shared object extension). In the Windows context this is either deliberate mislabeling to confuse analysts, or the C2 server serves a different payload type (e.g., shellcode) regardless of filename. The `.so` extension serves no Windows runtime purpose.

3. **Service masquerade**: The `DlinkSvc` service name and `Dlink_Service.exe` filename are designed to mimic D-Link router management software, targeting enterprise networks where such software might legitimately appear. Combined with an EV code-signing certificate, this provides a high-quality masquerade.

4. **Certificate subject**: 上海得物信息集团有限公司 is a major Chinese e-commerce platform ("Dewu" / "毒" sneaker resale app). This is almost certainly certificate abuse — either the cert was issued fraudulently, stolen, or the company's systems were compromised. The developer workspace username `30710` provides a small pivot indicator.

5. **C2 availability**: `43.133.164.200` (Tencent Cloud AS132203, CN) was not checked live. The C2 may be down — the sample may be part of an active or recently active campaign.

6. **Upstream installer unknown**: This binary only handles service execution; the SCM registration (installing the service key, setting start type) must be performed by an upstream component not present in this analysis. Recovery of the installer is recommended to understand the full delivery chain.

7. **Payload unknown**: The actual shellcode/PE served at `http://43.133.164.200:8083/dlink.so` was not retrieved. Network-level detection should block this IP/port. Decryption tooling is available (key recovered statically).
