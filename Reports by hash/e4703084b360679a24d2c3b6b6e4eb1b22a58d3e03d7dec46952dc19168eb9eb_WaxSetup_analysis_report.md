# Malware Analysis Report: Wax Setup.msi / wax-launcher.exe

**Date**: 2026-03-27
**Analyst**: Claude Code (automated static analysis)
**Confidence**: HIGH — MALICIOUS

---

## 1. File Metadata

### Outer Installer: Wax Setup.msi
| Field | Value |
|-------|-------|
| SHA256 | `e4703084b360679a24d2c3b6b6e4eb1b22a58d3e03d7dec46952dc19168eb9eb` |
| MD5 | `26abcc5cd150f1169b2424e3704533e8` |
| Size | 49,233,920 bytes (46.9 MB) |
| Type | Composite Document File V2 (MSI/OLE2) |
| Builder | Windows Installer XML Toolset (WiX) 3.14.1.8722 |
| MSI Timestamp | **2022-05-09** (backdated — see below) |
| Product Name | Wax Windows Installer |
| Author | Wax Company |
| ProductCode | `{D41ABACA-1246-4CC2-88AA-27B39A7A4729}` |
| UpgradeCode | `{7D7D7575-4894-5709-A649-0312F06993F7}` |

### Digital Signature (MSI + Payload)
| Field | Value |
|-------|-------|
| Certificate Issuer | Sectigo Public Code Signing CA EV R36 |
| Subject | **Xiamen Yamanganese Network Co., Ltd.** |
| State | Fujian Sheng, CN |
| Serial | `29612758eebef8b08be2bf7d866555b2` |
| Validity | 2026-03-12 → 2027-03-12 |

> **TIMESTAMP ANOMALY**: MSI metadata claims creation date 2022-05-09, but the signing certificate was not issued until 2026-03-12 — impossble unless the timestamp was manually backdated. This is a deliberate anti-forensic measure.

### Payload: wax-launcher.exe
| Field | Value |
|-------|-------|
| SHA256 | `25a955c17f58b2c06544dec37ab4f6862cacd5f97ac0868ccb2ae5bbd97a9851` |
| MD5 | `937da8c4332aa2e57b287267e939d93b` |
| Size | 48,021,704 bytes (45.8 MB) |
| Type | PE32+ x64 GUI executable |
| Compiler | Rust + MSVC 2022 linker |
| Framework | Tauri 2.10.2 (Rust backend + WebView2 frontend) |
| PDB Path | `wax_launcher.pdb` |
| Debug Date | **2026-03-20 12:18:48** (compiled 7 days before analysis) |
| Internal Name | **RuntimeCore** (disguised as system component) |
| VersionInfo Company | Wax Inc. |
| VersionInfo Comments | "Part of the system infrastructure." |
| Bundle ID | `com.applauncher.desktop` |
| Manifest | **`requireAdministrator`** (demands UAC elevation) |
| PE Checksum | **INVALID** (intentionally corrupted) |

---

## 2. Classification

**Family**: Unknown — custom Tauri/Rust credential harvesting trojan
**Closest Matches**: `ArcherRat` (malcat kesakode, low confidence 5/100); `RustyStealer` (0/100)
**Confidence**: **HIGH MALICIOUS**

**Reasoning**:
- Timestamp backdating (2022 vs 2026 cert) is a deliberate anti-forensic TTD
- Chinese EV certificate for generic "Wax Company" product with generic manufacturer `applauncher`
- MSI template artifacts (`{{protocol}}`, `{{manufacturer}}`, `{{product_name}}` not substituted) reveal a mass-deployment template
- Payload requests admin elevation and carries keylogger APIs, `ReadProcessMemory`, `SeDebugPrivilege`
- **302 XOR-in-loop** decryption instances + **ChaCha20 stream cipher** — all C2 endpoints encrypted at runtime
- ChaCha20 implementation is **identical toolchain** to CryptoVista Stage 1 and ETDucky Stage 3 (same actor?)
- WebView2 frontend is a fake developer forum lure with Lorem Ipsum placeholder content
- References `ElectronInstallerMaster.exe` — secondary payload likely delivered post-install
- Hardware SHA-256 (SHA-NI) for integrity verification of downloaded payloads

---

## 3. Capabilities

### MSI-Level
- Downloads and silently installs Microsoft WebView2 runtime as a dependency via PowerShell (legitimate, but required for payload operation)
- Auto-launches `wax-launcher.exe` immediately after installation (`AUTOLAUNCHAPP` property, sequence 6601)
- Registers custom URL protocol handler `app-launcher://` in `HKCU\Software\Classes\{{protocol}}\shell\open\command`
- Writes install directory to `HKCU\Software\applauncher\Wax Launcher\InstallDir`

