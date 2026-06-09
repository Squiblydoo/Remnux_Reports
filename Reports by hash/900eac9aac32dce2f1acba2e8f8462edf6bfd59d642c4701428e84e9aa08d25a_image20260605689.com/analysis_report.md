# Malware Analysis Report: image20260605689.com

**Date:** 2026-06-09  
**Analyst:** REMnux / Claude Code  
**Campaign:** ZhongStealer APT-Q-27 — `lecoo` Wave (June 2026)

---

## 1. File Metadata

| Field | Value |
|---|---|
| **Filename** | image20260605689.com |
| **SHA256** | `900eac9aac32dce2f1acba2e8f8462edf6bfd59d642c4701428e84e9aa08d25a` |
| **SHA1** | `d9c91dc84f0d92c0f28b416cc77e031d2dbdb636` |
| **MD5** | `3f952fc64ed5d5c1dd00dd2d0f337112` |
| **Size** | 67,632 bytes |
| **Type** | PE32 executable (GUI) Intel 80386 Mono/.NET assembly |
| **Architecture** | .NET Framework 4.8, x86 |
| **Build timestamp** | 2026-06-04 14:25:25 UTC (peframe) |
| **Imphash** | `f34d5f2d4577ed6d9ceec516c1f5a744` |
| **Assembly GUID** | `ffe518e3-c674-4cc0-bad3-1245c10d7a91` |

### Signing Certificate
| Field | Value |
|---|---|
| **Issuer** | Certum Code Signing 2021 CA (Asseco Data Systems S.A., PL) |
| **Subject** | Shi Hu |
| **Organization** | 四川/达州, CN |
| **Serial** | `33db6c7028fcf6afb84646806433d226` |
| **Validity** | 2025-11-17 → 2026-11-17 |

### VersionInfo (spoofed)
| Field | Value |
|---|---|
| **CompanyName** | Amazon.com |
| **FileDescription** | logs |
| **InternalName** | logs.exe |
| **OriginalFilename** | logs.exe |
| **LegalCopyright** | Copyright © Amazon.com 2027 |

---

## 2. Classification

**Family:** ZhongStealer / APT-Q-27 Staged Downloader  
**Confidence:** Confirmed (ANY.RUN 100/100; cert chain linkage to yynewyy campaign; behavioral match)  
**Campaign wave:** `lecoo` (June 2026), successor to `yynewyy` (May 2026)

### Attribution Evidence

This sample is directly linked to the previously analyzed ZhongStealer `yynewyy` campaign by two hard indicators:

1. **Identical certificate serial on payload DLL** — `DataPlugin.dll` (downloaded from GCS `lecoo` bucket) is signed with DigiCert cert serial `0d2ad57b10b7472bae03d3deff05f54f` (LENOVO UNITED STATES INC.) — the exact same stolen EV certificate used to sign `crashreport.dll` in the `yynewyy` campaign (SHA256: `27b722c6...`).

2. **Identical decoy image hash** — `image.jpg` from the `lecoo` bucket has SHA256 `0ce9b137f378211a4f6ba43bae5e7056d577d757441671028b94b46a05b2b0c1`, which is byte-for-byte identical to `image.jpg` from the `yynewyy` bucket used in May 2026.

Additionally, the dropper itself shares an **identical build artifact** with `photo20260528899.com` (SHA256: `b1e60364...`): same `InternalName=logs.exe`, same Amazon.com VersionInfo spoofing, same .NET custom VM obfuscator (GetManifestResourceStream + ConcurrentDictionary string cache + stack-trace anti-debug), and identical sandbox evasion class structure.

---

## 3. Capabilities

### Dropper (image20260605689.com)

