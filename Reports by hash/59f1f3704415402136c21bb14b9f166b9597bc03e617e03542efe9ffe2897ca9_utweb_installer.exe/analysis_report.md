# Malware Analysis Report: utweb_installer.exe

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | `utweb_installer.exe` |
| SHA256 | `59f1f3704415402136c21bb14b9f166b9597bc03e617e03542efe9ffe2897ca9` |
| SHA1 | `7e263c4e5fb8433cad85c8a672695465396faeac` |
| MD5 | `e39ef8a1dbed7aa93b3a1af5416c1e8e` |
| Type | PE32 executable (GUI), Intel 80386, 11 sections |
| Size | 2,186,048 bytes |
| Builder | Inno Setup 6.7.0 (SetupLdr.e32, Delphi) |
| File description (VersionInfo) | "µT Web®" |
| Product name | "µT Web®" |
| Legal copyright (VersionInfo, fake) | "©2026 BitTorrent Limited. All Rights Reserved" |
| Signing certificate | **RainCo HK Limited** — issuer DigiCert Trusted G4 Code Signing RSA4096 SHA384 2021 CA1, serial `05ad03da40d6a5b629a125c437c93882`, valid 2026-07-07 → 2027-07-06, location Kwun Tong, Hong Kong |
| Build artifact | PDB path `D:\Coding\Is\issrc-build\Components\ChaCha20.pas` (custom Inno Setup component) embedded as a UTF-16 user string |

The VersionInfo block impersonates BitTorrent/Rainberry copyright text, but the code-signing certificate belongs to an unrelated Hong Kong entity ("RainCo HK Limited") with no visible corporate tie to Rainberry, Inc. (the real publisher of uTorrent/µTorrent).

### Dropped/downloaded secondary payload
During analysis, the primary CloudFront download endpoint was still live and was retrieved for further static analysis:

| Field | Value |
|---|---|
| Filename | `utweb_installer_rr.exe` |
| SHA256 | `d6f547e7270366df500323d93653a8c2d00a13484f6f5a8c73719ce2bb468a50` |
| Type | NSIS self-extracting installer, PE32 |
| Size | 17,945,848 bytes |
| Signing certificate | **BitTorrent Inc** — issuer Symantec Class 3 SHA256 Code Signing CA, serial `6f13bcd50963d2f309439e37fd459c7c`, valid 2019-10-04 → 2022-12-16 (**expired**) |
| VersionInfo | ProductName "uTorrent Web", FileVersion 1.3.0.5416, CompanyName "Rainberry, Inc." |

This is a genuine, but outdated and expired-cert, uTorrent Web 1.3.0.5416 installer built by BitTorrent Inc./Rainberry — being redistributed wholesale through the `utweb_installer.exe` PPI wrapper rather than downloaded from an official BitTorrent/Rainberry channel.

## 2. Classification

**Malware family**: Not applicable — this is a **PPI (pay-per-install) adware bundler / "carrier" installer**, not a backdoor, stealer, or ransomware. ANY.RUN dynamic tags (`adware`, `offercore`, `innosetup`, `loader`) and behavior corroborate this.

**Confidence: High** (adware/PPI classification, corroborated by static Pascal-script decompilation, live infrastructure retrieval, and ANY.RUN dynamic detonation).

**KesaKode online lookup**: Verdict `Rakhni: 1.65%` — below the 20% noise threshold. **Discarded**; no family attribution signal.

