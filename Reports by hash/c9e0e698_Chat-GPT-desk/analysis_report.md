# Malware Analysis Report: Chat-GPT-desk.exe

**Date:** 2026-05-22  
**Analyst:** REMnux / Claude

---

## 1. File Metadata

| Field | Value |
|-------|-------|
| Filename | Chat-GPT-desk.exe |
| SHA256 | `c9e0e6985dca3a179c9bdea4e7b38f7dc57fe00ecedc2fd634256fc53bf2de2d` |
| SHA1 | `9e804f99b3e93154e7d72ea02a9f89c61de3ca82` |
| MD5 | `0a948a321da0d2fb58c688f434ec7971` |
| File Type | PE32 executable (GUI) Intel 80386 — Inno Setup 5.5.7 installer |
| File Size | 75,989,392 bytes (~72.5 MB) |
| Build Timestamp | 2018-06-14 13:27:46 (Inno Setup stub — typical old date) |
| Compiler | Borland Delphi (SetupLdr) |

**Certificate:**
- Issuer: Microsoft ID Verified CS AOC CA 01 (3-day validity)
- Subject: NETWORK CONNECTIONS PROJECT SRL
- Details: Arges, Pitesti, Romania
- Valid: 2026-03-02 → 2026-03-05
- Serial: `33000820b3cd4e8e8e726f0c4c0000000820b3`
- Algorithm: SHA256/RSA

**Version Info:**
- FileDescription: `LeronApplication Setup`
- ProductName: `LeronApplication`
- ProductVersion: `1.0.0`
- Comments: `This installation was built with Inno Setup.`

**Embedded Payload (app.asar):**

| Field | Value |
|-------|-------|
| SHA256 | `9b8666ca4826da27356a28df478ae7ef3d87ac281b9c5ee975a340758b9870e7` |
| App Name | "Embody" |
| Version | 2.9.9 |
| Entry Point | `increase.js` |

**Electron Host (EApp.exe):**

| Field | Value |
|-------|-------|
| SHA256 | `83fb7c7408b374efc4122ff309038464385595142fba43aac1f029985ebc93eb` |
| Architecture | PE32+ x64 |
| ProductName | `Blood` |
| FileDescription | `Blood` |
| LegalCopyright | `Copyright © 2026 Blood` |
| CompanyName | `GitHub, Inc.` (spoofed) |
| Export | `electron.exe` |
| PDB | `electron.exe.pdb` |

---

## 2. Classification

**Malware Family:** Electron-based Stealer ("Embody" / "Blood")  
**Confidence:** High  
**Lure:** Fake ChatGPT desktop application

**Reasoning:**
- ANY.RUN verdict: 100/100 Malicious, tagged `stealer`
- Installs a custom-branded Electron app with a 1.97MB heavily obfuscated JS payload (`increase.js`)
- Payload dependencies confirm stealer intent: `systeminformation` (victim fingerprinting), `child_process` (command execution), `zip-lib` (data archiving), `https` (exfiltration)
- CAPTCHA UI shown as delay tactic; malicious IPC fires automatically after 20 seconds
- Silent/hidden installation (forces `/SILENT`, hides wizard window entirely)
- 3-day MS ID Verified cert from Romanian entity (NETWORK CONNECTIONS PROJECT SRL) for SmartScreen bypass
- Custom Electron binary branded "Blood" and "Embody" — actor-controlled build, not a legitimate application

---

## 3. Capabilities

- **Hidden silent install:** Inno Setup script forces `/SILENT` mode on first run; wizard window is immediately hidden via `ShowWindow(0)`. The installer is entirely invisible to the user.
- **CAPTCHA delay tactic:** Shows a captcha UI to the victim; `script.js` fires `launch-success` IPC after a hardcoded 20-second timeout regardless of CAPTCHA interaction, launching the malicious payload.
- **System fingerprinting:** `systeminformation` npm package enumerates CPU, RAM, processes, network adapters, disks, graphics, OS details, USB devices, users.
- **Command execution:** `child_process` module enables shell command execution.
- **Data archiving:** `zip-lib` module enables creation of ZIP archives, consistent with staging stolen data for exfiltration.
- **HTTPS exfiltration:** `https` module used for C2 communication (C2 URL encoded in obfuscated `increase.js`; not recovered statically).
- **Privilege escalation:** `resources/elevate.exe` bundled for UAC bypass capability.
- **Persistence:** Inno Setup installs to `%APPDATA%\LeronApplication\` without uninstaller (Uninstallable=no); likely uses EApp.exe startup mechanisms.

---

## 4. Attack Chain

```
User downloads "Chat-GPT-desk.exe" (ChatGPT lure)
    ↓
3-day MS ID Verified cert → SmartScreen bypass
    ↓
Inno Setup loader (Delphi, SetupLdr)
  • Forces /SILENT relaunch → hides wizard window
  • Installs all files to %APPDATA%\LeronApplication\
    ↓
Electron app launched (EApp.exe → "Blood")
  • Loads app.asar ("Embody" v2.9.9)
  • Entry point: increase.js (1.97MB, heavily obfuscated)
    ↓
