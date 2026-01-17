#!/bin/bash

# Generates AndroidMavenLibrary dependencies from Maven POM files
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

echo "Runtime dependencies (for .csproj):"
echo ""

# Parse dependencies
deps=()
while IFS= read -r line; do
  [[ -n "$line" ]] && deps+=("$line")
done < <(parse_pom_dependencies "$pom_content")

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

  # Determine if we should bind (only Datadog packages)
  if [[ "$dep_group" == "com.datadoghq" ]]; then
    bind_attr=""
  else
    bind_attr=' Bind="false"'
  fi

  echo "    <AndroidMavenLibrary Include=\"${dep_group}:${dep_artifact}\" Version=\"${dep_version}\"${bind_attr} />"
done

echo ""
echo "Note: Non-Datadog dependencies have Bind=\"false\" to avoid duplicate bindings"
echo "Note: Test dependencies (scope=test/provided) are excluded"
