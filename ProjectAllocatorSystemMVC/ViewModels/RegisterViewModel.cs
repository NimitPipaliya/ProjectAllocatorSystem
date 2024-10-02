using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ProjectAllocatorSystemMVC.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "LoginId is required.")]
        [StringLength(15)]
        public string LoginId { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [StringLength(50)]
        [EmailAddress]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(12)]
        [DataType(DataType.PhoneNumber)]
        [DisplayName("Phone Number")]

        public string Phone { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        ErrorMessage = "The password must be at least 8 characters long and contain at least 1 uppercase letter, 1 number, and 1 special character.")]
        [DisplayName("Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password doesnot match.")]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please select the security question.")]
        public int SecurityQuestionId { get; set; }

        [Required(ErrorMessage = "Answer is required.")]
        [StringLength(15)]
        public string Answer { get; set; }

        [Required(ErrorMessage = "User role is required.")]
        public int UserRoleId { get; set; }
    }
}
