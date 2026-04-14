#!/usr/bin/env python3
"""
Zhong Stealer C2 Long-Running Emulator
========================================
Impersonates an infected Windows host on the Zhong Stealer (APT-Q-27)
C2 channel indefinitely. Logs all interactions to SQLite, attempts to
decrypt incoming command payloads, and can generate believable responses
via a pluggable handler table or an LLM backend.

Usage:
  python3 zs_emulator.py [--config config.json] [options]

  --config FILE        JSON config file (default: zs_emulator_config.json)
  --db FILE            SQLite log database (default: zs_sessions.db)
  --victim-host NAME   Hostname to report (default: DESKTOP-LAB01)
  --victim-ip IP       IP to report (default: 192.168.1.100)
  --os-build BUILD     Windows build number (default: 19045 = Win10 21H2)
  --host HOST          C2 hostname (default: uu.goldeyeuu.io)
  --port PORT          C2 port (default: 5188)
  --llm-url URL        Ollama/OpenAI-compat URL for response generation
  --llm-model MODEL    Model name (default: llama3)
  --no-respond         Log only, do not send any responses to commands
  --reconnect-mins N   Base reconnect delay in minutes (default: 2)
  -v / --verbose       Verbose logging

Architecture:
  MainThread      → ReconnectLoop → WebSocket → recv frames
  LoggingThread   → writes to SQLite immediately on each frame event
  ResponseThread  → for command frames: decrypt → dispatch → respond
  HeartbeatThread → maintains session by echoing server keepalives

Protocol reference (windui.dll SHA256: 81e276aa...):
  Frame header (12 bytes, LE):
    [0:4]  total_len   — total message length including 12-byte header
    [4:8]  orig_size   — original decompressed size (0 for keepalives)
    [8:12] session_id  — connection handle

  REGISTER frame (client→server, type=0x204 orig_size = 516 bytes):
    Encryption: custom 32-byte key cipher (sub_10001ade)
    Compression: zlib level=6 applied before encryption
    Key: 8A913610E905C3DD1F657811EA3B1933471B230F88E1C155616099A03AB0ABC0

  Server→Client frames:
    - 12-byte keepalive (orig_size=0): server heartbeat, ~every 4 minutes
    - Command frames (orig_size>0): payload likely Blowfish-encrypted
      Blowfish: P-array at VA 0x10041090 / S-boxes at VA 0x100410B8 in DLL
      Key: unknown; first attempt uses same 32-byte REGISTER key
"""

import argparse
import base64
import datetime
import hashlib
import json
import logging
import os
import queue
import random
import signal
import socket
import sqlite3
import struct
import sys
import threading
import time
import traceback
import zlib

# ── Crypto ─────────────────────────────────────────────────────────────────────

REGISTER_KEY = bytes.fromhex(
    "8A913610E905C3DD1F657811EA3B1933"
    "471B230F88E1C155616099A03AB0ABC0"
)


def _apply_cipher(data: bytes, key: bytes, encrypt: bool) -> bytes:
    """
    sub_10001ade custom cipher.
    encrypt=True  → forward transform (used when sending)
    encrypt=False → reverse transform (used when decrypting)
    """
    result = bytearray(data)
    for i in range(len(result)):
        op = i % 8
        k = key[i % 32]
        if op == 0:
            result[i] ^= k
        elif op == 1:
            if encrypt:
                result[i] = (result[i] + (k >> 1)) & 0xFF
            else:
                result[i] = (result[i] - (k >> 1)) & 0xFF
        elif op == 2:
            if encrypt:
                result[i] = (result[i] - k * 4) & 0xFF
            else:
                result[i] = (result[i] + k * 4) & 0xFF
        elif op == 3:
            if encrypt:
                result[i] = (result[i] + (k << 2)) & 0xFF
            else:
                result[i] = (result[i] - (k << 2)) & 0xFF
    return bytes(result)


def encrypt_payload(data: bytes, key: bytes = REGISTER_KEY) -> bytes:
    return _apply_cipher(data, key, encrypt=True)


def decrypt_payload(data: bytes, key: bytes = REGISTER_KEY) -> bytes:
    return _apply_cipher(data, key, encrypt=False)


def try_decompress(data: bytes) -> bytes | None:
    for wbits in (15, -15, 47):
        try:
            return zlib.decompress(data, wbits=wbits)
        except Exception:
            pass
    return None


