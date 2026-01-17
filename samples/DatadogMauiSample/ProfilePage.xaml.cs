using DatadogMauiSample.Models;

namespace DatadogMauiSample;

public partial class ProfilePage : ContentPage
{
    private User _currentUser;

    public ProfilePage()
    {
        InitializeComponent();
        _currentUser = User.Guest;
        UpdateUI();
    }

    private void OnSignInClicked(object? sender, EventArgs e)
    {
        var name = NameEntry.Text?.Trim();
        var email = EmailEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            DisplayAlert("Error", "Please enter your name", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            DisplayAlert("Error", "Please enter your email", "OK");
            return;
        }

        SignInUser(name, email);
    }

    private void OnQuickSignInClicked(object? sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string userName)
        {
            var email = $"{userName}@example.com";
            var displayName = char.ToUpper(userName[0]) + userName.Substring(1);
            SignInUser(displayName, email);
        }
    }

    private void SignInUser(string name, string email)
    {
        _currentUser = new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Email = email,
            AvatarUrl = $"https://api.dicebear.com/7.x/avataaars/svg?seed={email}"
        };

        // TODO: Set user in Datadog RUM
        // On Android:
        // GlobalRumMonitor.Get().SetUser(new UserInfo(_currentUser.Id, _currentUser.Name, _currentUser.Email))
        //
        // On iOS:
        // RUMMonitor.shared().setUserInfo(id: _currentUser.Id, name: _currentUser.Name, email: _currentUser.Email)

        Console.WriteLine($"[Datadog] User signed in: {_currentUser.Name} ({_currentUser.Email})");

        UpdateUI();
        DisplayAlert("Success", $"Welcome, {_currentUser.Name}!", "OK");
    }

    private void OnSignOutClicked(object? sender, EventArgs e)
    {
        _currentUser = User.Guest;

        // TODO: Clear user in Datadog RUM
        // On Android:
        // GlobalRumMonitor.Get().SetUser(null)
        //
        // On iOS:
        // RUMMonitor.shared().setUserInfo(id: nil, name: nil, email: nil)

        Console.WriteLine("[Datadog] User signed out");

        UpdateUI();
        DisplayAlert("Signed Out", "You have been signed out", "OK");
    }

    private void OnUpdateProfileClicked(object? sender, EventArgs e)
    {
        // TODO: Add user attribute to Datadog RUM
        // On Android:
        // GlobalRumMonitor.Get().AddUserAttribute("plan", "premium")
        //
        // On iOS:
        // RUMMonitor.shared().addUserAttribute(forKey: "plan", value: "premium")

        DisplayAlert("Profile Updated", "User attributes updated in Datadog RUM", "OK");
    }

    private void UpdateUI()
    {
        UserNameLabel.Text = _currentUser.Name;
        UserEmailLabel.Text = _currentUser.Email;
        UserIdLabel.Text = $"ID: {_currentUser.Id}";
        AvatarImage.Source = _currentUser.AvatarUrl;

        bool isGuest = _currentUser.Id == "guest";
        LoginForm.IsVisible = isGuest;
        LoggedInActions.IsVisible = !isGuest;
    }
}
