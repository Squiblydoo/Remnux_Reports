# Malware Analysis Report
**Sample:** `89e52fa31535e46b5f08becbe1c591aff709036554856e6d250a503719081705.bin`
**Date:** 2026-03-06
**Analyst:** Claude Code (claude-sonnet-4-6)

---

## 1. File Metadata

| Field | Value |
|---|---|
| SHA256 | `89e52fa31535e46b5f08becbe1c591aff709036554856e6d250a503719081705` |
| SHA1 | `2e62329fd4f8a02e59aeebabd6653052c0e9d524` |
| MD5 | `322291e44783484067d04f34af2de3ec` |
| imphash | `3e1b9abc7b12e5ae445d7539e5dd133e` |
| Size | 213,272 bytes (~208 KB) |
| Type | PE32 DLL (GUI), Intel i386 |
| Compiler | MSVC 2022 |
| Build timestamp | 2026-01-26 11:12:57 |
| PDB path | `C:\Users\ethan\source\repos\SDL2\Release\SDL2.pdb` |

### Version Info Spoofing (Mismatch Table)

| Field | Spoofed Value | Actual (exports/PDB) |
|---|---|---|
| VersionInfo InternalName | `SDL3.dll` | `SDL2.dll` |
| VersionInfo FileDescription | `SDL3` | SDL2 |
| VersionInfo CompanyName | `Valve` | — |
| Export module name | — | `SDL2.dll` |
| PDB | — | `SDL2.pdb` |

The DLL fraudulently impersonates Valve's SDL2 game library, with a deliberate SDL2/SDL3 version mismatch between version resources and exports — likely an oversight during build.

### Authenticode Certificate

| Field | Value |
|---|---|
| Subject | 成都美付通宝网络科技有限公司 (Chengdu Meifu Tongbao Network Technology Co., Ltd.) |
| Issuer | GlobalSign GCC R45 EV CodeSigning CA 2020 |
| Serial | `5949c884647ba999c2ac2f78` |
| Validity | 2026-01-20 to 2027-01-21 (1 year, currently valid) |
| Country | CN (Sichuan Province, Chengdu) |

An EV code-signing certificate issued to a Chinese payment-tech company signs a fake Valve SDL DLL — strong indicator the certificate was acquired fraudulently, stolen, or the company is a front entity.

---

## 2. Classification

| Field | Value |
|---|---|
| **Family** | Unknown / Custom — DLL Sideloading Dropper + Shellcode Injector |
| **Confidence** | High |
| **Type** | Trojanized SDL2 DLL targeting game players |
| **MITRE** | T1547.001 (Registry Run Key), T1027 (Obfuscated Payload), T1129 (DLL Sideloading), T1055 (Process Injection) |

**Reasoning:** Custom C++ DLL designed to be placed in a game's directory for DLL sideloading. Exports all expected SDL2 function stubs to avoid crashes, but injects two malicious hooks into `SDL_Init` (persistence) and `SwapWindow` (payload execution). No YARA family matches — custom/novel sample.

---

## 3. Capabilities

### Persistence
- `SDL_Init` calls `RegCreateKeyExA(HKCU, "Software\Microsoft\Windows\CurrentVersion\Run", ...)` and sets value `"Valve"` = `"<path_to_self>"` if not already present
- One-shot guard: checks for existing value before writing

### Payload Decryption — Two-Layer Scheme
Located in `sub_100011b0` (ea=0x10001456), called exclusively from `SwapWindow`:

**Layer 1 — XOR pre-processing (8 rounds):**
- Allocates heap buffer 0x12E40 (77,376) bytes
- Copies `.data` encrypted blob from `0x1001E8C0`
- Iterates 8 rounds (index 7→0), per round XOR-ing entire buffer with a single key byte broadcast to 32 bits
- Key bytes at `0x1001BAA0`: `99 67 8D 04 CB 1E 3B 3A`
- Unrolled 0x40-byte chunks for speed

**Layer 2 — AES-128-CBC decryption (BCrypt):**
- Algorithm: `AES` / `ChainingModeCBC`
- Key (hardcoded at `0x1001E8B0`): `A1 4B D1 C7 F8 84 E5 D7 AB 01 D1 B3 38 C2 44 13`
- IV: first 16 bytes of the XOR-decrypted buffer (dwords uStack_18..uStack_c)
- Ciphertext: 0x12E30 (77,360) bytes starting at XOR-buffer offset 16

