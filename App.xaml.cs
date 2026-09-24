using GhorSongsar.Services;
using GhorSongsar.Views;
using Microsoft.Extensions.DependencyInjection;

namespace GhorSongsar;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        UserAppTheme = AppTheme.Light;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var auth = MauiProgram.Services.GetRequiredService<AuthService>();
        return auth.IsLoggedIn
            ? new Window(new AppShell())
            : new Window(LoginRoot());
    }

    public static NavigationPage LoginRoot()
    {
        var login = MauiProgram.Services.GetRequiredService<LoginPage>();
        NavigationPage.SetHasNavigationBar(login, false);
        return new NavigationPage(login);
    }
}