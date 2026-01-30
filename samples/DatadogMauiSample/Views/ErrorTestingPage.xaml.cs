using System.Net;

namespace DatadogMauiSample.Views;

/// <summary>
/// Page for testing error logging and crash reporting.
/// </summary>
public partial class ErrorTestingPage : ContentPage
{
    private readonly Datadog.Maui.Logs.ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorTestingPage"/> class.
    /// </summary>
    public ErrorTestingPage()
    {
        InitializeComponent();
        _logger = Datadog.Maui.Logs.Logs.CreateLogger("ErrorTestingPage");
    }

    #region Exception Errors

    private void OnNullReferenceException(object sender, EventArgs e)
    {
        try
        {
            _logger.Info("Triggering NullReferenceException");
            string? nullString = null;
            var length = nullString!.Length; // This will throw NullReferenceException
        }
        catch (Exception ex)
        {
            _logger.Error("NullReferenceException occurred", ex);
            DisplayAlert("Error", $"NullReferenceException: {ex.Message}", "OK");
        }
    }

    private void OnDivisionByZero(object sender, EventArgs e)
    {
        try
        {
            _logger.Info("Triggering DivideByZeroException");
            int divisor = 0;
            int result = 100 / divisor; // This will throw DivideByZeroException
        }
        catch (Exception ex)
        {
            _logger.Error("DivideByZeroException occurred", ex);
            DisplayAlert("Error", $"DivideByZeroException: {ex.Message}", "OK");
        }
    }

    private void OnIndexOutOfRange(object sender, EventArgs e)
    {
        try
        {
            _logger.Info("Triggering IndexOutOfRangeException");
            var array = new int[] { 1, 2, 3 };
            var value = array[10]; // This will throw IndexOutOfRangeException
        }
        catch (Exception ex)
        {
            _logger.Error("IndexOutOfRangeException occurred", ex);
            DisplayAlert("Error", $"IndexOutOfRangeException: {ex.Message}", "OK");
        }
    }

    private void OnInvalidOperation(object sender, EventArgs e)
    {
        try
        {
            _logger.Info("Triggering InvalidOperationException");
            var list = new List<int>();
            var first = list.First(); // This will throw InvalidOperationException
        }
        catch (Exception ex)
        {
            _logger.Error("InvalidOperationException occurred", ex);
            DisplayAlert("Error", $"InvalidOperationException: {ex.Message}", "OK");
        }
    }

    private void OnArgumentException(object sender, EventArgs e)
    {
        try
        {
            _logger.Info("Triggering ArgumentException");
            ThrowArgumentException(""); // This will throw ArgumentException
        }
        catch (Exception ex)
        {
            _logger.Error("ArgumentException occurred", ex);
            DisplayAlert("Error", $"ArgumentException: {ex.Message}", "OK");
        }
    }

