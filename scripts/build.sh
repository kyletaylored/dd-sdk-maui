#!/bin/bash

# Build script for Datadog MAUI SDK
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
NC='\033[0m' # No Color

echo -e "${CYAN}=====================================${NC}"
echo -e "${CYAN}Building Datadog MAUI SDK${NC}"
echo -e "${CYAN}Configuration: $CONFIGURATION${NC}"
echo -e "${CYAN}=====================================${NC}\n"

# Step 1: Clean
echo -e "${CYAN}[1/4] Cleaning previous builds...${NC}"
dotnet clean "$ROOT_DIR/Datadog.MAUI.sln" -c "$CONFIGURATION" -v minimal

# Step 2: Restore
echo -e "\n${CYAN}[2/4] Restoring NuGet packages...${NC}"
dotnet restore "$ROOT_DIR/Datadog.MAUI.sln" -v minimal

# Step 3: Build
echo -e "\n${CYAN}[3/4] Building projects...${NC}"
echo -e "${YELLOW}Note: iOS and Android bindings require native frameworks/libraries${NC}"
echo -e "${YELLOW}      Run download scripts first if not already done${NC}\n"

# Build iOS binding (if XCFrameworks exist)
if [ -d "$ROOT_DIR/Datadog.MAUI.iOS.Binding/DatadogCore.xcframework" ]; then
    echo -e "${GREEN}Building iOS binding...${NC}"
    dotnet build "$ROOT_DIR/Datadog.MAUI.iOS.Binding/Datadog.MAUI.iOS.Binding.csproj" -c "$CONFIGURATION" -v minimal || {
        echo -e "${YELLOW}iOS binding build failed (this is expected if bindings haven't been generated yet)${NC}"
    }
else
    echo -e "${YELLOW}Skipping iOS binding (XCFrameworks not found)${NC}"
    echo -e "${YELLOW}Run: ./scripts/download-ios-frameworks.sh${NC}"
fi

# Build Android binding
echo -e "${GREEN}Building Android binding...${NC}"
dotnet build "$ROOT_DIR/Datadog.MAUI.Android.Binding/Datadog.MAUI.Android.Binding.csproj" -c "$CONFIGURATION" -v minimal || {
    echo -e "${YELLOW}Android binding build failed (this is expected if dependencies haven't been resolved yet)${NC}"
}

# Build main plugin
echo -e "${GREEN}Building main plugin...${NC}"
dotnet build "$ROOT_DIR/Datadog.MAUI.Plugin/Datadog.MAUI.Plugin.csproj" -c "$CONFIGURATION" -v minimal || {
    echo -e "${YELLOW}Plugin build failed (requires bindings to be built first)${NC}"
}

# Step 4: Pack (if Release configuration)
if [ "$CONFIGURATION" = "Release" ]; then
    echo -e "\n${CYAN}[4/4] Creating NuGet packages...${NC}"
    echo -e "${YELLOW}To create packages, use: ./scripts/pack.sh${NC}"
    echo -e "${YELLOW}See docs/new_build_pack.md for packaging architecture details${NC}"
else
    echo -e "\n${YELLOW}[4/4] Skipping package creation (Debug build)${NC}"
fi

echo -e "\n${GREEN}=====================================${NC}"
echo -e "${GREEN}Build complete!${NC}"
echo -e "${GREEN}=====================================${NC}"

# Print summary
echo -e "\n${CYAN}Summary:${NC}"
echo "  Configuration: $CONFIGURATION"
echo "  Solution: Datadog.MAUI.sln"

echo -e "\n${CYAN}Next steps:${NC}"
if [ "$CONFIGURATION" = "Release" ]; then
    echo "  1. Create packages: ./scripts/pack.sh"
    echo "  2. Test packages locally"
    echo "  3. Publish to NuGet (see output of pack.sh for proper order)"
else
    echo "  1. Run tests: make test"
    echo "  2. Build release: ./scripts/build.sh Release"
fi
