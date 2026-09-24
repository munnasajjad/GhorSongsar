using System.Security.Cryptography;
using GhorSongsar.Models;

namespace GhorSongsar.Services;

public class AuthService
{
    private const string SessionKey = "is_logged_in";
    private const int Iterations = 100_000;

    private readonly DatabaseService _database;

    public AuthService(DatabaseService database)
    {
        _database = database;
    }

    public bool IsLoggedIn => Preferences.Default.Get(SessionKey, false);

    public async Task<bool> HasAccountAsync() => (await _database.GetAccountAsync()) is not null;

    public async Task<string?> GetMobileAsync() => (await _database.GetAccountAsync())?.Mobile;

    public async Task<bool> RegisterAsync(string mobile, string password)
    {
        var existing = await _database.GetAccountAsync();
        if (existing is not null)
            return false;

        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Hash(password, salt);

        await _database.SaveAccountAsync(new Account
        {
            Id = 1,
            Mobile = mobile.Trim(),
            PasswordSalt = Convert.ToBase64String(salt),
            PasswordHash = Convert.ToBase64String(hash),
            CreatedAt = DateTime.Now
        });

        SetLoggedIn(true);
        return true;
    }

    public async Task<bool> LoginAsync(string mobile, string password)
    {
        var account = await _database.GetAccountAsync();
        if (account is null)
            return false;

        if (!string.Equals(account.Mobile.Trim(), mobile.Trim(), StringComparison.OrdinalIgnoreCase))
            return false;

        var salt = Convert.FromBase64String(account.PasswordSalt);
        var expected = Convert.FromBase64String(account.PasswordHash);
        var actual = Hash(password, salt);

        if (!CryptographicOperations.FixedTimeEquals(actual, expected))
            return false;

        SetLoggedIn(true);
        return true;
    }

    public void Logout() => SetLoggedIn(false);

    private static void SetLoggedIn(bool value) => Preferences.Default.Set(SessionKey, value);

    private static byte[] Hash(string password, byte[] salt) =>
        Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, 32);
}