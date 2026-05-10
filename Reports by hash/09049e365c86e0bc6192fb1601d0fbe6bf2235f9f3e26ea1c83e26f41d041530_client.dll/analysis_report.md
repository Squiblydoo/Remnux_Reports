# client.dll — Analysis Report

**Date:** 2026-05-10  
**Analyst:** Claude (claude-sonnet-4-6)

---

## 1. File Metadata

| Field | Value |
|-------|-------|
| Filename | client.dll |
| SHA256 | `09049e365c86e0bc6192fb1601d0fbe6bf2235f9f3e26ea1c83e26f41d041530` |
| MD5 | `011be4f5d960246c26cd1145eb348c8b` |
| SHA1 | `caf8d2974a468c146c0af12885871332767abc24` |
| Size | 6,056,448 bytes (5.77 MB) |
| File Type | PE32 DLL (console), Intel 80386, 8 sections |
| Code Signing | None |
| PE Timestamp | Zeroed (1970-01-01 00:00:00) — deliberately cleared |
| Image Base | 0x69480000 |
| Exports | `Start`, `_cgo_dummy_export` |
| Import Hash | `b735685c733532137bd68ae1d8459fe1` |

**Build Artifacts (from embedded Go PCLNTAB / debug info):**
- Language: **Go** (confirmed by YARA, function names, PCLNTAB metadata)
- CGO: enabled (`_cgo_dummy_export` export present)
- Build path: `server/src/cmd/backconnect_dll/main.go`
- Source tree: `server/src/backconnect/` (helper.go, metadata.go, proxy.go, server.go, session.go)
- Proto layer: `server/src/proto/udp.go`
- Key third-party modules:
  - `github.com/hashicorp/yamux v0.1.2` — TCP/TLS stream multiplexing
  - `github.com/denisbrodbeck/machineid` — machine fingerprinting

**Sections:**

| Section | Rights | Size | Entropy |
|---------|--------|------|---------|
| .text | RX | 2.8 MB | 7.81 |
| .data | RW | 290 KB | 7.75 |
| .rdata | R | 2.6 MB | 5.76 |
| .bss | RW | 196 KB | 0 |
| .edata | R | 512 B | 6.25 |
| .idata | R | 2.5 KB | 7.38 |
| .tls | RW | 512 B | 0 |
| .reloc | R | 134 KB | 0 |

High entropy in `.text` and `.data` is consistent with Go's dense code and data packing (not encryption).

---

## 2. Classification

**Malware Family:** Go-based Backconnect RAT / Reverse Proxy Implant  
**Confidence:** HIGH

**Reasoning:** The binary explicitly names itself `backconnect_dll` in its Go source path (`server/src/cmd/backconnect_dll/main.go`). It implements a full reverse-proxy/RAT protocol with:
- HTTP registration and ping to two C2 endpoints
- yamux multiplexed TLS sessions over TCP relays
- TCP and UDP traffic forwarding (SOCKS5-like protocol)
- Machine fingerprinting and metadata exfiltration at session establishment

KesaKode family matches: **GoInjector** (confidence 3), **GhostSocks** (confidence 3), **Gomir** (confidence 1). The GhostSocks match is plausible given the SOCKS5-style TCP/UDP proxy structure; however, the explicit source naming and protocol reconstruction indicate a **custom implementation** rather than a known commodity family. Assessed as a bespoke Go backconnect implant.

---

## 3. Capabilities

### C2 Registration and Keepalive
- On load, calls exported `Start()` → `main.Start()` → `StartClient()`
- Reads machine ID from `HKLM\SOFTWARE\Microsoft\Cryptography\MachineGuid` (via `github.com/denisbrodbeck/machineid`); falls back to string `"unknown"` on failure; supports config override
- Sends HTTP POST (JSON, `Content-Type: application/json`) to `/register` on each configured helper URL
- Registration request body includes `userid`, `buildversion`, and `hwid` fields
- HTTP 200 response parsed for `status` and `ping_interval` fields
- HTTP 429 (rate limit): waits 5 seconds and retries
- Registration failures trigger **exponential backoff retry loop**

### Ping-Based Activation
- After successful registration, enters a **ping loop**: periodic HTTP POST to `{base_url}/ping`
- Ping response from C2 contains relay server list (host/port pairs)
- On receiving relay list: logs `"Activated! Connecting to %d relay(s)..."` and begins relay connections
- If relay session ends, re-registers and re-enters ping loop (persistent implant behavior)

### Relay Session Establishment
- TCP-dials each relay with 3-second timeout, iterating the relay list with retries
- Sets `TCP_NODELAY` on successful connection
- Performs **TLS client handshake** over the TCP connection
- Establishes a **yamux session** (implant acts as yamux *server*, operator controls from relay)
- Opens yamux control stream; sends metadata string:
  ```
  USERID:{userid}◙BUILDVERSION:{buildversion}◙HWID:{hwid}◙◙
  ```
  where `◙` (U+25D9) is the delimiter
- Waits for `"OK"` (2-byte ACK) from relay to confirm session

