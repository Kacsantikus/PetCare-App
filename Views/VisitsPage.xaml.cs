using test.Models;
using test.ViewModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace test.Views;

[QueryProperty(nameof(PetId), "PetId")]
[QueryProperty(nameof(PetName), "PetName")]
public partial class VisitsPage : ContentPage
{
    private readonly VisitsViewModel _viewModel;

    public int PetId { get; set; }
    public string PetName { get; set; } = string.Empty;

    public VisitsPage(VisitsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Pet adatok átadása a ViewModelnek
        _viewModel.SetPet(PetId, PetName);
        await _viewModel.LoadAsync();
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        await _viewModel.SaveAsync();
    }

    private async void OnLoadClicked(object? sender, EventArgs e)
    {
        await _viewModel.LoadAsync();
    }

    private void OnEditClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is VetVisit visit)
        {
            _viewModel.Edit(visit);
        }
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is VetVisit visit)
        {
            await _viewModel.DeleteAsync(visit);
        }
    }
}