def try_decrypt_incoming(payload: bytes, key: bytes = REGISTER_KEY) -> bytes | None:
    """
    Attempt to decrypt a server-to-client payload.
    Tries the known REGISTER key first; logs raw on failure.
    Blowfish path (unknown key) is stubbed for future work.
    """
    if not payload:
        return None
    try:
        dec = decrypt_payload(payload, key)
        pt = try_decompress(dec)
        if pt:
            return pt
    except Exception:
        pass
    return None


# ── Blowfish (stub — key unknown, structure preserved for future recovery) ──────

# The DLL embeds standard Blowfish P-array init values at VA 0x10041090.
# At runtime, the init code XORs the P-array with the session key to produce
# the working key schedule. Until the session key is recovered via dynamic
# analysis, this stub records raw payloads for offline work.

class BlowfishStub:
    """Placeholder Blowfish decryptor — logs the attempt for future keying."""
    def __init__(self):
        self.key = None  # will be set when key recovery happens

    def decrypt(self, data: bytes) -> bytes | None:
        if self.key is None:
            return None
        # TODO: implement once key is recovered
        return None

    def set_key(self, key: bytes):
        self.key = key
        logging.getLogger("zs.crypto").info(
            f"[Blowfish] Key set ({len(key)} bytes): {key.hex()}"
        )


BLOWFISH = BlowfishStub()


# ── Victim Fingerprint ─────────────────────────────────────────────────────────

# Exact 516-byte plaintext from AnyRun PCAP (decrypted, decompressed)
# Used as the base template — only IP/hostname/timestamp are patched.
_KNOWN_ENC_191 = bytes.fromhex(
    "f2e4bba16460e0629fb2747306062686c612ae5be59837159f59375dc35826c6"
    "4908e846143bb0229afde579d83bc43f07e43f19c7c7cbc7e4584f8a819e79c1"
    "4608188b9c8161e71098c2380801b1e8f80c73818074e37f4233bfb35d03135d"
    "c98b2d83632b5333344563cac710850d67e73c981baa8f097ba49b9919fe8168"
    "1a462f3d8f191891d3a020a607907f8021d9d5b60c17970503353c1d60ff0331"
    "4e47a70a40eaedc13550530c03c9fd4043afe9317589f20bd0309c353530bf"
)

_TEMPLATE_PT = zlib.decompress(decrypt_payload(_KNOWN_ENC_191), wbits=15)
assert len(_TEMPLATE_PT) == 516


def build_fingerprint(
    victim_ip: str,
    victim_hostname: str,
    os_build: int = 19045,
    os_major: int = 10,
    os_minor: int = 0,
    timestamp: datetime.datetime = None,
) -> bytes:
    pt = bytearray(_TEMPLATE_PT)
    if timestamp is None:
        timestamp = datetime.datetime.now()

    struct.pack_into("<I", pt, 0,  284)
    struct.pack_into("<I", pt, 4,  os_major)
    struct.pack_into("<I", pt, 8,  os_minor)
    struct.pack_into("<I", pt, 12, os_build)

    try:
        ip_parts = [int(x) for x in victim_ip.split(".")]
        pt[164:168] = bytes(ip_parts)
    except Exception:
        pass

    hostname_bytes = victim_hostname.encode("ascii", errors="replace")[:40]
    pt[168:208] = b"\x00" * 40
    pt[168:168 + len(hostname_bytes)] = hostname_bytes

    ts_str = timestamp.strftime("%Y-%m-%d %H:%M:%S").encode("ascii")
    pt[286:306] = ts_str + b"\x00"

    return bytes(pt)


def build_register_frame(fingerprint: bytes, session_id: int = 0x094E) -> bytes:
    compressed = zlib.compress(fingerprint, level=6)
    encrypted = encrypt_payload(compressed)
    total_len = 12 + len(encrypted)
    header = struct.pack("<III", total_len, len(fingerprint), session_id)
    return header + encrypted


# ── WebSocket ──────────────────────────────────────────────────────────────────

WS_GUID = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"


