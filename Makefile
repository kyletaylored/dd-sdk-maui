.PHONY: help build-android build-ios build-plugin build pack clean clean-all test status check-prereqs dev-setup \
        sample-ios sample-android sample-build-ios sample-build-android download-ios-frameworks \
        sample-logs-android sample-logs-clear upload-symbols restore

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
	@SDK_VERSION=$$(grep -E '<DatadogSdkVersion>.*</DatadogSdkVersion>' Directory.Build.props | sed 's/.*<DatadogSdkVersion>\(.*\)<\/DatadogSdkVersion>.*/\1/'); \
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
		if scripts/download-ios-frameworks.sh; then \
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

build: build-android build-ios build-plugin ## Build all projects (Android, iOS, Plugin)

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
	@cd samples/DatadogMauiSample && \
		dotnet build -f net9.0-ios -c Debug && \
		dotnet build -f net9.0-ios -c Debug -t:Run
	@echo "$(GREEN)✓ iOS sample app launched$(NC)"

sample-android: ## Build and run Android sample app (Debug mode)
	@echo "$(BLUE)Building and running Android sample app...$(NC)"
	@cd samples/DatadogMauiSample && \
		dotnet build -f net10.0-android -c Debug && \
		dotnet build -f net10.0-android -c Debug -t:Run
	@echo "$(GREEN)✓ Android sample app launched$(NC)"

sample-build-ios: ## Build iOS sample in Debug mode (uses ProjectReference)
	@echo "$(BLUE)Building iOS sample app (Debug)...$(NC)"
	@cd samples/DatadogMauiSample && \
		dotnet restore && \
		dotnet build -f net9.0-ios -c Debug
	@echo "$(GREEN)✓ iOS sample built$(NC)"

sample-build-android: ## Build Android sample in Debug mode (uses ProjectReference)
	@echo "$(BLUE)Building Android sample app (Debug)...$(NC)"
	@cd samples/DatadogMauiSample && \
		dotnet restore && \
		dotnet build -f net10.0-android -c Debug
	@echo "$(GREEN)✓ Android sample built$(NC)"

sample-logs-android: ## View Android logs (filtered for Datadog and app)
	@echo "$(BLUE)Viewing Android logs (filtering for Datadog and app output)...$(NC)"
	@echo "$(YELLOW)Press Ctrl+C to exit$(NC)"
	@if command -v adb >/dev/null 2>&1; then \
		adb logcat | grep -E "\[Datadog\]|mono-stdout|DatadogMauiSample|DOTNET"; \
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

##@ Symbol Upload

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
