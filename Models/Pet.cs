using SQLite;

namespace test.Models
{
    public class Pet
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; }

        public string? PhotoPath { get; set; }

        public string? Notes { get; set; }
    }
}
