# GammaPanel_x64-setup.exe — Tauri+Python Dual-Component Loader / ChaCha-Encrypted C2

**Date**: 2026-06-08  
**Analyst**: REMnux Claude  
**ANY.RUN**: https://app.any.run/tasks/a14d7bc2-b414-4ab5-9c60-283e31535b1a

---

## 1. File Metadata

| Field | Value |
|---|---|
| **Filename** | GammaPanel_x64-setup.exe |
| **SHA256** | `fd11ebd6ca3e460bcd6e34e6c4f715297c5a4818b2bd141b0ea9a1ee794d00b1` |
| **MD5** | `8f8b3e7e33e50f0a9f5c309e926ae68b` |
| **SHA1** | `c62ea6b3f404149dd0cb23c4dfb5870982085378` |
| **Size** | 41,616,984 bytes (39.7 MB) |
| **Type** | PE32 x86 NSIS self-extracting installer |
| **Certificate Issuer** | GlobalSign GCC R45 EV CodeSigning CA 2020 |
| **Certificate Subject** | GANPATI ESTATES LLP |
| **Cert Org Details** | Rajasthan, Jaipur, India — Email: kisanvyas126@gmail.com |
| **Cert Validity** | 2025-06-12 → 2026-06-13 |
| **Cert Serial** | `1a68e8aefbdbd2b972f8d6bd` |
| **VersionInfo ProductName** | GammaPanelApp v0.1.0 |
| **Build Compiler** | MSVC 6 / 2003 (NSIS stub) |
| **PE Timestamp** | 2021-09-25 21:56:47 (NSIS default, not meaningful) |

### Extracted Components

| Component | SHA256 | Type | Build Date |
|---|---|---|---|
| GammaPanelApp.exe | `2663798220714c94e290e6496631d0cc3d9c282663cbe4be0d9a16ec1301f7a7` | PE32+ x64 Rust/Tauri | 2026-05-26 |
| setup.exe | `f4a77c89504c67cd5d60061e8e4e9e7d3819b34d90781c3ccf65ac5e22d0b9d1` | PE32+ x64 PyInstaller (Python 3.14) | 2026-03-12 |
| 81d243bd2c585b0f4821__mypyc.cp314-win_amd64.pyd | `f63901977f642fd25e1f8f93b96af3b5fc4c63888925ab192abca3e33564f30b` | PE64 mypyc compiled extension | 2026-03-06 |
| VC_redist.x64.exe | (legitimate) | Microsoft VC++ 2022 redistributable | — |

---

## 2. Classification

**Classification**: Malicious Dual-Component Loader / Infostealer  
**Confidence**: **HIGH** (ANY.RUN 100/100, "loader" tag; confirmed malicious behavioral indicators)  
**KesaKode Online**: Not applicable (NSIS stub has no matches; GammaPanelApp.exe offline: JloRAT(19)/Splinter(16) — below attribution threshold, discarded)

The installer drops two components behind a "GammaPanel" screen brightness/gamma adjustment lure:

1. **GammaPanelApp.exe** — A heavily obfuscated Tauri v1 (Rust/WebView2) frontend with confirmed malicious indicators: 452 XOR-in-loop instances, ChaCha cipher runtime string decryption, embedded keylogger API, hardware fingerprinting, and a custom Tauri command `stat4x(surl, userAgent, name)` used to beacon collected data to a runtime-decrypted C2 URL.

2. **setup.exe** — A Python 3.14 PyInstaller one-file bundle whose observable module names (`gammapanel.gamma`, `gammapanel.scheduler`, `gammapanel.location`, `gammapanel.tray`, etc.) are consistent with a legitimate f.lux-style gamma adjustment application. The actual code cannot be inspected: modules are compiled with `mypyc` and stored in a PYZ archive using Python 3.14 zstd compression, making offline extraction impossible with Python <3.14. It cannot be confirmed or ruled out that setup.exe contains malicious logic beyond what its module names suggest.

