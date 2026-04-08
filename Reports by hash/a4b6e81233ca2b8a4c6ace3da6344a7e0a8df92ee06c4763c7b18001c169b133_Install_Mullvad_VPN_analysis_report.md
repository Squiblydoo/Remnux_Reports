# Malware Analysis Report: Install Mullvad VPN.exe

## 1. File Metadata

| Field | Value |
|-------|-------|
| **Filename** | Install Mullvad VPN.exe |
| **SHA256** | `a4b6e81233ca2b8a4c6ace3da6344a7e0a8df92ee06c4763c7b18001c169b133` |
| **SHA1** | `d41b769ea80dee95b9b693993b844fcf11c5ee9d` |
| **MD5** | `4b6cda3faea6f438f896865b3ff83796` |
| **File Type** | PE32+ executable (GUI) x86-64, Windows |
| **Size** | 2,604,256 bytes (~2.6 MB) |
| **Architecture** | x86-64 |
| **Compiler** | Rust (YARA-confirmed) + MSVC 2022 linker (14.36.33813) |
| **PDB Path** | `installer_downloader.pdb` |
| **Internal Name** | `installer_downloader::environment` |
| **VersionInfo Product** | `installer-downloader v1.2.0` |
| **VersionInfo Copyright** | `(c) 2025 Mullvad VPN AB` |
| **Debug Timestamp** | 2025-10-02 07:48:04 UTC |

### Certificate / Authenticode

| Field | Value |
|-------|-------|
| **Issuer** | Sectigo Public Code Signing CA EV R36 (Sectigo Limited, GB) |
| **Subject** | **Xiamen Quanlian Information Technology Co., Ltd.** |
| **Org Details** | Xiamen Quanlian Information Technology Co., Ltd. / Fujian Sheng / **CN** |
| **Validity** | 2026-01-02 → 2027-01-02 (1-year EV cert) |
| **Serial** | `36453787ae28f52f1e09c6822dd816c3` |
| **Hash Algorithm** | SHA-256 / RSA |

> ⚠️ **Critical discrepancy**: The binary claims copyright by *Mullvad VPN AB* (Sweden) but is Authenticode-signed by *Xiamen Quanlian Information Technology Co., Ltd.* (China). Legitimate Mullvad VPN software is signed by Mullvad VPN AB.

---

## 2. Classification

**Family**: Re-signed Mullvad VPN Installer-Downloader (Authenticode Abuse / Supply-Chain Lure)  
**Confidence**: **MEDIUM-HIGH**  
**Threat Level**: MEDIUM (no independent payload confirmed; primary risk is identity spoofing and trust bypass)

### Reasoning

The binary is structurally and functionally consistent with Mullvad VPN's open-source `installer-downloader` component (available at their GitHub repository). Internally it:
- Uses the correct Mullvad API endpoint (`https://api[.]mullvad[.]net/app/releases/`)
- Verifies download integrity with SHA-256 (hardware-accelerated via Intel SHA-NI extensions)
- Displays Mullvad-branded UI and error messages
- Logs activity to `mullvad-loader.log`
- Matches the PDB path and module names of the legitimate project

However, **the Authenticode signature has been replaced** with one from a Chinese company not affiliated with Mullvad. This pattern — stripping a legitimate publisher's signature and applying a third-party EV certificate — is a documented TTP (T1036.001 + T1553.002). It is also consistent with multiple other samples in this workspace.

**Verdict**: The file is most likely a **re-signed authentic Mullvad VPN installer-downloader** distributed by a threat actor as a phishing/malvertising lure. Static analysis found no embedded secondary payload, no attacker-controlled C2 in strings, and no code patterns inconsistent with the legitimate Mullvad codebase. The risk is therefore classified as **identity spoofing** rather than payload delivery — but subtle code modifications cannot be fully excluded without successful dynamic analysis.

---

## 3. Capabilities

