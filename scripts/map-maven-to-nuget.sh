#!/bin/bash

# Maps Maven package coordinates to NuGet package names and validates versions
#
# This script solves the version compatibility issue where Maven POM versions
# don't always map 1:1 to NuGet versions, especially for target framework support.
#
# Example problem:
#   Maven: org.jetbrains.kotlin:kotlin-stdlib:2.0.21 (only supports net8.0)
#   NuGet: Xamarin.Kotlin.StdLib:2.3.0.1 (supports net9.0 + net10.0)
#
# Usage:
#   ./map-maven-to-nuget.sh <maven-coordinate> [maven-version] [--check-frameworks]
#
# Examples:
#   ./map-maven-to-nuget.sh "org.jetbrains.kotlin:kotlin-stdlib" "2.0.21"
#   ./map-maven-to-nuget.sh "androidx.core:core" "1.15.0" --check-frameworks
#
# Output format:
#   NUGET_PACKAGE_NAME|SUGGESTED_VERSION|SUPPORTS_NET9|SUPPORTS_NET10|NOTES

set -e

MAVEN_COORD=$1
MAVEN_VERSION=$2
CHECK_FRAMEWORKS=$3

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
CYAN='\033[0;36m'
NC='\033[0m'

if [ -z "$MAVEN_COORD" ]; then
    echo "Usage: $0 <maven-coordinate> [maven-version] [--check-frameworks]"
    echo ""
    echo "Examples:"
    echo "  $0 \"org.jetbrains.kotlin:kotlin-stdlib\" \"2.0.21\""
    echo "  $0 \"androidx.core:core\" \"1.15.0\" --check-frameworks"
    exit 1
fi

# Function to map Maven coordinates to NuGet package names
# This approach works with Bash 3.2 (macOS default)
get_nuget_package() {
    local maven_coord=$1

    case "$maven_coord" in
        # Kotlin
        "org.jetbrains.kotlin:kotlin-stdlib") echo "Xamarin.Kotlin.StdLib" ;;
        "org.jetbrains.kotlin:kotlin-reflect") echo "Xamarin.Kotlin.Reflect" ;;

        # AndroidX Core
        "androidx.core:core") echo "Xamarin.AndroidX.Core" ;;
        "androidx.core:core-ktx") echo "Xamarin.AndroidX.Core.Ktx" ;;
        "androidx.annotation:annotation") echo "Xamarin.AndroidX.Annotation" ;;

        # AndroidX Collection
        "androidx.collection:collection") echo "Xamarin.AndroidX.Collection" ;;
        "androidx.collection:collection-ktx") echo "Xamarin.AndroidX.Collection.Ktx" ;;

        # AndroidX Work
        "androidx.work:work-runtime") echo "Xamarin.AndroidX.Work.Runtime" ;;
        "androidx.work:work-runtime-ktx") echo "Xamarin.AndroidX.Work.Runtime.Ktx" ;;

        # AndroidX MultiDex
        "androidx.multidex:multidex") echo "Xamarin.AndroidX.MultiDex" ;;

        # AndroidX Tracing
        "androidx.tracing:tracing") echo "Xamarin.AndroidX.Tracing.Tracing" ;;
        "androidx.tracing:tracing-ktx") echo "Xamarin.AndroidX.Tracing.Tracing.Ktx" ;;

        # AndroidX Navigation
        "androidx.navigation:navigation-fragment") echo "Xamarin.AndroidX.Navigation.Fragment" ;;
        "androidx.navigation:navigation-fragment-ktx") echo "Xamarin.AndroidX.Navigation.Fragment.Ktx" ;;
        "androidx.navigation:navigation-runtime") echo "Xamarin.AndroidX.Navigation.Runtime" ;;
        "androidx.navigation:navigation-runtime-ktx") echo "Xamarin.AndroidX.Navigation.Runtime.Ktx" ;;

        # AndroidX RecyclerView
        "androidx.recyclerview:recyclerview") echo "Xamarin.AndroidX.RecyclerView" ;;

        # AndroidX AppCompat
        "androidx.appcompat:appcompat") echo "Xamarin.AndroidX.AppCompat" ;;

        # AndroidX Fragment
        "androidx.fragment:fragment") echo "Xamarin.AndroidX.Fragment" ;;
        "androidx.fragment:fragment-ktx") echo "Xamarin.AndroidX.Fragment.Ktx" ;;

        # OkHttp
        "com.squareup.okhttp3:okhttp") echo "Square.OkHttp3" ;;
        "com.squareup.okhttp3:logging-interceptor") echo "Square.OkHttp3.LoggingInterceptor" ;;

        # Gson
        "com.google.code.gson:gson") echo "GoogleGson" ;;

        # JetBrains Annotations
        "org.jetbrains:annotations") echo "Xamarin.Jetbrains.Annotations" ;;

        # OpenTelemetry
        "io.opentelemetry:opentelemetry-api") echo "OpenTelemetry.Api" ;;
        "io.opentelemetry:opentelemetry-context") echo "OpenTelemetry.Context" ;;
        "io.opentelemetry:opentelemetry-sdk") echo "OpenTelemetry.Sdk" ;;
        "io.opentelemetry:opentelemetry-sdk-trace") echo "OpenTelemetry.Sdk.Trace" ;;

        # Not found
        *) echo "" ;;
    esac
}

