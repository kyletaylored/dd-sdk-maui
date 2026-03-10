using MauiDatadog = Datadog.Maui.Datadog;
using MauiLogs = Datadog.Maui.Logs.Logs;
using MauiRum = Datadog.Maui.Rum.Rum;
using MauiTracer = Datadog.Maui.Tracing.Tracer;
using MauiDatadogConfiguration = Datadog.Maui.Configuration.DatadogConfiguration;
using MauiLogsConfiguration = Datadog.Maui.Configuration.LogsConfiguration;
using MauiRumConfiguration = Datadog.Maui.Configuration.RumConfiguration;
using MauiTracingConfiguration = Datadog.Maui.Configuration.TracingConfiguration;
using MauiUserInfo = Datadog.Maui.UserInfo;
using MauiRumActionType = Datadog.Maui.Rum.RumActionType;
using MauiRumErrorSource = Datadog.Maui.Rum.RumErrorSource;
using MauiRumResourceKind = Datadog.Maui.Rum.RumResourceKind;
using MauiSite = Datadog.Maui.DatadogSite;
using MauiSpan = Datadog.Maui.Tracing.ISpan;

namespace Datadog.MAUI;

/// <summary>
/// Main entry point for the Datadog SDK.
/// Provides access to platform-specific implementations of Datadog features.
/// </summary>
public static class DatadogSdk
{
    private static IDatadogSdk? _instance;
    private static IDatadogLogger? _logger;
    private static IDatadogRum? _rum;
    private static IDatadogTrace? _trace;

    /// <summary>
    /// Gets the current Datadog SDK instance.
    /// </summary>
    public static IDatadogSdk Instance
    {
        get
        {
            if (_instance == null)
            {
                throw new InvalidOperationException(
                    "Datadog SDK has not been initialized. Call DatadogSdk.Initialize() first.");
            }
            return _instance;
        }
    }

    /// <summary>
    /// Gets the Datadog logger instance.
    /// </summary>
    public static IDatadogLogger Logger
    {
        get
        {
            if (_logger == null)
            {
                throw new InvalidOperationException(
                    "Datadog SDK has not been initialized. Call DatadogSdk.Initialize() first.");
            }
            return _logger;
        }
    }

    /// <summary>
    /// Gets the Datadog RUM instance.
    /// </summary>
    public static IDatadogRum Rum
    {
        get
        {
            if (_rum == null)
            {
                throw new InvalidOperationException(
                    "Datadog SDK has not been initialized. Call DatadogSdk.Initialize() first.");
            }
            return _rum;
        }
    }

    /// <summary>
    /// Gets the Datadog Trace instance.
    /// </summary>
    public static IDatadogTrace Trace
    {
        get
        {
            if (_trace == null)
            {
                throw new InvalidOperationException(
                    "Datadog SDK has not been initialized. Call DatadogSdk.Initialize() first.");
            }
            return _trace;
        }
    }

    /// <summary>
    /// Initializes the Datadog SDK with the provided configuration.
    /// Must be called once during application startup.
    /// </summary>
    /// <param name="configuration">The SDK configuration</param>
    public static void Initialize(DatadogConfiguration configuration)
    {
        if (_instance != null)
        {
            throw new InvalidOperationException("Datadog SDK has already been initialized.");
        }

        _instance = new CompatibilityDatadogSdk();
        _logger = new CompatibilityDatadogLogger();
        _rum = new CompatibilityDatadogRum();
        _trace = new CompatibilityDatadogTrace();

        _instance.Initialize(configuration);
    }

    /// <summary>
    /// Resets the SDK (for testing purposes only).
    /// </summary>
    internal static void Reset()
    {
        _instance = null;
        _logger = null;
        _rum = null;
        _trace = null;
    }

    private sealed class CompatibilityDatadogSdk : IDatadogSdk
    {
        public void Initialize(DatadogConfiguration configuration)
        {
            var mapped = new MauiDatadogConfiguration
            {
                ClientToken = configuration.ClientToken,
                Environment = configuration.Environment,
                ServiceName = configuration.ServiceName ?? AppInfo.PackageName ?? AppInfo.Name ?? "unknown",
                Site = MapSite(configuration.Site),
                TrackingConsent = Datadog.Maui.TrackingConsent.Pending,
                GlobalTags = ToStringDictionary(configuration.AdditionalAttributes),
                VerboseLogging = false,
                Logs = new MauiLogsConfiguration
                {
                    SampleRate = ClampToPercentage(configuration.TraceSampleRate)
                },
                Tracing = new MauiTracingConfiguration
                {
                    SampleRate = ClampToPercentage(configuration.TraceSampleRate)
                },
                Rum = string.IsNullOrWhiteSpace(configuration.ApplicationId)
                    ? null
                    : new MauiRumConfiguration
                    {
                        ApplicationId = configuration.ApplicationId,
                        SessionSampleRate = ClampToPercentage(configuration.SessionSampleRate),
                        TrackUserInteractions = configuration.TrackUserInteractions,
                        TrackViewsAutomatically = configuration.TrackViewLifecycle,
                        TrackResources = configuration.TrackNetworkRequests,
                        TrackErrors = true
                    }
            };

            MauiDatadog.Initialize(mapped);
        }

