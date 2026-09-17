# PolicyMgr.exe — Malware Analysis Report

## 1. File Metadata

| Field | Value |
|---|---|
| Filename | PolicyMgr.exe |
| SHA256 | `88fce5bc260870ef6296c4c5967449d0dc38e83b3fcfea5a971446e8dfd1f5ff` |
| SHA1 | `6f61da7e3471b321f54537808a1578f389d90c24` |
| MD5 | `ba6158ffb7ef5e0bc522fc0118c9d2cf` |
| Imphash | `d2042fda34621d646dd03ddb4b86a69f` |
| File type | PE32 executable (GUI), Intel 80386, 6 sections |
| Size | 314,192 bytes |
| Compile TimeDateStamp | 2025-06-27 14:10:44 |
| Debug (Repro) TimeDateStamp | **2041-02-05 19:53:05** — in the future, deliberately manipulated (flagged anomaly, level 4) |
| Image base / entrypoint | 0x400000 / 0x41a5ff |

### Identity spoofing (three different faces)

The binary presents a **different identity depending on where it's inspected**, which is itself the primary evidence of intent:

| Inspection surface | Claimed identity |
|---|---|
| File Properties → VersionInfo | `CompanyName=ManageEngine`, `FileDescription=PolicyMgr policy manager`, `ProductName=PolicyDesk`, `LegalCopyright=Copyright (C) Zoho Corporation` |
| Embedded application manifest | `assemblyIdentity name="Microsoft.Windows.RuntimeBroker"`, `<description>Windows Runtime Broker</description>` |
| Actual UI shown to the victim | A 340×60 banner image reading **"Windows Update"** with the Windows logo |
| Actual code-signing certificate | Subject `YOUR CHANCE j.d.o.o` (Zagreb, Croatia), issued by SSL.com Code Signing Intermediate CA RSA R1 |

None of these four identities match. The real signer is a small Croatian entity ("YOUR CHANCE j.d.o.o") whose certificate was issued **2026-08-25**, roughly three weeks before this sample was observed — consistent with a freshly obtained or compromised code-signing certificate used to lend legitimacy to a lure that has nothing to do with either ManageEngine or Microsoft.

- Certificate serial: `66096fe6aab808036b840f230f5606a5`
- Certificate validity: 2026-08-25 → 2027-08-25

## 2. Classification

**Credential-phishing / local credential-validation tool disguised as a Windows Update prompt.**
Confidence: **High** (behavioral/code evidence is unambiguous), but **unattributed** to any named malware family — malcat's offline KesaKode returned zero matches, and the online KesaKode lookup (authoritative per workflow policy) also returned **no verdict** (empty result, i.e. below any reporting threshold). No family name is claimed.

Reasoning:
- The executable builds, entirely in memory/at runtime, a modal dialog with German-language labels `Benutzer:` (username), `Passwort` (password), `Best[ätigen]` (Confirm), `Abbreche[n]` (Cancel), styled with a "Windows Update" banner.
- It dynamically resolves `LogonUserA`, `NetUserEnum`, `NetWkstaGetInfo`, `NetApiBufferFree`, `GetAdaptersAddresses`, and `DialogBoxIndirectParamW` via `GetModuleHandleA`/`LoadLibraryA` + `GetProcAddress` instead of importing them statically — a classic static-analysis/import-table evasion technique (confirmed both in decompiled pseudocode and in speakeasy emulation, which resolved all six via hooked `GetProcAddress`).
- The entire rest of the program is gated behind a **real, successful Windows credential validation**: `EntryPoint` only proceeds past the dialog if it returns exit code `0x49`; otherwise it calls `TerminateProcess`.
- capa's "encrypt data using speck" flag is a **false positive** — traced to JPEG IDCT/zigzag arithmetic used to decode the embedded banner image, not an actual cipher.

## 3. Capabilities

- Displays a fake "Windows Update" branded credential prompt (German language: Benutzer/Passwort).
- Enumerates real local/domain user accounts via `NetUserEnum` + `NetWkstaGetInfo` and pre-populates a selectable username combo box with them — the attacker doesn't need to already know a valid username on the box.
- Enumerates network adapter info (`GetAdaptersAddresses`) purely to render decorative "MAC:/DHCP:/DNS:" strings that reinforce the fake "network policy manager" cover story.
- Captures the typed password and validates the (username, domain, password) triple live against real Windows accounts via `LogonUserA` (`LOGON32_LOGON_INTERACTIVE`, `LOGON32_PROVIDER_DEFAULT`), with a domain parsed from a `DOMAIN\user` combo entry, falling back to the `USERDOMAIN` environment variable, and finally retrying against `"."` (the local machine) if the first attempt fails.
- On failed validation: shows a German error box — *"Ungültige Anmeldedaten. Bitte versuchen Sie es erneut."* ("Invalid credentials, please try again") — and lets the user retry indefinitely (**no lockout, no attempt counter, no backdoor/bypass password** was found in the validation logic).
- On successful validation: zeroes the in-memory password buffer, disables all dialog controls, and plays a **cosmetic fake progress bar** (randomized-interval timer, no real work performed) before auto-closing the dialog with the specific return code required to satisfy the entry-point gate.
- Opens/creates a file handle at `%APPDATA%\PolicyMgr\policyMgr.log` on first submit, before the credential buffers are populated and zeroed — the actual write call sits inside un-named helper functions that were not further decompiled, but the sequencing (open → populate credential globals → validate → zero globals) is strongly suggestive that validated credentials are logged to this file for later collection by another stage/operator, rather than being used only transiently in memory.
- Contains a large (66,799-byte) embedded RCDATA XML resource of generic, repeated "help documentation" boilerplate (plugin architecture, REST APIs, performance monitoring, etc.) — confirmed to be templated filler/chaff, not functional configuration or C2 data.
- No network stack imports of any kind (no WinHTTP/WinInet/Winsock) — this sample has **no built-in exfiltration or C2 capability**; any transmission of the harvested credentials would require a separate component reading `policyMgr.log`.