- **Anti-sandbox / VM evasion** (10-category, double-pass with 30-second sleep)
  - Process name blocklist check (19 entries, e.g. procmon, x96dbg, wireshark)
  - File/directory existence check (7 sandbox-specific paths)
  - WMI MAC address prefix check (5 VM vendor prefixes)
  - WMI VM/hypervisor detection (Win32_ComputerSystem manufacturer)
  - WMI network adapter driver check
  - Drive size check: total disk < 40 GB → sandbox
  - Environment variable check (3 VM/AV environment variables)
  - Username blocklist (single entry check)
  - Device file existence check via CreateFile (e.g. `\\.\VBoxMiniRdr`)
  - Registry key check (VM-specific HKLM key)
  - WMI RAM check: < 4 GB → sandbox

- **String obfuscation** — all strings encrypted in embedded .NET manifest resource, decrypted at runtime via XOR+PRNG with TEA-derived key (DELTA=0x9E3779B9); ConcurrentDictionary cache; stack-trace anti-reflection guards

- **Double-Base64 C2 URL decoding** — config endpoint stored as Base64(Base64(URL)) in the encrypted string table

- **Parallel file download** — SemaphoreSlim(10) throttle, async HTTP GET, saves files hidden+NotContentIndexed to `%LOCALAPPDATA%\<token1>\<token2>\<token3>\{datetime}_{GUID[:8]}\`

- **SSL validation bypass** — ServerCertificateCustomValidationCallback returns true unconditionally

- **Resource list parsing** — fetches `le.txt` from C2, parses URLs separated by newline/comma/semicolon/pipe; supports Base64-encoded URLs in list; filters valid http/https URIs

- **Extension-based payload execution** (ordered):
  1. `.exe`/`.com`/other executables → ShellExecute, WindowStyle=Normal
  2. `.msi` → msiexec.exe /i (silent install)
  3. `.log`/`.dat` → rundll32.exe (shellcode/reflective loader)
  4. `.dll`/`.bat` → async Process.Start
  5. `.exe` payloads → Process.Start, wait 3 seconds, check HasExited

- **HTTP client hardening** — TLS 1.2/1.3 only, MaxConnections=20, Timeout=120s, MaxResponseContentBufferSize=100MB, random User-Agent from 3 choices, random jitter 1–3s before requests, no-cache headers

- **Custom request headers** — Referrer header and a custom header name+value (both obfuscated) added to all requests

### Payload DLL (DataPlugin.dll)

- **DLL sideloading** — loaded by legitimate `LecooPlatform.exe` (Lenovo Lecoo Manager v5.1.150.11131)
- **Export:** `LM_Init_With` (mirrors Lenovo plugin API naming)
- **FNV-1a import-by-hash** — no import table; all API names resolved at runtime via hash
- **File decryption** — reads `lecco.dat`, decrypts with rolling key derived from `LecooUpdat` / `QzioQzio` (exact cipher not recovered statically; different from yynewyy's `(byte+0x77)^0x62`)
- **Memory injection** — NtAllocateVirtualMemory + NtProtectVirtualMemory (RWX), executes decrypted shellcode
- **Control flow flattening** — OLLVM-style state machine obfuscation (iVar155 dispatcher pattern)

---

## 4. Attack Chain

```
1. Victim receives / downloads image20260605689.com
   └─ Certum cert (Shi Hu, 四川 CN) — new cert for this wave

2. Dropper executes (.com extension = direct exec)
   └─ 10-category sandbox evasion (double-pass + 30s sleep)
   └─ Decodes double-Base64 config endpoint → fetches le.txt from GCS

3. GCS staging bucket: storage.googleapis[.]com/lecoo/
   ├─ le.txt           — resource list (4 URLs)
   ├─ LecooPlatform.exe — Lenovo Lecoo Manager (legitimate, signed Lenovo Beijing cert)
   ├─ DataPlugin.dll    — trojanized plugin DLL (signed stolen LENOVO US cert)
   ├─ lecco.dat         — encrypted shellcode/reflective loader (updat.log equivalent)
   └─ image.jpg         — decoy (IDENTICAL to yynewyy image.jpg)

4. Dropper downloads all 4 files → hidden directory in %LOCALAPPDATA%
   └─ Executes LecooPlatform.exe (legitimate host process)

5. LecooPlatform.exe loads DataPlugin.dll via DLL sideloading
   └─ LM_Init_With() called

