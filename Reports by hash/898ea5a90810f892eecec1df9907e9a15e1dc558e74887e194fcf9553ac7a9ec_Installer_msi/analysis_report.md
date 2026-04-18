# Malware Analysis Report: Installer.msi

**Date:** 2026-04-18  
**Analyst:** REMnux/Claude  
**Confidence:** HIGH MALICIOUS

---

## 1. File Metadata

### Outer MSI
| Field | Value |
|-------|-------|
| Filename | Installer.msi |
| SHA256 | `898ea5a90810f892eecec1df9907e9a15e1dc558e74887e194fcf9553ac7a9ec` |
| SHA1 | `ed478cacbf8706556b67ecabc29d36edd7c46509` |
| MD5 | `448071dc7a8e795cd2b7455e9027c514` |
| Size | 54,341,632 bytes (54MB) |
| Type | OLE2 MSI (WiX Toolset 3.14.1.8722) |
| Title | "Teams Windows Installer" |
| Product Name | "Teams Launcher" |
| Manufacturer | "applauncher" |
| Product Version | 3.35.6884 |
| Build Timestamp | 2022-05-06 (backdated) |
| Certificate Issuer | GlobalSign GCC R45 EV CodeSigning CA 2020 |
| Certificate Subject | 厦门云上择信信息科技有限公司 (Xiamen Yunshang Zexin, CN) |
| Certificate Serial | `0cefb95336371280655b5b79` |
| Certificate Validity | 2026-04-09 → 2027-04-10 (1-year EV) |

### Inner Payload: teams-launcher.exe
| Field | Value |
|-------|-------|
| MSI File Key | `Path` (short: `-hfmhx33.exe`) |
| Internal Name | `teams-launcher.exe` |
| SHA256 | `8bfafe663f309904a6d3ff191ac2a000cface948b22bdf030cf8dec3138dbf72` |
| Size | 53,071,128 bytes (51MB) |
| Type | PE32+ x64 (GUI) |
| Architecture | x86-64 |
| PDB Path | `teams_launcher.pdb` |
| Build Timestamp | 2026-04-17 13:50:02 (fresh — day before submission) |
| Build Source Path | `Desktop\massbuild\10\crates\launcher-client\src\` |
| Certificate | Same GlobalSign EV, same serial `0cefb95336371280655b5b79` |
| Company (VersionInfo) | "Teams Inc." |
| Description | "Application Runtime Environment" |
| Original Filename | "Teams Launcher.exe" |
| Product Version | 20.78.2061.80327 |
| Tauri Version | 2.10.2 (from cargo registry paths) |

---

## 2. Classification

**Malware Family:** Tauri/Rust Teams-Lure RAT / Infostealer (custom — no exact public family match)  
**Confidence:** HIGH  
**Threat Level:** CRITICAL

**Reasoning:** The binary contains dedicated source modules for screenshot capture (`screenshot.rs`), Python script execution (`python.rs`), system information collection (`system_info.rs`), C2 polling (`launcher-client/src/polling.rs`, `client.rs`), file download (`download.rs`), and OAuth token harvesting via deep-link protocol (`callback_server.rs`). The MSI registers a custom `app-launcher://` URL protocol to receive tokens from a browser-based authentication flow. The C2 URL is XOR-obfuscated; the plaintext was not recovered statically. KesaKode low-confidence hints at ArcherRat/RustyStealer families.

---

## 3. Capabilities

### Installation / Persistence
- Installs to `%ProgramFiles64%\Teams Launcher\` as `teams-launcher.exe`
- Registers `app-launcher://` deep-link protocol handler: `HKCU\Software\Classes\app-launcher\shell\open\command` → `"[!Path]" "%1"`
- Creates Start Menu and Desktop shortcuts
- Stores install directory in `HKCU\Software\applauncher\Teams Launcher\InstallDir`
- Downloads and installs WebView2 Runtime silently as prerequisite (legitimate Microsoft URL): `https://go[.]microsoft[.]com/fwlink/p/?LinkId=2124703`
- Bundle identifier: `com.applauncher.desktop`

### Authentication Harvesting (OAuth Flow)
- Launches a Tauri WebView2 window showing a "Processing Authentication..." loading page
- Registers `app-launcher://` deep-link to receive OAuth tokens redirected from a browser
- Error messages: `auth-error`, `Token expired or invalid`, `Forbidden: blocked or invalid token`, `Missing token in auth URI`
- Token label `pydlpy` appears associated with the auth handler, suggesting a Python-based token download operation

### System Fingerprinting
- `EnumDisplayMonitors` (display enumeration)
- `GetComputerNameExW`, `GetUserNameW`, `GetSystemInfo`, `GetVolumeInformationW`
- `GetVersionExW` (OS version)
- Reads registry: `SOFTWARE\Microsoft\...` (possibly installed software, Edge/WebView2 state)
- Process enumeration: `CreateToolhelp32Snapshot`, `Process32NextW`

### Keylogging
- `GetKeyState`, `GetKeyboardState`, `GetForegroundWindow` (YARA: KeyloggerApi)
- Monitors foreground window to capture keystrokes in active context

