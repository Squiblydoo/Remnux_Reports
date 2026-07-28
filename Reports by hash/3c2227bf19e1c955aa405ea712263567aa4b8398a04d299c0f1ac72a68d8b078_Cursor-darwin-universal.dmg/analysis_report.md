# Cursor-darwin-universal.dmg — Malware Analysis Report

**Date:** 2026-07-28

## 1. File Metadata

| Property | Value |
|---|---|
| Filename | `Cursor-darwin-universal.dmg` |
| SHA256 | `3c2227bf19e1c955aa405ea712263567aa4b8398a04d299c0f1ac72a68d8b078` |
| SHA1 | `01ad8ae2f09ac2cdd2b2a6700848220978ef4d90` |
| MD5 | `0860e04de94b8c3d08b192c64edb9167` |
| Size | 30,169,301 bytes (~28.8 MB) |
| Type | Apple UDIF disk image, LZFSE/LZVN-compressed, single APFS partition |
| Volume name | "Cursor Installer" |
| Volume UUID | `dc8024d302d040a49bacb3f4968d4a60` |
| Volume created | 2026-07-17 14:50:24 UTC |
| Build tooling artifact | `.background/dmgcanvas_bg.tiff` + UDIF comment `disp-fix` → built with the commercial "DMG Canvas" tool |

### Contained items

| Path | Size | Note |
|---|---|---|
| `Cursor.app/` | — | folder deliberately named to impersonate the Cursor AI code editor |
| `Cursor.app/Contents/MacOS/auralis` | 306,992 bytes | **actual payload** — universal Mach-O, executable name is `auralis`, not `Cursor` |
| `Cursor.app/Contents/Resources/AuralisICNS.icns` | 90,520 bytes | icon resource is also branded "Auralis", not Cursor |
| `Cursor.app/Contents/Info.plist` | 1,225 bytes | reveals true bundle identity (see below) |
| `license.pdf` | 43,169,923 bytes | genuine PDF (version 1.7), **not malicious** — appears to be padding/decoy, see §8 |
| `Applications` | 14 bytes | standard symlink (normal DMG installer convention) |
| `.background/dmgcanvas_bg.tiff`, `.VolumeIcon.icns`, `.DS_Store`, `.fseventsd/*` | — | standard DMG furniture |

### `Cursor.app/Contents/Info.plist` (verbatim key findings)

```
CFBundleName          = Auralis
CFBundleDisplayName   = Auralis
CFBundleIdentifier    = team.auralis.dev
CFBundleVersion       = 12
CFBundleShortVersionString = 2.2.4
CFBundleExecutable    = auralis
CFBundleIconFile      = AuralisICNS
```

The folder the user sees and drags into `/Applications` is named `Cursor.app`, but the bundle inside it is entirely unrelated software called **"Auralis"**. The real Cursor editor is a multi-hundred-megabyte Electron application; this "installer" contains only a 300 KB native binary — there is no code editor functionality anywhere in this package.

### Payload binary: `Cursor.app/Contents/MacOS/auralis`

| Property | Value |
|---|---|
| SHA256 | `c8d571a9a03475fc3d85da44fa29f82c4c2f0f2955651aba138df2465f08e7f7` |
| Size | 306,992 bytes |
| Type | Universal (fat) Mach-O: x86_64 + ARM64 slices |
| Language | Swift (Foundation/CFNetwork/Dispatch/os.Logger, ObjC interop) |
| Swift module names | `auralis_x86_64`, `auralis_arm64` |
| Code signature | **Developer ID Application: Todor Madjarov (5A8SW7S333)** — chained to Apple's "Developer ID Certification Authority" and counter-signed by Apple's Timestamp Authority (validity window observed: 2026-06-22 to 2026-08-03) |

The code signature is a legitimate Apple Developer ID (a real, paid Apple Developer Program enrollment), not a self-signed or stolen/leaked certificate indicator we could confirm further — but it is being used to sign a covert downloader disguised as a well-known developer tool.