## 4. Attack Chain

This sample appears to be a single self-contained stage, most plausibly deployed **after** an attacker already has some foothold or delivery vector on the target (e.g. phishing attachment, dropped by another loader, or run manually by a lured user) to harvest a working set of domain/local credentials for lateral movement or privilege escalation:

1. Execution → dynamic API resolution (evades static import scanning).
2. Modal "Windows Update" credential dialog shown; username combo pre-filled from live account enumeration.
3. Victim enters a password, believing it's a routine Windows Update/policy prompt.
4. Credential validated live against the real OS via `LogonUserA` — guarantees only a *working* credential is captured/logged.
5. Credential (hypothesized, based on file-handle sequencing) logged to `%APPDATA%\PolicyMgr\policyMgr.log`.
6. Fake progress bar plays for user experience/plausibility, dialog closes, program exits — no further payload logic exists in this binary.

## 5. IOCs

**Network:** None — no network-capable imports or embedded C2 strings found.

**Filesystem:**
- `%APPDATA%\PolicyMgr\policyMgr.log` (hypothesized credential log; write call not fully confirmed by decompilation)

**Certificates:**
- Subject: `YOUR CHANCE j.d.o.o` (Zagreb, HR)
- Issuer: `SSL.com Code Signing Intermediate CA RSA R1`
- Serial: `66096fe6aab808036b840f230f5606a5`
- Validity: 2026-08-25 → 2027-08-25

**Registry / Mutexes:** None found (no such capability present).

**Hashes:**
- SHA256: `88fce5bc260870ef6296c4c5967449d0dc38e83b3fcfea5a971446e8dfd1f5ff`
- MD5: `ba6158ffb7ef5e0bc522fc0118c9d2cf`
- Imphash: `d2042fda34621d646dd03ddb4b86a69f`

## 6. Emulation Results

Speakeasy (x86, generic hook runner) emulated `EntryPoint` and confirmed all five sensitive APIs are resolved dynamically via hooked `GetProcAddress` (`LogonUserW`, `NetUserEnum`, `NetWkstaGetInfo`, `NetApiBufferFree`, `GetAdaptersAddresses`, plus `DialogBoxIndirectParamW`). Emulation naturally halted at the modal `DialogBoxIndirectParamW` call, since the sample's actual credential-capture/validation logic lives entirely inside the interactive `DialogProc` (`sub_418594`) and requires real user input to drive — this is expected and not a failure. No network/file/registry events were observed pre-interaction, consistent with static findings. The DialogProc logic itself was recovered instead via targeted decompilation (see Capabilities/Attack Chain above).

## 7. Sandbox Results

**ANY.RUN: Submission blocked, not attempted.** The Step 3.7.1 `curl` upload was denied by this session's Bash auto-mode classifier under a "PII Data Handling" label, traced to a contradictory/unfilled `autoMode.environment` policy line in `~/.claude/settings.json` ("share only with audiences cleared at the [named+specifics] bar" — the placeholder is never filled in, so no audience is ever considered cleared). This is a recurrence of a previously logged issue (see memory `feedback_anyrun_curl_upload_blocked`) under new wording. Resolving it requires the user to edit that policy line directly; the local static/emulation analysis above is the authoritative result for this sample.

## 8. Analyst Notes

- **Gaps:** The exact write call inside `%APPDATA%\PolicyMgr\policyMgr.log`'s handle (helper functions `sub_4125bd`/`sub_4112bf`/`sub_41137a`/`sub_4114bf`) was not fully decompiled/confirmed to actually write the credential buffers — this is inferred from call sequencing, not a directly observed `WriteFile`-equivalent call, since no `CreateFile`/`WriteFile` names appear as strings (they may in turn be dynamically resolved by ordinal or hash, a pattern consistent with the rest of the binary's API-hiding technique).
- **No sandbox coverage** was obtained (see §7) — dynamic confirmation of the log file write and any follow-on process/network activity by a second-stage component remains unverified.
- **No family attribution** — KesaKode (both offline and authoritative online lookup) returned no match; this does not appear to be a previously-catalogued toolkit in malcat's database.
- **Alternative hypothesis considered and rejected:** that this is a legitimate ManageEngine component merely re-signed by an unrelated party (a pattern seen before in this environment, e.g. `minibrowser.exe`, `ChilledWindows.exe`). This is inconsistent with the evidence here: a genuine ManageEngine PolicyMgr binary has no reason to (a) spoof its own manifest identity as `Microsoft.Windows.RuntimeBroker`, (b) build a German "Windows Update" credential-phishing dialog at runtime, or (c) hide its Windows-authentication API usage via dynamic resolution.
- **Recommended follow-up:** if this sample was recovered from an incident, check for `%APPDATA%\PolicyMgr\policyMgr.log` on affected hosts, and treat any credentials entered into "Windows Update" or "PolicyMgr" prompts around the observed timeframe as compromised. If additional related samples surface, look for the same certificate serial (`66096fe6aab808036b840f230f5606a5`) or the exact string `\PolicyMgr\policyMgr.log` as pivots.