### Shellcode Injection into Host Process
Located in `SwapWindow` export (ea=0x10000800):
1. Calls `sub_100011b0` — decrypt 77KB payload
2. Dynamically resolves API names from obfuscated `.data` strings (`0x1001BA00`–`0x1001BA5C`) via `GetModuleHandleA` + `GetProcAddress` (8+ function lookups)
3. `VirtualAlloc(NULL, payload_size, MEM_COMMIT|MEM_RESERVE, PAGE_READWRITE)`
4. `memcpy(alloc, decrypted_payload, size)`
5. `VirtualProtect(alloc, size, PAGE_EXECUTE_READWRITE=0x40, &old)`
6. Thread creation (NtCreateThreadEx-style, access mask `0x1FFFFF`) to execute shellcode
7. `WaitForSingleObject(thread, INFINITE)` + `CloseHandle`

### Anti-Analysis
- `IsDebuggerPresent` + `SetUnhandledExceptionFilter` / `RaiseException` anti-debug chain
- Dynamic API resolution with obfuscated string building at runtime
- Stack-based string construction (3 locations, `StackArrayInitialisationX86`)
- Two-layer encryption over the embedded payload
- Spaghetti-code control flow in 3 functions

### SDL Export Stubs
All SDL exports except `SDL_Init` and `SwapWindow` share the same code address (`0x10001FF0`) — they are no-op stubs that satisfy the game's import loader and prevent crashes while the malicious hooks execute silently.

---

## 4. Attack Chain

```
[Attacker]
  └─ Distributes malicious SDL2.dll (e.g., via cracked game, mod, torrent)
       └─ Placed in game installation directory

[Victim launches game]
  └─ Windows DLL search order: game dir before System32
       └─ Malicious SDL2.dll loaded instead of legitimate SDL2.dll

DllMain (benign stub — no action)

Game calls SDL_Init():
  └─ GetModuleFileNameA() → own path
  └─ HKCU\...\Run "Valve" = "<self>" → PERSISTENCE

Game render loop calls SDL_GL_SwapWindow() / SwapWindow() each frame:
  └─ Decrypt embedded 77KB payload:
       XOR (8 passes, key 9967...) → AES-128-CBC (key A14BD1..., CBC)
  └─ VirtualAlloc → memcpy → VirtualProtect(RWX) → CreateThread
       └─ [STAGE 2 SHELLCODE EXECUTES in game process]
            └─ C2 configuration embedded in decrypted payload (not recovered)
```

---

## 5. IOCs

### File Hashes
| Type | Value |
|---|---|
| SHA256 | `89e52fa31535e46b5f08becbe1c591aff709036554856e6d250a503719081705` |
| SHA1 | `2e62329fd4f8a02e59aeebabd6653052c0e9d524` |
| MD5 | `322291e44783484067d04f34af2de3ec` |
| imphash | `3e1b9abc7b12e5ae445d7539e5dd133e` |

### Registry
| Key | Value Name | Data |
|---|---|---|
| `HKCU\Software\Microsoft\Windows\CurrentVersion\Run` | `Valve` | `"<path_to_dll>"` |

### Filesystem
- Malicious DLL deployed as `SDL2.dll` in game installation directory (sideloading vector)

### Cryptographic Artifacts (Detection/Extraction)
| Type | Value |
|---|---|
| AES-128-CBC Key | `A1 4B D1 C7 F8 84 E5 D7 AB 01 D1 B3 38 C2 44 13` |
| XOR Key (8 bytes) | `99 67 8D 04 CB 1E 3B 3A` |
| Encrypted payload offset | File offset 0x1D400 (`.data` at 0x1001E8C0), size 77,376 bytes |

### Certificate
| Field | Value |
|---|---|
| Serial | `5949c884647ba999c2ac2f78` |
| Subject | 成都美付通宝网络科技有限公司 |
| Issuer | GlobalSign GCC R45 EV CodeSigning CA 2020 |

