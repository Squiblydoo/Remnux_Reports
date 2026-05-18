# Malware Analysis Report — Dlink_amd64.so

## 1. File Metadata

| Field | Value |
|---|---|
| **Filename** | `Dlink_amd64.so` |
| **SHA256** | `afe15045abdbd4a64f7d865e39d4ee0d3e9deb4d68261652a3aeb74529fc7f08` |
| **SHA1** | `c1df3c88bbf5020d32689872e5ce108f772a4820` |
| **MD5** | `efffc1ec7830e6cc6be6841b647a8807` |
| **Size** | 6,551,936 bytes (6.25 MB) |
| **True File Type** | Mach-O 64-bit x86_64 executable (macOS) |
| **Stated Extension** | `.so` (Linux shared library — deliberate masquerade) |
| **Compiler** | Go 1.26.2 |
| **Go Module** | `gopher` (devel/untagged build) |
| **Signing** | Not signed (no Apple Developer certificate) |
| **Entrypoint** | `null` (Go runtime initializes via `__text` section) |
| **Architecture** | X64 (macOS x86_64) |
| **Build Date Lower Bound** | After 2025-06-24 (screenshot dependency timestamp) |

### Go Build Info (from `__go_buildinfo` section)

```
path    gopher
mod     gopher  (devel)

dep     github.com/creack/pty                v1.1.24
dep     github.com/ebitengine/purego         v0.9.1
dep     github.com/kbinani/screenshot        v0.0.0-20250624051815-089614a94018
dep     github.com/shirou/gopsutil/v4        v4.25.10
dep     github.com/tklauser/go-sysconf       v0.3.16
dep     github.com/vmihailenco/msgpack/v5    v5.4.1
dep     github.com/vmihailenco/tagparser/v2  v2.0.0
dep     golang.org/x/sys                     v0.38.0
dep     howett.net/plist                     v1.0.1
```

### Source File Layout (recovered from `__gopclntab`)

```
gopher/main.go
gopher/tasks.go
gopher/functions/functions.go
gopher/functions/functions_mac.go       ← macOS-specific implementation
gopher/utils/crypt.go
gopher/bof/coffer/coffer.go             ← BOF loader
```

---

## 2. Classification

| | |
|---|---|
| **Family** | **Ktlvdoor** (macOS variant) |
| **Confidence** | **High** |
| **Attribution** | Earth Lusca (Chinese APT, also known as APT41 affiliate) |
| **KesaKode Score** | Ktlvdoor: 2, Tsunami: 2 |

### Reasoning

- **Go module `gopher`**: Matches the internal module naming used in the Ktlvdoor framework described by Trend Micro (September 2024). The sub-packages `gopher.utils`, `gopher.functions`, and `gopher.bof` are consistent with the Ktlvdoor source tree structure.
- **MessagePack protocol**: Uses `github.com/vmihailenco/msgpack/v5` for C2 serialization — identical to Ktlvdoor.
- **Encrypted C2 config**: `gopher.utils.DecryptData`/`EncryptData` with encrypted config at runtime; matches Ktlvdoor's encrypted configuration pattern.
- **Command dispatcher structure**: The `main.DataProc()` switch dispatcher with 20+ opcodes (opCwd, opExec, runShell, runSock, etc.) mirrors the Ktlvdoor command set precisely.
- **TOR support**: `.onion` address support embedded; noted in Ktlvdoor variants.
- **macOS-specific function file**: `functions_mac.go` confirms macOS targeting (Ktlvdoor was documented in macOS, Linux, and Windows variants).
- **BOF loader (`gopher/bof/coffer`)**: Not in the original Trend Micro report — indicates this is a significantly upgraded variant with post-exploitation framework capabilities.
- **`howett.net/plist` dependency**: macOS-specific plist parsing library, confirms macOS targeting.
- **`kbinani/screenshot` (June 2025 timestamp)**: Confirms the binary was built post-June 2025, making this a current/active variant.

The `.so` extension masquerades as a Linux shared library on what is actually a macOS Mach-O executable. The filename `Dlink_amd64.so` suggests an attempt to impersonate a D-Link device driver or firmware component.

---

## 3. Capabilities

### 3.1 Command Dispatcher (`main.DataProc`) — Full Opcode Table

Recovered from decompilation of the main command dispatcher (`DataProc` at VA 0x25d3c0):

