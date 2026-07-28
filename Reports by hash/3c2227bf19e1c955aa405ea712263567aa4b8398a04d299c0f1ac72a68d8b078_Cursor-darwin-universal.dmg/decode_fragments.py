#!/usr/bin/env python3
"""
Decode Swift small-string literal fragments used by the auralis/Cursor-darwin-universal.dmg
downloader to build its C2 URL at runtime (avoids static string scanning).

Each fragment is a 16-byte Swift _SmallString: 8 content bytes (LE) + up to 7 more content
bytes in the low 7 bytes of the second word, with the top byte of the second word encoding
0xE0 | length. Constants below were read from malcat fn_decompile output of
Endpoint.host / Endpoint.url in the x86-64 slice of Cursor.app/Contents/MacOS/auralis.
"""

def decode_small_string(word: int, discrim: int) -> bytes:
    b1 = word.to_bytes(8, "little")
    b2 = discrim.to_bytes(8, "little")
    allbytes = b1 + b2[:7]
    count = b2[7] & 0x0F
    return allbytes[:count]


HOST_FRAGMENTS = [
    (0x776172, 0xE300000000000000),              # "raw"
    (0x2E, 0xE100000000000000),                   # "."
    (0x627568746967, 0xE600000000000000),         # "github"
    (0x746E6F6372657375, 0xEB00000000746E65),     # "usercontent"
    (0x2E, 0xE100000000000000),                   # "."
    (0x6D6F63, 0xE300000000000000),               # "com"
]

SCHEME_FRAGMENTS = [
    (0x7468, 0xE200000000000000),   # "ht"
    (0x737074, 0xE300000000000000),  # "tps"
]

PATH_FRAGMENTS = [
    (0x6C636968746F676D, 0xEB0000000065766F),  # "mgothiclove"
    (0x61746164627573, 0xE700000000000000),    # "subdata"
    (0x6E69616D, 0xE400000000000000),          # "main"
    (0x632E646F6D627573, 0xEA00000000006766),  # "submod.cfg"
]

if __name__ == "__main__":
    host = b"".join(decode_small_string(w, d) for w, d in HOST_FRAGMENTS)
    scheme = b"".join(decode_small_string(w, d) for w, d in SCHEME_FRAGMENTS)
    path_parts = [decode_small_string(w, d) for w, d in PATH_FRAGMENTS]

    print("scheme:", scheme.decode())
    print("host:  ", host.decode())
    print("path parts:", [p.decode() for p in path_parts])
    print()
    print("Reconstructed URL (path parts joined with '/', GitHub raw convention):")
    print(f"  {scheme.decode()}://{host.decode()}/" + "/".join(p.decode() for p in path_parts))
