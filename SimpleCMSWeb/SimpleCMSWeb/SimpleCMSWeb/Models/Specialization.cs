using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleCMSWeb.Models
{
    public class Specialization
    {
        [Column("specialization_id")]
        public long SpecializationId { get; set; }

        public string? Code { get; set; }

        public string? Name { get; set; }

        public bool? IsMedical { get; set; }

        public bool? IsMd { get; set; }

        public string? Descriptions { get; set; }

        public ICollection<Employee>? Employees { get; set; }
    }
}
