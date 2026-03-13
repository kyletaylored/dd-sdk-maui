#!/bin/bash

# Validate NuGet package dependencies for Datadog MAUI SDK
#
# This script verifies that all module and meta-package dependencies
# are properly declared in their csproj files.
#
# Usage:
#   ./validate-dependencies.sh [output-format]
#
# Examples:
#   ./validate-dependencies.sh              # Default output
#   ./validate-dependencies.sh json         # JSON output

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(dirname "$SCRIPT_DIR")"
OUTPUT_FORMAT="${1:-text}"

# Colors for text output
GREEN='\033[0;32m'
CYAN='\033[0;36m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m'

# Validation results
TOTAL_CHECKS=0
PASSED_CHECKS=0
FAILED_CHECKS=0
WARNINGS=0

# Helper functions
check_file_exists() {
    local file="$1"
    if [ -f "$file" ]; then
        echo "true"
    else
        echo "false"
    fi
}

check_contains_dependency() {
    local file="$1"
    local dependency="$2"
    if grep -q "$dependency" "$file"; then
        echo "true"
    else
        echo "false"
    fi
}

validate_csproj() {
    local csproj_path="$1"
    local package_id="$2"
    local expected_dependencies=("${@:3}")
    
    TOTAL_CHECKS=$((TOTAL_CHECKS + 1))
    
    if [ ! -f "$csproj_path" ]; then
        if [ "$OUTPUT_FORMAT" = "text" ]; then
            echo -e "${RED}✗${NC} $package_id - Project file not found: $csproj_path"
        fi
        FAILED_CHECKS=$((FAILED_CHECKS + 1))
        return 1
    fi
    
    local failed_deps=()
    for dep in "${expected_dependencies[@]}"; do
        if ! grep -q "$dep" "$csproj_path"; then
            failed_deps+=("$dep")
        fi
    done
    
    if [ ${#failed_deps[@]} -eq 0 ]; then
        if [ "$OUTPUT_FORMAT" = "text" ]; then
            echo -e "${GREEN}✓${NC} $package_id - All dependencies declared"
        fi
        PASSED_CHECKS=$((PASSED_CHECKS + 1))
        return 0
    else
        if [ "$OUTPUT_FORMAT" = "text" ]; then
            echo -e "${RED}✗${NC} $package_id - Missing dependencies:"
            for dep in "${failed_deps[@]}"; do
                echo -e "    ${YELLOW}• $dep${NC}"
            done
        fi
        FAILED_CHECKS=$((FAILED_CHECKS + 1))
        return 1
    fi
}

if [ "$OUTPUT_FORMAT" = "text" ]; then
    echo -e "${CYAN}=====================================${NC}"
    echo -e "${CYAN}Validating NuGet Package Dependencies${NC}"
    echo -e "${CYAN}=====================================${NC}\n"
    
    echo -e "${CYAN}Checking iOS Meta-Package:${NC}"
fi

# iOS Meta-Package
validate_csproj \
    "$ROOT_DIR/Datadog.MAUI.iOS.Binding/Datadog.MAUI.iOS.Binding.csproj" \
    "Datadog.MAUI.iOS.Binding" \
    "Datadog.MAUI.iOS.Internal" \
    "Datadog.MAUI.iOS.Core" \
    "Datadog.MAUI.iOS.RUM" \
    "Datadog.MAUI.iOS.Logs" \
    "Datadog.MAUI.iOS.Trace" \
    "Datadog.MAUI.iOS.CrashReporting" \
    "Datadog.MAUI.iOS.SessionReplay" \
    "Datadog.MAUI.iOS.WebViewTracking" \
    "Datadog.MAUI.iOS.Flags"

if [ "$OUTPUT_FORMAT" = "text" ]; then
    echo -e "\n${CYAN}Checking Android Meta-Package:${NC}"
fi

# Android Meta-Package
validate_csproj \
    "$ROOT_DIR/Datadog.MAUI.Android.Binding/Datadog.MAUI.Android.Binding.csproj" \
    "Datadog.MAUI.Android.Binding" \
    "Datadog.MAUI.Android.Internal" \
    "Datadog.MAUI.Android.Core" \
    "Datadog.MAUI.Android.RUM" \
    "Datadog.MAUI.Android.Logs" \
    "Datadog.MAUI.Android.Trace" \
    "Datadog.MAUI.Android.NDK" \
    "Datadog.MAUI.Android.SessionReplay" \
    "Datadog.MAUI.Android.WebView" \
    "Datadog.MAUI.Android.Flags" \
    "Datadog.MAUI.Android.OkHttp"

if [ "$OUTPUT_FORMAT" = "text" ]; then
    echo -e "\n${CYAN}Checking Consumer Plugin Package:${NC}"
fi

# Consumer Plugin Package
validate_csproj \
    "$ROOT_DIR/Datadog.MAUI.Plugin/Datadog.MAUI.Plugin.csproj" \
    "Datadog.MAUI" \
    "Datadog.MAUI.iOS.Binding" \
    "Datadog.MAUI.Android.Binding" \
    "Microsoft.Maui.Controls"

# Additional checks for iOS Core binding
if [ "$OUTPUT_FORMAT" = "text" ]; then
    echo -e "\n${CYAN}Checking iOS Core Binding:${NC}"
fi

validate_csproj \
    "$ROOT_DIR/Datadog.MAUI.iOS.Binding/DatadogCore/DatadogCore.csproj" \
    "Datadog.MAUI.iOS.Core" \
    "DatadogInternal"

# Additional checks for Android Core binding
if [ "$OUTPUT_FORMAT" = "text" ]; then
    echo -e "\n${CYAN}Checking Android Core Binding:${NC}"
fi

validate_csproj \
    "$ROOT_DIR/Datadog.MAUI.Android.Binding/dd-sdk-android-core/dd-sdk-android-core.csproj" \
    "Datadog.MAUI.Android.Core" \
    "dd-sdk-android-internal" \
    "AndroidX" \
    "GoogleGson"

if [ "$OUTPUT_FORMAT" = "text" ]; then
    echo -e "\n${CYAN}=====================================${NC}"
    echo -e "${CYAN}Validation Summary${NC}"
    echo -e "${CYAN}=====================================${NC}\n"
    echo "  Total Checks: $TOTAL_CHECKS"
    echo -e "  ${GREEN}Passed: $PASSED_CHECKS${NC}"
    echo -e "  ${RED}Failed: $FAILED_CHECKS${NC}"
    
    if [ "$FAILED_CHECKS" -eq 0 ]; then
        echo -e "\n${GREEN}✓ All dependency checks passed!${NC}\n"
        exit 0
    else
        echo -e "\n${RED}✗ Some checks failed. Please review the dependencies.${NC}\n"
        exit 1
    fi
fi

# JSON output
if [ "$OUTPUT_FORMAT" = "json" ]; then
    echo "{\"total\": $TOTAL_CHECKS, \"passed\": $PASSED_CHECKS, \"failed\": $FAILED_CHECKS}"
    if [ "$FAILED_CHECKS" -eq 0 ]; then
        exit 0
    else
        exit 1
    fi
fi

