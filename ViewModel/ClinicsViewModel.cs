using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using test.Data;
using test.Models;

namespace test.ViewModel;

public partial class ClinicsViewModel : ObservableObject
{
    private readonly AppDatabase _db;

    [ObservableProperty]
    private ObservableCollection<VetClinic> clinics = new();

    [ObservableProperty]
    private VetClinic? selectedClinic;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string? address;

    [ObservableProperty]
    private string? phone;

    public ClinicsViewModel(AppDatabase db)
    {
        _db = db;
    }

    public async Task LoadAsync()
    {
        var list = await _db.GetClinicsAsync();
        Clinics = new ObservableCollection<VetClinic>(list);
    }

    public async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            await Shell.Current.DisplayAlert("Hiba", "A rendelő neve kötelező.", "OK");
            return;
        }

        VetClinic clinic;

        if (SelectedClinic == null)
        {
            clinic = new VetClinic();
        }
        else
        {
            clinic = SelectedClinic;
        }

        clinic.Name = Name;
        clinic.Address = Address;
        clinic.Phone = Phone;

        // Koordináták opcionálisak, most nem kezeljük: maradhatnak null-ként
        clinic.Latitude = null;
        clinic.Longitude = null;

        await _db.SaveClinicAsync(clinic);
        await LoadAsync();
        ClearForm();
    }

    public async Task DeleteAsync(VetClinic clinic)
    {
        await _db.DeleteClinicAsync(clinic);
        await LoadAsync();
    }

    public void Edit(VetClinic clinic)
    {
        SelectedClinic = clinic;
        Name = clinic.Name;
        Address = clinic.Address;
        Phone = clinic.Phone;
    }

    private void ClearForm()
    {
        SelectedClinic = null;
        Name = string.Empty;
        Address = string.Empty;
        Phone = string.Empty;
    }
}
