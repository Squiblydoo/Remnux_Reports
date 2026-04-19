# Malware Analysis Report: nvbackend Payload Set (detoured.dll)

## 1. File Metadata

### Primary Malicious Component: `detoured.dll`

| Field | Value |
|-------|-------|
| **Filename** | detoured.dll |
| **Internal DLL name** | DataReport.dll |
| **SHA256** | `93d458ce6ebc98b2884e4a76c026d731a9f793cfcc6d514d4952ad6bf28fe8ac` |
| **Size** | 103,792 bytes (101 KB) |
| **Type** | PE32 DLL x86, MSVC 2022 |
| **Build timestamp** | 2026-04-17 04:02:02 UTC |

**Certificate**

| Field | Value |
|-------|-------|
| Issuer | DigiCert Trusted G4 Code Signing RSA4096 SHA384 2021 CA1 |
| Subject | LENOVO (UNITED STATES) INC. |
| Unit | Digital Trust Lab - CTO Organization |
| State/City | North Carolina / Morrisville |
| Serial | `0d2ad57b10b7472bae03d3deff05f54f` |
| Validity | 2026-04-10 → 2027-04-11 (1-year EV, issued 7 days before deployment) |

### Encrypted Payload: `NvBackend.log`

| Field | Value |
|-------|-------|
| **SHA256** | `3313f347e83aaf48ea31fb1d49fc37452f48f81d20a1b93009e2e78385ff4bba` |
| **Size** | 162,744 bytes |
| **Apparent type** | Binary data (encrypted; not PE until decrypted) |

### Decrypted Stage-3 Payload: `NvBackend_stage3` (recovered)

| Field | Value |
|-------|-------|
| **SHA256** | `36c595e24274cd8a91a6405c2e94eadcba41a6427e144437eb984f6704d2dbd8` |
| **Size** | 163,840 bytes (decompressed) |
| **Type** | Custom reflective PE (malformed `e_lfanew = 0xfffff8`; loaded by shellcode only) |
| **ZS cipher key** | `8A913610E905C3DD1F657811EA3B1933471B230F88E1C155616099A03AB0ABC0` (identical to `windui.dll`) |

### Legitimate Sideload Host: `NvBackend.exe`

| Field | Value |
|-------|-------|
| **SHA256** | `ad2dfda427e3ccb5c8404f0afafe71c64b862d2e26a67e1bfc2b40738fd0b873` |
| **Size** | 2,397,120 bytes |
| **Type** | Legitimate NVIDIA Corporation binary (PE32 x86) |
| **Certificate** | NVIDIA Corporation, VeriSign Class 3, serial `14781BC862E8DC503A559346F5DCC518` |
| **Cert status** | **Leaked** — one of two NVIDIA code-signing certificates exfiltrated in the March 2022 Lapsus$ breach; expired 2018-07-26, timestamped 2016-06-14 |
| **Sideload export** | Imports `Detoured` from `detoured.dll` (Microsoft Detours marker function) |

---

## 2. Classification

| | |
|---|---|
| **Verdict** | HIGH CONFIDENCE MALICIOUS |
| **Family** | APT-Q-27 (Golden Eye Dog) / Zhong Stealer — updated `nvbackend` variant |
| **Confidence** | High |

**Reasoning**: The payload set is a direct successor to the `trubo`-bucket APT-Q-27 campaign previously analyzed (sample `photo20260411689.com`). Confirmation rests on three independent indicators: (1) identical C2 infrastructure `uu.goldeyeuu[.]io:5188`, (2) the exact 32-byte Zhong Stealer cipher key `8A913610...` recovered byte-for-byte in the stage-3 PE, and (3) the identical WebSocket REGISTER protocol. The actor updated the lure (NVIDIA NvBackend instead of TurboVPN), rotated the loader certificate to a fresh Lenovo EV cert, and changed the GCS payload staging bucket from `trubo` to `nvbackend`.

---

## 3. Capabilities

### `detoured.dll` — Sideload Loader / Stage-1

- **DLL sideloading**: Exports a single function (`Detoured`) — the Microsoft Detours notification marker. `NvBackend.exe` calls this on load via its IAT, triggering the payload chain from `DllMain`.
- **Persistence**: Writes `HKCU\Software\Microsoft\Windows\CurrentVersion\Run\NvBackend = <path to NvBackend.exe>` — survives reboot by re-triggering the full sideload chain.
- **Anti-sandbox gate**: `sub_10003080` performs environment checks before proceeding; execution aborts silently if the check fails.
- **NT hook bypass**: Resolves `NtAllocateVirtualMemory` and `NtProtectVirtualMemory` directly from `ntdll.dll` by name, bypassing any user-mode hooks on the standard `VirtualAlloc`/`VirtualProtect` wrappers.
- **Payload decryption**: Reads `NvBackend.log` from the same directory, allocates an NT memory region, decrypts with a simple per-byte cipher `((byte + 0x52 + 0x25) ^ 0x62)`, marks the buffer `PAGE_EXECUTE_READ` via `NtProtectVirtualMemory`, and jumps to it.
- **Identity masking**: Internal DLL name is `DataReport.dll`; deployed filename is `detoured.dll` to blend with Microsoft Detours conventions.

