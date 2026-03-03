#!/usr/bin/env python3
"""
Extract SimpleHelp Authenticode-stuffed configuration from unauthenticated
PKCS#7 attributes in a signed PE file.

The technique (described by GData, June 2025):
  - PE signed with a legitimate SimpleHelp Ltd certificate + RFC 3161 timestamp
  - Server configuration embedded as an unauthenticated attribute in the
    PKCS#7 SignerInfo, using a non-standard all-zero OID (00.00.00.00.00)
  - Unauthenticated attributes are excluded from the Authenticode hash, so
    the config can be freely replaced without invalidating the signature

Usage: python3 extract_simplehelp_config.py [input_exe] [output_dir]
"""
import sys
import os
import struct

def parse_der_length(data, offset):
    """Parse a DER length encoding. Returns (length, bytes_consumed)."""
    lb = data[offset]
    if lb < 0x80:
        return lb, 1
    n = lb & 0x7f
    if n == 1:
        return data[offset+1], 2
    elif n == 2:
        return struct.unpack_from('>H', data, offset+1)[0], 3
    elif n == 3:
        return struct.unpack_from('>I', b'\x00' + data[offset+1:offset+4])[0], 4
    elif n == 4:
        return struct.unpack_from('>I', data, offset+1)[0], 5
    raise ValueError(f"Unsupported DER length encoding: 0x{lb:02x} at offset {offset}")

def parse_der_tlv(data, offset):
    """Parse a DER TLV. Returns (tag, length, header_size, value_offset)."""
    tag = data[offset]
    length, lbytes = parse_der_length(data, offset + 1)
    header_size = 1 + lbytes
    return tag, length, header_size, offset + header_size

def find_security_dir(pe_data):
    """Return (file_offset, size) of the PE certificate table."""
    e_lfanew = struct.unpack_from('<I', pe_data, 0x3C)[0]
    opt_off = e_lfanew + 24
    magic = struct.unpack_from('<H', pe_data, opt_off)[0]
    if magic == 0x20b:    # PE32+
        data_dir_off = opt_off + 112
    else:                  # PE32
        data_dir_off = opt_off + 96
    sec_off = data_dir_off + 4 * 8  # directory[4] = security
    rva  = struct.unpack_from('<I', pe_data, sec_off)[0]
    size = struct.unpack_from('<I', pe_data, sec_off + 4)[0]
    return rva, size

