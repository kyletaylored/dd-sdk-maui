#!/bin/bash

# Automated Android Binding Setup
#
# This script automates the process of setting up Android bindings by:
# 1. Fetching dependencies from Maven POM files
# 2. Building the project
# 3. Parsing build errors to determine Maven vs NuGet dependencies
# 4. Automatically updating .csproj files with required dependencies
# 5. Iterating until build succeeds
#
# Usage:
#   ./setup-android-bindings.sh [SDK_VERSION] [PROJECT_PATH]
#
# Example:
#   ./setup-android-bindings.sh 3.5.0 ../Datadog.MAUI.Android.Binding/dd-sdk-android-core

set -euo pipefail

SDK_VERSION="${1:-3.5.0}"
PROJECT_PATH="${2:-.}"
MAVEN_BASE="https://repo1.maven.org/maven2/com/datadoghq"

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

echo -e "${BLUE}=========================================="
echo "Automated Android Binding Setup"
echo "=========================================="
echo -e "SDK Version: ${GREEN}$SDK_VERSION${NC}"
echo -e "Project Path: ${GREEN}$PROJECT_PATH${NC}"
echo ""

# Parse POM dependencies (reuse from generate-android-dependencies.sh)
parse_pom_dependencies() {
  local pom_content=$1
  local dependencies=()
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
        if [[ -z "$scope" ]] || [[ "$scope" == "compile" ]] || [[ "$scope" == "runtime" ]]; then
          if [[ -n "$group_id" ]] && [[ -n "$artifact_id" ]] && [[ -n "$version" ]]; then
            # Skip test libraries
            if [[ ! "$group_id" =~ ^org\.junit\. ]] && \
               [[ ! "$group_id" =~ ^org\.mockito ]] && \
               [[ ! "$group_id" =~ ^org\.assertj ]] && \
               [[ ! "$artifact_id" =~ junit ]] && \
               [[ ! "$artifact_id" =~ mockito ]] && \
               [[ ! "$artifact_id" =~ assertj ]] && \
               [[ ! "$artifact_id" =~ elmyr ]] && \
               [[ "$group_id" != "com.github.xgouchet.Elmyr" ]] && \
               [[ "$group_id" != "dd-sdk-android.tools" ]]; then
              dependencies+=("$group_id:$artifact_id:$version")
            fi
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

# Parse build errors and extract missing dependencies
parse_build_errors() {
  local build_output=$1
  local -n maven_deps=$2
  local -n nuget_deps=$3
  local -n ignored_deps=$4

  while IFS= read -r line; do
    # XA4242: NuGet package available
    if [[ "$line" =~ error\ XA4242:\ Java\ dependency\ \'([^:]+):([^:]+):([^\']+)\'.*NuGet\ package\ \'([^\']+)\' ]]; then
      local group="${BASH_REMATCH[1]}"
      local artifact="${BASH_REMATCH[2]}"
      local version="${BASH_REMATCH[3]}"
      local nuget="${BASH_REMATCH[4]}"
      nuget_deps["$group:$artifact"]="$nuget:$version"

    # XA4241: No NuGet available, use Maven
    elif [[ "$line" =~ error\ XA4241:\ Java\ dependency\ \'([^:]+):([^:]+):([^\']+)\' ]]; then
      local group="${BASH_REMATCH[1]}"
      local artifact="${BASH_REMATCH[2]}"
      local version="${BASH_REMATCH[3]}"

      # Check if it's a test dependency that should be ignored
      if [[ "$group" =~ ^org\.junit\. ]] || \
         [[ "$group" =~ ^org\.mockito ]] || \
         [[ "$group" =~ ^org\.assertj ]] || \
         [[ "$artifact" =~ junit ]] || \
         [[ "$artifact" =~ mockito ]] || \
         [[ "$artifact" =~ assertj ]] || \
         [[ "$artifact" =~ elmyr ]] || \
         [[ "$group" == "com.github.xgouchet.Elmyr" ]] || \
         [[ "$group" == "dd-sdk-android.tools" ]]; then
        ignored_deps["$group:$artifact"]="$version"
      else
        maven_deps["$group:$artifact"]="$version"
      fi
    fi
  done <<< "$build_output"
}

# Update .csproj file with dependencies
update_csproj() {
  local csproj_path=$1
  local -n maven_deps=$2
  local -n nuget_deps=$3
  local -n ignored_deps=$4

  echo -e "${YELLOW}Updating $csproj_path...${NC}"

  # TODO: This would need XML manipulation - for now, output what needs to be added
  if [ ${#maven_deps[@]} -gt 0 ]; then
    echo -e "${GREEN}Maven dependencies to add:${NC}"
    for dep in "${!maven_deps[@]}"; do
      local version="${maven_deps[$dep]}"
      echo "    <AndroidMavenLibrary Include=\"$dep\" Version=\"$version\" Bind=\"false\" />"
    done
  fi

  if [ ${#nuget_deps[@]} -gt 0 ]; then
    echo -e "${GREEN}NuGet packages to add:${NC}"
    for dep in "${!nuget_deps[@]}"; do
      local pkg_version="${nuget_deps[$dep]}"
      IFS=':' read -r package version <<< "$pkg_version"
      echo "    <PackageReference Include=\"$package\" Version=\"$version\" />"
    done
  fi

  if [ ${#ignored_deps[@]} -gt 0 ]; then
    echo -e "${GREEN}Test dependencies to ignore:${NC}"
    for dep in "${!ignored_deps[@]}"; do
      local version="${ignored_deps[$dep]}"
      echo "    <AndroidIgnoredJavaDependency Include=\"$dep:$version\" />"
    done
  fi
}

# Main workflow
main() {
  cd "$PROJECT_PATH"

  # Get the artifact name from the directory name
  local artifact_name=$(basename "$PWD")
  local csproj_file="$artifact_name.csproj"

  if [[ ! -f "$csproj_file" ]]; then
    echo -e "${RED}Error: Could not find $csproj_file${NC}"
    exit 1
  fi

  echo -e "${BLUE}Step 1: Fetching POM dependencies for $artifact_name...${NC}"
  local pom_url="$MAVEN_BASE/$artifact_name/$SDK_VERSION/$artifact_name-$SDK_VERSION.pom"
  local pom_content=$(curl -f -L -s "$pom_url" 2>/dev/null || echo "")

  if [[ -z "$pom_content" ]]; then
    echo -e "${RED}Error: Failed to fetch POM from $pom_url${NC}"
    exit 1
  fi

  # Parse POM dependencies
  local pom_deps=()
  while IFS= read -r line; do
    [[ -n "$line" ]] && pom_deps+=("$line")
  done < <(parse_pom_dependencies "$pom_content")

  echo -e "${GREEN}Found ${#pom_deps[@]} runtime dependencies in POM${NC}"

  echo -e "${BLUE}Step 2: Attempting build...${NC}"
  local build_output=$(dotnet build -p:DatadogSdkVersion=$SDK_VERSION 2>&1 || true)

  # Check if build succeeded
  if echo "$build_output" | grep -q "Build succeeded"; then
    echo -e "${GREEN}Build succeeded!${NC}"
    return 0
  fi

  echo -e "${YELLOW}Build failed, analyzing errors...${NC}"

  # Parse build errors
  declare -A maven_deps
  declare -A nuget_deps
  declare -A ignored_deps
  parse_build_errors "$build_output" maven_deps nuget_deps ignored_deps

  echo ""
  echo -e "${BLUE}Step 3: Required dependencies:${NC}"
  update_csproj "$csproj_file" maven_deps nuget_deps ignored_deps

  echo ""
  echo -e "${YELLOW}=========================================="
  echo "Next Steps:"
  echo "==========================================${NC}"
  echo "1. Review the dependencies listed above"
  echo "2. Add them to $csproj_file"
  echo "3. Run this script again to verify"
  echo ""
  echo -e "${BLUE}Or use the output above to update your .csproj manually${NC}"
}

main "$@"
