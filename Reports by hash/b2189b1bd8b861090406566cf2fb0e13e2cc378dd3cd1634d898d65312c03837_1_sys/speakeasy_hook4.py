"""
Speakeasy hook using win10_full_analysis config to match CLI behavior
"""
import speakeasy, os, json, pathlib, sys

OUTPUT = '/home/remnux/mal/output/Login_decrypted_sections/'
os.makedirs(OUTPUT, exist_ok=True)

# Use same config as CLI
config_path = '/opt/speakeasy/lib/python3.12/site-packages/speakeasy/configs/win10_full_analysis.json'
with open(config_path) as f:
    config = json.load(f)

# Set timeout in config
config['emu_timeout'] = 300

se = speakeasy.Speakeasy(config=config)
mod = se.load_module('/home/remnux/mal/Login.exe')

vp_count = [0]
dumps = {}

def on_vp(se_obj, call_conv, api_name, params):
    vp_count[0] += 1
    try:
        addr = params[0] if isinstance(params[0], int) else 0
        size = params[1] if isinstance(params[1], int) else 0
        prot = params[2] if isinstance(params[2], int) else 0
    except Exception as e:
        print(f'[VP #{vp_count[0]}] param error: {e} params={params}')
        return 1
    
    prot_name = {0x40:'RWX',0x20:'RX',0x04:'RW',0x02:'R',0x01:'NOACCESS'}.get(prot, hex(prot))
    print(f'[VP #{vp_count[0]:3d}] addr=0x{addr:08x} size=0x{size:08x} ({size//1024}KB) prot={prot_name}', flush=True)
    
    if prot in (0x20, 0x02, 0x04):
        key = (addr, size)
        if key not in dumps:
            dumps[key] = True
            try:
                data = bytes(se_obj.emu.uc.mem_read(addr, size))
                fname = f'{OUTPUT}0x{addr:08x}_{size:08x}_{prot_name}.bin'
                with open(fname, 'wb') as f:
                    f.write(data)
                print(f'       -> Dumped {len(data):,} bytes', flush=True)
            except Exception as e:
                print(f'       -> Failed: {e}', flush=True)
    return 1

se.add_api_hook(on_vp, 'kernel32', 'VirtualProtect')
print('Hook registered. Config: win10_full_analysis', flush=True)
print('Starting emulation...', flush=True)

try:
    se.run_module(mod, all_entrypoints=False)
except speakeasy.errors.SpeakeasyError as e:
    print(f'SpeakeasyError: {e}')
except Exception as e:
    import traceback
    print(f'Exception: {type(e).__name__}: {e}')
    traceback.print_exc()
finally:
    print(f'\nDone. VP calls: {vp_count[0]}, Dumped: {len(dumps)} regions')
