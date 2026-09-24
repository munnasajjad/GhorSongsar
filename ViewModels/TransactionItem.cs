using GhorSongsar.Models;
using GhorSongsar.Services;

namespace GhorSongsar.ViewModels;

public class TransactionItem
{
    public int Id { get; init; }

    public TransactionType Type { get; init; }

    public decimal Amount { get; init; }

    public string Category { get; init; } = string.Empty;

    public string Note { get; init; } = string.Empty;

    public DateTime Date { get; init; }

    public bool IsEarning => Type == TransactionType.Earning;

    public string TypeLabel => IsEarning ? "Earn" : "Spend";

    public string Sign => IsEarning ? "+" : "−";

    public string AmountText => Money.Format(Amount);

    public string DateText => Date.ToString("dd MMM yyyy");

    public string DescriptionText => string.IsNullOrWhiteSpace(Note) ? Category : $"{Category} • {Note}";

    public Color AmountColor => IsEarning ? Color.FromArgb("#0E7C61") : Color.FromArgb("#C2413B");

    public Color TagBackground => IsEarning ? Color.FromArgb("#DFF3EC") : Color.FromArgb("#FBE7E5");

    public static TransactionItem From(Transaction transaction) => new()
    {
        Id = transaction.Id,
        Type = transaction.Type,
        Amount = transaction.Amount,
        Category = transaction.Category,
        Note = transaction.Note,
        Date = transaction.Date
    };
}