# Malware Analysis Report — Login.exe

**Date**: 2026-03-26
**Analyst**: REMnux/Malcat automated workflow
**Confidence**: HIGH — MALICIOUS

---

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | Login.exe |
| SHA256 | `61b682d644fbdf9f0ddfec1bfda1315e2b8bed1105f8351d587af2812b64d651` |
| MD5 | `282eb531767272953ad16c6fc2969c0e` |
| Size | 70,762,688 bytes (~67.5 MB) |
| Type | PE32 executable (GUI) Intel 80386 |
| Compiler | MSVC 2010 (Rich header) |
| Subsystem | Windows GUI |
| Architecture | x86 (32-bit) |

### Code Signing Certificate
| Field | Value |
|---|---|
| Issuer | Certera Code Signing CA (Sectigo subsidiary, US) |
| Subject | 李忠梅 (personal name, not a company) |
| Organization | 李忠梅, State=贵州省 (Guizhou Province, CN) |
| Serial | `00d18ebe424562948192979c3f09bd3d00` |
| Validity | 2023-03-27 → **2026-03-26** (expires today) |
| Algorithms | SHA1 + RSA |

### Version Info
| Field | Value |
|---|---|
| CompanyName | 修仙天堂 (Xiuxian Tiantang — "Immortal Cultivation Heaven" MMORPG) |
| FileDescription | Login.exe |
| FileVersion | 1.0.6.8 |
| ProductName | **QQ:2160076913** (developer QQ contact number — anomalous for legitimate software) |
| OriginalFilename | Login.exe |

---

## 2. Classification

**Verdict**: **HIGH CONFIDENCE MALICIOUS**
**Likely family**: Trojanized Chinese game launcher / VMProtect-packed dropper or RAT
**Related verdicts**: Spedear, TrochilusRAT (low confidence from Kesakode; both are Chinese-origin RAT families)

### Reasoning

1. The binary presents as a launcher for **修仙天堂** (a real Chinese cultivation MMORPG), with a polished UI containing 20 game server selection buttons, progress bars, and an embedded IE WebBrowser ActiveX control.

2. The file is **70MB** — dominated by three **VMProtect** sections (`.svmp1`, `.svmp2`, `.svmp3`):
   - `.svmp2`: 69.9 MB on disk, RWX, **entropy=202** (packed/encrypted payload)
   - `.svmp1`: 84 MB virtual-only, RWX (unpacked in-memory staging area)
   - `.svmp3`: 293 KB, RWX (VMProtect stub/dispatcher)
   - A legitimate game launcher requires no such obfuscation at this scale.

3. A secondary protector, **MugenProtect** (Chinese software protection tool), is referenced via an embedded `MugenProtect.bmp` in the resource ZIP — suggesting dual-layer protection to evade analysis.

4. APIs are **imported by hash** to hide the true import table from static analysis tools.

5. **ValuableFileExtensions YARA** matched 28 file extension patterns (including `.ppt`, `.doc`, `.rtf`, `.sql`, `.pem`, `.jpeg`, `.png`, `.mp4`, `.3ds`, `.ps1`) — consistent with file-targeting ransomware or a stealer collecting sensitive documents.

6. The signing certificate was issued to a **Chinese private individual** (not a company) from Guizhou Province and expires today — suggesting it was obtained specifically for this campaign with no intent to renew.

---

## 3. Capabilities

- **Anti-debugging**: `IsDebuggerPresent` checks; obfuscated string fragments of "Debugger" throughout VMProtect sections
- **Packer / Code Virtualization**: VMProtect with 233 cross-section control-flow jumps; 388 XOR-in-loop constructs for in-memory decryption; import-by-hash; junk instruction insertion
- **HTTP Network Communication**: `InternetReadFile` (WinInet), `libcurl.dll`, `WININET.dll`; `s HTTP/1.1` string visible in packed section; `pic.ini` config retrieval (standard Chinese game C2 pattern)
- **File System Targeting**: 28 sensitive file extension patterns (documents, databases, scripts, images, media)
- **Shell Execution**: `ShellExecuteW` import visible; RunShell YARA match
- **Crypto Operations**: `CryptAcquireContext` import visible (may be used for encryption or credential operations)
- **Dynamic Library Loading**: `LoadLibraryExW` visible (runtime DLL injection / side-loading)
- **GUI Lure**: Authentic-looking Chinese MMORPG launcher UI with 20 server buttons, progress bars for "downloading" game updates, windowed/fullscreen mode toggle — consistent with a game launcher trojan targeting Chinese gamers
- **Privilege Escalation**: `requireAdministrator` in manifest — demands UAC elevation on first run
- **Embedded IE WebBrowser**: ActiveX CLSID `{8856F961-340A-11D0-A96B-00C04FD705A2}` — used to display attacker-controlled web content within the launcher window (phishing/drive-by potential)
- **Network Enumeration**: `IPHLPAPI.DLL` import (network interface enumeration)
- **Obfuscated C2 Domains**: Partial `.cn` and `.io` TLD fragments in packed sections (full domains decrypted at runtime by VMProtect)

