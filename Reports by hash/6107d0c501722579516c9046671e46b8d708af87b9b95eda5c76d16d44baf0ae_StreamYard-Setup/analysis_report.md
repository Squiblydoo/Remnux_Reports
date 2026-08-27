# Analysis Report: Fake "StreamYard" Desktop Installer

## 1. File Metadata

| Field | Value |
|---|---|
| Filename (as submitted) | `6107d0c501722579516c9046671e46b8d708af87b9b95eda5c76d16d44baf0ae` |
| SHA256 | `6107d0c501722579516c9046671e46b8d708af87b9b95eda5c76d16d44baf0ae` |
| SHA1 | `097cad21536a3371f89e0e705623172a8383ca8b` |
| MD5 | `111e0d89704ee2c4723bc893fbc3718d` |
| File type | PE32 (GUI) Intel 80386, Nullsoft Installer (NSIS) self-extracting archive, 5 sections |
| Size | 111,950,896 bytes (~112 MB); NSIS stub is 1,807,360 bytes, remainder (~111.4 MB) is a `7z`-compressed overlay (`app-64.7z`, electron-builder payload) |
| Claimed product | "StreamYard" v4.5.6 desktop installer |
| PDB / internal name | package.json `name`: `strm-launcher`, `productName`: `StreamYard`, `author`: `StereamYard` (note the transposed "Steream" typo) |
| App identity GUID | `d2f1e512-6dc8-537e-b8a5-07a2bb9c9187` (electron-builder app-id derived registry key) |

### Code signing

| Field | Value |
|---|---|
| Subject | Xiamen Weixiang Animation Design Co., Ltd. (CN, Fujian Sheng) |
| Issuer | Sectigo Public Code Signing CA EV R36 |
| Serial | `02480adfcaef3ffecd368d261f0f83fa` |
| Validity | 2026-03-24 → 2027-03-24 |
| Hash/Crypto | SHA256 / RSA |

**Signer/product mismatch**: the binary's VersionInfo claims `CompanyName=StereamYard`, `ProductName=StreamYard`, `LegalCopyright=(c) StreamYard. All Rights Reserved` — but the EV code-signing certificate actually backing the signature belongs to an unrelated Chinese entity, "Xiamen Weixiang Animation Design Co., Ltd." Real StreamYard (streamyard.com, owned by Hopin/RingCentral) is a browser-based live-streaming SaaS product with **no official native Windows desktop client** — this executable's entire premise (a "StreamYard desktop app") does not correspond to any real product.

## 2. Classification

**Fake/trojanized SaaS installer — credential-phishing lure, no confirmed known-family match.**
Confidence: **High** (based on direct source-code inspection of the bundled Electron app; not derived from YARA/heuristic scoring).

- **Offline KesaKode**: empty (`kesakode_verdict: []`) — no match.
- **Online KesaKode**: query returned zero hits (no family or capability matches at all, verbose mode produced no output) — no code-sharing signal with any tracked family.
- **Malcat YARA**: only generic hits — `NsisInstaller`, `Zlib`, `ElevatePrivileges` (UAC/AdjustTokenPrivileges, standard for any modern installer), `ValuableFileExtensions` (34 patterns — **assessed as a false positive**: the matched strings (`doc`, `pdf`, `ppt`, `mp4`, `7z`, `avi`, `sql`, `png`, etc.) are scattered noise from the internal file-listing/dictionary tables of the compressed 7z overlay, not a coherent ransomware/stealer targeting list — no code path references them).
- **capa**: refused to analyze meaningfully ("this sample appears to be an installer"; capa cannot unpack NSIS/7z).
- **peframe**: only generic NSIS-stub features (mutex, antidbg via `FindWindowExW`/`GetLastError`, XOR = LZMA/BZIP2 decompression, escalate-priv = UAC prompt) — consistent with an unmodified `electron-builder` NSIS installer template, nothing bespoke.

The installer script (`setup.nsis`, decompiled via Binary Refinery `xtinno`) is a **byte-for-byte standard electron-builder NSIS template** (GUID-keyed uninstall registry, `KeepShortcuts`, PowerShell-based process enumeration via `Get-CimInstance`/`tasklist`, `strm-launcher-updater` self-update staging). No injected/custom installer logic was found — the entire deceptive payload lives in the **bundled Electron application itself**, not the installer mechanics.

