using Datadog.MAUI;

namespace Datadog.MAUI.Tests;

/// <summary>
/// Tests for Datadog SDK interface implementations using mocks.
/// These tests verify the API contracts without requiring native bindings.
/// </summary>
public class IDatadogSdkTests
{
    private readonly DatadogSdkMock _sdk;

    public IDatadogSdkTests()
    {
        _sdk = new DatadogSdkMock();
    }

    [Fact]
    public void Initialize_FirstTime_ShouldSucceed()
    {
        // Arrange
        var config = new DatadogConfiguration
        {
            ClientToken = "test_token",
            Environment = "test"
        };

        // Act
        _sdk.Initialize(config);

        // Assert
        Assert.True(_sdk.IsInitialized);
        Assert.NotNull(_sdk.Configuration);
        Assert.Equal("test_token", _sdk.Configuration.ClientToken);
    }

    [Fact]
    public void Initialize_SecondTime_ShouldThrow()
    {
        // Arrange
        var config = new DatadogConfiguration { ClientToken = "token", Environment = "env" };
        _sdk.Initialize(config);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _sdk.Initialize(config));
    }

    [Fact]
    public void SetUser_WhenInitialized_ShouldStoreUserInfo()
    {
        // Arrange
        var config = new DatadogConfiguration { ClientToken = "token", Environment = "env" };
        _sdk.Initialize(config);

        // Act
        _sdk.SetUser("user-123", "John Doe", "john@example.com");

        // Assert
        Assert.Equal("user-123", _sdk.UserId);
        Assert.Equal("John Doe", _sdk.UserName);
        Assert.Equal("john@example.com", _sdk.UserEmail);
    }

    [Fact]
    public void SetUser_WithOnlyId_ShouldWork()
    {
        // Arrange
        var config = new DatadogConfiguration { ClientToken = "token", Environment = "env" };
        _sdk.Initialize(config);

        // Act
        _sdk.SetUser("user-123");

        // Assert
        Assert.Equal("user-123", _sdk.UserId);
        Assert.Null(_sdk.UserName);
        Assert.Null(_sdk.UserEmail);
    }

    [Fact]
    public void ClearUser_WhenUserSet_ShouldClearUserInfo()
    {
        // Arrange
        var config = new DatadogConfiguration { ClientToken = "token", Environment = "env" };
        _sdk.Initialize(config);
        _sdk.SetUser("user-123", "John Doe", "john@example.com");

        // Act
        _sdk.ClearUser();

        // Assert
        Assert.Null(_sdk.UserId);
        Assert.Null(_sdk.UserName);
        Assert.Null(_sdk.UserEmail);
    }

    [Fact]
    public void AddAttribute_ShouldStoreAttribute()
    {
        // Arrange
        var config = new DatadogConfiguration { ClientToken = "token", Environment = "env" };
        _sdk.Initialize(config);

        // Act
        _sdk.AddAttribute("app_version", "1.2.3");
        _sdk.AddAttribute("build_number", 456);
        _sdk.AddAttribute("is_beta", true);

        // Assert
        Assert.Equal("1.2.3", _sdk.Attributes["app_version"]);
        Assert.Equal(456, _sdk.Attributes["build_number"]);
        Assert.Equal(true, _sdk.Attributes["is_beta"]);
    }

    [Fact]
    public void RemoveAttribute_ShouldRemoveAttribute()
    {
        // Arrange
        var config = new DatadogConfiguration { ClientToken = "token", Environment = "env" };
        _sdk.Initialize(config);
        _sdk.AddAttribute("key", "value");

        // Act
        _sdk.RemoveAttribute("key");

        // Assert
        Assert.False(_sdk.Attributes.ContainsKey("key"));
    }

    [Fact]
    public void Initialize_WithAdditionalAttributes_ShouldApplyThem()
    {
        // Arrange
        var config = new DatadogConfiguration
        {
            ClientToken = "token",
            Environment = "env",
            AdditionalAttributes = new Dictionary<string, object>
            {
                { "initial_key", "initial_value" }
            }
        };

        // Act
        _sdk.Initialize(config);

        // Assert
        Assert.Equal("initial_value", _sdk.Attributes["initial_key"]);
    }
}

