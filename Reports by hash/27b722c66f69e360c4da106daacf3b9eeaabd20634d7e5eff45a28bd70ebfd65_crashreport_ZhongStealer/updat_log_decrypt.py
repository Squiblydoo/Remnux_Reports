#!/usr/bin/env python3
"""
Decryption script for updat.log (ZhongStealer stage-3 payload)
Key recovered from crashreport.dll::InitBugReport decompilation
Formula: plaintext[i] = (encrypted[i] + 0x77) ^ 0x62
"""
import sys

def decrypt(in_path, out_path):
    data = open(in_path, "rb").read()
    decrypted = bytes(((b + 0x77) ^ 0x62) & 0xFF for b in data)
    open(out_path, "wb").write(decrypted)
    print(f"Decrypted {len(data)} bytes → {out_path}")
    print(f"First 16 bytes: {decrypted[:16].hex()}")

if __name__ == "__main__":
    inp = sys.argv[1] if len(sys.argv) > 1 else "updat.log"
    out = sys.argv[2] if len(sys.argv) > 2 else inp + ".decrypted.bin"
    decrypt(inp, out)