class RawWebSocket:
    def __init__(self, host: str, port: int, path: str = "/\\", timeout: float = 30.0):
        self.host = host
        self.port = port
        self.path = path
        self.timeout = timeout
        self._sock = None
        self._buf = bytearray()
        self._lock = threading.Lock()

    def connect(self):
        self._sock = socket.create_connection((self.host, self.port), timeout=self.timeout)
        self._sock.setsockopt(socket.SOL_SOCKET, socket.SO_KEEPALIVE, 1)
        self._handshake()

    def _handshake(self):
        nonce = base64.b64encode(os.urandom(16)).decode()
        req = (
            f"GET {self.path} HTTP/1.1\r\n"
            f"Host: {self.host}:{self.port}\r\n"
            f"Connection: Upgrade\r\n"
            f"Upgrade: websocket\r\n"
            f"Sec-WebSocket-Version: 13\r\n"
            f"Sec-WebSocket-Key: {nonce}\r\n"
            f"Sec-WebSocket-Extensions: permessage-deflate; client_max_window_bits\r\n"
            f"\r\n"
        )
        self._sock.sendall(req.encode())
        resp = b""
        while b"\r\n\r\n" not in resp:
            chunk = self._sock.recv(4096)
            if not chunk:
                raise ConnectionError("Closed during WS handshake")
            resp += chunk
        if b"101 Switching Protocols" not in resp:
            raise ConnectionError(f"WS upgrade failed: {resp[:200]}")

    def send_binary(self, payload: bytes):
        mask = os.urandom(4)
        masked = bytes(b ^ mask[i % 4] for i, b in enumerate(payload))
        plen = len(payload)
        if plen < 126:
            hdr = bytes([0x82, 0x80 | plen])
        elif plen < 65536:
            hdr = bytes([0x82, 0xFE]) + struct.pack(">H", plen)
        else:
            hdr = bytes([0x82, 0xFF]) + struct.pack(">Q", plen)
        with self._lock:
            self._sock.sendall(hdr + mask + masked)

    def send_pong(self, data: bytes = b""):
        with self._lock:
            self._sock.sendall(bytes([0x8A, 0x80 | len(data)]) + os.urandom(4)
                               + data[:125])

    def recv_frame(self, timeout: float = 30.0) -> tuple[int, bytes]:
        self._sock.settimeout(timeout)

        def read_exactly(n):
            while len(self._buf) < n:
                chunk = self._sock.recv(4096)
                if not chunk:
                    raise ConnectionError("Connection closed by server")
                self._buf.extend(chunk)
            data = bytes(self._buf[:n])
            self._buf = self._buf[n:]
            return data

        b0, b1 = read_exactly(2)
        opcode = b0 & 0x0F
        masked = (b1 >> 7) & 1
        plen = b1 & 0x7F
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
                with self._lock:
                    self._sock.sendall(bytes([0x88, 0x80]) + os.urandom(4))
            except Exception:
                pass
            try:
                self._sock.close()
            except Exception:
                pass
            self._sock = None


# ── Frame Parser ───────────────────────────────────────────────────────────────

def parse_frame(data: bytes) -> dict:
    if len(data) < 12:
        return {"error": "too short", "raw": data.hex()}
    total_len, orig_size, session_id = struct.unpack_from("<III", data, 0)
    payload = data[12:]
    return {
        "total_len": total_len,
        "orig_size": orig_size,
        "session_id": session_id,
        "payload_len": len(payload),
        "payload": payload,
        "is_keepalive": (len(payload) == 0),
    }


# ── SQLite Session Logger ──────────────────────────────────────────────────────

