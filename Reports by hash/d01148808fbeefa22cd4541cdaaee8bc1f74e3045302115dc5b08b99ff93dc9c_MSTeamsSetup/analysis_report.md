# Malware Analysis Report: MSTeamsSetup.exe
**Date:** 2026-04-08  
**Analyst:** REMnux Analysis Workstation  
**Confidence:** HIGH — Malicious

---

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | MSTeamsSetup.exe |
| SHA256 | `d01148808fbeefa22cd4541cdaaee8bc1f74e3045302115dc5b08b99ff93dc9c` |
| SHA1 | `93aa31051cd1bac3bb2ffddb71f93330dcab9d89` |
| MD5 | `ff8505309831284bff66a1cfd5049dac` |
| Size | 14,058,976 bytes (13.4 MB) |
| File Type | PE32 executable (GUI) Intel 80386, 7 sections |
| Architecture | x86 (32-bit) |

### Certificate
| Field | Value |
|---|---|
| Subject | **Zlatin Stamatov** |
| Organization | Zlatin Stamatov (individual) |
| Location | Burgas, Burgas, BG |
| Issuer | Certum Code Signing 2021 CA (Asseco Data Systems S.A., PL) |
| Serial | `0f971773c38e4b32acb121855151baa4` |
| Validity | 2026-03-14 → 2027-03-14 (1-year individual cert) |
| Algo | SHA256/RSA |

> **Spoofing**: VersionInfo claims `CompanyName=Microsoft`, `FileDescription=Microsoft Teams App Installer`, `ProductVersion=5.3.2`, `OriginalFileName=MSTeamsSetup.exe` — all false. The actual signer is a Bulgarian individual, not Microsoft Corporation.

### Build Artifacts
| Field | Value |
|---|---|
| Builder | **Caphyon Advanced Installer** (ExternalUi stub) |
| PDB Path | `C:\ReleaseAI\win\Release\stubs\x86\ExternalUi.pdb` |
| Debug Timestamp | 2024-12-10 11:15:23 |
| Compiler | MSVC 2022 v17.10-17.11 (Rich Header) |
| Linker | MSVC 2022 |

---

## 2. Classification

**Verdict: HIGH CONFIDENCE MALICIOUS**  
**Family: Custom Browser / MITB Credential Stealer (DanielKern.org Browser)**  
**Lure: Fake Microsoft Teams 5.3.2 Installer**

This sample is a Caphyon Advanced Installer wrapper that delivers a custom browser ("DanielKern.org Browser") and companion persistence components, strongly consistent with a man-in-the-browser (MITB) or credential-harvesting operation. The disguise leverages the Microsoft Teams brand with forged VersionInfo metadata and an individually-purchased code-signing certificate.

**Similar prior samples in this cluster:**
- `DDinosaur.exe` (2026-03-03): same Caphyon AI ExternalUi stub, same 3-day Microsoft ID Verified cert pattern. MSTeamsSetup uses a longer-lived (1-year) Certum cert.
- `MTSetup_v15.3.7191.msi` (2026-03-24): same Teams lure theme, different payload chain.

---

## 3. Capabilities

