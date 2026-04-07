# Malware Analysis Report: ms_x64_update.exe

**Date**: 2026-04-07  
**Analyst**: REMnux Claude Agent  
**Confidence**: HIGH — MALICIOUS

---

## 1. File Metadata

| Field | Value |
|-------|-------|
| Filename | `ms_x64_update.exe` |
| SHA256 | `8f18c0411cea16c7b037582ec485ccb8cb8afbb48ba71608761fe1f4bb98433e` |
| MD5 | `f1bc44c21eaf0e7d4ac642a0b6b79c51` |
| SHA1 | `4f8170d94d3f6e43e8e473d0514a587d69ddbde2` |
| Size | 64,803,120 bytes (61.8 MB) |
| Type | PE32 x86 GUI, Inno Setup 6.7.0 stub |
| Compiler | Delphi (TurboLinker) |
| Certificate Subject | WILLIAM LAWLER (Acton, California, US) |
| Certificate Issuer | Microsoft ID Verified CS AOC CA 02 |
| Certificate Validity | 2026-03-30 → 2026-04-02 **(3-day cert)** |
| Certificate Serial | `3300089bebadc9e0c3b72199ad000000089beb` |
| Product Name | NativeJSEApp 1.0.0 |
| Internal Name | SetupLdr.e32 |
| Overlay | 63,904,048 bytes (99% of file = Inno Setup archive) |

**InnoSetup Archive**  
| Field | Value |
|-------|-------|
| SHA256 | `0a2b6c6153e9b376a6e683e317659e117fd5c3028d445509907c6ef06e8142cd` |
| App Name | NativeJSEApp v1.0.0 |
| Inno Version | 6.7.0 |
| Payload Encryption | ChaCha20 (developer path: `D:\Coding\Is\issrc-build\Components\ChaCha20.pas`) |

---

## 2. Classification

**Family**: Novel campaign — NVIDIA GeForce Experience Lure / Electron Infostealer  
**Confidence**: HIGH CONFIDENCE MALICIOUS  

**Reasoning**:
- 3-day Microsoft ID Verified certificate (disposable signing identity) — identical TTP to DDinosaur/MTSetup campaigns
- ChaCha20.pas developer build path matches CryptoVista Stage 1 and ETDucky Stage 3 installer — links to the same threat actor or tooling
- Legitimate NVIDIA UI components and GeForce Experience consent texts used as lure
- Electron main process (`film.js`, 2.2MB) is heavily obfuscated with `javascript-obfuscator`; references `child_process`, `systeminformation`, `zip-lib`, `cookie`, `keys`, `https` — consistent with a credential harvester/RAT
- PHP 8.3.15 runtime bundled (unusual for a legitimate NVIDIA installer)
- Auto-bypass captcha (20-second timer regardless of user input)
- Filename `ms_x64_update.exe` masquerades as a Windows x64 update

---

## 3. Capabilities

- **Lure / Social Engineering**: Presents as NVIDIA GeForce Experience installer with genuine NVIDIA UI components, real consent texts in Italian/Slovenian/Dutch, genuine `InstallerExtension.dll` (DigiCert/NVIDIA signed)
- **Captcha Anti-Analysis Gate**: "Create a nickname" UI with captcha; auto-bypassed after 20-second `setTimeout` (secret code = "Hello world!" — trivially guessable)
- **System Reconnaissance**: `systeminformation` Node.js module collects CPU, GPU, memory, network, processes, OS details
- **Process Execution**: `child_process` module enables shell command and executable spawning
- **File Archiving / Exfiltration**: `zip-lib` used to create ZIP archives (likely for data exfiltration)
- **HTTP/HTTPS C2**: `http` and `https` Node.js modules for network communication (C2 endpoint obfuscated)
- **Browser Credential Theft**: References to `cookie` and `keys` in film.js suggest Chromium/browser credential harvesting
- **ChaCha20 Payload Encryption**: Inno Setup payload encrypted with ChaCha20 (same developer code as CryptoVista/ETDucky)
- **PHP Runtime Bundling**: PHP 8.3.15 (build path `D:\a\php-ftw\php-ftw`) bundled in Electron app — possibly for server-side scripting or additional payloads
- **IPC Trigger**: `launch-success` Electron IPC event triggers main malicious routine from renderer process

---

## 4. Attack Chain