public class IDatadogLoggerTests
{
    private readonly DatadogLoggerMock _logger;

    public IDatadogLoggerTests()
    {
        _logger = new DatadogLoggerMock();
    }

    [Fact]
    public void Debug_ShouldLogMessage()
    {
        // Act
        _logger.Debug("Debug message");

        // Assert
        Assert.Single(_logger.Logs);
        Assert.Equal("Debug", _logger.Logs[0].Level);
        Assert.Equal("Debug message", _logger.Logs[0].Message);
    }

    [Fact]
    public void AllLevels_ShouldLogCorrectly()
    {
        // Act
        _logger.Debug("Debug");
        _logger.Info("Info");
        _logger.Warn("Warn");
        _logger.Error("Error");
        _logger.Critical("Critical");

        // Assert
        Assert.Equal(5, _logger.Logs.Count);
        Assert.Equal("Debug", _logger.Logs[0].Level);
        Assert.Equal("Info", _logger.Logs[1].Level);
        Assert.Equal("Warn", _logger.Logs[2].Level);
        Assert.Equal("Error", _logger.Logs[3].Level);
        Assert.Equal("Critical", _logger.Logs[4].Level);
    }

    [Fact]
    public void Log_WithAttributes_ShouldStoreAttributes()
    {
        // Arrange
        var attributes = new Dictionary<string, object>
        {
            { "user_id", "123" },
            { "action", "purchase" }
        };

        // Act
        _logger.Info("User action", attributes);

        // Assert
        Assert.NotNull(_logger.Logs[0].Attributes);
        Assert.Equal("123", _logger.Logs[0].Attributes["user_id"]);
        Assert.Equal("purchase", _logger.Logs[0].Attributes["action"]);
    }
}

public class IDatadogRumTests
{
    private readonly DatadogRumMock _rum;

    public IDatadogRumTests()
    {
        _rum = new DatadogRumMock();
    }

    [Fact]
    public void StartView_ShouldRecordEvent()
    {
        // Act
        _rum.StartView("checkout", "Checkout Page");

        // Assert
        Assert.Single(_rum.Events);
        Assert.Equal("ViewStart", _rum.Events[0].Type);
        Assert.Equal("checkout", _rum.Events[0].Key);
        Assert.Equal("Checkout Page", _rum.Events[0].Value);
    }

    [Fact]
    public void StopView_ShouldRecordEvent()
    {
        // Act
        _rum.StopView("checkout");

        // Assert
        Assert.Single(_rum.Events);
        Assert.Equal("ViewStop", _rum.Events[0].Type);
        Assert.Equal("checkout", _rum.Events[0].Key);
    }

    [Fact]
    public void AddAction_ShouldRecordEvent()
    {
        // Act
        _rum.AddAction("tap", "Purchase Button");

        // Assert
        Assert.Single(_rum.Events);
        Assert.Equal("Action", _rum.Events[0].Type);
        Assert.Equal("tap", _rum.Events[0].Key);
        Assert.Equal("Purchase Button", _rum.Events[0].Value);
    }

    [Fact]
    public void AddError_ShouldRecordEvent()
    {
        // Arrange
        var exception = new InvalidOperationException("Test error");

        // Act
        _rum.AddError("Error occurred", "network", exception);

        // Assert
        Assert.Single(_rum.Events);
        Assert.Equal("Error", _rum.Events[0].Type);
        Assert.Equal("network", _rum.Events[0].Key);
        Assert.Equal("Error occurred", _rum.Events[0].Value);
        Assert.NotNull(_rum.Events[0].Attributes);
        Assert.Equal(exception, _rum.Events[0].Attributes!["exception"]);
    }

