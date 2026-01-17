#!/bin/bash

# Downloads Datadog iOS XCFrameworks from GitHub releases
#
# Usage:
#   ./download-ios-frameworks.sh [version] [output_path]
#
# Examples:
#   ./download-ios-frameworks.sh 3.5.0
#   ./download-ios-frameworks.sh 3.5.0 Datadog.MAUI.iOS.Binding
#   ./download-ios-frameworks.sh 3.5.0 ../Datadog.MAUI.iOS.Binding
#
# The frameworks will be installed in <output_path>/artifacts/*.xcframework

set -e

VERSION="${1:-}"
OUTPUT_PATH="${2:-../Datadog.MAUI.iOS.Binding}"

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
GRAY='\033[0;90m'
NC='\033[0m' # No Color

# If no version specified, get latest from GitHub API
if [ -z "$VERSION" ]; then
    echo -e "${YELLOW}No version specified. Fetching latest release...${NC}"

    LATEST_JSON=$(curl -s -H "Accept: application/vnd.github+json" \
        -H "User-Agent: Datadog-MAUI-Binding-Script" \
        "https://api.github.com/repos/DataDog/dd-sdk-ios/releases/latest")

    VERSION=$(echo "$LATEST_JSON" | grep '"tag_name":' | sed -E 's/.*"tag_name": "([^"]+)".*/\1/')

    if [ -z "$VERSION" ]; then
        echo -e "${RED}Failed to fetch latest release from GitHub API${NC}"
        exit 1
    fi

    echo -e "${GREEN}Latest version: $VERSION${NC}"
fi

# Create output directory if it doesn't exist
mkdir -p "$OUTPUT_PATH"

# Check if output path is accessible
if [ ! -d "$OUTPUT_PATH" ]; then
    echo -e "${RED}Could not create or access output path: $OUTPUT_PATH${NC}"
    exit 1
fi

echo -e "${CYAN}Downloading Datadog iOS SDK v$VERSION...${NC}"

# Query GitHub API for release assets
RELEASE_JSON=$(curl -s -H "Accept: application/vnd.github+json" \
    -H "User-Agent: Datadog-MAUI-Binding-Script" \
    "https://api.github.com/repos/DataDog/dd-sdk-ios/releases/tags/$VERSION")

# Find XCFramework zip asset (exclude dSYMs and arm64e)
DOWNLOAD_URL=$(echo "$RELEASE_JSON" | grep '"browser_download_url":' | grep '.zip' | grep -v 'dSYMs' | grep -v 'arm64e' | head -n 1 | sed -E 's/.*"browser_download_url": "([^"]+)".*/\1/')

if [ -z "$DOWNLOAD_URL" ]; then
    echo -e "${YELLOW}Could not find asset via API, using fallback URL${NC}"
    DOWNLOAD_URL="https://github.com/DataDog/dd-sdk-ios/releases/download/$VERSION/Datadog.xcframework.zip"
fi

echo -e "${GRAY}Download URL: $DOWNLOAD_URL${NC}"

# Create temp directory
TEMP_DIR=$(mktemp -d)
ZIP_PATH="$TEMP_DIR/DatadogSDK.zip"

cleanup() {
    if [ -d "$TEMP_DIR" ]; then
        echo -e "${GRAY}Cleaning up temp files...${NC}"
        rm -rf "$TEMP_DIR"
    fi
}

trap cleanup EXIT

# Download the zip file
echo -e "${CYAN}Downloading XCFrameworks...${NC}"
curl -L -o "$ZIP_PATH" "$DOWNLOAD_URL"

FILE_SIZE=$(du -h "$ZIP_PATH" | cut -f1)
echo -e "${GREEN}Downloaded: $FILE_SIZE${NC}"

# Extract the zip
echo -e "${CYAN}Extracting XCFrameworks...${NC}"
unzip -q "$ZIP_PATH" -d "$TEMP_DIR"

# List of expected frameworks
FRAMEWORKS=(
    "DatadogCore.xcframework"
    "DatadogInternal.xcframework"
    "DatadogRUM.xcframework"
    "DatadogLogs.xcframework"
    "DatadogTrace.xcframework"
    "DatadogCrashReporting.xcframework"
    "DatadogSessionReplay.xcframework"
    "DatadogWebViewTracking.xcframework"
    "DatadogFlags.xcframework"
    "OpenTelemetryApi.xcframework"
)

# Create artifacts directory
mkdir -p "$OUTPUT_PATH/artifacts"

# Copy frameworks to output directory
echo -e "${CYAN}Copying XCFrameworks to $OUTPUT_PATH/artifacts...${NC}"

for FRAMEWORK in "${FRAMEWORKS[@]}"; do
    SOURCE=$(find "$TEMP_DIR" -name "$FRAMEWORK" -type d | head -n 1)

    if [ -n "$SOURCE" ]; then
        DEST="$OUTPUT_PATH/artifacts/$FRAMEWORK"

        if [ -d "$DEST" ]; then
            echo -e "${YELLOW}  Removing existing: $FRAMEWORK${NC}"
            rm -rf "$DEST"
        fi

        echo -e "${GREEN}  Copying: $FRAMEWORK${NC}"
        cp -R "$SOURCE" "$DEST"
    else
        echo -e "${YELLOW}  Warning: Framework not found: $FRAMEWORK${NC}"
    fi
done

echo -e "\n${GREEN}Download complete!${NC}"
echo -e "${CYAN}XCFrameworks installed in: $OUTPUT_PATH${NC}"

echo -e "\n${CYAN}Next steps:${NC}"
echo "1. Generate bindings using Objective Sharpie or manually define APIs in ApiDefinition.cs"
echo "2. Build the iOS binding project: dotnet build $OUTPUT_PATH/Datadog.MAUI.iOS.Binding.csproj"
