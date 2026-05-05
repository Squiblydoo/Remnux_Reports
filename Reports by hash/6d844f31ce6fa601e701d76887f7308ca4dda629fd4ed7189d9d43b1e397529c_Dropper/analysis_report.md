# Malware Analysis Report — Dropper.exe

**Date:** 2026-05-05  
**Analyst:** REMnux Automated Workflow  
**Sample:** `Dropper.exe`

---

## 1. File Metadata

| Field | Value |
|-------|-------|
| **Filename** | Dropper.exe |
| **SHA256** | `6d844f31ce6fa601e701d76887f7308ca4dda629fd4ed7189d9d43b1e397529c` |
| **SHA1** | `0515c909abc23ea4f2eeebb2ba15c0c7385965d5` |
| **MD5** | `8f94ad3c180f692f96c32c34ace9864e` |
| **Size** | 1,191,848 bytes (1.14 MB) |
| **Type** | PE32+ x86-64, GUI executable |
| **Sections** | 6 (`.text`, `.rdata`, `.data`, `.pdata`, `.rsrc`, `.reloc`) |
| **Compiler** | AutoIt 3 (aut2exe 3.3.14.3, x64) |
| **Debug Timestamp** | 2018-01-30 (forged; far pre-dates cert) |
| **VersionInfo** | OP Auto Clicker v4.1, `www.opautoclicker.com` |
| **Certificate Subject** | Minh Tran, Grand Prairie TX US |
| **Certificate Issuer** | Microsoft ID Verified CS EOC CA 03 |
| **Certificate Serial** | `330000b80523beb847f3bd8d4900000000b805` |
| **Certificate Validity** | 2026-05-03 → 2026-05-06 (**3-day Microsoft ID Verified cert**) |
| **Internal Branding** | OP Auto Clicker Advanced 4.0.1 (`ghost-mouse.com`) |

**Anomalies (malcat):**
- `.reloc` section entropy=226 with **no actual relocations** (level-4 anomaly — unusual padding/filler)
- 14,760-byte overlay (PKCS7 digital signature block)
- Debug timestamp (2018) inconsistent with certificate (2026)

---

## 2. Classification

| | |
|---|---|
| **Likely Family** | Re-signed / Trojanized OP Auto Clicker Advanced (ghost-mouse.com) |
| **Confidence** | Medium |
| **Type** | PUA / SmartScreen Bypass via fraudulent certificate; suspected dropper container |
| **Threat Level** | Medium — no confirmed payload delivered in this sample, but cert indicates active abuse campaign |

**Reasoning:**

The binary is a compiled AutoIt 3 script (`.a3x` bytecode embedded as an RCDATA resource in the PE32+ wrapper, extracted with `autoit-ripper`). The decompiled 694 KB source matches the feature set of **OP Auto Clicker Advanced 4.0.1** from `ghost-mouse.com`: GUI macro recorder with `SetWindowsHookEx` keyboard/mouse hooks, three `.ico` resource drops (`playback.ico`, `record.ico`, `stop.ico`), an INI config at `ACLib\ACA_conf.ini`, and help links pointing to `www.remouse.com`.

No network C2, download, or data exfiltration calls were found in the AutoIt source. The HKCU `Run` key string is present in the string table but no corresponding `RegWrite` call was located in the decompiled source.

The primary threat vector is the **fraudulent 3-day Microsoft ID Verified certificate** in the name of "Minh Tran." This cert type costs ~$50 USD, requires only email verification, and is explicitly used to bypass Windows SmartScreen reputation checks on newly encountered executables. The short 3-day window is consistent with an actor using a new identity per campaign, similar to previously analyzed samples in this workspace (Calc.exe, cmd.Exe, npp installer).

The "Dropper.exe" filename is highly suspicious for an auto clicker binary and suggests this may be one stage of a delivery chain — a clean or lightly modified legitimate tool signed with a fraudulent cert to serve as a trust anchor or cover binary, with the actual malicious payload delivered separately.

---

## 3. Capabilities

