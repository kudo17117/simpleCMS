using System.ComponentModel.DataAnnotations;

namespace SimpleCMSWeb.Models.ModelView
{
    public class ForgotPasswordVM
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^[a-zA-Z0-9]{6,16}$", ErrorMessage = "Password must be 6–16 characters and contain only letters and numbers.")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }

        public bool IsUsernameValid { get; set; } = false;
    }
}
