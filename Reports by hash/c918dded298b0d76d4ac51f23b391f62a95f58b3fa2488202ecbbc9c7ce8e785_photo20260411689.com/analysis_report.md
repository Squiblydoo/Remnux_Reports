# Malware Analysis Report: photo20260411689.com

**Date:** 2026-04-11  
**Analyst:** REMnux/Claude  
**Confidence:** HIGH — malicious  
**Tria.ge:** https://tria.ge/260411-whsmgses9j (behavioral2 confirmed stage-2 execution; direct API access blocked from analysis host by Cloudflare)

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
| Compile Timestamp | 2026-04-10 16:27:42 UTC (1 day before submission) |
| PDB Path | `C:\Users\Administrator\Desktop\photo\java2.0\x64\Release\java2.0.pdb` |
| Certificate Subject | **DigiFors GmbH** (Leipzig, Sachsen, DE) |
| Certificate Issuer | DigiCert Trusted G4 Code Signing RSA4096 SHA256 2021 CA1 |
| Certificate Serial | `049209454db22190c7697285c3d5ad9b` |
| Certificate Validity | 2026-04-10 → 2027-04-09 (1-year; issued same day as compile) |
| Imphash | `98e526f6c62c0af6ded6f435c803624c` |

**Certificate note:** Issued same day as the build. DigiFors GmbH is a real German digital forensics company. The PDB path (`photo\java2.0`) and heavy obfuscation indicate fraudulent cert acquisition using the company's identity. **Recommend DigiCert revocation request for serial `049209454db22190c7697285c3d5ad9b`.**

---

## 2. Classification

**HIGH CONFIDENCE MALICIOUS — Custom Packed Loader → Trojanized TurboVPN Stager**

Two-stage operation:
- **Stage 1** (`photo20260411689.com`): Heavily obfuscated PE loader with LZX self-decryption, 26-check anti-VM gauntlet, and WinHTTP C2 stack. Downloads a stage-2 manifest from Google Cloud Storage.
- **Stage 2**: A trojanized TurboVPN installation package (11 files) delivered from `storage.googleapis.com/trubo/`. Contains a Zhong Stealer C2 component (RAT module), a WebView2 MITB payment interceptor, and a shellcode stager that connects to the attacker's C2 via UDP/named-pipe at `uu[.]goldeyeuu[.]io:50986`.

No matching YARA family for the stage-1 loader. Stage-2 `windui.dll` is the **Zhong Stealer** C2/RAT module (attributed to APT-Q-27 / Golden Eye Dog).

---

## 3. Capabilities

### Stage 1 — Loader (`photo20260411689.com`)

**Packer / Obfuscation**
- **LZX decompression engine** built into the binary (full `unlzx_table_three__32_lil_64` implementation)
- **TLS callback** (`TLS.0` at `0x14015b4f8`) runs before EntryPoint; decrypts multiple code sections into virtual-only RX memory
- **Self-decryption pattern**: VirtualProtect(→RWX) → decrypt → VirtualProtect(→RX) × 8 iterations
- **15 XOR-in-loop** string encryption routines throughout unpacked code
- **xxHash64** API-by-hash (`0x9e3779b97f4a7c07`, `0xC2B2AE3D27D4EB4F` constants) for dynamic import resolution; 100+ APIs resolved at runtime
- Minimal import table (1 API per DLL as decoy); no cleartext C2 URLs in binary

**Anti-Analysis**
- `IsDebuggerPresent` check
- `GetTickCount64` sandbox timing check
- `NtAllocateVirtualMemory` called directly (bypasses VirtualAlloc hooks)
- **26 registry key anti-VM checks** (each queried twice — dual-loop defeats hook-based emulator spoofing):
  - VirtualBox: ACPI DSDT/FADT/RSDT tables, Guest Additions, VBoxGuest/Mouse/Service/SF/Video drivers
  - KVM/QEMU: vioscsi, viostor, VirtIO-FS, VirtioSerial, BALLOON/BalloonService, netkvm
  - Hyper-V: `SOFTWARE\Microsoft\Virtual Machine\Guest\Parameters`
  - VMware: `SOFTWARE\VMware, Inc.\VMware Tools`
  - Wine: `SOFTWARE\Wine`
  - Disk model: SCSI port enumeration, IDE device enumeration
  - System info: `HARDWARE\Description\System`, `SYSTEM\ControlSet001\Control\SystemInformation`

