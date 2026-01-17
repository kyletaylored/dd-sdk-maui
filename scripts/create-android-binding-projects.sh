#!/bin/bash

# Creates Android binding projects for Datadog SDK
#
# Usage:
#   ./create-android-binding-projects.sh [binding_root_path]
#
# Example:
#   ./create-android-binding-projects.sh ../Datadog.MAUI.Android.Binding

set -e

BINDING_ROOT="${1:-.}"
SDK_VERSION="3.5.0"

# Colors
GREEN='\033[0;32m'
CYAN='\033[0;36m'
NC='\033[0m'

echo -e "${CYAN}Creating Android binding projects for Datadog SDK v$SDK_VERSION${NC}\n"

# Function to create a binding project
create_binding_project() {
    local artifact_name=$1
    local display_name=$2
    local description=$3
    shift 3
    local dependencies=("$@")

    local project_dir="$BINDING_ROOT/$artifact_name"
    mkdir -p "$project_dir/Transforms"

    echo -e "${GREEN}Creating: $artifact_name${NC}"

    # Create .csproj
    cat > "$project_dir/$artifact_name.csproj" << EOF
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net9.0-android;net10.0-android</TargetFrameworks>
    <SupportedOSPlatformVersion>21</SupportedOSPlatformVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>true</ImplicitUsings>
    <IsBindingProject>true</IsBindingProject>

    <PackageId>Datadog.MAUI.Android.$display_name</PackageId>
    <Authors>Datadog</Authors>
    <Description>$description</Description>
    <RepositoryType>git</RepositoryType>
    <RepositoryUrl>https://github.com/DataDog/dd-sdk-maui</RepositoryUrl>
    <PackageTags>.NET MAUI Android Datadog Mobile Binding</PackageTags>
  </PropertyGroup>

EOF

    # Add project references if any dependencies
    if [ ${#dependencies[@]} -gt 0 ]; then
        echo "  <ItemGroup>" >> "$project_dir/$artifact_name.csproj"
        for dep in "${dependencies[@]}"; do
            echo "    <ProjectReference Include=\"..\\$dep\\$dep.csproj\" />" >> "$project_dir/$artifact_name.csproj"
        done
        echo "  </ItemGroup>" >> "$project_dir/$artifact_name.csproj"
        echo "" >> "$project_dir/$artifact_name.csproj"
    fi

    # Add AndroidMavenLibrary
    cat >> "$project_dir/$artifact_name.csproj" << EOF
  <ItemGroup>
    <AndroidMavenLibrary Include="com.datadoghq:$artifact_name" Version="\$(DatadogSdkVersion)" />
  </ItemGroup>

  <!-- Metadata transforms for binding fixes -->
  <ItemGroup>
    <TransformFile Include="Transforms\\Metadata.xml" />
  </ItemGroup>
</Project>
EOF

    # Create Transforms/Metadata.xml
    cat > "$project_dir/Transforms/Metadata.xml" << 'EOF'
<metadata>
  <!--
    Add metadata transforms here to fix binding issues
    Example:
    <remove-node path="/api/package[@name='com.example']/class[@name='Foo']" />
  -->
</metadata>
EOF
}

# Create Internal (no dependencies)
create_binding_project \
    "dd-sdk-android-internal" \
    "Internal" \
    ".NET bindings for Datadog Android SDK Internal module"

# Create Core (depends on Internal)
create_binding_project \
    "dd-sdk-android-core" \
    "Core" \
    ".NET bindings for Datadog Android SDK Core module" \
    "dd-sdk-android-internal"

# Create RUM (depends on Core)
create_binding_project \
    "dd-sdk-android-rum" \
    "RUM" \
    ".NET bindings for Datadog Android SDK RUM (Real User Monitoring) module" \
    "dd-sdk-android-core"

# Create Logs (depends on Core)
create_binding_project \
    "dd-sdk-android-logs" \
    "Logs" \
    ".NET bindings for Datadog Android SDK Logs module" \
    "dd-sdk-android-core"

# Create Trace (depends on Core)
create_binding_project \
    "dd-sdk-android-trace" \
    "Trace" \
    ".NET bindings for Datadog Android SDK Trace (APM) module" \
    "dd-sdk-android-core"

# Create NDK (depends on Core)
create_binding_project \
    "dd-sdk-android-ndk" \
    "NDK" \
    ".NET bindings for Datadog Android SDK NDK (Native Crash Reporting) module" \
    "dd-sdk-android-core"

# Create SessionReplay (depends on Core)
create_binding_project \
    "dd-sdk-android-session-replay" \
    "SessionReplay" \
    ".NET bindings for Datadog Android SDK Session Replay module" \
    "dd-sdk-android-core"

# Create WebView (depends on Core)
create_binding_project \
    "dd-sdk-android-webview" \
    "WebView" \
    ".NET bindings for Datadog Android SDK WebView tracking module" \
    "dd-sdk-android-core"

# Create Flags (depends on Core)
create_binding_project \
    "dd-sdk-android-flags" \
    "Flags" \
    ".NET bindings for Datadog Android SDK Feature Flags module" \
    "dd-sdk-android-core"

echo -e "\n${GREEN}All binding projects created successfully!${NC}"
echo -e "${CYAN}Projects created: 9${NC}"
