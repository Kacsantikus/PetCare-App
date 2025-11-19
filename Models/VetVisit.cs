using SQLite;

namespace test.Models
{
    public class VetVisit
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int PetId { get; set; }

        public DateTime VisitDate { get; set; }

        public string Reason { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Medication { get; set; }

        public DateTime? NextCheck { get; set; }
    }
}
