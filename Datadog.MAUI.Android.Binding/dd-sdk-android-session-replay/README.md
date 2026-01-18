# Datadog Android Session Replay SDK - .NET MAUI Binding

.NET MAUI bindings for Datadog Android Session Replay. Record and replay user sessions to understand user behavior, debug issues, and improve user experience.

## Installation

```bash
dotnet add package Datadog.MAUI.Android.SessionReplay
```

Or add to your `.csproj`:

```xml
<PackageReference Include="Datadog.MAUI.Android.SessionReplay" Version="3.5.0" />
```

**Note**: Requires `Datadog.MAUI.Android.Core` and `Datadog.MAUI.Android.RUM` to be initialized first.

## Overview

Session Replay provides:
- **Visual playback** of user sessions
- **Privacy controls** for sensitive data (text, images, touches)
- **Automatic recording** of views and interactions
- **Performance-optimized** recording
- **RUM integration** to correlate replays with errors and performance

## Quick Start

### 1. Enable Session Replay

In your `MainApplication.cs` after Core SDK and RUM initialization:

```csharp
using Com.Datadog.Android.Sessionreplay;
using Com.Datadog.Android.Privacy;

// Create session replay configuration
var sessionReplayConfig = new SessionReplayConfiguration.Builder(
    sampleRate: 100f  // Record 100% of sessions
)
.SetTextAndInputPrivacy(TextAndInputPrivacy.MaskSensitiveInputs)
.SetImagePrivacy(ImagePrivacy.MaskNone)
.SetTouchPrivacy(TouchPrivacy.Show)
.Build();

// Enable session replay
SessionReplay.Enable(sessionReplayConfig, Com.Datadog.Android.Datadog.Instance);

Console.WriteLine("[Datadog] Session Replay enabled");
```

That's it! Sessions are now recorded and can be viewed in Datadog RUM Explorer.

## Configuration

### SessionReplayConfiguration.Builder

```csharp
var config = new SessionReplayConfiguration.Builder(sampleRate)
    // Privacy settings for text and input fields
    .SetTextAndInputPrivacy(TextAndInputPrivacy.MaskSensitiveInputs)

    // Privacy settings for images
    .SetImagePrivacy(ImagePrivacy.MaskNone)

    // Privacy settings for touch interactions
    .SetTouchPrivacy(TouchPrivacy.Show)

    // Custom view mapper to transform/filter views
    .SetPrivacy(customPrivacyOption)

    .Build();
```

## Privacy Settings

Session Replay provides fine-grained privacy controls to protect sensitive user data.

### Text and Input Privacy

Controls how text and input fields are recorded:

```csharp
// Mask ALL text and inputs (most private)
.SetTextAndInputPrivacy(TextAndInputPrivacy.MaskAll)

// Mask SENSITIVE inputs only (recommended for most apps)
// - Passwords, credit cards, email fields masked
// - Regular text visible
.SetTextAndInputPrivacy(TextAndInputPrivacy.MaskSensitiveInputs)

// Allow ALL text (least private - use with caution)
.SetTextAndInputPrivacy(TextAndInputPrivacy.AllowAll)
```

**Recommended**: Use `MaskSensitiveInputs` for balance of privacy and visibility.

**What's Considered Sensitive:**
- Password fields (`inputType="textPassword"`)
- Credit card fields (detected by field names/hints)
- Email fields (`inputType="textEmailAddress"`)
- Phone number fields
- Fields marked with `android:importantForAutofill="no"`

### Image Privacy

Controls how images are recorded:

```csharp
// Mask ALL images (most private)
.SetImagePrivacy(ImagePrivacy.MaskAll)

// Mask NON-BUNDLED images only (recommended)
// - App bundled images (drawables) visible
// - User-uploaded images masked
.SetImagePrivacy(ImagePrivacy.MaskNonBundled)

// Show ALL images (least private)
.SetImagePrivacy(ImagePrivacy.MaskNone)
```

**Recommended**: Use `MaskNonBundled` to show UI elements while protecting user content.

### Touch Privacy

Controls whether touch indicators are shown:

```csharp
// HIDE touch indicators
.SetTouchPrivacy(TouchPrivacy.Hide)

// SHOW touch indicators (recommended for debugging)
.SetTouchPrivacy(TouchPrivacy.Show)
```

**Recommended**: Use `Show` for better debugging of user interactions.

## Sample Rate

Control what percentage of sessions to record:

```csharp
// Development - record all sessions
var config = new SessionReplayConfiguration.Builder(100f);  // 100%

// Production - record subset to manage costs
var config = new SessionReplayConfiguration.Builder(20f);   // 20%

// High-traffic app - record small sample
var config = new SessionReplayConfiguration.Builder(5f);    // 5%
```

**Sample Rate**: Value between 0-100 representing percentage of RUM sessions to record.

**Cost Considerations**:
- Session replay generates more data than standard RUM
- Use lower sample rates in production (10-20%)
- Consider sampling only error sessions (see Advanced)

## Complete Example