captcha.html / captcha.js displayed
  • CAPTCHA is theater — 20-second timer fires launch-success IPC
    ↓
increase.js receives launch-success IPC
  • Uses systeminformation for victim fingerprinting
  • Collects data, archives with zip-lib
  • Exfiltrates via HTTPS to C2 (URL not recovered — encoded in string array)
```

---

## 5. IOCs

### Hashes

| File | SHA256 |
|------|--------|
| Chat-GPT-desk.exe | `c9e0e6985dca3a179c9bdea4e7b38f7dc57fe00ecedc2fd634256fc53bf2de2d` |
| app.asar (Embody) | `9b8666ca4826da27356a28df478ae7ef3d87ac281b9c5ee975a340758b9870e7` |
| EApp.exe (Blood Electron) | `83fb7c7408b374efc4122ff309038464385595142fba43aac1f029985ebc93eb` |

### Network

| Type | Indicator |
|------|-----------|
| C2 IP | `188[.]137[.]246[.]189` (Latvia, AS unknown) |
| C2 URL | `hxxp://188[.]137[.]246[.]189/laravel[.]php` |
| Protocol | HTTP GET, port 80 |
| Path | `/laravel.php?api=api&hash=<base64_victim_id>&message=<base64_exfil_data>` |

**Exfiltration format** (observed in ANY.RUN capture):
- Method: HTTP GET
- `api` parameter: literal `api`
- `hash` parameter: base64-encoded victim/session identifier (`PUlYYjQ1MmJ5Vkdi`)
- `message` parameter: large base64-encoded blob of exfiltrated system data
- Server responded HTTP 200 (data successfully received)
- Backend: Laravel PHP application

### Filesystem

- **Install directory:** `%APPDATA%\LeronApplication\`
- **Main executable:** `%APPDATA%\LeronApplication\EApp.exe`
- **JS payload:** `%APPDATA%\LeronApplication\resources\app.asar`

### Certificates

- Serial: `33000820b3cd4e8e8e726f0c4c0000000820b3`
- Subject: NETWORK CONNECTIONS PROJECT SRL, Arges, Pitesti, RO
- Validity: 2026-03-02 → 2026-03-05

### Build Artifacts

- App internal name: `Embody`
- Electron binary branding: `Blood` / `Copyright © 2026 Blood`
- Inno Setup AppId: `LeronApplication`

---

## 6. Emulation Results

**Speakeasy (Inno Setup loader, x86):** No IOCs. The Inno Setup stub is a standard Delphi loader; speakeasy does not emulate the LZMA decompression and Inno extraction chain.

**Dynamic (ANY.RUN):** The 20-second CAPTCHA delay prevented full payload execution within the sandbox window. No C2 connections observed beyond standard Windows telemetry (OCSP, Windows Update, Microsoft settings endpoints). Dropped file hashes from sandbox match the installed Electron app components.

---

## 7. Sandbox Results

- **ANY.RUN Score:** 100/100
- **Threat Level:** Malicious activity
- **Family Tags:** `stealer`
- **Public Report:** https://app.any.run/tasks/26bfe7cf-7a91-47df-a4ad-b6d990eb1d26

The sandbox correctly classified this as a stealer despite the CAPTCHA delay. No behavioral C2 indicators recovered due to the 20-second delay in the malicious IPC trigger.

---

## 8. Analyst Notes

**Obfuscation scheme:** `increase.js` uses a multi-layer obfuscation scheme: a custom base64 alphabet (lowercase-first: `abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789+/=`) over 3,036 encoded string entries in `_0x856d()`, with a rotation-verified shuffle and a secondary `NiVqpde` lookup array. The C2 IP was recovered via dynamic sandbox capture, not static analysis.

**Exfiltrated data encoding:** The `message` parameter value is a multi-layer base64 blob. A second decode layer (`==gCOJVVRNTMFJlQkh...`) appears to use the same custom alphabet, consistent with the JS obfuscation scheme — the exfiltrated data is re-encoded before transmission.

**`hash` parameter:** Likely a victim/session UUID or campaign key generated at install time. The value `PUlYYjQ1MmJ5Vkdi` is stable for a given install and functions as a victim tracking identifier.

**"Blood"/"Embody" actor branding:** The actor uses consistent internal branding (`ProductName="Blood"`, app name `"Embody"`) in their custom Electron build. These strings are useful pivots for identifying other samples from the same actor.

**CAPTCHA anti-sandbox:** The 20-second auto-trigger in `script.js` bypasses the CAPTCHA for legitimate users while causing the malicious payload to fire after most sandbox analysis windows have captured the install phase. This is a deliberate anti-sandbox technique.

**Lure theme:** ChatGPT desktop application lure. The installer name and product branding (LeronApplication) do not match, suggesting the actor uses a single installer template and distributes it under different filenames targeting different victims.

**No cross-reference to prior samples:** No prior-analyzed sample shares an identical cert serial, C2 domain, config value, build path, or payload hash with this sample.
