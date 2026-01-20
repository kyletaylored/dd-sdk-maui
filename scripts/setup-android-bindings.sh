#!/bin/bash

# Automated Android Binding Setup
#
# This script automates the process of setting up Android bindings by:
# 1. Fetching dependencies from Maven POM files
# 2. Building the project
# 3. Parsing build errors to determine Maven vs NuGet dependencies
# 4. Providing guidance on updates needed for centralized configuration:
#    - AndroidMavenLibrary entries for .csproj (using $(DatadogSdkVersion))
#    - PackageVersion entries for Directory.Packages.props
#    - AndroidIgnoredJavaDependency entries for Directory.Build.targets
#
# See docs/new_build_pack.md and docs/android_dep_research.md for architecture details.
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

# Output dependencies for centralized configuration
output_dependencies() {
  local csproj_path=$1
  local -n maven_deps=$2
  local -n nuget_deps=$3
  local -n ignored_deps=$4

  echo ""
  echo -e "${BLUE}============================================${NC}"
  echo -e "${BLUE}Dependencies Analysis${NC}"
  echo -e "${BLUE}============================================${NC}"
  echo ""

  # Maven dependencies for .csproj
  if [ ${#maven_deps[@]} -gt 0 ]; then
    echo -e "${GREEN}[1] AndroidMavenLibrary entries for $csproj_path:${NC}"
    echo ""
    echo "  <ItemGroup>"
    for dep in "${!maven_deps[@]}"; do
      local version="${maven_deps[$dep]}"
      local group_id="${dep%%:*}"

      # Use $(DatadogSdkVersion) for Datadog packages, hardcode version for others
      if [[ "$group_id" == "com.datadoghq" ]]; then
        echo "    <AndroidMavenLibrary Include=\"$dep\" Version=\"\$(DatadogSdkVersion)\" VerifyDependencies=\"false\" Bind=\"false\" />"
      else
        echo "    <AndroidMavenLibrary Include=\"$dep\" Version=\"$version\" VerifyDependencies=\"false\" Bind=\"false\" />"
      fi
    done
    echo "  </ItemGroup>"
    echo ""
  fi

  # NuGet packages for .csproj (WITHOUT version attributes)
  if [ ${#nuget_deps[@]} -gt 0 ]; then
    echo -e "${GREEN}[2] PackageReference entries for $csproj_path:${NC}"
    echo ""
    echo "  <ItemGroup>"
    echo "    <!-- AndroidX and runtime dependencies as NuGet packages -->"
    for dep in "${!nuget_deps[@]}"; do
      local pkg_version="${nuget_deps[$dep]}"
      IFS=':' read -r package version <<< "$pkg_version"
      echo "    <PackageReference Include=\"$package\" />"
    done
    echo "  </ItemGroup>"
    echo ""
    echo "Note: No Version attribute needed - managed by Directory.Packages.props"
    echo ""

    # Also output PackageVersion entries for Directory.Packages.props
    echo -e "${GREEN}[3] PackageVersion entries for Directory.Packages.props:${NC}"
    echo ""
    echo "Add these to Datadog.MAUI.Android.Binding/Directory.Packages.props if not already present:"
    echo ""
    echo "  <ItemGroup>"
    for dep in "${!nuget_deps[@]}"; do
      local pkg_version="${nuget_deps[$dep]}"
      IFS=':' read -r package version <<< "$pkg_version"
      echo "    <PackageVersion Include=\"$package\" Version=\"$version\" />"
    done
    echo "  </ItemGroup>"
    echo ""
  fi

  # Test dependencies to ignore - these should go in Directory.Build.targets
  if [ ${#ignored_deps[@]} -gt 0 ]; then
    echo -e "${GREEN}[4] Test dependencies (likely already in Directory.Build.targets):${NC}"
    echo ""
    echo "These are already centrally ignored in Directory.Build.targets:"
    echo ""
    for dep in "${!ignored_deps[@]}"; do
      local version="${ignored_deps[$dep]}"
      echo "  - $dep:$version"
    done
    echo ""
    echo "If missing, add to Datadog.MAUI.Android.Binding/Directory.Build.targets:"
    echo ""
    echo "  <ItemGroup Condition=\"'\$(IsBindingProject)' == 'true'\">"
    for dep in "${!ignored_deps[@]}"; do
      local version="${ignored_deps[$dep]}"
      echo "    <AndroidIgnoredJavaDependency Include=\"$dep:$version\" />"
    done
    echo "  </ItemGroup>"
    echo ""
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
  output_dependencies "$csproj_file" maven_deps nuget_deps ignored_deps

  echo ""
  echo -e "${YELLOW}============================================${NC}"
  echo -e "${YELLOW}Next Steps:${NC}"
  echo -e "${YELLOW}============================================${NC}"
  echo ""
  echo "1. Review the dependencies listed above"
  echo ""
  echo "2. Update files according to the centralized configuration:"
  echo "   - Add AndroidMavenLibrary entries to $csproj_file"
  echo "   - Add PackageReference entries to $csproj_file (NO Version attributes)"
  echo "   - Add PackageVersion entries to Directory.Packages.props"
  echo "   - Verify test dependencies in Directory.Build.targets"
  echo ""
  echo "3. Run this script again to verify the build"
  echo ""
  echo -e "${BLUE}See docs/new_build_pack.md and docs/android_dep_research.md for details${NC}"
}

main "$@"
