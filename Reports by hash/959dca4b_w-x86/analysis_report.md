# Malware Analysis Report: w-x86.exe

**Analyst:** REMnux/Claude  
**Date:** 2026-05-14  
**TLP:** TLP:CLEAR

---

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | w-x86.exe |
| SHA256 | `959dca4b7989546a18a3f5e016c4bd78cfd825a1e679cefe0a355e739605937f` |
| SHA1 | `60b6e166d574f886016b93c4ccb49fe4b1ac47c6` |
| MD5 | `ba4480ad303a25d7b82164e576e33919` |
| File Type | PE32+ executable (GUI) x86-64, Windows (note: filename says "x86" but binary is x64) |
| Size | 8,850,952 bytes (8.44 MB) |
| Compiler | Go (goroutines, runtime strings, `runtime.morestack_noctxt`) |
| Build Timestamp | 0 (zeroed — intentional anti-forensics) |
| Subsystem | GUI (`-H windowsgui` ldflags) |
| Sections | `.text` (RX), `.rdata` (R), `.data` (RW), `.pdata`, `.xdata`, `.idata`, `.reloc`, `.symtab` + overlay (PKCS7 signature) |

### Signing Certificate
| Field | Value |
|---|---|
| Subject | Pingxiang De'a Zhiyun Technology Co., Ltd. |
| Issuer | Sectigo Public Code Signing CA EV R36 |
| Serial | `6c5efe09cd24511fddd320dd409c2d03` |
| Validity | 2026-03-12 to 2027-03-12 |
| Country | CN (Jiangxi Sheng) |

### Embedded Build Flags (recovered from binary strings)
```
-ldflags="-s -w
  -X 'github.com/custom-socks5/internal/agent.c2url=https://webhook902.securitysolut.com'
  -X 'github.com/custom-socks5/internal/agent.secret=b9b264513a4f9074dc8fa9c022fce263'
  -X 'github.com/custom-socks5/internal/agent.verbose=0'
  -X 'github.com/custom-socks5/internal/agent.minsleep='
  -X 'github.com/custom-socks5/internal/agent.maxsleep='
  -X 'github.com/custom-socks5/internal/agent.formfield='
  -H windowsgui"
```

---

## 2. Classification

| Field | Value |
|---|---|
| **Family** | Custom Go RAT (`github.com/custom-socks5/cmd/agent`) |
| **Confidence** | **HIGH** |
| **Variant** | Windows x64 build — paired with previously analyzed `w-arm64.exe` (SHA256: `6210caac...`) |
| **Threat Level** | High — full-featured remote access implant |
| **YARA Matches** | Golang (compiler), TorUsage, PostHttpForm, EnumerateProcesses |

**Reasoning:** Identical C2 URL, HMAC-SHA256 authentication secret, Go module path, capability set, persistence method, build artifact strings, and signing certificate (same serial `6c5efe09...`) as `w-arm64.exe`. The only difference is architecture — this binary targets Intel x86-64 Windows while `w-arm64.exe` targets ARM64 Windows. The operator compiled both variants from the same source repository with identical configuration.

**Cross-reference to w-arm64.exe (confirmed — same cert serial `6c5efe09cd24511fddd320dd409c2d03`):**
- Same C2 URL: `https://webhook902.securitysolut.com`
- Same HMAC secret: `b9b264513a4f9074dc8fa9c022fce263`
- Same module: `github.com/custom-socks5/internal/agent`
- Same persistence via `IShellLinkW`+`IPersistFile` COM shortcut
- Same command set (exec/push/download/sleep/socks5/persist/ide/nop)

---

## 3. Capabilities

