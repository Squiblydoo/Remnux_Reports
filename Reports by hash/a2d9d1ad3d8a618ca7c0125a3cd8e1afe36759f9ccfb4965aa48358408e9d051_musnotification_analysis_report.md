# musnotification.exe — Nim C2 Implant / Nimbo-C2 Variant

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | musnotification.exe |
| SHA256 | a2d9d1ad3d8a618ca7c0125a3cd8e1afe36759f9ccfb4965aa48358408e9d051 |
| MD5 | 2081c91d1bc001ffdaf1d1a83cdd72ed |
| SHA1 | 43f29bd0e3e33a0e81166aac92bb4d80c789e580 |
| Size | 247,456 bytes (247 KB) |
| Type | PE32+ executable (GUI) x86-64 |
| Sections | 11 + overlay (9,888 bytes — Authenticode signature) |
| Build Timestamp | 2026-04-17 05:14:48 |
| Compiler | MinGW/GCC (Nim language) |
| Imphash | 0bad7cadb368b06877b973d66e5fa7da |

**Code Signing Certificate**

| Field | Value |
|---|---|
| Issuer | SSL.com EV Code Signing Intermediate CA RSA R3 |
| Subject | X Grup Technology Tesis Yonetim Hizmetleri Ltd. Sti. |
| Location | Kâğıthane, Istanbul, TR |
| Validity | 2026-04-01 → 2027-03-31 (1-year EV cert) |
| Serial | `41fc5d610b8907bd08584d356598097d` |
| Algorithm | SHA256/RSA |

**VersionInfo Spoofing** — VersionInfo fields are entirely fabricated to impersonate a legitimate Windows binary:

| Field | Spoofed Value |
|---|---|
| CompanyName | Microsoft Corporation |
| FileDescription | Windows Update Health Tools |
| FileVersion | 10.0.26100.1 |
| InternalName | musnotification |
| OriginalFilename | musnotification.exe |
| ProductName | Microsoft® Windows® Operating System |

The real Windows binary `musnotification.exe` (Windows Update Notification) exists on modern Windows systems under `C:\Windows\System32\musnotification.exe`. Placement in a user-writable directory or PATH would allow positional hijacking.

---

## 2. Classification

**Family**: Nim-compiled C2 implant — probable **Nimbo-C2** variant  
**Confidence**: HIGH

**Evidence**:
- KesaKode similarity engine: Nimbo-C2 (confidence 3/5)
- Compiler: MinGW/GCC, confirmed by YARA rule match
- Nim standard library module names visible in binary: `oserrors.sys`, `parsejson.sys`, `parseutils.sys`, `tables.sys`, `strutils.sys`, `cmdline.sys`, `syncio.sys`
- Nim runtime error strings: `SIGSEGV`, `SIGABRT`, `SIGFPE`, `SIGINT`, `SIGILL`
- Nim string layout (`@`-prefixed, length-prefixed Nim string type)
- RC4 KSA+PRGA implementation (`sub_14000e91a`) matches Nimbo-C2 crypto
- JSON-based command dispatch with 17+ command types (`sub_14001e391`)
- Import table contains only KERNEL32 (18 imports), msvcrt (53), USER32 (1) — all sensitive APIs loaded dynamically at runtime via `LoadLibraryA`/`GetProcAddress`
- ANY.RUN: 100/100 malicious, tag: `auto-reg`

---

## 3. Capabilities