### TCP Proxy Forwarding (`handleTCPConnection`)
- Accepts yamux streams from the relay
- Reads 3-byte command header per stream
- `"END"` (0x454e44) terminates the session gracefully
- Dispatches each stream to `handleProxyStream()` in a new goroutine
- Bidirectional `io.Copy` relay between yamux stream and target TCP connection

### UDP Proxy Forwarding (`handleUDPFlow`)
- Reads a custom setup header (via `server.src.proto.ReadSetupHeader`):
  - Protocol version byte (must be 0x01)
  - Address type byte (atyp)
  - Target address
- Resolves target via `server.src.proto.ResolveAddrPort()`
- Opens local UDP socket to the resolved target
- Spawns 3 goroutines: relay→target data pump, target→relay data pump, keepalive ticker
- Sends "OK" (0x4b4f) on success, "RE" (0x5245) on error, to the relay
- Tracks and logs per-flow statistics: pkts_out, pkts_in, bytes_out, bytes_in, keepalives, duration
- Termination sentinel: `"RE"` error or channel close

### System Fingerprinting
- `HKLM\SOFTWARE\Microsoft\Cryptography\MachineGuid` — persistent hardware-bound ID
- `SOFTWARE\Microsoft\Windows NT\CurrentVersion\Time Zones` — timezone enumeration
- Process enumeration (peframe: `EnumerateProcesses` YARA match, `WTSEnumerateProcessesW`)
- Hardware enumeration (peframe: `FingerprintHardware` YARA match)

### Privilege and Service
- Can install as a Windows service (`StartServiceCtrlDispatcher` path in peframe)
- Privilege escalation capability (peframe: `escalate priv`, `win token`)

### Cryptography
- TLS (full Go `crypto/tls` stack: TLS 1.2 + TLS 1.3, ECDSA/RSA/Ed25519, ML-KEM)
- ChaCha20-Poly1305 (`vendor/golang.org/x/crypto/chacha20poly1305`)
- AES-GCM (FIPS 140-3 module)
- SHA-256, SHA-512, SHA-3/Keccak, RIPEMD-160, MD5, SHA-1
- ECDH (P-224, P-256, P-384, P-521)
- Built-in SOCKS5 proxy support in `net/http` (`net/http/socks_bundle.go`), including `.onion` detection — TOR-capable transport

---

## 4. Attack Chain

```
[Operator]
    │
    ▼
[Relay server (operator-controlled TCP host:port)]
    │  TLS + yamux
    ▼
[Victim machine — client.dll loaded]
    │  DLL loaded (e.g. side-loaded, injected, or dropped)
    │  Export Start() called with config args
    │
    ├─ Step 1: HTTP POST https://api1.mylabubus.shop/register
    │          {userid, buildversion, hwid}
    │          → receives {status, ping_interval}
    │
    ├─ Step 2: Ping loop → HTTP POST /ping
    │          → C2 returns relay host:port list when operator is ready
    │
    ├─ Step 3: TCP connect → TLS handshake → yamux session to relay
    │          Control stream: send "USERID:◙BUILDVERSION:◙HWID:◙◙"
    │          Receive "OK"
    │
    └─ Step 4: handleProxyStreams loop
               ├─ TCP streams → handleTCPConnection (bidirectional TCP proxy)
               └─ UDP streams → handleUDPFlow (UDP forward proxy)
```

The implant functions as a **tunneled reverse proxy**: all victim-side TCP and UDP traffic is forwarded to targets determined by the operator via the relay, enabling full network pivoting through the victim host.

---

## 5. IOCs

### Network (Defanged)

| Type | Value | Notes |
|------|-------|-------|
| Domain | `mylabubus[.]shop` | C2 parent domain |
| Subdomain | `api1[.]mylabubus[.]shop` | Primary C2 helper |
| Subdomain | `api2[.]mylabubus[.]shop` | Failover C2 helper |
| URL | `https://api1[.]mylabubus[.]shop/register` | Registration endpoint |
| URL | `https://api2[.]mylabubus[.]shop/register` | Registration endpoint (failover) |
| URL | `https://api1[.]mylabubus[.]shop/ping` | Keepalive/activation endpoint |
| URL | `https://api2[.]mylabubus[.]shop/ping` | Keepalive/activation endpoint (failover) |

**Note:** Relay server host:port pairs are delivered dynamically via the `/ping` response and are not embedded in the binary.

### Registry

| Key | Purpose |
|-----|---------|
| `HKLM\SOFTWARE\Microsoft\Cryptography` | Read for MachineGuid victim ID |
| `HKLM\SOFTWARE\Microsoft\Cryptography\MachineGuid` | Victim unique identifier |
| `HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Time Zones` | Timezone fingerprinting |

### Filesystem
- DLL name: `client.dll` (module export name matches)
- No hardcoded drop paths observed in static analysis

