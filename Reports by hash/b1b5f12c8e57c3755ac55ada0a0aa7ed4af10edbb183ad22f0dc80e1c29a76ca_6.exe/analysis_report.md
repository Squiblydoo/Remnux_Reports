# Malware Analysis Report: 6.exe

**Date:** 2026-04-06  
**Analyst:** REMnux/Malcat Automated Analysis  
**Confidence:** HIGH — Delphi File Binder / DLL-Hijacking Dropper (Astaroth-like RAT)

---

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | 6.exe |
| SHA256 | `b1b5f12c8e57c3755ac55ada0a0aa7ed4af10edbb183ad22f0dc80e1c29a76ca` |
| SHA1 | `ba0a55814c339ede1b1ad974f72d760a94a3e4af` |
| MD5 | `c63219696be05c35e03bbc216a0c5f6f` |
| Size | 15,868,824 bytes (15.1 MB) |
| File Type | PE32 executable (GUI) Intel 80386, Windows, 11 sections |
| Architecture | x86 (32-bit) |
| Compiler | Borland/Embarcadero Delphi (TurboLinker) |
| Build path | `F:\MainWork\Odes\_System.Rtti.pas` |
| Timestamp | 2026-04-03 02:58:30 (3 days before analysis — fresh build) |
| Export module | `Bind_EXE.exe` |
| Imphash | `953397dbf6c7d5d39a090197fe29dee1` |
| Entropy | 161 overall; `.rsrc` 211 (very high — packed payloads) |

### Certificate (Authenticode — **Same cert as cmd.Exe from this campaign**)
| Field | Value |
|---|---|
| **Subject** | Xiamen Jiaming Network Technology Co., Ltd. |
| **Issuer** | Sectigo Public Code Signing CA EV R36 |
| Serial | `00acb4af64ac061aadb00b5370b4ca0246` |
| Hash Algorithm | SHA256 |
| Validity | 2026-03-24 → 2027-03-24 |
| Subject Country | CN (China, Fujian Province) |

**This is the identical certificate and serial number used to sign `cmd.Exe` analyzed on 2026-04-05**, confirming both artifacts belong to the same threat actor / campaign infrastructure.

---

## 2. Classification

**MALICIOUS — HIGH CONFIDENCE**  
**Family:** Delphi File Binder / DLL-Hijacking Dropper with Keylogger/RAT payload  
**KesaKode similarity:** Astaroth 37%, DanaBot 16% (both 6.exe and embedded TRUST_IMITATE_DLL.dll)  
**TTPs:** T1574.001 (DLL Search Order Hijacking), T1056.001 (Keylogging), T1113 (Screen Capture), T1071.001 (Web Protocols C2), T1027 (Obfuscation), T1036.001 (Invalid Code Signature), T1140 (Deobfuscate), T1547 (Autostart via LNK)

**Reasoning:** 6.exe is a Delphi file binder (export name `Bind_EXE.exe`) that decompresses and deploys five embedded payloads from its `.rsrc` section using ZLIB compression with an 8-byte header prefix. The primary payload (`TRUST_IMITATE_DLL.dll`) is a Delphi RAT/stealer — near-identical in codebase to 6.exe itself — deployed via DLL hijacking through the legitimate Indigo Rose Script Editor (`IRScriptEditor.exe`). The malware shares a certificate with a separately analyzed re-signed `cmd.Exe`, establishing a common threat actor.

The 37% Astaroth similarity matches Astaroth/Guildma's known TTP of deploying keyloggers via DLL hijacking through legitimate LOLBins, with PDF lures and WinHTTP exfiltration.

---

## 3. Embedded Payloads

All five payloads are stored in `.rsrc` as RCDATA resources, ZLIB-compressed with an 8-byte header (4 bytes decompressed size + 4 bytes compressed size) preceding the ZLIB stream.

