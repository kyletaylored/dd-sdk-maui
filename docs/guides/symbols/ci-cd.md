---
layout: default
title: CI/CD Integration
nav_order: 3
parent: Symbol Upload Plugin
grand_parent: Guides
---

# CI/CD Integration

Guide for integrating Datadog.MAUI.Symbols into your CI/CD pipelines.

## Overview

The symbols plugin works in any CI/CD environment that supports:
- .NET SDK
- Node.js and npm (for `datadog-ci`)
- Environment variables

## Prerequisites

### Required Tools

All CI/CD environments need:

```bash
# .NET SDK (6.0+)
dotnet --version

# Node.js and npm
node --version
npm --version
```

### Required Secrets

Configure these secrets in your CI/CD platform:

| Secret Name | Description | Example |
|-------------|-------------|---------|
| `DATADOG_API_KEY` | Datadog API key | `********************************` |
| `DATADOG_SITE` (optional) | Datadog site | `us5.datadoghq.com` |

## GitHub Actions

### Basic Setup

```yaml
name: Build and Deploy

on:
  push:
    branches: [ main ]

jobs:
  build:
    runs-on: macos-latest # Required for iOS builds

    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 8.0.x

      - name: Setup Node.js
        uses: actions/setup-node@v4
        with:
          node-version: '18'

      - name: Restore dependencies
        run: dotnet restore

      - name: Build and Upload Symbols (Android)
        env:
          DD_API_KEY: ${{ secrets.DATADOG_API_KEY }}
          DD_SITE: ${{ secrets.DATADOG_SITE }}
        run: |
          dotnet publish -f net8.0-android -c Release

      - name: Build and Upload Symbols (iOS)
        env:
          DD_API_KEY: ${{ secrets.DATADOG_API_KEY }}
          DD_SITE: ${{ secrets.DATADOG_SITE }}
        run: |
          dotnet publish -f net8.0-ios -c Release
```

### With MAUI Workload

```yaml
- name: Install MAUI Workload
  run: dotnet workload install maui

- name: Publish Android
  env:
    DD_API_KEY: ${{ secrets.DATADOG_API_KEY }}
  run: |
    dotnet publish -f net8.0-android -c Release \
      -p:AndroidSigningKeyStore=${{ secrets.ANDROID_KEYSTORE }} \
      -p:AndroidSigningStorePass=${{ secrets.KEYSTORE_PASSWORD }}
```

### Matrix Builds

Build multiple platforms in parallel:

```yaml
strategy:
  matrix:
    include:
      - platform: android
        os: ubuntu-latest
        framework: net8.0-android
      - platform: ios
        os: macos-latest
        framework: net8.0-ios

runs-on: ${{ matrix.os }}

steps:
  - name: Publish ${{ matrix.platform }}
    env:
      DD_API_KEY: ${{ secrets.DATADOG_API_KEY }}
    run: |
      dotnet publish -f ${{ matrix.framework }} -c Release
```

## Azure DevOps

### Pipeline YAML

```yaml
trigger:
  - main

pool:
  vmImage: 'macOS-latest'

variables:
  - group: Datadog-Secrets # Variable group with DD_API_KEY

steps:
  - task: UseDotNet@2
    displayName: 'Install .NET SDK'
    inputs:
      version: '8.0.x'

  - task: NodeTool@0
    displayName: 'Install Node.js'
    inputs:
      versionSpec: '18.x'

  - script: dotnet workload install maui
    displayName: 'Install MAUI Workload'

  - script: |
      dotnet publish -f net8.0-android -c Release
    displayName: 'Publish Android with Symbols'
    env:
      DD_API_KEY: $(DatadogApiKey)
      DD_SITE: $(DatadogSite)

  - script: |
      dotnet publish -f net8.0-ios -c Release
    displayName: 'Publish iOS with Symbols'
    env:
      DD_API_KEY: $(DatadogApiKey)
      DD_SITE: $(DatadogSite)
```

### Variable Groups

Create a variable group named `Datadog-Secrets`:

1. Go to **Pipelines** → **Library**
2. Create new **Variable group**
3. Add variables:
   - `DatadogApiKey` (mark as secret)
   - `DatadogSite` (optional)

## GitLab CI

### .gitlab-ci.yml

```yaml
stages:
  - build

variables:
  DOTNET_VERSION: "8.0"
  NODE_VERSION: "18"

build-android:
  stage: build
  image: mcr.microsoft.com/dotnet/sdk:8.0
  before_script:
    - apt-get update && apt-get install -y nodejs npm
    - dotnet workload install maui
  script:
    - dotnet publish -f net8.0-android -c Release
  variables:
    DD_API_KEY: $DATADOG_API_KEY
    DD_SITE: $DATADOG_SITE

build-ios:
  stage: build
  tags:
    - macos # Requires macOS runner
  before_script:
    - dotnet workload install maui
  script:
    - dotnet publish -f net8.0-ios -c Release
  variables:
    DD_API_KEY: $DATADOG_API_KEY
    DD_SITE: $DATADOG_SITE
```

### Protected Variables

1. Go to **Settings** → **CI/CD** → **Variables**
2. Add variables:
   - `DATADOG_API_KEY` (Protected, Masked)
   - `DATADOG_SITE` (Protected, optional)

## Jenkins

### Jenkinsfile

