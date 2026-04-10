# Malware Analysis Report: Dienstangebot_D_Sibrac_23-03-26.exe

**Analyst:** Claude / REMnux  
**Date:** 2026-04-10  
**Sample:** `Dienstangebot_D_Sibrac_23-03-26.exe`

---

## 1. File Metadata

| Field | Value |
|---|---|
| **Filename** | `Dienstangebot_D_Sibrac_23-03-26.exe` |
| **SHA256** | `80b3c302cb1ab35193d4e54b3df270f5900a96d8848baa23f9505821b7d6610c` |
| **MD5** | `2372ed6cd5975873aff677460128bc18` |
| **SHA1** | `1025973511567d5f6dc9ded79de5211554fd9025` |
| **Size** | 95,712 bytes |
| **Type** | PE32+ executable (GUI) x86-64 |
| **Architecture** | AMD64 |
| **Compiler** | MSVC 2022 (v17.0) |
| **Build timestamp** | 2026-03-27 03:49:14 UTC |
| **VersionInfo ProductName** | `Pulse Vector Service` |
| **VersionInfo FileDescription** | `Pulse Vector Service` |
| **Certificate Subject** | Robert Walters (Placentia, CA, US) |
| **Certificate Issuer** | Microsoft ID Verified CS AOC CA 01 |
| **Certificate Serial** | `330008b3d4c50528713453810100000008b3d4` |
| **Certificate Validity** | 2026-03-25 → 2026-03-28 **(3-day cert)** |
| **Imphash** | `4a6e8d420f7acd5d98f7bfb61b4ca87b` |
| **Sections (suspicious entropy)** | `.text` (6.75), `.data` (7.69 — encrypted payload), `.rsrc` (7.51) |
| **Overlay** | 15,840 bytes (PKCS7 authenticode signature) |

---

## 2. Classification

| | |
|---|---|
| **Family** | Unknown / novel — no YARA family matches |
| **Category** | 2-stage dropper → Reflective DLL stager → COM hijack / browser credential stealer |
| **Confidence** | **HIGH** — full attack chain recovered by static analysis + payload decryption |
| **Campaign overlap** | 3-day cert from **Microsoft ID Verified CS AOC CA 01** — same CA as DDinosaur.exe (`Ricardo Reis`, 2026-02-28–03-03) and MTSetup_v15.3.7191.msi (`Tryphena Lewis`, 2026-03-23–26). Different subject (Robert Walters), same abuse channel. |
| **Lure theme** | German-language "Dienstangebot" (Service Offer / NDA) — likely targeting German-speaking corporate victims |

---

## 3. Capabilities

**Stage-1 Loader (Dienstangebot_D_Sibrac_23-03-26.exe)**
- Displays a German "Dienstangebot" (service offer / NDA) document viewer using Rich Edit (msftedit.dll / RICHEDIT50W)
- Three checkbox acknowledgements before "Fortfahren" (Continue) button is enabled:
  - *"Ich habe die Grundsätze…"* — I have read the principles
  - *"Ich verpflichte mich, die Identität…"* — I commit to protecting identity
  - *"Ich nehme zur Kenntnis, dass die Verschwiegenheitspflicht…"* — I acknowledge the confidentiality obligation
- **Anti-sandbox**: `GetTickCount() > 299,999 ms` (≈5 min uptime) required before payload thread spawns; `Sleep(1)` every 32,768 iterations of brute-force also adds delay
- **`-s` / `/s` CLI flag**: skips GUI entirely, runs payload thread directly (persistence re-exec mode)
- **Payload decryption**: Loads RCDATA resource #101 (25,600 bytes) into `VirtualAlloc`, brute-forces a 32-bit key `k` via modified MurmurHash3 finalizer until `hash(k) == 0x50e36f88 AND hash(~k) == 0x4c4db6b2`; found key: **`0x0676b03e`**
- Decryption cipher (sub_140001000): single-pass XOR with evolving key: `plaintext[i] = ciphertext[i] ^ (k1>>(i&3)<<3)&0xFF ^ (k2>>(i&3)<<3)&0xFF`; `k2 += ciphertext[i]` (ciphertext-feedback)
- After decryption, `VirtualProtect(PAGE_EXECUTE_READ)` → calls DLL entry at `buffer + *(buffer+0x1c)` with args `(buffer, 1, ref_counter)`