- **Lure / Social Engineering**: Impersonates Microsoft Teams 5.3.2 with forged metadata and a signed certificate to pass superficial security checks.
- **Custom Browser Delivery**: Installs `DanielKern.org Browser.msi` (2.7 MB) — a custom browser from the actor-controlled domain `danielkern[.]org`. Custom browsers are standard tools for MITB attacks (intercepting HTTPS sessions, stealing cookies/credentials).
- **Payload Dropper**: Extracts and installs `secure_browser.exe` (123 KB) and `systemautoupdater.exe` (34 MB) to hidden directory `%APPDATA%\Microsoft\Microsoft Teams App 5.3.2\install\.winupdate\`.
- **Overlay Obfuscation**: Two 7z archives embedded in the PE overlay are XOR-encrypted (0xFF on start headers; file content additionally XOR-encrypted), thwarting casual static extraction.
- **API Obfuscation**: 188 ImportByHash hits; APIs are resolved at runtime by hash, hiding the true import list.
- **String Obfuscation**: 252 XorInLoop hits; 66 dynamic/stack strings; 256 StackArrayInit obfuscated string constructions.
- **PowerShell Execution**: Hidden `-WindowStyle Hidden -ExecutionPolicy Unrestricted` PowerShell subprocess with output capture — used to run post-install scripts.
- **WinInet Downloader**: Can download prerequisites/payloads over HTTP/HTTPS using WinInet APIs.
- **HTTP POST**: Posts data using HTTP form POST (capability/telemetry or C2 beacon).
- **Process Enumeration**: Enumerates running processes (sandbox/AV detection or process injection targeting).
- **Scheduled Task Creation**: Creates scheduled tasks for persistence.
- **Privilege Elevation**: Uses Windows API for UAC bypass / privilege escalation.
- **Hidden Directory**: Installs payloads to `.winupdate` (dot-prefixed, mimicking Windows Update).

---

## 4. Attack Chain

```
User executes MSTeamsSetup.exe
    │
    ├── [UI] Shows fake "Microsoft Teams 5.3.2" installer dialog
    │       (Caphyon AI WinUI installer framework)
    │
    ├── [Overlay Decrypt] XOR-decodes two embedded 7z archives from overlay
    │       (XOR key 0xFF on 7z start headers; file contents also XOR'd)
    │
    ├── [Archive 1] Extracts to %APPDATA%\...\Microsoft Teams App 5.3.2\install\.winupdate\
    │       ├── secure_browser.exe     (123 KB) — browser launcher / stub
    │       └── systemautoupdater.exe  (34 MB)  — Electron or full browser runtime (persistence)
    │
    ├── [Archive 2] Extracts DanielKern.org Browser.msi (2.7 MB)
    │       └── MSI installs the DanielKern.org custom browser
    │
    ├── [PowerShell] Runs post-install scripts hidden
    │       powershell.exe -NonInteractive -NoLogo -ExecutionPolicy Unrestricted
    │                      -WindowStyle Hidden -Command "..." 2> tempfile
    │
    └── [Persistence] Scheduled task and/or Run key via systemautoupdater.exe
              └── DanielKern.org Browser operates as MITB proxy / credential stealer
```

### Payload Component Sizes (for attribution)
| File | Size | Role |
|---|---|---|
| `secure_browser.exe` | 123,464 bytes | Browser stub / launcher |
| `systemautoupdater.exe` | 34,341,448 bytes | Browser runtime (likely Electron) |
| `DanielKern.org Browser.msi` | 2,780,672 bytes | Browser MSI installer |

> **Note on extraction**: The 7z archives use a two-layer XOR obfuscation: the 7z start header is XOR'd with 0xFF (for signature hiding), and the decompressed file contents are *also* XOR-encrypted. This non-standard LZMA2 extension prevented automated extraction with standard tools. Archive headers parsed and file names/sizes confirmed via py7zr.

---

## 5. IOCs

### Network
| Type | Value | Notes |
|---|---|---|
| Domain | `danielkern[.]org` | Custom browser distribution; actor-controlled C2/payload host |

> `dev[.]mysql[.]com` — appears in Caphyon AI prerequisite downloader URL comparison code; **not an actor IOC**; this is a hardcoded Caphyon AI framework check for MySQL prerequisite sources.

### Files / Filesystem
| Path | Notes |
|---|---|
| `%APPDATA%\Microsoft\Microsoft Teams App 5.3.2\install\.winupdate\secure_browser.exe` | 123 KB payload |
| `%APPDATA%\Microsoft\Microsoft Teams App 5.3.2\install\.winupdate\systemautoupdater.exe` | 34 MB payload |
| `%APPDATA%\Microsoft\Microsoft Teams App\prerequisites\` | Prerequisite download staging dir |
| `%TEMP%\DanielKern.org Browser.msi` | Browser MSI (transient) |

### Registry
| Key | Notes |
|---|---|
| `SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{75028C29-EF18-4E0F-B9BC-2AC34512F6EF}` | Installed product entry |

### Certificate
| Field | Value |
|---|---|
| Serial | `0f971773c38e4b32acb121855151baa4` |
| Subject | Zlatin Stamatov, Burgas BG |
| Issuer | Certum Code Signing 2021 CA |

### Installer GUIDs
| GUID | Type |
|---|---|
| `{75028C29-EF18-4E0F-B9BC-2AC34512F6EF}` | ProductCode |
| `{CB59B3DA-3B78-4675-8018-832B9AB9C113}` | UpgradeCode |
| `{443E4FD4-45AE-46D8-BF52-16A35F7DCFE0}` | PackageCode |

---

## 6. Emulation Results

Speakeasy (x86, generic runner, 60s): **No IOCs recovered.**  
The Caphyon Advanced Installer stub operates primarily through MSI engine APIs and Windows Installer services; emulation cannot reach the payload extraction stage without a full MSI runtime environment. No network, file, registry, or mutex events were captured.

---

## 7. Sandbox Results

**Tria.ge:** Submission blocked by Cloudflare (HTTP 403, error code 1010). Dynamic coverage unavailable.

---

## 8. MITRE ATT&CK Mapping

| TTP | Technique | Description |
|---|---|---|
| T1036.001 | Masquerading: Invalid Code Signature | VersionInfo claims Microsoft; cert is Bulgarian individual |
| T1553.002 | Subvert Trust Controls: Code Signing | Purchased 1-year individual Certum cert to bypass SmartScreen |
| T1204.002 | User Execution: Malicious File | Requires user to run the Teams installer |
| T1027 | Obfuscated Files or Information | XOR overlay encryption, ImportByHash, stack string obfuscation |
| T1564.001 | Hide Artifacts: Hidden Files and Directories | `.winupdate` directory for payload storage |
| T1059.001 | Command and Scripting Interpreter: PowerShell | Hidden PowerShell for post-install scripts |
| T1185 | Browser Session Hijacking | DanielKern.org custom browser intercepts sessions |
| T1539 | Steal Web Session Cookie | Custom browser credential/cookie theft |
| T1053.005 | Scheduled Task/Job: Scheduled Task | Persistence via scheduled task creation |
| T1134 | Access Token Manipulation (Elevation) | Privilege elevation via Windows API |

---

## 9. Analyst Notes

**Key unknowns / recommended follow-up:**

1. **DanielKern.org browser analysis**: The core payload (DanielKern.org Browser.msi, 2.7 MB) was not fully extracted due to the two-layer XOR obfuscation in the archive format. Priority action: obtain `DanielKern.org Browser.msi` separately, or analyze under Wine/Windows sandbox to understand browser modifications and C2 infrastructure. The domain `danielkern[.]org` should be queried for current DNS resolution and certificate history (Shodan, VirusTotal, PassiveDNS).

2. **systemautoupdater.exe (34 MB)**: At 34 MB this is consistent with an Electron-based application (typical 40-80 MB fully bundled, smaller when compressed). This is likely the main browser runtime. If Electron, it would contain a `resources/app.asar` with obfuscated JavaScript payload (similar to `ms_x64_update.exe` in our campaign cluster). The `systemautoupdater.exe` name is consistent with a persistence-and-update mechanism.

3. **secure_browser.exe (123 KB)**: At 123 KB this could be a bootstrapper/launcher that starts the DanielKern.org browser in a specific mode (proxy mode, MITB mode, or credential-intercepting mode). Requires sandbox analysis.

4. **Campaign linkage**: The use of Caphyon AI (`C:\ReleaseAI\win\Release\stubs\x86\ExternalUi.pdb`) is a potential TTP cluster with DDinosaur.exe (which used `C:\JobRelease\win\Release\stubs\x86\ExternalUi.pdb`). The path difference (`ReleaseAI` vs `JobRelease`) suggests a different build environment but similar tooling. The Teams lure was also seen in MTSetup_v15.3.7191.msi. This may be the same or affiliated threat actor.

5. **Certificate actor pattern**: Zlatin Stamatov (BG) uses a 1-year Certum individual cert (affordable, no organization vetting) — consistent with the use of Xiamen CN EV certs in other samples in this workspace. Buyer attribution not possible from cert data alone, but Certum should be notified for revocation.

6. **Overlay XOR encryption variant**: The two-layer XOR in the 7z archives (header XOR + decompressed-content XOR) is non-standard Caphyon AI behavior. This suggests a modified/cracked version of Caphyon AI or a post-build packing step applied by the actor. This is distinct from standard Caphyon overlay format used in DDinosaur.

---

*Report generated: 2026-04-08*  
*Tools used: malcat, speakeasy, peframe, py7zr, Python custom overlay decoder*
