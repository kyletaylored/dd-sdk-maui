namespace Datadog.Maui;

/// <summary>
/// Datadog site (region) for data collection.
/// </summary>
public enum DatadogSite
{
    /// <summary>
    /// US1 site - datadoghq.com (default)
    /// </summary>
    US1,

    /// <summary>
    /// US3 site - us3.datadoghq.com
    /// </summary>
    US3,

    /// <summary>
    /// US5 site - us5.datadoghq.com
    /// </summary>
    US5,

    /// <summary>
    /// EU1 site - datadoghq.eu
    /// </summary>
    EU1,

    /// <summary>
    /// US1_FED site - ddog-gov.com (US Government)
    /// </summary>
    US1_FED,

    /// <summary>
    /// AP1 site - ap1.datadoghq.com (Asia Pacific)
    /// </summary>
    AP1
}
