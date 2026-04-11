#!/usr/bin/env python3
"""
Trubo.log decryption script
Sample: photo20260411689.com (SHA256: c918dded298b0d76d4ac51f23b391f62a95f58b3fa2488202ecbbc9c7ce8e785)

Algorithm recovered from crashreport.dll::InitBugReport() decompilation.
The function opens Trubo.log, maps it to memory, and decrypts in place:
    decrypted_byte = (encrypted_byte + 0x77) & 0xFF ^ 0x62

Usage:
    python3 Trubo_decrypt.py [input] [output]
    python3 Trubo_decrypt.py Trubo.log Trubo_decrypted.bin
"""

import sys
import os


def decrypt(data: bytes) -> bytes:
    return bytes([(b + 0x77) & 0xFF ^ 0x62 for b in data])


def main():
    input_path = sys.argv[1] if len(sys.argv) > 1 else "Trubo.log"
    output_path = sys.argv[2] if len(sys.argv) > 2 else "Trubo_decrypted.bin"

    if not os.path.exists(input_path):
        print(f"[!] Input file not found: {input_path}", file=sys.stderr)
        sys.exit(1)

    with open(input_path, "rb") as f:
        encrypted = f.read()

    decrypted = decrypt(encrypted)

    # Sanity check: decrypted should start with JMP opcode (0xE9)
    if decrypted[0] == 0xE9:
        jmp_target = int.from_bytes(decrypted[1:5], "little") + 5
        print(f"[+] Decrypted {len(decrypted)} bytes")
        print(f"[+] First byte 0xE9 = JMP, target offset: 0x{jmp_target:x}")
    else:
        print(f"[!] Warning: first byte is 0x{decrypted[0]:02x}, expected 0xE9 (JMP)")

    with open(output_path, "wb") as f:
        f.write(decrypted)

    print(f"[+] Written to: {output_path}")

    # Print SHA256 of decrypted output
    import hashlib
    sha256 = hashlib.sha256(decrypted).hexdigest()
    print(f"[+] SHA256 of decrypted: {sha256}")


if __name__ == "__main__":
    main()
