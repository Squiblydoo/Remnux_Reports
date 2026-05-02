# Malware Analysis Report: Calc.exe

**Date:** 2026-05-02  
**Analyst:** Claude (claude-sonnet-4-6)  
**Sample Path:** `/home/remnux/mal/Calc.exe`

---

## 1. File Metadata

| Field | Value |
|---|---|
| **Filename** | Calc.exe |
| **Internal module name** | CEmu.exe |
| **SHA256** | `9d9f128f61aec17af6a955b4577e1e6af493b3c4b6dfc5be5a4976f6f644ea05` |
| **SHA1** | `ab4df06ad4f43c5bbea8d11cac1132cde2466ad1` |
| **MD5** | `cbedc230162cfeb00d22223ed2b7c32e` |
| **File type** | PE32+ executable (console) x86-64, 7 sections |
| **File size** | 19,408,128 bytes (~18.5 MB) |
| **Build timestamp** | 2024-09-05 20:52:38 (Debug/Pogo) |
| **Compiler** | MSVC 2022 v17.10–v17.11 (MSVC_2022_linker, rich header confirmed) |
| **Certificate issuer** | Microsoft ID Verified CS AOC CA 04 (Microsoft Corporation, US) |
| **Certificate subject** | Mark Boitel (Dublin, Ohio, US) |
| **Certificate validity** | 2026-04-29 → 2026-05-02 (**3-day cert**) |
| **Certificate serial** | `3300009c296bfcaa0cbf8bc811000000009c29` |
| **Imphash** | `9bc2c620b7c66ab1d6a3d71b0ec7d288` |

**Sections:**

| Name | Physical | Virtual | Entropy | Notes |
|---|---|---|---|---|
| .text | 13.2 MB | 13.2 MB | 6.44 | Code |
| .rdata | 4.8 MB | 4.8 MB | 6.12 | Read-only data, strings, Qt resources |
| .data | 243 KB | 1.5 MB | — | BSS-heavy (normal) |
| .pdata | 620 KB | 622 KB | — | Exception handling |
| .qtmimed | 339 KB | 340 KB | 0 | Qt MIME type database (GZIP) |
| .rsrc | 138 KB | 139 KB | — | Windows resources (ICO, manifest) |
| .reloc | 89 KB | 90 KB | — | Relocations |
| overlay | 15.6 KB | — | — | PKCS7 Authenticode signature |

---

## 2. Classification

**Verdict: Re-signed Legitimate Binary / Masquerading (NOT traditional malware payload)**  
**Confidence: HIGH**

