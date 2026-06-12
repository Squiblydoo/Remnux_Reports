# Analysis Report: lM-lMAGE-iPhone-20260847329iWTaiQg31BW202WTaiN.SCR

**Date**: 2026-06-12  
**Analyst**: Claude Code (automated)

---

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | `lM-lMAGE-iPhone-20260847329iWTaiQg31BW202WTaiN.SCR` |
| SHA256 | `0ecf94aaad04c9bd55d2a41e809277e6c13f887b4d1edd94671aa76b986c646c` |
| MD5 | `430b4ab78fb5487d2789d67b272bf3cf` |
| SHA1 | `4e1eab3f2c484489e2fb86473c0990b7f586aef9` |
| Size | 237,384 bytes |
| Type | PE32 executable (GUI) Intel 80386, Windows |
| Extension | `.SCR` (Windows Screensaver — direct execution on double-click) |
| Import Hash | `1b461ed8d73479867e75976e9550dc52` |
| Compiler | MSVC 2022 (linker), build date 2026-04-17 04:22:17 |
| Sections | `.text`, `.rdata`, `.data`, `.fptable`, `.rsrc`, `.reloc` + overlay (12,104 bytes) |

### Certificate

| Field | Value |
|---|---|
| Issuer | Certum Extended Validation Code Signing 2021 CA (Asseco Data Systems S.A., PL) |
| Subject | 武汉伽跃寻信息咨询有限公司 (Wuhan Jiayuexun Information Consulting Co., Ltd.) |
| State/Locality | 湖北 / 武汉 (Hubei / Wuhan, CN) |
| Serial | `4c1d66965222ede4519cb068d9c93b28` |
| Validity | 2026-06-08 → 2027-06-08 (1-year term) |
| Algorithm | RSA / SHA256 |

### Version Info (spoofed)

| Field | Value |
|---|---|
| InternalName | `图书管理.exe` |
| ProductName | `图书管理系统` ("Library Management System") |
| FileDescription | `图书管理员的快速系统` ("Quick system for library administrators") |
| CompanyName | 武汉伽跃寻信息咨询有限公司 |
| FileVersion | 1.554.0.112 |

**Note:** Binary was compiled ~7 weeks before cert issuance (build 2026-04-17, cert 2026-06-08), suggesting pre-built payload signed with freshly acquired EV cert immediately before deployment.

---

## 2. Classification

**Family**: Key-Gated AES-256-CBC Shellcode Loader  
**Confidence**: **High** (confirmed same toolkit as previously analyzed sample; see cross-reference below)  
**Threat type**: Loader / dropper — executes encrypted shellcode payload when supplied a command-line key  
**Evasion strategy**: Without the correct key, the binary runs a functional library management console application as a decoy, producing no malicious behavior. Sandboxes and AV scanners that do not supply the key see benign software.

### Cross-Reference: Same Toolkit as `Real-time20screenshot2022-5-2026.scr`

The following specific indicators match a previously analyzed sample (SHA256 `68508736c04c34fd3ed97b5d7ca86cbb4ec12e2ae118fe1854f6813aa7706cf5`), meeting the strict cross-reference policy:

1. **Identical hardcoded decoy filenames** (config values match): Both samples' decoy path unconditionally creates/reads `userinfo.dat`, `bookinfo.dat`, and `recordinfo.dat`. These three specific internal filenames do not appear in known legitimate libraries.

2. **Identical key derivation algorithm** (build artifact match):
   ```
   key[i] = (cmdline[i % cmdline_len] XOR static_key[i % static_len]) + i * 7
   ```
   Both samples use this exact formula — same XOR then ADD structure, same multiplier constant (`7`). Only the embedded static key string differs: `"dro_coin_2136"` (prior) → `"TMsktG2U2ui46f2T"` (this sample).

3. **Identical execution gating pattern**: mutex check → cmdline non-empty gate → AES loader OR decoy console application, in the same code layout.

The different Certum certificate (vs DigiCert in the prior sample), different certificate subject entity, and different static key component are consistent with the same actor re-using a builder and rotating operational credentials.

