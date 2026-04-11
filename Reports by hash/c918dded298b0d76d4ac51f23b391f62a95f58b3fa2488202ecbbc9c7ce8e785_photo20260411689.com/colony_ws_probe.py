#!/usr/bin/env python3
"""
Colony RAT WebSocket C2 Probe
Target: uu.goldeyeuu.io:5188 (56.155.111.29)
Sample: photo20260411689.com (SHA256: c918dded298b0d76d4ac51f23b391f62a95f58b3fa2488202ecbbc9c7ce8e785)

Recovered from AnyRun PCAP (task 44182f44-35a2-46c3-b8aa-a96766bcdcb0):
  - WebSocket over plain HTTP/1.1 (no TLS)
  - Path: GET /\ HTTP/1.1
  - Extension: permessage-deflate; client_max_window_bits
  - Binary frames with 12-byte header: [uint32 len][uint32 type][uint32 session_id]
  - Inner payload is application-layer encrypted (key unknown)

This script:
  1. Replays the exact captured first frame (203-byte payload)
  2. Probes with zeroed/minimal frames to elicit server responses
  3. Tries type=0 (ACK/ping) and type=0x204 (connect) variants
  4. Logs all server responses raw + decoded

Usage: python3 colony_ws_probe.py [--host HOST] [--port PORT] [--mode replay|probe|both]
"""

import socket
import struct
import hashlib
import base64
import os
import sys
import time
import json
import argparse
from datetime import datetime

TARGET_HOST = "uu.goldeyeuu.io"
TARGET_PORT = 5188
WS_PATH = "/\\"

# Exact unmasked payload from captured PCAP session (frame 0, 203 bytes)
# Header: cb000000 04020000 4e090000 = len=203, type=0x204, session=0x94e
# Followed by 191 bytes of app-layer encrypted data
CAPTURED_FRAME0_HEX = (
    "cb000000040200004e090000"
    "f2e4bba16460e0629fb2747306062686c612ae5b"
    "e59837159f59375dc35826c64908e846143bb022"
    "9afde579d83bc43f07e43f19c7c7cbc7e4584f8a"
    "819e79c14608188b9c8161e71098c2380801b1e8"
    "f80c73818074e37f4233bfb35d03135dc98b2d83"
    "632b5333344563cac710850d67e73c981baa8f09"
    "7ba49b9919fe81681a462f3d8f191891d3a020a6"
    "07907f8021d9d5b60c17970503353c1d60ff0331"
    "4e47a70a40eaedc13550530c03c9fd4043afe931"
    "7589f20bd0309c353530bf"
)

# Frame 1 (client→server, 25 bytes): type=0, session=0x993
CAPTURED_FRAME1_HEX = "1900000000000000930900004020e3e4c6f4b6afd5d9d02021"

# Server ACK frames (from PCAP)
SERVER_ACK0_HEX = "0c0000000000000036150000"  # session=0x1536
SERVER_ACK1_HEX = "0c00000000000000989800000"  # session=0x9898


def log(msg, level="INFO"):
    ts = datetime.now().strftime("%H:%M:%S.%f")[:-3]
    print(f"[{ts}] [{level}] {msg}")


def ws_mask(payload: bytes, mask_key: bytes) -> bytes:
    return bytes([b ^ mask_key[i % 4] for i, b in enumerate(payload)])


def make_ws_frame(payload: bytes, opcode: int = 2, use_mask: bool = True) -> bytes:
    """Encode a WebSocket frame (client→server, masked)."""
    header = bytes([0x80 | opcode])  # FIN=1, opcode
    plen = len(payload)

    if plen < 126:
        len_byte = 0x80 | plen if use_mask else plen
        header += bytes([len_byte])
    elif plen < 65536:
        len_byte = 0x80 | 126 if use_mask else 126
        header += bytes([len_byte]) + struct.pack(">H", plen)
    else:
        len_byte = 0x80 | 127 if use_mask else 127
        header += bytes([len_byte]) + struct.pack(">Q", plen)

    if use_mask:
        mask_key = os.urandom(4)
        header += mask_key
        payload = ws_mask(payload, mask_key)

    return header + payload


def decode_ws_frame(data: bytes) -> tuple:
    """Decode a WebSocket frame, return (fin, opcode, payload, bytes_consumed)."""
    if len(data) < 2:
        return None
    i = 0
    b0 = data[i]; b1 = data[i + 1]; i += 2
    fin = (b0 >> 7) & 1
    opcode = b0 & 0xF
    masked = (b1 >> 7) & 1
    plen = b1 & 0x7F

    if plen == 126:
        if len(data) < i + 2: return None
        plen = struct.unpack_from(">H", data, i)[0]; i += 2
    elif plen == 127:
        if len(data) < i + 8: return None
        plen = struct.unpack_from(">Q", data, i)[0]; i += 8

    mask_key = None
    if masked:
        if len(data) < i + 4: return None
        mask_key = data[i:i + 4]; i += 4

    if len(data) < i + plen:
        return None

    payload = data[i:i + plen]; i += plen
    if mask_key:
        payload = ws_mask(payload, mask_key)

    return fin, opcode, payload, i


