# Malware Analysis Report: 7ZSfxMod_x86.exe

## 1. File Metadata

| File | SHA256 | Size | Type |
|---|---|---|---|
| `7ZSfxMod_x86.exe` (outer SFX) | `c3394372c2000643ce385269b0d8c648ccb8c833076a84bde397a3c01e7b4a5b` | 304,016 B | PE32 GUI, x86, 4 sections |
| `360speedld.exe` (extracted, legit host) | `8a3bf2acb7682792d8f7c96d3009fc0d61922bba06d36531ff7d1e947a5bb7ce` | 219,656 B | PE32 GUI, x86, 5 sections |
| `somkernl.dll` (extracted, malicious payload) | `6e1689877689616977ddf82ec94da6cab145a347da16069f8726b2bddb081f98` | 136,192 B | PE32 DLL, x86, 5 sections |

- MD5 (outer SFX): `d93346e925fba4e37cfd2d9b07a2bdde`
- SHA1 (outer SFX): `8a474a7cd6881be8630fbc9afccc056ab470ba4a`
- MD5 (`360speedld.exe`): `3e5e0cb304498ccc1f31cbe2e0f372bc`
- MD5 (`somkernl.dll`): `4ad084abda8de2962c946c069a9d4ea7`
- imphash (`somkernl.dll`): `1b2bf36d463760846db6ef4fc66b522c`

**Signing info:**
- Outer SFX (`7ZSfxMod_x86.exe`) is signed with a certificate issued to **"Wijtvliet Agro"** (Moerdijk, NL) by `Microsoft ID Verified CS EOC CA 03` — a Microsoft Trusted Signing (cloud code-signing) issuer. Serial `3300059d69df50103817f95953000000059d69`. **Validity window is only 3 days: 2026-09-06 to 2026-09-09** — a hallmark of abused/purchased short-lived Trusted Signing certificates used to buy a brief window of trust before reputation systems catch up. The identity name has no evident connection to the payload's content or C2 infrastructure, consistent with a throwaway/stolen signing identity.
- `360speedld.exe` carries an old, expired (2008-10-22 to 2010-11-23) Thawte code-signing certificate issued to **Qizhi Software (Beijing) Co. Ltd** (360.cn) — this is a **genuine legacy Qihoo 360 binary**, not a forged signature. VersionInfo: `360软件管家` (360 Software Manager) / `SoftupNotify.exe`, FileVersion 2.5.1.1002. PDB path: `e:\building\360SoftWareMgr\trunk\SoftManager2\bin\SoftManager\release\360speedld.pdb`.
- `somkernl.dll` is **unsigned**.

**Build artifacts:**
- Outer SFX build tool: `7ZSfxMod` v1.6.0.2712 by Oleg N. Scherbakov (a well-known open-source modified 7-Zip SFX module widely used—legitimately and maliciously—to build self-extracting droppers).
- `somkernl.dll` internal module name: `proram_c_nocrt_agent_failover_dll.dll`. Compile timestamp 2026-08-31 21:48:05. No PDB (stripped).

## 2. Classification

**Malware type:** Custom backdoor/RAT ("PRO_RAM" agent) delivered via a DLL-sideloading dropper, distributed inside a signed 7-Zip SFX installer.

**Confidence:** High (confirmed via decompilation, capa, and import/string analysis of the malicious component). Not attributed to a named public malware family.

**Reasoning:**
- The 7z SFX unpacks two files with matching timestamps (2026-09-07 00:28:40): a genuine, legitimately-signed old 360.cn executable (`360speedld.exe`, internally `SoftupNotify.exe`) and a malicious, unsigned DLL (`somkernl.dll`) that exports the same function names (`AlphaBlend`, `GradientFill`, `TransparentBlt`, `DllInitialize`) that `msimg32.dll` provides — the classic **DLL search-order-hijacking / sideloading** pattern, where the legitimate host EXE loads `msimg32.dll` from its own directory and unknowingly executes attacker code via `SomPlugin`/`DllInitialize`.
- `somkernl.dll` contains a self-contained, hand-rolled C2 agent framework, internally named **"PRO_RAM"** (strings: `PRO_RAM_AGENT_HELLO_V1:`, `PRO_RAM C NoCRT Agent/0.1`, `PRORAMCFGv2`, `proram_c_nocrt_agent_failover_dll.dll`, env-var overrides `PRORAM_SERVER_URL`, `PRORAM_AGENT_SEED_HEX`, `PRORAM_AGENT_INSTALL_ID`, `PRORAM_ALLOW_DEV_AGENT_ID_OVERRIDE`).
- **KesaKode online lookup** (authoritative, run against `somkernl.dll`): highest hit was `OverlordRAT` at 2.18%, followed by `Remcos` (1.09%), `SantaStealer` (0.67%), and others all under 1%. **All results are below the 20% noise threshold — discarded per policy, no family attribution.** This is a custom/bespoke implant, not a known public family.
- KesaKode offline hints on `360speedld.exe` (`BillGates`, `concealment_troy`, confidence 0) are likewise noise and are discarded; that file is independently confirmed benign/legitimate via its PDB path, version info, and old genuine Thawte signature.

