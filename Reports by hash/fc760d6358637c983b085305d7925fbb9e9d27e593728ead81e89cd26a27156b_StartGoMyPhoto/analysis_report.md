# Malware Analysis Report: StartGoMyPhoto.exe

**Date**: 2026-04-18  
**Analyst**: Claude (claude-sonnet-4-6)

---

## 1. File Metadata

| Field | Value |
|-------|-------|
| Filename | StartGoMyPhoto.exe |
| SHA256 | `fc760d6358637c983b085305d7925fbb9e9d27e593728ead81e89cd26a27156b` |
| MD5 | `1cbcf480226fdad66e7ff4916656dc04` |
| SHA1 | `2c0d437ba29d505c1d3d22136ab6fa51e56446f0` |
| File Type | PE32 .NET assembly (GUI), x86, .NET Framework 4.8 |
| Size | 372,576 bytes (363 KB) |
| Imphash | `f34d5f2d4577ed6d9ceec516c1f5a744` |
| PE Timestamp | 2024-11-06 09:11:57 UTC |
| Language | VB.NET |
| Packer/Obfuscator | SmartAssembly 6.9.0.114 |

### Code Signing Certificate

| Field | Value |
|-------|-------|
| Subject | Andre Fathurrohman |
| Org Details | Unit=???, State=Lampung, Locality=Sukamulya Banyumas, Country=ID |
| Issuer | SSL.com Code Signing Intermediate CA RSA R1 |
| Serial | `30e8b638035fce635662a7eda383da10` |
| Validity | 2025-05-19 to 2026-05-19 (1-year individual cert) |
| Hash Algorithm | SHA256/RSA |

**VersionInfo**: All fields contain placeholder garbage — `Comments="asd"`, `CompanyName="asd"`, `FileDescription="asdasd"`, `ProductName="d"`. Clearly not a legitimate application despite the code signing cert.

---

## 2. Classification

**Family**: TeleDoor RAT (SmartAssembly-packed VB.NET downloader/process-hollowing injector)  
**Confidence**: HIGH  
**Reasoning**: KesaKode similarity verdict of 4/5 for TeleDoor. Code structure matches known TeleDoor patterns: SmartAssembly 6.x obfuscation, the `Home.la()` stager class with reversed-URL C2 download, base64 decode, and process hollowing. The "StartGoMyPhoto" branding is a lure — the binary contains an embedded Brazilian ERP application dataset (`PROJETOAUTOMACAO.VB` namespace with Portuguese financial tables) as cover content.

---

## 3. Capabilities

- **SmartAssembly 6.9.0.114 obfuscation**: 205 non-ASCII function names, spaghetti control flow, encrypted string table, JIT-hook HouseOfCards protection
- **Lure content**: Embedded `BD_AUTOMCAODataSet` (Brazilian business management schema: TB_CADCLI, TB_CADPROD, TB_CONTASPAGAR, TB_CONTASRECEBER, TB_FORNECEDORES, TB_FUNCIONARIOS, TB_ORDEMSERVICO, TB_PEDIDO, TB_TIPOLANCAMENTO) — legitimate-looking data to deceive sandbox/AV
- **Anti-sandbox / Anti-analysis**:
  - WMI query via `ManagementObjectSearcher` + checks result string for a VM signature; calls `Environment.Exit(1)` if detected
  - SmartAssembly HouseOfCards JIT hook (intercepts JIT compilation to call `la()` dynamically — call site invisible in static decompilation)
  - `SuppressIldasm` attribute on assembly
- **C2 download (reversed URL technique)**: `WebClient.DownloadString(adress.Reverse())` — the C2 URL is stored reversed and reversed again before use; the downloaded content is likewise reversed before base64-decoding
- **AES-256 encrypted C2 comms**: Hardcoded 32-byte key (see IOCs); IV derived from MD5 of a second key parameter
- **Process hollowing (two paths)**:
  - Uses `LoadLibraryA`/`GetProcAddress` at runtime to resolve all WinAPI functions (no static imports)
  - Resolved delegates: SuspendThread, GetThreadContext(x86/x64), SetThreadContext(x86/x64), NtUnmapViewOfSection, VirtualAllocEx (type 12288 MEM_COMMIT|MEM_RESERVE, protect 64 PAGE_EXECUTE_READWRITE), WriteProcessMemory, ReadProcessMemory, ResumeThread, CreateProcess (flag `134217732` = CREATE_SUSPENDED|CREATE_NO_WINDOW)
  - Path 1: Hollows a fixed system process (path at obfuscated string index 1712)
  - Path 2: Hollows a configurable process `<prefix> + injection_param + <suffix>` (string indices 1770+1866)
  - Up to 4 retry attempts on failure; kills failed process instances on exception