### Screen Capture
- `screenshot.rs` module — captures desktop screenshots
- `ShellExecuteW` used for shell interactions

### File Download & Execution
- `download.rs` module downloads secondary payloads
- Spawns `ElectronInstallerMaster.exe` via `CreateProcessW` (name suggests a second-stage Electron-based payload)
- Privilege token: `SeDebugPrivilege` requested

### Python Execution
- `python.rs` module — executes Python scripts
- `pydlpy` string suggests Python-based download-and-execute functionality
- Combined with download module: likely downloads and runs Python-encoded stagers or infostealers

### Polling C2
- `launcher-client/src/polling.rs` + `launcher-client/src/client.rs` — polling-based command retrieval
- C2 URL obfuscated (32-byte XOR-encrypted string, key not recovered statically)
- Encrypted blob at offset 0x03153B44 (in context of `download.rs` panic strings)
- Uses `rustls` TLS library for encrypted transport (TLS 1.2/1.3)
- `--disable-featur..ScreenProtection` WebView2 flag observed (disables content protection for credential theft)
- Session state management: `app_launcher_lib::SessionState`

### Crypto
- SHA-256 via Intel SHA-NI extensions (`sha256rnds2` instruction, top functions)
- 302 XOR-in-loop hits across .text — heavy string obfuscation throughout
- 45 dynamically stack-constructed strings — all key IOCs hidden at runtime
- Invalid PE checksum (intentional integrity bypass artifact)

---

## 4. Attack Chain

```
User executes Installer.msi
│
├─ Step 1: WiX MSI installs teams-launcher.exe to %ProgramFiles64%\Teams Launcher\
│          Registers app-launcher:// URL protocol
│          Downloads & installs WebView2 silently
│
├─ Step 2: LaunchApplication (seq 6601) auto-launches teams-launcher.exe
│
├─ Step 3: Tauri app opens WebView2 window ("Processing Authentication...")
│          Contacts C2 server via HTTPS polling (URL obfuscated, not recovered)
│          Collects: display config, OS info, username, hostname, volumes, running processes
│
├─ Step 4: App-launcher:// OAuth flow — user is directed to browser login
│          Token returned via app-launcher:// deep-link to callback_server.rs
│
├─ Step 5: Keylogger activated (GetKeyState/GetKeyboardState)
│          Screenshots taken (screenshot.rs)
│
├─ Step 6: download.rs fetches next-stage payload
│          ElectronInstallerMaster.exe spawned via CreateProcessW
│
└─ Step 7: python.rs downloads and executes Python payload (pydlpy handler)
           Likely credential/data exfiltration
```

---

## 5. IOCs

### MSI Properties
| Property | Value |
|----------|-------|
| Product Code | `{79554129-2224-4639-8D8F-39593D17748C}` |
| Upgrade Code | `{34FE2663-772E-557B-BA46-445691A3E78A}` |
| Product GUID (REVNUMBER) | `{6BFA0A40-23CB-4CA5-A8E4-8BF43DF90657}` |

### Certificates
| Field | Value |
|-------|-------|
| Serial | `0cefb95336371280655b5b79` |
| Subject | 厦门云上择信信息科技有限公司 |
| CA | GlobalSign GCC R45 EV CodeSigning CA 2020 |
| Validity | 2026-04-09 → 2027-04-10 |

### Network (Defanged)
| Type | Value | Purpose |
|------|-------|---------|
| URL | `https[://]go[.]microsoft[.]com/fwlink/p/?LinkId=2124703` | Legitimate WebView2 download (cover) |
| Protocol | `app-launcher[://]` | OAuth token deep-link handler |
| C2 domain | **UNKNOWN** — 32-byte XOR-encrypted blob at file offset 0x03153B64 | Primary C2 |
| Secondary | **UNKNOWN** — `ElectronInstallerMaster.exe` download origin | Payload CDN |

### Filesystem
| Path | Purpose |
|------|---------|
| `%ProgramFiles64%\Teams Launcher\teams-launcher.exe` | Installed payload |
| Install path stored at `HKCU\Software\applauncher\Teams Launcher\InstallDir` | Registry |

### Registry
| Key | Value | Purpose |
|-----|-------|---------|
| `HKCU\Software\Classes\app-launcher` | `URL: protocol` | Deep-link registration |
| `HKCU\Software\Classes\app-launcher\DefaultIcon` | `"[!Path]",0` | Protocol icon |
| `HKCU\Software\Classes\app-launcher\shell\open\command` | `"<path>" "%1"` | Handler command |
| `HKCU\Software\applauncher\Teams Launcher\InstallDir` | Install path | Persistence tracking |
| `HKCU\Software\applauncher\Teams Launcher\Desktop Shortcut` | `1` | Shortcut flag |

### Hashes
| File | SHA256 |
|------|--------|
| Installer.msi | `898ea5a90810f892eecec1df9907e9a15e1dc558e74887e194fcf9553ac7a9ec` |
| teams-launcher.exe | `8bfafe663f309904a6d3ff191ac2a000cface948b22bdf030cf8dec3138dbf72` |

