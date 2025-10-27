#!/usr/bin/env bash
set -e

UNITY="C:\Program Files\Unity\Hub\Editor\6000.0.59f2\Editor\Unity.exe"
PROJECT="C:\Users\Philip\Projects\games\Language-Immersion-Virtual-Environment\client"
PACKS="C:\Users\Philip\Downloads\assets"
LOGDIR="D:\unity-import-logs"

mkdir -p "$LOGDIR"

echo "=== Importing all .unitypackage files from $PACKS ==="

shopt -s nullglob
for pkg in "$PACKS"/*.unitypackage; do
  name=$(basename "$pkg")
  base="${name%.unitypackage}"
  echo "[$name] importing..."
  "$UNITY" \
    -batchmode -nographics -accept-apiupdate \
    -projectPath "$PROJECT" \
    -importPackage "$pkg" \
    -quit \
    -logFile "$LOGDIR/$base.log"
done

echo "=== Final normalize pass ==="
"$UNITY" \
  -batchmode -nographics -accept-apiupdate \
  -projectPath "$PROJECT" \
  -executeMethod SyntyBulkImportAndNormalize.NormalizeOnly \
  -quit \
  -logFile "$LOGDIR/normalize.log"

echo
echo "✅ All imports complete."
echo "Logs saved in: $LOGDIR"
echo
read -n 1 -s -r -p "Press any key to close..."
echo