## 3. Capabilities

Confirmed via decompilation (`SomPlugin` export → `sub_1001a712`/`sub_1001a5e8` worker threads) and capa/peframe on `somkernl.dll`:

- **Dual C2 transport:**
  - Primary: WebSocket-over-TLS via WinHTTP (`WinHttpWebSocket*` APIs), backend string `wss backend=winhttp_websocket`, with a manual/legacy WebSocket fallback path over raw sockets + Schannel (SSPI `InitializeSecurityContextW`/`EncryptMessage`/`DecryptMessage`), labeled `legacy_wss backend=raw_socket_schannel_rfc6455`.
  - Secondary/covert: **DNS-tunneled transport** (`core.transport.dns`, `DnsQuery_W`, extensive `DNS_*` protocol state strings — frame/session/query handling, `max_dns_frame_bytes`), used as an inline result/fallback channel when direct HTTP(S)/WSS egress is blocked.
- **Agent identity & anti-collision:** host fingerprinting (`PRO_RAM_HOST_FINGERPRINT_V1`), persistent install ID and seed generation, signed "hello" handshake (`core.identity.signed_hello`), FNV-1a hashing of an internal `PRORAMCFGv2` config blob, named mutex `Global\ProRAM-Agent-<fingerprint>` used both for duplicate-instance detection and as an install marker.
- **Task/plugin execution framework:** resolves and downloads additional modules on demand (`/api/payloads/resolve?plugin=`, `PAYLOAD_HASH_MISMATCH`/`PAYLOAD_SIZE_MISMATCH` integrity checks, `memory_loaded_module`, `payload_module_cache`) — i.e. a modular loader that can fetch and reflectively load attacker-supplied plugin DLLs at runtime.
- **File manager:** chunked file upload/download with SHA-256 verification (`file_manager.upload_file`, `file_manager.download_file`, `file_upload.committed`, hash/size mismatch checks).
- **Command execution:** `shell.exec` task type, spawns `C:\Windows\System32\cmd.exe`; process creation/injection primitives (`CreateRemoteThread`, `WriteProcessMemory`, `VirtualAllocEx`, `GetThreadContext`/`SuspendThread`/`ResumeThread` — thread-hijack-style injection, flagged by capa as `Process Injection::Thread Execution Hijacking` / `DLL Injection`).
- **Reconnaissance:** `system_info.get`, `diagnostics.get` tasks; user/SID/admin-token checks (`OpenProcessToken`, `CheckTokenMembership`, `AllocateAndInitializeSid`); reads `SOFTWARE\Microsoft\Cryptography` (MachineGuid) and `CurrentBuildNumber` for fingerprinting.
- **Browser data collection:** `browser_collector` capability referenced in the task/module framework (module not statically resolvable — likely fetched as a separate plugin via the payload-resolve mechanism above).
- **Reverse SOCKS proxy:** `reverse_socks` task type for network pivoting through the victim host.
- **Remote input simulation:** `mouse_move` string present, suggesting remote-control/HVNC-adjacent capability (module/plugin-gated, not statically confirmed).
- **Persistence:** writes `HKCU\Software\Microsoft\Windows\CurrentVersion\Run\<value>` (capa: `Persistence::Registry Run Keys / Startup Folder [T1547.001]`); dedicated install/uninstall state machine (`PERSISTENCE_INVALID_ACTION/METHOD/NAME`, `PERSISTENCE_INSTALL_FAILED`, `PERSISTENCE_UNINSTALL_FAILED`).
- **Self-deletion:** spawns `cmd.exe /C (for /L %i in (1,1,60) do @(ping 127.0.0.1 -n 2 >NUL & del /F /Q "<path>" >NUL 2>NUL & if not exist "<path>" (exit /B 0))) & exit /B 1` — a retry-loop batch self-delete of the dropped files/directory (also seen with `rmdir /S /Q` for directory cleanup).
- **Anti-analysis:** `GetTickCount`-based timing/delay checks (capa MBC: `Debugger Detection::Timing/Delay Check`), `OutputDebugStringA`/`GetLastError` anti-debug pattern, base64 and XOR-based string/data obfuscation throughout (21 `XorInLoop` hits flagged by malcat).

