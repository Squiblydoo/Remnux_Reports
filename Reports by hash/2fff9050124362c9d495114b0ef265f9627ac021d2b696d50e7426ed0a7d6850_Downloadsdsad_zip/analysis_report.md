# Malware Analysis Report: Downloadsdsad.zip — AES-128-CBC Shellcode Loader via Python DLL Sideloading

**Date:** 2026-04-25  
**Analyst:** REMnux Automated Analysis  
**Report Path:** `/home/remnux/mal/output/Downloadsdsad_zip_analysis_report.md`

---

## 1. File Metadata

### Archive
| Field | Value |
|-------|-------|
| Filename | `Downloadsdsad.zip` |
| SHA256 | `416a07ddd669d2b71e2d28909ee2c72743663c447aa382ab6f40c9f08086bffa` |
| MD5 | `375638170a68ff8f36906ed8d4d3cfcc` |
| Size | 129,410 bytes |
| Type | ZIP archive (deflate compression) |
| Contents | `python310.dll` + `pythona.exe` |

### python310.dll (Primary Malicious Component)
| Field | Value |
|-------|-------|
| SHA256 | `2fff9050124362c9d495114b0ef265f9627ac021d2b696d50e7426ed0a7d6850` |
| MD5 | `015d8900a5ae2efca60030117738dcef` |
| SHA1 | `d62fb685c4253cf92ebdadfdc0679f257fbab586` |
| Size | 157,848 bytes |
| Type | PE32+ DLL x86-64 |
| Internal Module Name | `Suk30.dll` (not `python310.dll`) |
| Export | `Py_Main` (infinite loop keepalive) |
| Imphash | `8a8cff8233930ae2a4c32522606758a6` |
| Compiler | MSVC 2022 |
| Build Timestamp | 2026-01-29 12:55:45 |
| Certificate Subject | **Oh Development** |
| Certificate Org | Oh Development, Île-de-France, Louveciennes, FR |
| Certificate Issuer | SSL.com EV Code Signing Intermediate CA RSA R3 |
| Certificate Serial | `11e603b92a63487d692ad9519a0382fe` |
| Certificate Validity | 2025-05-21 → 2026-05-21 (1-year EV) |

### pythona.exe (Legitimate Sideload Host)
| Field | Value |
|-------|-------|
| SHA256 | `81918ea5fa5529f04a00bafc7e3fb54978a0b7790cfc7a5dad9fa9640666560a` |
| Size | 101,656 bytes |
| Type | PE32+ EXE x86-64 GUI |
| Identity | Legitimate **Python 3.10.11** `pythonw.exe` |
| Certificate Subject | Python Software Foundation |
| Certificate Issuer | DigiCert Trusted G4 Code Signing RSA4096 SHA384 2021 CA1 |
| Certificate Serial | `071f141b8b300d25f314eb230cd0d1dd` |
| Certificate Validity | 2022-01-17 → 2025-01-15 (expired, but file is genuine) |
| Critical Import | `python310.dll!Py_Main` — **the sideload trigger** |

---

## 2. Classification

| Field | Assessment |
|-------|------------|
| **Family** | Custom shellcode stager / loader |
| **Confidence** | **High** (full decompile of attack chain) |
| **YARA Matches** | MSVC 2022 linker only; no family attribution |
| **KesaKode** | No matches |
| **Role** | Second-stage stager; requires separately-delivered `C:\ProgramData\Appver.DAT` payload |

The DLL is a bespoke, EV-signed shellcode stager implementing Python DLL sideloading. It has no network communication of its own — all C2 capability is deferred to an encrypted blob (`Appver.DAT`) that must be pre-staged on the target. This design makes the component maximally evasive: without the payload file, it is entirely inert.

---

## 3. Attack Chain

### Stage 0 — Delivery
The ZIP is delivered to the victim. Both files are extracted to the same directory. pythona.exe is a legitimate, unmodified Python 3.10.11 launcher that Windows SmartScreen may not flag (the PSF cert is expired but the file is authentic).

### Stage 1 — DLL Sideloading (T1574.001)
When pythona.exe runs, Windows loads `python310.dll` from the same directory before the System32 version. `DllMain` is called with `DLL_PROCESS_ATTACH`.

**Sideload mechanics:**
- pythona.exe imports `python310.dll!Py_Main` — the exact symbol name from the legitimate Python C API
- The malicious DLL exports `Py_Main` as an infinite loop (`do {} while(true);`) — this prevents pythona.exe from exiting, keeping the host process alive as a process shell