```csharp
using Android.App;
using Com.Datadog.Android.Sessionreplay;
using Com.Datadog.Android.Privacy;

[Application]
public class MainApplication : MauiApplication
{
    public override void OnCreate()
    {
        base.OnCreate();

        // Initialize in order: Core → RUM → Session Replay
        InitializeDatadogCore();
        InitializeRUM();
        InitializeSessionReplay();
    }

    private void InitializeSessionReplay()
    {
        try
        {
            Console.WriteLine("[Datadog] Enabling Session Replay...");

            #if DEBUG
            var sampleRate = 100f;  // Record all sessions in debug
            #else
            var sampleRate = 20f;   // Record 20% in production
            #endif

            var sessionReplayConfig = new SessionReplayConfiguration.Builder(sampleRate)
                // Privacy settings
                .SetTextAndInputPrivacy(TextAndInputPrivacy.MaskSensitiveInputs)
                .SetImagePrivacy(ImagePrivacy.MaskNonBundled)
                .SetTouchPrivacy(TouchPrivacy.Show)
                .Build();

            SessionReplay.Enable(
                sessionReplayConfig,
                Com.Datadog.Android.Datadog.Instance
            );

            Console.WriteLine($"[Datadog] Session Replay enabled (sample rate: {sampleRate}%)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Datadog] Session Replay failed: {ex.Message}");
        }
    }
}
```

## Viewing Session Replays

### In Datadog RUM Explorer

1. Navigate to **RUM → Sessions**
2. Click on a session
3. Look for the **"Session Replay"** tab
4. Click **Play** to watch the session

### Filtering for Replays

```
@session.has_replay:true
```

Use this filter in RUM Explorer to show only sessions with replays.

### Replays with Errors

Find sessions where users encountered errors:

```
@session.has_replay:true AND @error.count:>0
```

## Advanced Features

### Conditional Recording

Record only sessions with errors (saves costs):

```csharp
// In RUM configuration
var rumConfig = new RumConfiguration.Builder(applicationId)
    .TrackUserInteractions()
    .Build();

Rum.Enable(rumConfig);

// In your error handling
var monitor = GlobalRumMonitor.Get();

try
{
    // ... code that might error ...
}
catch (Exception ex)
{
    // Log error to RUM
    monitor.AddError(ex.Message, RumErrorSource.Source, ex.StackTrace, null);

    // Start session replay when error occurs
    if (!IsSessionReplayActive())
    {
        StartSessionReplayForErrorSession();
    }
}
```

### Custom Privacy Rules

Implement custom privacy logic:

```csharp
// Mark specific views as sensitive
public class SensitiveView : View
{
    public SensitiveView(Context context) : base(context)
    {
        // Tag view as sensitive
        SetTag(Resource.Id.datadog_tag_privacy, "mask");
    }
}
```

### Manually Control Recording

Control recording programmatically:

```csharp
// Stop recording
SessionReplay.StopRecording();

// Resume recording
SessionReplay.StartRecording();
```

**Use Cases**:
- Stop recording during sensitive operations (payment entry)
- Pause recording during background execution
- Control recording based on user preferences

## Privacy Best Practices

### 1. Default to Privacy-Preserving Settings

```csharp
// Good - balance of privacy and visibility
.SetTextAndInputPrivacy(TextAndInputPrivacy.MaskSensitiveInputs)
.SetImagePrivacy(ImagePrivacy.MaskNonBundled)

// Avoid - exposes too much
.SetTextAndInputPrivacy(TextAndInputPrivacy.AllowAll)
.SetImagePrivacy(ImagePrivacy.MaskNone)
```

### 2. Mask Sensitive Screens

For screens with PII, increase privacy:

```csharp
public class PaymentActivity : Activity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // For this activity, mask all text
        SessionReplay.SetPrivacyOverride(
            TextAndInputPrivacy.MaskAll,
            ImagePrivacy.MaskAll
        );
    }

    protected override void OnDestroy()
    {
        // Reset to default privacy
        SessionReplay.ResetPrivacyOverride();
        base.OnDestroy();
    }
}
```

### 3. Disable for Sensitive Users

Allow users to opt out:

```csharp
// Check user preference
var userOptOut = Preferences.Get("session_replay_opt_out", false);

if (!userOptOut)
{
    SessionReplay.Enable(config, Datadog.Instance);
}
else
{
    Console.WriteLine("[Datadog] User opted out of session replay");
}
```

### 4. Comply with Privacy Regulations

**GDPR/CCPA Considerations**:
- Inform users about session recording
- Provide opt-out mechanism
- Use appropriate masking levels
- Document data retention policies

```csharp
// Show privacy notice
if (!Preferences.Get("privacy_notice_shown", false))
{
    ShowPrivacyNotice();
    Preferences.Set("privacy_notice_shown", true);
}
```

## Performance Considerations

Session Replay is optimized but does consume resources:

### Impact

- **CPU**: ~2-5% increase during recording
- **Memory**: ~10-20MB additional for buffering
- **Network**: ~2-5MB per minute of recorded session
- **Battery**: Minimal impact (<1% additional drain)

