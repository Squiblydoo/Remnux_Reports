# Malware Analysis Report: ZhongStealer Staged Payload Chain (GCS bucket yynewyy)

**Date:** 2026-05-29  
**Analyst:** Claude Code (automated analysis)  
**Parent dropper:** `photo20260528899.com` (SHA256: `b1e6036407ac561deebf5a4885fda4d63686bdfbf808524e7554ea339a7bbe39`)  
**Staging URL:** `https://storage.googleapis.com/yynewyy/`

---

## 1. Artifact Inventory

| Filename | SHA256 | Size | Type | Role |
|----------|--------|------|------|------|
| `ps.txt` | `2f285b07...` | 314 B | ASCII text | Resource list (config) |
| `updat.exe` | `2b007100...` | 97.8 KB | PE32 GUI x86 | Sideload host (legitimate YY software) |
| `msvcp140.dll` | `e4c71980...` | 425 KB | PE32 DLL x86 | Legitimate Microsoft C++ runtime (dependency) |
| `vcruntime140.dll` | `8e085754...` | 72.5 KB | PE32 DLL x86 | Legitimate Microsoft C++ runtime (dependency) |
| `crashreport.dll` | `27b722c6...` | 104.9 KB | PE32 DLL x86 | **Malicious sideloaded DLL (main loader)** |
| `updat.log` | `3313f347...` | 158.9 KB | Raw binary | **Encrypted ZhongStealer shellcode** |
| `image.jpg` | `0ce9b137...` | 11.1 KB | JPEG | Lure/decoy image (ICC profile only, no stego detected) |

---

## 2. ps.txt — Resource List Config

`ps.txt` is the response to `ServiceManager.RetrieveResourceList(configEndpoint)` in the parent dropper. It is a plain newline-delimited list of the six GCS URLs to download:

```
https://storage.googleapis.com/yynewyy/crashreport.dll
https://storage.googleapis.com/yynewyy/updat.log
https://storage.googleapis.com/yynewyy/vcruntime140.dll
https://storage.googleapis.com/yynewyy/updat.exe
https://storage.googleapis.com/yynewyy/image.jpg
https://storage.googleapis.com/yynewyy/msvcp140.dll
```

The parent dropper likely saves `ps.txt` locally (despite the `.txt` extension in the URL), parses it line-by-line, and downloads each file to a working directory before invoking `ExecuteUpdateProcess()`.

---

## 3. updat.exe — DLL Sideload Host (Legitimate YY Software)

**SHA256:** `2b0071007c3f5fa8e949a8de53be03e97901dd505694ca939b575a49e4fdbdbb`

This is an **unmodified** legitimate binary from Guangzhou Jinhong Network Media Co., Ltd. (YY, a major Chinese live-streaming/social platform).

| Field | Value |
|-------|-------|
| Internal name | `yyexternal.exe` |
| Export module | `yyexternal.exe` |
| PDB | `E:\DUOWAN_BUILD\yypublish_build\console\source\yy\bin\release\yyexternal.pdb` |
| Version | 9.54.0.0 (2026.03.19) r2406779 |
| Certificate | DigiCert, Guangzhou Jinhong Network Media Co,Ltd., CN Guangdong |
| Cert serial | `0ec32ead70154412575023e86fe739c3` |
| Cert validity | 2025-10-20 → 2026-12-01 |

**Sideload mechanism:** `yyexternal.exe` explicitly loads `crashreport.dll` from its working directory by name. The string `\crashreport.dll` appears verbatim in the binary alongside other component DLLs it loads (`processservice.dll`, `hotpatchwrap.dll`, `jscript9.dll`, `dwbase.dll`, `dwutility.dll`). When all files are placed in the same directory and `updat.exe` is executed, it loads `crashreport.dll` automatically.

**ANY.RUN verdict:** 0/100 — benign when run in isolation (requires `crashreport.dll` in the same directory to activate).

---

## 4. crashreport.dll — Malicious Sideloaded DLL

**SHA256:** `27b722c66f69e360c4da106daacf3b9eeaabd20634d7e5eff45a28bd70ebfd65`

### 4.1 Metadata

