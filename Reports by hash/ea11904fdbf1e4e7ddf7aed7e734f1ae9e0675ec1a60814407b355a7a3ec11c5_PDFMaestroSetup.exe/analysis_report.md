# PDFMaestroSetup.exe — Malware Analysis Report

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | PDFMaestroSetup.exe |
| SHA256 | `ea11904fdbf1e4e7ddf7aed7e734f1ae9e0675ec1a60814407b355a7a3ec11c5` |
| SHA1 | `4d3ac8b75a2d1531ad88450cdb4abaa733141ae5` |
| MD5 | `b440fb44874136f10660b7962a25b486` |
| File type | PE32 (GUI), .NET assembly (Mono/.NET), 3 sections, x86 (AnyCPU), large-address-aware |
| Size | 1,886,320 bytes |
| Compile timestamp | 2026-06-30 13:17:33 (PE header) |
| FileDescription / ProductName | "PDFMaestroSetup" / "PDFMaestro" |
| FileVersion / ProductVersion | 1.1.0.18 |
| Signing cert | **Subject:** Secure PC Software LLC, Delaware/Wilmington, US<br>**Issuer:** DigiCert Trusted G4 Code Signing RSA4096 SHA384 2021 CA1<br>**Serial:** `045745c545a5280deeead8eba7e8704f`<br>**Validity:** 2026-02-13 → 2029-02-12 |
| Embedded config (plaintext resource `PDFMaestroSetup.appSettings.json`) | `TenantId=1`, `ProductId=16`, `EnvironmentId=34`, `DownloadUrl=https://cdn.pdfmaestro.ai/main/1.1.0.18/PDFMaestro.zip`, `TelemetryUrl=https://events.pdfmaestro.ai/api/v1/events` |

## 2. Classification

**PPI (Pay-Per-Install) / affiliate-monetized software bundler — Medium-High confidence.**

This is not a novel malware family; it is a commercially-obfuscated **.NET installer/updater framework** ("PDFMaestro.InstallerUpdaterLib") built around a monetization pattern common to PPI/CPA install networks:

- Collects a machine fingerprint (OS, OS bit, language, .NET version, processor, **VM/laptop detection**) and affiliate-campaign parameters (`TrafficSource`, `Campaign`, `Adgroup`, `CampaignGeo`, `LpId`, `InstalledBrowser`) into a `TParams`/`MetaData` object.
- Phones home to a telemetry/config backend (`events.pdfmaestro.ai`, `config.pdfmaestro.ai`) and a **third-party PPI platform, `api.configtower.com/api/cfu`** ("cfu" = check-for-update), which is not disclosed anywhere in the product branding — this is the actual affiliate-network backend behind the "PDF Maestro" storefront.
- Gates a download URL behind an `isUpdateAvailable` server response before fetching/installing further components.
- Registers **two persistent Windows Scheduled Tasks** ("Launcher Task" recurring every 3 minutes, "Updater Task" one-shot) ~2 minutes after install — auto-start persistence with no user-facing toggle.
- Performs VM/sandbox detection via WMI (`Win32_Battery` chassis/battery-type codes, `Win32_ComputerSystem`-class queries) before deciding install behavior — a hallmark of PPI networks serving different payloads to analysts/sandboxes vs. real victims.
- The whole assembly is protected by a commercial .NET string-encryption/anti-tamper obfuscator (Eazfuscator.NET-style): all string literals resolve through a runtime decrypt-by-ID call (`_0006_0018._0005(int)`) backed by an embedded resource with a deliberately unprintable Unicode name, and the static constructor performs `StackTrace`/`Assembly.GetCallingAssembly()` checks to detect reflection-based extraction and alter its decryption key accordingly. This defeated static string recovery for most in-code literals; the network endpoints below were recovered from the plaintext config resource and from live sandbox network capture instead.

KesaKode (both offline local-DB and online-authoritative lookup) returned **zero matches** — no known-family code overlap, consistent with this being a bespoke/commissioned installer rather than a reused malware toolkit.

ANY.RUN's automated verdict is **100/100, "Malicious activity"**, driven specifically by `autoStart` (scheduled-task persistence), `multiprocessing`, and `debugOutput` heuristics plus MITRE-mapped signatures for Scheduled Task creation/abuse (T1053.005), Startup-directory file drop (T1547.001), Virtualization/Sandbox Evasion (T1497/T1497.003), and **Disables trace logs** (T1562.002) — i.e., the installer suppresses its own `Trace`/debug logging at runtime, an active anti-analysis behavior on top of the static anti-tamper protections.

