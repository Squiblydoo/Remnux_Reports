#!/usr/bin/env python3
"""
Decode the payload from 14bvp.exe .data section.

Section layout (from PE headers):
  .data: VA=0x28000, RawOff=0x27200, RawSize=0x2600
  .rdata: VA=0x1a000, RawOff=0x19200
  image_base = 0x140000000

malcat EA  -> file_offset:
  .text  (EA 1024-103424):   file_off = EA          (no gap)
  .rdata (EA 103424-160768): file_off = EA - 512
  .data  (EA 160768+):       file_off = EA - 512
"""

import struct, sys

SAMPLE = "/home/remnux/mal/14bvp.exe"

# section layout from PE headers
DATA_VA      = 0x28000
DATA_RAWOFF  = 0x27200
IMAGE_BASE   = 0x140000000

def va_to_fileoff(va):
    rva = va - IMAGE_BASE
    # .data section: RVA 0x28000, RawOff 0x27200
    return DATA_RAWOFF + (rva - DATA_VA)

# VA of the start of the decode source buffer
VA_START = 0x1400298c8
FO_START = va_to_fileoff(VA_START)
print(f"Source VA: 0x{VA_START:x}  File offset: 0x{FO_START:x} ({FO_START})")

# Verify
with open(SAMPLE, "rb") as f:
    f.seek(FO_START)
    check = f.read(4)
    print(f"Bytes at source start: {check.hex()}")

# Loop: 1530 iters, pcVar2 decrements by 4 each iter
# reads pcVar2[0] and pcVar2[-2] (1 byte each)
# Total source range: FO_START down to FO_START - 4*(1529) - 2

ITERS = 1530
FO_MIN = FO_START - 4*(ITERS-1) - 2
FO_MAX = FO_START + 1

with open(SAMPLE, "rb") as f:
    f.seek(FO_MIN)
    raw = f.read(FO_MAX - FO_MIN)

print(f"Read {len(raw)} bytes from file offset 0x{FO_MIN:x}-0x{FO_MAX:x}")

output = bytearray(3061)

# counters (all 8-bit signed arithmetic via wrap)
def s8(v):
    v = v & 0xFF
    return v - 256 if v >= 128 else v

cVar8 = 0
cVar4 = -1
uVar6 = 0  # uint16 or uint32 (treated as unsigned)

fo_cursor = FO_START  # current pcVar2 file offset
iVar7 = 0

for i in range(ITERS + 1):
    cVar1 = ((uVar6 // 5) & 0xFE) * 5  # 8-bit result
    
    # raw index for current cursor
    idx0 = fo_cursor - FO_MIN
    idx_minus2 = fo_cursor - 2 - FO_MIN
    
    b0 = raw[idx0]         if 0 <= idx0 < len(raw) else 0
    b_m2 = raw[idx_minus2] if 0 <= idx_minus2 < len(raw) else 0
    
    output[iVar7] = s8(cVar8 + cVar1 + b0) & 0xFF
    
    if iVar7 == 0xbf4:
        break
    
    output[iVar7 + 1] = s8(cVar1 + cVar4 + b_m2) & 0xFF
    
    iVar7 += 2
    fo_cursor -= 4
    cVar4 = s8(cVar4 - 2)
    uVar6 = (uVar6 + 2) & 0xFFFFFFFF
    cVar8 = s8(cVar8 - 2)

result = bytes(output[:0xbf5])
out_path = "/home/remnux/mal/output/14bvp_payload.bin"
with open(out_path, "wb") as f:
    f.write(result)

print(f"\nDecoded {len(result)} bytes -> {out_path}")
print(f"First 64 bytes: {result[:64].hex()}")
print(f"Last  16 bytes: {result[-16:].hex()}")

# Check magic bytes
if result[:2] == b'MZ':
    print("\n*** PE MAGIC (MZ) at offset 0 ***")
    pe_offset = struct.unpack('<I', result[60:64])[0] if len(result) > 64 else 0
    print(f"  e_lfanew = 0x{pe_offset:x}")
elif b'MZ' in result:
    pos = result.index(b'MZ')
    print(f"\n*** MZ found at offset {pos} (0x{pos:x}) ***")
else:
    # check for shellcode-like patterns
    print("\nNo PE magic. Checking for shellcode patterns...")
    # x64 shellcode often starts with 0x48, 0x55, 0x48 89 e5, etc.
    for pat, name in [(b'\x48\x89', 'x64 prologue'), (b'\x55\x48', 'push rbp'), (b'\xfc\x48', 'cld+'), (b'\xe8\x00\x00\x00\x00', 'call+0 shellcode')]:
        if pat in result[:200]:
            pos = result.index(pat)
            print(f"  Found {name} at offset {pos}: {result[pos:pos+8].hex()}")
    # print full hex for manual inspection
    print(f"\nFull output (hex):")
    for i in range(0, min(128, len(result)), 16):
        print(f"  {i:04x}: {result[i:i+16].hex()} | {result[i:i+16]}")
