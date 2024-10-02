using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ProjectAllocatorSystemAPI.Dtos
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Login id is required.")]
        [StringLength(15)]
        [DisplayName("Login id")]
        public string LoginId { get; set; }

        [Required(ErrorMessage = "Password hint is required.")]
        [DisplayName("Security question")]
        public int SecurityQuestionId { get; set; }

        [Required(ErrorMessage = "Answer is required.")]
        [StringLength(50)]
        [DisplayName("Security answer 1")]
        public string Answer { get; set; }

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
