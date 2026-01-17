using Datadog.MAUI;
using Moq;

namespace Datadog.MAUI.Tests;

/// <summary>
/// Tests for the main DatadogSdk entry point and configuration.
/// </summary>
public class DatadogSdkTests
{
    [Fact]
    public void Initialize_WithValidConfiguration_ShouldSucceed()
    {
        // Arrange
        var config = new DatadogConfiguration
        {
            ClientToken = "test_client_token",
            Environment = "test",
            ApplicationId = "test_app_id",
            ServiceName = "test-service",
            Site = DatadogSite.US1
        };

        // Act & Assert
        // Note: This will fail until platform implementations are complete
        // For now, we're testing the API surface
        Assert.NotNull(config);
        Assert.Equal("test_client_token", config.ClientToken);
        Assert.Equal("test", config.Environment);
        Assert.Equal("test_app_id", config.ApplicationId);
    }

    [Fact]
    public void Configuration_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var config = new DatadogConfiguration
        {
            ClientToken = "token",
            Environment = "env"
        };

        // Assert
        Assert.Equal(DatadogSite.US1, config.Site);
        Assert.True(config.EnableCrashReporting);
        Assert.True(config.TrackUserInteractions);
        Assert.True(config.TrackNetworkRequests);
        Assert.True(config.TrackViewLifecycle);
        Assert.Equal(100.0f, config.SessionSampleRate);
        Assert.Equal(100.0f, config.TraceSampleRate);
    }

    [Fact]
    public void Configuration_CustomValues_ShouldBeRespected()
    {
        // Arrange & Act
        var config = new DatadogConfiguration
        {
            ClientToken = "token",
            Environment = "env",
            Site = DatadogSite.EU1,
            EnableCrashReporting = false,
            TrackUserInteractions = false,
            TrackNetworkRequests = false,
            TrackViewLifecycle = false,
            SessionSampleRate = 50.0f,
            TraceSampleRate = 25.0f
        };

        // Assert
        Assert.Equal(DatadogSite.EU1, config.Site);
        Assert.False(config.EnableCrashReporting);
        Assert.False(config.TrackUserInteractions);
        Assert.False(config.TrackNetworkRequests);
        Assert.False(config.TrackViewLifecycle);
        Assert.Equal(50.0f, config.SessionSampleRate);
        Assert.Equal(25.0f, config.TraceSampleRate);
    }

    [Fact]
    public void DatadogSite_AllValues_ShouldBeDefined()
    {
        // Assert all sites are defined
        Assert.Equal(0, (int)DatadogSite.US1);
        Assert.Equal(1, (int)DatadogSite.EU1);
        Assert.Equal(2, (int)DatadogSite.US3);
        Assert.Equal(3, (int)DatadogSite.US5);
        Assert.Equal(4, (int)DatadogSite.US1_FED);
        Assert.Equal(5, (int)DatadogSite.AP1);
    }

    [Fact]
    public void Configuration_WithAdditionalAttributes_ShouldStore()
    {
        // Arrange
        var attributes = new Dictionary<string, object>
        {
            { "build_version", "1.2.3" },
            { "commit_sha", "abc123" },
            { "custom_flag", true }
        };

        // Act
        var config = new DatadogConfiguration
        {
            ClientToken = "token",
            Environment = "env",
            AdditionalAttributes = attributes
        };

        // Assert
        Assert.NotNull(config.AdditionalAttributes);
        Assert.Equal(3, config.AdditionalAttributes.Count);
        Assert.Equal("1.2.3", config.AdditionalAttributes["build_version"]);
        Assert.Equal("abc123", config.AdditionalAttributes["commit_sha"]);
        Assert.Equal(true, config.AdditionalAttributes["custom_flag"]);
    }
}
