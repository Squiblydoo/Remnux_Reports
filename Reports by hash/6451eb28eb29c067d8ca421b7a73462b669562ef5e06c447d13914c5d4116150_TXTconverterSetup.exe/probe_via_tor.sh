#!/usr/bin/env bash
# Re-probes download.txtconverters.com/check_latest_version through a Tor exit node
# (different ASN/geo than the DigitalOcean host used in the original probe) to test
# whether the gate's response depends on source IP. Same 8 fingerprint bodies as
# probe_fingerprint_gate.py, routed via curl --socks5-hostname 127.0.0.1:9050.
set -u
ENDPOINT="https://download.txtconverters.com/check_latest_version"
OUT_DIR="/home/remnux/mal/output/TXTconverterSetup_c2_probe/tor_probe"
mkdir -p "$OUT_DIR"

names=(baseline_real_laptop vm_like_no_acpi already_installed legacy_os_build_win7 future_os_build_win11_24h2 empty_osbuild_fallback checkbox_not_approved stripped_installer_version)
bodies=(
'{"osBuild": "19045", "installerVersion": "3.1.1.2", "appExeExists": false, "approvedCheckbox": true, "powerProfile": "F"}'
'{"osBuild": "19045", "installerVersion": "3.1.1.2", "appExeExists": false, "approvedCheckbox": true, "powerProfile": "0"}'
'{"osBuild": "19045", "installerVersion": "3.1.1.2", "appExeExists": true, "approvedCheckbox": true, "powerProfile": "F"}'
'{"osBuild": "7601", "installerVersion": "3.1.1.2", "appExeExists": false, "approvedCheckbox": true, "powerProfile": "F"}'
'{"osBuild": "26100", "installerVersion": "3.1.1.2", "appExeExists": false, "approvedCheckbox": true, "powerProfile": "F"}'
'{"osBuild": "", "installerVersion": "3.1.1.2", "appExeExists": false, "approvedCheckbox": true, "powerProfile": "0"}'
'{"osBuild": "19045", "installerVersion": "3.1.1.2", "appExeExists": false, "approvedCheckbox": false, "powerProfile": "F"}'
'{"osBuild": "19045", "installerVersion": "0.0.0.0", "appExeExists": false, "approvedCheckbox": true, "powerProfile": "F"}'
)

for i in "${!names[@]}"; do
  name="${names[$i]}"
  body="${bodies[$i]}"
  echo "=== $name (via Tor) ==="
  curl -s --socks5-hostname 127.0.0.1:9050 --max-time 60 \
    -X POST -H "Content-Type: application/json; charset=utf-8" \
    -d "$body" \
    -D "$OUT_DIR/${name}.headers.txt" \
    -o "$OUT_DIR/${name}.response.txt" \
    "$ENDPOINT"
  echo "status/headers:"; head -5 "$OUT_DIR/${name}.headers.txt"
  python3 -c "
import base64, hashlib, zipfile, io, sys
p = '$OUT_DIR/${name}.response.txt'
data = open(p, 'r', errors='replace').read()
try:
    decoded = base64.b64decode(data, validate=True)
    h = hashlib.sha256(decoded).hexdigest()
    print('decoded_sha256:', h, 'decoded_size:', len(decoded))
    open(p + '.decoded.bin', 'wb').write(decoded)
    try:
        with zipfile.ZipFile(io.BytesIO(decoded)) as zf:
            print('zip_members:', zf.namelist())
    except zipfile.BadZipFile:
        print('not a valid zip')
except Exception as e:
    print('base64 decode failed:', e, '| raw sample:', data[:200])
"
  sleep 3
done
echo "DONE"
