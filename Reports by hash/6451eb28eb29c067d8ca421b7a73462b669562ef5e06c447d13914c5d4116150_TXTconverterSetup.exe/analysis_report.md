# Malware Analysis Report: TXTconverterSetup.exe

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | TXTconverterSetup.exe |
| SHA256 | `6451eb28eb29c067d8ca421b7a73462b669562ef5e06c447d13914c5d4116150` |
| SHA1 | `e8ab7406d6f6430f7c6aba21607d12e7815702f6` |
| MD5 | `0d46017a3c37466dcecf99dcc1632bcc` |
| File type | PE32 executable (GUI), Intel 80386, .NET (Mono/.NET) assembly, 3 sections |
| Size | 1,488,704 bytes (~1.42 MB) |
| Compile timestamp | 2026-05-17 10:42:40 UTC |
| Imphash | `f34d5f2d4577ed6d9ceec516c1f5a744` |
| Framework | WPF (.NET, PresentationFramework/PresentationCore) |
| Entry point | `DotNetEntryPoint` (managed, `mscoree.dll` native stub) |

### Signing
- **Signed**: Yes — valid Authenticode signature
- **Issuer**: GlobalSign GCC R45 EV CodeSigning CA 2020
- **Subject**: KALIM LIMITED (Nicosia/Tseri, Cyprus; email `andri@UpKalim.com`)
- **Serial**: `75cfed98acf1d361fbff156b`
- **Validity**: 2026-04-28 → 2027-04-29 (cert is current/recently issued)

### Build artifacts / version info
- FileDescription: `TXTconverter.Installer`
- ProductName: `TXTConverter`, ProductVersion/FileVersion: `3.1.1.2`
- Comments: `TXTConverter Installer`
- Internal namespaces: `TXTconverter.Installer`, `TXTconverter.Installer.Services`, `TXTconverter.Installer.Views`, `TXTconverter.Installer.Helpers`, `TXTconverter.Installer.Constants`

## 2. Classification

**Confirmed (ANY.RUN behavioral/static detection, score 100/100, "known threat")**: This installer functions as a **PureCrypter-style loader that deploys PureLogs Stealer** as its final payload.

- ANY.RUN sandbox verdict: **Malicious activity**, threat score **100**, `knownThreat: true`
- ANY.RUN tags: `stealer`, `purelogs`, `purecrypter`
- Malcat offline KesaKode: `StomExfiltrator` at 0% confidence — not meaningful (below 20% discard threshold)
- Malcat online KesaKode (via `cloud.malcat.fr`, authoritative per policy): **no matches returned** — the installer stub itself is not a byte-for-byte/function-level match to any tracked family in KesaKode's database. This is consistent with the malicious component being **server-delivered at runtime** rather than embedded in the installer binary — KesaKode has nothing to fingerprint until the payload lands.

**Reasoning**: Static analysis of the decompiled .NET code (via ilspycmd) shows this is architecturally a **server-gated dropper/downloader** disguised as a legitimate WPF installer for a "TXTconverter — PDF Utility Suite" product:

1. Before downloading anything, it silently collects and POSTs a machine fingerprint JSON (`osBuild`, `installerVersion`, `appExeExists`, `approvedCheckbox`, `powerProfile`) to `download.txtconverters.com/check_latest_version`.
2. The server's response — a base64-encoded ZIP — is decoded and extracted directly into the install path and later launched as `TXTconverter.exe`. **The actual payload is never present in the installer file itself; it is chosen server-side per request**, based on the submitted fingerprint. This is a classic gating technique that lets the operator serve a clean/benign app to researchers, automated scanners, or specific geographies/OS builds, while serving the stealer payload to real targets.
3. ANY.RUN's detonation triggered the malicious branch, and its detection engine flagged the resulting behavior/payload as PureLogs Stealer delivered via a PureCrypter-style loader chain.
4. A second endpoint, `api.txtconverters.com/finish`, receives a "finish" callback (either installation-error telemetry or a success/error JSON) — functioning as install-outcome telemetry back to the operator. ANY.RUN flagged this URL with reputation 2 (malicious) and the domain `api.txtconverters.com` was also scored malicious (reputation 2) in DNS requests.

**Confidence: High** for "malicious server-gated downloader delivering a stealer," based on independent sandbox detonation evidence. The specific "PureLogs / PureCrypter" family attribution is ANY.RUN's classification (not independently confirmed via KesaKode); treat the installer itself as unattributed/novel, and the delivered final-stage payload as PureLogs Stealer per ANY.RUN.

## 3. Capabilities

From decompiled source (`TXTconverter.Installer.Services.ExternalCallsService`, `MachineInfoService`, `InstallerService`, `RegistryHelper`, `ShortcutHelper`):

