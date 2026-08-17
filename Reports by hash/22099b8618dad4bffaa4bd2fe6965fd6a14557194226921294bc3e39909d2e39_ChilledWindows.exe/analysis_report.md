# ChilledWindows.exe — Analysis Report

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | ChilledWindows.exe |
| SHA256 | `22099b8618dad4bffaa4bd2fe6965fd6a14557194226921294bc3e39909d2e39` |
| SHA1 | `a9672d783542e1d192708f56a186e30b37ab5614` |
| MD5 | `283ba91983504cd24f022a8f106ffa11` |
| Size | 4,588,096 bytes (4.5 MB) |
| Type | PE32 GUI, .NET (Mono/.NET assembly), 3 sections, x86 (AnyCPU) |
| Compile timestamp | 2016-09-10 17:32:23 |
| PDB path | `C:\Users\gamel\documents\visual studio 2015\Projects\ChilledWindowsWPF\ChilledWindows\obj\Release\ChilledWindows.pdb` |
| VersionInfo | CompanyName=GAMELASTER, ProductName=ChilledWindows, FileVersion=1.0.0.0, Copyright © GAMELASTER 2016 |
| Imphash | `f34d5f2d4577ed6d9ceec516c1f5a744` |
| Signing cert | SSL.com EV Code Signing (Intermediate CA RSA R3); Subject **"Hypixel Studios Canada Inc"**, Quebec/L'Ange-Gardien, CA; Serial `664c671f5dcc74dd520d1ac53cfd85b6`; Valid 2026-01-01 → 2026-12-19 |

**Notable anomaly:** the code-signing certificate subject ("Hypixel Studios Canada Inc") does not match the binary's internal branding (GAMELASTER / ChilledWindows / the well-known 2016 open-source "ChilledWindows" project). This is a cert/product mismatch, not evidence of code tampering — the decompiled source is identical to the legitimate open-source app.

## 2. Classification

**Verdict: Benign — legitimate open-source Windows "prank"/parody screensaver app, no malicious functionality found.**

Confidence: **high**.

Reasoning:
- Full .NET decompilation (ilspycmd) recovered complete, readable C# source matching the well-known open-source "ChilledWindows" GitHub project by GameLaster (2016) — a joke app that makes open windows appear to "flip away" revealing a spinning/aesthetic MP4.
- All 17 functions in the assembly are WPF UI/animation boilerplate; there is no networking code, no persistence mechanism, no credential access, no process injection, and no obfuscation.
- Offline and **online KesaKode** lookups both returned **zero family matches** (empty verdict) — no code overlap with any known malware family.
- capa flags ("Screen Capture", "Reflective Code Loading") are false-positive-prone generic heuristics matching the app's legitimate `Graphics.CopyFromScreen` (desktop screenshot used as an animation background) and `Type.InvokeMember`/`Activator.CreateInstance` COM calls (used only to invoke `Shell.Application.MinimizeAll()` to reveal the desktop before the animation plays).
- The single "BigStaticArray"/"BigResourceHighEntropy" anomaly is the embedded `chilledwindows.mp4` video resource (3.77 MB), written to `%TEMP%\chilledwindows.mp4`, played via `MediaElement`, then deleted on completion — exactly matching the decompiled logic.

The only genuinely suspicious element is the **code-signing certificate mismatch** (see Analyst Notes) — this alone does not indicate malicious code, but it is an integrity/provenance red flag worth tracking (e.g., cert reuse, resale of a signing identity, or an EV cert purchased under an unrelated shell/LLC name).

## 3. Capabilities

- Invokes `Shell.Application` COM object → `MinimizeAll()` to minimize all open windows and reveal the desktop.
- Takes a screenshot of the primary screen (`Graphics.CopyFromScreen`) and uses it as an animated background layer (simulates the desktop "flipping").
- Writes an embedded MP4 resource (`Chilled_Windows`, 3,771,441 bytes) to `chilledwindows.mp4` in the working/temp directory and plays it via WPF `MediaElement`.
- Drives a frame-synced animation (rotate/flip/scale transforms) timed against video playback position.
- On playback completion (frame 1260), deletes the temporary MP4 file and calls `Application.Current.Shutdown()`.
- Pressing Alt (KeyDown, VK 18) toggles a flip transform manually.

No networking, no registry persistence, no credential harvesting, no additional payload download, no process injection.

## 4. Attack Chain

Not applicable — this is a self-contained, single-stage GUI application with no droppers, downloaders, or secondary payloads beyond its own embedded video resource.

## 5. IOCs

