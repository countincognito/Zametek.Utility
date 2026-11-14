#!/usr/bin/env bash
# Packs the NuGet package, with its symbol package, into the folder given as the argument. The package is validated
# against the newest earlier release of the same major version that nuget.org holds, so a change that breaks the API of
# that release fails the pack. A first release, and the first release of a new major version, have nothing to be
# compared with and are packed without the comparison.
#
# Build in Release first. The script needs bash 4 and curl, and a connection to nuget.org, which the restore needs as well.
set -euo pipefail

if [ $# -ne 1 ]; then
  echo "usage: pack.sh <output folder>" >&2
  exit 2
fi

mkdir -p "$1"
output=$(cd "$1" && pwd)
cd "$(dirname "${BASH_SOURCE[0]}")/../.."

id=Zametek.Utility
project="src/$id/$id.csproj"
version=$(sed -n 's:.*<Version>\(.*\)</Version>.*:\1:p' "$project")
if [ -z "$version" ]; then
  echo "No <Version> in $project" >&2
  exit 1
fi
major=${version%%.*}

# Prints the newest release of the package that is earlier than this version and has the same major version, if there is one.
baseline_of() {
  # Bash clears -e inside a command substitution, so every failure here is handled by hand.
  local id=$1 index status versions
  index=$(mktemp)
  status=$(curl --silent --show-error --location --retry 3 --output "$index" --write-out '%{http_code}' \
    "https://api.nuget.org/v3-flatcontainer/${id,,}/index.json") || status=000
  case $status in
    200) ;;
    404) rm -f "$index"; return 0 ;; # not published yet
    *) rm -f "$index"; echo "nuget.org answered $status for the versions of $id" >&2; return 1 ;;
  esac
  # The answer is {"versions":["3.0.0","3.1.0","3.2.0","3.3.0-rc.1"]}. A release is a version in quotes without a suffix.
  if ! grep -q '"versions"' "$index"; then
    rm -f "$index"; echo "The versions of $id from nuget.org could not be read" >&2; return 1
  fi
  versions=$(grep -oE "\"$major\.[0-9]+\.[0-9]+\"" "$index" | tr -d '"' || true)
  rm -f "$index"
  { printf '%s\n' "$versions"; echo "$version"; } | sort -Vu | awk -v v="$version" '$0 == v { print previous; exit } { previous = $0 }'
}

baseline=$(baseline_of "$id")
args=()
if [ -n "$baseline" ]; then
  echo "Packing $id $version, validated against $baseline"
  args=("-p:PackageValidationBaselineVersion=$baseline")
else
  echo "Packing $id $version: no earlier release with the major version $major to validate against"
fi
dotnet pack "src/$id" --configuration Release --output "$output" -p:EnablePackageValidation=true ${args[@]+"${args[@]}"}