6. DataPlugin.dll
   └─ Opens lecco.dat → decrypts → NtAllocateVirtualMemory(RWX) → executes shellcode

7. Shellcode / Stage 3 (not fully recovered)
   └─ Reflective PE loader → ZhongStealer core DLL in-memory
   └─ WebSocket C2 → api[.]keensie[.]com (new, replaces uu[.]goldeyeuu[.]io)
   └─ login[.]live[.]com/RST2[.]srf contact (WAM token theft)
```

---

## 5. IOCs

### Network — Dropper C2

| Type | Indicator | Notes |
|---|---|---|
| Domain | `api[.]keensie[.]com` | NEW WebSocket C2; ANY.RUN rep=2 (malicious) |
| URL | `https://storage[.]googleapis[.]com/lecoo/le[.]txt` | Resource list |
| URL | `https://storage[.]googleapis[.]com/lecoo/DataPlugin[.]dll` | Payload DLL |
| URL | `https://storage[.]googleapis[.]com/lecoo/lecco[.]dat` | Encrypted shellcode |
| URL | `https://storage[.]googleapis[.]com/lecoo/LecooPlatform[.]exe` | Sideload host |
| URL | `https://storage[.]googleapis[.]com/lecoo/image[.]jpg` | Decoy image |
| Domain | `login[.]live[.]com` | WAM token theft (RST2.srf) |

### Payload Hashes

| File | SHA256 |
|---|---|
| image20260605689.com (dropper) | `900eac9aac32dce2f1acba2e8f8462edf6bfd59d642c4701428e84e9aa08d25a` |
| DataPlugin.dll | `96ccd05743137bdb83b4ecd22d5ba2bf5af3a5fc43ad6fa7b7b6cf62bb39aa81` |
| lecco.dat (encrypted) | `208de811ad293565cece4589be2e2794353f318b8296c3fddf5e69682fd00910` |
| LecooPlatform.exe | `9ff7afa7c1121266b79100221c7fae99dfd9889f5e533abe398e7479853c55d5` |
| image.jpg (decoy) | `0ce9b137f378211a4f6ba43bae5e7056d577d757441671028b94b46a05b2b0c1` |

### Certificate Indicators

| File | Subject | Serial | Notes |
|---|---|---|---|
| image20260605689.com | Shi Hu (四川 CN) | `33db6c7028fcf6afb84646806433d226` | Certum; NEW cert this wave |
| DataPlugin.dll | LENOVO (UNITED STATES) INC. | `0d2ad57b10b7472bae03d3deff05f54f` | **SAME stolen cert as crashreport.dll (yynewyy)** |
| LecooPlatform.exe | Lenovo (Beijing) Limited | `04d69f52134bdcc35f48efcfead38e48` | Legitimate Lenovo cert; sideload host |

### Filesystem

