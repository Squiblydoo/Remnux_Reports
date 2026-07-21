# SAC_tool.exe — Malware Analysis Report

> **Revision note**: this report was substantially revised after a second ANY.RUN detonation. SAC_tool.exe contains a self-check that requires its own process filename to contain a specific string; the first submission (original filename) hit this gate and produced a false-clean "No threats detected" verdict. Resubmitting under the filename `image20260714569#sac.exe` (contains `sac`, satisfying the gate) produced a full detonation with a 100/100 malicious verdict, confirmed C2 traffic, and an ANY.RUN vendor attribution to **APT-Q-27**. All findings below reflect the corrected, full picture. §7 documents both runs for transparency.

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | SAC_tool.exe |
| SHA256 | `9c5b840e50cd2672f803b9ac8ad8285a01bf0a6292b65d9fe471af4d1a5e5384` |
| SHA1 | `572ca22c6aaadf83f0d60987d6bb4df15370d3e1` |
| MD5 | `8638fe00c832429fe8f52a3f7d0f4327` |
| Size | 61,752 bytes |
| Type | PE32 executable (GUI), .NET (Mono/.NET assembly), Intel 80386, 3 sections + overlay |
| Imphash | `f34d5f2d4577ed6d9ceec516c1f5a744` |
| Compile/PE timestamp | 2024-01-18 13:31:55 |
| Target framework | .NET Framework 4.8 (`net48`), WinExe, Prefer32Bit |
| Entry point | `DotNetEntryPoint` @ 0x10c4 |

**Signing**: Signed with a **Certum Extended Validation Code Signing 2021 CA** certificate.
- Subject: `杭州思维宇宙科技有限公司` (Hangzhou Siwei Universe Technology Co., Ltd.), Zhejiang, CN
- Serial: `5df273a440e188cfd64188d1ef1e5931`
- Validity: 2026-04-27 → 2027-03-03
- The PE overlay (25,912 bytes) is the Authenticode PKCS#7 `SignedData` blob itself — not a hidden secondary payload.

**Version-info / build metadata mismatch**: `VersionInfo`/`AssemblyInfo` claim `CompanyName: Amazon.com`, `ProductName: SAC tool`, `LegalCopyright: Copyright © Amazon.com 2024` — while the signing certificate belongs to an unrelated Chinese company. Brand-spoofing masquerade paired with an unrelated signing identity.

## 2. Classification

