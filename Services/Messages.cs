using GhorSongsar.Models;

namespace GhorSongsar.Services;

public record TransactionEditRequest(int? TransactionId = null, TransactionType? PreSelectType = null);