using SQLite;
using test.Models;

namespace test.Data
{
    public class AppDatabase
    {
        private readonly SQLiteAsyncConnection _connection;
        private bool _initialized = false;

        public AppDatabase(string dbPath)
        {
            _connection = new SQLiteAsyncConnection(dbPath);
        }

        private async Task EnsureInitializedAsync()
        {
            if (_initialized) return;

            await _connection.CreateTableAsync<Pet>();
            await _connection.CreateTableAsync<VetVisit>();
            await _connection.CreateTableAsync<VetClinic>();

            _initialized = true;
        }

        // ---- Pet CRUD ----
        public async Task<List<Pet>> GetPetsAsync()
        {
            await EnsureInitializedAsync();
            return await _connection.Table<Pet>()
                                    .OrderBy(p => p.Name)
                                    .ToListAsync();
        }

        public async Task<Pet?> GetPetAsync(int id)
        {
            await EnsureInitializedAsync();
            return await _connection.FindAsync<Pet>(id);
        }

        public async Task<int> SavePetAsync(Pet pet)
        {
            await EnsureInitializedAsync();
            if (pet.Id == 0)
                return await _connection.InsertAsync(pet);
            else
                return await _connection.UpdateAsync(pet);
        }

        public async Task<int> DeletePetAsync(Pet pet)
        {
            await EnsureInitializedAsync();
            return await _connection.DeleteAsync(pet);
        }

        // ---- VetVisit CRUD ----
        public async Task<List<VetVisit>> GetVisitsForPetAsync(int petId)
        {
            await EnsureInitializedAsync();
            return await _connection.Table<VetVisit>()
                                    .Where(v => v.PetId == petId)
                                    .OrderByDescending(v => v.VisitDate)
                                    .ToListAsync();
        }

        public async Task<int> SaveVisitAsync(VetVisit visit)
        {
            await EnsureInitializedAsync();
            if (visit.Id == 0)
                return await _connection.InsertAsync(visit);
            else
                return await _connection.UpdateAsync(visit);
        }

        public async Task<int> DeleteVisitAsync(VetVisit visit)
        {
            await EnsureInitializedAsync();
            return await _connection.DeleteAsync(visit);
        }

        // ---- VetClinic CRUD ----
        public async Task<List<VetClinic>> GetClinicsAsync()
        {
            await EnsureInitializedAsync();
            return await _connection.Table<VetClinic>()
                                    .OrderBy(c => c.Name)
                                    .ToListAsync();
        }

        public async Task<int> SaveClinicAsync(VetClinic clinic)
        {
            await EnsureInitializedAsync();
            if (clinic.Id == 0)
                return await _connection.InsertAsync(clinic);
            else
                return await _connection.UpdateAsync(clinic);
        }

        public async Task<int> DeleteClinicAsync(VetClinic clinic)
        {
            await EnsureInitializedAsync();
            return await _connection.DeleteAsync(clinic);
        }

        // ---- Statisztikához segéd metódusok ----

        public async Task<List<VetVisit>> GetAllVisitsAsync()
        {
            await EnsureInitializedAsync();
            return await _connection.Table<VetVisit>()
                                    .ToListAsync();
        }

        public async Task<VetVisit?> GetNextUpcomingVisitAsync()
        {
            await EnsureInitializedAsync();

            var today = DateTime.Today;

            return await _connection.Table<VetVisit>()
                                    .Where(v => v.NextCheck != null && v.NextCheck > today)
                                    .OrderBy(v => v.NextCheck)
                                    .FirstOrDefaultAsync();
        }

    }
}
