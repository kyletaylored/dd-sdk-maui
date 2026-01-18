using ObjCRuntime;

namespace Datadog.iOS.SessionReplay
{
	[Native]
	public enum DDImagePrivacyLevel : long
	{
		NonBundledOnly = 0,
		All = 1,
		None = 2
	}

	[Native]
	public enum DDImagePrivacyLevelOverride : long
	{
		None = 0,
		MaskNone = 1,
		MaskNonBundledOnly = 2,
		MaskAll = 3
	}

	[Native]
	public enum DDSessionReplayConfigurationPrivacyLevel : long
	{
		Allow = 0,
		Mask = 1,
		MaskUserInput = 2
	}

	[Native]
	public enum DDTextAndInputPrivacyLevel : long
	{
		SensitiveInputs = 0,
		AllInputs = 1,
		All = 2
	}

	[Native]
	public enum DDTextAndInputPrivacyLevelOverride : long
	{
		None = 0,
		MaskSensitiveInputs = 1,
		MaskAllInputs = 2,
		MaskAll = 3
	}

	[Native]
	public enum DDTouchPrivacyLevel : long
	{
		Show = 0,
		Hide = 1
	}

	[Native]
	public enum DDTouchPrivacyLevelOverride : long
	{
		None = 0,
		Show = 1,
		Hide = 2
	}
}