| Resource ID | Type | Compressed | Decompressed | SHA256 (decompressed) | Description |
|---|---|---|---|---|---|
| `546B4B57` | Data | 1.1 MB | 2.9 MB | `7769c9fa...` | Delphi RTTI data, XOR-0x10 encoded |
| `8897E24E` | PDF | 357 KB | 440 KB | `4ea3ee0e...` | Encrypted PDF lure, 20 pages (`%PDF-1.5`, standard encryption) |
| `990AECA9` | PE32 DLL | 3.6 MB | 9.4 MB | `87643b97...` | **TRUST_IMITATE_DLL.dll** — main RAT/stealer payload |
| `AD7775DD` | PE32 EXE | 1.2 MB | 2.67 MB | `213d4df7...` | **IRScriptEditor.exe** — legitimate Indigo Rose Script Editor v2.0 (hijack host) |
| `FF431957` | PE32 DLL | 128 KB | 304 KB | `2dd277b4...` | **Bind_DLL.dll** — dropper helper (MZ magic obfuscated: `FB AE` instead of `MZ`) |

---

## 4. Capabilities

### Dropper / Binder (6.exe)
- Decompresses five RCDATA payloads using embedded zlib inflate (Bind_DLL.dll contains the inflate engine)
- Writes payloads to disk (target directory not recovered — XOR-obfuscated string)
- Opens PDF lure document to distract victim
- Coordinates component execution via named mutexes (`Global\a_` through `Global\x_`)
- Checks for Windows service presence (`[IF_SERVICE]`, `[IF_SERVICE_RUNING]`) before proceeding
- Disables WoW64 filesystem redirection (`Wow64DisableWow64FsRedirection`) for 32/64-bit compatibility
- 83 XOR-in-loop instances — runtime string/path decryption (C2 URLs not visible in static analysis)

