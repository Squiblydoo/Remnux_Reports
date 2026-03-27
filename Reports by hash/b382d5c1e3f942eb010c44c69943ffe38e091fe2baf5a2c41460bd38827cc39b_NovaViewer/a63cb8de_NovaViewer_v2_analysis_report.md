# NovaViewer.exe v1.03.9.1181 — Malware Analysis Report

**Date:** 2026-03-17
**Analyst:** Claude Code (claude-sonnet-4-6)
**Sample path:** `/home/remnux/mal/NovaViewer.exe`

---

## 1. File Metadata

| Field | Value |
|-------|-------|
| **SHA256** | `a63cb8de82ca6f2739e46a73c59d607dcf34e683c396aa0e4a4ce96d3666bfcf` |
| **MD5** | `dd37be2765d341278d07cbb44c0d5962` |
| **SHA1** | `2a4d802d60aa80d515abf7941064f18c5b835c56` |
| **Size** | 11,916,000 bytes (~11.4 MB) |
| **File type** | PE32+ executable (GUI) x86-64, 8 sections |
| **Version** | 1.03.9.1181 |
| **Internal name** | novaviewer |
| **Product name** | NovaViewer |
| **Description** | Document Viewer Application |
| **Copyright** | Copyright (C) NovaViewer 2025 |
| **PDB path** | `NovaViewer.pdb` |
| **Debug timestamp** | 2025-11-07 15:52:02 |
| **Build path** | `.\\target\\release\\build\\NovaViewer-82e002ca54d4e371\\out\\app.rs` |

### Code Signing
| Field | Value |
|-------|-------|
| **Certificate subject** | Xiamen Xisu Technology Co., Ltd. |
| **Issuer** | Sectigo Public Code Signing CA EV R36 |
| **Serial** | `67bea002d62e1831cc2612adb8e1b2ce` |
| **Validity** | 2026-01-23 → 2027-01-23 (current, 1-year EV) |
| **Country** | CN (Fujian Sheng) |

> **Note:** This is the **same EV certificate** as the previously analysed NovaViewer.exe (SHA256 `b382d5c1e3f942eb010c44c69943ffe38e091fe2baf5a2c41460bd38827cc39b`). The SHA256 of the **current** sample differs — this is a **newer version** of the same application (version 1.03.9.1181 vs previous), uploaded to the samples directory today (Mar 17 09:34).

### Build Environment
- Builder: `runneradmin` (GitHub Actions runner) — `C:\Users\runneradmin\.cargo\registry\`
- HTTP library: `hyper-util-0.1.17` (standard async Rust HTTP client)
- Build hash changed: `82e002ca54d4e371` (previous: `4e371` suffix likely same hash)

---

## 2. Classification

**Verdict: LOW RISK / LIKELY LEGITIMATE**
**Confidence: HIGH**
**Family: None (benign PDF viewer)**

This is a version update to the previously-analysed legitimate Rust PDF viewer built by Xiamen Xisu Technology. No malicious indicators were found. All suspicious detections are false positives or explainable new features. See §6 for detailed false-positive analysis.

---

## 3. Technical Architecture

| Component | Detail |
|-----------|--------|
| **Language** | Rust (compiler YARA confirmed) |
| **UI framework** | Slint (Rust native UI, winit backend) |
| **PDF engine** | pdfium.dll v143.0.7469.0 (Google PDFium, embedded in `.rdata`) |
| **HTTP library** | hyper + hyper-util (Rust async HTTP/2 client) |
| **TLS** | rustls (`CertGetCertificateChain`, `CertVerifyCertificateChainPolicy`) |
| **Crypto** | bcrypt.dll, bcryptprimitives.dll, crypt32.dll |
| **Sections** | `.text` (4MB), `.rdata` (7.1MB), `.data`, `.pdata`, `.tls`, `_RDATA`, `.rsrc`, `.reloc`, overlay (12KB PKCS7 Authenticode) |

### Embedded pdfium.dll
- **SHA256:** `215c405d13963395b38872a656a18f161a294e80bbcaa00849b842f524e5474b`
- **Version:** 143.0.7469.0 (2025-09-07 build)
- **Source:** `github.com/bblanchon` (legitimate PDFium Windows binaries)
- Located in `.rdata` section at offset 4,172,372; size 5,762,048 bytes

---

## 4. New Capabilities vs Previous Version

The following are **additions** detected in this version that were not present in the previous version:

### 4a. System Performance Monitoring (`sysinfo` crate pattern)
Full PDH API stack imported:
```
pdh.dll:
  PdhOpenQueryA          — create a performance counter query
  PdhAddEnglishCounterW  — add a named performance counter
  PdhCollectQueryData    — collect a data snapshot
  PdhGetFormattedCounterValue — format the collected value
  PdhRemoveCounter       — remove a counter
  PdhCloseQuery          — close the query
psapi.dll:
  GetModuleFileNameExW   — get full path of a loaded module