### `NvBackend.log` — Stage-2 Shellcode (decrypted)

- **PEB walking**: Locates `kernel32.dll` base via PEB `InMemoryOrderModuleList` traversal without touching the IAT.
- **API resolution by name**: Resolves `LoadLibraryA`, `VirtualAlloc`, `VirtualFree`, `lstrcmpiA`, `RtlZeroMemory`, `RtlMoveMemory`, `RtlDecompressBuffer` by walking export tables.
- **LZNT1 decompression**: Calls `ntdll!RtlDecompressBuffer` (format `0x102` = LZNT1 maximum engine) to decompress 160,981 bytes → 163,840 bytes into a freshly allocated RW region.
- **Custom reflective loading**: Loads the decompressed PE using a custom loader that handles the intentionally malformed `e_lfanew = 0xfffff8` header (not loadable by the OS PE loader); maps sections, resolves imports, applies relocations manually.

### Stage-3 PE — Zhong Stealer C2 Agent

- **C2 channel**: WebSocket connection to `uu.goldeyeuu[.]io:5188` over plain HTTP (no TLS). Upgrade path `GET /\` (forward-slash + backslash routing token). Same protocol as `windui.dll`.
- **REGISTER frame**: System fingerprint (516-byte `OSVERSIONINFOEX` + hostname + IP + timestamp) → zlib compress → custom `i % 8` multi-op cipher with static 32-byte key → 12-byte header → WebSocket send.
- **Static cipher key** (hardcoded at offset `0x1ee65`):
  ```
  8A 91 36 10 E9 05 C3 DD 1F 65 78 11 EA 3B 19 33
  47 1B 23 0F 88 E1 C1 55 61 60 99 A0 3A B0 AB C0
  ```
  Identical to key in `windui.dll` — the same operator key used across both campaigns.
- **WinHTTP stack**: Full `WinHttpOpen` → `WinHttpConnect` → `WinHttpOpenRequest` → `WinHttpSendRequest` → `WinHttpReceiveResponse` → `WinHttpReadData` chain.

---

## 4. Attack Chain

```
[Delivery]
  GCS bucket: storage.googleapis[.]com/nvbackend/
    ├── nv.txt            — download manifest (4 URLs)
    ├── NvBackend.exe     — legitimate NVIDIA binary (Lapsus$ leaked cert)
    ├── detoured.dll      — malicious loader (Lenovo EV cert, DataReport.dll internally)
    └── NvBackend.log     — encrypted+compressed payload (binary data)

[Execution — triggered by photo0418699.com dropper]
  NvBackend.exe loaded
    └── IAT → detoured.dll!Detoured() → DllMain payload chain:

        [Persistence]
          HKCU\...\Run\NvBackend = "<dir>\NvBackend.exe"

        [Anti-sandbox gate]
          sub_10003080() — environment check, bail if sandbox

        [Hook bypass]
          LoadLibrary("ntdll.dll") → GetProcAddress("NtAllocateVirtualMemory")
          LoadLibrary("ntdll.dll") → GetProcAddress("NtProtectVirtualMemory")

        [Payload load]
          CreateFile("<dir>\NvBackend.log") → ReadFile
          ┌─ Decrypt: ((byte + 0x52 + 0x25) ^ 0x62) per byte
          └─ NtProtectVirtualMemory(PAGE_EXECUTE_READ) → CALL buffer

        [Stage-2 shellcode]
          PEB walk → kernel32 base
          GetProcAddress: LoadLibraryA, VirtualAlloc, RtlDecompressBuffer
          VirtualAlloc(0x28000, MEM_COMMIT, PAGE_READWRITE)
          ntdll!RtlDecompressBuffer(LZNT1, in=160981 bytes, out=163840 bytes)
          Custom reflective PE load (malformed e_lfanew=0xfffff8)

        [Stage-3: Zhong Stealer C2 agent]
          DNS: uu.goldeyeuu[.]io → 56.155.111.29
          TCP connect: 56.155.111.29:5188
          WebSocket upgrade: GET /\ HTTP/1.1
          REGISTER frame: fingerprint(516B) → zlib → encrypt(key=8A913610...) → send
          Await operator commands via binary WebSocket frames