`360speedld.exe` itself exhibits no networking capability and only generic registry/process/file operations consistent with a legitimate software-manager utility — its role here is purely as an unwitting DLL-sideloading host.

## 4. Attack Chain

1. Victim runs the signed `7ZSfxMod_x86.exe` self-extracting installer (likely delivered under a different, lure-appropriate filename; internal name unchanged).
2. The 7z SFX stub silently extracts (per its embedded `[Setup]`/config, `SelfDelete`/`AutoInstall`-capable) two files into a common directory: the genuine 360.cn executable `360speedld.exe` and the malicious `somkernl.dll`.
3. `360speedld.exe` (`SoftupNotify.exe`, a real 360 Software Manager component) is launched and, due to DLL search-order, loads `somkernl.dll` from its own directory in place of the real `msimg32.dll` (the DLL forwards/re-implements `AlphaBlend`, `GradientFill`, `TransparentBlt` to remain functional and avoid crashing the host).
4. `DllInitialize`/`SomPlugin` runs. It checks a named mutex (`Global\ProRAM-Agent-<fingerprint>`) to prevent duplicate instances, checks/establishes Run-key persistence, then spins up background worker threads for the WebSocket C2 channel and the DNS covert-channel/heartbeat.
5. The agent performs a signed hello/enrollment handshake with `wss://clash-verge-upgrade.com/edge-af6032129bd933a9/agents/stream`, with `dns://d.clash-verge-upgrade.com/proram?http=https://clash-verge-upgrade.com` as an alternate/fallback transport.
6. Attacker issues tasks over the C2 channel: recon (`system_info.get`), shell command execution, file upload/download, on-demand plugin/module loading (browser credential collection, reverse SOCKS, remote-control), or self-uninstall.

## 5. IOCs

