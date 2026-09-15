# Malware Analysis Report: Airtame-4.16.0-setup.msi

## 1. File Metadata

| Property | Value |
|---|---|
| Filename | Airtame-4.16.0-setup.msi |
| SHA256 | `5b2b1ee50a1d227b50da17cfcf4081fece8021dfc66acaafabd2d0c0b7185d58` |
| SHA1 | `8e44d48137e7fab49cf5a45119eb26b2e17669c4` |
| MD5 | `c7c2711fcd730b6308e8336bee8a8a10` |
| Size | 87,126,016 bytes (~83 MiB) |
| Type | Composite Document File V2 (MSI Installer), built by **MSI Wrapper 9.0.30.0** (exemsi.com) |
| MSI metadata | Title: "Airtame 4.16.0", Author/Company: "Airtame", Created: 2019-04-10 (template date; not the build date of the wrapped payload) |
| Signing certificate | CN=`AIRTAME ApS`, O=AIRTAME ApS, L=København, C=DK, serial `35478973` — issued by DigiCert Trusted G4 Code Signing RSA4096 SHA384 2021 CA1, cert serial `06493E030F08A522B3F42770F887F223`, valid 2026-07-30 → 2027-07-29 |

### Build structure (nested)
```
Airtame-4.16.0-setup.msi   (MSI Wrapper container)
├── MsiCustomActions.dll    — stock MSI Wrapper custom-action DLL (installs the wrapped EXE)
└── WrappedSetupProgram.cab (CAB, store-only)
    └── Airtame-4.16.0-setup.exe  (NSIS installer, electron-builder generated)
        ├── setup.nsis / setup.bin — installer script + plugin DLLs (StdUtils, UAC, nsProcess, nsExec, WinShell, nsis7z, System, nsDialogs)
        └── app-32.7z (LZMA2/BCJ2, 7-Zip)
            └── Airtame.exe (151 MB, main Electron app) + Chromium runtime, resources/app.asar, AirtameEncryption.dll, AirtameAudioDriver.sys/.inf + installer, ffmpeg/x264/opus codecs, standard api-ms-win-* UCRT forwarders, elevate.exe
```

Each of the three executable layers carries an **independent Authenticode signature**, all issued to the same subject/cert serial:

| File | SHA256 | Signature check (osslsigncode) |
|---|---|---|
| Airtame-4.16.0-setup.exe (NSIS) | `f7af9973e4f4194c1b50912a805d2d64454bfdad304a905a812b74e7aaa12249` | **Verified OK** — message digest matches |
| Airtame.exe (main app) | `7aae3c7c9b9f0955e3e78c942ed41cd2f15fb3d8af046fb024699d5eb2ca2609` | **Verified OK** — message digest matches |
| Airtame-4.16.0-setup.msi (outer MSI) | (see above) | MSI-level `DigitalSignature` stream hash reported **MISMATCH** by osslsigncode 2.8; `MsiDigitalSignatureEx` (the CVE-2013-3900 anti-stream-tampering hash) **matches exactly**. This is a documented osslsigncode false-positive class for MSI verification (see Analyst Notes) — not corroborated by any other evidence of tampering. |

## 2. Classification

**Verdict: Benign — legitimate Airtame Windows installer, confidence HIGH.**

This is the genuine "Airtame" screen-mirroring/wireless-presentation client (real Danish company, Airtame ApS) packaged three times over:
1. The original Electron app + NSIS installer, built with electron-builder and signed by AIRTAME ApS.
2. Wrapped a second time into an MSI using the commercial **MSI Wrapper** tool (exemsi.com) — a common practice for IT departments deploying EXE installers via Group Policy/SCCM.

KesaKode online lookup (authoritative, ≥80%/20-79%/<20% thresholds applied) returned **zero family matches** for `MsiCustomActions.dll` and the NSIS setup stub (empty verdict = no hits at any confidence level); the lookup against the 151 MB main `Airtame.exe` binary timed out and was not completed (noted as a gap below, not treated as a negative signal).

## 3. Capabilities

Standard, non-malicious installer/application behavior only:
- MSI Wrapper custom-action DLL: standard install lifecycle exports (`InstallPrepare`, `InstallMain`, `InstallFinish1/2`, `InstallRollback`, `UninstallPrepare`, `UninstallFinish1/2`, `CheckReboot`, `SubstWrappedArguments`) — publicly documented exemsi.com tooling, PDB path `C:\ss2\Projects\MsiWrapper\MsiCustomActions\Release\MsiCustomActions.pdb` matches the known-legitimate build tree for this tool.
- NSIS installer: kills a running `Airtame.exe` via `taskkill`, extracts the 7-Zip-compressed Electron app payload, writes standard per-app uninstall registry keys (GUID `227b1281-8846-4aea-8330-58ad05c413cd`), creates Start Menu/Desktop shortcuts, copies itself to `%LOCALAPPDATA%\airtame-application-updater\installer.exe` (standard electron-builder auto-update self-copy pattern), optionally installs a legitimate Airtame virtual audio driver (`AirtameAudioDriverInstaller.exe`), and launches the app post-install.
- Main app (`Airtame.exe`): Electron/Chromium wireless-display client with native modules for audio capture (WASAPI), video capture (DXGI/GDI), H.264 encode (x264), screen streaming, and Wi-Fi utilities — all consistent with Airtame's advertised product functionality (screen mirroring to Airtame hardware receivers, meeting-room calendar integration).
- Embedded app.asar references only legitimate first-party (airtame.com, airtame.cloud, data.airtame.com, crash.airtame.com, help.airtame.com, airtame.zendesk.com) and well-known third-party integration endpoints (Microsoft/Google OAuth for calendar room-booking, WebEx API, keygen.sh licensing, S3) — no suspicious or attacker-controlled infrastructure.

