# Host_Integrity_Service.exe — Analysis Report

**Date:** 2026-04-22  
**Analyst:** REMnux / Claude Code

---

## 1. File Metadata

| Field | Value |
|---|---|
| **Filename** | Host_Integrity_Service.exe |
| **SHA256** | `82396d9708f9a3d6a745b3cbb74d4cab03d79156de1d15ca0b10e8c5f3082870` |
| **SHA1** | `83c02e5263294ef92b0dfb7649da44293a094e41` |
| **MD5** | `9b8885d326b8a645526c73d55ea3cea2` |
| **Size** | 792,056 bytes (773 KB) |
| **File Type** | PE32 executable (GUI) Intel 80386, MSVC 2022 |
| **Architecture** | x86 (32-bit) |
| **Compiler** | Visual Studio 2022 v17.10.3 (MSVC linker) |
| **Build Date** | 2026-04-20 09:08:21 (debug PDB / Pogo timestamps) — 2 days before analysis |
| **Sections** | `.text` (RX), `.rdata` (R), `.data` (RW), `.fptable` (RW, non-standard), `.rsrc`, `.reloc` |
| **Overlay** | 7,672-byte PKCS#7 blob (Authenticode signature) |
| **imphash** | `a2a859a8a4a976d9f464ff9dd68966f4` |

### Certificate

| Field | Value |
|---|---|
| **Issuer** | SSL.com EV Code Signing Intermediate CA RSA R3 |
| **Subject** | Brooks Bridge LLC |
| **Org Details** | Brooks Bridge LLC, Austin TX, US |
| **Validity** | 2025-06-09 → 2026-05-29 (1 year EV cert) |
| **Serial** | `3e5e792aa1dc3074beaa78419204cfd7` |
| **Algorithm** | ECDSA / SHA-256 |

The EV cert was issued ~10 months before the build date. "Brooks Bridge LLC" appears to be a shell company used solely for code signing.

---

## 2. Classification

**Family:** Unknown / Custom C2 Implant  
**Confidence:** High  
**Type:** Windows Service Backdoor  
**Capability level:** Sophisticated  

**KesaKode similarity matches:** Grayrabbit (low confidence), Orchard (low confidence) — cross-reference unconfirmed; treat as suggestive, not attributive.

The binary masquerades as a host integrity/security service while functioning as a fully-featured C2 implant. It installs as a Windows service with a dynamically-decrypted service name, communicates with a command-and-control server at `telemetry.getdirectapps[.]com`, and supports TOR-based fallback communications. All identifying strings (service name, registry key/value paths, C2 URL) are XOR-obfuscated using a TEA golden-ratio stream cipher.

---

## 3. Capabilities

### Persistence & Service Installation
- Registers itself as a Windows service via `StartServiceCtrlDispatcherW` and `RegisterServiceCtrlHandlerW`
- Service name is XOR-decrypted at runtime from an embedded blob (not recovered in static analysis)
- Creates/writes a registry key under `HKLM\SOFTWARE\WOW6432Node\<encrypted_name>` with `RegCreateKeyExW` + `RegSetValueExW`
- Registry key/value names are XOR-obfuscated and decrypted at runtime

### Command & Control
- Embedded **libcurl 8.x** for HTTP/HTTPS communication
- C2 endpoint: `https://telemetry[.]getdirectapps[.]com/api` (revealed by ANY.RUN sandbox)
- Posts system data and receives commands via HTTPS
- **Certificate pinning** (SHA-256 public key hash via `sha256//` prefix) to prevent MITM interception
- HTTP form POST (`Content-Type: application/x-www-form-urlencoded`, `multipart/form-data`)
- **TOR/SOCKS5 support**: `.onion` address capability for fallback C2 channel
- Internal version string: `3.1.0` (observed in HTTP connection setup)
- Retry logic: 3 attempts with exponential backoff (5s, 10s, 20s → max 30s), then 3600s delay

### Anti-Analysis
- `IsDebuggerPresent` check (capa-confirmed: software breakpoints detection)
- Conditional execution: only runs fully as a Windows service (`B0025.007`)
- XOR string obfuscation: 44 XOR-in-loop instances; 28 spaghetti functions
- Stack string construction: 23 instances of array-on-stack initialization
- 11 dynamically-resolved API imports (runtime linking)
- All identifying strings (service name, registry paths, C2 URL) decrypted at runtime using a XTEA-variant stream cipher (constant `0x1E3779B1`, 2-byte-stepped XOR)
- PE export parsing by hash (17 matches); import-by-hash for API resolution

### Data Collection & Exfiltration
- **Geographic location** query (`capa: get geographical location`)
- System information discovery (T1082)
- File and directory enumeration (T1083)
- Registry query (T1012)
- System network configuration discovery (T1016)
- File read/write/delete/move operations
- Environment variable enumeration and set

### Cryptographic Primitives
- MD5, HMAC, CRC32, Adler32 (integrity/fingerprinting)
- Base64 encode/decode
- JSON parser (command parsing / C2 protocol)
- zlib compress/decompress
- `CryptAcquireContext`, WinCrypt hash API
- PRNG via `CryptGenRandom`

### Network Infrastructure
- TCP client and TCP server capability (can pivot / accept connections)
- UDP client/server
- DNS resolution
- Pipe read capability (inter-process communication)

---

## 4. Attack Chain

