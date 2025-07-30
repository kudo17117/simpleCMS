using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleCMSWeb.Models
{
    [Table("municipality")]
    public class Municipality
    {
        public int MunicipalityId { get; set; }

        public int? RegionId { get; set; }

        public int? ProvinceId { get; set; }

        public string? MunicipalityName { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsDefault { get; set; }

        public long? CreatedById { get; set; }

        public DateTime? DateCreated { get; set; }

        public long? UpdateById { get; set; }

        public DateTime? DateUpdate { get; set; }

        public virtual ICollection<Clinic>? Clinics { get; set; }
        public virtual Province? Province { get; set; }
    }
}