```groovy
pipeline {
    agent {
        label 'macos' // For iOS builds
    }

    environment {
        DD_API_KEY = credentials('datadog-api-key')
        DD_SITE = 'us5.datadoghq.com'
        DOTNET_ROOT = '/usr/local/share/dotnet'
    }

    stages {
        stage('Setup') {
            steps {
                sh 'dotnet --version'
                sh 'node --version'
                sh 'dotnet workload install maui'
            }
        }

        stage('Build Android') {
            steps {
                sh 'dotnet publish -f net8.0-android -c Release'
            }
        }

        stage('Build iOS') {
            steps {
                sh 'dotnet publish -f net8.0-ios -c Release'
            }
        }
    }
}
```

### Credentials Setup

1. Go to **Manage Jenkins** → **Credentials**
2. Add **Secret text** credential:
   - ID: `datadog-api-key`
   - Secret: Your Datadog API key

## CircleCI

### .circleci/config.yml

```yaml
version: 2.1

executors:
  macos-executor:
    macos:
      xcode: 14.3.0

jobs:
  build-symbols:
    executor: macos-executor
    steps:
      - checkout

      - run:
          name: Install .NET SDK
          command: |
            brew install dotnet-sdk

      - run:
          name: Install Node.js
          command: |
            brew install node

      - run:
          name: Install MAUI
          command: |
            dotnet workload install maui

      - run:
          name: Publish Android
          command: |
            dotnet publish -f net8.0-android -c Release
          environment:
            DD_API_KEY: $DATADOG_API_KEY

      - run:
          name: Publish iOS
          command: |
            dotnet publish -f net8.0-ios -c Release
          environment:
            DD_API_KEY: $DATADOG_API_KEY

workflows:
  version: 2
  build:
    jobs:
      - build-symbols
```

## App Center

### appcenter-build.sh

```bash
#!/usr/bin/env bash
set -e

# App Center automatically provides Node.js and .NET

# Install MAUI workload
dotnet workload install maui

# Set Datadog environment (from App Center environment variables)
export DD_API_KEY="$DATADOG_API_KEY"
export DD_SITE="$DATADOG_SITE"

# Publish with symbol upload
if [ "$APPCENTER_XCODE_PROJECT" ]; then
    # iOS build
    dotnet publish -f net8.0-ios -c Release
else
    # Android build
    dotnet publish -f net8.0-android -c Release
fi
```

Set environment variables in App Center:
1. Go to **Build** → **Build configuration**
2. Add **Environment variables**:
   - `DATADOG_API_KEY`
   - `DATADOG_SITE`

## Best Practices

### 1. Secret Management

Never hardcode secrets:

```xml
<!-- ❌ DON'T DO THIS -->
<DatadogApiKey>abcd1234...</DatadogApiKey>

<!-- ✅ DO THIS -->
<!-- DD_API_KEY set via CI/CD secrets -->
```

### 2. Conditional Upload

Only upload from main/release branches:

```yaml
# GitHub Actions
- name: Upload Symbols
  if: github.ref == 'refs/heads/main'
  env:
    DD_API_KEY: ${{ secrets.DATADOG_API_KEY }}
  run: dotnet publish -c Release
```

### 3. Dry Run for PRs

Test configuration in pull requests without uploading:

```yaml
- name: Test Symbol Upload (Dry Run)
  if: github.event_name == 'pull_request'
  run: dotnet publish -c Release -p:DatadogDryRun=true
```

### 4. Caching

Cache NuGet packages and Node modules:

```yaml
# GitHub Actions
- name: Cache NuGet packages
  uses: actions/cache@v3
  with:
    path: ~/.nuget/packages
    key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}

- name: Cache npm packages
  uses: actions/cache@v3
  with:
    path: ~/.npm
    key: ${{ runner.os }}-npm-${{ hashFiles('**/package-lock.json') }}
```

### 5. Build Artifacts

Save build artifacts for debugging:

```yaml
- name: Upload Build Artifacts
  if: failure()
  uses: actions/upload-artifact@v3
  with:
    name: build-logs
    path: |
      **/*.binlog
      **/msbuild.log
```

## Troubleshooting CI/CD

### Node.js Not Found

**Error**: `npx: command not found`

**Solution**: Install Node.js in your CI environment:

```yaml
- name: Setup Node.js
  uses: actions/setup-node@v4
  with:
    node-version: '18'
```

### API Key Not Set

**Error**: `DD_API_KEY is not set`

**Solution**: Verify secret is configured and accessible:

```yaml
- name: Debug Environment
  run: |
    echo "DD_API_KEY is set: $([[ -n "$DD_API_KEY" ]] && echo 'yes' || echo 'no')"
```

### Upload Timeout

**Error**: Upload times out in CI

**Solution**: Increase timeout or check network connectivity:

```yaml
- name: Publish with Timeout
  timeout-minutes: 15
  run: dotnet publish -c Release
```

### Symbol Files Not Found

**Error**: `mapping.txt not found` or `dSYM folder not found`

**Solution**: Verify Release configuration and obfuscation:

```yaml
- name: Verify Symbol Files
  run: |
    find . -name "mapping.txt" -o -name "*.dSYM"
```

## Validation

Test your CI/CD setup:

1. **Create a test branch**
2. **Push a change**
3. **Check CI logs** for:
   ```
   [Datadog] Uploading android symbols to Datadog...
   [Datadog] Successfully uploaded android symbols.
   ```
4. **Verify in Datadog** under Settings → Symbol Files

## Next Steps

- [Troubleshooting Guide](troubleshooting.html)
- [Configuration Reference](configuration.html)
- [Getting Started](getting-started.html)
