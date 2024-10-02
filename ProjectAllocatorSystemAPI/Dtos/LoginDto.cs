using System.ComponentModel.DataAnnotations;

namespace ProjectAllocatorSystemAPI.Dtos
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Username is requried")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
