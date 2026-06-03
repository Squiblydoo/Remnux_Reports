# Analysis Report: image2026060356.com

**Date:** 2026-06-03  
**Analyst:** REMnux Workstation  
**Filename:** `image2026060356.com`

---

## 1. File Metadata

| Field | Value |
|-------|-------|
| **SHA256** | `a60f58618c8548ff60978d7c50176bebb38486690d87ba24a8581b62dd478228` |
| **MD5** | `d760b0cee0d67f1024f099edf280ea07` |
| **SHA1** | `a179996effa6eaa7a4268ea3a5b9a615d3478d26` |
| **File Type** | PE32 executable (GUI) Intel 80386 / .NET 4.8 assembly |
| **Size** | 27,072 bytes |
| **Architecture** | x86 (.NET IL bytecode) |
| **Internal Name** | `java.net.exe` |
| **Namespace** | `java.net` |
| **VersionInfo Company** | Amazon.com (spoofed) |
| **VersionInfo Copyright** | "Copyright © Amazon.com 2026" (spoofed) |
| **Code Signing Cert** | Subject: **Biao Zhao** |
| | Issuer: Certum Code Signing 2021 CA (Asseco Data Systems S.A., PL) |
| | Serial: `5bd368336404058da643d99a3bbeb530` |
| | Validity: 2026-03-18 → 2027-03-18 |
| | Org: Biao Zhao / Sichuan / Nanchong / CN |
| **PDB Path** | `C:\Users\Administrator\Desktop\084049\java.net\java.net\obj\Release\java.net.pdb` |
| **Debug Timestamp** | 2062-11-12 (forged — future date) |

---

## 2. Classification

**Family:** ZhongStealer (APT-Q-27) first-stage dropper  
**Confidence:** Confirmed (high)  
**Reasoning:**

- KesaKode online: All matches < 3% (no known family match; custom/novel dropper)
- ANY.RUN tags: `apt-q-27`, `loader`, `backdoor`, `websocket`, `evasion`
- **Payload identity match**: `crashreport.dll` downloaded from C2 bucket has SHA256 `27b722c66f69e360c4da106daacf3b9eeaabd20634d7e5eff45a28bd70ebfd65` — exact match to previously attributed ZhongStealer loader (same binary, new staging bucket)
- **Payload identity match**: `updat.log` downloaded from C2 bucket has SHA256 `3313f347e83aaf48ea31fb1d49fc37452f48f81d20a1b93009e2e78385ff4bba` — exact match to previously analyzed encrypted ZhongStealer stage-2 payload
- **Build artifact match**: PDB path contains `Desktop\084049\` — same build directory as `crashreport.dll` PDB path (`C:\Users\Administrator\Desktop\084049\crashreport_new\Release\crashreport_new.pdb`), confirming same developer workstation
- ANY.RUN DNS resolution: `uu.goldeyeuu.io` (rep=2 malicious) — the established ZhongStealer C2

This sample is a **new wave** of the ZhongStealer staged delivery chain, migrated from the `yynewyy` GCS bucket to a new `kiki001` bucket, with identical payload hashes.

---

## 3. Capabilities

- **Anti-analysis / sandbox evasion** (10 distinct check categories, all in `CS()` — see Section 5)
- **Downloads resource list** from GCS staging bucket (`kiki001/as.txt`)
- **Downloads and executes** arbitrary EXE payloads listed in the resource file
- **Opens decoy images** (JPG/PNG/BMP/GIF/JPEG) listed in the resource file
- **Downloads and displays decoy PDF** from `pkgnew/image.pdf` on the Desktop
- **Self-deletes** via a hidden BAT file loop after execution
- **No persistence mechanism** — one-shot dropper, relies on payloads for persistence

---

## 4. Attack Chain

```
image2026060356.com (this sample, .NET dropper)
  │
  ├─ CS() × 2 — sandbox checks (30s sleep between attempts)
  │     If sandbox detected both times → Environment.Exit(0)
  │
  ├─ Download: storage.googleapis.com/kiki001/as.txt → %TEMP%\ps_list.txt
  │
  ├─ For each URL in as.txt → download to C:\Users\Public\Videos\
  │     kiki001/crashreport.dll   → SHA256: 27b722c6... (ZhongStealer loader)
  │     kiki001/updat.log         → SHA256: 3313f347... (encrypted stage-2)
  │     kiki001/vcruntime140.dll  → SHA256: 8e085754... (legitimate runtime)
  │     kiki001/updat.exe         → SHA256: 2b007100... (legitimate YY sideload host)
  │     kiki001/msvcp140.dll      → SHA256: e4c71980... (runtime/possible trojanized DLL)
  │     kiki001/image.jpg         → [404 Not Found — removed from bucket]
  │
  ├─ RUNALL() — Process.Start() on all downloaded EXEs
  │     → updat.exe sideloads crashreport.dll via InitBugReport export
  │     → crashreport.dll reads updat.log, decrypts (byte+0x77)^0x62, executes shellcode
  │     → shellcode loads ZhongStealer core → C2: uu.goldeyeuu.io:5188
  │
  ├─ Download: storage.googleapis.com/pkgnew/image.pdf (decoy PDF, 13.8KB)
  │     → Copy to Desktop as <malware_filename>.pdf → Process.Start() (open for victim)
  │
  └─ SelfDelete() — write+execute hidden BAT to del /f /q the dropper EXE
