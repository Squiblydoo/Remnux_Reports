import sys

# 32-byte repeating XOR key extracted from runInjection:
# 0x9078563412efcdab -> ab cd ef 12 34 56 78 90 (little-endian)
# 0xbebafecaefbeadde -> de ad be ef ca fe ba be
# 0xf0debc9a78563412 -> 12 34 56 78 9a bc de f0
# 0x78695a4b3c2d1e0f -> 0f 1e 2d 3c 4b 5a 69 78
key = bytes([
    0xab, 0xcd, 0xef, 0x12, 0x34, 0x56, 0x78, 0x90,
    0xde, 0xad, 0xbe, 0xef, 0xca, 0xfe, 0xba, 0xbe,
    0x12, 0x34, 0x56, 0x78, 0x9a, 0xbc, 0xde, 0xf0,
    0x0f, 0x1e, 0x2d, 0x3c, 0x4b, 0x5a, 0x69, 0x78,
])

with open("/home/remnux/mal/security_audit_20260514.log", "rb") as f:
    data = f.read()

decrypted = bytes(b ^ key[i % 32] for i, b in enumerate(data))

with open("/home/remnux/mal/output/security_audit_decrypted.bin", "wb") as f:
    f.write(decrypted)

print(f"Input size: {len(data)} bytes")
print(f"First 32 bytes (hex): {decrypted[:32].hex()}")
print(f"First 16 bytes (ascii): {decrypted[:16]}")
# Check for PE magic
if decrypted[:2] == b'MZ':
    print(">>> RESULT: Decrypted file begins with MZ - this is a PE executable!")
elif decrypted[:4] == b'\x7fELF':
    print(">>> RESULT: Decrypted file begins with ELF!")
elif decrypted[0] == 0xfc or decrypted[0] == 0xe8 or decrypted[0] == 0x55:
    print(f">>> RESULT: Possible shellcode (first byte: 0x{decrypted[0]:02x})")
else:
    print(f">>> RESULT: Unknown format, first byte: 0x{decrypted[0]:02x}")
