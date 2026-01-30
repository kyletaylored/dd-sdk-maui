using DatadogMauiSample.Models;
using DatadogMauiSample.Services;

namespace DatadogMauiSample.Views;

/// <summary>
/// Page for displaying and managing user profile.
/// </summary>
public partial class ProfilePage : ContentPage
{
    private User _currentUser;
    private readonly ShopistApiService _apiService;
    private List<FakeStoreUser> _availableUsers = new();

    private static void Log(string message)
    {
        System.Diagnostics.Debug.WriteLine(message);
        Console.WriteLine(message);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProfilePage"/> class.
    /// </summary>
    public ProfilePage()
    {
        InitializeComponent();
        _apiService = new ShopistApiService();
        _currentUser = User.Guest;
        UpdateUI();
        // Don't load users immediately - wait for page to appear and SDK to be ready
    }

    /// <summary>
    /// Called when the page is appearing.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Load users when page appears (SDK should be ready by now)
        if (_availableUsers.Count == 0)
        {
            LoadAvailableUsersAsync();
        }
    }

    private async void LoadAvailableUsersAsync()
    {
        try
        {
            var msg = "[ProfilePage] Loading available users from API";
            System.Diagnostics.Debug.WriteLine(msg);
            Console.WriteLine(msg);
            _availableUsers = await _apiService.GetUsersAsync();
            var msg2 = $"[ProfilePage] Loaded {_availableUsers.Count} users";
            System.Diagnostics.Debug.WriteLine(msg2);
            Console.WriteLine(msg2);
        }
        catch (Exception ex)
        {
            var msg = $"[ProfilePage] Error loading users: {ex.Message}";
            System.Diagnostics.Debug.WriteLine(msg);
            Console.WriteLine(msg);
        }
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

    private async void OnQuickSignInClicked(object? sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string userName)
        {
            // Use real FakeStore API users
            FakeStoreUser? fakeStoreUser = null;

            // Map button command parameters to FakeStore users (user IDs 1-3)
            switch (userName.ToLower())
            {
                case "alice":
                    fakeStoreUser = _availableUsers.FirstOrDefault(u => u.Id == 1);
                    break;
                case "bob":
                    fakeStoreUser = _availableUsers.FirstOrDefault(u => u.Id == 2);
                    break;
                case "charlie":
                    fakeStoreUser = _availableUsers.FirstOrDefault(u => u.Id == 3);
                    break;
            }

            if (fakeStoreUser != null)
            {
                await SignInWithApiAsync(fakeStoreUser.Username, fakeStoreUser.Password);
            }
            else
            {
                // Fallback to old behavior if API users not loaded
                var email = $"{userName}@example.com";
                var displayName = char.ToUpper(userName[0]) + userName.Substring(1);
                SignInUser(displayName, email);
            }
        }
    }

    private async Task<bool> SignInWithApiAsync(string username, string password)
    {
        try
        {
            Log($"[ProfilePage] Attempting login with username: {username}");

            var (success, token, error) = await _apiService.LoginAsync(username, password);

            if (success && !string.IsNullOrEmpty(token))
            {
                // Find the user details from loaded users
                var fakeStoreUser = _availableUsers.FirstOrDefault(u => u.Username == username);

                if (fakeStoreUser != null)
                {
                    var displayName = fakeStoreUser.Name != null
                        ? $"{fakeStoreUser.Name.Firstname} {fakeStoreUser.Name.Lastname}"
                        : fakeStoreUser.Username;

                    _currentUser = new User
                    {
                        Id = fakeStoreUser.Id.ToString(),
                        Name = displayName,
                        Email = fakeStoreUser.Email,
                        AvatarUrl = $"https://api.dicebear.com/7.x/avataaars/svg?seed={fakeStoreUser.Email}"
                    };

                    // Set user in Datadog RUM
                    if (App.Current is App app)
                    {
                        app.SetCurrentUser(_currentUser.Name);
                    }

                    Log($"[Datadog] User signed in via API: {_currentUser.Name} ({_currentUser.Email})");

                    UpdateUI();
                    await DisplayAlert("Success", $"Welcome, {_currentUser.Name}!\n\nAuthentication token received.", "OK");
                    return true;
                }
            }
            else
            {
                Log($"[ProfilePage] Login failed: {error}");
                await DisplayAlert("Login Failed", error ?? "Unknown error", "OK");
                return false;
            }
        }
        catch (Exception ex)
        {
            Log($"[ProfilePage] Login exception: {ex.Message}");
            await DisplayAlert("Error", $"Login error: {ex.Message}", "OK");
        }

        return false;
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

        // Set user in Datadog RUM and update app state
        if (App.Current is App app)
        {
            app.SetCurrentUser(_currentUser.Name);
        }

        Console.WriteLine($"[Datadog] User signed in: {_currentUser.Name} ({_currentUser.Email})");

        UpdateUI();
        DisplayAlert("Success", $"Welcome, {_currentUser.Name}!", "OK");
    }

    private void OnSignOutClicked(object? sender, EventArgs e)
    {
        _currentUser = User.Guest;

        // Clear user in Datadog RUM and update app state
        if (App.Current is App app)
        {
            app.SetCurrentUser(string.Empty);
        }

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

    private async void OnTestBadLoginClicked(object? sender, EventArgs e)
    {
        Log("[ProfilePage] Testing bad login credentials");
        await SignInWithApiAsync("invaliduser", "wrongpassword");
    }

    private async void OnDebugInfoClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new DebugInfoPage());
    }
}
