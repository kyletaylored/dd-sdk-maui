namespace Datadog.Maui.Rum;

/// <summary>
/// Type of RUM resource.
/// </summary>
public enum RumResourceKind
{
    /// <summary>
    /// Image resource.
    /// </summary>
    Image,

    /// <summary>
    /// XHR/Fetch request.
    /// </summary>
    Xhr,

    /// <summary>
    /// Beacon.
    /// </summary>
    Beacon,

    /// <summary>
    /// CSS resource.
    /// </summary>
    Css,

    /// <summary>
    /// Document.
    /// </summary>
    Document,

    /// <summary>
    /// Font resource.
    /// </summary>
    Font,

    /// <summary>
    /// JavaScript resource.
    /// </summary>
    Js,

    /// <summary>
    /// Media (audio/video).
    /// </summary>
    Media,

    /// <summary>
    /// Native resource (API call).
    /// </summary>
    Native,

    /// <summary>
    /// Other resource type.
    /// </summary>
    Other
}
