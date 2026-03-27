# Malware Analysis Report: ETDucky_Installer.dll

**Date:** 2026-03-26
**Analyst:** Claude Code (automated static analysis)
**Confidence:** HIGH — malicious

---

## 1. File Metadata

| Field | Value |
|-------|-------|
| **Filename** | ETDucky_Installer.dll |
| **SHA256 (outer)** | `eae8cd926d6e304636c68e9923bc3f8132aebf27e330ecb61c8f5c8c7e77f385` |
| **MD5 (outer)** | `210f60b26ef14fcacca95b0a9fa1fc46` |
| **SHA1 (outer)** | `ecb9db239426ee36e9bf1598e6be7707d4ffba40` |
| **SHA256 (inner .NET DLL)** | `826a2379aa7b1f10e4c649bdde189e54fae755cbcdfedcefef70e774670bc621` |
| **SHA256 (ETDucky_AgentSetup.exe)** | `15d297b8287ab307ad66d204be389c2bebf733f0f5ba6ba5ffd8525e166d34ec` |
| **SHA256 (embedded ZIP)** | `e26910093d5aa4e63a81660a7d0aff2afe6f5fc45f03a169da9a8f85bc8e7e0b` |
| **Size** | 32,721,016 bytes (31.2MB) |
| **Type** | PE32+ executable (GUI) x86-64, .NET 8 Single-File Bundle |
| **Architecture** | x64 |

### Code Signing Certificate
| Field | Value |
|-------|-------|
| **Issuer** | SSL.com EV Code Signing Intermediate CA RSA R3 |
| **Subject** | ET Ducky LLC |
| **Org Details** | ET Ducky LLC, Bellingham, Washington, US |
| **Valid** | 2026-03-05 → 2027-03-05 (1-year EV cert) |
| **Serial** | `6607c6d3aa188e3ea1cedbec3a764f36` |
| **Algorithm** | SHA256/RSA |

