# Malware Analysis Report: photo20260528899.com

**Date:** 2026-05-29  
**Analyst:** Claude Code (automated analysis)

---

## 1. File Metadata

| Field | Value |
|-------|-------|
| Filename | photo20260528899.com |
| SHA256 | `b1e6036407ac561deebf5a4885fda4d63686bdfbf808524e7554ea339a7bbe39` |
| SHA1 | `190968beeea39364a83cea8370a7348739c937ea` |
| MD5 | `dd931872204b283b37cd3dd148d9ad2f` |
| Size | 742,128 bytes (724 KB) |
| Type | PE32 executable (GUI) Intel 80386, Mono/.NET assembly |
| Arch | x86 / .NET Framework 4.8 |
| Namespace | `WindowsService.Agent` |
| Internal name | `logs.exe` |
| Assembly GUID | `ffe518e3-c674-4cc0-bad3-1245c10d7a91` |
| PE Timestamp | 2103-03-23 09:38:23 UTC (**far-future, forged**) |

**Code Signing Certificate:**
- Issuer: Sectigo Public Code Signing CA EV R36
- Subject: **Xiamen Shunhuitong E-commerce Co., Ltd.**
- State: Fujian Sheng, CN
- Serial: `3eaa4bd40d5da98036b33023e0052869`
- Valid: 2026-03-26 → 2027-03-26 (1-year EV cert)

**VersionInfo (spoofed to impersonate Amazon):**
- CompanyName: `Amazon.com`
- FileDescription: `logs`
- InternalName: `logs.exe`
- LegalCopyright: `Copyright © Amazon.com 2026`

---

## 2. Classification

**Family:** Unattributed .NET Downloader + WebSocket Backdoor  
**Confidence:** High (malicious; confirmed by dynamic analysis)  
**APT Attribution:** ANY.RUN tags `apt-q-27` — **treat as hypothesis only**; no independent corroboration from static IOCs  

**Reasoning:**
- ANY.RUN verdict 100/100 with tags: `loader`, `backdoor`, `websocket`, `evasion`, `auto-reg`, `apt-q-27`
- Dynamic execution confirmed download of 7 staged payloads from GCS and WebSocket C2 beacon to `uu[.]goldeyeuu[.]io`
- All method bodies virtualized by custom .NET VM — static decompilation yields stubs only
- KesaKode online lookup: **no match** (script bug prevented authoritative lookup; offline score `EnemyBot` confidence=0, discard)

---

## 3. Capabilities

- **Custom .NET VM obfuscation**: 452 non-ASCII function names, 285 spaghetti-code functions, all method bodies replaced with `throw new Exception("Runtime exception")` stubs; actual logic runs through VM bytecode embedded in high-entropy `.text` section (entropy=109, 383 KB)
- **Anti-debug**: `IsDebuggerPresent`, `CheckRemoteDebuggerPresent`, `NtQueryInformationProcess` (ProcessDebugPort), `NtQuerySystemInformation` (SystemKernelDebuggerInformation), `NtSetInformationDebugObject`
- **Multi-vector sandbox evasion**: `SuspiciousProcesses`, `SuspiciousPaths`, `SuspiciousMacPrefixes`, `SuspiciousDrivers`, `SuspiciousEnvVars` arrays; WMI-based process/driver enumeration (`ManagementObjectSearcher`); Linux `/proc/{pid}/status` TracerPid check; `windir` env-var check
- **SSL validation bypass**: `set_ServerCertificateValidationCallback` returns true (HTTPS MITM/inspection bypass)
- **Staged downloader**: `ServiceManager.RetrieveResourceList(configEndpoint)` fetches JSON resource list from C2; `ServiceManager.DownloadResourceFile(resourceUrl)` streams files to disk; `MaxParallelDownloads`, `throttleController` — parallel download with rate limiting
- **Payload execution**: `ExecuteUpdateProcess()` → `LaunchExecutable(executablePath)`; launches `updat.exe`
- **WebSocket backdoor**: Establishes persistent WebSocket connection to `uu[.]goldeyeuu[.]io`
- **Cross-platform indicators**: References to `/proc/self/status`, `/proc/self/maps`, `/usr/lib/libSystem.dylib`, `libdl.so`, `libc.so.6`, `mprotect` — likely a shared codebase targeting Windows and Linux/macOS
- **In-memory PE loading**: Embedded 3072-byte .NET PE (`_vr_jg13lkjqjklw`, module `v660a1d814e576a6d`) loaded via `Marshal.AllocHGlobal` + `Marshal.Copy` without writing to disk; acts as VM runtime loader
- **Reflective loading**: 47 `Assembly.Load`/reflection emit calls; loads subsequent stage assemblies entirely in memory
- **GZip decompression**: Payloads likely gzip-compressed in transit/storage
- **Auto-start registration** (confirmed by `auto-reg` sandbox tag)
- **RWX memory allocation** (3 hits; injects/executes shellcode or mapped PE)

---

## 4. Attack Chain

