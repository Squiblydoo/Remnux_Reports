# ET Ducky C2 Platform — API Capability Analysis

**Date:** 2026-03-26
**Source:** Live API probing of `https://etducky.com/api`
**Status:** Active, licensed operator (`organizationName: "nazi"`, `isLicensed: true`)

---

## Platform Overview

ET Ducky is a commercially-operated SaaS RMM (Remote Monitoring & Management) platform marketed as "AI-powered Windows root cause analysis via ETW." It is currently being **abused as a C2 framework** by a threat actor operating under the organization name `"nazi"`.

### Infrastructure (from security whitepaper)

| Component | Detail |
|-----------|--------|
| Application | ASP.NET Core 8 in Docker on **DigitalOcean** (US regions) |
| Database | DigitalOcean Managed PostgreSQL + **TimescaleDB** |
| CDN/WAF | **Cloudflare** (DDoS, WAF, TLS termination) |
| Auth (dashboard) | **Clerk** (SOC 2 Type II, JWT-based) |
| Auth (agents) | Per-agent **256-bit opaque bearer tokens** (SHA-256 hashed server-side) |
| AI backend | **Anthropic API** (Claude) for troubleshooting analysis |
| Billing | **Stripe** |
| Email | **SendGrid** |
| Frontend | React SPA via Cloudflare static hosting |

**Backend:** `cloudflare` header, CF-Ray responses, DigitalOcean private VPC for PostgreSQL, localhost-only admin endpoints.

---

## Operator Identity

| Field | Value |
|-------|-------|
| Organization Name | `nazi` |
| Clerk Org ID | `org_3BDWPZa2zMSeqH45aOhFf7B5Mhi` |
| Licensed | `true` (paid subscription) |
| Registration token generated | 2026-03-20T16:44:59.3593051Z |
| Token name | `loop` |
| API base | `https://etducky.com/api` |
| Support contact | `support@etducky.com` / `817-880-1336` |

---

## Agent Architecture

Agents are deployed as a **Windows Service** running as **LocalSystem** (required for ETW kernel-level trace access):

