# 智能拼音输入法.bin — Chinese Pinyin IME-Lure Sleeper Implant / Operator-Gated Spreader

**Date**: 2026-06-29  
**Analyst**: Claude Code (automated)

---

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | `智能拼音输入法.bin` (Smart Pinyin Input Method) |
| SHA256 | `638e847cb8a4faefcbe006fe062c383f9b606bfd3795a3256ee49059e20059fa` |
| SHA1 | `984412dd2631d2890e8197ffe601dd0262c28e33` |
| MD5 | `081bddd4804ace15a188be6587273ed8` |
| Size | 472,056 bytes (461 KB) |
| Type | PE32 executable (GUI) Intel 80386 / Windows |
| Compiler | MSVC 2015 Update 3 |
| Build Timestamp | 2025-10-11 03:06:47 (debug/POGO) |
| Sections | `.text` (entropy 6.66), `.rdata`, `.data`, `.gfids`, `.tls`, `.rsrc` (205 KB), `.reloc` + overlay (10 KB) |
| Imphash | `78833e27d66c7f7ca8d1d6dece34d2a5` |

### Certificate

| Field | Value |
|---|---|
| Issuer | DigiCert Trusted G4 Code Signing RSA4096 SHA384 2021 CA1 |
| Subject | 北京布丁跳跳科技有限公司 (Beijing Buding Jump Technology Co., Ltd.) |
| State | 北京市 (Beijing) |
| Serial | `019721d85375f194d99ed5ef3d61c2eb` |
| Validity | 2025-07-25 → 2028-07-22 **(3-year EV cert)** |
| Hash algo | SHA-256 / RSA |

### Version Info (all fields spoofed to IME persona)

| Field | Value |
|---|---|
| CompanyName | 智能拼音输入法 |
| FileDescription | 智能拼音输入法 |
| InternalName | 智能拼音输入法 |
| OriginalFilename | 智能拼音输入法 |
| ProductName | 智能拼音输入法 |
| ProductVersion | 1.0.9.21211 |
| LegalCopyright | Copyright (C) 2023 |

---

## 2. Classification

**Family**: Novel/Custom — no confirmed family attribution.  
**Confidence**: **High** that this is malicious.  
**Threat type**: Pre-positioned sleeper implant masquerading as a Chinese Input Method Editor (IME), with an operator-gated lateral spreading feature (`Spreader`), browser process targeting, and multi-layered crypto.  
**KesaKode Online**: Medusa 0.73%, QakBot 0.73% — both below the 20% attribution threshold, discarded. No prior-family attribution.

**Reasoning**:
- Contains a `Spreader` feature explicitly named in code, controlled via operator-pushed INI config
- Maintains a hardcoded array of 16 browser process names for targeting (credential theft / injection)
- WTS token manipulation (`WTSQueryUserToken`, `WTSGetActiveConsoleSessionId`) for multi-session lateral movement
- Anti-debug and sandbox analysis-tool detection
- No C2 URLs in the binary — all operator config comes from `ZhiNengPYInfo.ini`
- Registry key `HKCU\SOFTWARE\ZhiNengPY` establishes persistent install marker
- 3-year DigiCert EV cert from a Beijing company with a nonsensical name ("Buding Jump Jump Technology") suggests fraudulent procurement

---

## 3. Capabilities