### The "app" is a fake login/signup flow, not a functioning product

Decompiling `resources/app.asar` (via `asar`) shows:
- **Main process** (`out/main/index.js`, 113 lines): creates a window, loads a **fully local, bundled** `renderer/index.html` (it never loads `streamyard.com` or any remote URL in production), sets `AppUserModelId = com.streamyard.app`, registers the `streamyard-join://` protocol handler, and initializes a "Sentry" client:
  ```js
  var SYSTEM_INFO = "4a10e09b7ab34648bf610500dd73c69f";
  new Sentry({
    key: SYSTEM_INFO,
    projectName: "StreamYard",
    dsn: atob("aHR0cHM6Ly9zdHJlYW15YXJkLmV1LmNvbS9hcGkvbGF1bmNoZXI=")   // https://streamyard.eu.com/api/launcher
  }).init()
  ```
  The DSN decodes to `streamyard.eu.com` — **not** the real `streamyard.com` domain. This is a lookalike/typosquat domain that beacons on every app launch with a "SYSTEM_INFO" identifier (host/install fingerprinting), disguised as ordinary crash telemetry.
- **Renderer** (`out/renderer/assets/index-*.js`, ~23.6k lines, React/Radix/Tailwind/Zustand stack — genuinely well-built, not a crude phishing kit): implements a pixel-perfect `Log in` / `Create your account` / `Initiate Recovery` / `OTP` flow.
  - `LogIn` and `CreateAccount` collect **email + password** into a client-side form; on submit they `setIsLoading(true)`, wait a randomized 5–7 second fake "processing" delay (`getRandom(5e3,7e3)`), then navigate to an OTP entry screen.
  - `OTP.onSubmit()` **always fails** after another random delay: `form.setError("otp", { message: "Code is invalid or expired. Try requesting a new code." })` — there is no real backend call anywhere in this flow (no `fetch`/`axios`/`XMLHttpRequest`/`ipcRenderer.invoke` was found servicing the login, signup, or OTP forms). The password field's value is discarded — only the `email` is retained in the in-memory Zustand store (`useStore`), and no network transmission of it was found in this build either.
  - The UI borrows real StreamYard branding details (`support.streamyard.com` help-center links, `yourfriends@streamyard.com`) to increase credibility.

**Assessment**: this is a deceptively realistic **fake-software / credential-harvesting lure** — a convincing "StreamYard desktop client" that does not perform any real function, presents an always-failing login funnel designed to capture victim emails (and elicit repeated OTP entry, useful for testing stolen credentials/2FA codes on the real service in a separate step, or simply to fatigue and stall the victim), and silently beacons a host fingerprint to an attacker-controlled domain typosquatting the real brand. No credential exfiltration network call was located in *this specific build* — either it has not yet been wired up, is gated behind a code path not reached statically, or is intended to ship in a later "connected" build. The `$LOCALAPPDATA\strm-launcher-updater\installer.exe` auto-update staging (standard electron-builder behavior) is a plausible mechanism for silently delivering an updated/fully-armed build later.

## 3. Capabilities

