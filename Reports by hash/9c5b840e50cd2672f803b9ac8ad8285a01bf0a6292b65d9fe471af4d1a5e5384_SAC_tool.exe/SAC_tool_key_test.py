#!/usr/bin/env python3
"""Test whether the known ZhongStealer REGISTER_KEY/MODULE_KEY + cipher1/cipher2
still decrypt the new nikeupdat-wave payloads (oihtq.uqv, ousctr.gtk, vcnfq.uqv).
"""
import sys
import zlib

REGISTER_KEY = bytes.fromhex(
    "8A913610E905C3DD1F657811EA3B1933"
    "471B230F88E1C155616099A03AB0ABC0"
)
MODULE_KEY = bytes.fromhex(
    "2031A71C399563ADAF1572E10ABB3953"
    "87EB132208A001C5E140496D7A3E0B26"
)


def cipher1(data: bytes, key: bytes, decrypt: bool) -> bytes:
    result = bytearray(data)
    sign = -1 if decrypt else 1
    for i in range(len(result)):
        k = key[i % 32]
        op = i % 8
        if op == 0:
            result[i] ^= k
        elif op == 1:
            result[i] = (result[i] + sign * (k >> 1)) & 0xFF
        elif op == 2:
            result[i] = (result[i] - sign * (k * 4)) & 0xFF
        elif op == 3:
            result[i] = (result[i] + sign * (k << 2)) & 0xFF
    return bytes(result)


def cipher2_at_rest_decrypt(data: bytes, key: bytes) -> bytes:
    buf = bytearray(data)
    for i in range(len(buf)):
        op = i % 6
        k = key[i & 0x1F]
        if op == 0:
            buf[i] = (buf[i] - (k >> 2)) & 0xFF
        elif op == 1:
            buf[i] = (buf[i] - (k * 2)) & 0xFF
        elif op == 2:
            if i > 0:
                buf[i] ^= (k % i + k * 4 + i) & 0xFF
        elif op == 3:
            buf[i] = (buf[i] + (k * 2)) & 0xFF
    return bytes(buf)


def score(data: bytes, label: str, fname: str):
    hits = []
    if data[:2] == b"MZ":
        hits.append("MZ-header")
    if b"UPX!" in data[:4096]:
        hits.append("UPX-sig")
    if data[:2] == b"\x78\x9c" or data[:2] == b"\x78\xda" or data[:2] == b"\x78\x01":
        hits.append("zlib-magic")
    if data[:4] == b"PK\x03\x04":
        hits.append("ZIP-header")
    # printable-ratio heuristic
    printable = sum(1 for b in data[:4096] if 0x20 <= b < 0x7f or b in (9, 10, 13))
    ratio = printable / min(len(data), 4096)
    if hits or ratio > 0.85:
        print(f"[{fname}] {label}: hits={hits} printable_ratio={ratio:.2f} first16={data[:16].hex()}")
        return True
    return False


def try_zlib(data: bytes):
    for wbits in (15, -15, 47):
        try:
            return zlib.decompress(data, wbits=wbits)
        except zlib.error:
            continue
    return None


files = {
    "oihtq.uqv": "/home/remnux/mal/nikeupdat_oihtq.uqv",
    "ousctr.gtk": "/home/remnux/mal/nikeupdat_ousctr.gtk",
    "vcnfq.uqv": "/home/remnux/mal/nikeupdat_vcnfq.uqv",
}

for name, path in files.items():
    data = open(path, "rb").read()
    print(f"=== {name} ({len(data)} bytes) — first 16 raw: {data[:16].hex()} ===")
    found_any = False

    # cipher2/MODULE_KEY at-rest, no LZNT1, no zlib
    d = cipher2_at_rest_decrypt(data, MODULE_KEY)
    found_any |= score(d, "cipher2/MODULE_KEY raw", name)
    dz = try_zlib(d)
    if dz:
        found_any |= score(dz, "cipher2/MODULE_KEY + zlib", name)

    # cipher1/REGISTER_KEY, no zlib / with zlib
    d = cipher1(data, REGISTER_KEY, decrypt=True)
    found_any |= score(d, "cipher1/REGISTER_KEY raw", name)
    dz = try_zlib(d)
    if dz:
        found_any |= score(dz, "cipher1/REGISTER_KEY + zlib", name)

    # cross keys (in case roles swapped)
    d = cipher2_at_rest_decrypt(data, REGISTER_KEY)
    found_any |= score(d, "cipher2/REGISTER_KEY raw", name)
    d = cipher1(data, MODULE_KEY, decrypt=True)
    found_any |= score(d, "cipher1/MODULE_KEY raw", name)

    # plain zlib (no cipher at all)
    dz = try_zlib(data)
    if dz:
        found_any |= score(dz, "plain zlib (no cipher)", name)

    if not found_any:
        print(f"[{name}] no hits with known keys/ciphers")
    print()