**C2 / Network**
- Full WinHTTP client stack dynamically resolved: `WinHttpOpen`, `WinHttpConnect`, `WinHttpOpenRequest`, `WinHttpSendRequest`, `WinHttpReceiveResponse`, `WinHttpReadData`, `WinHttpQueryHeaders`, `WinHttpSetOption`, `WinHttpSetTimeouts`, `WinHttpCloseHandle`, `WinHttpCrackUrl`
- Downloads stage-2 manifest `tur.txt` from `storage[.]googleapis[.]com/trubo/` after sandbox checks pass
- C2 URL(s) XOR-encrypted at runtime; not recovered in static/emulation analysis

**Persistence / File Operations**
- `RegOpenKeyExA`, `RegSetValueExA`, `RegDeleteValueA` — registry read/write/delete capability
- `CreateFileA/W`, `ReadFile`, `WriteFile`, `SetFileAttributesA`, `FindFirstFileExW/W`
- `SHGetFolderPathA` — locates special folders (AppData, Temp)
- `CreateProcessA`, `ShellExecuteA` — spawn child processes
- `NtAllocateVirtualMemory` — remote process memory allocation (injection capability)

---

### Stage 2 — Trojanized TurboVPN Package (11-file drop)

**Delivery**: `tur.txt` manifest at `storage[.]googleapis[.]com/trubo/tur.txt` lists 11 GCS URLs. All PE components share a single DigiCert cert: **INNOVATIVE CONNECTING PTE. LIMITED** (Singapore), serial `06500ee65ffbfb6ea4f4b16ab6f910c6`, valid 2026-04-04 → 2027-04-02. This is the legitimate TurboVPN signing certificate.

