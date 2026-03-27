# ET Ducky Infrastructure Analysis — DigitalOcean & Stripe

**Date:** 2026-03-26
**Source:** Live API probing + security whitepaper + JS bundle analysis

---

## DigitalOcean

### Hosting Architecture
| Component | Detail |
|-----------|--------|
| Application | ASP.NET Core 8 in Docker Compose, single-container deployment |
| Service account | Runs as LocalSystem (agent); Docker on Droplet (server) |
| Data residency | DigitalOcean **US regions only** |
| Database | DigitalOcean Managed PostgreSQL + TimescaleDB extension |
| Database access | Private VPC only — no public internet exposure |
| Admin endpoints | Bound to `127.0.0.1`/`::1` — localhost only, not internet-reachable |
| API container | Ports bound to localhost; external traffic via Cloudflare reverse proxy |
| Backups | Automated daily encrypted backups (DigitalOcean managed) |

### Network / IP Masking
- All DNS for `etducky.com` and `status.etducky.com` resolves to **Cloudflare anycast IPs**:
  - `104.21.59.51`
  - `172.67.214.142`
- No subdomains bypass Cloudflare proxy — real DigitalOcean Droplet IP is fully concealed
- TLS certificate: **Let's Encrypt E7** wildcard (`*.etducky.com` + `etducky.com`), issued via Cloudflare integration, valid until **2026-06-23**
- Certificate transparency logs yield no direct-IP hostnames

### Takedown Vector
A request to **DigitalOcean Abuse** (`abuse@digitalocean.com`) referencing domain `etducky.com` can identify the Droplet behind Cloudflare. DigitalOcean maintains strong abuse policies and cooperates with law enforcement. The database (Managed PostgreSQL) is a separate DigitalOcean service and would require a separate legal request.

---

## Stripe

### Account Details
| Field | Value |
|-------|-------|
| **Account ID** | `acct_1SYYvbFgHdWs2UiV` |
| **Key type** | Live / production |
| **Publishable key** | `pk_live_51SYYvbFgHdWs2UiVicEkeZ0xk5JUz30RD0lvKrnHf52dkiKJas90O2kGrrk65kyuFdvirNbngw03gI2G93t2p4mS00aljlvQS2` |
| **Confirmation method** | Stripe API error `request_log_url` leaked account ID directly |
| **Analysis probe request ID** | `req_Ciwoi1uKp2OpaP` (Stripe-logged during analysis on 2026-03-26) |

### Pricing Tiers
| Tier | Monthly Price | AI Queries | Notes |
|------|--------------|-----------|-------|
| Free | $0 | BYOK (unlimited with own API key) | 14-day data retention |
| Professional | $39 | 1,000/month | 20 free agent seats included |
| Business | $99 | 5,000/month | Team collaboration, advanced correlation |
| Enterprise | $249 | 50,000/month | SLA guarantee, white-glove support |
| Agent seats (add-on) | $5/agent/month | — | Volume discount available down to $2/agent |

### Stripe Price IDs (from `config.min.js`)
| Tier | Stripe Price ID |
|------|----------------|
| Professional | `price_1SZfmXFgHdWs2UiVUErrjPXC` |
| Business | `price_1SZfnhFgHdWs2UiV3uFK3jpM` |
| Enterprise | `price_1SZfohFgHdWs2UiV3HKS58gv` |

### "nazi" Organization Subscription State
Queried via issued agent bearer token (`etd_agent_d9e442cc...`):

| Field | Value |
|-------|-------|
| `subscriptionType` | `none` |
| `tier` | `0` (Free) |
| `tierName` | `Free` |
| `isActive` | `false` |
| `queriesPerMonth` | `0` |
| `retentionTier` | `0` |
| `retentionDays` | `14` |
| `currentPeriodEnd` | `null` |
| `isLicensed` (registration) | `true` |

The `isLicensed: true` returned during agent registration is a platform-level agent deployment flag (Clerk org is valid), not tied to the Stripe subscription tier. The "nazi" org has no active paid Stripe subscription — they are operating on the Free tier.

### Takedown Vector
Stripe account ID `acct_1SYYvbFgHdWs2UiV` combined with probe request log `req_Ciwoi1uKp2OpaP` can be submitted to **Stripe Abuse** (`abuse@stripe.com`) to identify the ET Ducky platform operator and freeze their merchant account. Since the "nazi" org is on the Free tier, no direct payment transaction exists to trace to the attacker — however, pressuring the platform operator via Stripe can result in forced org revocation. Any paying customers of ET Ducky are billed through this same account.

---

## Summary of Abuse Contact Points

| Provider | Contact | Evidence to Submit |
|----------|---------|-------------------|
| **DigitalOcean** | `abuse@digitalocean.com` | Domain `etducky.com`, org name `nazi`, Clerk org ID `org_3BDWPZa2zMSeqH45aOhFf7B5Mhi` |
| **Stripe** | `abuse@stripe.com` | Account `acct_1SYYvbFgHdWs2UiV`, request ID `req_Ciwoi1uKp2OpaP` |
| **Cloudflare** | `abuse.cloudflare.com` | Domain `etducky.com` |
| **Clerk** | `trust@clerk.com` | Org ID `org_3BDWPZa2zMSeqH45aOhFf7B5Mhi`, org name `nazi` |
| **ET Ducky platform** | `support@etducky.com` / `817-880-1336` | Malicious use of platform under org name `nazi` |
| **SSL.com** (cert issuer) | `support@ssl.com` | EV cert serial `6607c6d3aa188e3ea1cedbec3a764f36`, subject "ET Ducky LLC" |