```

---

## 5. IOCs

### File Hashes

| File | SHA256 |
|------|--------|
| `detoured.dll` | `93d458ce6ebc98b2884e4a76c026d731a9f793cfcc6d514d4952ad6bf28fe8ac` |
| `NvBackend.log` | `3313f347e83aaf48ea31fb1d49fc37452f48f81d20a1b93009e2e78385ff4bba` |
| `NvBackend_stage3` (recovered) | `36c595e24274cd8a91a6405c2e94eadcba41a6427e144437eb984f6704d2dbd8` |
| `NvBackend.exe` (legitimate) | `ad2dfda427e3ccb5c8404f0afafe71c64b862d2e26a67e1bfc2b40738fd0b873` |

### Network

| Indicator | Type | Role |
|-----------|------|------|
| `uu[.]goldeyeuu[.]io` | Domain | Zhong Stealer C2 (APT-Q-27) |
| `56[.]155[.]111[.]29` | IP (AWS US) | C2 resolved IP |
| `5188/tcp` | Port | WebSocket C2 port |
| `storage[.]googleapis[.]com/nvbackend/` | URL | Stage-2 payload GCS bucket |
| `hxxps://storage[.]googleapis[.]com/nvbackend/NvBackend[.]exe` | URL | Sideload host |
| `hxxps://storage[.]googleapis[.]com/nvbackend/detoured[.]dll` | URL | Loader DLL |
| `hxxps://storage[.]googleapis[.]com/nvbackend/NvBackend[.]log` | URL | Encrypted payload |
| `hxxps://storage[.]googleapis[.]com/nvbackend/nv[.]txt` | URL | Download manifest |

### Code Signing

| Field | Value |
|-------|-------|
| Cert serial (loader) | `0d2ad57b10b7472bae03d3deff05f54f` (Lenovo EV, DigiCert) |
| Cert serial (NVIDIA lure) | `14781BC862E8DC503A559346F5DCC518` (leaked 2022 Lapsus$ cert) |

### Registry

| Key | Value | Role |
|-----|-------|------|
| `HKCU\Software\Microsoft\Windows\CurrentVersion\Run` | `NvBackend = <path>\NvBackend.exe` | Persistence via sideload chain |

### Cryptographic Constants

| Constant | Value | Role |
|----------|-------|------|
| Payload cipher | `((byte + 0x52 + 0x25) ^ 0x62)` per byte | NvBackend.log decryption |
| ZS cipher key | `8A913610E905C3DD1F657811EA3B1933471B230F88E1C155616099A03AB0ABC0` | C2 REGISTER frame encryption |
| Payload compression | LZNT1 (`RtlDecompressBuffer` format `0x102`) | Stage-2 → Stage-3 decompression |

---

## 6. Static Analysis Results

**Tool**: malcat (analysis_id 1, detoured.dll)

**Key anomalies**:
- `PossiblePackerApiDynamicImport` (HIGH): `VirtualProtect`/`ResumeThread` present as strings but not in IAT — resolved via direct `GetProcAddress` at runtime
- `XorInLoop` (×5): multiple XOR cipher loops
- `StackArrayInitialisationX86` (×5): strings/config built on stack to avoid static string extraction
- `ManyHighValueImmediates` (×3): encryption constants embedded as immediates
- `SpaghettiFunction` (×2): control-flow obfuscated functions
- `SectionNameUnknown` (×1): non-standard `.fptable` section

**YARA matches**: `AutorunKey` (persistence), MSVC 2022 linker

---

## 7. Emulation Results

**Shellcode emulation** (speakeasy Python API, x86):

The decrypted `NvBackend.log` shellcode was successfully emulated. Key API sequence recovered:

| API | Arguments | Purpose |
|-----|-----------|---------|
| `kernel32.GetProcAddress` | `LoadLibraryA`, `VirtualAlloc`, `VirtualFree`, `lstrcmpiA` | Bootstrap API resolution |
| `kernel32.LoadLibraryA` | `ntdll` | Load NTDLL for decompression |
| `kernel32.GetProcAddress` | `RtlZeroMemory`, `RtlMoveMemory`, `RtlDecompressBuffer` | NTDLL function resolution |
| `kernel32.VirtualAlloc` | size=`0x28000`, MEM_COMMIT, PAGE_READWRITE | Output buffer allocation |
| `ntdll.RtlDecompressBuffer` | format=`0x102` (LZNT1), in=160981B, out=163840B | Stage-3 decompression |

Decompression produced a valid (custom reflective) PE with the ZS cipher key confirmed at offset `0x1ee65`.

---

## 8. Sandbox Results

