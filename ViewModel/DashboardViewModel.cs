using CommunityToolkit.Mvvm.ComponentModel;
using test.Data;
using test.Models;

namespace test.ViewModel;

public partial class DashboardViewModel : ObservableObject
{
    private readonly AppDatabase _db;

    [ObservableProperty]
    private int totalPets;

    [ObservableProperty]
    private int totalVisits;

    [ObservableProperty]
    private int totalClinics;

    [ObservableProperty]
    private bool hasUpcomingVisit;

    [ObservableProperty]
    private string upcomingVisitText = "Nincs közelgő kontroll.";

    public DashboardViewModel(AppDatabase db)
    {
        _db = db;
    }

    public async Task LoadAsync()
    {
        // Állatok
        var pets = await _db.GetPetsAsync();
        TotalPets = pets.Count;

        // Rendelők
        var clinics = await _db.GetClinicsAsync();
        TotalClinics = clinics.Count;

        // Vizitek + közelgő kontroll
        var allVisits = await _db.GetAllVisitsAsync();
        TotalVisits = allVisits.Count;

        var nextVisit = await _db.GetNextUpcomingVisitAsync();

        if (nextVisit != null && nextVisit.NextCheck != null)
        {
            var pet = await _db.GetPetAsync(nextVisit.PetId);

            UpcomingVisitText =
                $"{nextVisit.NextCheck:yyyy.MM.dd} – " +
                $"{pet?.Name ?? "Ismeretlen állat"} – " +
                $"{nextVisit.Reason}";

            HasUpcomingVisit = true;
        }
        else
        {
            HasUpcomingVisit = false;
            UpcomingVisitText = "Nincs közelgő kontroll.";
        }
    }
}
