---
layout: default
title: Code Examples
nav_order: 4
permalink: /examples
---

# Code Examples

Practical examples for using the Datadog MAUI SDK in your applications.

---

## Table of Contents

- [Basic Setup](#basic-setup)
- [Logging Examples](#logging-examples)
- [RUM Examples](#rum-examples)
- [Tracing Examples](#tracing-examples)
- [Real-World Scenarios](#real-world-scenarios)
- [MAUI Integration](#maui-integration)

---

## Basic Setup

### Minimal Configuration

```csharp
// MauiProgram.cs
using Datadog.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseDatadog(config =>
            {
                config.ClientToken = "YOUR_CLIENT_TOKEN";
                config.Environment = "production";
                config.ServiceName = "my-maui-app";
            });

        return builder.Build();
    }
}
```

### Full Configuration

```csharp
builder.UseDatadog(config =>
{
    // Required settings
    config.ClientToken = "YOUR_CLIENT_TOKEN";
    config.Environment = "production";
    config.ServiceName = "my-maui-app";
    config.Site = DatadogSite.US1;

    // Optional settings
    config.VerboseLogging = true;
    config.TrackingConsent = TrackingConsent.Granted;

    // Global tags
    config.GlobalTags["app_version"] = VersionTracking.CurrentVersion;
    config.GlobalTags["app_build"] = VersionTracking.CurrentBuild;
    config.GlobalTags["platform"] = DeviceInfo.Platform.ToString();
    config.GlobalTags["device_model"] = DeviceInfo.Model;

    // First-party hosts for distributed tracing
    config.FirstPartyHosts = new[]
    {
        "api.myapp.com",
        "backend.myapp.com"
    };

    // Enable RUM
    config.EnableRum(rum =>
    {
        rum.SetApplicationId("YOUR_APPLICATION_ID");
        rum.SetSessionSampleRate(100);
        rum.SetTelemetrySampleRate(20);
        rum.TrackViewsAutomatically(true);
        rum.TrackUserInteractions(true);
        rum.TrackResources(true);
        rum.TrackErrors(true);
        rum.SetVitalsUpdateFrequency(VitalsUpdateFrequency.Average);
    });

    // Enable Logs
    config.EnableLogs(logs =>
    {
        logs.SetSampleRate(100);
        logs.EnableNetworkInfo(true);
        logs.BundleWithRum(true);
    });

    // Enable Tracing
    config.EnableTracing(tracing =>
    {
        tracing.SetSampleRate(100);
        tracing.EnableTraceIdGeneration(true);
    });
});
```

---

## Logging Examples

### Basic Logging

```csharp
using Datadog.Maui;

public class ExampleService
{
    private readonly ILogger _logger = Logs.CreateLogger("example-service");

    public void DoWork()
    {
        _logger.Debug("Starting work");
        _logger.Info("Processing item");
        _logger.Warn("Low memory warning");
        _logger.Error("Operation failed");
        _logger.Critical("System failure");
    }
}
```

### Logging with Context

```csharp
public class PaymentService
{
    private readonly ILogger _logger = Logs.CreateLogger("payment");

    public async Task ProcessPayment(PaymentRequest request)
    {
        _logger.Info("Processing payment", new Dictionary<string, object>
        {
            { "payment_id", request.Id },
            { "amount", request.Amount },
            { "currency", request.Currency },
            { "payment_method", request.Method },
            { "customer_id", request.CustomerId }
        });

        try
        {
            await _paymentGateway.ChargeAsync(request);

            _logger.Info("Payment successful", new Dictionary<string, object>
            {
                { "payment_id", request.Id },
                { "transaction_id", response.TransactionId },
                { "duration_ms", stopwatch.ElapsedMilliseconds }
            });
        }
        catch (PaymentException ex)
        {
            _logger.Error("Payment failed", ex, new Dictionary<string, object>
            {
                { "payment_id", request.Id },
                { "error_code", ex.Code },
                { "retry_count", retryCount }
            });

            throw;
        }
    }
}
```

### Component-Specific Loggers

```csharp
public class Application
{
    // Create component-specific loggers
    private readonly ILogger _networkLogger = Logs.CreateLogger("network");
    private readonly ILogger _databaseLogger = Logs.CreateLogger("database");
    private readonly ILogger _authLogger = Logs.CreateLogger("authentication");
    private readonly ILogger _analyticsLogger = Logs.CreateLogger("analytics");

    public void Initialize()
    {
        // Add component-specific attributes
        _networkLogger.AddAttribute("component", "network");
        _networkLogger.AddTag("layer", "infrastructure");

        _databaseLogger.AddAttribute("component", "database");
        _databaseLogger.AddTag("layer", "data");

        _authLogger.AddAttribute("component", "auth");
        _authLogger.AddTag("layer", "security");
    }

    public async Task FetchData()
    {
        _networkLogger.Info("Fetching data from API");
        var data = await _apiClient.GetAsync("/data");

        _databaseLogger.Info("Caching data");
        await _cache.SetAsync("data", data);

        _analyticsLogger.Info("Data fetch completed", new Dictionary<string, object>
        {
            { "record_count", data.Count },
            { "cache_size_kb", data.Length / 1024 }
        });
    }
}
```

### Global Logger Configuration

```csharp
public class App : Application
{
    public App()
    {
        // Add global attributes to all logs
        Logs.AddAttribute("app_version", VersionTracking.CurrentVersion);
        Logs.AddAttribute("platform", DeviceInfo.Platform.ToString());
        Logs.AddAttribute("device_model", DeviceInfo.Model);

        // Add global tags
        Logs.AddTag("env", "production");
        Logs.AddTag("region", GetUserRegion());

        // Create logger
        var logger = Logs.CreateLogger("app");
        logger.Info("Application started");
    }
}
```

---

## RUM Examples

### Page View Tracking

```csharp
public class HomePage : ContentPage
{
    private const string ViewKey = "home_page";

    public HomePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Start tracking this view
        Rum.StartView(ViewKey, "Home Page", new Dictionary<string, object>
        {
            { "user_tier", GetUserTier() },
            { "feature_flags", GetEnabledFeatures() }
        });
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Stop tracking this view
        Rum.StopView(ViewKey, new Dictionary<string, object>
        {
            { "session_duration_sec", GetSessionDuration() }
        });
    }
}
```

### User Interaction Tracking

```csharp
public class ProductPage : ContentPage
{
    private void OnAddToCartClicked(object sender, EventArgs e)
    {
        // Track user action
        Rum.AddAction(RumActionType.Tap, "add_to_cart", new Dictionary<string, object>
        {
            { "product_id", _product.Id },
            { "product_name", _product.Name },
            { "price", _product.Price },
            { "quantity", _quantityInput.Value }
        });

        AddToCart(_product);
    }

    private void OnProductImageTapped(object sender, EventArgs e)
    {
        Rum.AddAction(RumActionType.Tap, "view_product_image", new Dictionary<string, object>
        {
            { "product_id", _product.Id },
            { "image_index", GetImageIndex() }
        });

        ShowImageGallery();
    }

    private void OnScrolled(object sender, ScrolledEventArgs e)
    {
        // Track scroll depth for analytics
        var scrollPercent = (e.ScrollY / _scrollView.ContentSize.Height) * 100;

        if (scrollPercent > 75 && !_scrolledTo75)
        {
            _scrolledTo75 = true;
            Rum.AddAction(RumActionType.Scroll, "scrolled_75_percent", new Dictionary<string, object>
            {
                { "product_id", _product.Id }
            });
        }
    }
}
```

### Network Request Tracking

```csharp
public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger = Logs.CreateLogger("api");

    public async Task<List<User>> FetchUsersAsync()
    {
        var resourceKey = $"fetch_users_{Guid.NewGuid()}";
        var url = "https://api.myapp.com/users";

        // Start tracking the resource
        Rum.StartResource(resourceKey, "GET", url);

        try
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await _httpClient.GetAsync(url);
            stopwatch.Stop();

            var content = await response.Content.ReadAsStringAsync();

            // Stop tracking with success
            Rum.StopResource(
                resourceKey,
                statusCode: (int)response.StatusCode,
                size: content.Length,
                kind: RumResourceKind.Xhr,
                attributes: new Dictionary<string, object>
                {
                    { "duration_ms", stopwatch.ElapsedMilliseconds },
                    { "cache_hit", response.Headers.Contains("X-Cache-Hit") }
                }
            );

            return JsonSerializer.Deserialize<List<User>>(content);
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to fetch users", ex);

            // Stop tracking with error
            Rum.StopResourceWithError(resourceKey, ex, new Dictionary<string, object>
            {
                { "retry_count", GetRetryCount() }
            });

            throw;
        }
    }
}
```

### Error Tracking

```csharp
public class ExampleViewModel
{
    public async Task LoadDataAsync()
    {
        try
        {
            await FetchData();
        }
        catch (NetworkException ex)
        {
            // Track network error
            Rum.AddError(ex, RumErrorSource.Network, new Dictionary<string, object>
            {
                { "endpoint", ex.Endpoint },
                { "status_code", ex.StatusCode },
                { "retry_count", _retryCount }
            });

            ShowErrorMessage("Network error. Please check your connection.");
        }
        catch (ValidationException ex)
        {
            // Track validation error
            Rum.AddError(ex, RumErrorSource.Source, new Dictionary<string, object>
            {
                { "field", ex.FieldName },
                { "validation_rule", ex.Rule }
            });

            ShowErrorMessage($"Invalid {ex.FieldName}");
        }
        catch (Exception ex)
        {
            // Track general error
            Rum.AddError(ex, RumErrorSource.Source);

            ShowErrorMessage("An unexpected error occurred");
        }
    }
}
```

### Custom Performance Timings

```csharp
public class CheckoutPage : ContentPage
{
    private const string ViewKey = "checkout_page";

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        Rum.StartView(ViewKey, "Checkout");

        // Load payment methods
        await LoadPaymentMethods();
        Rum.AddTiming("payment_methods_loaded");

        // Load shipping options
        await LoadShippingOptions();
        Rum.AddTiming("shipping_options_loaded");

        // Load order summary
        await LoadOrderSummary();
        Rum.AddTiming("order_summary_loaded");

        // All data loaded
        Rum.AddTiming("checkout_ready");
    }

    private async Task ProcessCheckoutAsync()
    {
        Rum.AddTiming("checkout_started");

        await ValidatePayment();
        Rum.AddTiming("payment_validated");

        await ProcessOrder();
        Rum.AddTiming("order_processed");

        await SendConfirmationEmail();
        Rum.AddTiming("confirmation_sent");

        Rum.AddTiming("checkout_completed");
    }
}
```

---

## Tracing Examples

### Basic Tracing

```csharp
public class DataService
{
    public async Task<Data> FetchAndProcessDataAsync()
    {
        // Start a span
        using (var span = Tracer.StartSpan("fetch_and_process"))
        {
            span.SetTag("operation_type", "data_processing");

            try
            {
                var data = await FetchDataAsync();
                var processed = ProcessData(data);

                span.SetTag("record_count", processed.Count);

                return processed;
            }
            catch (Exception ex)
            {
                span.SetError(ex);
                throw;
            }
            // Span automatically finishes when disposed
        }
    }
}
```

### Nested Spans

```csharp
public class OrderService
{
    public async Task ProcessOrderAsync(Order order)
    {
        // Parent span
        using (var parentSpan = Tracer.StartSpan("process_order"))
        {
            parentSpan.SetTag("order_id", order.Id);
            parentSpan.SetTag("customer_id", order.CustomerId);

            // Child span - validate
            using (var validateSpan = Tracer.StartSpan("validate_order", parentSpan))
            {
                validateSpan.SetTag("item_count", order.Items.Count);
                await ValidateOrder(order);
            }

            // Child span - payment
            using (var paymentSpan = Tracer.StartSpan("process_payment", parentSpan))
            {
                paymentSpan.SetTag("amount", order.TotalAmount);
                paymentSpan.SetTag("payment_method", order.PaymentMethod);

                try
                {
                    await ProcessPayment(order);
                }
                catch (PaymentException ex)
                {
                    paymentSpan.SetError(ex);
                    throw;
                }
            }

            // Child span - inventory
            using (var inventorySpan = Tracer.StartSpan("update_inventory", parentSpan))
            {
                await UpdateInventory(order);
            }

            // Child span - notification
            using (var notifySpan = Tracer.StartSpan("send_notification", parentSpan))
            {
                await SendOrderConfirmation(order);
            }

            parentSpan.SetTag("status", "completed");
        }
    }
}
```

### HTTP Client with Tracing

```csharp
public class TracedHttpClient
{
    private readonly HttpClient _httpClient;

    public async Task<T> GetAsync<T>(string url)
    {
        using (var span = Tracer.StartSpan("http.request"))
        {
            span.SetTag("http.method", "GET");
            span.SetTag("http.url", url);

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // Inject trace context into request headers
            Tracer.Inject(request.Headers, span);

            try
            {
                var response = await _httpClient.SendAsync(request);

                span.SetTag("http.status_code", (int)response.StatusCode);
                span.SetTag("http.response_size", response.Content.Headers.ContentLength ?? 0);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content);
            }
            catch (HttpRequestException ex)
            {
                span.SetError(ex);
                span.SetTag("error", true);
                throw;
            }
        }
    }
}
```

### Trace Extraction (Server-Side)

```csharp
public class ApiController
{
    [HttpGet("data")]
    public async Task<IActionResult> GetData()
    {
        // Extract trace context from incoming request
        var headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.FirstOrDefault());
        var parentSpan = Tracer.Extract(headers);

        // Continue the trace
        using (var span = Tracer.StartSpan("api.get_data", parentSpan))
        {
            span.SetTag("endpoint", "/data");
            span.SetTag("user_id", GetUserId());

            var data = await _dataService.FetchDataAsync();

            span.SetTag("record_count", data.Count);

            return Ok(data);
        }
    }
}
```

### Span Events

```csharp
public class CacheService
{
    public async Task<Data> GetDataAsync(string key)
    {
        using (var span = Tracer.StartSpan("cache.get"))
        {
            span.SetTag("cache_key", key);

            // Check cache
            var cached = await _cache.GetAsync(key);

            if (cached != null)
            {
                span.AddEvent("cache_hit", new Dictionary<string, object>
                {
                    { "cache_age_seconds", GetCacheAge(cached) }
                });

                return cached;
            }

            span.AddEvent("cache_miss");

            // Fetch from source
            var data = await _source.FetchAsync(key);

            span.AddEvent("fetched_from_source", new Dictionary<string, object>
            {
                { "data_size_bytes", data.Size }
            });

            // Update cache
            await _cache.SetAsync(key, data);

            span.AddEvent("cache_updated");

            return data;
        }
    }
}
```

---

## Real-World Scenarios

### E-Commerce Checkout Flow

```csharp
public class CheckoutService
{
    private readonly ILogger _logger = Logs.CreateLogger("checkout");

    public async Task<CheckoutResult> ProcessCheckoutAsync(Cart cart)
    {
        // Start trace
        using (var checkoutSpan = Tracer.StartSpan("checkout.process"))
        {
            checkoutSpan.SetTag("cart_id", cart.Id);
            checkoutSpan.SetTag("item_count", cart.Items.Count);
            checkoutSpan.SetTag("total_amount", cart.TotalAmount);

            // Start RUM view
            Rum.StartView("checkout", "Checkout");

            try
            {
                // Step 1: Validate cart
                _logger.Info("Validating cart");
                using (var validateSpan = Tracer.StartSpan("checkout.validate", checkoutSpan))
                {
                    await ValidateCart(cart);
                }
                Rum.AddTiming("cart_validated");

                // Step 2: Process payment
                _logger.Info("Processing payment", new Dictionary<string, object>
                {
                    { "amount", cart.TotalAmount },
                    { "method", cart.PaymentMethod }
                });

                using (var paymentSpan = Tracer.StartSpan("checkout.payment", checkoutSpan))
                {
                    paymentSpan.SetTag("payment_method", cart.PaymentMethod);

                    try
                    {
                        await ProcessPayment(cart);
                        Rum.AddAction(RumActionType.Custom, "payment_successful");
                    }
                    catch (PaymentException ex)
                    {
                        paymentSpan.SetError(ex);
                        Rum.AddError(ex, RumErrorSource.Source, new Dictionary<string, object>
                        {
                            { "payment_method", cart.PaymentMethod },
                            { "error_code", ex.Code }
                        });
                        throw;
                    }
                }
                Rum.AddTiming("payment_processed");

                // Step 3: Create order
                _logger.Info("Creating order");
                using (var orderSpan = Tracer.StartSpan("checkout.create_order", checkoutSpan))
                {
                    var order = await CreateOrder(cart);
                    orderSpan.SetTag("order_id", order.Id);
                    checkoutSpan.SetTag("order_id", order.Id);
                }
                Rum.AddTiming("order_created");

                // Step 4: Update inventory
                _logger.Info("Updating inventory");
                using (var inventorySpan = Tracer.StartSpan("checkout.update_inventory", checkoutSpan))
                {
                    await UpdateInventory(cart);
                }
                Rum.AddTiming("inventory_updated");

                // Step 5: Send confirmation
                _logger.Info("Sending confirmation");
                using (var notifySpan = Tracer.StartSpan("checkout.send_confirmation", checkoutSpan))
                {
                    await SendConfirmation(cart);
                }
                Rum.AddTiming("confirmation_sent");

                // Success
                checkoutSpan.SetTag("status", "success");
                Rum.AddAction(RumActionType.Custom, "checkout_completed", new Dictionary<string, object>
                {
                    { "total_amount", cart.TotalAmount },
                    { "item_count", cart.Items.Count }
                });

                _logger.Info("Checkout completed successfully");

                Rum.StopView("checkout");

                return CheckoutResult.Success;
            }
            catch (Exception ex)
            {
                checkoutSpan.SetError(ex);
                _logger.Error("Checkout failed", ex);
                Rum.AddError(ex);
                Rum.StopView("checkout");
                throw;
            }
        }
    }
}
```

### Authentication Flow

```csharp
public class AuthenticationService
{
    private readonly ILogger _logger = Logs.CreateLogger("auth");

    public async Task<AuthResult> LoginAsync(string username, string password)
    {
        using (var span = Tracer.StartSpan("auth.login"))
        {
            span.SetTag("username", username);
            span.SetTag("auth_method", "password");

            _logger.Info("Login attempt", new Dictionary<string, object>
            {
                { "username", username }
            });

            Rum.StartView("login", "Login");

            try
            {
                // Validate credentials
                using (var validateSpan = Tracer.StartSpan("auth.validate_credentials", span))
                {
                    var user = await ValidateCredentials(username, password);

                    if (user == null)
                    {
                        _logger.Warn("Invalid credentials", new Dictionary<string, object>
                        {
                            { "username", username }
                        });

                        Rum.AddError("Invalid credentials", RumErrorSource.Source);
                        Rum.AddAction(RumActionType.Custom, "login_failed");

                        return AuthResult.InvalidCredentials;
                    }

                    validateSpan.SetTag("user_id", user.Id);
                    span.SetTag("user_id", user.Id);
                }

                // Create session
                using (var sessionSpan = Tracer.StartSpan("auth.create_session", span))
                {
                    var session = await CreateSession(user);
                    sessionSpan.SetTag("session_id", session.Id);
                }

                // Set user info in Datadog
                Datadog.SetUser(new UserInfo
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    ExtraInfo = new Dictionary<string, object>
                    {
                        { "account_type", user.AccountType },
                        { "signup_date", user.SignupDate }
                    }
                });

                _logger.Info("Login successful", new Dictionary<string, object>
                {
                    { "user_id", user.Id }
                });

                Rum.AddAction(RumActionType.Custom, "login_successful");
                Rum.StopView("login");

                return AuthResult.Success;
            }
            catch (Exception ex)
            {
                span.SetError(ex);
                _logger.Error("Login error", ex);
                Rum.AddError(ex);
                Rum.StopView("login");
                throw;
            }
        }
    }

    public void Logout()
    {
        _logger.Info("User logged out");
        Rum.AddAction(RumActionType.Custom, "logout");

        // Clear user info
        Datadog.ClearUser();
    }
}
```

### Background Task with Monitoring

```csharp
public class DataSyncService
{
    private readonly ILogger _logger = Logs.CreateLogger("data-sync");

    public async Task SyncDataAsync()
    {
        using (var span = Tracer.StartSpan("data_sync.full"))
        {
            _logger.Info("Starting data sync");

            try
            {
                // Fetch updates from server
                using (var fetchSpan = Tracer.StartSpan("data_sync.fetch", span))
                {
                    var updates = await FetchUpdatesFromServer();
                    fetchSpan.SetTag("update_count", updates.Count);

                    _logger.Info("Fetched updates", new Dictionary<string, object>
                    {
                        { "count", updates.Count },
                        { "size_kb", CalculateSize(updates) / 1024 }
                    });
                }

                // Apply updates locally
                using (var applySpan = Tracer.StartSpan("data_sync.apply", span))
                {
                    var applied = await ApplyUpdates(updates);
                    applySpan.SetTag("applied_count", applied);

                    _logger.Info("Applied updates", new Dictionary<string, object>
                    {
                        { "count", applied }
                    });
                }

                // Upload pending changes
                using (var uploadSpan = Tracer.StartSpan("data_sync.upload", span))
                {
                    var pending = await GetPendingChanges();
                    await UploadChanges(pending);
                    uploadSpan.SetTag("uploaded_count", pending.Count);

                    _logger.Info("Uploaded changes", new Dictionary<string, object>
                    {
                        { "count", pending.Count }
                    });
                }

                span.SetTag("status", "success");
                _logger.Info("Data sync completed successfully");
            }
            catch (Exception ex)
            {
                span.SetError(ex);
                _logger.Error("Data sync failed", ex);
                throw;
            }
        }
    }
}
```

---

## MAUI Integration

### MVVM Pattern

```csharp
public class ProductViewModel : INotifyPropertyChanged
{
    private readonly ILogger _logger = Logs.CreateLogger("product-view-model");
    private readonly IProductService _productService;

    public ObservableCollection<Product> Products { get; } = new();

    public ICommand LoadProductsCommand { get; }
    public ICommand AddToCartCommand { get; }

    public ProductViewModel(IProductService productService)
    {
        _productService = productService;

        LoadProductsCommand = new Command(async () => await LoadProductsAsync());
        AddToCartCommand = new Command<Product>(async (product) => await AddToCartAsync(product));
    }

    private async Task LoadProductsAsync()
    {
        using (var span = Tracer.StartSpan("products.load"))
        {
            try
            {
                _logger.Info("Loading products");

                var products = await _productService.GetProductsAsync();

                Products.Clear();
                foreach (var product in products)
                {
                    Products.Add(product);
                }

                span.SetTag("product_count", products.Count);

                _logger.Info("Products loaded", new Dictionary<string, object>
                {
                    { "count", products.Count }
                });

                Rum.AddAction(RumActionType.Custom, "products_loaded", new Dictionary<string, object>
                {
                    { "count", products.Count }
                });
            }
            catch (Exception ex)
            {
                span.SetError(ex);
                _logger.Error("Failed to load products", ex);
                Rum.AddError(ex);
            }
        }
    }

    private async Task AddToCartAsync(Product product)
    {
        _logger.Info("Adding product to cart", new Dictionary<string, object>
        {
            { "product_id", product.Id },
            { "product_name", product.Name }
        });

        Rum.AddAction(RumActionType.Custom, "add_to_cart", new Dictionary<string, object>
        {
            { "product_id", product.Id },
            { "price", product.Price }
        });

        await _cartService.AddAsync(product);
    }
}
```

### Dependency Injection

```csharp
// MauiProgram.cs
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseDatadog(config => { /* ... */ });

        // Register Datadog interfaces for DI
        builder.Services.AddSingleton<IDatadogSdk>(DatadogSdk.Instance);
        builder.Services.AddSingleton<IDatadogLogger>(DatadogSdk.Logger);
        builder.Services.AddSingleton<IDatadogRum>(DatadogSdk.Rum);
        builder.Services.AddSingleton<IDatadogTrace>(DatadogSdk.Trace);

        // Register your services
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddTransient<ProductViewModel>();

        return builder.Build();
    }
}

// Using in a service
public class ApiService : IApiService
{
    private readonly IDatadogLogger _logger;
    private readonly IDatadogTrace _tracer;

    public ApiService(IDatadogLogger logger, IDatadogTrace tracer)
    {
        _logger = logger;
        _tracer = tracer;
    }

    public async Task<Data> FetchDataAsync()
    {
        _logger.Info("Fetching data");
        using (var span = _tracer.StartSpan("api.fetch_data"))
        {
            // ...
        }
    }
}
```

---

## See Also

- [Using the SDK](getting-started/using-the-sdk) - Complete usage guide
- [API Reference](api-reference) - Full API documentation
- [Datadog Documentation](https://docs.datadoghq.com/) - Official Datadog docs