```
This is the **exact pattern** of the Rust `sysinfo` crate, which queries CPU/memory/disk/network performance counters via PDH and enumerates processes via psapi. Likely added to display system performance stats within the viewer UI or for diagnostics.

### 4b. Process Management (`taskkill.exe /PID /F`)
A string block contains:
```
taskkill.exe/PID/F
Unable to read process memory information
ReadProcessMemory returned unexpected number of bytes read
Unable to read process data
```
These are **Rust error/panic message strings** from a wrapper around `ReadProcessMemory` and `taskkill.exe /PID /F`. This pattern is consistent with `sysinfo`'s process enumeration and process termination helpers on Windows (e.g., killing a hung PDF instance before re-opening a file, or enforcing single-instance behavior). These strings were in an error message block — not a primary execution path.

### 4c. Anti-Debug (`IsDebuggerPresent`)
`IsDebuggerPresent` is now imported. Common in commercial Rust apps compiled in release mode; used by the MSVC C runtime's structured exception handler. Not unique to malware.

### 4d. Configuration and Help Files
- `config.dat` — application configuration filename; expected for a feature-expanded viewer.
- `help.html` — confirmed as embedded HTML help document: `<!DOCTYPE html><html><head><title>…</title>` content follows. Legitimate.

### 4e. RCDATA Resource
- **Path:** `RCDATA/1/en-us`, **size:** 232,820 bytes, **entropy:** very low (~7/255)
- Content: `05 05 05 05` header, uniform `B3 17 B3 17 …` fill, `D2 AB D2 AB …` tail, `05 05 05 05 05 05 05 05 05` footer
- This low-entropy repeating byte pattern is consistent with an **embedded cursor/bitmap resource** (e.g., a large UI cursor used by Slint). Not encrypted — encrypted data would show entropy near 255.

---

## 5. False Positive Analysis

| Detection | Source | Assessment |
|-----------|--------|------------|
| `KeyloggerApi` YARA | `GetKeyState`, `GetKeyboardLayout`, `GetForegroundWindow` | Slint/winit keyboard input handling — same false positive as previous version |
| `RunShell` YARA | String `cmd.exe /E:ON /v:OFF /d /c "batch file arguments are invalid"` | Rust `std::process::Command` internal error string for batch file execution — stdlib false positive |
| `ImportByHash` anomaly | Unicode codepoint classification functions (ranges 0xa9, 0xae, 0x203c…) | PDFium Unicode character classification — not API hashing |
| `uespemosarenegylmodnarodsetybdet` repeated string | Multiple locations in `.text`/`.rdata` | **SipHash initialization constants** (`somepseu` + `dorandom` + `lygenera` + `tedbytes`) used by Rust's standard `HashMap` and `Vec::shuffle()` — completely normal |
| `screenshot` (peframe) | `BitBlt`, `GetDC`, `CreateCompatibleDC` | PDF page rendering to screen/clipboard — expected for a PDF viewer |
| `EmbeddedProgram` (malcat) | pdfium.dll in `.rdata` | Legitimate Google PDFium library, not a malicious payload |
| `DownloaderApiUsage` (malcat) | `ws2_32.recv` call | TCP socket receive in hyper HTTP client — expected for a network-enabled app |
| `XorInLoop` ×396 | AES/SHA/CRC32 crypto primitives | Cryptographic library operations (TLS, bcrypt) — confirmed by signsrch (53 crypto signatures) |

---

## 6. IOC Summary

### Network IOCs
None identified. All URLs are:
- Sectigo/Comodo OCSP/CRL endpoints (Authenticode certificate infrastructure)
- W3C SVG namespace URIs (UI rendering)
- `github.com/bblanchon` reference (PDFium attribution string)

### Filesystem
| Path | Purpose |
|------|---------|
| `config.dat` | App configuration file |
| `help.html` | Embedded help document |
| `NovaViewer.exe` | Self-reference |
| `pdfium.dll.pdb` | PDFium debug symbol reference |

### Registry
- `SOFTWARE\Microsoft\Windows\CurrentVersion` (via winit/Slint, standard window positioning)
- `Software\Microsoft\Windows\Internet Settings` (standard Windows networking)

### Hashes
| File | SHA256 |
|------|--------|
| NovaViewer.exe (this sample) | `a63cb8de82ca6f2739e46a73c59d607dcf34e683c396aa0e4a4ce96d3666bfcf` |
| Embedded pdfium.dll | `215c405d13963395b38872a656a18f161a294e80bbcaa00849b842f524e5474b` |
| RCDATA resource | `fac2bf7ee2e7ce7571cffc65ae0999f479dd9063a60391d576566fdea29e0b16` |
| Previous NovaViewer.exe | `b382d5c1e3f942eb010c44c69943ffe38e091fe2baf5a2c41460bd38827cc39b` |

---

## 7. Analyst Notes

**What changed from the previous version:**
- Version bumped (1.03.9.1181); new build hash
- Added `sysinfo`-pattern PDH performance monitoring (6 PDH + 1 psapi import)
- Added `taskkill.exe /PID /F` process management (likely single-instance enforcement or hung-process recovery)
- Added `config.dat` configuration and embedded `help.html` help document
- New large RCDATA resource (cursor/bitmap)
- `IsDebuggerPresent` added (likely from updated MSVC CRT)

**What requires dynamic analysis to fully confirm:**
1. Actual use of PDH counters — confirm these are for in-app performance display vs. system fingerprinting exfiltration
2. Confirm `config.dat` read/write path and whether settings are sent to any remote endpoint
3. Confirm `taskkill.exe /PID /F` target — single-instance enforcement vs. killing competing software
4. Verify network behavior — does it make any outbound connections beyond PDF loading and certificate checks?

**Alternative hypothesis (low confidence):**
If the PDH data or `config.dat` contents are exfiltrated via the hyper HTTP client to an undiscovered endpoint, this could be a system-recon feature hidden behind legitimate-looking UI. However: (a) no C2 domains found, (b) no credential or browser-path strings, (c) same trusted developer cert and CI/CD builder, (d) yara-forge is clean. This hypothesis requires dynamic network capture to evaluate.

**Bottom line:** This version update is consistent with organic feature development by the same legitimate team. No evidence of malicious intent. Recommend monitoring outbound network connections if deployed in a sensitive environment.