- **C2 check-in:** HTTP GET `{c2url}/api/check` on a sleep loop with jittered backoff; auth via `PHPSESSID` cookie = `{agentID}.{unixTimestamp}.{HMAC-SHA256(secret, timestamp)}`
- **Windows service support:** Can run as a Windows service (`golang.org/x/sys/windows/svc.Run`) or as a standalone GUI process with a hidden window
- **Shell command execution:** `handleExecCmd` — arbitrary OS command execution
- **File push (upload to C2):** `handlePush` / `uploadChunksFromFile` — chunked file upload to C2
- **File download (from C2):** `handleDownload` / `downloadFile` / `downloadDir` / `downloadMultiple`
- **SOCKS5 tunnel:** `OpenTunnel` — establishes an authenticated reverse SOCKS5 proxy (1-hour timeout); operator can route arbitrary TCP through victim
- **Streaming shell:** `handleStreamShellCmd` — interactive shell session
- **Persistence:** `Persist()` — COM shortcut via `IShellLinkW`+`IPersistFile` to `%APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup\`
- **Unpersist + self-destruct:** `Unpersist()` + `SelfDestruct()` — triggered by `ide` command or max retry count exceeded
- **Sleep control:** `handleSleepCmd` — operator-adjustable beacon interval
- **Hidden window:** `createHiddenWindow()` — spawns goroutine to create an invisible GUI window to suppress taskbar presence
- **TLS fingerprint evasion:** uTLS library (`github.com/refraction-networking/utls v1.8.2`) emulates Chrome115_PQ or Safari TLS fingerprints to bypass TLS-based detection
- **Process enumeration:** WMI / `EnumerateProcesses` (YARA match)
- **TOR-capable transport:** `.onion` string present; TorUsage YARA match — likely used as fallback transport
- **Privilege escalation:** peframe detected `EscalatePriv` API patterns
- **Registry operations:** `SOFTWARE\Microsoft\...\Version\Time Zones` read (timezone fingerprinting)
- **Working directory tracking:** `os.Getwd()` stored in Agent struct
- **Compression:** Response bodies decompressed (brotli/zstd/zlib via `klauspost/compress`)
- **Max-retry self-destruct:** After `maxRetries` consecutive check-in failures, agent calls `Unpersist()` + `SelfDestruct()`

---

## 4. Attack Chain

```
Delivery (unknown) 
    │
    ▼
w-x86.exe launched (signed with EV cert — SmartScreen bypass)
    │
    ▼
main.main() → DefaultConfig() → generateID() → runAgent()
    │
    ├─► createHiddenWindow() [hidden GUI goroutine]
    │
    ├─► IsWindowsService() check
    │       ├─ YES → Register as Windows service
    │       └─ NO  → (*Agent).Run() directly
    │
    ▼
(*Agent).Run() — C2 loop:
    │  GET https://webhook902.securitysolut.com/api/check
    │  Auth: PHPSESSID={id}.{ts}.{HMAC-SHA256}
    │
    └─► Dispatch on command string:
            exec     → shell execution
            push     → file upload to C2
            download → file download from C2
            sleep    → adjust beacon interval
            socks5   → open reverse SOCKS5 tunnel
            persist  → create startup LNK shortcut
            ide      → unpersist + self-destruct
            nop      → skip cycle
```

---

## 5. IOCs

### Network
| Type | Value | Notes |
|---|---|---|
| Domain | `webhook902[.]securitysolut[.]com` | C2 hostname (Cloudflare-proxied likely) |
| URL | `hxxps://webhook902[.]securitysolut[.]com/api/check` | Check-in endpoint (GET) |
| HMAC Secret | `b9b264513a4f9074dc8fa9c022fce263` | HMAC-SHA256 auth key (embedded in binary) |
| User-Agent | `Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/144.0.0.0 Safari/537.36` | Mimics Chrome |

### Filesystem
| Type | Path | Notes |
|---|---|---|
| Startup LNK | `%APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup\<name>.lnk` | `IShellLinkW` COM persistence |
| Other LNK patterns observed | `SearchAppStartup.lnk`, `OfficeClickToRun.lnk`, `OneDriveSyncHelper.lnk`, `RuntimeBrokerHelper.lnk`, `RuntimeBroker.lnk` | Plausible lure names for the shortcut |

### Registry
| Type | Key | Notes |
|---|---|---|
| Read | `SOFTWARE\Microsoft\Windows NT\CurrentVersion\Time Zones` | Timezone/locale fingerprinting |

### Certificate
| Field | Value |
|---|---|
| Serial | `6c5efe09cd24511fddd320dd409c2d03` |
| Subject | Pingxiang De'a Zhiyun Technology Co., Ltd. (CN) |
| CA | Sectigo EV |

### Code Artifacts
| Type | Value |
|---|---|
| Go module path | `github.com/custom-socks5/cmd/agent` |
| Internal package | `github.com/custom-socks5/internal/agent` |
| Internal package | `github.com/custom-socks5/internal/auth` |
| Internal package | `github.com/custom-socks5/internal/proto` |
| Dependencies | `github.com/andybalholm/brotli v1.0.6`, `github.com/klauspost/compress v1.17.4`, `github.com/refraction-networking/utls v1.8.2` |

---

## 6. Emulation Results

**Speakeasy (generic runner + plain mode):** No network IOCs captured. Go binaries use goroutines and a custom TLS stack that speakeasy cannot emulate — the TLS handshake to `webhook902.securitysolut.com` is never attempted in emulation. The binary terminates early (`ExitProcess(2)`) after initialization without reaching the C2 loop under emulation. This is expected for Go RATs.

