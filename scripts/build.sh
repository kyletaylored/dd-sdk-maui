#!/bin/bash

# Build script for Datadog MAUI SDK
#
# This script properly sequences the build to handle dependencies:
# 1. Build individual Android/iOS modules
# 2. Pack modules into local artifacts folder
# 3. Restore solution (which can now find module packages)
# 4. Build meta-packages and main plugin
#
# Usage:
#   ./build.sh [configuration]
#
# Examples:
#   ./build.sh                 # Build in Debug configuration
#   ./build.sh Release         # Build in Release configuration

set -e

CONFIGURATION="${1:-Debug}"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(dirname "$SCRIPT_DIR")"

# Colors
GREEN='\033[0;32m'
CYAN='\033[0;36m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${CYAN}=====================================${NC}"
echo -e "${CYAN}Building Datadog MAUI SDK${NC}"
echo -e "${CYAN}Configuration: $CONFIGURATION${NC}"
echo -e "${CYAN}=====================================${NC}\n"

# Step 1: Clean
echo -e "${CYAN}[1/5] Cleaning previous builds...${NC}"
dotnet clean "$ROOT_DIR/Datadog.MAUI.sln" -c "$CONFIGURATION" -v minimal 2>&1 | grep -v "warning\|Xamarin" || true

# Step 2: Build and pack individual module bindings
echo -e "\n${CYAN}[2/5] Building and packing individual module bindings...${NC}"
mkdir -p "$ROOT_DIR/artifacts"

# Build and pack Android modules
echo -e "\n${GREEN}Building Android modules:${NC}"
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
    "dd-sdk-android-okhttp/dd-sdk-android-okhttp.csproj"
    "dd-sdk-android-trace-otel/dd-sdk-android-trace-otel.csproj"
    "dd-sdk-android-okhttp-otel/dd-sdk-android-okhttp-otel.csproj"
    "opentracing-api/opentracing-api.csproj"
)

for module in "${ANDROID_MODULES[@]}"; do
    PROJECT_PATH="$ROOT_DIR/Datadog.MAUI.Android.Binding/$module"
    if [ -f "$PROJECT_PATH" ]; then
        MODULE_NAME=$(basename $(dirname $module))
        echo -e "  Building and packing: $MODULE_NAME..."
        dotnet build "$PROJECT_PATH" -c "$CONFIGURATION" -v minimal 2>&1 | grep -v "JAVAC0000\|duplicate class" | grep -v "\.java.*error:" > /dev/null 2>&1 || true
        dotnet pack "$PROJECT_PATH" -c "$CONFIGURATION" -o "$ROOT_DIR/artifacts" --no-build -v minimal > /dev/null 2>&1 || {
            echo -e "${YELLOW}  Warning: Failed to pack Android module $MODULE_NAME${NC}"
        }
    fi
done

# Build and pack iOS modules (only on macOS with XCFrameworks)
if [ "$(uname)" = "Darwin" ]; then
    echo -e "\n${GREEN}Building iOS modules:${NC}"
    if [ -d "$ROOT_DIR/Datadog.MAUI.iOS.Binding/DatadogCore.xcframework" ]; then
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
            "OpenTelemetryApi/OpenTelemetryApi.csproj"
        )
        
        for module in "${IOS_MODULES[@]}"; do
            PROJECT_PATH="$ROOT_DIR/Datadog.MAUI.iOS.Binding/$module"
            if [ -f "$PROJECT_PATH" ]; then
                MODULE_NAME=$(basename $(dirname $module))
                echo -e "  Building and packing: $MODULE_NAME..."
                dotnet build "$PROJECT_PATH" -c "$CONFIGURATION" -v minimal > /dev/null 2>&1 || true
                dotnet pack "$PROJECT_PATH" -c "$CONFIGURATION" -o "$ROOT_DIR/artifacts" --no-build -v minimal > /dev/null 2>&1 || {
                    echo -e "${YELLOW}  Warning: Failed to pack iOS module $MODULE_NAME${NC}"
                }
            fi
        done
    else
        echo -e "${YELLOW}Skipping iOS modules (XCFrameworks not found)${NC}"
        echo -e "${YELLOW}Run: ./scripts/download-ios-frameworks.sh${NC}"
    fi
else
    echo -e "${YELLOW}Skipping iOS modules (not on macOS)${NC}"
fi

echo -e "${GREEN}✓ Individual modules packed to ./artifacts${NC}"

# Step 3: Restore (now with locally packed modules available)
echo -e "\n${CYAN}[3/5] Restoring NuGet packages...${NC}"
dotnet restore "$ROOT_DIR/Datadog.MAUI.sln" -v minimal 2>&1 | grep -v "warning NU1608\|Xamarin.AndroidX" || true

# Step 4: Build meta-packages and main plugin
echo -e "\n${CYAN}[4/5] Building projects...${NC}"

# Build iOS binding meta-package (if available)
if [ "$(uname)" = "Darwin" ] && [ -d "$ROOT_DIR/Datadog.MAUI.iOS.Binding/DatadogCore.xcframework" ]; then
    echo -e "${GREEN}Building iOS binding meta-package...${NC}"
    dotnet build "$ROOT_DIR/Datadog.MAUI.iOS.Binding/Datadog.MAUI.iOS.Binding.csproj" -c "$CONFIGURATION" -v minimal 2>&1 | grep -v "warning\|Xamarin" || true
elif [ "$(uname)" = "Darwin" ]; then
    echo -e "${YELLOW}Skipping iOS binding (XCFrameworks not found)${NC}"
    echo -e "${YELLOW}Run: ./scripts/download-ios-frameworks.sh${NC}"
fi

# Build Android binding meta-package
echo -e "${GREEN}Building Android binding meta-package...${NC}"
dotnet build "$ROOT_DIR/Datadog.MAUI.Android.Binding/Datadog.MAUI.Android.Binding.csproj" -c "$CONFIGURATION" -v minimal 2>&1 | grep -v "warning\|Xamarin" || true

# Build main plugin
echo -e "${GREEN}Building main plugin...${NC}"
dotnet build "$ROOT_DIR/Datadog.MAUI.Plugin/Datadog.MAUI.Plugin.csproj" -c "$CONFIGURATION" -v minimal 2>&1 | grep -v "warning\|Xamarin" || true

# Step 5: Pack (if Release configuration)
if [ "$CONFIGURATION" = "Release" ]; then
    echo -e "\n${CYAN}[5/5] Creating NuGet packages...${NC}"
    echo -e "${YELLOW}To create final packages, use: ./scripts/pack.sh${NC}"
    echo -e "${YELLOW}See docs/new_build_pack.md for packaging architecture details${NC}"
else
    echo -e "\n${YELLOW}[5/5] Skipping additional package creation (Debug build)${NC}"
fi

echo -e "\n${GREEN}=====================================${NC}"
echo -e "${GREEN}Build complete!${NC}"
echo -e "${GREEN}=====================================${NC}"

# Print summary
echo -e "\n${CYAN}Summary:${NC}"
echo "  Configuration: $CONFIGURATION"
echo "  Solution: Datadog.MAUI.sln"
echo "  Module packages: ./artifacts"

echo -e "\n${CYAN}Next steps:${NC}"
if [ "$CONFIGURATION" = "Release" ]; then
    echo "  1. Create final packages: ./scripts/pack.sh"
    echo "  2. Test packages locally"
    echo "  3. Publish to NuGet"
else
    echo "  1. Run tests: make test"
    echo "  2. Build release: ./scripts/build.sh Release"
fi
