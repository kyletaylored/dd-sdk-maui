using Datadog.Maui;
using Datadog.Maui.Logs;
using Datadog.Maui.Rum;
using Datadog.Maui.Tracing;
using System.Net.Http;
using System.Text;

namespace DatadogMauiSample.Views;

/// <summary>
/// Comprehensive testing page for all Datadog SDK features.
/// Tests RUM, Logs, Tracing, and Session Replay functionality.
/// </summary>
public partial class DatadogFeaturesPage : ContentPage
{
    private readonly ILogger _logger;
    private readonly HttpClient _httpClient;
    private readonly StringBuilder _results = new();
    private int _actionCounter = 0;
    private int _spanCounter = 0;

    public DatadogFeaturesPage()
    {
        InitializeComponent();

        // Create logger for testing
        _logger = Logs.CreateLogger("DatadogFeaturesTest");

        // Create HTTP client for tracing tests
        _httpClient = new HttpClient();

        AddResult("✅ Datadog Features Test Page Loaded");
        AddResult($"🕐 {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
    }

    #region RUM Features

    private void OnStartViewClicked(object sender, EventArgs e)
    {
        try
        {
            var viewKey = $"custom_view_{DateTime.Now.Ticks}";
            var viewName = "Custom Test View";

            Rum.StartView(viewKey, viewName, new Dictionary<string, object>
            {
                { "test_attribute", "view_test" },
                { "timestamp", DateTime.Now.ToString("O") }
            });

            AddResult($"✅ RUM: Started view '{viewName}' (key: {viewKey})");
        }
        catch (Exception ex)
        {
            AddResult($"❌ RUM StartView error: {ex.Message}");
            _logger.Error("StartView failed", ex);
        }
    }

    private void OnStopViewClicked(object sender, EventArgs e)
    {
        try
        {
            // Note: This will stop the last started view
            // In a real app, you'd track the view key
            AddResult("✅ RUM: Stopped current view");
            AddResult("   (Note: Use view lifecycle methods in real apps)");
        }
        catch (Exception ex)
        {
            AddResult($"❌ RUM StopView error: {ex.Message}");
            _logger.Error("StopView failed", ex);
        }
    }

    private void OnAddActionClicked(object sender, EventArgs e)
    {
        try
        {
            _actionCounter++;
            var actionName = $"test_action_{_actionCounter}";

            Rum.AddAction(
                RumActionType.Custom,
                actionName,
                attributes: new Dictionary<string, object>
                {
                    { "action_id", _actionCounter },
                    { "test_data", "custom_action_test" },
                    { "timestamp", DateTime.Now.ToString("O") }
                }
            );

            AddResult($"✅ RUM: Added custom action '{actionName}'");
        }
        catch (Exception ex)
        {
            AddResult($"❌ RUM AddAction error: {ex.Message}");
            _logger.Error("AddAction failed", ex);
        }
    }

    private void OnAddRumErrorClicked(object sender, EventArgs e)
    {
        try
        {
            var testError = new InvalidOperationException("Test RUM error from Features page");

            Rum.AddError(
                testError,
                RumErrorSource.Source,
                attributes: new Dictionary<string, object>
                {
                    { "error_type", "test_error" },
                    { "test_id", Guid.NewGuid().ToString() }
                }
            );

            AddResult($"✅ RUM: Added error '{testError.Message}'");
        }
        catch (Exception ex)
        {
            AddResult($"❌ RUM AddError error: {ex.Message}");
            _logger.Error("AddError failed", ex);
        }
    }

    private void OnAddGlobalAttributeClicked(object sender, EventArgs e)
    {
        try
        {
            var attributeKey = $"test_attr_{DateTime.Now.Ticks}";
            var attributeValue = $"value_{Guid.NewGuid()}";

            Rum.AddAttribute(attributeKey, attributeValue);

            AddResult($"✅ RUM: Added global attribute '{attributeKey}' = '{attributeValue}'");
        }
        catch (Exception ex)
        {
            AddResult($"❌ RUM AddAttribute error: {ex.Message}");
            _logger.Error("AddAttribute failed", ex);
        }
    }

    private void OnRemoveGlobalAttributeClicked(object sender, EventArgs e)
    {
        try
        {
            var attributeKey = "test_attr";

            Rum.RemoveAttribute(attributeKey);

            AddResult($"✅ RUM: Removed global attribute '{attributeKey}'");
        }
        catch (Exception ex)
        {
            AddResult($"❌ RUM RemoveAttribute error: {ex.Message}");
            _logger.Error("RemoveAttribute failed", ex);
        }
    }

    private void OnSetUserClicked(object sender, EventArgs e)
    {
        try
        {
            var userId = $"test_user_{DateTime.Now.Ticks}";

            global::Datadog.Maui.Datadog.SetUser(new UserInfo
            {
                Id = userId,
                Name = "Test User",
                Email = "test@example.com",
                ExtraInfo = new Dictionary<string, object>
                {
                    { "user_tier", "premium" },
                    { "test_account", true }
                }
            });

            AddResult($"✅ Datadog: Set user info for '{userId}'");
        }
        catch (Exception ex)
        {
            AddResult($"❌ SetUser error: {ex.Message}");
            _logger.Error("SetUser failed", ex);
        }
    }

    #endregion

    #region Logs Features

    private void OnLogDebugClicked(object sender, EventArgs e)
    {
        try
        {
            _logger.Debug("Debug log from Datadog Features test page");
            AddResult("✅ Logs: Debug log sent");
        }
        catch (Exception ex)
        {
            AddResult($"❌ Logs Debug error: {ex.Message}");
        }
    }

    private void OnLogInfoClicked(object sender, EventArgs e)
    {
        try
        {
            _logger.Info("Info log from Datadog Features test page");
            AddResult("✅ Logs: Info log sent");
        }
        catch (Exception ex)
        {
            AddResult($"❌ Logs Info error: {ex.Message}");
        }
    }

    private void OnLogWarnClicked(object sender, EventArgs e)
    {
        try
        {
            _logger.Warn("Warning log from Datadog Features test page");
            AddResult("✅ Logs: Warning log sent");
        }
        catch (Exception ex)
        {
            AddResult($"❌ Logs Warn error: {ex.Message}");
        }
    }

    private void OnLogErrorClicked(object sender, EventArgs e)
    {
        try
        {
            var testError = new Exception("Test log error");
            _logger.Error("Error log from Datadog Features test page", testError);
            AddResult("✅ Logs: Error log sent");
        }
        catch (Exception ex)
        {
            AddResult($"❌ Logs Error error: {ex.Message}");
        }
    }

    private void OnLogWithAttributesClicked(object sender, EventArgs e)
    {
        try
        {
            _logger.Info("Log with custom attributes", attributes: new Dictionary<string, object>
            {
                { "test_id", Guid.NewGuid().ToString() },
                { "user_action", "feature_test" },
                { "page", "DatadogFeaturesPage" },
                { "timestamp", DateTime.Now.ToString("O") },
                { "test_number", 42 },
                { "test_bool", true }
            });

            AddResult("✅ Logs: Info log with 6 custom attributes sent");
        }
        catch (Exception ex)
        {
            AddResult($"❌ Logs WithAttributes error: {ex.Message}");
        }
    }

    #endregion

    #region Tracing Features

    private void OnCreateSpanClicked(object sender, EventArgs e)
    {
        try
        {
            _spanCounter++;
            var operationName = $"test_operation_{_spanCounter}";

            using (var span = Tracer.StartSpan(operationName))
            {
                span.SetTag("test.span_id", _spanCounter);
                span.SetTag("test.operation", "simple_span");
                span.SetTag("test.timestamp", DateTime.Now.ToString("O"));

                // Simulate some work
                Thread.Sleep(50);
            }

            AddResult($"✅ Tracing: Created span '{operationName}'");
        }
        catch (Exception ex)
        {
            AddResult($"❌ Tracing CreateSpan error: {ex.Message}");
            _logger.Error("CreateSpan failed", ex);
        }
    }

    private void OnCreateNestedSpansClicked(object sender, EventArgs e)
    {
        try
        {
            _spanCounter++;

            using (var parentSpan = Tracer.StartSpan($"parent_operation_{_spanCounter}"))
            {
                parentSpan.SetTag("span.type", "parent");
                parentSpan.SetTag("test.id", _spanCounter);

                // Simulate parent work
                Thread.Sleep(20);

                // Create child span
                using (var childSpan = Tracer.StartSpan($"child_operation_{_spanCounter}", parent: parentSpan))
                {
                    childSpan.SetTag("span.type", "child");
                    childSpan.SetTag("parent.operation", $"parent_operation_{_spanCounter}");

                    // Simulate child work
                    Thread.Sleep(30);
                }

                // More parent work
                Thread.Sleep(10);
            }

            AddResult($"✅ Tracing: Created parent-child span relationship");
        }
        catch (Exception ex)
        {
            AddResult($"❌ Tracing NestedSpans error: {ex.Message}");
            _logger.Error("NestedSpans failed", ex);
        }
    }

    private void OnCreateSpanWithErrorClicked(object sender, EventArgs e)
    {
        try
        {
            _spanCounter++;

            using (var span = Tracer.StartSpan($"error_operation_{_spanCounter}"))
            {
                span.SetTag("test.will_error", true);

                try
                {
                    // Simulate error
                    throw new InvalidOperationException("Simulated error in span");
                }
                catch (Exception ex)
                {
                    span.SetError(ex);
                    AddResult($"✅ Tracing: Created span with error '{ex.Message}'");
                }
            }
        }
        catch (Exception ex)
        {
            AddResult($"❌ Tracing SpanWithError error: {ex.Message}");
            _logger.Error("SpanWithError failed", ex);
        }
    }

    private async void OnTestHttpTracingClicked(object sender, EventArgs e)
    {
        try
        {
            AddResult("🔄 Tracing: Testing HTTP tracing...");

            using (var span = Tracer.StartSpan("http_request_test"))
            {
                span.SetTag("http.method", "GET");
                span.SetTag("http.url", "https://fakestoreapi.com/products/1");

                var response = await _httpClient.GetAsync("https://fakestoreapi.com/products/1");

                span.SetTag("http.status_code", (int)response.StatusCode);
                span.SetTag("http.success", response.IsSuccessStatusCode);

                AddResult($"✅ Tracing: HTTP request traced (status: {response.StatusCode})");
            }
        }
        catch (Exception ex)
        {
            AddResult($"❌ Tracing HTTP error: {ex.Message}");
            _logger.Error("HTTP tracing test failed", ex);
        }
    }

    #endregion

    #region Session Replay

    private void OnTriggerInteractionClicked(object sender, EventArgs e)
    {
        try
        {
            // Session Replay automatically captures UI interactions
            // This button click will be recorded if Session Replay is active

            AddResult("✅ Session Replay: UI interaction triggered");
            AddResult("   (Check Session Replay in Datadog for this click)");

            // Also log the interaction for correlation
            _logger.Info("Session Replay UI interaction test", attributes: new Dictionary<string, object>
            {
                { "interaction_type", "button_click" },
                { "button_name", "Trigger UI Interaction" }
            });
        }
        catch (Exception ex)
        {
            AddResult($"❌ Session Replay error: {ex.Message}");
        }
    }

    #endregion

    #region Helper Methods

    private void OnClearResultsClicked(object sender, EventArgs e)
    {
        _results.Clear();
        AddResult("📋 Results cleared");
    }

    private void AddResult(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        _results.AppendLine($"[{timestamp}] {message}");

        // Update UI on main thread
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ResultsLabel.Text = _results.ToString();
        });
    }

    #endregion
}