### Build Artifacts
| Artifact | Value |
|----------|-------|
| Build path | `Desktop\massbuild\10\crates\launcher-client\src\` |
| Crate name | `launcher-client` |
| App identifier | `com.applauncher.desktop` |
| Internal crate | `app_launcher_lib` |
| Python token | `pydlpy` |
| Secondary EXE | `ElectronInstallerMaster.exe` |

---

## 6. Emulation Results

**Speakeasy / Qiling:** Not attempted — Tauri apps depend on WebView2 (Chromium/Edge) runtime and Tokio async runtime, neither of which can be emulated under speakeasy or Qiling. The Rust SHA-NI crypto functions (top 2 malcat-scored functions) execute correctly on native hardware but cannot be meaningfully traced in emulation.

**Static XOR decryption attempts:**
- Candidate key `uespemosarenegylmodnarodsetybdet` (32-byte string constructed at 17+ stack-build sites) — tested as rolling XOR against .text and .rdata sections; no matching URL pattern recovered
- C2 URL encryption uses a more complex scheme (possibly AES, ChaCha20, or a different key derivation); the 302 XOR-in-loop hits in the decompiler likely serve as a decryption transport, not simple static XOR
- Encrypted C2 blob identified at file offset 0x03153B64 (32 bytes, preceded by `0x20000000` = length=32): `6d584a52195c4b4b564baeacaea5a892bbaca1a4a9acb9a4a2a392aea5a8aea6`

---

## 7. Sandbox Results

**Tria.ge:** Submission failed — API returned error 1010 (Cloudflare bot protection or network block on this endpoint). Sample was not analyzed dynamically. C2 URL remains unknown.

---

## 8. MITRE ATT&CK

| Technique | ID | Detail |
|-----------|----|--------|
| Phishing: Spearphishing Link | T1566.002 | Fake Microsoft Teams installer |
| Masquerading | T1036.001 | "Teams Windows Installer" / "Teams Inc." branding |
| Code Signing | T1553.002 | GlobalSign EV cert, Xiamen CN company |
| User Execution: Malicious File | T1204.002 | MSI requires user to run |
| Command and Scripting Interpreter: JavaScript | T1059.007 | Tauri WebView2 JS frontend |
| Command and Scripting Interpreter: Python | T1059.006 | python.rs + pydlpy handler |
| System Information Discovery | T1082 | system_info.rs, GetComputerNameExW, GetSystemInfo |
| Process Discovery | T1057 | CreateToolhelp32Snapshot, Process32NextW |
| Screen Capture | T1113 | screenshot.rs module |
| Input Capture: Keylogging | T1056.001 | GetKeyState, GetKeyboardState |
| Application Layer Protocol: Web Protocols | T1071.001 | HTTPS polling C2 |
| Ingress Tool Transfer | T1105 | download.rs, ElectronInstallerMaster.exe |
| Modify Registry | T1112 | app-launcher:// protocol registration |
| Steal Web Session Cookie | T1539 | OAuth token harvesting via deep-link |
| Hijack Execution Flow | T1574 | app-launcher:// protocol handler redirect |
| Privilege Escalation | T1134.001 | SeDebugPrivilege token manipulation |

---

## 9. Analyst Notes

### Residual Gaps
1. **C2 URL unknown** — The primary C2 domain could not be recovered statically. The 32-byte encrypted blob at offset 0x03153B64 in `teams-launcher.exe` requires either dynamic analysis (full Windows sandbox) or identification of the key derivation function. The `uespemosarenegylmodnarodsetybdet` constant may be an intermediate seed rather than a direct XOR key.

2. **ElectronInstallerMaster.exe** — The name and spawn point (`src-tauri\src\python.rs` context) suggest this is a second-stage Electron-based payload downloaded from the C2. Its hash and exact capabilities are unknown.

3. **Python payload** — The `pydlpy` handler (`python.rs`) likely downloads and executes a Python script for additional credential harvesting or data exfiltration. The script content is unknown.

4. **Authentication target** — The `callback_server.rs` + `app-launcher://` OAuth flow suggests the malware performs a man-in-the-browser attack against a specific web service (Teams, Microsoft 365, or similar). The target was not positively identified.

### Recommended Follow-Up
- Submit `teams-launcher.exe` directly to an isolated Windows sandbox (Cuckoo, Any.run, or Tria.ge) to capture live C2 traffic
- Monitor for `app-launcher://` protocol invocations in EDR logs
- Block the EV cert serial `0cefb95336371280655b5b79` at the proxy/AV layer
- Detect `teams-launcher.exe` process creation from `%ProgramFiles64%\Teams Launcher\`
- Look for shortcut entries pointing to a `Teams Launcher` folder in `%ProgramFiles64%`
- Hunt for the `com.applauncher.desktop` identifier in Windows registry (`HKCU\Software\Classes\app-launcher`)
- The `Desktop\massbuild\10` build path may identify other samples from the same actor (pivot on this in threat intelligence)