### Build Artifacts
- **Outer wrapper PDB**: `D:\a\_work\1\s\artifacts\obj\coreclr\windows.x64.Release\Corehost.Static\singlefilehost.pdb` — GitHub Actions build runner (`D:\a\_work\1\s\`)
- **Inner .NET DLL PDB**: `D:\VS_Projects\ETDucky.InstallerLauncher\obj\Release\net8.0-windows\win-x64\linked\ETDucky_Installer.pdb` — Developer machine
- **Inner DLL debug timestamp**: `2058-03-15 10:16:23` — **manipulated/future timestamp**
- **Bundle ID**: `6qGsZuirRogk`
- **Product Version**: `1.0.0.0` (installer launcher) / `1.8.8.4` (AgentSetup)

---

## 2. Classification

**Family:** ET Ducky Agent — cloud-based agent/RAT deployment framework
**Confidence:** HIGH
**Type:** Multi-stage self-extracting installer, cloud-enrolled implant

**Reasoning:**
- 4-stage installation chain with silent UAC elevation and operator-gated registration
- Hardcoded `organizationName: "nazi"` in tenant config (clear malicious operator identifier)
- EV code signing cert for a company with no public legitimate presence ("ET Ducky LLC", Bellingham WA)
- Debug timestamp manipulation (2058 future date)
- Inno Setup payload layer uses ChaCha20 encryption — identical tooling pattern (same `ChaCha20.pas` PDB path prefix `D:\Coding\Is\iss`) as the **CryptoVista campaign Stage 1** installer analyzed 2026-03-04. Strong indicator of shared threat actor or toolset.
- Cloud C2 registration token: `etd_EBowrs6xjdMoy93Mz2taoEWExVCHi3dTLGNyclKe` — active, generated 2026-03-20

---

## 3. Capabilities

- **UAC Elevation**: Re-launches self via `runas` verb if not already Administrator; uses `--etd-elevated` flag to avoid re-elevation loop
- **Self-Extraction**: Reads own binary, locates embedded ZIP payload by searching for `ETDUCKY\x00` magic marker (8 bytes) before Authenticode signature boundary
- **Payload Extraction**: Extracts ZIP to `%TEMP%\ETDucky_Install_<8-char-guid>\`; ZIP contains `ETDucky_AgentSetup.exe` + `tenant.json`
- **Tenant Configuration**: Reads `tenant.json` for `registrationToken`, `apiBaseUrl`, `organizationName`
- **Silent Execution**: Launches `ETDucky_AgentSetup.exe /VERYSILENT /SUPPRESSMSGBOXES /NORESTART /REG_TOKEN="<token>"` — invisible to user
- **Cleanup**: Deletes temp extraction directory after 1s sleep
- **Cloud Registration**: Agent registers to `https://etducky.com/api` with the operator-supplied token
- **Privilege Check**: `WindowsIdentity.GetCurrent()` → `WindowsPrincipal.IsInRole(Administrator)`

---

## 4. Attack Chain

### Stage 1 — Outer Launcher (ETDucky_Installer.dll as singlefilehost.exe)
- PE32+ x64, .NET 8 Self-Contained Single-File Bundle signed with SSL.com EV cert (ET Ducky LLC)
- Acts as a host executable for the bundled .NET runtime + payload DLL
- Contains legitimate `mscordaccore.dll` (Microsoft .NET DAC, signed by Microsoft) in RCDATA resources — standard single-file bundle component
- Entrypoint delegates immediately to the bundled `ETDucky_Installer.dll`

### Stage 2 — Installer Launcher (.NET 8, `ETDucky.InstallerLauncher` namespace)
**Full source decompiled via ilspycmd:**
1. Checks `IsRunningAsAdmin()` — if not elevated, spawns self with `Verb = "runas"` and `--etd-elevated` flag, waits, exits
2. Reads own binary bytes from `Environment.ProcessPath`
3. `GetPreSignatureLength()`: parses PE headers to find the start of the Authenticode signature directory entry (avoids searching over the signature)
4. `FindMagicOffset()`: scans backward from signature boundary for 8-byte magic `ETDUCKY\x00` (bytes `45 54 44 55 43 4B 59 00`)
5. Reads 4-byte little-endian ZIP size immediately before the magic
6. Extracts ZIP bytes and unpacks to `%TEMP%\ETDucky_Install_<8hexchars>\`
7. Reads `tenant.json` → parses JSON → extracts `registrationToken`
8. Launches `ETDucky_AgentSetup.exe /VERYSILENT /SUPPRESSMSGBOXES /NORESTART /REG_TOKEN="etd_EBowrs6xjdMoy93Mz2taoEWExVCHi3dTLGNyclKe"`
9. Waits for process exit; sleeps 1s; deletes temp dir

### Stage 3 — Agent Setup (ETDucky_AgentSetup.exe, Inno Setup 6.7.0)
- Delphi x86 Inno Setup installer, version 1.8.8.4, product "ET Ducky Agent"
- **18.7MB encrypted overlay** (entropy 225 — near-maximum, ChaCha20-encrypted payload archive)
- Custom ChaCha20 encryption for Inno Setup data (PDB: `D:\Coding\Is\iss...\ChaCha20.pas`) — **same ChaCha20.pas tooling as CryptoVista campaign Stage 1**
- `BCryptGenRandom` for strong random generation
- Privilege elevation via `AdjustTokenPrivileges`, `CheckTokenMembership`, ACL manipulation
- Registers the deployed agent with the ET Ducky cloud platform using the `REG_TOKEN` CLI parameter

### Stage 4 — ET Ducky Agent (deployed payload, not extracted)
- Registers to `https://etducky.com/api` with operator-issued registration token
- Organization: `"nazi"`, token name: `"loop"`, generated 2026-03-20T16:44:59Z
- Capabilities of deployed agent: unknown without dynamic analysis

---

## 5. IOCs

### Network
| Type | Value | Notes |
|------|-------|-------|
| Domain | `etducky.com` | C2 / agent management platform |
| URL | `https://etducky.com/api` | Agent registration & C2 API endpoint |
| Registration Token | `etd_EBowrs6xjdMoy93Mz2taoEWExVCHi3dTLGNyclKe` | Operator-issued token for org "nazi" |

### File System
| Path | Description |
|------|-------------|
| `%TEMP%\ETDucky_Install_<8hexchars>\` | Temporary extraction directory (self-deleting) |
| `%TEMP%\ETDucky_Install_<8hexchars>\ETDucky_AgentSetup.exe` | Extracted Inno Setup stage |
| `%TEMP%\ETDucky_Install_<8hexchars>\tenant.json` | Extracted tenant config |
| Wherever Inno Setup installs to | ET Ducky Agent installation directory (unknown without dynamic analysis) |

### Hashes
| File | SHA256 |
|------|--------|
| ETDucky_Installer.dll (full bundle) | `eae8cd926d6e304636c68e9923bc3f8132aebf27e330ecb61c8f5c8c7e77f385` |
| ETDucky_Installer.dll (inner .NET) | `826a2379aa7b1f10e4c649bdde189e54fae755cbcdfedcefef70e774670bc621` |
| ETDucky_AgentSetup.exe | `15d297b8287ab307ad66d204be389c2bebf733f0f5ba6ba5ffd8525e166d34ec` |
| Embedded ZIP (AgentSetup + tenant.json) | `e26910093d5aa4e63a81660a7d0aff2afe6f5fc45f03a169da9a8f85bc8e7e0b` |

### Strings / Artifacts
| Artifact | Value |
|----------|-------|
| Magic marker | `ETDUCKY\x00` (bytes: `45 54 44 55 43 4B 59 00`) |
| Inno Setup version | `Inno Setup Setup Data (6.7.0)` |
| CLI flag (elevation) | `--etd-elevated` |
| CLI flag (silent install) | `/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /REG_TOKEN="..."` |
| Operator org name | `nazi` |
| Token name | `loop` |
| Token generated | `2026-03-20T16:44:59.3593051Z` |

### Process Behavior
- Spawns self elevated via `ShellExecute` with `runas` verb
- Spawns `ETDucky_AgentSetup.exe` as child process with hidden window
- `Thread.Sleep(1000)` before temp dir cleanup

---

## 6. Analyst Notes

### Gaps / Requires Dynamic Analysis
1. **Stage 4 (actual agent)**: The ET Ducky Agent itself was not extracted — it is locked inside the ChaCha20-encrypted Inno Setup payload. Dynamic execution of `ETDucky_AgentSetup.exe` in a sandbox would reveal the final agent, its persistence mechanism, and full C2 protocol.
2. **`etducky.com` infrastructure**: Live querying of the API would reveal what capabilities the registered agent exposes. The `/api` endpoint may accept agent telemetry, commands, or file operations.
3. **Registry/persistence**: Inno Setup installers typically write to `HKCU\SOFTWARE\...` or `HKLM\SOFTWARE\...`; run keys likely set for persistence.

### Campaign Attribution — CryptoVista Link
The `ChaCha20.pas` custom Inno Setup encryption component (PDB prefix `D:\Coding\Is\iss`) is identical to the pattern found in the CryptoVista campaign Stage 1 installer (`CryptoVista_Installer_HNc38.exe`, analyzed 2026-03-04). This is a strong indicator of either:
- **Same threat actor** reusing their custom Inno Setup ChaCha20 build environment
- **Shared tooling/kit** sold/shared among multiple operators

### Alternative Hypotheses
- **Legitimate commercial RMM abused**: "ET Ducky" could be a legitimate commercial remote management agent that was licensed under a malicious organization name. However, the debug timestamp manipulation (2058), EV cert for an obscure entity, and `organizationName: "nazi"` strongly argue against legitimate use.
- **Internal pentest/red team tool**: Unlikely given no pentest engagement indicators and the `organizationName` value.

### Authenticode Structure
The self-extraction relies on the Authenticode signature directory entry (PE optional header security directory) as an anchor to find the appended payload. The `GetPreSignatureLength()` function reads `e_lfanew` (offset 0x3C) → PE header → optional header magic → security directory RVA to compute where the certificate table begins. The ZIP payload + magic + size integer sit immediately before this boundary, allowing extraction without invalidating the signature.