### Stage 2 — Thread Launch (T1055)
`DllMain` on `DLL_PROCESS_ATTACH`:
1. Constructs `"kernel32.dll"` via byte-push stack string (anti-static-analysis)
2. Calls `LoadLibraryA` on the constructed string
3. Resolves `CreateThread` via `GetProcAddress`
4. Spawns background thread running `sub_1800029f0`

### Stage 3 — Dynamic API Resolution (T1027, T1129)
The shellcode loader thread (`sub_1800029f0`) performs its own independent API resolution to avoid import table visibility:
- Constructs `"kernel32.dll"` via stack string again
- Resolves: `GetModuleHandleA`, `GetProcAddress`, `CreateFileA`, `ReadFile`, `CloseHandle`, `VirtualAlloc`, `VirtualProtect`, `VirtualFree`, `WaitForSingleObject`, `CreateThread`
- Additionally loads `ntdll.dll` and resolves `NtCreateThreadEx` (avoids higher-level thread creation hooks)

### Stage 4 — Payload Decryption (T1027.002)
```
CreateFileA("C:\ProgramData\Appver.DAT", GENERIC_READ, FILE_SHARE_READ, ...)
→ ReadFile() [loop, 512-byte chunks, dynamic realloc]
→ AES-128-CBC decrypt:
    Key: "Keet96vUkMdJThac"  (16 bytes, hardcoded)
    IV:  "ivnpFrICQCEKklCi"  (16 bytes, hardcoded)
→ PKCS#7 unpad
```

The decrypted buffer is the next-stage shellcode (or PE). The AES key and IV are embedded as plaintext strings in the binary, extractable via static analysis.

### Stage 5 — In-Process Shellcode Execution (T1055.002)
```
VirtualAlloc(NULL, size, MEM_COMMIT|MEM_RESERVE, PAGE_READWRITE)
→ memcpy(decrypted_payload → allocated_region)
→ VirtualProtect(region, size, PAGE_EXECUTE_READ, &old)
→ NtCreateThreadEx(&handle, THREAD_ALL_ACCESS, NULL, GetCurrentProcess(),
                   shellcode_entry, NULL, 0, ...)
→ WaitForSingleObject(handle, INFINITE)
→ VirtualFree + cleanup
```

Execution is entirely within the current process (`pythona.exe`). `NtCreateThreadEx` is used instead of `CreateThread` to bypass user-mode security hooks on the higher-level API.

---

## 4. Capabilities

| Capability | Detail |
|-----------|--------|
| **DLL Sideloading** | Masquerades as `python310.dll`, exports `Py_Main` |
| **Code Signing Abuse** | SSL.com EV cert (Oh Development, FR) — 1-year cert |
| **Stack String Obfuscation** | `"kernel32.dll"` pushed byte-by-byte in multiple functions |
| **Dynamic API Resolution** | 32 runtime `GetProcAddress` calls; no sensitive imports visible |
| **AES-128-CBC Decryption** | Full Rijndael implementation (Td0–Td3 lookup tables) |
| **In-Process Shellcode** | VirtualAlloc → VirtualProtect → NtCreateThreadEx |
| **Anti-Debug** | `IsDebuggerPresent`, `IsProcessorFeaturePresent`, SEH filter hooks |
| **Anti-Sandbox/AV** | Payload-gated: completely inert without `Appver.DAT` |
| **Process Keepalive** | `Py_Main` infinite loop prevents host process exit |

---

## 5. IOCs

### Filesystem
| Type | Value | Notes |
|------|-------|-------|
| File path | `C:\ProgramData\Appver.DAT` | Encrypted shellcode staging path |
| File | `python310.dll` (malicious) | SHA256: `2fff9050...` |
| File | `pythona.exe` | Legitimate Python 3.10.11 launcher, abused as host |

### Cryptographic Indicators
| Type | Value |
|------|-------|
| AES-128-CBC Key | `Keet96vUkMdJThac` |
| AES-128-CBC IV | `ivnpFrICQCEKklCi` |

### Code Signing
| Field | Value |
|-------|-------|
| Subject | Oh Development |
| Issuer | SSL.com EV Code Signing Intermediate CA RSA R3 |
| Serial | `11e603b92a63487d692ad9519a0382fe` |
| Validity | 2025-05-21 → 2026-05-21 |

### Network IOCs
None directly in the DLL. All network activity is deferred to the `Appver.DAT` shellcode payload, which was not available for analysis.

---

## 6. Emulation Results

