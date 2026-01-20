#!/bin/bash

# Pack script for Datadog MAUI SDK
#
# This script follows the packaging architecture documented in docs/new_build_pack.md
#
# The packaging order is critical:
# 1. Pack all module binding packages (individual Android + iOS bindings)
# 2. Pack platform meta packages (dependency-only packages)
# 3. Pack consumer plugin package (final user-facing package)
#
# Usage:
#   ./pack.sh [configuration] [output-dir]
#
# Examples:
#   ./pack.sh                           # Pack in Release to ./artifacts
#   ./pack.sh Release ./my-packages     # Pack in Release to custom directory
#   ./pack.sh Debug                     # Pack in Debug to ./artifacts

set -e

CONFIGURATION="${1:-Release}"
OUTPUT_DIR="${2:-./artifacts}"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(dirname "$SCRIPT_DIR")"

# Colors
GREEN='\033[0;32m'
CYAN='\033[0;36m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${CYAN}=====================================${NC}"
echo -e "${CYAN}Datadog MAUI SDK - NuGet Packaging${NC}"
echo -e "${CYAN}Configuration: $CONFIGURATION${NC}"
echo -e "${CYAN}Output: $OUTPUT_DIR${NC}"
echo -e "${CYAN}=====================================${NC}\n"

# Ensure output directory exists
mkdir -p "$OUTPUT_DIR"

# Get absolute path for output directory
OUTPUT_DIR="$(cd "$OUTPUT_DIR" && pwd)"

echo -e "${CYAN}Output directory: $OUTPUT_DIR${NC}\n"

#
# Step A: Pack all module binding packages (Android + iOS)
#
echo -e "${CYAN}[Step A] Packing module binding packages...${NC}\n"

# Android module packages
echo -e "${GREEN}Packing Android modules:${NC}"
ANDROID_MODULES=(
    "dd-sdk-android-internal/dd-sdk-android-internal.csproj"
    "dd-sdk-android-core/dd-sdk-android-core.csproj"
    "dd-sdk-android-logs/dd-sdk-android-logs.csproj"
    "dd-sdk-android-rum/dd-sdk-android-rum.csproj"
    "dd-sdk-android-trace/dd-sdk-android-trace.csproj"
    "dd-sdk-android-ndk/dd-sdk-android-ndk.csproj"
    "dd-sdk-android-session-replay/dd-sdk-android-session-replay.csproj"
    "dd-sdk-android-webview/dd-sdk-android-webview.csproj"
    "dd-sdk-android-flags/dd-sdk-android-flags.csproj"
)

for module in "${ANDROID_MODULES[@]}"; do
    PROJECT_PATH="$ROOT_DIR/Datadog.MAUI.Android.Binding/$module"
    if [ -f "$PROJECT_PATH" ]; then
        echo -e "  Packing: $(basename $(dirname $module))..."
        dotnet pack "$PROJECT_PATH" -c "$CONFIGURATION" -o "$OUTPUT_DIR" --no-build -v minimal || {
            echo -e "${RED}  ✗ Failed to pack $module${NC}"
            exit 1
        }
    else
        echo -e "${YELLOW}  Warning: $module not found, skipping${NC}"
    fi
done

# iOS module packages (only on macOS)
if [ "$(uname)" = "Darwin" ]; then
    echo -e "\n${GREEN}Packing iOS modules:${NC}"
    IOS_MODULES=(
        "DatadogInternal/DatadogInternal.csproj"
        "DatadogCore/DatadogCore.csproj"
        "DatadogLogs/DatadogLogs.csproj"
        "DatadogRUM/DatadogRUM.csproj"
        "DatadogTrace/DatadogTrace.csproj"
        "DatadogCrashReporting/DatadogCrashReporting.csproj"
        "DatadogSessionReplay/DatadogSessionReplay.csproj"
        "DatadogWebViewTracking/DatadogWebViewTracking.csproj"
        "DatadogFlags/DatadogFlags.csproj"
    )

    for module in "${IOS_MODULES[@]}"; do
        PROJECT_PATH="$ROOT_DIR/Datadog.MAUI.iOS.Binding/$module"
        if [ -f "$PROJECT_PATH" ]; then
            echo -e "  Packing: $(basename $(dirname $module))..."
            dotnet pack "$PROJECT_PATH" -c "$CONFIGURATION" -o "$OUTPUT_DIR" --no-build -v minimal || {
                echo -e "${RED}  ✗ Failed to pack $module${NC}"
                exit 1
            }
        else
            echo -e "${YELLOW}  Warning: $module not found, skipping${NC}"
        fi
    done
else
    echo -e "\n${YELLOW}Skipping iOS modules (not on macOS)${NC}"
fi

echo -e "\n${GREEN}✓ Module packages created${NC}"

#
# Step B: Pack platform meta packages
#
echo -e "\n${CYAN}[Step B] Packing platform meta packages...${NC}\n"

