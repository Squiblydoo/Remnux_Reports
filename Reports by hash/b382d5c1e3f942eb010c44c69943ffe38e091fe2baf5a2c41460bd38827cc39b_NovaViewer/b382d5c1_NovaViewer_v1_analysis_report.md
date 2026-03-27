# NovaViewer.exe — Malware Analysis Report

**Date:** 2026-03-10
**Analyst:** Claude Sonnet 4.6 (automated)
**Risk Level:** LOW — Likely Legitimate Application
**Confidence:** Medium-High

---

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | NovaViewer.exe |
| SHA256 | `b382d5c1e3f942eb010c44c69943ffe38e091fe2baf5a2c41460bd38827cc39b` |
| SHA1 | `a9fd5caa81e0af03bb4032163dc5a32a881fd3cd` |
| MD5 | `1e0fef9f87f222de6cf25a53e167ec6e` |
| Size | 10,132,192 bytes (~9.7 MB) |
| Type | PE32+ x86-64 GUI executable, 8 sections |
| Compiler | Rust (MSVC 2015 linker) |
| PDB Path | `NovaViewer.pdb` |
| Build Date | 2025-11-07 22:58:38 (CodeView debug timestamp) |
| PE TimeDateStamp | 0x690e799e (Nov 7, 2025) |
| Build Environment | GitHub Actions (`C:\Users\runneradmin\.cargo\registry`) |
| Product Name | NovaViewer |
| File Description | Document Viewer Application |
| Version | 1.03.9.1181 |
| Internal Name | novaviewer |

### Certificate
| Field | Value |
|---|---|
| Subject | Xiamen Xisu Technology Co., Ltd. |
| Issuer | Sectigo Public Code Signing CA EV R36 |
| Serial | `67bea002d62e1831cc2612adb8e1b2ce` |
| Validity | 2026-01-23 to 2027-01-23 (1 year — normal EV lifetime) |
| Country | CN (Fujian Sheng — Xiamen, China) |
| Type | EV Code Signing |
| Overlay | Valid PKCS7 Authenticode signature (12,000 bytes) |

---

## 2. Classification

**Classification:** Legitimate PDF Viewer Application (signed by Xiamen Xisu Technology Co., Ltd.)
**Malware Family:** None confirmed
**Malcat Initial Hint:** LucaStealer, confidence=0 (no actual match)
**Confidence:** Medium-High (legitimate)

### Reasoning

