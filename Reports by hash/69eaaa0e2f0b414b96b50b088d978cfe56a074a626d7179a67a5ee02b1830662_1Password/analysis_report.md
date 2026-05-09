# Malware Analysis Report: 1Password.exe

## 1. File Metadata

| Field | Value |
|-------|-------|
| **Filename** | 1Password.exe |
| **SHA256** | `69eaaa0e2f0b414b96b50b088d978cfe56a074a626d7179a67a5ee02b1830662` |
| **SHA1** | `2af75c797e7720518289980f52acb249e3f951fe` |
| **MD5** | `cf71869025e24b08d4528d70c6bcf252` |
| **Size** | 45,227,896 bytes (~45 MB) |
| **Type** | PE32 x86 GUI (Caphyon Advanced Installer ExternalUi stub) |
| **Cert Subject** | Minh Tran (Grand Prairie, TX, US) |
| **Cert Issuer** | Microsoft ID Verified CS AOC CA 04 |
| **Cert Serial** | `330000be4129fab1c1b414d19700000000be41` |
| **Cert Validity** | 2026-05-04 → 2026-05-07 **(3-day certificate)** |
| **Debug Timestamp** | 2023-09-07 12:39:10 (Caphyon stub reused timestamp) |
| **PDB Path** | `C:\ReleaseAI\win\Release\stubs\x86\ExternalUi.pdb` |
| **VersionInfo** | ProductName=1Password, ProductVersion=8.12.8.26, Company=Agilebits |
| **InternalName** | 1pass-50mb.exe |

---

## 2. Classification

**Classification:** Trojanized Installer — Fake 1Password Dropper with MSI Custom-Action C2 Stager

**Confidence:** High

**Reasoning:**

This sample is a **Caphyon Advanced Installer (AI) ExternalUi stub** carrying a 41.4 MB overlay containing two embedded 7-zip archives. The stub is signed with a fraudulent 3-day Microsoft ID Verified code-signing certificate to bypass Windows SmartScreen. The overlay bundles a legitimate copy of 1Password 8.12.8.26 as cover alongside a trojanized MSI installer. During sandbox execution, `msiexec.exe` installing the embedded MSI triggered a DNS lookup for **`apx-broadord[.]com`**, a domain with no known association to Agilebits/1Password, indicating a malicious MSI custom action that contacts attacker-controlled infrastructure.

---

## 3. Capabilities

- **SmartScreen bypass** via fraudulent 3-day Microsoft ID Verified code-signing certificate ("Minh Tran")
- **Masquerades as legitimate 1Password 8.12.8.26 installer** — VersionInfo, icons, and bundled app are genuine
- **XOR-encoded payload delivery**: Overlay archives have their 7z start header and packed streams XOR-0xFF encoded (end header is unencoded); a standard signature-evasion technique used by Advanced Installer
- **Bundles legitimate 1Password app** (~40 MB FILES.7z) to provide a working installation and reduce victim suspicion
- **Trojanized MSI custom action** (`1Password.msi`, ~789 KB) contacts `apx-broadord[.]com` during installation — likely a download stager, beacon, or credential-capture endpoint
- **Installer drops files to** `%APPDATA%\Agilebits\1Password 8.12.8.26\install\2A99D24\` and installs to `C:\Program Files (x86)\Agilebits\1Password\`
- **Leverages Windows Installer (msiexec)** for privilege-elevated execution of the custom action

---

## 4. Attack Chain

```
Victim downloads "1Password.exe" (social engineering / malvertising / typosquat)
  └─▶ Caphyon AI ExternalUi stub runs
        └─▶ Decodes overlay archives (XOR-0xFF on header+packed streams, raw end header)
              ├─▶ Extracts FILES.7z → %APPDATA%\...\install\2A99D24\ (legitimate 1Password app files)
              └─▶ Extracts 1Password.7z → 1Password.msi
                    └─▶ Spawns: msiexec.exe /i 1Password.msi (elevated)
                          ├─▶ Installs legitimate 1Password to C:\Program Files (x86)\Agilebits\1Password\
                          └─▶ [MALICIOUS CUSTOM ACTION]
                                └─▶ DNS: apx-broadord[.]com  ← attacker C2
                                      └─▶ (Download/beacon/stager — payload not captured)
```

**Process tree observed in sandbox (ANY.RUN):**
- PID 2436: `"C:\Users\admin\AppData\Local\Temp\1Password.exe"` (parent installer)
- PID 1504: `1Password.exe /i "...\2A99D24\1Password.msi" AI_EUIMSI=1 IAgree=Yes APPDIR="C:\Program Files (x86)\Agilebits\1Password\"` (MSI launcher)
- PID 2792: `msiexec.exe /i "...\2A99D24\1Password.msi" AI_EUIMSI=1 ...` ← triggers C2 contact
- PID 1824: `MsiExec.exe -Embedding 5E98B2BF63F53C4C3135039CDDF2D8DC C` (custom action host)

