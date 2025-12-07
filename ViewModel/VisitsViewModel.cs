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
            await Shell.Current.DisplayAlert("Hiba", "A vizsgálat oka kötelező.", "OK");
            return;
        }

        if (VisitDate > DateTime.Today.AddDays(1))
        {
            await Shell.Current.DisplayAlert("Hiba", "A vizsgálat dátuma nem lehet túl messze a jövőben.", "OK");
            return;
        }

        if (HasNextCheck) // vagy ahogy a checkboxhoz kötött bool property-t hívod
        {
            if (NextCheck == null)
            {
                await Shell.Current.DisplayAlert("Hiba", "Ha kontrollt jelölsz, a kontroll dátuma is kötelező.", "OK");
                return;
            }

            if (NextCheck <= VisitDate)
            {
                await Shell.Current.DisplayAlert("Hiba", "A kontroll dátuma a vizsgálat után kell legyen.", "OK");
                return;
            }
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
        var confirm = await Shell.Current.DisplayAlert(
            "Törlés",
            $"Biztosan törlöd ezt a vizitet?",
            "Igen",
            "Mégse");

        if (!confirm)
            return;

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
