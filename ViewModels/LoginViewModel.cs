using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GhorSongsar;
using GhorSongsar.Services;

namespace GhorSongsar.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly AuthService _auth;

    public LoginViewModel(AuthService auth)
    {
        _auth = auth;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ModeHeader))]
    [NotifyPropertyChangedFor(nameof(ButtonText))]
    private bool isSetupMode;

    public string ModeHeader => IsSetupMode ? "Create your household account" : "Welcome back";

    public string ButtonText => IsSetupMode ? "Create Account" : "Log In";

    [ObservableProperty]
    private string mobile = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    [ObservableProperty]
    private string message = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    public async Task InitializeAsync()
    {
        try
        {
            IsSetupMode = !await _auth.HasAccountAsync();
            if (!IsSetupMode)
                Mobile = await _auth.GetMobileAsync() ?? Mobile;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
            Message = string.Empty;
        }
        catch (Exception ex)
        {
            Message = "Could not open the app database.";
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }

    [RelayCommand]
    private async Task PrimaryActionAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        Message = string.Empty;
        try
        {
            var mobile = Mobile.Trim();
            if (mobile.Length < 10 || !mobile.All(char.IsDigit))
            {
                Message = "Please enter a valid mobile number.";
                return;
            }

            if (IsSetupMode)
            {
                if (Password.Length < 4)
                {
                    Message = "Password must be at least 4 characters.";
                    return;
                }

                if (Password != ConfirmPassword)
                {
                    Message = "Passwords do not match.";
                    return;
                }

                await _auth.RegisterAsync(mobile, Password);
            }
            else
            {
                var ok = await _auth.LoginAsync(mobile, Password);
                if (!ok)
                {
                    Message = "Wrong mobile number or password.";
                    return;
                }
            }

            ShowApp();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static void ShowApp()
    {
        var window = Application.Current?.Windows.FirstOrDefault();
        if (window is null)
        {
            // App pending; startup will route automatically on next run.
            return;
        }

        window.Page = new AppShell();
    }
}