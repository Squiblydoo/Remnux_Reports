#!/usr/bin/env python3
"""
APT-Q-27 (Zhong Stealer) — NvBackend.log Payload Decryptor
============================================================
Sample:   detoured.dll / NvBackend.log (nvbackend GCS bucket)
SHA256 (NvBackend.log): 3313f347e83aaf48ea31fb1d49fc37452f48f81d20a1b93009e2e78385ff4bba
Analysis: REMnux static analysis (malcat + speakeasy)
Date:     2026-04-19

--- Encryption (detoured.dll!Detoured, decompiled) ---

For each byte c in NvBackend.log:
    c += 0x52  ('R')
    c += 0x25  ('%')
    c ^= 0x62

Simplified: plaintext = ((ciphertext + 0x77) & 0xFF) ^ 0x62

--- Output structure ---

    [0x000] JMP +0x44b              — bootstrap entry (x86 shellcode)
    [0x005 - 0x44a]                 — shellcode loader code / data
    [0x44b]                         — shellcode main function
    [0x6e3 - end]  160981 bytes     — LZNT1 compressed stage-3 PE
                                      (decompress with RtlDecompressBuffer format 0x102)

Usage:
    python3 nvbackend_log_decrypt.py NvBackend.log [output.bin]

    Default output: NvBackend_decrypted.bin
    Then decompress [0x6e3:] with LZNT1 to recover stage-3 PE.
"""

import sys
import struct
import hashlib


def decrypt(data: bytes) -> bytes:
    return bytes(((b + 0x77) & 0xFF) ^ 0x62 for b in data)


def lznt1_decompress(data: bytes) -> bytes:
    out = bytearray()
    i = 0
    while i < len(data):
        if i + 2 > len(data):
            break
        hdr = int.from_bytes(data[i:i+2], 'little')
        i += 2
        if hdr == 0:
            break
        chunk_size = (hdr & 0x0FFF) + 1
        is_compressed = (hdr >> 15) & 1
        chunk_end = min(i + chunk_size, len(data))
        if not is_compressed:
            out.extend(data[i:chunk_end])
            i = chunk_end
        else:
            chunk_start_out = len(out)
            j = i
            while j < chunk_end:
                if j >= len(data):
                    break
                flags = data[j]; j += 1
                for bit in range(8):
                    if j >= chunk_end:
                        break
                    if flags & (1 << bit):
                        if j + 2 > len(data):
                            break
                        ref = int.from_bytes(data[j:j+2], 'little'); j += 2
                        cur_len = max(len(out) - chunk_start_out, 1)
                        l_mask = 0xF; o_shift = 12
                        while (1 << (o_shift - 1)) >= cur_len and o_shift > 4:
                            l_mask = (l_mask << 1) | 1
                            o_shift -= 1
                        length = (ref & l_mask) + 3
                        offset = (ref >> o_shift) + 1
                        pos = len(out) - offset
                        for _ in range(length):
                            out.append(out[pos] if 0 <= pos < len(out) else 0)
                            pos += 1
                    else:
                        out.append(data[j]); j += 1
            i = chunk_end
    return bytes(out)


def main():
    infile = sys.argv[1] if len(sys.argv) > 1 else 'NvBackend.log'
    outfile = sys.argv[2] if len(sys.argv) > 2 else 'NvBackend_decrypted.bin'
    stage3_file = outfile.replace('.bin', '_stage3.pe')

    with open(infile, 'rb') as f:
        ciphertext = f.read()

    print(f'[*] Input:      {infile} ({len(ciphertext)} bytes)')
    print(f'[*] SHA256:     {hashlib.sha256(ciphertext).hexdigest()}')

    shellcode = decrypt(ciphertext)
    with open(outfile, 'wb') as f:
        f.write(shellcode)
    print(f'[+] Decrypted shellcode → {outfile}')
    print(f'    First bytes: {shellcode[:8].hex()}  (expect: e946040000558bec)')

    # LZNT1-compressed PE starts at offset 0x6e3
    LZNT1_OFFSET = 0x6e3
    compressed_payload = shellcode[LZNT1_OFFSET:]
    print(f'\n[*] LZNT1 payload at offset {hex(LZNT1_OFFSET)} ({len(compressed_payload)} bytes compressed)')

    try:
        stage3 = lznt1_decompress(compressed_payload)
        print(f'[+] Decompressed: {len(stage3)} bytes')
        if stage3[:2] == b'MZ':
            print(f'[+] MZ header confirmed — stage-3 PE recovered')
            print(f'    SHA256: {hashlib.sha256(stage3).hexdigest()}')
            with open(stage3_file, 'wb') as f:
                f.write(stage3)
            print(f'[+] Stage-3 PE → {stage3_file}')
            print(f'\n[!] NOTE: stage-3 uses malformed e_lfanew=0xfffff8 — custom reflective loader only.')
            print(f'    ZS cipher key at offset 0x1ee65:')
            key = stage3[0x1ee65:0x1ee65+32]
            print(f'    {key.hex()}')
        else:
            print(f'[-] No MZ at decompressed output start: {stage3[:8].hex()}')
    except Exception as e:
        print(f'[-] LZNT1 decompression failed: {e}')


if __name__ == '__main__':
    main()
