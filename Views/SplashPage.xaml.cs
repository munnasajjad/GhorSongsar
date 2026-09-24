using GhorSongsar.Services;

namespace GhorSongsar.Views;

public partial class SplashPage : ContentPage
{
    private bool _navigated;

    public SplashPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_navigated)
            return;
        _navigated = true;

        await Task.Delay(1600);

        var window = Application.Current?.Windows.FirstOrDefault();
        if (window is null)
            return;

        var auth = MauiProgram.Services.GetRequiredService<AuthService>();
        window.Page = auth.IsLoggedIn ? new AppShell() : App.LoginRoot();
    }
}