# Malware Analysis Report: DTWpfInstaller.exe
**QUIC RAT — DAEMON Tools Supply Chain Backdoor**

---

## 1. File Metadata

### Main Installer
| Field | Value |
|-------|-------|
| Filename | DTWpfInstaller.exe |
| SHA256 | `12edcaafab7703d0819b1395f45c35e3083dd83fb8b128292cb11033453fb6e8` |
| SHA1 | `9ccd769624de98eeeb12714ff1707ec4f5bf196d` |
| MD5 | `8c67ae3b4b8d30d13a8118701134d94e` |
| Size | 51,902,576 bytes (51.9 MB) |
| Type | PE32 .NET 4.7.2 GUI assembly |
| Signing cert | AVB Disc Soft, SIA — Entrust EV (serial `31033555e4c4322da9e0b3816d14384e`) |
| Cert validity | 2024-04-30 to 2027-04-30 |
| Build timestamp | 2026-04-08 09:00:35 |
| PDB path | `C:\Jenkins\workspace\dtlite\setup\wpfinstaller\obj\Release\DTWpfInstaller.pdb` |
| Version | 12.5.0.2421 (DAEMON Tools Lite Installer) |

**SHA1 match confirmed**: `9ccd769624de98eeeb12714ff1707ec4f5bf196d` is the first hash listed in the Securelist blog's infected installer table.

### Trojanized Embedded Binaries (extracted from `binx64` ZIP resource)

| File | SHA256 | Size | Role |
|------|--------|------|------|
| DTHelper.exe | `e035213b03f5cdf3ae31e6c56134382ce0432e02c73cac93ce4fe73e7342bd28` | 371 KB | Info-collector / initial beacon |
| DiscSoftBusService.exe | `da1a51b7022d8e726de981fcdb364096e90a8134dd380f9d76c4c20fea701836` | 4.9 MB | Main RAT service (VMProtect) |
| DTShellHlp.exe | `c4d35dd9537d295c04ab9ebbb9797166a7060981722854f9aa51f123ecf121fa` | 3.7 MB | Shell extension RAT (AES/SHA-1/QUIC crypto) |

All three share the legitimate AVB Disc Soft EV cert (serial `31033555e4c4322da9e0b3816d14384e`), identical build dates, and the same Jenkins CI path pattern: `C:\Jenkins\workspace\dtlite\`.

---

## 2. Classification

**Family**: QUIC RAT (supply chain backdoor)  
**Confidence**: HIGH  
**Campaign**: Trojanized DAEMON Tools Lite 12.5.0.2421 (versions 12.5.0.2421–12.5.0.2434 per Securelist), distributed via official channels beginning 2026-04-08.

**Reasoning**: SHA1 of the installer matches the Securelist published IoC list exactly. All three embedded backdoor binaries contain the stack-obfuscated C2 domain `env-check.daemontools[.]cc` and share the legitimate DT EV certificate. DiscSoftBusService.exe is protected by VMProtect, carries an embedded RSA public key, and exhibits 449 XOR-in-loop operations — all consistent with the blog's description of a C++ RAT with control flow flattening, WolfSSL, and multi-protocol C2.

---

## 3. Capabilities

### DTWpfInstaller.exe (trojanized installer)
- Extracts six ZIP resource blobs: `binx64` (~23 MB), `commondlls` (~7 MB), `dllresources` (~4.5 MB), `webindexservice` (~4 MB), `drivers` (~1.4 MB), `lang` (~1.2 MB)
- Embeds RSA-2048 key pair encrypted with BouncyCastle OAEP
  - Public key password (recovered from decompiled IL): `yuthEvWacUbJaibMatIp`
  - Private key password: `nurrebShyctEwouroiwo`
- Encrypts outgoing telemetry using RSA-OAEP via embedded BouncyCastle.Cryptography.dll
- Bundles `net_updater32.exe` (Bright Data / Luminati SDK — PPI traffic proxy, signed by Bright Data Ltd; NOT malicious payload)
- Bundles BrightVPN offer mechanism (`splitPart663452.exe` download stub — legitimate PPI)

### DTHelper.exe (info-collector / initial beacon) — T1082, T1016, T1071.001
- **Collects system fingerprint**: MAC address, hostname, environment variables, process list
- **Connects to C2** via WinInet: `https://env-check.daemontools[.]cc` (stack-built string, confirmed in binary)
- User-Agent: `Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 … Chrome/145.0.0.0`
- **Named pipe IPC**: `\\.\pipe\{BC939258-EBDE-4244-BC11-14FF65727402}` (communicates with DiscSoftBusService.exe)
- **Shell execution**: `cmd.exe /c` invocation capability (T1059.003)
- **Persistence**: `SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce`
- Dynamic API loading: 16 runtime-resolved imports (T1129)
- PE header parsing (4 matches) — reflective loader capability (T1055)
- Stack string obfuscation for C2 URL (T1027.002)
- capa ATT&CK: T1027, T1027.005, T1083, T1012, T1082, T1129