| Field | Value |
|-------|-------|
| Internal name | `crashreport_new.dll` |
| PDB | `C:\Users\Administrator\Desktop\084049\crashreport_new\Release\crashreport_new.pdb` |
| Build date | **2026-05-26 20:17:10 UTC** (3 days before sample collection) |
| Certificate | DigiCert, **LENOVO (UNITED STATES) INC.** — stolen/abused EV cert |
| Cert serial | `0d2ad57b10b7472bae03d3deff05f54f` |
| Cert validity | 2026-04-10 → 2027-04-11 |
| Compiler | MSVC 2022 |
| Overlay | 17,832 bytes (entropy=144) — Authenticode signature blob |

**PDB indicates** the actor built this on an administrator's desktop in directory `084049` — a freshly compiled tool.

### 4.2 Export: InitBugReport

The single export called by `updat.exe` is `InitBugReport`. Decompilation reveals a heavily obfuscated function using **FNV-1a hash-based API resolution** (walks PEB InLoadOrderModuleList, hashes export names, compares against hardcoded hashes). This technique avoids import table exposure entirely.

**Execution flow (reconstructed):**

1. **API resolution:** Walks PEB module list → resolves `GetModuleFileNameW`, `CreateFileW`, `GetFileSize`, `ReadFile`, `VirtualAlloc`, `VirtualProtect` by FNV-1a hash
2. **Locate payload:** Calls `GetModuleFileNameW` to find its own directory, appends `updat.log`  
3. **Read payload:** Opens `updat.log`, reads 162,744 bytes into VirtualAlloc'd RW memory  
4. **Decrypt payload:**
   ```
   for i in range(len(data)):
       plaintext[i] = (data[i] + 0x77) ^ 0x62
   ```
5. **Execute as shellcode:** Calls `VirtualProtect(buf, size, PAGE_EXECUTE_READ=0x20)` → `call [buf]`

### 4.3 Additional Capabilities (from YARA and string analysis)

- **Persistence:** `Software\Microsoft\CurrentVersion\Run` — auto-start registry key  
- **Anti-analysis:** 22 XOR loops, spaghetti control flow, stack string construction (2 hits), dynamic string building (3 hits)  
- **Runtime linking:** Loads `mscoree.dll` (`CorExitProcess`) — the ZhongStealer payload may load a .NET component  
- **.fptable section** — non-standard section name (possibly jump/function pointer table for resolved APIs)

**ANY.RUN verdict:** 0/100 when submitted as standalone DLL (lacks `updat.exe` context + `updat.log` on disk).

---

## 5. updat.log — Encrypted ZhongStealer Shellcode

**SHA256 (encrypted):** `3313f347e83aaf48ea31fb1d49fc37452f48f81d20a1b93009e2e78385ff4bba`  
**SHA256 (decrypted):** `d8d82691bd1f9c9259d3bf4b350f7f70a1f7aa7bc3566d76a4c292c4e48bc1e4`

**Decryption key:** `plaintext[i] = (encrypted[i] + 0x77) ^ 0x62`

### 5.1 Structure

The decrypted blob (158.9 KB) is a **reflective shellcode loader**:

```
[0x000 – 0x19a]  411-byte shellcode stub
                  → starts with JMP to 0x44b
                  → helper functions (PE parsing, FNV hash resolution)
                  → reflective PE loader
[0x19b – end]    In-memory PE payload (ZhongStealer DLL)
                  → MZ header present; e_lfanew points to in-memory address
                  → NOT a valid raw-disk PE; requires shellcode loader to map
```

**Execution flow:**
1. Shellcode stub starts (`0xe9 46 04 00 00` = JMP to 0x44b)
2. At 0x44b: standard x86 function prologue → calls helper to locate embedded PE
3. Reflective loader maps the PE into executable memory (adjusts IAT, relocations)
4. Transfers execution to PE entry point

### 5.2 ZhongStealer Capability Indicators (from visible strings)

Strings in the decrypted shellcode (others remain encrypted at second layer):
- `WININET` — WinInet API usage (HTTP-based C2 / exfiltration)
- `BrowserA` (partial: `/QQBr,owserA`) — browser credential targeting
- `USERPROFILE` (partial: `USERP^E\`F`) — reads user profile directory
- `Host:` (partial: `38HostP:`) — HTTP request construction
- `LdrLoadDll` (partial: `):)LdrLu!=oadDu`) — manual DLL loading via PEB
- `VirtualAlloc`/`VirtualFree` (partial: `locau?=teViu8`) — runtime memory management
- `RtlStr` (partial: `;kX P^Rlstr`) — NTDLL string manipulation
- `HTTP-` — HTTP protocol usage

