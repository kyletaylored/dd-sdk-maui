#!/bin/bash

# Analyzes Datadog Android SDK dependencies from Maven POMs
#
# Usage:
#   ./analyze-android-dependencies.sh [version]
#
# Example:
#   ./analyze-android-dependencies.sh 3.5.0

set -e

VERSION="${1:-3.5.0}"
MAVEN_BASE="https://repo1.maven.org/maven2/com/datadoghq"

# Colors
GREEN='\033[0;32m'
CYAN='\033[0;36m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${CYAN}Analyzing Datadog Android SDK v$VERSION${NC}\n"

# Core packages (com.datadoghq group)
CORE_PACKAGES=(
    "dd-sdk-android-core"
    "dd-sdk-android-internal"
)

# Feature packages (com.datadoghq group)
FEATURE_PACKAGES=(
    "dd-sdk-android-rum"
    "dd-sdk-android-logs"
    "dd-sdk-android-trace"
    "dd-sdk-android-ndk"
    "dd-sdk-android-session-replay"
    "dd-sdk-android-webview"
    "dd-sdk-android-flags"
)

# Integration packages (com.datadoghq group)
INTEGRATION_PACKAGES=(
    "dd-sdk-android-okhttp"
    "dd-sdk-android-okhttp-otel"
    "dd-sdk-android-rum-coroutines"
    "dd-sdk-android-rum-debug-widget"
    "dd-sdk-android-trace-coroutines"
    "dd-sdk-android-trace-otel"
    "dd-sdk-android-rx"
    "dd-sdk-android-timber"
    "dd-sdk-android-glide"
    "dd-sdk-android-fresco"
    "dd-sdk-android-coil"
    "dd-sdk-android-coil3"
    "dd-sdk-android-compose"
    "dd-sdk-android-session-replay-compose"
    "dd-sdk-android-session-replay-material"
    "dd-sdk-android-sqldelight"
    "dd-sdk-android-cronet"
    "dd-sdk-android-tv"
)

ALL_PACKAGES=("${CORE_PACKAGES[@]}" "${FEATURE_PACKAGES[@]}" "${INTEGRATION_PACKAGES[@]}")

echo -e "${CYAN}Package Analysis:${NC}"
echo -e "${CYAN}Core Packages: ${#CORE_PACKAGES[@]}${NC}"
echo -e "${CYAN}Feature Packages: ${#FEATURE_PACKAGES[@]}${NC}"
echo -e "${CYAN}Integration Packages: ${#INTEGRATION_PACKAGES[@]}${NC}"
echo -e "${CYAN}Total: ${#ALL_PACKAGES[@]}${NC}\n"

# Function to parse POM dependencies
parse_pom_dependencies() {
    local package=$1
    local pom_url="$MAVEN_BASE/$package/$VERSION/$package-$VERSION.pom"

    echo -e "${CYAN}Analyzing: $package${NC}"

    # Download POM
    local pom_content=$(curl -sL "$pom_url")

    if [ -z "$pom_content" ]; then
        echo -e "${RED}  Failed to download POM${NC}"
        return 1
    fi

    # Extract Datadog dependencies
    local dd_deps=$(echo "$pom_content" | grep -A 2 '<groupId>com.datadoghq</groupId>' | grep '<artifactId>' | sed -E 's/.*<artifactId>([^<]+)<\/artifactId>.*/\1/' | grep -v 'com.datadoghq')

    if [ -n "$dd_deps" ]; then
        echo -e "${GREEN}  Datadog Dependencies:${NC}"
        echo "$dd_deps" | while read dep; do
            echo -e "${GREEN}    - $dep${NC}"
        done
    else
        echo -e "${YELLOW}  No Datadog dependencies${NC}"
    fi

    echo ""
}

# Analyze all packages
for package in "${ALL_PACKAGES[@]}"; do
    parse_pom_dependencies "$package"
done

echo -e "${GREEN}Analysis complete!${NC}"
echo -e "\n${CYAN}Next steps:${NC}"
echo "1. Create separate binding project for each package"
echo "2. Add ProjectReference elements based on dependencies"
echo "3. Create meta-package that references all"
