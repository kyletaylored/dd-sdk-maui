namespace DatadogMauiSample.Views;

public partial class WebViewPage : ContentPage
{
    public WebViewPage()
    {
        InitializeComponent();
    }

    private void OnGoClicked(object? sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(UrlEntry.Text))
        {
            var url = UrlEntry.Text;
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                url = "https://" + url;
            }

            ShopistWebView.Source = url;
        }
    }

    private void OnRefreshClicked(object? sender, EventArgs e)
    {
        ShopistWebView.Reload();
    }

    private void OnNavigating(object? sender, WebNavigatingEventArgs e)
    {
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;
    }

    private void OnNavigated(object? sender, WebNavigatedEventArgs e)
    {
        LoadingIndicator.IsVisible = false;
        LoadingIndicator.IsRunning = false;

        if (e.Result == WebNavigationResult.Success)
        {
            UrlEntry.Text = e.Url;
        }
    }
}