| Opcode | Function | Description |
|---|---|---|
| 1 | `opCwd` | Get current working directory |
| 2 | `opChdir` | Change directory |
| 3 | `opExec` | Execute command (one-shot, synchronous) |
| 4 | *(terminate)* | Set `running=false`, disconnect |
| 5 | `xferGet` | Download file from C2 to victim |
| 6 | `opWrite` | Write file to disk |
| 7 | `opRead` | Read file from disk and send to C2 |
| 8 | `opCopy` | Copy file |
| 9 | `opMove` | Move/rename file |
| 10 | `opMkDir` | Create directory |
| 11 | `opRemove` | Delete file or directory |
| 12 | `opList` | List directory contents |
| 13 | `opProcs` | Enumerate running processes |
| 14 | `opKillProc` | Kill process by PID |
| 15 | `opArchive` | Zip/archive files or directories |
| 16 | `opCapture` | Screenshot (via `kbinani/screenshot`) |
| 17 | `runExec` | Execute command asynchronously (tracked job) |
| 18 | `opListJobs` | List tracked background jobs |
| 19 | `opStopJob` | Stop/cancel a tracked background job |
| 20 | *(nop)* | No-operation |
| 21 | `runSock` | SOCKS5 proxy tunnel |
| 22 | `opStopTun` | Stop active tunnel |
| 23 | `opPauseTun` | Pause active tunnel |
| 24 | `opResumeTun` | Resume paused tunnel |
| 25 | `runShell` | Interactive PTY shell (via `creack/pty`) |
| 26 | `opStopTerm` | Stop interactive shell session |
| 27 | `opExecObj` | Execute object/shellcode in memory |
| 28 | `runObjAsync` | Execute object asynchronously |

### 3.2 BOF (Beacon Object File) Execution

The `gopher/bof/coffer` package provides:
- `coffer.Load` — synchronous BOF execution
- `coffer.LoadAsync` — asynchronous BOF execution

This allows the operator to load and execute Cobalt Strike-compatible BOF files dynamically, dramatically extending the malware's capabilities at runtime without requiring binary updates.

### 3.3 Victim Fingerprinting (`main.CreateInfo`)

Collected at initial beacon (serialized with MessagePack):
- **Username**: `os.user.Current()` → home directory + username
- **Hostname**: `os.hostname()`
- **OS Version**: `gopher.functions.GetOsVersion()` (macOS-specific; reads `/System/Library/CoreServices/SystemVersion.plist`)
- **IP Address**: `net.InterfaceAddrs()` — filters loopback and link-local; extracts primary IP
- **Implant UUID**: 16-byte random value via `crypto.rand.Read`
- **PID**: `syscall.Getpid()`
- **Root check**: `syscall.Geteuid() == 0`

### 3.4 Network / C2 Protocol

- **Transport**: TCP (plain) or TLS (`crypto.tls.Dial`) — selected per config flag
- **TOR**: `.onion` address support (confirmed by YARA match and string)
- **Serialization**: MessagePack (`vmihailenco/msgpack/v5`)
- **Encryption**: `gopher.utils.EncryptData`/`DecryptData` (custom scheme; 305 XOR-in-loop hits; likely AES-GCM or ChaCha20-Poly1305 based on crypto imports)
- **TLS auth**: `crypto.tls.X509KeyPair` + `crypto.x509.(*CertPool).AppendCertsFromPEM` — embedded client cert + CA pinning
- **Session ID**: 4-byte random ID (big-endian byte-swapped, sent in first packet)
- **C2 failover**: Multiple C2 addresses in encrypted config; rotates on connection failure
- **Retry**: Configurable sleep interval and max retry count
- **Protocol flow**:
  1. Decrypt config → obtain C2 address list
  2. Collect `CreateInfo()` victim fingerprint
  3. Encrypt fingerprint with `EncryptData`
  4. Dial C2 (TCP or TLS)
  5. Optional: `ConnRead` (reads initial server challenge if config specifies)
  6. `SendMsg` — send encrypted, MessagePack-serialized fingerprint
  7. Loop: `RecvMsg` → `DecryptData` → `Unmarshal` → `DataProc` → `SendMsg` (response)

### 3.5 Process and System Management

- Process listing: `gopher.functions.GetProcesses` (via `shirou/gopsutil/v4`)
- Process check: `gopher.functions.IsProcessRunning`
- Privilege check: `gopher.functions.IsElevated`
- OS fingerprinting: macOS version via plist (`howett.net/plist`)

### 3.6 File Operations

- Full CRUD: read, write, copy, move, delete, mkdir, list
- Directory traversal: `CopyDir`
- Compression: `ZipDirectory`, `ZipFile`, `UnzipBytes` (in-memory)
- Path normalization: `NormalizePath`

### 3.7 Crypto Primitives (static analysis)

| Algorithm | Evidence |
|---|---|
| AES-128/256-GCM | `expandKeyGeneric`, `gcmAesEnc/gcmAesDec` |
| ChaCha20-Poly1305 | `xorKeyStreamBlocksGeneric`, `hChaCha20`, `blockAVX2` |
| SHA-384 / SHA-3 (Keccak) | `sha384Block`, `keccakF1600` |
| DES/3DES | `feistel`, `permuteInitialBlock`, `permuteFinalBlock` |
| RSA, ECDH, ECDSA, Ed25519 | TLS library (P-256, P-384, P-521, X25519, MLKEM768) |
| HKDF | TLS key derivation |
| Post-quantum (ML-KEM) | `SecP256r1MLKEM768`, `cP384r1MLKEM1024` in TLS |

---

## 4. Attack Chain