**KesaKode online**: No family match (score below threshold — builder output, not a known named family).

---

## 3. Capabilities

- **Key-gated execution**: Checks if `lpCmdLine` is non-NULL and non-empty; only activates loader if key is present
- **Anti-rerun mutex**: Creates mutex `Local\` (anti-double-execution gate; exits if `ERROR_ALREADY_EXISTS`)
- **XOR pre-decryption**: XORs embedded encrypted blob with the command-line key character-by-character before AES decryption
- **AES-256-CBC shellcode decryption**: Uses `CryptAcquireContextA`/`CryptImportKey`/`CryptDecrypt` (WinCrypt API) with derived 256-bit key; IV = first 16 bytes of XOR-decrypted blob
- **Dynamic API resolution**: Resolves `NtAllocateVirtualMemory` and `NtProtectVirtualMemory` at runtime via `GetModuleHandleA("ntdll.dll")` + `GetProcAddress` — avoids import table detection
- **Shellcode allocation and execution**: Allocates RW memory via `NtAllocateVirtualMemory`, copies decrypted shellcode, sets RX via `NtProtectVirtualMemory`, then calls the shellcode directly
- **Functional decoy application**: When run without key, allocates a console window and runs a working library management system (reads/writes `userinfo.dat`, `bookinfo.dat`, `recordinfo.dat`)
- **Stack string obfuscation**: Several strings built on stack at runtime (capa: B0032.017)
- **Anti-debug**: `IsDebuggerPresent`, `IsProcessorFeaturePresent`, `RaiseException`, `UnhandledExceptionFilter`
- **XOR encoding** (10 loop instances): heavy use of XOR in loops throughout code

### Key Derivation Detail

```
cmdline_len = lstrlenA(lpCmdLine)
static_len  = lstrlenA("TMsktG2U2ui46f2T")  // = 16

for i in range(0x20):  // 32 bytes = AES-256 key
    key[i] = (cmdline[i % cmdline_len] XOR "TMsktG2U2ui46f2T"[i % 16]) + (i * 7)
```

The 32-byte derived key is imported via `CryptImportKey` with `CALG_AES_256` (0x6610).

---

## 4. Attack Chain

```
1. Delivery:  .SCR file (Windows screensaver) delivered via phishing/lure
              Filename impersonates iPhone photo: "lM-lMAGE-iPhone-20260847329..."
              EV cert (Certum/CN entity) lends trust indicator in Windows UI

2. Execution: Victim double-clicks → Windows executes .SCR as executable
              WinMain parses lpCmdLine

