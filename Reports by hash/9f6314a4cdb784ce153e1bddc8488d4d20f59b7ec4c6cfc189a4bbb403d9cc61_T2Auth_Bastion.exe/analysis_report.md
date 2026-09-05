# Analysis Report: T2Auth_Bastion.exe

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | T2Auth_Bastion.exe |
| SHA256 | `9f6314a4cdb784ce153e1bddc8488d4d20f59b7ec4c6cfc189a4bbb403d9cc61` |
| SHA1 | `11e5e45e15feed9a8f850b58473c6d9be17547b0` |
| MD5 | `5287450b091c6d65f9e69094c934c881` |
| File type | PE32 executable (GUI), Intel 80386, 7 sections |
| Size | 9,724,984 bytes (~9.27 MiB); overlay accounts for ~5.13 MiB of that |
| Compiler | MSVC (Visual Studio 2022, v17.14.2), linked with `/GUARD:CF` (Control Flow Guard) |
| Compile/PE timestamp | 2026-07-21 14:57:09 |
| PDB path | `D:\ReleaseAI\win\Release\stubs\x86\ExternalUi.pdb` |
| Imphash | `d61098bb34ea41207b7b575f9f5f033b` |

**Signing:** Authenticode-signed, valid chain.
- Issuer: `Certum Code Signing 2021 CA` (Asseco Data Systems S.A., Poland)
- Subject: `PROGRAMVARE PARTNER ANS` (Manger, Norway)
- Serial: `184af3d177711eac2c281de26e44f41b`
- Validity: 2026-08-24 → 2027-08-24
- Algorithm: SHA1/RSA

**VersionInfo:**
- ProductName / InternalName: `T2Auth Bastion`
- FileDescription: `T2Auth Bastion Installer`
- Comments: `T2Auth Bastion (Evaluation Installer)`
- CompanyName: `Mykhacode`
- FileVersion / ProductVersion: `12.2.5`

**Build artifacts:** The `stubs\x86\ExternalUi.pdb` path, the string table (`AI_BOOTSTRAPPER*`, `AI_INST_PRODCODE`, `WinUiBootstrapperEui.App`, `Software\Caphyon\Setups`, `instname-target.msi`, `majorupgrade-content.mst`) unambiguously identify this as a **Caphyon Advanced Installer** bootstrapper/EUI stub — a commercial, widely-used third-party Windows installer-authoring toolkit. This is the generic bootstrapper shell that Advanced Installer generates for *any* customer product; it is not unique to this sample and is seen in both legitimate and (rarely) abused installers.

## 1.5 Certificate Signer OSINT

Researched `PROGRAMVARE PARTNER ANS` (the Authenticode signer) against the Norwegian company registry (Brønnøysundregisteret) and open web:

- **Real, long-standing legal entity**: Org. number `984637519`, registered 2002-06-28 (24 years old), organization form ANS (general partnership, joint/several liability), industry code 46.500 ("Wholesale of information and communication technology equipment"), stated purpose includes "developing and selling data programs, selling data equipment and supplies, web design and related products." Status: active, not in bankruptcy/liquidation. Address: Lie, 5936 Manger — matches the certificate's `Locality=Manger` field exactly. Registered contact: `post@programvarepartner.no`, phone `56 34 70 23`.
- **Ownership**: Sole active participant/managing director (`Daglig leder`) and board chair is **Aron Berg**. A second original partner, **Matthew Gordon Hauge**, is recorded as deceased and deregistered — this is effectively a one-person operation today.
- **Web presence is essentially nonexistent**: `www.programvarepartner.no` resolves and returns HTTP 200, but is a bare, content-free **GoDaddy "Launching Soon" template page** (copyright 2026) — no products, no portfolio, no software listed, no mention of "T2Auth Bastion" or "Mykhacode" anywhere. Its TLS certificate doesn't even match the domain (serves a generic `*.secureserversites.net` cert), consistent with a parked/never-fully-configured site. A Yelp listing exists (category: "Computers") but with no further detail.
- **"T2Auth Bastion" and "Mykhacode" have zero independent internet footprint.** Targeted web searches for both terms — together and separately — returned no product page, no download mirror, no company site, no GitHub repo, no review/PUP-tracking site, nothing. For a product versioned `12.2.5` (implying a multi-year release history if genuine), a total absence of any corroborating trace anywhere online is conspicuous.

