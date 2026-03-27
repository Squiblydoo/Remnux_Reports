# Malware Analysis Report: 1.sys

**Date:** 2026-03-26
**Analyst:** Claude Code (automated static analysis)
**Confidence:** HIGH — Kernel-Mode Reflective PE Loader / Rootkit Component

---

## 1. File Metadata

| Field | Value |
|-------|-------|
| Filename | `1.sys` |
| SHA256 | `b2189b1bd8b861090406566cf2fb0e13e2cc378dd3cd1634d898d65312c03837` |
| SHA1 | `23beb8814a178004a0aa369daac82d31f450f542` |
| MD5 | `6e704594c739021d2675cc32d186ec1c` |
| Size | 178,880 bytes (174.7 KB) |
| Type | PE32+ executable (native) x86-64, Windows kernel driver |
| Compiler | MSVC 2017 linker (RichHeader) |
| Timestamp | **Zeroed** (anti-forensic; PDB path not present) |
| PE Checksum | **Invalid** (tampered) |
| Sections | `.text`, `.rdata`, `.data`, `.pdata`, `INIT` + PKCS7 overlay |
| Entrypoint | `0x583D` (RVA) |

### Certificate
| Field | Value |
|-------|-------|
| Subject | **李忠梅** |
| Organization | 李忠梅 |
| State | 贵州省 (Guizhou Province, China) |
| Country | CN |
| Issuer | Certera Code Signing CA |
| Serial | `00d18ebe424562948192979c3f09bd3d00` |
| Valid | 2023-03-27 → **2026-03-26** (expires today) |
| Hash Algorithm | SHA1 (deprecated) |

> ⚠️ **Same certificate subject/issuer/state as `Login.exe`** (VMProtect Chinese game trojan analyzed 2026-03-26). This is almost certainly the same threat actor or signing infrastructure.

---

## 2. Classification

**Verdict:** MALICIOUS — Kernel-mode reflective PE loader / rootkit dropper
**Confidence:** HIGH
**Family:** Unknown (no YARA family match; custom implant)
**Related sample:** Login.exe (SHA256: `61b682d644...`) — same `李忠梅` cert

### Reasoning
- Kernel driver with no legitimate software identity (zeroed timestamp, invalid checksum, no PDB)
- Signed with a Chinese individual's certificate shared with known malware (Login.exe)
- Implements a complete reflective PE loader in kernel mode (hidden PE loading, reloc processing, export-by-hash)
- Heavy anti-analysis obfuscation (control flow flattening, XOR encryption, import-by-hash, anti-debug)
- All sensitive strings encrypted at rest; resolved dynamically at runtime
- CPUID-based hardware fingerprinting (machine-specific IDs)

---

## 3. Capabilities

### Anti-Analysis / Evasion
- **Control flow flattening**: All functions use a state-machine pattern with 32-bit random state values and opaque predicates (`([global+offset] + -1) * [global+offset] & 1U`), making decompilation extremely difficult
- **Import-by-hash (3 instances)**: API names hidden; resolved at runtime via PE export table walking using hash `0x81d83b80` (malcat identifies as `NetAlertRaise` hash — possibly a collision or non-ntoskrnl target)
- **Anti-debug**: Anti-debugging instructions (capa: B0001.034)
- **String encryption**: All sensitive strings (kernel API names, registry paths, device names) are XOR-encrypted; 79 functions contain XOR-in-loop patterns
- **Dynamic string construction**: 4 dynamically-constructed strings (stack strings)
- **Invalid PE checksum**: Evades simple integrity checks
- **Zeroed timestamp**: Hinders forensic timeline analysis
- **TLS directory**: Unusual in kernel drivers; may execute code before DriverEntry (anti-sandbox/pre-initialization hooking)

### Core Engine: Reflective PE Loader
The driver implements a complete in-memory PE loader (`sub_14001d921`, `sub_14000a437` and related):

1. **PE export walker** (`sub_14000a437`): Walks PE export tables by parsing:
   - `e_lfanew` → NT headers → Export directory (offset 0x88)
   - `AddressOfNames`, `AddressOfNameOrdinals`, `AddressOfFunctions`
   - Hashes each export name via `sub_14000bce9`, compares against target hash
   - Returns resolved function pointer (classic shellcode-style import resolution)

2. **PE base relocation processor** (`sub_14001d921`): Applies base relocations to a loaded PE:
   - Reads relocation block headers and WORD entries
   - Type `0xA` (IMAGE_REL_BASED_DIR64) handled
   - Computes delta `new_base - ImageBase` and patches addresses
   - Uses LOCK-prefixed atomic writes (thread-safe)

3. **XOR decryption engine** (`sub_140017762`, `sub_140008bce`): Byte-by-byte XOR transformation with complex obfuscated constant folding — used to decrypt the embedded PE payload before loading