### Attribution Artifacts
| Field | Value |
|---|---|
| Developer username | `ethan` |
| Build path | `C:\Users\ethan\source\repos\SDL2\Release\` |
| Signer org location | Chengdu, Sichuan, CN |

### Network IOCs
None identified in static analysis. C2 configuration is embedded within the encrypted 77KB payload and requires dynamic extraction.

---

## 6. Analyst Notes

### Stage 2 Payload — Statically Decrypted

Static decryption was performed (see `decrypt_stage2.py`). The two-layer scheme was applied:
1. XOR: 8 passes (key bytes `99 67 8D 04 CB 1E 3B 3A`, index 7→0) — net effect: each byte XOR'd with `0xA3`
2. AES-128-CBC: key `A14BD1C7F884E5D7AB01D1B338C24413`, IV = first 16 bytes of XOR output

**Decrypted output:** `output/speakeasy_dump/stage2_decrypted.bin`

| Field | Value |
|---|---|
| SHA256 | `2ef76d4546c593ee6698c63944ba4048c75fd71eaecb251fea386aa9c3f67503` |
| SHA1 | `6dda5bab78d52432688bc3623f5796f022eb1004` |
| MD5 | `7a9c9c7fb192025a544f5919d5fafefe` |
| Size | 77,360 bytes |
| Type | Raw shellcode (not PE) |
| Entropy | High (195/255) — further obfuscation or compression within |

**Structure of decrypted payload:**

```
Offset 0x00000  [5 bytes]  x86 GetPC stub: E8 C0 CB 00 00 (CALL +0xCBC5)
                           → pushes return address 0x5 onto stack, jumps to 0xCBC5
Offset 0x00005  [~52KB]    Encrypted/compressed data block (random-looking bytes)
                           → indexed via the return address; decoded in-memory by x64 stage
Offset 0xCBC5   [~25KB]    x64 shellcode entry point
                           59              POP RCX        (GetPC: RCX = 0x5 = shellcode base ref)
                           31 C0           XOR EAX, EAX
                           48 0F 88 ...    (control flow, jumps into main body)
                           55              PUSH RBP
                           48 89 E5        MOV RBP, RSP   ← standard x64 prologue
                           48 83 E4 F0     AND RSP, -16   ← stack alignment
                           48 83 EC 20     SUB RSP, 32    ← shadow space
                           E8 05 00 00 00  CALL +5        ← second-level GetPC
```

**Key observations:**
- This is a **Heaven's Gate** style payload: the 32-bit host DLL (`SwapWindow`) creates a thread, which begins in x86 mode, then the x86 stub (`E8 C0 CB 00 00`) jumps into the x64 code at offset 0xCBC5
- The x64 shellcode uses a standard Windows x64 function prologue and calling convention
- No cleartext strings/URLs/C2 indicators are visible in the decrypted buffer — the x64 stage contains a third layer of string obfuscation (API hashing or XOR strings at runtime)
- Entropy remains high throughout the x64 portion, consistent with packed/compressed code or inline-decrypted API/config data
- Pattern resembles professional C2 framework implants (Cobalt Strike beacon, Brute Ratel C4, or custom loader) — **requires dynamic analysis to confirm**

**What dynamic analysis should look for:**
- Memory dumps after the second `CALL +5` GetPC pattern resolves (second-level base address calculation)
- Any `VirtualAlloc` + `LoadLibrary` / manual PE mapping calls inside the x64 stage
- Network connections (C2 beaconing) after thread execution begins

### Trigger Rate Concern
`SwapWindow` fires on every rendered frame (typically 30–144+ times/second). If no one-shot guard exists in the payload execution path, thread spawning will be extremely aggressive. Dynamic analysis should confirm whether a mutex or flag prevents repeated execution.

### DLL Sideloading Target Game
The specific targeted game is not identified from static analysis. SDL2 is used by thousands of games (Steam, itch.io, indie titles). The attacker may distribute this as a replacement SDL2.dll with a cracked game installer or malicious game mod.

### Certificate Abuse Pattern
The GlobalSign EV certificate for a Chinese payment company signing a fake Valve DLL matches the pattern seen in this campaign workspace (3-day Microsoft Trusted Signing certs for IoTrust/DDinosaur samples). However, this is a 1-year GlobalSign EV cert, suggesting a higher-effort operation or different threat actor. Recommend reporting serial `5949c884647ba999c2ac2f78` to GlobalSign for revocation.

### YARA Detection Suggestion
```yara
rule FakeSDL2_Sideload_Dropper {
    meta:
        description = "Trojanized SDL2.dll DLL sideloading dropper"
        hash = "89e52fa31535e46b5f08becbe1c591aff709036554856e6d250a503719081705"
    strings:
        // AES key hardcoded in .data
        $aes_key = { A1 4B D1 C7 F8 84 E5 D7 AB 01 D1 B3 38 C2 44 13 }
        // XOR key
        $xor_key = { 99 67 8D 04 CB 1E 3B 3A }
        // Autorun registry path
        $reg = "Software\\Micro" ascii wide
        $valve_val = "Valve" ascii
        // SDL exports present
        $sdl_init = "SDL_Init" ascii
        $sdl_swap = "SwapWindow" ascii
    condition:
        uint16(0) == 0x5A4D and
        $aes_key and $xor_key and
        $reg and $valve_val and
        $sdl_init and $sdl_swap
}
```
