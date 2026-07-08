# PlaySndSrv.dll — Analysis Report

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | PlaySndSrv.dll |
| SHA256 | `f10468a027953d15f8321d40f470307b1a62d38cffbaca4d94fb8837c8df5210` |
| SHA1 | `29aeb6ceefc597494e27faf0f982da7895c17c2a` |
| MD5 | `1e44b8a2ad69aedd3e4a0592108e2d26` |
| Size | 420,504 bytes |
| Type | PE32+ DLL, x86-64, 7 sections |
| Imphash | `4ebd680771eb00baa147f57da60038df` |
| Compiler | MSVC 2022 (v17.14.2) |
| PE timestamp / PDOGO/ILTCG debug dates | 2026-06-30 11:20:24 |
| Entry point | 0x25a14 |

**Signing:** Signed with a valid GlobalSign GCC R45 CodeSigning CA 2020 certificate, subject **"SSM TECH"** (Navi Mumbai, Maharashtra, IN; email `ssmtechofficial@gmail.com`; serial `1020f6b4114c9072aed37a1d`), valid 2026-05-06 → 2027-05-07. This is a legitimately-issued certificate for an unrelated small Indian entity — it is not a Microsoft certificate and has no relationship to the file's claimed identity.

**Identity spoofing (VersionInfo vs. reality):**
- `CompanyName`: "Microsoft Corporation", `FileDescription`: "PlaySound Service", `ProductName`: "Microsoft® Windows® Operating System", `LegalCopyright`: "© Microsoft Corporation. All rights reserved." — all fabricated.
- **Export table module name is `profapi.dll`**, not `PlaySndSrv.dll` — a linker-level mismatch showing the PE was built/renamed from a different project.
- Real exports: `DllCanUnloadNow`, `DllGetClassObject`, `PlaySoundServerInitialize`, `PlaySoundServerTerminate` (the latter two are genuine Windows `PlaySndSrv.dll` export names, copied to make the proxy transparent — see §4).

## 2. Classification

**Type:** DLL-sideloading proxy loader carrying a license-gated game-cheat ("aimbot") payload.

**Confidence:** High (static + partial dynamic confirmation of behavior). **No malware family attribution** — this is not a match to any tracked stealer/backdoor family:
- Offline KesaKode: best hit `OctalynStealer` confidence 8 (out of unnormalized raw counts) — hypothesis-only, per policy.
- Online KesaKode (authoritative): highest score **BlackByte 3.27%**, OctalynStealer 2.91%, PandaBanker 1.45%, StxRAT 1.45%, BazarBackdoor 0.36%, VMZeus 0.36% — **all below the 20% discard threshold**. No family overlap or attribution supported.
- capa/peframe: no packer identified; ATT&CK techniques limited to Indicator Removal from Tools (stack-string obfuscation), Discovery (file/process/software/system info), Shared Modules (COM DLL forwarding).

This is best characterized as a **HackTool / PUA-class game cheat loader** that uses malware-grade tradecraft (deceptive signing, process/binary masquerading, DLL search-order sideloading, dynamically-constructed C2 host, license-gated remote activation) rather than a data-theft or destructive payload.

## 3. Capabilities

