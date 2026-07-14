# Malware Analysis Report: WireVpn_v3.6.0.3-6872e76.exe

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | WireVpn_v3.6.0.3-6872e76.exe |
| SHA256 | `f5111a55125fcc4223805a676963852e9cf238287a51e4caad46791331127a24` |
| SHA1 | `7d16a1e77c487364021673e4eeb2552852f53039` |
| MD5 | `6872e762ad08e54a7158359ff92e61dd` |
| Size | 14,920,312 bytes (14.2 MB) |
| Type | PE32 (GUI), Intel 80386, Nullsoft Installer (NSIS) self-extracting archive |
| Signing cert | `WEILAI NETWORK TECHNOLOGY CO., LIMITED` (London, GB) — GlobalSign GCC R45 EV CodeSigning CA 2020, serial `03a9188aa510c0f8343426bf`, valid 2023-04-26 → 2026-04-26 |
| Product info | FileDescription/ProductName: "WireVpn Install", FileVersion/ProductVersion: 3.6.0.3, Copyright (C) 2011 |

The outer file is a stock NSIS installer stub (small `.text`/`.rdata`/`.data`, high-entropy 14.8 MB overlay holding the compressed NSIS archive). All payload binaries extracted from the archive are signed with the **same certificate and serial number** as the outer installer.

