namespace Datadog.Maui.Configuration;

/// <summary>
/// Configuration for Session Replay.
/// </summary>
public class SessionReplayConfiguration
{
    /// <summary>
    /// Session replay sampling rate (0-100).
    /// </summary>
    public float SampleRate { get; init; } = 100;

    /// <summary>
    /// Privacy level for text and input fields.
    /// </summary>
    public TextAndInputPrivacy TextAndInputPrivacy { get; init; } = TextAndInputPrivacy.MaskSensitiveInputs;

    /// <summary>
    /// Privacy level for images.
    /// </summary>
    public ImagePrivacy ImagePrivacy { get; init; } = ImagePrivacy.MaskNonBundledOnly;

    /// <summary>
    /// Privacy level for touch interactions.
    /// </summary>
    public TouchPrivacy TouchPrivacy { get; init; } = TouchPrivacy.Show;

    /// <summary>
    /// Builder for creating SessionReplayConfiguration instances.
    /// </summary>
    public class Builder
    {
        private float _sampleRate = 100;
        private TextAndInputPrivacy _textAndInputPrivacy = TextAndInputPrivacy.MaskSensitiveInputs;
        private ImagePrivacy _imagePrivacy = ImagePrivacy.MaskNonBundledOnly;
        private TouchPrivacy _touchPrivacy = TouchPrivacy.Show;

        /// <summary>
        /// Sets the session replay sampling rate (0-100).
        /// </summary>
        /// <param name="rate">Sampling rate (0-100). 100 means all sessions are recorded.</param>
        public Builder SetSampleRate(float rate)
        {
            if (rate < 0 || rate > 100)
                throw new ArgumentOutOfRangeException(nameof(rate), "Sample rate must be between 0 and 100");

            _sampleRate = rate;
            return this;
        }

        /// <summary>
        /// Sets the privacy level for text and input fields.
        /// </summary>
        public Builder SetTextAndInputPrivacy(TextAndInputPrivacy privacy)
        {
            _textAndInputPrivacy = privacy;
            return this;
        }

        /// <summary>
        /// Sets the privacy level for images.
        /// </summary>
        public Builder SetImagePrivacy(ImagePrivacy privacy)
        {
            _imagePrivacy = privacy;
            return this;
        }

        /// <summary>
        /// Sets the privacy level for touch interactions.
        /// </summary>
        public Builder SetTouchPrivacy(TouchPrivacy privacy)
        {
            _touchPrivacy = privacy;
            return this;
        }

        /// <summary>
        /// Builds the SessionReplayConfiguration.
        /// </summary>
        public SessionReplayConfiguration Build()
        {
            return new SessionReplayConfiguration
            {
                SampleRate = _sampleRate,
                TextAndInputPrivacy = _textAndInputPrivacy,
                ImagePrivacy = _imagePrivacy,
                TouchPrivacy = _touchPrivacy
            };
        }
    }
}

/// <summary>
/// Privacy level for text and input fields in session replay.
/// </summary>
public enum TextAndInputPrivacy
{
    /// <summary>
    /// Mask only sensitive input fields (passwords, credit cards, etc.).
    /// </summary>
    MaskSensitiveInputs,

    /// <summary>
    /// Mask all input fields.
    /// </summary>
    MaskAllInputs,

    /// <summary>
    /// Mask all text and input fields.
    /// </summary>
    MaskAll
}

/// <summary>
/// Privacy level for images in session replay.
/// </summary>
public enum ImagePrivacy
{
    /// <summary>
    /// Mask only non-bundled images (e.g., user-uploaded content from network).
    /// </summary>
    MaskNonBundledOnly,

    /// <summary>
    /// Mask all images.
    /// </summary>
    MaskAll,

    /// <summary>
    /// Don't mask any images.
    /// </summary>
    MaskNone
}

/// <summary>
/// Privacy level for touch interactions in session replay.
/// </summary>
public enum TouchPrivacy
{
    /// <summary>
    /// Show touch interactions (taps, swipes, etc.).
    /// </summary>
    Show,

    /// <summary>
    /// Hide touch interactions.
    /// </summary>
    Hide
}
