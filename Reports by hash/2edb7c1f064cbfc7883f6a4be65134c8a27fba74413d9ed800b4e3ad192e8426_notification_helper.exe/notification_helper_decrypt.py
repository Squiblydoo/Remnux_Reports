#!/usr/bin/env python3
"""
notification_helper.exe (SHA256 2edb7c1f...) companion-payload decryptor.

- erigfj.gtk : decrypted DIRECTLY BY notification_helper.exe itself
              (added loader stub sub_41f420/sub_41f2a0), new cipher
              for this wave: out[i] = (in[i] + 0x70) ^ 0x88
              -> x86 shellcode w/ hash-based reflective PE-loader stub
- oihtq.uqv  : XOR w/ unchanged 32-byte MODULE_KEY (same as the
              2026-07-21 SAC_tool.exe/nikeupdat wave) -> plugin32.dll,
              byte-identical to that wave's SHA256 813c4a2a...
- vcnfq.uqv  : XOR w/ unchanged 16-byte key (same as nikeupdat wave)
              -> updated loader/core DLL (new hash vs. that wave)
"""
import sys

MODULE_KEY = bytes.fromhex(
    "2031A71C399563ADAF1572E10ABB3953"
    "87EB132208A001C5E140496D7A3E0B26"
)
VCNFQ_KEY = bytes.fromhex("33C83BCF7507B94FE640BBEB1085CE75")


def xor_decrypt(data: bytes, key: bytes) -> bytes:
    return bytes(b ^ key[i % len(key)] for i, b in enumerate(data))


def erigfj_decrypt(data: bytes) -> bytes:
    return bytes(((b + 0x70) & 0xFF) ^ 0x88 for b in data)


TARGETS = [
    ("oihtq.uqv", xor_decrypt, MODULE_KEY, "oihtq_decrypted_plugin32.bin"),
    ("vcnfq.uqv", xor_decrypt, VCNFQ_KEY, "vcnfq_decrypted_loader.bin"),
    ("erigfj.gtk", erigfj_decrypt, None, "erigfj_decrypted_shellcode.bin"),
]


def main():
    base = sys.argv[1] if len(sys.argv) > 1 else "."
    for fname, fn, key, out_name in TARGETS:
        try:
            data = open(f"{base}/{fname}", "rb").read()
        except FileNotFoundError:
            print(f"{fname}: not found, skipping")
            continue
        out = fn(data, key) if key is not None else fn(data)
        open(out_name, "wb").write(out)
        print(f"{fname} ({len(data)}B) -> {out_name}  MZ={out[:2] == b'MZ'}  first16={out[:16].hex()}")


if __name__ == "__main__":
    sys.exit(main())
