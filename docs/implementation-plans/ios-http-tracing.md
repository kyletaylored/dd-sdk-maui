---
layout: default
title: iOS HTTP Tracing Implementation Plan
parent: Implementation Plans
---

# iOS HTTP Tracing - Implementation Plan

Plan for implementing automatic HTTP request tracing on iOS.

---

## Problem Statement

Currently, iOS HTTP requests through `HttpClient` are not automatically traced. The Android SDK has automatic tracing, but iOS requires:
1. URLSession instrumentation configuration
2. Delegate class specification
3. First-party hosts configuration

None of these are currently implemented in the MAUI plugin's iOS platform code.

---

## Root Cause Analysis

### Why Android Works Automatically

```csharp
// Android - works automatically
config.SetFirstPartyHosts(firstPartyHosts.ToList());
// ✅ Native SDK's OkHttp interceptor handles the rest
```

The Datadog Android SDK uses OkHttp interceptors which are globally registered. Every `HttpClient` on Android uses OkHttp under the hood, so automatic instrumentation "just works."

### Why iOS Doesn't Work

```csharp
// iOS - currently incomplete
var traceConfiguration = new DDTraceConfiguration();
traceConfiguration.SampleRate = tracingConfig.SampleRate;
DDTrace.EnableWith(traceConfiguration);
// ❌ No first-party hosts
// ❌ No URLSession tracking configuration
// ❌ No URLSession instrumentation enabled
```

iOS requires:
1. **URLSession Instrumentation** - Enable swizzling of NSURLSession delegate methods
2. **Delegate Class** - Specify which delegate class to instrument (this is the blocker)
3. **First-Party Hosts** - Configure which hosts to trace

---

## The Delegate Class Challenge

The core technical challenge is that `DDURLSessionInstrumentation.EnableWithConfiguration()` requires an `INSUrlSessionDataDelegate` class reference:

```csharp
var config = new DDURLSessionInstrumentationConfiguration(
    delegateClass: typeof(MyDelegate) // ❌ But what delegate class?
);
DDURLSessionInstrumentation.EnableWithConfiguration(config);
```

**The Problem:**
- .NET MAUI's `HttpClient` uses `NSUrlSessionHandler` internally
- `NSUrlSessionHandler` creates an internal, private delegate class
- We have no access to this delegate class
- We can't pass it to Datadog's instrumentation

---

## Proposed Solutions

### Solution 1: Configure Trace URLSession Tracking (Partial Fix)

**Status:** ✅ Can implement immediately
**Effort:** Low
**Coverage:** Partial - enables some tracking but not full instrumentation

**What it does:**
- Configures `DDTraceConfiguration.SetURLSessionTracking()` with first-party hosts
- Enables trace propagation for URLSession requests
- May enable automatic span generation (needs testing)

**What it doesn't do:**
- Doesn't call `DDURLSessionInstrumentation.EnableWithConfiguration()`
- May not fully instrument without the separate instrumentation step

**Implementation:**

```csharp
// File: Datadog.MAUI.Plugin/Platforms/iOS/Datadog.ios.cs

private static void InitializeTracing(TracingConfiguration tracingConfig)
{
    var traceConfiguration = new DDTraceConfiguration();
    traceConfiguration.SampleRate = tracingConfig.SampleRate;

    // ✅ NEW: Configure URLSession tracking with first-party hosts
    if (tracingConfig.FirstPartyHosts.Length > 0)
    {
        var hosts = new NSSet<NSString>(
            tracingConfig.FirstPartyHosts.Select(h => new NSString(h)).ToArray()
        );

        var firstPartyHostsTracing = new DDTraceFirstPartyHostsTracing(hosts);
        var urlSessionTracking = new DDTraceURLSessionTracking(firstPartyHostsTracing);

        traceConfiguration.SetURLSessionTracking(urlSessionTracking);
    }

    DDTrace.EnableWith(traceConfiguration);
}
```

**Testing Required:**
Need to verify if this alone enables automatic tracing or if `URLSessionInstrumentation` is still required.

---

### Solution 2: Use NSObject as Delegate Class (Experimental)

**Status:** 🧪 Needs testing
**Effort:** Low
**Coverage:** Unknown - may enable full instrumentation