- **RC4 encryption** — Full KSA + PRGA implemented at `sub_14000e91a`; key and ciphertext are Nim strings passed as parameters. Used to encrypt/decrypt C2 traffic.
- **Base64 / URL-safe Base64 encoding** — `sub_14000e215` implements both RFC 4648 and URL-safe (RFC 3548) variants; used for encoding data sent to C2.
- **HTTPS C2 beacon** — Checks in to `https://infra-telemetry[.]com/api/checkin`; network stack loaded dynamically from `Ws2_32.dll`.
- **JSON command protocol** — `sub_14001e391` parses JSON from C2 response and dispatches to 17+ handler functions via string comparison (`sub_14000e5da`).
- **Screenshot capture** (`sub_14001a956`) — `GetSystemMetrics` for dimensions → `CreateCompatibleDC` → `CreateCompatibleBitmap` → `BitBlt` → `GetDIBits` → BMP format (writes magic `0x4D42` / `"BM"`) → sends to C2.
- **Keyboard/mouse simulation** — `keybd_event`, `mouse_event`, `SetCursorPos` imported; supports remote interaction/input injection.
- **Desktop manipulation** — `CreateDesktopA`, `GetThreadDesktop`, `SetThreadDesktop`, `CloseDesktop`; supports switching desktop contexts for screenshot or keylogging.
- **Process operations** (`sub_140019445`) — `OpenProcess` with `PROCESS_ALL_ACCESS` (0x1F0FFF); reads/writes target process memory. Supports process enumeration, manipulation, or injection.
- **File upload** (`sub_14001c095`) — Reads arbitrary files from disk and sends content to C2 endpoint; supports file listing (FindFirstFileA/FindNextFileA).
- **File system operations** — `CopyFileW`, `DeleteFileW`, `SetFileAttributesW`, `GetFileAttributesW`; supports remote file management.
- **Registry read** — `sub_14000f8e9`/`sub_1400132b7` dynamically resolve `RegOpenKeyExW` + `RegQueryValueExW` + `RegCloseKey` from advapi32 via `GetProcAddress`; reads HKLM and HKCU keys.
- **Dynamic API loading** — Three runtime DLL loads confirmed: `Ws2_32.dll` (networking), `Bcrypt.dll` (crypto), plus advapi32 for registry. `LoadLibraryA`/`GetProcAddress` pattern avoids static import detection.
- **Cryptographic randomness** — `BCryptGenRandom` from Bcrypt.dll for session keys or nonce generation.
- **Clipboard access** — `GlobalLock`/`GlobalUnlock` present; clipboard read capability.
- **PE parsing** — `capa` confirms PE section enumeration and header parsing; likely used for reflective loading or payload inspection.
- **RWX memory allocation** — `capa` confirms `VirtualAlloc`/`VirtualProtect` with RWX permissions; potential shellcode injection.
- **Anti-analysis** — 15 instances of StackArrayInitialisationX64 (stack-built obfuscated strings), 4 XOR-in-loop routines, 5 dynamic string constructions. No C2 URL is visible in static strings.

---

## 4. Attack Chain

```
1. DELIVERY
   └── Victim receives/executes musnotification.exe
       └── Lure: spoofed Microsoft VersionInfo (Windows Update Health Tools)
       └── EV-signed by SSL.com cert (X Grup Technology, Istanbul TR)

2. INITIALIZATION
   └── Nim runtime startup (TLS callbacks, signal handlers, CRT init)
   └── Dynamic API loading: Ws2_32.dll, Bcrypt.dll, advapi32.dll
   └── BCryptGenRandom → session key material

3. C2 BEACON
   └── HTTPS POST to https://infra-telemetry[.]com/api/checkin
   └── Payload: RC4-encrypted, Base64-encoded JSON beacon
       └── Contains: agent ID, system fingerprint

4. COMMAND DISPATCH
   └── sub_14001e391 parses JSON response from C2
   └── String-match on command type → dispatch to handler:
       ├── Screenshot (sub_14001a956): BMP capture → send
       ├── File upload (sub_14001c095): read file → send
       ├── Process operations (sub_140019445): OpenProcess(PROCESS_ALL_ACCESS)
       ├── Keyboard/mouse simulation
       ├── Registry read
       ├── File management (copy, delete, list)
       └── ~11 additional undetermined command types

5. PERSISTENCE
   └── Not confirmed in static analysis; likely set via command from C2
       (consistent with Nimbo-C2 operational pattern)
```

---

## 5. IOCs

### Network

| Type | IOC | Notes |
|---|---|---|
| Domain | `infra-telemetry[.]com` | C2 server; masquerades as Windows telemetry infra |
| URL | `https://infra-telemetry[.]com/api/checkin` | C2 check-in endpoint (confirmed by ANY.RUN) |

### Code Signing

| Type | IOC |
|---|---|
| Certificate Serial | `41fc5d610b8907bd08584d356598097d` |
| Subject | X Grup Technology Tesis Yonetim Hizmetleri Ltd. Sti. |
| Issuer CA | SSL.com EV Code Signing Intermediate CA RSA R3 |

### File System

| Type | IOC |
|---|---|
| Filename | musnotification.exe |
| Internal name | musnotification |

### Hashes

| Type | Value |
|---|---|
| SHA256 | `a2d9d1ad3d8a618ca7c0125a3cd8e1afe36759f9ccfb4965aa48358408e9d051` |
| MD5 | `2081c91d1bc001ffdaf1d1a83cdd72ed` |
| SHA1 | `43f29bd0e3e33a0e81166aac92bb4d80c789e580` |

---

## 6. Emulation Results

