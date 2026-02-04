.PHONY: help build-android build-ios build-plugin build pack clean clean-all test status check-prereqs dev-setup \
        sample-ios sample-android sample-build-ios sample-build-android run-ios run-android \
        download-ios-frameworks sample-logs-android sample-logs-clear upload-symbols restore \
        publish-android publish-ios publish-all install-android run-android-release install-ios run-ios-release

# Default target
.DEFAULT_GOAL := help

# Colors for output
BLUE := \033[0;34m
GREEN := \033[0;32m
YELLOW := \033[1;33m
RED := \033[0;31m
NC := \033[0m # No Color

##@ General

help: ## Display this help message
	@awk 'BEGIN {FS = ":.*##"; printf "\n$(BLUE)Usage:$(NC)\n  make $(GREEN)<target>$(NC)\n"} /^[a-zA-Z_0-9-]+:.*?##/ { printf "  $(GREEN)%-25s$(NC) %s\n", $$1, $$2 } /^##@/ { printf "\n$(BLUE)%s$(NC)\n", substr($$0, 5) } ' $(MAKEFILE_LIST)

status: ## Show current build status and git state
	@echo "$(BLUE)==========================================="
	@echo "Project Status"
	@echo "==========================================$(NC)"
	@echo ""
	@echo "$(YELLOW).NET SDK:$(NC)"
	@dotnet --version
	@echo ""
	@echo "$(YELLOW)Git Status:$(NC)"
	@git status --short
	@echo ""

check-prereqs: ## Check if all prerequisites are installed
	@echo "$(BLUE)Checking prerequisites...$(NC)"
	@command -v dotnet >/dev/null 2>&1 || { echo "$(RED)✗ .NET SDK not found$(NC)"; exit 1; }
	@echo "$(GREEN)✓ .NET SDK found:$(NC) $$(dotnet --version)"
	@if [ "$$(uname)" = "Darwin" ]; then \
		command -v xcodebuild >/dev/null 2>&1 || { echo "$(RED)✗ Xcode not found (required for iOS)$(NC)"; exit 1; }; \
		echo "$(GREEN)✓ Xcode found:$(NC) $$(xcodebuild -version | head -n 1)"; \
	fi
	@echo "$(GREEN)✓ All prerequisites met$(NC)"

##@ Build

download-ios-frameworks: ## Download iOS XCFrameworks (macOS only)
	@if [ "$$(uname)" != "Darwin" ]; then \
		echo "$(RED)Error: iOS framework download requires macOS$(NC)"; \
		exit 1; \
	fi
	@SDK_VERSION=$$(grep -E '<DatadogiOSSdkVersion>.*</DatadogiOSSdkVersion>' Directory.Build.props | sed 's/.*<DatadogiOSSdkVersion>\(.*\)<\/DatadogiOSSdkVersion>.*/\1/'); \
	if [ -z "$$SDK_VERSION" ]; then \
		SDK_VERSION=$$(grep -E '<DatadogSdkVersion>.*</DatadogSdkVersion>' Directory.Build.props | sed 's/.*<DatadogSdkVersion>\(.*\)<\/DatadogSdkVersion>.*/\1/'); \
	fi; \
	VERSION_FILE="Datadog.MAUI.iOS.Binding/artifacts/.version"; \
	NEED_DOWNLOAD=false; \
	if [ ! -d "Datadog.MAUI.iOS.Binding/artifacts" ] || [ -z "$$(ls -A Datadog.MAUI.iOS.Binding/artifacts/*.xcframework 2>/dev/null)" ]; then \
		echo "$(YELLOW)XCFrameworks not found$(NC)"; \
		NEED_DOWNLOAD=true; \
	elif [ ! -f "$$VERSION_FILE" ]; then \
		echo "$(YELLOW)Version file not found$(NC)"; \
		NEED_DOWNLOAD=true; \
	elif [ "$$(cat $$VERSION_FILE)" != "$$SDK_VERSION" ]; then \
		echo "$(YELLOW)Version mismatch: have $$(cat $$VERSION_FILE), need $$SDK_VERSION$(NC)"; \
		NEED_DOWNLOAD=true; \
	else \
		echo "$(GREEN)✓ XCFrameworks already current (v$$SDK_VERSION)$(NC)"; \
	fi; \
	if [ "$$NEED_DOWNLOAD" = "true" ]; then \
		echo "$(BLUE)Downloading iOS XCFrameworks v$$SDK_VERSION...$(NC)"; \
		chmod +x scripts/download-ios-frameworks.sh; \
		if scripts/download-ios-frameworks.sh "$$SDK_VERSION"; then \
			echo "$$SDK_VERSION" > "$$VERSION_FILE"; \
			echo "$(GREEN)✓ XCFrameworks downloaded$(NC)"; \
		else \
			echo "$(RED)Failed to download XCFrameworks$(NC)"; \
			exit 1; \
		fi; \
	fi

build-ios: download-ios-frameworks ## Build iOS binding projects (macOS only)
	@echo "$(BLUE)Building iOS bindings...$(NC)"
	@if [ "$$(uname)" != "Darwin" ]; then \
		echo "$(RED)Error: iOS builds require macOS$(NC)"; \
		exit 1; \
	fi
	@cd Datadog.MAUI.iOS.Binding && \
	for module in DatadogInternal DatadogCore DatadogLogs DatadogRUM DatadogTrace DatadogCrashReporting DatadogSessionReplay DatadogWebViewTracking DatadogFlags OpenTelemetryApi; do \
		echo "  Building $$module..."; \
		dotnet build $$module/$$module.csproj --configuration Release --verbosity quiet || exit 1; \
	done
	@echo "$(GREEN)✓ iOS bindings built$(NC)"

build-android: ## Build Android binding projects
	@echo "$(BLUE)Building Android bindings...$(NC)"
	@cd Datadog.MAUI.Android.Binding && \
	for module in dd-sdk-android-internal dd-sdk-android-core dd-sdk-android-logs dd-sdk-android-rum dd-sdk-android-trace dd-sdk-android-ndk dd-sdk-android-session-replay dd-sdk-android-webview dd-sdk-android-flags dd-sdk-android-okhttp dd-sdk-android-okhttp-otel dd-sdk-android-trace-otel opentracing-api; do \
		echo "  Building $$module..."; \
		dotnet build $$module/$$module.csproj --configuration Release --verbosity quiet || exit 1; \
	done
	@echo "$(GREEN)✓ Android bindings built$(NC)"

build-plugin: ## Build the MAUI plugin project
	@echo "$(BLUE)Building MAUI Plugin...$(NC)"
	@dotnet restore Datadog.MAUI.Plugin/Datadog.MAUI.Plugin.csproj --verbosity quiet
	@dotnet build Datadog.MAUI.Plugin/Datadog.MAUI.Plugin.csproj --configuration Release --no-restore --verbosity minimal
	@echo "$(GREEN)✓ MAUI Plugin built$(NC)"

build-symbols: ## Build the Symbols upload plugin
	@echo "$(BLUE)Building Symbols plugin...$(NC)"
	@dotnet build Datadog.MAUI.Symbols/Datadog.MAUI.Symbols.csproj --configuration Release --nologo --verbosity minimal
	@echo "$(GREEN)✓ Symbols plugin built$(NC)"

build: build-android build-ios build-plugin build-symbols ## Build all projects (Android, iOS, Plugin, Symbols)

##@ Packaging

pack: build ## Build and pack all NuGet packages with proper dependency order
	@echo "$(BLUE)Creating NuGet packages...$(NC)"
	@chmod +x scripts/pack.sh
	@./scripts/pack.sh Release ./artifacts
	@echo "$(GREEN)✓ All packages created in ./artifacts$(NC)"

##@ Sample App

sample-ios: ## Build and run iOS sample app (Debug mode)
	@echo "$(BLUE)Building and running iOS sample app...$(NC)"
	@if [ "$$(uname)" != "Darwin" ]; then \
		echo "$(RED)Error: iOS sample requires macOS$(NC)"; \
		exit 1; \
	fi
	@echo "$(YELLOW)Note: Credentials loaded from appsettings.Development.json$(NC)"
	@cd samples/DatadogMauiSample && \
		dotnet build -f net9.0-ios -c Debug && \
		dotnet build -f net9.0-ios -c Debug -t:Run
	@echo "$(GREEN)✓ iOS sample app launched$(NC)"

sample-android: ## Build and run Android sample app (Debug mode)
	@echo "$(BLUE)Building and running Android sample app...$(NC)"
	@echo "$(YELLOW)Note: Credentials loaded from appsettings.Development.json$(NC)"
	@cd samples/DatadogMauiSample && \
		dotnet build -f net9.0-android -c Debug && \
		dotnet build -f net9.0-android -c Debug -t:Run
	@echo "$(GREEN)✓ Android sample app launched$(NC)"

sample-build-ios: ## Build iOS sample in Debug mode (uses ProjectReference)
	@echo "$(BLUE)Building iOS sample app (Debug)...$(NC)"
	@echo "$(YELLOW)Note: Credentials loaded from appsettings.Development.json$(NC)"
	@cd samples/DatadogMauiSample && \
		dotnet restore && \
		dotnet build -f net9.0-ios -c Debug
	@echo "$(GREEN)✓ iOS sample built$(NC)"

sample-build-android: ## Build Android sample in Debug mode (uses ProjectReference)
	@echo "$(BLUE)Building Android sample app (Debug)...$(NC)"
	@echo "$(YELLOW)Note: Credentials loaded from appsettings.Development.json$(NC)"
	@cd samples/DatadogMauiSample && \
		dotnet restore && \
		dotnet build -f net9.0-android -c Debug
	@echo "$(GREEN)✓ Android sample built$(NC)"

run-ios: ## Build and run iOS sample app (loads credentials from appsettings.Development.json)
	@echo "$(BLUE)Building and running iOS sample...$(NC)"
	@if [ "$$(uname)" != "Darwin" ]; then \
		echo "$(RED)Error: iOS sample requires macOS$(NC)"; \
		exit 1; \
	fi
	@if [ ! -f samples/DatadogMauiSample/appsettings.Development.json ]; then \
		echo "$(RED)❌ appsettings.Development.json not found$(NC)"; \
		echo "$(YELLOW)Create from template:$(NC)"; \
		echo "  cp samples/DatadogMauiSample/appsettings.Development.json.example samples/DatadogMauiSample/appsettings.Development.json"; \
		echo "  # Edit appsettings.Development.json with your credentials"; \
		exit 1; \
	fi
	@echo "$(GREEN)✓ Found appsettings.Development.json$(NC)"
	@echo "$(YELLOW)Note: Credentials loaded from appsettings.Development.json$(NC)"
	@cd samples/DatadogMauiSample && \
		dotnet build -f net9.0-ios -c Debug && \
		dotnet build -f net9.0-ios -c Debug -t:Run
	@echo "$(GREEN)✓ iOS sample app launched$(NC)"

run-android: ## Build and run Android sample app (loads credentials from appsettings.Development.json)
	@echo "$(BLUE)Building and running Android sample...$(NC)"
	@if [ ! -f samples/DatadogMauiSample/appsettings.Development.json ]; then \
		echo "$(RED)❌ appsettings.Development.json not found$(NC)"; \
		echo "$(YELLOW)Create from template:$(NC)"; \
		echo "  cp samples/DatadogMauiSample/appsettings.Development.json.example samples/DatadogMauiSample/appsettings.Development.json"; \
		echo "  # Edit appsettings.Development.json with your credentials"; \
		exit 1; \
	fi
	@echo "$(GREEN)✓ Found appsettings.Development.json$(NC)"
	@echo "$(YELLOW)Note: Credentials loaded from appsettings.Development.json$(NC)"
	@cd samples/DatadogMauiSample && \
		dotnet build -f net9.0-android -c Debug && \
		dotnet build -f net9.0-android -c Debug -t:Run
	@echo "$(GREEN)✓ Android sample app launched$(NC)"

sample-logs-android: ## View Android logs (WARN and up: WARN, ERROR, FATAL)
	@echo "$(BLUE)Viewing Android logs (WARN, ERROR, FATAL only)...$(NC)"
	@echo "$(YELLOW)Press Ctrl+C to exit$(NC)"
	@if command -v adb >/dev/null 2>&1; then \
		adb logcat -v color '*:W'; \
	else \
		echo "$(RED)❌ adb not found in PATH$(NC)"; \
		echo "Try: ~/Library/Android/sdk/platform-tools/adb logcat -v color '*:W'"; \
	fi

sample-logs-android-filtered: ## View Android logs (filtered for Datadog and app only)
	@echo "$(BLUE)Viewing Android logs (filtering for Datadog and app output)...$(NC)"
	@echo "$(YELLOW)Press Ctrl+C to exit. For all logs use: make sample-logs-android$(NC)"
	@if command -v adb >/dev/null 2>&1; then \
		adb logcat -v color '*:*' | grep --color=never -E "\[Datadog\]|mono-stdout|DatadogMauiSample|DOTNET|FATAL|AndroidRuntime"; \
	else \
		echo "$(RED)❌ adb not found in PATH$(NC)"; \
		echo "Try: ~/Library/Android/sdk/platform-tools/adb logcat | grep '\\[Datadog\\]'"; \
	fi

sample-logs-clear: ## Clear Android logs
	@echo "$(BLUE)Clearing Android logs...$(NC)"
	@if command -v adb >/dev/null 2>&1; then \
		adb logcat -c; \
		echo "$(GREEN)✓ Logs cleared$(NC)"; \
	else \
		echo "$(RED)❌ adb not found in PATH$(NC)"; \
	fi

##@ Symbol Upload & Release Publishing

publish-android: ## Publish Android sample in Release mode with symbols (APK for testing)
	@echo "$(BLUE)Publishing Android sample with symbols...$(NC)"
	@if [ ! -f samples/DatadogMauiSample/appsettings.Development.json ]; then \
		echo "$(RED)❌ appsettings.Development.json not found$(NC)"; \
		echo "$(YELLOW)Create from template:$(NC)"; \
		echo "  cp samples/DatadogMauiSample/appsettings.Development.json.example samples/DatadogMauiSample/appsettings.Development.json"; \
		exit 1; \
	fi
	@echo "$(YELLOW)Building Symbols package in Release...$(NC)"
	@dotnet build Datadog.MAUI.Symbols/Datadog.MAUI.Symbols.csproj -c Release --nologo -v q
	@if [ -z "$$DD_API_KEY" ]; then \
		echo "$(YELLOW)⚠️  DD_API_KEY not set - symbol upload may fail$(NC)"; \
	fi
	@cd samples/DatadogMauiSample && \
		dotnet publish -f net9.0-android -c Release -v normal \
			-p:AndroidPackageFormat=apk \
			-p:DatadogSymbolsApiKey="$$DD_API_KEY" \
			$$( [ -n "$$DD_BUILD_FLAVOR" ] && echo -p:DatadogSymbolsFlavor=\"$$DD_BUILD_FLAVOR\" )
	@echo "$(GREEN)✓ Android app published with symbols$(NC)"
	@echo "$(YELLOW)Output: samples/DatadogMauiSample/bin/Release/net9.0-android/publish/$(NC)"
	@MAPPING_FILE=$$(find samples/DatadogMauiSample -name "mapping.txt" -type f 2>/dev/null | head -1); \
	APK_FILE=$$(find samples/DatadogMauiSample/bin/Release/net9.0-android -name "*.apk" -type f 2>/dev/null | head -1); \
	if [ -n "$$MAPPING_FILE" ]; then \
		echo "$(GREEN)✓ Mapping file: $$MAPPING_FILE$(NC)"; \
	else \
		echo "$(YELLOW)⚠️  No mapping.txt found$(NC)"; \
	fi; \
	if [ -n "$$APK_FILE" ]; then \
		echo "$(GREEN)✓ APK file: $$APK_FILE$(NC)"; \
		echo "$(YELLOW)Install with: make install-android$(NC)"; \
	fi

publish-android-staging:
	@DD_BUILD_FLAVOR=staging $(MAKE) publish-android

publish-android-prod:
	@DD_BUILD_FLAVOR=production $(MAKE) publish-android

publish-android-dev:
	@if [ -z "$$USER" ]; then \
		echo "$(RED)❌ USER environment variable not set$(NC)"; \
		exit 1; \
	fi
	@SAFE_USER=$$(echo "$$USER" | tr '.@' '--'); \
	echo "$(BLUE)Publishing Android app with dev-$$SAFE_USER flavor...$(NC)"; \
	DD_BUILD_FLAVOR=dev-$$SAFE_USER $(MAKE) publish-android

install-android: ## Install published Android APK to connected device/emulator
	@echo "$(BLUE)Installing Android APK...$(NC)"
	@APK_FILE=$$(find samples/DatadogMauiSample/bin/Release/net9.0-android -name "*-Signed.apk" -type f 2>/dev/null | head -1); \
	if [ -z "$$APK_FILE" ]; then \
		echo "$(RED)❌ No APK file found. Run 'make publish-android' first$(NC)"; \
		exit 1; \
	fi; \
	if ! command -v adb >/dev/null 2>&1; then \
		echo "$(RED)❌ adb not found in PATH$(NC)"; \
		exit 1; \
	fi; \
	echo "$(YELLOW)Installing $$APK_FILE$(NC)"; \
	adb install -r "$$APK_FILE" && \
	echo "$(GREEN)✓ APK installed$(NC)" && \
	echo "$(YELLOW)Launch with: adb shell am start -n com.datadog.datadog_maui_shopist.demo/crc64f2aa0ed48bd5d29c.MainActivity$(NC)"

run-android-release: ## Install and run published Android APK
	@echo "$(BLUE)Installing and running Android APK...$(NC)"
	@APK_FILE=$$(find samples/DatadogMauiSample/bin/Release/net9.0-android -name "*-Signed.apk" -type f 2>/dev/null | head -1); \
	if [ -z "$$APK_FILE" ]; then \
		echo "$(RED)❌ No APK file found. Run 'make publish-android' first$(NC)"; \
		exit 1; \
	fi; \
	if ! command -v adb >/dev/null 2>&1; then \
		echo "$(RED)❌ adb not found in PATH$(NC)"; \
		exit 1; \
	fi; \
	echo "$(YELLOW)Installing $$APK_FILE$(NC)"; \
	adb install -r "$$APK_FILE" && \
	echo "$(GREEN)✓ APK installed$(NC)" && \
	echo "$(YELLOW)Launching app...$(NC)" && \
	adb shell am start -n com.datadog.datadog_maui_shopist.demo/crc64f2aa0ed48bd5d29c.MainActivity && \
	echo "$(GREEN)✓ App launched$(NC)"

test-ios-symbols: ## Test iOS symbol upload via MSBuild target (macOS only)
	@echo "$(BLUE)Testing iOS symbol upload...$(NC)"
	@if [ "$$(uname)" != "Darwin" ]; then \
		echo "$(RED)Error: iOS operations require macOS$(NC)"; \
		exit 1; \
	fi
	@if [ ! -f samples/DatadogMauiSample/appsettings.Development.json ]; then \
		echo "$(RED)❌ appsettings.Development.json not found$(NC)"; \
		exit 1; \
	fi
	@echo "$(YELLOW)Building Symbols package in Release...$(NC)"
	@dotnet build Datadog.MAUI.Symbols/Datadog.MAUI.Symbols.csproj -c Release --nologo -v q
	@cd samples/DatadogMauiSample && \
		dotnet publish -f net9.0-ios -c Release -v normal \
			-p:CreatePackage=false \
			-p:BuildIpa=false \
			-p:DatadogSymbolsApiKey="$$DD_API_KEY" \
			$$( [ -n "$$DD_BUILD_FLAVOR" ] && echo -p:DatadogSymbolsFlavor=\"$$DD_BUILD_FLAVOR\" )
	@echo "$(GREEN)✓ iOS symbol upload test complete$(NC)"

publish-ios: ## Publish iOS sample in Release mode with symbols (macOS only)
	@echo "$(BLUE)Publishing iOS sample with symbols...$(NC)"
	@if [ "$$(uname)" != "Darwin" ]; then \
		echo "$(RED)Error: iOS publish requires macOS$(NC)"; \
		exit 1; \
	fi
	@if [ ! -f samples/DatadogMauiSample/appsettings.Development.json ]; then \
		echo "$(RED)❌ appsettings.Development.json not found$(NC)"; \
		exit 1; \
	fi
	@echo "$(YELLOW)Building Symbols package in Release...$(NC)"
	@dotnet build Datadog.MAUI.Symbols/Datadog.MAUI.Symbols.csproj -c Release --nologo -v q
	@if [ -z "$$DD_API_KEY" ]; then \
		echo "$(YELLOW)⚠️  DD_API_KEY not set - symbol upload may fail$(NC)"; \
	fi
	@cd samples/DatadogMauiSample && \
		dotnet publish -f net9.0-ios -c Release -v normal \
			-p:RuntimeIdentifier=ios-arm64 \
			-p:CodesignKey="Apple Development" \
			-p:CodesignProvision="Automatic" \
			-p:DatadogSymbolsApiKey="$$DD_API_KEY" \
			$$( [ -n "$$DD_BUILD_FLAVOR" ] && echo -p:DatadogSymbolsFlavor=\"$$DD_BUILD_FLAVOR\" )
	@echo "$(GREEN)✓ iOS app published with symbols$(NC)"

publish-ios-staging:
	@DD_BUILD_FLAVOR=staging $(MAKE) publish-ios

publish-ios-prod:
	@DD_BUILD_FLAVOR=production $(MAKE) publish-ios

publish-ios-dev:
	@if [ -z "$$USER" ]; then \
		echo "$(RED)❌ USER environment variable not set$(NC)"; \
		exit 1; \
	fi
	@DD_BUILD_FLAVOR=dev-$$USER $(MAKE) publish-ios

install-ios: ## Install published iOS app to running simulator (macOS only)
	@echo "$(BLUE)Installing iOS app to simulator...$(NC)"
	@if [ "$$(uname)" != "Darwin" ]; then \
		echo "$(RED)Error: iOS operations require macOS$(NC)"; \
		exit 1; \
	fi
	@APP_DIR=$$(find samples/DatadogMauiSample/bin/Release/net9.0-ios/iossimulator-arm64 -type d -name "*.app" 2>/dev/null | head -1); \
	if [ -z "$$APP_DIR" ]; then \
		echo "$(RED)❌ No .app bundle found. Run 'make publish-ios' first$(NC)"; \
		exit 1; \
	fi; \
	SIMULATOR_ID=$$(xcrun simctl list devices booted | grep -E "iPhone|iPad" | head -1 | sed 's/.*(\([^)]*\)).*/\1/'); \
	if [ -z "$$SIMULATOR_ID" ]; then \
		echo "$(RED)❌ No booted simulator found. Start a simulator first$(NC)"; \
		exit 1; \
	fi; \
	echo "$(YELLOW)Installing to simulator $$SIMULATOR_ID$(NC)"; \
	xcrun simctl install $$SIMULATOR_ID "$$APP_DIR" && \
	echo "$(GREEN)✓ App installed to simulator$(NC)" && \
	echo "$(YELLOW)Launch with: make run-ios-release$(NC)"

run-ios-release: ## Install and launch published iOS app in simulator (macOS only)
	@echo "$(BLUE)Installing and launching iOS app...$(NC)"
	@if [ "$$(uname)" != "Darwin" ]; then \
		echo "$(RED)Error: iOS operations require macOS$(NC)"; \
		exit 1; \
	fi
	@APP_DIR=$$(find samples/DatadogMauiSample/bin/Release/net9.0-ios/iossimulator-arm64 -type d -name "*.app" 2>/dev/null | head -1); \
	if [ -z "$$APP_DIR" ]; then \
		echo "$(RED)❌ No .app bundle found. Run 'make publish-ios' first$(NC)"; \
		exit 1; \
	fi; \
	SIMULATOR_ID=$$(xcrun simctl list devices booted | grep -E "iPhone|iPad" | head -1 | sed 's/.*(\([^)]*\)).*/\1/'); \
	if [ -z "$$SIMULATOR_ID" ]; then \
		echo "$(RED)❌ No booted simulator found. Start a simulator first$(NC)"; \
		exit 1; \
	fi; \
	BUNDLE_ID="com.datadog.datadog-maui-shopist.demo"; \
	echo "$(YELLOW)Installing to simulator $$SIMULATOR_ID$(NC)"; \
	xcrun simctl install $$SIMULATOR_ID "$$APP_DIR" && \
	echo "$(GREEN)✓ App installed$(NC)" && \
	echo "$(YELLOW)Launching app...$(NC)" && \
	xcrun simctl launch $$SIMULATOR_ID $$BUNDLE_ID && \
	echo "$(GREEN)✓ App launched$(NC)"

publish-all: publish-android publish-ios ## Publish both Android and iOS samples with symbols

upload-symbols: ## Upload both Android and iOS symbols to Datadog
	@echo "$(BLUE)Uploading symbols to Datadog...$(NC)"
	@if [ -z "$(DD_API_KEY)" ]; then \
		echo "$(RED)Error: DD_API_KEY environment variable not set$(NC)"; \
		echo "Set it with: export DD_API_KEY=your_api_key"; \
		exit 1; \
	fi
	@MAPPING_FILE=$$(find samples/DatadogMauiSample/bin/Release -name "mapping.txt" | head -1); \
	DSYM_DIR=$$(find samples/DatadogMauiSample/bin/Release -type d -name "*.app.dSYM" | head -1); \
	if [ -z "$$MAPPING_FILE" ] && [ -z "$$DSYM_DIR" ]; then \
		echo "$(RED)Error: No symbol files found. Build in Release mode first.$(NC)"; \
		echo "Run: make pack && cd samples/DatadogMauiSample && dotnet build -c Release"; \
		exit 1; \
	fi; \
	CMD="npx @datadog/datadog-ci flutter-symbols upload --service-name shopist-maui-demo --version 1.0"; \
	if [ -n "$$MAPPING_FILE" ]; then \
		echo "$(YELLOW)Found Android mapping: $$MAPPING_FILE$(NC)"; \
		CMD="$$CMD --android-mapping --android-mapping-location \"$$MAPPING_FILE\""; \
	fi; \
	if [ -n "$$DSYM_DIR" ]; then \
		echo "$(YELLOW)Found iOS dSYM: $$DSYM_DIR$(NC)"; \
		CMD="$$CMD --ios-dsyms --ios-dsyms-location \"$$DSYM_DIR\""; \
	fi; \
	eval $$CMD
	@echo "$(GREEN)✓ Symbols uploaded$(NC)"

##@ Testing

test: ## Run unit tests
	@echo "$(BLUE)Running tests...$(NC)"
	@dotnet test Datadog.MAUI.Plugin.Tests/Datadog.MAUI.Plugin.Tests.csproj --configuration Release
	@echo "$(GREEN)✓ Tests completed$(NC)"

##@ Cleanup

clean: ## Clean build artifacts and bin/obj directories
	@echo "$(BLUE)Cleaning build artifacts...$(NC)"
	@rm -rf local-packages temp-packages-* artifacts-* release-packages-*
	@find . -type d \( -name "bin" -o -name "obj" \) -not -path "*/\.*" | xargs rm -rf 2>/dev/null || true
	@echo "$(GREEN)✓ Clean complete$(NC)"

clean-all: clean ## Deep clean including XCFrameworks and generated files
	@echo "$(BLUE)Deep cleaning...$(NC)"
	@rm -rf Datadog.MAUI.iOS.Binding/artifacts/*.xcframework 2>/dev/null || true
	@rm -rf Datadog.MAUI.iOS.Binding/*/Generated 2>/dev/null || true
	@find . -name "*.nupkg" -not -path "*/\.*" -delete 2>/dev/null || true
	@echo "$(GREEN)✓ Deep clean complete$(NC)"

##@ Development

dev-setup: check-prereqs ## Set up development environment
	@echo "$(BLUE)Setting up development environment...$(NC)"
	@dotnet workload install android
	@if [ "$$(uname)" = "Darwin" ]; then \
		dotnet workload install ios; \
		echo "$(GREEN)✓ iOS workload installed$(NC)"; \
	fi
	@dotnet restore Datadog.MAUI.sln
	@echo "$(GREEN)✓ Development environment ready$(NC)"

restore: ## Restore all NuGet packages
	@echo "$(BLUE)Restoring NuGet packages...$(NC)"
	@dotnet restore Datadog.MAUI.sln
	@dotnet restore Datadog.MAUI.Android.Binding/Datadog.MAUI.Android.Binding.sln
	@if [ "$$(uname)" = "Darwin" ]; then \
		dotnet restore Datadog.MAUI.iOS.Binding/Datadog.MAUI.iOS.Binding.sln; \
	fi
	@dotnet restore samples/DatadogMauiSample/DatadogMauiSample.csproj
	@echo "$(GREEN)✓ All packages restored$(NC)"

##@ Binding Generation

generate-android-deps: ## Analyze Maven POM and generate dependency entries
	@echo "$(BLUE)Analyzing Android Maven dependencies...$(NC)"
	@if [ -z "$(MODULE)" ] || [ -z "$(VERSION)" ]; then \
		echo "$(RED)Error: MODULE and VERSION parameters required$(NC)"; \
		echo "Usage: make generate-android-deps MODULE=<name> VERSION=<version>"; \
		echo "Example: make generate-android-deps MODULE=dd-sdk-android-core VERSION=3.5.0"; \
		exit 1; \
	fi
	@chmod +x scripts/generate-android-dependencies.sh
	@./scripts/generate-android-dependencies.sh $(MODULE) $(VERSION)

setup-android-binding: ## Analyze build errors and suggest dependency fixes
	@echo "$(BLUE)Setting up Android binding...$(NC)"
	@if [ -z "$(PROJECT)" ]; then \
		echo "$(RED)Error: PROJECT parameter required$(NC)"; \
		echo "Usage: make setup-android-binding PROJECT=<path> [VERSION=<version>]"; \
		echo "Example: make setup-android-binding PROJECT=Datadog.MAUI.Android.Binding/dd-sdk-android-core"; \
		exit 1; \
	fi
	@chmod +x scripts/setup-android-bindings.sh
	@./scripts/setup-android-bindings.sh $(VERSION) $(PROJECT)

generate-ios-bindings: ## Generate iOS bindings using Objective Sharpie (macOS only)
	@echo "$(BLUE)Generating iOS bindings with Objective Sharpie...$(NC)"
	@if [ "$$(uname)" != "Darwin" ]; then \
		echo "$(RED)Error: iOS binding generation requires macOS$(NC)"; \
		exit 1; \
	fi
	@chmod +x scripts/generate-ios-bindings-sharpie.sh && ./scripts/generate-ios-bindings-sharpie.sh
	@echo "$(GREEN)✓ iOS bindings generated$(NC)"
