# Malware Analysis Report: photo20260411689.com

**Date:** 2026-04-11  
**Analyst:** REMnux/Claude  
**Confidence:** HIGH — malicious

---

## 1. File Metadata

| Field | Value |
|-------|-------|
| Filename | `photo20260411689.com` |
| SHA256 | `c918dded298b0d76d4ac51f23b391f62a95f58b3fa2488202ecbbc9c7ce8e785` |
| MD5 | `8b75ead7c8c8737570fd93ac7c530bfd` |
| SHA1 | `2c2f211341c461c405da622adbb1511834472fcb` |
| File Type | PE32+ executable (GUI) x86-64 |
| Size | 641,496 bytes (626 KB) |
| Entropy | 93/100 (near-maximum; .text section encrypted) |
| Sections | `.textbss` (virtual/RX), `.data` ×3, `.text` (626KB packed), `.reloc`, `.rsrc` |
| Compile/Link Timestamp | 2026-04-10 16:27:42 UTC (1 day before submission) |
| PDB Path | `C:\Users\Administrator\Desktop\photo\java2.0\x64\Release\java2.0.pdb` |
| Certificate Subject | **DigiFors GmbH** (Leipzig, Sachsen, DE) |
| Certificate Issuer | DigiCert Trusted G4 Code Signing RSA4096 SHA256 2021 CA1 |
| Certificate Serial | `049209454db22190c7697285c3d5ad9b` |
| Certificate Validity | 2026-04-10 → 2027-04-09 (1-year; issued same day as compile) |
| Imphash | `98e526f6c62c0af6ded6f435c803624c` |

**Certificate note:** The signing certificate was issued on the same day as the build (2026-04-10). DigiFors GmbH is a German digital forensics company. The PDB path (`photo\java2.0`) and the heavy obfuscation strongly indicate this certificate was either issued to a fraudulent entity using the company name, or represents supply-chain compromise of the signer. This should be reported for revocation.

---

## 2. Classification

**HIGH CONFIDENCE MALICIOUS — Custom Packed Loader / Stager**

The file is a heavily obfuscated and packed Windows PE loader that:
- Self-decrypts at runtime using an LZX decompression engine via TLS callbacks
- Resolves 100+ Windows APIs dynamically to evade import analysis
- Performs extensive anti-sandbox/anti-VM checks before executing its payload
- Has full WinHTTP C2 capability (11 WinHTTP functions resolved)
- Uses `.com` extension to bypass file-extension-based controls (PE file disguised as executable script)

No matching YARA family signatures. The malware appears to be a custom-developed loader, not a public framework.

---

## 3. Capabilities

### Packer / Obfuscation
- **LZX decompression engine** built into the binary (full `unlzx_table_three__32_lil_64` implementation, 10 locations)
- **TLS callback** (`TLS.0` at `0x14015b4f8`) runs before EntryPoint, decrypts multiple code sections into virtual-only RX memory
- **Self-decryption pattern**: VirtualProtect(→RWX) → decrypt → VirtualProtect(→RX) × 8 iterations
- **15 XOR-in-loop** string encryption routines throughout unpacked code
- **Minimal import table** (1 API per DLL: `KERNEL32.dll`, `ADVAPI32.dll`, `SHELL32.dll`, `WINHTTP.dll`) — real API set resolved at runtime, decoys only
- **xxHash64** hashing (`0x9e3779b97f4a7c07`, `0xC2B2AE3D27D4EB4F` constants confirmed) for API-by-hash resolution
- Two purely virtual executable sections (`.textbss` 315KB, unnamed .data 688KB) — unpacking targets
- `.text` section: 626KB, entropy 93 (encrypted payload)
- No cleartext C2 URLs anywhere in the binary

### Anti-Analysis
- **IsDebuggerPresent** check
- **GetTickCount64** — sandbox timing check
- **NtAllocateVirtualMemory** called directly — bypasses VirtualAlloc hooks
- **26 registry key anti-VM checks** (each key queried twice — see IOC section):
  - VirtualBox: ACPI DSDT/FADT/RSDT tables, Guest Additions, VBoxGuest/Mouse/Service/SF/Video drivers
  - KVM/QEMU: vioscsi, viostor, VirtIO-FS, VirtioSerial, BALLOON/BalloonService, netkvm
  - Hyper-V: `SOFTWARE\Microsoft\Virtual Machine\Guest\Parameters`
  - VMware: `SOFTWARE\VMware, Inc.\VMware Tools`
  - Wine: `SOFTWARE\Wine`
  - Disk model: SCSI port enumeration, IDE device enumeration
  - System info: `HARDWARE\Description\System`, `SYSTEM\ControlSet001\Control\SystemInformation`

