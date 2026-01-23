# Datadog iOS Session Replay - .NET MAUI Binding

.NET MAUI bindings for Datadog iOS Session Replay. Record and replay user sessions with privacy controls to understand user behavior and debug issues.

## Installation

```bash
dotnet add package Datadog.MAUI.iOS.SessionReplay
```

Or add to your `.csproj`:

```xml
<PackageReference Include="Datadog.MAUI.iOS.SessionReplay" Version="3.5.0" />
```

**Note**: Requires `Datadog.MAUI.iOS.Core` and `Datadog.MAUI.iOS.RUM` to be initialized first.

## Overview

Session Replay provides:
- **Visual recording** of user sessions
- **Replay user interactions** to understand behavior
- **Debug issues** by seeing exactly what users saw
- **Privacy controls** to mask sensitive data
- **Automatic RUM integration** - replays linked to sessions
- **Touch indicators** to see user interactions

## Quick Start

### Enable Session Replay

In your `AppDelegate.cs` after Core and RUM initialization:

```csharp
using Datadog.iOS.Core;
using Datadog.iOS.RUM;
using Datadog.iOS.SessionReplay;

// Initialize Core first
DDDatadog.Initialize(config, DDTrackingConsent.Granted);

// Enable RUM (required)
DDRUM.Enable(rumConfig);

// Enable Session Replay
var replayConfig = new DDSessionReplayConfiguration(
    replaySampleRate: 100.0f  // Record 100% of sessions
);

// Set privacy controls
replayConfig.TextAndInputPrivacy = DDTextAndInputPrivacy.MaskSensitiveInputs;
replayConfig.ImagePrivacy = DDImagePrivacy.MaskNone;
replayConfig.TouchPrivacy = DDTouchPrivacy.Show;

DDSessionReplay.Enable(replayConfig);

Console.WriteLine("[Datadog] Session Replay enabled");
```

## Privacy Controls

### Text and Input Privacy

Control how text and input fields are recorded:

```csharp
// Mask all text content
replayConfig.TextAndInputPrivacy = DDTextAndInputPrivacy.MaskAll;

// Mask only sensitive inputs (passwords, credit cards, etc.)
replayConfig.TextAndInputPrivacy = DDTextAndInputPrivacy.MaskSensitiveInputs;  // Recommended

// Record all text (use with caution)
replayConfig.TextAndInputPrivacy = DDTextAndInputPrivacy.MaskAllInputs;

// Allow all text to be recorded
replayConfig.TextAndInputPrivacy = DDTextAndInputPrivacy.AllowAll;
```

**Recommended**: Use `MaskSensitiveInputs` for good balance between visibility and privacy.

### Image Privacy

Control how images are recorded:

```csharp
// Mask all images
replayConfig.ImagePrivacy = DDImagePrivacy.MaskAll;

// Mask images not bundled with app (user photos, downloaded images)
replayConfig.ImagePrivacy = DDImagePrivacy.MaskNonBundled;  // Recommended

// Show all images
replayConfig.ImagePrivacy = DDImagePrivacy.MaskNone;
```

**Recommended**: Use `MaskNonBundled` to hide user-uploaded content while showing UI elements.

### Touch Privacy

Control touch indicator visibility:

```csharp
// Hide touch indicators
replayConfig.TouchPrivacy = DDTouchPrivacy.Hide;

// Show touch indicators (helps understand user actions)
replayConfig.TouchPrivacy = DDTouchPrivacy.Show;  // Recommended
```

**Recommended**: Use `Show` to see where users tap.

## Sample Rate

Control what percentage of sessions to record:

```csharp
// Record 100% of sessions (development)
var replayConfig = new DDSessionReplayConfiguration(100.0f);

// Record 20% of sessions (production - cost control)
var replayConfig = new DDSessionReplayConfiguration(20.0f);

// Don't record any sessions
var replayConfig = new DDSessionReplayConfiguration(0.0f);
```

## Enums and Constants

### DDTextAndInputPrivacy

```csharp
public enum DDTextAndInputPrivacy
{
    MaskAll,              // Mask all text and inputs
    MaskAllInputs,        // Mask all input fields only
    MaskSensitiveInputs,  // Mask passwords, credit cards, etc. (recommended)
    AllowAll              // Record all text (use with caution)
}
```

### DDImagePrivacy

```csharp
public enum DDImagePrivacy
{
    MaskAll,         // Mask all images
    MaskNonBundled,  // Mask external images only (recommended)
    MaskNone         // Show all images
}
```

### DDTouchPrivacy

```csharp
public enum DDTouchPrivacy
{
    Hide,  // Hide touch indicators
    Show   // Show touch indicators (recommended)
}
```

## Complete Example