**Does this make sense for this signer?** No. A 24-year-old, one-person, small Norwegian ICT-wholesale/web-design partnership with an unfinished placeholder website is not a plausible publisher of a versioned commercial "T2Auth Bastion" authentication/bastion product that itself has no discoverable identity anywhere on the internet. This pattern — a real-but-dormant small foreign business entity holding a valid Certum code-signing certificate, signing software for a brand with no independent existence — is a known signature of **certificate-of-convenience abuse**: threat actors and PPI/bundler operators obtaining code-signing certs through small, easy-to-register (or purchasable/rentable) shell-like businesses to get commodity/private-label installers past SmartScreen and AV signature heuristics, rather than the certificate genuinely belonging to the software's actual publisher. This OSINT finding **raises** the suspicion level versus the binary-only static/dynamic review: it does not itself prove malicious intent, but it removes the main reason (a named, plausible national publisher) to lean toward "legitimate installer," and should be weighed accordingly.

## 2. Classification

**Verdict: Suspicious-but-unconfirmed — technically clean in every reachable code path, but the publisher identity does not hold up.** No malicious code, C2, or confirmed-bad behavior was found in the binary or the observed sandbox run (ANY.RUN's 100/Malicious score is driven by generic installer heuristics, not confirmed malicious behavior — see below). However, certificate-signer OSINT (§1.5) found that "T2Auth Bastion"/"Mykhacode" have no independent internet footprint at all, and the signing entity is a real but dormant one-person Norwegian shell-like partnership with no visible software business — a pattern consistent with certificate-of-convenience abuse rather than a genuine software publisher. Net effect: the *code* looks like a stock installer with nothing malicious reachable, but the *identity behind it* does not check out, so this should not be treated as cleared.

Reasoning:
- The binary is a stock Advanced Installer bootstrapper (`stubs\x86\ExternalUi.pdb`) wrapping a product called "T2Auth Bastion" (v12.2.5) by "Mykhacode" — sounds like a legitimate authentication/PAM ("bastion") product, consistent with the branding, version scheme, and EULA/UI resource templates present.
- It is Authenticode-signed with a valid chain from a real public CA (Certum/Asseco), issued to a named Norwegian company (`PROGRAMVARE PARTNER ANS`). No certificate abuse indicators (no serial/company overlap with any tracked campaign in memory).
- Static analysis found no hardcoded C2 domains/IPs, no credential-harvesting code, no ransomware/wiper logic, and no evidence of an embedded second-stage backdoor in the reachable (non-overlay) code.
- Dynamic sandbox execution (ANY.RUN) shows exactly the behavior expected of an Advanced Installer bootstrapper: check/install Microsoft Edge WebView2 Runtime (downloaded from genuine Microsoft CDN endpoints), then stage `T2Auth Bastion.msi` under `%APPDATA%\Mykhacode\` for the Windows Installer service to consume. **No non-Microsoft network destinations were contacted** — every URL observed (`login.live.com`, `settings-win.data.microsoft.com`, `msedge.api.cdp.microsoft.com`, `config.edge.skype.com`, OCSP/CRL endpoints, `www.bing.com`) is standard Windows/Edge OS telemetry generated by the sandbox VM itself, not sample-driven C2.
- peframe's "keylogger"/"screenshot"/"disable antivirus"/"antisb" behavior tags are generic string/API-pattern heuristics; manual review of the one relevant import (`GetAsyncKeyState`, called once) and the surrounding decompiled code shows normal installer-UI logic (dialog cancel-key/keyboard-focus handling), not a logging loop. No evidence supports an actual keylogger.
- ANY.RUN's `Malicious/100` score reduces to generic `specs` flags — `autoStart`, `multiprocessing`, `serviceLauncher`, `debugOutput` — which are true of essentially any MSI-based installer that spawns msiexec/EdgeWebView2 sub-processes and registers uninstall/RunOnce keys. The only descriptive tags returned were `advancedinstaller` and `auto-reg` (no trojan/stealer/backdoor/ransomware family tag).
- The 5.13 MiB overlay (entropy ~224/255, no recognizable file magic) is almost certainly Advanced Installer's proprietary compressed/obfuscated payload container holding the embedded MSI + resources; it could not be carved with standard tools (Binary Refinery has no Advanced-Installer-specific unpacker) and was not further reverse engineered given the scope/cost tradeoff — the actual MSI (`T2Auth Bastion.msi`) was recovered instead via dynamic execution (see §5/§6).
- **KesaKode online:** no matches (empty result set — effectively 0% across all tracked families). Offline KesaKode also returned an empty verdict list. No code-sharing signal with any known malware family.

**capa findings — verified, not confirmatory of malice.** capa (9.3.1) matched 112 rules on this binary, including several alarming-sounding ones: `log keystrokes` / `log keystrokes via polling`, `parse credit card information`, `check for unmoving mouse cursor`, `reference anti-VM strings targeting Xen`, `schedule task via ITaskService`, `download and write a file`, `receive and write data from server to client`. Each was manually decompiled and checked against its actual code:
- **"log keystrokes via polling"** (VA `0x575EA0`) — a `ShellExecuteExW` wrapper that loops `EnumWindows`+`GetAsyncKeyState(VK_MENU)`+`SendInput` to synthesize a dummy Alt keypress. This is the well-known "force SetForegroundWindow" idiom Windows apps use to legally steal foreground focus for an elevated child window — a single synthetic key send, not a keylogging loop. capa's heuristic pattern-matches the `GetAsyncKeyState`-in-a-loop shape and false-positives on it.
- **"parse credit card information"** (VA `0x453070`, `0x5DC110`) — generic MSVC `<regex>` parser internals (tokenizing `(`, `)`, `[`, `]`, `{`, `}`, `\`, `.`, `*`, `+`, `?`, `^`, `$`), consistent with the `regex_error(...)` strings and the `linked against CPP regex library` capa match elsewhere. Not a hardcoded credit-card regex or exfil path.
- **"check for unmoving mouse cursor"** (VA `0x542180`) — a custom-control WndProc handling `WM_MOUSEMOVE`/`WM_MOUSELEAVE`/`TrackMouseEvent`/`GetCursorPos`/`PtInRect`/`RedrawWindow` — ordinary UI hover-highlight logic for an installer button/link control, not sandbox-evasion mouse-movement polling.
- **"reference anti-VM strings targeting Xen"** — a single `/^Xen/i` regex hit at file offset 6712145, inside the high-entropy (~224/255) encrypted overlay; the surrounding bytes are non-printable binary noise (`...xen@Yt\x08|...`) — a coincidental byte match in ciphertext, not a real "Xen" string or hypervisor-detection routine.
- **"download and write a file" / "receive and write data from server to client"** (VA `0x5B34E0`) — a generic WinInet download loop (`InternetQueryDataAvailable`→`InternetReadFile`→`WriteFile`→`MoveFileW`/`CopyFileW`) matching the `DownloaderApiUsage` anomaly and the ANY.RUN-observed WebView2 Runtime download — the standard AI prerequisite-fetcher, not a C2 payload channel (no non-Microsoft URLs were ever supplied to it in the sandbox run).
- **"schedule task via ITaskService"** (VA `0x61B860`, the single highest-scored "interesting function") — a fully generic, parameterized COM Task Scheduler wrapper (`ITaskService`→`GetFolder`→`ITaskDefinition`→`IExecAction`, executable path passed in as a function argument, not hardcoded). This matches Advanced Installer's built-in "run a scheduled task after install" project option; the actual task name/target would be defined by the AI project XML shipped inside the encrypted overlay, not visible in the stub itself.

None of these seven checked findings survive verification as genuine malicious functionality — they are stock MSVC/Advanced-Installer/Win32-UI idioms plus one coincidental byte match in encrypted filler data. This is included in detail specifically because capa's raw rule names, taken at face value, would otherwise read as a stealer/keylogger/anti-analysis profile; the decompiled evidence does not support that reading.

This does **not** rule out that "T2Auth Bastion" could be a bundler/PPI (pay-per-install) offer or unwanted software wrapped around a legitimate-sounding name — that class of product cannot be fully excluded from the bootstrapper alone, since the actual application logic ships inside the encrypted overlay/MSI that was not fully installed in the sandbox run (it stalled at ~52s, mid-WebView2 dependency install). Treat as **suspicious-but-unconfirmed** rather than clean, and re-review if the same publisher/cert/product resurfaces with different behavior.

## 3. Capabilities (as observed)

- Advanced Installer bootstrapper/EUI: prerequisite detection & installation (Microsoft Edge WebView2 Runtime), UAC elevation, MSI staging and invocation via `msiexec.exe`.
- Runs `powershell.exe -NonInteractive -NoLogo -ExecutionPolicy Unrestricted -WindowStyle Hidden -Command ...` to execute a script and capture its output to a temp file — standard Advanced Installer mechanism for `Add-AppxPackage`/`Remove-AppxPackage`/`Add-AppProvisionedPackage` custom actions (used for optional AppX/MSIX component installs), not an arbitrary payload dropper based on decompiled logic.
- Writes a `Software\Microsoft\Windows\CurrentVersion\RunOnce` value pointing at a PowerShell one-liner to complete deferred/elevated post-install steps across a reboot — a documented Advanced Installer bootstrapper feature, not a persistence backdoor per se, though the mechanism is generically reusable for persistence and should be revisited if the encrypted overlay is ever decoded.
- Registry read/write across `Uninstall`, `Policies\System`, `Internet Explorer\BROWSER_EMULATION`, `Explorer\Settings`, `ProductOptions`, `Caphyon\Setups` — all standard installer bookkeeping.
- Downloads files over WinInet/FTP APIs (delay-imported) — used for prerequisite retrieval (WebView2 runtime), consistent with observed sandbox traffic.
- Process enumeration (`Process32First/NextW`), `IsDebuggerPresent`/`OutputDebugStringW` — CRT/AI runtime housekeeping and standard anti-tamper checks bundled in the AI stub, not confirmed to gate malicious behavior.
- Drops `T2Auth Bastion.msi` to `%APPDATA%\Mykhacode\T2Auth Bastion 12.2.5\install\<random>\` for installation by the Windows Installer service.

## 4. Attack Chain (as far as observed)

1. `T2Auth_Bastion.exe` launches → Advanced Installer WinUI bootstrapper UI appears.
2. Checks/downloads/installs Microsoft Edge WebView2 Runtime (legitimate Microsoft CDN, `msedge.api.cdp.microsoft.com`).
3. Extracts embedded, encrypted overlay payload → stages `T2Auth Bastion.msi` under `%APPDATA%\Mykhacode\...\install\9281889\`.
4. (Not reached in the 52-second sandbox window) Hands off to `msiexec.exe` to install the actual product; PowerShell/RunOnce mechanisms available for any deferred AppX/elevated steps.

The sandbox run ended before the MSI installation phase completed/the final application executed, so the installed product's own runtime behavior (what "T2Auth Bastion" actually does once installed) is **not directly observed** — this is the primary analytical gap.

## 5. IOCs

### Network
All observed destinations are legitimate Microsoft/Windows infrastructure generated by the sandbox OS and WebView2 update flow — **no malicious network IOCs identified**:
- `login[.]live[.]com` (Windows account/telemetry)
- `settings-win[.]data[.]microsoft[.]com`
- `msedge[.]api[.]cdp[.]microsoft[.]com`
- `config[.]edge[.]skype[.]com`
- `go[.]microsoft[.]com`
- `www[.]bing[.]com`
- `crl[.]microsoft[.]com`, `www[.]microsoft[.]com/pkiops`, `ocsp[.]digicert[.]com`, `oneocsp[.]microsoft[.]com` (CRL/OCSP)

### Filesystem
- `C:\Users\admin\AppData\Roaming\Mykhacode\T2Auth Bastion 12.2.5\install\9281889\T2Auth Bastion.msi` (SHA256 `b9015cd63d7c9030078a7a89b2a13d630006032e704c807c122ff94893861f74` — not independently verified, length appears truncated/malformed in sandbox report)
- `C:\Windows\Installer\SourceHash{921BC8A2-1616-439C-8564-996839281889}`
- `C:\Users\admin\AppData\Local\Microsoft\EdgeWebView\Temp\source8064_583619199\msedge_7z.data`
- Assorted `C:\Windows\Temp\~DF*.TMP` (standard MSI/temp scratch files)

### Registry
- `HKCU\Software\Microsoft\Windows\CurrentVersion\RunOnce`
- `HKLM\...\CurrentVersion\Uninstall\`
- `HKCU\Software\Caphyon\Setups`
- `HKLM\SOFTWARE\Policies\System`

### Mutexes
None identified.

### Certificate
- Serial `184af3d177711eac2c281de26e44f41b`, issued by Certum Code Signing 2021 CA to `PROGRAMVARE PARTNER ANS` (Norway) — no overlap with any previously tracked campaign/family in analyst memory.

## 6. Emulation Results

- **Speakeasy (x86, generic runner):** Loaded successfully but only reached CRT/API-set startup (`api-ms-win-core-synch`, `api-ms-win-core-fibers` resolution, initial `VirtualProtect` calls on the CFG guard table) before the emulator's API coverage was exhausted. No application-level logic, strings, or IOCs were reached — expected for a large, GUI-heavy, multi-megabyte installer stub rather than shellcode/a packer. Not pursued further (angr/custom hooks) given the definitive dynamic-sandbox result already obtained.
- No packer or dedicated anti-analysis protector was present; the CFG-driven `ImportByHash`/`DelayImports` anomalies are standard MSVC/Advanced-Installer artifacts, not deliberate unpacking obstruction.

## 7. Sandbox Results (ANY.RUN)

- Task: `d5c8a348-b918-417a-b560-fee786d01ef8`
- Verdict: **100/100, "Malicious activity"** (heuristic score — see caveats in §2)
- Tags: `advancedinstaller`, `auto-reg`
- Behavioral specs flagged: `autoStart`, `debugOutput`, `multiprocessing`, `serviceLauncher` (all consistent with a normal MSI-based installer, not confirmed-malicious behavior)
- Observed activity: installer UI → WebView2 Runtime download/install (genuine Microsoft endpoints) → MSI staged to `%APPDATA%\Mykhacode\...` — run ended (screenshots stop ~52s) before the MSI installation phase or the final application launched
- No DNS/Connection-category IOCs recorded (only HTTP/HTTPS to Microsoft-owned hosts)
- Public URL: https://app.any.run/tasks/d5c8a348-b918-417a-b560-fee786d01ef8

## 8. Analyst Notes

- **Primary gap:** the actual "T2Auth Bastion" application (inside the MSI/encrypted overlay) never executed in the observed sandbox window, so its real runtime behavior — the thing that would confirm or refute malicious intent — is unverified. If further conclusions are needed, either (a) let the sandbox run to completion / extend the sandbox timeout, or (b) invest in reverse-engineering Advanced Installer's proprietary overlay container format to recover the embedded MSI statically.
- capa took ~13 minutes to complete on this 9.7 MB binary (200 hash-resolved imports slow its feature matching significantly) but did finish; all 112 matched rules were reviewed, and the handful of alarming-sounding ones were manually decompiled and debunked (see §2). Full capa JSON retained at `T2Auth_Bastion_capa.json`.
- Alternative hypothesis: "T2Auth Bastion" could be a PPI/bundler-style offer using a legitimate-sounding security-product name (a pattern seen elsewhere in this workspace's general threat landscape, though **not** linked here to any specific tracked campaign per the strict cross-reference policy — no matching cert serial, C2, config value, build artifact, or payload hash was found). Recommend re-analysis if a completed installation / the standalone installed application binary becomes available.
- No cross-references to previously analyzed samples in memory met the required bar (identical cert serial, matching IOC, matching config/build artifact, or identical payload hash) — analyzed entirely on its own merits.