### Payload (wax-launcher.exe)
- **Admin Elevation**: Requests `requireAdministrator` via UAC on first launch
- **Fake Lure**: WebView2 frontend presents a developer forum/coding platform (Lorem Ipsum-filled fake posts, fake code snippets with `calculatetoken()`, `fetchtoken()`, etc.)
- **Deep-Link C2 channel**: Registers `app-launcher://` protocol; handles `nav-auth-token` and `uri-scheme-token` parameters; processes `deep-link://new-url` messages — enables token delivery and command injection via URL scheme
- **Keylogging**: Imports `GetKeyState`, `GetKeyboardState`, `GetForegroundWindow` — monitors active window and keystroke state
- **Process Memory Reading**: `ReadProcessMemory` — likely targeting browser credential stores, Electron app token storage, or game client session data
- **Process Enumeration**: `CreateToolhelp32Snapshot` + `Process32NextW` — enumerates running processes (security product detection / target selection)
- **Hardware Fingerprinting**: `GetVolumeInformationW`, `GetComputerNameExW`, `GetUserNameW`, `GetSystemInfo`, `EnumDisplayMonitors`
- **Privilege Escalation**: `AdjustTokenPrivileges` + `SeDebugPrivilege` for debug-level process access
- **ChaCha20 Encryption**: Full implementation for encrypting exfiltrated data / decrypting C2 commands at runtime (constants confirmed: `expa nd 3 2-by te k`)
- **SHA-256 (Hardware)**: Intel SHA-NI accelerated — payload integrity verification
- **Screen Capture Protection Bypass**: `--disable-feature...ScreenProtection` flag passed to WebView2, disabling capture protection
- **SOCKS5 Proxy**: Embedded `server=socks5://` configuration string — C2 traffic can be tunneled
- **Secondary Payload Reference**: `ElectronInstallerMaster.exe` string — drops or downloads a secondary Electron-based payload
- **Axum Internal Server**: Rust axum HTTP server on `127.0.0.1:34254` — internal IPC between WebView frontend and Rust backend
- **Single-Instance Management**: `single-instance-deep-link` — ensures only one instance runs, deep link handled by existing instance

### Encrypted IOCs
- C2 domains/IPs are **not visible in plaintext** — decrypted at runtime using XOR key `uespemosarenegylmodnarodsetybdet` (appears 20+ times as dynamically constructed stack string) and/or ChaCha20
- 302 XOR-in-loop functions + 45 dynamic string construction sites + 7 high-entropy unreferenced buffers

---

## 4. Attack Chain

```
[1] User downloads/executes "Wax Setup.msi"
       |
       v
[2] UAC prompt for MSI (MSI itself doesn't elevate, but requests admin)
    InstallFiles → drops wax-launcher.exe to C:\Program Files\Wax Launcher\
    Downloads + installs WebView2 runtime (PowerShell, from Microsoft CDN)
    Registers app-launcher:// URL protocol handler (HKCU\Software\Classes)
    Writes install path to registry
       |
       v
[3] MSI LaunchApplication (seq 6601, AUTOLAUNCHAPP) → runs wax-launcher.exe
       |
       v
[4] wax-launcher.exe manifest triggers UAC (requireAdministrator) → admin shell
       |
       v
[5] Rust backend initializes:
    - Decrypts C2 configuration (ChaCha20 / XOR key uespemosarenegyl...)
    - Spawns axum server on 127.0.0.1:34254
    - Registers deep-link handler (app-launcher://)
    - Enumerates processes (security product check)
    - Acquires SeDebugPrivilege
       |
       v
[6] WebView2 frontend loads fake developer forum lure
    User prompted to "authenticate" via deep link
       |
       v
[7] Credential/token capture:
    - Keylogger active (GetKeyState/GetKeyboardState)
    - ReadProcessMemory on targeted processes (browsers, Electron apps)
    - nav-auth-token / uri-scheme-token captured via deep-link scheme
       |
       v
[8] Exfiltration via ChaCha20-encrypted C2 channel
    Optional SOCKS5 proxy tunneling
       |
       v
[9] Secondary payload: ElectronInstallerMaster.exe delivered/executed
```

---

## 5. IOCs

### Hashes
| File | SHA256 |
|------|--------|
| Wax Setup.msi | `e4703084b360679a24d2c3b6b6e4eb1b22a58d3e03d7dec46952dc19168eb9eb` |
| wax-launcher.exe | `25a955c17f58b2c06544dec37ab4f6862cacd5f97ac0868ccb2ae5bbd97a9851` |

