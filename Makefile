.PHONY: help build-android build-ios build-all clean clean-all test status check-prereqs dev-setup sample-ios sample-android

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

##@ iOS Build

build-ios: ## Build iOS binding projects
	@echo "$(BLUE)Building iOS bindings...$(NC)"
	@if [ "$$(uname)" != "Darwin" ]; then \
		echo "$(RED)Error: iOS builds require macOS$(NC)"; \
		exit 1; \
	fi
	@if [ ! -d "Datadog.MAUI.iOS.Binding/artifacts" ] || [ -z "$$(ls -A Datadog.MAUI.iOS.Binding/artifacts/*.xcframework 2>/dev/null)" ]; then \
		echo "$(YELLOW)Warning: No XCFrameworks found in artifacts directory$(NC)"; \
		echo "$(YELLOW)Please ensure XCFrameworks are in: Datadog.MAUI.iOS.Binding/artifacts/$(NC)"; \
		exit 1; \
	fi
	@cd Datadog.MAUI.iOS.Binding && \
	for module in DatadogInternal DatadogCore DatadogLogs DatadogRUM DatadogTrace DatadogCrashReporting DatadogSessionReplay DatadogWebViewTracking DatadogFlags OpenTelemetryApi; do \
		echo "  Building $$module..."; \
		dotnet build $$module/$$module.csproj --configuration Release --verbosity quiet || exit 1; \
	done
	@echo "$(GREEN)✓ iOS binding modules built successfully$(NC)"
	@echo "$(YELLOW)Note: Meta-package (Datadog.MAUI.iOS.Binding) skipped - only built during 'make pack'$(NC)"

build-ios-debug: ## Build iOS binding module projects in Debug mode
	@echo "$(BLUE)Building iOS binding modules (Debug)...$(NC)"
	@if [ "$$(uname)" != "Darwin" ]; then \
		echo "$(RED)Error: iOS builds require macOS$(NC)"; \
		exit 1; \
	fi
	@cd Datadog.MAUI.iOS.Binding && \
	for module in DatadogInternal DatadogCore DatadogLogs DatadogRUM DatadogTrace DatadogCrashReporting DatadogSessionReplay DatadogWebViewTracking DatadogFlags OpenTelemetryApi; do \
		echo "  Building $$module..."; \
		dotnet build $$module/$$module.csproj --configuration Debug --verbosity quiet || exit 1; \
	done
	@echo "$(GREEN)✓ iOS binding modules built successfully (Debug)$(NC)"

build-ios-errors: ## Build iOS binding modules showing only errors (no warnings)
	@echo "$(BLUE)Building iOS binding modules (errors only)...$(NC)"
	@if [ "$$(uname)" != "Darwin" ]; then \
		echo "$(RED)Error: iOS builds require macOS$(NC)"; \
		exit 1; \
	fi
	@cd Datadog.MAUI.iOS.Binding && \
	for module in DatadogInternal DatadogCore DatadogLogs DatadogRUM DatadogTrace DatadogCrashReporting DatadogSessionReplay DatadogWebViewTracking DatadogFlags OpenTelemetryApi; do \
		echo "  Checking $$module..."; \
		dotnet build $$module/$$module.csproj --configuration Release --verbosity quiet --nologo 2>&1 | grep -i "error" | grep -v "warning" || true; \
	done
	@echo "$(YELLOW)Note: If no errors shown above, build succeeded (warnings hidden)$(NC)"

