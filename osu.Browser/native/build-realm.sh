#!/bin/bash
# Builds a static Realm .NET native wrappers library for the browser (Emscripten).
#
# The source must match the version of the Realm package referenced by osu.Game (see https://github.com/realm/realm-dotnet),
# with submodules initialised.
#
# The Emscripten environment is provided by osu.Framework.Browser, so a local checkout of osu-framework is required.
#
# Usage: ./build-realm.sh <path to realm-dotnet source> [path to osu-framework]

set -e

if [ -z "$1" ]; then
    echo "Usage: $0 <path to realm-dotnet source> [path to osu-framework]"
    exit 1
fi

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
SOURCE_DIR="$(cd "$1" && pwd)"
FRAMEWORK_DIR="$(cd "${2:-$SCRIPT_DIR/../../../osu-framework}" && pwd)"
BUILD_DIR="$SCRIPT_DIR/obj/realm"
OUTPUT_DIR="$SCRIPT_DIR/../runtimes/browser-wasm/native"

source "$FRAMEWORK_DIR/osu.Framework.Browser/native/emsdk-env.sh"

emcmake cmake -S "$SCRIPT_DIR/realm" -B "$BUILD_DIR" -G Ninja \
    -DCMAKE_BUILD_TYPE=Release \
    -DCMAKE_C_FLAGS="$OSU_EMSCRIPTEN_CFLAGS" \
    -DCMAKE_CXX_FLAGS="$OSU_EMSCRIPTEN_CFLAGS" \
    -DREALM_DOTNET_SOURCE_DIR="$SOURCE_DIR"

cmake --build "$BUILD_DIR" --target realm-wrappers

# Merge all dependent static libraries into a single archive.
# The archive name must match the library name used by the P/Invoke declarations ("realm-wrappers").
mkdir -p "$OUTPUT_DIR"
rm -f "$OUTPUT_DIR/realm-wrappers.a"

ARCHIVES=$(find "$BUILD_DIR" -name "*.a" -not -path "*/CMakeFiles/*")

{
    echo "CREATE $OUTPUT_DIR/realm-wrappers.a"
    for archive in $ARCHIVES; do
        echo "ADDLIB $archive"
    done
    echo "SAVE"
    echo "END"
} | emar -M

emranlib "$OUTPUT_DIR/realm-wrappers.a"

echo "Built $OUTPUT_DIR/realm-wrappers.a"
