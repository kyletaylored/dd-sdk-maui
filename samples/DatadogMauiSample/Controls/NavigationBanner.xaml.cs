using CommunityToolkit.Mvvm.Messaging;

namespace DatadogMauiSample.Controls;

/// <summary>
/// A navigation banner control that displays user information.
/// </summary>
public partial class NavigationBanner : ContentView
{
    /// <summary>
    /// Bindable property for the user name.
    /// </summary>
    public static readonly BindableProperty UserNameProperty =
        BindableProperty.Create(nameof(UserName), typeof(string), typeof(NavigationBanner), string.Empty, propertyChanged: OnUserNameChanged);

    /// <summary>
    /// Gets or sets the user name to display.
    /// </summary>
    public string UserName
    {
        get => (string)GetValue(UserNameProperty);
        set => SetValue(UserNameProperty, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationBanner"/> class.
    /// </summary>
    public NavigationBanner()
    {
        InitializeComponent();

        // Subscribe to global user changes
        WeakReferenceMessenger.Default.Register<UserLoggedInMessage>(this, (r, m) =>
        {
            UpdateUserDisplay(m.UserName);
        });

        WeakReferenceMessenger.Default.Register<UserLoggedOutMessage>(this, (r, m) =>
        {
            UpdateUserDisplay(string.Empty);
        });

        // Check if there's already a logged-in user
        if (App.Current is App app && !string.IsNullOrEmpty(app.CurrentUser))
        {
            UpdateUserDisplay(app.CurrentUser);
        }
    }

    private static void OnUserNameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is NavigationBanner banner && newValue is string userName)
        {
            banner.UpdateUserDisplay(userName);
        }
    }

    private void UpdateUserDisplay(string userName)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                UserNameLabel.IsVisible = false;
                UserBadge.IsVisible = false;
                UserNameLabel.Text = string.Empty;
                UserInitialsLabel.Text = string.Empty;
            }
            else
            {
                UserNameLabel.IsVisible = true;
                UserBadge.IsVisible = true;
                UserNameLabel.Text = userName;
                UserInitialsLabel.Text = GetInitials(userName);
            }
        });
    }

    private string GetInitials(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1)
        {
            return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();
        }
        else
        {
            return $"{parts[0][0]}{parts[^1][0]}".ToUpper();
        }
    }
}
