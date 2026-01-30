using ObjCRuntime;

namespace Datadog.iOS.SessionReplay
{
	/// <summary>
	/// Privacy level for images in session replay.
	/// </summary>
	[Native]
	public enum DDImagePrivacyLevel : long
	{
		/// <summary>
		/// Mask only non-bundled images (e.g., user-uploaded content).
		/// </summary>
		NonBundledOnly = 0,

		/// <summary>
		/// Mask all images.
		/// </summary>
		All = 1,

		/// <summary>
		/// Don't mask any images.
		/// </summary>
		None = 2
	}

	/// <summary>
	/// Override for image privacy level on specific views.
	/// </summary>
	[Native]
	public enum DDImagePrivacyLevelOverride : long
	{
		/// <summary>
		/// No override - use default privacy level.
		/// </summary>
		None = 0,

		/// <summary>
		/// Don't mask any images.
		/// </summary>
		MaskNone = 1,

		/// <summary>
		/// Mask only non-bundled images.
		/// </summary>
		MaskNonBundledOnly = 2,

		/// <summary>
		/// Mask all images.
		/// </summary>
		MaskAll = 3
	}

	/// <summary>
	/// Overall privacy level for session replay configuration.
	/// </summary>
	[Native]
	public enum DDSessionReplayConfigurationPrivacyLevel : long
	{
		/// <summary>
		/// Allow all content to be recorded.
		/// </summary>
		Allow = 0,

		/// <summary>
		/// Mask sensitive content.
		/// </summary>
		Mask = 1,

		/// <summary>
		/// Mask only user input fields.
		/// </summary>
		MaskUserInput = 2
	}

	/// <summary>
	/// Privacy level for text and input fields in session replay.
	/// </summary>
	[Native]
	public enum DDTextAndInputPrivacyLevel : long
	{
		/// <summary>
		/// Mask only sensitive input fields (passwords, etc.).
		/// </summary>
		SensitiveInputs = 0,

		/// <summary>
		/// Mask all input fields.
		/// </summary>
		AllInputs = 1,

		/// <summary>
		/// Mask all text and input fields.
		/// </summary>
		All = 2
	}

	/// <summary>
	/// Override for text and input privacy level on specific views.
	/// </summary>
	[Native]
	public enum DDTextAndInputPrivacyLevelOverride : long
	{
		/// <summary>
		/// No override - use default privacy level.
		/// </summary>
		None = 0,

		/// <summary>
		/// Mask only sensitive inputs.
		/// </summary>
		MaskSensitiveInputs = 1,

		/// <summary>
		/// Mask all input fields.
		/// </summary>
		MaskAllInputs = 2,

		/// <summary>
		/// Mask all text and inputs.
		/// </summary>
		MaskAll = 3
	}

	/// <summary>
	/// Privacy level for touch interactions in session replay.
	/// </summary>
	[Native]
	public enum DDTouchPrivacyLevel : long
	{
		/// <summary>
		/// Show touch interactions.
		/// </summary>
		Show = 0,

		/// <summary>
		/// Hide touch interactions.
		/// </summary>
		Hide = 1
	}

	/// <summary>
	/// Override for touch privacy level on specific views.
	/// </summary>
	[Native]
	public enum DDTouchPrivacyLevelOverride : long
	{
		/// <summary>
		/// No override - use default privacy level.
		/// </summary>
		None = 0,

		/// <summary>
		/// Show touch interactions.
		/// </summary>
		Show = 1,

		/// <summary>
		/// Hide touch interactions.
		/// </summary>
		Hide = 2
	}
}
