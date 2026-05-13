# Malware Analysis Report: PDFWorld-new.exe

**Date:** 2026-05-13  
**Analyst:** REMnux / Claude  
**Sample:** `PDFWorld-new.exe`

---

## 1. File Metadata

| Field | Value |
|---|---|
| **SHA256** | `ef21946b69c03458a0d9ebc84e272622cd7ae14e2170624182ff18dc34b0f458` |
| **SHA1** | `865b346dce6163e2ba0bb8a42f3a108a3239624d` |
| **MD5** | `6e94d5a308e02649cc6e63e3d07ba13b` |
| **Size** | 152,612,960 bytes (152 MB) |
| **Type** | PE32+ x64 GUI — .NET 8 Single-File Bundle (AppHost + 142 MB overlay) |
| **Certificate** | GlobalSign GCC R45 EV CodeSigning CA 2020 |
| **Cert Subject** | METROPOLITAN DESIGN LLC |
| **Cert Details** | Hawthorne, New Jersey, US |
| **Cert Serial** | `44b8cf369d520790fd7e3654` |
| **Cert Validity** | 2025-05-15 → 2026-05-16 (1-year EV) |
| **VersionInfo** | CompanyName=PDFWorld, FileDescription=PDF World, v1.0.4.0 |
| **InternalName** | `PDFWorld.dll` |
| **Copyright** | 2024 (c) KMG |
| **Product Version** | `1.0.4+6a9692fb86071a460dee5dfe21d56ed9baae4e6d` |
| **Debug PDB (host)** | `D:\a\_work\1\s\artifacts\obj\coreclr\windows.x64.Release\Corehost.Static\singlefilehost.pdb` |
| **Build Date (host)** | 2026-03-19 (standard .NET 8.0.26 CoreHost build) |
| **Bundle ID** | `8YTdk3IDHlicpnnylmovRxqijnaRajU=` |

### PDFWorld.dll (payload DLL, extracted from bundle)

| Field | Value |
|---|---|
| **SHA256** | `d0b64889bf277b3ef379da1bc10c87ac0eea0b59247aab5863a13a2a4ebb58fb` |
| **Size** | 6,721,024 bytes (6.4 MB) |
| **Type** | PE/DOTNET x64 DLL |
| **Debug PDB** | `C:\development\pdfworld\PDFWorld\PDFWorld\obj\Release\net8.0-windows\win-x64\PDFWorld.pdb` |
| **Build Timestamp** | 2053-04-09 (manipulated — future date indicates zeroed/forged timestamp) |
| **Architecture** | .NET 8 managed code (WPF + Prism MVVM) |

---

## 2. Classification

**Family:** PUP / Browser Hijacker (Chrome Search Engine Hijacker)  
**Type:** PPI (Pay-Per-Install) bundler with affiliate tracking  
**Confidence:** **High**

**Reasoning:**
- Full source-level decompilation via `ilspycmd` reveals `ChromeService.ChangeSearchEngine()` — uses Win32 `SendKeys` + clipboard automation to navigate Chrome's `chrome://settings/searchEngines` UI and replace the default search engine with a server-specified domain.
- `RestService.SendInfo()` beacons all user consent events (initiate, accepted, declined, cancelled, validated, nonvalidated) to `api.pdf-world.com/ping.php?cid={affiliate_id}`.
- `LoadData()` extracts the campaign/affiliate ID directly from the **executable filename** (e.g. `Setup_12345.exe` → ID `12345`) — standard PPI distribution pattern.
- The EULA embedded in the binary explicitly names **Smart Web Search** (Israeli company, Tel-Aviv courts) as the operator, not "PDF World" or "KMG".
- The real company domains are `pdf.smart-websearch.com` and `amazing-search.com`, not the `pdf-world.com` C2.

---

## 3. Capabilities

### C2 / Exfiltration
- **Registration beacon:** POST to `https://api.pdf-world.com/first_run.php` with JSON payload containing:
  - `ProductName` = "PDF World"
  - `BrowserName` = "Chrome"
  - `Os_Version` — WMI query `SELECT Caption, Version FROM Win32_OperatingSystem`
  - `WizardVersion` = "1.0.4"
  - `PortNumber` — ACPI device count (see VM detection below)
- **Event tracking:** POST to `https://api.pdf-world.com/ping.php?cid={id}&step={step}&type={type}` for all user decisions.
- **URL parameters appended to all redirects:** `p1={OS_version}-{ACPI_count}`, `p2={dotnet_versions}`, `p3=c{extension_count}`, `p4={assembly_version}` — leaks system profile on every click.

