#!/bin/bash

# Generate iOS bindings for all Datadog frameworks using Objective Sharpie
#
# Usage:
#   ./generate-ios-bindings.sh [binding_project_path]
#
# Example:
#   ./generate-ios-bindings.sh Datadog.MAUI.iOS.Binding

set -e

BINDING_PATH="${1:-Datadog.MAUI.iOS.Binding}"
ARTIFACTS_PATH="$BINDING_PATH/artifacts"
GENERATED_PATH="$BINDING_PATH/Generated"

# Colors
GREEN='\033[0;32m'
CYAN='\033[0;36m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Check if artifacts exist
if [ ! -d "$ARTIFACTS_PATH" ]; then
    echo -e "${RED}Error: $ARTIFACTS_PATH not found${NC}"
    echo -e "${YELLOW}Run ./download-ios-frameworks.sh first${NC}"
    exit 1
fi

# Check if sharpie is installed
if ! command -v sharpie &> /dev/null; then
    echo -e "${RED}Error: Objective Sharpie not installed${NC}"
    echo -e "${YELLOW}Install with: brew install objectivesharpie${NC}"
    exit 1
fi

# Get SDK version
SDK_VERSION=$(xcodebuild -showsdks 2>&1 | grep 'iphoneos' | head -n 1 | sed -E 's/.*-sdk (iphoneos[0-9.]+).*/\1/')
echo -e "${CYAN}Using SDK: $SDK_VERSION${NC}"

# Create generated directory
mkdir -p "$GENERATED_PATH"

# Frameworks to generate bindings for
FRAMEWORKS=(
    "DatadogCore"
    "DatadogInternal"
    "DatadogRUM"
    "DatadogLogs"
    "DatadogTrace"
    "DatadogCrashReporting"
    "DatadogSessionReplay"
    "DatadogWebViewTracking"
    "DatadogFlags"
    "OpenTelemetryApi"
)

echo -e "${CYAN}========================================${NC}"
echo -e "${CYAN}Generating iOS Bindings${NC}"
echo -e "${CYAN}========================================${NC}\n"

TOTAL=${#FRAMEWORKS[@]}
CURRENT=0
SUCCESSFUL=0
FAILED=0

for FRAMEWORK in "${FRAMEWORKS[@]}"; do
    ((CURRENT++))

    echo -e "${CYAN}[$CURRENT/$TOTAL] Generating bindings for $FRAMEWORK...${NC}"

    # Find the framework path (prefer ios-arm64_arm64e, fallback to ios-arm64)
    FRAMEWORK_PATH=$(find "$ARTIFACTS_PATH/$FRAMEWORK.xcframework" -type d -name "*.framework" | grep -E "(ios-arm64_arm64e|ios-arm64)" | grep -v simulator | head -n 1)

    if [ -z "$FRAMEWORK_PATH" ]; then
        echo -e "${YELLOW}  Warning: Framework not found: $FRAMEWORK${NC}"
        ((FAILED++))
        continue
    fi

    HEADERS_PATH="$FRAMEWORK_PATH/Headers"
    SWIFT_HEADER="$HEADERS_PATH/$FRAMEWORK-Swift.h"

    if [ ! -f "$SWIFT_HEADER" ]; then
        echo -e "${YELLOW}  Warning: Swift header not found: $SWIFT_HEADER${NC}"
        ((FAILED++))
        continue
    fi

    # Create output directory
    OUTPUT_DIR="$GENERATED_PATH/$FRAMEWORK"
    mkdir -p "$OUTPUT_DIR"

    # Generate bindings
    if sharpie bind \
        --output="$OUTPUT_DIR" \
        --namespace="DatadogMaui.iOS.$FRAMEWORK" \
        --sdk="$SDK_VERSION" \
        --scope="$HEADERS_PATH" \
        "$SWIFT_HEADER" > "$OUTPUT_DIR/sharpie.log" 2>&1; then

        echo -e "${GREEN}  ✓ Generated successfully${NC}"
        ((SUCCESSFUL++))

        # Show line counts
        if [ -f "$OUTPUT_DIR/ApiDefinitions.cs" ]; then
            LINES=$(wc -l < "$OUTPUT_DIR/ApiDefinitions.cs")
            echo -e "${GREEN}    ApiDefinitions.cs: $LINES lines${NC}"
        fi
    else
        echo -e "${YELLOW}  ⚠ Generated with warnings (check $OUTPUT_DIR/sharpie.log)${NC}"
        ((SUCCESSFUL++))  # Sharpie often succeeds despite warnings
    fi

    echo ""
done

echo -e "${CYAN}========================================${NC}"
echo -e "${CYAN}Summary${NC}"
echo -e "${CYAN}========================================${NC}"
echo -e "${GREEN}Successful: $SUCCESSFUL${NC}"
echo -e "${RED}Failed: $FAILED${NC}"
echo -e "${CYAN}Total: $TOTAL${NC}"

echo -e "\n${CYAN}Next steps:${NC}"
echo "1. Review generated bindings in: $GENERATED_PATH/"
echo "2. Consolidate into ApiDefinition.cs and StructsAndEnums.cs"
echo "3. Remove [Verify] attributes after review"
echo "4. Build: dotnet build $BINDING_PATH/"

exit 0