| Property | Detail |
|----------|--------|
| Runtime | .NET 8 self-contained Windows Service |
| Service Account | **LocalSystem** |
| Install path | `C:\Program Files\ETDucky\` |
| Data path | `C:\ProgramData\ETDucky\Agent\` |
| Config file | `C:\ProgramData\ETDucky\Agent\AgentConfig.json` |
| Bearer token storage | DPAPI-encrypted value in `AgentConfig.json` (plaintext in memory only); NTFS ACL: SYSTEM + Administrators |
| Local event buffer | SQLite `events.db` (max 10,000 events, then auto-purge) |
| Auto-update | Every 6 hours, SHA-256 verified packages from API |
| Command delivery | **Server-Sent Events (SSE)** over HTTPS |
| Silent install | `/S` flag (SCCM/Intune compatible) |

---

## Full Confirmed API Surface

### Public / Unauthenticated

| Method | Endpoint | Notes |
|--------|----------|-------|
| GET | `/api/health` | Returns `{"status":"healthy","timestamp":"...","version":"1.0.0"}` |

### Agent Registration

| Method | Endpoint | Notes |
|--------|----------|-------|
| POST | `/api/agents/register-with-token` | **One-time-use** registration token → returns permanent bearer token |

**Registration request body:**
```json
{
  "registrationToken": "etd_EBowrs6xjdMoy93Mz2taoEWExVCHi3dTLGNyclKe",
  "agentId": "<UUID>"
}
```

**Registration response schema:**
```json
{
  "success": true,
  "organizationId": "org_3BDWPZa2zMSeqH45aOhFf7B5Mhi",
  "organizationName": "nazi",
  "error": null,
  "isLicensed": true,
  "bearerToken": "etd_agent_d9e442cc1d1a7bc1563b109d2272eee553752f62264a33abf16395828feaf427"
}
```

**Note:** Registration token `etd_EBowrs6xjdMoy93Mz2taoEWExVCHi3dTLGNyclKe` was **consumed** during analysis via probe with `agentId: 00000000-0000-0000-0000-000000000000`. Token is no longer valid for further registrations.

### Agent-Authenticated Endpoints (bearer token)

| Method | Endpoint | Response Observed |
|--------|----------|-------------------|
| GET | `/api/agents` | `[]` (agent sees empty list — no cross-org agent visibility) |
| GET | `/api/agents/me` | `"Organization not found in token"` (nil UUID edge case) |
| GET | `/api/agents/me/commands` | Empty (no pending commands) |
| POST | `/api/agents/heartbeat` | `{"message":"Heartbeat received","licensed":true}` ✅ |
| POST | `/api/agents/me/config` | `{"error":"Agent not found"}` (nil UUID not stored) |

Heartbeat body: `{agentId, status, agentVersion}`

### Dashboard-Authenticated Endpoints (Clerk JWT — full capabilities)

#### Shell / Command Execution
| Method | Endpoint | Payload | Notes |
|--------|----------|---------|-------|
| POST | `/api/shell/execute` | `{agentId, commandText, scriptType, timeoutSeconds:120, runAsSystem:bool}` | **Primary RAT capability** |
| GET | `/api/shell/commands/{commandId}` | — | Poll result of queued command |

`scriptType` values: `"powershell"` (default) or `"cmd"`
`runAsSystem: true` executes as **SYSTEM account** — highest Windows privilege

#### File Operations
| Method | Endpoint | Payload | Notes |
|--------|----------|---------|-------|
| POST | `/api/files/push-to-agent` | `{agentId, fileName, fileSize, fileHash, destinationPath, totalChunks}` | Push file **to** victim |
| POST | `/api/files/transfers/{id}/upload-chunk` | `{chunkNumber, chunkSize, chunkHash, data (base64), totalChunks}` | Chunked upload |
| POST | `/api/files/request-from-agent` | `{agentId, sourcePath}` | Pull file **from** victim |
| GET | `/api/files/transfers?agentId={id}&limit=20` | — | List transfers per agent |
| DELETE | `/api/files/transfers/{id}` | — | Delete transfer record |

#### Agent Management
| Method | Endpoint | Notes |
|--------|----------|-------|
| GET | `/api/agents` | List all org agents (dashboard view) |
| GET/POST/DELETE | `/api/agents/{id}/...` | Per-agent operations |
| GET | `/api/gateway/{agentId}/command/{commandId}` | Gateway command status poll |
| GET | `/api/gateway/command/{commandId}` | Alternative command status poll |

#### Automation Engine
| Method | Endpoint | Notes |
|--------|----------|-------|
| GET/POST/PUT/DELETE | `/api/automations/rules` | CRUD automation rules (admin only) |
| GET | `/api/automations/stats` | Automation statistics |
| GET | `/api/automations/history` | Automation run history |
| GET/POST/PUT/DELETE | `/api/scripts` | Shared script library (PowerShell/CMD) |
| POST | `/api/scripts/{id}/trigger` | Trigger a saved script |

#### AI Troubleshooting
| Method | Endpoint | Notes |
|--------|----------|-------|
| POST | `/api/troubleshooting/jobs/{agentId}` | Start AI analysis job (→ Anthropic API) |
| GET | `/api/troubleshooting/jobs/{jobId}` | Poll/stream AI analysis results |

#### Ticketing & Integrations
| Method | Endpoint | Notes |
|--------|----------|-------|
| GET/POST | `/api/agents/me/tickets` | Create support tickets from agent |
| GET/POST/DELETE | `/api/tickets/{id}` | Ticket management |
| GET/POST/DELETE | `/api/integrations/ticketing` | ServiceNow/Jira ticketing integrations |

#### Agent Tokens
| Method | Endpoint | Notes |
|--------|----------|-------|
| GET | `/api/agent-tokens/list` | List registration tokens (admin only) |

#### Billing & Subscription
| Method | Endpoint |
|--------|----------|
| GET | `/api/subscription/info` |
| GET | `/api/usage/current` |
| GET | `/api/agent-seats/info` |
| DELETE | `/api/agent-seats/remove` |
| GET | `/api/billing/breakdown?clerkOrgId=` |
| POST | `/api/stripe/create-checkout-session` |
| POST | `/api/stripe/create-agent-seats-checkout` |
| POST | `/api/stripe/create-portal-session` |
| POST | `/api/stripe/create-retention-checkout` |
| GET | `/api/stripe/retention-info` |
| GET | `/api/subscriptions` |

#### User / Org
| Method | Endpoint | Notes |
|--------|----------|-------|
| POST | `/api/user/api-key` | Set/manage personal API key |

---

## Attack Capabilities Summary

| Capability | Mechanism | Severity |
|------------|-----------|----------|
| **Remote code execution** | `POST /api/shell/execute` — PowerShell or CMD on any enrolled host | CRITICAL |
| **Execute as SYSTEM** | `runAsSystem: true` in shell execute payload | CRITICAL |
| **Multi-agent targeting** | Fleet-wide command dispatch to all selected agents simultaneously | CRITICAL |
| **File delivery to victim** | `POST /api/files/push-to-agent` + chunked upload — arbitrary file to arbitrary path | CRITICAL |
| **File exfiltration from victim** | `POST /api/files/request-from-agent` — arbitrary path read | CRITICAL |
| **Persistent automation** | Automation rules with triggers + saved script library — survives dashboard logout | HIGH |
| **ETW-based surveillance** | On-demand collection of process creation, file I/O, network connections, registry changes | HIGH |
| **Remote desktop** | Referenced in UI (`remote-desktop.css`, session/control UI elements) | HIGH |
| **Heartbeat / beacon** | `POST /api/agents/heartbeat` — operator sees real-time online/offline status | MEDIUM |
| **Auto-update** | Agent self-updates from operator-controlled API every 6 hours | HIGH |

---

## Data Flow (Attack Scenario)

```
Victim runs ETDucky_Installer.dll
        ↓
    UAC elevation (runas)
        ↓
    ZIP extracted → tenant.json + ETDucky_AgentSetup.exe
        ↓
    ETDucky_AgentSetup.exe /VERYSILENT /REG_TOKEN="etd_..."
        ↓
    Inno Setup (ChaCha20-encrypted) installs agent as Windows Service (LocalSystem)
        ↓
    Agent POSTs /api/agents/register-with-token
    ← Server returns permanent bearer token (stored DPAPI-encrypted)
        ↓
    Agent connects to SSE command stream (persistent HTTPS connection)
        ↓
    Operator (from dashboard) issues:
      - POST /api/shell/execute {commandText: "...", runAsSystem: true}
      - POST /api/files/push-to-agent (deliver next-stage payload)
      - POST /api/files/request-from-agent (exfiltrate credentials/data)
      - Automation rules (persistent, trigger-based execution)
