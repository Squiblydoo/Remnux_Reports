#!/usr/bin/env python3
"""
Static payload decryptor for FakeSDL2 DLL sideloading dropper.
SHA256: 89e52fa31535e46b5f08becbe1c591aff709036554856e6d250a503719081705

Decrypts the embedded 77KB shellcode from the .data section using:
  Layer 1: XOR — 8 passes with key bytes from offset 0x1A0A0 (index 7 down to 0)
  Layer 2: AES-128-CBC — key at offset 0x1C2B0 (16 bytes), IV = first 16 bytes of XOR output

File offsets (physical):
  XOR key:       0x1A0A0  (8 bytes)   VA: 0x1001BAA0
  AES key:       0x1C2B0  (16 bytes)  VA: 0x1001E8B0
  Encrypted blob: 0x1C2C0  (77376 bytes) VA: 0x1001E8C0

Decrypted output: 77360 bytes raw shellcode.
  Offset 0x00:    5-byte x86 GetPC stub (E8 C0 CB 00 00 = CALL +0xCBC5)
  Offset 0xCBC5:  x64 shellcode entry (Heaven's Gate pattern)
"""

import os
import sys
from Cryptodome.Cipher import AES

# --- Configuration ---
XOR_KEY_OFFSET = 0x1A0A0   # file offset of 8-byte XOR key
AES_KEY_OFFSET = 0x1C2B0   # file offset of 16-byte AES key
BLOB_OFFSET    = 0x1C2C0   # file offset of encrypted blob
BLOB_SIZE      = 0x12E40   # 77376 bytes


def decrypt(sample_path: str, output_path: str) -> None:
    with open(sample_path, "rb") as f:
        f.seek(XOR_KEY_OFFSET)
        xor_key = list(f.read(8))

        f.seek(AES_KEY_OFFSET)
        aes_key = f.read(16)

        f.seek(BLOB_OFFSET)
        blob = bytearray(f.read(BLOB_SIZE))

    if len(blob) != BLOB_SIZE:
        raise ValueError(f"Short read: got {len(blob)} bytes, expected {BLOB_SIZE}")

    print(f"XOR key (8 bytes): {bytes(xor_key).hex()}")
    print(f"AES-128 key:       {aes_key.hex()}")
    print(f"Encrypted blob[0:16]: {bytes(blob[:16]).hex()}")

    # Layer 1: XOR — 8 passes, index 7 down to 0
    # Each pass broadcasts the key byte to a 32-bit mask and XORs the full buffer.
    # Net effect: each byte ^= xor_key[7] ^ xor_key[6] ^ ... ^ xor_key[0] = 0xA3
    for i in range(7, -1, -1):
        k = xor_key[i]
        for j in range(0, len(blob), 4):
            blob[j]   ^= k
            blob[j+1] ^= k
            blob[j+2] ^= k
            blob[j+3] ^= k

    print(f"After XOR blob[0:16]:  {bytes(blob[:16]).hex()}")

    # Layer 2: AES-128-CBC
    # IV = first 16 bytes of XOR-decrypted buffer
    # Ciphertext = remainder (blob[16:])
    iv         = bytes(blob[:16])
    ciphertext = bytes(blob[16:])
    print(f"IV:                {iv.hex()}")

    cipher    = AES.new(aes_key, AES.MODE_CBC, iv)
    plaintext = cipher.decrypt(ciphertext)

    magic = plaintext[:4]
    print(f"\nDecrypted {len(plaintext)} bytes")
    print(f"First 4 bytes: {magic.hex()} ({magic})")

    if magic[:2] == b"MZ":
        print("Type: PE/MZ executable")
    elif magic[:4] == b"\x7fELF":
        print("Type: ELF")
    elif magic[:2] == b"PK":
        print("Type: ZIP/archive")
    elif magic[0] == 0xE8:
        print("Type: Raw shellcode (CALL stub at entry)")
    else:
        print("Type: Unknown / raw data")

    os.makedirs(os.path.dirname(os.path.abspath(output_path)), exist_ok=True)
    with open(output_path, "wb") as f:
        f.write(plaintext)
    print(f"Saved: {output_path}")


if __name__ == "__main__":
    if len(sys.argv) < 2:
        script = os.path.basename(__file__)
        print(f"Usage: python3 {script} <sample.bin> [output.bin]")
        print(f"  sample.bin  — the malicious SDL2.dll")
        print(f"  output.bin  — decrypted stage 2 shellcode (default: stage2_decrypted.bin)")
        sys.exit(1)

    sample = sys.argv[1]
    output = sys.argv[2] if len(sys.argv) > 2 else "stage2_decrypted.bin"
    decrypt(sample, output)