### Hardware Fingerprinting
- `sub_14001c96d` calls `cpuid_Version_info(1)` to read CPU identification registers
- Applies heavy bit-manipulation to extract CPU brand/model data
- Formats result with `%08X%08X` → 16-character hardware fingerprint string
- Fingerprint likely used to generate machine-specific device name (`\Device\XXXXXXXXXXXXXXXX`) or registry key, making each installation unique and harder to detect via static IoC matching

### Kernel Infrastructure
- **`MmGetSystemRoutineAddress`**: Resolves kernel APIs at runtime by name (beyond the small import table)
- **`NtOpenFile`**: File access — likely to read encrypted payload from disk, or to open kernel objects
- **`\Registry\Machine`**: Registry access in EntryPoint — likely for configuration read or persistence check
- **`KeDelayExecutionThread`**: Timing delay — possible sandbox evasion or synchronization
- **`RtlGetVersion`**: OS version check — gates behavior on Windows version (capa: host-interaction/os/version)

---

## 4. Attack Chain (Reconstructed)

### 4a. Emulation-Confirmed Sequence (speakeasy, 2026-03-27)

The following sequence was directly observed via custom speakeasy emulation (125,087 instructions):

```
DriverEntry @ 0x14000583D
    │
    ├─► [1] ExAllocatePoolWithTag(NonPagedPool, 0xFF0, 'EGAP') → pool @ 0xBD4000
    │        tag 'EGAP' = 'PAGE' reversed (common kernel pool tag)
    │
    ├─► [2] OS version check: RtlGetVersion(buf) → STATUS_SUCCESS
    │
    ├─► [3] XOR decryption loop (~50,000 instructions @ 0x14000a3e0)
    │        Decrypts API name "ZwQuerySystemInformation" into buffer
    │
    ├─► [4] RtlInitUnicodeString(dest, "ZwQuerySystemInformation")
    │
    ├─► [5] MmGetSystemRoutineAddress(&unicode_str) → thunk for ZwQuerySystemInformation
    │        Driver resolves ZwQuerySystemInformation at runtime (not in IAT)
    │
    ├─► [6] ZwQuerySystemInformation(0xB, NULL, 0, &needed)
    │        Class 0xB = SystemModuleInformation (loaded kernel modules)
    │        Returns STATUS_INFO_LENGTH_MISMATCH, needed = 0xA70 bytes
    │
    ├─► [7] ExAllocatePoolWithTag(NonPagedPool, 0xA70, 'EGAP') → buf @ 0xBD5000
    │
    ├─► [8] ZwQuerySystemInformation(0xB, buf, 0xA70, &needed) → STATUS_SUCCESS
    │        Receives list of 9 loaded modules including ntoskrnl.exe base address
    │
    ├─► [9] Parse ntoskrnl.exe PE headers (~60,000 instructions)
    │        Walks export table via sub_14000a437 (hash-based export resolver)
    │        Scans for specific unexported kernel functions by hash (0x81D83B80 × 3)
    │        Likely targets: PsLoadedModuleList, KeServiceDescriptorTable (SSDT), etc.
    │
    ├─► [10] ExFreePoolWithTag(buf @ 0xBD5000) — free module info buffer
    │
    ├─► [11] Post-scan computation (~5,000 instructions @ 0x140002A7A)
    │         Likely: CPUID fingerprint via sub_14001C96D → 16-char machine ID
    │         OR: registry/device setup conditional on successful ntoskrnl scan
    │
    └─► [12] DriverEntry returns STATUS_SUCCESS (0x0)
```

> **Emulation caveat**: Speakeasy maps ntoskrnl.exe as a 7,856-byte stub (base 0x803D0000), far smaller than the real kernel. The driver's hash-based signature scan found no targets, causing it to exit without installing hooks. In a real Windows environment, step [9] would locate SSDT / unexported kernel functions and install rootkit hooks.

### 4b. Inferred Full Attack Chain

```
DriverEntry
    │
    ├─► [1-12] (Emulation-confirmed — see above)
    │
    ├─► [13] CPUID fingerprinting → 16-char machine ID (%08X%08X format)
    │
    ├─► [14] Registry access: ZwOpenKey(\Registry\Machine\...[encrypted path])
    │         Reads configuration or checks prior installation
    │
    ├─► [15] Locate ntoskrnl unexported functions by hash (0x81D83B80):
    │         Candidates: PsLoadedModuleList, KeServiceDescriptorTable, PsActiveProcessHead
    │
    ├─► [16] Install kernel hooks (SSDT modification / DKOM / callback registration)
    │         → Process hiding, file hiding, privilege escalation for Login.exe
    │
    ├─► [17] IoCreateDevice with machine-specific name (derived from CPUID ID)
    │
    └─► [18] XOR-decrypt embedded PE payload → load via reflective loader
              (sub_14001D921 base relocs + sub_14000A437 import resolution)
              Stage 2 = actual rootkit payload (process/file hiding engine)
```