| Path | Notes |
|---|---|
| `%LOCALAPPDATA%\<token1>\<token2>\<token3>\{datetime}_{GUID8}\` | Hidden drop directory; tokens are encrypted strings |
| `lecco.dat` | Encrypted payload (same role as `updat.log` in yynewyy) |
| `LecooUpdat` | Internal file reference string in DataPlugin.dll |

### Internal Strings (DataPlugin.dll, floss-decoded)

| String | Notes |
|---|---|
| `LecooUpdat` | File reference (dat file name or path component) |
| `QzioQzio` | Likely decryption key or marker used in lecco.dat decryption |
| `NtProtectVirtualMemory` | NT API resolved by hash at runtime |
| `NtAllocateVirtualMemory` | NT API resolved by hash at runtime |

---

## 6. Emulation Results

**Speakeasy (generic runner, DataPlugin.dll):** 0 IOCs recovered. The DLL requires `LecooPlatform.exe` as the host loader and `lecco.dat` to be present at the expected path; the isolated speakeasy environment did not satisfy these prerequisites.

**lecco.dat decryption:** Static decryption attempted with variants of the known ZhongStealer key `(byte+0x77)^0x62` and floss-recovered candidates (`QzioQzio` XOR, `LecooUpdat` rolling XOR, per-byte add-then-XOR combinations). None produced a valid PE/shellcode header. The cipher appears to use a more complex key derivation than previous waves; dynamic execution of the full chain (LecooPlatform.exe → DataPlugin.dll → lecco.dat) would be required to recover the decrypted payload.

---

## 7. Sandbox Results

| Field | Value |
|---|---|
| **ANY.RUN score** | **100/100 — Malicious activity** |
| **Family tags** | `apt-q-27`, `loader`, `backdoor`, `websocket`, `evasion`, `auto-reg` |
| **Public URL** | https://app.any.run/tasks/6b0438ed-8fdb-4bce-9aea-eb29aacda7d5 |
| **C2 resolved** | `api.keensie.com` (rep=2 malicious) |
| **GCS bucket accessed** | `storage.googleapis.com/lecoo/` (all 4 payload files downloaded) |
| **Microsoft auth contacted** | `login.live.com/RST2.srf`, `login.live.com/ppsecure/deviceaddcredential.srf` |

---

## 8. Campaign Infrastructure Comparison (yynewyy → lecoo)

| Component | yynewyy wave (May 2026) | lecoo wave (June 2026) |
|---|---|---|
| GCS bucket | `storage.googleapis.com/yynewyy/` | `storage.googleapis.com/lecoo/` |
| Resource list | `ps.txt` | `le.txt` |
| Sideload host | `updat.exe` (YY yyexternal.exe v9.54) | `LecooPlatform.exe` (Lenovo Lecoo Manager) |
| Payload DLL | `crashreport.dll` (export: `InitBugReport`) | `DataPlugin.dll` (export: `LM_Init_With`) |
| Encrypted payload | `updat.log` (162,744 bytes) | `lecco.dat` (340,992 bytes) |
| Decrypt key | `(byte+0x77)^0x62` | Not recovered (key: `QzioQzio`?) |
| WebSocket C2 | `uu[.]goldeyeuu[.]io:5188` | `api[.]keensie[.]com` |
| Dropper cert | Sectigo EV / Biao Zhao | Certum / Shi Hu |
| Payload DLL cert | **DigiCert LENOVO US `0d2ad57b...`** | **DigiCert LENOVO US `0d2ad57b...`** (SAME) |
| Decoy image SHA256 | `0ce9b137...` | `0ce9b137...` (SAME) |

---

## 9. Analyst Notes

1. **Cert reuse confirms shared infrastructure.** The stolen Lenovo (US) DigiCert EV cert (`0d2ad57b...`) was previously observed on `crashreport.dll` (built 2026-05-26). Its reuse on `DataPlugin.dll` (new June 2026 wave) confirms the same threat actor controls this certificate and is rotating campaign names/buckets while keeping their signing chain.

2. **Dropper cert is fresh.** The Certum cert issued to "Shi Hu" in Sichuan is new to this wave. The actor is acquiring new code-signing certificates per campaign wave to avoid blocklist.

3. **lecco.dat is significantly larger** than `updat.log` (341 KB vs 163 KB), suggesting additional capabilities or a larger embedded PE in the new payload.

4. **`api.keensie.com` C2 is new.** No WHOIS data resolved during analysis (domain likely registered recently or behind registrar privacy). The `websocket` tag from ANY.RUN confirms the same WebSocket backdoor communication pattern as the May 2026 wave.

5. **Recommended follow-up:**
   - Execute the full `LecooPlatform.exe → DataPlugin.dll → lecco.dat` chain in a controlled sandbox to recover the decrypted shellcode and Stage 3 PE
   - Pivot on cert serial `0d2ad57b10b7472bae03d3deff05f54f` (LENOVO US) in threat intelligence platforms for additional campaign samples
   - Monitor / sinkhole `api[.]keensie[.]com`
   - Submit `DataPlugin.dll` and `lecco.dat` to VirusTotal for broader detection coverage
   - Hunt for `LecooUpdat` string in enterprise EDR telemetry as a detection artifact