class SessionLogger:
    """
    Thread-safe SQLite logger. Writes every frame immediately.
    Schema supports long-running sessions spanning multiple reconnects.
    """

    SCHEMA = """
    CREATE TABLE IF NOT EXISTS sessions (
        id          INTEGER PRIMARY KEY AUTOINCREMENT,
        started_at  TEXT NOT NULL,
        ended_at    TEXT,
        victim_host TEXT,
        victim_ip   TEXT,
        c2_host     TEXT,
        c2_port     INTEGER,
        assigned_session_id INTEGER
    );
    CREATE TABLE IF NOT EXISTS frames (
        id          INTEGER PRIMARY KEY AUTOINCREMENT,
        session_id  INTEGER NOT NULL,
        ts          TEXT NOT NULL,
        direction   TEXT NOT NULL,  -- 'recv' or 'send'
        ws_opcode   INTEGER,
        total_len   INTEGER,
        orig_size   INTEGER,
        c2_session_id INTEGER,
        payload_len INTEGER,
        payload_hex TEXT,
        decrypted_hex TEXT,
        is_keepalive INTEGER,
        notes       TEXT,
        FOREIGN KEY(session_id) REFERENCES sessions(id)
    );
    CREATE TABLE IF NOT EXISTS commands (
        id          INTEGER PRIMARY KEY AUTOINCREMENT,
        frame_id    INTEGER,
        ts          TEXT NOT NULL,
        session_id  INTEGER,
        raw_hex     TEXT,
        decrypted_hex TEXT,
        interpreted TEXT,
        response_hex TEXT,
        response_notes TEXT,
        FOREIGN KEY(frame_id) REFERENCES frames(id)
    );
    """

    def __init__(self, db_path: str):
        self.db_path = db_path
        self._local = threading.local()
        self._init_schema()
        self.current_session_id = None

    def _conn(self) -> sqlite3.Connection:
        if not hasattr(self._local, "conn") or self._local.conn is None:
            self._local.conn = sqlite3.connect(
                self.db_path,
                check_same_thread=False,
                isolation_level=None,  # autocommit
            )
            self._local.conn.execute("PRAGMA journal_mode=WAL")
        return self._local.conn

    def _init_schema(self):
        conn = self._conn()
        for stmt in self.SCHEMA.strip().split(";"):
            stmt = stmt.strip()
            if stmt:
                conn.execute(stmt)

    def start_session(self, victim_host, victim_ip, c2_host, c2_port) -> int:
        conn = self._conn()
        cur = conn.execute(
            "INSERT INTO sessions (started_at, victim_host, victim_ip, c2_host, c2_port) "
            "VALUES (?, ?, ?, ?, ?)",
            (datetime.datetime.now(datetime.timezone.utc).isoformat(), victim_host, victim_ip,
             c2_host, c2_port),
        )
        sid = cur.lastrowid
        self.current_session_id = sid
        return sid

    def end_session(self, session_id: int, assigned_c2_session: int = None):
        conn = self._conn()
        conn.execute(
            "UPDATE sessions SET ended_at=?, assigned_session_id=? WHERE id=?",
            (datetime.datetime.now(datetime.timezone.utc).isoformat(), assigned_c2_session, session_id),
        )

    def log_frame(
        self,
        session_id: int,
        direction: str,
        ws_opcode: int,
        parsed: dict,
        decrypted: bytes = None,
        notes: str = None,
    ) -> int:
        conn = self._conn()
        dec_hex = decrypted.hex() if decrypted else None
        payload_hex = parsed.get("payload", b"").hex() or None
        cur = conn.execute(
            "INSERT INTO frames "
            "(session_id, ts, direction, ws_opcode, total_len, orig_size, "
            " c2_session_id, payload_len, payload_hex, decrypted_hex, is_keepalive, notes) "
            "VALUES (?,?,?,?,?,?,?,?,?,?,?,?)",
            (
                session_id,
                datetime.datetime.now(datetime.timezone.utc).isoformat(),
                direction,
                ws_opcode,
                parsed.get("total_len"),
                parsed.get("orig_size"),
                parsed.get("session_id"),
                parsed.get("payload_len", 0),
                payload_hex,
                dec_hex,
                int(parsed.get("is_keepalive", False)),
                notes,
            ),
        )
        return cur.lastrowid

    def log_command(
        self,
        frame_id: int,
        session_id: int,
        raw: bytes,
        decrypted: bytes = None,
        interpreted: str = None,
        response: bytes = None,
        response_notes: str = None,
    ):
        conn = self._conn()
        conn.execute(
            "INSERT INTO commands "
            "(frame_id, ts, session_id, raw_hex, decrypted_hex, interpreted, "
            " response_hex, response_notes) VALUES (?,?,?,?,?,?,?,?)",
            (
                frame_id,
                datetime.datetime.now(datetime.timezone.utc).isoformat(),
                session_id,
                raw.hex(),
                decrypted.hex() if decrypted else None,
                interpreted,
                response.hex() if response else None,
                response_notes,
            ),
        )


