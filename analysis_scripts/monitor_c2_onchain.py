#!/usr/bin/env python3
"""
monitor_c2_onchain.py — Ethereum on-chain C2 URL monitor
Polls the smart contract used by ce.ps1 RAT and alerts on changes.

Usage:
    python3 monitor_c2_onchain.py              # run once, print result
    python3 monitor_c2_onchain.py --watch      # loop every 5 minutes

Cron example (check every 30 minutes, append to log):
    */30 * * * * /usr/bin/python3 /home/remnux/mal/monitor_c2_onchain.py >> /home/remnux/mal/output/c2_monitor.log 2>&1
"""

import urllib.request
import urllib.error
import json
import re
import sys
import os
import time
import datetime

# ── Contract configuration ────────────────────────────────────────────────────
CONTRACT    = "0x45729d7424d7310a0c041a2906ba95a4bd5ebfca"
ARG_ADDR    = "0xA001D3863b138eD523f255f62725AB1ddc82af87"
SELECTOR    = "0x7d434425"
CALLDATA    = (
    SELECTOR
    + "000000000000000000000000"
    + ARG_ADDR[2:].lower()          # strip 0x, pad to 32 bytes
)

RPC_NODES = [
    "https://mainnet.gateway.tenderly.co",
    "https://eth.merkle.io",
    "https://ethereum-rpc.publicnode.com",
    "https://rpc.flashbots.net/fast",
    "https://rpc.mevblocker.io",
    "https://eth-mainnet.public.blastapi.io",
    "https://eth.llamarpc.com",
    "https://eth.drpc.org",
    "https://rpc.payload.de",
]

STATE_FILE  = os.path.join(os.path.dirname(__file__), "output", "c2_monitor_state.json")
TIMEOUT_SEC = 10

# ── ABI decode ────────────────────────────────────────────────────────────────

def decode_abi_string(hex_result: str) -> str | None:
    """Decode ABI-encoded string return value from eth_call hex result."""
    try:
        data = hex_result[2:] if hex_result.startswith("0x") else hex_result
        if len(data) < 128:
            return None
        str_len = int(data[64:128], 16)
        raw_hex = data[128 : 128 + str_len * 2]
        decoded = bytes.fromhex(raw_hex).decode("utf-8")
        # Sanity: must look like a URL
        if re.match(r"^https?://", decoded):
            return decoded
        return None
    except Exception:
        return None


# ── RPC query ─────────────────────────────────────────────────────────────────

def query_rpc(rpc_url: str) -> str | None:
    """Call eth_call on one RPC node. Returns decoded URL string or None."""
    payload = json.dumps({
        "jsonrpc": "2.0",
        "method":  "eth_call",
        "params":  [{"to": CONTRACT, "data": CALLDATA}, "latest"],
        "id":      1,
    }).encode()
    req = urllib.request.Request(
        rpc_url,
        data    = payload,
        headers = {"Content-Type": "application/json"},
        method  = "POST",
    )
    try:
        with urllib.request.urlopen(req, timeout=TIMEOUT_SEC) as r:
            body = json.loads(r.read())
            result = body.get("result", "")
            if not result or result == "0x":
                return None
            return decode_abi_string(result)
    except Exception:
        return None


def fetch_c2_url() -> tuple[str | None, list[str]]:
    """
    Query all RPC nodes, return majority-vote URL and list of responding nodes.
    Returns (url_or_none, [responding_rpcs]).
    """
    votes:     dict[str, list[str]] = {}
    responded: list[str]            = []

    for rpc in RPC_NODES:
        url = query_rpc(rpc)
        if url:
            responded.append(rpc)
            votes.setdefault(url, []).append(rpc)

    if not votes:
        return None, []

    # Return the URL with the most confirming nodes
    winner = max(votes, key=lambda u: len(votes[u]))
    return winner, votes[winner]


# ── State persistence ─────────────────────────────────────────────────────────

def load_state() -> dict:
    try:
        with open(STATE_FILE) as f:
            return json.load(f)
    except Exception:
        return {}


def save_state(state: dict) -> None:
    os.makedirs(os.path.dirname(STATE_FILE), exist_ok=True)
    with open(STATE_FILE, "w") as f:
        json.dump(state, f, indent=2)


# ── Main logic ────────────────────────────────────────────────────────────────

def check_once() -> None:
    ts  = datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")
    state = load_state()

    current_url, confirming_nodes = fetch_c2_url()

    if current_url is None:
        print(f"[{ts}] WARN  No RPC responded with a valid URL "
              f"(tried {len(RPC_NODES)} nodes)")
        return

    previous_url       = state.get("c2_url")
    first_seen         = state.get("first_seen", ts)
    previous_confirmed = state.get("confirmed_by", [])

    if previous_url is None:
        # First run
        state.update({
            "c2_url":       current_url,
            "first_seen":   ts,
            "last_checked": ts,
            "confirmed_by": confirming_nodes,
            "history":      [],
        })
        save_state(state)
        print(f"[{ts}] INIT  C2 URL first recorded: {current_url}")
        print(f"         Confirmed by: {', '.join(confirming_nodes)}")

    elif current_url != previous_url:
        # URL changed — this is the alert
        history = state.get("history", [])
        history.append({
            "url":        previous_url,
            "first_seen": first_seen,
            "replaced_at": ts,
        })
        state.update({
            "c2_url":       current_url,
            "first_seen":   ts,
            "last_checked": ts,
            "confirmed_by": confirming_nodes,
            "history":      history,
        })
        save_state(state)

        print(f"[{ts}] ALERT *** C2 URL CHANGED ***")
        print(f"         Old: {previous_url}")
        print(f"         New: {current_url}")
        print(f"         Confirmed by: {', '.join(confirming_nodes)}")
        print(f"         Previous URL was seen since: {first_seen}")

    else:
        # No change
        state.update({
            "last_checked": ts,
            "confirmed_by": confirming_nodes,
        })
        save_state(state)
        print(f"[{ts}] OK    C2 URL unchanged: {current_url}")
        print(f"         Confirmed by {len(confirming_nodes)}/{len(RPC_NODES)} nodes: "
              f"{', '.join(confirming_nodes)}")


def main() -> None:
    watch_mode   = "--watch"   in sys.argv
    interval_sec = 300  # 5 minutes default for --watch

    for i, arg in enumerate(sys.argv[1:]):
        if arg.startswith("--interval="):
            try:
                interval_sec = int(arg.split("=", 1)[1])
            except ValueError:
                pass

    if watch_mode:
        print(f"Watching contract {CONTRACT} every {interval_sec}s  (Ctrl+C to stop)")
        while True:
            check_once()
            time.sleep(interval_sec)
    else:
        check_once()


if __name__ == "__main__":
    main()
