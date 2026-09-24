using GhorSongsar.ViewModels;

namespace GhorSongsar.Views;

public partial class HistoryPage : ContentPage
{
    private readonly HistoryViewModel _viewModel;

    public HistoryPage()
    {
        InitializeComponent();
        _viewModel = MauiProgram.Services.GetRequiredService<HistoryViewModel>();
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _viewModel.LoadAsync();
    }

    private async void OnEditClicked(object? sender, EventArgs e)
    {
        if (sender is not Button { BindingContext: TransactionItem item })
            return;

        await _viewModel.EditCommand.ExecuteAsync(item);
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (sender is not Button { BindingContext: TransactionItem item })
            return;

        var confirmed = await DisplayAlertAsync(
            "Delete entry",
            "Delete this transaction? This cannot be undone.",
            "Delete",
            "Cancel");

        if (confirmed)
            await _viewModel.DeleteCommand.ExecuteAsync(item);
    }
}