# ── Command Handlers ───────────────────────────────────────────────────────────
#
# Each handler receives (command_bytes, context_dict) and returns response_bytes
# or None if no response should be sent.
#
# Known command type IDs from windui.dll vtable scan:
#   These are the c2_session_id values seen in the 12-byte header of
#   SERVER→CLIENT frames; they encode the command type.
#   0x1536, 0x1537, 0x1565 — observed in live capture
#   0x0C8E, 0x098E, 0x0C91 — from handler registration table
#
# Response format is unknown until we capture and decrypt a real command frame.
# Handlers below are stubs that log the raw command and return None.
# Enable LLM path to auto-generate responses.

class CommandDispatcher:
    def __init__(self, config: dict):
        self.config = config
        self.llm_url = config.get("llm_url")
        self.llm_model = config.get("llm_model", "llama3")
        self.no_respond = config.get("no_respond", False)
        self._log = logging.getLogger("zs.cmd")

    def dispatch(self, parsed: dict, decrypted: bytes | None) -> tuple[bytes | None, str]:
        """
        Dispatch an incoming command frame.
        Returns (response_bytes_or_None, notes_string).
        """
        c2_sid = parsed.get("session_id", 0)
        payload = parsed.get("payload", b"")

        notes = f"command frame: c2_sid=0x{c2_sid:04x} payload={len(payload)}B"

        if self.no_respond:
            return None, notes + " [no-respond mode]"

        # --- Try to interpret the decrypted payload ---
        if decrypted:
            interpreted = self._interpret_plaintext(c2_sid, decrypted)
            notes += f" | interpreted: {interpreted}"
            if self.llm_url:
                response = self._llm_respond(c2_sid, decrypted, interpreted)
                if response:
                    return response, notes + " [llm response]"

        # --- Hardcoded stubs for known types ---
        handler = {
            # Will be populated as protocol is reverse engineered
            # e.g., 0x1536: self._handle_screenshot_request,
        }.get(c2_sid)

        if handler:
            return handler(payload, decrypted)

        # Default: send keepalive acknowledgement (12-byte empty frame)
        # Using the same session_id back to acknowledge
        if len(payload) == 0:
            return None, notes + " [keepalive, no response needed]"

        # Unknown non-empty command: log and send empty ack
        self._log.warning(
            f"Unknown command type 0x{c2_sid:04x} ({len(payload)}B) — "
            f"logged raw, sending keepalive ack"
        )
        ack = struct.pack("<III", 12, 0, c2_sid)
        return ack, notes + " [unknown command, sent empty ack]"

    def _interpret_plaintext(self, cmd_type: int, data: bytes) -> str:
        """Try to make sense of decrypted command bytes."""
        try:
            text = data.decode("utf-8", errors="replace")
            # Check if it looks like JSON
            if text.strip().startswith("{"):
                return f"JSON command: {text[:200]}"
            # Check for common Windows path / command strings
            if "\\" in text or "System32" in text or "cmd.exe" in text:
                return f"Shell/path command: {text[:200]}"
            return f"text: {text[:200]}"
        except Exception:
            return f"binary ({len(data)} bytes)"

    def _llm_respond(
        self, cmd_type: int, decrypted: bytes, interpreted: str
    ) -> bytes | None:
        """
        Call a local LLM (Ollama) or Claude API to generate a believable response.
        The LLM receives context about what Zhong Stealer would return for this command.
        """
        try:
            import urllib.request
            prompt = (
                "You are simulating a compromised Windows 10 host running Zhong Stealer malware "
                f"(APT-Q-27). The C2 server sent command type 0x{cmd_type:04x}.\n\n"
                f"Decrypted command content:\n{interpreted}\n\n"
                "Generate a realistic binary response that a real infected host would send back. "
                "Respond with ONLY the response bytes as a hex string (no spaces, no explanation). "
                "If the command asks for system info, use DESKTOP-LAB01 / 192.168.1.100. "
                "If the command asks for credentials/cookies, return plausible but fake data. "
                "If the command asks for a screenshot, return a placeholder 100-byte PNG header. "
                "The response will be zlib-compressed and then encrypted before sending."
            )

            if "ollama" in self.llm_url or "11434" in self.llm_url:
                # Ollama API
                payload = json.dumps({
                    "model": self.llm_model,
                    "prompt": prompt,
                    "stream": False,
                }).encode()
                req = urllib.request.Request(
                    self.llm_url,
                    data=payload,
                    headers={"Content-Type": "application/json"},
                )
                with urllib.request.urlopen(req, timeout=30) as r:
                    resp = json.loads(r.read())
                    hex_response = resp.get("response", "").strip()
            else:
                # Claude API (via ANTHROPIC_API_KEY env var)
                api_key = os.environ.get("ANTHROPIC_API_KEY", "")
                if not api_key:
                    return None
                payload = json.dumps({
                    "model": "claude-haiku-4-5-20251001",
                    "max_tokens": 256,
                    "messages": [{"role": "user", "content": prompt}],
                }).encode()
                req = urllib.request.Request(
                    "https://api.anthropic.com/v1/messages",
                    data=payload,
                    headers={
                        "Content-Type": "application/json",
                        "x-api-key": api_key,
                        "anthropic-version": "2023-06-01",
                    },
                )
                with urllib.request.urlopen(req, timeout=30) as r:
                    resp = json.loads(r.read())
                    hex_response = resp["content"][0]["text"].strip()

            # Parse hex response and wrap it
            hex_response = "".join(c for c in hex_response if c in "0123456789abcdefABCDEF")
            if hex_response:
                raw_response = bytes.fromhex(hex_response)
                # Compress + encrypt for transmission
                compressed = zlib.compress(raw_response, level=6)
                encrypted = encrypt_payload(compressed)
                total_len = 12 + len(encrypted)
                header = struct.pack("<III", total_len, len(raw_response), cmd_type)
                return header + encrypted

        except Exception as e:
            logging.getLogger("zs.llm").warning(f"LLM call failed: {e}")
        return None