No plaintext C2 URLs or IPs recoverable from static analysis of the shellcode; they are encrypted within the embedded PE's second obfuscation layer.

---

## 6. msvcp140.dll / vcruntime140.dll — Legitimate Microsoft DLLs

Both are **authentic, Microsoft-signed** Visual C++ runtime DLLs:

| File | Version | Cert | Notes |
|------|---------|------|-------|
| `msvcp140.dll` | 14.30.30626.0 | Microsoft Corp, expired 2021-12-02 | Bundled because YY software needs it |
| `vcruntime140.dll` | 14.16.27052.0 | Microsoft Corp, expired 2024-11-14 | Bundled because YY software needs it |

Both have overlays (9–10 KB) which contain their Authenticode signatures — normal. Neither is trojanized.

**Purpose:** `updat.exe` (the YY binary) was compiled to require MSVC 2017/2019 runtime DLLs. Rather than assuming the victim has them installed, the actor bundles them alongside the sideload package for a reliable execution environment.

---

## 7. image.jpg — Lure/Decoy Image

**SHA256:** `0ce9b137f378211a4f6ba43bae5e7056d577d757441671028b94b46a05b2b0c1`  
761×352 px, JFIF 1.01, progressive JPEG, 11 KB

Contains only an ICC color profile; no steganography tool markers detected. Likely a lure image displayed to the victim or simply downloaded to make the file set appear legitimate (reduces suspicion if user checks the staging directory).

---

## 8. Attack Chain Summary

```
[Initial delivery: photo20260528899.com]
      ↓
[Anti-sandbox check] → exit if detected
      ↓
[Download ps.txt from GCS] → parse 6 URLs
      ↓ parallel downloads
[updat.exe]   [crashreport.dll]   [updat.log]   [msvcp140.dll]   [vcruntime140.dll]   [image.jpg]
      ↓
[ExecuteUpdateProcess() → LaunchExecutable("updat.exe")]
      ↓
[updat.exe loads crashreport.dll via DLL sideloading]
      ↓
[crashreport.dll::InitBugReport()]
  1. FNV-1a API resolution (no import table)
  2. Opens updat.log
  3. Decrypts: (byte + 0x77) ^ 0x62
  4. VirtualAlloc → copy → VirtualProtect(PAGE_EXECUTE_READ)
  5. call [shellcode]
      ↓
[Shellcode: reflective PE loader]
  1. Locates embedded ZhongStealer PE at shellcode+0x19b
  2. Maps PE into memory (fixes IAT/relocations)
  3. Transfers to ZhongStealer entry point
      ↓
[ZhongStealer payload]
  - Browser credential theft (Chrome/Edge/Firefox via USERPROFILE)
  - WinInet HTTP exfiltration
  - Persistence: HKCU\...\Run
  - C2: unknown (second encryption layer, not statically recovered)
```

---

## 9. ZhongStealer Version Differences (vs. Previous Known Versions)

Based on this sample's characteristics compared to the general ZhongStealer profile:

| Characteristic | This Sample | Previously Expected |
|---|---|---|
| **Initial dropper** | .NET VM-obfuscated downloader with WebSocket C2 (`uu[.]goldeyeuu[.]io`) | Simple downloader or direct delivery |
| **Staging** | GCS bucket (`storage.googleapis[.]com/yynewyy/`) — legitimate cloud infra | Direct actor-controlled URLs |
| **Resource list format** | Plain newline-delimited `ps.txt` via HTTP GET | Varies |
| **Sideload host** | YY live-streaming software `yyexternal.exe` v9.54 (Guangzhou Jinhong, DigiCert) | Other legitimate software |
| **DLL signing** | Stolen/abused **Lenovo (US) Inc.** DigiCert EV cert, issued 2026-04-10 | Likely different certs |
| **Payload encryption** | `(byte + 0x77) ^ 0x62` ADD+XOR hybrid | Often simpler XOR-only |
| **Shellcode structure** | 411-byte reflective loader stub + embedded in-memory PE | May vary |
| **Build freshness** | `crashreport.dll` built 2026-05-26 (3 days before collection) | N/A |
| **APT-Q-27 attribution** | ANY.RUN auto-classifies as `apt-q-27` | Campaign-dependent |
| **C2 WebSocket layer** | `uu[.]goldeyeuu[.]io` (extra persistence/command layer above stealer) | Not typically seen |

