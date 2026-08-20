#!/bin/sh
# Type-check the game against the Unity API stubs. Runs once per input-handling
# setting, because that is the one Project Settings value we cannot control
# from here -- whichever way the project lands, it has to compile.
cd "$(dirname "$0")/.." || exit 1
fail=0
for defs in \
  "ENABLE_LEGACY_INPUT_MANAGER" \
  "ENABLE_INPUT_SYSTEM" \
  "ENABLE_LEGACY_INPUT_MANAGER;ENABLE_INPUT_SYSTEM" \
  "ENABLE_LEGACY_INPUT_MANAGER;UNITY_EDITOR"
do
  printf '=== %s\n' "$defs"
  if csc -target:library -langversion:7.2 -nologo -warn:4 -nowarn:0169,0414,0649 \
         -define:"$defs" -out:/dev/null \
         Assets/Scripts/*.cs tools/stubs/*.cs 2>&1 | sed 's/^/    /' | grep . ; then
    fail=1
  else
    echo "    clean"
  fi
done
if [ "$fail" = 0 ]; then echo; echo "ALL CONFIGURATIONS COMPILE"; else echo; echo "PROBLEMS ABOVE"; fi
exit $fail