**Stage-2 DLL (s164.dll — internal name, reflective)**  
SHA256: `450d5f65ddae975c84614f8b223093fba1dea89b5ba4fcc32b0cdeeb04e69c03`

- **Reflective Loader** (`ReflectiveEntryPoint64`): walks PEB InLoadOrderModuleList to find `LoadLibraryA` / `GetProcAddress` / `VirtualAlloc` by string matching; copies self to new allocation, processes base relocations, resolves IAT, calls own entrypoint with magic `0xaf8e206c`
- **Victim fingerprinting**: reads `HKLM\SOFTWARE\Microsoft\Cryptography\MachineGuid`, strips dashes → 16-char hex machine ID sent to C2
- **GetProcessSelector**: enumerates processes / windows to identify target browser or application (selector string sent to C2)
- **C2 beacon**: HTTP POST to `http://sealchecks.com/api.php` with MachineGuid + process selector; custom headers include `Client-Id`, `Client-Version`, `Trace-Id`, `Trace-Session`, `Trace-Unique`, `Trace-Selector`, `Request-Id`; User-Agent mimics Chrome/Safari
- **Response validation**: verifies checksum field, confirms response body is valid PE (MZ header, size ≥ 0x40)
- **Extension installer** (`%APPDATA%\omicron_index.db`): downloads Stage-3 PE from C2, writes via memory-mapped file with delayed MZ-header write (anti-AV write evasion), performs 10-step rename chain with 500 ms delays before final placement
- **COM Hijack persistence (dynamic GUID)**: generates random GUID via `CoCreateGuid`; registers `HKCU\Software\Classes\CLSID\{random}\InprocServer32` → DLL path; registers `HKCU\Software\Classes\Directory\Background\shellex\ContextMenuHandlers\{name}` → GUID
- **COM Hijack persistence (fixed CLSID)**: registers `HKCU\Software\Classes\CLSID\{BCDE0395-E52F-467C-8E3D-C4579291692E}\InprocServer32` with `ThreadingModel=Both`
- **COM activation trigger**: simulates desktop right-click by targeting `Progman → SHELLDLL_DefView → SysListView32` (or `WorkerW` variant), attaches thread input, sends `WM_CONTEXTMENU` — forces Windows to load the registered ContextMenuHandler DLL without user interaction
- **State persistence**: saves request counter + encrypted config to `HKCU\Software\Microsoft\Windows` (unnamed binary value); encrypted with an xorshift-based stream cipher seeded from system time × PID × TID × tick
- **Keylogging**: capa confirmed T1056.001
- **File management**: temp files in `%APPDATA%\{guid}.tmp`; backs up prior `omicron_index.db` before overwrite; deletes temp on failure
- **Version info collection**: reads FileVersionInfo from target processes via `GetFileVersionInfoW`

---

## 4. Attack Chain

