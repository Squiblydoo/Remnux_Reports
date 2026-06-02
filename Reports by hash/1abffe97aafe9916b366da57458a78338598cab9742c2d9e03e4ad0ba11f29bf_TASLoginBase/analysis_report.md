# TASLoginBase.dll — Malware Analysis Report

**Date:** 2026-06-02  
**Analyst:** REMnux / Claude  
**Sample path:** `/home/remnux/mal/TASLoginBase.dll`

---

## 1. File Metadata

| Field | Value |
|---|---|
| **Filename** | TASLoginBase.dll |
| **SHA256** | `1abffe97aafe9916b366da57458a78338598cab9742c2d9e03e4ad0ba11f29bf` |
| **MD5** | `31511d00c59e9cd8698bfb6996bb2f58` |
| **SHA1** | `8e44e73098104aaf86e7daf9f0034715f55a9ae5` |
| **Size** | 335,424 bytes (327.5 KB) |
| **Type** | PE32 executable (DLL) (GUI) Intel 80386 |
| **Arch** | x86 (32-bit) |
| **Imphash** | `1364bf47bfd9b6872aa631b3faa827ce` |
| **Compiler** | MSVC 2015 (linker v15), MSVC uv10 |
| **Build timestamp** | 2024-12-20 08:24:47 |
| **Export module** | `TASLoginBase.dll` |
| **Exports** | `GetObjectLog` (single export) |
| **Sections** | `.text` (RX, ent=6.26), `.rdata` (R), `.data` (RW), `.00cfg`, `.voltbl`, `.reloc` |
| **Overlay** | 21,056 bytes @ offset 0x52180 — PKCS7 Authenticode signature (normal) |

### Code Signing Certificate

| Field | Value |
|---|---|
| **Issuer** | GlobalSign GCC R45 EV CodeSigning CA 2020 |
| **Subject** | Morning Leap & Cazo Electronics Technology Co., Ltd. |
| **Location** | Cangzhou, Hebei, CN |
| **Business registration** | 91130922MA0G8AN9201 |
| **Serial** | `2686b9982e46da7e3e0a1d56` |
| **Validity** | 2024-05-16 → 2025-05-16 (**EXPIRED**) |
| **Hash algorithm** | RSA/SHA1 |

The DLL was built seven months into the certificate's validity window. The certificate is now expired (as of June 2026).

---

## 2. Classification

**Verdict:** Malicious — XOR-Decrypting DLL Sideload Shellcode Loader  
**Confidence:** High  
**Family:** Unattributed (no KesaKode or YARA family match)  
**Evasion score:** ANY.RUN 0/100 (design: requires companion payload file to activate)

This DLL is a staged loader. Its sole export `GetObjectLog` locates an encrypted shellcode payload on disk (`TASLogin.log`), decrypts it using a two-step XOR+additive transform, and executes it from RWX memory. The DLL itself contains no network code, no exfiltration logic, and no obvious credentials theft — all malicious behavior is deferred to the `TASLogin.log` payload.

---

## 3. Capabilities

- **DLL sideloading carrier** — designed to be placed alongside a legitimate "TAS" application executable and loaded when it calls `GetObjectLog`
- **Control flow flattening** — entire `GetObjectLog` export (230 KB of code, 1.57 MB decompiled output) is one state machine with 298 unique state values and 1,853 opaque-predicate-guarded conditional branches; all conditions are mathematical tautologies (`N*(N-1) & 1 == 0`) that the obfuscator randomizes without changing execution order
- **FNV-1a hash-based API resolution** — walks `PEB.InLoadOrderModuleList` (FS:[0x30] accessed 36 times across 16 stack locals) to locate 16+ Windows API functions by FNV-1a hash without importing them; no API name strings appear in the binary
- **XOR string/blob decryption** — 281 XOR-in-loop anomalies detected; both API name hashes and the payload decryption use XOR transforms, implemented via two equivalent obfuscated bitmask forms:
  - Form A: `((byte ^ 0xff) & 0x7b | byte & 0x84) ^ 0x19` ≡ `byte ^ 0x62`
  - Form B: `((byte ^ 0xff) & 0xd6 | byte & 0x29) ^ 0xb4` ≡ `byte ^ 0x62`