---

## 5. IOCs

### Network

| Type | Value | Notes |
|------|-------|-------|
| DNS request | `apx-broadord[.]com` | C2 domain, contacted during MSI install |

### Filesystem

| Path | Description |
|------|-------------|
| `%TEMP%\1Password.exe` | Installer execution path (sandbox) |
| `%APPDATA%\Agilebits\1Password 8.12.8.26\install\2A99D24\` | Staging directory (GUID `{B376A2A4-855C-42B4-82AE-8A5352A99D24}`) |
| `%APPDATA%\Agilebits\1Password 8.12.8.26\install\2A99D24\1Password.msi` | Trojanized MSI with malicious custom action |
| `%APPDATA%\Agilebits\1Password 8.12.8.26\install\2A99D24\chrome_100_percent.pak` | Legitimate 1Password app file (lure) |
| `C:\Program Files (x86)\Agilebits\1Password\` | Legitimate app install target |
| `C:\Windows\Installer\SourceHash{B376A2A4-855C-42B4-82AE-8A5352A99D24}` | MSI product code |
| `C:\Windows\Installer\MSI41AC.tmp` | Windows Installer temp file |
| `C:\Config.Msi\e3aa4.rbs` | Windows Installer rollback script |

### Hashes

| File | SHA256 |
|------|--------|
| 1Password.exe (main sample) | `69eaaa0e2f0b414b96b50b088d978cfe56a074a626d7179a67a5ee02b1830662` |
| chrome_100_percent.pak (dropped lure) | `45d14a4278b1e152b363197401a5604aa5a3cee6512a6b52df978038fa521a0f` |
| chrome_200_percent.pak (dropped lure) | `aafc61b89748d17fcbc9fecd9844a77be2c584529a81714c98e0c4d453ea9496` |

### Certificate

| Field | Value |
|-------|-------|
| Subject | Minh Tran / Grand Prairie / Texas / US |
| Serial | `330000be4129fab1c1b414d19700000000be41` |
| Issuer | Microsoft ID Verified CS AOC CA 04 |
| Valid | 2026-05-04 → 2026-05-07 |

---

## 6. Overlay Architecture & Encoding

The file has a **41.4 MB overlay** appended after the Caphyon AI PE sections. The overlay contains two XOR-encoded 7-zip archives:

### Encoding scheme

Caphyon AI uses a non-standard archive protection:
- The 7z **start header (32 bytes)** is XOR-0xFF encoded (flips the signature to hide `37 7A BC AF 27 1C`)
- The **packed streams** (LZMA2-compressed body) are also XOR-0xFF encoded
- The **end header** (plain 7z metadata footer) is stored **raw** (no XOR)
- A **UTF-16LE manifest** at the tail of the overlay (file offset ~45,212,836) contains archive paths, sizes, and absolute file offsets for each archive

### Archive layout

| Archive | File Offset | Size | Contents |
|---------|-------------|------|----------|
| FILES.7z | 3,740,160 (0x00391200) | 40,682,511 bytes | 1Password 8.12.8.26 Electron app (LZMA2:22, solid block) |
| 1Password.7z | 44,422,671 (0x02A5D20F) | 789,586 bytes | 1Password.msi with malicious custom action (LZMA2:3m) |

### Manifest

The overlay manifest at file offset 45,212,836 includes:
- Archive path: `2A99D24\FILES.7z` (offset 0x00391200, size 0x026CC3FF)
- Archive path: `2A99D24\1Password.7z` (size 0x000C0C52 = 789,586)
- INI reference: `1pass-50mb.ini`
- Installer GUID fragment: `{...4114-A81D-768E0195069E}`

---

## 7. Static Analysis Notes

**Anomalies detected (malcat):**
- `ImportByHash` (169 instances) — APIs resolved at runtime by hash
- `XorInLoop` (231 instances) — overlay decode loops
- `StackArrayInitialisationX86` (247 instances) — dynamic string construction
- `CrossSectionJump` (3 instances) — unusual control flow
- `PictureResourceWrongType` — ICO/BMP resources with wrong magic (benign in Caphyon AI)

**YARA matches:**
- `DownloadUsingWininet`, `PostHttpForm` — WinInet downloader capability (Caphyon AI framework)
- `CreateScheduledTask`, `AutorunKey` — persistence primitives (Caphyon AI standard features)
- `FingerprintEnvironment`, `EnumerateProcesses` — environment checks
- `RunShell`, `Powershell` — shell/PowerShell execution (Caphyon AI uninstall/update logic)

**KesaKode verdict:** MetaStealer (10% confidence) — low confidence, likely false positive from Caphyon AI framework code patterns.

---

## 8. Emulation Results

**Speakeasy (generic runner, pass 1):** No IOCs — expected behavior for an interactive installer UI stub that requires user interaction to reach the overlay decode and extraction path. The Caphyon ExternalUi displays an installation wizard; the overlay decryption loop is not reached during automated emulation.

---

## 9. Sandbox Results (ANY.RUN)

| Field | Value |
|-------|-------|
| Score | **100 / 100** |
| Verdict | **Malicious activity** |
| Tags | `advancedinstaller` |
| Public URL | https://app.any.run/tasks/23d9a320-8d5f-42fd-9ca9-14b62870330f |

**Key sandbox findings:**
- Installer auto-ran the full installation chain under a user session
- Extracted 1Password.msi to `%APPDATA%\Agilebits\...\2A99D24\`
- Ran `msiexec.exe /i 1Password.msi` with elevated MSI custom action host (`MsiExec.exe -Embedding`)
- **DNS lookup for `apx-broadord[.]com`** — only non-Microsoft, non-Agilebits external domain
- No HTTP payload recovered (connection likely timed out or required specific configuration in sandbox)
- All other network traffic: Windows Update telemetry, OCSP/CRL for certificate validation, Microsoft authentication endpoints

---

## 10. Analyst Notes

### What is confirmed

1. **The certificate is fraudulent.** A 3-day Microsoft ID Verified certificate for an individual ("Minh Tran") is a known abuse pattern for SmartScreen bypass (T1553.002). The very short validity window is characteristic of certificates obtained immediately before use.

2. **The bundled 1Password app appears legitimate.** The FILES.7z archive contents (chrome_100_percent.pak hashes match known-good builds, file names match 1Password 8.12.8.26 Electron structure) indicate the threat actor copied a real 1Password installer to use as a convincing decoy.

3. **`apx-broadord[.]com` is not a 1Password/Agilebits domain.** DNS was triggered from within the msiexec process during MSI custom action execution. The domain has no known legitimate association. This is the primary malicious indicator.

4. **The MSI custom action payload was not recovered.** The 1Password.msi's LZMA2-compressed contents (inside the XOR-encoded 7z archive) could not be fully extracted with available tooling. The custom action's full functionality — whether it is a downloader, credential harvester, or beacon — is unknown.

### Gaps and residual unknowns

- **`apx-broadord[.]com` IP not resolved** — no A record or HTTP connection was captured in the sandbox. The server may be geo-filtered, rate-limited, or offline during the sandbox window.
- **No second-stage payload recovered** — if the custom action downloads a further payload, it was not captured.
- **MSI custom action type not determined** — could be Type 1 (DLL), Type 2 (EXE), or Type 51 (script). Requires MSI extraction for confirmation.
- **Delivery vector unknown** — likely malvertising, search engine poisoning (typosquatting "1password download"), or targeted spearphishing.

### Recommended follow-up

1. Resolve `apx-broadord[.]com` using passive DNS and threat intelligence to identify infrastructure age, registrant, and co-hosted malicious domains.
2. Extract `1Password.msi` using a Windows environment (run `msitools` or `lessmsi` after proper decompression) to recover the custom action bytecode.
3. Check if `330000be4129fab1c1b414d19700000000be41` (cert serial) or `Minh Tran`/Grand Prairie TX is associated with other malicious signed binaries (same re-signing campaign as Dropper.exe `330000b80523beb847f3bd8d4900000000b805`, different serial but same subject and locality — possible same individual/service).

---

## 11. MITRE ATT&CK

| Technique | Description |
|-----------|-------------|
| T1553.002 | Code Signing — fraudulent 3-day MS ID Verified cert for SmartScreen bypass |
| T1036.001 | Masquerade: Match Legitimate Name or Location — poses as 1Password installer |
| T1027 | Obfuscated Files or Information — XOR-encoded 7z archives in overlay |
| T1027.002 | Software Packing — Caphyon AI ExternalUi loader obfuscation |
| T1071.001 | Application Layer Protocol: Web Protocols — HTTP/DNS to `apx-broadord[.]com` |
| T1105 | Ingress Tool Transfer — suspected download from C2 (not confirmed) |
| T1218.007 | System Binary Proxy Execution: Msiexec — malicious custom action via msiexec |
| T1566.002 | Phishing: Spearphishing Link — suspected delivery (not directly observed) |