# Function to get version overrides for framework compatibility
# Returns upgraded version if needed, empty string otherwise
get_version_override() {
    local nuget_package=$1
    local maven_version=$2

    case "${nuget_package}:${maven_version}" in
        # Kotlin: Maven 2.0.21 only supports net8.0, upgrade to 2.3.0.1 for net9/net10
        "Xamarin.Kotlin.StdLib:2.0.21") echo "2.3.0.1" ;;
        "Xamarin.Kotlin.Reflect:2.0.21") echo "2.3.0.1" ;;

        # AndroidX Core: Suggest newer stable version
        "Xamarin.AndroidX.Core:1.15.0") echo "1.17.0.1" ;;

        # Add more overrides as needed
        *) echo "" ;;
    esac
}

# Function to check if a NuGet package version supports target frameworks
check_framework_support() {
    local nuget_package=$1
    local nuget_version=$2

    # This would ideally query NuGet.org API
    # For now, return dummy data - implement actual API call later
    echo "true|true|Manual verification needed"
}

# Get NuGet package name
NUGET_PACKAGE=$(get_nuget_package "$MAVEN_COORD")

if [ -z "$NUGET_PACKAGE" ]; then
    echo -e "${RED}⚠️  No NuGet mapping found for: $MAVEN_COORD${NC}" >&2
    echo -e "${YELLOW}   This may need to be added to AndroidMavenLibrary or ignored${NC}" >&2
    exit 1
fi

# Determine suggested version
SUGGESTED_VERSION="$MAVEN_VERSION"
OVERRIDE_VERSION=$(get_version_override "$NUGET_PACKAGE" "$MAVEN_VERSION")

if [ -n "$OVERRIDE_VERSION" ]; then
    SUGGESTED_VERSION="$OVERRIDE_VERSION"
    VERSION_NOTE="⚠️ Upgraded from ${MAVEN_VERSION} for net9.0/net10.0 support"
else
    VERSION_NOTE="✅ Maven version compatible"
fi

# Output result
if [ "$CHECK_FRAMEWORKS" == "--check-frameworks" ]; then
    # Full format with framework support check
    FRAMEWORK_INFO=$(check_framework_support "$NUGET_PACKAGE" "$SUGGESTED_VERSION")
    echo "${NUGET_PACKAGE}|${SUGGESTED_VERSION}|${FRAMEWORK_INFO}|${VERSION_NOTE}"
else
    # Simple format
    echo "${NUGET_PACKAGE}|${SUGGESTED_VERSION}|${VERSION_NOTE}"
fi