## 2. Classification

**Malware family/type: Novel macOS trojan downloader ("Auralis" stager) masquerading as the Cursor AI code editor.**
**Confidence: High** (behavioral/structural evidence is unambiguous; not "confirmed" against a named tracked family because no offline or online KesaKode match and no prior sample exists in memory for cross-reference).

- Offline KesaKode: empty verdict (`kesakode_verdict: []`) for both the DMG and the `auralis` Mach-O — no local signature hits.
- Online KesaKode: not run — `$MALCAT_KEY` is not configured in this environment.
- No YARA family/capability matches beyond a generic, low-reliability (20) `ValuableFileExtensions` hit on the compressed DMG stream (noise from LZFSE-compressed data, discarded).

Reasoning for the classification, independent of any signature match:
1. **Identity masquerade** — folder/DMG named "Cursor", but `Info.plist` and the executable itself identify as "Auralis" (`team.auralis.dev`). A legitimate installer would never disagree with itself this way.
2. **Impossible size for the claimed product** — the entire "app" is a 300 KB native stub; the real Cursor editor ships as a >100 MB Electron bundle. There is no editor functionality present at all.
3. **Structured downloader-and-execute logic**, recovered directly from Swift symbol demangling and decompilation (§3), matching the classic "stage-0 loader" pattern: environment gate → single-instance lock → fetch → validate → drop to temp → chmod executable → run → delete → exit.
4. **C2 host deliberately fragmented in code** to avoid static string-scanning tools (§4) — no malware author does this to a benign update URL.
5. **43 MB decoy PDF** bundled purely to inflate the package to a "reasonable" installer size (§8).

## 3. Capabilities (recovered from Swift symbol table + `fn_decompile`)

Swift's mangled export names demangle cleanly and map 1:1 onto the decompiled functions (verified via `lief` symbol-to-VA mapping, cross-checked against malcat's file-offset-based EAs):

| Symbol | malcat EA (x86_64 slice) | Purpose |
|---|---|---|
| `isEnvironmentAllowed() -> Bool` | 0x3f30 | Reads `ProcessInfo.processInfo.environment`, hash-looks-up keys against an internal set; refuses to proceed if a match is found in the environment — an **anti-sandbox / analysis-environment gate** |
| `ExecutionGate` (class) `.acquire()` / `.release()` | 0x4200 / 0x44b0 | Takes an exclusive file lock (`NSFileHandle` on a `lockURL`) before running — enforces **single-instance execution**, preventing re-entrancy/relaunch during analysis |
| `Endpoint.host` (getter) | 0x3d20 | Builds the C2 hostname at runtime from **6 fragmented string literals** joined together (§4) instead of a static string |
| `Endpoint.url` (getter) | 0x39c0 | Builds full URL: fragmented scheme + fragmented host + fragmented path components (§4) |
| `fetchWithRetry() -> Data?` / `fetch(from:) -> Data?` | 0x57c0 / — | HTTP GET via an **ephemeral `URLSession`** (no cache/cookie persistence) with a custom `User-Agent` header, wrapped in a retry loop with `backoff(_:)` delay between attempts (`Config.maxAttempts`, `Config.timeout`) |
| `validate(_:Data) -> Bool` | 0x4c00 | Decodes the fetched bytes as UTF-8 and checks for a prefix marker + minimum length before trusting the response — implies the payload is **text-based** (e.g. a script or encoded blob), not a raw second executable |
| `writeTemporary(_:Data) -> URL?` | 0x4e40 | Writes the validated payload to a uniquely named file (`globallyUniqueString`) inside the temp directory and sets POSIX permissions to `0755` (executable) |
| `execute(_:URL) -> Bool` | 0x5320 | Configures an `NSTask`/`Process` with the dropped file as `executableURL`, empty arguments, launches it, waits for exit, checks `terminationStatus == 0`, then **deletes the dropped file** (`removeItemAtURL`) |
| `run() -> ExitCode` | 0x5900 | Orchestrator: gate check → acquire lock → fetch-with-retry → validate → write-temp → execute → release lock → structured exit code |
| `Config` (enum) | — | Static tunables: `timeout`, `maxAttempts`, `maxPayloadSize`, `userAgent` |
| os.Logger subsystem | — | `com.taskd.runtime` — visible in the macOS unified log / Console.app if inspected live |

