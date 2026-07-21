# SAC_tool.exe — Malware Analysis Report

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
- The PE overlay (25,912 bytes, present because `HasOverlay`) is the Authenticode PKCS#7 `SignedData` blob itself — not a hidden secondary payload.

**Version-info / build metadata mismatch (notable anomaly)**: The embedded `VersionInfo` and `AssemblyInfo` claim `CompanyName: Amazon.com`, `ProductName: SAC tool`, `LegalCopyright: Copyright © Amazon.com 2024` — while the actual code-signing certificate belongs to an unrelated Chinese company. This is a masquerade pattern (fake brand metadata paired with an unrelated signing identity), not a genuine Amazon tool.

## 2. Classification

**Confidence: Medium (downloader/loader with active anti-analysis logic; no confirmed family attribution).**

- KesaKode offline (local hash DB) and KesaKode online (cloud lookup, verbose mode, 9,994/10,000 monthly quota remaining) both returned **zero family matches** — no code-sharing signal with any tracked family in the KesaKode corpus.
- No YARA family/capability hits beyond generic `DotNet` and compiler-detection signatures.
- **Not attributed to APT-Q-27/ZhongStealer** (a family tracked in prior analyses) despite surface similarities (GCS-bucket staging, `image.jpg` decoy): none of the strict cross-reference criteria were met — no exact payload-hash reuse, no matching cert serial (`0d2ad57b...` LENOVO cert not present), no PDB path match (`084049` not found), and the known `(byte+0x77)^0x62` / LZNT1 extraction pipeline applied to the three downloaded payloads produced no valid PE and no matching strings (see §8). One candidate payload (`vcnfq.uqv`) coincidentally matches the exact byte size (340,992) of the confirmed ZhongStealer `windui.dll` core, but its SHA256 differs entirely and it does not decode to a PE — this is a size coincidence, not evidence.
- Functionally, this is a **generic .NET downloader/dropper** with heavy string obfuscation (custom resource-based decryption, Eazfuscator/ConfuserEx-style control-flow flattening with anti-tamper caller-assembly checks) and a built-in anti-sandbox/anti-VM gate.

## 3. Capabilities

Recovered from full ILSpy decompilation (`/home/remnux/mal/output/SAC_tool_decompiled/`) plus capa:

- **Execution gate**: refuses to run its main logic unless its own process filename (minus extension) contains a specific (still-encrypted) substring; otherwise it shows a MessageBox and exits. This blocks naive sandbox submissions that rename the sample.
- **Anti-sandbox / anti-VM checks** (all must fail to allow execution):
  - Known analysis-tool process names (2 candidates, e.g. likely `vboxservice`/`vmtoolsd`-class names)
  - MAC address OUI prefix matching (3 candidate vendor prefixes — typical VMware/VirtualBox OUI check)
  - System drive total size < 40 GB
  - Presence of known VM guest-tool driver files (3 candidate paths)
  - Sandbox-related environment variables (3 candidates, e.g. `SbieDll`-class)
  - Machine name / username / domain name containing a sandbox marker string, plus a matching env var
  - `SystemDirectory` path containing the sandbox marker + 3 suspicious process names
  - Exact username match against a known analyst/sandbox account name
  - WMI `Win32_BIOS`/related class Serial/Version check against a known VM string, plus DNS-suffix check, plus a `hosts`-file content check
  - WMI `Win32_ComputerSystem` Manufacturer/Model string check (2 separate WMI queries)
