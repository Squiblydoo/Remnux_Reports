# Malware Analysis Report — Real-time20screenshot2022-5-2026.scr

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | Real-time20screenshot2022-5-2026.scr |
| SHA256 | `68508736c04c34fd3ed97b5d7ca86cbb4ec12e2ae118fe1854f6813aa7706cf5` |
| SHA1 | `3f49b44e10733a03f98d996942b90e43e2af6cd1` |
| MD5 | `9e6a5c287bdd9c0e7ed5528b245008af` |
| Size | 322,608 bytes (315 KB) |
| File Type | PE32 executable (GUI) Intel 80386, MS Windows |
| Build Timestamp | 2026-05-08 13:45:00 UTC |
| Imphash | `9906ccdee80f5af01e40f1f9ab16940f` |

### Certificate
| Field | Value |
|---|---|
| Issuer | DigiCert Trusted G4 Code Signing RSA4096 SHA384 2021 CA1 |
| Subject | 北京宏芯互联网销售有限公司 (Beijing Hongxin Internet Sales Co., Ltd.) |
| State | Beijing, CN |
| Serial | `08acb08347d8976bdb47a62a203c8b16` |
| Validity | 2025-09-05 → 2028-12-01 (3-year EV certificate) |
| Hash Algorithm | SHA1 |

### Version Info (Spoofed)
| Field | Value |
|---|---|
| CompanyName | Beijing Hongmeng Technology Development |
| FileDescription | Control Panel |
| InternalName | Control Panel.exe |
| OriginalFilename | Control Panel.exe |
| ProductName | Control Panel |
| ProductVersion | 311.0.5.112 |
| LegalCopyright | Copyright (C) 2025 Beijing Hongmeng Technology Development |

### Section Layout
| Section | File Offset | Physical Size | Entropy |
|---|---|---|---|
| .text | 1024 | 126464 | 141 (high) |
| .rdata | 128000 | 34816 | 96 |
| .data | 164864 | 2560 | 62 |
| .rsrc | 173568 | 122880 | 91 |
| .reloc | 296448 | 5632 | 126 |
| overlay (Authenticode sig) | 304640 | 28720 | 169 |

---

## 2. Classification

**Threat**: Command-line key-gated AES-256-CBC Shellcode Loader  
**Confidence**: High  
**Family**: Unattributed custom loader

### Reasoning

The binary implements a two-path execution model:
- **Without command-line argument**: launches a fully functional console-based library management system (user/book/borrow-record CRUD with console UI). This is the analyst decoy — a complete functional application designed to mislead manual and automated analysis.
- **With command-line argument**: the argument is used as an AES-256-CBC decryption key to decrypt and execute embedded shellcode without ever touching disk.

The `.scr` extension (Windows screensaver, which Windows executes directly like an EXE) combined with "Control Panel.exe" VersionInfo and a 3-year DigiCert EV certificate represent a multi-layer masquerade for bypassing SmartScreen and analyst suspicion.

---

## 3. Capabilities

- **Masquerade**: `.scr` extension + "Control Panel.exe" VersionInfo + EV code-signing cert (T1036.001, T1553.002)
- **Anti-debug**: `IsDebuggerPresent`, `IsProcessorFeaturePresent`, `RaiseException`, `TerminateProcess`, `UnhandledExceptionFilter`
- **Anti-analysis decoy**: Fully functional library management application shown when run without a key; impossible to distinguish from benign software at first glance
- **Mutex / instance guard**: Creates `Global\library_management`; exits immediately if mutex already exists (single-instance enforcement) (T1480)
- **Anti-analysis sleep**: `Sleep(rand() % 2000 + 5000)` — sleeps 5–7 seconds before any malicious action (T1497.003)
- **Dynamic API resolution**: Resolves `NtAllocateVirtualMemory` and `NtProtectVirtualMemory` at runtime via `GetModuleHandleA("ntdll.dll")` + `GetProcAddress`; neither appears in import table (T1027.007)
- **XOR decryption**: XORs 0x780 bytes from `.rdata` VA `0x426a58` with the command-line key string (repeating key)
- **Key derivation**: Derives 32-byte AES key: `key[i] = (cmdline[i % len] XOR "dro_coin_2136"[i % 13]) + i * 7`
- **AES-256-CBC shellcode decryption**: Windows CryptoAPI (`CryptAcquireContextW` provider `0x18`, `CryptImportKey` ALG `0x6610`=`CALG_AES_256`), CBC mode; IV = first 16 bytes of XOR output; ciphertext = next 0x770 bytes
- **Shellcode execution**: `NtProtectVirtualMemory` → PAGE\_EXECUTE\_READWRITE (0x40) → direct call to decrypted buffer (T1055.004)
- **Shell execution capability**: `cmd.exe` / COMSPEC referenced; `CreateProcessW` imported (T1059.003)
- **File I/O**: Reads/writes `userinfo.dat` (0x288-byte user structs), `bookinfo.dat` (0x590-byte book structs), `recordinfo.dat` (0x588-byte borrow record structs) — all part of the decoy application's state persistence
- **Locale targeting**: `zh-CN`, `zh-TW`, `ko-KR` locale strings suggest East Asian region targeting or operator origin

---

## 4. Attack Chain

```
Delivery → victim receives Real-time20screenshot2022-5-2026.scr
           (lure: "screenshot" filename, screensaver extension, signed)
           
Stage 1  → WinMain: CreateMutex "Global\library_management"
           Sleep(5–7s) [sandbox evasion]
           
Key Check → lpCmdLine empty?
             YES → Show "Library Management System" (analyst decoy)
             NO  → sub_401c50(lpCmdLine)
             
Stage 2  → sub_401c50(key):
           GetProcAddress("NtAllocateVirtualMemory")
           Allocate 0x780 bytes
           XOR decrypt blob from .rdata @ 0x426a58 with key (repeating)
           Extract IV (bytes 0–15), ciphertext (bytes 16–1903)
           Derive 32-byte AES-256 key from key XOR "dro_coin_2136" + i*7
           CryptDecrypt(AES-256-CBC) → 0x770-byte plaintext shellcode
           GetProcAddress("NtProtectVirtualMemory") → RWX
           Call shellcode

Stage 3  → Unknown (shellcode not recovered; key required)
```