- **Registry persistence**: `HKCU\Software\Microsoft\Windows\CurrentVersion\Run` → `<name>.exe` pointing to `%TEMP%\<startupname>.exe`
- **Alternate persistence (admin path)**: Copies self to `%ProgramData%\<startupname>.exe` and writes/runs a BAT script from `%TEMP%`; optionally invokes PowerShell for copy + Run key setup
- **File self-copy**: Copies from `Directory.GetCurrentDirectory()` to `%TEMP%\<startupname>.exe`
- **x86/x64 shellcode trampolines** embedded: Two stub trampolines in `\u000f.\u0001` class (40-byte x64 `mov rax,imm64; jmp rax` trampoline; 30-byte x86 stdcall stub) used by the JIT hooking mechanism
- **Dynamic .NET assembly loading** via `Reflection.Assembly` + `ILGenerator`/`DynamicMethod` for delegate construction

---

## 4. Attack Chain

```
1. LAUNCH
   StartGoMyPhoto.exe executes
   └─ SmartAssembly static constructors run:
      - Decrypts string table (AES-256, custom LZ77)
      - Installs HouseOfCards JIT hook (x86/x64 trampolines)
      - Resolves WinAPI delegates via LoadLibraryA/GetProcAddress

2. ANTI-SANDBOX CHECK
   WMI ManagementObjectSearcher query (obfuscated namespace+query)
   └─ If VM/sandbox detected → Environment.Exit(1)

3. PAYLOAD DOWNLOAD  (Home.la() invoked via JIT hook)
   C2 address parameter stored reversed (e.g. "moc.example/daolnwod" → real URL)
   └─ WebClient.DownloadString(reversed_address)
   └─ Reverse the downloaded content
   └─ Base64-decode → raw PE bytes of stage-2 payload

4. PROCESS HOLLOW (branch on architecture flag)
   Path A (32-bit default):
     CreateProcess("fixed_system_exe", CREATE_SUSPENDED)
     GetThreadContext → Read PEB base → NtUnmapViewOfSection
     VirtualAllocEx(RWX) → WriteProcessMemory (headers + all sections)
     SetThreadContext(EBX=new_base, EAX=entry_point) → ResumeThread

   Path B (configurable target):
     Same hollowing steps with caller-provided process path

5. PERSISTENCE
   Copy self → %TEMP%\<startupname>.exe
   HKCU\...\Run\<name> = "%TEMP%\<startupname>.exe"
   Optionally: write BAT to %TEMP% and execute hidden
```

---

## 5. IOCs

### File Hashes
| Type | Value |
|------|-------|
| SHA256 | `fc760d6358637c983b085305d7925fbb9e9d27e593728ead81e89cd26a27156b` |
| MD5 | `1cbcf480226fdad66e7ff4916656dc04` |
| SHA1 | `2c0d437ba29d505c1d3d22136ab6fa51e56446f0` |
| Imphash | `f34d5f2d4577ed6d9ceec516c1f5a744` |

### Network
**C2 URL**: Not recovered — stored as reversed SmartAssembly-encrypted string, unresolvable without executing the binary. The download uses `System.Net.WebClient` over HTTP/HTTPS.

### Certificates
| Field | Value |
|-------|-------|
| Serial | `30e8b638035fce635662a7eda383da10` |
| Subject | Andre Fathurrohman, Lampung, ID |
| Issuer | SSL.com |
| Validity | 2025-05-19 – 2026-05-19 |

### Cryptographic Artifacts
| Item | Value |
|------|-------|
| AES-256 key (hex) | `52 66 68 6E 20 4D 18 22 76 B5 33 11 12 33 0C 6D 0A 20 4D 18 22 9E A1 29 61 1C 76 B5 05 19 01 58` |
| AES-256 key (decimal) | `82,102,104,110,32,77,24,34,118,181,51,17,18,51,12,109,10,32,77,24,34,158,161,41,97,28,118,181,5,25,1,88` |
| IV derivation | MD5 of Unicode-encoded second key parameter |

### Registry
```
HKCU\Software\Microsoft\Windows\CurrentVersion\Run\<obfuscated_name> = %TEMP%\<startupname>.exe
```

### Filesystem
```
%TEMP%\<startupname>.exe   — persistent copy of malware
%TEMP%\<script>.bat        — optional persistence BAT script
%ProgramData%\<startupname>.exe — alternate persistence (admin path)
```