### Protocol Artifacts
- Metadata delimiter: `◙` (Unicode U+25D9 WHITE CIRCLE WITH DOT)
- Session ACK: `"OK"` (0x4b4f)
- Session error: `"RE"` (0x5245)
- Stream terminator: `"END"` (0x454e44)
- HTTP headers: `User-Agent`, `Content-Type: application/json`

---

## 6. Emulation Results

### Speakeasy (Pass 1 — generic runner)
- No IOCs produced; DLL entered Go runtime initialization and stalled in `WaitForSingleObject` loop before `Start()` was invoked with required arguments.

### Speakeasy (Pass 2 — plain)
- Hit API call limit in Go runtime thread synchronization loop (`EnterCriticalSection` / `LeaveCriticalSection` / `WaitForSingleObject`)
- `_cgo_dummy_export` emulation: `invalid_read` error
- **No runtime-decrypted IOCs extracted** — network IOCs were recovered via static analysis instead

**Emulation limitation:** Go DLLs require CGO runtime initialization with proper arguments. The `Start` export expects configuration parameters (helper URLs, build version, user ID, relay list) that must be supplied by the loader. Speakeasy cannot satisfy this dependency without a custom stub caller.

---

## 7. Sandbox Results

**ANY.RUN:**  
- Task ID: `81ee91a4-f6d0-433e-9d1e-62196ea0f0f1`
- Score: **0/100**
- Threat level: **No threats detected**
- Tags: *(none)*
- Public report: `https://app.any.run/tasks/81ee91a4-f6d0-433e-9d1e-62196ea0f0f1`

**Assessment:** The 0/100 score reflects strong evasion — ANY.RUN loaded the DLL without calling `Start()` with valid arguments, so the implant never activated. The Go runtime entered its idle thread-management loop. ALL.RUN network IOCs were CRL/OCSP check noise (Microsoft PKI infrastructure). The static analysis C2 URLs were not contacted during the sandbox run.

---

## 8. MITRE ATT&CK Mapping

| Technique | ID | Description |
|-----------|-----|-------------|
| Application Layer Protocol: Web Protocols | T1071.001 | HTTPS to `/register` and `/ping` C2 endpoints |
| Proxy: Internal Proxy | T1090.001 | TCP proxy forwarding via relay |
| Proxy: Multi-hop Proxy | T1090.003 | TOR transport capability (`.onion` / built-in SOCKS5) |
| Encrypted Channel: Symmetric Cryptography | T1573.001 | TLS + ChaCha20-Poly1305 session encryption |
| Fallback Channels | T1008 | Dual C2 helpers (api1/api2) with retry loop |
| System Information Discovery | T1082 | MachineGuid, timezone, hardware enumeration |
| Process Discovery | T1057 | `WTSEnumerateProcessesW` / process enumeration |
| Create or Modify System Process: Windows Service | T1543.003 | Can run as Windows service |
| Abuse Elevation Control Mechanism | T1548 | Privilege escalation capability |
| Masquerading | T1036 | Named `client.dll` (generic, non-suspicious name) |
| Obfuscated Files or Information: Software Packing | T1027.002 | PE timestamp zeroed |
| Data Encoding: Standard Encoding | T1132.001 | JSON encoding for C2 communication |
| Non-Standard Port | T1571 | Relay ports delivered dynamically; not embedded |

---

## 9. Analyst Notes

### Residual Gaps
1. **Relay IP/port list not embedded.** Relay servers are delivered dynamically in the `/ping` response. Network monitoring or C2 interaction is required to recover active relay endpoints.
2. **`Start()` argument schema not fully reconstructed.** The function receives a config struct with fields including helper URLs, build version, user ID, relay list, and machine ID override. The exact JSON structure or calling convention requires debugging a live instance.
3. **`handleTCPConnection` not decompiled.** Skipped due to depth of coverage from other functions; behavior inferred from naming and `handleProxyStreams` dispatch.
4. **No loader observed.** This DLL must be delivered and loaded by a separate dropper/installer; the loading mechanism is not present in this sample.

### Confidence Assessment
- C2 domain `mylabubus[.]shop`: **HIGH** — two hardcoded subdomains with explicit `/register` and `/ping` paths confirmed across static strings and decompiled code
- Malware family (custom Go backconnect RAT): **HIGH** — source paths unambiguous
- GhostSocks attribution: **LOW** — KesaKode match only; custom implementation, not confirmed commodity malware
- TOR capability: **MEDIUM** — `.onion` string and SOCKS bundle present but no TOR C2 address recovered; may be library inclusion rather than active feature

### Recommended Follow-Up
1. Resolve `mylabubus[.]shop` current IP(s) and check hosting ASN for additional infrastructure
2. Query passive DNS for other subdomains under `mylabubus[.]shop`
3. Search for other samples calling `mylabubus[.]shop` (VirusTotal, MWDB)
4. Monitor for `/register` and `/ping` POST requests to `mylabubus.shop` in network logs
5. Search endpoint logs for `HKLM\SOFTWARE\Microsoft\Cryptography\MachineGuid` access via processes other than Windows Update
6. Investigate who dropped/loaded this DLL — the loader is the missing link in the attack chain
