# notification_helper.exe — Analysis Report

**Date:** 2026-08-06
**Analyst:** Claude (automated REMnux/malcat workflow)

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | `notification_helper.exe` (extracted from `helper/notification_helper/`, sourced from `notification_helper.zip`) |
| SHA256 | `2edb7c1f064cbfc7883f6a4be65134c8a27fba74413d9ed800b4e3ad192e8426` |
| SHA1 | `45f4be7c38ea573ede82edf5875bd9bd64f4a113` |
| MD5 | `65058836b98fbe291febfbf645252fbf` |
| Imphash | `a015389f60165399e844c3b119b7e185` |
| Type | PE32 executable (GUI), Intel 80386, 5 sections |
| Size | 376,408 bytes |
| Build timestamp | 2017-08-24 02:56:21 (inherited from the original SDK build — not indicative of when this sample was produced) |
| PDB path | `D:\rdm\projects\28820\pdb\Release\minibrowser_exe.pdb` |
| VersionInfo (impersonated) | CompanyName="The NW.js Community", ProductName="nwjs", FileDescription="nwjs", OriginalFilename="notification_helper.exe" |

**Signing:** Certum Extended Validation Code Signing 2021 CA → **Chengdu Yongyingli Technology Co., Ltd.**, serial `222a7ef10b49297ba094cc38e127fc25`, valid 2026-07-14 to 2027-07-14.

**Note the mismatch:** VersionInfo claims this is an NW.js "notification_helper" component, but the embedded PDB path, registry paths, and internal strings are all genuine Tencent TBS/QQ **MiniBrowser SDK** — the version resource was edited to impersonate an unrelated legitimate application while the actual code is a re-signed, re-purposed browser-SDK component.

## 2. Classification

**Confirmed APT-Q-27 / ZhongStealer ("nikeupdat" wave) delivery component — CONFIRMED via byte-identical payload hash.**

- `notification_helper.exe` itself is a genuine, functionally-unmodified-in-most-respects Tencent MiniBrowser SDK build (online KesaKode: FatDuke 2.18%, below the 20% noise threshold — discarded), **except for one added function pair** (`sub_41f420`/`sub_41f2a0`) that reads, decrypts, and directly executes a co-located file, `erigfj.gtk`, as shellcode. This is the weaponization point.
- Two of the three co-located opaque payload files (`oihtq.uqv`, `vcnfq.uqv`) decrypt cleanly with the **exact same keys** documented for the 2026-07-21 `SAC_tool.exe`/nikeupdat APT-Q-27 wave (see prior memory `project_sactool_nikeupdat_downloader`). `oihtq.uqv` decrypts to a **byte-identical `plugin32.dll`** (SHA256 `813c4a2aab04fbb1f56ae5c0b4cb73188de57ffb3dc58b6019f631560de4ad33`) — an exact payload-hash match to the previously confirmed sample, meeting the strict same-payload-hash cross-reference bar for attribution.
- `vcnfq.uqv` decrypts with the same 16-byte key to a **different** loader/core DLL (SHA256 `71685bdb5f5e67df6b1690eb21c63c75c7f910d8e25fac881532ee96e0461a6d`, vs. the prior wave's `90b6c23a...`) — an updated variant of that module.
- This confirms the sample is a fresh instance of the same actor's delivery chain, ~2.5 weeks after the last documented wave, now delivered under a "notification_helper" NW.js disguise instead of a bare `minibrowser.exe` name, and with a genuinely modified loader (previous wave's `minibrowser.exe` was byte-for-byte unmodified; this one has an added shellcode-loader stub).

## 3. Capabilities

**Inherited (genuine Tencent MiniBrowser SDK, benign):**
- Browser shell UI, PPAPI/Flash support, search-engine provider configuration (Baidu/Sogou/Bing/Google), Tencent update-checking (`Software\Tencent\MiniBrowser\Cfg`, `Tencent\DeskUpdate\GlobalMgr.db`)