### Optimization Tips

```csharp
// 1. Use lower sample rates in production
var config = new SessionReplayConfiguration.Builder(10f);  // 10% of sessions

// 2. Record only error sessions
if (errorOccurred)
{
    SessionReplay.StartRecording();
}

// 3. Stop recording during intensive operations
protected override void OnPause()
{
    SessionReplay.StopRecording();
    base.OnPause();
}

protected override void OnResume()
{
    SessionReplay.StartRecording();
    base.OnResume();
}
```

## Troubleshooting

### Session Replays Not Appearing

1. **Check RUM is enabled**:
```csharp
// Session Replay requires RUM
Rum.Enable(rumConfig);
SessionReplay.Enable(sessionReplayConfig, Datadog.Instance);
```

2. **Check sample rate**:
```csharp
// Temporarily set to 100% for testing
var config = new SessionReplayConfiguration.Builder(100f);
```

3. **Check RUM session exists**:
```
// In Datadog, verify RUM sessions are being created
@session.id:*
```

4. **Wait for upload**:
- Replays are uploaded at end of session
- May take 1-2 minutes to appear in Datadog

### Replays Are Blank/Black

1. **Check privacy settings**:
```csharp
// Ensure not masking everything
.SetTextAndInputPrivacy(TextAndInputPrivacy.MaskSensitiveInputs)  // Not MaskAll
.SetImagePrivacy(ImagePrivacy.MaskNonBundled)  // Not MaskAll
```

2. **Check view compatibility**:
- Custom views may not be fully supported
- WebViews show placeholder (content not recorded)

### High Data Usage

1. **Reduce sample rate**:
```csharp
var config = new SessionReplayConfiguration.Builder(10f);  // 10% instead of 100%
```

2. **Increase masking**:
```csharp
.SetImagePrivacy(ImagePrivacy.MaskAll)  // Reduces data size
```

3. **Shorter sessions**:
- Consider only recording first 5 minutes of session
- Stop recording after key user flows complete

## Comparison: Privacy Levels

| Setting | `MaskAll` | `MaskSensitiveInputs` | `AllowAll` |
|---------|-----------|----------------------|------------|
| **Regular Text** | Masked (█████) | Visible | Visible |
| **Button Labels** | Masked | Visible | Visible |
| **Password Fields** | Masked | Masked | Visible |
| **Email Fields** | Masked | Masked | Visible |
| **Credit Card** | Masked | Masked | Visible |
| **Privacy Level** | Highest | Recommended | Lowest |
| **Debug Value** | Low | High | Highest |
| **GDPR Friendly** | ✅ Yes | ✅ Yes | ⚠️ Depends |

## API Reference

### SessionReplayConfiguration.Builder

| Method | Description |
|--------|-------------|
| `Builder(float sampleRate)` | Constructor with sample rate (0-100) |
| `SetTextAndInputPrivacy(TextAndInputPrivacy)` | Set text/input masking level |
| `SetImagePrivacy(ImagePrivacy)` | Set image masking level |
| `SetTouchPrivacy(TouchPrivacy)` | Show/hide touch indicators |
| `SetPrivacy(PrivacyOption)` | Custom privacy option |
| `Build()` | Create configuration |

### SessionReplay Static Methods

| Method | Description |
|--------|-------------|
| `Enable(config, sdkInstance)` | Enable session replay |
| `StartRecording()` | Start/resume recording |
| `StopRecording()` | Stop/pause recording |
| `SetPrivacyOverride(...)` | Override privacy for current screen |
| `ResetPrivacyOverride()` | Reset to default privacy |

### Privacy Enums

#### TextAndInputPrivacy
- `MaskAll` - Mask all text and inputs
- `MaskSensitiveInputs` - Mask passwords, emails, credit cards (recommended)
- `AllowAll` - Show all text

#### ImagePrivacy
- `MaskAll` - Mask all images
- `MaskNonBundled` - Mask user images, show app images (recommended)
- `MaskNone` - Show all images

#### TouchPrivacy
- `Hide` - Don't show touch indicators
- `Show` - Show touch indicators (recommended)

## Related Modules

- **[dd-sdk-android-core](../dd-sdk-android-core/README.md)** - Required: Core SDK
- **[dd-sdk-android-rum](../dd-sdk-android-rum/README.md)** - Required: RUM tracking
- **[dd-sdk-android-logs](../dd-sdk-android-logs/README.md)** - Optional: Log correlation

## Resources

- [Session Replay Documentation](https://docs.datadoghq.com/real_user_monitoring/session_replay/android/)
- [Session Replay Privacy](https://docs.datadoghq.com/real_user_monitoring/session_replay/privacy_options/)
- [RUM Explorer](https://docs.datadoghq.com/real_user_monitoring/explorer/)
- [GitHub Repository](https://github.com/DataDog/dd-sdk-maui)

## License

Apache 2.0. See [LICENSE](../../LICENSE) for details.

Datadog Android SDK is copyright Datadog, Inc.
