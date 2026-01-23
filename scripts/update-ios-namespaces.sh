#!/bin/bash

# Updates iOS binding namespaces to remove redundant "Datadog" prefix
# Changes: Datadog.iOS.DatadogCore → Datadog.iOS.Core
#
# Usage:
#   ./update-ios-namespaces.sh

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

BINDING_DIR="$PROJECT_ROOT/Datadog.MAUI.iOS.Binding"

echo -e "${CYAN}Updating iOS binding namespaces to remove redundant 'Datadog' prefix...${NC}\n"

# Old and new namespaces (parallel arrays)
OLD_NAMESPACES=(
    "Datadog.iOS.DatadogCore"
    "Datadog.iOS.DatadogInternal"
    "Datadog.iOS.DatadogRUM"
    "Datadog.iOS.DatadogLogs"
    "Datadog.iOS.DatadogTrace"
    "Datadog.iOS.DatadogCrashReporting"
    "Datadog.iOS.DatadogSessionReplay"
    "Datadog.iOS.DatadogWebViewTracking"
    "Datadog.iOS.DatadogFlags"
)

NEW_NAMESPACES=(
    "Datadog.iOS.Core"
    "Datadog.iOS.Internal"
    "Datadog.iOS.RUM"
    "Datadog.iOS.Logs"
    "Datadog.iOS.Trace"
    "Datadog.iOS.CrashReporting"
    "Datadog.iOS.SessionReplay"
    "Datadog.iOS.WebViewTracking"
    "Datadog.iOS.Flags"
)

# Directories to process
DIRS=(
    "DatadogCore"
    "DatadogInternal"
    "DatadogRUM"
    "DatadogLogs"
    "DatadogTrace"
    "DatadogCrashReporting"
    "DatadogSessionReplay"
    "DatadogWebViewTracking"
    "DatadogFlags"
    "Generated/DatadogCore"
    "Generated/DatadogInternal"
    "Generated/DatadogRUM"
    "Generated/DatadogLogs"
    "Generated/DatadogTrace"
    "Generated/DatadogCrashReporting"
    "Generated/DatadogSessionReplay"
    "Generated/DatadogWebViewTracking"
)

# Process each directory
for DIR in "${DIRS[@]}"; do
    FULL_DIR="$BINDING_DIR/$DIR"

    if [ ! -d "$FULL_DIR" ]; then
        echo -e "${YELLOW}⚠ Skipping $DIR (not found)${NC}"
        continue
    fi

    echo -e "${CYAN}Processing $DIR...${NC}"

    # Find all .cs files in the directory
    find "$FULL_DIR" -name "*.cs" -type f | while read CS_FILE; do
        FILE_NAME=$(basename "$CS_FILE")

        # Skip files in obj/bin directories
        if [[ "$CS_FILE" == *"/obj/"* ]] || [[ "$CS_FILE" == *"/bin/"* ]]; then
            continue
        fi

        # Check if file contains any of our old namespaces
        NEEDS_UPDATE=false
        for OLD_NS in "${OLD_NAMESPACES[@]}"; do
            if grep -q "$OLD_NS" "$CS_FILE"; then
                NEEDS_UPDATE=true
                break
            fi
        done

        if [ "$NEEDS_UPDATE" = true ]; then
            echo -e "  Updating $FILE_NAME..."

            # Create a backup
            cp "$CS_FILE" "$CS_FILE.bak"

            # Replace all namespace occurrences (in order)
            for i in "${!OLD_NAMESPACES[@]}"; do
                OLD_NS="${OLD_NAMESPACES[$i]}"
                NEW_NS="${NEW_NAMESPACES[$i]}"
                sed -i '' "s|$OLD_NS|$NEW_NS|g" "$CS_FILE"
            done

            echo -e "  ${GREEN}✓${NC} Updated $FILE_NAME"
        fi
    done

    echo ""
done

echo -e "${GREEN}Namespace update complete!${NC}\n"

echo -e "${YELLOW}Next steps:${NC}"
echo "1. Review the changes in each project directory"
echo "2. Build the iOS bindings: dotnet build Datadog.MAUI.iOS.Binding/*.csproj"
echo "3. Update any references in:"
echo "   - Datadog.MAUI.Plugin/Platforms/iOS/*.cs"
echo "   - samples/DatadogMauiSample/Platforms/iOS/AppDelegate.cs"
echo "4. If everything works, remove .bak files:"
echo "   find Datadog.MAUI.iOS.Binding -name '*.bak' -delete"
echo ""
echo -e "${CYAN}Backup files created with .bak extension - delete them after verifying changes${NC}"