Net effect: **silent fetch → validate → drop-executable → run → self-erase**, gated by environment checks and a run-once mutex. No actual editor, no user-visible functionality is ever provided — launching "Cursor.app" only executes this hidden downloader chain.

## 4. C2 / Endpoint reconstruction (string de-fragmentation)

The host and URL are never present as plain strings in the binary (confirmed by exhaustive `strings`/malcat scans — the only ASCII network strings present are boilerplate from the embedded Apple code-signing certificate chain). Instead, `Endpoint.host` and `Endpoint.url` assemble the value at runtime from an array of Swift small-string literals (each ≤11 characters) that are concatenated:

- Host fragments: `"raw"` + `"."` + `"github"` + `"usercontent"` + `"."` + `"com"` → **`raw.githubusercontent.com`**
- Scheme fragments: `"ht"` + `"tps"` → **`https`**
- Path fragments: `"mgothiclove"`, `"subdata"`, `"main"`, `"submod.cfg"` → joined with `/` (GitHub raw-content URL convention: user/repo/branch/file) → **`mgothiclove/subdata/main/submod.cfg`**

**Reconstructed URL:** `https://raw.githubusercontent.com/mgothiclove/subdata/main/submod.cfg`

This was recovered by mapping Swift `_SmallString` literal encoding (content bytes + `0xE0|length` discriminator byte) out of the decompiled constant assignments; the decoder script is included alongside this report (`Cursor-darwin-universal.dmg_decode_fragments.py`). The fragmentation is a deliberate anti-string-scanning technique — abusing GitHub's raw-content CDN as a free, reputable-looking hosting point for a mutable second-stage config/payload. This URL was **not fetched** during this analysis (no interaction with the live endpoint); its content, if any, may change or be removed by the operator at any time and was not retrieved.

## 5. Attack Chain

1. Victim obtains `Cursor-darwin-universal.dmg` believing it to be the Cursor AI code editor installer (distribution vector not present in this artifact — likely malvertising/typosquat, consistent with prior fake-developer-tool campaigns).
2. Victim mounts the DMG; Finder shows `Cursor.app` + an `Applications` shortcut — the standard, familiar macOS drag-to-install UX.
3. Victim drags `Cursor.app` to `/Applications` and launches it, expecting the Cursor editor.
4. The launched binary (`auralis`) runs `isEnvironmentAllowed()` to check for hostile/analysis environment indicators.
5. `ExecutionGate.acquire()` takes an exclusive lock to prevent concurrent/repeated execution.
6. `fetchWithRetry()` requests `https://raw.githubusercontent.com/mgothiclove/subdata/main/submod.cfg` over an ephemeral HTTPS session with retry/backoff.
7. `validate()` checks the response is well-formed text with an expected prefix and minimum size.
8. `writeTemporary()` drops the validated payload to a uniquely named, executable (`0755`) file in the temp directory.
9. `execute()` launches the dropped file as a child process and waits for it to finish.
10. The dropped file is deleted (`removeItemAtURL`) and the lock is released — leaving no persistent artifact from this stage.
11. No Cursor editor UI or functionality is ever shown to the victim.

## 6. IOCs

### Network (defanged)
- `raw[.]githubusercontent[.]com` — abused legitimate GitHub CDN host, used as the fetch endpoint (built at runtime from fragmented literals, not a plaintext string in the binary)
- `hxxps[://]raw[.]githubusercontent[.]com/mgothiclove/subdata/main/submod[.]cfg` — reconstructed full URL (not fetched/verified live during this analysis)