def extract_stuffed_attrs(exe_path, output_dir):
    with open(exe_path, 'rb') as f:
        pe_data = f.read()

    cert_file_off, cert_total_size = find_security_dir(pe_data)
    print(f"[+] Certificate table: file offset 0x{cert_file_off:x}, size {cert_total_size} bytes")

    # WIN_CERTIFICATE header is 8 bytes: dwLength(4) + wRevision(2) + wCertType(2)
    pkcs7_off = cert_file_off + 8
    pkcs7_data = pe_data[pkcs7_off : cert_file_off + cert_total_size]
    print(f"[+] PKCS#7 data: {len(pkcs7_data)} bytes")

    # Parse PKCS#7 ContentInfo → SignedData → signerInfos SET
    # ContentInfo SEQUENCE
    _, sd_len, sd_hl, sd_val = parse_der_tlv(pkcs7_data, 0)
    # Skip OID + CONTEXT[0] → SignedData SEQUENCE
    # OID is at sd_val
    oid_tag, oid_len, oid_hl, _ = parse_der_tlv(pkcs7_data, sd_val)
    after_oid = sd_val + oid_hl + oid_len
    # cont[0] wrapper
    ctx_tag, ctx_len, ctx_hl, ctx_val = parse_der_tlv(pkcs7_data, after_oid)
    # SignedData SEQUENCE
    sdata_tag, sdata_len, sdata_hl, sdata_val = parse_der_tlv(pkcs7_data, ctx_val)

    # Walk SignedData members: version(INTEGER), digestAlgorithms(SET),
    # encapContentInfo(SEQUENCE), [0]certificates, [1]crls, signerInfos(SET)
    pos = sdata_val
    end = sdata_val + sdata_len
    signerinfos_off = None

    while pos < end:
        tag = pkcs7_data[pos]
        length, lbytes = parse_der_length(pkcs7_data, pos + 1)
        hl = 1 + lbytes
        if tag == 0x31:  # SET = signerInfos (last SET in SignedData)
            signerinfos_off = pos
        pos += hl + length

    if signerinfos_off is None:
        print("[-] Could not find signerInfos SET")
        return False

    print(f"[+] signerInfos SET at pkcs7 offset 0x{signerinfos_off:x}")

    # Walk the signerInfos SET → single SignerInfo SEQUENCE
    si_tag, si_len, si_hl, si_val = parse_der_tlv(pkcs7_data, signerinfos_off)
    sinfo_tag, sinfo_len, sinfo_hl, sinfo_val = parse_der_tlv(pkcs7_data, si_val)

    # Walk SignerInfo: version, issuerAndSerialNumber, digestAlg,
    # [0]signedAttrs, digestEncryptionAlg, encryptedDigest, [1]unsignedAttrs
    pos = sinfo_val
    end = sinfo_val + sinfo_len
    unsigned_attrs_off = None

    while pos < end:
        tag = pkcs7_data[pos]
        length, lbytes = parse_der_length(pkcs7_data, pos + 1)
        hl = 1 + lbytes
        if tag == 0xa1:  # [1] IMPLICIT unsignedAttrs
            unsigned_attrs_off = pos
            unsigned_attrs_len = length
            unsigned_attrs_hl = hl
            break
        pos += hl + length

    if unsigned_attrs_off is None:
        print("[-] No unauthenticated attributes found")
        return False

    print(f"[+] unsignedAttrs at pkcs7 offset 0x{unsigned_attrs_off:x}, {unsigned_attrs_len} bytes")

    # Walk unauthenticated attributes
    pos = unsigned_attrs_off + unsigned_attrs_hl
    end = unsigned_attrs_off + unsigned_attrs_hl + unsigned_attrs_len
    attr_num = 0

    os.makedirs(output_dir, exist_ok=True)

    while pos < end:
        tag = pkcs7_data[pos]
        if tag != 0x30:
            print(f"  [!] Unexpected tag 0x{tag:02x} at offset 0x{pos:x}, stopping")
            break
        length, lbytes = parse_der_length(pkcs7_data, pos + 1)
        hl = 1 + lbytes
        attr_val_start = pos + hl
        attr_val_end = pos + hl + length
        attr_data = pkcs7_data[attr_val_start:attr_val_end]

        # Read the OID of this attribute
        oid_tag2 = attr_data[0]
        oid_len2, oid_lbytes2 = parse_der_length(attr_data, 1)
        oid_bytes = attr_data[1 + oid_lbytes2 : 1 + oid_lbytes2 + oid_len2]
        oid_hex = oid_bytes.hex()

        print(f"\n  [+] Attribute {attr_num}: {length+hl} bytes total, OID hex={oid_hex}")

        # The value SET comes after the OID
        val_start = 1 + oid_lbytes2 + oid_len2
        if val_start < len(attr_data):
            val_tag = attr_data[val_start]
            val_len, val_lbytes = parse_der_length(attr_data, val_start + 1)
            val_hl = 1 + val_lbytes
            val_content = attr_data[val_start + val_hl : val_start + val_hl + val_len]
            print(f"      Value SET: tag=0x{val_tag:02x}, {val_len} bytes")
            print(f"      First 64 bytes: {val_content[:64].hex()}")
            # Show readable strings
            readable = bytes(b if 32 <= b < 127 else ord('.') for b in val_content[:200])
            print(f"      As text (first 200): {readable.decode('ascii', errors='replace')}")

            # Save the raw value content to file
            out_fname = os.path.join(output_dir, f"unauth_attr_{attr_num}_oid_{oid_hex}.bin")
            with open(out_fname, 'wb') as f:
                f.write(val_content)
            print(f"      Saved to: {out_fname}")

            # If this looks like SimpleHelp stuffed data, decode config
            if b'sg_install' in val_content or b'simplehelp' in val_content.lower():
                print("      [!] SimpleHelp configuration found in this attribute!")
                import re
                # Extract all printable strings, filter noise
                strings = re.findall(rb'[ -~]{4,}', val_content)
                config = {}
                i = 0
                while i < len(strings):
                    s = strings[i].decode('ascii', errors='replace')
                    # Skip the repeating ASCII printable table
                    if s.startswith(' !"#$%') or len(s) == 95:
                        i += 1
                        continue
                    # Known config keys
                    known_keys = {
                        'sg_install_shortcuts', 'jre_name', 'wrapper_gu_versions',
                        'sg_confirm_timeout', 'sh_app_profile', 'wrapper_app_versions',
                        'shpkhash', 'sg_monitor', 'sg_silent_install', 'sg_name',
                        'sg_confirm', 'update_url', 'sg_servers', 'sg_proxy',
                        'language', 'supported_langs', 'skip_system_jre',
                        'auto_disable_appnap', 'sg_password', 'show_no_ui',
                        'sg_script', 'splash_buffer', 'splash_image',
                    }
                    if s in known_keys and i + 1 < len(strings):
                        val = strings[i + 1].decode('ascii', errors='replace')
                        if s in ('splash_image', 'splash_buffer') and len(val) > 60:
                            val = val[:60] + f'... ({len(val)} chars total)'
                        # Decode hex-encoded values
                        if all(c in '0123456789ABCDEFabcdef' for c in val) and len(val) % 2 == 0 and len(val) >= 8:
                            try:
                                decoded = bytes.fromhex(val).decode('ascii')
                                val = f'{val}  [hex → {decoded}]'
                            except Exception:
                                pass
                        config[s] = val
                        i += 2
                    else:
                        i += 1
                print()
                print("      === Decoded SimpleHelp Configuration ===")
                for k, v in config.items():
                    print(f"        {k:30s} = {v}")

                # Save config to text file
                config_fname = os.path.join(output_dir, f"simplehelp_config_attr_{attr_num}.txt")
                with open(config_fname, 'w') as cf:
                    cf.write(f"# SimpleHelp configuration extracted from unauthenticated attribute {attr_num}\n")
                    cf.write(f"# Source: {exe_path}\n")
                    cf.write(f"# OID: {oid_hex}\n\n")
                    for k, v in config.items():
                        cf.write(f"{k} = {v}\n")
                print(f"\n      Config saved to: {config_fname}")

        attr_num += 1
        pos = attr_val_end

    print(f"\n[+] Total unauthenticated attributes found: {attr_num}")
    return True

if __name__ == '__main__':
    exe  = sys.argv[1] if len(sys.argv) > 1 else 'sample.exe'
    outd = sys.argv[2] if len(sys.argv) > 2 else 'output'
    extract_stuffed_attrs(exe, outd)