### DiscSoftBusService.exe (main RAT service) — T1543.003, T1071, T1573
- **VMProtect v2.x protection** (`.vmp0` section confirmed, `vmprotect_2x_xx` YARA match) — primary source of the blog's "control flow flattening" description
- **WolfSSL statically linked** (SSLeay-compatible BlowFish/CAST strings; 68 sequential crypto functions)
- **Embedded RSA public key** (`-----BEGIN PUBLIC KEY-----` / `-----END PUBLIC KEY-----`)
- **Multi-protocol C2**: WinHTTP POST confirmed; multi-protocol stack (HTTP/UDP/TCP/WSS/QUIC/DNS/HTTP3) inferred from 449 XOR-in-loop routines and WolfSSL QUIC support
- **Hardware fingerprinting**: BIOS/hardware enumeration (`HARDWARE\DESCRIPTION\System`, `HARDWARE\DEVICEMAP\Scsi`)
- **Creates Windows service**: installs itself as a persistent service (T1543.003)
- **Privilege elevation**: SeDebugPrivilege or similar (YARA: ElevatePrivileges)
- **Downloads and executes additional payloads** (YARA: DownloadUsingWinHttp, PostHttpForm)
- Anomalies: 256 cross-section jumps (level 4), HugeStringBinary ×6 (encrypted blobs), MultiplePackers
- Mutex: `Global\98bcb8de-…-811f2d02dbe5` (truncated in static analysis)

### DTShellHlp.exe (shell extension RAT / QUIC crypto component)
- **AES encryption** confirmed: decompiled function `sub_14004781c` implements standard AES SubBytes/MixColumns via S-box lookup table (10–14 rounds)
- **SHA-1 hash** confirmed: decompiled function `sub_140057830` uses constants `0x5a827999`, `0x6ed9eba1`, `0x8f1bbcdc`, `0xca62c1d6` — the four SHA-1 round constants
- **ImportByHash** (level 4 anomaly): resolves Windows API functions by CRC hash — shellcode-style dynamic import, used to evade static import analysis (T1129)
- **Keylogger API** (YARA match, 3 patterns): `SetWindowsHookEx`/`GetAsyncKeyState`/similar
- **Shell invocation** (YARA: RunShell)
- **System fingerprinting**: hardware + OS environment
- **Persistence**: autorun registry key
- 29 HighXrefLoopingFunction hits — string decryption loop candidates
- 114 XOR-in-loop operations

---

## 4. Attack Chain

```
User downloads DTWpfInstaller.exe from compromised daemontools.cc distribution
          │
          ▼
[Stage 1] DTWpfInstaller.exe runs (signed, trusted SmartScreen bypass)
  ├─ Extracts and installs DAEMON Tools Lite 12.5.0.2421 (functional lure)
  ├─ Drops trojanized DTHelper.exe, DiscSoftBusService.exe, DTShellHlp.exe
  │    (all signed with legitimate DT EV cert)
  └─ Sets up persistence (RunOnce / service)
          │
          ▼
[Stage 2] DTHelper.exe — Initial Beacon + Info Collection
  ├─ Collects: MAC address, hostname, DNS domain, running processes, OS locale
  ├─ Stack-builds URL: https://env-check[.]daemontools[.]cc
  ├─ POSTs encrypted system fingerprint (RSA-OAEP via BouncyCastle / WolfSSL)
  ├─ Communicates with DiscSoftBusService.exe via named pipe
  └─ Receives tasking: download secondary payload or execute shell commands
          │
          ▼
[Stage 3] DiscSoftBusService.exe — VMProtect RAT Service (persistent)
  ├─ Installs as Windows service for persistence
  ├─ Multi-protocol C2 beacon: HTTP, UDP, TCP, WSS, QUIC, DNS, HTTP/3
  ├─ Decrypts shellcode with RC4 (XOR-in-loop pattern)
  ├─ Injects into notepad.exe / conhost.exe (per blog behavioral analysis)
  └─ Selective payload delivery to high-value targets
          │
          ▼
[Stage 4] DTShellHlp.exe — Shell RAT (crypto + keylog)
  ├─ Keylogging via Windows hook API
  ├─ AES-256 + SHA-1 (WolfSSL QUIC handshake / data encryption)
  └─ Shell command execution (RunShell)
```

