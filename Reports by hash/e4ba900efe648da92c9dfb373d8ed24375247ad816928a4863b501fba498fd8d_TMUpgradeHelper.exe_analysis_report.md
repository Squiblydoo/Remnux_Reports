# Malware Analysis Report: TMUpgradeHelper.exe

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | TMUpgradeHelper.exe |
| SHA256 | `e4ba900efe648da92c9dfb373d8ed24375247ad816928a4863b501fba498fd8d` |
| SHA1 | `ccbf18454fd0d0c02c54f529bb0c25fd44ad2b98` |
| MD5 | `d5bdd6261f62142a4bbbdec838a86f48` |
| File type | PE32 executable (GUI), Intel 80386, 5 sections |
| Size | 641,800 bytes |
| Imphash | `e87e69067e8ec0b60d35dba244831f42` |
| Compiler | Visual Studio 2022 (v17.3.4), MSVC linker, LTCG/POGO-optimized |
| Compile/link timestamp | 2026-05-08 14:15:13 |
| PDB path | `E:\Projects\TeleMate\v8.01\Build\bin\TMUpgradeHelper.pdb` |
| Code signing | **Valid** — Subject: `TELEMATE LLC` (Georgia, US); Issuer: Sectigo Public Code Signing CA EV R36; Serial: `7c9413c342cdb901c946942df0723ce6`; Validity: 2024-08-09 to 2027-08-09 |
| Overlay | 12,040 bytes — confirmed to be the PKCS#7 Authenticode signature block only (no appended payload) |

## 2. Classification

**Verdict: Benign — legitimate vendor software (TeleMate.Net / UC Analytics call-accounting suite upgrade utility). No malware indicators identified.**

Confidence: **high** (not "confirmed malicious" — this is a clean-verdict determination).

Reasoning:
- Valid, non-expired EV code-signing certificate matching the embedded PDB project path (`TeleMate\v8.01`) and internal build strings (`E:\Projects\TeleMate\Code\TMICoreDll\Path.cpp`, `TMUpgradeHelper.cpp`).
- All strings, registry paths (`Software\Telemate\TELEMATE.Net`, `SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\TeleMate Collector Pro_is1`), and referenced executables (`TelemateServiceManager.exe`, `RemoteCollectionEngineATL.exe`, `DirectCollectionEngineATL.exe`, `SchedulerEngineATL.exe`, `TMWebService.exe`) are internally consistent with a real upgrade/maintenance helper for the TeleMate.Net UC Analytics product line.
- No C2 domains, IPs, or suspicious URLs anywhere in the binary — the only URL strings present are the standard Sectigo/Comodo/USERTrust CRL and OCSP endpoints embedded in the Authenticode signature chain.
- Manually reviewed two of malcat's flagged "string decryption candidate" functions: one is a standard MSVC CPU-dispatch `memcpy`/`memmove` implementation; the other is a recursive file/directory deletion routine referencing the legitimate `TMICoreDll\Path.cpp` source path — both benign, compiler-generated or ordinary utility code.
- `capa`'s "create reverse shell (2 matches)" and MBC "Remote Access::Reverse Shell" flags are consistent with capa's known false-positive pattern for pipe-based child-process I/O redirection — this binary legitimately shells out to `ruby.exe` (`sql_version_check.rb`, ActiveRecord SQL Server adapter migrations) and `RegAsm.exe` (COM registration of `TMREFormatLib.dll`, `TMRERender.dll`), both requiring redirected stdout/stderr pipes.
- `peframe`'s "packer" and "antidbg" tags resolve to VC8/MSVC compiler-identification YARA hits and standard CRT anti-debug APIs (`IsDebuggerPresent`, `GetLastError`), not evasion logic.
- floss decoded-string extraction (13 hits) recovered only trivial substrings of already-visible legitimate strings (`TMProcess`, `TELEMATE.Net`, `TMWebService`) — no hidden secrets or C2.
- **Online KesaKode**: query executed successfully (verified functional against a control sample) and returned an **empty verdict** — no code-sharing overlap with any known malware family in the corpus. Offline KesaKode's `Avoslocker(confidence 1)` hint is noise-level (well below any usable threshold) and is discarded per the analysis policy.
- **ANY.RUN sandbox**: verdict score **0/100**, threat level **"No threats detected"**, zero behavior tags.

## 3. Capabilities

