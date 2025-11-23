using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using System.IO;
using test.Models;
using test.ViewModel;


namespace test.Views;

public partial class PetsPage : ContentPage
{
    private readonly PetsViewModel _viewModel;

    public PetsPage(PetsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }

    // Mentés gomb
    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        await _viewModel.SaveAsync();
    }

    // Betöltés gomb
    private async void OnLoadClicked(object? sender, EventArgs e)
    {
        await _viewModel.LoadAsync();
    }

    // Szerk. gomb – a sorhoz tartozó Pet megy át a VM-nek
    private void OnEditClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is Pet pet)
        {
            _viewModel.Edit(pet);
        }
    }

    // Törlés gomb
    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is Pet pet)
        {
            await _viewModel.DeleteAsync(pet);
        }
    }

    private async void OnVisitsClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is Pet pet)
        {
            var parameters = new Dictionary<string, object>
            {
                ["PetId"] = pet.Id,
                ["PetName"] = pet.Name
            };

            await Shell.Current.GoToAsync(nameof(VisitsPage), parameters);
        }
    }

    private async void OnPickPhotoClicked(object? sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Válassz egy képet a háziállathoz",
                FileTypes = FilePickerFileType.Images
            });

            // Ha a user bezárta a párbeszédablakot
            if (result == null)
                return;

            // Másolat az app saját mappájába
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(result.FileName)}";
            var localPath = Path.Combine(FileSystem.AppDataDirectory, fileName);

            await using (var sourceStream = await result.OpenReadAsync())
            await using (var destStream = File.OpenWrite(localPath))
            {
                await sourceStream.CopyToAsync(destStream);
            }

            // ViewModel frissítése
            _viewModel.PhotoPath = localPath;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hiba", $"Nem sikerült fotót hozzáadni: {ex.Message}", "OK");
        }
    }

}
