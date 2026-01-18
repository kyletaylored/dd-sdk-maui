#!/bin/bash

# Generates C# bindings from Datadog iOS XCFrameworks using Objective Sharpie
#
# Usage:
#   ./generate-ios-bindings-sharpie.sh
#
# Prerequisite: Objective Sharpie must be installed
# Install from: https://aka.ms/objective-sharpie
#
# Note: Objective Sharpie works best with iOS SDK 17.x or earlier
#       Use 'xcodes' to install an older Xcode version if needed

set -e

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
CYAN='\033[0;36m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Get the directory where this script is located
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

# Use absolute paths based on project root
ARTIFACTS_DIR="$PROJECT_ROOT/Datadog.MAUI.iOS.Binding/artifacts"
OUTPUT_DIR="$PROJECT_ROOT/Datadog.MAUI.iOS.Binding/Generated"
PLATFORM_DIR="ios-arm64_x86_64-simulator"  # Use simulator architecture for Sharpie compatibility

# Check if sharpie is installed
if ! command -v sharpie &> /dev/null; then
    echo -e "${RED}Error: Objective Sharpie is not installed${NC}"
    echo "Install from: https://aka.ms/objective-sharpie"
    exit 1
fi

echo -e "${CYAN}Objective Sharpie version: $(sharpie -v)${NC}\n"

# Find the most compatible SDK for Objective Sharpie
# Sharpie 3.5 works best with iOS SDKs up to ~17.x
# Try to find an older Xcode installation
echo -e "${CYAN}Detecting compatible Xcode/SDK...${NC}"

# Priority list: Xcode 15.x (iOS 17.x SDK) works best with Objective Sharpie
XCODE_FOUND=false

# Check for Xcode 15.x installations (iOS 17.x SDK - best compatibility)
for XCODE_PATH in /Applications/Xcode-15*.app; do
    if [ -d "$XCODE_PATH" ]; then
        XCODE_VERSION=$(defaults read "$XCODE_PATH/Contents/version" CFBundleShortVersionString 2>/dev/null || echo "unknown")
        echo -e "${GREEN}Found compatible Xcode $XCODE_VERSION at $XCODE_PATH${NC}"
        export DEVELOPER_DIR="$XCODE_PATH/Contents/Developer"
        echo -e "${CYAN}Using DEVELOPER_DIR: $DEVELOPER_DIR${NC}"
        XCODE_FOUND=true
        break
    fi
done

# Fallback to Xcode 16.x if no 15.x found
if [ "$XCODE_FOUND" = false ]; then
    for XCODE_PATH in /Applications/Xcode-16*.app; do
        if [ -d "$XCODE_PATH" ]; then
            XCODE_VERSION=$(defaults read "$XCODE_PATH/Contents/version" CFBundleShortVersionString 2>/dev/null || echo "unknown")
            echo -e "${YELLOW}Using Xcode $XCODE_VERSION at $XCODE_PATH (may have compatibility issues)${NC}"
            export DEVELOPER_DIR="$XCODE_PATH/Contents/Developer"
            echo -e "${CYAN}Using DEVELOPER_DIR: $DEVELOPER_DIR${NC}"
            XCODE_FOUND=true
            break
        fi
    done
fi

if [ "$XCODE_FOUND" = false ]; then
    echo -e "${YELLOW}Warning: No compatible Xcode found, using system default${NC}"
    echo -e "${YELLOW}For best results, install Xcode 15.4: xcodes install 15.4${NC}"
fi

# Get available iOS SDKs and select the best one
AVAILABLE_SDKS=$(xcodebuild -showsdks 2>/dev/null | grep iphoneos | awk '{print $NF}' | sed 's/-sdk //')
SDK_VERSION=$(echo "$AVAILABLE_SDKS" | head -1)

if [ -z "$SDK_VERSION" ]; then
    echo -e "${YELLOW}Warning: Could not detect iOS SDK, using default 'iphoneos'${NC}"
    SDK="iphoneos"