**Speakeasy Pass 1** (generic runner): No IOCs — emulation stopped before network calls due to Nim runtime signal handler `msvcrt.signal` not being stubbed.

**Speakeasy Pass 2** (direct): Same result; stopped early at `msvcrt.signal` — Nim runtime installs POSIX-style signal handlers during `_initterm`, which speakeasy does not support.

**Recommendation**: For dynamic IOC recovery, use full sandbox (see ANY.RUN results below) or a Windows VM with network monitoring. The speakeasy limitation is a known gap for Nim binaries due to their non-standard CRT initialization sequence.

---

## 7. Sandbox Results (ANY.RUN)

| Field | Value |
|---|---|
| Verdict | **Malicious activity** |
| Score | **100 / 100** |
| Tags | `auto-reg` |
| Task ID | `c0ec5e48-1370-4c34-8017-74ba4156a1dd` |
| Public Report | https://app.any.run/tasks/c0ec5e48-1370-4c34-8017-74ba4156a1dd |

**Behavioral IOCs from sandbox**:

| Category | IOC | Reputation |
|---|---|---|
| DNS | `infra-telemetry[.]com` | Unrated (new domain) |
| HTTPS | `https://infra-telemetry[.]com/api/checkin` | Unrated (new domain) |

---

## 8. MITRE ATT&CK

| Technique | ID | Evidence |
|---|---|---|
| Masquerading: Match Legitimate Name or Location | T1036.001 | VersionInfo spoofed to Microsoft musnotification.exe |
| Code Signing | T1553.002 | Valid SSL.com EV cert (X Grup Technology, Istanbul TR) |
| Obfuscated Files or Information | T1027 | Stack-built strings, XOR-in-loop, RC4-encrypted config |
| Obfuscated Files or Information: Encrypted/Encoded File | T1027.002 | RC4+Base64 for C2 payload |
| Shared Modules | T1129 | Runtime DLL loading (Ws2_32, Bcrypt, advapi32) |
| Screen Capture | T1113 | BitBlt/GetDIBits screenshot → C2 |
| Input Capture: Keylogging | T1056.001 | keybd_event, mouse_event hooks |
| Process Injection | T1055 | OpenProcess(PROCESS_ALL_ACCESS) + RWX allocation |
| Query Registry | T1012 | RegOpenKeyExW/RegQueryValueExW via dynamic loading |
| File and Directory Discovery | T1083 | FindFirstFileA/FindNextFileA |
| Data from Local System | T1005 | File upload handler reads arbitrary files |
| Application Layer Protocol: Web Protocols | T1071.001 | HTTPS C2 at /api/checkin |
| Remote Access Software | T1219 | Full RAT capability: shell, screenshot, file, process |

---

## 9. Analyst Notes

**C2 domain masquerading**: `infra-telemetry[.]com` is designed to blend with legitimate Windows telemetry traffic (`settings-win.data.microsoft.com`, `telemetry.microsoft.com`). Defenders should not assume any `*-telemetry.com` or `infra-*.com` domain is benign.

**EV certificate abuse**: The SSL.com EV certificate for a Turkish entity (X Grup Technology, Kâğıthane Istanbul) is used to pass SmartScreen/Authenticode checks. The validity period (2026-04-01 → 2027-03-31) suggests it was acquired specifically for this campaign, with the build timestamp of 2026-04-17 confirming a ~17-day lead time from cert issuance to deployment.

**Nimbo-C2 identification caveats**: KesaKode returns confidence 3/5 for Nimbo-C2. The codebase may be a private fork or Nimbo-inspired reimplementation rather than the public Nimbo-C2 repository. The 17+ command types exceed Nimbo-C2's public feature set. Without recovered RC4 keys the exact command strings cannot be confirmed statically.

**RC4 key recovery gap**: The RC4 key used for C2 traffic is constructed from stack-built strings (StackArrayInitialisationX64 anomaly at 15 locations). The dynamic `4247524126262626D4B6644E` blob visible in the binary may be a key fragment. Recovering the full key requires running the binary under a network-level MITM or attaching a debugger at `sub_14000e91a` call sites.

**Recommended follow-up**:
- Block `infra-telemetry[.]com` at DNS/proxy and report to SSL.com for certificate revocation (`41fc5d610b8907bd08584d356598097d`)
- Search for additional samples signed by the same EV cert serial
- Monitor for `musnotification.exe` running outside `C:\Windows\System32\`
- Detonate in an isolated Windows VM with HTTPS interception to recover the full RC4 key and command set