- Presents a fake StreamYard-branded desktop application (Electron/Chromium, ~150MB installed) with no genuine streaming functionality.
- Simulated login, account-creation, password-recovery, and OTP verification screens that collect email + password but never actually authenticate (OTP always reports "invalid or expired").
- Beacons a fixed host/install identifier to a typosquat domain (`streamyard.eu.com`) disguised as Sentry crash telemetry, on every app launch.
- Registers `streamyard-join://` custom URL protocol handler (mirrors real StreamYard's meeting-join links — plausible social-engineering vector for follow-on lures).
- Standard electron-builder self-update staging to `%LOCALAPPDATA%\strm-launcher-updater\installer.exe` — a mechanism through which a future/updated build could be silently delivered.
- Installer performs legitimate-looking install/uninstall registry management, shortcut creation, and running-instance termination (`Get-CimInstance`/`tasklist`+`taskkill`) — all stock electron-builder NSIS behavior, not bespoke.

## 4. Attack Chain

1. Victim downloads/runs a fake "StreamYard Setup 4.5.6.exe" (likely via search-ad/SEO poisoning or a fake download site, consistent with prior fake-installer campaigns in this workspace's dataset — not confirmed here since no download source was provided).
2. EV-signed NSIS installer runs, extracts and installs a full Electron app to `%LOCALAPPDATA%`/Program Files under the "StreamYard" name, creates shortcuts, registers uninstall entries.
3. On first launch, the app phones home to `streamyard.eu.com/api/launcher` with a fixed install fingerprint.
4. Victim is presented with a realistic account login/creation screen and enters credentials, believing they are signing into their real StreamYard account.
5. App simulates processing then always demands a 6-digit OTP, which always "fails" — victim is stalled/frustrated with no functioning product ever delivered.
6. (Unconfirmed) Any real credential exfiltration or additional payload delivery would occur via a mechanism not present in this specific build (e.g., a future update via the staged `strm-launcher-updater`).

## 5. IOCs

### Network
- `streamyard[.]eu[.]com` — typosquat domain, Sentry-disguised beacon endpoint; resolves via Cloudflare to `172.67.155.68`, `104.21.72.213` (distinct Cloudflare account from the real `streamyard[.]com`, which resolves to `104.18.12.37`/`104.18.13.37`)
- `hxxps[://]streamyard[.]eu[.]com/api/launcher` — beacon/telemetry URL (base64-encoded in the binary: `aHR0cHM6Ly9zdHJlYW15YXJkLmV1LmNvbS9hcGkvbGF1bmNoZXI=`)

### Filesystem
- `%INSTDIR%\streamyard.exe`
- `%INSTDIR%\Uninstall streamyard.exe`
- `%LOCALAPPDATA%\strm-launcher-updater\installer.exe`

### Registry
- `Software\d2f1e512-6dc8-537e-b8a5-07a2bb9c9187` (InstallLocation, KeepShortcuts, ShortcutName)
- `Software\Microsoft\Windows\CurrentVersion\Uninstall\d2f1e512-6dc8-537e-b8a5-07a2bb9c9187` (DisplayName="StreamYard", Publisher="StereamYard")

### Certificate
- Serial `02480adfcaef3ffecd368d261f0f83fa` — Sectigo EV, subject "Xiamen Weixiang Animation Design Co., Ltd.", valid 2026-03-24 to 2027-03-24

### Other identifiers
- Custom URL protocol: `streamyard-join`
- Fixed "Sentry" project key / host fingerprint constant: `4a10e09b7ab34648bf610500dd73c69f`
- AppUserModelId: `com.streamyard.app`
- npm package name: `strm-launcher`

## 6. Emulation Results

Not performed. The installer is a stock electron-builder NSIS stub whose only "logic" is file extraction and standard install bookkeeping (fully recovered via static extraction/decompilation of the NSIS script and the bundled Electron app source — a more complete and reliable picture than emulation could provide for this file type). No decrypt/obfuscation routines were present to justify angr/speakeasy follow-up.

## 7. Sandbox Results

**ANY.RUN: submission failed.** The file (111,950,896 bytes / ~112 MB) exceeds the account's upload size limit (`"exceeds the limit of size bytes"`). Static analysis (full decompilation of the installer script and the bundled Electron application source) was used in its place and is considered sufficient for classification given the malicious/deceptive logic was directly recovered in source form.

## 8. Analyst Notes

- **Novel sample** — zero KesaKode matches (online and offline), no YARA family hits, not part of any tracked campaign in this workspace's memory. No cross-referencing criteria (identical cert serial, matching C2, matching config/key, identical build artifact, identical payload hash) were met against any previously analyzed sample.
- The credential-harvesting logic is unusually well-engineered compared to typical crude phishing kits — full TypeScript/React source structure, proper form validation (zod schemas), realistic UX pacing (randomized delays) — suggesting a deliberate, resourced effort rather than an opportunistic script-kiddie build.
- **Gap**: no code path was found that actually transmits the captured email/password anywhere. Recommended follow-up if a live download source or newer build of this installer surfaces: diff the renderer bundle for an added `fetch`/`ipcRenderer.invoke` call in the login/signup/OTP handlers, and monitor `streamyard.eu.com/api/launcher` for any evidence of a POST-based exfil parameter beyond the launch beacon.
- Recommend blocking `streamyard.eu.com` and flagging any further code-signing certificates issued to "Xiamen Weixiang Animation Design Co., Ltd." (serial `02480adfcaef3ffecd368d261f0f83fa`) as an IOC for pivoting to related/future builds of this campaign.