### Filesystem
- `Cursor.app/Contents/MacOS/auralis` — main payload, SHA256 `c8d571a9a03475fc3d85da44fa29f82c4c2f0f2955651aba138df2465f08e7f7`
- `NSTemporaryDirectory()/<UUID>` — transient dropped/executed second-stage file, self-deletes after execution (no fixed name to pin down)

### Code signing
- `Developer ID Application: Todor Madjarov (5A8SW7S333)` — Apple Developer ID used to sign the malicious `auralis` binary

### Logging
- os_log subsystem `com.taskd.runtime` — visible via `log show --predicate 'subsystem == "com.taskd.runtime"'` or Console.app on an infected host

### Hashes
- DMG: `3c2227bf19e1c955aa405ea712263567aa4b8398a04d299c0f1ac72a68d8b078`
- `auralis` Mach-O: `c8d571a9a03475fc3d85da44fa29f82c4c2f0f2955651aba138df2465f08e7f7`

## 7. Emulation Results

Not applicable. The runtime-emulation tooling available in this environment (speakeasy, Qiling in the configured rootfs) targets Windows PE binaries; there is no equivalent macOS Mach-O emulator configured here. All behavioral conclusions in this report come from static decompilation of the Swift-demangled logic (§3–4), which was sufficiently unambiguous (function names survive intact in the Swift symbol table) to reconstruct the full control flow without dynamic execution.

## 8. Sandbox Results

**ANY.RUN: not submitted.** Per prior analysis experience in this environment ([[feedback_anyrun_macos_limitation]]), ANY.RUN has no macOS sandbox — DMG/Mach-O submissions detonate (if at all) inside a Windows VM and never actually execute the real payload, producing a meaningless verdict. `$ANY_RUN_KEY` is also not configured in this environment. Both factors independently rule out this step for this artifact.

## 9. Analyst Notes

- **`license.pdf` (43 MB) is a genuine, non-malicious PDF** — its `com.apple.metadata:kMDItemWhereFroms` extended attribute records it was downloaded from a public Nextcloud share at `nextcloud.documentfoundation.org` (LibreOffice/TDF's own file-sharing instance). It contains ordinary embedded photos and is almost certainly repurposed by the attacker purely as **filler to inflate the DMG to a plausible installer size** (~29 MB total vs. a 300 KB payload) — it is not part of the attack logic and carries no IOC value of its own beyond confirming operator tradecraft.
- The `validate()` function expects a **UTF-8 text response with a specific prefix**, meaning the second-stage artifact fetched from GitHub is likely a script, base64-encoded blob, or config — not a raw Mach-O — consistent with `Config.maxPayloadSize` and `Config.userAgent` static fields seen in the symbol table suggesting a deliberately small, constrained fetch.
- The `isEnvironmentAllowed()` gate could not be fully reverse engineered to the specific list of blocked/required environment variable names within the time invested (it performs a hashed dictionary-membership check rather than a simple string comparison); a deeper static effort or live-environment fuzzing could recover the exact gating keys.
- The reconstructed GitHub path (`mgothiclove/subdata/main/submod.cfg`) was **not fetched** — its live content is unknown and may have changed since this DMG was built. Any follow-up should treat that URL as a hostile-infrastructure lead only, and fetch it from an isolated/monitored environment if further investigation is warranted.
- capa was attempted against the universal Mach-O but the installed version rejected the file format outright ("Input file does not appear to be a supported file") — no capa capability data available for this sample.
- Recommend blocking/monitoring outbound requests from developer workstations to `raw.githubusercontent.com/mgothiclove/*` and flagging any macOS app bundle whose `CFBundleExecutable`/`CFBundleIdentifier` disagree with its enclosing folder/DMG name, which is the most durable detection signal here (independent of any single hash/URL that the operator can trivially rotate).
