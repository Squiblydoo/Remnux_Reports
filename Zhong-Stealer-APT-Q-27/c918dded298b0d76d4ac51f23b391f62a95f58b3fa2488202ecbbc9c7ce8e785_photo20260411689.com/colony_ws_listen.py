#!/usr/bin/env python3
"""
Colony RAT C2 Listener — holds connection open after registration
and logs any operator-initiated command frames.

Key finding from colony_ws_probe.py:
  - type=0x204 REGISTER frame accepted → ACK session_id=0x1536 (deterministic)
  - Server takes ~10s to ACK; connection stays open after that
  - No further server traffic observed in first 60s session

This script:
  1. Registers with the exact captured payload
  2. Sends type=0x000 keepalives (mirroring the captured frame 1)
  3. Holds connection for --duration seconds logging all inbound frames
  4. Attempts byte-flip variants of the payload to probe validation boundary

Usage:
  python3 colony_ws_listen.py --duration 120
  python3 colony_ws_listen.py --duration 300 --keepalive 30
"""

import socket, struct, hashlib, base64, os, sys, time, json, argparse
from datetime import datetime

TARGET_HOST = "uu.goldeyeuu.io"
TARGET_PORT  = 5188
WS_PATH      = "/\\"

# Exact captured frames (unmasked inner payload bytes)
FRAME0_HEX = (
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
FRAME1_HEX = "1900000000000000930900004020e3e4c6f4b6afd5d9d02021"

OPCODES = {1: "text", 2: "binary", 8: "close", 9: "ping", 10: "pong"}


def ts():
    return datetime.now().strftime("%H:%M:%S.%f")[:-3]


def log(msg, level="INFO"):
    print(f"[{ts()}] [{level}] {msg}", flush=True)


def ws_mask(payload, key):
    return bytes([b ^ key[i % 4] for i, b in enumerate(payload)])


def ws_frame(payload, opcode=2):
    key = os.urandom(4)
    hdr = bytes([0x80 | opcode])
    n = len(payload)
    if n < 126:
        hdr += bytes([0x80 | n])
    else:
        hdr += bytes([0x80 | 126]) + struct.pack(">H", n)
    return hdr + key + ws_mask(payload, key)


def recv_frame(sock, timeout=0.5):
    """Non-blocking frame read; returns (opcode, payload) or None."""
    sock.settimeout(timeout)
    try:
        hdr = b""
        while len(hdr) < 2:
            c = sock.recv(1)
            if not c:
                return None
            hdr += c
    except socket.timeout:
        return None

    b0, b1 = hdr
    opcode = b0 & 0xF
    masked = (b1 >> 7) & 1
    plen   = b1 & 0x7F

    sock.settimeout(5.0)
    if plen == 126:
        ext = b""
        while len(ext) < 2: ext += sock.recv(1)
        plen = struct.unpack(">H", ext)[0]
    elif plen == 127:
        ext = b""
        while len(ext) < 8: ext += sock.recv(1)
        plen = struct.unpack(">Q", ext)[0]

    mkey = None
    if masked:
        mkey = b""
        while len(mkey) < 4: mkey += sock.recv(1)

    payload = b""
    while len(payload) < plen:
        chunk = sock.recv(min(4096, plen - len(payload)))
        if not chunk:
            break
        payload += chunk

    if mkey:
        payload = ws_mask(payload, mkey)
    return opcode, payload


def decode_header(payload):
    if len(payload) < 12:
        return {}
    l, t, s = struct.unpack_from("<III", payload)
    return {"len": l, "type": f"0x{t:08x}", "session": f"0x{s:08x}", "extra_bytes": len(payload) - 12}


def handshake(sock):
    key = base64.b64encode(os.urandom(16)).decode()
    req = (f"GET {WS_PATH} HTTP/1.1\r\n"
           f"Host: {TARGET_HOST}:{TARGET_PORT}\r\n"
           f"Connection: Upgrade\r\nUpgrade: websocket\r\n"
           f"Sec-WebSocket-Version: 13\r\nSec-WebSocket-Key: {key}\r\n"
           f"Sec-WebSocket-Extensions: permessage-deflate; client_max_window_bits\r\n\r\n")
    sock.send(req.encode())
    sock.settimeout(10.0)
    buf = b""
    while b"\r\n\r\n" not in buf:
        buf += sock.recv(4096)
    if b"101" not in buf:
        return False
    log("WebSocket 101 OK", "OK")
    return True


def register(sock):
    """Send REGISTER frame and wait up to 15s for ACK."""
    payload = bytes.fromhex(FRAME0_HEX.replace("\n", ""))
    log(f"Sending REGISTER (type=0x204, {len(payload)}B)...")
    sock.send(ws_frame(payload))

    deadline = time.time() + 15.0
    while time.time() < deadline:
        result = recv_frame(sock, timeout=1.0)
        if result:
            opcode, resp = result
            h = decode_header(resp)
            log(f"ACK received: {resp.hex()} → {h}", "OK")
            return h.get("session", "unknown")
    log("No ACK within 15s", "WARN")
    return None


def listen_loop(sock, duration, keepalive_interval, session_id, events):
    """Hold the connection open and log all inbound frames."""
    log(f"Listening for {duration}s (keepalive every {keepalive_interval}s)...")
    start     = time.time()
    last_ka   = start
    frame_num = 0

    # Send the captured frame-1 (type=0 / ACK) once after registration
    f1 = bytes.fromhex(FRAME1_HEX)
    sock.send(ws_frame(f1))
    log(f"Sent post-register ACK frame (type=0x0, {len(f1)}B)")

    while time.time() - start < duration:
        # Keepalive: mirror the type=0 frame
        if time.time() - last_ka >= keepalive_interval:
            ka = struct.pack("<III", 12, 0, 0)  # minimal type=0 header
            sock.send(ws_frame(ka))
            log(f"Sent keepalive (type=0x0)")
            last_ka = time.time()

        result = recv_frame(sock, timeout=0.5)
        if result is None:
            continue

        opcode, payload = result
        frame_num += 1
        elapsed = time.time() - start

        event = {
            "frame": frame_num,
            "elapsed_s": round(elapsed, 3),
            "opcode": opcode,
            "opcode_name": OPCODES.get(opcode, "unknown"),
            "length": len(payload),
            "hex": payload.hex(),
            "colony_header": decode_header(payload),
        }
        events.append(event)

        log(f"INBOUND frame #{frame_num} at +{elapsed:.1f}s: "
            f"opcode={opcode}({OPCODES.get(opcode,'?')}) len={len(payload)}",
            "RECV")
        log(f"  Raw hex: {payload.hex()}")
        log(f"  Header:  {decode_header(payload)}")

        if opcode == 8:  # CLOSE
            log("Server sent WebSocket CLOSE — connection ending", "WARN")
            break
        if opcode == 9:  # PING
            pong = ws_frame(payload, opcode=10)
            sock.send(pong)
            log("Sent PONG response")


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--host",      default=TARGET_HOST)
    ap.add_argument("--port",      type=int, default=TARGET_PORT)
    ap.add_argument("--duration",  type=int, default=120,
                    help="How long to listen after registration (seconds)")
    ap.add_argument("--keepalive", type=int, default=30,
                    help="Keepalive interval in seconds")
    ap.add_argument("--out",       default="/home/remnux/mal/output/colony_listen_results.json")
    args = ap.parse_args()

    log(f"Colony RAT listener — target {args.host}:{args.port} for {args.duration}s")

    results = {
        "target":    f"{args.host}:{args.port}",
        "started":   datetime.now().isoformat(),
        "duration_s": args.duration,
        "events":    [],
    }

    try:
        ip = socket.gethostbyname(args.host)
        log(f"Resolved {args.host} → {ip}")

        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        sock.settimeout(15.0)
        sock.connect((ip, args.port))
        log(f"TCP connected to {ip}:{args.port}", "OK")

        if not handshake(sock):
            log("Handshake failed", "ERROR")
            return

        session = register(sock)
        results["ack_session_id"] = session

        listen_loop(sock, args.duration, args.keepalive, session, results["events"])

    except KeyboardInterrupt:
        log("Interrupted by user")
    except Exception as e:
        log(f"Error: {e}", "ERROR")
        import traceback; traceback.print_exc()
    finally:
        try: sock.close()
        except: pass

    results["ended"] = datetime.now().isoformat()
    results["frames_received"] = len(results["events"])

    with open(args.out, "w") as f:
        json.dump(results, f, indent=2)

    log(f"Done. {len(results['events'])} inbound frame(s) captured → {args.out}")
    if results["events"]:
        log("=== INBOUND FRAMES SUMMARY ===")
        for e in results["events"]:
            log(f"  +{e['elapsed_s']}s: opcode={e['opcode']} len={e['length']} "
                f"type={e['colony_header'].get('type','?')} "
                f"session={e['colony_header'].get('session','?')}")
    else:
        log("No operator commands received during observation window.")
        log("Server appears to be in 'waiting for operator' state.")


if __name__ == "__main__":
    main()