```
Dienstangebot_D_Sibrac_23-03-26.exe
  [Stage 1 — Lure + Loader]
  │
  ├─ GUI: German NDA document viewer (RICHEDIT50W)
  │       3 checkboxes → "Fortfahren" button → 2nd page
  │
  ├─ Anti-sandbox: GetTickCount > 5 min
  │
  ├─ Brute-force key (k=0x0676b03e) → decrypt RCDATA #101 (25,600 bytes)
  │
  └─ In-memory execute s164.dll via reflective bootstrap
         │
         [Stage 2 — Reflective DLL / Stager]
         │
         ├─ PEB walk → resolve LoadLibraryA / GetProcAddress / VirtualAlloc
         ├─ Self-copy + relocate + IAT fix → call own DllMain with 0xaf8e206c
         │
         ├─ Read MachineGuid → machine fingerprint
         ├─ GetProcessSelector → target browser/process selector
         │
         ├─ HTTP POST → http://sealchecks.com/api.php
         │              [Client-Id: <machineGuid> | Trace-Selector: <selector>]
         │
         ├─ Response: Stage-3 PE payload
         │
         ├─ Write %APPDATA%\omicron_index.db (MZ-header-last, 10-rename chain)
         │
         ├─ COM persistence:
         │   ├─ HKCU\...\CLSID\{random}\InprocServer32 = omicron_index.db
         │   ├─ HKCU\...\ContextMenuHandlers\{name} = {random GUID}
         │   └─ HKCU\...\CLSID\{BCDE0395-...}\InprocServer32 = omicron_index.db
         │
         ├─ Save state → HKCU\Software\Microsoft\Windows (encrypted binary)
         │
         └─ Simulate desktop right-click → COM handler auto-loads
                │
                [Stage 3 — omicron_index.db (PE, not recovered)]
                └─ Keylogging + browser credential theft (inferred from s164 strings)
```

**`-s` persistence re-exec path**: Stage-1 skips GUI, directly spawns payload thread (used after persistence installation to re-run silently)

---

## 5. IOCs

### Network
| Type | Value |
|---|---|
| **URL (C2)** | `hxxp://sealchecks[.]com/api.php` |
| **Domain** | `sealchecks[.]com` |
| **User-Agent** | `Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/... Safari/537.36` |

### File System
| Path | Description |
|---|---|
| `%APPDATA%\omicron_index.db` | Stage-3 PE payload (masquerading as SQLite DB) |
| `%APPDATA%\{guid}.tmp` | Temporary download staging file |

### Registry
| Key | Description |
|---|---|
| `HKCU\Software\Classes\CLSID\{BCDE0395-E52F-467C-8E3D-C4579291692E}\InprocServer32` | Fixed COM hijack persistence |
| `HKCU\Software\Classes\CLSID\{random-guid}\InprocServer32` | Dynamic COM hijack (ThreadingModel=Apartment) |
| `HKCU\Software\Classes\Directory\Background\shellex\ContextMenuHandlers\{name}` | ContextMenuHandler registration |
| `HKCU\Software\Microsoft\Windows` (unnamed value) | Encrypted C2 state/config blob |
| `HKLM\SOFTWARE\Microsoft\Cryptography\MachineGuid` | Read for victim fingerprint |

### Certificate
| Field | Value |
|---|---|
| Subject | Robert Walters, Placentia, CA, US |
| Issuer | Microsoft ID Verified CS AOC CA 01 |
| Serial | `330008b3d4c50528713453810100000008b3d4` |
| Validity | 2026-03-25 → 2026-03-28 (3-day) |

### Hashes
| File | SHA256 |
|---|---|
| Stage-1 loader | `80b3c302cb1ab35193d4e54b3df270f5900a96d8848baa23f9505821b7d6610c` |
| Stage-2 DLL (decrypted) | `450d5f65ddae975c84614f8b223093fba1dea89b5ba4fcc32b0cdeeb04e69c03` |

### Cryptographic Material (Brute-Force Key)
| Item | Value |
|---|---|
| Brute-force key `k` | `0x0676b03e` |
| Derived XOR key1 | `0xcfd98a2e` |
| XOR key2 (fixed) | `0x9a77bad3` |

---

## 6. Emulation Results

**Speakeasy (Stage-1):** Not attempted (no network calls in loader; payload brute-force would time out emulation; key recovered analytically).

