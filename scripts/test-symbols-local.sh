#!/bin/bash
set -e

echo "🔨 Building Datadog.MAUI.Symbols package..."
cd Datadog.MAUI.Symbols
dotnet build -c Release --nologo
dotnet pack -c Release --no-build --nologo

echo ""
echo "🧹 Clearing NuGet cache..."
dotnet nuget locals all --clear > /dev/null 2>&1

echo ""
echo "📦 Restoring sample app with local package..."
cd ../samples/DatadogMauiSample
dotnet restore --force --verbosity quiet > /dev/null 2>&1

echo ""
echo "✅ Ready to test! Run one of these commands:"
echo ""
echo "  # Android:"
echo "  dotnet publish -f net9.0-android -c Release"
echo ""
echo "  # iOS:"
echo "  dotnet publish -f net9.0-ios -c Release"
echo ""
