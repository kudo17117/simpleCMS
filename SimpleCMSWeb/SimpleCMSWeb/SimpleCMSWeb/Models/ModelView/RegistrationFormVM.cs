using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SimpleCMSWeb.Models.ModelView
{
    public class RegistrationFormVM
    {
        // Patient fields
        [Required]
        [RegularExpression(@"^[a-zA-Z\s\.\'\-]+$", ErrorMessage = "Only letters, spaces, apostrophes ('), periods (.), and hyphens (-) are allowed.")]
        public string? Lastname { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z\s\.\'\-]+$", ErrorMessage = "Only letters, spaces, apostrophes ('), periods (.), and hyphens (-) are allowed.")]
        public string? Firstname { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z\s\.\'\-]+$", ErrorMessage = "Only letters, spaces, apostrophes ('), periods (.), and hyphens (-) are allowed.")]
        public string? Middlename { get; set; }

        [RegularExpression(@"^[a-zA-Z\s\.\'\-]*$", ErrorMessage = "Only letters, spaces, apostrophes ('), periods (.), and hyphens (-) are allowed.")]
        public string? Suffixname { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime? Birthdate { get; set; }

        [Required] public string? Gender { get; set; }
        [Required] public string? Civilstatus { get; set; }

        // Contact
        public string? Emailaddress { get; set; }

        [Required]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Mobile number must be 11 digits.")]
        public string? Mobileno { get; set; }

        public string? Homeaddress { get; set; }
        public string? Housenostreet { get; set; }
        public string? Barangayid { get; set; }

        [Required]
        public int? Municipalityid { get; set; }

        // Medical
        public string? Bloodtype { get; set; }
        public string? Allergies { get; set; }
        public string? Familymedicalhistory { get; set; }
        public string? Socialhistory { get; set; }

        // Others
        public string? Philhealthno { get; set; }
        public bool? Isphilhealthmember { get; set; }
        public string? Occupation { get; set; }
        public string? Company { get; set; }

        // Account
        [Required]
        [Remote(action: "IsUsernameAvailable", controller: "PatientAccount", ErrorMessage = "Username is already taken.")]
        public string? Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 16 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Password must contain only letters and numbers (no special characters).")]
        public string? Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }

        // Dropdown list
        public List<Municipality> Municipalities { get; set; } = new();
    }
}