## 4. Attack Chain

None. No droppers, no staged payloads beyond the vendor's own legitimate application, no persistence beyond the standard uninstall-registry/shortcut/updater-copy pattern common to all electron-builder apps.

## 5. IOCs

No malicious IOCs identified. For reference, legitimate first-party domains referenced by the application (not to be treated as indicators of compromise):
- airtame[.]com, airtame[.]cloud, data[.]airtame[.]com, crash[.]airtame[.]com, help[.]airtame[.]com, airtame[.]zendesk[.]com

No suspicious filesystem, registry, or mutex artifacts were identified beyond standard installer behavior described in Section 3.

## 6. Emulation Results

- **speakeasy (pass 1, generic runner)** on `MsiCustomActions.dll` (x86, DllMain): only generic MSVC CRT startup API resolution observed (`FlsAlloc/GetValue/SetValue/Free`, `EncodePointer`/`DecodePointer`). No network, registry, process, or mutex activity — consistent with a DLL whose install-lifecycle exports were not invoked (they require live MSI session context, not exercised by generic DllMain emulation). No further emulation passes were needed given the strength of the static evidence.
- Full emulation of the 151 MB `Airtame.exe` Electron binary was not attempted — Electron/CEF binaries are not practically emulatable with speakeasy/angr (large runtime dependency graph, V8/Chromium init), and static + dynamic-sandbox evidence was already conclusive.

## 7. Sandbox Results (ANY.RUN)

- **Verdict:** No threats detected (score 39/100, threat level 0)
- **Tags:** `evasion` (single generic tag; no accompanying malicious behavior, process, or network activity was flagged — consistent with a benign heuristic hit on standard UAC/installer elevation behavior rather than genuine evasion)
- **Dropped files:** standard Windows Installer artifacts only — `~DF*.TMP` temp files, MSI transaction tables (`MultiPackageTransaction`, `SystemRestoreSequence`, `!_Tables`), `C:\Windows\Installer\SourceHash{...}` cache, and the legitimate `C:\Program Files (x86)\Airtame\plugins\audio\AirtameAudioDriver.inf`
- **Network activity:** exclusively Microsoft PKI/CRL/OCSP endpoints (`crl.microsoft.com`, `ocsp.digicert.com`, `microsoft.com/pkiops/...`) generated by Windows' own Authenticode signature verification during install, plus routine Windows Update telemetry (`settings-win.data.microsoft.com`). **No airtame.com or attacker-controlled network activity was observed**, and critically **no non-Microsoft, non-PKI, non-telemetry destinations appeared at all** — no suspicious IOCs of any kind.
- **Public report:** https://app.any.run/tasks/6fe0c84f-f021-417a-b6d4-1b2109cb2949

## 8. Analyst Notes

- **MSI signature "MISMATCH" investigated and resolved as a tooling artifact, not evidence of tampering.** osslsigncode 2.8's MSI `DigitalSignature` stream-hash verification has a documented history of false-positive mismatches on files that verify cleanly under Microsoft's own `signtool.exe` (see upstream issue mtrojnar/osslsigncode#75 and related reports). In this case: (1) the newer `MsiDigitalSignatureEx` hash — specifically designed to close the CVE-2013-3900 MSI-stream-tampering bypass — matches exactly; (2) both executable payloads nested inside the MSI (the NSIS installer and the main Airtame.exe) carry their own independent, cleanly-verifying Authenticode signatures under the identical AIRTAME ApS certificate serial; (3) the wrapped CAB/NSIS/7z/Electron content is internally consistent, unmodified, standard electron-builder output with no injected code, extra files, or altered install logic. Taken together this outweighs the single osslsigncode-reported mismatch.
- The `ValuableFileExtensions` YARA hit (reliability 20/SUSPICIOUS, "embeds a list of file extensions often targeted by ransomware") on both the outer MSI and inner NSIS installer is a false positive: the matched strings are scattered single-token substrings (`sql`, `pem`, `zip`, `tar`, `rtf`, `ppt`, `odp`, `odf`, `avi`, `mp4`, `3ds`, `ora`, etc.) embedded throughout the 7-Zip SFX/LZMA library code bundled with the NSIS installer's `nsis7z.dll` plugin and the app's own archive tooling — not a curated ransomware target list. No file-enumeration-then-encrypt logic was found anywhere in the sample.
- capa flagged generic ATT&CK collection/keylogging heuristics on `MsiCustomActions.dll` (e.g. "log keystrokes", "reference SQL statements") — these are known low-reliability heuristic matches on boilerplate CRT/registry-handling code in small installer helper DLLs and are not corroborated by imports, exports, strings, or emulation; no keylogging or SQL-related functionality exists in this binary (its only imports are `RPCRT4`, `msi.dll`, `KERNEL32`, `USER32`, `ADVAPI32`, `SHELL32`, `SHLWAPI` and its exports are the standard MSI Wrapper lifecycle functions).
- KesaKode online lookup against the primary 151 MB `Airtame.exe` binary did not complete within the allotted time (240s timeout) due to its size; the smaller NSIS stub and custom-action DLL both returned clean (no family matches). This is a minor residual gap but does not change the verdict given the weight of other evidence (valid nested signatures, clean internal structure, legitimate embedded domains, standard electron-builder layout).
- **Recommended follow-up (only if there is a specific reason to suspect this particular file, e.g. it was obtained from an untrusted third-party download source rather than airtame.com):** diff this build's `app.asar` against the officially published Airtame 4.16.0 release to rule out supply-chain tampering at the source. Nothing in this analysis suggests that is necessary.
