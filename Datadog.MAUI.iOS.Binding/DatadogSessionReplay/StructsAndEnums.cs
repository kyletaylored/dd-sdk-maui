namespace DatadogMaui.iOS.SessionReplay
{
	public enum DDImagePrivacyLevel
	{
		NonBundledOnly = 0,
		All = 1,
		None = 2
	}

	public enum DDImagePrivacyLevelOverride
	{
		None = 0,
		MaskNone = 1,
		MaskNonBundledOnly = 2,
		MaskAll = 3
	}

	public enum DDTextAndInputPrivacyLevel
	{
		SensitiveInputs = 0,
		AllInputs = 1,
		All = 2
	}

	public enum DDTextAndInputPrivacyLevelOverride
	{
		None = 0,
		MaskSensitiveInputs = 1,
		MaskAllInputs = 2,
		MaskAll = 3
	}

	public enum DDTouchPrivacyLevel
	{
		Show = 0,
		Hide = 1
	}

	public enum DDTouchPrivacyLevelOverride
	{
		None = 0,
		Show = 1,
		Hide = 2
	}
}
