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
        return new Window(new SplashPage());
    }

    public static NavigationPage LoginRoot()
    {
        var login = MauiProgram.Services.GetRequiredService<LoginPage>();
        NavigationPage.SetHasNavigationBar(login, false);
        return new NavigationPage(login);
    }
}