def recv_until(sock: socket.socket, timeout: float = 5.0) -> bytes:
    """Receive all available data within timeout."""
    sock.settimeout(timeout)
    data = b""
    try:
        while True:
            chunk = sock.recv(4096)
            if not chunk:
                break
            data += chunk
    except socket.timeout:
        pass
    except ConnectionResetError:
        log("Connection reset by server", "WARN")
    return data


def do_handshake(sock: socket.socket) -> bool:
    """Perform WebSocket HTTP upgrade."""
    key_bytes = os.urandom(16)
    key_b64 = base64.b64encode(key_bytes).decode()

    # Compute expected Sec-WebSocket-Accept
    expected_accept = base64.b64encode(
        hashlib.sha1(
            (key_b64 + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11").encode()
        ).digest()
    ).decode()

    request = (
        f"GET {WS_PATH} HTTP/1.1\r\n"
        f"Host: {TARGET_HOST}:{TARGET_PORT}\r\n"
        f"Connection: Upgrade\r\n"
        f"Upgrade: websocket\r\n"
        f"Sec-WebSocket-Version: 13\r\n"
        f"Sec-WebSocket-Key: {key_b64}\r\n"
        f"Sec-WebSocket-Extensions: permessage-deflate; client_max_window_bits\r\n"
        f"\r\n"
    )
    log(f"Sending HTTP upgrade: GET {WS_PATH} HTTP/1.1 Host: {TARGET_HOST}:{TARGET_PORT}")
    sock.send(request.encode())

    sock.settimeout(10.0)
    response = b""
    while b"\r\n\r\n" not in response:
        chunk = sock.recv(4096)
        if not chunk:
            log("Connection closed during handshake", "ERROR")
            return False
        response += chunk

    resp_str = response.decode("utf-8", errors="replace")
    log(f"Server response:\n{resp_str.strip()}")

    if "101" not in resp_str:
        log("Did not receive 101 Switching Protocols", "ERROR")
        return False

    log("WebSocket handshake complete", "OK")
    return True


def decode_colony_header(payload: bytes) -> dict:
    """Decode the 12-byte Colony RAT frame header."""
    if len(payload) < 12:
        return {}
    length, msg_type, session_id = struct.unpack_from("<III", payload, 0)
    return {
        "length": length,
        "type": f"0x{msg_type:08x}",
        "session_id": f"0x{session_id:08x}",
        "payload_bytes": len(payload) - 12,
        "encrypted_payload_hex": payload[12:12+32].hex() + ("..." if len(payload) > 44 else ""),
    }


def send_and_receive(sock: socket.socket, payload: bytes, label: str, wait: float = 3.0) -> list:
    """Send a frame and collect all server responses."""
    frame = make_ws_frame(payload)
    log(f"Sending frame [{label}]: {len(payload)} bytes payload")
    log(f"  Header: {payload[:12].hex()}")
    header_info = decode_colony_header(payload)
    log(f"  Decoded: type={header_info.get('type')} session={header_info.get('session_id')}")
    sock.send(frame)

    time.sleep(wait)
    raw = recv_until(sock, timeout=wait)

    responses = []
    i = 0
    while i < len(raw):
        result = decode_ws_frame(raw[i:])
        if result is None:
            break
        fin, opcode, resp_payload, consumed = result
        i += consumed
        header_info = decode_colony_header(resp_payload)
        responses.append({
            "opcode": opcode,
            "length": len(resp_payload),
            "hex": resp_payload.hex(),
            "colony_header": header_info,
        })
        log(f"  ← Server frame: opcode={opcode} len={len(resp_payload)} hex={resp_payload[:24].hex()}")
        log(f"    Colony header: {header_info}")

    if not responses:
        log(f"  ← No server response within {wait}s", "WARN")

    return responses


def make_colony_frame(msg_type: int, session_id: int, inner_payload: bytes = b"") -> bytes:
    """Build a Colony RAT protocol message (12-byte header + payload)."""
    total_len = 12 + len(inner_payload)
    header = struct.pack("<III", total_len, msg_type, session_id)
    return header + inner_payload


def mode_replay(sock: socket.socket):
    """Replay the exact captured frames from the PCAP."""
    log("=== MODE: REPLAY (sending captured frames) ===")

    frame0 = bytes.fromhex(CAPTURED_FRAME0_HEX.replace("\n", "").replace(" ", ""))
    responses = send_and_receive(sock, frame0, "REPLAY_frame0_type0x204_203bytes", wait=5.0)

    if responses:
        frame1 = bytes.fromhex(CAPTURED_FRAME1_HEX)
        send_and_receive(sock, frame1, "REPLAY_frame1_type0x0_25bytes", wait=3.0)

    return responses


def mode_probe(sock: socket.socket):
    """Probe the server with crafted minimal frames to enumerate protocol."""
    log("=== MODE: PROBE (crafted minimal frames) ===")
    all_responses = {}

    # 1. Minimal ACK: type=0, session=0, no payload (12 bytes total)
    frame = make_colony_frame(msg_type=0x00000000, session_id=0x00000000)
    r = send_and_receive(sock, frame, "PROBE_type0x0_ack_minimal", wait=3.0)
    all_responses["type_0_ack"] = r

    # 2. Type 0x204 (connect) with zero payload - same type as captured frame 0
    frame = make_colony_frame(msg_type=0x00000204, session_id=0x00000001, inner_payload=bytes(191))
    r = send_and_receive(sock, frame, "PROBE_type0x204_zeroed_payload", wait=5.0)
    all_responses["type_0x204_zeroed"] = r

    # 3. Type 0x1 (often "ping" or "hello" in custom protocols)
    frame = make_colony_frame(msg_type=0x00000001, session_id=0x00000001)
    r = send_and_receive(sock, frame, "PROBE_type0x1", wait=3.0)
    all_responses["type_0x1"] = r

    # 4. Type 0x2 (often "data" or "command")
    frame = make_colony_frame(msg_type=0x00000002, session_id=0x00000001)
    r = send_and_receive(sock, frame, "PROBE_type0x2", wait=3.0)
    all_responses["type_0x2"] = r

    # 5. Type 0x100 (variation of connect type)
    frame = make_colony_frame(msg_type=0x00000100, session_id=0x00000001)
    r = send_and_receive(sock, frame, "PROBE_type0x100", wait=3.0)
    all_responses["type_0x100"] = r

    # 6. Send WS ping (opcode 9) to check keepalive
    ping_frame = make_ws_frame(b"ping", opcode=9)
    log("Sending WebSocket PING (opcode 9)")
    sock.send(ping_frame)
    time.sleep(2.0)
    raw = recv_until(sock, timeout=2.0)
    if raw:
        log(f"  ← Ping response: {raw.hex()[:64]}")
        all_responses["ws_ping"] = raw.hex()

    return all_responses


def main():
    parser = argparse.ArgumentParser(description="Colony RAT WebSocket C2 Probe")
    parser.add_argument("--host", default=TARGET_HOST)
    parser.add_argument("--port", type=int, default=TARGET_PORT)
    parser.add_argument("--mode", choices=["replay", "probe", "both"], default="both")
    parser.add_argument("--out", default="/home/remnux/mal/output/colony_probe_results.json")
    args = parser.parse_args()

    log(f"Target: {args.host}:{args.port} (path={WS_PATH})")
    log(f"Mode: {args.mode}")
    log("WARNING: Connecting to live C2 infrastructure. Your IP will be visible.")

    results = {"target": f"{args.host}:{args.port}", "timestamp": datetime.now().isoformat(), "sessions": []}

    try:
        # Resolve and connect
        log(f"Resolving {args.host}...")
        ip = socket.gethostbyname(args.host)
        log(f"Resolved: {args.host} → {ip}")

        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        sock.settimeout(15.0)
        log(f"Connecting to {ip}:{args.port}...")
        sock.connect((ip, args.port))
        log(f"TCP connected to {ip}:{args.port}", "OK")

        if not do_handshake(sock):
            log("Handshake failed — aborting", "ERROR")
            return

        session_result = {"ip": ip, "mode": args.mode, "responses": {}}

        if args.mode in ("replay", "both"):
            r = mode_replay(sock)
            session_result["responses"]["replay"] = r

        if args.mode in ("probe", "both"):
            r = mode_probe(sock)
            session_result["responses"]["probe"] = r

        results["sessions"].append(session_result)

    except socket.gaierror as e:
        log(f"DNS resolution failed: {e}", "ERROR")
    except ConnectionRefusedError:
        log(f"Connection refused — server may be down", "ERROR")
    except Exception as e:
        log(f"Error: {e}", "ERROR")
        import traceback; traceback.print_exc()
    finally:
        try:
            sock.close()
        except:
            pass

    # Save results
    with open(args.out, "w") as f:
        json.dump(results, f, indent=2)
    log(f"Results saved to {args.out}")

    # Summary
    log("=== SUMMARY ===")
    for session in results["sessions"]:
        for mode_name, mode_results in session.get("responses", {}).items():
            if isinstance(mode_results, list):
                log(f"  {mode_name}: {len(mode_results)} server response(s)")
            elif isinstance(mode_results, dict):
                for probe_name, probe_results in mode_results.items():
                    count = len(probe_results) if isinstance(probe_results, list) else 1
                    log(f"  {mode_name}/{probe_name}: {count} response(s)")


if __name__ == "__main__":
    main()