### Cross-reference note (CryptoVista actor toolkit)
The embedded PDB path `D:\Coding\Is\issrc-build\Components\ChaCha20.pas` is an **exact, verbatim match** to the fingerprint build artifact tracked for the [[family_cryptovista]] actor (a custom Inno Setup component used only by that actor's installer builder, previously seen in SDK_Driver.exe, UtilifySetup.exe, UltraPlusSetup.msi, and others). This meets the strict cross-reference bar (identical build artifact).

However, the *behavior* here is materially different from previously tracked CryptoVista samples: those built Electron-app RCE backdoors (readme.txt/setup.txt beacon config, `eval()`-based operator RCE). This sample contains **no such structure** — no readme.txt/setup.txt config pair, no Electron payload, no RCE logic. Instead it implements a JSON ad-server beacon + conditional payload download + silent-install chain (`/S` flag), consistent with a PPI/adware "carrier" business model. This is a new observed use-case of the shared Inno Setup builder toolkit — consistent with the existing memory note that "the toolkit is reused for different end-goals across builds." Recorded here as a new sample under that actor's toolkit, not as a new distinct family.

## 3. Capabilities

- Silently beacons a JSON payload (installer version, OS version, language, `"a":"BitTorrent"`, `"i":"uTorrent_Web"`, `"s":"uTorrent_Web_ZB_2"` campaign/sub-ID) via `WinHttp.WinHttpRequest.5.1` to an ad-mediation endpoint (`dkdsdc6kiej7h.cloudfront.net/o`) before/during install.
- Parses the JSON ad-server response (custom `PARSEJSON`/`CLEARJSONPARSER` Pascal-script routines) to determine which advertiser offer to route the victim to.
- Downloads a large secondary installer over HTTPS from a primary CloudFront URL, with a hardcoded fallback URL (`utweb_installer_rr.exe`) if the primary fails or a required SHA256 mismatch occurs (`DownloadTemporaryFile`/`TDownloadWizardPage`).
- Silently executes the downloaded installer with `/S` (`Exec()` call, `TExecWait` = wait-until-terminated).
- Dynamic detonation (ANY.RUN) shows the ad-mediation layer routing to **multiple rotating third-party PPI offers** — Avast (full EULA/consent flow via Adobe Target/Ensighten tracking), McAfee WebAdvisor, and Opera GX banner creatives — served from the same `dkdsdc6kiej7h.cloudfront.net` CDN path (`/f/AVAST/...`, `/f/WebAdvisor/...`, `/f/OperaGX/...`), alongside a call to `api.playanext.com/httpapi` — PlayAnext is a known commercial PPI/bundling network.
- Reuses genuine BitTorrent Limited legal text (EULA authored by a real BitTorrent legal staffer, dated 2023) inside `license.rtf` to lend the installer legitimacy.
- Elevates privileges (Yara: `ElevatePrivileges`, `AdjustTokenPrivileges`, `SeShutdownPrivilege`, `CheckTokenMembership`) — standard Inno Setup elevation for install-to-Program-Files, not attacker-specific.
- The recovered secondary payload (`utweb_installer_rr.exe`) is a legitimate, but expired-certificate (2022), Rainberry uTorrent Web 1.3.0.5416 NSIS installer; its own script contains dormant logic to silently chain-execute a `play_installer.exe` bundled-offer installer if one is dropped to `%LOCALAPPDATA%\utweb_install_temp\` (`-q /SRC AUTOMATIC_BTWEB /FROI /BUNDLED /USP /FORCEINSTALL`), consistent with PlayAnext-style forced bundled-offer installation.

## 4. Attack Chain

1. Victim downloads/runs `utweb_installer.exe`, presented as a µTorrent Web installer, signed by "RainCo HK Limited."
2. Installer wizard launches; in parallel it POSTs device/campaign telemetry to an ad-mediation CloudFront endpoint (`/o`).
3. Ad-mediation response is parsed; based on response, the wizard silently downloads a large installer from CloudFront (fallback `utweb_installer_rr.exe` observed live).
4. The downloaded installer is silently executed (`/S`).
5. In dynamic detonation, the ad-mediation layer additionally served/loaded third-party PPI offer creatives and full consent/EULA flows for Avast, McAfee WebAdvisor, and Opera GX (via `api.playanext.com` and CloudFront `/f/<Brand>/...` image assets) — indicating the ad exchange may route victims to install one or more of these bundled products depending on geography/campaign, in addition to (or instead of) uTorrent Web.
6. The genuine but outdated/expired-cert uTorrent Web is installed as the ostensible "real" product, creating a Start Menu/Desktop shortcut and uninstall registry key under `Software\Microsoft\Windows\CurrentVersion\Uninstall\utweb`.

## 5. IOCs

### Network
- `hxxps[://]dkdsdc6kiej7h[.]cloudfront[.]net/o` — ad-mediation JSON beacon endpoint
- `hxxps[://]dkdsdc6kiej7h[.]cloudfront[.]net/zbd` — secondary ad-mediation endpoint
- `dkdsdc6kiej7h[.]cloudfront[.]net/f/AVAST/images/DOTPS-2113/547x280/EN[.]png` — Avast offer creative (dynamic)
- `dkdsdc6kiej7h[.]cloudfront[.]net/f/WebAdvisor/images/943/EN[.]png` — McAfee WebAdvisor offer creative (dynamic)
- `dkdsdc6kiej7h[.]cloudfront[.]net/f/OperaGX/images/DOTPS-1867/LightBG/EN[.]png` — Opera GX offer creative (dynamic)
- `hxxps[://]d280l0babyev03[.]cloudfront[.]net` — primary payload download URL
- `hxxps[://]d2ue7gwtozyjnm[.]cloudfront[.]net/utweb_installer_rr[.]exe` — fallback payload download URL (live, retrieved during analysis)
- `d76z6hqjnkuxp[.]cloudfront[.]net/pp` , `/eula`, `/tos` — privacy policy / EULA / ToS pages
- `hxxps[://]api[.]playanext[.]com/httpapi` — PlayAnext PPI/bundling network API (dynamic, third-party)
- `i-4101.b-5416.utweb.bench.utorrent[.]com` / `i-4101.b-6445.utweb.bench.utorrent[.]com` — legitimate Rainberry A/B-test telemetry endpoint (inherited from bundled genuine uTorrent Web installer)

### Filesystem
- `%LOCALAPPDATA%\utweb_install_temp\play_installer.exe` (conditional, chain-execution target inside the bundled uTorrent Web NSIS installer — not embedded/dropped by this sample directly)
- `%TEMP%\ut_web_redist\vcredist_x86.exe` (bundled inside secondary NSIS payload)

### Registry
- `HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\utweb` — uninstall entry created by the bundled genuine uTorrent Web installer (DisplayName "uTorrent Web", Publisher "Rainberry, Inc.")

### Mutex
- `µT WebMutex` (AppMutex)
- `µTorrent/268d6644-f2c2-4c...` (SetupMutex)

### Hashes
- `59f1f3704415402136c21bb14b9f166b9597bc03e617e03542efe9ffe2897ca9` — `utweb_installer.exe` (analyzed sample)
- `d6f547e7270366df500323d93653a8c2d00a13484f6f5a8c73719ce2bb468a50` — `utweb_installer_rr.exe` (retrieved secondary payload, genuine expired-cert uTorrent Web 1.3.0.5416)

### Certificates
- `05ad03da40d6a5b629a125c437c93882` — RainCo HK Limited (signer of analyzed sample)
- `6f13bcd50963d2f309439e37fd459c7c` — BitTorrent Inc (expired, signer of retrieved secondary payload)

## 6. Emulation Results

- **Speakeasy (pass 1, generic runner, x86)**: Emulation started but produced only a single trivial IOC (`GetProcAddress('wine_get_version')` — a standard Wine-detection probe common in Inno Setup stubs) before stalling. The installer's meaningful logic (ad-server beacon, download, silent-exec) lives entirely in the **Inno Setup Pascal Script bytecode interpreter**, not in raw native code reachable by early emulation — this logic was instead fully recovered via static decompilation of `embedded/script.ps` (Binary Refinery `xtinno`).
- Deeper emulation passes (angr/custom hooks) were not required — full network/exec logic was already recovered statically with higher fidelity than emulation could provide for this installer type.

## 7. Sandbox Results (ANY.RUN)

- **Verdict score**: 100/100 — **Malicious activity**
- **Tags**: `adware`, `offercore`, `innosetup`, `delphi`, `inno`, `installer`, `arch-exec`, `arch-scr`, `loader`
- **Public report**: https://app.any.run/tasks/cd736fbb-a597-410d-a7dc-dad19a7d92c8
- Dynamic run confirmed the ad-mediation → third-party-offer routing behavior described in Section 3/4: DNS/HTTP traffic to `dkdsdc6kiej7h.cloudfront.net` serving Avast/WebAdvisor/OperaGX offer creatives, a full Avast EULA consent-tracking flow (Adobe Target `symantec.tt.omtrdc.net`), and a call to `api.playanext.com/httpapi` (a known PPI/bundling network, matching the `play_installer.exe` chain-execution logic found statically in the bundled uTorrent Web NSIS installer).
- The `offercore` tag is a known ANY.RUN classifier for the "OfferCore" PPI/adware installer SDK family, independently corroborating the static PPI-carrier assessment.

## 8. Analyst Notes

- This sample is best described as a **deceptive PPI/adware distribution wrapper**, not a conventional malware payload: its ultimate observed action is silently installing a genuine (if outdated and expired-cert) uTorrent Web build, while an ad-mediation layer simultaneously/alternatively routes installs to legitimate third-party software (Avast, McAfee WebAdvisor, Opera GX) through a commercial PPI network (PlayAnext). The primary abuse vector is **undisclosed bundling and brand impersonation** (fake BitTorrent copyright string under an unrelated Hong Kong signer) rather than data theft, persistence, or destructive capability.
- Because the ad-server response is dynamic and campaign/geo-dependent, a different detonation (different IP, timing, or campaign ID) could route to a different set of bundled offers, or potentially a different final payload than the uTorrent Web build recovered here. The offer surface should be treated as non-exhaustive.
- The exact meaning of the `"s": "uTorrent_Web_ZB_2"` campaign identifier (likely a sub-affiliate/tracking ID within the PPI network) was not further resolved.
- No readme.txt/setup.txt/RCE structure (the usual CryptoVista actor fingerprint payload) was found — this appears to be a legitimate-adjacent commercial use of the same Inno Setup builder toolkit (shared `ChaCha20.pas` component) rather than the actor's backdoor tooling. Recommended follow-up: watch for additional samples sharing this exact PDB path to determine whether "CryptoVista" is better understood as a builder-for-hire used by multiple downstream operators (backdoor operators *and* PPI/adware operators) rather than a single monolithic actor.
- No readable/decodable encrypted strings were identified requiring `decrypt_string`/`chain_decrypt_analysis` — Inno Setup's `XChaCha20` archive encryption field in `meta/setup.json` reported `Scope: NoEncryption` (i.e., the installer data itself is not password-encrypted, only structurally compressed).