```
Stage 1: ms_x64_update.exe (Inno Setup 6.7.0 stub)
│ • Signed with 3-day Microsoft ID Verified cert (WILLIAM LAWLER)
│ • ChaCha20-encrypted 63.9MB overlay
│ • Delphi SetupLdr.e32 stub launches installer
│
▼
Stage 2: Inno Setup Extraction (NativeJSEApp v1.0.0)
│ • Decrypts ChaCha20 payload
│ • Extracts to {app}/ directory:
│   - App.exe (120MB Electron executable)
│   - resources/app.asar (13.7MB Electron application)
│   - resources/elevate.exe (UAC elevation helper)
│   - Genuine NVIDIA DLLs (InstallerExtension.dll, etc.)
│   - PHP 8.3.15 runtime files
│
▼
Stage 3: Electron App Launch (App.exe)
│ • Loads app.asar containing film.js (main process)
│ • Shows NVIDIA GeForce Experience UI lure
│ • Displays "Create a nickname" captcha page
│
▼
Stage 4: Captcha Bypass → Payload Trigger (20-second timer)
│ • setTimeout(20000) fires window.api.showSecretWindow("launch-success")
│ • Sends IPC "launch-success" to Electron main process (film.js)
│ • OR user enters "Hello world!" to manually trigger
│
▼
Stage 5: film.js Malicious Execution (C2 unknown — fully obfuscated)
  • systeminformation → system/hardware/network recon
  • child_process → execute downloaded payloads or shell commands
  • zip-lib → archive and exfiltrate data
  • https → C2 communication (endpoint undetermined)
  • Browser cookie/key theft (Chromium credential access)
```

---

## 5. IOCs

### Network (defanged)
> **C2 endpoints could not be recovered** — all URLs, IPs, and hostnames in `film.js` are fully obfuscated with `javascript-obfuscator` (hex-encoded char arrays, string rotation, control flow flattening). Dynamic analysis required to recover runtime C2 configuration.

