# Analysis Report: Screenshot_Tool.msix

**Date:** 2026-05-26  
**Analyst:** Claude Sonnet 4.6 (REMnux MCP)  
**Classification:** **MALICIOUS — Go Backconnect RAT v2 / Gogs-Embedded C2 / Lightshot Impersonation Trojan**  
**Confidence:** High

---

## 1. File Metadata

### MSIX Container
| Field | Value |
|-------|-------|
| Filename | `Screenshot_Tool.msix` |
| SHA256 | `ce668b048458502529ed71292bcfaa2bbc2b21020662a58d76f6c50111a9e538` |
| MD5 | `c83e8d089cf48aa1240274c030b3ea8f` |
| SHA1 | `c5ad97a7ec8c1dc32ebb030a7413a1c9b8f03df5` |
| Size | 138,810,333 bytes (138 MB) |
| Type | MSIX (ZIP) — Windows application package |
| Signing | **Unsigned** (Publisher: `CN=A107EA2C-DE6F-4E13-BF00-654F5AD5A3A2`, self-generated UUID identity) |

### Embedded client.dll (primary malicious payload)
| Field | Value |
|-------|-------|
| Filename | `client.dll` |
| SHA256 | `2c253d8131cf8a948115884467aeeba28f43a85a289b730b5e490fb59ad4c921` |
| Size | 68,127,944 bytes (65 MB, compressed to 23.7 MB in MSIX) |
| Type | PE32 DLL (x86), Go binary |
| Certificate | OC Agro ApS (Midtjylland, DK) — Sectigo Public Code Signing CA EV E36 |
| Cert Serial | `281cca56f214f9e84b03992ba076e318` |
| Cert Validity | 2026-04-17 → 2027-04-17 |
| Exports | `Start` (only export) |
| Source path | `server/src/backconnect` |

### Embedded app.asar (Electron JS payload)
| Field | Value |
|-------|-------|
| SHA256 | `94c76806e8fa829c688beb95c446e50f91f5c2ef00bc685f99e803dad0949e9e` |
| Size | 30,876,636 bytes (uncompressed) |
| Type | Electron ASAR archive |

---

## 2. Classification

**Primary classification:** Go Backconnect RAT v2 — updated C2 infrastructure from the same source codebase (`server/src/backconnect*`) as a previously confirmed implant. The RAT component is co-compiled into a single 65 MB DLL alongside a full **Gogs Git server** (`gogs.io/gogs`), which provides the SQLite database, web templates, and possible attacker-facing management interface.

**Attack type:** MSIX Trojan — legitimate screenshot application (fully functional lure) used to silently deploy a backconnect proxy/RAT on first launch and on every subsequent login via an auto-start task.

**KesaKode (online):** Bruteentry 26.45% (code sharing overlap, not confirmed attribution); GolangGhost 12.67% (discard <20%).

---

## 3. Capabilities

### Execution & Persistence
- **Auto-start on login** via MSIX `uap5:StartupTask` (`Enabled="true"`, TaskId=`ScreenshotToolAutoRun`)
- **Desktop + Start Menu shortcuts** created at install (`ScreenshotTool.lnk`)
- **`runFullTrust` capability** — full system access without elevation prompt
- **`internetClient` capability** — unrestricted outbound network access
- RAT thread starts **before any UI is shown** (`startClientDll()` is the first call in `app.whenReady()`)

### RAT / Backconnect
- **Registers to C2** via HTTPS POST to `api1/api2.storeappsupdatesapi[.]xyz/register` with dual failover
- **Beacon format:** `USERID:%s\nBUILDVERSION:%s\nHWID:%s\n\n`
  - USERID = `-user_id=microsoft_store_ScreenshotTool_9NTC2KMZMGND` (campaign tag embedded in Electron JS)
  - HWID = Windows `MachineGuid` via `github.com/denisbrodbeck/machineid` (`HKLM\SOFTWARE\Microsoft\Cryptography\MachineGuid`)
  - BUILDVERSION = embedded version string from `server/src/backconnect.buildVersion`
- **Session multiplexing:** `github.com/hashicorp/yamux` over TLS
- **SOCKS5 proxy** support (full implementation)
- **TCP + UDP relay** (`[UDP] setup header read failed: %v`, `Failed to read TCP...destination`)
- **Relay loop:** `connectLoopWithRelays` function — operator-delivered relay IPs via `/ping` response
- **Ping/health check:** `/ping` endpoint (confirmed from "Direct relay..er/ping workflow" string)
- **Hardware fingerprinting:** hardware enumeration APIs
- **Process enumeration:** running process listing
- **Keylogger APIs** (YARA: `KeyloggerApi`)
- **TOR support** (YARA: `TorUsage`)
- **HTTP POST form** (data exfiltration path)
- **Import by hash** (4 hits — API call obfuscation)
- 582 XOR-in-loop instances across code
- 156 Base64 string blobs