    [Fact]
    public void ResourceTracking_ShouldRecordEvents()
    {
        // Act
        _rum.StartResource("req-1", "https://api.example.com/users", "GET");
        _rum.StopResource("req-1", 200, 1024);

        // Assert
        Assert.Equal(2, _rum.Events.Count);

        // Check start event
        Assert.Equal("ResourceStart", _rum.Events[0].Type);
        Assert.Equal("https://api.example.com/users", _rum.Events[0].Attributes!["url"]);
        Assert.Equal("GET", _rum.Events[0].Attributes["method"]);

        // Check stop event
        Assert.Equal("ResourceStop", _rum.Events[1].Type);
        Assert.Equal(200, _rum.Events[1].Attributes!["status_code"]);
        Assert.Equal(1024L, _rum.Events[1].Attributes["size"]);
    }

    [Fact]
    public void ResourceError_ShouldRecordEvent()
    {
        // Act
        _rum.StartResource("req-1", "https://api.example.com/users", "GET");
        _rum.StopResourceWithError("req-1", "Network timeout", "network");

        // Assert
        Assert.Equal(2, _rum.Events.Count);
        Assert.Equal("ResourceError", _rum.Events[1].Type);
        Assert.Equal("Network timeout", _rum.Events[1].Attributes!["error_message"]);
        Assert.Equal("network", _rum.Events[1].Attributes["error_source"]);
    }
}

public class IDatadogTraceTests
{
    private readonly DatadogTraceMock _trace;

    public IDatadogTraceTests()
    {
        _trace = new DatadogTraceMock();
    }

    [Fact]
    public void StartSpan_ShouldCreateSpan()
    {
        // Act
        var span = _trace.StartSpan("process_payment");

        // Assert
        Assert.NotNull(span);
        Assert.Single(_trace.Spans);
        Assert.Equal("process_payment", _trace.Spans[0].OperationName);
    }

    [Fact]
    public void StartSpan_WithTags_ShouldApplyTags()
    {
        // Arrange
        var tags = new Dictionary<string, object>
        {
            { "payment_method", "credit_card" },
            { "amount", 99.99 }
        };

        // Act
        var span = _trace.StartSpan("process_payment", tags);

        // Assert
        var spanMock = _trace.Spans[0];
        Assert.Equal("credit_card", spanMock.Tags["payment_method"]);
        Assert.Equal("99.99", spanMock.Tags["amount"]);
    }

    [Fact]
    public void Span_SetTag_ShouldUpdateTags()
    {
        // Arrange
        var span = _trace.StartSpan("operation");

        // Act
        span.SetTag("custom_tag", "value");

        // Assert
        var spanMock = _trace.Spans[0];
        Assert.Equal("value", spanMock.Tags["custom_tag"]);
    }

    [Fact]
    public void Span_SetError_ShouldStoreException()
    {
        // Arrange
        var span = _trace.StartSpan("operation");
        var exception = new InvalidOperationException("Test error");

        // Act
        span.SetError(exception);

        // Assert
        var spanMock = _trace.Spans[0];
        Assert.Equal(exception, spanMock.Error);
    }

    [Fact]
    public void Span_Finish_ShouldMarkAsFinished()
    {
        // Arrange
        var span = _trace.StartSpan("operation");

        // Act
        span.Finish();

        // Assert
        var spanMock = _trace.Spans[0];
        Assert.True(spanMock.IsFinished);
        Assert.NotNull(spanMock.FinishTime);
    }

    [Fact]
    public void Span_Dispose_ShouldFinish()
    {
        // Arrange & Act
        using (var span = _trace.StartSpan("operation"))
        {
            Assert.False(_trace.Spans[0].IsFinished);
        }

        // Assert
        Assert.True(_trace.Spans[0].IsFinished);
    }

    [Fact]
    public void Span_FinishTwice_ShouldThrow()
    {
        // Arrange
        var span = _trace.StartSpan("operation");
        span.Finish();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => span.Finish());
    }

    [Fact]
    public void Span_SetTagAfterFinish_ShouldThrow()
    {
        // Arrange
        var span = _trace.StartSpan("operation");
        span.Finish();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => span.SetTag("key", "value"));
    }
}