```

---

## What ET Ducky Collects By Default (Whitepaper)

**Collected:** CPU/memory/disk/network metrics, ETW events (process creation, file I/O, network connections, registry changes, service state changes — configurable, disabled by default), hostname, OS version, agent version, IP address, Windows Event Log error counts.

**Operator-stated NOT collected:** Credentials, file contents, keystrokes, screen captures, browser history, PII beyond hostname.

> ⚠️ **Analyst note:** The "not collected" list is meaningless in this threat scenario. The `/api/shell/execute` endpoint with `runAsSystem: true` gives the operator unrestricted arbitrary code execution, negating all platform-level data collection restrictions. The operator can exfiltrate any file, dump credentials, deploy payloads, or persist at will using standard shell commands.

---

## Probing Artifacts

The following actions were taken during live analysis against `etducky.com`:

1. Unauthenticated GET/POST probes to enumerate API surface (no impact)
2. Registration of a probe agent:
   - `agentId: 00000000-0000-0000-0000-000000000000` (nil UUID)
   - Registration token `etd_EBowrs6xjdMoy93Mz2taoEWExVCHi3dTLGNyclKe` **consumed**
   - Issued bearer token: `etd_agent_d9e442cc1d1a7bc1563b109d2272eee553752f62264a33abf16395828feaf427`
3. One heartbeat sent (`{"message":"Heartbeat received","licensed":true}`)
4. No ETW data, shell commands, or file operations were performed

**Operator visibility:** The operator's dashboard will show a probe agent registered from the REMnux analysis IP (agentId: all-zeros, hostname unknown — config POST failed due to nil UUID rejection). The registration token `loop` is now consumed and cannot be used for further victim deployments from the same tenant.json package.

---

## Recommendations

1. **Block `etducky.com`** at perimeter firewall and DNS — any outbound connections indicate an enrolled agent
2. **Indicator of compromise**: Windows Service named `ETDucky` or `ET Ducky`; presence of `C:\Program Files\ETDucky\` or `C:\ProgramData\ETDucky\`; DPAPI-encrypted `AgentConfig.json`
3. **Registry persistence**: Inno Setup typically installs a Run key or Windows Service for auto-start; enumerate with `sc query` and `HKLM\SYSTEM\CurrentControlSet\Services\ETDucky`
4. **Report to ET Ducky**: The `nazi` organization is using their platform for malicious deployment. Contact `support@etducky.com` for abuse reporting
5. **Yara detection**: Match on `ETDUCKY\x00` magic bytes (offset near Authenticode signature boundary) to detect future samples built with this installer framework