Functional behavior consistent with an installer/upgrade-maintenance helper for the TeleMate.Net suite:
- Enumerates and terminates running TeleMate/UC Analytics processes before upgrade (`WTSEnumerateProcesses`, `TerminateProcess`)
- Stops, starts, and reconfigures Windows services (`TelemateServiceManager`, `RemoteCollectionEngineATL`, `DirectCollectionEngineATL`, `SchedulerEngineATL`)
- Recursively deletes/moves/copies legacy install files and folders during upgrade cleanup
- Reads/writes registry keys under `Software\Telemate\*` and the standard Windows Uninstall key to track install state and versions
- Re-registers COM components via `regsvr32.exe` and `RegAsm.exe` (`TMREFormatLib.dll`, `TMRERender.dll`, `CAAlarm.dll`, `CACost.dll`, `CAControl.dll`, `CAMerge.dll`, `CVM.dll`, `TMProcessps.dll`, `TMActiveCtrl.ocx`)
- Invokes a bundled Ruby 2.3.0 interpreter with ActiveRecord `sqlserver-adapter` gem to run SQL Server schema/version migration checks against `MainData2.sql`
- Logs to `%s.log` with configurable `log.level` / `log.days.to.keep` settings

## 4. Attack Chain

Not applicable — this is a legitimate maintenance utility, not a malicious dropper or stager. No multi-stage payload, no network beaconing, no persistence mechanism beyond the product's own registered Windows services (which is expected/by-design for this software category).

## 5. IOCs

**Network**: None identified. The only URLs present are Sectigo/Comodo/USERTrust CRL/OCSP endpoints from the Authenticode chain, and ANY.RUN's sandbox network capture showed only standard Windows OS telemetry/CRL traffic (all `reputation: 0` / clean) — none attributable to this binary's own logic. No IOCs are reported.

**Filesystem** (legitimate product paths, not indicators of compromise):
- `bin\RemoteCollectionEngineATL.exe`, `bin\DirectCollectionEngineATL.exe`, `bin\SchedulerEngineATL.exe`, `bin\TMProcess.exe`, `bin\CAControl.dll`, `bin\CACost.dll`, `bin\CAMerge.dll`, `bin\CVM.dll`, `bin\CAReformat.dll`

**Registry** (legitimate product paths):
- `Software\Telemate\TELEMATE.Net`, `Software\Telemate\Telemate.Net`, `SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\TeleMate Collector Pro_is1`

**Mutexes**: None identified (only `WaitForSingleObject` API reference, no named mutex object created).

## 6. Emulation Results

- Speakeasy generic runner (x86) executed successfully but only reached CRT/API-set initialization (69 `LoadLibraryExW`/`GetProcAddress` resolution events) before stalling — typical for a GUI application whose real logic is gated behind UI/argument-driven entry points that speakeasy's headless harness doesn't drive. No network, file, registry, or mutex API calls were captured during the reachable execution window.
- No further emulation passes (angr/custom hooks) were pursued — no encrypted/obfuscated routine was identified that would justify deeper targeted emulation.

## 7. Sandbox Results (ANY.RUN)

- **Verdict score**: 0 / 100
- **Threat level**: "No threats detected"
- **Behavior tags**: none
- **IOC report**: 14 total entries, all `reputation: 0`; categories limited to `Main object` and `HTTP/HTTPS requests` (all Microsoft telemetry/CRL/OCSP domains — `settings-win.data.microsoft.com`, `login.live.com`, `crl.microsoft.com`, `ocsp.digicert.com`); no DNS or Connections entries beyond OS baseline noise.
- **Public report**: https://app.any.run/tasks/616b304b-0e7e-4de9-8dcc-c6f3efc9402e

## 8. Analyst Notes

- This sample was submitted with a fresh timestamp (added to the working directory 2026-07-28, same day as analysis) but shows no behavioral or static overlap with any tracked family or campaign in memory. Per the cross-referencing policy, no prior-sample links apply (no matching cert serial, C2/network IOC, config value, build artifact, or payload hash against tracked entries).
- The valid, unexpired EV certificate issued to TELEMATE LLC is internally consistent with the product identity throughout the binary (PDB path, source paths, product/service names) — no signs of a stolen/misused certificate or authenticode stuffing.
- Residual gap: emulation did not reach the application's core upgrade logic (UI-gated), so dynamic *file-system/registry* behavior was only confirmed via capa/decompilation, not runtime capture. Given the convergent evidence from ANY.RUN (0/100, no tags), static review, and string/config analysis, this is not considered a material gap.
- Recommendation: no further action required. If this binary is encountered again in a different context (e.g., different signer, different distribution channel, or bundled with an unrelated dropper), re-analyze rather than assuming benign based on this report — verdicts are tied to this exact hash/signing state.
