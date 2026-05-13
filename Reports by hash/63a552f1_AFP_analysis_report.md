# Malware Analysis Report — AFP.exe

**Date**: 2026-05-13  
**Analyst**: Claude (automated)

---

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | AFP.exe |
| SHA256 | `63a552f1683cdaf569b1d72c565524a3d783da996fa0d37076a6018c6a256e7f` |
| SHA1 | `6373c28cb93fbe0336c365ef2dd12da4e842ffa5` |
| MD5 | `d8dcbe7e930c86c93e63dfbdab7946ae` |
| Size | 44,504 bytes (43.4 KB) |
| File Type | PE32 executable (GUI) Intel 80386 Mono/.NET assembly |
| Target Framework | .NET Framework 4.6 |
| Architecture | x86 (32-bit) |
| Entropy | 98/100 (very high — heavily packed/encrypted sections) |
| Section Entropy | `.text`=93, `.rsrc`=92, overlay=137 |

### Certificate
| Field | Value |
|---|---|
| Issuer | Microsoft ID Verified CS AOC CA 03 |
| Subject | Benjamin Tillinger |
| Org Details | Benjamin Tillinger, Merritt Island, Florida, US |
| Serial | `330000cc0e1db7a3b7ec1f637800000000cc0e` |
| Validity | 2026-05-07 → 2026-05-10 (3-day MS ID Verified cert) |
| Algorithm | RSA/SHA256 |

### Build Artifacts
| Field | Value |
|---|---|
| PDB Path | `C:\Users\Jack\source\repos\WindowsFormsApp1\WindowsFormsApp1\obj\Release\AFP.pdb` |
| Debug Timestamp | 2055-03-22 15:16:27 (**FORGED** — far-future date) |
| PE Timestamp | Also in the future (**FORGED**) |
| VersionInfo | CompanyName=AFP, ProductName=AFP, FileVersion=2.0.3.1, InternalName=AFP.exe |
| Assembly GUID | `7ba69f94-97b5-4ccc-a0a8-35129b34e4e9` |
| .NET Module Name | AFP.exe |

---

## 2. Classification

**Family**: Custom Telegram-based RAT / Keylogger  
**Confidence**: High  

The malware is a self-contained .NET Windows Forms application that installs a persistent keylogger and provides full remote access via the Telegram Bot API. The architecture closely resembles the **ToxicEye** RAT family (KesaKode: ToxicEye 22%, AsyncRAT 18%) but the implementation appears custom or derived from a common open-source template. It is not meaningfully obfuscated — the full source was recovered via `ilspycmd` decompilation.

The 3-day Microsoft ID Verified certificate is a SmartScreen bypass technique — signing the binary with any valid authenticode certificate is sufficient to suppress the SmartScreen "unknown publisher" warning, even if the cert is not code-signing trusted by Windows Defender.

---

## 3. Capabilities

- **Keylogging**: Installs a global WH_KEYBOARD_LL hook via `SetWindowsHookEx`. Captures all keystrokes (including special keys [SPACE], [ENTER], [CTRL], [Shift], [WIN], [Tab], [ESC], F1–F24, CapsLock). Logs window titles as section headers. Writes to `%LOCALAPPDATA%\Microsoft\Windows\0\log.txt`.
- **Screenshot capture**: `CopyFromScreen` across all monitors (multi-monitor aware), saves as PNG bytes.
- **Remote command execution**: Any Telegram message not matching commands 1/2/3 is passed verbatim to `Process.Start()` as a shell command. Stdout/stderr returned to attacker.
- **Arbitrary file exfiltration**: Command `3;<path>` sends any file on the filesystem to the attacker via `sendDocument`.
- **Keylog exfiltration**: Command `2` sends `log.txt` via `sendDocument`.
- **Persistence**: Copies self to `%LOCALAPPDATA%\Microsoft\Windows\0\<filename>`, then creates a per-minute scheduled task named `User_Feed_Synchronization-{53CB5C5D-3094-4B43-9376-<random>}`.
- **Lure webpage**: Hidden WebBrowser control navigates to `https://idantre.com/index.html` on form load (shown to non-installed instances to distract the user).
- **Bot token hot-swap**: Command `2;<newtoken>` allows the attacker to replace the hardcoded bot token stored in `config.dll` at runtime, enabling infrastructure rotation without redeployment.
- **External IP discovery**: Queries `api.ipify.org` (observed in sandbox).
- **Single-instance / path check**: Uses mutex `Global\AFP10201` and checks if startup path matches install location. If already installed and running from the right path, exits immediately to avoid duplicate instances.