pack-ios: build-ios ## Build and pack iOS NuGet packages (DEPRECATED: use 'make pack' instead)
	@echo "$(YELLOW)Warning: pack-ios is deprecated. Use 'make pack' for proper packaging.$(NC)"
	@echo "$(YELLOW)See docs/new_build_pack.md for architecture details.$(NC)"
	@echo ""
	@echo "$(BLUE)Creating iOS NuGet packages...$(NC)"
	@rm -rf ./local-packages
	@mkdir -p ./local-packages
	@dotnet pack Datadog.MAUI.iOS.Binding/Datadog.MAUI.iOS.Binding.sln --configuration Release --no-build --output ./local-packages
	@echo "$(GREEN)✓ iOS packages created$(NC)"
	@echo ""
	@echo "Packages location: ./local-packages/"
	@ls -lh ./local-packages/*.nupkg 2>/dev/null || true

##@ Android Build

build-android: ## Build Android binding module projects (excludes meta-package)
	@echo "$(BLUE)Building Android binding modules...$(NC)"
	@cd Datadog.MAUI.Android.Binding && \
	for module in dd-sdk-android-internal dd-sdk-android-core dd-sdk-android-logs dd-sdk-android-rum dd-sdk-android-trace dd-sdk-android-ndk dd-sdk-android-session-replay dd-sdk-android-webview dd-sdk-android-flags; do \
		echo "  Building $$module..."; \
		dotnet build $$module/$$module.csproj --configuration Release --verbosity quiet || exit 1; \
	done
	@echo "$(GREEN)✓ Android binding modules built successfully$(NC)"
	@echo "$(YELLOW)Note: Meta-package (Datadog.MAUI.Android.Binding) skipped - only built during 'make pack'$(NC)"

build-android-debug: ## Build Android binding module projects in Debug mode
	@echo "$(BLUE)Building Android binding modules (Debug)...$(NC)"
	@cd Datadog.MAUI.Android.Binding && \
	for module in dd-sdk-android-internal dd-sdk-android-core dd-sdk-android-logs dd-sdk-android-rum dd-sdk-android-trace dd-sdk-android-ndk dd-sdk-android-session-replay dd-sdk-android-webview dd-sdk-android-flags; do \
		echo "  Building $$module..."; \
		dotnet build $$module/$$module.csproj --configuration Debug --verbosity quiet || exit 1; \
	done
	@echo "$(GREEN)✓ Android binding modules built successfully (Debug)$(NC)"

build-android-errors: ## Build Android binding modules showing only errors (no warnings)
	@echo "$(BLUE)Building Android binding modules (errors only)...$(NC)"
	@cd Datadog.MAUI.Android.Binding && \
	for module in dd-sdk-android-internal dd-sdk-android-core dd-sdk-android-logs dd-sdk-android-rum dd-sdk-android-trace dd-sdk-android-ndk dd-sdk-android-session-replay dd-sdk-android-webview dd-sdk-android-flags; do \
		echo "  Checking $$module..."; \
		dotnet build $$module/$$module.csproj --configuration Release --verbosity quiet --nologo 2>&1 | grep -i "error" | grep -v "warning" || true; \
	done
	@echo "$(YELLOW)Note: If no errors shown above, build succeeded (warnings hidden)$(NC)"

pack-android: build-android ## Build and pack Android NuGet packages (DEPRECATED: use 'make pack' instead)
	@echo "$(YELLOW)Warning: pack-android is deprecated. Use 'make pack' for proper packaging.$(NC)"
	@echo "$(YELLOW)See docs/new_build_pack.md for architecture details.$(NC)"
	@echo ""
	@echo "$(BLUE)Creating Android NuGet packages...$(NC)"
	@rm -rf ./local-packages
	@mkdir -p ./local-packages
	@dotnet pack Datadog.MAUI.Android.Binding/Datadog.MAUI.Android.Binding.sln --configuration Release --no-build --output ./local-packages
	@echo "$(GREEN)✓ Android packages created$(NC)"
	@echo ""
	@echo "Packages location: ./local-packages/"
	@ls -lh ./local-packages/*.nupkg 2>/dev/null || true

##@ Plugin Build

build-plugin: ## Build the MAUI plugin project
	@echo "$(BLUE)Building MAUI Plugin...$(NC)"
	@dotnet restore Datadog.MAUI.Plugin/Datadog.MAUI.Plugin.csproj --verbosity quiet
	@dotnet build Datadog.MAUI.Plugin/Datadog.MAUI.Plugin.csproj --configuration Release --no-restore --verbosity minimal
	@echo "$(GREEN)✓ MAUI Plugin built successfully$(NC)"

pack-plugin: build-plugin ## Build and pack MAUI plugin NuGet package (DEPRECATED: use 'make pack' instead)
	@echo "$(YELLOW)Warning: pack-plugin is deprecated. Use 'make pack' for proper packaging.$(NC)"
	@echo "$(YELLOW)See docs/new_build_pack.md for architecture details.$(NC)"
	@echo ""
	@echo "$(BLUE)Creating MAUI Plugin NuGet package...$(NC)"
	@rm -rf ./local-packages
	@mkdir -p ./local-packages
	@dotnet pack Datadog.MAUI.Plugin/Datadog.MAUI.Plugin.csproj --configuration Release --no-build --output ./local-packages
	@echo "$(GREEN)✓ Plugin package created$(NC)"
	@echo ""
	@echo "Packages location: ./local-packages/"
	@ls -lh ./local-packages/*.nupkg 2>/dev/null || true

##@ Combined Build

build-all: build-android build-ios build-plugin ## Build all projects (Android, iOS, Plugin)

build: build-all ## Alias for build-all

build-errors: build-android-errors build-ios-errors ## Build all projects showing only errors and warnings

##@ Packaging

pack: build-all ## Build and pack all NuGet packages following proper dependency order
	@echo "$(BLUE)Creating NuGet packages with proper dependency order...$(NC)"
	@echo "$(YELLOW)Following packaging architecture from docs/new_build_pack.md$(NC)"
	@echo ""
	@chmod +x scripts/pack.sh
	@./scripts/pack.sh Release ./artifacts
	@echo ""
	@echo "$(GREEN)✓ All packages created in ./artifacts$(NC)"

pack-debug: ## Pack all packages in Debug configuration
	@echo "$(BLUE)Creating NuGet packages (Debug)...$(NC)"
	@chmod +x scripts/pack.sh
	@./scripts/pack.sh Debug ./artifacts-debug

pack-all: pack ## Build and pack all NuGet packages (DEPRECATED: use 'make pack' instead)
	@echo "$(YELLOW)Note: pack-all now uses the new pack.sh script$(NC)"

##@ Sample App

sample-ios: ## Build and run iOS sample app
	@echo "$(BLUE)Building and running iOS sample app...$(NC)"
	@if [ "$$(uname)" != "Darwin" ]; then \
		echo "$(RED)Error: iOS sample requires macOS$(NC)"; \
		exit 1; \
	fi
	@cd samples/DatadogMauiSample && \
		dotnet build -f net9.0-ios -c Debug && \
		dotnet build -f net9.0-ios -c Debug -t:Run
	@echo "$(GREEN)✓ iOS sample app launched$(NC)"

sample-android: ## Build and run Android sample app
	@echo "$(BLUE)Building and running Android sample app...$(NC)"
	@cd samples/DatadogMauiSample && \
		dotnet build -f net10.0-android -c Debug && \
		dotnet build -f net10.0-android -c Debug -t:Run
	@echo "$(GREEN)✓ Android sample app launched$(NC)"

sample-ios-simulator: ## Build and run iOS sample on specific simulator
	@echo "$(BLUE)Available iOS simulators:$(NC)"
	@xcrun simctl list devices available | grep -E "iPhone|iPad" | grep -v "unavailable"
	@echo ""
	@echo "$(YELLOW)Launching on iPhone 15 Pro...$(NC)"
	@cd samples/DatadogMauiSample && \
		dotnet build -f net9.0-ios -c Debug -t:Run /p:_DeviceName=:v2:udid=5A1AB396-285D-464E-B00C-267CEE8F9F8D

sample-build-ios: ## Build iOS sample without running
	@echo "$(BLUE)Building iOS sample app...$(NC)"
	@cd samples/DatadogMauiSample && \
		dotnet restore && \
		dotnet build -f net9.0-ios -c Debug
	@echo "$(GREEN)✓ iOS sample app built$(NC)"

sample-build-android: ## Build Android sample without running
	@echo "$(BLUE)Building Android sample app...$(NC)"
	@cd samples/DatadogMauiSample && \
		dotnet restore && \
		dotnet build -f net10.0-android -c Debug
	@echo "$(GREEN)✓ Android sample app built$(NC)"

##@ Testing

test: ## Run unit tests
	@echo "$(BLUE)Running tests...$(NC)"
	@dotnet test Datadog.MAUI.Plugin.Tests/Datadog.MAUI.Plugin.Tests.csproj --configuration Release
	@echo "$(GREEN)✓ Tests completed$(NC)"

test-verbose: ## Run tests with verbose output
	@echo "$(BLUE)Running tests (verbose)...$(NC)"
	@dotnet test Datadog.MAUI.Plugin.Tests/Datadog.MAUI.Plugin.Tests.csproj --configuration Release --verbosity detailed

##@ Cleanup

clean: ## Clean build artifacts and bin/obj directories
	@echo "$(BLUE)Cleaning build artifacts...$(NC)"
	@rm -rf local-packages temp-packages-* artifacts-* release-packages-*
	@find . -type d \( -name "bin" -o -name "obj" \) -not -path "*/\.*" | xargs rm -rf 2>/dev/null || true
	@echo "$(GREEN)✓ Clean complete$(NC)"