### Filesystem
| Path | Description |
|------|-------------|
| `{LOCALAPPDATA}\NativeJSEApp\` (inferred) | Expected Inno Setup install location |
| `{app}\resources\app.asar` | Electron app archive containing film.js |
| `{app}\App.exe` | 120MB Electron launcher |
| `{app}\resources\elevate.exe` | UAC elevation helper |

### Certificate
| Field | Value |
|-------|-------|
| Subject | WILLIAM LAWLER |
| Issuer | Microsoft ID Verified CS AOC CA 02 |
| Serial | `3300089bebadc9e0c3b72199ad000000089beb` |
| Valid | 2026-03-30 → 2026-04-02 |

### Hashes
| File | SHA256 |
|------|--------|
| ms_x64_update.exe | `8f18c0411cea16c7b037582ec485ccb8cb8afbb48ba71608761fe1f4bb98433e` |
| InnoSetup archive | `0a2b6c6153e9b376a6e683e317659e117fd5c3028d445509907c6ef06e8142cd` |
| film.js (app.asar) | (extracted from ASAR, 2,261,331 bytes) |
| InstallerExtension.dll | `22c39092e1810af94b4d2bc6285b763c429052d92cd0a4d015ca8ad27761693b` *(legitimate NVIDIA)* |
| System.Net.dll | `87f94839e0a7bf2b0713af9565717f608444d82e902b5f25e6a1b02ddfcbdd1a` *(legitimate Microsoft)* |
| TextTransformCoreResolver.dll | `26e977e04327927dc4a194137585db9b20f54b20d838d2be703dcb980b865a82` *(legitimate Microsoft)* |

### YARA Indicators
- `InnoInstaller` (Inno Setup 6.7.0)
- `ValuableFileExtensions` (ransomware-style file extension list in Inno stub)
- `ElevatePrivileges`

---

## 6. Emulation Results

**Speakeasy (x86, 60s)**: No IOCs recovered. The Inno Setup stub initializes Windows GUI and decompresses the overlay — the actual malicious behavior is in the Electron runtime which speakeasy cannot emulate (Node.js/V8 dependency).

**Emulation notes**: Full behavioral coverage requires running the Electron application in a real Windows environment with network monitoring. The 20-second timer before `launch-success` fires may evade short-lived sandbox runs.

---

## 7. Sandbox Results

**Tria.ge**: API key not configured — submission skipped.

**Recommended**: Submit to Tria.ge or Any.run with a Windows 10 profile, allow 5+ minutes, capture:
- Network traffic (PCAP) during Electron execution
- `%APPDATA%` and `%LOCALAPPDATA%` file activity
- Process tree spawned by App.exe → child_process calls

---

## 8. Key Embedded Files Analysis

### film.js (Electron Main Process, 2.26MB)
- **Obfuscation**: `javascript-obfuscator` with string array rotation, control flow flattening, hex char-by-char encoding of all string literals
- **Modules required**: `http`, `https`, `child_process`, `systeminformation`, `path`, `electron`, `os`, `fs`, `zip-lib`
- **Notable strings** (decoded from hex): `cookie`, `keys`, `child_process`, `zip-lib`, `http`, `https`
- **Captcha logic** (`assets/js/script.js`): Secret code = `"Hello world!"`; auto-trigger after 20,000ms regardless
- **IPC events**: `captcha-success` (manual), `launch-success` (automatic 20s timer) → triggers main payload
- **C2**: Fully obfuscated — not recoverable via static analysis

### InstallerExtension.dll (1.04MB)
- **Legitimate NVIDIA** DLL signed by NVIDIA Corporation / DigiCert
- File: Extension for NVIDIA Installer UI, v2.1002.408.0
- PDB: `Z:\sw\rel\gfclient\rel_03_28\InstallerUiMsg\InstallerExtension\Release\InstallerExtension.pdb`
- 9.7KB overlay = standard Authenticode signature (WIN_CERTIFICATE structure)
- **Assessment**: Entirely legitimate — used as part of the GeForce Experience lure

### System.Net.dll / TextTransformCoreResolver.dll
- Both legitimate Microsoft-signed .NET 6.0/VS17 components
- Forged future debug timestamps (2079, 2073) — cosmetic artifact, not indicative of tampering
- Both overlays are standard Authenticode signatures
- **Assessment**: Legitimate — bundled as Electron/.NET runtime dependencies

### NVIDIA UI Files (FunctionalConsent_it-IT.txt, 0401.ui.forms)
- Genuine NVIDIA GeForce Experience consent text (Italian)
- Genuine NVIDIA installer XML form definitions (locale 0x0401 Arabic/Arabic-SA)
- **Assessment**: Legitimate — used as high-fidelity lure components

### PHP Runtime (vital/snapshot.txt)
- PHP 8.3.15, built 2024-12-17, Windows x64 VS16
- Build path: `D:\a\php-ftw\php-ftw\php\vs16\x64\obj\Release`
- Includes curl, openssl, sqlite3, zip, mbstring — full-featured PHP
- **Purpose unknown** — may be used by film.js via `child_process` to execute PHP payloads, or present as decoy

---

## 9. Campaign Attribution

| Indicator | Match |
|-----------|-------|
| `ChaCha20.pas` path (`D:\Coding\Is\issrc-build\Components\ChaCha20.pas`) | CryptoVista Stage 1, ETDucky Stage 3 |
| 3-day Microsoft ID Verified CS AOC CA 02 cert | DDinosaur (Ricardo Reis), MTSetup (Tryphena Lewis) |
| Inno Setup 6.7.0 with ChaCha20 | CryptoVista Stage 1 |
| NVIDIA GeForce lure + Italian consent text | ETDucky (Italian text theme) |
| javascript-obfuscator on Electron app | ETDucky, CryptoVista |

**Assessment**: This sample is highly likely from the **same threat actor** as the CryptoVista/ETDucky campaign cluster, now deploying a new NVIDIA GeForce Experience lure vector. The `ChaCha20.pas` build path is a strong developer fingerprint.

---

## 10. Analyst Notes

1. **C2 not recovered**: All network endpoints in film.js are obfuscated below the level of static analysis. Dynamic execution in a monitored sandbox is the recommended next step.

2. **PHP runtime purpose**: The inclusion of PHP 8.3.15 is unusual and not yet explained. Possible uses: executing a PHP-based infostealer stage, acting as a local HTTP server for C2 relay, or purely as camouflage.

3. **Electron app vs. NVIDIA installer confusion**: The installer ships genuine NVIDIA components alongside the malicious Electron app. On execution, a victim would see a seemingly legitimate NVIDIA GeForce Experience installation sequence, making this a high-credibility lure.

4. **20-second timer**: The captcha bypass timer (`setTimeout 20000ms`) means the malware fires immediately without any user interaction beyond running the installer — the "captcha" is purely UI theater.

5. **YARA match `ValuableFileExtensions`**: The Inno Setup stub contains a list of file extensions commonly targeted by ransomware-style stealers. This may indicate a file-exfiltration capability in the Inno script or payload.

6. **Recommended follow-up**:
   - Dynamic analysis in Any.run / Tria.ge (5+ minute session, Windows 10)
   - Extract and deobfuscate film.js using a JavaScript deobfuscation sandbox (e.g., `synchrony`)
   - Monitor for `php.exe` or `php-cgi.exe` spawning from App.exe process tree
   - Check `webpack-node-externals` node_modules for additional hidden payloads

---

*Report generated by REMnux Claude Agent — 2026-04-07*
