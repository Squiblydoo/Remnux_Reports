# Malware Analysis Report: Build.exe

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | Build.exe |
| SHA256 | `b2e8e5722b0910f174a4ca688ac8ca6925149c90999d2475fbeb506488ae218d` |
| SHA1 | `7f902fec012bf86581fdd20c9b802b5b79daece8` |
| MD5 | `6dd4495bb967b93be830ba8a930a03e1` |
| File type | PE32+ (GUI) x86-64 |
| File size | 529,500,104 bytes (~505 MB) |
| Packer | UPX (5.x-compatible stub) |
| Signing | Self-signed X.509 cert, Subject/Issuer = **"Qilin Ransomware"**, Serial `497d0beffcb738ad48b5ba922e361b52`, SHA1/RSA, valid 2026-07-15 → 2027-07-15 |
| VersionInfo | CompanyName/FileDescription/InternalName/ProductName = **"Runtime Broker"** (masquerades as the legitimate Windows Runtime Broker process) |
| Compiler | MSVC 2022 linker artifacts; embedded self-contained .NET runtime (CoreCLR/NativeAOT-style single-file publish — bundles System.Drawing/GDI+, System.Windows.Forms, SharpDX.DXGI, NAudio) |

### Structural note — 505 MB is almost entirely padding
The PE (header + UPX0 + UPX1 + .rsrc) occupies only the first **5,204,992 bytes** of the file. The remaining **524,295,112 bytes** are an unreferenced `overlay` region (malcat: `entropy 225`, `BigBufferNoXrefMediumToHighEntropy` ×85, no known structure). Manual byte inspection at the start, middle, and end of the overlay showed uniformly high-entropy/random-looking data with no repeating pattern or structure — i.e., **junk padding**, not an embedded payload. The only structured artifact recovered by file carving inside the overlay is a trailing PKCS7 blob (the Authenticode signature) at file offset 529,493,000. This bulk-padding technique is a known evasion method to defeat sandboxes/AV engines and upload portals that skip or reject oversized files (confirmed in practice during this analysis — see Sandbox Results).

Stripping the overlay and running `upx -d` successfully decompressed the real payload:
- Trimmed (still UPX-packed) PE: 5,204,992 bytes, SHA256 `7d1749f809a7e2b037193f88af53e142d23b0cb65ed84146e769e9ab9dff45ab`
- Unpacked PE: 16,764,416 bytes, SHA256 `e90ae4467f53b7f924d0b2695e332c2f7c1152ccce629a1259961d86e41dd219`

All function/string/capability analysis below was performed on the unpacked payload unless noted.

## 2. Classification

**Primary classification: Full-featured .NET Remote Access Trojan (RAT) with SeroRAT code overlap. Confidence: Medium-High for RAT capability class.**

**"Qilin Ransomware" cert branding: NOT substantiated — discard for attribution.** The self-signed certificate's Subject/Issuer fields literally contain the text "Qilin Ransomware," but no corresponding ransomware capability exists anywhere in the binary: no ransom-note text, no custom file extension, no mass file-encryption routine, and no "your files have been encrypted"-style messaging were found in static strings, functions, or dynamic detonation. Every cryptography-related string traces back to the standard .NET BCL `System.Security.Cryptography` surface (RSA/ECDSA/SHA3 API names, padding modes, key-container strings) rather than a bespoke encryptor. The cert text is most plausibly a builder placeholder or intentional troll/misdirection (consistent with this sample's built-in "Rickroll" prank command — see Capabilities) rather than a genuine link to the Qilin RaaS operation.

**KesaKode (offline):** weak/noise-level signal — `Tsunami` confidence 2–4 (raw unnormalized count). Discarded per threshold policy (<20% equivalent).

**KesaKode (online, authoritative):** top match `SeroRAT: 23.27%` at the whole-file level — falls in the 20–79% band, i.e. **code-sharing/toolkit overlap, not confirmed attribution**. However, 16 individual functions independently matched `SeroRAT` at 100% confidence each (a stronger function-level reuse signal than the aggregate score alone suggests). Scattered low-confidence (20%) string-level hits to Tsunami/Luxy/MASSLogger/Umbral/Tsunamikit/QuirkyLoader/AresLoader/NamzMiner were all traced to generic, non-exclusive strings (`Add-MpPreference -ExclusionPath`, `Runtime Broker.dll/.exe`) shared across many families' reference databases and are not meaningful attribution signal.