No secondary payload was observed being fetched/executed during the 24-minute-window sandbox run — the observed traffic (see IOCs) resolved to the legitimate-looking `pdfmaestro.ai` marketing site (complete with Criteo ad-retargeting pixels firing off a `thank-you.html` conversion page), which is itself evidence this is CPA/affiliate-monetized software distribution rather than a directly destructive payload. Whether `cdn.pdfmaestro.ai/.../PDFMaestro.zip` or the `configtower` gate ever serves something more harmful is server-side and geo/campaign-gated — it was not observed in this run.

## 3. Capabilities

- HTTP(S) client communication (`HttpWebRequest`/`HttpClient`), with `CheckCertificateRevocationList` handling and configurable TLS/SSL protocol selection
- Machine/environment fingerprinting: computer name, OS version/bit/language, .NET Framework version, processor info, installed browsers, machine GUID, security-zone/IE settings
- **VM/sandbox detection** via WMI (`Win32_Battery` chassis codes `{8,9,10,11,14,21}`, secondary `ManagementObjectSearcher` query) and geolocation/location-settings checks
- Affiliate/campaign parameter capture and persistence to disk (`TrafficSource`, `Campaign`, `Adgroup`, `CampaignGeo`, `LpId`, `InstalledBrowser`) — used to compute an "IsOrganic" flag distinguishing direct vs. affiliate-driven installs
- Remote version/config check-in with conditional download-URL disclosure (`VersionInfoRequest`/`VersionInfoResponse`, `isUpdateAvailable`, `downloadUrl`)
- Zip download and extraction (`System.IO.Compression.FileSystem`), AES decrypt and Base64 encode/decode (capa: `decrypt data using AES`, `encode/decode Base64`)
- Reflection-based dynamic code/method generation and invocation (19+ capa matches — used both by the obfuscator's runtime and by legitimate WPF/XAML infrastructure)
- **Persistence:** creates two Windows Scheduled Tasks via Task Scheduler XML (`GenerateLauncherTask` — recurring every 3 min, first fire 2 min after install; `GenerateUpdaterTask` — one-shot, 2 min after install) and drops a file in the Startup directory
- Process creation/termination/enumeration, registry query/set/delete, directory create/delete, mutex creation
- Self-uninstall routine that finds and terminates related installer/uninstaller processes and deletes the install/component folder after a configurable delay
- Anti-tamper: reflection/calling-assembly detection in the string-decryption static constructor; runtime suppression of its own trace/debug logging (T1562.002)
- WPF/XAML-based GUI (install wizard: Welcome, Progress, Complete, Error, Uninstall variants) — presents as a conventional consumer installer UX

## 4. Attack Chain

1. User runs `PDFMaestroSetup.exe` (digitally signed, "Secure PC Software LLC" / DigiCert).
2. WPF installer UI launches; `ResourceEmbedderHelper` extracts embedded app-config/settings to a per-install working folder.
3. `MetaDataCollector`/`TParamsHelper` build a machine + campaign fingerprint (OS, VM check, affiliate params) and persist it locally.
4. `VersionInfoProcessor.CheckForUpdate()` POSTs the fingerprint to the telemetry/config backend; separately the sandbox observed check-in traffic to the third-party PPI platform `api.configtower.com/api/cfu`.
5. If the server signals `isUpdateAvailable`, a `downloadUrl` (derived from `cdn.pdfmaestro.ai`) is used to fetch and install the actual "PDF Maestro" application/zip.
6. `Executer.GetInstallerProcesses()` runs the full install-step pipeline, including `GenerateLauncherTask` and `GenerateUpdaterTask`, registering two Scheduled Tasks for persistent auto-launch/auto-update.
7. Marketing/conversion-tracking traffic (Criteo `widget.us.criteo.com`, `gum.criteo.com`, `dynamic.criteo.com`) fires against the `pdfmaestro.ai` storefront, consistent with a completed, monetized affiliate install.
8. On uninstall, `Executer.GetUnInstallerProcesses()` reverses steps (terminates processes, removes scheduled tasks/registry/shortcuts, deletes the component folder) via `ResourceEmbedderHelper.DeleteComponents()`.

## 5. IOCs

### Network — Domains
- pdfmaestro[.]ai
- cdn[.]pdfmaestro[.]ai
- events[.]pdfmaestro[.]ai
- config[.]pdfmaestro[.]ai
- pdfmastero[.]ai *(typo-variant, resolved during sandbox run)*
- api[.]configtower[.]com *(undisclosed third-party PPI/affiliate backend)*

### Network — URLs
- hxxps[://]cdn[.]pdfmaestro[.]ai/main/1[.]1[.]0[.]18/PDFMaestro[.]zip *(static config — gated payload/app download)*
- hxxps[://]events[.]pdfmaestro[.]ai/api/v1/events *(telemetry)*
- hxxps[://]config[.]pdfmaestro[.]ai/getconfig
- hxxps[://]api[.]configtower[.]com/api/cfu *(affiliate check-for-update gate)*
- hxxps[://]pdfmaestro[.]ai/ , /thank-you[.]html *(storefront + CPA conversion page)*
- Criteo ad-tech (conversion tracking, not malicious infrastructure): widget[.]us[.]criteo[.]com, gum[.]criteo[.]com, dynamic[.]criteo[.]com

*(Microsoft telemetry/CRL/OCSP and `login.live.com` traffic observed in the sandbox is standard Windows/Edge background noise, filtered from the list above.)*

### Filesystem / Persistence
- Windows Scheduled Task: "Launcher Task" — recurring every 3 minutes, first trigger 2 minutes post-install (`GenerateLauncherTask`/`TaskSchedulerHelper.CreateLauncherTask`)
- Windows Scheduled Task: "Updater Task" — one-shot, 2 minutes post-install (`GenerateUpdaterTask`/`TaskSchedulerHelper.CreateUpdaterTask`)
- File drop in the Startup directory (ANY.RUN T1547.001, filename not resolved from static analysis)
- 14 dropped files observed by ANY.RUN (component install payload; individual filenames not resolved — hashes retained in `PDFMaestroSetup_anyrun_iocs.json`)

### Certificate
- Serial `045745c545a5280deeead8eba7e8704f`, Subject "Secure PC Software LLC" (Delaware, US), issued by DigiCert Trusted G4 Code Signing RSA4096 SHA384 2021 CA1, valid 2026-02-13 to 2029-02-12. *(No match to any certificate serial in prior analyses.)*

## 6. Emulation Results

Native emulation (speakeasy/angr) was **not applicable**: this binary is pure .NET/CLR IL (architecture `DOTNET`, entry point is the standard `_CorExeMain` CLR stub). Speakeasy and angr operate on native x86/x64/ARM machine code, not managed IL, so no emulation pass was run. malcat's native decompiler (`fn_decompile`) likewise returned empty output for the same reason.

.NET decompilation was performed instead via `ilspycmd` (full project decompile) and `monodis --mresources` (used to recover and extract an embedded resource with a deliberately unprintable-Unicode name that malcat's anomaly detector flagged as `NonAsciiResourceName`/`BigResourceHighEntropy`). That resource was confirmed to be the obfuscator's own encrypted string table (consumed by the `_0006_0018` string-decryption class), **not** a secondary payload.

## 7. Sandbox Results

- **Verdict:** 100/100 — "Malicious activity"
- **Tags:** `auto-startup`
- **Environment:** Windows 10 Professional (build 19044, 64-bit)
- **Key MITRE ATT&CK signatures:** T1053.005 (Creates scheduled task from XML file / Uses Task Scheduler to run other applications — threat level 2), T1547.001 (Create files in the Startup directory — threat level 2), T1497/T1497.003 (Virtualization/Sandbox Evasion via Task Scheduler), T1614 (Process checks computer location settings — geofencing), T1562.002 (Disables trace logs), T1518/T1012 (Searches for installed software / registry discovery)
- **Public report:** https://app.any.run/tasks/0124bcb9-1531-48e4-a4e3-2bc60625b046

## 8. Analyst Notes

- The ANY.RUN "Malicious" verdict is driven by persistence-mechanism heuristics (scheduled tasks, startup-folder drop, self-log-suppression) rather than an observed destructive payload; no secondary executable beyond the advertised "PDF Maestro" app/zip was seen being fetched in this run. Classification here is **PPI/affiliate bundler with aggressive, undisclosed persistence and anti-analysis behavior**, not a confirmed backdoor/stealer family.
- The real monetization backend is `api.configtower.com`, invisible from the "PDF Maestro" branding/website — this is the actual mechanism by which install behavior could be varied per campaign/geo/traffic-source (`CampaignGeo`, `TrafficSource`, `LpId` params sent server-side). Because the gate is server-controlled, this sample's harmlessness in this one run does **not** rule out different behavior for other campaign IDs or victim profiles.
- Recommended follow-up: fetch `https://cdn.pdfmaestro.ai/main/1.1.0.18/PDFMaestro.zip` directly and `https://api.configtower.com/api/cfu` with varied `TParams`/geo values to check for payload variance; monitor `pdfmastero.ai` (typosquat-style variant) for phishing/lookalike use; the 14 dropped-file hashes captured by ANY.RUN (`PDFMaestroSetup_anyrun_iocs.json`) were not individually triaged and are worth a follow-up hash lookup.
- Per the strict cross-reference policy, no prior tracked sample in memory shares this certificate serial, C2 domains, config values, or build artifacts — analyzed entirely on its own merits.