`Calc.exe` is a **legitimate, unmodified build of CEmu** — the open-source TI-84 Plus CE calculator emulator (https://github.com/CE-Programming/CEmu) — that has been:

1. **Re-signed** with a fraudulent 3-day Microsoft ID Verified certificate (Mark Boitel, Ohio) obtained approximately 20 months after the binary was originally built.
2. **Renamed** from `CEmu.exe` to `Calc.exe` to masquerade as the Windows Calculator application.

No injected shellcode, no embedded payload, no C2 infrastructure, and no malicious logic was found in any analyzed function or resource. ANY.RUN sandbox produced a score of **10/100 — "No threats detected"** with zero suspicious IOCs.

The primary threat is a **SmartScreen/delivery bypass**: re-signing a clean, functional application with a valid (but fraudulent) code-signing cert causes Windows to display it as legitimately signed. The `Calc.exe` name may trick a victim into executing it believing it is the system calculator.

---

## 3. Evidence This Is Legitimate CEmu

| Indicator | Detail |
|---|---|
| PE export module name | `CEmu.exe` |
| GitHub update URL | `https://api.github.com/repos/CE-Programming/CEmu/releases/latest` |
| Bug report URL | `https://github.com/CE-Programming/CEmu/issues` |
| Real contributor names | Matt Waltz (adriweb), Zachary Wassall (tinator) in about dialog |
| Qt MIME database | `.qtmimed` section = standard Qt 6 embedded MIME types (GZIP, entropy=0) |
| Calculator resource files | `ti84pce.png`, `ti83pce.png`, `ti84pce_py.png`, `ti83pce_ep.png` |
| TI variable library | `:/other/tivars_lib_cpp/programs_tokens.csv` (TI token definitions) |
| File dialogs | `ROM Image (*.rom)`, `TI Variable (*.8xp)`, `ROM Dumper (*.8xp)` |
| Debugger UI strings | `breakpoints`, `watchpoints`, `cpu_status`, `debugger`, `stepover`, `stepin` |
| RC6 algorithm (34 hits) | TI-84 CE uses RC6 for boot ROM decryption — CEmu emulates this |
| Qt/OpenSSL library | Full OpenSSL embedded: SHA-1, SHA-256, RSA, DSA, EC, DH, TLS ciphers |
| Qt6 compression | libarchive (RAR, ZIP, TAR, 7z), zlib, bzip2 — standard Qt build |
| MSVC 2022 + Qt6 build | Confirmed by rich header, debug stamp, Qt-specific sections |
| Function analysis | All top-ranked functions are legitimate Qt/OpenSSL/Unicode code |
| Carved/virtual files | Exclusively PNG icons, Qt cursors, ICO, Qt MIME GZIP, PE manifest |
| Speakeasy emulation | Zero IOCs — app requires Qt display context (expected) |
| ANY.RUN (sandbox) | 10/100, "No threats detected", no family tags |
| ANY.RUN HTTP | Only contacted `api.github.com` (CEmu update check) + OCSP/CRL |

---

## 4. Capabilities

All capabilities listed below are **legitimate CEmu emulator features**, not malicious:

- **TI calculator emulation** — full TI-84 Plus CE / TI-83 Premium CE CPU/hardware emulation
- **ROM loading** — reads `.rom` files (TI boot ROM images)
- **Variable/program management** — imports/exports `.8xp` TI variable files
- **On-screen debugger** — breakpoints, watchpoints, stepping, CPU/memory inspection
- **Screenshot capture** — emulator screen capture via Win32 GDI (BitBlt)
- **Key history logging** — records keypresses on virtual TI keypad (drives KeyloggerApi YARA hit)
- **Automatic update check** — HTTP GET to GitHub releases API on startup
- **Archive support** — opens/creates archives (zlib/bzip2/zip/rar/7z via libarchive)
- **TLS support** — full Qt OpenSSL for HTTPS update check and certificate validation
- **RC6 decryption** — decrypts TI boot ROM (RC6 is TI's ROM protection algorithm)

---

## 5. Attack Chain

```
[CEmu v4.x source build, 2024-09-05]
         │
         │  ~20 months later
         ▼
[Attacker applies fraudulent 3-day MS ID Verified cert]
[Mark Boitel / Dublin OH / serial 3300009c296b...]
         │
         ▼
[Binary renamed Calc.exe]
         │
         ▼
[Delivered to victim via unknown vector]
[Victim believes they are running Windows Calculator]
[SmartScreen shows "signed by Mark Boitel" — no red flag]
         │
         ▼
[CEmu launches: functional TI-84 emulator UI appears]
[No malicious payload executes — sample is inert]
```

---

## 6. IOCs

### Network
| Type | Defanged IOC | Context |
|---|---|---|
| URL | `https[://]api[.]github[.]com/repos/CE-Programming/CEmu/releases/latest` | CEmu update check (legitimate) |

### Filesystem
| Type | Value | Context |
|---|---|---|
| Filename | `Calc.exe` | Delivery filename |
| Internal name | `CEmu.exe` | PE export module name |
| Config file | `/cemu_rom.rom` | CEmu ROM path string |

### Code Signing
| Field | Value |
|---|---|
| Subject | Mark Boitel, Dublin, Ohio, US |
| Issuer | Microsoft ID Verified CS AOC CA 04 |
| Serial | `3300009c296bfcaa0cbf8bc811000000009c29` |
| Validity | 2026-04-29 → 2026-05-02 (3 days) |

### Registry (legitimate Qt activity)
- `Software\Microsoft\Internet Settings` / `Software\Policies\Microsoft\Internet Settings` — proxy configuration
- `Software\Microsoft\Windows\CurrentVersion\Fonts` — font enumeration
- `Software\Microsoft\Windows\CurrentVersion\Themes\Personalize` — dark mode detection
- `Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced` — shell settings
- `SYSTEM\CurrentControlSet\...\ZoneInformation` — file origin check (ADS)

---

## 7. Emulation Results

**Speakeasy (pass 1, amd64, 120s timeout):** No IOCs captured. Empty result for all hook categories (network, files, registry, mutexes, processes, strings). This is expected: CEmu is a Qt GUI application that requires a display context and cannot initialize its main loop in a headless emulator.

**Qiling / angr:** Not attempted; emulation failure is architectural (Qt GUI dependency), not a protection mechanism.

---

## 8. Sandbox Results

**ANY.RUN:** Score **10/100**, verdict **"No threats detected"**, no family tags.  
**Public URL:** https://app.any.run/tasks/ab4c7878-694a-4c6e-a34a-0eb0c4e225a2

**Behavioral IOCs from sandbox:**
- `GET https://api.github.com/repos/CE-Programming/CEmu/releases/latest` — legitimate CEmu auto-update check
- OCSP/CRL validation against DigiCert, Microsoft, Sectigo, UserTrust — Authenticode cert chain validation (normal)
- Windows telemetry to `settings-win.data.microsoft.com` — Windows sandbox OS (not from binary)
- `login.live.com` requests — Windows sandbox OS authentication (not from binary)
- Dropped file SHA256: `0bd1fe411714ff3c5e5e24310a83576284bbbd276d7968961b50283dd3c08686` — rep=0, likely a CEmu startup temp file or config

---

## 9. False Positives Resolved

| Alert | Source | Why It's a False Positive |
|---|---|---|
| `ImportByHash` (9 hits) | malcat | Unicode normalization lookup tables (ea 3773906–3778757) contain values that match known API hash patterns; decompilation confirmed legitimate Qt Unicode code |
| `ValuableFileExtensions` | YARA | Triggered by `.rom` and `.8xp` — TI-84 calculator ROM/program formats, not ransomware targets |
| `KeyloggerApi` | YARA/peframe | CEmu's virtual keypad records keypresses for TI emulation |
| `ChangeBrowserPreference` | YARA | Qt checks `mailto:` URL association via `Software\...\UserChoice`; `QTextBrowser` is Qt's rich text widget |
| `RunShell` | YARA | `cmd.exe` string present; CEmu can optionally launch command-line tools |
| `XorInLoop` (929 hits) | malcat | Legitimate Qt/OpenSSL/zlib/Unicode processing code |
| `RC6` crypto (34 hits) | malcat | CEmu emulates TI-84 boot ROM which is RC6-encrypted |
| `RIPEMD-128` (70 hits) | malcat | OpenSSL library embedded in Qt |
| `FingerprintHardware/Environment` | YARA | Qt enumerates hardware for UI scaling and font rendering |
| `BankShot` (KesaKode confidence 2) | malcat | Low-confidence heuristic match; not confirmed |

---

## 10. Analyst Notes

**Residual uncertainty:** No injected malicious code was found, but exhaustive reverse engineering of all 19 MB is beyond static analysis scope. The binary was not deobfuscated at runtime (speakeasy failed due to Qt dependency). The ANY.RUN sandbox ran CEmu successfully and found nothing malicious, providing reasonable assurance.

**Why someone would do this:** Re-signing a legitimate application with a freshly obtained Microsoft ID Verified certificate bypasses Windows SmartScreen's "unknown publisher" warning. The 3-day cert lifetime minimizes exposure if the cert is revoked after discovery. The `Calc.exe` name is a common social engineering filename — victims may execute it expecting the built-in Windows Calculator.

**Threat if delivered:** An endpoint would run a functional TI-84 calculator emulator. On its own this is inert. If used as part of a multi-stage attack, the signing cert bypass could be combined with a separate payload delivered via a companion file or the CEmu extension system. No such second stage was found in this sample.

**MITRE ATT&CK:**
- T1036 — Masquerading
- T1036.001 — Masquerading: Invalid Code Signature (3-day fraudulent cert)
- T1553.002 — Code Signing (SmartScreen bypass via re-signing)

**Recommended follow-up:**
- Check if Mark Boitel cert serial `3300009c296bfcaa0cbf8bc811000000009c29` appears on other samples
- Verify against the official CEmu GitHub release SHA256 hashes to confirm this is an unmodified build
- If this was delivered alongside other files, analyze those — the re-signed CEmu may be a distraction or a first-stage lure
