namespace GhorSongsar.Models;

public static class Categories
{
    public static readonly IReadOnlyList<string> Earnings = new[]
    {
        "Salary",
        "Business",
        "Freelance",
        "Side Income",
        "Gift",
        "Other"
    };

    public static readonly IReadOnlyList<string> Expenses = new[]
    {
        "Food",
        "Rent",
        "Household",
        "Transport",
        "Utilities",
        "Medical",
        "Education",
        "Shopping",
        "Entertainment",
        "Mobile & Internet",
        "Savings",
        "Other"
    };

    public static IReadOnlyList<string> For(TransactionType type) =>
        type == TransactionType.Earning ? Earnings : Expenses;
}