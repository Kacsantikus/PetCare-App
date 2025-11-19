using SQLite;

namespace test.Models
{
    public class VetClinic
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string? Phone { get; set; }
    }
}
