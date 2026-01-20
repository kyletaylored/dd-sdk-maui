#!/bin/bash

# Generates AndroidMavenLibrary dependencies from Maven POM files
#
# This script analyzes Maven POM files and outputs dependencies in the format
# compatible with the centralized configuration architecture:
# - AndroidMavenLibrary entries for .csproj files (using $(DatadogSdkVersion))
# - PackageVersion entries for Directory.Packages.props
# - AndroidIgnoredJavaDependency entries for Directory.Build.targets
#
# See docs/new_build_pack.md and docs/android_dep_research.md for architecture details.
#
# Usage:
#   ./generate-android-dependencies.sh <artifact-name> <version>
#
# Example:
#   ./generate-android-dependencies.sh dd-sdk-android-core 3.5.0

set -e

ARTIFACT_NAME=$1
VERSION=$2
MAVEN_BASE="https://repo1.maven.org/maven2/com/datadoghq"

if [ -z "$ARTIFACT_NAME" ] || [ -z "$VERSION" ]; then
    echo "Usage: $0 <artifact-name> <version>"
    exit 1
fi

POM_URL="$MAVEN_BASE/$ARTIFACT_NAME/$VERSION/$ARTIFACT_NAME-$VERSION.pom"

echo "Fetching POM from: $POM_URL"
echo ""