```csharp
using Foundation;
using UIKit;
using Datadog.iOS.Core;
using Datadog.iOS.RUM;
using Datadog.iOS.SessionReplay;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    private const string CLIENT_TOKEN = "YOUR_CLIENT_TOKEN";
    private const string RUM_APPLICATION_ID = "YOUR_RUM_APP_ID";

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        InitializeDatadog();
        return base.FinishedLaunching(application, launchOptions);
    }

    private void InitializeDatadog()
    {
        try
        {
            // Initialize Core SDK
            var config = new DDConfiguration(CLIENT_TOKEN, "prod");
            config.Service = "com.example.myapp";
            DDDatadog.Initialize(config, DDTrackingConsent.Granted);

            // Enable RUM (required for Session Replay)
            var rumConfig = new DDRUMConfiguration(RUM_APPLICATION_ID);
            rumConfig.SessionSampleRate = 100.0f;
            rumConfig.TrackUIKitViews();
            rumConfig.TrackUIKitActions();
            DDRUM.Enable(rumConfig);

            // Configure Session Replay
            #if DEBUG
            var sampleRate = 100.0f;  // Record all sessions in debug
            #else
            var sampleRate = 20.0f;   // Record 20% in production
            #endif

            var replayConfig = new DDSessionReplayConfiguration(sampleRate);

            // Privacy settings - balance between visibility and privacy
            replayConfig.TextAndInputPrivacy = DDTextAndInputPrivacy.MaskSensitiveInputs;
            replayConfig.ImagePrivacy = DDImagePrivacy.MaskNonBundled;
            replayConfig.TouchPrivacy = DDTouchPrivacy.Show;

            // Enable Session Replay
            DDSessionReplay.Enable(replayConfig);

            Console.WriteLine("[Datadog] Session Replay enabled");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Datadog] Session Replay initialization failed: {ex.Message}");
        }
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
```

## What Gets Recorded

### Captured Information

- ✅ **Screen layout** - UI elements, positions, sizes
- ✅ **User interactions** - Taps, swipes, scrolls (with touch indicators)
- ✅ **Navigation** - View transitions, page changes
- ✅ **Text content** - Based on privacy settings
- ✅ **Images** - Based on privacy settings
- ✅ **Form inputs** - Based on privacy settings
- ✅ **Animations** - View animations and transitions

### NOT Recorded

- ❌ **WebView content** - Embedded web pages not captured
- ❌ **Video playback** - Video content not recorded
- ❌ **Camera feed** - Live camera view not captured
- ❌ **Maps** - MapKit views show placeholder
- ❌ **System keyboards** - Native keyboard not recorded

## RUM Integration

### Automatic Correlation

Session replays are automatically linked to RUM sessions:

```csharp
// RUM must be enabled first
DDRUM.Enable(rumConfig);

// Then enable Session Replay
DDSessionReplay.Enable(replayConfig);

// Replays now include:
// - session_id: Links to RUM session
// - view_id: Links to specific RUM views
// - User interactions from RUM
```

### Viewing Replays

In Datadog RUM Explorer:
1. Navigate to **RUM → Sessions**
2. Find a session with replay available (📹 icon)
3. Click to view session details
4. Click **View Replay** to watch the session
5. See timeline with user actions, views, errors

## Privacy Best Practices

### 1. Start with Conservative Settings

```csharp
// Good default for production
replayConfig.TextAndInputPrivacy = DDTextAndInputPrivacy.MaskSensitiveInputs;
replayConfig.ImagePrivacy = DDImagePrivacy.MaskNonBundled;
replayConfig.TouchPrivacy = DDTouchPrivacy.Show;
```

### 2. Review Sensitive Screens

For screens with highly sensitive data (banking, medical):

```csharp
// Consider stricter privacy
replayConfig.TextAndInputPrivacy = DDTextAndInputPrivacy.MaskAll;
replayConfig.ImagePrivacy = DDImagePrivacy.MaskAll;
```

### 3. Test Privacy Settings

Record test sessions and verify:
- Sensitive inputs are masked
- User photos are masked
- PII is not visible in replays

### 4. Use Appropriate Sample Rate

```csharp
#if DEBUG
var sampleRate = 100.0f;  // All sessions for testing
#else
var sampleRate = 20.0f;   // Subset for production
#endif
```

### 5. Document Privacy Policy

Inform users about session recording in your privacy policy.

## Use Cases

### 1. Debug User-Reported Issues

```
User: "The app crashes when I tap the submit button"

Solution:
1. Find user's session in RUM
2. Watch replay to see exact steps
3. See the error in context
4. Identify the issue and fix it
```

### 2. Understand User Behavior

```
Question: "Why don't users complete checkout?"

Solution:
1. Watch replays of incomplete checkouts
2. See where users drop off
3. Identify confusing UI or errors
4. Improve the flow
```