**Theory:**
The Datadog iOS SDK may not actually need a specific delegate class - it may just need *any* class that could be a delegate. Passing `typeof(NSObject)` or a base `NSUrlSessionDelegate` class might work.

**Implementation:**

```csharp
// File: Datadog.MAUI.Plugin/Platforms/iOS/Datadog.ios.cs

private static void EnableURLSessionInstrumentation(TracingConfiguration tracingConfig)
{
    if (tracingConfig.FirstPartyHosts.Length > 0)
    {
        try
        {
            // Try using NSUrlSessionDelegate base class
            var delegateClass = Java.Lang.Class.FromType(typeof(NSUrlSessionDelegate));

            var instrumentation = new DDURLSessionInstrumentationConfiguration(delegateClass);

            // Set first-party hosts
            var hosts = new NSSet<NSString>(
                tracingConfig.FirstPartyHosts.Select(h => new NSString(h)).ToArray()
            );
            var firstPartyHostsTracing = new DDURLSessionInstrumentationFirstPartyHostsTracing(hosts);
            instrumentation.SetFirstPartyHostsTracing(firstPartyHostsTracing);

            // Enable instrumentation
            DDURLSessionInstrumentation.EnableWithConfiguration(instrumentation);

            Console.WriteLine("[Datadog] URLSession instrumentation enabled (experimental)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Datadog] URLSession instrumentation failed: {ex.Message}");
            // Fall back to Solution 1
        }
    }
}
```

**Testing Required:**
1. Test if Datadog SDK accepts base delegate class
2. Verify if HttpClient requests get traced
3. Check if trace headers are injected

---

### Solution 3: Create Custom Delegate + Configure NSUrlSessionHandler

**Status:** ⚠️ Complex - requires MAUI internals knowledge
**Effort:** Medium
**Coverage:** Partial - only for HttpClient instances created with custom handler

**Approach:**
Create a custom `NSUrlSessionDelegate` subclass and provide a way for users to create HttpClient with a properly configured handler.

**Implementation:**

```csharp
// File: Datadog.MAUI.Plugin/Platforms/iOS/DatadogURLSessionDelegate.cs

using Foundation;
using System;

namespace Datadog.Maui.iOS
{
    [Register("DatadogURLSessionDelegate")]
    public class DatadogURLSessionDelegate : NSUrlSessionDataDelegate
    {
        public DatadogURLSessionDelegate() { }

        public DatadogURLSessionDelegate(NativeHandle handle) : base(handle) { }

        // Implement required delegate methods
        [Export("URLSession:dataTask:didReceiveResponse:completionHandler:")]
        public override void DidReceiveResponse(NSUrlSession session, NSUrlSessionDataTask dataTask,
            NSUrlResponse response, Action<NSUrlSessionResponseDisposition> completionHandler)
        {
            // Let Datadog instrumentation handle this
            completionHandler(NSUrlSessionResponseDisposition.Allow);
        }
    }
}
```

```csharp
// File: Datadog.MAUI.Plugin/Platforms/iOS/HttpClientFactory.cs

using Foundation;
using System.Net.Http;

namespace Datadog.Maui.iOS
{
    public static class HttpClientFactory
    {
        private static bool _instrumentationEnabled = false;

        public static HttpClient CreateTracedHttpClient()
        {
            if (!_instrumentationEnabled)
            {
                EnableInstrumentation();
            }

            // Create handler with custom delegate
            var configuration = NSUrlSessionConfiguration.DefaultSessionConfiguration;
            var session = NSUrlSession.FromConfiguration(
                configuration,
                new DatadogURLSessionDelegate(),
                null
            );

            // Create HttpClient with this session
            // Problem: NSUrlSessionHandler doesn't accept a custom session
            // This approach has limitations

            return new HttpClient(new NSUrlSessionHandler());
        }

        private static void EnableInstrumentation()
        {
            var config = new DDURLSessionInstrumentationConfiguration(
                typeof(DatadogURLSessionDelegate)
            );
            DDURLSessionInstrumentation.EnableWithConfiguration(config);
            _instrumentationEnabled = true;
        }
    }
}
```