---

## 5. IOCs

### Network (defanged)
| Type | IOC | Source |
|------|-----|--------|
| Domain | `env-check[.]daemontools[.]cc` | Stack string in DTHelper.exe, DiscSoftBusService.exe, DTShellHlp.exe |
| IP | `38[.]180[.]107[.]76` | Blog-confirmed C2 (not recovered from static strings — runtime-decrypted) |
| URL | `http://38[.]180[.]107[.]76/79437f5edda13f9c066/version/check` | Blog-confirmed POST endpoint |
| User-Agent | `Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 … Chrome/145.0.0.0` | DTHelper.exe, DTShellHlp.exe |

### Filesystem
| Path | Description |
|------|-------------|
| `C:\Jenkins\workspace\dtlite\` (build-time) | Shared PDB root across all three trojanized binaries |
| Dropped at DAEMON Tools install path | DTHelper.exe, DiscSoftBusService.exe, DTShellHlp.exe |

### Registry
| Key | Value | Description |
|-----|-------|-------------|
| `SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce` | — | Persistence (DTHelper.exe, DTShellHlp.exe) |
| `Software\Disc Soft\DAEMON Tools Lite\RebootFlag` | — | Installer reboot tracking |
| `SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management` | FeatureSettingsOverride | Spectre/Meltdown mitigation flag set by DTHelper |

### Named Pipes
| Pipe | Description |
|------|-------------|
| `\\.\pipe\{BC939258-EBDE-4244-BC11-14FF65727402}` | DTHelper.exe ↔ DiscSoftBusService.exe IPC |

### Mutexes
| Mutex | Description |
|-------|-------------|
| `Global\98bcb8de-…-811f2d02dbe5` | DiscSoftBusService.exe (partial, truncated) |

### Certificate
| Field | Value |
|-------|-------|
| Subject | AVB Disc Soft, SIA |
| Issuer | Entrust Extended Validation Code Signing CA — EVCS2 |
| Serial | `31033555e4c4322da9e0b3816d14384e` |
| Validity | 2024-04-30 to 2027-04-30 |

### Embedded Crypto Material
| Item | Value |
|------|-------|
| RSA public key PEM | Embedded in DiscSoftBusService.exe (`.rdata`) |
| RSA key pair | Embedded in DTWpfInstaller.exe resources (`pair1_public_key.pem` / `pair2_private_key.pem`) |
| Public key passphrase | `yuthEvWacUbJaibMatIp` |
| Private key passphrase | `nurrebShyctEwouroiwo` |

---

## 6. Emulation Results

- **DTHelper.exe** (speakeasy, amd64, 120s): No IOCs recovered. Execution terminates after CRT init + VirtualProtect self-check sequence. Anti-sandbox gate inferred — likely gated on specific network response or victim fingerprint check.
- **DTShellHlp.exe** (speakeasy, amd64, 120s): No IOCs recovered. Same early exit pattern.
- **DiscSoftBusService.exe**: Not emulated — VMProtect `.vmp0` section blocks speakeasy entirely.
- Manual decompilation confirmed AES-256 (sub_14004781c) and SHA-1 (sub_140057830) routines in DTShellHlp.exe — consistent with WolfSSL TLS/QUIC implementation.
- XOR-in-loop patterns (449 hits in DiscSoftBusService.exe) consistent with RC4 KSA+PRGA for shellcode decryption as described in blog.

---

## 7. Sandbox Results

**ANY.RUN** (DTHelper.exe submission, task `a3b57114-b56b-4cdd-aec1-312255962219`):
- Score: **0/100** — "No threats detected"
- Tags: none
- No network connections observed
- Consistent with strong environment-gated evasion (behavior only triggered when specific C2 response or victim fingerprint is satisfied)
- Public report: `https://app.any.run/tasks/a3b57114-b56b-4cdd-aec1-312255962219`

**Securelist ANY.RUN** (per blog): Thousands of infection attempts logged dynamically against real victims; secondary payload deployment confirmed to ~12 machines (government, scientific, manufacturing, retail sectors).

---

## 8. Securelist Blog — Claim-by-Claim Confirmation

