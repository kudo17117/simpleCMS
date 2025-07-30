using System.ComponentModel.DataAnnotations;

namespace SimpleCMSWeb.Models
{
    public class Province
    {
        [Key]
        public int ProvinceId { get; set; }

        public int? RegionId { get; set; }

        public string? ProvinceName { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsDefault { get; set; }

        public long? CreatedById { get; set; }

        public DateTime? DateCreated { get; set; }

        public long? UpdateById { get; set; }

        public DateTime? DateUpdate { get; set; }
        public virtual ICollection<Municipality>? Municipalities { get; set; }
        public virtual Region? Region { get; set; }
    }
}
