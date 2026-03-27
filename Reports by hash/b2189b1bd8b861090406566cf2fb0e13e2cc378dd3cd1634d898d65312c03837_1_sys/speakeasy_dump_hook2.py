"""
Improved Speakeasy hook: code hooks at known VP-caller addresses + long timeout
"""
import speakeasy, os

OUTPUT = '/home/remnux/mal/output/Login_decrypted_sections/'
os.makedirs(OUTPUT, exist_ok=True)

# Known VirtualProtect caller PCs from CLI runs  
VP_PCS = {0x9f4f12e, 0x9f4f24c, 0x9f4f460, 0x9f4f548}

se = speakeasy.Speakeasy(config=None)

# Override default timeout
try:
    se.timeout = 600
    se.max_api_count = 0  # no limit
except:
    pass

mod = se.load_module('/home/remnux/mal/Login.exe')
vp_count = [0]
dumps = {}

def on_api(se, call_conv, name, params):
    if 'VirtualProtect' in name:
        addr  = params[0] if isinstance(params[0], int) else int(str(params[0]), 16)
        size  = params[1] if isinstance(params[1], int) else int(str(params[1]), 16)
        prot  = params[2] if isinstance(params[2], int) else int(str(params[2]), 16)
        prot_name = {0x40:'RWX',0x20:'RX',0x04:'RW',0x02:'R'}.get(prot, hex(prot))
        vp_count[0] += 1
        print(f'[{vp_count[0]:3d}] VirtualProtect(0x{addr:08x}, 0x{size:08x}, {prot_name})')
        # Dump region when it transitions to non-writable (i.e., after decryption write phase)
        if prot in (0x20, 0x02, 0x04):
            key = (addr, size)
            if key not in dumps:
                dumps[key] = True
                try:
                    mem = se.get_address_map(addr)
                    data = se.mem_read(addr, size) if hasattr(se, 'mem_read') else None
                    if data is None:
                        import speakeasy.winenv.arch as archmod
                        emu = se.get_emulator()
                        data = bytes(emu.mem_read(addr, size))
                    fname = f'{OUTPUT}0x{addr:08x}_{size:08x}_{prot_name}.bin'
                    with open(fname, 'wb') as f:
                        f.write(data)
                    print(f'    -> Dumped to {fname}')
                except Exception as e:
                    print(f'    -> Dump failed: {e}')
    return 1

se.add_api_hook(on_api, 'kernel32', 'VirtualProtect')

print("Starting emulation (this will take several minutes)...")
try:
    se.run_module(mod, all_entrypoints=False)
except speakeasy.errors.SpeakeasyError as e:
    print(f'SpeakeasyError: {e}')
except Exception as e:
    print(f'Exception: {type(e).__name__}: {e}')
finally:
    print(f'\nTotal VirtualProtect calls: {vp_count[0]}')
    print(f'Dumped {len(dumps)} regions')