**All C2 IOCs were recovered via static analysis** (embedded ldflags in the Go build metadata, strings output, and decompiled source symbols).

---

## 7. Sandbox Results

| Field | Value |
|---|---|
| **ANY.RUN Score** | **0 / 100** |
| **Threat Level** | "No threats detected" |
| **Tags** | `golang` |
| **Public Report** | https://app.any.run/tasks/3f9294b8-3551-4dd6-a44b-500fc309c1ba |

**DNS IOCs observed by sandbox:**
- `webhook902[.]securitysolut[.]com` — C2 domain resolved (DNS query confirmed)

**Analysis note:** Score of 0 is consistent with the w-arm64.exe sandbox result. The uTLS fingerprint spoofing, combined with the C2 being offline or blocking sandbox IPs, prevents any behavioral triggers. ANY.RUN tagged only the Go runtime (`golang`). The DNS query confirms the binary attempted to reach its C2 — static analysis and embedded build flags provide the full IOC picture.

---

## 8. MITRE ATT&CK

| Technique | Description |
|---|---|
| T1036.001 | Masquerading: Invalid Code Signature (misleading "x86" name for x64 binary) |
| T1553.002 | Code Signing: EV cert (Sectigo, Pingxiang CN) for SmartScreen bypass |
| T1071.001 | Application Layer Protocol: HTTPS C2 |
| T1071.003 | Application Layer Protocol: TOR (.onion fallback) |
| T1573.001 | Encrypted Channel: TLS with uTLS fingerprint spoofing |
| T1090.001 | Proxy: SOCKS5 reverse tunnel |
| T1547.001 | Boot/Logon Autostart: Startup folder LNK via IShellLinkW COM |
| T1543.003 | Windows Service installation (`svc.Run`) |
| T1059 | Command and Scripting Interpreter: `handleExecCmd` |
| T1105 | Ingress Tool Transfer: `handleDownload` |
| T1041 | Exfiltration Over C2: `handlePush`/`uploadChunksFromFile` |
| T1057 | Process Discovery: WMI process enumeration |
| T1082 | System Information Discovery: timezone/OS fingerprinting |
| T1070.004 | File Deletion: `SelfDestruct()` |
| T1497 | Virtualization/Sandbox Evasion: TLS fingerprint evasion via uTLS |
| T1219 | Remote Access Software: Full-featured RAT with SOCKS5 tunneling |

---

## 9. Analyst Notes

1. **Architecture mismatch:** The filename `w-x86.exe` is misleading — the binary is PE32+ x64 (AMD64), not 32-bit x86. The operator likely names the x64 build "x86" relative to the ARM64 variant (`w-arm64.exe`) in their build pipeline (x86-family vs ARM).

2. **C2 infrastructure shared with w-arm64.exe:** `webhook902.securitysolut.com` is the C2 for both the ARM64 and x64 Windows builds. This is a purposeful multi-architecture deployment targeting both standard Intel PCs and ARM-based Windows devices (e.g., Surface Pro X, Snapdragon laptops, Windows on ARM VMs).

3. **Cookie-based auth protocol:** The `Checkin` function constructs auth as `PHPSESSID={agentID}.{unixTimestamp}.{HMAC-SHA256(secret, timestamp)}`. The timestamp is normalized against a reference constant (`0xe7791f700`), providing replay protection. Any session replay window is limited by timestamp checking on the server side.

4. **Graceful degradation + self-preservation:** The implant has a configurable max-retry count. If it fails to reach the C2 more than `maxRetries` times consecutively, it self-destructs and unpersists. This limits forensic exposure on isolated/burned infrastructure.

5. **Streaming shell capability:** The presence of `handleStreamShellCmd` in this x64 build (also identified in w-arm64.exe) suggests the operator runs interactive shell sessions, not just batch commands. This indicates hands-on-keyboard activity rather than automated tasking.

6. **Recommended follow-up:**
   - Pivot on certificate serial `6c5efe09cd24511fddd320dd409c2d03` in certificate transparency logs / Censys for additional infrastructure
   - Check if `webhook902.securitysolut.com` resolves; correlate IP with w-arm64.exe's `56.155.111.29` (APT-Q-27) if any overlap
   - Look for additional architecture variants (Linux ELF x64, Linux ARM, macOS — the Go module path suggests cross-compilation capability)
   - The `github.com/custom-socks5` module path is private/custom; if any public leak of the source exists it would reveal additional variants and configuration options