3a. No key:   → CreateMutex("Local\")
              → AllocConsole + display library management console (decoy)
              → Writes userinfo.dat, bookinfo.dat, recordinfo.dat
              → [EXIT — no malicious activity]

3b. With key: → CreateMutex("Local\")
              → Derive 32-byte AES key from cmdline + "TMsktG2U2ui46f2T"
              → Allocate RW buffer; copy encrypted payload from .data section
              → XOR pre-decrypt blob with cmdline
              → AES-256-CBC decrypt blob (IV = first 16 bytes)
              → NtAllocateVirtualMemory (RW)
              → Copy decrypted shellcode to new region
              → NtProtectVirtualMemory → PAGE_EXECUTE_READ
              → CALL shellcode (payload unknown — key not available)
```

**Delivery method**: The filename mimics a legitimate iPhone photo image filename with random-looking ID string and random characters (`iWTaiQg31BW202WTaiN`). The `.SCR` extension may be hidden by default Windows Explorer settings.

---

## 5. IOCs

### Network
None recoverable — shellcode payload encrypted; key not available. No C2 embedded in the loader itself.

### Filesystem
| Path | Purpose |
|---|---|
| `userinfo.dat` (current dir) | Decoy data file written by library management app |
| `bookinfo.dat` (current dir) | Decoy data file written by library management app |
| `recordinfo.dat` (current dir) | Decoy data file written by library management app |

### Registry
None observed.

### Mutex
| Name | Purpose |
|---|---|
| `Local\` | Anti-rerun mutex (exact full name may include trailing component not visible in decompiler) |

### Certificate
| Field | Value |
|---|---|
| Serial | `4c1d66965222ede4519cb068d9c93b28` |
| Subject entity | 武汉伽跃寻信息咨询有限公司 (Wuhan Jiayuexun Information Consulting Co., Ltd.) |
| CA | Certum Extended Validation Code Signing 2021 CA |

### Embedded Constant
| Value | Role |
|---|---|
| `TMsktG2U2ui46f2T` | Static XOR component in AES key derivation |

---

## 6. Emulation Results

**Speakeasy pass 1 (generic runner, x86)**: No IOCs. Emulator followed the no-cmdline path (decoy) — mutex created, decoy functions called, no shellcode decryption triggered.

**Speakeasy pass 2 (plain speakeasy, x86)**: Crashed on `FlsGetValue2` (unimplemented API stub) during CRT startup. No IOCs.

**Angr / manual decrypt**: Not attempted — key is operator-supplied at runtime; without it, the encrypted blob cannot be decrypted offline (AES-256-CBC, key uniquely derived from caller-controlled input).

**Assessment**: Emulation is fundamentally blocked by key-gating design. The loader is purpose-built to produce zero IOCs in automated sandbox environments that do not supply the key as a command-line argument.

---

## 7. Sandbox Results

| Field | Value |
|---|---|
| Platform | ANY.RUN |
| Task ID | `ba5736da-160f-41f3-885a-1b04b2661266` |
| Score | **20 / 100** |
| Verdict | **No threats detected** |
| Family tags | (none) |
| Behavioral IOCs | None — only Windows telemetry and OCSP certificate traffic observed |
| Public URL | https://app.any.run/tasks/ba5736da-160f-41f3-885a-1b04b2661266 |

**Interpretation**: The sandbox executed the decoy (no key supplied), confirming the evasion is effective against automated analysis. The 20/100 score reflects minor heuristic hits on the PE structure rather than behavioral detection.

---

## 8. Analyst Notes

### Residual gaps

1. **Shellcode payload unknown**: The second-stage shellcode (and its C2) cannot be recovered without the command-line key. The key is expected to be delivered out-of-band (e.g., embedded in the phishing message, encoded in a URL parameter, or stored in a separate file downloaded first). Recovery requires either:
   - Dynamic analysis with the key (intercept delivery chain)
   - Brute-force if key is short (unlikely for 16-char string)
   - Network capture from a live infection (if C2 communication occurs at delivery stage)

2. **Mutex full name**: The decompiler shows mutex name as `"Local\\"` but this may be a prefix; full name may include additional characters from a register not shown. Reversing the exact string would require disassembly of the CreateMutexA call site.

3. **Encrypted blob location**: The payload is embedded in the `.data` section at VA ~`0x429000` (roughly 2×0xA8=0x150 bytes for two XOR-mixed data blocks, plus size at `0x429540`). The blob + overlay (12,104 bytes PKCS7 Authenticode signature) are the only non-standard embedded content.

### Actor assessment

The Certum EV cert was issued to a Wuhan-based Chinese entity just 4 days before the sample was observed (cert: 2026-06-08, file date: 2026-06-12). The prior toolkit sample used a DigiCert EV cert from a Beijing entity. The pattern of acquiring short-term EV certificates from Chinese entities through different CAs to sign loader iterations is consistent with a recurring threat actor maintaining this loader family.

The `.SCR` delivery vector combined with an iPhone-photo filename lure suggests targeting of Chinese-speaking users via social engineering (WeChat/messaging app attachment).

### Recommended follow-up

- Monitor for additional `.SCR` samples using the same key derivation structure (`+i*7` constant, `TMsktG2U2ui46f2T` string, `userinfo.dat`/`bookinfo.dat`/`recordinfo.dat` filenames)
- Hunt for samples signed by the same Certum serial `4c1d66965222ede4519cb068d9c93b28`
- Attempt to identify delivery mechanism — if the key is in a URL or message, intercepting delivery infrastructure may recover the shellcode
- Write YARA rule targeting the key derivation loop and static key string