echo -e "${GREEN}Packing Android meta-package:${NC}"
ANDROID_META="$ROOT_DIR/Datadog.MAUI.Android.Binding/Datadog.MAUI.Android.Binding.csproj"
if [ -f "$ANDROID_META" ]; then
    echo -e "  Packing: Datadog.MAUI.Android.Binding..."
    # IMPORTANT: Use --source to allow the meta-package to find the module packages we just packed
    # Use --no-build since meta-packages don't produce assemblies
    dotnet pack "$ANDROID_META" -c "$CONFIGURATION" -o "$OUTPUT_DIR" --source "$OUTPUT_DIR" --no-build -v minimal || {
        echo -e "${RED}  ✗ Failed to pack Android meta-package${NC}"
        exit 1
    }
else
    echo -e "${RED}  ✗ Android meta-package project not found${NC}"
    exit 1
fi

if [ "$(uname)" = "Darwin" ]; then
    echo -e "\n${GREEN}Packing iOS meta-package:${NC}"
    IOS_META="$ROOT_DIR/Datadog.MAUI.iOS.Binding/Datadog.MAUI.iOS.Binding.csproj"
    if [ -f "$IOS_META" ]; then
        echo -e "  Packing: Datadog.MAUI.iOS.Binding..."
        # Use --no-build since meta-packages don't produce assemblies
        dotnet pack "$IOS_META" -c "$CONFIGURATION" -o "$OUTPUT_DIR" --source "$OUTPUT_DIR" --no-build -v minimal || {
            echo -e "${RED}  ✗ Failed to pack iOS meta-package${NC}"
            exit 1
        }
    else
        echo -e "${RED}  ✗ iOS meta-package project not found${NC}"
        exit 1
    fi
else
    echo -e "\n${YELLOW}Skipping iOS meta-package (not on macOS)${NC}"
fi

echo -e "\n${GREEN}✓ Platform meta-packages created${NC}"

#
# Step C: Pack consumer plugin package
#
echo -e "\n${CYAN}[Step C] Packing consumer plugin package...${NC}\n"

PLUGIN="$ROOT_DIR/Datadog.MAUI.Plugin/Datadog.MAUI.Plugin.csproj"
if [ -f "$PLUGIN" ]; then
    echo -e "${GREEN}Packing: Datadog.MAUI (consumer plugin)...${NC}"
    # IMPORTANT: Use --source to allow the plugin to find the platform meta-packages
    dotnet pack "$PLUGIN" -c "$CONFIGURATION" -o "$OUTPUT_DIR" --source "$OUTPUT_DIR" -v minimal || {
        echo -e "${RED}  ✗ Failed to pack consumer plugin package${NC}"
        exit 1
    }
else
    echo -e "${RED}  ✗ Consumer plugin project not found${NC}"
    exit 1
fi

echo -e "\n${GREEN}✓ Consumer plugin package created${NC}"

#
# Summary
#
echo -e "\n${GREEN}=====================================${NC}"
echo -e "${GREEN}Packaging complete!${NC}"
echo -e "${GREEN}=====================================${NC}"

echo -e "\n${CYAN}Package Summary:${NC}"
echo -e "  Output directory: $OUTPUT_DIR"
echo -e "  Configuration: $CONFIGURATION"
echo ""
ls -lh "$OUTPUT_DIR"/*.nupkg 2>/dev/null || echo -e "${YELLOW}No packages found${NC}"

echo -e "\n${CYAN}Next steps:${NC}"
echo "  1. Test packages locally:"
echo "     dotnet nuget add source $OUTPUT_DIR --name LocalDatadogMaui"
echo ""
echo "  2. Publish to nuget.org (in order):"
echo "     # Push module packages first"
echo "     dotnet nuget push $OUTPUT_DIR/Datadog.MAUI.Android.*.nupkg --api-key YOUR_KEY --source https://api.nuget.org/v3/index.json"
echo "     dotnet nuget push $OUTPUT_DIR/Datadog.MAUI.iOS.*.nupkg --api-key YOUR_KEY --source https://api.nuget.org/v3/index.json"
echo ""
echo "     # Push platform meta packages"
echo "     dotnet nuget push $OUTPUT_DIR/Datadog.MAUI.Android.Binding.*.nupkg --api-key YOUR_KEY --source https://api.nuget.org/v3/index.json"
echo "     dotnet nuget push $OUTPUT_DIR/Datadog.MAUI.iOS.Binding.*.nupkg --api-key YOUR_KEY --source https://api.nuget.org/v3/index.json"
echo ""
echo "     # Push consumer plugin package last"
echo "     dotnet nuget push $OUTPUT_DIR/Datadog.MAUI.*.nupkg --api-key YOUR_KEY --source https://api.nuget.org/v3/index.json"
echo ""