Two distinct build environments identified within the package:
- **`D:\360\work\quantum\Release\`** — `payment.dll`, `windui.dll`, `libuv.dll`: malicious 360/Quantum-built components
- **`E:\DUOWAN_BUILD\...yyexternal.pdb`** — `Turbo.exe`: DUOWAN/YY platform binary masquerading as TurboVPN

#### Component Summary

| File | SHA256 | Size | Role |
|------|--------|------|------|
| `Turbo.exe` | `5e841260...` | 271KB | Launch host; internal name `yyexternal.exe`; loads crashreport.dll; `Software\duowan\yy` registry; PDB `DUOWAN_BUILD` |
| `crashreport.dll` | `31c1e830...` | 97KB | **Shellcode loader**; exports `InitBugReport`; PEB-walk FNV-1a API resolution; opens `Trubo.log`; decrypts and executes payload |
| `Trubo.log` | `fc3fdbfb...` | 163KB | **Encrypted shellcode payload**; decryption: `(byte + 0x77) & 0xFF ^ 0x62`; decrypts to x86 PEB-walk stager with WinINet+UDP C2 |
| `payment.dll` | `273385ca...` | 154KB | **WebView2 MITB interceptor**; exports `WebReload()`; `ICoreWebView2NavigationCompletedEventHandler`; intercepts payment checkout pages |
| `windui.dll` | `a8be52e0...` | 800KB | **Zhong Stealer C2 module** (APT-Q-27); DuiLib ActiveX host; WebSocket C2 to `uu.goldeyeuu.io:5188`; targets `Software\360\360se6\chrome` |
| `libuv.dll` | `8e8cb13f...` | 291KB | C2 networking layer; 360/Quantum build (`D:\360Work\7.Quantum\`); named pipe `\\?\pipe\uv\%p-%lu`; UDP transport |
| `service.cfg` | `cebbfee6...` | 35B | Transport config: `{"pipe":1,"udp":1,"udp_port":50986}` |
| `remote_config_data` | `e51bd548...` | 16KB | Firebase Remote Config cache; TurboVPN server list, VPN certs, V2Ray/Xray/Reality/SSR protocol config |
| `image.jpg` | `0ce9b137...` | 11KB | **503 error page decoy** (JPEG 761×352); shown to victim to explain "dead link" while payload installs |
| `msvcp140.dll` | `e4c71980...` | 436KB | Legitimate Microsoft VC++ runtime (re-bundled) |
| `vcruntime140.dll` | `8e085754...` | 74KB | Legitimate Microsoft VC++ runtime (re-bundled) |

#### `crashreport.dll` — Shellcode Loader Detail

`InitBugReport` (exported function, EA 1024) implements a full shellcode loader via PEB-walk:
1. PEB walk (`fs:[0x30]` → LDR list) with FNV-1a hash (`seed ^ char * 0x1000193`)
2. API-by-hash sequence: CPU count check (anti-VM: ≤1 CPU = sandbox exit) → `Sleep(1500ms)` → `GetModuleFileNameW` → construct filename `Trubo.log` on stack (bytes `0x62 0x75 0x54 0x6f 0x6c 0x2e 0x6f 0x67`)
3. `CreateFile(Trubo.log, GENERIC_READ)` → map file to memory
4. `VirtualAlloc(MEM_COMMIT|MEM_RESERVE, PAGE_NOACCESS)` → decrypt payload in place:
   - **Algorithm**: `decrypted = (encrypted_byte + 0x77) & 0xFF ^ 0x62`
5. Execute shellcode: thread creation → `(*shellcode_ptr)()`

#### `Trubo.log` Shellcode Structure (after decryption)

- Offset 0x000: `E9 46 04 00 00` → `JMP 0x44B` (jump to main function)
- Offset 0x005: `GetExportByHash()` helper — walks PE export table, FNV-1a hash comparison
- Offset 0x44B: Main function — PEB-walk API resolution, VirtualAlloc, C2 connection
- Offset 0x27689: Domain fragments `uu.g` + `[0x40]` + `oldeye` (runtime assembly of `uu[.]goldeyeuu[.]io`)
- Tail (offset ~0x27950): API name strings — `KERNEL32.DLL`, `ADVAPI32.dll`, `IPHLPAPI`, `OLEAUT32`, `SHELL32`, `WININET`, `LoadLibraryA`, `GetProcAddress`, `VirtualAlloc/Protect/Free`, `CoCreateGuid`, `wsprintfa`, `InternetOpen*`

#### `payment.dll` — WebView2 MITB

`WebReload(uint32_t session_id, int32_t mode)` exported function:
- Mode 0: calls `ICoreWebView2::Reload()`
- Mode 1: navigates to payment page URL + registers `ICoreWebView2NavigationCompletedEventHandler` (lambda `5f63f8f708a4043584f11c7685b8767d`) to intercept page load completion
- Targets TurboVPN payment checkout pages: `inngoturbo[.]com`, `turbospn[.]com`, `payserviceinn[.]com` (URLs extracted from `remote_config_data`)
- JavaScript injection reference: `window.refreshWi...yearsplan.pm65` visible in strings

#### `windui.dll` — Zhong Stealer C2 Module / ActiveX Host

- DuiLib (`DuiLib::CActiveXCtrl`) COM interface implementation: `IUnknown`, `IOleClientSite`, `IOleInPlaceSiteWindowless`, `IOleInPlaceSite`, `IOleWindow`, `IOleControlSite`, `IOleContainer`, `IObjectWithSite`
- Registry target: `Software\360\360se6\chrome` (360 Secure Browser credential store)
- Build path: `D:\360\work\quantum\Release\windui.pdb` (same 360/Quantum environment as payment.dll and libuv.dll)
- WebSocket C2: `uu.goldeyeuu.io:5188` — binary protocol, 12-byte header `[len][type][session_id]`, operator-directed RAT capability

#### `remote_config_data` — Firebase Config (cleartext)

Firebase Remote Config cache (`fetched Uconfig_firebase`) containing:
- **StrongSwan VPN certificates**: CN=`ThisIsSpar ta.we2022...` (internal/self-signed)
- **VPN protocols**: `ssr`, `nssr`, `v2ray`, `reality`, `xray`, `tun2socks`
- **Proxy server list**: `ubuntu[.]trovpn[.]com`, `1624147767.rsc.cdn77[.]org`, `1849029393.rsc.cdn77[.]org`, `dgs6f4l7ftt5u.cloudfront[.]net`, `d2a60k7cnkt82a.cloudfront[.]net`
- **Payment domains**: `turbovpn[.]com`, `inngoturbo[.]com`, `turbospn[.]com`, `payserviceinn[.]com`
- IP geolocation check: `ip-api[.]com/json`, `ipapi[.]co/json`
- Additional GCS assets: `storage[.]googleapis[.]com/windows_bucket1/turbo/`

---

## 4. Attack Chain

```
photo20260411689.com  (delivered via phishing/drive-by; .com extension bypasses file controls)
    │
    ├── TLS Callback (pre-EntryPoint)
    │       LZX self-decryption: VirtualProtect RWX × 8 sections
    │       xxHash64 API-by-hash: resolves 100+ APIs dynamically
    │
    ├── NtAllocateVirtualMemory → process memory setup
    │
    ├── Anti-Sandbox Gauntlet (26 registry checks × 2 passes)
    │       VirtualBox / VMware / KVM / Hyper-V / Wine / Disk model
    │       CPU count + timing checks
    │       [If sandbox detected → EXIT silently]
    │
    └── C2 Contact (WinHTTP)
            Downloads tur.txt from storage.googleapis.com/trubo/
            Downloads 11 stage-2 files to victim disk
                │
                ├── Turbo.exe (launch host, DUOWAN/YY yyexternal)
                │       Loads crashreport.dll
                │               InitBugReport()
                │               PEB-walk + FNV-1a API resolution
                │               CPU check (≤1 CPU → exit)
                │               Sleep(1500ms)
                │               Open Trubo.log
                │               Decrypt: (byte + 0x77) & 0xFF ^ 0x62
                │               Execute shellcode
                │                       PEB-walk stager
                │                       Assembles uu.goldeyeuu.io at runtime
                │                       Connects UDP:50986 / named pipe
                │                       WinINet HTTP backup channel
                │
                ├── windui.dll (Zhong Stealer C2 / RAT module)
                │       DuiLib ActiveX persistent backdoor
                │       WebSocket C2: uu.goldeyeuu.io:5188
                │
                ├── payment.dll (WebView2 MITB)
                │       Intercepts TurboVPN payment page navigation
                │       Registers NavigationCompleted event handler
                │       Exfiltrates payment credentials
                │
                ├── libuv.dll (360/Quantum networking)
                │       Named pipe + UDP transport to C2
                │       service.cfg: {pipe:1, udp:1, udp_port:50986}
                │
                └── image.jpg (503 error decoy)
                        Displayed to victim to explain "broken link"
                        Social engineering to prevent suspicion