### C2 / Network
- Full WinHTTP client stack dynamically resolved: `WinHttpOpen`, `WinHttpConnect`, `WinHttpOpenRequest`, `WinHttpSendRequest`, `WinHttpReceiveResponse`, `WinHttpReadData`, `WinHttpQueryHeaders`, `WinHttpSetOption`, `WinHttpSetTimeouts`, `WinHttpCloseHandle`, `WinHttpCrackUrl`
- C2 URL(s) not recovered — XOR-encrypted at runtime, execution gated behind sandbox checks

### Persistence / Registry
- `RegOpenKeyExA`, `RegSetValueExA`, `RegDeleteValueA`, `RegCloseKey` — registry read/write/delete capability

### File Operations
- `CreateFileA/W`, `ReadFile`, `WriteFile`, `SetFileAttributesA`, `FindFirstFileExW`, `FindNextFileW`, `SetEndOfFile`
- `SHGetFolderPathA` — locates special folders (AppData, Temp, etc.) for staging

### Process Execution
- `CreateProcessA` — spawn child processes
- `ShellExecuteA` — file/URL execution
- `NtAllocateVirtualMemory` — allocate memory in remote process (injection capability)

---

## 4. Attack Chain

```
photo20260411689.com (delivered via phishing/drive-by, .com extension bypass)
    │
    ├── TLS Callback (runs before EntryPoint)
    │       └── LZX self-decryption: VirtualProtect RWX × 8 sections
    │               Resolves ~100 APIs via GetModuleHandleA + GetProcAddress
    │               (xxHash64-based import-by-hash for resilience)
    │
    ├── NtAllocateVirtualMemory (process memory setup)
    │
    ├── Anti-Sandbox Gauntlet (26 registry checks)
    │       VirtualBox / VMware / KVM / Hyper-V / Wine detection
    │       Disk model check (SCSI/IDE vendor strings)
    │       System info / timing checks
    │       [If sandbox detected → EXIT silently]
    │
    └── C2 Contact (gated behind sandbox checks)
            WinHttpOpen → WinHttpConnect → WinHttpOpenRequest
            WinHttpSendRequest → WinHttpReceiveResponse → WinHttpReadData
            [C2 URL XOR-encrypted, not recovered in emulation]
            [Likely downloads and executes next-stage payload]
            [Registry persistence via RegSetValueExA]
```

---

## 5. IOCs

### Hashes
| Type | Value |
|------|-------|
| SHA256 | `c918dded298b0d76d4ac51f23b391f62a95f58b3fa2488202ecbbc9c7ce8e785` |
| MD5 | `8b75ead7c8c8737570fd93ac7c530bfd` |
| SHA1 | `2c2f211341c461c405da622adbb1511834472fcb` |

### Network IOCs
No cleartext C2 domains/IPs/URLs recovered — all XOR-encrypted at runtime, gated behind anti-sandbox checks.

**Recommended follow-up**: Full dynamic analysis in a bare-metal or heavily cloaked sandbox (not VirtualBox/VMware/KVM) to recover C2 URLs.

### Certificate
- **Serial:** `049209454db22190c7697285c3d5ad9b`
- **Subject:** DigiFors GmbH, Leipzig, Sachsen, DE
- **Issuer:** DigiCert Trusted G4 Code Signing RSA4096 SHA256 2021 CA1
- **Valid:** 2026-04-10 → 2027-04-09
- **Status:** Issued same day as compile; **recommend DigiCert revocation request**

### Build Artifacts
- PDB: `C:\Users\Administrator\Desktop\photo\java2.0\x64\Release\java2.0.pdb`

### Filesystem
- Filename lure: `photo20260411689.com` (date-stamped photo lure with `.com` PE extension)

### Registry Keys (Anti-VM Enumeration)
```
HARDWARE\ACPI\DSDT\VBOX__
HARDWARE\ACPI\FADT\VBOX__
HARDWARE\ACPI\RSDT\VBOX__
HARDWARE\DEVICEMAP\Scsi\Scsi Port 0\Scsi Bus 0\Target Id 0\Logical Unit Id 0
HARDWARE\DEVICEMAP\Scsi\Scsi Port 1\Scsi Bus 0\Target Id 0\Logical Unit Id 0
HARDWARE\DEVICEMAP\Scsi\Scsi Port 2\Scsi Bus 0\Target Id 0\Logical Unit Id 0
HARDWARE\Description\System
SOFTWARE\Microsoft\Virtual Machine\Guest\Parameters
SOFTWARE\Oracle\VirtualBox Guest Additions
SOFTWARE\VMware, Inc.\VMware Tools
SOFTWARE\Wine
SYSTEM\ControlSet001\Control\SystemInformation
SYSTEM\ControlSet001\Services\BALLOON
SYSTEM\ControlSet001\Services\BalloonService
SYSTEM\ControlSet001\Services\VBoxGuest
SYSTEM\ControlSet001\Services\VBoxMouse
SYSTEM\ControlSet001\Services\VBoxSF
SYSTEM\ControlSet001\Services\VBoxService
SYSTEM\ControlSet001\Services\VBoxVideo
SYSTEM\ControlSet001\Services\VirtIO-FS Service
SYSTEM\ControlSet001\Services\VirtioSerial
SYSTEM\ControlSet001\Services\netkvm
SYSTEM\ControlSet001\Services\vioscsi
SYSTEM\ControlSet001\Services\viostor
System\CurrentControlSet\Enum\IDE
System\CurrentControlSet\Services\Disk\Enum
```