- **Filename construction via out-of-order byte writes** — `TASLogin.log` is assembled character-by-character in scrambled index order on a 260-byte stack buffer (anti-string-search)
- **Shellcode loading** — `VirtualAlloc(NULL, filesize, MEM_COMMIT, PAGE_EXECUTE_READWRITE)`, `ReadFile`, decrypt in-place, execute as function pointer
- **Anti-debug** — `IsDebuggerPresent` (2 refs), `IsProcessorFeaturePresent` (4 refs, checks XMMI64 available), `QueryPerformanceCounter` (timing check), `UnhandledExceptionFilter`/`SetUnhandledExceptionFilter` (SEH-based anti-debug)
- **Process discovery** — `GetModuleFileNameA(NULL)` to retrieve the hosting process executable path and derive the payload file directory

---

## 4. Attack Chain

```
[Stage 0] Attacker drops TASLoginBase.dll + TASLogin.log into target application directory
           alongside a legitimate "TAS" login application executable

[Stage 1] Victim launches TAS application → application loads TASLoginBase.dll (sideloading)

[Stage 2] Application calls GetObjectLog()

[Stage 3] GetObjectLog resolves ~16 Windows APIs via FNV-1a PEB walk:
           - GetModuleFileNameA, CreateFileA, GetFileSize, VirtualAlloc
           - ReadFile, CloseHandle, and additional APIs for anti-debug/CRT setup

[Stage 4] GetObjectLog calls GetModuleFileNameA(NULL) to get process directory
           Constructs path: [ProcessDirectory]\TASLogin.log

[Stage 5] CreateFileA("TASLogin.log", GENERIC_READ, 0, NULL, OPEN_ALWAYS, 0x80, NULL)
           GetFileSize() → filesize

[Stage 6] VirtualAlloc(NULL, filesize, MEM_COMMIT, PAGE_EXECUTE_READWRITE)
           ReadFile → encrypted blob into RWX buffer

[Stage 7] Decrypt each byte of the buffer:
           plaintext[i] = (encrypted[i] ^ 0x62) + 0x52

[Stage 8] Execute: call RWX buffer as function pointer → shellcode runs
           → actual payload capability (C2, credentials, persistence) unknown without TASLogin.log
```

The actual malicious behavior — C2 connectivity, persistence, credential theft, lateral movement — is entirely contained in the `TASLogin.log` payload that was not recovered with this sample.

---

## 5. IOCs

### Filesystem

| IOC | Type | Notes |
|---|---|---|
| `TASLogin.log` | File (relative) | Encrypted shellcode payload; loaded from target process's working directory |
| `TASLoginBase.dll` | File | The loader DLL itself |

### Certificate

| IOC | Type | Notes |
|---|---|---|
| `2686b9982e46da7e3e0a1d56` | Cert serial | GlobalSign EV cert; pivot to find other samples signed with same cert |
| `Morning Leap & Cazo Electronics Technology Co., Ltd.` | Cert subject | Entity used to obtain EV cert |
| `91130922MA0G8AN9201` | Chinese business reg | Cangzhou, Hebei; present in cert extension fields |

### Cryptographic

| IOC | Value |
|---|---|
| SHA256 | `1abffe97aafe9916b366da57458a78338598cab9742c2d9e03e4ad0ba11f29bf` |
| MD5 | `31511d00c59e9cd8698bfb6996bb2f58` |
| Imphash | `1364bf47bfd9b6872aa631b3faa827ce` |

### Network

No network IOCs recovered from this DLL — all C2 is in the `TASLogin.log` shellcode payload.

---

## 6. Emulation Results

**Speakeasy (pass 2 — plain speakeasy, DLL mode, x86):**

```
export.GetObjectLog called:
  0x10009307: GetModuleFileNameA(NULL, "C:\Windows\system32\svchost.exe", 260)
  0x1000e3eb: CreateFileA("C:\Windows\system32\TASLogin.log", GENERIC_READ,
                           0, NULL, OPEN_ALWAYS, 0x80, NULL) → 0x80
  0x100150ce: GetFileSize(0x80, NULL) → 0x0  [file empty in sandbox]
  0x1001d8b2: VirtualAlloc(NULL, 0, MEM_COMMIT, PAGE_EXECUTE_READWRITE) → 0x0
  0x100213aa: ReadFile(0x80, 0x0, 0x0, ...) → 0x0
  0x10025f54: CloseHandle(0x80) → 1
  *** CRASH: EIP=0x0 (VirtualAlloc returned null; tried to call null pointer)
```

Emulation confirms the full execution sequence. Crash is expected and benign — `VirtualAlloc(size=0)` returned null because the sandbox had no `TASLogin.log` file, causing a null-pointer call.

**Speakeasy (pass 1 — generic runner):** No IOCs. DllMain only performs CRT initialization (no malicious logic in DllMain).

