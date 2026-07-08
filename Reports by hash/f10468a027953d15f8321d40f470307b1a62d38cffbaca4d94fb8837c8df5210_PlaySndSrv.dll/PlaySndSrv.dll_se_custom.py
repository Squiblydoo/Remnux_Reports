import sys, os, logging
sys.path.insert(0, "/home/remnux/mal")
sys.path.insert(0, "/opt/speakeasy/lib/python3.12/site-packages")
import warnings; warnings.filterwarnings("ignore")

import speakeasy as se_mod
from speakeasy_lib.hooks import IOCCollector, register_all_hooks

logger = logging.getLogger('speakeasy')
if not logger.handlers:
    sh = logging.StreamHandler()
    logger.addHandler(sh)
logger.setLevel(logging.INFO)

target = "/home/remnux/mal/PlaySndSrv.dll"
dump_dir = "/home/remnux/mal/output/se_dumps"
out_path = "/home/remnux/mal/output/PlaySndSrv.dll_hooks2.json"
os.makedirs(dump_dir, exist_ok=True)

ioc = IOCCollector()
se = se_mod.Speakeasy(logger=logger)
mod = se.load_module(target)
register_all_hooks(se, ioc, dump_dir)

try:
    se.run_module(mod, all_entrypoints=True)
except Exception as e:
    print(f"[!] Exception: {e}")

print("\n[*] IOC Summary:")
print(ioc.summary())
ioc.dump(out_path)