# Function to parse POM and extract compile dependencies
parse_pom_dependencies() {
  local pom_content=$1
  local dependencies=()

  # Extract compile and runtime scope dependencies (excluding test/provided)
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

# Fetch POM content
pom_content=$(curl -f -L -s "$POM_URL" 2>/dev/null || echo "")

if [[ -z "$pom_content" ]]; then
    echo "Error: Failed to fetch POM from $POM_URL" >&2
    exit 1
fi

# Parse dependencies
deps=()
while IFS= read -r line; do
  [[ -n "$line" ]] && deps+=("$line")
done < <(parse_pom_dependencies "$pom_content")

# Collect dependencies by type
declare -a datadog_deps
declare -a maven_deps
declare -a androidx_deps
declare -a kotlin_deps
declare -a other_deps

for dep in "${deps[@]}"; do
  if [[ -z "$dep" ]]; then continue; fi

  IFS=':' read -r dep_group dep_artifact dep_version <<< "$dep"

  # Skip known test libraries (even if marked as runtime in POM)
  if [[ "$dep_group" =~ ^org\.junit\. ]] || \
     [[ "$dep_group" =~ ^org\.mockito ]] || \
     [[ "$dep_group" =~ ^org\.assertj ]] || \
     [[ "$dep_artifact" =~ junit ]] || \
     [[ "$dep_artifact" =~ mockito ]] || \
     [[ "$dep_artifact" =~ assertj ]] || \
     [[ "$dep_artifact" =~ elmyr ]] || \
     [[ "$dep_group" == "com.github.xgouchet.Elmyr" ]]; then
    continue
  fi

  # Categorize dependencies
  if [[ "$dep_group" == "com.datadoghq" ]]; then
    datadog_deps+=("$dep")
  elif [[ "$dep_group" == "org.jetbrains.kotlin" ]]; then
    kotlin_deps+=("$dep")
  elif [[ "$dep_group" =~ ^androidx\. ]] || [[ "$dep_group" =~ ^com\.android\. ]]; then
    androidx_deps+=("$dep")
  else
    other_deps+=("$dep")
  fi
done

# Output for .csproj file
echo "============================================"
echo "For .csproj file (AndroidMavenLibrary):"
echo "============================================"
echo ""
echo "  <ItemGroup>"

# Datadog dependencies (with binding, use $(DatadogSdkVersion))
if [ ${#datadog_deps[@]} -gt 0 ]; then
  echo "    <!-- Datadog Maven dependencies -->"
  for dep in "${datadog_deps[@]}"; do
    IFS=':' read -r dep_group dep_artifact dep_version <<< "$dep"
    echo "    <AndroidMavenLibrary Include=\"${dep_group}:${dep_artifact}\" Version=\"\$(DatadogSdkVersion)\" VerifyDependencies=\"false\" Bind=\"false\" />"
  done
fi

# Other Maven dependencies (no binding)
if [ ${#other_deps[@]} -gt 0 ]; then
  echo ""
  echo "    <!-- Other Maven dependencies -->"
  for dep in "${other_deps[@]}"; do
    IFS=':' read -r dep_group dep_artifact dep_version <<< "$dep"
    echo "    <AndroidMavenLibrary Include=\"${dep_group}:${dep_artifact}\" Version=\"${dep_version}\" VerifyDependencies=\"false\" Bind=\"false\" />"
  done
fi

echo "  </ItemGroup>"
echo ""

# Output for .csproj file (PackageReference - without versions, managed by CPM)
if [ ${#androidx_deps[@]} -gt 0 ]; then
  echo "============================================"
  echo "For .csproj file (PackageReference):"
  echo "============================================"
  echo ""
  echo "  <ItemGroup>"
  echo "    <!-- AndroidX and runtime dependencies as NuGet packages -->"
  for dep in "${androidx_deps[@]}"; do
    IFS=':' read -r dep_group dep_artifact dep_version <<< "$dep"
    # Map Maven coordinates to NuGet package names
    # This is a simplified mapping - may need manual adjustment
    if [[ "$dep_group" =~ ^androidx\. ]]; then
      # androidx.core:core-ktx -> Xamarin.AndroidX.Core.Ktx
      nuget_name="Xamarin.AndroidX.${dep_artifact^}"
      nuget_name="${nuget_name//-/.}"
      nuget_name="${nuget_name//Ktx/Ktx}"
      echo "    <PackageReference Include=\"${nuget_name}\" />"
    fi
  done
  echo "  </ItemGroup>"
  echo ""
  echo "Note: Versions are managed centrally in Directory.Packages.props"
  echo "      Add <PackageVersion> entries there (see below)"
  echo ""
fi

# Output for Directory.Packages.props (if needed for AndroidX)
if [ ${#androidx_deps[@]} -gt 0 ]; then
  echo "============================================"
  echo "For Directory.Packages.props (if not already present):"
  echo "============================================"
  echo ""
  echo "  <ItemGroup>"
  for dep in "${androidx_deps[@]}"; do
    IFS=':' read -r dep_group dep_artifact dep_version <<< "$dep"
    if [[ "$dep_group" =~ ^androidx\. ]]; then
      nuget_name="Xamarin.AndroidX.${dep_artifact^}"
      nuget_name="${nuget_name//-/.}"
      # Note: The actual NuGet version may differ from the Maven version
      echo "    <PackageVersion Include=\"${nuget_name}\" Version=\"${dep_version}\" />"
    fi
  done
  echo "  </ItemGroup>"
  echo ""
  echo "Note: Verify actual NuGet package versions at nuget.org"
  echo ""
fi

# Output Kotlin dependencies info
if [ ${#kotlin_deps[@]} -gt 0 ]; then
  echo "============================================"
  echo "Kotlin Dependencies (should be ignored):"
  echo "============================================"
  echo ""
  echo "These Kotlin dependencies are already handled centrally in Directory.Build.targets"
  echo "via AndroidIgnoredJavaDependency entries and Xamarin.Kotlin.* NuGet packages."
  echo ""
  echo "Found Kotlin dependencies:"
  for dep in "${kotlin_deps[@]}"; do
    IFS=':' read -r dep_group dep_artifact dep_version <<< "$dep"
    echo "  - ${dep_group}:${dep_artifact}:${dep_version}"
  done
  echo ""
  echo "See docs/android_dep_research.md for details on Kotlin dependency handling."
  echo ""
fi

echo "============================================"
echo "Notes:"
echo "============================================"
echo "1. Use \$(DatadogSdkVersion) for all Datadog packages"
echo "2. Add VerifyDependencies=\"false\" to all AndroidMavenLibrary entries"
echo "3. PackageReference items should NOT have Version attributes (managed by CPM)"
echo "4. Test dependencies are already ignored in Directory.Build.targets"
echo "5. Kotlin dependencies are already handled centrally"