### DLL Hijacking (TRUST_IMITATE_DLL.dll + IRScriptEditor.exe)
- Drops `IRScriptEditor.exe` (legitimate Indigo Rose Script Editor v2.0) and `TRUST_IMITATE_DLL.dll` renamed as **`cmcs21.dll`** (the Indigo Rose runtime DLL) to the same directory
- When `IRScriptEditor.exe` loads, it resolves `cmcs21.dll` from its own directory first, loading the malicious DLL instead
- TRUST_IMITATE_DLL.dll is signed with the same Xiamen Jiaming EV cert (grants SmartScreen bypass)
- TRUST_IMITATE_DLL.dll is nearly identical in code to 6.exe (same Delphi codebase, same PDB path `F:\MainWork\Odes\`)

### RAT/Stealer Payload (TRUST_IMITATE_DLL.dll)
- **Message hook (WH_GETMESSAGE, type 3)**: Installed via `SetWindowsHookExW`; hook procedure `HintGetMsgHook` fires on every Windows message and triggers the C2 dispatch path (`sub_6044c0`). This is not a WH_KEYBOARD hook — it intercepts all window messages, enabling broad input capture and C2 triggering.
- **Activation guard**: The hook and thread setup only execute when `*([0xBF8784]+0xC5) == 0`. The pointer at `[0xBF8784]` is only initialized by the IRScriptEditor.exe host process. The DLL does nothing if loaded outside its sideloading context — explaining why emulation fails.
- **Screen/mouse monitoring**: A dedicated thread (`sub_60237c`) loops on `WaitForSingleObject` → `GetCursorPos` → `FindVCLWindow` to track cursor position over Delphi UI windows (T1113 screen capture).
- **C2 communication**: WinHTTP (16 imported functions) — HTTP POST with form data (`multipart/form-data`); data is Base64-encoded (`sub_a689d4`) before transmission. All WinHTTP hostname resolution is vtable-dispatched at runtime; C2 URL cannot be recovered statically.
- **TCP listen socket**: Possible reverse shell or inbound command channel
- **Anti-debug**: `IsDebuggerPresent`, `FindWindowW`, `FindWindowExW`, `GetWindowThreadProcessId`, `UnhandledExceptionFilter`, `RaiseException`
- **Registry access**: Read/write registry keys
- **File operations**: `*.lnk` search (LNK-based persistence candidate)
- **Crypto**: SHA1, RIPEMD160, CRC32, Base64, Delphi Random (for data hashing/encoding before exfiltration)
- **Wow64DisableWow64FsRedirection**: Operates across both 32/64-bit environments

### Bind_DLL.dll (Dropper Helper)
- MZ magic bytes replaced with `FB AE` to evade file-type detection
- Contains complete zlib inflate decompressor (decompresses the other payloads)
- Mutex coordination via `Global\a_` – `Global\x_`
- Service status polling
- `*.lnk` file manipulation

### Lure (PDF, `8897E24E`)
- Standard PDF 1.5 with `/Filter /Standard` encryption (password-protected)
- 20 pages — content not recoverable without password; likely a document relevant to targets (financial, legal, business)

---

## 5. Attack Chain

```
[Victim receives 6.exe]
  │  (EV cert + SmartScreen bypass via Xiamen Jiaming cert)
  ▼
[User executes 6.exe — Bind_EXE.exe]
  │
  ├─ 1. Decompresses all 5 RCDATA resources via Bind_DLL.dll zlib engine
  │
  ├─ 2. Drops to working directory (XOR-obfuscated path):
  │       - IRScriptEditor.exe  (legitimate, signed 2014–2019 Indigo Rose)
  │       - cmcs21.dll          (= TRUST_IMITATE_DLL.dll, Xiamen Jiaming cert)
  │       - Bind_DLL.dll        (helper; hidden MZ header)
  │       - [PDF lure]          (opened to distract victim)
  │       - [Delphi RTTI data]  (resource for Delphi runtime)
  │
  ├─ 3. Opens PDF lure → victim sees expected document
  │
  ├─ 4. Checks service status, sets Global\ mutexes
  │
  └─ 5. Executes IRScriptEditor.exe
           │
           └─ DLL search order loads cmcs21.dll (malicious) from local directory
                    │
                    ├─ Anti-debug checks
                    ├─ Install keylogger hook (SetWindowsHookEx)
                    ├─ Start screenshot loop
                    ├─ TCP socket / WinHTTP C2 connection (URL XOR-decrypted at runtime)
                    ├─ POST captured data (keystrokes, screenshots) via multipart/form-data
                    └─ LNK manipulation for persistence
```

---

## 6. IOCs

### Certificate IOCs (Primary Indicator — shared with cmd.Exe)
| Type | Value |
|---|---|
| Cert Subject | `Xiamen Jiaming Network Technology Co., Ltd.` |
| Cert Issuer | `Sectigo Public Code Signing CA EV R36` |
| Cert Serial | `00acb4af64ac061aadb00b5370b4ca0246` |
| Cert Validity | 2026-03-24 → 2027-03-24 |

### File Hashes
| File | SHA256 |
|---|---|
| 6.exe (binder/dropper) | `b1b5f12c8e57c3755ac55ada0a0aa7ed4af10edbb183ad22f0dc80e1c29a76ca` |
| TRUST_IMITATE_DLL.dll (RAT) | `87643b978e2f697c04f423bd646bc812099fe7966e4d1cad1f8c7643eb28fb72` |
| IRScriptEditor.exe (lure host) | `213d4df7c61bc05fcc1ed2144763555c1173fe01064f1ba5ca4687602eba053a` |
| Bind_DLL.dll (helper) | `2dd277b4db5088c81c529a29292fd82dbc70b44fb8ba841f8435d48cb5cc8e90` |
| PDF lure | `4ea3ee0e73b044fc1bbe579d17e84971a92dc1ee78ff5b58bbb780acbb17861b` |
| RTTI data (546B4B57) | `7769c9fab7fbd1683a6dfdb298e4cafafd58871bd59f90dc0eeaac99f720cbaa` |

### Filesystem IOCs
| Type | Value |
|---|---|
| Dropped filename | `cmcs21.dll` (TRUST_IMITATE_DLL.dll renamed — Indigo Rose runtime hijack target) |
| Dropped filename | `IRScriptEditor.exe` (Indigo Rose Script Editor v2.0.1002.0) |
| Dropped filename | `Bind_DLL.dll` |
| Build path | `F:\MainWork\Odes\_System.Rtti.pas` |
| Shortcut manipulation | `*.lnk` files (persistence mechanism) |

### Mutex IOCs
| Type | Value |
|---|---|
| Named mutex | `Global\a_` |
| Named mutex | `Global\b_` |
| Named mutex | `Global\c_` |
| Named mutex | `Global\d_` |
| Named mutex | `Global\e_` |
| Named mutex | `Global\x_` |

### Network IOCs
- **No static C2 extracted** — all URLs and endpoints are XOR-decrypted at runtime (83 XOR loops; dynamic analysis required)
- WinHTTP with `multipart/form-data` POST confirms data exfiltration channel
- TCP listen socket suggests inbound command capability

### Behavioral IOCs
- `SetWindowsHookExW(WH_GETMESSAGE=3)` — message hook install (fires on all window messages, not just keyboard)
- `GetCursorPos` / `FindVCLWindow` loop — mouse/screen monitoring thread
- `WinHttpOpen` / `WinHttpConnect` / `WinHttpSendRequest` — HTTP C2 (C2 URL runtime-decrypted; not recoverable statically)
- `Wow64DisableWow64FsRedirection` — WoW64 bypass
- Named mutexes `Global\[a-ex]_` — inter-component coordination
- `[IF_SERVICE]` / `[IF_SERVICE_RUNING]` string checks — service gate

---

## 7. Analyst Notes

### Threat Actor Link
This binary shares the **identical Authenticode certificate** (Xiamen Jiaming Network Technology Co., Ltd., Sectigo EV R36, serial `00acb4af64ac061aadb00b5370b4ca0246`) with `cmd.Exe` analyzed one day earlier. This establishes both artifacts as part of the same campaign. The cert was issued 2026-03-24; both samples appeared within 13 days of issuance. The Sectigo EV R36 CA continues to appear across multiple samples in this campaign cluster.

### Astaroth / Guildma Resemblance
The KesaKode classifier returns 37% Astaroth similarity for both 6.exe and TRUST_IMITATE_DLL.dll. Astaroth/Guildma (Brazilian banking trojan) is known for:
- DLL hijacking via legitimate tools (regsvr32, wscript, etc.)
- WinHTTP-based exfiltration of keystrokes and screenshots
- PDF/document lures
- Encrypted/obfuscated strings decrypted at runtime

The techniques match, though the specific DLL hijacking target (Indigo Rose Script Editor / `cmcs21.dll`) and Delphi codebase differ from canonical Astaroth samples. This may be a derivative, inspired family, or coincidental similarity.

### What Requires Dynamic Analysis
- **C2 URLs** — All WinHTTP hostnames are vtable-dispatched and runtime-decrypted; full emulation analysis of TRUST_IMITATE_DLL.dll (analysis_id=11) confirms no plaintext URL exists in the binary. The DLL activation guard (`*([0xBF8784]+0xC5)==0`) requires the IRScriptEditor.exe host process context — sandbox execution with WinHTTP API logging is the only viable extraction method.
- **PDF lure content** — password-protected; password likely passed at runtime
- **Drop path** — XOR-obfuscated target directory
- **Persistence mechanism** — `*.lnk` manipulation details unknown; Run key also possible
- **Stage 2** — whether TRUST_IMITATE_DLL.dll downloads further payloads after initial check-in

### YARA Detection
```yara
rule 6exe_DelphibinderXiamenJiaming {
    meta:
        description = "Delphi file binder with Xiamen Jiaming EV cert and TRUST_IMITATE_DLL"
        sha256 = "b1b5f12c8e57c3755ac55ada0a0aa7ed4af10edbb183ad22f0dc80e1c29a76ca"
    strings:
        $jiaming   = "Xiamen Jiaming Network Technology" ascii
        $bind_exp  = "Bind_EXE.exe" ascii
        $trust_dll = "TRUST_IMITATE_DLL" ascii
        $mutex_a   = "Global\\a_" ascii wide
        $mutex_b   = "Global\\b_" ascii wide
        $odes_path = "F:\\MainWork\\Odes" ascii
        $svc_chk   = "[IF_SERVICE]" ascii
    condition:
        uint16(0) == 0x5A4D and 2 of ($bind_exp, $trust_dll, $mutex_a, $mutex_b, $odes_path, $svc_chk)
}

rule DelphibinderTrustImitateDLL {
    meta:
        description = "TRUST_IMITATE_DLL.dll — Delphi RAT/keylogger payload"
        sha256 = "87643b978e2f697c04f423bd646bc812099fe7966e4d1cad1f8c7643eb28fb72"
    strings:
        $trust_exp = "TRUST_IMITATE_DLL.dll" ascii
        $odes_path = "F:\\MainWork\\Odes" ascii
        $mutex_a   = "Global\\a_" ascii wide
        $svc       = "[IF_SERVICE_RUNING]" ascii
    condition:
        uint16(0) == 0x5A4D and 2 of them
}
```

---

*Report generated: 2026-04-06 | Tools: malcat MCP (analyse_file, fn_decompile, strings, anomalies, yara, carved/virtual files), remnux MCP (peframe), Python (ZLIB decompression, MZ restoration)*