---

## 4. Attack Chain

```
1. DELIVERY
   AFP.exe delivered to victim (method unknown — likely phishing/social engineering)

2. FIRST RUN (from delivery location)
   └─ Mutex Global\AFP10201 created (createdNew=true)
   └─ CopySelfOnce() called:
       ├─ Creates %LOCALAPPDATA%\Microsoft\Windows\0\
       ├─ Copies AFP.exe there
       ├─ Registers schtask "User_Feed_Synchronization-{...}" (every 1 min)
       ├─ Launches installed copy
       └─ Opens hidden WebBrowser to idantre.com (lure for user)

3. SECOND RUN (from installed location, scheduled)
   └─ Mutex check: createdNew=false but path matches → full RAT activation:
       ├─ LoadBotToken() → hardcoded or config.dll token
       ├─ Thread 1: SetWindowsHookEx(WH_KEYBOARD_LL) → keylog to log.txt
       └─ Thread 2: r() — Telegram polling loop (getUpdates):
           ├─ Command "1"  → screenshot → sendPhoto to attacker's chat
           ├─ Command "2"  → send log.txt → sendDocument
           ├─ Command "3;<path>" → send arbitrary file → sendDocument
           ├─ Command "2;<token>" → update bot token in config.dll
           └─ Any other text → execute as shell command → send output

4. PERSISTENCE
   Scheduled task fires every 1 minute, relaunches AFP.exe from installed path
```

---

## 5. IOCs

### Network (defanged)

| Type | Value | Notes |
|---|---|---|
| C2 Domain | `api[.]telegram[.]org` | Telegram Bot API — all C2 traffic |
| Full C2 base URL | `https[://]api[.]telegram[.]org/bot5085618113:AAGZvs-8wrWE9pEugrXQW2-uFfDhLmDA7EE/` | Hardcoded in binary |
| Bot Token | `5085618113:AAGZvs-8wrWE9pEugrXQW2-uFfDhLmDA7EE` | Hardcoded; replaceable via command |
| Lure URL | `https[://]idantre[.]com/index.html` | Shown to distract user on first run |
| IP check | `https[://]api[.]ipify[.]org/?format=json` | External IP discovery |

**C2 Endpoints**:
- `GET /bot<token>/getUpdates?offset=<n>` — command polling
- `GET /bot<token>/getUpdates?timeout=30&offset=<n>` — long-poll variant
- `GET /bot<token>/sendMessage?chat_id=<id>&text=<msg>` — operator feedback
- `POST /bot<token>/sendPhoto` — screenshot exfiltration (multipart/form-data)
- `POST /bot<token>/sendDocument` — file exfiltration (multipart/form-data)

### Filesystem

| Path | Purpose |
|---|---|
| `%LOCALAPPDATA%\Microsoft\Windows\0\AFP.exe` | Installed copy of malware |
| `%LOCALAPPDATA%\Microsoft\Windows\0\log.txt` | Keylogger output |
| `%LOCALAPPDATA%\Microsoft\Windows\0\config.dll` | Replaceable bot token storage |

### Mutex
- `Global\AFP10201`

### Scheduled Task
- Name pattern: `User_Feed_Synchronization-{53CB5C5D-3094-4B43-9376-<random_int>}`
- Trigger: every 1 minute
- Action: `%LOCALAPPDATA%\Microsoft\Windows\0\AFP.exe`