    private void ThrowArgumentException(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("Value cannot be null or empty", nameof(value));
        }
    }

    #endregion

    #region Async Errors

    private async void OnTaskCancellation(object sender, EventArgs e)
    {
        try
        {
            _logger.Info("Triggering TaskCanceledException");
            var cts = new CancellationTokenSource();
            cts.Cancel(); // Cancel immediately
            await Task.Delay(1000, cts.Token); // This will throw TaskCanceledException
        }
        catch (TaskCanceledException ex)
        {
            _logger.Error("TaskCanceledException occurred", ex);
            await DisplayAlert("Error", $"TaskCanceledException: {ex.Message}", "OK");
        }
        catch (Exception ex)
        {
            _logger.Error("Unexpected exception in TaskCancellation", ex);
            await DisplayAlert("Error", $"Unexpected error: {ex.Message}", "OK");
        }
    }

    private async void OnUnhandledTaskException(object sender, EventArgs e)
    {
        _logger.Info("Triggering unhandled task exception");

        // Fire and forget - this will cause an unhandled exception
        _ = Task.Run(async () =>
        {
            await Task.Delay(100);
            throw new InvalidOperationException("Unhandled exception in background task");
        });

        await DisplayAlert("Info", "Unhandled task exception triggered. Check logs in 1 second.", "OK");
    }

    private async void OnAggregateException(object sender, EventArgs e)
    {
        try
        {
            _logger.Info("Triggering AggregateException");

            var tasks = new[]
            {
                Task.Run(() => throw new InvalidOperationException("Task 1 failed")),
                Task.Run(() => throw new ArgumentException("Task 2 failed")),
                Task.Run(() => throw new NullReferenceException("Task 3 failed"))
            };

            await Task.WhenAll(tasks); // This will throw AggregateException
        }
        catch (AggregateException ex)
        {
            _logger.Error("AggregateException occurred", ex, new Dictionary<string, object>
            {
                ["InnerExceptionCount"] = ex.InnerExceptions.Count,
                ["ExceptionTypes"] = string.Join(", ", ex.InnerExceptions.Select(e => e.GetType().Name))
            });
            await DisplayAlert("Error", $"AggregateException with {ex.InnerExceptions.Count} inner exceptions", "OK");
        }
        catch (Exception ex)
        {
            _logger.Error("Unexpected exception in AggregateException test", ex);
            await DisplayAlert("Error", $"Unexpected error: {ex.Message}", "OK");
        }
    }

    #endregion

    #region Network Errors

    private async void OnHttpTimeout(object sender, EventArgs e)
    {
        try
        {
            _logger.Info("Triggering HTTP timeout");

            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMilliseconds(1) // Very short timeout
            };

            await client.GetAsync("https://httpbin.org/delay/5"); // This will timeout
        }
        catch (TaskCanceledException ex)
        {
            _logger.Error("HTTP request timeout", ex, new Dictionary<string, object>
            {
                ["Url"] = "https://httpbin.org/delay/5",
                ["TimeoutMs"] = 1
            });
            await DisplayAlert("Error", "HTTP request timed out", "OK");
        }
        catch (Exception ex)
        {
            _logger.Error("HTTP timeout test failed", ex);
            await DisplayAlert("Error", $"Unexpected error: {ex.Message}", "OK");
        }
    }

    private async void OnHttp404(object sender, EventArgs e)
    {
        try
        {
            _logger.Info("Triggering HTTP 404");

            var client = new HttpClient();
            var response = await client.GetAsync("https://httpbin.org/status/404");

            if (!response.IsSuccessStatusCode)
            {
                var error = new HttpRequestException($"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}");
                _logger.Error("HTTP 404 Not Found", error, new Dictionary<string, object>
                {
                    ["StatusCode"] = (int)response.StatusCode,
                    ["Url"] = "https://httpbin.org/status/404"
                });
                await DisplayAlert("Error", $"HTTP 404: Resource not found", "OK");
            }
        }
        catch (Exception ex)
        {
            _logger.Error("HTTP 404 test failed", ex);
            await DisplayAlert("Error", $"Unexpected error: {ex.Message}", "OK");
        }
    }

    private async void OnHttp500(object sender, EventArgs e)
    {
        try
        {
            _logger.Info("Triggering HTTP 500");

            var client = new HttpClient();
            var response = await client.GetAsync("https://httpbin.org/status/500");

            if (!response.IsSuccessStatusCode)
            {
                var error = new HttpRequestException($"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}");
                _logger.Error("HTTP 500 Server Error", error, new Dictionary<string, object>
                {
                    ["StatusCode"] = (int)response.StatusCode,
                    ["Url"] = "https://httpbin.org/status/500"
                });
                await DisplayAlert("Error", $"HTTP 500: Internal server error", "OK");
            }
        }
        catch (Exception ex)
        {
            _logger.Error("HTTP 500 test failed", ex);
            await DisplayAlert("Error", $"Unexpected error: {ex.Message}", "OK");
        }
    }

    #endregion

    #region Logging Tests

    private void OnLogError(object sender, EventArgs e)
    {
        _logger.Error("Test error message", null, new Dictionary<string, object>
        {
            ["test_type"] = "manual_error_log",
            ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ["user_action"] = "clicked_log_error_button"
        });

        DisplayAlert("Success", "Error log sent to Datadog", "OK");
    }

    private void OnLogWarning(object sender, EventArgs e)
    {
        _logger.Warn("Test warning message", null, new Dictionary<string, object>
        {
            ["test_type"] = "manual_warning_log",
            ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ["severity"] = "medium"
        });

        DisplayAlert("Success", "Warning log sent to Datadog", "OK");
    }

    private void OnLogInfoWithAttributes(object sender, EventArgs e)
    {
        _logger.Info("Test info message with custom attributes", null, new Dictionary<string, object>
        {
            ["test_type"] = "manual_info_log",
            ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ["device_platform"] = DeviceInfo.Platform.ToString(),
            ["device_model"] = DeviceInfo.Model,
            ["app_version"] = AppInfo.VersionString,
            ["custom_metric"] = 42,
            ["custom_tags"] = new[] { "testing", "manual", "info" }
        });

        DisplayAlert("Success", "Info log with attributes sent to Datadog", "OK");
    }

    #endregion

    #region Critical Crashes

    private void OnNativeCrash(object sender, EventArgs e)
    {
        DisplayAlert("Warning", "This will crash the app immediately!", "Cancel", "Crash")
            .ContinueWith(async (task) =>
            {
                if (!task.Result) // User clicked "Crash"
                {
                    await Task.Delay(500); // Brief delay to allow alert to close

                    _logger.Error("Triggering native crash", null, new Dictionary<string, object>
                    {
                        ["crash_type"] = "native",
                        ["platform"] = DeviceInfo.Platform.ToString()
                    });

#if ANDROID
                    // Trigger Android native crash
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        throw new Java.Lang.RuntimeException("Test native crash from .NET MAUI");
                    });
#elif IOS
                    // Trigger iOS native crash
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        throw new Foundation.NSErrorException(
                            new Foundation.NSError(
                                new Foundation.NSString("TestCrashDomain"),
                                -1,
                                Foundation.NSDictionary.FromObjectAndKey(
                                    new Foundation.NSString("Test native crash from .NET MAUI"),
                                    Foundation.NSError.LocalizedDescriptionKey
                                )
                            )
                        );
                    });
#else
                    throw new PlatformNotSupportedException("Native crash not implemented for this platform");
#endif
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void OnFatalException(object sender, EventArgs e)
    {
        DisplayAlert("Warning", "This will crash the app with an unhandled exception!", "Cancel", "Crash")
            .ContinueWith(async (task) =>
            {
                if (!task.Result) // User clicked "Crash"
                {
                    await Task.Delay(500); // Brief delay to allow alert to close

                    _logger.Error("Triggering fatal unhandled exception", null, new Dictionary<string, object>
                    {
                        ["crash_type"] = "unhandled_exception",
                        ["intentional"] = true
                    });

                    // Throw unhandled exception on main thread
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        throw new InvalidOperationException("Fatal unhandled exception - testing crash reporting");
                    });
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    #endregion
}