        public void SetUser(string id, string? name = null, string? email = null)
        {
            MauiDatadog.SetUser(new MauiUserInfo
            {
                Id = id,
                Name = name,
                Email = email
            });
        }

        public void ClearUser()
        {
            MauiDatadog.ClearUser();
        }

        public void AddAttribute(string key, object value)
        {
            MauiRum.AddAttribute(key, value);
            MauiLogs.AddAttribute(key, value);
        }

        public void RemoveAttribute(string key)
        {
            MauiRum.RemoveAttribute(key);
            MauiLogs.RemoveAttribute(key);
        }

        private static Dictionary<string, string> ToStringDictionary(Dictionary<string, object>? source)
        {
            if (source == null || source.Count == 0)
            {
                return new Dictionary<string, string>();
            }

            return source.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString() ?? string.Empty);
        }

        private static int ClampToPercentage(float value)
        {
            return (int)Math.Clamp(value, 0f, 100f);
        }

        private static MauiSite MapSite(DatadogSite site)
        {
            return site switch
            {
                DatadogSite.US1 => MauiSite.US1,
                DatadogSite.US3 => MauiSite.US3,
                DatadogSite.US5 => MauiSite.US5,
                DatadogSite.EU1 => MauiSite.EU1,
                DatadogSite.US1_FED => MauiSite.US1_FED,
                DatadogSite.AP1 => MauiSite.AP1,
                _ => MauiSite.US1
            };
        }
    }

    private sealed class CompatibilityDatadogLogger : IDatadogLogger
    {
        private readonly Datadog.Maui.Logs.ILogger _logger = MauiLogs.CreateLogger("DatadogSdk");

        public void Debug(string message, Dictionary<string, object>? attributes = null) => _logger.Debug(message, attributes: attributes);
        public void Info(string message, Dictionary<string, object>? attributes = null) => _logger.Info(message, attributes: attributes);
        public void Warn(string message, Dictionary<string, object>? attributes = null) => _logger.Warn(message, attributes: attributes);
        public void Error(string message, Dictionary<string, object>? attributes = null) => _logger.Error(message, attributes: attributes);
        public void Critical(string message, Dictionary<string, object>? attributes = null) => _logger.Critical(message, attributes: attributes);
    }

    private sealed class CompatibilityDatadogRum : IDatadogRum
    {
        public void StartView(string key, string name, Dictionary<string, object>? attributes = null) => MauiRum.StartView(key, name, attributes);

        public void StopView(string key, Dictionary<string, object>? attributes = null) => MauiRum.StopView(key, attributes);

        public void AddAction(string type, string name, Dictionary<string, object>? attributes = null)
        {
            var actionType = Enum.TryParse<MauiRumActionType>(type, ignoreCase: true, out var parsed)
                ? parsed
                : MauiRumActionType.Custom;

            MauiRum.AddAction(actionType, name, attributes);
        }

        public void AddError(string message, string source, Exception? exception = null, Dictionary<string, object>? attributes = null)
        {
            var errorSource = Enum.TryParse<MauiRumErrorSource>(source, ignoreCase: true, out var parsed)
                ? parsed
                : MauiRumErrorSource.Custom;

            MauiRum.AddError(message, errorSource, exception, attributes);
        }

        public void StartResource(string key, string url, string method)
        {
            MauiRum.StartResource(key, method, url);
        }

        public void StopResource(string key, int statusCode, long size, Dictionary<string, object>? attributes = null)
        {
            MauiRum.StopResource(key, statusCode, size, MauiRumResourceKind.Native, attributes);
        }

        public void StopResourceWithError(string key, string message, string source)
        {
            var errorSource = Enum.TryParse<MauiRumErrorSource>(source, ignoreCase: true, out var parsed)
                ? parsed
                : MauiRumErrorSource.Custom;

            MauiRum.AddError(message, errorSource, new Exception(message));
            MauiRum.StopResourceWithError(key, new Exception(message));
        }
    }

    private sealed class CompatibilityDatadogTrace : IDatadogTrace
    {
        public IDatadogSpan StartSpan(string operationName, Dictionary<string, object>? tags = null)
        {
            var span = MauiTracer.StartSpan(operationName);

            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    span.SetTag(tag.Key, tag.Value);
                }
            }

            return new CompatibilityDatadogSpan(span);
        }
    }

    private sealed class CompatibilityDatadogSpan(MauiSpan span) : IDatadogSpan
    {
        public void SetTag(string key, string value) => span.SetTag(key, value);
        public void SetError(Exception exception) => span.SetError(exception);
        public void Finish() => span.Finish();
        public void Dispose() => span.Dispose();
    }
}
