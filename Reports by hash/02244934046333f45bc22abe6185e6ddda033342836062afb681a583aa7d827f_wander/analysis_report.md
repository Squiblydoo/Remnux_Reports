# Malware Analysis Report: wander.exe

**Date:** 2026-06-02  
**Analyst:** REMnux automated analysis  
**Sample:** `wander.exe`

---

## 1. File Metadata

| Field | Value |
|---|---|
| **Filename** | wander.exe |
| **SHA256** | `02244934046333f45bc22abe6185e6ddda033342836062afb681a583aa7d827f` |
| **MD5** | `778b6521dd2b07d7db0eaeaab9a2f86b` |
| **SHA1** | `ce120e922ed4156dbd07de8335c5a632974ec527` |
| **File type** | PE32 .NET 4.8 assembly (GUI), 32-bit |
| **Size** | 25,864 bytes |
| **Entropy** | 81 (high; packed) |
| **PE sections** | `.text` (RX, entropy 82), `.rsrc`, `.reloc` |
| **Overlay** | 5,896 bytes at offset 0xA200 (PKCS7 digital signature) |

**Digital Signature:**
| Field | Value |
|---|---|
| Issuer | GlobalSign GCC R45 EV CodeSigning CA 2020 |
| Subject | Morning Leap & Cazo Electronics Technology Co., Ltd. |
| Org | State=Hebei / Locality=Cangzhou / Country=CN |
| Validity | 2024-05-16 → 2025-05-16 (**EXPIRED**) |
| Serial | `2686b9982e46da7e3e0a1d56` |
| Hash algorithm | SHA1 |

**VersionInfo (spoofed):**
| Field | Value |
|---|---|
| FileDescription | Microsoft |
| FileVersion | 128.1.2479.97 (mimics Chromium/Edge versioning) |
| LegalCopyright | "Copyright Microsoft Corporation. All rights **reserced**." (typo) |
| InternalName | wander.exe |
| Assembly Version | 1.0.0.0 |

**Other artifacts:**
- PE timestamp: `2057-01-19 14:33:36` (forged future date)
- .NET module: `wander.exe`
- Imphash: `f34d5f2d4577ed6d9ceec516c1f5a744`
- Chinese business reg embedded in cert: `91130922MA0G8AN9201`
- Obfuscator marker: `Confuser.Core 1.6.0+447341964f`

---

## 2. Classification

**Family:** Staged Downloader / Dropper  
**Confidence:** High (full decompilation via ilspycmd confirms attack chain)  
**Obfuscation:** ConfuserEx 1.6.0

**KesaKode (offline):** No match  
**KesaKode (online):** No match (no known family attribution)

The sample is a ConfuserEx-obfuscated .NET 4.8 WinForms application acting as a first-stage downloader. It retrieves a resource list from an Alibaba Cloud OSS staging bucket (Hong Kong region), downloads and GZip-decompresses payloads, then executes the first `.exe` in the list. All execution is gated on the filename containing `_20241220`, which simultaneously serves as a campaign tag and a sandbox evasion mechanism.

---

## 3. Capabilities

- **Filename gate (sandbox evasion):** Only activates if `Process.GetCurrentProcess().MainModule.FileName` contains `_20241220` — the sample exits silently if renamed
- **Anti-profiler:** Checks `COR_ENABLE_PROFILING` environment variable via reflection; calls `Environment.FailFast(null)` if set (blocks .NET profiling tools and analysis)
- **Anti-debug thread:** Spawns background watchdog thread loop; calls `FailFast` if `Debugger.IsAttached || Debugger.IsLogging()` at any point
- **HTTP download:** Uses `System.Net.Http.HttpClient` to download resource list and individual payloads
- **GZip decompression:** Decompresses each downloaded file using `GZipStream` before writing to disk
- **Base64 decode:** Decodes embedded C2 URL from base64 at runtime to evade static string detection
- **Staged execution:** Reads URL list from staging server, downloads each item, executes the `.exe`
- **Directory creation:** Creates `%USERPROFILE%\<random-GUID>@27\` as the drop directory
- **Process launch:** `Process.Start()` on the downloaded and decompressed executable (3-second delay before exec)
- **VersionInfo masquerade:** Spoofed Microsoft branding with a fake Chromium/Edge version number
- **PE timestamp forgery:** Timestamp set to year 2057
- **EV code signing (expired):** Signed with a GlobalSign EV cert from a Chinese entity; certificate expired 2025-05-16

---

## 4. Attack Chain

```
wander_20241220.exe (renamed by operator with campaign tag)
  │
  ├── [Anti-debug / Anti-profiler] COR_ENABLE_PROFILING check + watchdog thread
  │
  ├── [Filename gate] Must contain "_20241220" → else exit silently
  │
  ├── [Decode C2 URL] base64("aHR0cHM6Ly...") → https://kkuu.oss-cn-hongkong.aliyuncs.com/ss/uu.txt
  │
  ├── [HTTP GET] Download uu.txt (line-delimited URL list)
  │
  ├── [For each URL in uu.txt]
  │     ├── [HTTP GET] Download payload to %USERPROFILE%\<GUID>@27\<filename>
  │     ├── [GZip decompress] Decompress in-place
  │     └── [Track .exe] If extension == .exe, mark as execution target
  │
  └── [Sleep 3s] → Process.Start(<exe_path>)  → Stage 2 execution
