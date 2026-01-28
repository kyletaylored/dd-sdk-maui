# Datadog MAUI Sample App

A comprehensive example application demonstrating how to integrate Datadog monitoring into a .NET MAUI application with Real User Monitoring (RUM), Logging, and APM Tracing.

## Features Demonstrated

### 1. Real User Monitoring (RUM)

The sample app tracks:

- **View Tracking** - Automatically tracks page navigation and time spent on each screen
- **User Actions** - Captures button taps, product selections, and user interactions
- **Resource Loading** - Monitors API calls and network requests
- **Errors** - Captures and reports exceptions with context
- **Custom Timings** - Measures performance of specific operations
- **Custom Attributes** - Adds business context to RUM sessions

**Example (ProductsPage.xaml.cs)**:
```csharp
// Start view tracking
Rum.StartView("products", "Products Page", new Dictionary<string, object>
{
    { "screen_class", "ProductsPage" }
});

// Track user actions
Rum.AddAction(RumActionType.Tap, "product_selected", new Dictionary<string, object>
{
    { "product_id", product.Id },
    { "product_name", product.Name }
});

// Track custom performance metrics
Rum.AddTiming("products_load_time");

// Track errors
Rum.AddError("Failed to load products", RumErrorSource.Source, exception);

// Stop view tracking when leaving
Rum.StopView("products");
```

### 2. Logging

The app demonstrates structured logging with context:

- **Multiple Log Levels** - Debug, Info, Warning, Error
- **Structured Logging** - Adds custom attributes for filtering
- **Integration with RUM** - Logs are correlated with RUM sessions
- **Per-Component Loggers** - Each page has its own logger

**Example**:
```csharp
var logger = Logs.CreateLogger("ProductsPage");

logger.Info("Successfully loaded products", new Dictionary<string, object>
{
    { "product_count", products.Count },
    { "load_duration_ms", duration }
});

logger.Error("Error loading products", new Dictionary<string, object>
{
    { "error_type", ex.GetType().Name },
    { "stack_trace", ex.StackTrace }
});
```

### 3. APM Distributed Tracing

The ShopistApiService demonstrates HTTP request tracing:

- **Automatic Span Creation** - HTTP requests create spans automatically
- **Parent-Child Relationships** - Tracks full request flows
- **Custom Tags** - Adds business context to traces
- **Error Tracking** - Failed requests are marked as errors

**Configuration (MauiProgram.cs)**:
```csharp
.WithTracing(tracing => tracing
    .WithSampleRate(100.0)
)
.WithFirstPartyHosts(new[] { "fakestoreapi.com" })
```

## App Structure

### Pages

1. **MainPage** - Simple counter demonstration with basic RUM tracking
2. **ProductsPage** - E-commerce product list with comprehensive instrumentation:
   - Product loading with timing metrics
   - Product selection tracking
   - Cart and purchase flow monitoring
   - Error handling and reporting

3. **ProfilePage** - User profile management (future)
4. **WebViewPage** - WebView tracking demonstration (future)

### Services

**ShopistApiService** - HTTP API client that demonstrates:
- REST API calls with the FakeStore API
- Multi-step purchase flows
- Error handling

## Configuration

### 1. Set Environment Variables

The sample app uses environment variables for credentials. There are two ways to set them:

#### Option A: Using `.env` file (Recommended)

Create a `.env` file in the sample app directory:

```bash
# From the sample directory
cd samples/DatadogMauiSample
cp .env.example .env
nano .env  # Edit with your credentials

# Or from repository root
cp samples/DatadogMauiSample/.env.example samples/DatadogMauiSample/.env
nano samples/DatadogMauiSample/.env
```

Add your credentials:

