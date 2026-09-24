using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GhorSongsar;
using GhorSongsar.Models;
using GhorSongsar.Services;

namespace GhorSongsar.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly DatabaseService _database;
    private readonly AuthService _auth;

    public DashboardViewModel(DatabaseService database, AuthService auth)
    {
        _database = database;
        _auth = auth;
    }

    public ObservableCollection<TransactionItem> Recent { get; } = new();

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

    [ObservableProperty]
    private bool hasEntries;

    public async Task LoadAsync()
    {
        try
        {
            var now = DateTime.Now;
            var from = new DateTime(now.Year, now.Month, 1);
            var to = from.AddMonths(1);

            MonthLabel = from.ToString("MMMM yyyy");

            var entries = await _database.GetTransactionsAsync(from, to);
            var earnings = entries.Where(e => e.Type == TransactionType.Earning).Sum(e => e.Amount);
            var expenses = entries.Where(e => e.Type == TransactionType.Expense).Sum(e => e.Amount);

            EarningsTotal = Money.Format(earnings);
            ExpensesTotal = Money.Format(expenses);
            Balance = Money.Format(earnings - expenses);
            HasEntries = entries.Count > 0;
            IsEmpty = entries.Count == 0;

            Recent.Clear();
            var recent = await _database.GetRecentAsync(5);
            foreach (var transaction in recent)
                Recent.Add(TransactionItem.From(transaction));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }

    [RelayCommand]
    private void Add(string type)
    {
        if (type == "earning")
            WeakReferenceMessenger.Default.Send(new TransactionEditRequest(null, TransactionType.Earning));
        else
            WeakReferenceMessenger.Default.Send(new TransactionEditRequest(null, TransactionType.Expense));

        _ = Shell.Current.GoToAsync("//add");
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        var page = Application.Current?.Windows.FirstOrDefault()?.Page;
        if (page is not null)
        {
            var confirmed = await page.DisplayAlertAsync(
                "Log out",
                "Log out of GhorSongsar?",
                "Log out",
                "Cancel");

            if (!confirmed)
                return;
        }

        _auth.Logout();

        var window = Application.Current?.Windows.FirstOrDefault();
        if (window is not null)
            window.Page = App.LoginRoot();
    }
}