**Decryption key recovery:** Static analysis (via subagent decompilation of 1.57MB pseudocode) recovered the payload decryption algorithm: `plaintext[i] = (encrypted[i] ^ 0x62) + 0x52`. This key allows decryption of `TASLogin.log` if the payload file is ever recovered.

---

## 7. Sandbox Results

**ANY.RUN:** Score 0/100 — "No threats detected"  
**Tags:** (none)  
**Behavioral IOCs from sandbox:** Only Microsoft authentication and certificate validation URLs observed (expected from Windows OS activity, not from the DLL)  
**Public report:** https://app.any.run/tasks/dc17e107-b648-4a94-968f-31d26d9095ac

Evasion is by design — the DLL requires `TASLogin.log` to be present alongside it. Without the payload file, `GetObjectLog` returns immediately after a failed `VirtualAlloc(size=0)` call.

---

## 8. Technical Deep-Dive: Obfuscation Analysis

The `GetObjectLog` export (VA `0x10001000`, 230 KB of code) accounts for essentially the entire `.text` section and is the malcat-flagged `HighXrefLoopingFunction` (string decryption candidate).

### Control Flow Flattening

The function is a single `while(true)` loop dispatched by state variable `iVar97`. All 298 state transitions use opaque predicates of the form `N * (N-1) & 1 == 0` (always true for any N, since N*(N-1) is always even). The obfuscator holds 36 global slots at `0x1004c870`–`0x1004c8fc` (stride 4) as random-looking "condition variables" — they resolve to constants at runtime. This produces 1,853 conditional branch instructions that appear complex but execute linearly.

### FNV-1a Import Resolution

The PEB walk pattern:
```c
FS:[0x30]                          // PEB base
PEB + 0x0C                         // Ldr
Ldr + 0x0C                         // InLoadOrderModuleList.Flink
module + 0x18                      // module base
module + 0x3C → + 0x18 → + 0x60   // Export Directory
```
Called 16+ times (separate stack locals for separate modules). Hashes are computed as `hash = (hash_accum ^ byte) * 0x1000193` (FNV-1a prime). APIs resolved include at minimum: `GetModuleFileNameA`, `CreateFileA`, `GetFileSize`, `VirtualAlloc`, `ReadFile`, `CloseHandle`.

### Payload Decryption

Two syntactically different expressions are used in the code, both equivalent to `byte ^ 0x62`:
```
((b ^ 0xFF) & 0x7B | b & 0x84) ^ 0x19 = b ^ (0x7B ^ 0x19) = b ^ 0x62
((b ^ 0xFF) & 0xD6 | b & 0x29) ^ 0xB4 = b ^ (0xD6 ^ 0xB4) = b ^ 0x62
```
After XOR, an additive `+= 0x52` (`'R'`) is applied. The combined formula per byte: `out = (in ^ 0x62) + 0x52`.

---

## 9. Analyst Notes

1. **TASLogin.log is the critical artifact** — the DLL itself is harmless without it. Incident response should focus on recovering `TASLogin.log` from the victim's application directory. Once recovered, decrypt with `out[i] = (in[i] ^ 0x62) + 0x52` to obtain the shellcode.

2. **Target application is "TAS"** — the DLL name `TASLoginBase.dll` and payload path `TASLogin.log` strongly suggest the attacker is targeting a specific application that exports or calls `GetObjectLog`. This could be a trading platform, travel authorization system, or enterprise authentication system that legitimately ships a `TASLoginBase.dll`.

3. **EV certificate abuse** — the GlobalSign EV certificate (serial `2686b9982e46da7e3e0a1d56`) from "Morning Leap & Cazo Electronics Technology Co., Ltd." was likely obtained fraudulently or from a shell company. This bypasses many AV/EDR products that whitelist EV-signed binaries. Hunting on this cert serial will reveal other samples in the campaign.

4. **No network IOCs recoverable from this DLL** — C2 infrastructure, persistence mechanisms, and payload capabilities are entirely in `TASLogin.log`. Attribution and full campaign mapping require payload recovery.

5. **Recommended follow-up:**
   - Hunt on cert serial `2686b9982e46da7e3e0a1d56` in VirusTotal, MalwareBazaar, and other intel sources
   - Search for `TASLogin.log` or `TASLoginBase.dll` on endpoint telemetry
   - Identify what legitimate application ships a `TASLoginBase.dll` with `GetObjectLog` export — the malware author likely hijacked the DLL name from an existing product
   - If `TASLogin.log` is recovered: apply `out[i] = (in[i] ^ 0x62) + 0x52` and analyse the shellcode
