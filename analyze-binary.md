Perform a full malware analysis of the binary at path: $ARGUMENTS

Follow this structured workflow using both the `remnux` and `malcat` MCP servers.

## Step 1 — File Identification (remnux)
Call `mcp__remnux__get_file_info` on the file path.
Report: SHA256, MD5, file type, size. Save the SHA256 — you will need it throughout.

## Step 2 — Malcat Deep Static Analysis
Call `mcp__malcat__analyse_file` with the absolute file path. Save the returned analysis_id for all subsequent malcat calls.

From the analysis_id, run these in order:
- `mcp__malcat__analyse_infos` — file type, architecture, compiler, protections, packer detection, anomalies summary
- `mcp__malcat__strings_top_list` — top strings (use maximum_number_of_strings=256)
- `mcp__malcat__anomalies_list` — all detected anomalies
- `mcp__malcat__yara_list` — family/capability YARA matches
- `mcp__malcat__fns_top_list` — top interesting functions

If the file contains carved or virtual sub-files (e.g. embedded PE, archives):
- `mcp__malcat__file_list_carved` then `mcp__malcat__analyse_carved_file` for each
- `mcp__malcat__file_list_virtual_files` then `mcp__malcat__analyse_virtual_file` for each

For interesting functions (scored high or suspiciously named):
- `mcp__malcat__fn_decompile` to get pseudocode
- `mcp__malcat__fn_disassemble` if decompile is unclear

If malcat identifies encrypted/obfuscated strings:
- `mcp__malcat__decrypt_string` on candidate strings
- `mcp__malcat__chain_decrypt_analysis` to trace multi-layer decryption routines

## Step 3 — Specialized REMnux Tool Analysis
Based on file type from Step 1:
- **Any PE**: `mcp__remnux__run_tool` with tool=`capa`, then `peframe`
- **Any PE (strings)**: run `floss` — use args `-q --no-static-strings <filepath>` to extract only deobfuscated/stack/tight strings (skip static strings already seen in malcat); parse output for decrypted URLs, IPs, registry keys, mutex names
- **.NET PE**: also run `ilspycmd` (decompile) and `dotnetfile_dump`
- **Office/OLE**: `olevba`, `oleid`
- **PDF**: `pdfid`, `pdf-parser`
- **Script/JS**: `js-beautify`, `box-js`
- **AutoIt**: `autoit-ripper`

## Step 3.5 — Emulation (PE files only)

Emulation extracts runtime-decrypted IOCs (C2 URLs, IPs, registry keys, file paths, mutex names) that static analysis cannot recover. Run for all PE executables and DLLs unless the file is known-clean or a packer/protector completely blocks emulation.

### Shared hook library
A reusable speakeasy hook library lives at `/home/remnux/mal/speakeasy_lib/`:
- `hooks.py` — hook sets: WinHTTP, WinInet, Winsock, VirtualAlloc/Protect memory dumps, CreateProcess/WinExec, CreateMutex, RegSetValue, CryptDecrypt/BCryptDecrypt; plus `IOCCollector` and `register_all_hooks()`
- `runner.py` — generic runner that loads the target, registers all hooks, and saves structured JSON

**After every analysis**, if you wrote a new hook or technique that is not already in `hooks.py`, add it. If a hook in `hooks.py` was insufficient and you extended it for this sample, update the library function in place. Each hook function should have a short comment naming the malware family/campaign that motivated it.

### Emulation pass 1 — generic runner (always try first)
```bash
/opt/speakeasy/bin/python3 /home/remnux/mal/speakeasy_lib/runner.py \
  <filepath> --arch <x86|amd64> [--dll] \
  --dump-dir /home/remnux/mal/output/se_dumps \
  --out /home/remnux/mal/output/<filename>_hooks.json \
  --timeout 120
```
Run via Bash tool or `mcp__remnux__run_tool` tool=`bash`.

Parse `/home/remnux/mal/output/<filename>_hooks.json` for the structured `network`, `files`, `registry`, `mutexes`, `processes`, `strings`, `memory` arrays.

### Emulation pass 2 — plain speakeasy (if pass 1 produced no IOCs)
Run via `mcp__remnux__run_tool` with tool=`speakeasy`.