The use of an **Indian real estate company's EV certificate** (GANPATI ESTATES LLP, Rajasthan) to sign a screen dimming utility is a strong indicator of a stolen or fraudulently obtained signing credential.

---

## 3. Capabilities

### Confirmed Malicious — GammaPanelApp.exe (Tauri Frontend)
- **Runtime string decryption** via ChaCha cipher (8 instances) and ACSS (1 instance)
- **Keylogger API** (Windows keyboard hook — YARA: `KeyloggerApi`)
- **Hardware fingerprinting** (`FingerprintHardware` YARA): `HARDWARE\DESCRIPTION\System\BIOS`, `HKLM` enumeration
- **Custom Tauri command `stat4x(surl, userAgent, name)`**: passes a runtime-decrypted server URL, user-agent string, and victim identifier to the Rust backend for outbound HTTP beaconing
- **HTTP POST form data** (`PostHttpForm` YARA) — sends collected data to C2
- **CSP `**https://*/*`**: allows the embedded WebView2 frontend to reach any external HTTPS endpoint
- Contacts `https://login.live.com/RST2.srf` (WAM/WS-Trust endpoint — possible Windows token theft via WebView2 SSO)
- **Sidecar process launch**: spawns `setup.exe` as a Tauri sidecar over `127.0.0.1:34254`

### Cover Story Functionality — setup.exe (Python 3.14 Backend)
The following behaviors are observable from module names only; actual code is inaccessible due to mypyc compilation and Python 3.14 zstd PYZ encryption. All are consistent with a legitimate f.lux-style application and cannot be confirmed malicious without code extraction.

- **Gamma control** (`gammapanel.gamma`): Windows gamma/color temperature adjustment — the stated purpose of the app
- **IP geolocation** (`gammapanel.location`): standard practice for solar-position apps to determine local sunrise/sunset times
- **Solar-position scheduling** (`gammapanel.scheduler` + `astral`): `astral` is a well-known Python library used by display temperature apps (f.lux, Redshift) to schedule color shifts at dusk/dawn
- **System tray** (`gammapanel.tray`): expected for a background display utility
- **Hotkey registration** (`gammapanel.hotkeys`): expected for a display utility
- **Autostart persistence** (`gammapanel.autostart`): normal for apps that should run at login
- **Configuration management** (`gammapanel.config`): reads/writes user settings
- **Tkinter UI** (`gammapanel.ui`): basic settings GUI
- **Mutex**: `GammaPanelMutex` — single-instance enforcement, normal behavior
- IPC server on `127.0.0.1:34254` — serves data to the Tauri frontend (architecture is unusual for a standalone utility but consistent with the Tauri sidecar pattern)

### NSIS Installer
- Checks for and silently installs WebView2 (required for Tauri)
- Silently installs VC++ 2022 redistributable via PowerShell: `powershell -WindowStyleHidden -Command Start-Process -FilePath 'vc_redist.x64.exe' -ArgumentList '/install', '/quiet', '/norestart' -Wait`
- Writes registry key `Software\GammaPanel\GammaPanelApp`
- Creates standard uninstall entry under `HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\GammaPanelApp`
- Runs `GammaPanelApp.exe` with `ExecShell "runas"` (privilege escalation) on install complete

---

## 4. Attack Chain

