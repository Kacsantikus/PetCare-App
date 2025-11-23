using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using test.Data;
using test.Models;

namespace test.ViewModel;

public partial class PetsViewModel : ObservableObject
{
    private readonly AppDatabase _db;

    [ObservableProperty]
    private ObservableCollection<Pet> pets = new();

    [ObservableProperty]
    private Pet? selectedPet;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string species = string.Empty;

    [ObservableProperty]
    private DateTime? birthDate = DateTime.Today;

    [ObservableProperty]
    private string? notes;

    [ObservableProperty]
    private string? photoPath;

    public PetsViewModel(AppDatabase db)
    {
        _db = db;
    }

    public async Task LoadAsync()
    {
        var list = await _db.GetPetsAsync();
        Pets = new ObservableCollection<Pet>(list);
    }

    public async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            await Shell.Current.DisplayAlert("Hiba", "A név megadása kötelező.", "OK");
            return;
        }

        Pet pet;

        if (SelectedPet == null)
        {
            pet = new Pet
            {
                Name = Name,
                Species = Species,
                BirthDate = BirthDate,
                Notes = Notes,
                PhotoPath = PhotoPath
            };
        }
        else
        {
            pet = SelectedPet;
            pet.Name = Name;
            pet.Species = Species;
            pet.BirthDate = BirthDate;
            pet.Notes = Notes;
            pet.PhotoPath = PhotoPath;
        }

        await _db.SavePetAsync(pet);
        await LoadAsync();
        ClearForm();
    }

    public async Task DeleteAsync(Pet pet)
    {
        await _db.DeletePetAsync(pet);
        await LoadAsync();
    }

    public void Edit(Pet pet)
    {
        SelectedPet = pet;
        Name = pet.Name;
        Species = pet.Species;
        BirthDate = pet.BirthDate;
        Notes = pet.Notes;
        PhotoPath = pet.PhotoPath;
    }

    private void ClearForm()
    {
        SelectedPet = null;
        Name = string.Empty;
        Species = string.Empty;
        BirthDate = DateTime.Today;
        Notes = string.Empty;
        PhotoPath = null;
    }
}