**Added/malicious (present in this build, absent from the previously-documented unmodified `minibrowser.exe`):**
- Resolves its own directory (`GetModuleFileNameW` + `PathRemoveFileSpecW`), builds a path to a co-located file named **`erigfj.gtk`** (`PathCombineW`) — this exact filename is hardcoded inside the binary
- Opens and reads `erigfj.gtk` (`CreateFileW`/`ReadFile`, capped at 1MB) into a `VirtualAlloc`'d RWX buffer
- Decrypts the buffer in place, byte-by-byte: `out[i] = (in[i] + 0x70) ^ 0x88`
- **Directly calls the decrypted buffer as code** via an indirect function pointer call — classic shellcode execution
- The decrypted shellcode (recovered, see IOCs) opens with a `jmp` stub and contains a **hash-based `GetProcAddress`/export-table-walking routine** (rolling hash: `hash = hash*0x83 + char`), the standard building block of reflective PE-loader shellcode (Metasploit/Cobalt-Strike-style tradecraft) — used to manually map an embedded PE into memory without `LoadLibrary`/disk writes
- capa flags consistent with this: Hidden Window (`T1564.003`), Obfuscated Files or Information (`T1027`), Shared Modules (`T1129`), Registry Run Key persistence (`T1547.001`), shellcode execution via indirect call
- Standard anti-debug probes present (`IsDebuggerPresent`, `GetWindowThreadProcessId`, `UnhandledExceptionFilter`) — inherited from the SDK's own CRT/crash-handling code, not clearly malicious-specific

## 4. Attack Chain

```
setup1.exe (InstallShield launcher, fake "Quectel_Windows_USB_Driver(Q)_NDIS" branding)
    │  DigiCert cert "Monarch Innovation Private Limited", serial 01bdc9c2c1a1922c5a71c2575cf61317
    ▼
drops into a shared directory:
  ├── Updater.exe          (PE32+ x64, PDB "C:\Telegram\tx64\out\Release\Updater.pdb",
  │                          SAME cert serial as setup1.exe — same actor signed both)
  ├── notification_helper.exe   (weaponized Tencent MiniBrowser, THIS ANALYSIS TARGET)
  ├── erigfj.gtk            (372,422B — read/decrypted/executed directly by notification_helper.exe)
  ├── oihtq.uqv             (350,208B — XOR w/ unchanged MODULE_KEY → byte-identical plugin32.dll)
  └── vcnfq.uqv             (340,992B — XOR w/ known VCNFQ_KEY → updated loader/core DLL)
```

`notification_helper.exe`, on execution, reads and shellcode-executes `erigfj.gtk` from its own directory. The exact mechanism by which `oihtq.uqv`/`vcnfq.uqv` get consumed (by `notification_helper.exe`, `Updater.exe`, or the shellcode loaded from `erigfj.gtk`) was **not fully reversed in this pass** — same open item noted in the prior nikeupdat-wave analysis. A full standalone reverse of `setup1.exe` and `Updater.exe` was out of scope for this single-file analysis request and is recommended as follow-up.

## 5. IOCs

