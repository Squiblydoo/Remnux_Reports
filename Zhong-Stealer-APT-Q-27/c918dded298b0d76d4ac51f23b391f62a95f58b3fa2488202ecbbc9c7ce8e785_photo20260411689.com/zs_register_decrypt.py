#!/usr/bin/env python3
"""
Zhong Stealer (APT-Q-27 / Golden Eye Dog) — REGISTER Frame Decryptor
=======================================================================
Sample:   windui.dll / stage2_upx_unpacked_manually.bin
SHA256:   81e276aaa3eb9b3f595663c316b3c6414cc3dde5e6cc3a82856b7276acabb7de
Analysis: REMnux static analysis of stage-2 PE (malcat + speakeasy)
Date:     2026-04-12

--- Protocol ---

WebSocket frame → unmask → 203-byte application payload:

  [0:4]   compressed_total_len  = 0xCB (203, LE uint32)
  [4:8]   orig_plaintext_size   = 0x204 (516, LE uint32) ← NOT "message type"
  [8:12]  session_id            = 0x94E (2382, LE uint32)
  [12:203] encrypted_payload   = 191 bytes (custom cipher, zlib compressed)

--- Encryption (sub_10001ade, EA 3806) ---

  Key (32 bytes, hardcoded at VA 0x10041060 / EA 263264):
  8A913610E905C3DD1F657811EA3B1933471B230F88E1C155616099A03AB0ABC0

  For each byte i (0-indexed):
    op = i % 8
    k  = key[i % 32]
    op0: byte ^= k
    op1: byte = (byte + (k>>1)) & 0xFF
    op2: byte = (byte - (k*4)) & 0xFF   [subtract]
    op3: byte = (byte + (k<<2)) & 0xFF
    op4-op7: no change (identity)

  Applied AFTER zlib compression. Decryption reverses each op.

--- Compression ---

  zlib compress (Python wbits=15) applied to raw 516-byte fingerprint
  BEFORE encryption. Decompress with: zlib.decompress(decrypted, wbits=15)

--- Plaintext Layout (516 bytes / 0x204) ---

  [0:284]   OSVERSIONINFOEX (filled by RtlGetVersion)
              dwOSVersionInfoSize = 284
              dwMajorVersion / dwMinorVersion / dwBuildNumber / dwPlatformId
              szCSDVersion[128] = empty string + stack residue in trailing bytes
  [284:]    Additional system fingerprint fields:
              - 2-byte prefix (type/flag: 0x81, 0xFF)
              - C-string timestamp "YYYY-MM-DD HH:MM:SS"
              - Additional DWORDs (CPU/RAM/disk metrics, network data)
  Stack residue in szCSDVersion buffer often contains:
    - MAC address(es) from GetIfTable call
    - IP address and hostname from earlier collectors

Usage:
  python3 zs_register_decrypt.py [hex_file]

  Without args: uses the known AnyRun PCAP payload as test vector.
  With args: reads a hex file containing the 191-byte encrypted payload.
"""

import zlib
import struct
import sys
import binascii

# 32-byte hardcoded encryption key (VA 0x10041060 in stage-2 DLL)
KEY = bytes.fromhex(
    "8A913610E905C3DD1F657811EA3B1933"
    "471B230F88E1C155616099A03AB0ABC0"
)

# Known test vector: AnyRun PCAP frame (collected 2026-04-11, validated)
# Source: 44182f44-35a2-46c3-b8aa-a96766bcdcb0.pcap frame 3697
# Frame header: total_len=0xCB, orig_size=0x204, session_id=0x94E
KNOWN_INNER_191_HEX = (
    "f2e4bba16460e0629fb2747306062686c612ae5be59837159f59375dc35826c6"
    "4908e846143bb0229afde579d83bc43f07e43f19c7c7cbc7e4584f8a819e79c1"
    "4608188b9c8161e71098c2380801b1e8f80c73818074e37f4233bfb35d03135d"
    "c98b2d83632b5333344563cac710850d67e73c981baa8f097ba49b9919fe8168"
    "1a462f3d8f191891d3a020a607907f8021d9d5b60c17970503353c1d60ff0331"
    "4e47a70a40eaedc13550530c03c9fd4043afe9317589f20bd0309c353530bf"
)


def decrypt_payload(data: bytes, key: bytes = KEY) -> bytes:
    """
    Reverse the sub_10001ade encryption.
    Decrypts 191 bytes → zlib-compressed plaintext.
    """
    result = bytearray(data)
    for i in range(len(result)):
        op = i % 8
        k = key[i % 32]
        if op == 0:
            result[i] ^= k
        elif op == 1:
            result[i] = (result[i] - (k >> 1)) & 0xFF
        elif op == 2:
            result[i] = (result[i] + k * 4) & 0xFF     # reverse of subtract
        elif op == 3:
            result[i] = (result[i] - (k << 2)) & 0xFF
        # ops 4-7: identity (no change)
    return bytes(result)