The key is delivered out-of-band (e.g., via a separate message, document, or second-stage dropper), making the binary non-executable — and non-attributable — without it.

---

## 5. IOCs

### Network
None statically recoverable. The encrypted shellcode payload (Stage 3) may contain C2 infrastructure; the decryption key is required.

### Filesystem
| Path | Purpose |
|---|---|
| `userinfo.dat` (CWD) | Decoy app user database (0x288-byte structs) |
| `bookinfo.dat` (CWD) | Decoy app book database (0x590-byte structs) |
| `recordinfo.dat` (CWD) | Decoy app borrow-record database (0x588-byte structs) |

### Registry
None identified (decoy app only uses CWD files).

### Mutexes
| Name | Purpose |
|---|---|
| `Global\library_management` | Single-instance enforcement |

### Crypto Artifacts
| Artifact | Value |
|---|---|
| Key derivation salt | `dro_coin_2136` |
| AES algorithm | AES-256-CBC (CALG\_AES\_256 = 0x6610) |
| Encrypted blob location | VA `0x426a58` / file offset `0x25E58` in `.rdata` |
| Blob size | 1920 bytes (0x780); IV=first 16, ciphertext=next 1904 |

### Certificate Serial (Attribution)
`08acb08347d8976bdb47a62a203c8b16` — DigiCert EV, issued to 北京宏芯互联网销售有限公司

---

## 6. Emulation Results

### Speakeasy Pass 1 (hooks runner)
No IOCs extracted. Emulator hit CRT initialization VirtualProtect calls and stalled before reaching WinMain logic. Absence of `FlsGetValue2` stub (unsupported API) caused early exit.

### Speakeasy Pass 2 (plain speakeasy)
Same result: emulation terminated at `FlsGetValue2` (unsupported API stub) during CRT initialization. The malicious code path requires a non-empty `lpCmdLine` argument, which speakeasy does not provide.

### Angr / Manual Decrypt
Not possible without the command-line key. The encrypted blob at file offset `0x25E58` has 256 unique byte values (maximum entropy) across all 1920 bytes, confirming strong encryption. Brute-force attack surface is undefined without knowing key format or length.

---

## 7. Sandbox Results

### ANY.RUN
- **Task ID**: `e59b14d9-b1c6-4f93-9d2b-9cb9aca8856e`
- **Score**: 100 / 100 — Malicious activity
- **Family Tags**: (none identified)
- **Public URL**: `https://app.any.run/tasks/e59b14d9-b1c6-4f93-9d2b-9cb9aca8856e`

**Behavioral findings**: The sandbox executed the binary without a command-line argument, triggering the decoy library management application path. The binary dropped one file (SHA256: `90168a7fd3566eb77a022e9a06c61689bfa8edffe495cf1037c8985d7396c250`) — likely a state `.dat` file created by the decoy app. All network requests were Microsoft/DigiCert certificate verification and Windows telemetry (no C2 observed). The 100/100 malicious score was likely driven by behavioral heuristics (CryptoAPI usage, NT API dynamic resolution pattern, mutex name), not by observed C2 traffic or shellcode execution.

**Absence of C2 IOCs confirms the key-gated architecture**: the shellcode was never decrypted or executed during sandboxing because no command-line argument was provided.

---

## 8. Analyst Notes

### Key Design Observations

1. **Key-as-argument architecture** is a deliberate anti-forensics choice. The binary alone is not weaponizable. Distribution requires two components: the binary + a delivery mechanism that provides the key (e.g., spearphish email with the key in the Subject/Body, or a document macro that launches the SCR with the correct argument).

2. **Library management decoy is fully functional**: the console application reads/writes persistent state to `.dat` files, implements user authentication with username/password comparison, and has separate admin and regular-user menu paths. This level of complexity suggests the decoy is intended to be run in front of a victim (social engineering) or to survive extended sandbox analysis.

3. **`dro_coin_2136` key salt**: the string "dro_coin_2136" embedded in the key derivation may be a campaign tag, developer identifier, or version string. The "dro" prefix could suggest "dropper," but this is speculative.

4. **3-year EV certificate** (valid until 2028-12-01) issued to a Chinese entity with a Beijing address suggests either a compromised/fraudulently obtained certificate or an actor with resources to obtain legitimate EV certificates under false pretenses. The 3-year duration indicates long-term operational planning.

5. **Shellcode size** (0x770 = 1904 bytes) is modest but sufficient for a download-and-execute, reverse shell stager, or reflective DLL loader. Stage 3 remains unknown.

### Recommended Follow-Up

- Monitor for delivery infrastructure that provides a command-line argument to `.scr` files
- Hunt for sibling samples signed with the same certificate serial (`08acb08347d8976bdb47a62a203c8b16`)
- Hunt for samples containing the string `dro_coin_2136` (potential campaign tag)
- If the key is recovered (e.g., from email metadata, endpoint logs, or another sample), re-run with speakeasy passing the key as `lpCmdLine`
- Check DigiCert revocation status for serial `08acb08347d8976bdb47a62a203c8b16`

### ATT&CK Coverage

T1027 · T1027.005 · T1027.007 · T1036.001 · T1055.004 · T1059.003 · T1082 · T1083 · T1129 · T1480 · T1497.003 · T1553.002