### Embedded Gogs Git Server
- Full `gogs.io/gogs` source compiled into the DLL (accounts for most of the 65 MB size)
- Embedded SQLite via `modernc.org/sqlite` (pure Go, no CGO dependency)
- Gogs web templates, SQL schema, backup system (`gogs-backup-%s.zip` format)
- Likely purpose: operator-accessible local Git repository for data staging, or the Gogs web UI serves as a management console reachable via the yamux tunnel

### Lure (Functional Decoy)
- Fully operational screenshot tool (hotkey `Ctrl+Shift+S` / PrintScreen)
- Region capture, preview window, pin-on-top, history (last 20 screenshots)
- Upload to Imgur (`api.imgur.com`, Client-ID `546c25a59c58ad7`)
- Microsoft Store review prompt (stores lock file in `%TEMP%\screenshot-tool-review-done`)

---

## 4. Attack Chain

```
User installs Screenshot_Tool.msix (sideloaded — NOT from Microsoft Store)
         │
         ▼
MSIX registers auto-start task (fires on every login, -start flag)
         │
         ▼
app\Screenshot Tool.exe (Electron) launches
         │
         ├─► app.whenReady() fires:
         │       startClientDll(process.execPath)
         │           → ffi-rs opens app\client.dll
         │           → calls Start(0, 0, "-user_id=microsoft_store_ScreenshotTool_9NTC2KMZMGND", 0)
         │           → Go runtime initializes in new thread
         │           └─► Gogs Git server starts (local SQLite DB)
         │               backconnect RAT registers to C2:
         │               HTTPS POST api1.storeappsupdatesapi.xyz/register
         │               Body: USERID:-user_id=...\nBUILDVERSION:...\nHWID:<MachineGuid>
         │               ← Server returns relay IPs via /ping
         │               ← yamux session established (implant = yamux server)
         │               ← Operator can now proxy TCP/UDP through victim, browse Gogs UI
         │
         └─► (if not -start flag) UI loads:
                 Tray icon + screenshot hotkeys registered
                 User sees legitimate screenshot functionality
```

---

## 5. IOCs

### Network (defanged)

| Type | Indicator |
|------|-----------|
| Domain | `api1[.]storeappsupdatesapi[.]xyz` |
| Domain | `api2[.]storeappsupdatesapi[.]xyz` |
| URL | `https[://]api1[.]storeappsupdatesapi[.]xyz/register` |
| URL | `https[://]api2[.]storeappsupdatesapi[.]xyz/register` |
| URL | `https[://]api1[.]storeappsupdatesapi[.]xyz/ping` |
| URL | `https[://]api2[.]storeappsupdatesapi[.]xyz/ping` |

### Filesystem

