"""
Custom speakeasy emulation for photo20260411689.com
Patches NtAllocateVirtualMemory (unsupported) and captures WinHTTP C2 calls.
"""
import sys, json, os
sys.path.insert(0, "/opt/speakeasy/lib/python3.12/site-packages")
sys.path.insert(0, "/home/remnux/mal")
import warnings; warnings.filterwarnings("ignore")

import speakeasy
from speakeasy_lib.hooks import IOCCollector, read_astr, read_wstr, register_all_hooks

SAMPLE = "/home/remnux/mal/photo20260411689.com"
OUT    = "/home/remnux/mal/output/photo20260411689_custom_hooks.json"
DUMP_DIR = "/home/remnux/mal/output/se_dumps"

iocs = IOCCollector()
se = speakeasy.Speakeasy()
m  = se.load_module(SAMPLE)

# Register all standard hooks
register_all_hooks(se, iocs, DUMP_DIR)

# ── Patch: NtAllocateVirtualMemory → return 0 (STATUS_SUCCESS) ─────────────
# photo20260411689.com TLS callback stops here; stub it to continue execution
def on_ntalloc(emu, api_name, func_va, params):
    print(f"[NtAllocateVirtualMemory stub] returning STATUS_SUCCESS")
    return 0

se.add_api_hook(on_ntalloc, "ntdll", "NtAllocateVirtualMemory")
se.add_api_hook(on_ntalloc, "kernel32", "NtAllocateVirtualMemory")

# ── Extra WinHTTP hooks (C2 extraction) ──────────────────────────────────────
def on_winhttpconnect(emu, api_name, func_va, params):
    host = read_wstr(emu, params[1]) if params[1] else "<null>"
    port = params[2]
    print(f"[WinHttpConnect] host={host!r} port={port}")
    iocs.add("network", api="WinHttpConnect", host=host, port=port)

def on_winhttpopenreq(emu, api_name, func_va, params):
    method = read_wstr(emu, params[1]) if params[1] else "GET"
    target = read_wstr(emu, params[2]) if params[2] else "<null>"
    print(f"[WinHttpOpenRequest] method={method!r} path={target!r}")
    iocs.add("network", api="WinHttpOpenRequest", method=method, path=target)

def on_winhttpsend(emu, api_name, func_va, params):
    headers = read_wstr(emu, params[1]) if params[1] else ""
    print(f"[WinHttpSendRequest] headers={headers[:200]!r}")
    iocs.add("network", api="WinHttpSendRequest", headers=headers[:200])

def on_winhttp_crackurl(emu, api_name, func_va, params):
    url_ptr = params[0]
    url_len = params[1]
    url = ""
    if url_ptr:
        try:
            size = (url_len or 512) * 2
            raw = bytes(emu.mem_read(url_ptr, size))
            url = raw.decode("utf-16-le", errors="replace").split("\x00")[0]
        except Exception:
            url = read_astr(emu, url_ptr)
    print(f"[WinHttpCrackUrl] url={url!r}")
    iocs.add("network", api="WinHttpCrackUrl", url=url)

se.add_api_hook(on_winhttpconnect,   "winhttp", "WinHttpConnect")
se.add_api_hook(on_winhttpopenreq,   "winhttp", "WinHttpOpenRequest")
se.add_api_hook(on_winhttpsend,      "winhttp", "WinHttpSendRequest")
se.add_api_hook(on_winhttp_crackurl, "winhttp", "WinHttpCrackUrl")

# ── Registry hooks ────────────────────────────────────────────────────────────
def on_regopen(emu, api_name, func_va, params):
    key = read_astr(emu, params[1]) if params[1] else "<null>"
    print(f"[RegOpenKeyExA] key={key!r}")
    iocs.add("registry", api="RegOpenKeyExA", key=key)

def on_regset(emu, api_name, func_va, params):
    val = read_astr(emu, params[1]) if params[1] else "<null>"
    print(f"[RegSetValueExA] value_name={val!r}")
    iocs.add("registry", api="RegSetValueExA", value=val)

se.add_api_hook(on_regopen, "advapi32", "RegOpenKeyExA")
se.add_api_hook(on_regset,  "advapi32", "RegSetValueExA")

# ── Shell / process hooks ─────────────────────────────────────────────────────
def on_shellexec(emu, api_name, func_va, params):
    op   = read_astr(emu, params[1]) if params[1] else ""
    file = read_astr(emu, params[2]) if params[2] else "<null>"
    args = read_astr(emu, params[3]) if params[3] else ""
    print(f"[ShellExecuteA] op={op!r} file={file!r} args={args[:200]!r}")
    iocs.add("processes", api="ShellExecuteA", op=op, file=file, args=args[:200])

def on_createproc(emu, api_name, func_va, params):
    app = read_astr(emu, params[0]) if params[0] else "<null>"
    cmd = read_astr(emu, params[1]) if params[1] else "<null>"
    print(f"[CreateProcessA] app={app!r} cmd={cmd[:200]!r}")
    iocs.add("processes", api="CreateProcessA", app=app, cmd=cmd[:200])

se.add_api_hook(on_shellexec,  "shell32",  "ShellExecuteA")
se.add_api_hook(on_createproc, "kernel32", "CreateProcessA")

# ── Run ───────────────────────────────────────────────────────────────────────
print("[*] Starting emulation (all_entrypoints=True) ...")
try:
    se.run_module(m, all_entrypoints=True)
except speakeasy.errors.SpeakeasyError as e:
    print(f"[!] SpeakeasyError: {e}")
except Exception as e:
    print(f"[!] Exception: {e}")

os.makedirs(DUMP_DIR, exist_ok=True)
iocs.dump(OUT)
print(f"[*] Done → {OUT}")
print(json.dumps(json.load(open(OUT)), indent=2))
