#!/usr/bin/env python3
"""
Probes the TXTconverterSetup.exe payload-selection gate at
https://download.txtconverters.com/check_latest_version with varied
machine-fingerprint JSON bodies, replicating ExternalCallsService.FetchAndExtractFromServer
and MachineInfoService.GetMachineInfo from the decompiled installer.

Does NOT execute any returned payload. Responses are base64-decoded (if valid),
saved to disk, and the zip's member listing + sha256 are recorded for static
follow-up only.
"""
import base64
import hashlib
import io
import json
import time
import zipfile
from pathlib import Path

import requests

ENDPOINT = "https://download.txtconverters.com/check_latest_version"
OUT_DIR = Path("/home/remnux/mal/output/TXTconverterSetup_c2_probe")
OUT_DIR.mkdir(parents=True, exist_ok=True)

# powerProfile bit layout recovered from MachineInfoService.GetPowerProfile:
#   bit3 (0x8) ProcessorThrottle | bit2 (0x4) ThermalControl | bit1 (0x2) SystemS3 | bit0 (0x1) LidPresent
# "F" = full laptop-like ACPI profile (throttle+thermal+sleep+lid), "0" = no ACPI extras (typical bare VM/desktop)

FINGERPRINTS = [
    {
        "name": "baseline_real_laptop",
        "body": {"osBuild": "19045", "installerVersion": "3.1.1.2", "appExeExists": False,
                  "approvedCheckbox": True, "powerProfile": "F"},
    },
    {
        "name": "vm_like_no_acpi",
        "body": {"osBuild": "19045", "installerVersion": "3.1.1.2", "appExeExists": False,
                  "approvedCheckbox": True, "powerProfile": "0"},
    },
    {
        "name": "already_installed",
        "body": {"osBuild": "19045", "installerVersion": "3.1.1.2", "appExeExists": True,
                  "approvedCheckbox": True, "powerProfile": "F"},
    },
    {
        "name": "legacy_os_build_win7",
        "body": {"osBuild": "7601", "installerVersion": "3.1.1.2", "appExeExists": False,
                  "approvedCheckbox": True, "powerProfile": "F"},
    },
    {
        "name": "future_os_build_win11_24h2",
        "body": {"osBuild": "26100", "installerVersion": "3.1.1.2", "appExeExists": False,
                  "approvedCheckbox": True, "powerProfile": "F"},
    },
    {
        "name": "empty_osbuild_fallback",
        "body": {"osBuild": "", "installerVersion": "3.1.1.2", "appExeExists": False,
                  "approvedCheckbox": True, "powerProfile": "0"},
    },
    {
        "name": "checkbox_not_approved",
        "body": {"osBuild": "19045", "installerVersion": "3.1.1.2", "appExeExists": False,
                  "approvedCheckbox": False, "powerProfile": "F"},
    },
    {
        "name": "stripped_installer_version",
        "body": {"osBuild": "19045", "installerVersion": "0.0.0.0", "appExeExists": False,
                  "approvedCheckbox": True, "powerProfile": "F"},
    },
]

results = []

for fp in FINGERPRINTS:
    name = fp["name"]
    body_json = json.dumps(fp["body"])
    print(f"=== {name} === {body_json}")
    entry = {"name": name, "request_body": fp["body"]}
    try:
        resp = requests.post(
            ENDPOINT,
            data=body_json.encode("utf-8"),
            headers={"Content-Type": "application/json; charset=utf-8"},
            timeout=30,
        )
        entry["status_code"] = resp.status_code
        entry["response_len"] = len(resp.content)
        entry["response_headers"] = dict(resp.headers)
        raw_text = resp.text

        # Try to treat the body as base64 zip, exactly like FetchAndExtractFromServer does
        try:
            decoded = base64.b64decode(raw_text, validate=True)
            entry["decoded_sha256"] = hashlib.sha256(decoded).hexdigest()
            entry["decoded_size"] = len(decoded)
            dump_path = OUT_DIR / f"{name}.b64decoded.bin"
            dump_path.write_bytes(decoded)
            entry["saved_to"] = str(dump_path)
            try:
                with zipfile.ZipFile(io.BytesIO(decoded)) as zf:
                    entry["zip_members"] = zf.namelist()
            except zipfile.BadZipFile:
                entry["zip_members"] = None
                entry["note"] = "decoded but not a valid zip"
        except Exception:
            entry["decoded_sha256"] = None
            entry["raw_response_sample"] = raw_text[:300]
            raw_dump = OUT_DIR / f"{name}.raw_response.txt"
            raw_dump.write_text(raw_text)
            entry["saved_to"] = str(raw_dump)

    except requests.RequestException as e:
        entry["error"] = str(e)

    results.append(entry)
    print(json.dumps({k: v for k, v in entry.items() if k not in ("zip_members",)}, indent=2))
    time.sleep(3)

with open(OUT_DIR / "probe_results.json", "w") as f:
    json.dump(results, f, indent=2)

print("\n\nSummary:")
for r in results:
    print(r["name"], "->", r.get("status_code"), "sha256:", r.get("decoded_sha256"), "zip:", r.get("zip_members"))
