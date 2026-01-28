#!/bin/bash
set -e

# Test sample app build using local artifacts (simulates CI workflow)
# Usage: ./scripts/test-sample-build.sh

echo "=========================================="
echo "Testing Sample App Build (CI Simulation)"
echo "=========================================="
echo ""

# Check if artifacts exist
if [ ! -d "./artifacts" ] || [ -z "$(ls -A ./artifacts/*.nupkg 2>/dev/null)" ]; then
    echo "❌ Error: No packages found in ./artifacts/"
    echo "Run 'make pack' first to build packages"
    exit 1
fi

echo "✓ Found packages in ./artifacts/"
echo ""

# List available packages
echo "Available packages:"
ls -1 ./artifacts/*.nupkg | xargs -n1 basename
echo ""

# Create temporary nuget.config
echo "Creating nuget.config..."
cat > samples/DatadogMauiSample/nuget.config << 'EOF'
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="LocalArtifacts" value="../../artifacts" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
EOF

echo "✓ Created nuget.config"
echo ""

# Restore
echo "=========================================="
echo "Restoring sample app dependencies..."
echo "=========================================="
cd samples/DatadogMauiSample
dotnet restore DatadogMauiSample.csproj --configfile nuget.config -p:Configuration=Release
echo ""

# Build Android
echo "=========================================="
echo "Building Android (net10.0-android)..."
echo "=========================================="
dotnet build DatadogMauiSample.csproj -f net10.0-android -c Release --no-restore -v q
echo ""

# Build iOS (only on macOS)
if [ "$(uname)" = "Darwin" ]; then
    echo "=========================================="
    echo "Building iOS (net10.0-ios)..."
    echo "=========================================="
    dotnet build DatadogMauiSample.csproj -f net10.0-ios -c Release --no-restore -v q
    echo ""
else
    echo "⚠️  Skipping iOS build (not on macOS)"
    echo ""
fi

# Cleanup
cd ../..
rm -f samples/DatadogMauiSample/nuget.config

echo "=========================================="
echo "✅ Sample app build test completed!"
echo "=========================================="
