using DatadogSessionReplay;
using Foundation;
using ObjCRuntime;

namespace DatadogMaui.iOS.SessionReplay
{
	// @interface DatadogSessionReplay_Swift_782
	interface DatadogSessionReplay_Swift_782
	{
		// @property (readonly, nonatomic, strong) DDSessionReplayPrivacyOverrides * _Nonnull ddSessionReplayPrivacyOverrides;
		[Export ("ddSessionReplayPrivacyOverrides", ArgumentSemantic.Strong)]
		DDSessionReplayPrivacyOverrides DdSessionReplayPrivacyOverrides { }
	}

	// @interface DDSessionReplay
	[DisableDefaultCtor]
	interface DDSessionReplay
	{
		// +(void)enableWith:(DDSessionReplayConfiguration * _Nonnull)configuration;
		[Static]
		[Export ("enableWith:")]
		void EnableWith (DDSessionReplayConfiguration configuration);

		// +(void)startRecording;
		[Static]
		[Export ("startRecording")]
		void StartRecording ();

		// +(void)stopRecording;
		[Static]
		[Export ("stopRecording")]
		void StopRecording ();
	}

	// @interface DDSessionReplayConfiguration
	[DisableDefaultCtor]
	interface DDSessionReplayConfiguration
	{
		// @property (nonatomic) float replaySampleRate;
		[Export ("replaySampleRate")]
		float ReplaySampleRate { get; set; }

		// @property (nonatomic) enum DDTextAndInputPrivacyLevel textAndInputPrivacyLevel;
		[Export ("textAndInputPrivacyLevel", ArgumentSemantic.Assign)]
		DDTextAndInputPrivacyLevel TextAndInputPrivacyLevel { get; set; }

		// @property (nonatomic) enum DDImagePrivacyLevel imagePrivacyLevel;
		[Export ("imagePrivacyLevel", ArgumentSemantic.Assign)]
		DDImagePrivacyLevel ImagePrivacyLevel { get; set; }

		// @property (nonatomic) enum DDTouchPrivacyLevel touchPrivacyLevel;
		[Export ("touchPrivacyLevel", ArgumentSemantic.Assign)]
		DDTouchPrivacyLevel TouchPrivacyLevel { get; set; }

		// @property (nonatomic) int startRecordingImmediately;
		[Export ("startRecordingImmediately")]
		int StartRecordingImmediately { get; set; }

		// @property (copy, nonatomic) NSURL * _Nullable customEndpoint;
		[NullAllowed, Export ("customEndpoint", ArgumentSemantic.Copy)]
		NSURL CustomEndpoint { get; set; }

		// -(instancetype _Nonnull)initWithReplaySampleRate:(float)replaySampleRate textAndInputPrivacyLevel:(enum DDTextAndInputPrivacyLevel)textAndInputPrivacyLevel imagePrivacyLevel:(enum DDImagePrivacyLevel)imagePrivacyLevel touchPrivacyLevel:(enum DDTouchPrivacyLevel)touchPrivacyLevel featureFlags:(id)featureFlags __attribute__((objc_designated_initializer));
		[Export ("initWithReplaySampleRate:textAndInputPrivacyLevel:imagePrivacyLevel:touchPrivacyLevel:featureFlags:")]
		[DesignatedInitializer]
		NativeHandle Constructor (float replaySampleRate, DDTextAndInputPrivacyLevel textAndInputPrivacyLevel, DDImagePrivacyLevel imagePrivacyLevel, DDTouchPrivacyLevel touchPrivacyLevel, NSObject featureFlags);

		// -(instancetype _Nonnull)initWithReplaySampleRate:(float)replaySampleRate textAndInputPrivacyLevel:(enum DDTextAndInputPrivacyLevel)textAndInputPrivacyLevel imagePrivacyLevel:(enum DDImagePrivacyLevel)imagePrivacyLevel touchPrivacyLevel:(enum DDTouchPrivacyLevel)touchPrivacyLevel;
		[Export ("initWithReplaySampleRate:textAndInputPrivacyLevel:imagePrivacyLevel:touchPrivacyLevel:")]
		NativeHandle Constructor (float replaySampleRate, DDTextAndInputPrivacyLevel textAndInputPrivacyLevel, DDImagePrivacyLevel imagePrivacyLevel, DDTouchPrivacyLevel touchPrivacyLevel);
	}

	// @interface DDSessionReplayPrivacyOverrides
	[DisableDefaultCtor]
	interface DDSessionReplayPrivacyOverrides
	{
		// -(instancetype _Nonnull)initWithView:(id)view __attribute__((objc_designated_initializer));
		[Export ("initWithView:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSObject view);

		// @property (nonatomic) enum DDTextAndInputPrivacyLevelOverride textAndInputPrivacy;
		[Export ("textAndInputPrivacy", ArgumentSemantic.Assign)]
		DDTextAndInputPrivacyLevelOverride TextAndInputPrivacy { get; set; }

		// @property (nonatomic) enum DDImagePrivacyLevelOverride imagePrivacy;
		[Export ("imagePrivacy", ArgumentSemantic.Assign)]
		DDImagePrivacyLevelOverride ImagePrivacy { get; set; }

		// @property (nonatomic) enum DDTouchPrivacyLevelOverride touchPrivacy;
		[Export ("touchPrivacy", ArgumentSemantic.Assign)]
		DDTouchPrivacyLevelOverride TouchPrivacy { get; set; }

		// @property (nonatomic, strong) NSNumber * _Nullable hide;
		[NullAllowed, Export ("hide", ArgumentSemantic.Strong)]
		NSNumber Hide { get; set; }
	}
}
