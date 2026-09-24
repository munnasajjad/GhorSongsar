using GhorSongsar.ViewModels;

namespace GhorSongsar.Views;

public partial class AddEntryPage : ContentPage
{
    private readonly AddEntryViewModel _viewModel;

    public AddEntryPage()
    {
        InitializeComponent();
        _viewModel = MauiProgram.Services.GetRequiredService<AddEntryViewModel>();
        BindingContext = _viewModel;
    }
}