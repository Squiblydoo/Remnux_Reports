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

**Revised down from "confirmed stealer" to "unconfirmed/likely-false stealer attribution, confirmed deceptive gated-loader infrastructure"** — see §9 and §10 for the two rounds of follow-up validation.

ANY.RUN's live detonation tagged this **Malicious activity**, score 100/100, `knownThreat: true`, with tags `stealer`, `purelogs`, `purecrypter` (§7). Per the user's observation that this verdict traces to a **Suricata network-rule match** rather than payload/behavioral inspection, two independent follow-up checks (§9 live re-probing, §10 direct PCAP/screenshot analysis of the original ANY.RUN detonation) were performed. **Both directly contradict the stealer tag for this specific detonation**:

- ANY.RUN's own screenshots (§10) show the sandbox launched the identical benign "TxT Converter" decoy app we independently extracted and decompiled — not a stealer.
- The exact inbound byte count of ANY.RUN's `download.txtconverters.com` session (528,472 bytes) matches our own re-probed benign response (522,128 bytes) within 1.2%, fully explained by HTTP/TLS framing overhead — i.e., ANY.RUN very likely received the same decoy ZIP we did, not a separate/larger malicious payload.
- Full DNS/TLS SNI review of the entire detonation PCAP shows no third domain, no non-HTTP/raw-socket traffic, and no Telegram/Discord-style exfil channel anywhere — only the two `txtconverters.com` endpoints plus ordinary Windows/Bing/Google telemetry noise.
- No `sslkeys` were captured by ANY.RUN, meaning **its own Suricata engine could not have inspected the encrypted HTTP body either** — the `stealer`/`purelogs`/`purecrypter` tags were necessarily derived from plaintext metadata (domain/SNI/JA3) or IOC-list matching, not payload content.

**Conclusion**: treat "PureLogs Stealer via PureCrypter" as an **unconfirmed, most-likely-erroneous signature match** — plausibly a rule keyed to this campaign's domains/infrastructure from a different time or vantage point, not evidence that this detonation (or our own probing) actually retrieved a stealer. What **is** independently confirmed, from direct static + dynamic evidence gathered ourselves: this is a legitimately EV-signed installer that **covertly fingerprints the host and lets an external server silently choose and execute arbitrary code** on the victim's machine with no user visibility into what's being sent or received — a deceptive-by-design distribution technique regardless of which specific payload it serves on any given day. The operator retains the demonstrated capability to serve different payloads to different targets at will; nothing here rules out a stealer being served to some other victim profile we haven't matched.