- **Download from API**: Fetches installer metadata from `https://api[.]mullvad[.]net/app/releases/`
- **Cryptographic verification**: SHA-256 (Intel SHA-NI hardware instructions) + SHA-384/512 via TLS; PKCS #7 DigestDecoration for certificate validation of downloaded payload
- **TLS/HTTPS communication**: Full TLS 1.3 stack (rustls); certificates embedded in binary
- **Installer execution**: Spawns downloaded installer via `RunShell` (CreateProcess / ShellExecute path) — YARA `RunShell` match
- **Progress UI**: Windows GUI subsystem; sends WM_USER+1 (`0x10001`) messages for install status
- **Logging**: Writes to `mullvad-loader.log`
- **Anti-debug**: `IsDebuggerPresent` referenced (signsrch confirmed; common in Rust via Windows API abstraction layer — low significance alone)
- **Async I/O**: Tokio runtime (FlsAlloc/FlsGetValue2 for fiber-local storage); WinAPI concurrency primitives

---

## 4. Attack Chain

```
Victim downloads "Install Mullvad VPN.exe"
    ↓
Binary is EV-signed by Xiamen Quanlian (CN) — SmartScreen bypass via EV trust
    ↓
Victim executes installer (trusting the EV signature)
    ↓
Binary contacts api.mullvad.net → fetches real Mullvad VPN installer
    ↓
SHA-256 verifies downloaded installer integrity
    ↓
Downloaded installer launched → user receives legitimate Mullvad VPN
    (threat: victim believes they installed authentic software through a trusted channel)
```

The campaign distributes a functional VPN installer, meaning the victim receives the actual product. The threat actor's gain is likely one or more of:
1. Planting a re-signed binary on a phishing site to deliver Mullvad as a post-exploitation tool (VPN for C2 tunneling)
2. Credential/reputation abuse: convincing victims to trust Xiamen Quanlian-signed software in future campaigns
3. Data interception: if binary was modified to also connect to attacker infrastructure before/after contacting Mullvad API (not confirmed by static analysis)

---

## 5. IOCs

### Network (Static)

| Indicator | Type | Notes |
|-----------|------|-------|
| `api[.]mullvad[.]net` | Domain | Mullvad VPN API — appears legitimate |
| `https://api[.]mullvad[.]net/app/releases/` | URL | Release metadata endpoint |

> No attacker-controlled network infrastructure identified in static strings.

### Certificate

| Indicator | Type | Notes |
|-----------|------|-------|
| `36453787ae28f52f1e09c6822dd816c3` | Cert Serial | Xiamen Quanlian EV cert — should be blocklisted |
| Xiamen Quanlian Information Technology Co., Ltd. | Subject DN | Unauthorized Mullvad signer |

### File System

| Indicator | Type | Notes |
|-----------|------|-------|
| `mullvad-loader.log` | Log file | Created in working directory during execution |
| `installer_downloader.pdb` | PDB artifact | Build artifact in binary |

### Embedded Verification Hashes (pecheck extracted — these are Mullvad installer integrity hashes, not threat IOCs)

These hashes were embedded in the binary for verifying the legitimacy of the downloaded Mullvad VPN installer package. They are not attacker infrastructure but are included for completeness:

- MD5: `0f9d85a4bc22ae02d2255bb785907bac`, `1af54cd8b45aaff33a50283e0db3e269`, `31681b9d795800e81e5c60d7c967c58a`, and 5 others
- SHA-1: `155d2c1f86084a440e2f160c444ddfa42e923863`, and 7 others
- SHA-256: `076a27c79e5ace2a3d47f9dd2e83e4ff6ea8872b3c2218f66c92b89b55f36560`, and 7 others

---

## 6. Emulation Results

### Speakeasy (Pass 1 — Generic Runner + Pass 2 — Direct)

- **Result**: Emulation terminated early at `FlsGetValue2` (Fiber Local Storage API not implemented in speakeasy)
- **Root cause**: Rust's Tokio async runtime initializes via `FlsAlloc`/`FlsGetValue2` during TLS initialization; speakeasy stub is too small to emulate this Win10+ API
- **IOCs captured**: None
- **API calls before crash**: `InitializeCriticalSectionEx` × 8, `GetProcessHeap`, `LoadLibraryExW("api-ms-win-core-fibers-l1-1-2")`, `GetProcAddress("FlsGetValue2")`, `VirtualProtect` × 3