# ── Emulator Core ──────────────────────────────────────────────────────────────

class ZhongEmulator:
    """
    Long-running emulated Zhong Stealer host.
    Runs in a reconnect loop; handles all frames; logs everything to SQLite.
    """

    def __init__(self, config: dict):
        self.cfg = config
        self.logger = SessionLogger(config["db"])
        self.dispatcher = CommandDispatcher(config)
        self._stop = threading.Event()
        self._log = logging.getLogger("zs.emu")

        # Statistics
        self.stats = {
            "connections": 0,
            "frames_recv": 0,
            "frames_sent": 0,
            "commands_recv": 0,
            "keepalives_recv": 0,
            "reconnects": 0,
            "start_time": datetime.datetime.now(datetime.timezone.utc).isoformat(),
        }

    def stop(self):
        self._log.info("Stop requested.")
        self._stop.set()

    def run(self):
        self._log.info("=" * 60)
        self._log.info("Zhong Stealer C2 Emulator starting")
        self._log.info(f"  C2:     {self.cfg['host']}:{self.cfg['port']}")
        self._log.info(f"  Victim: {self.cfg['victim_host']} / {self.cfg['victim_ip']}")
        self._log.info(f"  DB:     {self.cfg['db']}")
        self._log.info("=" * 60)

        base_delay = self.cfg.get("reconnect_mins", 2) * 60
        attempt = 0

        while not self._stop.is_set():
            attempt += 1
            self._log.info(f"Connection attempt #{attempt}")
            try:
                self._run_session()
                self.stats["reconnects"] += 1
            except KeyboardInterrupt:
                self._log.info("Keyboard interrupt — stopping.")
                break
            except Exception as e:
                self._log.error(f"Session error: {e}")
                if self.cfg.get("verbose"):
                    traceback.print_exc()

            if self._stop.is_set():
                break

            # Exponential backoff with jitter — looks natural to the server
            delay = min(base_delay * (2 ** min(attempt - 1, 5)), 3600)
            jitter = random.uniform(0.8, 1.2)
            wait = delay * jitter
            self._log.info(
                f"Reconnecting in {wait:.0f}s "
                f"(base={base_delay}s, attempt={attempt})"
            )
            self._stop.wait(timeout=wait)

        self._log.info("Emulator stopped.")
        self._print_stats()

    def _run_session(self):
        cfg = self.cfg
        db_session_id = self.logger.start_session(
            cfg["victim_host"], cfg["victim_ip"],
            cfg["host"], cfg["port"],
        )
        self.stats["connections"] += 1
        assigned_c2_session = None

        ws = RawWebSocket(
            cfg["host"], cfg["port"],
            path="/\\",
            timeout=cfg.get("connect_timeout", 20.0),
        )

        try:
            self._log.info(f"Connecting to {cfg['host']}:{cfg['port']}…")
            ws.connect()
            self._log.info("WebSocket connected.")

            # Build and send REGISTER
            fp = build_fingerprint(
                cfg["victim_ip"],
                cfg["victim_host"],
                os_build=cfg.get("os_build", 19045),
                timestamp=datetime.datetime.now(),
            )
            reg_frame = build_register_frame(fp, session_id=0x094E)
            ws.send_binary(reg_frame)
            self.stats["frames_sent"] += 1
            self._log.info(f"REGISTER sent ({len(reg_frame)}B)")

            # Log the sent REGISTER
            self.logger.log_frame(
                db_session_id, "send", 2,
                parse_frame(reg_frame),
                notes="REGISTER frame",
            )

            # Receive loop
            recv_timeout = cfg.get("recv_timeout", 300.0)  # 5-min recv window

            while not self._stop.is_set():
                try:
                    opcode, raw_frame = ws.recv_frame(timeout=recv_timeout)
                except socket.timeout:
                    # Server quiet — send a keepalive ping to keep connection alive
                    self._log.debug("recv timeout — sending WS ping")
                    ws._sock.settimeout(5.0)
                    try:
                        ws._sock.sendall(bytes([0x89, 0x80]) + os.urandom(4))
                    except Exception:
                        raise ConnectionError("Ping send failed — connection dead")
                    continue

                # Handle WebSocket control frames
                if opcode == 8:    # CLOSE
                    self._log.info("Server sent CLOSE — reconnecting")
                    break
                elif opcode == 9:  # PING
                    ws.send_pong()
                    continue
                elif opcode == 10: # PONG
                    continue
                elif opcode != 2:  # Not binary
                    self._log.debug(f"Ignoring WS opcode={opcode}")
                    continue

                # Binary application frame
                self.stats["frames_recv"] += 1
                parsed = parse_frame(raw_frame)

                # Attempt decryption
                decrypted = None
                if parsed["payload_len"] > 0:
                    decrypted = try_decrypt_incoming(parsed["payload"])
                    if decrypted is None:
                        # Try Blowfish stub
                        decrypted = BLOWFISH.decrypt(parsed["payload"])

                # Log to database
                frame_db_id = self.logger.log_frame(
                    db_session_id, "recv", opcode, parsed, decrypted=decrypted,
                    notes=None,
                )

                ts = datetime.datetime.now().strftime("%H:%M:%S.%f")[:12]

                if parsed["is_keepalive"]:
                    self.stats["keepalives_recv"] += 1
                    c2_sid = parsed["session_id"]

                    if assigned_c2_session is None:
                        assigned_c2_session = c2_sid
                        self.logger.end_session(db_session_id, c2_sid)
                        self._log.info(
                            f"[{ts}] [+] SERVER ACK  session=0x{c2_sid:04x}  "
                            + ("(deterministic 0x1536)" if c2_sid == 0x1536 else "(new session)")
                        )
                    else:
                        self._log.info(
                            f"[{ts}] KEEPALIVE  session=0x{c2_sid:04x}"
                        )
                else:
                    # Command frame
                    self.stats["commands_recv"] += 1
                    c2_sid = parsed["session_id"]
                    self._log.warning(
                        f"[{ts}] *** COMMAND FRAME ***  "
                        f"session=0x{c2_sid:04x}  payload={parsed['payload_len']}B"
                    )
                    if decrypted:
                        self._log.warning(
                            f"         Decrypted: {decrypted[:64].hex()}"
                            + (" ..." if len(decrypted) > 64 else "")
                        )
                    else:
                        self._log.warning(
                            f"         Raw (encrypted): {parsed['payload'][:64].hex()}"
                        )

                    # Dispatch to handler
                    response_bytes, notes = self.dispatcher.dispatch(parsed, decrypted)

                    # Log command
                    self.logger.log_command(
                        frame_id=frame_db_id,
                        session_id=db_session_id,
                        raw=parsed["payload"],
                        decrypted=decrypted,
                        interpreted=notes,
                        response=response_bytes,
                        response_notes=notes,
                    )

                    # Send response if we have one
                    if response_bytes and not cfg.get("no_respond"):
                        ws.send_binary(response_bytes)
                        self.stats["frames_sent"] += 1
                        self.logger.log_frame(
                            db_session_id, "send", 2,
                            parse_frame(response_bytes),
                            notes=f"response to cmd 0x{c2_sid:04x}",
                        )
                        self._log.info(
                            f"         Response sent: {len(response_bytes)}B"
                        )

        except (ConnectionError, OSError) as e:
            self._log.warning(f"Connection lost: {e}")
        finally:
            ws.close()
            if assigned_c2_session:
                self.logger.end_session(db_session_id, assigned_c2_session)
            self._log.info("Session closed.")

    def _print_stats(self):
        self._log.info("─" * 40)
        self._log.info("Session statistics:")
        for k, v in self.stats.items():
            self._log.info(f"  {k:<22}: {v}")
        self._log.info("─" * 40)


