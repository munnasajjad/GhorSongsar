using System.Globalization;

namespace GhorSongsar.Services;

public static class Money
{
    public const string Symbol = "৳";

    public static string Format(decimal amount)
    {
        var isWhole = amount == decimal.Truncate(amount);
        var text = isWhole ? amount.ToString("#,##0") : amount.ToString("#,##0.00");
        return $"{Symbol}{text}";
    }

    public static bool TryParse(string? text, out decimal amount)
    {
        if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out amount))
            return true;
        return decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out amount);
    }
}