```
GammaPanel_x64-setup.exe (NSIS installer, signed GANPATI ESTATES LLP)
    ├─ Checks/installs WebView2 + VC++ 2022
    ├─ Drops to %LOCALAPPDATA%\GammaPanelApp\:
    │   ├─ GammaPanelApp.exe  (Tauri Rust frontend)
    │   ├─ setup.exe          (PyInstaller Python backend)
    │   └─ VC_redist.x64.exe
    ├─ Registers uninstall key in HKCU
    └─ ExecShell "runas" GammaPanelApp.exe  [PRIVILEGE ESCALATION]
         │
         └─ GammaPanelApp.exe starts
              ├─ Launches setup.exe as sidecar → listens on 127.0.0.1:34254
              ├─ Decrypts C2 URL via ChaCha cipher at runtime
              ├─ WebView2 loads embedded index.html frontend
              │    └─ Invokes: invoke('stat4x', { surl: <C2_URL>, userAgent: <UA>, name: <id> })
              ├─ Rust backend handles stat4x → HTTP POST to C2 with victim data
              ├─ Contacts login.live.com/RST2.srf (possible WAM token theft)
              └─ Keylogger API active
         │
         └─ setup.exe (Python 3.14 backend — cover story component)
              ├─ Mutex: GammaPanelMutex
              ├─ gammapanel.gamma → gamma/color temperature adjustment (stated purpose)
              ├─ gammapanel.location → IP geolocation (for sunrise/sunset calculation)
              ├─ gammapanel.scheduler + astral → schedule color shifts at dusk/dawn
              ├─ gammapanel.autostart → run at login (normal for display utility)
              └─ IPC on 127.0.0.1:34254 → serves data to Tauri frontend
              [actual code unverifiable: mypyc + Python 3.14 zstd PYZ]
```

---

## 5. IOCs

### Network
| Type | Indicator | Notes |
|---|---|---|
| URL | `https://login.live.com/RST2.srf` | WAM/WS-Trust endpoint — ANY.RUN confirmed contact |
| URL | `https://login.live.com/ppsecure/deviceaddcredential.srf` | Microsoft account device credential endpoint |
| Host | `127.0.0.1:34254` | Internal IPC between Tauri frontend and Python sidecar |
| **C2** | **NOT RECOVERED** | ChaCha-encrypted at runtime inside GammaPanelApp.exe |

*Note: The actual C2 URL (`surl` parameter of `stat4x` command) is decrypted via ChaCha cipher at runtime; static extraction is blocked. No external non-Microsoft C2 was observed in sandbox due to likely environment/geo gating.*