- **DLL proxying / sideloading masquerade**: on `DLL_PROCESS_ATTACH`, resolves and loads the real `PackageServices.dll` (first from its own directory, then `%SystemRoot%\System32\`) and forwards `DllCanUnloadNow`, `DllGetClassObject`, `PlaySoundServerInitialize`, `PlaySoundServerTerminate` to it via `GetProcAddress`, so a legitimate host process continues to function normally while the malicious code runs in the background — classic transparent DLL-proxy sideloading.
- **Host-process fingerprint check**: decodes a XOR-obfuscated (key `0x55`) string to `"taskhostw.exe"` and compares it (case-insensitively) against `GetModuleFileNameA` of the running process — i.e. the payload is built to be sideloaded specifically into (a copy of) the legitimate Windows **`taskhostw.exe`** binary.
- **Runtime import resolution**: resolves ~40 API pointers (including several `api-ms-win-core-*` API-set forwarders) via hashed/decoded names + `GetProcAddress`, avoiding a static import table for those functions (anti-static-analysis).
- **Stack-string / argument obfuscation**: capa flags 3 stack-string constructions; the HTTPS host used for license verification is built this way and was not recoverable from static strings.
- **License/HWID verification beacon**: builds a hardware-fingerprint-like value, sends it as an HTTPS (port 443) GET via WinHTTP to a dynamically-constructed host, reads back a response, and gates further execution on "Verification OK"/"Verification FAILED" (`FreeLibraryAndExitThread` on failure). This is a remote-gated activation/licensing check-in, not a data-exfiltration channel — but it does notify an external, attacker-controlled server of every install.
- **Hotkey-driven cheat menu**: a polling loop (`Sleep(70ms)`) checks `GetAsyncKeyState` for F2–F9 and Home, logging `"F2 pressed"` … `"F9 pressed"`, `"HOME "` and dispatching to distinct handler routines per key — a classic external game-cheat trainer hotkey menu.
- **Aimbot / memory-patch routine**: enumerates the target process's loaded modules (`CreateToolhelp32Snapshot`, `Process32NextW`, `Module32NextW`), reads 4-byte values at fixed offsets from each module base (via a `ReadProcessMemory`-style wrapper), pattern-matches them against sentinel values, and on match writes/reads further associated fields before logging `"Aimbot applied successfully!"`. This is consistent with an external cheat locating known offsets inside a specific (unidentified) game module before patching aim/vision-related memory. The exact target game/module could not be determined from static analysis alone.
- **Anti-debug / discovery**: `IsDebuggerPresent`, `NtQuerySystemInformation`, `RtlAdjustPrivilege`, `NtDuplicateObject`, `GetSystemInfo`, process/module enumeration — used both for the aimbot's process-finding logic and generic environment fingerprinting.
- **Local artifacts referenced**: `PackageServices.dll` (proxy target, expected already present on the host or dropped alongside), `_CharaLightIntensity` (game-engine rendering parameter string, suggests a specific title's cheat-menu feature set).

## 4. Attack Chain

1. `PlaySndSrv.dll` is dropped alongside (or in the search path of) a copy of the legitimate **`taskhostw.exe`**, exploiting DLL search-order / sideloading so that launching `taskhostw.exe` loads this DLL instead of (or in addition to) any real `PlaySndSrv.dll`.
2. `DLL_PROCESS_ATTACH` fires: `DisableThreadLibraryCalls`, then the DLL locates and loads the real `PackageServices.dll` and wires up forwarding pointers for its four exports, keeping the host process outwardly unaffected.
3. A background thread is spawned that sleeps 3s, verifies the host process is `taskhostw.exe`, performs the HTTPS license/HWID check-in, and — if the server responds affirmatively — spawns two further threads: one polling F2–F9/Home hotkeys, and one continuously enumerating processes/modules to locate and patch the aimbot target's memory.
4. All genuine DLL exports (`PlaySoundServerInitialize`/`Terminate`) remain functional via the proxy, so the sideload is not visibly disruptive to the host.

## 5. IOCs

**Network:**
- No genuine C2 domain/IP could be recovered. The HTTPS host used for the license check-in is built via runtime stack-string construction (capa: `Executable Code Obfuscation::Stack Strings`) and was not resolved by static string analysis, floss, or emulation (see §6). This is a **gap**, not a negative finding — treat any outbound HTTPS traffic from a `taskhostw.exe` process on port 443 as suspicious in this context.
- Only certificate-infrastructure URLs present statically (all legitimate CA/CRL/OCSP endpoints — not IOCs): `secure.globalsign[.]com`, `crl.globalsign[.]com`, `ocsp.globalsign[.]com`, `www.microsoft[.]com/pkiops/*`.

**Filesystem:**
- `PackageServices.dll` — legitimate DLL this sample proxies to (checked in its own directory, then `%SystemRoot%\System32\PackageServices.dll`).
- Expected host binary: `taskhostw.exe` (fingerprint-checked by basename).

**Certificate:**
- GlobalSign GCC R45 CodeSigning CA 2020 → **"SSM TECH"**, Navi Mumbai, Maharashtra, IN, `ssmtechofficial@gmail.com`, serial `1020f6b4114c9072aed37a1d`, valid 2026-05-06 to 2027-05-07.

**Registry / mutex:** none identified statically or dynamically.

## 6. Emulation Results

Speakeasy (x64, DLL entry) was used with the shared hook library (`/home/remnux/mal/speakeasy_lib/hooks.py`), extended for this sample:
- Added a **CRT compatibility shim** for `kernel32.FlsGetValue2` (unimplemented in speakeasy's kernel32 model; without a stub, MSVC2022 CRT init in `DLL_PROCESS_ATTACH` crashed with `UC_ERR_READ_UNMAPPED` before any payload thread could run).
- Added **diagnostic hooks** for `LoadLibraryEx*`, `GetProcAddress`, and `CreateThread` to trace dynamic API resolution and thread scheduling (both are now part of `register_all_hooks`).

With these fixes, emulation progressed further and confirmed real behavior:
- `LoadLibraryExA("C:\Windows\system32\PackageServices.dll", ...)` attempted twice (own-directory then System32) — both returned `NULL` in the emulated filesystem (expected; the emulator has no `PackageServices.dll`), consistent with the proxy-loader logic in §3.
- A genuine payload thread was scheduled and began executing (CRT per-thread FLS init, `IsProcessorFeaturePresent("PF_FASTFAIL_AVAILABLE")`), but hit an **unhandled `int 0x29` (`__fastfail`)** shortly after — an emulator-fidelity limitation in FLS/TLS thread-init handling, not a sample self-destruct. Execution did not reach the WinHTTP license-check call before this crash.
- No network IOCs were recoverable via emulation as a result.

## 7. Sandbox Results (ANY.RUN)

- Task: `fd3972a8-92ad-490e-b1d1-dcb5b1aa9916`
- Verdict: **score 15/100 — "No threats detected"**, no behavioral tags assigned.
- IOC report contained only Windows OS telemetry/CRL noise (`settings-win.data.microsoft.com`, `login.live.com`, Microsoft CRL/OCSP endpoints) — none attributable to this sample's own payload.
- **Interpretation**: consistent with the static/emulation findings — the sample's default sandbox execution (loaded via the sandbox's generic DLL-execution harness, not inside an actual `taskhostw.exe`) very likely failed the `taskhostw.exe` process-name gate identified in §3, so the license check-in / hotkey / aimbot threads never activated. This explains the clean verdict despite the concrete cheat-loader logic found statically.
- Public URL: `https://app.any.run/tasks/fd3972a8-92ad-490e-b1d1-dcb5b1aa9916`

## 8. Analyst Notes

- **This sample is best understood as a commercial/underground game-cheat ("aimbot") loader**, not a conventional infostealer, backdoor, or ransomware. Its malicious/unwanted classification rests on its delivery tradecraft: fake Microsoft branding, mismatched export module name, DLL-proxy sideloading into a legitimate system binary (`taskhostw.exe`), and a real (though unrecovered) external license-server check-in — all of which are abusive regardless of the "aimbot" payload's specific target.
- **Primary residual gap**: the license-check HTTPS host. It is constructed via runtime stack-string logic that neither static analysis, floss, nor emulation recovered, and the ANY.RUN sandbox run did not trigger the code path that builds/uses it (see §7). Recommended follow-up: re-submit to a sandbox configured to execute the DLL specifically as `taskhostw.exe` (rename/rundll32 host-name spoof) to force the gate to pass and observe the real check-in traffic; or use angr/manual tracing of the stack-string construction routine (candidate function chain: `sub_18001c7c0`/`sub_1800193a0` build path feeding `WinHttpConnect`) with a concrete input to resolve the host string.
- **Target game unidentified**: the memory-scan sentinel values (0x1024/0x2048/0x8192 at fixed module-relative offsets) and the string `_CharaLightIntensity` suggest a specific game engine/title, but this could not be pinned down from static analysis alone.
- No cross-references to previously analyzed samples in this workspace met the strict matching bar (identical cert serial, C2, config value, build artifact, or payload hash) — this sample is analyzed entirely on its own merits.