```
photo20260528899.com  (.exe renamed .com — lure via photo filename with embedded date 2026-05-28)
  │
  ├─ [1] Anti-sandbox/anti-debug checks (SuspiciousProcesses, debugger APIs, /proc TracerPid)
  │       → exits cleanly if detected (confirmed evasion: ANY.RUN caught it only with interactivity enabled)
  │
  ├─ [2] Load VM runtime (3072-byte embedded PE from .NET resource _vr_jg13lkjqjklw)
  │       in-memory via Marshal.AllocHGlobal — no disk artifact
  │
  ├─ [3] RetrieveResourceList(configEndpoint)
  │       → HTTP GET to config endpoint (URL runtime-decrypted by VM, not statically recoverable)
  │       → returns list of URLs to download
  │
  ├─ [4] Parallel download from GCS staging bucket  storage.googleapis[.]com/yynewyy/
  │       crashreport.dll  updat.log  msvcp140.dll  vcruntime140.dll  image.jpg  updat.exe  ps.txt
  │       (runtime DLLs may be trojanized; image.jpg may carry steganographic payload)
  │
  ├─ [5] ExecuteUpdateProcess() → LaunchExecutable("updat.exe")
  │       next-stage PE; capabilities unknown without sample
  │
  └─ [6] WebSocket beacon/backdoor to uu[.]goldeyeuu[.]io
          persistent C2 channel; commands unknown (backdoor tag suggests remote shell/control)
```

---

## 5. IOCs

### Network (defanged)

| Type | IOC | Notes |
|------|-----|-------|
| Domain | `uu[.]goldeyeuu[.]io` | WebSocket backdoor C2; ANY.RUN rep=2 (malicious) |
| URL | `https[://]storage.googleapis[.]com/yynewyy/crashreport.dll` | Staged DLL payload |
| URL | `https[://]storage.googleapis[.]com/yynewyy/updat.exe` | Staged EXE (next stage) |
| URL | `https[://]storage.googleapis[.]com/yynewyy/updat.log` | Staged data/config |
| URL | `https[://]storage.googleapis[.]com/yynewyy/ps.txt` | Staged script (PowerShell?) |
| URL | `https[://]storage.googleapis[.]com/yynewyy/image.jpg` | Staged image (stego?) |
| URL | `https[://]storage.googleapis[.]com/yynewyy/msvcp140.dll` | Runtime DLL (possibly trojanized) |
| URL | `https[://]storage.googleapis[.]com/yynewyy/vcruntime140.dll` | Runtime DLL (possibly trojanized) |
| URL | `https[://]login.live.com/ppsecure/deviceaddcredential.srf` | Visited during execution — possible credential phishing |
| URL | `https[://]login.live.com/RST2.srf` | Visited — WS-Trust endpoint; possible WAM/OAuth theft |

**GCS Bucket:** `yynewyy` (staging server; may be taken down; report to Google)

### Filesystem

| Path | Notes |
|------|-------|
| (runtime-constructed paths) | Download destination unknown — likely `%TEMP%` or `%APPDATA%` subtree |

### Certificate

| Field | Value |
|-------|-------|
| Serial | `3eaa4bd40d5da98036b33023e0052869` |
| Subject | Xiamen Shunhuitong E-commerce Co., Ltd. |
| Issuer | Sectigo Public Code Signing CA EV R36 |

---

## 6. Emulation Results

**Speakeasy**: Not applicable — `.NET assemblies are not currently supported` by speakeasy.

**ILSpy decompilation**: Partial. Class skeleton and method signatures recovered:
- Namespace: `WindowsService.Agent`
- Class: `ServiceManager` with `RetrieveResourceList`, `DownloadResourceFile`, `ExecuteUpdateProcess`, `LaunchExecutable`, `SetupHttpClient`, and sandbox detection methods
- All method bodies virtualized — actual logic inaccessible without VM runtime emulation
- VM opcodes: `vb_dn_vm_dec`, `vb_dn_vm_idx`, `vb_dn_vm_next` (custom .NET VM, not ConfuserEx or KoiVM standard)

---

## 7. Sandbox Results

**ANY.RUN** — verdict: **100/100 Malicious activity**  
- Tags: `evasion`, `loader`, `auto-reg`, `apt-q-27`, `backdoor`, `websocket`  
- Specs: `autoStart=true`, `multiprocessing=true`, `networkLoader=true`, `networkThreats=true`  
- Duration: 60 seconds  
- Public report: https://app.any.run/tasks/3550375d-3427-48d9-a6ab-d02e80b7d185

---

## 8. Analyst Notes

**Residual gaps:**
1. **C2 config endpoint URL**: Hardcoded URL for `RetrieveResourceList(configEndpoint)` is VM-encrypted and was not recovered statically. The GCS bucket serves as staging after config fetch.
2. **`updat.exe` / `crashreport.dll`**: Next-stage payloads were not captured from the GCS bucket for further analysis. These should be downloaded and analyzed separately.
3. **`ps.txt`**: Likely a PowerShell payload; content unknown.
4. **`image.jpg`**: May carry a steganographically embedded payload — worth examining with `stegdetect` / `zsteg`.
5. **WebSocket protocol**: Full command set and C2 protocol structure not recovered.
6. **KesaKode online**: Lookup failed due to script bug (fpath NameError in filepath mode; stdin redirect produced no output). Manual hash submission to cloud.malcat.fr recommended.
7. **Login.live.com visits**: Two Microsoft authentication endpoints (`deviceaddcredential.srf`, `RST2.srf`) were contacted. This could indicate credential phishing, token theft, or WAM OAuth abuse (compare WallStealer pattern — but no matching IOCs confirm a link).

**APT-Q-27 note**: The `apt-q-27` tag is applied by ANY.RUN's automated classifier. Independent corroboration would require matching infrastructure pivots or tool overlaps not established here. Treat as a lead, not attribution.

**Lure / delivery**: Filename `photo20260528899.com` embeds a date (`20260528`) and uses `.com` extension on a PE — common social engineering trick on Windows (hides `.exe` nature). The photo-lure theme suggests spear-phishing delivery.

**Cert abuse**: The Sectigo EV cert (Xiamen Shunhuitong) provides a trusted SmartScreen bypass. Given the 1-year validity (2026-03-26 → 2027-03-26), revocation request to Sectigo is recommended.
