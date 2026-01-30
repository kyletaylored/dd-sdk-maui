using CommunityToolkit.Mvvm.Messaging;

namespace DatadogMauiSample;

/// <summary>
/// Main application class for the Datadog MAUI sample app.
/// </summary>
public partial class App : Application
{
	/// <summary>
	/// Gets or sets the current logged-in user name.
	/// </summary>
	public string CurrentUser { get; set; } = string.Empty;

	/// <summary>
	/// Initializes a new instance of the <see cref="App"/> class.
	/// </summary>
	public App()
	{
		InitializeComponent();
	}

	/// <summary>
	/// Creates the main application window.
	/// </summary>
	/// <param name="activationState">The activation state.</param>
	/// <returns>The main window.</returns>
	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}

	/// <summary>
	/// Sets the current user and updates Datadog user information.
	/// </summary>
	/// <param name="userName">The user name to set.</param>
	public void SetCurrentUser(string userName)
	{
		CurrentUser = userName;

		// Update Datadog user info
		if (!string.IsNullOrWhiteSpace(userName))
		{
			Datadog.Maui.Datadog.SetUser(new Datadog.Maui.UserInfo
			{
				Id = userName.ToLowerInvariant().Replace(" ", "_"),
				Name = userName,
				Email = $"{userName.ToLowerInvariant().Replace(" ", ".")}@example.com"
			});

			WeakReferenceMessenger.Default.Send(new UserLoggedInMessage(userName));
		}
		else
		{
			Datadog.Maui.Datadog.ClearUser();
			WeakReferenceMessenger.Default.Send(new UserLoggedOutMessage());
		}
	}
}

/// <summary>
/// Message sent when a user logs in.
/// </summary>
/// <param name="UserName">The user name.</param>
public record UserLoggedInMessage(string UserName);

/// <summary>
/// Message sent when a user logs out.
/// </summary>
public record UserLoggedOutMessage;