**Limitations:**
- Requires users to use custom factory method
- Not truly "automatic" - users must change code
- May not work with standard `HttpClient()` constructor

---

### Solution 4: Custom DelegatingHandler (Manual Instrumentation)

**Status:** ✅ Can implement immediately
**Effort:** Medium
**Coverage:** Full - works for any HttpClient with custom handler

**Approach:**
Create a `DelegatingHandler` that wraps `NSUrlSessionHandler` and manually creates spans + injects headers.

**Implementation:**

```csharp
// File: Datadog.MAUI.Plugin/Platforms/iOS/DatadogHttpHandler.cs

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Datadog.Maui;

namespace Datadog.Maui.iOS
{
    /// <summary>
    /// HTTP handler that automatically traces requests using Datadog.
    /// Use this as the HttpClient handler for automatic distributed tracing.
    /// </summary>
    public class DatadogHttpHandler : DelegatingHandler
    {
        private readonly string[] _firstPartyHosts;

        public DatadogHttpHandler() : this(Array.Empty<string>())
        {
        }

        public DatadogHttpHandler(string[] firstPartyHosts)
            : base(new NSUrlSessionHandler())
        {
            _firstPartyHosts = firstPartyHosts;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // Check if this is a first-party host
            var isFirstParty = IsFirstPartyHost(request.RequestUri?.Host);

            if (!isFirstParty)
            {
                // Not a first-party host - just pass through
                return await base.SendAsync(request, cancellationToken);
            }

            // Create span for the request
            using (var span = Tracer.StartSpan("http.request"))
            {
                span.SetTag("http.method", request.Method.Method);
                span.SetTag("http.url", request.RequestUri?.ToString() ?? "");
                span.SetTag("resource.name", $"{request.Method.Method} {request.RequestUri?.PathAndQuery}");
                span.SetTag("span.kind", "client");

                try
                {
                    // Inject trace headers
                    Tracer.Inject(request.Headers, span);

                    // Make the request
                    var response = await base.SendAsync(request, cancellationToken);

                    // Tag with response info
                    span.SetTag("http.status_code", (int)response.StatusCode);

                    if ((int)response.StatusCode >= 400)
                    {
                        span.SetTag("error", true);
                    }

                    return response;
                }
                catch (Exception ex)
                {
                    span.SetError(ex);
                    span.SetTag("error", true);
                    throw;
                }
            }
        }

        private bool IsFirstPartyHost(string? host)
        {
            if (string.IsNullOrEmpty(host) || _firstPartyHosts.Length == 0)
            {
                return false;
            }

            foreach (var firstPartyHost in _firstPartyHosts)
            {
                if (host.Equals(firstPartyHost, StringComparison.OrdinalIgnoreCase) ||
                    host.EndsWith($".{firstPartyHost}", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
```

**Usage:**

```csharp
// Option A: Create HttpClient with handler
var httpClient = new HttpClient(new DatadogHttpHandler(new[] { "api.myapp.com" }))
{
    BaseAddress = new Uri("https://api.myapp.com")
};

// Option B: Factory method in platform code
#if IOS
public static class HttpClientFactory
{
    public static HttpClient Create(string[] firstPartyHosts)
    {
        return new HttpClient(new DatadogHttpHandler(firstPartyHosts));
    }
}
#endif
```

**Pros:**
- Works immediately
- Full control over tracing behavior
- No dependency on native URLSession instrumentation

**Cons:**
- Requires users to use custom handler
- Not truly "automatic" unless we can inject it globally
- Duplicate code with manual tracing approach

---

## Recommended Implementation Path

### Phase 1: Immediate (Low Risk)

**Goal:** Enable partial tracing with minimal code changes

1. **Implement Solution 1** - Configure `DDTraceConfiguration.SetURLSessionTracking()`
   - File: `Datadog.MAUI.Plugin/Platforms/iOS/Datadog.ios.cs`
   - Add first-party hosts to trace configuration
   - Test if this alone enables automatic tracing

2. **Update Documentation**
   - Update http-tracing.md with current status
   - Add note that Phase 1 is implemented
   - Provide clear examples

### Phase 2: Experimental (Medium Risk)

**Goal:** Test if native instrumentation can work without specific delegate