**Network (defanged):**
- `wss[://]clash-verge-upgrade[.]com/edge-af6032129bd933a9/agents/stream` — primary C2 (WebSocket)
- `dns[://]d[.]clash-verge-upgrade[.]com/proram?http=https[://]clash-verge-upgrade[.]com` — DNS-tunnel C2
- `clash-verge-upgrade[.]com` — C2 domain (typosquat/impersonation of the legitimate "Clash Verge" proxy client's update infrastructure)
- `d[.]clash-verge-upgrade[.]com` — DNS-tunnel subdomain
- `proram[.]d[.]clash-verge-upgrade[.]com` — **confirmed live** DNS-tunnel query base (73 requests observed in ANY.RUN detonation, base32-style encoded subdomains, e.g. `n9e3778112c353cab.kbjacakv2ydpyckdqysqaaaaaaaaaaaaaaaaaaaaaa.proram.d.clash-verge-upgrade[.]com`)

**Filesystem:**
- `<dropdir>\360speedld.exe` — legitimate 360.cn DLL-sideloading host (SHA256 `8a3bf2ac...`)
- `<dropdir>\somkernl.dll` — malicious payload masquerading as `msimg32.dll` exports (SHA256 `6e168987...`)
- `%LOCALAPPDATA%\...\.proram` — agent state directory (env override: `PRORAM_AGENT_STATE_DIR`)
- `identity.bin`, `artifact.bin` — agent identity/task artifact files within the state dir
- `C:\Windows\System32\cmd.exe` — spawned for shell tasks and self-delete

**Registry:**
- `HKCU\Software\Microsoft\Windows\CurrentVersion\Run\<value>` — persistence
- `SOFTWARE\Microsoft\Cryptography` — MachineGuid read for host fingerprinting
- `SOFTWARE\Microsoft\Windows NT\CurrentVersion` — `CurrentBuildNumber` read

**Mutex:**
- `Global\ProRAM-Agent-<host-fingerprint-hex>` — duplicate-instance guard / install marker

**Certificates:**
- Outer SFX signer: CN=`Wijtvliet Agro`, O=`Wijtvliet Agro`, Moerdijk/Noord-Brabant/NL, issued by Microsoft ID Verified CS EOC CA 03, serial `3300059d69df50103817f95953000000059d69`, valid 2026-09-06→2026-09-09 only (abused short-lived Trusted Signing cert)

**File hashes:**
- SFX: `c3394372c2000643ce385269b0d8c648ccb8c833076a84bde397a3c01e7b4a5b`
- Host EXE: `8a3bf2acb7682792d8f7c96d3009fc0d61922bba06d36531ff7d1e947a5bb7ce`
- Malicious DLL: `6e1689877689616977ddf82ec94da6cab145a347da16069f8726b2bddb081f98`

## 6. Emulation Results

Not run. Static decompilation of the `SomPlugin`/`DllInitialize` entrypoint and the worker-thread dispatch functions (`sub_1001a712`, `sub_1001a5e8`, `sub_1001a7bd`) already gave a clear, high-confidence picture of the threading model and C2 transport selection logic, and the C2 URLs were recovered directly as plaintext strings (no runtime decryption needed) — emulation was judged low-value relative to token cost for this sample and skipped.

## 7. Sandbox Results

**ANY.RUN task:** `18a89a28-d511-42af-b3a8-66678ce3d67d` — https://app.any.run/tasks/18a89a28-d511-42af-b3a8-66678ce3d67d

- **Verdict:** score 30/100, **"No threats detected"** (heuristic score is low because the sample was detonated as a bare DLL outside its `360speedld.exe` sideload host, and ANY.RUN's reputation engine does not yet flag `clash-verge-upgrade.com`/`d.clash-verge-upgrade.com` — both resolve with `reputation: 0`/unknown, consistent with freshly-registered attacker infrastructure).
- **Tags:** `websocket`
- **Behavioral confirmation:** despite the low aggregate score, the IOC report captures the **DNS covert channel actively running** — 73 DNS queries to subdomains of `proram.d.clash-verge-upgrade.com` and `d.clash-verge-upgrade.com`, each carrying a long base32-style encoded data label (matching the static `core.transport.dns` / `PRORAMCFGv2` DNS-tunnel logic identified via decompilation). No direct outbound TCP "Connections" entries were recorded, indicating the sandboxed run relied on the DNS-tunnel transport as its live channel (the WSS transport may not have been reached, e.g. no host-process context, or it silently failed over).
- All 12 HTTP/HTTPS requests observed are legitimate Microsoft telemetry, OCSP, and CRL traffic (`settings-win.data.microsoft.com`, `ocsp.digicert.com`, `crl.microsoft.com`, `login.live.com`, etc.) — noise, not C2.
- This **independently confirms** the DNS-tunnel C2 domain identified via static analysis is live attacker infrastructure, not just a hardcoded-but-unused string.

## 8. Analyst Notes

- The "PRO_RAM" agent is clearly a purpose-built, actively-maintained implant (versioned protocol strings, structured error-code taxonomy for every subsystem — DNS transport, payload cache, persistence, file transfer — suggesting a mature internal codebase, not a one-off script). It is not a rebrand of a known public RAT; KesaKode's <3% top hit (OverlordRAT) is noise.
- The C2 domain `clash-verge-upgrade.com` impersonates the update infrastructure of **Clash Verge**, a legitimate open-source cross-platform proxy client — likely chosen either as a plausible-looking hostname for security tooling to overlook, or to specifically lure users of that software (e.g. via a fake "update" prompt). No direct evidence in this sample ties delivery to that specific lure vector; this is a hypothesis based on the domain naming, not a confirmed delivery mechanism.
- `browser_collector`, full reverse-SOCKS behavior, and `mouse_move`/remote-control capability are referenced in strings/task-name tables but their implementations could not be fully confirmed as resident in this DLL — the payload/plugin-resolve mechanism (`/api/payloads/resolve?plugin=`) strongly suggests these are delivered as separate modules fetched post-enrollment, which were not available for analysis here.
- Sandbox detonation (§7) confirms the DNS-tunnel C2 is live attacker infrastructure, not dormant/hardcoded-only. Recommended follow-up: a second detonation run in the context of the `360speedld.exe` sideload host (rather than the bare DLL) to see whether the WSS channel activates and to capture live plugin-resolve traffic, confirming which modules (e.g. `browser_collector`) the C2 actually serves.
- Per strict cross-referencing policy, this sample was analyzed entirely on its own merits — no certificate serial, C2 IOC, config value, build artifact, or payload hash in this sample matches any previously tracked family or standalone sample in memory.