### 3. Validate Bug Fixes

```
Process:
1. Record session showing the bug
2. Fix the bug
3. Record new session with fix
4. Compare before/after replays
5. Confirm fix works
```

### 4. Optimize User Experience

```
Goal: Improve onboarding

Solution:
1. Watch replays of new users
2. See where they struggle
3. Identify unclear steps
4. Simplify onboarding flow
```

## Performance Impact

Session Replay has minimal performance impact:

- **CPU**: <5% additional usage
- **Memory**: ~10-20MB for recording buffer
- **Network**: Compressed recording data uploaded in batches
- **Battery**: Minimal impact (<2% additional drain)
- **App size**: ~2MB

## Limitations

### Platform Limitations

- **WebViews**: Content not recorded (shows placeholder)
- **Custom rendering**: Some custom drawn views may not record properly
- **System views**: Some system views show placeholder
- **MapKit**: Maps show placeholder
- **SceneKit/SpriteKit**: 3D/game views not fully supported

### Privacy Limitations

- **Secure text fields**: Always masked regardless of settings
- **Passwords**: Always masked (iOS secure entry)
- **Credit cards**: Auto-detected and masked

## Troubleshooting

### Replays Not Appearing

1. **Check RUM enabled** (required):
```csharp
DDRUM.Enable(rumConfig);
```

2. **Verify Session Replay enabled**:
```csharp
DDSessionReplay.Enable(replayConfig);
```

3. **Check sample rate**:
```csharp
replayConfig = new DDSessionReplayConfiguration(100.0f);  // Record all sessions
```

4. **Wait for upload**: Replays are uploaded in batches, may take a few minutes

### Replays Missing Content

1. **Check privacy settings** - may be too restrictive:
```csharp
replayConfig.TextAndInputPrivacy = DDTextAndInputPrivacy.AllowAll;  // For testing
```

2. **Verify view hierarchy** - some custom views may not record

3. **Check view types** - WebViews, Maps don't record

### High Data Usage

Reduce sample rate:
```csharp
replayConfig = new DDSessionReplayConfiguration(10.0f);  // Only 10% of sessions
```

### Performance Issues

1. **Reduce sample rate** to record fewer sessions
2. **Check for memory leaks** in your app
3. **Update to latest SDK version**

## API Reference

### DDSessionReplay

| Method | Description |
|--------|-------------|
| `Enable(DDSessionReplayConfiguration)` | Enable session replay with configuration |

### DDSessionReplayConfiguration

| Constructor | Description |
|-------------|-------------|
| `DDSessionReplayConfiguration(float)` | Create configuration with sample rate (0-100) |

| Property | Description |
|----------|-------------|
| `TextAndInputPrivacy` | Text/input masking level |
| `ImagePrivacy` | Image masking level |
| `TouchPrivacy` | Touch indicator visibility |

## Best Practices

### 1. Enable After RUM

```csharp
// Correct order
DDDatadog.Initialize(config, consent);
DDRUM.Enable(rumConfig);
DDSessionReplay.Enable(replayConfig);  // Last
```

### 2. Use Conservative Privacy Settings

```csharp
replayConfig.TextAndInputPrivacy = DDTextAndInputPrivacy.MaskSensitiveInputs;
replayConfig.ImagePrivacy = DDImagePrivacy.MaskNonBundled;
```

### 3. Adjust Sample Rate for Production

```csharp
#if DEBUG
var sampleRate = 100.0f;
#else
var sampleRate = 20.0f;
#endif
```

### 4. Test Replay Quality

Before production:
1. Enable 100% sample rate
2. Record test sessions
3. Verify all screens record properly
4. Check privacy masking works
5. Adjust settings as needed

### 5. Monitor Data Volume

In Datadog dashboard:
1. Check replay data volume
2. Adjust sample rate if too high
3. Consider privacy settings impact on data size

## Related Modules

- **[DatadogCore](../DatadogCore/README.md)** - Required: Core SDK
- **[DatadogRUM](../DatadogRUM/README.md)** - Required: RUM tracking (replays appear in RUM sessions)
- **[DatadogCrashReporting](../DatadogCrashReporting/README.md)** - Optional: See replays leading to crashes

## Resources

- [iOS Session Replay Documentation](https://docs.datadoghq.com/real_user_monitoring/session_replay/mobile/)
- [Session Replay Privacy](https://docs.datadoghq.com/real_user_monitoring/session_replay/mobile/privacy_options/)
- [RUM Explorer](https://docs.datadoghq.com/real_user_monitoring/explorer/)
- [GitHub Repository](https://github.com/DataDog/dd-sdk-maui)

## License

Apache 2.0. See [LICENSE](../../LICENSE) for details.

Datadog iOS SDK is copyright Datadog, Inc.