else
    SDK="$SDK_VERSION"
    echo -e "${GREEN}Found iOS SDK: $SDK${NC}"
fi

# Create output directory
mkdir -p "$OUTPUT_DIR"

# List of frameworks to process
FRAMEWORKS=(
    "DatadogCore"
    "DatadogInternal"
    "DatadogRUM"
    "DatadogLogs"
    "DatadogTrace"
    "DatadogCrashReporting"
    "DatadogSessionReplay"
    "DatadogWebViewTracking"
)

echo -e "${CYAN}Using iOS SDK: $SDK${NC}\n"

for FRAMEWORK in "${FRAMEWORKS[@]}"; do
    echo -e "${YELLOW}Processing $FRAMEWORK...${NC}"

    FRAMEWORK_PATH="$ARTIFACTS_DIR/$FRAMEWORK.xcframework/$PLATFORM_DIR/$FRAMEWORK.framework"
    HEADERS_PATH="$FRAMEWORK_PATH/Headers"
    SWIFT_HEADER="$HEADERS_PATH/$FRAMEWORK-Swift.h"
    UMBRELLA_HEADER="$HEADERS_PATH/$FRAMEWORK.h"

    # Check if framework exists
    if [ ! -d "$FRAMEWORK_PATH" ]; then
        echo -e "${RED}  ✗ Framework not found: $FRAMEWORK_PATH${NC}"
        continue
    fi

    # Determine which header to use
    HEADER_TO_BIND=""
    if [ -f "$SWIFT_HEADER" ]; then
        HEADER_TO_BIND="$SWIFT_HEADER"
        echo -e "  Using Swift header: $FRAMEWORK-Swift.h"
    elif [ -f "$UMBRELLA_HEADER" ]; then
        HEADER_TO_BIND="$UMBRELLA_HEADER"
        echo -e "  Using umbrella header: $FRAMEWORK.h"
    else
        echo -e "${RED}  ✗ No header file found${NC}"
        continue
    fi

    # Create framework-specific output directory
    FRAMEWORK_OUTPUT="$OUTPUT_DIR/$FRAMEWORK"
    mkdir -p "$FRAMEWORK_OUTPUT"

    # Run Objective Sharpie
    echo -e "  Generating bindings..."
    sharpie bind \
        --output="$FRAMEWORK_OUTPUT" \
        --namespace="Datadog.iOS.$FRAMEWORK" \
        --sdk="$SDK" \
        -scope "$HEADERS_PATH" \
        "$HEADER_TO_BIND" \
        2>&1 | grep -v "warning:" || true

    if [ -f "$FRAMEWORK_OUTPUT/ApiDefinitions.cs" ]; then
        echo -e "${GREEN}  ✓ Generated ApiDefinitions.cs${NC}"
    else
        echo -e "${RED}  ✗ Failed to generate ApiDefinitions.cs${NC}"
    fi

    if [ -f "$FRAMEWORK_OUTPUT/StructsAndEnums.cs" ]; then
        echo -e "${GREEN}  ✓ Generated StructsAndEnums.cs${NC}"
    else
        echo -e "${YELLOW}  ⚠ No StructsAndEnums.cs generated (may not be needed)${NC}"
    fi

    echo ""
done

echo -e "${GREEN}Binding generation complete!${NC}"
echo -e "${CYAN}Generated bindings are in: $OUTPUT_DIR${NC}"
echo ""
echo -e "${YELLOW}Next steps:${NC}"
echo "1. Review the generated files in $OUTPUT_DIR"
echo "2. Look for [Verify] attributes that need manual attention"
echo "3. Consolidate the bindings into Datadog.MAUI.iOS.Binding/ApiDefinition.cs"
echo "4. Consolidate enums/structs into Datadog.MAUI.iOS.Binding/StructsAndEnums.cs"
echo "5. Build the iOS binding project: dotnet build Datadog.MAUI.iOS.Binding/Datadog.MAUI.iOS.Binding.csproj"