- **Machine fingerprinting** prior to any download:
  - OS build number (`HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\CurrentBuildNumber`)
  - Installer/assembly version
  - Whether the app already exists at the default install path (`AppExeAlreadyExists`)
  - EULA "approved checkbox" state (always hardcoded `true`)
  - **Power profile bitmask** via `PowrProf.dll!GetPwrCapabilities` (P/Invoke) — encodes `ProcessorThrottle`, `ThermalControl`, `SystemS3` (sleep support), and `LidPresent` into a single hex nibble. This is a common **anti-VM/anti-sandbox signal** — physical laptops report lid/battery/sleep-state capabilities that many sandboxes and desktop VMs do not.
- **HTTP POST fingerprint JSON** to `download.txtconverters.com/check_latest_version`; server response (base64) is decoded and unzipped **directly into the chosen install path** and later executed as `TXTconverter.exe` — i.e., the installer is a **thin stager whose payload is 100% determined server-side per victim**.
- **Zip-slip-aware extraction** (path is validated to stay under installPath — this specific check is implemented correctly, unlike many zip-slip-vulnerable droppers).
- **Process launch**: `LaunchAndMonitorApp` starts the extracted `TXTconverter.exe` and monitors exit code.
- **Persistence / install artifacts**:
  - Desktop shortcut: `%USERPROFILE%\Desktop\TXTconverter.lnk`
  - Start Menu shortcut: `%APPDATA%\Microsoft\Windows\Start Menu\Programs\TXTconverter\TXTconverter.lnk`
  - Uninstall registry key: `HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\TXTconverter` (DisplayName, DisplayVersion, Publisher, InstallLocation, UninstallString, DisplayIcon, NoModify)
  - Custom registry value: `HKLM\SOFTWARE\TXTconverter\MachineGuid` — writes/reads its own machine identifier, separate from Windows' native `Cryptography\MachineGuid`, likely used as a persistent per-victim tracking ID across reinstalls.
  - Shortcut creation uses `WScript.Shell` COM object (`Type.GetTypeFromProgID`) — flagged by malcat YARA as `Wscript` (lateral-movement category, low reliability here since it's legitimate `.lnk` creation, not script execution).
- **Telemetry/"finish" callback** to `api.txtconverters.com/finish` reporting install success/failure details — also usable by the operator to confirm real-machine detonation (as opposed to a sandbox that fails or aborts early).
- capa (static, dotnet) confirms: file/directory discovery, registry query/set/delete, HTTP send/receive, Base64 decode, zip extraction, process create/terminate/suspend-thread, unmanaged (P/Invoke) call — consistent with the above.

## 4. Attack Chain

1. **Delivery**: User downloads/runs `TXTconverterSetup.exe`, a legitimately EV-code-signed WPF installer masquerading as a "TXTconverter - PDF Utility Suite" product.
2. **Fingerprinting gate**: Before fetching any payload, the installer silently profiles the host (OS build, power/ACPI capabilities suggesting physical vs. VM hardware, whether app already installed) and POSTs it to `download.txtconverters.com/check_latest_version`.
3. **Server-side payload selection**: The server returns a base64 ZIP tailored to the fingerprint. Against ANY.RUN's sandbox, this branch delivered content that ANY.RUN's engine classified as **PureLogs Stealer**, consistent with a **PureCrypter**-style loader chain (PureCrypter is a widely-used MaaS loader/crypter frequently used to deliver commodity stealers).
4. **Deployment**: ZIP is extracted to `%LOCALAPPDATA%\Programs\TXTconverter\`, shortcuts and an uninstall registry entry are created (giving the install a legitimate-looking footprint), and the extracted `TXTconverter.exe` is launched.
5. **Callback**: Install outcome is POSTed to `api.txtconverters.com/finish`.

## 5. IOCs

### Network (defanged)
| Indicator | Type | Notes |
|---|---|---|
| `download[.]txtconverters[.]com` | domain | payload-fetch endpoint (fingerprint gate); DNS reputation 0 in this run, but functionally the malicious payload delivery point |
| `download[.]txtconverters[.]com/check_latest_version` | URL | POST target for machine-fingerprint JSON; returns base64 ZIP payload |
| `api[.]txtconverters[.]com` | domain | ANY.RUN DNS reputation: **2 (malicious)** |
| `api[.]txtconverters[.]com/finish` | URL | install-outcome telemetry callback; ANY.RUN reputation: **2 (malicious)** |
| `www[.]txtconverters[.]com/txt-convertor-terms` | URL | ToS lure page (legitimacy dressing) |
| `www[.]txtconverters[.]com/txt-convertor-pp` | URL | Privacy-policy lure page (legitimacy dressing) |

### Filesystem
- `%LOCALAPPDATA%\Programs\TXTconverter\` — default install path
- `%LOCALAPPDATA%\Programs\TXTconverter\TXTconverter.exe` — server-delivered payload (hash unknown/not captured — never touches disk on this installer binary; varies per detonation)
- `%LOCALAPPDATA%\Programs\TXTconverter\Uninstaller.exe`
- `C:\Users\admin\Desktop\TXTconverter.lnk` (SHA256 `577f546981cc38683f7fb00c99a540ecb6730d414cf63c933c334e159dbc2dfd`)
- `C:\Users\admin\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\TXTconverter\TXTconverter.lnk` (SHA256 `6ecbd60239270c75fd47e1037b1b11e7c82ec08eeb154240feed679f2d92195f`)

### Registry
- `HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\TXTconverter` — uninstall entry (DisplayName, DisplayVersion, Publisher, InstallLocation, UninstallString, DisplayIcon, NoModify)
- `HKLM\SOFTWARE\TXTconverter\MachineGuid` — operator-controlled per-victim ID, distinct from the OS `Cryptography\MachineGuid`

### Mutexes
- None identified (static analysis found no `CreateMutex` calls in decompiled code; not surfaced in ANY.RUN IOC report)

### Certificate
- Serial `75cfed98acf1d361fbff156b`, Subject "KALIM LIMITED", issued by GlobalSign GCC R45 EV CodeSigning CA 2020 — recently issued (2026-04-28), likely used to lend legitimacy/bypass SmartScreen for this specific campaign.

## 6. Emulation Results

- **Speakeasy**: Not applicable — `.NET assemblies are not currently supported` (raised `NotSupportedError` immediately on load). Speakeasy emulates native x86/x64 code and cannot execute managed CLR IL.
- **angr / custom hooks**: Not attempted — no native decrypt/decode routine was identified in decompiled code requiring concrete execution (the only "decryption" in the installer is a trivial `Convert.FromBase64String` of the server response, already reversed via ilspycmd decompile).
- **floss**: Explicitly unsupported for .NET binaries ("FLOSS does NOT attempt to deobfuscate any strings from .NET binaries") — skipped per tool's own warning.
- Full logic recovery was instead achieved via **ilspycmd decompilation** (`/home/remnux/mal/output/TXTconverterSetup_ilspy/TXTconverterSetup.decompiled.cs`), which is complete and unobfuscated — no packer/obfuscator was applied to the .NET IL.

## 7. Sandbox Results (ANY.RUN)

- **Task**: `7d359bb7-46a7-4ffe-8ffb-acd01c83cb75`
- **Verdict**: **Malicious activity**, score **100/100**
- **Flags**: `multiprocessing: true`, `networkThreats: true`, `knownThreat: true`
- **Tags**: `stealer`, `purelogs`, `purecrypter`
- **Public report**: https://app.any.run/tasks/7d359bb7-46a7-4ffe-8ffb-acd01c83cb75
- Network activity captured: DNS resolution + HTTPS traffic to both `txtconverters.com` subdomains, interleaved with legitimate Windows telemetry/OCSP/CRL noise (Microsoft WaaS assessment, OCSP checks for the installer's own EV cert chain — filtered out of the IOC table above).
- Dropped files observed in this run were limited to the two `.lnk` shortcuts; the server-delivered payload EXE itself was not separately hashed in the IOC report (ANY.RUN's family tagging is derived from the detonation's overall behavioral/network signature rather than a discrete artifact hash surfaced in this report).

## 8. Analyst Notes

- **Server-side gating is the core evasion mechanism here.** Because the payload is fetched fresh per install based on a submitted fingerprint (OS build, power/ACPI profile, whether app already exists), static analysis of the installer binary alone cannot recover the final payload — it must be observed via dynamic detonation. This analysis relied on ANY.RUN's live detonation to establish the "stealer/PureLogs/PureCrypter" classification; a re-run at a different time, from a different IP/ASN, or with a different fingerprint could plausibly receive a different (or entirely benign) response from the server.
- The `PowrProf.dll!GetPwrCapabilities` P/Invoke call is a lightweight, easily overlooked anti-sandbox signal — worth flagging to detection engineering as a fingerprint-tell for future triage (physical battery/lid/sleep-state presence vs. typical VM/sandbox profiles).
- `HKLM\SOFTWARE\TXTconverter\MachineGuid` (a custom, operator-defined registry value distinct from the OS's own `Cryptography\MachineGuid`) is worth pivoting on in EDR telemetry — it functions as a persistent, operator-issued victim ID that would survive uninstall/reinstall if the value isn't cleaned up.
- Recommended follow-up: capture and hash the actual `TXTconverter.exe` payload dropped during detonation (via ANY.RUN's dropped-files/process-dump artifacts) for a discrete IOC/second-stage analysis; pull the ANY.RUN PCAP for the raw base64 ZIP response body to confirm payload identity independently of ANY.RUN's own tagging.
- No cross-references to previously tracked families/campaigns in memory met the strict evidentiary bar (no matching cert serial, C2, config value, build artifact, or payload hash against tracked entries) — analyzed entirely on its own merits.
