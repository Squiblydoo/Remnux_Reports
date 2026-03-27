"""
Custom Speakeasy script to hook VirtualProtect and dump decrypted memory
after section setup is complete.
"""
import speakeasy
import speakeasy.winenv.api as api
import json, os, struct

OUTPUT = '/home/remnux/mal/output/Login_decrypted_sections/'
os.makedirs(OUTPUT, exist_ok=True)

se = speakeasy.Speakeasy()
mod = se.load_module('/home/remnux/mal/Login.exe')

vp_calls = []
dumped = set()

def hook_virtual_protect(se, call_conv, name, params):
    addr, size, new_prot, old_prot_ptr = params
    prot_name = {0x40:'RWX', 0x20:'RX', 0x04:'RW', 0x02:'R', 0x01:'NOACCESS'}.get(new_prot, hex(new_prot))
    print(f'[VirtualProtect] addr=0x{addr:08x} size=0x{size:08x} prot={prot_name}')
    vp_calls.append((addr, size, new_prot))
    
    # After protection changes to RX or R (read-only = after write/decrypt phase),
    # dump the region
    if new_prot in (0x20, 0x02, 0x04) and (addr, size) not in dumped:
        dumped.add((addr, size))
        try:
            mem = se.emu.mem_read(addr, size)
            fname = f'{OUTPUT}region_0x{addr:08x}_{size:08x}_{prot_name}.bin'
            with open(fname, 'wb') as f:
                f.write(bytes(mem))
            print(f'  -> Dumped {size} bytes to {fname}')
        except Exception as e:
            print(f'  -> Dump failed: {e}')
    return 1

se.add_api_hook(hook_virtual_protect, 'kernel32', 'VirtualProtect')

try:
    se.run_module(mod, all_entrypoints=False)
except speakeasy.errors.SpeakeasyError as e:
    print(f'Emulation error: {e}')
except Exception as e:
    print(f'Exception: {e}')

print(f'\nTotal VirtualProtect calls: {len(vp_calls)}')
print(f'Dumped {len(dumped)} regions')
