using System.ComponentModel.DataAnnotations;

namespace SimpleCMSWeb.Models.ModelView
{
    public class ForgotPasswordVM
    {
        public required string Username { get; set; }

        //[Required(ErrorMessage = "New password is required.")]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        //[Required(ErrorMessage = "Confirm password is required.")]
        [DataType(DataType.Password)]
        //[Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }

        public bool IsUsernameValid { get; set; } = false;
    }
}