- **String table obfuscation**: All 4,274 string constants stored as hex-encoded values in a single binary blob inside function `_G2NEG45M58K()`, split by the `TB6{` delimiter and loaded at startup via `#OnAutoItStartRegister "A4F00202C4F_"`. Decoded at runtime via `StringSplit(FileRead(tempfile), 'TB6{', 1)` into `$OS[]` array.
- **Keyboard/mouse hook recording** (`SetWindowsHookEx`): Captures keystroke and mouse events into `$A558F401A05` buffer via `DllCallbackRegister`. **Data feeds into playback only** — no exfiltration path found.
- **Macro playback**: Replays recorded events via `A4700D0402E()` / `A1910003B29()`.
- **GUI resource drops**: Writes three ICO files to `@ScriptDir\ACLib\` on first run.
- **Persistence candidate**: `HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run` key present in string table; no confirmed `RegWrite` call found in decompiled source (may be dead code or encoded differently).
- **SmartScreen bypass**: 3-day Microsoft ID Verified cert suppresses SmartScreen "Unknown publisher" warning (T1553.002).
- **Masquerading**: Claims to be "OP Auto Clicker" v4.1 via VersionInfo; binary branding and string table reference `ghost-mouse.com` / OP Auto Clicker Advanced 4.0.1 (T1036.001).

---

## 4. Attack Chain

```
[Delivery] → Dropper.exe (re-signed AutoIt auto clicker)
                │
                ├─ Fraudulent 3-day cert → SmartScreen suppressed
                ├─ AutoIt runtime starts → #OnAutoItStartRegister fires
                │     └─ _G2NEG45M58K() blob loaded → $OS[] string table decoded
                │
                ├─ GUI presented as "OP Auto Clicker Advanced"
                │     ├─ SetWindowsHookEx → keyboard/mouse recording
                │     └─ ACLib\*.ico resources dropped
                │
                └─ [No confirmed C2/payload in this sample]
                      └─ "Dropper" filename suggests separate payload delivery chain
```

**Assessment:** This sample alone does not deliver a malicious payload. It is either:
1. A **signed trust anchor** in a multi-stage chain, where the user is socially engineered to run this first to establish a trusted process context, or
2. A **proof-of-concept re-signing** of a legitimate tool to test the fraudulent cert before attaching a real dropper, or
3. The **actual dropper** with the payload not yet embedded (test/staging build)

---

## 5. IOCs

### Network
| Indicator | Type | Notes |
|-----------|------|-------|
| `ghost-mouse[.]com` | Domain | Legitimate OP Auto Clicker branding; **not C2** |
| `www[.]remouse[.]com` | Domain | Legitimate help link in string table; **not C2** |
| `www[.]opautoclicker[.]com` | Domain | VersionInfo lure string; **not C2** |

*No malicious network IOCs recovered. ANY.RUN confirmed no malicious connections.*

### Certificate
| Indicator | Type | Notes |
|-----------|------|-------|
| `330000b80523beb847f3bd8d4900000000b805` | Cert Serial | Fraudulent 3-day Microsoft ID Verified cert; "Minh Tran", Grand Prairie TX |

### File System
| Indicator | Type | Notes |
|-----------|------|-------|
| `<ScriptDir>\ACLib\ACA_conf.ini` | File path | AutoIt config file |
| `<ScriptDir>\ACLib\playback.ico` | File path | GUI resource |
| `<ScriptDir>\ACLib\record.ico` | File path | GUI resource |
| `<ScriptDir>\ACLib\stop.ico` | File path | GUI resource |

### Registry
| Indicator | Type | Notes |
|-----------|------|-------|
| `HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run` | Registry key | In string table; no confirmed write in decompiled source |

### Hashes
| Hash | Algorithm |
|------|-----------|
| `6d844f31ce6fa601e701d76887f7308ca4dda629fd4ed7189d9d43b1e397529c` | SHA256 |
| `0515c909abc23ea4f2eeebb2ba15c0c7385965d5` | SHA1 |
| `8f94ad3c180f692f96c32c34ace9864e` | MD5 |

---

## 6. Emulation Results

**Speakeasy (generic runner, pass 1):** 0 IOCs recovered. AutoIt executables embed a proprietary runtime (`#AutoIt3_x64.dll` equivalent) that is not stubbed in speakeasy — the emulator executes only the PE stub loader without reaching the AutoIt script engine. No network, registry, or file events captured.