```

---

## 5. Anti-Analysis and Sandbox Evasion Checks

All evasion logic lives in the `CS()` function at VA `0x004024d0` (malcat metadata token offset: `ea=1744`). Three helper functions are called from `CS()`. The function is called twice from `Main()` before any payload activity — if it returns `true` both times (30 second sleep between calls), the process exits cleanly.

### 5.1 Process Blacklist — `CP()` helper, called from `CS()`

**Function VA:** `0x00402398` (malcat ea: `1432`)  
**Method:** `Process.GetProcessesByName(name)` — returns true if any matching process exists.  
**Sets:** `sand_Reason += "PROC:" + processname + "|"`

Checked processes (19 total):

| Process Name | Category |
|---|---|
| `vmsrvc` | VMware service |
| `vboxservice` | VirtualBox service |
| `vboxtray` | VirtualBox tray |
| `procmon` | Sysinternals Process Monitor |
| `procexp` | Sysinternals Process Explorer |
| `regmon` | Sysinternals Regmon |
| `filemon` | Sysinternals Filemon |
| `tcpview` | Sysinternals TCPView |
| `wireshark` | Network analyzer |
| `fakenet` | FLARE FakeNet |
| `sandboxie` | Sandboxie isolation |
| `sbiesvc` | Sandboxie service |
| `vmwaretray` | VMware tray |
| `vmwareuser` | VMware user process |
| `vmtoolsd` | VMware Tools daemon |
| `google_osconfig` | GCP OS Config agent |
| `google_metadata_script_runner` | GCP metadata runner |
| `gce_agent` | Google Compute Engine agent |
| `cexecsvc` | Windows Sandbox execution service |

### 5.2 Windows Sandbox Username Check — `CU()` helper, called from `CS()`

**Function VA:** `0x004023e8` (malcat ea: `1512`)  
**Method:** `Environment.UserName.ToLower() == "wdagutilityaccount"`  
**Sets:** `sand_Reason += "WIN_SANDBOX_USER:WDAGUtilityAccount|"`  
**Detects:** Windows Sandbox (WDAG container), which uses this fixed username.

### 5.3 DNS Domain Check via WMI — `CDNS()` helper, called from `CS()`

**Function VA:** `0x00402424` (malcat ea: `1572`)  
**Method:** WMI query `SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled=True`, reads `DNSDomain` property.  
**Check:** `obj.ToString().ToLower() == "mshome.net"`  
**Sets:** `sand_Reason += "WIN_SANDBOX_DNS:mshome.net|"`  
**Detects:** Older Windows sandbox network configurations that use `mshome.net` DNS suffix.

### 5.4 Filesystem Path / File Existence Checks — `CS()` inline

**Function VA:** `0x004024d0` (malcat ea: `1744`)  
**Method:** `Directory.Exists(path) || File.Exists(path)` for each path.  
**Sets:** `sand_Reason += "PATH:" + path + "|"`

Checked paths (7 total):

| Path | Detects |
|---|---|
| `C:\Program Files\Sandboxie\` | Sandboxie installation |
| `C:\Program Files\VMware\VMware Tools\` | VMware Tools |
| `C:\Program Files\Oracle\VirtualBox Guest Additions\` | VirtualBox Guest Additions |
| `C:\windows\system32\drivers\vmmouse.sys` | VMware mouse driver |
| `C:\windows\system32\drivers\vboxguest.sys` | VirtualBox guest driver |
| `C:\Program Files\Google\Compute Engine\` | GCP Compute Engine |
| `C:\Program Files\Google\Google Compute Engine\` | GCP Compute Engine (alt path) |

### 5.5 MAC Address Prefix Check via WMI — `CS()` inline

**Function VA:** `0x004024d0` (malcat ea: `1744`)  
**Method:** WMI query `SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled=True`, reads `MACAddress` property, calls `.StartsWith()` for each prefix.  
**Sets:** `sand_Reason += "MAC:" + mac + "|"`

Checked MAC prefixes (5 total):

| Prefix | Vendor / Sandbox |
|---|---|
| `00:0c:29` | VMware (common workstation) |
| `00:50:56` | VMware (ESX/vSphere) |
| `00:05:69` | VMware (older) |
| `08:00:27` | VirtualBox |
| `42:01:0a` | Google Cloud Platform (GCE) |

### 5.6 Hardware Model / Manufacturer Check via WMI — `CS()` inline

**Function VA:** `0x004024d0` (malcat ea: `1744`)  
**Method:** WMI query `SELECT * FROM Win32_ComputerSystem`, reads `Model` and `Manufacturer` properties.  
**Sets:** `sand_Reason += "HW:" + model + "|"`

Trigger strings (checked with `Contains()`):

| String | Field | Detects |
|---|---|---|
| `VMware` | Model or Manufacturer | VMware VM |
| `VirtualBox` | Model | VirtualBox VM |
| `innotek` | Manufacturer | VirtualBox (parent org) |
| `Google` | Model or Manufacturer | GCP / GCE |
| `Compute Engine` | Model | GCP Compute Engine |

### 5.7 BIOS Manufacturer Check via WMI — `CS()` inline

**Function VA:** `0x004024d0` (malcat ea: `1744`)  
**Method:** WMI query `SELECT * FROM Win32_BIOS`, reads `Manufacturer` property.  
**Sets:** `sand_Reason += "BIOS:" + manufacturer + "|"`

Trigger: `manufacturer.Contains("Google")` — detects GCP instance BIOS.

### 5.8 Disk Size Check — `CS()` inline

**Function VA:** `0x004024d0` (malcat ea: `1744`)  
**Method:** `new DriveInfo("C:").TotalSize / 1073741824.0 < 40.0`  
**Sets:** `sand_Reason += "DISK:" + size + "GB|"`  
**Detects:** Sandbox VMs with small provisioned C: drives (< 40 GB).

### 5.9 GCP Driver File Checks — `CS()` inline

**Function VA:** `0x004024d0` (malcat ea: `1744`)  
**Method:** `File.Exists("C:\\Windows\\System32\\drivers\\" + drivername)`  
**Sets:** `sand_Reason += "DRV:" + filename + "|"`

Checked driver filenames (3 total):

| Driver | Detects |
|---|---|
| `gv3.sys` | GCP virtual GPU driver |
| `gcbd.sys` | GCP block device driver |
| `gce_bridge.sys` | GCP network bridge driver |

### 5.10 GCE Environment Variable Checks — `CS()` inline

**Function VA:** `0x004024d0` (malcat ea: `1744`)  
**Method:** `!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(varname))`  
**Sets:** `sand_Reason += "ENV:" + varname + "|"`

Checked environment variables (3 total):

| Variable | Detects |
|---|---|
| `GCE_METADATA_HOST` | GCP Compute Engine metadata server |
| `GCE_METADATA_ROOT` | GCP Compute Engine metadata root |
| `GOOGLE_CLOUD_PROJECT` | GCP project environment |

### Evasion Summary

The `CS()` function runs 10 independent check categories. Any positive result sets `sand_Flag = true`. The function returns `sand_Flag` after exhausting all checks (it does **not** short-circuit). This means the `sand_Reason` string accumulates **all** detected indicators, which suggests it may be reported back (though no exfiltration of `sand_Reason` is observed in this stage).

The double-call pattern in `Main()`:
```csharp
if (CS()) {
    Thread.Sleep(30000);  // 30-second delay
    if (CS()) {
        Environment.Exit(0);
    }
}
```
This implements a basic anti-timing check: if the sandbox is detected on first pass, it waits 30 seconds. Many sandboxes time out within this window. Only if *both* calls return true does the process exit cleanly.

---

## 6. Downloaded Payload Hashes (kiki001 bucket)

| Filename | SHA256 | Notes |
|---|---|---|
| `crashreport.dll` | `27b722c66f69e360c4da106daacf3b9eeaabd20634d7e5eff45a28bd70ebfd65` | ✅ **EXACT MATCH** to previously analyzed ZhongStealer loader |
| `updat.log` | `3313f347e83aaf48ea31fb1d49fc37452f48f81d20a1b93009e2e78385ff4bba` | ✅ **EXACT MATCH** to previously analyzed encrypted ZhongStealer stage-2 payload |
| `updat.exe` | `2b0071007c3f5fa8e949a8de53be03e97901dd505694ca939b575a49e4fdbdbb` | Legitimate YY sideload host (yyexternal.exe) |
| `vcruntime140.dll` | `8e08575492175e042f093f325b07a5c14ca71e7c581474838db3d48f5aab1312` | Legitimate Microsoft runtime |
| `msvcp140.dll` | `e4c71980dbb4a1e1a86816687afdaea043b639b531135fc4516fb2429fe623fc` | Different hash from known trojanized version — may be legitimate or new variant |
| `image.jpg` | N/A | **404 Not Found** (removed from bucket at time of analysis) |

Decoy document:
| File | SHA256 | Notes |
|---|---|---|
| `pkgnew/image.pdf` | (13,830 bytes) | Single-page JPEG-embedded PDF; legitimate decoy document |

---

## 7. IOCs

### Network (all defanged)

| Type | IOC | Notes |
|---|---|---|
| URL | `hxxps://storage[.]googleapis[.]com/kiki001/as.txt` | Resource list (stage 1 C2) |
| URL | `hxxps://storage[.]googleapis[.]com/kiki001/crashreport[.]dll` | ZhongStealer loader |
| URL | `hxxps://storage[.]googleapis[.]com/kiki001/updat[.]log` | Encrypted stage-2 payload |
| URL | `hxxps://storage[.]googleapis[.]com/kiki001/updat[.]exe` | Sideload host |
| URL | `hxxps://storage[.]googleapis[.]com/kiki001/msvcp140[.]dll` | Runtime dependency |
| URL | `hxxps://storage[.]googleapis[.]com/kiki001/vcruntime140[.]dll` | Runtime dependency |
| URL | `hxxps://storage[.]googleapis[.]com/kiki001/image[.]jpg` | Decoy image (404) |
| URL | `hxxps://storage[.]googleapis[.]com/pkgnew/image[.]pdf` | Decoy PDF |
| Domain | `uu[.]goldeyeuu[.]io` | ZhongStealer C2 (port 5188) — confirmed via ANY.RUN DNS resolution, rep=2 malicious |