def parse_osversioninfoex(data: bytes) -> dict:
    """Parse OSVERSIONINFOEX structure (first 284 bytes of plaintext)."""
    if len(data) < 20:
        return {"error": "truncated"}
    sz, major, minor, build, platform = struct.unpack_from('<IIIII', data, 0)
    csd_bytes = data[20:276] if len(data) >= 276 else b''
    # CSD version is a null-terminated wide string; rest is stack residue
    csd_null = csd_bytes.find(b'\x00\x00')
    csd = ""
    if csd_null >= 0:
        try:
            csd = csd_bytes[:csd_null+1].decode('utf-16-le', errors='replace').rstrip('\x00')
        except Exception:
            pass
    sp_major = sp_minor = suite = ptype = 0
    if len(data) >= 284:
        sp_major, sp_minor, suite = struct.unpack_from('<HHH', data, 276)
        ptype = data[282]

    product_types = {1: "VER_NT_WORKSTATION", 2: "VER_NT_DOMAIN_CONTROLLER", 3: "VER_NT_SERVER"}
    win_versions = {
        19041: "Windows 10 2004", 19042: "Windows 10 20H2",
        19043: "Windows 10 21H1", 19044: "Windows 10 21H2 alt",
        19045: "Windows 10 21H2", 22000: "Windows 11 21H2",
        22621: "Windows 11 22H2", 22631: "Windows 11 23H2",
    }
    return {
        "size": sz,
        "major": major, "minor": minor, "build": build,
        "platform": platform,
        "csd_version": csd or "(empty)",
        "sp_major": sp_major, "sp_minor": sp_minor,
        "suite_mask": suite,
        "product_type": f"{ptype} ({product_types.get(ptype, 'unknown')})",
        "os_name": win_versions.get(build, f"Build {build}"),
        "os_string": f"Windows {major}.{minor} Build {build}",
    }


def find_stack_artifacts(plaintext: bytes) -> dict:
    """
    Extract MAC addresses, IPs, and hostname from szCSDVersion stack residue.
    These are left in the 256-byte CSD buffer (bytes 20-275) by prior function calls
    (GetIfTable for MAC, inet_addr for IP, etc.) before RtlGetVersion overwrites the header.
    """
    csd_area = plaintext[20:276]
    artifacts = {"macs": [], "ips": [], "hostname": None}

    # IP detection in full plaintext
    for i in range(0, len(plaintext) - 3):
        b = plaintext[i:i+4]
        if b[0] in (10, 172, 192) or (b[0] == 169 and b[1] == 254):
            ip = f"{b[0]}.{b[1]}.{b[2]}.{b[3]}"
            if ip not in artifacts["ips"]:
                artifacts["ips"].append((i, ip))

    # Hostname: ASCII string after IP in the CSD region / post-OSVER area
    host_region = plaintext[150:220]
    i = 0
    while i < len(host_region):
        if 0x20 <= host_region[i] <= 0x7e:
            end = i
            while end < len(host_region) and 0x20 <= host_region[end] <= 0x7e:
                end += 1
            candidate = host_region[i:end].decode('ascii')
            if len(candidate) >= 8 and candidate.replace('-', '').replace('_', '').isalnum():
                artifacts["hostname"] = (150 + i, candidate)
                break
        i += 1

    # MAC: look for 6-byte non-trivial sequences in CSD area
    for i in range(0, len(csd_area) - 5):
        chunk = csd_area[i:i+6]
        if (all(b != 0 for b in chunk)
                and len(set(chunk)) >= 4
                and sum(1 for b in chunk if 0x80 <= b <= 0xff) >= 1):
            mac = ':'.join(f'{b:02x}' for b in chunk)
            if not any(m[1] == mac for m in artifacts["macs"]):
                artifacts["macs"].append((20 + i, mac))
                break  # first candidate only for now

    return artifacts


def parse_post_osver(plaintext: bytes) -> dict:
    """Parse fields after OSVERSIONINFOEX (offset 284+)."""
    if len(plaintext) < 286:
        return {}
    result = {}

    # 2-byte prefix at 284-285 (likely a type/flag field)
    result["prefix_bytes"] = plaintext[284:286].hex()

    # C-string timestamp at 286
    ts_end = plaintext.find(b'\x00', 286)
    if ts_end > 286:
        try:
            result["timestamp"] = plaintext[286:ts_end].decode('ascii')
            result["timestamp_offset"] = 286
            result["post_ts_offset"] = ts_end + 1
        except Exception:
            result["timestamp"] = "(parse error)"
    else:
        result["timestamp"] = "(not found)"

    # Remaining DWORDs after timestamp
    off = result.get("post_ts_offset", 306)
    dwords = []
    for i in range(off, min(len(plaintext), off + 80), 4):
        if i + 4 <= len(plaintext):
            v = struct.unpack_from('<I', plaintext, i)[0]
            if v != 0:
                dwords.append((i, v))
    result["nonzero_dwords"] = dwords

    return result


