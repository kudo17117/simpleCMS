using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleCMSWeb.Models.ModelView
{
    public class DoctorRegisterVM
    {
        public string? LastName { get; set; }

        public string? FirstName { get; set; }

        public string? MiddleName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Gender { get; set; }

        public DateTime? BirthDate { get; set; }

        public string? ContactNo { get; set; }

        public int? EmployeeTypeId { get; set; }

        public long? SpecializationId { get; set; }

        public string? LicenceNo { get; set; }

        public string? PTR { get; set; }
        
        public string? Address { get; set; }

        [NotMapped]
        public IFormFile? Photo { get; set; }

        [Required]
        public required string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Userpass { get; set; }

        [NotMapped]
        [Required]
        [DataType(DataType.Password)]
        [Compare("Userpass", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