**Speakeasy (pass 2 / plain):** Not attempted — outcome would be identical given AutoIt runtime dependency.

**autoit-ripper:** Successfully decompiled embedded `.a3x` bytecode → 693.9 KB source at `/home/remnux/mal/output/autoit_extracted/script.au3`. Manual analysis of decompiled source substituted for emulation.

---

## 7. Sandbox Results

**ANY.RUN (Ally Tier — Public Submission)**

| Field | Value |
|-------|-------|
| **Task ID** | `becc9db2-514b-4c02-9858-6ceeb299b31f` |
| **Verdict Score** | **100 / 100** |
| **Threat Level** | Malicious activity |
| **Family Tags** | `autoit` |
| **Public Report** | `https://app.any.run/tasks/becc9db2-514b-4c02-9858-6ceeb299b31f` |

**Behavioral findings:**
- Tagged as `autoit` — sandbox recognizes the runtime
- No malicious DNS requests, connections, or HTTP activity in IOC report
- Windows baseline traffic only (OCSP, telemetry)

**Interpretation:** The 100/100 score likely reflects the short-lived fraudulent certificate and AutoIt runtime detection rather than observed malicious behavior. The absence of network IOCs in the sandbox is consistent with static analysis findings — no C2 hardcoded.

---

## 8. Analyst Notes

**Key uncertainties:**

1. **Missing payload**: The filename "Dropper.exe" is unexplained if this is purely a repackaged auto clicker. The most likely scenario is that this is a **test or staging build** — the cert was minted 2026-05-03 (2 days ago), and the payload has not yet been embedded or was stripped before submission.

2. **HKCU Run key**: The persistence key string is present but no write call found. AutoIt's string obfuscation is heavy — the `RegWrite` call may exist in a section not decompiled cleanly, or the Run key may be set conditionally at install time only. Recommend dynamic analysis in a Windows VM to confirm persistence behavior.

3. **Autoit-ripper fidelity**: Decompiled source is approximate. Obfuscated variable names and large function count (hundreds) mean some code paths may have been missed in manual review. A full trace execution in a Windows AutoIt interpreter would be definitive.

4. **Ghost-mouse.com vs OP Auto Clicker**: The VersionInfo claims `opautoclicker.com` (a different product) while the string table references `ghost-mouse.com` (OP Auto Clicker Advanced 4.0.1). This mismatch may indicate a partially updated re-sign job, or intentional VersionInfo spoofing to claim a different product.

5. **Certificate abuse pattern**: The 3-day Microsoft ID Verified cert technique has appeared in multiple samples in this workspace (Calc.exe, Dienstangebot lure). This cert type is being actively abused by multiple actors; cert serial `330000b80523beb847f3bd8d4900000000b805` should be blocklisted.

**Recommended follow-up:**
- Execute in a Windows VM sandbox with network capture to confirm or rule out runtime payload download
- Search for other samples signed with the same cert serial or same subject "Minh Tran" / Grand Prairie TX on VirusTotal, Malshare
- Correlate with any "OP Auto Clicker" lure-themed phishing campaigns active week of 2026-05-03

**MITRE ATT&CK TTPs:**
- T1036 — Masquerading
- T1036.001 — Invalid Code Signature (fraudulent Microsoft ID Verified cert)
- T1553.002 — Code Signing (SmartScreen bypass)
- T1027 — Obfuscated Files or Information (hex string table, AutoIt bytecode)
- T1027.002 — Software Packing (AutoIt aut2exe wrapper)
- T1056.001 — Keylogging (SetWindowsHookEx; assessed as auto clicker feature, not exfiltration)
- T1547.001 — Registry Run Keys / Startup Folder (string present; write unconfirmed)
