using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GhorSongsar.Models;
using GhorSongsar.Services;

namespace GhorSongsar.ViewModels;

public partial class HistoryViewModel : ObservableObject
{
    private readonly DatabaseService _database;

    public HistoryViewModel(DatabaseService database)
    {
        _database = database;
    }

    public ObservableCollection<TransactionItem> Entries { get; } = new();

    [ObservableProperty]
    private DateTime monthStart = new(DateTime.Now.Year, DateTime.Now.Month, 1);

    [ObservableProperty]
    private string monthLabel = string.Empty;

    [ObservableProperty]
    private string earningsTotal = Money.Format(0);

    [ObservableProperty]
    private string expensesTotal = Money.Format(0);

    [ObservableProperty]
    private string balance = Money.Format(0);

    [ObservableProperty]
    private bool isEmpty = true;

    public async Task LoadAsync()
    {
        try
        {
            MonthLabel = MonthStart.ToString("MMMM yyyy");

            var from = MonthStart;
            var to = from.AddMonths(1);
            var entries = await _database.GetTransactionsAsync(from, to);

            Entries.Clear();
            foreach (var transaction in entries)
                Entries.Add(TransactionItem.From(transaction));

            await RefreshTotalsAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }

    private async Task RefreshTotalsAsync()
    {
        var earnings = Entries.Where(e => e.Type == TransactionType.Earning).Sum(e => e.Amount);
        var expenses = Entries.Where(e => e.Type == TransactionType.Expense).Sum(e => e.Amount);

        EarningsTotal = Money.Format(earnings);
        ExpensesTotal = Money.Format(expenses);
        Balance = Money.Format(earnings - expenses);
        IsEmpty = Entries.Count == 0;
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task PreviousMonthAsync()
    {
        MonthStart = MonthStart.AddMonths(-1);
        await LoadAsync();
    }

    [RelayCommand]
    private async Task NextMonthAsync()
    {
        MonthStart = MonthStart.AddMonths(1);
        await LoadAsync();
    }

    [RelayCommand]
    private async Task EditAsync(TransactionItem item)
    {
        WeakReferenceMessenger.Default.Send(new TransactionEditRequest(item.Id));
        await Shell.Current.GoToAsync("//add");
    }

    [RelayCommand]
    private async Task DeleteAsync(TransactionItem item)
    {
        try
        {
            var transaction = await _database.GetTransactionAsync(item.Id);
            if (transaction is not null)
                await _database.DeleteTransactionAsync(transaction);

            Entries.Remove(item);
            await RefreshTotalsAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }
}