def decrypt_register(encrypted_191: bytes) -> dict:
    """
    Full pipeline: decrypt → decompress → parse.
    Returns dict with all parsed fields.
    """
    result = {}

    # Decrypt
    decrypted = decrypt_payload(encrypted_191)
    result["decrypted_header"] = decrypted[:4].hex()
    result["zlib_magic_ok"] = decrypted[:2] == b'\x78\x9c'

    # Decompress
    try:
        plaintext = zlib.decompress(decrypted, wbits=15)
        result["decompressed_ok"] = True
        result["plaintext_size"] = len(plaintext)
    except zlib.error as e:
        try:
            plaintext = zlib.decompress(decrypted, wbits=-15)
            result["decompressed_ok"] = True
            result["plaintext_size"] = len(plaintext)
            result["checksum_warning"] = str(e)
        except zlib.error as e2:
            result["decompressed_ok"] = False
            result["error"] = str(e2)
            return result

    # Parse
    result["plaintext"] = plaintext
    result["osver"] = parse_osversioninfoex(plaintext)
    result["stack_artifacts"] = find_stack_artifacts(plaintext)
    result["extra_fields"] = parse_post_osver(plaintext)
    return result


def format_report(r: dict) -> str:
    """Format parsed results as a human-readable report."""
    lines = []
    lines.append("=== Zhong Stealer REGISTER Frame — Decrypted Fingerprint ===")
    lines.append(f"  Key (static, all bots): {KEY.hex().upper()}")
    lines.append(f"  Decrypted zlib magic:   {r['decrypted_header']} {'✓ 78 9C' if r.get('zlib_magic_ok') else '✗ unexpected'}")
    lines.append(f"  Decompressed:           {'OK' if r.get('decompressed_ok') else 'FAILED'} ({r.get('plaintext_size', '?')} bytes, expected 516)")
    if not r.get("decompressed_ok"):
        lines.append(f"  Error: {r.get('error')}")
        return '\n'.join(lines)
    if r.get("checksum_warning"):
        lines.append(f"  ⚠ Checksum warning (raw deflate used): {r['checksum_warning']}")

    lines.append("")
    lines.append("[OS Version (OSVERSIONINFOEX)]")
    os = r["osver"]
    lines.append(f"  OS:              {os['os_string']} ({os['os_name']})")
    lines.append(f"  Product type:    {os['product_type']}")
    lines.append(f"  Suite mask:      0x{os['suite_mask']:04x}")
    lines.append(f"  CSD version:     {os['csd_version']}")

    lines.append("")
    lines.append("[Network / Identity]")
    sa = r["stack_artifacts"]
    for off, ip in sa.get("ips", []):
        lines.append(f"  IP address:      {ip}  (offset {off})")
    if sa.get("hostname"):
        off, host = sa["hostname"]
        lines.append(f"  Hostname:        {host}  (offset {off})")
    for off, mac in sa.get("macs", []):
        lines.append(f"  MAC (candidate): {mac}  (offset {off})")

    lines.append("")
    lines.append("[Additional Fields (post-OSVERSIONINFOEX)]")
    ex = r["extra_fields"]
    lines.append(f"  Prefix bytes:    0x{ex.get('prefix_bytes', '?')}")
    if ts := ex.get("timestamp"):
        lines.append(f"  Timestamp:       {ts}  (offset {ex.get('timestamp_offset', '?')})")
    if dwords := ex.get("nonzero_dwords"):
        lines.append(f"  Non-zero DWORDs after timestamp:")
        for off, val in dwords[:12]:
            lines.append(f"    offset {off:4d}: 0x{val:08x} = {val}")

    return '\n'.join(lines)


def main():
    if len(sys.argv) > 1:
        # Read from file or hex argument
        arg = sys.argv[1]
        if len(arg) == 382 or len(arg) == 384:
            # Hex string on command line
            encrypted = bytes.fromhex(arg.replace(' ', '').replace('\n', ''))
        else:
            with open(arg, 'rb') as f:
                data = f.read()
            if all(c in b'0123456789abcdefABCDEF \n\r' for c in data):
                encrypted = bytes.fromhex(data.decode().replace(' ', '').replace('\n', '').replace('\r', ''))
            else:
                encrypted = data
    else:
        # Use known test vector
        print("No input file specified. Using known AnyRun PCAP test vector.")
        print()
        encrypted = bytes.fromhex(KNOWN_INNER_191_HEX)

    print(f"Input: {len(encrypted)} bytes encrypted payload")
    print()

    r = decrypt_register(encrypted)
    print(format_report(r))

    if r.get("decompressed_ok") and r.get("plaintext"):
        out_path = "/home/remnux/mal/output/zs_register_plaintext.bin"
        with open(out_path, 'wb') as f:
            f.write(r["plaintext"])
        print(f"\nPlaintext saved to: {out_path}")

        # Full hex dump
        print(f"\n=== Full plaintext hex dump ({len(r['plaintext'])} bytes) ===")
        pt = r["plaintext"]
        for i in range(0, len(pt), 16):
            row = pt[i:i+16]
            hex_s = ' '.join(f'{b:02x}' for b in row)
            asc = ''.join(chr(b) if 0x20 <= b <= 0x7e else '.' for b in row)
            print(f"  {i:04x}: {hex_s:<48}  |{asc}|")


if __name__ == "__main__":
    main()