### Filesystem

| Path | Purpose |
|---|---|
| `C:\Users\Public\Videos\` | Payload drop directory (created if absent) |
| `C:\Users\Public\Videos\crashreport.dll` | ZhongStealer loader dropped here |
| `C:\Users\Public\Videos\updat.exe` | Sideload host dropped here |
| `C:\Users\Public\Videos\updat.log` | Encrypted payload dropped here |
| `C:\Users\Public\Videos\msvcp140.dll` | Runtime/DLL dropped here |
| `C:\Users\Public\Videos\vcruntime140.dll` | Runtime dropped here |
| `%TEMP%\ps_list.txt` | Temporary resource list (deleted after parsing) |
| `%TEMP%\temp_image.pdf` | Temporary decoy PDF (deleted after open) |
| `%USERPROFILE%\Desktop\<malware_name>.pdf` | Decoy PDF copied to Desktop for victim display |
| `%TEMP%\<GUID>.bat` | Self-delete batch script (hidden, deletes self) |

### Registry

No registry writes observed in this stage. All persistence is handled by the `crashreport.dll` → `updat.log` chain (which writes `HKCU\...\Run` persistence as documented in prior analysis).

### Mutexes / Other

No mutexes created in this stage.

---

## 8. Emulation Results

Speakeasy emulation was not attempted — this is a .NET assembly requiring the .NET 4.8 runtime, which speakeasy does not support. All behavioral information was obtained from:
- `ilspycmd` full decompilation (complete, no obfuscation)
- ANY.RUN sandbox (full execution observed)
- Direct download of C2-hosted payloads

ANY.RUN executed the full chain and captured DNS resolution of `uu.goldeyeuu.io`.

---

## 9. Sandbox Results

| Field | Value |
|---|---|
| **Score** | 100/100 |
| **Threat Level** | Malicious activity |
| **Tags** | `apt-q-27`, `loader`, `backdoor`, `websocket`, `evasion`, `auto-reg`, `upx` |
| **DNS IOCs** | `uu.goldeyeuu.io` (rep=2 malicious) |
| **Public Report** | https://app.any.run/tasks/58d5e350-e851-4b5e-8a7e-e1c6d6442e5c |

The `upx` tag likely applies to the `updat.exe` YY binary (UPX-packed). The `websocket` and `backdoor` tags confirm the ZhongStealer core ran to C2 contact.

---

## 10. Analyst Notes

**New GCS bucket:** The `yynewyy` bucket used in the May 2026 campaign has been replaced by `kiki001`. The payload list file was renamed from `ps.txt` → `as.txt`. The payload binaries are identical (same SHA256 hashes for `crashreport.dll` and `updat.log`).

**No obfuscation on the dropper:** This .NET dropper has zero obfuscation — all function names, strings, and logic are plaintext. The anti-analysis protection is entirely behavioral, relying on the 10-category VM/sandbox detection in `CS()`.

**Build environment match:** The PDB path `Desktop\084049\` appears verbatim in both this dropper's PDB (`java.net\...\java.net.pdb`) and the previously analyzed `crashreport.dll` (`crashreport_new\Release\crashreport_new.pdb`), confirming they were built on the same developer machine within the same project directory.

**msvcp140.dll hash difference:** The `msvcp140.dll` in this `kiki001` bucket (`e4c71980...`) differs from the trojanized version in the `bsnet` analysis (`2571a8f5...`). The `bsnet` variant was used with `RevoSrp.exe` via exit() IAT hook; the version here may be the legitimate Microsoft runtime used as a genuine sideloading dependency for `updat.exe`, or a different trojanized variant not yet analyzed.

**image.jpg placeholder:** The resource list references `kiki001/image.jpg` but the object has been removed from the bucket (returns 404 XML). This slot may have previously held a decoy image or a second-stage payload.

**Recommended follow-up:**
- Hash `msvcp140.dll` (`e4c71980...`) against VirusTotal and compare exports/behavior to the `bsnet` variant
- Monitor `kiki001` and `pkgnew` GCS buckets for new objects or updated payloads
- Check if `updat.exe` hash `2b007100...` matches the previously analyzed legitimate YY binary or a new variant
