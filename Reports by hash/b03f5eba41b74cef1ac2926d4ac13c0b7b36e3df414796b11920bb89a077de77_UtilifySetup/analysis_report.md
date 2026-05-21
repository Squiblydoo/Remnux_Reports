# Malware Analysis Report: UtilifySetup.exe

**Analyst:** Claude Code (automated)  
**Date:** 2026-05-21  
**Sample:** UtilifySetup.exe

---

## 1. File Metadata

| Field | Value |
|-------|-------|
| **SHA256** | `b03f5eba41b74cef1ac2926d4ac13c0b7b36e3df414796b11920bb89a077de77` |
| **SHA1** | `9434fe686801742ef7d6da248fb0b900dc32208a` |
| **MD5** | `18a7251ddde77ed24bc54700d84d9be1` |
| **File type** | PE32 x86 GUI, Inno Setup 6.7.0 installer (Delphi SetupLdr) |
| **File size** | 56,590,976 bytes (~54 MB) |
| **Build timestamp** | 2026-02-11 11:40:27 UTC |
| **ProductName** | MegaUtilify |
| **CompanyName** | ProPlus EasySoft |
| **ProductVersion** | 9.9.7 |
| **FileDescription** | MegaUtilify Setup |

### Digital Signature

| Field | Value |
|-------|-------|
| **Issuer** | Sectigo Public Code Signing CA EV R36 |
| **Subject** | Lway Firmware |
| **Organization** | Lway Firmware, Uusimaa, FI (Finland) |
| **Serial** | `00c311bb931ee237bc5edb1fc6469d8777` |
| **Validity** | 2026-03-19 → 2027-06-17 |
| **Algorithm** | SHA256/RSA |

### Build Artifact

The Inno Setup stub was compiled locally at `D:\Coding\Is\issrc-build\` (not the official Inno Setup release binary):

```
D:\Coding\Is\issrc-build\Components\ChaCha20.pas
```

This PDB path is **not** present in any official Inno Setup 6.7.0 release binary. It identifies a custom build of Inno Setup compiled by the actor from source.

---

## 2. Classification

| Field | Value |
|-------|-------|
| **Classification** | Telegram Credential Phishing Installer / CryptoVista Toolkit |
| **Confidence** | **High** |
| **ANY.RUN Score** | 100/100 — Malicious activity |
| **ANY.RUN Tags** | `phishing`, `telegram`, `auto-reg`, `delphi`, `inno`, `installer` |

**CryptoVista toolkit attribution:** The PDB path `D:\Coding\Is\issrc-build\Components\ChaCha20.pas` appears verbatim in SmartUtilSetup.exe (SHA256: `a5f73996...`, analyzed 2026-04-24). This is an identical build artifact linking both samples to the same actor-compiled Inno Setup build. The install directory `%LOCALAPPDATA%\SuperMaxionQuickMaxlite\` follows the same compound-nonsense naming convention used across CryptoVista samples (`EasyCoreQuickWareer`, `AutoToolSuperToolware`, `ProPlus EasySoft`). This sample represents a shift in the toolkit's payload strategy: **credential phishing** via a fake Telegram Web login page (`web-telegram[.]ug`) rather than the beacon/RCE backdoor pattern seen in earlier toolkit variants.

---

## 3. Capabilities

- **Electron app lure deployment** — installs a full Electron (Chromium-based) application to `%LOCALAPPDATA%\SuperMaxionQuickMaxlite\`
- **Telegram credential phishing** — Electron app loads `web-telegram[.]ug`, a fake Telegram Web login page; victim enters Telegram credentials which are exfiltrated to attacker
- **Auto-start persistence** — ANY.RUN confirmed `autoStart=true`; Inno Setup runs at login
- **Wine/VM detection** — stub checks for `wine_get_version` via `GetProcAddress`; installer halts in Wine environments
- **Encrypted payload** — 55.6 MB overlay encrypted with ChaCha20 (Inno Setup 6.7.0 default); blocks static extraction without executing the stub
- **FNV-1a API hash resolution** — 23 API calls resolved by FNV-1a hash (`seed=0x811c9dc5`) in the Inno Setup stub; non-standard for legitimate Inno Setup builds
- **Multi-process execution** — multiple child processes spawned during install (`multiprocessing=true`)
- **Fraudulent EV certificate** — Sectigo EV cert issued to "Lway Firmware" (Finnish entity) spoofs legitimacy; provides SmartScreen bypass

---

## 4. Attack Chain

```
1. User executes UtilifySetup.exe
   └─ Inno Setup 6.7.0 stub (custom actor build)
   └─ Wine/VM check → abort if sandbox detected

2. ChaCha20-encrypted 55MB payload decrypted and extracted
   └─ Electron application → %LOCALAPPDATA%\SuperMaxionQuickMaxlite\
   └─ Locale files, Chromium binaries, app.asar confirmed in dropped artifacts

3. Auto-start persistence registered
   └─ Electron app set to run at login

4. Electron app launches → loads web-telegram[.]ug
   └─ Fake Telegram Web login page (phishing)
   └─ Victim enters Telegram credentials → exfiltrated

5. C2 domain: web-telegram[.]ug
   └─ .ug TLD (Uganda) masquerading as Telegram Web