**Limitation**: Emulation cannot confirm or deny runtime network behavior. A real execution environment (VM with network capture) is required to observe actual API calls to `api.mullvad.net`.

---

## 7. Sandbox Results

**Tria.ge**: Submission failed — API returned error 1010 (Cloudflare access denial / rate limit). No dynamic analysis from Tria.ge available.

**Recommendation**: Submit to an isolated Windows 10 VM with Wireshark/Fiddler capturing all network traffic to:
1. Confirm the binary only contacts `api.mullvad.net`
2. Rule out additional C2 beaconing to attacker infrastructure
3. Capture the full HTTP request/response to verify it downloads the legitimate Mullvad installer

---

## 8. Analyst Notes

### Anomalies Summary

| Anomaly | Severity | Interpretation |
|---------|----------|----------------|
| Invalid PE checksum | Level 4 | Consistent with post-build signature replacement |
| 167 XOR-in-loop hits | Level 3 | Normal for Rust TLS/crypto (AES-GCM, SHA, HKDF) |
| `.fptable` section | Level 3 | MSVC/LLVM function pointer table — not malicious |
| WeirdDebugInfoType | Level 3 | Rust PDB format difference from MSVC norm |
| 45 dynamic strings | Level 3 | Rust string formatting (format!/write! macros) |
| Huge binary strings | Level 4 | Embedded TLS certificate chain + RSA public key |
| DownloaderApiUsage | Level 2 | Expected — this is a downloader |
| RunShell | YARA | Expected — executes downloaded installer |

### YARA / Family Matching

- **Rust compiler**: Confirmed Rust binary
- **MSVC 2022 linker**: Consistent with Windows Rust toolchain (MSVC ABI)
- **RunShell** (lateral movement): Expected for installer-launcher
- **KesaKode**: ArcherRat 7%, PakiStealer 3% — low confidence, likely false positives for shared Rust library code

### Relation to Prior Campaigns

The Xiamen Quanlian cert follows the same pattern observed across multiple samples in this workspace:
- **cmd.Exe** (2026-04-06): Re-signed legitimate Windows cmd.exe with Xiamen Jiaming cert → T1036.001 + T1553.002
- **sethc.exe** (2026-04-03): Trojanized Sticky Keys backdoor with Sectigo EV Xiamen cert
- **6.exe** (2026-04-06): Delphi dropper with same Xiamen Jiaming cert as cmd.Exe

While these samples use different Xiamen company names (Jiaming, Quanlian, Xisu), the consistent pattern of CN-based EV certificates from Sectigo being applied to re-signed or trojanized binaries suggests either:
- A shared certificate acquisition service operating in China that supplies EV certs to malware campaigns
- Related threat actor(s) using a common supply chain for code-signing infrastructure

### Residual Gaps

1. **No confirmed dynamic behavior**: Cannot rule out subtle code modifications or secondary C2 without live execution in a monitored environment
2. **Overlay (11,488 bytes PKCS7)**: This is the Authenticode signature block — normal for EV-signed PE files
3. **Embedded installer hashes**: The SHA-256/SHA-1/MD5 hashes in the binary correspond to expected Mullvad installer packages. If the binary was modified to accept hashes of a malicious installer, this would not be detectable statically without comparing against known-good Mullvad source builds.

### Recommended Follow-Up

1. **Binary diff**: Compare byte-for-byte against a known-good `installer_downloader.exe` built from Mullvad's GitHub repository at the commit matching debug timestamp 2025-10-02. Hash mismatch (excluding the signature block) would confirm modification.
2. **VM dynamic analysis**: Run in isolated Windows 10 VM with full network capture; verify all HTTP(S) destinations
3. **Certificate revocation**: Report `36453787ae28f52f1e09c6822dd816c3` (Xiamen Quanlian) to Sectigo for revocation
4. **VirusTotal**: Submit SHA256 `a4b6e81233ca2b8a4c6ace3da6344a7e0a8df92ee06c4763c7b18001c169b133` for community detection context

---

*Analysis date: 2026-04-08*  
*Analyst: REMnux/malcat automated + manual review*
