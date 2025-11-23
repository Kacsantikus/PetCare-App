using System;
using System.Globalization;
using test.Models;
using test.ViewModel;


namespace test.Views;

public partial class ClinicsPage : ContentPage
{
    private readonly ClinicsViewModel _viewModel;

    public ClinicsPage(ClinicsViewModel viewModel)
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
        if (sender is Button btn && btn.BindingContext is VetClinic clinic)
        {
            _viewModel.Edit(clinic);

            // Ha már van cím, frissítjük a térképet is arra
            UpdateMap(clinic.Name, clinic.Address);
        }
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is VetClinic clinic)
        {
            await _viewModel.DeleteAsync(clinic);
        }
    }

    private async void OnShowMapClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_viewModel.Address))
        {
            await DisplayAlert("Térkép", "Adj meg egy címet a rendelõhöz.", "OK");
            return;
        }

        // Próbálunk aktuális helyet kérni
        Location? location = null;
        try
        {
            var request = new GeolocationRequest(
                GeolocationAccuracy.Medium,
                TimeSpan.FromSeconds(5));

            location = await Geolocation.Default.GetLocationAsync(request);
        }
        catch (FeatureNotEnabledException)
        {
            await DisplayAlert("GPS", "A helymeghatározás ki van kapcsolva a készüléken.", "OK");
        }
        catch (PermissionException)
        {
            await DisplayAlert("GPS", "Nincs engedély a helyadatok használatára.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("GPS hiba", ex.Message, "OK");
        }

        UpdateMapWithRoute(_viewModel.Address!, location);
    }

    private void UpdateMapWithRoute(string address, Location? originLocation)
    {
        var dest = Uri.EscapeDataString(address);

        string url;

        if (originLocation is not null)
        {
            var lat = originLocation.Latitude.ToString(CultureInfo.InvariantCulture);
            var lon = originLocation.Longitude.ToString(CultureInfo.InvariantCulture);

            // ÚTVONAL: jelenlegi hely -> rendelõ címe
            url = $"https://www.google.com/maps/dir/?api=1&origin={lat},{lon}&destination={dest}";
        }
        else
        {
            // Ha nem tudtunk helyet kérni, sima keresés a címre
            url = $"https://www.google.com/maps/search/?api=1&query={dest}";
        }

        ClinicMap.Source = url;
    }


    private void UpdateMap(string? name, string? address)
    {
        var query = $"{name} {address}".Trim();
        if (string.IsNullOrWhiteSpace(query))
        {
            ClinicMap.Source = "https://www.google.com/maps";
            return;
        }

        var url =
            "https://www.google.com/maps/search/?api=1&query=" +
            Uri.EscapeDataString(query);

        ClinicMap.Source = url;
    }
}