**Confidence: Confirmed — APT-Q-27 / ZhongStealer family (loader/downloader + backdoor stage), new "nikeupdat" wave.** (Upgraded from "High" after §7.5 payload decryption recovered an exact-match legacy C2 string, `e.nkking.com`, embedded in the core/loader DLL — matching the strict cross-reference bar independent of ANY.RUN's vendor tag.)

This supersedes the initial static-only assessment. Confirmed via dynamic detonation (§7):
- **ANY.RUN's own proprietary Suricata signature fired**: `BACKDOOR [ANY.RUN] APT-Q-27 related HTTP activity` (SID 85006522, priority 1) against the C2 connection — this is vendor threat-intel attribution, not pattern-matching against our own prior samples.
- ANY.RUN behavioral tags: `apt-q-27`, `backdoor`, `websocket`, `auto-reg` (the `auto-reg` tag matches the family's documented WebSocket REGISTER-frame C2 protocol).
- **C2 domain `host.keensie.com`** (resolves to `35.78.126.246:3133`, AWS/AMAZON-02, reputation malicious) shares the apex domain with the previously-documented family C2 rotation `api.keensie.com` (see `family_apt_q27_zhongstealer.md` fingerprint #4) — a different subdomain, not a byte-identical match, but combined with the vendor signature this is treated as confirmatory rather than coincidental.
- The WebSocket handshake (`GET http://host.keensie.com:3133/\`, HTTP 101 Switching Protocols) uses a malformed/non-standard request path, consistent with the family's documented non-standard WS handshake behavior.

**What does NOT match prior waves** (noted for completeness — this appears to be a TTP evolution, not a reused toolkit): no `(byte+0x77)^0x62` decrypt / LZNT1 Delphi-core pattern was recovered from the three opaque downloaded payloads; no LENOVO cert serial; no `084049` PDB path; KesaKode (online, authoritative, and offline) returned zero code-sharing matches against the corpus. The C2-facing component in this wave is a **legitimate, unmodified, correctly-signed Tencent TBS SDK executable (`minibrowser.exe`)** rather than a custom shellcode-loader DLL — no anomalous modules were loaded into its process, so this is not classic DLL side-loading; the malicious behavior is most likely driven by one of the co-downloaded data files being consumed as a config/update-channel input by `minibrowser.exe`'s own legitimate remote-update logic (mechanism not fully reversed — see §8).

## 3. Capabilities

Recovered from full ILSpy decompilation (`/home/remnux/mal/output/SAC_tool_decompiled/`), capa, and confirmed/extended by dynamic detonation:

- **Execution gate**: refuses to run its main logic unless its own process filename (minus extension) contains a specific substring (confirmed to include, at minimum, the substring `sac`, case-insensitive or matched as a fragment — satisfied by both `SAC_tool` and `image20260714569#sac`); otherwise shows a MessageBox and exits.
- **Anti-sandbox / anti-VM gate**: ~10 checks (VM process names, MAC OUI prefixes, disk size <40GB, VM driver files, Sandboxie-class env vars, sandbox username/machine-name markers, WMI BIOS/ComputerSystem manufacturer checks) — did not visibly trip in the successful run, so remains only partially characterized.
- **Manifest-based downloader**: fetches `https://storage.googleapis.com/nikeupdat/ls.txt` (**confirmed dynamically** — this is the real manifest URL), a 5-line plaintext list of absolute URLs, and downloads each to a hidden folder: **confirmed path** `C:\Users\admin\AppData\Local\Microsoft\SACtool\PCManager\Update\temp_<yyyyMMdd>_<4-char>@<year>\`.
- **Conditional auto-execution** by extension: `.exe` → launched directly; `.ps1`/`.vbs` → interpreter-launched; image extensions → shell-opened (decoy display); no-match extensions (the `.uqv`/`.gtk` files) are downloaded but not auto-launched by SAC_tool.exe itself.
- **Persistence**: writes `HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run` value `miniUpdate` = path to the downloaded `minibrowser.exe` — Run-key autorun persistence, confirmed dynamically (fired twice, once per launch).
- **C2 component** (`minibrowser.exe`, launched by SAC_tool.exe): creates a mutex with non-ASCII/non-standard bytes in its name, opens a raw TCP connection to `35.78.126.246:3133` and performs an HTTP `GET .../\ ` → `101 Switching Protocols` WebSocket upgrade — i.e. establishes a live WebSocket C2 channel, matching the family's documented backdoor protocol.
- TLS 1.2-only `HttpClient` with a custom `ServerCertificateCustomValidationCallback` that accepts all certificates.
- capa MBC/ATT&CK: `Deobfuscate/Decode Files or Information [T1140]`, `Hide Artifacts::Hidden Window [T1564.003]`, discovery techniques, `Windows Management Instrumentation [T1047]`. Dynamic run adds `T1547.001` (Registry Run Key persistence) and backdoor/C2 behavior.

## 4. Attack Chain (confirmed via dynamic detonation, task `30e95295-c35b-46dd-9282-a59e06f381c6`)

1. Victim runs `SAC_tool.exe` (filename gate must be satisfied — likely why the actor named the delivered sample to include "sac" as a substring, or relies on it not being renamed by casual sandboxes/analysts).
2. Tool GETs `https://storage.googleapis.com/nikeupdat/ls.txt` (200 OK, 265 bytes) — the manifest.
3. Downloads all 5 listed files concurrently into `%LOCALAPPDATA%\Microsoft\SACtool\PCManager\Update\temp_20260721_7637@2026\`:
   - `oihtq.uqv` (350,208B, sha256 `b30886bf461f3d27c7d83bf1678c1fc4fe9ca1b709caf26b624d14f7f8b2ec61`) — opaque, not auto-launched
   - `ousctr.gtk` (433,576B, sha256 `68d29c03dbe279669e7ec6e9ac5aff72002442a9db2c74f8c1b9beb909e438c2`) — opaque, not auto-launched
   - `vcnfq.uqv` (340,992B, sha256 `8791799132966c34f547c31f496e927ccf9580b5e4bfac295772cf86386b4bc6`) — opaque, not auto-launched
   - `minibrowser.exe` (368,288B, sha256 `26fba07c17efbb6c48a2e746e42df1ee26405c6aa557039492553e5bc27598a1`, exact match to a previously analyzed genuine Tencent TBS SDK component) — **launched immediately**
   - `image.jpg` (11,343B, sha256 `0ce9b137f378211a4f6ba43bae5e7056d577d757441671028b94b46a05b2b0c1`) — decoy, opened with default viewer
4. Writes `HKCU\...\Run\miniUpdate` = path to the downloaded `minibrowser.exe` (persistence).
5. `minibrowser.exe` runs from the SACtool temp directory, creates a non-standard-named mutex, and opens a WebSocket connection to `host.keensie.com:3133` (`35.78.126.246`) — flagged by ANY.RUN's own signature set as APT-Q-27 backdoor traffic.
6. On a second launch (triggered by the freshly-written Run key, pid 6180, parent Explorer), the same mutex-creation and Run-key-write behavior repeats, confirming persistence works as intended.
7. Net effect: victim sees a browser/PC-manager-style app open (decoy), while a live WebSocket C2 channel is established in the background and persistence is installed; three unidentified payloads sit on disk for a purpose not fully determined (see §8).

## 5. IOCs

**Network**
- `hxxps[://]storage[.]googleapis[.]com/nikeupdat/ls[.]txt` — manifest (confirmed)
- `hxxps[://]storage[.]googleapis[.]com/nikeupdat/oihtq[.]uqv`
- `hxxps[://]storage[.]googleapis[.]com/nikeupdat/ousctr[.]gtk`
- `hxxps[://]storage[.]googleapis[.]com/nikeupdat/vcnfq[.]uqv`
- `hxxps[://]storage[.]googleapis[.]com/nikeupdat/minibrowser[.]exe`
- `hxxps[://]storage[.]googleapis[.]com/nikeupdat/image[.]jpg`
- **C2**: `host[.]keensie[.]com` → `35[.]78[.]126[.]246:3133/TCP` (reputation: malicious; ANY.RUN signature `BACKDOOR APT-Q-27 related HTTP activity`); WebSocket upgrade path `/\ ` (malformed/non-standard)

**Filesystem**
- `C:\Users\admin\AppData\Local\Microsoft\SACtool\PCManager\Update\temp_<yyyyMMdd>_<4-char-hex>@<year>\` (hidden staging folder; all downloaded files placed here, hidden attribute set)
- `C:\Users\<user>\AppData\Local\Temp\<original filename>` (drop location observed for the sample itself)

**Registry**
- `HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run` value `miniUpdate` = `...\temp_<...>\minibrowser.exe` (persistence)

**Embedded (legacy) IOC**
- `e.nkking.com` — literal string found in the decrypted `vcnfq.uqv` core/loader DLL; exact match to the C2 domain used by the previously-tracked `Samp.exe` sample (`Samp.exe:12940`, Feb 2025 build). Likely a vestigial/legacy string carried across builds rather than the currently active C2 (which is `host.keensie.com:3133` per dynamic observation) — worth monitoring if traffic to it is ever seen.

**Mutex**
- Non-ASCII/non-standard-character mutex name (raw: contains bytes `56 89 15 B0 34 12 87 FE AB 49` interspersed with non-printable/high Unicode code points), created by `minibrowser.exe` at C2-connect time

**Certificates**
- Code-signing cert serial `5df273a440e188cfd64188d1ef1e5931`, subject 杭州思维宇宙科技有限公司, Certum EV Code Signing 2021 CA

**Hashes**
- SAC_tool.exe: `9c5b840e50cd2672f803b9ac8ad8285a01bf0a6292b65d9fe471af4d1a5e5384`
- Downloaded `minibrowser.exe` (C2/backdoor component in this wave; exact-hash match to prior standalone analysis): `26fba07c17efbb6c48a2e746e42df1ee26405c6aa557039492553e5bc27598a1`
- `oihtq.uqv`: `b30886bf461f3d27c7d83bf1678c1fc4fe9ca1b709caf26b624d14f7f8b2ec61`
- `ousctr.gtk`: `68d29c03dbe279669e7ec6e9ac5aff72002442a9db2c74f8c1b9beb909e438c2`
- `vcnfq.uqv`: `8791799132966c34f547c31f496e927ccf9580b5e4bfac295772cf86386b4bc6`
- `image.jpg`: `0ce9b137f378211a4f6ba43bae5e7056d577d757441671028b94b46a05b2b0c1`
- `ls.txt` manifest: MD5 `0176812039e3e0e6d2b5e45d3e1fe30a`

## 6. Emulation Results

Not applicable — pure .NET assembly, no native code path for speakeasy/angr to target. FLOSS does not support .NET string deobfuscation (confirmed via direct run). The custom resource-backed string-encryption routine remains undefeated statically (StackTrace-based anti-tamper caller check; would require de4dot or IL patching, not installed on this host) — however dynamic detonation recovered the operationally relevant strings (manifest URL, download path, C2) directly from behavior, making further static string-decryption work lower priority.

## 7. Sandbox Results

**Run 1 — original filename** (`SAC_tool.exe`): task `35c57d69-047f-41cb-b9e6-684a69f9c89c`, verdict **0/100, "No threats detected"**, no tags, only Microsoft telemetry traffic observed. https://app.any.run/tasks/35c57d69-047f-41cb-b9e6-684a69f9c89c — **false negative**, caused by the sample's own filename-content execution gate not being satisfied by ANY.RUN's stored filename.

**Run 2 — renamed to `image20260714569#sac.exe`** (satisfies the filename gate): task `30e95295-c35b-46dd-9282-a59e06f381c6`, verdict **100/100, "Malicious activity"**, tags `apt-q-27`, `backdoor`, `websocket`, `auto-reg`. Full manifest download, `minibrowser.exe` launch, Run-key persistence, and WebSocket C2 connection to `host.keensie.com:3133` all observed and logged, including ANY.RUN's own `BACKDOOR APT-Q-27 related HTTP activity` Suricata detection. https://app.any.run/tasks/30e95295-c35b-46dd-9282-a59e06f381c6

**Lesson generalized**: this sample's anti-analysis design specifically targets naive automated submission (original/hash-based filenames). Any future ANY.RUN (or similar sandbox) submission of a sample with a visible "must contain substring in own filename" gate should be retried with a filename containing the likely substring (e.g. fragments of the claimed product name) before trusting a clean verdict.

## 7.5 Payload Decryption — MODULE_KEY / REGISTER_KEY Status (2026-07-21 follow-up)

Full reverse-engineering of how the three co-downloaded files are encrypted/loaded, cross-checked against the family's previously-recovered `family_apt_q27_zhongstealer.md` key material (`REGISTER_KEY`, `MODULE_KEY`, cipher1/cipher2 pseudocode — see `zhongstealer_protocol_reference.md`).

**REGISTER_KEY: UNCHANGED, live wire cipher UNCHANGED.** The ANY.RUN PCAP (`SAC_tool_renamed_anyrun.pcap`, TCP stream 15, `192.168.100.11:49691 → 35.78.126.246:3133`) was pulled and the first post-handshake WebSocket binary frame decoded: 12-byte header `total_len=188, orig_size=516, session_id=0x094E` (516 bytes is the exact documented REGISTER-frame plaintext size). Decrypting the payload (including the `f2e4` WS-artifact prefix, which for this build is *not* stripped before cipher application) with the **original, unmodified 8-operation cipher1** and the **unmodified REGISTER_KEY**, then zlib-inflating, yields exactly 516 bytes beginning `dwOSVersionInfoSize=284, dwMajorVersion=10, dwMinorVersion=0, dwBuildNumber=19045` — a byte-perfect OSVERSIONINFOEX matching the ANY.RUN sandbox's actual guest OS (Windows 10.0.19045, confirmed independently via the Microsoft telemetry URLs in the same capture). **Conclusion: REGISTER_KEY and the live C2 protocol are fully unchanged from the family's documented April/May 2026 build.**

**MODULE_KEY: key material UNCHANGED, at-rest cipher SIMPLIFIED.** `oihtq.uqv` (350,208B) decrypts byte-perfectly to a valid PE32 DLL using a **plain single-pass repeating XOR with the unmodified 32-byte MODULE_KEY** (`2031A71C399563ADAF1572E10ABB395387EB132208A001C5E140496D7A3E0B26`) — not the old 6-operation mixed cipher2. The result (SHA256 `813c4a2aab04fbb1f56ae5c0b4cb73188de57ffb3dc58b6019f631560de4ad33`) has export module name **`plugin32.dll`** and build timestamp **2022-09-25 08:41:33** — identical module name and identical timestamp to the previously-documented ZhongStealer core plugin (`family_apt_q27_zhongstealer.md` §7), and its offline KesaKode signature (`GhostRAT(15)/YoungLotus(15)/ValleyRAT(14)/PNGPlugLoader(13)`) exactly matches the prior sample's fingerprint. **This is effectively the same plugin32.dll module, now pre-staged via direct GCS download instead of being pushed over the WebSocket in encrypted chunks — a delivery-mechanism change, not a payload change.** This resolves why "the old cipher no longer works": the key never changed, only the wrapping algorithm for at-rest/staged files was simplified from the 6-op cipher2 to plain XOR.

**New per-module key discovered.** `vcnfq.uqv` (340,992B) also decrypts via plain repeating XOR but with a **previously-unknown 16-byte key**: `33C83BCF7507B94FE640BBEB1085CE75`. This is neither MODULE_KEY nor REGISTER_KEY nor a simple transform of either — a genuinely new key. The result (SHA256 `90b6c23ad9c39a4026ad1f188b38c08d46823fd3237c07164903a6b9d18b2ad5`, PE timestamp 2023-07-06 14:32:02) is the **core/loader DLL** (the `windui.dll`-class component): it contains a large AV/security-product process-enumeration list (`360sd.exe`, `Mcshield.exe`, `MsMpEng.exe`, `safedog.exe`, `KvMonXP.exe`, `rtvscan.exe`, `360tray.exe`, etc.), a browser-credential-theft target list (`Chrome.exe`, `firefox.exe`, `QQBrowser.exe`, `360se.exe`, `Skype.exe`, `Telegram.exe`, `WhatsApp.exe`) with associated data-file names (`FormData3.dat`, `Favorite3.dat`, `HistoryUrl3.db`), a reference to `Plugin.log` (matching the family's `.log`-staging convention), and — critically — the literal string **`e.nkking.com`**. This is an **exact, verbatim C2 domain match** to the previously-tracked `Samp.exe` sample (`topic_sampexe.md`, Feb 2025 build, analyzed 2026-05-31: "C2 `e.nkking.com:12940`; same Firefox/11 UA as windui.dll") — meeting the strict cross-reference bar (exact matching network IOC string embedded in both binaries) independent of the ANY.RUN vendor tag already establishing attribution.

**`ousctr.gtk` (433,576B) remains undecrypted.** Exhausted: plain XOR and the original multi-op cipher1/cipher2 with MODULE_KEY, REGISTER_KEY, and the new VCNFQ_KEY at all reasonable phase/header-offset alignments (0–31 bytes); ADD/SUB variants of the same keys; period-autocorrelation scan (no clean single-byte-XOR period found in range 2–128, unlike `vcnfq.uqv` which showed a clean 16-byte period against a known-plaintext crib); LZNT1 decompression raw and after XOR with all three known keys. Its Shannon entropy (7.44 bits/byte) is measurably lower than the two cracked files (7.86 and 7.71) and byte `0x7D` occurs anomalously often (~4.8% of the file, vs. ~0.4% expected for uniformly encrypted/compressed data) — neither cracked key's byte set contains `0x7D`, so this is very likely a **fourth, distinct key or an altogether different format** (compressed resource bundle, config data, or an unrelated blob) rather than another XOR'd PE module. Flagged as an open item.

**How the components fit together (assessed)**: `oihtq.uqv`=plugin32.dll (capability plugin, file manager/screenshot/shell/keylogger/proxy per the family's documented export table) and `vcnfq.uqv`=the core/loader DLL (AV evasion + browser theft + legacy `e.nkking.com` C2 string, `Plugin.log` reference suggesting it is the component that loads plugin32.dll) are both confirmed. Neither was observed being `LoadLibrary`'d or reflectively mapped during the ~2-minute ANY.RUN window — the observed WebSocket C2 traffic came directly from `minibrowser.exe` using only stock system DLLs. The most consistent explanation is that these two modules are staged on disk for a **later phase** (loaded reflectively by native code not yet triggered in this short sandbox run, e.g. on a live operator command), rather than being loaded immediately at drop time. `ousctr.gtk`'s role is undetermined.

Scripts and artifacts: `/home/remnux/mal/output/SAC_tool_key_test.py` (key/cipher brute-force harness), `oihtq_decrypted.bin` (=plugin32.dll), `vcnfq_decrypted.bin` (=core/loader DLL), `SAC_tool_renamed_anyrun.pcap` (raw network capture), `tcp_stream15_raw.txt` / `ws_frame_2_unmasked.bin` (decoded REGISTER-frame WS traffic).

## 8. Analyst Notes

- **Mechanism behind `minibrowser.exe`'s C2 behavior not fully reversed**: no anomalous DLL modules were loaded into its process (all 65 loaded modules are standard Windows system DLLs), ruling out classic DLL side-loading as the mechanism. The most likely explanation is that `minibrowser.exe` (part of a legitimate PC-manager/updater application suite, per the `PCManager\Update` staging path) has built-in remote-config/auto-update networking, and the attacker is supplying it a malicious config or update-channel target via one of the co-downloaded opaque files (`oihtq.uqv`/`ousctr.gtk`/`vcnfq.uqv`) rather than via code injection. This was not confirmed by direct reversal of `minibrowser.exe`'s config-reading logic — recommended follow-up if this wave recurs.
- **Correction to prior assessment**: `minibrowser.exe` (hash `26fba07c...`) was previously assessed standalone as "low-risk/likely benign" (see `topic_minibrowserexe.md`). That assessment's own caveat — "if this file surfaces again alongside other files in a delivery chain, that companion context is where the real signal would be" — is now confirmed: in this delivery context, the identical binary performs live WebSocket backdoor C2. The file itself is unmodified; the risk is entirely contextual/environmental (files it reads from its own directory).
- **Purpose of the 3 opaque payloads still not fully determined**: they are not auto-executed by SAC_tool.exe and were not loaded as PE modules by `minibrowser.exe`. Recommended follow-up: manually extract `minibrowser.exe` and identify what local files/paths its legitimate update-check or config-load routine reads, to determine which (if any) of the three blobs it consumes and how.
- **TTP evolution vs. reused toolkit**: this wave does not match the family's previously documented shellcode/LZNT1/Delphi-core delivery mechanism (fingerprints #1, #2, #5, #6 in `family_apt_q27_zhongstealer.md` all failed to match). Only the GCS-staging-bucket pattern (#3, new bucket name `nikeupdat`) and a partial C2-domain-family match (#4, `keensie.com` apex, new subdomain and port) tie it to the family, corroborated independently by ANY.RUN's proprietary detection. This may represent a new delivery mechanism for the same actor rather than a variant of the known loader.
