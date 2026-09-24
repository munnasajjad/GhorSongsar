using SQLite;

namespace GhorSongsar.Models;

[Table("Account")]
public class Account
{
    // A single shared household account (this phone).
    [PrimaryKey]
    public int Id { get; set; } = 1;

    [Unique]
    public string Mobile { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string PasswordSalt { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}