- **Manifest-based downloader**: decodes a base64 (possibly double-base64) embedded string to a manifest URL, HTTP GETs it, parses the response as newline-delimited absolute URIs, and downloads each (5-way concurrent, `SemaphoreSlim(5)`) to a hidden folder under `%LOCALAPPDATA%\<obfuscated subpath>\<date>_<4-char GUID>\`, setting `FileAttributes.Hidden` on both the folder and each downloaded file.
- **Conditional auto-execution** of downloaded files by extension only:
  - `.exe` / another exec-type extension → `Process.Start` (hidden window)
  - `.ps1` → `powershell -File <path>` (hidden window)
  - `.vbs` → `wscript <path>` (hidden window)
  - `.jpg/.png/.gif/.bmp/.ico`-class extensions → `Process.Start(UseShellExecute:true)` (opens with default viewer — decoy display)
  - Files with **no matching extension are downloaded but never auto-launched by SAC_tool.exe itself**
- TLS 1.2-only `HttpClient` with a custom `ServerCertificateCustomValidationCallback` that **accepts all certificates** (disables cert validation for its own outbound traffic), custom User-Agent and headers, 45s timeout, cookies disabled.
- Cleans up its own temp working directory on disposal.
- capa MBC/ATT&CK highlights: `Deobfuscate/Decode Files or Information [T1140]`, `Hide Artifacts::Hidden Window [T1564.003]`, `File and Directory Permissions Modification [T1222]`, `Account/Process/System/Software/Network Discovery`, `Windows Management Instrumentation [T1047]`.

## 4. Attack Chain

1. User/victim runs `SAC_tool.exe` (must be run under a filename containing the expected substring, or it silently self-terminates after a decoy MessageBox).
2. Anti-sandbox gate evaluates ~10 independent VM/sandbox indicators; if any hit, the downloader logic is skipped (`_0002()` returns `false` silently — no error, no visible failure).
3. If clear, the tool decodes an embedded base64 manifest URL and fetches a newline-delimited list of absolute URLs.
4. **Observed manifest content** (recovered from this session's companion investigation into the same delivery chain, downloaded directly from the bucket the tool is architected to fetch from — `https://storage.googleapis.com/nikeupdat/`):
   - `oihtq.uqv` (350,208 bytes) — high-entropy opaque blob, no PE magic, not auto-executed by SAC_tool.exe (no extension match)
   - `ousctr.gtk` (433,576 bytes) — high-entropy opaque blob, same as above
   - `vcnfq.uqv` (340,992 bytes) — high-entropy opaque blob (shows partial repeating byte pattern suggestive of block/stream-cipher output), same as above
   - `minibrowser.exe` (368,288 bytes) — **exact SHA256 match** (`26fba07c17efbb6c48a2e746e42df1ee26405c6aa557039492553e5bc27598a1`) to a previously analyzed sample: genuine, unmodified Tencent TBS MiniBrowser SDK component, re-signed with an unrelated "Feidelai (Chengdu) Home Co., Ltd." EV cert, assessed low-risk. `.exe` extension → SAC_tool.exe **will auto-launch this**.
   - `image.jpg` (11,343 bytes) — decoy image → SAC_tool.exe **will auto-open this** with the default viewer.
5. Net effect of SAC_tool.exe's own execution logic: it launches the benign-looking `minibrowser.exe` and opens a decoy JPEG, producing an innocuous-looking UX for the victim, while three unidentified opaque payloads sit hidden on disk, downloaded but not executed by this component. Their purpose (staged for a later-stage loader, or meant to be side-loaded/read by `minibrowser.exe` or another dropped component not present in this manifest snapshot) could not be determined from static analysis alone.

## 5. IOCs

**Network**
- `hxxps[://]storage[.]googleapis[.]com/nikeupdat/oihtq[.]uqv`
- `hxxps[://]storage[.]googleapis[.]com/nikeupdat/ousctr[.]gtk`
- `hxxps[://]storage[.]googleapis[.]com/nikeupdat/vcnfq[.]uqv`
- `hxxps[://]storage[.]googleapis[.]com/nikeupdat/minibrowser[.]exe`
- `hxxps[://]storage[.]googleapis[.]com/nikeupdat/image[.]jpg`
- The actual manifest URL fetched by SAC_tool.exe itself remains string-encrypted and was not recovered (see §8); the above bucket is assessed with medium confidence to be its target, based on this session's parallel download of the bucket contents and the exact structural match to the tool's file-type handling logic (exe + image decoy pattern), but this was not confirmed via dynamic capture of SAC_tool.exe's own traffic (ANY.RUN did not observe it reaching any non-Microsoft host — see §7).

**Filesystem**
- Downloads to a hidden subfolder of `%LOCALAPPDATA%` with a `<date>_<4-char-GUID>` naming pattern (exact subpath string-encrypted, not recovered)
- Temp working file: `%TEMP%\<GUID-based name>` (created/deleted at runtime)

**Certificates**
- Code-signing cert serial `5df273a440e188cfd64188d1ef1e5931`, subject 杭州思维宇宙科技有限公司, issued by Certum EV Code Signing 2021 CA

