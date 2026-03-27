"""
Speakeasy hook: correct API hook signature to dump decrypted sections
"""
import speakeasy, os, sys

OUTPUT = '/home/remnux/mal/output/Login_decrypted_sections/'
os.makedirs(OUTPUT, exist_ok=True)

se = speakeasy.Speakeasy(config=None)
mod = se.load_module('/home/remnux/mal/Login.exe')

vp_count = [0]
dumps = {}

def on_vp(se_obj, call_conv, api_name, params):
    """Hook for kernel32.VirtualProtect"""
    vp_count[0] += 1
    try:
        addr  = params[0] if isinstance(params[0], int) else 0
        size  = params[1] if isinstance(params[1], int) else 0
        prot  = params[2] if isinstance(params[2], int) else 0
    except:
        print(f'[VP #{vp_count[0]}] Could not parse params: {params}')
        return 1

    prot_name = {0x40:'RWX',0x20:'RX',0x04:'RW',0x02:'R'}.get(prot, hex(prot))
    print(f'[VP #{vp_count[0]:3d}] addr=0x{addr:08x} size=0x{size:08x} prot={prot_name}', flush=True)
    
    # Dump when set to read-only or execute-read (after decryption)
    if prot in (0x20, 0x02, 0x04, 0x01):
        key = (addr, size)
        if key not in dumps:
            dumps[key] = True
            try:
                emu = se_obj.emu
                data = bytes(emu.mem_read(addr, size))
                fname = f'{OUTPUT}0x{addr:08x}_{size:08x}_{prot_name}.bin'
                with open(fname, 'wb') as f:
                    f.write(data)
                print(f'       -> Dumped {len(data):,} bytes to {fname}', flush=True)
            except Exception as e:
                # Try alternate read method
                try:
                    data = bytes(se_obj.emu.uc.mem_read(addr, size))
                    fname = f'{OUTPUT}0x{addr:08x}_{size:08x}_{prot_name}.bin'
                    with open(fname, 'wb') as f:
                        f.write(data)
                    print(f'       -> Dumped {len(data):,} bytes (uc) to {fname}', flush=True)
                except Exception as e2:
                    print(f'       -> Dump FAILED: {e} / {e2}', flush=True)
    return 1

# Register hook - try both approaches
try:
    se.add_api_hook(on_vp, 'kernel32', 'VirtualProtect')
    print("Registered API hook")
except Exception as e:
    print(f"API hook registration failed: {e}", flush=True)

print("Starting emulation...", flush=True)
try:
    se.run_module(mod, all_entrypoints=False)
except speakeasy.errors.SpeakeasyError as e:
    print(f'SpeakeasyError: {e}')
except Exception as e:
    print(f'Exception: {type(e).__name__}: {e}')
finally:
    print(f'\nTotal VirtualProtect calls: {vp_count[0]}')
    print(f'Dumped {len(dumps)} regions')
