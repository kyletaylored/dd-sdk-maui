using CommunityToolkit.Mvvm.Messaging;

namespace DatadogMauiSample;

public partial class App : Application
{
	public string CurrentUser { get; set; } = string.Empty;

	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}

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

// Message classes for user login/logout events
public record UserLoggedInMessage(string UserName);
public record UserLoggedOutMessage;