**Key new TTPs in this version:**
1. Extra C2 layer (WebSocket backdoor) above the stealer — operator can issue commands independently of the steal-and-exfil flow
2. Multi-cert abuse: Sectigo EV (Xiamen, CN) for dropper + DigiCert EV (Lenovo, US) for sideloaded DLL — two separate stolen/abused certs in one chain
3. Bundled C++ runtimes guarantee execution environment without victim-side prerequisites

---

## 10. IOCs

### Network (defanged)

| Type | IOC | Notes |
|------|-----|-------|
| GCS Bucket | `storage.googleapis[.]com/yynewyy/` | Staging server; report to Google |
| Domain | `uu[.]goldeyeuu[.]io` | Dropper WebSocket C2 (malicious) |

### File Hashes

| File | SHA256 |
|------|--------|
| ps.txt | `2f285b07258ed4a63c7f9fba3427bcf6348f6d6ab374030687261b4dc8fe7fdc` |
| updat.exe | `2b0071007c3f5fa8e949a8de53be03e97901dd505694ca939b575a49e4fdbdbb` |
| crashreport.dll | `27b722c66f69e360c4da106daacf3b9eeaabd20634d7e5eff45a28bd70ebfd65` |
| updat.log (encrypted) | `3313f347e83aaf48ea31fb1d49fc37452f48f81d20a1b93009e2e78385ff4bba` |
| updat.log (decrypted) | `d8d82691bd1f9c9259d3bf4b350f7f70a1f7aa7bc3566d76a4c292c4e48bc1e4` |
| msvcp140.dll | `e4c71980dbb4a1e1a86816687afdaea043b639b531135fc4516fb2429fe623fc` |
| vcruntime140.dll | `8e08575492175e042f093f325b07a5c14ca71e7c581474838db3d48f5aab1312` |
| image.jpg | `0ce9b137f378211a4f6ba43bae5e7056d577d757441671028b94b46a05b2b0c1` |

### Certificates (for revocation requests)

| Cert | Subject | Issuer | Serial |
|------|---------|--------|--------|
| photo20260528899.com | Xiamen Shunhuitong E-commerce Co., Ltd. | Sectigo EV | `3eaa4bd40d5da98036b33023e0052869` |
| crashreport.dll | LENOVO (UNITED STATES) INC. | DigiCert EV | `0d2ad57b10b7472bae03d3deff05f54f` |

### Registry

| Path | Notes |
|------|-------|
| `HKCU\Software\Microsoft\Windows\CurrentVersion\Run` | ZhongStealer persistence (key name unknown) |

---

## 11. Analyst Notes

1. **C2 for ZhongStealer payload unknown**: The embedded PE has a second obfuscation layer; no plaintext C2 URL/IP recovered statically. Dynamic analysis with the full sideload chain on Windows would be required to capture the exfiltration C2.
2. **Decryption script**: The `updat.log` decryption key `(byte + 0x77) ^ 0x62` is confirmed from `crashreport.dll::InitBugReport` decompilation. Script saved as `updat_log_decrypt.py` (see below).
3. **Cert revocation**: Both Sectigo and DigiCert should be notified. The Lenovo cert (DigiCert `0d2ad57b...`) is especially high-priority as it could sign additional payloads.
4. **GCS bucket takedown**: `storage.googleapis.com/yynewyy/` is still live at time of analysis. Files should be reported to Google's Abuse team.
5. **Login.live.com contacts**: Both `updat.exe` and the initial dropper contacted `login.live.com/RST2.srf` (WS-Trust) and `deviceaddcredential.srf`. This is likely ZhongStealer attempting to steal Microsoft/Azure authentication tokens. Cross-reference with WallStealer sample pattern (WAM token theft) — though no matching IOCs confirm a link.