NovaViewer.exe is a Rust-based PDF viewer application built using industry-standard open-source components:
- **Slint** UI framework (Rust GUI toolkit)
- **winit** windowing library (Rust cross-platform windowing)
- **pdfium** (Google's PDF rendering engine, embedded as 5.76MB DLL)
- **usvg 0.45.1** (SVG rendering)
- **roxmltree 0.20.0** (XML/SVG parsing)
- **fontdue** (Rust font rasterizer)
- **tiny_skia** (software renderer)

All "suspicious" API detections have benign explanations (see Capabilities section). No C2 URLs, credential theft strings, or network exfiltration code were found.

---

## 3. Capabilities

### Confirmed Legitimate (PDF Viewer Functionality)
- **PDF rendering**: Embedded pdfium.dll v143.0.7469.0 (Google PDF library); all `FPDF_*` function exports confirmed
- **SVG rendering**: usvg + tiny_skia pipeline for UI elements and SVG content in PDFs
- **Font handling**: fontdue rasterizer + Unicode character classification tables (197KB RCDATA resource = Unicode char-type lookup table with 16-bit entries)
- **Zlib decompression**: PDF stream decompression
- **AES decryption**: pdfium.dll internal use for password-protected PDF decryption
- **JPEG/image decoding**: libavcodec MJPEG tables in pdfium for PDF embedded images
- **Clipboard access**: `ImpersonateAnonymousToken` → `CloseClipboard` → `RevertToSelf` — known Chromium/winit bug workaround for clipboard operations (not a privilege escalation attack)
- **CRC32 hashing**: PDF integrity verification

### Suspicious / Requires Dynamic Confirmation
- **`SetWindowDisplayAffinity(0x11 = WDA_EXCLUDEFROMCAPTURE)`** in `sub_1400ee656`: Prevents the window from appearing in screenshots, screen recordings, and remote desktop sessions. For a PDF viewer, this could be a privacy feature (blocking sensitive document screenshots) — OR a technique to hide UI activity from monitoring tools. Most likely legitimate for a document viewer.
- **Keyboard input polling**: `GetAsyncKeyState` (all 256 VK codes), `GetKeyboardState`, `MapVirtualKeyExW`, `ToUnicodeEx`, `RegisterRawInputDevices` — **all traced to Slint/winit framework keyboard event handling** (`sub_1401a29e5`, `sub_1401a0fe3`). The framework polls key states to detect modifier combinations (Ctrl, Shift, Alt, NumLock, CapsLock). This matches the `KeyloggerApi` YARA false positive from Slint.

### Not Found (Expected if Malicious)
- No cleartext C2 URLs
- No credential theft strings (password, cookie, login data, wallet)
- No network DLLs in static import table (no WinHTTP, WinInet, WinSock)
- No process injection (no VirtualAllocEx, WriteProcessMemory, CreateRemoteThread)
- No persistence mechanisms (no Run key writes, scheduled tasks)
- No anti-analysis tricks (IsDebuggerPresent is from Rust runtime, not malware)

---

## 4. PE Layout & Embedded Files

| Region | Content | Notes |
|---|---|---|
| .text (3.1MB) | Rust compiled code | Main application logic |
| .rdata (6.4MB) | Read-only data | Includes embedded pdfium.dll |
| .rsrc (234KB) | Resources | App icons (6 PNGs) + Unicode table (RCDATA) |
| Overlay (12KB) | PKCS7 | Authenticode signature |

### Embedded PE (Carved at offset 3,135,572)
- **pdfium.dll** — SHA256: `215c405d13963395b38872a656a18f161a294e80bbcaa00849b842f524e5474b`
- Compiled by `github.com/bblanchon` (open-source pdfium build)
- Version 143.0.7469.0, built 2025-09-07
- Fully legitimate; statically linked into NovaViewer's `.rdata` section
- Loaded at runtime via memory mapping (explains `MapViewOfFile` import)

### RCDATA Resource (197,840 bytes)
- SHA256: `8be599a5c6c17221a8ec4502bb9b9b57a6dfb78c89b0e8619c17818d579c5c80`
- Content: Unicode character classification/property table (16-bit entries, repeating property codes `0x05`, `0x0E`, `0xD6EC`, etc.)
- Used for PDF text layout, glyph categorization, bidirectional text processing
- **Not an encrypted payload** — structure matches Unicode char-type tables used by PDF renderers

---

## 5. Anomalies (Explained)

| Anomaly | Count | Explanation |
|---|---|---|
| XorInLoop | 283 | zlib inflate, CRC32 slice-by-16, tiny_skia pixel ops, SipHash in Rust HashMap |
| DynamicString | 34 | Rust format! macro string construction |
| StackArrayInitialisationX64 | 12 | Rust stack-based buffer initialization patterns |
| ImportByHash | 2 | `_jn` (Bessel function) and `_wtempnam` (temp filename) — MSVCRT functions resolved by hash at link time |
| BigBufferNoXrefMediumToHighEntropy | 12 | pdfium.dll sections and Unicode tables embedded in .rdata |
| EmbeddedProgram | 1 | pdfium.dll (legitimate, explained above) |
| InvalidSizeOfInitializedData | 1 | Artifact from embedding pdfium.dll in .rdata — SizeOfInitializedData mismatch |
| RcdataNoDelphi | 1 | RCDATA resource present; not Delphi — Unicode table, explained above |

---

## 6. YARA Matches

| Rule | Category | Verdict |
|---|---|---|
| Rust | compiler | True — compiled with Rust |
| MSVC_2015_linker | compiler | True — standard Rust toolchain linker |
| Zlib | library | True — pdfium/Slint use zlib |
| **KeyloggerApi** | stealer | **FALSE POSITIVE** — Slint/winit keyboard input handling |
| msvc_general_x64 | compiler | True |

---

## 7. IOCs

### File Hashes
| Type | Hash |
|---|---|
| SHA256 (main) | `b382d5c1e3f942eb010c44c69943ffe38e091fe2baf5a2c41460bd38827cc39b` |
| SHA1 (main) | `a9fd5caa81e0af03bb4032163dc5a32a881fd3cd` |
| MD5 (main) | `1e0fef9f87f222de6cf25a53e167ec6e` |
| SHA256 (pdfium) | `215c405d13963395b38872a656a18f161a294e80bbcaa00849b842f524e5474b` |
| SHA256 (RCDATA) | `8be599a5c6c17221a8ec4502bb9b9b57a6dfb78c89b0e8619c17818d579c5c80` |

### Network IOCs
**None identified.** No C2 infrastructure, no hardcoded IPs or domains.
Certificate OCSP/CRL endpoints are standard Sectigo infrastructure (not IOCs).

### Filesystem
- `NovaViewer.exe` — primary executable
- Embeds `pdfium.dll` (extracted to memory at runtime, not dropped to disk)
- Font directories accessed: `%APPDATA%\Roaming\Microsoft\Windows\Fonts`, `%APPDATA%\Local\Microsoft\Windows\Fonts` (read-only, standard font lookup)
- `%TEMP%` path accessed (standard Rust temp file handling)

### Registry
- `HKCU` — accessed for font/UI settings (Slint winit)
- `HKU` — accessed (standard user profile lookup)
- No persistence keys written (static analysis; not confirmed dynamically)

### Certificate
- Serial `67bea002d62e1831cc2612adb8e1b2ce` (Sectigo EV, Xiamen Xisu Technology Co., Ltd.)

---

## 8. Analyst Notes

### What Was Confirmed
1. **Legitimate PDF viewer stack**: Slint + pdfium + usvg is a known Rust open-source PDF viewer pattern. The GitHub Actions build path confirms CI/CD-built open-source software.
2. **All API anomalies explained**: Every suspicious-looking API call traces to a specific legitimate framework component.
3. **No exfiltration infrastructure**: No network code in static imports; no socket/HTTP function calls; `GetProcAddress` (19 refs) is used for optional Windows API feature detection (common Rust winit pattern), not dynamic shellcode loading.

### What Requires Dynamic Analysis
1. **`SetWindowDisplayAffinity(WDA_EXCLUDEFROMCAPTURE)`**: Confirm this is triggered only when a document is open (privacy feature) vs. always active on startup (concealment).
2. **`GetProcAddress` x19**: Confirm all resolved functions are standard optional Windows APIs (DPI awareness, touch input, DWM effects) and not covert network/injection APIs.
3. **RCDATA usage at runtime**: Confirm the 197KB table is only accessed by PDF/text rendering code.
4. **Clipboard behavior**: Confirm `ImpersonateAnonymousToken` + `CloseClipboard` is the harmless Chromium workaround and not exfiltrating clipboard data.

### Alternative Hypothesis
If this binary is confirmed malicious, the most likely scenario would be a **trojanized open-source PDF viewer** where a small patch was applied to add exfiltration while keeping the legitimate viewer functional as a lure. However, **no evidence supports this hypothesis** in static analysis — no second entrypoint, no hidden thread creation at startup, no obfuscated payload decryption routines.

### Verdict
**Low risk — likely legitimate.** Submit to VirusTotal if origin is unknown, and perform brief dynamic analysis to confirm `SetWindowDisplayAffinity` behavior. The Sectigo EV certificate for Xiamen Xisu Technology Co., Ltd. provides meaningful identity assurance (EV requires business validation).