---

## 6. Emulation Results

**Speakeasy**: Not supported for .NET assemblies — skipped.  
**FLOSS**: .NET language-specific string extraction not supported — skipped.  
**Manual C# decompilation** (ilspycmd): Partial success. `Home.la()` fully recovered; `\u0008.\u0001.\u0001.\u0001()` (hollowing function) fully recovered. C2 URL and hollow target process name remain obfuscated behind SmartAssembly 6.9 encrypted string table (AES + custom LZ77; resource `{12c47dd7-8fe6-4468-b167-30e009e5b7cb}`, 492 bytes).

---

## 7. Sandbox Results

**Tria.ge**: Submission blocked (Cloudflare 1010 error from REMnux host). API key configured but network blocked.

---

## 8. Analyst Notes

### Lure Strategy
The binary name "StartGoMyPhoto" and the embedded `PROJETOAUTOMACAO.VB` ERP dataset suggest this may be distributed to Brazilian targets under a photo/productivity app pretext, with the Portuguese ERP schema serving as filler to inflate file size and plausibility.

### SmartAssembly String Table — Unrecovered
The SmartAssembly 6.9 string table (resource `gGDYbmxMTQwAeZoGid.HTRVQrdRRftaiIZjqT`, 22,942 bytes) is AES-encrypted with a custom LZ77 wrapper. Without executing the binary or implementing the full SmartAssembly 6.9 decryptor, the C2 URL (string index 1712/1770/1866), hollow target process (string index 1712), startup name, and WMI query (indices 188, 212, 274, 286) remain unknown. Strings at indices 322, 328, 340, 354, 448, 472, 544, 558, 564, 666, 680 represent the persistence mechanism configuration.

### Certificate Abuse
The SSL.com individual code signing cert issued to "Andre Fathurrohman" (Indonesian individual, Lampung province) bypasses SmartScreen/Authenticode checks. The same SSL.com CA has appeared in other campaigns, but different serial numbers — no cross-reference applicable.

### HouseOfCards JIT Hook
SmartAssembly's HouseOfCards mechanism patches the CLR JIT compiler at startup and calls `Home.la()` with the decrypted C2 address and configuration — making `la()` appear to have no static call sites and defeating many static analysis approaches.

### Recommended Follow-up
1. **Dynamic analysis**: Execute in isolated .NET-capable sandbox (ANY.RUN, FlareVM) to recover the C2 URL from network traffic and identify the target process.
2. **String decryption**: Implement SmartAssembly 6.9 decryptor or use de4dot on a system with Mono/.NET to decrypt the string table offline.
3. **Payload analysis**: After C2 URL recovery, download and analyze the hollow payload — likely another .NET RAT (TeleDoor stage 2 based on KesaKode hit).
4. **Hunt by cert serial**: `30e8b638035fce635662a7eda383da10` — search VirusTotal/AnyRun for other samples signed with this cert.
5. **Hunt by AES key**: The hardcoded 32-byte AES key is a strong campaign-level indicator; search for it in other TeleDoor samples.

---

## 9. MITRE ATT&CK Mapping

| Technique | ID | Notes |
|-----------|-----|-------|
| Obfuscated Files or Information | T1027 | SmartAssembly 6.9, spaghetti control flow, encrypted strings |
| Software Packing | T1027.002 | SmartAssembly HouseOfCards JIT hook |
| Deobfuscate/Decode Files | T1140 | Base64 decode of C2 response |
| Reflective Code Loading | T1620 | Dynamic method/assembly loading via Reflection |
| Process Hollowing | T1055.012 | Full WinAPI hollowing via runtime-resolved delegates |
| Code Signing | T1553.002 | Individual SSL.com cert (Andre Fathurrohman, ID) |
| Masquerading | T1036 | "StartGoMyPhoto" name + placeholder VersionInfo |
| Registry Run Keys / Startup Folder | T1547.001 | HKCU\...\Run persistence |
| Command and Scripting: PowerShell | T1059.001 | BAT/PS1 optional persistence script |
| Ingress Tool Transfer | T1105 | Stage-2 download from reversed C2 URL |
| Web Protocols | T1071.001 | HTTP/HTTPS WebClient download |
| Virtualization/Sandbox Evasion | T1497.001 | WMI anti-sandbox check → Exit(1) |
| Query Registry | T1012 | WMI queries for environment fingerprinting |
| System Information Discovery | T1082 | WMI ManagementObjectSearcher |
| Shared Modules | T1129 | LoadLibraryA/GetProcAddress at runtime |