Determine bitness from Step 1/Step 2:
- 64-bit PE: args = `-t <filepath> -a amd64 -o /home/remnux/mal/output/<filename>_speakeasy.json -q 120`
- 32-bit PE: args = `-t <filepath> -a x86 -o /home/remnux/mal/output/<filename>_speakeasy.json -q 120`
- DLL (32-bit): args = `-t <filepath> -a x86 -d /tmp/se_drops -o /home/remnux/mal/output/<filename>_speakeasy.json -q 120`
- DLL (64-bit): args = `-t <filepath> -a amd64 -d /tmp/se_drops -o /home/remnux/mal/output/<filename>_speakeasy.json -q 120`

Parse the speakeasy JSON for:
- **`network_events`** → extract all URLs, hostnames, IPs, ports
- **`file_events`** → created/written file paths
- **`registry_events`** → read/written keys
- **`api_calls`** → `CreateMutex*` names, `WinExec`/`ShellExecute` commands, `CryptDecrypt` outputs, `VirtualAlloc`+write sequences
- **`errors`** / emulation stop reason → note limitations (e.g. "API not implemented", short trace)

### Emulation pass 3 — angr decrypt function extraction
Use angr when malcat's decompiler shows a decrypt/decode function and speakeasy can't reach it
(anti-debug gate, missing API stub, kernel dependency, etc.).

**When to use**: malcat `fn_decompile` reveals a self-contained crypto/XOR/decode function
(takes buffer + key + length args, returns nothing or length). angr calls it concretely —
no symbolic reasoning needed, just fast concrete execution with the real encrypted data.

**Workflow**:
1. `mcp__malcat__fn_decompile` on the decrypt function → note its VA, calling convention, arg order
2. `mcp__malcat__file_read` or `mcp__remnux__run_tool` to extract the encrypted blob bytes
3. Write `/home/remnux/mal/output/<filename>_angr.py`:
```python
import sys; sys.path.insert(0, "/home/remnux/mal")
from speakeasy_lib.angr_decrypt import DecryptFunctionRunner

runner = DecryptFunctionRunner("/path/to/sample.exe")
# Map encrypted data into angr memory at a scratch address
INPUT_VA = 0x200000
encrypted = open("/path/to/encrypted_blob.bin", "rb").read()
result = runner.call_function(
    func_va=0x<func_va_from_malcat>,   # already rebased; add runner.image_base() if RVA
    args=[INPUT_VA, <key_ptr_or_value>, len(encrypted)],
    input_data=encrypted,
    input_va=INPUT_VA,
    output_addr=INPUT_VA,
    output_len=len(encrypted),
    timeout=60,
)
print(result)
```
4. Run via Bash tool: `python3 /home/remnux/mal/output/<filename>_angr.py`
5. Parse result for plaintext C2 URLs, keys, config

Or use the CLI directly for simple cases:
```bash
python3 /home/remnux/mal/speakeasy_lib/angr_decrypt.py \
  /path/to/sample.exe \
  --func-va 0x<va> \
  --input /path/to/encrypted.bin \
  --input-va 0x200000 \
  --args 0x<key_va> <length> \
  --out /home/remnux/mal/output/<filename>_decrypted.bin \
  --timeout 60
```

For XOR loops specifically, `find_xor_key()` from `angr_decrypt.py` uses symbolic execution
to recover the key without knowing it in advance.

### Emulation pass 4 — custom speakeasy hook script
If passes 1–3 all fail (heavy VM-detect, .NET runtime dependency, kernel driver):
1. `mcp__malcat__fn_decompile` on the target function
2. Write `/home/remnux/mal/output/<filename>_se_custom.py`:
   - Import `from speakeasy_lib.hooks import IOCCollector, register_all_hooks`
   - Add sample-specific hooks (patch anti-debug, stub missing API, call export directly)
3. Run via Bash tool
4. **After success**: add the new hook to `hooks.py` with a comment `# <family> (<date>)`

If all emulation fails (kernel driver, .NET dependency):
- Try Qiling: `qltool run -f <filepath> --rootfs /usr/share/qiling/rootfs/x8664_windows`
- Note emulation limits in the report; Tria.ge sandbox (Step 3.7) provides dynamic coverage