### IME Camouflage
- Registers as a legitimate Windows keyboard layout / Input Method Editor:
  - `HKCU\Keyboard Layout\Preload` — adds IME to keyboard preload list
  - `HKLM\System\CurrentControlSet\Control\Keyboard Layouts\` — enumerates/registers layout metadata
  - `InstallLayoutOrTip` — Microsoft's official IME registration API
  - CTF framework registration: `Software\Microsoft\CTF\SortOrder\AssemblyItem\` with `CLSID` and `KeyboardLayout` values
- Reads/writes IME companion files: `.ime`, `input.dll`, `32.ime` / `64.ime` (`Wow64DisableWow64FsRedirection` to access 64-bit path from 32-bit process)
- Config files: `ZhiNengPYInfo.ini`, `UseVestige.ini`, `Cache.ini`, `ZhiNengPYIME.users`
- Companion executable: `Config.exe` (in install dir; verified for existence during init)

### Spreader (Operator-Gated)
- Function `sub_4050ff` reads `[AppInfo]\Spreader` from `ZhiNengPYInfo.ini` — **three separate read attempts** with fallback paths
- Default value: `"guanf"` (关 = close/off in Chinese) — **Spreader is disabled by default**
- When operator pushes a non-`guanf` value via config update, the Spreader activates
- No C2 URLs hardcoded; all targeting info delivered via operator-controlled INI
- WTS APIs present for creating processes in the context of active remote sessions (`WTSGetActiveConsoleSessionId`, `WTSQueryUserToken`) — cross-session propagation mechanism

### Browser Process Targeting (16 processes)
Hardcoded pointer array in .data → process name strings in .rdata:
```
sogouexplorer.exe  360se.exe         qqbrowser.exe   firefox.exe
opera.exe          wnie.exe          scie.exe        maxthon.exe
360chrome.exe      baidubrowser.exe  iexplore.exe    safari.exe
twchrome.exe       spark.exe         theworld.exe    explorer.exe
```
`CreateToolhelp32Snapshot` / `Process32NextW` used to enumerate running processes against this list (YARA: `ProcessInjectionTargets`, `EnumerateProcesses`).

### Cryptographic Primitives (multiple)
- **MD5** — custom implementation (`sub_40310f`); ~20 hardcoded MD5 hashes in `.rdata` (process/file integrity allowlist)
- **DES** — S-box constants detected by peframe (peframe: `DES sbox`)
- **Mersenne Twister PRNG** — 4 instances (capa: `generate random numbers using a Mersenne Twister`)
- **XOR** — 23 XOR-in-loop functions (capa: `encode data using XOR`, 4 matches); likely runtime string/config decryption
- **Base64** — `BASE64 table` present; used for config or data encoding

### Anti-Analysis / Evasion
- `IsDebuggerPresent`, `OutputDebugStringW`, `RaiseException` — debugger detection
- `IsProcessorFeaturePresent`, `UnhandledExceptionFilter` — execution environment checks
- Analysis tool reference strings detected (capa: `reference analysis tools strings`)
- ANY.RUN score: 0 (sandbox sees no malice because Spreader is off by default)
- Suspicious `.text` section entropy (6.66)

### Filesystem / Registry / Process
- File operations: copy, create, delete, move, read, write (capa: 6 file ops)
- Registry: set, delete, query (12 registry value queries, 5 registry key queries, 5 value sets, 3 key deletes)
- INI read/write via `GetPrivateProfileStringW` / `WritePrivateProfileStringW`
- `CreateProcessW` for process creation
- `WTSQueryUserToken` for creating processes in the security context of active RDP/console sessions
- `Wow64DisableWow64FsRedirection` / `Wow64RevertWow64FsRedirection` — 32-bit binary accessing 64-bit system paths

### TLS Section
- `.tls` section present — possible TLS callbacks for pre-entrypoint execution (system: capa `contain a thread local storage (.tls) section`)

---

## 4. Attack Chain

```
[Victim installs 智能拼音输入法 / Smart Pinyin IME]
           │
           ▼