```

---

## 5. IOCs

### Network
| Type | Indicator | Notes |
|---|---|---|
| URL | `hxxps://kkuu[.]oss-cn-hongkong[.]aliyuncs[.]com/ss/uu[.]txt` | Resource list (stage-1 C2; bucket active, key deleted at analysis time) |
| Domain | `kkuu[.]oss-cn-hongkong[.]aliyuncs[.]com` | Alibaba Cloud OSS, Hong Kong region |

*Additional payload URLs would be delivered via `uu.txt`; list was removed from staging server before analysis.*

### Filesystem
| Path | Notes |
|---|---|
| `%USERPROFILE%\<random-GUID>@27\` | Drop directory (GUID regenerated each run) |
| `%USERPROFILE%\<random-GUID>@27\<payload>.exe` | Dropped + decompressed second-stage PE |

### Registry
None identified.

### Mutexes
None identified.

### Execution gate
| Indicator | Value |
|---|---|
| Filename substring (campaign tag) | `_20241220` |

---

## 6. Emulation Results

**Speakeasy:** Not attempted — .NET runtime binaries are not supported by speakeasy's native PE emulator.

**FLOSS:** Not applicable — FLOSS does not support .NET language-specific string extraction.

**ilspycmd (decompilation):** Full .NET decompilation successful. All critical logic (anti-debug, filename gate, C2 URL, download/decompress/exec flow) recovered from decompiled C#. ConfuserEx obfuscation rendered all identifiers as non-ASCII Unicode control characters, but the control flow was intact.

**Manual URL decode:**
```
base64("aHR0cHM6Ly9ra3V1Lm9zcy1jbi1ob25na29uZy5hbGl5dW5jcy5jb20vc3MvdXUudHh0")
→ https://kkuu.oss-cn-hongkong.aliyuncs.com/ss/uu.txt
```

**Staging server probe:** Alibaba OSS bucket `kkuu` confirmed reachable; `ss/uu.txt` resource returns `NoSuchKey` (deleted or time-limited staging).

---

## 7. Sandbox Results

**ANY.RUN Task ID:** `cb21700f-64aa-41dd-b4be-bf350fcf0bf2`  
**Public URL:** https://app.any.run/tasks/cb21700f-64aa-41dd-b4be-bf350fcf0bf2  
**Score:** 15/100  
**Threat Level:** No threats detected  
**Tags:** (none)

**Explanation:** The filename gate (`_20241220`) triggered immediately upon execution — since the sandbox ran the sample as `wander.exe`, the check failed and the malware exited before performing any malicious activity. Network connections in the sandbox report are entirely Windows telemetry (OCSP, CRL, Microsoft settings). This is a deliberate sandbox evasion technique and does not indicate the sample is benign.

---

## 8. Analyst Notes

**Certificate actor profile:**
- Signing entity: "Morning Leap & Cazo Electronics Technology Co., Ltd." (晨跃卡佐 or similar), Cangzhou, Hebei, CN
- Chinese business registration: `91130922MA0G8AN9201` (embedded in cert — `1309` prefix = Cangzhou prefecture code)
- Certificate validity: 2024-05-16 to 2025-05-16 (expired ~7 months before this analysis)
- The name "Morning Leap & Cazo" does not appear in public Chinese business records as a recognized software company; likely a shell company established for code signing

**Campaign tag `_20241220`:**
- The string `_20241220` in the filename almost certainly encodes a build/campaign date: December 20, 2024
- Operators must rename the binary (e.g., `wander_20241220.exe`) before delivering it to victims; the file as-named here is inert

**`@27` directory suffix:**
- The drop directory uses `<GUID>@27` as a naming pattern; the `@27` literal suffix is hardcoded and could serve as a threat-hunting pivot in EDR telemetry

**Staging infrastructure pattern:**
- The Alibaba Cloud OSS Hong Kong region bucket pattern (`*.oss-cn-hongkong.aliyuncs.com`) has been observed in Chinese-origin malware campaigns. The specific bucket `kkuu` and path `ss/uu.txt` are unique to this sample. The staging infrastructure was active but already cleaned by time of analysis.

**Residual unknowns:**
- The contents of `uu.txt` are unknown — the second-stage payload(s) were not retrieved
- Whether the delivery mechanism involves phishing, supply-chain compromise, or watering hole is undetermined
- KesaKode (offline and online) returned no family match, suggesting this tooling is bespoke or not yet catalogued

**Recommended follow-up:**
- Pivot on cert serial `2686b9982e46da7e3e0a1d56` in VirusTotal / certificate transparency logs to identify other samples signed by same actor
- Search for `_20241220` filename pattern in threat intel platforms
- Hunt for `@27` directory creation events in EDR telemetry
- Monitor `kkuu.oss-cn-hongkong.aliyuncs.com` for re-activation of `ss/uu.txt`
