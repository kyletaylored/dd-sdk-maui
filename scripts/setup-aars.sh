#!/usr/bin/env bash
#
# Smart AAR/JAR Setup - Unified dependency management
#
# This script intelligently manages Android dependencies by:
# 1. Reading dependency configuration from dependencies.conf
# 2. Reading POM files from Maven Central to discover dependencies
# 3. Downloading only required AARs/JARs based on configuration
# 4. Placing them in the correct binding project directories
#
# Usage:
#   ./setup-aars.sh [SDK_VERSION]
#
# If SDK_VERSION is not provided, it reads from dd-sdk-android submodule
#
# Configuration:
#   Edit src/Android/dependencies.yaml to control which dependencies to download
#
# Requirements:
#   yq (YAML processor) - Install via: brew install yq (macOS) or snap install yq (Linux)

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
DD_SDK_ROOT="$REPO_ROOT/dd-sdk-android"

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

MAVEN_URL="https://repo1.maven.org/maven2"

# Determine SDK version
if [ $# -eq 0 ]; then
  if [ -d "$DD_SDK_ROOT" ]; then
    cd "$DD_SDK_ROOT"
    SDK_VERSION=$(git describe --tags --abbrev=0 2>/dev/null || echo "")
    cd - > /dev/null
    if [ -z "$SDK_VERSION" ]; then
      echo -e "${RED}Error: Could not determine SDK version from submodule${NC}"
      echo "Usage: $0 [SDK_VERSION]"
      exit 1
    fi
  else
    echo -e "${RED}Error: dd-sdk-android submodule not found${NC}"
    echo "Usage: $0 [SDK_VERSION]"
    exit 1
  fi
else
  SDK_VERSION="$1"
fi

echo -e "${BLUE}=========================================="
echo "Smart AAR/JAR Setup"
echo "=========================================="
echo -e "SDK Version: ${GREEN}$SDK_VERSION${NC}"
echo ""

# Function to download a file
download_file() {
  local url=$1
  local output_path=$2
  local description=$3

  if curl -f -L -s "$url" -o "$output_path" 2>/dev/null; then
    echo -e "${GREEN}✓ ${description}${NC}"
    return 0
  else
    echo -e "${RED}✗ Failed: ${description}${NC}"
    return 1
  fi
}

# Function to parse POM and extract compile dependencies
parse_pom_dependencies() {
  local pom_content=$1
  local dependencies=()

  # Extract compile and runtime scope dependencies (excluding test/provided)
  # This is a simplified parser - in production you'd want proper XML parsing
  local in_dependencies=false
  local in_dependency=false
  local group_id=""
  local artifact_id=""
  local version=""
  local scope=""

  while IFS= read -r line; do
    if [[ "$line" =~ \<dependencies\> ]]; then
      in_dependencies=true
    elif [[ "$line" =~ \</dependencies\> ]]; then
      in_dependencies=false
    elif [[ "$in_dependencies" == true ]]; then
      if [[ "$line" =~ \<dependency\> ]]; then
        in_dependency=true
        group_id=""
        artifact_id=""
        version=""
        scope=""
      elif [[ "$line" =~ \</dependency\> ]]; then
        # Include compile and runtime scope (exclude test/provided)
        if [[ -z "$scope" ]] || [[ "$scope" == "compile" ]] || [[ "$scope" == "runtime" ]]; then
          if [[ -n "$group_id" ]] && [[ -n "$artifact_id" ]] && [[ -n "$version" ]]; then
            dependencies+=("$group_id:$artifact_id:$version")
          fi
        fi
        in_dependency=false
      elif [[ "$in_dependency" == true ]]; then
        if [[ "$line" =~ \<groupId\>([^<]+)\</groupId\> ]]; then
          group_id="${BASH_REMATCH[1]}"
        elif [[ "$line" =~ \<artifactId\>([^<]+)\</artifactId\> ]]; then
          artifact_id="${BASH_REMATCH[1]}"
        elif [[ "$line" =~ \<version\>([^<]+)\</version\> ]]; then
          version="${BASH_REMATCH[1]}"
        elif [[ "$line" =~ \<scope\>([^<]+)\</scope\> ]]; then
          scope="${BASH_REMATCH[1]}"
        fi
      fi
    fi
  done <<< "$pom_content"

  printf '%s\n' "${dependencies[@]}"
}

# Configuration file path
DEPENDENCIES_YAML="$SCRIPT_DIR/dependencies.yaml"

# Function to check if a dependency is required
# Returns: 0 = download, 1 = use NuGet, 2 = skip
is_dependency_required() {
  local group_id=$1
  local artifact_id=$2

  if [[ ! -f "$DEPENDENCIES_YAML" ]]; then
    echo -e "${RED}Error: Configuration file not found: $DEPENDENCIES_YAML${NC}" >&2
    exit 1
  fi

  # Check download list - yq returns the object or null
  local download_check=$(yq e "[ .download[] | select(.groupId == \"$group_id\" and .artifactId == \"$artifact_id\") ] | length" "$DEPENDENCIES_YAML")
  if [[ "$download_check" -gt 0 ]]; then
    return 0  # Download
  fi

  # Check nuget list
  local nuget_check=$(yq e "[ .nuget[] | select(.groupId == \"$group_id\" and .artifactId == \"$artifact_id\") ] | length" "$DEPENDENCIES_YAML")
  if [[ "$nuget_check" -gt 0 ]]; then
    return 1  # Use NuGet
  fi

  # Check skip list (or not found - default to skip)
  return 2  # Skip
}

# Function to download AAR
download_aar() {
  local group_id=$1
  local artifact_id=$2
  local version=$3
  local output_dir=$4
  local output_name=$5

  mkdir -p "$output_dir"
  local url="${MAVEN_URL}/${group_id//.//}/${artifact_id}/${version}/${artifact_id}-${version}.aar"
  local output_path="${output_dir}/${output_name}"

  download_file "$url" "$output_path" "AAR: ${artifact_id}"
}

# Function to download JAR
download_jar() {
  local group_id=$1
  local artifact_id=$2
  local version=$3
  local output_dir=$4
  local output_name=$5

  mkdir -p "$output_dir"
  local url="${MAVEN_URL}/${group_id//.//}/${artifact_id}/${version}/${artifact_id}-${version}.jar"
  local output_path="${output_dir}/${output_name}"

  download_file "$url" "$output_path" "JAR: ${artifact_id}"
}

# Datadog modules configuration
# Using arrays instead of associative array to avoid bash 4.x requirements
DATADOG_MODULES=(
  "dd-sdk-android-core:Core"
  "dd-sdk-android-internal:Internal"
  "dd-sdk-android-logs:DatadogLogs"
  "dd-sdk-android-ndk:Ndk"
  "dd-sdk-android-rum:Rum"
  "dd-sdk-android-session-replay:SessionReplay"
  "dd-sdk-android-session-replay-material:SessionReplay.Material"
  "dd-sdk-android-trace:Trace"
  "dd-sdk-android-trace-otel:Trace.Otel"
  "dd-sdk-android-webview:WebView"
)

# Download Datadog modules
echo -e "${BLUE}Downloading Datadog Android SDK modules...${NC}"
echo ""

for module_entry in "${DATADOG_MODULES[@]}"; do
  IFS=':' read -r module binding_dir <<< "$module_entry"
  output_dir="$SCRIPT_DIR/Bindings/${binding_dir}/aars"

  echo -e "${YELLOW}Processing ${module}...${NC}"

  # Download the module AAR
  download_aar "com.datadoghq" "$module" "$SDK_VERSION" \
    "$output_dir" "${module}-release.aar"

  # Download and parse POM to get dependencies
  pom_url="${MAVEN_URL}/com/datadoghq/${module}/${SDK_VERSION}/${module}-${SDK_VERSION}.pom"
  pom_content=$(curl -f -L -s "$pom_url" 2>/dev/null || echo "")

  if [[ -n "$pom_content" ]]; then
    # Parse dependencies (compatible with macOS bash)
    deps=()
    while IFS= read -r line; do
      [[ -n "$line" ]] && deps+=("$line")
    done < <(parse_pom_dependencies "$pom_content")

    for dep in "${deps[@]}"; do
      if [[ -z "$dep" ]]; then continue; fi

      IFS=':' read -r dep_group dep_artifact dep_version <<< "$dep"

      # Check if we need this dependency
      # Temporarily disable set -e to allow non-zero returns
      set +e
      is_dependency_required "$dep_group" "$dep_artifact"
      result=$?
      set -e

      if [[ $result -eq 0 ]]; then
        # Need to download it
        echo -e "  ${BLUE}→ Dependency: ${dep_artifact}${NC}"

        # Try AAR first, fallback to JAR
        aar_url="${MAVEN_URL}/${dep_group//.//}/${dep_artifact}/${dep_version}/${dep_artifact}-${dep_version}.aar"
        if curl -f -L -s -I --max-time 5 "$aar_url" > /dev/null 2>&1; then
          download_aar "$dep_group" "$dep_artifact" "$dep_version" \
            "$output_dir" "${dep_artifact}-${dep_version}.aar" || true
        else
          download_jar "$dep_group" "$dep_artifact" "$dep_version" \
            "$output_dir" "${dep_artifact}-${dep_version}.jar" || true
        fi
      elif [[ $result -eq 1 ]]; then
        echo -e "  ${GREEN}→ ${dep_artifact} (using NuGet binding)${NC}"
      fi
    done
  fi

  echo ""
done

echo -e "${GREEN}=========================================="
echo "✓ Setup Complete!"
echo "==========================================${NC}"
echo ""
echo -e "${YELLOW}Summary:${NC}"
echo "- Datadog SDK modules: ${#DATADOG_MODULES[@]}"
echo "- Downloaded to: src/Android/Bindings/*/aars/"
echo "- Dependencies resolved from POM files"
echo ""
echo -e "${BLUE}Next steps:${NC}"
echo "1. Build bindings: dotnet build src/Android/AndroidDatadogBindings.sln"
echo "2. Create packages: ./scripts/build-local-android-packages.sh"
echo ""