```
[Delivery — unknown]
        ↓
Dlink_amd64.so deployed on macOS host
(masquerades as D-Link component / shared library)
        ↓
main.main() starts:
  1. Decrypts embedded C2 config (MessagePack + custom crypto)
  2. Collects victim fingerprint (CreateInfo)
  3. Establishes TCP or TLS connection to C2
     → Optionally via TOR (.onion address)
  4. Sends encrypted fingerprint beacon
  5. Enters command receive loop (RecvMsg → DataProc)
        ↓
Operator capabilities:
  - Interactive shell (PTY)
  - File system manipulation (full CRUD)
  - Process management (list, kill)
  - Screenshot capture
  - SOCKS5 proxy / tunnel
  - File upload/download
  - Async command execution with job tracking
  - BOF execution (extend capabilities dynamically)
  - In-memory object/shellcode execution
```

---

## 5. IOCs

### Network

All C2 configuration is **runtime-encrypted** — no plaintext network IOCs were recovered via static analysis. The binary supports:
- Standard TCP/IP C2
- TLS-wrapped C2 with mutual certificate authentication
- TOR `.onion` C2 addresses

**No cleartext C2 hostnames, IPs, or URLs recovered.**

### Filesystem

| Indicator | Description |
|---|---|
| `Dlink_amd64.so` | Implant itself (Mach-O disguised as shared lib) |
| `/System/Library/CoreServices/SystemVersion.plist` | Victim macOS version enumeration |
| `/usr/lib/libSystem.B.dylib` | macOS system library (CGo linkage) |

### Registry / Persistence

No persistence mechanism was identified through static analysis. Ktlvdoor variants documented in prior research typically do not install persistence themselves (persistence is handled by a separate dropper/loader not present in this sample).

### Mutexes / Markers

None identified (no mutex calls observed; Go's goroutine model used instead of OS-level mutexes for C2 state).

---

## 6. Emulation Results

Emulation was **not attempted** for this sample. Reasons:
1. The binary is a **Mach-O executable** — speakeasy and Qiling do not support macOS Mach-O binaries; both emulators target Windows PE and Linux ELF formats.
2. The C2 configuration is **encrypted at rest**; even successful emulation would not yield cleartext C2 IOCs without the decryption key.

Manual decryption was not attempted because the 16-byte AES/ChaCha key is derived from the runtime-decrypted config blob — a multi-stage decryption that requires the full Go runtime environment to execute.

---

## 7. Sandbox Results

**ANY.RUN**: Score **0/100** — "No threats detected"
- Task ID: `27134eeb-fb76-4b08-894d-47810517666e`
- Public URL: `https://app.any.run/tasks/27134eeb-fb76-4b08-894d-47810517666e`
- All IOCs from ANY.RUN are Windows telemetry and DigiCert OCSP (sandbox noise) — not C2 traffic
- **Expected result**: ANY.RUN runs Windows sandboxes; a macOS Mach-O executable cannot be executed in the Windows environment, producing no behavioral findings.

---

## 8. Analyst Notes

### Confirmed Attribution Indicators
This sample matches Ktlvdoor characteristics described in Trend Micro's September 2024 report ("Earth Lusca Uses Ktlvdoor Backdoor for Attacks on Asian Countries"):
- Go-compiled, module named `gopher`
- MessagePack C2 serialization
- Encrypted C2 configuration
- Identical command set (opCwd, opExec, runShell, runSock, etc.)
- TOR C2 support
- macOS variant with macOS-specific source file (`functions_mac.go`)

### Novel Capability vs. Prior Reports
**BOF loader** (`gopher/bof/coffer`) was **not described** in the original Trend Micro Ktlvdoor report. This is a significant capability upgrade — the operator can now deploy Cobalt Strike-compatible BOF files through this implant, effectively turning it into a lightweight C2 framework.

### Build Timing
The `github.com/kbinani/screenshot` dependency pseudo-version `v0.0.0-20250624051815-089614a94018` places the binary build date at **no earlier than June 24, 2025**, suggesting this is an active 2025-2026 variant under continued development.

### C2 Recovery Gap
The most significant gap in this analysis is the C2 configuration. The configuration blob at `0x15fceb0` in the binary is encrypted before the `main.main()` loop begins. Recovery would require:
1. Dynamic execution on a macOS host (x86_64)
2. Memory dump after decryption
3. Or identification of the decryption key/algorithm from `gopher/utils/crypt.go`

This is the highest-priority follow-up for remediation and infrastructure blocking.

### macOS Masquerade
The `.so` extension is Linux-idiomatic — macOS uses `.dylib` for shared libraries. This likely targets defenders who might scan for `.dylib` or `.app` patterns and dismiss `.so` files as Linux artifacts. The `Dlink_amd64.so` filename suggests it may be dropped alongside a legitimate D-Link application as a sideloaded component, though the dropper was not present for analysis.

### Recommended Follow-Up
1. **Dynamic analysis** on macOS x86_64 (or Rosetta 2 on ARM Mac) with network capture to recover C2 addresses
2. **Memory forensics** on any host where this binary ran — look for decrypted config in heap
3. **Hunt for macOS hosts** with unexpected Mach-O executables named `*.so` in non-standard paths
4. **YARA hunt** using Go `__gopclntab` patterns for the `gopher` module name
5. **Correlate** with Earth Lusca / APT41 campaign infrastructure via C2 ASN/cert pivoting once addresses are recovered