clean-all: clean ## Deep clean including generated files
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

format: ## Format code (run dotnet format)
	@echo "$(BLUE)Formatting code...$(NC)"
	@dotnet format Datadog.MAUI.sln
	@echo "$(GREEN)✓ Code formatted$(NC)"

##@ Android Binding Generation

generate-android-deps: ## Analyze Maven POM and generate dependency entries for an Android module
	@echo "$(BLUE)Analyzing Android Maven dependencies...$(NC)"
	@echo ""
	@if [ -z "$(MODULE)" ] || [ -z "$(VERSION)" ]; then \
		echo "$(RED)Error: MODULE and VERSION parameters required$(NC)"; \
		echo ""; \
		echo "Usage: make generate-android-deps MODULE=<name> VERSION=<version>"; \
		echo ""; \
		echo "Example:"; \
		echo "  make generate-android-deps MODULE=dd-sdk-android-core VERSION=3.5.0"; \
		echo ""; \
		exit 1; \
	fi
	@chmod +x scripts/generate-android-dependencies.sh
	@./scripts/generate-android-dependencies.sh $(MODULE) $(VERSION)

setup-android-binding: ## Analyze build errors and suggest dependency fixes for an Android binding
	@echo "$(BLUE)Setting up Android binding...$(NC)"
	@echo ""
	@if [ -z "$(PROJECT)" ]; then \
		echo "$(RED)Error: PROJECT parameter required$(NC)"; \
		echo ""; \
		echo "Usage: make setup-android-binding PROJECT=<path> [VERSION=<version>]"; \
		echo ""; \
		echo "Example:"; \
		echo "  make setup-android-binding PROJECT=Datadog.MAUI.Android.Binding/dd-sdk-android-core VERSION=3.5.0"; \
		echo ""; \
		exit 1; \
	fi
	@chmod +x scripts/setup-android-bindings.sh
	@./scripts/setup-android-bindings.sh $(VERSION) $(PROJECT)