```
[Delivery — unknown; likely dropper/installer]
        ↓
[Host_Integrity_Service.exe executed as admin]
        ↓
[Runtime string decryption (XTEA-variant stream cipher)]
        ↓
[HKLM\SOFTWARE\WOW6432Node\<name> registry key created → stores config/URL]
        ↓
[RegisterServiceCtrlHandlerW → installs as Windows Service]
        ↓
[Service starts worker thread → loops checking event / sleeping 3s]
        ↓
[libcurl HTTPS POST to telemetry.getdirectapps.com/api]
  - Certificate-pinned connection
  - TOR fallback if primary fails
  - JSON command protocol (v3.1.0)
        ↓
[C2 receives: file ops, recon, sys info, additional payloads]
```

---

## 5. IOCs

### Network

| Type | Value | Notes |
|---|---|---|
| Domain | `telemetry[.]getdirectapps[.]com` | C2 endpoint (ANY.RUN confirmed) |
| URL | `https[://]telemetry[.]getdirectapps[.]com/api` | Primary C2 POST endpoint |
| IP | `172.67.219.234` | Cloudflare proxy; real server IP hidden |

### File System
- Executable installs as a Windows service; likely drops to a persistent path (not confirmed)
- Reads/writes files during operation (paths not recovered — XOR-obfuscated)

### Registry
- `HKLM\SOFTWARE\WOW6432Node\<XOR-encrypted key name>` — created/written at startup
- Key/value names not recovered statically

### Signing Certificate (shared infrastructure indicator)
- Cert serial: `3e5e792aa1dc3074beaa78419204cfd7`
- Subject: Brooks Bridge LLC, Austin TX
- Issuer: SSL.com EV Code Signing

---

## 6. Emulation Results

### Speakeasy (Pass 1 — generic runner)
- Emulator exited during CRT/TLS initialization, before the service dispatch code was reached
- No IOCs recovered

### Speakeasy (Pass 2 — direct)
- Crashed on `kernel32.FlsGetValue2` (unsupported API stub in speakeasy's Windows 10 model)
- Emulation terminated at CRT startup before `main()` was executed

**Root cause:** The binary uses `FlsGetValue2` (Fiber Local Storage v2) from a newer Windows 10 API set (`api-ms-win-core-fibers-l1-1-2`), which is not implemented in speakeasy's default stubs. Additionally, the service pattern (`StartServiceCtrlDispatcherW`) prevents standard execution outside a service context.

---

## 7. Sandbox Results (ANY.RUN)

| Field | Value |
|---|---|
| **Score** | 100 / 100 |
| **Verdict** | Malicious activity |
| **Family tags** | None (novel / uncategorized) |
| **C2 contacts** | `telemetry.getdirectapps.com` → `/api` |
| **Public report** | https://app.any.run/tasks/ef1efa8d-7377-4541-b862-62179d00b62c |

The sandbox confirmed malicious activity at maximum score. The C2 domain `getdirectapps.com` uses the pattern of appearing to be a legitimate app distribution or analytics service. Other sandbox-observed connections were Windows telemetry/OCSP noise unrelated to the malware.

---

## 8. Analyst Notes

### What is confirmed
1. This is a new (built 2026-04-20), fully-functional Windows service backdoor
2. It communicates with `telemetry.getdirectapps[.]com/api` over certificate-pinned HTTPS
3. All identifying strings are runtime-decrypted — the service name, registry paths, and C2 URL are never stored in plaintext
4. The EV code signing cert (Brooks Bridge LLC) lends SmartScreen bypass capability
5. TOR/SOCKS5 support suggests a resilient, operator-controlled infrastructure design

### What is NOT confirmed
- The full service name (XOR key stream decryption produced non-ASCII Unicode output — likely a UTF-16 service name)
- The registry key and value names (same encryption scheme)
- Stage 2 payloads — the C2 delivers commands/files dynamically; no payload was captured
- Attribution — KesaKode similarity to "Grayrabbit" and "Orchard" is at minimum confidence; insufficient evidence for attribution

### Recommended follow-up
1. **Patch speakeasy FlsGetValue2 stub** or run under a full Windows sandbox (Cuckoo/Hatching Triage) with admin privileges to recover the decrypted service name and registry paths
2. **Enumerate Brooks Bridge LLC** — check if other malware samples share this EV cert serial; it may be a recently-purchased shell company for a targeted campaign
3. **Monitor / sinkhole `getdirectapps.com`** — victim beaconing to `/api` could reveal campaign scale
4. **Extract XOR-decrypted strings** via angr or x64dbg (breakpoint on the XTEA loop at `sub_4155a0`, EA 84384) to recover service name, registry path, and any embedded C2 fallback URLs
5. **Hunt for related samples**: search by cert serial `3e5e792aa1dc3074beaa78419204cfd7` and imphash `a2a859a8a4a976d9f464ff9dd68966f4`

### MITRE ATT&CK

| ID | Technique |
|---|---|
| T1027 | Obfuscated Files or Information |
| T1027.002 | Software Packing (string obfuscation) |
| T1036.001 | Masquerading: Invalid Code Signature (fake service name) |
| T1553.002 | Subvert Trust Controls: Code Signing (EV cert, shell company) |
| T1543.003 | Create or Modify System Process: Windows Service |
| T1012 | Query Registry |
| T1112 | Modify Registry |
| T1082 | System Information Discovery |
| T1016 | System Network Configuration Discovery |
| T1083 | File and Directory Discovery |
| T1614 | System Location Discovery |
| T1071.001 | Application Layer Protocol: Web Protocols (HTTPS C2) |
| T1071.003 | Application Layer Protocol: Mail Protocols (SMTP socket) |
| T1090.003 | Proxy: Multi-hop Proxy (TOR support) |
| T1573.001 | Encrypted Channel: Symmetric Cryptography |
| T1105 | Ingress Tool Transfer |
| T1129 | Shared Modules (runtime API resolution) |