**Filesystem (delivery chain, this sample's directory):**
- `notification_helper.exe` — SHA256 `2edb7c1f064cbfc7883f6a4be65134c8a27fba74413d9ed800b4e3ad192e8426`
- `erigfj.gtk` — SHA256 `f37f58406b51e640a355cca6fa334a80806ddbc459c82ff3901919e1f43e01fc` (encrypted shellcode blob)
- `oihtq.uqv` — SHA256 `b30886bf461f3d27c7d83bf1678c1fc4fe9ca1b709caf26b624d14f7f8b2ec61` → decrypts to `plugin32.dll` SHA256 `813c4a2aab04fbb1f56ae5c0b4cb73188de57ffb3dc58b6019f631560de4ad33` (byte-identical to prior confirmed nikeupdat-wave core plugin)
- `vcnfq.uqv` — SHA256 `cf1c74ed2fdced3c3987f237e727461e6c1d929183bea3d496e8702244d7cfd0` → decrypts to loader/core DLL SHA256 `71685bdb5f5e67df6b1690eb21c63c75c7f910d8e25fac881532ee96e0461a6d` (new variant)
- `Updater.exe` — SHA256 `c09884b9921be1acefb556745f9f22ab8474ce24ce306f90319fcb3408a1c0c5`
- `setup1.exe` — SHA256 `ba3d3301734d31d36ff25bbe2ada1180fc4357c03b9cba21d3e18445b76b6f7e`

**Decryption keys recovered/reused:**
- `erigfj.gtk` cipher (new): `out[i] = (in[i] + 0x70) ^ 0x88` — single-byte ADD+XOR, structurally similar to but numerically distinct from the documented APT-Q-27 `(byte+0x77)^0x62` fingerprint
- `oihtq.uqv` MODULE_KEY (unchanged from prior wave): `2031a71c399563adaf1572e10abb395387eb132208a001c5e140496d7a3e0b26`
- `vcnfq.uqv` key (unchanged from prior wave): `33c83bcf7507b94fe640bbeb1085ce75`

**Certificates:**
- `notification_helper.exe`: Certum EV, subject "Chengdu Yongyingli Technology Co., Ltd.", serial `222a7ef10b49297ba094cc38e127fc25`
- `Updater.exe` / `setup1.exe`: DigiCert, subject "Monarch Innovation Private Limited", serial `01bdc9c2c1a1922c5a71c2575cf61317` (identical serial on both — confirms same actor signed both components)

**Network:** None recovered. All domain strings inside `notification_helper.exe` (`www.google.com.hk`, `www.baidu.com`, `www.sogou.com`, `cn.bing.com`, `daohang.qq.com`, `www.qq.com`) are the genuine MiniBrowser SDK's own search-engine provider defaults, not attacker infrastructure. ANY.RUN's standalone run produced only benign Windows OS telemetry traffic (`login.live.com`, `settings-win.data.microsoft.com`).

**Registry / Mutex:** None observed beyond the SDK's own benign `Software\Tencent\MiniBrowser\Cfg` configuration keys.

## 6. Emulation Results

speakeasy (x86, 120s timeout) only reached CRT/import-resolution (37 `GetProcAddress` calls) and did not traverse into the GUI-init path that eventually calls `sub_41f420` — no dynamic network/file/registry/mutex events were captured. The loader mechanism and decryption key were instead recovered via static decompilation (malcat `fn_decompile`) and manual reimplementation of the recovered cipher (see Capabilities/IOCs).

`erigfj.gtk`, once decrypted, was found to contain x86 shellcode with a hash-based reflective-PE-loader stub. An embedded/staged PE payload is present but its DOS/PE headers are further corrupted or packed beyond a simple `e_lfanew` fix-up — full unpacking was not completed in this pass (see Analyst Notes).

## 7. Sandbox Results

**ANY.RUN task:** `c1a51d1d-8c04-4592-aaa9-3841696904fb`
**Verdict:** 0/100, "No threats detected", no family/behavior tags
**Public URL:** https://app.any.run/tasks/c1a51d1d-8c04-4592-aaa9-3841696904fb

HTTP IOC report contains only standard Windows OS telemetry (`login.live.com`, `settings-win.data.microsoft.com`) — no malicious traffic.

**This clean verdict should not be trusted in isolation.** It matches the exact pattern previously documented for this actor's `minibrowser.exe` component: a standalone submission lacks the co-located companion files (`erigfj.gtk`, `oihtq.uqv`, `vcnfq.uqv`) required to trigger the loader path, so the sandbox never observes the actual malicious behavior. The static evidence (hardcoded companion filename, decrypt+execute routine, byte-identical payload hash to a confirmed prior sample) is the authoritative signal here, not the sandbox score.

## 8. Analyst Notes

- **Residual gap:** the embedded PE inside the decrypted `erigfj.gtk` shellcode was not fully unpacked. A DOS/PE header candidate was located but its `e_lfanew`/header fields do not resolve to a clean parse even after offset correction — likely an additional packing/encoding layer, or intentional header corruption relied upon by the custom loader rather than the OS loader. Recommended follow-up: trace the reflective-loader's hash-based `GetProcAddress` routine with angr/unicorn to recover the exact offset/format it expects.
- **Toolkit evolution:** the `erigfj.gtk` cipher `(byte+0x70)^0x88` is a new construction, distinct from both the legacy `(byte+0x77)^0x62` fingerprint and the nikeupdat-wave's plain-XOR construction for `oihtq.uqv`/`vcnfq.uqv` — this actor appears to vary the at-rest cipher per delivery wave/module while reusing core keys and payloads (`plugin32.dll` unchanged) across waves.
- **Not analyzed in this pass:** `setup1.exe` (InstallShield dropper) and `Updater.exe` (possible orchestrator analogous to the prior wave's `SAC_tool.exe`) were only fingerprinted (hash, cert, PDB path), not fully reversed. Their network behavior, persistence mechanism, and exact relationship to the `.uqv`/`.gtk` payloads remain open.
- **Alternative hypothesis considered and rejected:** the added loader function could theoretically be an unrelated legitimate update-check mechanism reading a benign resource — rejected because the decrypted `erigfj.gtk` contents are unambiguous x86 shellcode with a reflective-loader stub, not configuration data, and because `oihtq.uqv` independently decrypts to a byte-identical copy of a previously confirmed malicious plugin DLL.
