using Datadog.Maui.Logs;
using Datadog.Maui.Rum;

namespace DatadogMauiSample.Views;

/// <summary>
/// The main page of the application.
/// </summary>
public partial class MainPage : ContentPage
{
	int count = 0;
	private readonly ILogger _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="MainPage"/> class.
	/// </summary>
	public MainPage()
	{
		InitializeComponent();

		// Create a logger for this page
		_logger = Logs.CreateLogger("MainPage");

		// Start RUM view tracking for this page
		Rum.StartView("MainPage", "Main Page");

		_logger.Info("MainPage initialized");
	}

	private void OnCounterClicked(object? sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);

		// Track the user action in RUM
		Rum.AddAction(RumActionType.Tap, "CounterButton", new Dictionary<string, object>
		{
			{ "count", count }
		});

		// Log the button click
		_logger.Debug($"Counter button clicked {count} times");
	}
}