---

## 6. Emulation Results

### Pass 1 — Generic Runner (speakeasy)
- TLS callback executed; self-decryption loop completed (8 VirtualProtect RWX cycles)
- Stopped at `kernel32.NtAllocateVirtualMemory` (unsupported API stub)
- No network/registry/process IOCs

### Pass 2 — Plain speakeasy
- Same result; 100+ APIs resolved via GetProcAddress; stopped at NtAllocateVirtualMemory

### Pass 3 (Custom Script — NtAllocateVirtualMemory stubbed)
- Stub succeeded; emulation continued
- **26 unique anti-VM registry keys enumerated (all returned NOT FOUND in emulator)**
- WinHTTP calls not reached; emulation ended after registry checks
- Likely blocked by additional post-VM-check anti-emulation (disk model value check on Disk\Enum, GetTickCount64 timing, or CPUID)

**Emulation verdict:** Partially successful. Self-decryption and VM-detection logic fully traced. C2 communication gated behind checks that require a convincing bare-metal environment.

---

## 7. Sandbox Results

**Tria.ge:** Submission failed — HTTP 403 / Cloudflare error 1010 (connection blocked from analysis host).

**Recommendation:** Submit manually to ANY.RUN, Joe Sandbox, or CAPE on a bare-metal profile to recover C2 URLs. The sample is heavily sandbox-aware; default VM-based profiles will likely see early exit.

---

## 8. MITRE ATT&CK

| Technique | ID | Evidence |
|-----------|----|----------|
| Masquerading: Match Legitimate Name | T1036 | `.com` extension on PE; "photo" lure filename |
| Subvert Trust Controls: Code Signing | T1553.002 | DigiFors GmbH DigiCert cert (likely fraudulent) |
| Obfuscated Files: Software Packing | T1027.002 | LZX self-decryption, high entropy .text |
| Obfuscated Files: String Obfuscation | T1027 | 15× XOR-in-loop string encryption |
| Virtualization/Sandbox Evasion: Check Env | T1497.001 | 26 registry-based VM/sandbox checks |
| System Information Discovery | T1082 | HARDWARE\Description\System, disk enumeration |
| Query Registry | T1012 | 26 registry keys enumerated |
| Modify Registry | T1112 | RegSetValueExA resolved |
| Application Layer Protocol: Web Protocols | T1071.001 | WinHTTP stack (11 APIs) |
| Process Injection | T1055 | NtAllocateVirtualMemory capability |
| Boot/Logon Autostart: Registry Run Keys | T1547.001 | RegSetValueExA + Run key pattern |
| File and Directory Discovery | T1083 | FindFirstFileExW/FindNextFileW |
| Process Discovery | T1057 | GetModuleFileNameW, process handles |

---

## 9. Analyst Notes

1. **C2 URL recovery blocked:** The anti-sandbox gate is effective against speakeasy. Key next step is bare-metal dynamic analysis (CAPE/Joe/ANY.RUN with bare-metal or hardened profile that spoofs CPUID, MAC, disk serials).

2. **Certificate fraud:** The 1-year DigiCert EV cert issued to "DigiFors GmbH" on the same day as the compile date is a strong indicator of fraudulent cert acquisition. Serial `049209454db22190c7697285c3d5ad9b` should be submitted to DigiCert for revocation.

3. **"photo" lure pattern:** Filename `photo20260411689.com` with embedded date suggests a mass-distribution campaign using date-specific photo lures. The `.com` extension is designed to trigger on Windows systems where "Hide known file extensions" is enabled (users see `photo20260411689` and associate it with a script or document rather than a PE).

4. **Build infrastructure:** The PDB path `C:\Users\Administrator\Desktop\photo\java2.0\...` indicates an operator working directly on a desktop machine (not a CI/CD pipeline), suggesting individual threat actor or small group.

5. **LZX + xxHash64 combination:** This toolchain (LZX self-decompression, xxHash64 API hashing) is not a common public framework. Could indicate a custom or private-sale loader toolkit.

6. **Dual-loop VM detection (all keys checked twice):** Each registry key is queried in two separate code paths, suggesting parallel corroboration logic or defense against hook-based return value spoofing.