##@ iOS Binding Generation

generate-ios-bindings: ## Generate iOS bindings using Objective Sharpie
	@echo "$(BLUE)Generating iOS bindings with Objective Sharpie...$(NC)"
	@if [ "$$(uname)" != "Darwin" ]; then \
		echo "$(RED)Error: iOS binding generation requires macOS$(NC)"; \
		exit 1; \
	fi
	@if [ ! -f "Datadog.MAUI.iOS.Binding/generate-ios-bindings-sharpie.sh" ]; then \
		echo "$(RED)Error: generate-ios-bindings-sharpie.sh not found$(NC)"; \
		exit 1; \
	fi
	@cd Datadog.MAUI.iOS.Binding && chmod +x generate-ios-bindings-sharpie.sh && ./generate-ios-bindings-sharpie.sh
	@echo "$(GREEN)✓ iOS bindings generated$(NC)"

##@ Utilities

list-simulators: ## List available iOS simulators
	@echo "$(BLUE)Available iOS Simulators:$(NC)"
	@xcrun simctl list devices available | grep -E "iPhone|iPad"

list-packages: ## List generated NuGet packages
	@echo "$(BLUE)Local NuGet Packages:$(NC)"
	@if [ -d "./local-packages" ]; then \
		ls -lh ./local-packages/*.nupkg 2>/dev/null || echo "No packages found"; \
	else \
		echo "No local-packages directory found"; \
	fi

watch-ios: ## Watch and rebuild iOS bindings on file changes
	@echo "$(BLUE)Watching iOS bindings for changes...$(NC)"
	@echo "$(YELLOW)Press Ctrl+C to stop$(NC)"
	@dotnet watch --project Datadog.MAUI.iOS.Binding/Datadog.MAUI.iOS.Binding.csproj build

watch-android: ## Watch and rebuild Android bindings on file changes
	@echo "$(BLUE)Watching Android bindings for changes...$(NC)"
	@echo "$(YELLOW)Press Ctrl+C to stop$(NC)"
	@dotnet watch --project Datadog.MAUI.Android.Binding/Datadog.MAUI.Android.Binding.csproj build

##@ Documentation

readme: ## Display quick reference README
	@echo "$(BLUE)==========================================="
	@echo "Datadog MAUI SDK - Quick Reference"
	@echo "==========================================$(NC)"
	@echo ""
	@echo "$(GREEN)Common Commands:$(NC)"
	@echo "  make help              - Show all available commands"
	@echo "  make status            - Show current state"
	@echo "  make dev-setup         - First-time setup"
	@echo "  make build             - Build all projects"
	@echo "  make pack              - Create all NuGet packages (proper order)"
	@echo "  make test              - Run unit tests"
	@echo "  make clean             - Clean build artifacts"
	@echo ""
	@echo "$(GREEN)Platform-Specific:$(NC)"
	@echo "  make build-android     - Build Android bindings"
	@echo "  make build-ios         - Build iOS bindings (macOS only)"
	@echo ""
	@echo "$(GREEN)Sample Apps:$(NC)"
	@echo "  make sample-ios        - Run iOS sample app"
	@echo "  make sample-android    - Run Android sample app"
	@echo ""
	@echo "$(GREEN)Development:$(NC)"
	@echo "  make restore           - Restore all packages"
	@echo "  make format            - Format code"
	@echo ""
	@echo "$(GREEN)Binding Generation:$(NC)"
	@echo "  make generate-android-deps MODULE=<name> VERSION=<ver>"
	@echo "                         - Analyze Maven POM for Android module"
	@echo "  make setup-android-binding PROJECT=<path> [VERSION=<ver>]"
	@echo "                         - Analyze build errors for Android binding"
	@echo "  make generate-ios-bindings - Generate iOS bindings with Sharpie"
	@echo ""
	@echo "$(GREEN)Documentation:$(NC)"
	@echo "  See docs/new_build_pack.md for packaging architecture"
	@echo "  See docs/android_dep_research.md for Android dependency details"
	@echo ""