---

## 4. Attack Chain (Inferred — Static Analysis)

```
User launches Login.exe (game launcher lure)
│
├─ UAC prompt fires (requireAdministrator manifest)
│   └─ Victim grants admin rights expecting game install
│
├─ VMProtect stub decrypts .svmp2 (69MB payload) into .svmp1 virtual memory
│
├─ Anti-debug check (IsDebuggerPresent) — exits if debugger present
│
├─ HTTP request to fetch pic.ini (C2 config or server list)
│   └─ Domain hidden behind VMProtect; .cn / .io TLD fragments visible
│
├─ Game launcher UI displayed (lure — 20 server buttons, progress bar)
│   └─ Embedded IE ActiveX may load attacker-controlled web content
│
└─ Payload execution (capabilities gated by C2 response):
    ├─ File targeting (28 extension types: documents, DBs, media, scripts)
    ├─ Possible credential theft / keylogging (CryptAcquireContext + dynamic LoadLibrary)
    ├─ Shell execution for dropped payloads
    └─ Persistence (mechanism not recoverable from static analysis)
```

> **Note**: The full attack chain is hidden behind VMProtect. Dynamic analysis in a sandbox is required to observe actual network calls, file operations, and dropped payloads.

---

## 5. IOCs

### Hashes
| Type | Value |
|---|---|
| SHA256 | `61b682d644fbdf9f0ddfec1bfda1315e2b8bed1105f8351d587af2812b64d651` |
| MD5 | `282eb531767272953ad16c6fc2969c0e` |
| SHA1 | `3c80750c60d934a6db6e83cd7a8b2a10d29eaf80` |
| ZIP resource SHA256 | `15f68b6039e94222df0cd76e6b2d5e6ee227824f187c096440960955fce999b7` |

### Network (Partial — Obfuscated)
| Type | Value | Confidence |
|---|---|---|
| Domain fragment | `.cn` TLD references (p.CN, R.CN, Rf.Cn, S?.cn) | Low — full domains require dynamic analysis |
| Domain fragment | `.io` TLD references (&S.IO, .Io10, .IoM) | Low — full domains require dynamic analysis |
| Config fetch | `pic.ini` (standard Chinese launcher C2 config pattern) | Medium |
| Protocol | HTTP/1.1 via WinInet + libcurl | Confirmed |

### Certificate
| Type | Value |
|---|---|
| Cert serial | `00d18ebe424562948192979c3f09bd3d00` |
| Subject | 李忠梅 |
| Issuer | Certera Code Signing CA |

### Filesystem
| Artifact | Notes |
|---|---|
| `pic.ini` | Config file fetched from C2 at runtime |
| Game launcher UI | RC_SKIN ZIP resource contains full launcher UI skin |

### Registry / Mutex / Persistence
Not recoverable from static analysis — hidden behind VMProtect.

---

## 6. Analyst Notes

### What static analysis confirms
- This is definitively a malicious binary. A legitimate game launcher does not require 70MB of VMProtect-virtualized code, dual-layer protection (VMProtect + MugenProtect), import-by-hash, anti-debugging, or `requireAdministrator` for a simple server-selection UI.
- The certificate issued to a private individual in Guizhou Province (rather than a registered company) and expiring on the same day as analysis is a strong indicator of a short-lived certificate obtained for a single campaign.
- The QQ contact number in `ProductName` (`QQ:2160076913`) is a common Chinese crimeware distribution pattern where operators embed their contact in samples for victim outreach or sales.

### What requires dynamic analysis
- **Actual C2 domains/IPs** — fully obfuscated by VMProtect; only TLD fragments (`.cn`, `.io`) visible statically
- **Unpacked payload capabilities** — the 69MB `.svmp2` section contains the full payload; unpacking requires execution in a controlled sandbox
- **Persistence mechanism** — registry run key, scheduled task, or service installation not determinable statically
- **Full file operation scope** — whether the file-extension targeting constitutes ransomware, exfiltration, or both
- **Credential theft specifics** — CryptAcquireContext present but target scope (browser? game client? OS?) unknown

### Alternative hypotheses
- **Aggressive legitimate DRM**: Very unlikely at this protection scale for a launcher binary. VMProtect + MugenProtect dual protection on a 70MB file for a game server selector is not a credible DRM use case.
- **Gray-ware / Adware**: Possible but the admin privilege requirement and anti-debug behavior exceed typical adware. The ValuableFileExtensions targeting in particular is inconsistent with adware.

### Recommended next steps
1. Run in an isolated sandbox (Cuckoo/ANY.RUN) with network interception to capture actual C2 URLs and POST data
2. Memory dump after execution to extract unpacked `.svmp2` payload for further static analysis
3. Monitor filesystem, registry, and network activity during execution
4. Pivot on cert serial `00d18ebe424562948192979c3f09bd3d00` in VirusTotal for related samples
5. Search for QQ number `2160076913` in threat intelligence platforms