### Certificate (for blocking/hunting)
- Serial: `330000cc0e1db7a3b7ec1f637800000000cc0e`
- Subject: Benjamin Tillinger

---

## 6. Emulation Results

Speakeasy emulation was not run — the binary is a .NET assembly requiring the .NET runtime, and `ilspycmd` provided complete source decompilation (full C# source recovered), making emulation redundant for IOC extraction. All C2 URLs, tokens, paths, and mutex names were recovered through static analysis.

---

## 7. Sandbox Results (ANY.RUN)

| Field | Value |
|---|---|
| Task ID | `60164028-c10e-407a-a9c0-67bc2c3dda4b` |
| Score | 35/100 |
| Verdict | No threats detected |
| Tags | `evasion` |
| Public URL | https://app.any.run/tasks/60164028-c10e-407a-a9c0-67bc2c3dda4b |

**Assessment**: The low sandbox score is expected — the malware checks its startup path against the installed location (`%LOCALAPPDATA%\Microsoft\Windows\0\`) and only activates the RAT when run from there. In the ANY.RUN sandbox, the first execution opens the Form1 lure (navigates to `idantre[.]com/index.html`) but does not activate keylogging or the Telegram polling loop. The Telegram C2 was not contacted during sandboxing. The `evasion` tag reflects the path-check anti-analysis technique.

**Sandbox-observed network activity**:
- DNS: `idantre.com` (lure)
- HTTPS: `idantre[.]com/index.html`
- HTTPS: `api.ipify[.]org/?format=json` (external IP check)
- Various Microsoft OCSP/CRL/telemetry (noise)

---

## 8. Analyst Notes

### Threat Assessment
This is a fully functional keylogger and remote access tool. The complete source is available for analysis — no obfuscation was applied beyond the 3-day fraudulent Authenticode certificate. The attacker's Telegram bot token is hardcoded and exposed: `5085618113:AAGZvs-8wrWE9pEugrXQW2-uFfDhLmDA7EE`. Sending a `getMe` request to this token would reveal the bot's username and confirm if it is still active.

### Bot Token Abuse
If the bot token remains active, the attacker can be attributed via the bot owner's Telegram account. The `getUpdates` endpoint could be queried to review pending messages. **Note**: Do not interact with the bot from infrastructure that can be attributed — use a controlled environment.

### Developer Fingerprinting
The PDB path (`C:\Users\Jack\source\repos\WindowsFormsApp1\`) and the misspelled debug strings (`"wating anser ..."`, `"Acces Old Toekn"`) suggest a non-native English speaker operating under the username "Jack" on their development machine.

### Lure Domain
`idantre[.]com` appears to be a genuine domain unrelated to the malware infrastructure — the malware simply navigates there to distract the victim during the first-run installation. This is not a malicious domain under attacker control.

### Residual Gaps
- The Telegram chat ID used by the attacker to receive data is not embedded in the binary — it is discovered at runtime when the attacker messages the bot. Querying `getUpdates` would reveal this.
- The delivery mechanism is unknown.
- Whether the scheduled task name suffix `53CB5C5D-3094-4B43-9376-` is constant (hunting value) or randomly generated per install requires behavioral confirmation — the code uses `new Random().Next()` for the final portion only.

### ATT&CK Techniques
`T1056.001` (Keylogging) · `T1113` (Screen Capture) · `T1053.005` (Scheduled Task) · `T1036.001` (Invalid Code Signature) · `T1553.002` (Code Signing) · `T1071.001` (Application Layer Protocol: Web Protocols) · `T1102` (Web Service — Telegram as C2) · `T1059` (Command and Scripting Interpreter) · `T1083` (File and Directory Discovery) · `T1057` (Process Discovery) · `T1105` (Ingress Tool Transfer) · `T1547.001` (Registry Run Keys / Startup Folder — scheduled task variant) · `T1082` (System Information Discovery) · `T1010` (Application Window Discovery) · `T1497.001` (Virtualization/Sandbox Evasion: System Checks — path check)