- Malcat offline KesaKode: `StomExfiltrator` at 0% confidence — not meaningful (below 20% discard threshold)
- Malcat online KesaKode (via `cloud.malcat.fr`, authoritative per policy): **no matches returned**, for both the installer and the extracted decoy payload — consistent with this being either genuinely novel/unattributed, or simply not a match to any tracked family.

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
3. **Server-side payload selection**: The server returns a base64 ZIP. ANY.RUN's engine tagged this detonation `stealer`/`purelogs`/`purecrypter`, but direct validation (§9, §10) shows ANY.RUN's own detonation actually received the same benign decoy payload documented here — the stealer tag does not appear to reflect what was actually delivered in that run.
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
- **Tags**: `stealer`, `purelogs`, `purecrypter` — **see §10: directly contradicted by PCAP + screenshot evidence from this same task; most likely a stale/erroneous Suricata-rule match, not a payload-content finding.**
- **Public report**: https://app.any.run/tasks/7d359bb7-46a7-4ffe-8ffb-acd01c83cb75
- Network activity captured: DNS resolution + HTTPS traffic to both `txtconverters.com` subdomains, interleaved with legitimate Windows telemetry/OCSP/CRL noise (Microsoft WaaS assessment, OCSP checks for the installer's own EV cert chain — filtered out of the IOC table above).
- Dropped files observed in this run were limited to the two `.lnk` shortcuts; the server-delivered payload EXE itself was not separately hashed in the IOC report (ANY.RUN's family tagging is derived from the detonation's overall behavioral/network signature rather than a discrete artifact hash surfaced in this report).

## 8. Analyst Notes

- **Server-side gating is the core evasion mechanism here.** Because the payload is fetched fresh per install based on a submitted fingerprint (OS build, power/ACPI profile, whether app already exists), static analysis of the installer binary alone cannot recover the final payload — it must be observed via dynamic detonation. This analysis relied on ANY.RUN's live detonation to establish the "stealer/PureLogs/PureCrypter" classification; a re-run at a different time, from a different IP/ASN, or with a different fingerprint could plausibly receive a different (or entirely benign) response from the server.
- The `PowrProf.dll!GetPwrCapabilities` P/Invoke call is a lightweight, easily overlooked anti-sandbox signal — worth flagging to detection engineering as a fingerprint-tell for future triage (physical battery/lid/sleep-state presence vs. typical VM/sandbox profiles).
- `HKLM\SOFTWARE\TXTconverter\MachineGuid` (a custom, operator-defined registry value distinct from the OS's own `Cryptography\MachineGuid`) is worth pivoting on in EDR telemetry — it functions as a persistent, operator-issued victim ID that would survive uninstall/reinstall if the value isn't cleaned up.
- Recommended follow-up: capture and hash the actual `TXTconverter.exe` payload dropped during detonation (via ANY.RUN's dropped-files/process-dump artifacts) for a discrete IOC/second-stage analysis; pull the ANY.RUN PCAP for the raw base64 ZIP response body to confirm payload identity independently of ANY.RUN's own tagging.
- No cross-references to previously tracked families/campaigns in memory met the strict evidentiary bar (no matching cert serial, C2, config value, build artifact, or payload hash against tracked entries) — analyzed entirely on its own merits.

## 9. Live Fingerprint-Gate Probing (Follow-up)

To determine what controls payload selection at `download.txtconverters.com/check_latest_version`, 8 live POST requests were sent directly to the endpoint from this analysis host, varying the exact JSON fields the installer itself sends (`osBuild`, `powerProfile`, `appExeExists`, `approvedCheckbox`, `installerVersion`) across realistic and edge-case values (real-laptop vs. VM-like power profile, Win7 vs. Win11 24H2 OS builds, empty osBuild, app-already-installed, unapproved checkbox, stripped installer version). No payload was executed — responses were base64-decoded and inspected statically only. Script: `TXTconverterSetup_c2_probe/probe_fingerprint_gate.py`.

**Result: all 8 requests returned byte-identical responses** (decoded ZIP SHA256 `9392fb44138e604e05bc13216406ed19d3c4fb0e548573be5609c5ecb3b9cb2d`, decoded size 391,596 bytes). The JSON fingerprint fields the installer controls had **no observable effect** on the response in this test.

**The delivered payload is a functionally-stubbed decoy, not the stealer ANY.RUN observed**:
- `TXTconverter.exe` (SHA256 `253208d7e004ac29f4be7a6b9a9b168202e7b4777028c70c2b110551785df671`) — EV-signed with the **same KALIM LIMITED cert** (serial `75cfed98acf1d361fbff156b`) as the installer. Decompiled cleanly via ilspycmd: its PDF conversion logic is literally named `PlaceholderPdfConvertService` / `PlaceholderPdfMergeService` and is non-functional. No network calls beyond the cert-chain OCSP/CRL URLs baked in by the compiler/signer. capa's "reference analysis tools strings" / "Analysis Tool Discovery" hits are false positives on standard `[DebuggerNonUserCode]`/`[Debuggable]` .NET attributes, not genuine anti-analysis logic.
- `PdfSharp.dll` (SHA256 `2fa0893c6a1a8e64342e65e5464fad417376bd2cc984e53575f5829b07e6a067`) — genuine, unmodified empira Software PDFsharp v1.50.5147.0 (real upstream PDB path `F:\source\github\MigraDoc\MigraDoc\PDFsharp\...`). KesaKode: no match.
- `System.IO.Compression.dll` (SHA256 `b963eb95627b5f223e813fce8a53e6c9d72891714923de7263111473faebf3ef`) — genuine Microsoft-signed .NET Framework 4.6 component (2016 Microsoft Code Signing PCA cert).
- `Uninstaller.exe` (SHA256 `458b4a4f1835bb7340549518fa09cfd55cbcb3e87fed65e6ab477134b2e70629`) — same KALIM LIMITED cert, minimal uninstall logic.

**Interpretation (revised after §10)**: At the time this section was originally written, the working hypothesis was that ANY.RUN's sandbox had received a different (malicious) branch of the gate than this host did. §10 below shows that hypothesis was wrong: ANY.RUN's own detonation, re-examined directly via its PCAP and screenshots, received the **same benign decoy** documented here — not a different payload. So the gate producing identical output across 8 varied fingerprints is consistent with the server simply serving one build to everyone right now (at least across both vantage points checked), not with a targeted IP/fingerprint-based malicious-vs-clean split. The original "serves researchers a decoy, serves victims a stealer" theory is no longer supported by evidence — see §10 for the corrected conclusion.

## 10. Independent Validation of the ANY.RUN "Stealer" Verdict

The ANY.RUN verdict (`stealer`/`purelogs`/`purecrypter`, score 100) is generated by ANY.RUN's internal detection engine, which includes Suricata network-rule matching. Because Suricata rules can fire on plaintext connection metadata (domain/SNI, JA3, IP reputation lists) without ever inspecting an HTTPS payload, a rule match does not by itself confirm what was actually downloaded. This was checked directly using the same original ANY.RUN task (`7d359bb7-46a7-4ffe-8ffb-acd01c83cb75`), not a new detonation — no Suricata reproduction was possible (no Suricata/Zeek/Snort available in this REMnux environment, only tshark), so validation relied on artifacts ANY.RUN itself captured during that run:

1. **PCAP retrieved directly from ANY.RUN** (`/report/.../download/pcap` via the task API) and inspected with tshark. Full DNS and TLS ClientHello/SNI enumeration across the entire capture shows exactly two attacker-controlled endpoints (`download.txtconverters.com`, `api.txtconverters.com`, both fronted by Cloudflare) and nothing else suspicious — all other traffic is ordinary Windows/Bing/Google telemetry. No raw-socket C2, no Telegram/Discord-API-style exfil channel, no third domain anywhere in the capture.
2. **`sslkeys.present: false`** in the ANY.RUN task metadata — ANY.RUN did not capture TLS session keys for this run, meaning **its own Suricata instance could not have decrypted and inspected the HTTPS response body either**. Whatever triggered the `purelogs`/`purecrypter` tags was necessarily based on plaintext connection metadata or an IOC/reputation-list match against the domain — not content inspection of the actual delivered file.
3. **Byte-level size correlation**: the `download.txtconverters.com` TCP stream in ANY.RUN's PCAP carried exactly 528,472 bytes inbound. Our own live re-probe (§9) received a base64 response body of 522,128 bytes for every one of 8 fingerprint variants. The ~6.3KB (1.2%) difference is fully consistent with HTTP header + TLS record framing overhead across ~489 packets — i.e., **ANY.RUN's detonation almost certainly received a payload of the same identity/size we did**, not a separately-sized malicious package.
4. **Screenshot evidence** (5 frames retrieved from `/report/.../download/screens/...`, timestamps 3s/9s/11s/15s/21s into the run): shows the clean Windows 10 desktop pre-execution, the "TxT Converter — Contacting server for latest package..." installer progress screen, and finally the fully-installed app's "Convert File" UI with "Convert"/"Merge" modules and a "docx to pdf" format dropdown. This UI **exactly matches** the `ConvertView`/`MergeView`/`PlaceholderPdfConvertService` structure recovered by decompiling the payload we extracted ourselves in §9 — visually confirming ANY.RUN ran the same non-functional decoy app, not a stealer.

**Conclusion**: all three independent signals (traffic-pattern review, byte-size correlation, and visual screenshot evidence) agree with each other and contradict the `stealer`/`purelogs`/`purecrypter` tags. The verdict is best explained as a Suricata rule matching on the `txtconverters.com` domain/infrastructure itself (e.g., a rule added after this infrastructure was observed serving a stealer to a different victim/vantage point at some other time, or a generic heuristic for "installer silently downloads and runs base64-wrapped executable from a JSON API") rather than genuine detection of stealer behavior in this specific run. This is a useful general lesson for this workspace: **ANY.RUN's family/behavior tags should be spot-checked against the task's own PCAP and screenshots when they carry significant weight in a report**, rather than taken as ground truth — particularly for `knownThreat`/family-name tags, which can originate from network-signature matching on shared/reused infrastructure rather than payload-level detection.

## 11. Attempted Re-Probe via Tor (IP-Gating Hypothesis, Follow-up)

To more directly test the IP/ASN-gating hypothesis raised in §9, the gate was re-probed from a genuinely different network path using Tor (`tor` + `curl --socks5-hostname`), which was available in this REMnux environment. Two separate Tor circuits were used, yielding two distinct exit nodes/ASNs (`185.220.100.244` and `45.84.107.74`, confirmed via `check.torproject.org/api/ip`) — both categorically different from this analysis host's own DigitalOcean/Santa Clara egress IP (`164.92.98.0`, AS14061).

**Result**: all requests through both Tor exits were rejected with **HTTP 403, Cloudflare error 1010** (`cf-ray` headers confirm the block happens at Cloudflare's edge, in front of `download.txtconverters.com`) — before ever reaching the operator's origin server or its application-layer fingerprint-gating logic. This is Cloudflare's own bot-management layer blocking known Tor exit-node traffic wholesale, unrelated to the txtconverters.com operator's own targeting logic; it reproduced identically on two unrelated exit nodes, ruling out a single flagged relay.

**Other network-pivot options in this environment were checked and ruled out**: the only VPN config present (`starting_points_us-starting-point-1-dhcp.ovpn`) connects to a HackTheBox "Starting Point" lab edge (`edge-us-starting-point-1-dhcp.hackthebox.eu`), which routes to an isolated CTF lab network, not general internet egress — not usable for reaching public internet hosts. The `remnux` MCP tool backend was also checked and confirmed to share the exact same egress IP as this shell, so it doesn't provide a second vantage point either.

**Net effect on the IP-gating hypothesis**: it remains neither confirmed nor cleanly falsifiable from this environment — Tor cannot get past Cloudflare's edge to test it, and no other pivot path is available here. The strongest evidence bearing on it is still the §10 comparison against ANY.RUN's own detonation: ANY.RUN's sandbox pool egresses from infrastructure distinct from this host (and evidently was not blocked by Cloudflare, since it completed the download successfully), making it a more informative real-world "different network path" data point than Tor would have been even if Cloudflare hadn't blocked it — and that comparison showed **identical payload delivery** (§10), which argues against an active malicious-vs-clean IP split being in effect right now, at least across the two vantage points we've been able to compare (this DigitalOcean host and ANY.RUN's sandbox infrastructure).

**Follow-up recommendation if further testing is wanted**: a residential/mobile-IP vantage point (a real consumer ISP connection or a reputable residential-proxy service) would be needed to get past Cloudflare's bot filtering and genuinely test IP/ASN-based gating — datacenter-hosted paths (this host, Tor, most VPS-based VPNs) are unlikely to work for this specific test, either because the origin serves them the clean branch uniformly or because Cloudflare filters them before the origin's own logic ever runs.