# ── Config ─────────────────────────────────────────────────────────────────────

DEFAULT_CONFIG = {
    "host": "uu.goldeyeuu.io",
    "port": 5188,
    "victim_host": "DESKTOP-LAB01",
    "victim_ip": "192.168.1.100",
    "os_build": 19045,
    "db": "/home/remnux/mal/output/zs_sessions.db",
    "reconnect_mins": 2,
    "connect_timeout": 20.0,
    "recv_timeout": 300.0,
    "no_respond": False,
    "llm_url": None,
    "llm_model": "llama3",
    "verbose": False,
}


def load_config(args) -> dict:
    cfg = dict(DEFAULT_CONFIG)

    # Config file
    if hasattr(args, "config") and args.config and os.path.exists(args.config):
        with open(args.config) as f:
            cfg.update(json.load(f))

    # CLI overrides
    overrides = {
        "host": args.host,
        "port": args.port,
        "victim_host": args.victim_host,
        "victim_ip": args.victim_ip,
        "os_build": args.os_build,
        "db": args.db,
        "reconnect_mins": args.reconnect_mins,
        "no_respond": args.no_respond,
        "verbose": args.verbose,
    }
    if args.llm_url:
        overrides["llm_url"] = args.llm_url
    if args.llm_model:
        overrides["llm_model"] = args.llm_model

    for k, v in overrides.items():
        if v is not None:
            cfg[k] = v

    return cfg


