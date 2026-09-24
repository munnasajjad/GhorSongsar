using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GhorSongsar.Models;
using GhorSongsar.Services;

namespace GhorSongsar.ViewModels;

public partial class AddEntryViewModel : ObservableObject
{
    private readonly DatabaseService _database;
    private int? _transactionId;

    public AddEntryViewModel(DatabaseService database)
    {
        _database = database;

        CategoryOptions.Clear();
        foreach (var category in Categories.Expenses)
            CategoryOptions.Add(category);
        SelectedCategory = CategoryOptions.FirstOrDefault() ?? string.Empty;

        WeakReferenceMessenger.Default.Register<TransactionEditRequest>(
            this,
            (_, message) => _ = HandleRequestAsync(message));
    }

    public ObservableCollection<string> CategoryOptions { get; } = new();

    public IReadOnlyList<string> TypeOptions { get; } = new[] { "Expense", "Earning" };

    [ObservableProperty]
    private int typeIndex;

    partial void OnTypeIndexChanged(int value)
    {
        var options = Categories.For(value == 1 ? TransactionType.Earning : TransactionType.Expense);
        CategoryOptions.Clear();
        foreach (var category in options)
            CategoryOptions.Add(category);
        SelectedCategory = CategoryOptions.FirstOrDefault() ?? string.Empty;
    }

    [ObservableProperty]
    private string amountText = string.Empty;

    [ObservableProperty]
    private string note = string.Empty;

    [ObservableProperty]
    private DateTime selectedDate = DateTime.Today;

    [ObservableProperty]
    private DateTime maxDate = DateTime.Today;

    [ObservableProperty]
    private string selectedCategory = string.Empty;

    [ObservableProperty]
    private bool isEditing;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    private async Task HandleRequestAsync(TransactionEditRequest request)
    {
        try
        {
            StatusMessage = string.Empty;

            if (request.TransactionId is int id && id > 0)
            {
                var transaction = await _database.GetTransactionAsync(id);
                if (transaction is not null)
                {
                    IsEditing = true;
                    _transactionId = transaction.Id;
                    TypeIndex = transaction.Type == TransactionType.Earning ? 1 : 0;
                    AmountText = transaction.Amount.ToString("#0.##", CultureInfo.InvariantCulture);
                    Note = transaction.Note;
                    SelectedDate = transaction.Date;
                    if (CategoryOptions.Contains(transaction.Category))
                        SelectedCategory = transaction.Category;
                    return;
                }
            }

            IsEditing = false;
            _transactionId = null;

            if (request.PreSelectType is TransactionType preSelected)
                TypeIndex = preSelected == TransactionType.Earning ? 1 : 0;

            ResetFields();
        }
        catch (Exception ex)
        {
            StatusMessage = "Could not open the entry.";
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }

    private void ResetFields()
    {
        AmountText = string.Empty;
        Note = string.Empty;
        SelectedDate = DateTime.Today;
        SelectedCategory = CategoryOptions.FirstOrDefault() ?? string.Empty;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        StatusMessage = string.Empty;
        try
        {
            if (!Money.TryParse(AmountText, out var amount) || amount <= 0)
            {
                StatusMessage = "Enter a valid amount.";
                return;
            }

            var type = TypeIndex == 1 ? TransactionType.Earning : TransactionType.Expense;

            if (_transactionId is int id && id > 0)
            {
                var transaction = await _database.GetTransactionAsync(id);
                if (transaction is null)
                {
                    StatusMessage = "This entry no longer exists.";
                    return;
                }

                transaction.Type = type;
                transaction.Amount = amount;
                transaction.Category = SelectedCategory;
                transaction.Note = Note.Trim();
                transaction.Date = SelectedDate.Date;
                await _database.UpdateTransactionAsync(transaction);
            }
            else
            {
                await _database.InsertTransactionAsync(new Transaction
                {
                    Type = type,
                    Amount = amount,
                    Category = SelectedCategory,
                    Note = Note.Trim(),
                    Date = SelectedDate.Date
                });
            }

            IsEditing = false;
            _transactionId = null;
            ResetFields();
            StatusMessage = "Saved.";
        }
        catch (Exception ex)
        {
            StatusMessage = "Could not save. Please try again.";
            System.Diagnostics.Debug.WriteLine(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }
}