### Manual decrypt follow-up
If any emulation pass reveals a decryption routine or key:
- Use `mcp__malcat__decrypt_string` / `mcp__malcat__chain_decrypt_analysis` with the recovered key to batch-decrypt remaining strings
- Write a standalone decrypt script to `/home/remnux/mal/output/<filename>_decrypt.py` if rerunnable

## Step 3.7 — Tria.ge Sandbox Submission

> **API key required.** Check `$TRIAGE_API_KEY` environment variable. If unset, check `/home/remnux/.triage_api_key`. If neither exists, skip this step and note "Tria.ge: API key not configured" in the report.

All `curl` calls go through `mcp__remnux__run_tool` with tool=`bash` (or use the Bash tool directly).

### 3.7.1 — Submit sample
```bash
TRIAGE_KEY="${TRIAGE_API_KEY:-$(cat /home/remnux/.triage_api_key 2>/dev/null)}"
curl -s -X POST "https://api.tria.ge/v0/samples" \
  -H "Authorization: Bearer ${TRIAGE_KEY}" \
  -F "file=@<filepath>" \
  -F "_json={\"kind\":\"file\",\"interactive\":false}" \
  | tee /home/remnux/mal/output/<filename>_triage_submit.json
```
Extract `id` from the JSON response (e.g. `240601-abcd1234ef`). If the response contains `error`, report it and skip the rest of this step.

### 3.7.2 — Poll for completion
Poll every 30 seconds (max 15 attempts = ~7.5 minutes):
```bash
curl -s "https://api.tria.ge/v0/samples/${SAMPLE_ID}" \
  -H "Authorization: Bearer ${TRIAGE_KEY}"
```
Continue polling until `status` is `reported` or `failed`. If `failed`, note it and skip to 3.7.4.

### 3.7.3 — Fetch overview report
```bash
curl -s "https://api.tria.ge/v0/samples/${SAMPLE_ID}/overview.json" \
  -H "Authorization: Bearer ${TRIAGE_KEY}" \
  | tee /home/remnux/mal/output/<filename>_triage_overview.json
```
Extract from overview:
- `targets[].iocs` → domains, IPs, URLs, emails
- `targets[].signatures` → matched behavioral signatures + scores
- `targets[].tags` → family tags (ransomware, trojan, etc.)
- `analysis.score` → overall threat score (1–10)
- `extracted[]` → extracted configs (C2s, encryption keys, campaign IDs)

### 3.7.4 — Fetch per-task triage reports
List tasks from the overview `tasks[]` array. For each task with `kind: "behavioral"`:
```bash
curl -s "https://api.tria.ge/v0/samples/${SAMPLE_ID}/reports/${TASK_ID}/triage_report.json" \
  -H "Authorization: Bearer ${TRIAGE_KEY}" \
  | tee /home/remnux/mal/output/<filename>_triage_${TASK_ID}.json
```
Extract from each triage report:
- `network.requests[]` → full HTTP requests/responses (URLs, user-agents, POST bodies, response codes)
- `network.flows[]` → raw TCP/UDP connections (IP:port)
- `network.dns[]` → DNS queries and responses
- `processes[]` → spawned process tree, command lines, injections
- `dumped[]` → memory dumps and carved files (download if relevant)
- `signatures[]` → MITRE ATT&CK mappings, behavioral indicators

### 3.7.5 — Download extracted configs (if present)
If the overview `extracted[]` array contains items with `config` type:
```bash
curl -s "https://api.tria.ge/v0/samples/${SAMPLE_ID}/files/${DUMP_NAME}" \
  -H "Authorization: Bearer ${TRIAGE_KEY}" \
  -o /home/remnux/mal/output/<filename>_triage_config_${DUMP_NAME}
```

### Tria.ge public report URL
`https://tria.ge/<SAMPLE_ID>` — include this in the final report.

## Step 4 — IOC Extraction (remnux)
Pass combined output from Steps 2–3.7 to `mcp__remnux__extract_iocs`.
Supplement with IOCs manually recovered from:
- speakeasy `network_events`
- floss deobfuscated strings
- Tria.ge `network.requests`, `network.dns`, `extracted` config blocks

De-duplicate and defang all network IOCs in the final report (replace `.` → `[.]`, `://` → `[://]`).