# ── Entry Point ────────────────────────────────────────────────────────────────

def main():
    parser = argparse.ArgumentParser(
        description="Zhong Stealer C2 long-running emulator / honeypot",
        formatter_class=argparse.ArgumentDefaultsHelpFormatter,
    )
    parser.add_argument("--config", default="zs_emulator_config.json",
                        help="JSON config file")
    parser.add_argument("--db", default="/home/remnux/mal/output/zs_sessions.db",
                        help="SQLite log database path")
    parser.add_argument("--host", default="uu.goldeyeuu.io")
    parser.add_argument("--port", type=int, default=5188)
    parser.add_argument("--victim-host", dest="victim_host", default="DESKTOP-LAB01")
    parser.add_argument("--victim-ip", dest="victim_ip", default="192.168.1.100")
    parser.add_argument("--os-build", dest="os_build", type=int, default=19045)
    parser.add_argument("--reconnect-mins", dest="reconnect_mins",
                        type=float, default=2.0,
                        help="Base reconnect delay in minutes (doubles on each failure)")
    parser.add_argument("--llm-url", dest="llm_url", default=None,
                        help="Ollama URL (e.g. http://localhost:11434/api/generate) "
                             "or 'claude' to use ANTHROPIC_API_KEY")
    parser.add_argument("--llm-model", dest="llm_model", default="llama3")
    parser.add_argument("--no-respond", dest="no_respond", action="store_true",
                        help="Log only — do not send any responses")
    parser.add_argument("-v", "--verbose", action="store_true")
    args = parser.parse_args()

    cfg = load_config(args)

    logging.basicConfig(
        level=logging.DEBUG if cfg["verbose"] else logging.INFO,
        format="%(asctime)s  %(levelname)-8s  %(name)-12s  %(message)s",
        datefmt="%Y-%m-%d %H:%M:%S",
        handlers=[
            logging.StreamHandler(sys.stdout),
            logging.FileHandler(cfg["db"].replace(".db", ".log")),
        ],
    )

    emu = ZhongEmulator(cfg)

    # Graceful shutdown on SIGTERM/SIGINT
    def _sig_handler(sig, frame):
        emu.stop()

    signal.signal(signal.SIGTERM, _sig_handler)
    signal.signal(signal.SIGINT, _sig_handler)

    emu.run()


if __name__ == "__main__":
    main()