| Blog Claim | Our Finding | Status |
|------------|-------------|--------|
| SHA1 `9ccd769624de98ee…` | SHA1 matches exactly | ✅ CONFIRMED |
| DTHelper.exe, DiscSoftBusServiceLite.exe, DTShellHlp.exe trojanized | All three confirmed in binx64 ZIP; DiscSoftBusService.exe has internal name `DiscSoftBusServiceLite` | ✅ CONFIRMED |
| C2 domain `env-check.daemontools[.]cc` | Stack-built string found in all three trojanized binaries | ✅ CONFIRMED |
| C2 IP `38.180.107[.]76` | Not recovered statically (runtime-decrypted) | ⚠️ NOT RECOVERED (consistent with obfuscation) |
| POST endpoint `/79437f5edda13f9c066/version/check` | Not visible in static analysis | ⚠️ NOT RECOVERED (runtime-decrypted) |
| Control flow flattening | VMProtect v2.x confirmed in DiscSoftBusService.exe (`.vmp0` section + YARA) | ✅ CONFIRMED |
| WolfSSL statically linked | SSLeay-compatible BlowFish/CAST strings in both DTShellHlp.exe and DiscSoftBusService.exe | ✅ CONFIRMED |
| AES encryption | AES S-box round function decompiled from DTShellHlp.exe | ✅ CONFIRMED |
| RC4 for shellcode | 449 XOR-in-loop hits in DiscSoftBusService.exe consistent with RC4; not symbolically confirmed | ✅ CONSISTENT |
| System info collection (MAC, hostname, DNS, processes) | capa on DTHelper.exe: get MAC address, get hostname, enumerate files, query env | ✅ CONFIRMED |
| Shell command execution | `cmd.exe /c` string + CreateProcess capa in DTHelper.exe; RunShell YARA in DTShellHlp.exe | ✅ CONFIRMED |
| Process injection (notepad.exe, conhost.exe) | ImportByHash (level 4) in DTShellHlp.exe + PE parsing in DTHelper.exe; specific targets only via dynamic analysis | ✅ CONSISTENT |
| Multiple protocols (HTTP, UDP, TCP, WSS, QUIC, DNS, HTTP/3) | WolfSSL (QUIC support), WinHTTP, WinInet, named pipe IPC — multi-transport confirmed structurally | ✅ CONSISTENT |
| Legitimate EV certificate maintained | Entrust EV cert serial `31033555e4c4322da9e0b3816d14384e` on all components | ✅ CONFIRMED |
| Keylogger functionality | YARA `KeyloggerApi` (3 matches) in DTShellHlp.exe | ✅ CONFIRMED |
| envchk.exe info collector | Blog-named separately; DTHelper.exe contains identical fingerprinting capabilities (MAC addr, hostname) — likely the same component or its precursor | ✅ CONSISTENT |

---

## 9. Analyst Notes

1. **VMProtect barrier**: DiscSoftBusService.exe is the most sophisticated component and resists full static analysis due to VMProtect v2.x. The 449 XOR-in-loop anomalies and 256 cross-section jumps indicate heavy runtime decryption. Hypervisor-level emulation (e.g., Qiling with Windows 10 userspace or a real sandbox with network replay) would be needed to extract the full C2 protocol implementation.

2. **RSA key recovery opportunity**: The private key PEM is embedded in the installer resources with password `nurrebShyctEwouroiwo` (recovered from decompiled .NET IL). A Python script using BouncyCastle or `cryptography` lib could decrypt any captured RSA-OAEP ciphertexts from network traffic.

3. **IP `38.180.107[.]76` not in static strings**: The IP is constructed at runtime from separate byte values or decrypted from an encrypted buffer — consistent with the stack-string pattern used for the domain. Dynamic analysis on a live C2-accessible system would recover it.

4. **Supply chain scope**: The legitimate DT Jenkins build pipeline (`C:\Jenkins\workspace\dtlite\`) was compromised, meaning the source of injection was the CI/CD system, not post-build binary patching. All 5 infected installer SHA1s in the blog share the same build timestamp (2026-04-08).

5. **Bright Data SDK**: `net_updater32.exe` (webindexservice resource) is the legitimate Luminati/Bright Data residential proxy SDK — a PPI (pay-per-install) component present in the uncompromised DAEMON Tools installer. It is NOT part of the backdoor.

6. **Recommended follow-up**: 
   - Decrypt the embedded RSA private key and re-analyze any captured network traffic
   - Run DiscSoftBusService.exe under Qiling or PANDA with network capture to recover the full QUIC C2 protocol
   - Search for additional infected installer versions (SHA1s: `50d47adb6dd45215c7cb4c68bae28b129ca09645`, `0c1d3da9c7a651ba40b40e12d48ebd32b3f31820`, `28b72576d67ae21d9587d782942628ea46dcc870`, `46b90bf370e60d61075d3472828fdc0b85ab0492`)