```env
# iOS Configuration
DD_RUM_IOS_CLIENT_TOKEN=pub1234567890abcdef1234567890abcd
DD_RUM_IOS_APPLICATION_ID=12345678-1234-1234-1234-123456789012

# Android Configuration
DD_RUM_ANDROID_CLIENT_TOKEN=pub9876543210fedcba9876543210fedc
DD_RUM_ANDROID_APPLICATION_ID=87654321-4321-4321-4321-210987654321

# Optional: General settings
DD_ENV=dev
DD_SERVICE_NAME=shopist-maui-demo
DD_SITE=US1
DD_VERBOSE_LOGGING=true
```

Then run from the repository root:

#### Option B: Export Directly

Export environment variables in your terminal:

```bash
# iOS
export DD_RUM_IOS_CLIENT_TOKEN="your-ios-client-token"
export DD_RUM_IOS_APPLICATION_ID="your-ios-app-id"

# Android
export DD_RUM_ANDROID_CLIENT_TOKEN="your-android-client-token"
export DD_RUM_ANDROID_APPLICATION_ID="your-android-app-id"
```

### 2. Get Datadog Credentials

1. Log into [Datadog](https://app.datadoghq.com/)
2. Go to **Organization Settings > Client Tokens**
   - Create a new token or copy an existing one
3. Go to **UX Monitoring > RUM Applications**
   - Create separate applications for iOS and Android
   - Copy the **Application ID** for each platform

### 3. Configure in MauiProgram.cs

The app automatically loads configuration from the `.env` file:

```csharp
.UseDatadog(config =>
{
    config
        .WithClientToken(clientToken)
        .WithEnvironment("dev")
        .WithServiceName("datadog-maui-sample")
        .WithSite(DatadogSite.US1) // or US3, US5, EU1, AP1
        .WithRum(rum => rum
            .WithApplicationId(rumApplicationId)
            .WithSessionSampleRate(100.0)
            .WithTrackUserInteractions(true)
            .WithVitalsUpdateFrequency(VitalsUpdateFrequency.Average)
        )
        .WithLogs(logs => logs
            .WithNetworkInfoEnabled(true)
            .WithBundleWithRum(true)
        )
        .WithTracing(tracing => tracing
            .WithSampleRate(100.0)
        )
        .WithFirstPartyHosts(firstPartyHosts)
        .WithTrackingConsent(TrackingConsent.Granted)
        .WithVerboseLogging(true);
});
```

## Running the Sample

### Prerequisites

- .NET 9.0 or .NET 10.0 SDK
- Visual Studio 2022 or Visual Studio Code with C# Dev Kit
- Android SDK (for Android development)
- Xcode (for iOS development on macOS)

### Quick Start with Make

```bash
# Set up credentials (first time only)
cd samples/DatadogMauiSample
cp .env.example .env
nano .env  # Edit with your Datadog credentials

# Run from repository root with automatic .env loading (recommended)
cd ../..
make run-ios        # iOS (macOS only) - auto-loads samples/DatadogMauiSample/.env
make run-android    # Android - auto-loads samples/DatadogMauiSample/.env

# Or manually source .env first
source samples/DatadogMauiSample/.env
make sample-ios     # iOS (macOS only)
make sample-android # Android
```

The `run-ios` and `run-android` targets automatically:
- ✅ Check if `.env` file exists in samples/DatadogMauiSample/
- ✅ Load environment variables from `.env`
- ✅ Validate credentials are set
- ✅ Build and run the app

### Manual Build

If you prefer not to use Make:

```bash
# Set environment variables
export DD_RUM_IOS_CLIENT_TOKEN="your-token"
export DD_RUM_IOS_APPLICATION_ID="your-app-id"

# Run on iOS (macOS only)
cd samples/DatadogMauiSample
dotnet build -f net9.0-ios -c Debug \
  -p:IosClientToken="$DD_RUM_IOS_CLIENT_TOKEN" \
  -p:IosApplicationId="$DD_RUM_IOS_APPLICATION_ID"
dotnet build -f net9.0-ios -c Debug -t:Run \
  -p:IosClientToken="$DD_RUM_IOS_CLIENT_TOKEN" \
  -p:IosApplicationId="$DD_RUM_IOS_APPLICATION_ID"

# Run on Android
export DD_RUM_ANDROID_CLIENT_TOKEN="your-token"
export DD_RUM_ANDROID_APPLICATION_ID="your-app-id"

dotnet build -f net10.0-android -c Debug \
  -p:AndroidClientToken="$DD_RUM_ANDROID_CLIENT_TOKEN" \
  -p:AndroidApplicationId="$DD_RUM_ANDROID_APPLICATION_ID"
dotnet build -f net10.0-android -c Debug -t:Run \
  -p:AndroidClientToken="$DD_RUM_ANDROID_CLIENT_TOKEN" \
  -p:AndroidApplicationId="$DD_RUM_ANDROID_APPLICATION_ID"
```

## Viewing Data in Datadog

### RUM Dashboard

1. Navigate to **UX Monitoring > RUM Applications**
2. Select your application
3. View:
   - **Views** - Page navigation and time spent
   - **Actions** - User interactions (taps, scrolls, clicks)
   - **Resources** - Network requests and their performance
   - **Errors** - Exceptions and error rates
   - **Sessions** - User session recordings

### Logs

1. Navigate to **Logs > Search**
2. Filter by:
   - `service:datadog-maui-sample`
   - `env:dev`
   - Logger name (e.g., `@logger.name:ProductsPage`)

### APM Traces

1. Navigate to **APM > Traces**
2. Filter by service: `datadog-maui-sample`
3. View distributed traces showing:
   - HTTP request spans
   - Request duration and status
   - Error traces

## Key Instrumentation Points

### ProductsPage Example

```csharp
// 1. Start RUM view when page loads
Rum.StartView("products", "Products Page");

// 2. Track user action
Rum.AddAction(RumActionType.Custom, "load_products");

// 3. Add custom timing
var startTime = DateTime.UtcNow;
await LoadDataAsync();
Rum.AddTiming("products_load_time");

// 4. Log with structured context
_logger.Info("Products loaded", new Dictionary<string, object>
{
    { "count", products.Count },
    { "duration_ms", (DateTime.UtcNow - startTime).TotalMilliseconds }
});

// 5. Track errors
catch (Exception ex)
{
    Rum.AddError("Load failed", RumErrorSource.Source, ex);
    _logger.Error("Error loading products", new { error = ex.Message });
}

// 6. Stop view when leaving
Rum.StopView("products");
```

## Best Practices Demonstrated

1. **View Lifecycle Management** - Properly start/stop RUM views
2. **Structured Logging** - Use dictionaries for log attributes
3. **Error Context** - Include relevant data when reporting errors
4. **Performance Tracking** - Measure and report custom timings
5. **User Journey Tracking** - Track complete flows (browse → select → purchase)
6. **Resource Monitoring** - Automatic tracking of HTTP calls
7. **Session Attribution** - Add custom attributes to sessions

## Troubleshooting

### No Data in Datadog

1. **Check credentials** - Verify your client token and application ID
2. **Check network** - Ensure the device can reach Datadog servers
3. **Enable verbose logging** - Set `DATADOG_VERBOSE_LOGGING=true`
4. **Check site configuration** - Verify you're using the correct Datadog site (US1, EU1, etc.)

### Build Errors

1. **Restore workloads**:
   ```bash
   dotnet workload restore
   ```

2. **Clear NuGet cache**:
   ```bash
   dotnet nuget locals all --clear
   ```

3. **Rebuild**:
   ```bash
   dotnet clean
   dotnet build
   ```

## Additional Resources

- [Datadog RUM Documentation](https://docs.datadoghq.com/real_user_monitoring/)
- [Datadog Mobile Monitoring](https://docs.datadoghq.com/real_user_monitoring/mobile_and_tv_monitoring/)
- [Datadog .NET Tracing](https://docs.datadoghq.com/tracing/setup_overview/setup/dotnet-core/)
- [Project Documentation](../../docs/README.md)

## License

Apache 2.0