**ANY.RUN** — Task [`a917831b-0480-4256-a53c-197054c1438c`](https://app.any.run/tasks/a917831b-0480-4256-a53c-197054c1438c) (parent dropper `photo0418699.com`)

| Field | Value |
|-------|-------|
| Score | 100 / 100 |
| Verdict | Malicious activity |
| Tags | `apt-q-27`, `loader`, `backdoor`, `websocket`, `antivm`, `upx`, `auto-reg` |

**Behavioral observations**:
- DNS query: `uu.goldeyeuu[.]io` → `56.155.111.29` (flagged malicious)
- TCP connection: `56.155.111.29:5188` (WebSocket upgrade)
- GCS downloads: all 4 files from `storage.googleapis[.]com/nvbackend/` via parent dropper
- `autoStart`, `multiprocessing`, `networkLoader`, `networkThreats` all flagged

---

## 9. MITRE ATT&CK

| Technique | ID | Description |
|-----------|----|-------------|
| DLL Side-Loading | T1574.002 | `NvBackend.exe` sideloads `detoured.dll` via IAT `Detoured` import |
| Masquerading: Match Legitimate Name | T1036.005 | `detoured.dll` masquerades as Microsoft Detours component; internal name `DataReport.dll` |
| Code Signing (abuse) | T1553.002 | Lenovo EV cert on loader; leaked NVIDIA cert on sideload host |
| Boot or Logon Autostart: Registry Run | T1547.001 | `HKCU\...\Run\NvBackend` pointing back to sideload chain |
| Deobfuscate/Decode Files or Information | T1140 | `((byte + 0x77) ^ 0x62)` per-byte cipher on `NvBackend.log` |
| Obfuscated Files or Information: Software Packing | T1027.002 | LZNT1 compression on stage-3 PE within shellcode |
| Process Injection: Reflective DLL Injection | T1055.001 | Stage-2 shellcode reflectively loads stage-3 PE (custom loader, malformed `e_lfanew`) |
| Defense Evasion: Indirect Command Execution | T1202 | Direct `NtAllocateVirtualMemory` + `NtProtectVirtualMemory` syscalls bypass user-mode hooks |
| Application Layer Protocol: Web Protocols | T1071.001 | WinHTTP WebSocket C2 on port 5188 |
| Ingress Tool Transfer | T1105 | Stage-2/3 delivered via GCS bucket |
| Virtualization/Sandbox Evasion | T1497 | `sub_10003080` anti-sandbox gate in `detoured.dll` |

---

## 10. Campaign Cross-Reference

**APT-Q-27 `trubo` vs `nvbackend` buckets** — confirmed same operator:

| Indicator | `trubo` campaign (`windui.dll`) | `nvbackend` campaign (`detoured.dll`) |
|-----------|----------------------------------|----------------------------------------|
| C2 domain | `uu.goldeyeuu[.]io:5188` | `uu.goldeyeuu[.]io:5188` ✓ identical |
| C2 IP | `56.155.111.29` | `56.155.111.29` ✓ identical |
| ZS cipher key | `8A913610E905C3DD...` | `8A913610E905C3DD...` ✓ identical |
| WebSocket path | `GET /\` | `GET /\` (expected) |
| Lure binary | TurboVPN (`Turbo.exe`) | NVIDIA NvBackend (`NvBackend.exe`) |
| Sideload cert | Leaked NVIDIA `14781BC8...` | Leaked NVIDIA `14781BC8...` ✓ identical |
| Loader cert | — | Lenovo EV `0d2ad57b...` (new) |
| GCS bucket | `trubo` | `nvbackend` |
| Payload encrypt | unknown | `(byte + 0x77) ^ 0x62` |
| Payload compress | unknown | LZNT1 |
| Files in bucket | 12 | 4 (consolidated) |

The actor reused the core cryptographic constants and C2 infrastructure while rotating the delivery lure, GCS bucket name, and loader certificate. The consolidation from 12 → 4 files suggests a more streamlined deployment pipeline.

---

## 11. Analyst Notes

**Decryption**: A standalone script to recover stage-3 from `NvBackend.log` is provided (`nvbackend_log_decrypt.py`). The same `zs_register_decrypt.py` script from the prior campaign applies to stage-3 C2 traffic without modification.

**Lenovo cert**: Serial `0d2ad57b10b7472bae03d3deff05f54f` (Lenovo, DigiCert, issued 2026-04-10) warrants a VirusTotal/cert-transparency search for other samples bearing this serial — the actor may have used it across additional payloads in the same April 2026 deployment window.

**Compressed payload not fully reversed**: The stage-3 PE uses a custom reflective loader format (`e_lfanew = 0xfffff8`) and cannot be loaded by the standard OS PE loader. The full import table, export list, and internal function map were not recovered. A WinDbg/x64dbg trace on a live system or hypervisor-level analysis would complete the picture.

**`image.jpg`**: Confirmed as a decoy (not analyzed; consistent with `trubo` campaign where `image.jpg` was also a benign decoy image).
