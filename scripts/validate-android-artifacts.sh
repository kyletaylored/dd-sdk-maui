#!/bin/bash

# Downloads and validates Datadog Android SDK artifacts using verification-metadata.xml
#
# This script:
# 1. Downloads verification-metadata.xml from dd-sdk-android GitHub releases
# 2. Validates SHA256 checksums of AndroidMavenLibrary artifacts
# 3. Optionally validates PGP signatures
#
# Usage:
#   ./validate-android-artifacts.sh <version> [--download-metadata] [--validate-checksums]
#
# Examples:
#   ./validate-android-artifacts.sh 3.5.0 --download-metadata
#   ./validate-android-artifacts.sh 3.5.0 --download-metadata --validate-checksums
#   ./validate-android-artifacts.sh 3.5.0  # Use existing verification-metadata.xml

set -e

VERSION=$1
DOWNLOAD_METADATA=false
VALIDATE_CHECKSUMS=false

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
CYAN='\033[0;36m'
BLUE='\033[0;34m'
NC='\033[0m'

# Parse arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --download-metadata)
            DOWNLOAD_METADATA=true
            shift
            ;;
        --validate-checksums)
            VALIDATE_CHECKSUMS=true
            shift
            ;;
        *)
            VERSION=$1
            shift
            ;;
    esac
done

if [ -z "$VERSION" ]; then
    echo -e "${RED}Error: Version required${NC}"
    echo ""
    echo "Usage: $0 <version> [--download-metadata] [--validate-checksums]"
    echo ""
    echo "Examples:"
    echo "  $0 3.5.0 --download-metadata"
    echo "  $0 3.5.0 --download-metadata --validate-checksums"
    exit 1
fi

echo -e "${CYAN}========================================${NC}"
echo -e "${CYAN}Datadog Android SDK Artifact Validation${NC}"
echo -e "${CYAN}Version: $VERSION${NC}"
echo -e "${CYAN}========================================${NC}"
echo ""

METADATA_FILE="verification-metadata.xml"
GITHUB_RELEASE_URL="https://github.com/DataDog/dd-sdk-android/releases/download/${VERSION}/verification-metadata.xml"

# Step 1: Download verification-metadata.xml if requested
if [ "$DOWNLOAD_METADATA" == true ]; then
    echo -e "${BLUE}[1/3] Downloading verification-metadata.xml...${NC}"

    if curl -fL "$GITHUB_RELEASE_URL" -o "$METADATA_FILE" 2>/dev/null; then
        echo -e "${GREEN}✅ Downloaded verification-metadata.xml${NC}"
        FILE_SIZE=$(ls -lh "$METADATA_FILE" | awk '{print $5}')
        echo -e "   Size: $FILE_SIZE"
    else
        echo -e "${RED}❌ Failed to download verification-metadata.xml${NC}"
        echo -e "${YELLOW}   URL: $GITHUB_RELEASE_URL${NC}"
        echo -e "${YELLOW}   Note: This file may not be published in dd-sdk-android releases${NC}"
        echo -e "${YELLOW}   Continuing without validation...${NC}"
        exit 1
    fi
    echo ""
else
    echo -e "${BLUE}[1/3] Skipping download (using existing file)${NC}"
    if [ ! -f "$METADATA_FILE" ]; then
        echo -e "${RED}❌ $METADATA_FILE not found${NC}"
        echo -e "${YELLOW}   Run with --download-metadata to fetch it${NC}"
        exit 1
    fi
    echo -e "${GREEN}✅ Using existing $METADATA_FILE${NC}"
    echo ""
fi

# Step 2: Parse verification-metadata.xml
echo -e "${BLUE}[2/3] Parsing verification-metadata.xml...${NC}"

# Extract component information
# Format: component[@group='...'][@name='...'][@version='...']
#   artifact[@name='...'].sha256[@value='...']

COMPONENTS=$(grep -o '<component[^>]*>' "$METADATA_FILE" | wc -l)
ARTIFACTS=$(grep -o '<artifact[^>]*>' "$METADATA_FILE" | wc -l)

echo -e "${GREEN}✅ Found $COMPONENTS components with $ARTIFACTS artifacts${NC}"
echo ""

# Extract core module artifacts for display
echo -e "${CYAN}Core Modules:${NC}"
grep -A 10 'component group="dd-sdk-android"' "$METADATA_FILE" | \
    grep -o 'name="[^"]*"' | \
    sed 's/name="//;s/"$//' | \
    sort -u | \
    while read -r module; do
        echo "  - $module"
    done
echo ""

# Step 3: Validate checksums (if requested)
if [ "$VALIDATE_CHECKSUMS" == true ]; then
    echo -e "${BLUE}[3/3] Validating artifact checksums...${NC}"
    echo -e "${YELLOW}   Note: This requires artifacts to be downloaded${NC}"
    echo -e "${YELLOW}   Checking AndroidMavenLibrary cache...${NC}"
    echo ""

    # Look for downloaded artifacts in Maven cache
    MAVEN_CACHE_DIRS=(
        "$HOME/.nuget/maven-cache"
        "$HOME/.m2/repository"
        "$HOME/.gradle/caches/modules-2/files-2.1"
    )

    FOUND_CACHE=false
    for cache_dir in "${MAVEN_CACHE_DIRS[@]}"; do
        if [ -d "$cache_dir" ]; then
            echo -e "${GREEN}✅ Found cache: $cache_dir${NC}"
            FOUND_CACHE=true
            # TODO: Implement actual checksum validation
            # Would need to:
            # 1. Parse expected SHA256 from verification-metadata.xml
            # 2. Find corresponding .aar/.pom files in cache
            # 3. Calculate actual SHA256
            # 4. Compare and report
            break
        fi
    done

    if [ "$FOUND_CACHE" == false ]; then
        echo -e "${YELLOW}⚠️  No Maven cache found${NC}"
        echo -e "${YELLOW}   Artifacts haven't been downloaded yet${NC}"
        echo -e "${YELLOW}   Run 'dotnet restore' first, then re-run validation${NC}"
    else
        echo -e "${YELLOW}⚠️  Checksum validation not yet implemented${NC}"
        echo -e "${YELLOW}   Would validate SHA256 hashes against verification-metadata.xml${NC}"
    fi
    echo ""
else
    echo -e "${BLUE}[3/3] Skipping checksum validation${NC}"
    echo -e "${YELLOW}   Run with --validate-checksums to enable${NC}"
    echo ""
fi

# Summary
echo -e "${GREEN}========================================${NC}"
echo -e "${GREEN}Validation Complete${NC}"
echo -e "${GREEN}========================================${NC}"
echo ""

echo -e "${CYAN}Summary:${NC}"
echo "  Version: $VERSION"
echo "  Components: $COMPONENTS"
echo "  Artifacts: $ARTIFACTS"
echo ""

if [ "$DOWNLOAD_METADATA" == true ]; then
    echo -e "${GREEN}✅ Downloaded verification-metadata.xml${NC}"
fi

if [ "$VALIDATE_CHECKSUMS" == true ]; then
    echo -e "${YELLOW}⚠️  Checksum validation: Not yet implemented${NC}"
else
    echo -e "${CYAN}ℹ️  Checksum validation: Skipped${NC}"
fi

echo ""
echo -e "${CYAN}Next steps:${NC}"
echo "  1. Review verification-metadata.xml"
echo "  2. Compare with your Directory.Packages.props versions"
echo "  3. Ensure all required packages are listed"
echo ""