**Hashes**
- SAC_tool.exe: `9c5b840e50cd2672f803b9ac8ad8285a01bf0a6292b65d9fe471af4d1a5e5384`
- Downloaded `minibrowser.exe` (exact match to prior analysis, assessed low-risk): `26fba07c17efbb6c48a2e746e42df1ee26405c6aa557039492553e5bc27598a1`
- Downloaded `oihtq.uqv`: `mk7...` — sha256 not yet computed/needed beyond confirming non-PE
- Downloaded `ousctr.gtk` / `vcnfq.uqv`: opaque, unidentified format

**Mutexes**: none observed (static or dynamic).

## 6. Emulation Results

Speakeasy/angr emulation was not applicable — this is a .NET-only assembly (no unmanaged code beyond P/Invoke declarations for `kernel32`/`user32` console/window functions), and speakeasy targets native x86/x64 code paths. FLOSS explicitly does not support .NET string deobfuscation (confirmed via direct run: *"FLOSS does NOT attempt to deobfuscate any strings from .NET binaries"*).

String recovery was attempted via full ILSpy decompilation instead. The string-decryption routine (`_0002_0015._0002(int id)`) uses a custom scheme (embedded-resource-backed byte stream + XOR + LZNT1-style back-reference decompression) protected by a `StackTrace`-based caller-assembly check (classic Eazfuscator/ConfuserEx-style anti-tamper) that returns a canary value if invoked from outside the original assembly context — this blocks straightforward reflection-based bulk string extraction and was not defeated in this pass (would require IL-level unpacking, e.g. de4dot, which is not installed on this REMnux instance).

## 7. Sandbox Results

**ANY.RUN**: Task `35c57d69-047f-41cb-b9e6-684a69f9c89c` — verdict score **0/100**, "No threats detected", no behavioral tags. Only HTTP traffic observed was routine Windows/Microsoft telemetry (OCSP/CRL checks, `settings-win.data.microsoft.com`, `login.live.com`) — **no contact with `storage.googleapis.com` or any non-Microsoft host was observed.**

Public report: https://app.any.run/tasks/35c57d69-047f-41cb-b9e6-684a69f9c89c

**This "No threats detected" verdict should be treated with low confidence as evidence of benignity.** Static analysis shows SAC_tool.exe implements ~10 independent anti-sandbox/anti-VM checks (§3) plus a filename-content execution gate, any one of which — if triggered by ANY.RUN's environment or submission filename — would cause the tool to silently skip its download/execution logic without any visible error, exactly matching what was observed (clean run, no network activity beyond OS noise). The absence of malicious network activity is therefore most plausibly explained by successful anti-analysis evasion, not absence of malicious functionality.

## 8. Analyst Notes

- **ZhongStealer extraction pipeline attempted and inconclusive**: as part of this session's investigation, the three opaque downloaded payloads (`oihtq.uqv`, `ousctr.gtk`, `vcnfq.uqv`) were run through the established `updat.log → LZNT1 → UPX` ZhongStealer extraction procedure. All three produced non-PE, unidentifiable output (`file` reported "data" / a PGP-sub-key false-positive) with no `.pdb` or `LENOVO`-cert strings recovered. This is a negative result — either these payloads use a different encoding entirely, or the extraction parameters (key/offset) differ from the known ZhongStealer wave formula. **No attribution should be drawn from this attempt in either direction.**
- **Residual gap — true C2/manifest URL unrecovered**: the base64-decoded manifest string that SAC_tool.exe itself fetches was not statically recovered due to the anti-tamper string-decryption gate (§6). Recommended follow-up: install `de4dot` or manually patch out the `StackTrace` caller-assembly check in a copy of the assembly (via `dnlib`/IL rewriting) to force real string decryption, or execute the sample under controlled dynamic instrumentation (x64dbg/dnSpy with a breakpoint on the resource-decrypt method) with the correct filename to satisfy the execution gate.
- **Residual gap — purpose of the 3 opaque payloads unresolved**: they are downloaded by SAC_tool.exe's logic but not auto-executed by any code path found in this binary. Recommended follow-up: determine whether `minibrowser.exe` (genuine Tencent TBS SDK) has a DLL side-load path that any renamed version of these blobs could satisfy, or whether a separate, not-yet-observed component in the delivery chain is responsible for loading them.
- **Alternative hypothesis**: given the fake "Amazon.com" branding metadata, the mismatched Chinese EV signing identity, the anti-sandbox gate, and the generic-downloader architecture with no family-specific fingerprint, this may be a bespoke/one-off loader built for a specific campaign rather than a reused toolkit — consistent with the zero KesaKode match (no code shared with any sample in the corpus).