### Speakeasy (Pass 1 — Generic Runner)
- **Result:** 0 IOCs captured
- **Reason:** `CreateFileA("C:\ProgramData\Appver.DAT")` returns INVALID_HANDLE_VALUE in the emulated environment; loader exits cleanly at the file-open guard. This is by design — the DLL is intentionally inert without the payload.

### Speakeasy (Pass 2 — Plain)
- Not attempted (same file-open guard would block execution path)

---

## 7. Sandbox Results

**ANY.RUN** (task `6540fb44-9bca-42fb-9a7e-5e96ac9b3cf1`):
- **Score:** 0/100 — No threats detected
- **Tags:** None
- **Reason:** Same as emulation — `C:\ProgramData\Appver.DAT` not present; DLL executes but silently does nothing
- **Public URL:** `https://app.any.run/tasks/6540fb44-9bca-42fb-9a7e-5e96ac9b3cf1`

**Evasion effectiveness:** Both dynamic analysis platforms gave a clean verdict, demonstrating that the payload-gating strategy is effective against automated sandbox analysis. Static analysis and manual decompilation were required to reveal the malicious logic.

---

## 8. MITRE ATT&CK

| Technique | ID | Detail |
|-----------|-----|--------|
| DLL Search Order Hijacking | T1574.001 | `python310.dll` placed alongside `pythona.exe` |
| Masquerading | T1036.001 | Named as legitimate Python DLL, exports `Py_Main` |
| Code Signing | T1553.002 | EV cert (Oh Development, SSL.com) |
| Obfuscated Files or Information | T1027 | Stack strings for API names |
| Encrypted/Encoded File | T1027.002 | AES-128-CBC encrypted `Appver.DAT` payload |
| Process Injection (Thread Execution Hijacking) | T1055 | In-process via NtCreateThreadEx |
| Shared Modules | T1129 | DLL sideloading execution |
| Dynamic Resolution | T1027.010 | All sensitive APIs resolved at runtime |

---

## 9. Analyst Notes

### Key Gaps
- **`C:\ProgramData\Appver.DAT` not available.** This is the critical missing piece. The shellcode payload is fully encrypted and was not present in the ZIP or recoverable through emulation. The actual C2 infrastructure, persistence mechanism, and final payload (stealer, RAT, ransomware) cannot be determined from this component alone.
- **Delivery vector unknown.** The ZIP is likely delivered via spearphishing, malvertising, or a download from a compromised site. A separate dropper likely stages `Appver.DAT` before or alongside this ZIP.

### Notable Design Choices
- Using a **real, signed Python launcher** (pythona.exe) as the host is particularly clean — the process that appears in Task Manager is a legitimate Microsoft-signed binary, and the sideloaded DLL itself carries an EV signature. Detection requires inspecting the DLL load path or the DLL's true identity, not just checking process signatures.
- The `Py_Main` **infinite loop** is elegant: instead of creating a persistent process that sits idle, the malware abuses the normal control flow of the Python launcher to keep itself alive without any additional persistence mechanism needed at this stage.
- **Payload-gating** (requiring `Appver.DAT`) means this component can be distributed widely without triggering automated defenses; only targets that also receive the payload file are compromised.

### Recommended Follow-Up
1. Hunt for `C:\ProgramData\Appver.DAT` on any potentially compromised hosts.
2. If `Appver.DAT` is recovered, decrypt with AES-128-CBC: key=`Keet96vUkMdJThac`, IV=`ivnpFrICQCEKklCi` to obtain and analyze the shellcode payload.
3. Hunt for SSL.com EV cert serial `11e603b92a63487d692ad9519a0382fe` across other samples.
4. Monitor for `pythona.exe`/`pythonw.exe` running from non-standard directories (outside `C:\Python310\` or Python install paths).
5. YARA hunt for the hardcoded AES key/IV pair (`Keet96vUkMdJThac`, `ivnpFrICQCEKklCi`) across enterprise file systems and memory.

### Decryption Script
To decrypt `Appver.DAT` if recovered:
```python
from Crypto.Cipher import AES
from Crypto.Util.Padding import unpad

KEY = b"Keet96vUkMdJThac"
IV  = b"ivnpFrICQCEKklCi"

with open("Appver.DAT", "rb") as f:
    data = f.read()

cipher = AES.new(KEY, AES.MODE_CBC, IV)
plaintext = unpad(cipher.decrypt(data), 16)

with open("Appver_decrypted.bin", "wb") as f:
    f.write(plaintext)
print(f"Decrypted {len(plaintext)} bytes → Appver_decrypted.bin")
```