The extensive RAT capability set recovered (keylogger, clipboard capture, webcam/mic capture, DXGI screen capture, remote input control, heartbeat-based C2 command loop, and a distinctive set of "troll" commands — BlockInput, SwapMouse, CrazyWindows, HideDesktop, BeepSpam, Rickroll, AutoRotate, FakeUpdate) is consistent with SeroRAT, a publicly available open-source .NET RAT known for exactly this style of surveillance + prank feature set. This sample appears to be a SeroRAT-derived build, NativeAOT-compiled into a large self-contained single-file executable, padded to ~505 MB, and signed with a joke/placeholder certificate.

## 3. Capabilities

- Masquerades as the legitimate Windows "Runtime Broker" process via PE version info
- UPX-packed; ~524 MB of high-entropy junk padding appended as an overlay (anti-sandbox / anti-upload-limit evasion — confirmed effective against ANY.RUN's Cloudflare-fronted upload endpoint, see Sandbox Results)
- Self-signed certificate with a "Qilin Ransomware"-labeled identity (troll/placeholder, not indicative of ransomware functionality)
- **Persistence:**
  - Registry Run key (`...\CurrentVersion\Run`)
  - Two scheduled tasks: `schtasks /create /tn "WindowsSysHost" /tr "..."` and `schtasks /create /tn "WindowsSysHostUser" /tr "..."` (masquerading as legitimate system tasks)
- **Defense evasion:**
  - PowerShell Defender exclusion: `Add-MpPreference ... -ExclusionPath '...'`
  - Registry write to `SOFTWARE\Policies\Microsoft\Windows Defender` (`DisableAntiSpyware`)
  - Disables Windows Firewall entirely: `netsh advfirewall firewall set allprofiles state off`
  - Enumerates installed antivirus products (checks for `avastui`, generic AV enumeration API usage)
  - Anti-debug checks (`IsDebuggerPresent`, `GetLastError`/`RaiseException` patterns) and sandbox/VM environment fingerprinting (malcat: `BlacklistSandbox`, `FingerprintEnvironment`, `FingerprintHardware`)
- **Privilege escalation:** `AdjustTokenPrivileges`, `SeDebugPrivilege`, `SeLockMemoryPrivilege`
- **Command execution:** spawns `powershell.exe` (`-WindowStyle Hidden -NoProfile -ExecutionPolicy Bypass -Command` / `-EncodedCommand`), `wscript.exe`, `cmd.exe`; also instantiates `WScript.Shell` COM object to run commands
- **Surveillance / RAT functions:**
  - Keylogger (`SetWindowsHookEx`, `GetForegroundWindow`, `GetWindowText`) — start/stop handlers present
  - Clipboard get/set
  - Webcam capture (start/stop handlers, `AotWebcamStreamer`)
  - Microphone/audio capture (`waveIn*` APIs via NAudio)
  - Screen capture via DXGI Desktop Duplication (`DxDesktopDuplicator`, `CaptureHiddenDesktopCompositor`) including a hidden-desktop compositor path
  - File transfer (chunked file receive handlers)
  - Remote wallpaper replacement, volume/sound control, URL-open command
  - Service enumeration and control
- **C2 protocol:** heartbeat-based command dispatch loop (`SendHeartbeat` → `ProcessServerMessage` handler pattern)
- **"Prank"/harassment commands:** `BlockInput`, `SwapMouse`, `CrazyWindows`, `HideDesktop`, `BeepSpam`, `Rickroll`, `AutoRotate`, `FakeUpdate`, `InvertMouse`, `CrazyMouse`, `GdiIcons` — hallmark of SeroRAT-style open-source RATs
- System/hardware fingerprinting: CPU name (registry), computer name, volume information, SID enumeration

## 4. Attack Chain (as determinable from static/dynamic evidence)

1. Delivery of the 505 MB `Build.exe` — the extreme size is itself a delivery/evasion mechanism (defeats size-limited scanning/upload pipelines).
2. Execution → UPX stub decompresses the ~16.7 MB .NET NativeAOT payload in memory.
3. Defense evasion executes: Defender exclusion (PowerShell), Defender policy registry write, firewall disable (netsh), AV enumeration, anti-debug/sandbox checks.
4. Persistence established: Registry Run key + two scheduled tasks (`WindowsSysHost`, `WindowsSysHostUser`).
5. Privilege/token adjustment attempted.
6. C2 heartbeat loop starts; the implant awaits operator commands to activate surveillance modules (keylogger, clipboard, webcam, mic, screen capture), remote input control, file transfer, service control, or prank/harassment payloads.

## 5. IOCs

**Network:** No attacker-controlled C2 domain or IP was recovered from static strings or from dynamic detonation. All 10 HTTP/HTTPS requests observed during sandbox execution were legitimate Microsoft telemetry/OCSP/CRL endpoints (reputation 0/clean) — e.g. `settings-win[.]data[.]microsoft[.]com`, `go[.]microsoft[.]com`, `ocsp[.]digicert[.]com`, `login[.]live[.]com`, `crl[.]microsoft[.]com`. The RAT's actual C2 endpoint was not observed in this run; it may not be embedded in this build, may be resolved only after operator interaction, or the sandbox session did not reach that code path.

**Filesystem:** None recovered beyond generic AppData/LocalAppData path references (no dropped-file paths with fixed names identified).

**Registry:**
- `HKCU\Software\Microsoft\Windows\CurrentVersion\Run` (persistence)
- `SOFTWARE\Policies\Microsoft\Windows Defender` → `DisableAntiSpyware` (defense evasion)

**Scheduled Tasks:**
- `WindowsSysHost`
- `WindowsSysHostUser`

**Mutex:** None confirmed — no malware-specific mutex name string identified (`Local\AllSessionsPrefix` is a generic .NET runtime string, not sample-specific).

**Certificate:**
- Subject/Issuer: "Qilin Ransomware"
- Serial: `497d0beffcb738ad48b5ba922e361b52`
- Validity: 2026-07-15 → 2027-07-15

## 6. Emulation Results

- **Speakeasy (generic runner, pass 1):** Loaded the unpacked PE (amd64) but produced **0 IOCs**. This is an expected limitation — this binary is a self-contained .NET CoreCLR/NativeAOT application; its runtime bootstrap sequence is incompatible with speakeasy's Win32-API-level emulation model, so meaningful application code was never reached before the emulation session ended.
- **capa:** Attempted twice (300s and 590s timeouts) against the unpacked 16.7 MB binary; both attempts timed out due to the binary's size/complexity. Capability confirmation was instead obtained via peframe + malcat YARA + manual string/function review (see Capabilities section), which is considered sufficient given the volume of corroborating evidence.
- **angr / custom hook passes:** Not attempted — no single isolated decrypt/config-resolution function was identified as a productive target; the RAT's C2 configuration appears to be resolved through full CLR execution rather than an isolated native routine.

## 7. Sandbox Results (ANY.RUN)

Submission of the original 505 MB file was **rejected (HTTP 413 Payload Too Large)** at the Cloudflare edge in front of ANY.RUN's API — direct confirmation that the overlay padding is an effective, real-world upload-evasion technique. The trimmed/de-padded executable (SHA256 `7d1749f809a7e2b037193f88af53e142d23b0cb65ed84146e769e9ab9dff45ab`, functionally identical — the overlay bytes are never mapped or read by the program) was submitted in its place.

- **Verdict score:** 100 / 100 — **"Malicious activity"**
- **Tags:** `auto-sch`, `auto-startup`, `auto-reg`, `upx` (corroborates the static persistence findings above)
- **IOC report:** Only benign Microsoft telemetry HTTP requests (reputation 0); no attacker C2 traffic captured in this run
- **Public report:** https://app.any.run/tasks/06e87957-f813-4e1e-8651-f95df974853d

## 8. Analyst Notes

- **Residual gap:** No actual C2 server address/domain was recovered by any method (static strings, speakeasy, or ANY.RUN dynamic run). Given the heartbeat/command-dispatch architecture found in code, the config is likely resolved at runtime through the full .NET CoreCLR execution path rather than being a static string — a longer-duration sandbox run (with operator-interaction simulation) or a manual CoreCLR-hosted dynamic run (rather than emulation) would be needed to observe it.
- **Alternative hypothesis considered and rejected:** that this is genuine Qilin ransomware. Rejected because (a) no encryption routine, ransom note, or file-extension-rename logic exists anywhere in the ~16.7 MB unpacked codebase, (b) the capability profile is a complete match for a surveillance/prank RAT rather than an encryptor, and (c) KesaKode's only meaningful signal (SeroRAT, RAT toolkit) is inconsistent with ransomware. The cert text is treated as social/troll signaling, not technical evidence.
- **Recommended follow-up:** If additional samples referencing "WindowsSysHost"/"WindowsSysHostUser" scheduled tasks or the same self-signed "Qilin Ransomware" certificate serial (`497d0beffcb738ad48b5ba922e361b52`) are seen, they should be treated as the same actor/builder output and cross-referenced directly (exact serial match or exact scheduled-task name match), per this workspace's cross-referencing policy. No existing tracked family in memory meets that bar for this sample, so it is filed standalone.