## Step 5 — Summary Report
Write a structured report to `/home/remnux/mal/output/<filename>_analysis_report.md` with:

1. **File Metadata** — hashes, type, size, signing info, build artifacts
2. **Classification** — malware family guess, confidence (low/medium/high), reasoning
3. **Capabilities** — bulleted list of what the malware does
4. **Attack Chain** — stages if determinable
5. **IOCs** — grouped: network (IPs, domains, URLs defanged), filesystem, registry, mutexes
6. **Emulation Results** — speakeasy/qiling findings; note if emulation was partial or blocked
7. **Sandbox Results** — Tria.ge score, signatures, behavioral summary, public URL; or "API key not configured" / "Submission failed: <reason>"
8. **Analyst Notes** — residual gaps, alternative hypotheses, recommended follow-up

### Cross-referencing previous analyses — strict policy
**Do NOT mention, compare, or link to previously analyzed samples unless at least one of the following is true:**
- **Identical certificate serial number** — same signing cert confirms shared infrastructure or resigning campaign
- **Matching C2 / network IOC** — exact same domain, IP, or URL path (not just similar patterns)
- **Extracted config values match** — same encryption key, campaign ID, mutex name, or hardcoded token recovered from both samples
- **Identical build artifact** — same PDB path, compiler-specific string, or internal build path appearing verbatim in both
- **Same payload hash** — a dropped or embedded file has the same SHA256 as a previously analyzed sample

Superficial similarities (same language, same packer, same general TTP category, similar lure theme, same cert issuer/CA but different serial) are **not** grounds for attribution or cross-reference. If the evidence meets the bar, state the specific matching indicator explicitly. If it does not, analyze this sample entirely on its own merits.

Then update `/home/remnux/.claude/projects/-home-remnux-mal/memory/MEMORY.md` with a one-paragraph summary entry if the sample is significant.

## Step 6 — Publish Report to GitHub

Local clone: `/home/remnux/mal/Remnux_Reports/`
Target subdirectory within clone: `Reports by hash/`

### Determine layout: flat file vs directory

**Use a directory** if the analysis produced any of the following alongside the main report:
- Sub-reports (e.g. API capability report, infrastructure report, protocol analysis)
- Analysis scripts (`.py`, `.ps1`, etc.)
- Emulation/tool output files (speakeasy JSON, triage JSON, etc.)

**Use a flat file** if the main report is the only output.

### Flat file layout (single report only)
Destination: `Reports by hash/<sha256>_<filename>_analysis_report.md`

```
cd "/home/remnux/mal/Remnux_Reports"
cp /home/remnux/mal/output/<filename>_analysis_report.md \
   "Reports by hash/<sha256>_<filename>_analysis_report.md"
git add "Reports by hash/<sha256>_<filename>_analysis_report.md"
```

### Directory layout (multiple files)
Destination: `Reports by hash/<sha256>_<filename>/`

Place inside the directory:
- Main report as `analysis_report.md`
- All sub-reports with descriptive names (e.g. `api_capability_report.md`)
- All analysis scripts retaining their original filenames
- All tool output files (speakeasy JSON, triage JSON, decrypt scripts, etc.)

```
cd "/home/remnux/mal/Remnux_Reports"
mkdir -p "Reports by hash/<sha256>_<filename>"
cp /home/remnux/mal/output/<filename>_analysis_report.md \
   "Reports by hash/<sha256>_<filename>/analysis_report.md"
# copy sub-reports, scripts, and output files into the directory
git add "Reports by hash/<sha256>_<filename>/"
```

### Commit and push (both cases)
```
git commit -m "analysis: <filename> (<sha256 first 8 chars>)"
git push origin main
```

Report the GitHub URL to the user on success:
- Flat: `https://github.com/Squiblydoo/Remnux_Reports/blob/main/Reports%20by%20hash/<sha256>_<filename>_analysis_report.md`
- Directory: `https://github.com/Squiblydoo/Remnux_Reports/tree/main/Reports%20by%20hash/<sha256>_<filename>/`

If the push fails (e.g. token not set, network error), warn the user but do not treat it as a fatal error — the local report in `/home/remnux/mal/output/` is the authoritative copy.