> **Note**: Steps 14-18 were NOT observed in emulation. They are inferred from static analysis. Dynamic analysis with a full Windows kernel (WinDbg/hypervisor) is required to observe them.

---

## 5. IOCs

### File
| Type | Value |
|------|-------|
| SHA256 | `b2189b1bd8b861090406566cf2fb0e13e2cc378dd3cd1634d898d65312c03837` |
| SHA1 | `23beb8814a178004a0aa369daac82d31f450f542` |
| MD5 | `6e704594c739021d2675cc32d186ec1c` |
| imphash | `351ab8b4ed361e491b4dd3b3e749f719` |

### Code Signatures
| Type | Value |
|------|-------|
| API hash | `0x81d83b80` (used 3×; maps to `NetAlertRaise` under malcat's algorithm) |
| CPUID fingerprint format | `%08X%08X` → 16-char machine ID |

### Certificate (pivot indicator — shared with Login.exe)
| Field | Value |
|-------|-------|
| Subject | `李忠梅` |
| Issuer | `Certera Code Signing CA` |
| Serial | `00d18ebe424562948192979c3f09bd3d00` |
| Expiry | `2026-03-26` |

### Kernel / Registry
| Type | Value |
|------|-------|
| Registry path (partial, encrypted) | `\Registry\Machine\...` |

### Network
None observed in static analysis (all strings encrypted; dynamic analysis required).

---

## 6. Analyst Notes

### Emulation Results Summary (speakeasy, 2026-03-27)
| Finding | Value |
|---------|-------|
| Instructions emulated | 125,087 |
| Runtime | 0.54s |
| Entry point return | STATUS_SUCCESS (0x0) |
| APIs resolved via MmGetSystemRoutineAddress | `ZwQuerySystemInformation` only |
| Pool tag (both allocs) | `EGAP` (='PAGE' reversed) |
| Module info query class | `0xB` = SystemModuleInformation |
| Modules in speakeasy env | 9 (ntoskrnl, hal, ntfs, netio, volmgr, disk, tcpip, ndis, 1.sys) |
| ntoskrnl emulated base | `0x803D0000` |
| ntoskrnl emulated size | `0x1EB0` (7,856 bytes — stub only) |

**Emulation limitation**: Speakeasy's ntoskrnl stub is too small for the driver's signature scan to find its target functions. The driver exits cleanly without installing hooks. Full dynamic behavior requires a complete Windows kernel image.

**Emulation script**: `/home/remnux/mal/se_driver_emulate.py`

### Gaps Requiring Dynamic Analysis
1. **Stage 2 payload**: The XOR-encrypted embedded PE was not decrypted — Stage 2 loading is conditional on successful ntoskrnl signature matching (not triggered in emulation). A kernel debugger or hypervisor hook is needed to extract it at runtime.
2. **Hash 0x81D83B80 targets**: What ntoskrnl unexported function this hash resolves to (SSDT? PsLoadedModuleList? Internal callback table?). On a real kernel, `sub_14000A437` would match and return a pointer.
3. **Registry path**: The `\Registry\Machine\...` path is XOR-encrypted; needs decryption via the 79-function XOR engine traced on a real system.
4. **Device/registry names**: Machine-specific values derived from CPUID — can only be observed on the actual infected host.
5. **TLS callbacks**: Execute before DriverEntry; not captured in emulation.
6. **Network activity**: No network IOCs statically visible; Stage 2 may contain C2.
7. **`ZwQu` FLOSS fragments × 2**: Suggest a second `ZwQuery*` API call not observed in emulation (gated behind ntoskrnl scan success).

### Relationship to Login.exe Campaign
- Same `李忠梅` / Certera CA certificate as Login.exe (VMProtect+MugenProtect, QQ:2160076913)
- 1.sys is almost certainly a companion kernel driver loaded by Login.exe or another component of the same campaign
- Login.exe was a 67MB VMProtect-protected game trojan with file-targeting and download capabilities
- 1.sys likely provides the kernel-level rootkit layer (process/file hiding, privilege escalation, AV bypass) to protect and support Login.exe's user-mode operations

### Alternative Hypotheses
- **Legitimate kernel driver**: Highly unlikely given zeroed timestamp, invalid checksum, certificate shared with known malware, reflective loading, and comprehensive anti-analysis
- **Security product**: Some AV/EDR products use similar techniques; however, the certificate attribution to a Chinese individual (not a company) and overlap with Login.exe rules this out

---

## 7. Recommended Actions

1. **Block by certificate serial** `00d18ebe424562948192979c3f09bd3d00` and revoke/report to Certera CA
2. **Hunt for related files** signed by `李忠梅` / Certera CA in your environment
3. **Dynamic analysis** in isolated VM with kernel debugger to extract Stage 2 payload
4. **Correlate with Login.exe** investigation — 1.sys is likely deployed by or alongside it
5. **Memory forensics** on infected systems to find loaded driver and extract decrypted Stage 2 from kernel pool