**Note on prior cases:** the certificate subject `WEILAI NETWORK TECHNOLOGY CO., LIMITED` also appears on a previously-analyzed sample (`DecManSrv.dll`, 2026-07-14), but that sample used a **different serial number** (`4618a59a39416181b7d7c6c9` vs. this sample's `03a9188aa510c0f8343426bf`). Per policy, same-issuer/different-serial is not sufficient grounds for cross-reference — this sample is analyzed independently.

## 2. Classification

**Confidence: Low — Unconfirmed / Likely PUP-adjacent Chinese VPN client with anomalous decoy-domain infrastructure.** Not confirmed malware.

Reasoning:
- KesaKode (both offline and online, run against the installer and every extracted payload binary) returned scores of **0–15%** against all suggested families (MoriAgent, Zenar, WinBeast, TFlower, GoldBackdoor, AridGopher, etc.) — all well below the 20% attribution threshold. **Discarded entirely per policy; no valid family attribution exists for this sample.**
- The installer unpacks into a coherent, professionally-structured commercial VPN product: a GUI client, a Windows-service tunnel engine, a Go-based proxy engine, matched TDI/WFP kernel filter drivers for Win7/Win10 x86/x64, an updater, and a crash reporter — internal PDB paths (`D:\job\jumpservice\...`, `D:\youqu_driver\savitar\savitar-proxy\...`, `D:\youqu_job\SuperBrowser\wirevpnLauncher\...`) show a real, versioned internal codebase rather than a hastily-assembled dropper.
- capa on the core tunnel binary (`wire.exe`) flagged only benign/expected capabilities (runs as service, writes files, sets environment variables, geolocation lookup) — no injection, credential access, or persistence-evasion techniques.
- No evidence of credential theft, keylogging, screen capture exfiltration, or C2 beaconing was found in static review of the extracted binaries.
- **However**, the core tunnel binary (`wire.exe`) hardcodes a long list of unrelated-looking "cover" domains alongside the vendor's real API endpoints (see IOCs) — a pattern consistent with either legitimate CDN/domain-fronting for GFW circumvention, **or** reuse of ad-fraud/malvertising redirect infrastructure. This could not be resolved with certainty from static analysis alone, and ANY.RUN sandbox detonation failed to execute the installer (see Section 6), so the domains' actual purpose is unconfirmed.
- A class named `holaService` (MSVC-mangled `.?AVholaService@@`) drives the Windows service lifecycle in `wire.exe`, and `upWire.exe` contains the strings `hola` / `StartHola`. This is suggestive but **not conclusive** of code lineage shared with "Hola"-style peer-to-peer VPN/bandwidth-reselling services; no actual relay/exit-node code exposing the host as a proxy for third parties was found in the reviewed decompilation, so this is noted as an anomaly only, not an attributed capability.

## 3. Capabilities (of the installed product)

- Installs a full VPN client under `%INSTDIR%` (`Wirevpn.exe` GUI, `wire.exe` tunnel service, `wireEngine.dll`/`proxyg.dll`/`Channel.dll` engine components)
- Installs matched kernel-mode packet-filter drivers (`wvpack.sys`) for both **TDI** (legacy, Windows 7) and **WFP** (Windows Filtering Platform, Windows 10) architectures, in both x86 and amd64 builds — standard VPN driver architecture
- Registers/starts Windows services via `CreateServiceW`/`OpenSCManagerW`/`StartServiceW` (present in `wireEngine.dll` and `proxyg.dll`)
- Runs a custom installer UI via a private NSIS plugin (`setupdll.dll`) driving a Chinese-language wizard through custom window messages (`WM_SJONECUSTOMINSTALL`, `WM_SJONECHECKSTATUS`, etc.)
- Writes standard `Uninstall` registry keys (`HKLM\...\Uninstall\WireVpnPc`) and Start Menu shortcuts
- Ships an auto-updater (`upWire.exe`, internal name `up7zupdate.exe`, uses 7z-based update packages) and a crash reporter (`crashreport.exe`)
- `wire.exe` (the tunnel service) fetches its configuration from `/client_v1/config/http` against a rotating pool of endpoints, including the vendor's own domains (`wirevpn.app/.cc/.io`) and a long list of unrelated "cover" domains (see IOCs) — consistent with a multi-CDN/domain-fronting evasion technique
- Elevates privileges (`AdjustTokenPrivileges`/`SeShutdownPrivilege`) during install/uninstall (`ElevatePrivileges` YARA hit on the outer NSIS stub — standard for driver installation)

## 4. Attack Chain

Not applicable in the traditional sense — no dropper/loader/payload staging chain was identified. This is a self-contained software installer:

1. NSIS stub self-extracts payload from its overlay to `%TEMP%`
2. Custom `setupdll.dll` plugin drives the install wizard (directory selection, license, progress)
3. Files copied to `$INSTDIR` (`WireVpnPc`), kernel drivers copied under `driver\{tdi,wfp}\{windows7,windows10}\{i386,amd64}\wvpack.sys`
4. Uninstaller and Start Menu shortcuts registered
5. `WireVpn.exe` launched post-install (`Exec "$INSTDIR\WireVpn.exe"`)

## 5. IOCs

### Filesystem (post-install artifacts)
- `%INSTDIR%\Wirevpn.exe`, `wire.exe`, `wireEngine.dll`, `proxyg.dll`, `Channel.dll`, `driver.dll`, `upWire.exe`, `crashreport.exe`, `wrvPlay.exe`, `wrvSavi.exe`, `uninstall.exe`, `license.txt`
- `%INSTDIR%\driver\tdi\{windows7,windows10}\{i386,amd64}\wvpack.sys`
- `%INSTDIR%\driver\wfp\{windows7,windows10}\{i386,amd64}\wvpack.sys`
- `%INSTDIR%\Wire Vpn.url` (InternetShortcut → `www.wirevpn[.]app`)

### Registry
- `HKLM\Software\Microsoft\Windows\CurrentVersion\Uninstall\WireVpnPc` (DisplayName, UninstallString, DisplayIcon, InstallLocation, Publisher, HelpLink, DisplayVersion, EstimatedSize)

### Network (defanged) — vendor + embedded "cover" domain pool from `wire.exe`
Vendor-branded:
- `https://api[.]wirevpn[.]app`, `apiv2[.]wirevpn[.]app`, `apiv3[.]wirevpn[.]app`, `apiv4[.]wirevpn[.]app`
- `https://api[.]wirevpn[.]cc`
- `https://api[.]wirevpn[.]io`
- `www[.]wirevpn[.]app` (product URL shortcut)

Unrelated "cover"/candidate front domains (purpose unconfirmed — see Section 2):
- `https://abc[.]breakoursilence[.]com`
- `https://aff[.]9breakingnews[.]com`
- `https://api[.]betflixfree[.]net`
- `https://api[.]isharkvpn[.]com`
- `https://app[.]businesssy[.]com`
- `https://app[.]telegram-install[.]com`
- `https://bin[.]visitbenin[.]org`
- `https://cate[.]norton-com-nu16[.]com`
- `https://check[.]5topvpn[.]net`
- `https://dn[.]equalmarriagefl[.]com`
- `https://gg[.]afn360[.]com`
- `https://go[.]chatwithsky[.]com`
- `https://js[.]dapr0n[.]com`
- `https://la[.]downloadfreetheme[.]net`
- `https://prox[.]sarahsoriano[.]com`
- `https://ruu[.]rocketapp[.]cc`
- `https://soft[.]prosoftwarestore[.]com`
- `https://tk[.]speedyshare[.]com`
- `https://v2[.]appsdownloadfull[.]com`
- `https://version[.]spincityclub-official[.]com`

### Certificate
- Subject: `WEILAI NETWORK TECHNOLOGY CO., LIMITED`
- Issuer: GlobalSign GCC R45 EV CodeSigning CA 2020
- Serial: `03a9188aa510c0f8343426bf`
- Validity: 2023-04-26 → 2026-04-26
- Used to sign every payload binary in this installer (installer, GUI, service, engine DLLs, updater, crash reporter, replay/capture helpers)

### Hashes of extracted payload binaries
| File | SHA256 |
|---|---|
| Wirevpn.exe | (8.6MB, main GUI) |
| wire.exe | `6729c71328ce8498c26e6a46b7ba2fe84c611814d92703bdc2c1ebb7339d43fd` |
| proxyg.dll | `b71fdb1e1cee470b75bd34006221bab84a00132e02b78a50ba66e089dea73488` |
| wireEngine.dll | `2146ccaca2966cb55ceaed8e3d0fa57fc8b7dccc236d8f09a34b4c12710d4792` |
| Channel.dll | `a64569669898736981b411dae920ee298e080ca625ab4ca18a6e738a967c343b` |
| upWire.exe | `90b89a6dc4565c9817e7db8323702006860cb2b49f352863f2b18ba21c66b435` |
| crashreport.exe | `ef23426b1392cbb684eac0a501c303debee4be6fbe65c4ee205d65b46b2e6697` |
| wrvPlay.exe | `921af32905c67b48d5aa9195e177963970b27bef2a91c69050a89e1f3e96b035` |
| wrvSavi.exe | `f31ccafef91c97d94726e79571c06def0cc5e5db7c96fec86f3708fe5dc7a6d3` |

## 6. Emulation Results

- **Speakeasy** on `wire.exe` (x86): emulation only progressed through CRT/API-resolution stubs (67 `GetProcAddress`/`LoadLibraryExW` entries) before timing out at 120s. No network, registry, mutex, or file-write events were captured — `wire.exe`'s real logic runs from `ServiceMain`, which the generic entry-point emulator does not reach without SCM-specific hooking. Treat as inconclusive, not as evidence of inactivity.
- No custom hook script was written for this sample; the existing `speakeasy_lib/hooks.py` library was not extended, since no new decrypt/obfuscation routine was found that required it.

## 7. Sandbox Results (ANY.RUN)

**Submission did not execute the installer.** Task `190a9385-b79f-4b81-a126-2afd7db57d69` shows the sample was renamed by the sandbox harness to `WireVpn_v3.6.0.3-6872e76.exe.mexw32` and launched via `explorer.exe`; because `.mexw32` has no file association, Windows opened `OpenWith.exe` and the installer was never actually run. Consequently:
- Verdict: score 0, "No threats detected" — **this is an artifact of non-detonation, not a genuine clean verdict.**
- No process tree, network requests, or dropped files attributable to the installer's actual behavior were captured (the only observed network traffic was routine Windows/Office telemetry and CRL/OCSP checks unrelated to the sample).
- Tags: none assigned.
- Public report: https://app.any.run/tasks/190a9385-b79f-4b81-a126-2afd7db57d69

## 8. Analyst Notes

- This installer decompresses into a substantial, internally consistent commercial VPN software suite (GUI, service, Go proxy engine, matched TDI/WFP drivers for two OS generations and both architectures, updater, crash reporter) signed end-to-end with a single valid EV certificate. This is a materially different profile from typical droppers or loaders analyzed in this workspace.
- The two open questions that keep this from a clean "benign" verdict:
  1. **The embedded pool of ~20 unrelated "cover" domains** in `wire.exe`'s config-fetch logic. These could be legitimate CDN edge/domain-fronting infrastructure (common for VPN tools serving GFW-restricted users), or repurposed ad-fraud/redirect domains. Recommend passive-DNS / hosting-infrastructure pivot on a sample of these domains (e.g., `norton-com-nu16[.]com`, `telegram-install[.]com`) to determine if they resolve to the same IP space as `wirevpn.app`'s real API, which would support the domain-fronting explanation.
  2. **The `holaService`/`StartHola` naming** in the service-lifecycle code. Inconclusive on its own; would need decompilation of `proxyg.dll`'s Go-compiled networking/relay logic (not performed due to time — Go binaries produce noisy capa/KesaKode output and the binary is large) to confirm or rule out any peer-relay/exit-node functionality.
- ANY.RUN detonation failure means there is **no genuine dynamic verdict** for this sample; if follow-up is warranted, resubmit with the installer's actual `.exe` extension preserved (or run manually in an isolated VM) to get real behavioral telemetry.
- Recommended follow-up if this vendor/product recurs: pivot on the exact PDB paths (`jumpservice`, `savitar-proxy`, `SuperBrowser\wirevpnLauncher`) and the cert serial `03a9188aa510c0f8343426bf` to link future samples.