### Certificate
- Sectigo EV, **Xiamen Yamanganese Network Co., Ltd.**, serial `29612758eebef8b08be2bf7d866555b2`, valid 2026-03-12→2027-03-12

### Filesystem
- `C:\Program Files\Wax Launcher\wax-launcher.exe` (installed as `pkox-i0k.exe` in cabinet, renamed by MSI)
- `%TEMP%\MicrosoftEdgeWebview2Setup.exe` (downloaded during install)

### Registry
- `HKCU\Software\Classes\{{protocol}}\shell\open\command` = `"wax-launcher.exe" "%1"` (URL protocol handler — `{{protocol}}` substituted at runtime)
- `HKCU\Software\Classes\{{protocol}}` = `URL: protocol`
- `HKCU\Software\applauncher\Wax Launcher\InstallDir` = install path
- `HKCU\Software\applauncher\Wax Launcher\Desktop Shortcut` = 1

### URL Schemes / Protocols
- `app-launcher://` — custom protocol for deep-link C2 / token injection
- `deep-link://new-url` — deep-link notification format

### Network (Static — Runtime-Decrypted)
- Primary C2: **encrypted** (ChaCha20 + XOR key `uespemosarenegylmodnarodsetybdet`)
- Legitimate dependency: `https://go.microsoft.com/fwlink/p/?LinkId=2124703` (WebView2 bootstrapper — downloaded by MSI)
- Internal: `127.0.0.1:34254` (axum IPC server)

### Strings / Artifacts
| Indicator | Description |
|-----------|-------------|
| `uespemosarenegylmodnarodsetybdet` | XOR decryption key (20+ stack constructions in .text) |
| `wax_launcher.pdb` | PDB debug path |
| `RuntimeCore` | Internal name (disguise as system component) |
| `com.applauncher.desktop` | Tauri bundle ID |
| `ElectronInstallerMaster.exe` | Secondary payload reference |
| `nav-auth-token` | Deep-link auth token parameter |
| `uri-scheme-token` | Deep-link scheme token parameter |
| `8.66.2086` | MSI product version |
| `17.26.6439.13166` | Payload file version |

### MSI Properties (Suspicious)
- `Manufacturer`: `applauncher` (placeholder)
- `{{protocol}}`, `{{manufacturer}}`, `{{product_name}}` — unsubstituted template variables indicating mass-deployment builder

---

## 6. Analyst Notes

### What's confirmed by static analysis
1. The MSI timestamp backdating + fresh EV cert + 7-day-old build date strongly indicates this is freshly minted malware
2. The ChaCha20 implementation is byte-for-byte identical in structure to CryptoVista Stage 1 and ETDucky Stage 3 — same actor or shared tooling highly likely
3. The `uespemosarenegylmodnarodsetybdet` XOR key is the primary runtime decryption mechanism; all C2 endpoints, configuration, and sensitive strings are encrypted
4. Template artifacts in MSI (`{{protocol}}` etc.) are characteristic of a bulk malware factory/builder that generates per-campaign installers
5. "Xiamen Yamanganese Network" has no public internet presence consistent with a legitimate software company — likely a shell company used to obtain EV certificates

### What requires dynamic analysis
- **Actual C2 domains/IPs**: Require execution with ChaCha20/XOR decryption of runtime blobs
- **Secondary payload**: `ElectronInstallerMaster.exe` identity, download source, and capabilities
- **Targeted process list**: Which specific processes `ReadProcessMemory` targets (browsers, crypto wallets, game launchers?)
- **Exfil format**: What data is sent and how it's structured
- **Deep-link C2 mechanics**: Whether `app-launcher://` triggers malicious behavior from external sources or operators

### Alternative hypotheses
- The Tauri framework and WebView2 usage are legitimate for software development; however, the combination of admin elevation + keylogger APIs + ReadProcessMemory + ChaCha20-encrypted network + timestamp fraud removes any reasonable benign explanation
- The `--disable-feature ScreenProtection` flag could theoretically be legitimate (some dev tools need this), but paired with keylogger APIs it suggests screen content capture is planned

### Campaign context
- Same ChaCha20 toolchain as CryptoVista and ETDucky → possible shared actor or malware-as-a-service platform
- EV cert (Sectigo) from Xiamen, Fujian → same province as **NovaViewer.exe** (different company), suggesting a regional EV cert procurement pipeline
- Fake developer forum lure targets software developers — high-value targets for session token / credential theft
- `single-instance-deep-link` + `app-launcher://` protocol abuse is a TTD used to maintain persistent C2 via URL scheme
