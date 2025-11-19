using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using test.Data;
using test.Models;

namespace test.ViewModel;

public partial class VisitsViewModel : ObservableObject
{
    private readonly AppDatabase _db;

    [ObservableProperty]
    private int petId;

    [ObservableProperty]
    private string petName = string.Empty;

    [ObservableProperty]
    private ObservableCollection<VetVisit> visits = new();

    [ObservableProperty]
    private VetVisit? selectedVisit;

    [ObservableProperty]
    private DateTime visitDate = DateTime.Today;

    [ObservableProperty]
    private string reason = string.Empty;

    [ObservableProperty]
    private string? description;

    [ObservableProperty]
    private string? medication;

    [ObservableProperty]
    private DateTime? nextCheck;

    [ObservableProperty]
    private bool hasNextCheck;


    public VisitsViewModel(AppDatabase db)
    {
        _db = db;
    }

    public void SetPet(int id, string name)
    {
        PetId = id;
        PetName = name;
    }

    public async Task LoadAsync()
    {
        if (PetId <= 0)
            return;

        var list = await _db.GetVisitsForPetAsync(PetId);
        Visits = new ObservableCollection<VetVisit>(list);
    }

    public async Task SaveAsync()
    {
        if (PetId <= 0)
        {
            await Shell.Current.DisplayAlert("Hiba", "Hiányzik az állat azonosítója.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Reason))
        {
            await Shell.Current.DisplayAlert("Hiba", "Az ok megadása kötelező.", "OK");
            return;
        }

        VetVisit visit;

        if (SelectedVisit == null)
        {
            visit = new VetVisit
            {
                PetId = PetId
            };
        }
        else
        {
            visit = SelectedVisit;
        }

        visit.VisitDate = VisitDate;
        visit.Reason = Reason;
        visit.Description = Description;
        visit.Medication = Medication;
        visit.NextCheck = HasNextCheck ? NextCheck : null;


        await _db.SaveVisitAsync(visit);
        await LoadAsync();
        ClearForm();
    }

    public async Task DeleteAsync(VetVisit visit)
    {
        await _db.DeleteVisitAsync(visit);
        await LoadAsync();
    }

    public void Edit(VetVisit visit)
    {
        SelectedVisit = visit;
        VisitDate = visit.VisitDate;
        Reason = visit.Reason;
        Description = visit.Description;
        Medication = visit.Medication;
        NextCheck = visit.NextCheck;
        HasNextCheck = visit.NextCheck != null;

    }

    private void ClearForm()
    {
        SelectedVisit = null;
        VisitDate = DateTime.Today;
        Reason = string.Empty;
        Description = null;
        Medication = null;
        NextCheck = null;
    }
}