**Speakeasy (Stage-2 DLL):** Reflective loader bootstraps successfully, allocates `PAGE_EXECUTE_READWRITE` at 0x18000a000 (40 KB), loads ntdll.dll, calls `GetProcAddress(ntdll, "RtlAddFunctionTable")` → **unsupported API**; emulation terminates before C2 call. No network IOCs recovered via emulation; C2 URL recovered from static strings (`http://sealchecks.com/api.php`).

---

## 7. Sandbox Results

**Tria.ge:** Submission blocked at Cloudflare layer (HTTP 1010) from this analysis environment. No dynamic results available.

---

## 8. MITRE ATT&CK

| Technique | ID | Notes |
|---|---|---|
| Masquerading: Invalid Code Signing | T1036.001 | "Robert Walters" 3-day cert |
| Subvert Trust Controls: Code Signing | T1553.002 | Microsoft ID Verified CA abused |
| User Execution: Malicious File | T1204.002 | German NDA social engineering lure |
| Obfuscated Files or Information | T1027 | RCDATA XOR+hash cipher, reflective DLL |
| Virtualization / Sandbox Evasion | T1497 | GetTickCount > 5 min anti-sandbox |
| Input Capture: Keylogging | T1056.001 | capa confirmed in stage-2 |
| Ingress Tool Transfer | T1105 | Downloads stage-3 from sealchecks.com |
| Event Triggered Execution: COM Hijacking | T1546.015 | ContextMenuHandlers + CLSID registration |
| System Information Discovery | T1082 | MachineGuid collection |
| Query Registry | T1012 | MachineGuid read |
| File and Directory Discovery | T1083 | Process/window enumeration |
| Application Window Discovery | T1010 | Targets Progman/WorkerW/SysListView32 |
| Indicator Removal: File Deletion | T1070.004 | Temp file cleanup on failure |
| Command and Scripting Interpreter | T1059 | capa match |
| Shared Modules / Reflective Loading | T1129 | ReflectiveEntryPoint64 |

---

## 9. Analyst Notes

**Stage-3 not recovered.** The `omicron_index.db` PE payload is delivered at runtime from `sealchecks.com/api.php` and was not live at the time of analysis. Based on s164.dll strings (`omicron_index.db`, keylogging capa, browser window targeting via SHELLDLL_DefView, `GetFileVersionInfoW` on target processes), Stage-3 is likely a **browser credential stealer / keylogger** in the spirit of FlashTestInstaller / NotAWord from this campaign set. The `omicron_index.db` name echoes the "omicron" variant naming used by some MaaS stealer platforms.

**Fixed CLSID `{BCDE0395-E52F-467C-8E3D-C4579291692E}`** is not a well-known system CLSID — it appears to be a custom GUID hardcoded in the malware. It persists alongside the randomly-generated ContextMenuHandler GUID.

**Campaign linkage (3-day Microsoft ID Verified cert pattern):** This is the third sample with a 3-day cert from "Microsoft ID Verified CS AOC CA 01": DDinosaur (Ricardo Reis, 2026-02-28), MTSetup (Tryphena Lewis, 2026-03-23), and now Dienstangebot (Robert Walters, 2026-03-25). Rotation suggests a single threat actor purchasing short-validity Microsoft-verified code signing certificates in a sequence, with different victim personas per campaign. German-language lure is a departure from prior English/game lures.

**Recommended follow-up:**
1. Hunt `sealchecks.com` in DNS logs — domain may still be live
2. Search for `omicron_index.db` in endpoint telemetry
3. Hunt registry key `HKCU\Software\Classes\CLSID\{BCDE0395-E52F-467C-8E3D-C4579291692E}`
4. Hunt for `.exe` files with `Pulse Vector Service` VersionInfo
5. Submit to Tria.ge from a non-Cloudflare-blocked IP for dynamic coverage of Stage-3
6. Check certificate revocation — serial `330008b3d4c50528713453810100000008b3d4`

---

*Report generated: 2026-04-10*
