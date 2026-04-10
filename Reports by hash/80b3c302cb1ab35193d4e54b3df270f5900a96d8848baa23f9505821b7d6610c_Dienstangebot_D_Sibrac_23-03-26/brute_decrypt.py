#!/usr/bin/env python3
"""
Brute-force key recovery + RCDATA payload decryption for
Dienstangebot_D_Sibrac_23-03-26.exe (SHA256: 80b3c302...)

Algorithm from sub_140002978 decompilation:
  1. Brute-force uint32 k where:
       hash(k)  == 0x50e36f88
       hash(~k) == 0x4c4db6b2
     where hash() is a modified MurmurHash3 finalizer:
       x ^= x >> 12
       x *= MULT1  (-0x025cad7f mod 2^32)
       x ^= x >> 13
       x *= MULT2  (-0x75d0c077 mod 2^32)
       x ^= x >> 14
  2. Derive decryption key:
       uVar2 = ROTL32(0, 5)*64 accumulating k
       key1  = hash(uVar2)  (same hash function, final full value)
       key2  = 0x9a77bad3
  3. Decrypt RCDATA (sub_140001000 XOR cipher with feedback):
       for i in range(len):
           shift = (i & 3) << 3
           orig   = buf[i]
           buf[i] = orig ^ byte(key1 >> shift) ^ byte(key2 >> shift)
           key2  += orig
"""
import sys
import numpy as np

MULT1    = np.uint32((-0x025cad7f) & 0xFFFFFFFF)
MULT2    = np.uint32((-0x75d0c077) & 0xFFFFFFFF)
TARGET1  = np.uint32(0x50e36f88)
TARGET2  = np.uint32(0x4c4db6b2)
RCDATA   = "/home/remnux/mal/output/dienstangebot_rcdata101.bin"
OUT_FILE = "/home/remnux/mal/output/dienstangebot_payload_decrypted.bin"

def hash_batch(arr: np.ndarray) -> np.ndarray:
    a = arr.astype(np.uint32)
    a = a ^ (a >> np.uint32(12))
    a = (a * MULT1).astype(np.uint32)
    a = a ^ (a >> np.uint32(13))
    a = (a * MULT2).astype(np.uint32)
    a = a ^ (a >> np.uint32(14))
    return a

def hash_scalar(x: int) -> int:
    x = x & 0xFFFFFFFF
    x ^= x >> 12
    x = (x * int(MULT1)) & 0xFFFFFFFF
    x ^= x >> 13
    x = (x * int(MULT2)) & 0xFFFFFFFF
    x ^= x >> 14
    return x

print("[*] Brute-forcing key (targets: 0x50e36f88 / 0x4c4db6b2) ...")
CHUNK = 1 << 22  # 4M per batch
found_key = None
for start in range(0, 1 << 32, CHUNK):
    end = min(start + CHUNK, 1 << 32)
    arr = np.arange(start, end, dtype=np.uint32)
    h1  = hash_batch(arr)
    h2  = hash_batch(~arr)
    mask = (h1 == TARGET1) & (h2 == TARGET2)
    if np.any(mask):
        found_key = int(arr[np.where(mask)[0][0]])
        break
    if (start >> 22) % 64 == 0:
        print(f"    ... {start:#010x}")

if found_key is None:
    print("[!] Key not found in 2^32 range.")
    sys.exit(1)

print(f"[+] Found brute-force key: {found_key:#010x}")

# Derive decryption key: 64x ROTL32(uVar2,5) + found_key
uVar2 = np.uint32(0)
bf    = np.uint32(found_key)
for _ in range(0x40):
    v    = int(uVar2)
    uVar2 = np.uint32(((v << 5) | (v >> 27)) & 0xFFFFFFFF) + bf

key1 = hash_scalar(int(uVar2))
key2 = 0x9a77bad3

print(f"[+] Decryption key1: {key1:#010x}  key2: {key2:#010x}")

# Decrypt RCDATA using sub_140001000 cipher
with open(RCDATA, "rb") as f:
    data = bytearray(f.read())

k1, k2 = key1, key2
for i in range(len(data)):
    shift  = (i & 3) << 3
    orig   = data[i]
    data[i] = orig ^ ((k1 >> shift) & 0xFF) ^ ((k2 >> shift) & 0xFF)
    k2      = (k2 + orig) & 0xFFFFFFFF

with open(OUT_FILE, "wb") as f:
    f.write(data)

print(f"[+] Decrypted payload: {OUT_FILE}  ({len(data)} bytes)")
print(f"[+] Magic bytes: {bytes(data[:16]).hex()}")
if data[:2] == b'MZ':
    print("[!] Decrypted payload is a PE executable (MZ header)")
elif data[1:4] == b'ELF':
    print("[!] Decrypted payload is an ELF binary")
else:
    print(f"[?] Unknown format, first 4 bytes: {data[:4].hex()}")