| Path | Description |
|------|-------------|
| `%LOCALAPPDATA%\Packages\HobbyApps.Lightshot_*\` | MSIX install directory |
| `app\client.dll` (relative to EXE) | Backconnect RAT + Gogs DLL |
| `%TEMP%\screenshot-tool-review-done` | Review lock file (persistence marker) |
| `%TEMP%\ScreenshotTool.lnk` (desktop) | Auto-created shortcut |

### Registry

| Key | Purpose |
|-----|---------|
| `HKLM\SOFTWARE\Microsoft\Cryptography\MachineGuid` | HWID read for beacon |

### Hashes

| File | SHA256 |
|------|--------|
| `Screenshot_Tool.msix` | `ce668b048458502529ed71292bcfaa2bbc2b21020662a58d76f6c50111a9e538` |
| `client.dll` | `2c253d8131cf8a948115884467aeeba28f43a85a289b730b5e490fb59ad4c921` |
| `app.asar` | `94c76806e8fa829c688beb95c446e50f91f5c2ef00bc685f99e803dad0949e9e` |

### Certificates (abused)
- **Subject:** OC Agro ApS (Midtjylland, Denmark)
- **Issuer:** Sectigo Public Code Signing CA EV E36
- **Serial:** `281cca56f214f9e84b03992ba076e318`
- **Validity:** 2026-04-17 → 2027-04-17
- **Assessment:** Legitimate Danish agricultural company cert — stolen/compromised for code signing abuse

### Campaign Identifiers
- **USER_ID / Campaign tag:** `-user_id=microsoft_store_ScreenshotTool_9NTC2KMZMGND`
- **Implied store product:** `9NTC2KMZMGND` (Microsoft Store product ID used as victim tracking token)
- **Package identity:** `HobbyApps.Lightshot` (impersonating Lightshot screenshot tool)
- **Publisher display name:** `Hobby Apps`

---

## 6. Emulation Results

**Speakeasy (generic runner, x86 DLL):** No IOCs extracted. Expected result — the Go runtime in the DLL requires the full Windows environment; speakeasy's API stubs do not satisfy the Go scheduler and runtime initialization requirements.

**capa / peframe:** Both timed out (>300 s) on the 65 MB Go binary. Consistent with known capa limitations on large Go binaries.

**Manual emulation (angr):** Not attempted — no isolated decrypt routine identified; the C2 URLs are plaintext in the Go string table, making emulation unnecessary for IOC recovery.

---

## 7. Sandbox Results

**ANY.RUN:** Submission failed — file size (138 MB) exceeds the Ally tier upload limit. API returned `"exceeds the limit of size bytes"`.

---

## 8. Cross-Reference: client.dll (mylabubus.shop)

This `client.dll` is a **confirmed variant** of the Go Backconnect RAT analyzed 2026-05-10 (SHA256: `09049e36...`, C2: `mylabubus[.]shop`). The following indicators appear verbatim in both samples:

| Indicator type | Previous sample | This sample |
|---|---|---|
| Source package path | `server/src/cmd/backconnect_dll/` | `server/src/backconnect` |
| Session library | `github.com/hashicorp/yamux` | `github.com/hashicorp/yamux` |
| HWID library | `github.com/denisbrodbeck/machineid` | `github.com/denisbrodbeck/machineid` |
| HWID source | `HKLM\...\MachineGuid` | `HKLM\...\MachineGuid` |
| Beacon fields | `USERID:◙BUILDVERSION:◙HWID:◙◙` | `USERID:%s\nBUILDVERSION:%s\nHWID:%s\n\n` |
| C2 path | `/register` (api1+api2 dual failover) | `/register` (api1+api2 dual failover) |
| C2 domain | `mylabubus[.]shop` | `storeappsupdatesapi[.]xyz` |
| Deployment | Electron + ffi-rs + `Start` export | Electron + ffi-rs + `Start` export |

**Assessment:** C2 infrastructure has rotated (`mylabubus.shop` → `storeappsupdatesapi.xyz`); source tree and beacon format are identical. This is an updated build of the same operator's tooling, packaged in a new Lightshot-themed lure rather than the unnamed DLL-only deployment of the earlier sample.

---

## 9. Analyst Notes

**Gogs embedding rationale:** The 65 MB binary is dominated by the `gogs.io/gogs` Go module. Most likely explanation: the operator maintains a private Gogs server, and the DLL exposes a local Gogs web UI (reachable via the yamux tunnel) so the operator can push/pull exfiltrated data as Git objects, browse repository content, or deliver second-stage payloads as repository files. The SQLite database stores the Gogs data locally on the victim. This design provides a plausible-looking Git workflow for C2 that is harder to fingerprint than a custom protocol.

**Certificate abuse pattern:** Sectigo EV code-signing certificates on non-obvious companies (OC Agro ApS = Danish agricultural operation) is consistent with the cert-theft pattern seen in other recent campaigns analyzed in this workspace.

**Sideloading mechanism:** The MSIX has no valid store certificate. It must be installed via `Add-AppxPackage` after enabling sideloading, or through a social-engineering install flow. The `runFullTrust` capability is granted by the MSIX manifest without UAC since the package declares it — no elevation prompt on install.

**MSIX package name `HobbyApps.Lightshot`:** Impersonates Lightshot (getlightshot.com), a popular screenshot tool with ~100M downloads. Users searching for a free screenshot app are the likely target.

**Recommended follow-up:**
1. Block `storeappsupdatesapi[.]xyz` at DNS/proxy level; pivot for additional subdomains
2. Hunt for `client.dll` in `%LOCALAPPDATA%\Packages\HobbyApps.Lightshot_*\` across endpoints
3. Search for Sectigo cert serial `281cca56f214f9e84b03992ba076e318` across other observed binaries
4. Monitor HTTPS connections to `*.storeappsupdatesapi.xyz` — the yamux connection will appear as long-lived TLS sessions
5. Revocation request for Sectigo cert serial `281cca56f214f9e84b03992ba076e318`

---

## MITRE ATT&CK

| Technique | ID | Notes |
|-----------|-----|-------|
| Masquerading: Match Legitimate Name | T1036.005 | Package name `HobbyApps.Lightshot` impersonates Lightshot |
| Code Signing | T1553.002 | Sectigo EV cert on client.dll (abused) |
| Boot/Logon Autostart: Startup Task (MSIX) | T1547 | `uap5:StartupTask Enabled=true` |
| Native API | T1106 | Go runtime, Win32 APIs via CGO |
| Proxy: Multi-hop Proxy | T1090.003 | SOCKS5 + TCP/UDP relay via yamux |
| Application Layer Protocol: HTTPS | T1071.001 | C2 over HTTPS |
| System Information Discovery | T1082 | MachineGuid, hardware fingerprint |
| Process Discovery | T1057 | Running process enumeration |
| Input Capture: Keylogging | T1056.001 | Keylogger APIs present |
| Obfuscated Files: Software Packing | T1027.002 | Import by hash (4 hits) |
| Modify Registry | T1112 | MachineGuid read |
