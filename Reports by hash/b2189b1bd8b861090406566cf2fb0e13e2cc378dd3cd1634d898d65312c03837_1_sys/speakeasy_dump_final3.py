import speakeasy, os, json

OUTPUT = '/home/remnux/mal/output/Login_decrypted_sections/'
os.makedirs(OUTPUT, exist_ok=True)

config_path = '/opt/speakeasy/lib/python3.12/site-packages/speakeasy/configs/win10_full_analysis.json'
with open(config_path) as f:
    config = json.load(f)
config['timeout'] = 300
config['command_line'] = 'Login.exe'  # required by FileManager

se = speakeasy.Speakeasy(config=config, argv=['Login.exe'])
mod = se.load_module('/home/remnux/mal/Login.exe')

vp_count = [0]
dumps = {}

def on_vp(se_obj, call_conv, api_name, params):
    vp_count[0] += 1
    try:
        addr = params[0] if isinstance(params[0], int) else 0
        size = params[1] if isinstance(params[1], int) else 0
        prot = params[2] if isinstance(params[2], int) else 0
    except:
        return 1
    prot_name = {0x40:'RWX',0x20:'RX',0x04:'RW',0x02:'R'}.get(prot, hex(prot))
    print(f'[{vp_count[0]:3d}] 0x{addr:08x} {size//1024}KB -> {prot_name}', flush=True)
    if prot in (0x20, 0x02, 0x04):
        key = (addr, size)
        if key not in dumps:
            dumps[key] = True
            try:
                data = bytes(se_obj.emu.uc.mem_read(addr, size))
                fname = f'{OUTPUT}0x{addr:08x}_{prot_name}.bin'
                with open(fname, 'wb') as fw:
                    fw.write(data)
                print(f'     -> Saved {len(data):,}B: {os.path.basename(fname)}', flush=True)
            except Exception as e:
                print(f'     -> Fail: {e}', flush=True)
    return 1

se.add_api_hook(on_vp, 'kernel32', 'VirtualProtect')
print('Starting (win10_full, 300s, cmdline set)...', flush=True)
try:
    se.run_module(mod, all_entrypoints=False)
except speakeasy.errors.SpeakeasyError as e:
    print(f'Error: {e}')
except Exception as e:
    import traceback; traceback.print_exc()
finally:
    print(f'VP: {vp_count[0]}, Dumps: {len(dumps)}')