1. **Implement Solution 2** - Try enabling URLSession instrumentation with base delegate class
   - Add to iOS initialization after Phase 1
   - Wrap in try-catch to gracefully fall back
   - Log success/failure for telemetry

2. **Test with Real Apps**
   - Create test app with HttpClient requests
   - Verify if spans are created
   - Check if headers are injected
   - Validate distributed tracing works

### Phase 3: Fallback (If Phase 2 Fails)

**Goal:** Provide working solution even if native instrumentation doesn't work

1. **Implement Solution 4** - Create `DatadogHttpHandler`
   - Add to iOS platform code
   - Document usage clearly
   - Provide factory methods

2. **Consider API Changes**
   - Could auto-inject handler if we control HttpClient creation
   - Explore dependency injection integration

---

## Success Criteria

### Minimum Viable Product (MVP)
- [ ] iOS first-party hosts are configured on `DDTraceConfiguration`
- [ ] URLSession tracking is enabled via `SetURLSessionTracking()`
- [ ] HttpClient requests to first-party hosts generate APM spans
- [ ] Trace headers (`x-datadog-trace-id`, etc.) are injected
- [ ] Distributed tracing works end-to-end

### Ideal Solution
- [ ] Zero code changes required from users
- [ ] Works with standard `HttpClient()` constructor
- [ ] Parity with Android automatic instrumentation
- [ ] No performance degradation

---

## Testing Plan

### Unit Tests
- [ ] Test first-party host matching logic
- [ ] Test span creation for HTTP requests
- [ ] Test header injection
- [ ] Test error handling

### Integration Tests
- [ ] Test HttpClient with first-party host
- [ ] Test HttpClient with third-party host (should not trace)
- [ ] Test distributed tracing across services
- [ ] Test with various HTTP methods (GET, POST, PUT, DELETE)

### Manual Testing
- [ ] Test in DatadogMauiSample app
- [ ] Verify spans appear in Datadog APM
- [ ] Verify trace headers in backend logs
- [ ] Test with real Datadog account

---

## Open Questions

1. **Does `SetURLSessionTracking()` alone enable automatic instrumentation?**
   - Need to test with real iOS app
   - Check Datadog iOS SDK source code

2. **Can we pass base delegate class to URLSession instrumentation?**
   - Try `typeof(NSUrlSessionDelegate)`
   - Try `typeof(NSObject)`

3. **Is there a way to globally intercept NSUrlSessionHandler creation?**
   - Explore .NET MAUI internals
   - Check if we can use method swizzling from C#

4. **Would Datadog accept a PR to make delegate class optional?**
   - File issue on dd-sdk-ios GitHub
   - Explain .NET use case

---

## Files to Modify

### Phase 1
- `Datadog.MAUI.Plugin/Platforms/iOS/Datadog.ios.cs` - Add URLSession tracking config
- `docs/guides/http-tracing.md` - Update documentation

### Phase 2
- `Datadog.MAUI.Plugin/Platforms/iOS/Datadog.ios.cs` - Add URLSession instrumentation (experimental)
- `samples/DatadogMauiSample/Services/ShopistApiService.cs` - Add test HTTP calls

### Phase 3 (if needed)
- `Datadog.MAUI.Plugin/Platforms/iOS/DatadogHttpHandler.cs` - New file
- `Datadog.MAUI.Plugin/Platforms/iOS/HttpClientFactory.cs` - New file
- `docs/getting-started/using-the-sdk.md` - Document handler usage

---

## Timeline Estimate

- **Phase 1**: 2-4 hours (implementation + testing)
- **Phase 2**: 4-8 hours (experimentation + testing)
- **Phase 3**: 8-16 hours (full implementation + testing + docs)

**Total**: 1-3 days depending on which solution works

---

## References

- [Datadog iOS Tracing Documentation](https://docs.datadoghq.com/tracing/trace_collection/dd_libraries/ios/)
- [iOS URLSession Instrumentation](https://github.com/DataDog/dd-sdk-ios)
- [.NET MAUI HttpClient](https://learn.microsoft.com/en-us/dotnet/maui/data-cloud/rest)
- [NSUrlSessionHandler](https://learn.microsoft.com/en-us/dotnet/api/foundation.nsurlsessionhandler)
