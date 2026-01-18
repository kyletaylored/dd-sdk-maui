using Foundation;
using ObjCRuntime;
using UIKit;

namespace Datadog.iOS.SessionReplay
{
	// @interface DatadogSessionReplay_Swift_818 (UIView)
	// Note: Category properties don't work in .NET 9/10 binding generator
	// This extension has been commented out - functionality may be accessed through other means
	// [Category]
	// [BaseType (typeof(UIView))]
	// interface UIView_DatadogSessionReplay_Swift_818
	// {
	// 	// @property (readonly, nonatomic, strong) DDSessionReplayPrivacyOverrides * _Nonnull ddSessionReplayPrivacyOverrides;
	// 	[Export ("ddSessionReplayPrivacyOverrides")]
	// 	DDSessionReplayPrivacyOverrides DdSessionReplayPrivacyOverrides { get; }
	// }

	// @interface DDSessionReplay : NSObject
	[BaseType (typeof(NSObject))]
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

	// @interface DDSessionReplayConfiguration : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDSessionReplayConfiguration
	{
		// @property (nonatomic) float replaySampleRate;
		[Export ("replaySampleRate")]
		float ReplaySampleRate { get; set; }

		// @property (nonatomic) enum DDSessionReplayConfigurationPrivacyLevel defaultPrivacyLevel __attribute__((deprecated("This will be removed in future versions of the SDK. Use the new privacy levels instead.")));
		[Export ("defaultPrivacyLevel", ArgumentSemantic.Assign)]
		DDSessionReplayConfigurationPrivacyLevel DefaultPrivacyLevel { get; set; }

		// @property (nonatomic) enum DDTextAndInputPrivacyLevel textAndInputPrivacyLevel;
		[Export ("textAndInputPrivacyLevel", ArgumentSemantic.Assign)]
		DDTextAndInputPrivacyLevel TextAndInputPrivacyLevel { get; set; }

		// @property (nonatomic) enum DDImagePrivacyLevel imagePrivacyLevel;
		[Export ("imagePrivacyLevel", ArgumentSemantic.Assign)]
		DDImagePrivacyLevel ImagePrivacyLevel { get; set; }

		// @property (nonatomic) enum DDTouchPrivacyLevel touchPrivacyLevel;
		[Export ("touchPrivacyLevel", ArgumentSemantic.Assign)]
		DDTouchPrivacyLevel TouchPrivacyLevel { get; set; }

		// @property (nonatomic) BOOL startRecordingImmediately;
		[Export ("startRecordingImmediately")]
		bool StartRecordingImmediately { get; set; }

		// @property (copy, nonatomic) NSUrl * _Nullable customEndpoint;
		[NullAllowed, Export ("customEndpoint", ArgumentSemantic.Copy)]
		NSUrl CustomEndpoint { get; set; }

		// @property (copy, nonatomic) NSDictionary<NSString *,NSNumber *> * _Nonnull featureFlags;
		[Export ("featureFlags", ArgumentSemantic.Copy)]
		NSDictionary<NSString, NSNumber> FeatureFlags { get; set; }

		// -(instancetype _Nonnull)initWithReplaySampleRate:(float)replaySampleRate textAndInputPrivacyLevel:(enum DDTextAndInputPrivacyLevel)textAndInputPrivacyLevel imagePrivacyLevel:(enum DDImagePrivacyLevel)imagePrivacyLevel touchPrivacyLevel:(enum DDTouchPrivacyLevel)touchPrivacyLevel featureFlags:(NSDictionary<NSString *,NSNumber *> * _Nullable)featureFlags __attribute__((objc_designated_initializer));
		[Export ("initWithReplaySampleRate:textAndInputPrivacyLevel:imagePrivacyLevel:touchPrivacyLevel:featureFlags:")]
		[DesignatedInitializer]
		NativeHandle Constructor (float replaySampleRate, DDTextAndInputPrivacyLevel textAndInputPrivacyLevel, DDImagePrivacyLevel imagePrivacyLevel, DDTouchPrivacyLevel touchPrivacyLevel, [NullAllowed] NSDictionary<NSString, NSNumber> featureFlags);

		// -(instancetype _Nonnull)initWithReplaySampleRate:(float)replaySampleRate textAndInputPrivacyLevel:(enum DDTextAndInputPrivacyLevel)textAndInputPrivacyLevel imagePrivacyLevel:(enum DDImagePrivacyLevel)imagePrivacyLevel touchPrivacyLevel:(enum DDTouchPrivacyLevel)touchPrivacyLevel;
		[Export ("initWithReplaySampleRate:textAndInputPrivacyLevel:imagePrivacyLevel:touchPrivacyLevel:")]
		NativeHandle Constructor (float replaySampleRate, DDTextAndInputPrivacyLevel textAndInputPrivacyLevel, DDImagePrivacyLevel imagePrivacyLevel, DDTouchPrivacyLevel touchPrivacyLevel);

		// -(instancetype _Nonnull)initWithReplaySampleRate:(float)replaySampleRate __attribute__((objc_designated_initializer)) __attribute__((deprecated("This will be removed in future versions of the SDK. Use `init(replaySampleRate:textAndInputPrivacyLevel:imagePrivacyLevel:touchPrivacyLevel:)` instead.")));
		[Export ("initWithReplaySampleRate:")]
		[DesignatedInitializer]
		NativeHandle Constructor (float replaySampleRate);
	}

	// @interface DDSessionReplayPrivacyOverrides : NSObject
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDSessionReplayPrivacyOverrides
	{
		// -(instancetype _Nonnull)initWithView:(UIView * _Nonnull)view __attribute__((objc_designated_initializer));
		[Export ("initWithView:")]
		[DesignatedInitializer]
		NativeHandle Constructor (UIView view);

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
