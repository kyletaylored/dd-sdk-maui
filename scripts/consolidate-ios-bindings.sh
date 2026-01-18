#!/bin/bash

# Consolidates generated iOS bindings from Objective Sharpie into main binding files
#
# Usage:
#   ./consolidate-ios-bindings.sh

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

GENERATED_DIR="$PROJECT_ROOT/Datadog.MAUI.iOS.Binding/Generated"
TARGET_DIR="$PROJECT_ROOT/Datadog.MAUI.iOS.Binding"
API_DEF="$TARGET_DIR/ApiDefinition.cs"
STRUCTS_ENUMS="$TARGET_DIR/StructsAndEnums.cs"

echo -e "${CYAN}Consolidating iOS bindings...${NC}\n"

# Check if generated directory exists
if [ ! -d "$GENERATED_DIR" ]; then
    echo -e "${RED}Error: Generated directory not found: $GENERATED_DIR${NC}"
    echo "Run ./generate-ios-bindings-sharpie.sh first"
    exit 1
fi

# Backup existing files if they exist
if [ -f "$API_DEF" ] && [ -s "$API_DEF" ]; then
    BACKUP="$API_DEF.backup.$(date +%Y%m%d_%H%M%S)"
    cp "$API_DEF" "$BACKUP"
    echo -e "${YELLOW}Backed up existing ApiDefinition.cs to $(basename $BACKUP)${NC}"
fi

if [ -f "$STRUCTS_ENUMS" ] && [ -s "$STRUCTS_ENUMS" ]; then
    BACKUP="$STRUCTS_ENUMS.backup.$(date +%Y%m%d_%H%M%S)"
    cp "$STRUCTS_ENUMS" "$BACKUP"
    echo -e "${YELLOW}Backed up existing StructsAndEnums.cs to $(basename $BACKUP)${NC}"
fi

# Start building ApiDefinition.cs
echo -e "${CYAN}Building ApiDefinition.cs...${NC}"

cat > "$API_DEF" << 'HEADER'
using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Datadog.iOS
{
HEADER

# Consolidate all ApiDefinitions.cs files
for FRAMEWORK_DIR in "$GENERATED_DIR"/*; do
    if [ -d "$FRAMEWORK_DIR" ]; then
        API_FILE="$FRAMEWORK_DIR/ApiDefinitions.cs"
        if [ -f "$API_FILE" ]; then
            FRAMEWORK_NAME=$(basename "$FRAMEWORK_DIR")
            echo -e "  Adding $FRAMEWORK_NAME..."

            # Extract content between namespace braces, skip usings and namespace declaration
            sed -n '/^namespace/,${
                /^namespace/d
                /^{$/d
                /^}$/d
                /^using /d
                p
            }' "$API_FILE" >> "$API_DEF"

            # Add a separator comment
            echo "" >> "$API_DEF"
            echo "	// ========================================" >> "$API_DEF"
            echo "	// End of $FRAMEWORK_NAME" >> "$API_DEF"
            echo "	// ========================================" >> "$API_DEF"
            echo "" >> "$API_DEF"
        fi
    fi
done

# Close the namespace
echo "}" >> "$API_DEF"

echo -e "${GREEN}✓ ApiDefinition.cs created${NC}\n"

# Start building StructsAndEnums.cs
echo -e "${CYAN}Building StructsAndEnums.cs...${NC}"

cat > "$STRUCTS_ENUMS" << 'HEADER'
using System;
using Foundation;
using ObjCRuntime;

namespace Datadog.iOS
{
HEADER

# Consolidate all StructsAndEnums.cs files
FOUND_CONTENT=false
for FRAMEWORK_DIR in "$GENERATED_DIR"/*; do
    if [ -d "$FRAMEWORK_DIR" ]; then
        STRUCTS_FILE="$FRAMEWORK_DIR/StructsAndEnums.cs"
        if [ -f "$STRUCTS_FILE" ] && [ -s "$STRUCTS_FILE" ]; then
            # Check if file has actual content (more than just namespace)
            if grep -q "enum\|struct\|interface" "$STRUCTS_FILE"; then
                FRAMEWORK_NAME=$(basename "$FRAMEWORK_DIR")
                echo -e "  Adding $FRAMEWORK_NAME..."
                FOUND_CONTENT=true

                # Extract content between namespace braces
                sed -n '/^namespace/,${
                    /^namespace/d
                    /^{$/d
                    /^}$/d
                    /^using /d
                    p
                }' "$STRUCTS_FILE" >> "$STRUCTS_ENUMS"

                # Add a separator comment
                echo "" >> "$STRUCTS_ENUMS"
                echo "	// ========================================" >> "$STRUCTS_ENUMS"
                echo "	// End of $FRAMEWORK_NAME" >> "$STRUCTS_ENUMS"
                echo "	// ========================================" >> "$STRUCTS_ENUMS"
                echo "" >> "$STRUCTS_ENUMS"
            fi
        fi
    fi
done

# Close the namespace
echo "}" >> "$STRUCTS_ENUMS"

if [ "$FOUND_CONTENT" = true ]; then
    echo -e "${GREEN}✓ StructsAndEnums.cs created${NC}\n"
else
    echo -e "${YELLOW}⚠ No enums or structs found (file created but may be empty)${NC}\n"
fi

# Count the generated types
API_TYPES=$(grep -c "^	interface\|^	class\|^	\[Protocol\]" "$API_DEF" || echo "0")
ENUM_TYPES=$(grep -c "^	enum\|^	struct" "$STRUCTS_ENUMS" || echo "0")

echo -e "${GREEN}Consolidation complete!${NC}"
echo -e "${CYAN}Summary:${NC}"
echo -e "  API types: $API_TYPES"
echo -e "  Enums/Structs: $ENUM_TYPES"
echo -e "\n${YELLOW}Next steps:${NC}"
echo "1. Review $API_DEF and $STRUCTS_ENUMS"
echo "2. Search for [Verify] attributes and fix them"
echo "3. Resolve any namespace conflicts"
echo "4. Build the iOS binding project: dotnet build Datadog.MAUI.iOS.Binding/Datadog.MAUI.iOS.Binding.csproj"
