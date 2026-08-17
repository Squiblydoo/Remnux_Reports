#!/usr/bin/env python3
# XOR-decrypts the embedded sc.msi.enc using key.dat as a repeating XOR key,
# matching Program.<Main>d__2.MoveNext IL: local5[i] ^ local4[i % local4.Length]
import sys

with open("/home/remnux/mal/output/Build.key.dat", "rb") as f:
    key = f.read()

with open("/home/remnux/mal/output/Build.sc.msi.enc", "rb") as f:
    enc = f.read()

out = bytearray(len(enc))
for i in range(len(enc)):
    out[i] = enc[i] ^ key[i % len(key)]

with open("/home/remnux/mal/output/sc_decrypted.msi", "wb") as f:
    f.write(out)

print(f"key length: {len(key)}")
print(f"key hex: {key.hex()}")
print(f"decrypted {len(out)} bytes -> /home/remnux/mal/output/sc_decrypted.msi")
print(f"first 8 bytes: {out[:8].hex()}")