### Affiliate / Campaign ID
- `LoadData()` parses the **EXE filename** to extract an affiliate ID: filename like `Setup_ABCDE.exe` → ID `ABCDE` → sent as `cid=ABCDE` in all C2 calls.
- Enables per-affiliate commission tracking for the PPI operator.

### VM Detection
- `Check()` (RestService) runs WMI: `SELECT * FROM Win32_PnPEntity WHERE DeviceID LIKE '%ACPI%'`, counts ACPI-named devices.
- Result sent as `PortNumber` — real machines have more ACPI devices than sandboxes; the C2 can filter sandbox submissions.

### Chrome Profile Inspection
- Reads `%LOCALAPPDATA%\Google\Chrome\User Data\Default\Preferences` and `Secure Preferences`.
- Counts occurrences of `default_search_provider_data.template_url_data` to detect pre-existing extension changes.
- Reads `mirrored_template_url_data.url` to capture the current search engine URL before hijacking.

### Chrome Search Engine Hijacking
- Hijack is performed via UI automation (not direct file modification), bypassing Chrome's integrity checks on Preferences:
  1. Opens a new Chrome tab via `Ctrl+T`.
  2. Pastes `chrome://settings/searchEngines` via clipboard and `Ctrl+V`, then `Enter`.
  3. Waits 5 seconds for the page to load.
  4. Detects Chrome version UI variant ("sit" in tab title = old UI, else new).
  5. Tabs through the search engine list, uses spacebar to select the injected search engine.
  6. Opens a new tab to `ValidationLink` URL; checks if tab title contains "validate" to confirm success.
  7. Sends `validated` or `nonvalidated` beacon to C2.
- The target search engine URL/domain is delivered **dynamically from `first_run.php`** response (`offer_search_domain` field) — operator can hot-swap destination without a new build.

### Window Title Monitoring
- `GetActiveTabTitle()` uses Win32 `GetWindowText()` / `GetWindowTextLength()` to read the foreground Chrome window title.
- Used for validation confirmation, not credential theft — but provides a channel for passive browsing activity monitoring.

### Installer UI (WPF Wizard)
- Multi-screen WPF installer: EULA/Terms → Optional offer (search engine change).
- Shows a promotional video (`FreePDFInstallerVideo.wmv`) extracted to `%LOCALAPPDATA%\PDFWorld\`.
- Optional offer checkbox: `IsOptionalOfferSelected` / `IsExtensionOfferEnabled`.
- Sets installer variant number (02 = no extensions detected, 03 = extensions detected).

### Registry Persistence
- Writes to `HKCU\Software\DK\DKClose` — likely marks completion to prevent re-display.

---

## 4. Attack Chain

```
1. User downloads "PDFWorld-new.exe" (or affiliate-renamed variant)
         ↓
2. On launch — LoadData() extracts affiliate ID from filename
         ↓
3. SendInfo("initiate", "w1") → ping.php?cid={id}&step=w1&type=initiate
         ↓
4. GetDataInfo() → POST first_run.php (OS info, ACPI count, wizard version)
   ← Response: offer_name, offer_search_domain, offer_eula_url, validation_url, typ_url, decline_url
         ↓
5. WPF installer shows EULA + optional search engine offer screen
         ↓
6a. User ACCEPTS:
    - SendInfo("accepted") → ping.php
    - ChromeService.ChangeSearchEngine(): keyboard/clipboard automation
      to chrome://settings/searchEngines → replace default search engine
    - Verify via tab title → SendInfo("validated" / "nonvalidated")
    - Open "thank you" page (typ_url) in Chrome

6b. User DECLINES:
    - SendInfo("declined") → ping.php
    - Open decline_url in Chrome

7. Registry: HKCU\Software\DK\DKClose written
```

---

## 5. IOCs

### Network (defanged)

| Type | Value | Note |
|---|---|---|
| URL | `hxxps://api[.]pdf-world[.]com/first_run[.]php` | Registration / system fingerprint POST |
| URL | `hxxps://api[.]pdf-world[.]com/ping[.]php` | Event beacon (cid, step, type params) |
| Domain | `api[.]pdf-world[.]com` | Primary C2 |
| Domain | `www[.]pdf-world[.]com` | Legal/EULA pages |
| Domain | `www[.]amazing-search[.]com` | Support link |
| Domain | `www[.]pdf[.]smart-websearch[.]com` | Operator's real domain (in EULA) |
| URL | `hxxps://www[.]pdf-world[.]com/legal/terms[.]html` | Terms page |
| URL | `hxxps://www[.]pdf-world[.]com/legal/privacy[.]html` | Privacy policy |
| URL | `hxxps://www[.]pdf-world[.]com/legal/eula[.]html` | EULA |

*Note: The actual hijacked search engine domain is delivered dynamically from `first_run.php` and was not recovered in static analysis.*