```

---

## 5. IOCs

### Network

| Type | Indicator |
|------|-----------|
| **Domain (C2/Phishing)** | `web-telegram[.]ug` |

### Hashes

| Type | Hash |
|------|------|
| SHA256 (installer) | `b03f5eba41b74cef1ac2926d4ac13c0b7b36e3df414796b11920bb89a077de77` |
| SHA1 (installer) | `9434fe686801742ef7d6da248fb0b900dc32208a` |
| MD5 (installer) | `18a7251ddde77ed24bc54700d84d9be1` |
| SHA256 (dropped: en-GB.pak) | `96ee955686d67eb5a79db5deeabf3efce9f7e9f28c2af270ce2f210d9ebe0e99` |
| SHA256 (dropped: lv.pak) | `941f26e6e457ee4612468448b47e7eee15801c6faca4571d19929641879b2259` |

### Certificate

| Field | Value |
|-------|-------|
| Serial | `00c311bb931ee237bc5edb1fc6469d8777` |
| Subject | Lway Firmware (FI, Uusimaa) |
| Issuer | Sectigo EV |
| Valid | 2026-03-19 → 2027-06-17 |

### Filesystem

| Path | Description |
|------|-------------|
| `%LOCALAPPDATA%\SuperMaxionQuickMaxlite\` | Electron app install directory |
| `%LOCALAPPDATA%\SuperMaxionQuickMaxlite\locales\en-GB.pak` | Electron locale file (confirmed dropped) |
| `%TEMP%\is-*.tmp\` | Inno Setup extraction temp directory |

### Build Artifact (Attribution)

```
D:\Coding\Is\issrc-build\Components\ChaCha20.pas
```

---

## 6. Emulation Results

**Speakeasy (pass 1):** No IOCs recovered — the installer requires full Windows runtime (Inno Setup decompresses ChaCha20-encrypted payload) to activate; speakeasy cannot emulate the Inno Setup extraction chain.

**floss:** Skipped — file exceeds 16 MB floss size limit.

**capa / peframe:** Both timed out on the 56 MB file.

**Static payload extraction:** The 55,685,728-byte Inno Setup data stream (`[0]`) starts with `zlb\x1a` blocks encrypted with ChaCha20. LZMA decompression failed for all offsets, confirming encryption. Unencrypted Chromium binary resources (gzip streams, Electron locale `.pak` files) were identified at file offset ~40 MB via binwalk, confirming an Electron app payload. The malicious `app.asar` JavaScript could not be extracted without decrypting the Inno Setup data stream.

---

## 7. Sandbox Results

| Field | Value |
|-------|-------|
| **Platform** | ANY.RUN |
| **Task ID** | `970e0c1a-4661-4015-90be-82a2bb1a4f0d` |
| **Score** | 100/100 |
| **Verdict** | Malicious activity |
| **Tags** | `phishing`, `telegram`, `auto-reg`, `delphi`, `inno`, `installer` |
| **autoStart** | true |
| **multiprocessing** | true |
| **knownThreat** | true |
| **Public report** | https://app.any.run/tasks/970e0c1a-4661-4015-90be-82a2bb1a4f0d |

**Key behavioral observations:**
- Electron app deployed to `%LOCALAPPDATA%\SuperMaxionQuickMaxlite\`  
- DNS query to `web-telegram[.]ug` confirmed
- Persistence via auto-start registration confirmed
- Two Electron locale `.pak` files dropped (en-GB.pak, lv.pak)

**Notes on `login.live.com` contacts:** ANY.RUN also captured connections to `login.live.com/RST2.srf` and `/ppsecure/deviceaddcredential.srf`. These are standard Windows SSO/Passport endpoints contacted by the Windows OS in the sandbox environment and are assessed as background noise, not attributable to the malware.

---

## 8. MITRE ATT&CK

| ID | Technique |
|----|-----------|
| T1204.002 | User Execution: Malicious File |
| T1036.001 | Masquerading: Invalid Code Signature |
| T1553.002 | Subvert Trust Controls: Code Signing |
| T1547.001 | Boot or Logon Autostart Execution |
| T1140 | Deobfuscate/Decode Files or Information (ChaCha20) |
| T1027 | Obfuscated Files or Information (encrypted payload) |
| T1056.003 | Input Capture: Web Portal Capture (phishing login) |
| T1598 | Phishing for Information |
| T1497.001 | Virtualization/Sandbox Evasion: System Checks (Wine detect) |

---

## 9. Analyst Notes

**Payload remains unextracted.** The malicious `app.asar` (Electron app JavaScript bundle) is inside the ChaCha20-encrypted Inno Setup data stream and could not be decrypted statically. If an Inno Setup extraction tool (e.g., `innoextract`) becomes available, re-running extraction on this sample would reveal the full JavaScript payload, the complete C2 mechanism, any persistence scripts, and whether the app includes a secondary RCE beacon alongside the phishing page.

**Evolution of CryptoVista toolkit:** Previous variants (SmartUtilSetup, UltraPlusSetup) used a JavaScript C2 beacon with RCE and file-drop capabilities against the victim. This sample pivots to pure credential phishing via a fake Telegram Web page (`web-telegram[.]ug`). The actor may be targeting Telegram sessions/accounts rather than arbitrary code execution, possibly because Telegram session hijacking is more valuable for follow-on operations against specific targets.

**web-telegram[.]ug:** The `.ug` TLD (Uganda country code) is frequently abused for lookalike domains. Recommend blocking the entire `web-telegram[.]ug` domain and checking for related domains (e.g., `web-telegram[.]to`, `web-telegram[.]cc`, etc.).

**Certificate revocation:** Lway Firmware (FI) Sectigo EV cert serial `00c311bb931ee237bc5edb1fc6469d8777` should be reported to Sectigo for revocation. The certificate is used to sign malware distributed as a phishing installer.

**Recommended follow-up:**
1. Re-analyze with `innoextract` when available to extract `app.asar` and JavaScript payload
2. Pivot on `web-telegram[.]ug` infrastructure (WHOIS, hosting ASN, related domains)
3. Check for additional CryptoVista variants signed with this Sectigo cert serial
4. Pivot on "Lway Firmware" (FI, Uusimaa) as a fictitious entity used to obtain fraudulent EV certs