### Filesystem
| Path | Description |
|---|---|
| `%LOCALAPPDATA%\GammaPanelApp\GammaPanelApp.exe` | Tauri frontend |
| `%LOCALAPPDATA%\GammaPanelApp\setup.exe` | Python backend (PyInstaller) |
| `%LOCALAPPDATA%\GammaPanelApp\VC_redist.x64.exe` | VC++ install helper |
| `%LOCALAPPDATA%\GammaPanelApp\uninstall.exe` | Uninstaller |
| `%LOCALAPPDATA%\com.GammaPanel.app\EBWebView\` | WebView2 profile directory |

### Registry
| Key | Value | Description |
|---|---|---|
| `HKCU\Software\GammaPanel\GammaPanelApp` | `""` = `%LOCALAPPDATA%\GammaPanelApp` | Install path |
| `HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\GammaPanelApp` | Various | Uninstall entry |
| `HKCU\Software\GammaPanel\GammaPanelApp` | `Installer Language` | Language setting |

### Mutexes
| Mutex | Binary |
|---|---|
| `GammaPanelMutex` | setup.exe (Python backend) |

### Code Artifacts
| Artifact | Value |
|---|---|
| PDB path | `GammaPanel.pdb` (GammaPanelApp.exe) |
| App identifier | `com.GammaPanel.app` |
| Tauri command | `stat4x` (custom C2 callback) |
| Tauri command params | `surl`, `userAgent`, `name` |
| Publisher string | `GammaPanel` |
| Cert serial | `1a68e8aefbdbd2b972f8d6bd` (GANPATI ESTATES LLP / GlobalSign GCC R45 EV) |

---

## 6. Emulation Results

### speakeasy (runner.py) — GammaPanelApp.exe (amd64, 60s timeout)
- **Result**: No IOCs. Emulation loads binary but halts early; requires WebView2 and Win32 GUI subsystem not emulated by speakeasy.

### speakeasy (runner.py) — setup.exe (amd64, 60s timeout)
- **Result**: No IOCs. PyInstaller bootstrap requires Python 3.14 runtime DLL; not available in speakeasy environment.

### Angr / manual decrypt
- Not attempted: ChaCha key is embedded within GammaPanelApp.exe's 452 XOR-obfuscated string table; key extraction requires symbolic execution over a large code region that exceeds practical angr scope without substantial manual reversing.

---

## 7. Sandbox Results (ANY.RUN)

| Field | Value |
|---|---|
| **Verdict** | 100/100 — **Malicious activity** |
| **Tags** | `loader` |
| **Behavioral Specs** | `debugOutput`, `multiprocessing`, `networkLoader`, `serviceLauncher`, `privEscalation` |
| **Public Report** | https://app.any.run/tasks/a14d7bc2-b414-4ab5-9c60-283e31535b1a |

**Key behavioral observations**:
- `networkLoader`: sample downloads resources from the internet
- `serviceLauncher`: creates or launches Windows services (consistent with `gammapanel.autostart`)
- `privEscalation`: privilege escalation observed (NSIS `runas`, PowerShell `-Wait`)
- `login.live.com/RST2.srf` contacted (WAM token theft or WebView2 SSO)
- No external C2 observed in sandbox (geo-gating or environment check likely)

---

## 8. Analyst Notes

### Certificate Anomaly
GANPATI ESTATES LLP is an Indian real estate company (Jaipur, Rajasthan). The presence of a personal Gmail address (`kisanvyas126@gmail.com`) in an EV certificate subject field is irregular for a legitimate code-signing certificate, and a real estate firm is an atypical holder of software signing credentials. This cert is almost certainly obtained fraudulently, stolen from a legitimate business, or issued through a compromised CA verification process.

### C2 Not Recovered — Recommended Follow-Up
The actual C2 URL is protected by:
1. **Python 3.14 zstd PYZ compression** in setup.exe — the `gammapanel.config` module likely holds the server URL and is inaccessible without Python 3.14
2. **ChaCha runtime decryption** in GammaPanelApp.exe — the `stat4x` command's `surl` argument is decrypted from an embedded byte array using the ChaCha stream cipher; recovering the key requires setting a breakpoint at the ChaCha decryption call and observing the plaintext output under a Windows debugger (x64dbg/WinDbg)

**Recommended follow-up**:
- Run the sample under a full Windows VM with Process Monitor to capture the `stat4x` HTTP request and log the outbound C2 URL
- Use x64dbg with a conditional breakpoint at GammaPanelApp.exe's HTTP client to catch the decrypted `surl` at send time
- Install Python 3.14.x on an isolated VM, run `pyinstxtractor.py` against setup.exe to extract `gammapanel.config.pyc`, then decompile with `pycdc` or equivalent

### Architecture Assessment
The Tauri + Python sidecar pattern is increasingly used for malware delivery because:
- Tauri produces signed-looking, unfamiliar Rust binaries that evade Electron-specific YARA rules
- Python sidecar hides behind the Tauri app with no standalone execution path
- Python 3.14 zstd PYZ prevents offline analysis with standard tools
- mypyc compilation compiles Python to C extensions, defeating bytecode decompilers

### Lure Quality
The "GammaPanel" branding (screen gamma adjustment utility) is plausible; the presence of the `astral` library and `gammapanel.location` is consistent with f.lux-style solar-position-based display color temperature adjustment. This gives the installer a believable cover story. The CJK characters in the installer dialog strings (`胤`, `肂`) suggest possible targeting of East Asian users or actor infrastructure reuse.

### Gamma-as-a-Service Loader Pattern
The combination of `gammapanel.scheduler` (time-gated) + `gammapanel.location` (geo-aware) + ChaCha-encrypted C2 suggests possible geo/time-gated payload delivery: the C2 might only serve active payloads within specific geographic regions or time windows, which would explain why ANY.RUN's sandbox (US-based) observed no external C2 despite a 100/100 malicious verdict.
