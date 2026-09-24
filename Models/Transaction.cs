using SQLite;

namespace GhorSongsar.Models;

[Table("Transaction")]
public class Transaction
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public TransactionType Type { get; set; }

    public decimal Amount { get; set; }

    public string Category { get; set; } = string.Empty;

    public string Note { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.Now;
}