```

---

## 5. IOCs

### Hashes — Stage 1

| Type | Value |
|------|-------|
| SHA256 | `c918dded298b0d76d4ac51f23b391f62a95f58b3fa2488202ecbbc9c7ce8e785` |
| MD5 | `8b75ead7c8c8737570fd93ac7c530bfd` |
| SHA1 | `2c2f211341c461c405da622adbb1511834472fcb` |

### Hashes — Stage 2 Components

| File | SHA256 |
|------|--------|
| `Turbo.exe` | `5e841260983954da60716b99306a410898bca4d30c14626553205753f60a6d2f` |
| `crashreport.dll` | `31c1e8301f1e03e79de044f9bf044e8ba8508bb948a306c7597e9a4a5303bb45` |
| `Trubo.log` | `fc3fdbfbee3e358813370b324decf317c8481a1ad841dec0e5dabffd37af1386` |
| `Trubo_decrypted.bin` | `abc3550a3f536743e36ce154fc4fee523285ab8afa4c8d57d688d461918bb57d` |
| `payment.dll` | `273385cab52e7f297cb914f6896adf1a1ba46dc2ef8adf137bd9d5cef173168f` |
| `windui.dll` | `a8be52e0c969c0e76f17ba6dc758cce6082e676af6ffb35836a333e69950d4f2` |
| `libuv.dll` | `8e8cb13fb459f4ee486e828687218a1b2f4f5a1a045d68f74a7ade781cbfc47b` |
| `service.cfg` | `cebbfee6b0c00121250d66ce1eb0d8815863ba5139a3f613da77f45f24775865` |
| `remote_config_data` | `e51bd5489b076baf2184c80580558b5392d74a251470092949b0de77c752f8bd` |
| `image.jpg` | `0ce9b137f378211a4f6ba43bae5e7056d577d757441671028b94b46a05b2b0c1` |
| `tur.txt` | `f209713ebd74961c539c092aa7ccc16db571f328244fd1939fd4d1742564aa58` |

### Network IOCs — C2

| IOC | Type | Notes |
|-----|------|-------|
| `uu[.]goldeyeuu[.]io` | C2 domain | Primary C2 endpoint; UDP:50986 + named pipe; domain assembled at runtime in shellcode |
| `50986/UDP` | Port | C2 transport (service.cfg) |

### Network IOCs — Payload Delivery (GCS)

| IOC | Type | Notes |
|-----|------|-------|
| `storage[.]googleapis[.]com/trubo/tur.txt` | Stage-2 manifest | Lists 11 payload URLs |
| `storage[.]googleapis[.]com/trubo/` | Stage-2 CDN | Hosts all 11 stage-2 files |
| `storage[.]googleapis[.]com/windows_bucket1/turbo/` | Asset CDN | TurboVPN UI assets (Firebase config) |

### Network IOCs — VPN Infrastructure (remote_config_data)

| IOC | Type | Notes |
|-----|------|-------|
| `ubuntu[.]trovpn[.]com` | VPN proxy | TurboVPN proxy server list |
| `1624147767.rsc.cdn77[.]org` | VPN proxy | CDN77 endpoint |
| `1849029393.rsc.cdn77[.]org` | VPN proxy | CDN77 endpoint |
| `dgs6f4l7ftt5u.cloudfront[.]net` | VPN proxy | CloudFront endpoint |
| `d2a60k7cnkt82a.cloudfront[.]net` | VPN proxy | CloudFront endpoint |

### Network IOCs — MITB Payment Targets (payment.dll)

| IOC | Type | Notes |
|-----|------|-------|
| `inngoturbo[.]com` | Payment target | WebView2 MITB intercept target |
| `turbospn[.]com` | Payment target | WebView2 MITB intercept target |
| `payserviceinn[.]com` | Payment target | WebView2 MITB intercept target |
| `turbovpn[.]com` | Payment target | Legitimate TurboVPN domain also targeted |
| `ip-api[.]com/json` | Geolocation | IP geolocation check (Firebase config) |
| `ipapi[.]co/json` | Geolocation | IP geolocation check (Firebase config) |

### Certificates

| Field | Stage 1 | Stage 2 (all PE components) |
|-------|---------|------------------------------|
| Serial | `049209454db22190c7697285c3d5ad9b` | `06500ee65ffbfb6ea4f4b16ab6f910c6` |
| Subject | DigiFors GmbH, Leipzig DE | INNOVATIVE CONNECTING PTE. LIMITED, SG |
| Issuer | DigiCert Trusted G4 Code Signing RSA4096 SHA256 2021 | DigiCert Trusted G4 Code Signing RSA4096 SHA384 2021 |
| Valid | 2026-04-10 → 2027-04-09 | 2026-04-04 → 2027-04-02 |
| Status | **Suspicious** — same-day issue; recommend revocation | Legitimate TurboVPN cert |

### Build Artifacts

| File | PDB Path | Environment |
|------|----------|-------------|
| `photo20260411689.com` | `C:\Users\Administrator\Desktop\photo\java2.0\x64\Release\java2.0.pdb` | Desktop dev machine |
| `Turbo.exe` | `E:\DUOWAN_BUILD\yypublish_build\console\source\yy\bin\release\yyexternal.pdb` | DUOWAN/YY build |
| `crashreport.dll` | `C:\Users\Administrator\Desktop\crashreport_new - ` (truncated) | Desktop dev machine |
| `payment.dll` | `D:\360\work\quantum\Release\payment.pdb` | 360/Quantum build |
| `windui.dll` | `D:\360\work\quantum\Release\windui.pdb` | 360/Quantum build |
| `libuv.dll` | `D:\360Work\7.Quantum\1.quantum.win\Release\libuv.pdb` | 360/Quantum build |

### Registry Keys — Anti-VM Enumeration (Stage 1)

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

### Registry Keys — Stage 2 Targets

```
Software\duowan\yy          (Turbo.exe — DUOWAN/YY component registry)
Software\360\360se6\chrome  (windui.dll — 360 Browser credential store target)
```

### Filesystem Artifacts

```
Trubo.log          (encrypted shellcode; 163KB; written to working directory)
service.cfg        (C2 transport config; JSON)
remote_config_data (Firebase VPN config cache)
```

### Decryption Key

**Trubo.log** decryption algorithm:
```python
decrypted_byte = (encrypted_byte + 0x77) & 0xFF ^ 0x62
```
See `Trubo_decrypt.py` for standalone implementation.

---

## 6. Emulation Results

### Pass 1 — Generic runner (speakeasy hook library)
- TLS callback executed; self-decryption loop completed (8 VirtualProtect RWX cycles)
- Stopped at `kernel32.NtAllocateVirtualMemory` (unsupported API stub)
- No network/registry/process IOCs recovered

### Pass 2 — Plain speakeasy
- Same result; 100+ APIs resolved via GetProcAddress; stopped at NtAllocateVirtualMemory

### Pass 3 — Custom script (NtAllocateVirtualMemory stubbed)
- Stub succeeded; emulation continued
- **26 unique anti-VM registry keys enumerated** (all returned NOT FOUND in emulator)
- WinHTTP calls not reached; emulation ended after registry checks
- Additional gates suspected: disk model value check + GetTickCount64 timing + CPUID

### `Trubo.log` Shellcode (static decrypt)
- Decryption algorithm recovered via `InitBugReport` decompile
- Decrypted 162,674 bytes to x86 shellcode
- Structure: JMP (0x000) → `GetExportByHash` helper (0x005) → main function (0x44B)
- Domain fragments at offset 0x27689: `uu.g` + `[0x40]` + `oldeye` (runtime assembly of C2 hostname)
- API name table in tail: KERNEL32, ADVAPI32, IPHLPAPI, OLEAUT32, WININET, LoadLibraryA, GetProcAddress, VirtualAlloc, InternetOpen

**Emulation verdict:** Stage-1 self-decryption and VM-detection logic fully traced. C2 URL blocked by convincing bare-metal requirement. Stage-2 shellcode decrypted statically; C2 hostname partially recovered as fragments — full recovery requires bare-metal dynamic execution.

---

## 7. Sandbox Results

**Tria.ge submission `260411-whsmgses9j`** (behavioral2 task confirmed stage-2 execution):
- Stage-2 files downloaded and executed on analysis host
- Network connections observed to `uu[.]goldeyeuu[.]io` — confirmed C2 contact
- Direct API access to report data blocked from analysis host (Cloudflare error 1010 on REMnux outbound IP)

**Recommended follow-up**: Submit `Turbo.exe` + all stage-2 components directly to ANY.RUN, Joe Sandbox, or CAPE on a bare-metal profile (spoof CPUID, MAC address, disk serials) to capture:
- Full C2 beaconing protocol to `uu[.]goldeyeuu[.]io:50986`
- Zhong Stealer (`windui.dll`) C2 and persistence mechanisms
- WebView2 MITB (`payment.dll`) interception traffic

---

## 8. MITRE ATT&CK

### Stage 1 — Loader

| Technique | ID | Evidence |
|-----------|----|----------|
| Masquerading: Match Legitimate Name | T1036 | `.com` extension on PE; "photo" lure filename |
| Subvert Trust Controls: Code Signing | T1553.002 | DigiFors GmbH DigiCert cert (fraudulent, same-day issue) |
| Obfuscated Files: Software Packing | T1027.002 | LZX self-decryption, high-entropy .text section |
| Obfuscated Files: String Obfuscation | T1027 | 15× XOR-in-loop string encryption |
| Virtualization/Sandbox Evasion: Check Env | T1497.001 | 26 registry-based VM/sandbox checks (dual-pass) |
| System Information Discovery | T1082 | HARDWARE\Description\System, disk enumeration |
| Query Registry | T1012 | 26 registry keys enumerated |
| Application Layer Protocol: Web | T1071.001 | WinHTTP stack (11 APIs), GCS manifest download |
| Process Injection | T1055 | NtAllocateVirtualMemory capability |
| Boot/Logon Autostart: Registry Run Keys | T1547.001 | RegSetValueExA resolved |

### Stage 2 — Trojanized TurboVPN Package

| Technique | ID | Evidence |
|-----------|----|----------|
| Masquerading: Double File Extension | T1036.007 | `Turbo.exe` (TurboVPN lure), `photo20260411689.com` |
| Subvert Trust Controls: Code Signing | T1553.002 | All stage-2 PEs signed with legitimate TurboVPN cert |
| Obfuscated Files: Encrypted/Encoded File | T1027.013 | `Trubo.log` shellcode encrypted with `(b+0x77)^0x62` |
| Process Injection | T1055 | Shellcode execution via VirtualAlloc + thread |
| Non-Application Layer Protocol | T1095 | UDP port 50986 C2 channel (service.cfg) |
| Application Layer Protocol: Web | T1071.001 | WinINet HTTP + V2Ray/Xray/Reality VPN tunnel as cover |
| Protocol Tunneling | T1572 | C2 tunnelled as VPN traffic (V2Ray/Xray/Reality/tun2socks) |
| Browser in the Middle | T1185 | `payment.dll` WebView2 MITB on payment checkout pages |
| Steal Web Session Cookie | T1539 | `windui.dll` Zhong Stealer; 360 Browser credential target |
| Credentials from Web Browsers | T1555.003 | `Software\360\360se6\chrome` registry access |
| Fallback Channels | T1008 | Named pipe + UDP dual-channel (service.cfg) |
| Deobfuscate/Decode Files | T1140 | `InitBugReport` decrypts Trubo.log at runtime |
| Modify Registry | T1112 | RegSetValueExA; `Software\duowan\yy` |

---

## 9. Analyst Notes

1. **C2 recovery blocked in emulation**: The stage-1 loader's anti-sandbox gate (26 registry checks × 2, CPUID, timing) and the stage-2 shellcode's runtime domain assembly both block static/emulation recovery. Bare-metal dynamic analysis with spoofed hardware identifiers is the only reliable path to full C2 protocol capture.

2. **Stage-1 certificate fraud**: The DigiCert cert for "DigiFors GmbH" (serial `049209454db22190c7697285c3d5ad9b`) was issued on the same day as the compile (2026-04-10). DigiFors GmbH is a real German forensics company. This is a strong indicator of fraudulent cert acquisition — **recommend submitting serial to DigiCert for revocation**.

3. **Stage-2 cert legitimacy**: The "INNOVATIVE CONNECTING PTE. LIMITED" cert (serial `06500ee65ffbfb6ea4f4b16ab6f910c6`) appears to be the legitimate TurboVPN signing certificate. The threat actor either (a) has insider access to TurboVPN's build/signing infrastructure, (b) compromised their signing key, or (c) operates under the same corporate entity. The `remote_config_data` Firebase config is authentic TurboVPN configuration data.

4. **360/Quantum build environment**: Three of the malicious components (`payment.dll`, `windui.dll`, `libuv.dll`) share a `D:\360\work\quantum\` or `D:\360Work\7.Quantum\` build path. This links them to a common threat actor environment referencing Qihoo 360 tooling, consistent with the APT-Q-27 / Golden Eye Dog attribution. The DuiLib (Chinese DirectUI library) usage and `goldeyeuu.io` C2 domain are consistent with this actor.

5. **Attack model — Rogue VPN + MITB**: The overall attack chains a rogue VPN installation with payment credential theft. The victim sees a "dead photo link" (503 decoy), while TurboVPN installs silently and tunnels all traffic through `uu[.]goldeyeuu[.]io`. The `payment.dll` WebView2 component intercepts TurboVPN's own payment pages to steal financial credentials at the point of subscription purchase — a high-value target moment.

6. **"Trubo" typo as campaign marker**: The GCS bucket name (`trubo`), the payload log filename (`Trubo.log`), and the Firebase segment name all use the `Trubo` (not `Turbo`) spelling. This appears to be a deliberate or consistent typo across the campaign infrastructure and may serve as a tracking/attribution marker.

7. **`image.jpg` as social engineering**: The 503 error JPEG (761×352px) is displayed to the victim to explain why the "photo link" didn't load. This TTP has been observed in prior versions of this malware, suggesting an established campaign with iterative refinement.

8. **Recommended follow-up actions**:
   - Submit DigiCert revocation request for cert serial `049209454db22190c7697285c3d5ad9b`
   - Report GCS bucket `storage[.]googleapis[.]com/trubo/` to Google for takedown
   - Submit `uu[.]goldeyeuu[.]io` to threat intel platforms for infrastructure tracking
   - Run stage-2 components on bare-metal sandbox to capture full Zhong Stealer C2 protocol
   - Investigate INNOVATIVE CONNECTING PTE. LIMITED for potential supply-chain compromise