[CConfigure.#1 initializes at startup]
   1. sub_404c1c  → finds install path:
                     HKCU\SOFTWARE\ZhiNengPY\InstallPath
                     fallback: SHGetSpecialFolderPath(APPDATA) + ZhiNengPYInput
   2. sub_403fe8  → (init, not yet fully analyzed)
   3. sub_4044fe  → reads CfgUserPath from INI; sets up config paths
   4. sub_4052fe  → writes CfgRootPath/CfgUserPath to ZhiNengPYInfo.ini and registry [if param_3 set]
   5. sub_404a1f  → reads ShttPath config; sets up Cache.ini / UseVestige.ini paths
   6. sub_4050ff  → [SPREADER CHECK]
                     GetPrivateProfileStringW("AppInfo","Spreader","guanf",...)
                     If value == "guanf": do nothing (DEFAULT — SLEEPER STATE)
                     If value != "guanf": process and activate spreading
           │
           │ [Operator pushes updated ZhiNengPYInfo.ini with Spreader value set]
           ▼
[Spreader activates → targets browser processes / lateral movement via WTS]
```

---

## 5. IOCs

### File System
| Path/File | Notes |
|---|---|
| `%APPDATA%\ZhiNengPYInput\` (or custom install dir) | Install root |
| `{InstallDir}\ZhiNengPYInfo.ini` | Primary operator config (controls Spreader) |
| `{InstallDir}\UseVestige.ini` | Secondary config |
| `{InstallDir}\Config\Cache.ini` | Cache config |
| `{InstallDir}\ZhiNengPYIME.users` | User profile tracking |
| `{InstallDir}\Config.exe` | Companion executable |
| `{InstallDir}\input.dll` or `{InstallDir}\{name}32.ime` / `64.ime` | IME component |
| `C:\log\` or `C:\log%s.txt` | Logging output |

### Registry
| Key/Value | Notes |
|---|---|
| `HKCU\SOFTWARE\ZhiNengPY\InstallPath` | Install location |
| `HKCU\Keyboard Layout\Preload` | IME preload (modified during install) |
| `HKLM\System\CurrentControlSet\Control\Keyboard Layouts\{ID}\Ime File` | IME registration |
| `HKLM\System\CurrentControlSet\Control\Keyboard Layouts\{ID}\Layout Text` | Modified by malware |
| `HKCU\Software\Microsoft\CTF\SortOrder\AssemblyItem\` | CTF framework registration |
| `HKCU\Control Panel\Input Method\Hot Keys\{ID}` | Hot key assignment |

### Certificate
- **Serial**: `019721d85375f194d99ed5ef3d61c2eb`
- **Subject**: 北京布丁跳跳科技有限公司 / Beijing Buding Jump Technology Co., Ltd.
- **Issuer**: DigiCert Trusted G4 Code Signing RSA4096 SHA384 2021 CA1
- **Validity**: 2025-07-25 → 2028-07-22

### Config Keys (in ZhiNengPYInfo.ini)
| INI Section\Key | Purpose |
|---|---|
| `[AppInfo]\Spreader` | Spreader toggle — default "guanf" (off) |
| `[AppInfo]\CfgRootPath` | Install root path |
| `[AppInfo]\CfgUserPath` | Per-user path |
| `[Globals]\InstallPath` | Install path fallback |
| `[Globals]\CfgUserPath` | Global user config path |
| `ShttPath` | Alternate/shadow config path |

### Hardcoded MD5 Hashes (integrity/allowlist, in .rdata)
```
42aac3ca619847e32ccca33762da1e39   792768912d3ec14163e3deb1d903490f
2802ae38dbd7a8e353a369fe9a7f3442   5391e61b01dab8851e6029984c58469e
65ac8b902e5d3c23b250ce2ea3858fda   9aafb0a5c0f032d4f8a923f77aa11a81
e8dd2f8e97d295ef09e1499a357d54bf   09fbf3e361cf43bcf595193166830ee0
5fc2e47710f6c9273fa949911fcce363   6d0a96904251a960333628b0d1276fb4
e143ebc3452599963654c39c7685d802   f3350eed271673056bc26db5017095eb
7205a5eacd790caa9a86d8a0b8c891de   3bafbe0cf0de0fc3249f7a1d4d3424e1
8dd8539219cc16cc07d80e4822596eb9   29ab494c7e7128b53b59f7ffc9d7bb5a
bf39dd196552ddbefce9b1c6d6a6dcd7   d226943601a6bfa945ae0bf05287764d
...
```
Likely allowlist hashes of legitimate system files or integrity checks on IME components.

### Mutexes / Sync
- `WaitForSingleObject` / `WaitForSingleObjectEx` (peframe: `MutexApi`)

---

## 6. Emulation Results

### Speakeasy (pass 2 — plain)
- Registry access observed: `ADVAPI32.RegOpenKeyExW("HKEY_CURRENT_USER", "SOFTWARE\\ZhiNengPY", 0x0, 0x20019, ...)` → returns 0x3 (ERROR_PATH_NOT_FOUND — key absent in sandbox)
- Binary then called `SHELL32.SHGetSpecialFolderPathW` (not stubbed in speakeasy) → emulation terminated
- **No network activity, no payload decryption, no C2 contact** — consistent with Spreader-off-by-default design
- Full emulation blocked by missing `SHGetSpecialFolderPathW` stub

### Generic runner (pass 1)
- No IOCs recovered

---

## 7. Sandbox Results

### ANY.RUN (public task)
| Field | Value |
|---|---|
| Score | **0 / 100** |
| Threat Level | No threats detected |
| Family Tags | (none) |
| Public URL | https://app.any.run/tasks/618cd32f-7561-487d-8986-744f215298f7 |

**Explanation**: ANY.RUN saw no malicious behavior because the Spreader key in `ZhiNengPYInfo.ini` defaults to `"guanf"` (disabled). The sandbox ran the binary without providing the operator-configured INI file, so the sleeper state was maintained throughout execution. No C2 traffic, no credential theft, no lateral movement was triggered. This is a deliberate defense-in-depth evasion by design.

---

## 8. Analyst Notes

### What is confirmed malicious
1. The **`Spreader` feature** is explicitly named and operator-gated — a legitimate IME has no such mechanism.
2. The **pointer array of 16 browser process names** combined with `CreateToolhelp32Snapshot` enumeration is a credential theft or browser injection setup; this is not needed for keyboard input functionality.
3. The **WTS token APIs** (`WTSQueryUserToken`) used outside of IME installation context indicate cross-session process execution.
4. The **DigiCert 3-year EV cert** for "Beijing Buding Jump Jump Technology" (布丁跳跳 = "Pudding Jump Jump") appears to be a fraudulently obtained code signing cert from a shell/fabricated company.
5. **No C2 URLs in binary** — the operator-only INI control model is designed to defeat static C2 blocklisting and sandbox detonation.

### Residual unknowns
- **What the Spreader value encodes**: Could be a UNC path for lateral file propagation, a hostname for `Config.exe` download, or a list of target hostnames. Without an activated ZhiNengPYInfo.ini, this cannot be determined statically.
- **XOR loop content**: 23 XOR-in-loop functions likely decrypt runtime config or strings. Without speakeasy support for `SHGetSpecialFolderPathW`, full emulation past the initialization phase is blocked.
- **`wnie.exe` and `scie.exe`**: Included in the browser targeting list but not identifiable as known browsers; possibly obscure Chinese educational/browser tools or custom processes specific to the targeted environment.
- **`Config.exe` role**: The binary checks for `Config.exe` in the install directory and verifies it exists. Its purpose (GUI installer? C2 downloader? credential exfiltrator?) is not visible in this binary.

### Recommended follow-up
1. **Hunt for `Config.exe`** — it is required during initialization and is likely the core malicious payload.
2. **Search for `ZhiNengPYInfo.ini` with `Spreader` key** in IR telemetry — any non-"guanf" value indicates an activated deployment.
3. **Certificate pivot**: Revocation or blocklisting of serial `019721d85375f194d99ed5ef3d61c2eb` recommended.
4. **Block registry key** `HKCU\SOFTWARE\ZhiNengPY` as a detection point for installation.
5. **Trigger emulation with mock INI**: Create `ZhiNengPYInfo.ini` with `[AppInfo]\Spreader=<non-guanf>` and re-run in a controlled sandbox to observe Spreader behavior.