### Filesystem

| Path | Action |
|---|---|
| `%LOCALAPPDATA%\PDFWorld\FreePDFInstallerVideo.wmv` | Created — promotional video |
| `%LOCALAPPDATA%\PDFWorld\` | Created — application directory |
| `%LOCALAPPDATA%\Google\Chrome\User Data\Default\Preferences` | Read — Chrome profile inspection |
| `%LOCALAPPDATA%\Google\Chrome\User Data\Default\Secure Preferences` | Read — Chrome profile inspection |

### Registry

| Key | Value | Action |
|---|---|---|
| `HKCU\Software\DK\DKClose` | (written) | Completion marker |
| `HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe` | (read) | Locates Chrome executable |

### Mutexes / Named Objects
None identified in static analysis.

---

## 6. Emulation Results

**Speakeasy / angr:** Not attempted. This is a .NET 8 WPF application requiring a full Windows desktop environment with Chrome installed. Speakeasy cannot stub the required WPF runtime, `System.Windows.Forms.SendKeys`, nor the Win32 UI interaction (`SetForegroundWindow`, `GetWindowText`). All meaningful IOCs were recovered via static analysis and .NET decompilation.

**Floss:** Not productive (tool version incompatibility with `--no-static-strings` flag; static strings already captured via malcat and ilspycmd).

**capa:** Timed out (152 MB binary exceeds practical capa analysis time).

---

## 7. Sandbox Results

**ANY.RUN:** Submission failed — file size (152 MB) exceeds the platform's upload limit.

---

## 8. YARA Match Analysis

| Match | Verdict | Reasoning |
|---|---|---|
| `DotNetSingleFileBundle` | True positive | Confirmed .NET 8 self-contained bundle |
| `KeyloggerApi` | False positive | `GetWindowText()` reads Chrome tab title for hijack validation, not credentials |
| `DownloadUsingWininet` | True positive | WinInet used for C2 HTTP communication |
| `PostHttpForm` | True positive | `application/json` POST to `first_run.php` |
| `FingerprintHardware` | True positive | WMI ACPI device enumeration for VM detection |
| `FingerprintEnvironment` | True positive | WMI OS version query |
| `ValuableFileExtensions` | False positive | Extension strings appear in embedded EULA text, not file targeting |
| `ElevatePrivileges` | Likely false positive | Standard installer operations; no UAC bypass confirmed |
| `RunShell` | True positive | `Process.Start(chromeExePath, ...)` to open Chrome |

---

## 9. Analyst Notes

### Attribution
- Real operator: **Smart Web Search** (Israeli company, Tel-Aviv court jurisdiction per EULA).
- Front branding: "PDF World" / "KMG" copyright.
- Signing entity: "METROPOLITAN DESIGN LLC" (Hawthorne, NJ) — likely a shelf company or reseller obtaining EV certs for distribution.
- The `Software\DK` registry key and `DKClose` value suggest internal operator codename "DK".

### Distribution Model
This is a PPI (Pay-Per-Install) Chrome search hijacker. Affiliates receive campaign-tagged copies (e.g. `Setup_XXXX.exe`) and earn commissions per validated install. The affiliate ID is encoded in the filename, extracted at runtime, and sent in all C2 calls. "Validated" means Chrome's search engine was successfully changed and confirmed.

### Dynamic Search Engine
The hijacked search engine domain is not hardcoded — it is retrieved from `first_run.php`. This means:
- Operator can hot-swap the target search engine without redistributing the installer.
- The actual search domain used against each victim is logged server-side but not recoverable from static analysis.
- Historical Smart Web Search campaigns have pointed to domains like `amazing-search.com` and related properties (note: `amazing-search.com` appears as the support link, suggesting this may be the current search domain).

### Legitimacy Assessment
The EULA does technically disclose search engine modification ("1.5.2 The default search engine in your Internet Browser's built-in search box"). However:
- The primary lure is a "PDF World" installer with no functional PDF capability evident in the application.
- The EULA is 19 pages of dense legal text and the company name disclosed ("Smart Web Search") does not match the installer branding ("PDFWorld").
- UI automation to navigate Chrome's settings page is deliberately obfuscated as a normal install.
- This pattern is well-established as PUP distribution even when nominally consented to.

### Follow-up Recommendations
1. Attempt to live-detonate in a Windows sandbox with Chrome installed to recover the dynamic search engine domain from `first_run.php` response.
2. Search VirusTotal/Hybrid-Analysis for other samples sharing `api.pdf-world.com` or cert serial `44b8cf369d520790fd7e3654`.
3. Check for sibling installers with `_XXXX` campaign ID suffixes targeting the same C2.
4. Investigate `amazing-search.com` as the likely hijack destination.