**Network:** None. No C2 domains, IPs, or URLs. Only certificate-infrastructure URLs are embedded (SSL.com OCSP/CRL/repository endpoints — standard Authenticode chain artifacts, not indicators of compromise):
- `http[://]www[.]ssl[.]com/repository`
- `http[://]cert[.]ssl[.]com/SSLcom-SubCA-EV-CodeSigning-RSA-4096-R3.cer`
- `http[://]crls[.]ssl[.]com/SSLcom-SubCA-EV-CodeSigning-RSA-4096-R3.crl`
- `http[://]cert[.]ssl[.]com/SSLcom-SubCA-CodeSigning-I-RSA-R1.cer`
- `http[://]crls[.]ssl[.]com/SSLcom-SubCA-CodeSigning-I-RSA-R1.crl`
- `http[://]crls[.]ssl[.]com/SSLcom-rsa-RootCA.crl`

**Filesystem:**
- `%TEMP%\chilledwindows.mp4` (created then deleted at end of playback)

**Registry:** None written; benign reads only (`HKLM\SYSTEM\ControlSet001\Control\Nls\Sorting\Versions`, `...\ComputerName\ActiveComputerName`) — standard .NET runtime startup behavior, observed in sandbox.

**Mutexes:** "SQL CE related mutex" observed by ANY.RUN — a standard .NET/SQL Compact Edition runtime artifact, not sample-specific.

**Certificate:**
- Subject: Hypixel Studios Canada Inc
- Serial: `664c671f5dcc74dd520d1ac53cfd85b6`
- Issuer: SSL.com EV Code Signing Intermediate CA RSA R3

## 6. Emulation Results

Not performed. Speakeasy/angr emulation targets native x86/x64 entrypoints; this is a pure .NET managed-code assembly whose entire logic was already recovered at 100% fidelity via `ilspycmd` decompilation (see `/home/remnux/mal/output/ChilledWindows_ilspy/ChilledWindows/MainWindow.cs`). Emulation would add no additional visibility. floss was run per workflow but does not support .NET string extraction (expected, no-op).

## 7. Sandbox Results (ANY.RUN)

- **Score:** 100 / **Threat Level: "Malicious activity"**
- **Family tags:** none
- **Public report:** https://app.any.run/tasks/4792f5b0-d6cb-436f-a283-d1c2731f7869

**Verdict validation (per standing analyst practice of cross-checking ANY.RUN verdicts against actual behavior):** The 100/Malicious score is a false positive driven almost entirely by a single triggered signature, **"Executing a file with an untrusted certificate"** (threatLevel 2) — i.e., ANY.RUN's own reputation heuristic reacting to the same GAMELASTER-vs-"Hypixel Studios Canada Inc" cert/product mismatch identified in static analysis. The remaining 7 triggered incidents are threatLevel 0–1 and entirely benign/generic (temp file creation of `chilledwindows.mp4`, registry reads of computer name/machine GUID/language settings, IE security-zone read, and a routine .NET SQL CE mutex). The full IOC report contains **no non-Microsoft network destinations** — every HTTP(S)/DNS entry is Windows Update/telemetry/OCSP noise (`settings-win.data.microsoft.com`, `login.live.com`, `ocsp.digicert.com`, `crl.microsoft.com`), none originated by this sample's own logic. No dropped payload beyond the app's own MP4 resource; the one "Dropped file" entry (`LocalMLS_3.wmdb`) is a standard Windows Media Player local-library cache artifact created as a side effect of MediaElement/WMP playback.

**Conclusion: the ANY.RUN verdict should be disregarded for attribution/malice purposes — it reflects certificate reputation, not observed behavior.**

## 8. Analyst Notes

- **Certificate mismatch is the only open question.** The binary is byte-for-byte consistent with the legitimate, publicly available "ChilledWindows" open-source project (2016, GameLaster), but is signed with an SSL.com EV Code Signing cert issued to "Hypixel Studios Canada Inc" — an unrelated name, with a 2026 validity window on a 2016-compiled binary. Plausible explanations, in rough order of likelihood: (a) a third party purchased an EV cert under a shell/LLC name and used it to re-sign a public open-source binary for redistribution (common with adware bundlers or SEO-poisoning download sites, even when the payload itself is untouched); (b) the cert was resold/reused across unrelated products. No corroborating evidence (matching prior serials, C2, or altered code) ties this to any tracked family in memory, so per policy no cross-reference is made.
- No secondary payload, dropper, or persistence was found — if this file arrived via a suspicious channel (spam, fake download site), the delivery vector — not the executable — would be the actual risk vector to investigate.
- Recommended follow-up: if the delivery URL/channel for this sample is known, check it for bundling with other payloads (this is a common redistribution pattern for legitimate open-source freeware); the executable itself requires no remediation.
