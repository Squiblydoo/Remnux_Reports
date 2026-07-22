#!/usr/bin/env python3
"""
APT-Q-27 / ZhongStealer — nikeupdat-wave at-rest payload decryptor.

Construction change vs. legacy waves (documented 2026-07-21, SAC_tool.exe):
  OLD: MODULE_KEY (32B) + cipher2 (6-op mixed ADD/SUB/positional-XOR, i%6)
  NEW: plain single-pass repeating-key XOR: out[i] = data[i] ^ key[i % len(key)]

Keys are no longer uniform across modules in a wave:
  - oihtq.uqv -> plugin32.dll   : MODULE_KEY (32B, unchanged from legacy waves)
  - vcnfq.uqv -> core/loader DLL: new module-specific 16B key, recovered via
                                   crib-drag against oihtq's decrypted header
  - ousctr.gtk                  : UNRESOLVED — neither key works under plain
                                   XOR; likely a 4th distinct key or non-PE
                                   format (entropy 7.44 vs 7.86/7.71 bits/byte
                                   for the two solved files, anomalous 0x7D
                                   byte frequency ~4.8%)

REGISTER_KEY + the original 8-op cipher1 (live WebSocket traffic) are
UNCHANGED — verified separately against a PCAP-captured REGISTER frame.
"""
import sys

MODULE_KEY = bytes.fromhex(
    "2031A71C399563ADAF1572E10ABB3953"
    "87EB132208A001C5E140496D7A3E0B26"
)
VCNFQ_KEY = bytes.fromhex("33C83BCF7507B94FE640BBEB1085CE75")


def xor_decrypt(data: bytes, key: bytes) -> bytes:
    return bytes(b ^ key[i % len(key)] for i, b in enumerate(data))


TARGETS = [
    ("/home/remnux/mal/nikeupdat_oihtq.uqv", MODULE_KEY,
     "/home/remnux/mal/output/oihtq_decrypted.bin"),
    ("/home/remnux/mal/nikeupdat_vcnfq.uqv", VCNFQ_KEY,
     "/home/remnux/mal/output/vcnfq_decrypted.bin"),
]


def main():
    for src, key, dst in TARGETS:
        data = open(src, "rb").read()
        out = xor_decrypt(data, key)
        with open(dst, "wb") as f:
            f.write(out)
        mz = out[:2] == b"MZ"
        print(f"{src} ({len(data)}B) -> {dst}")
        print(f"  key={key.hex()} ({len(key)}B)  MZ-header={mz}  first16={out[:16].hex()}")

    # ousctr.gtk: document the negative result rather than silently omitting it
    ousctr = open("/home/remnux/mal/nikeupdat_ousctr.gtk", "rb").read()
    for label, key in (("MODULE_KEY", MODULE_KEY), ("VCNFQ_KEY", VCNFQ_KEY)):
        out = xor_decrypt(ousctr, key)
        print(f"ousctr.gtk plain-XOR/{label}: MZ-header={out[:2] == b'MZ'} first16={out[:16].hex()}  (expected: no hit)")
    print("ousctr.gtk (433576B): UNRESOLVED under this construction — see module docstring.")


if __name__ == "__main__":
    sys.exit(main())
