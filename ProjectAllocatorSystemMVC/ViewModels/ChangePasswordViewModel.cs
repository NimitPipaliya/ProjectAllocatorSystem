using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ProjectAllocatorSystemMVC.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Login id is required")]
        [StringLength(15)]
        [DisplayName("Login id")]
        public string LoginId { get; set; }
        [Required(ErrorMessage = "Old password is required")]
        [DataType(DataType.Password)]
        [DisplayName("Old password")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        ErrorMessage = "The password must be at least 8 characters long and contain at least 1 uppercase letter, 1 number, and 1 special character.")]
        [DisplayName("New password")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm new password is required")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        [DisplayName("Confirm new password")]
        public string ConfirmNewPassword { get; set; }
    }
}
