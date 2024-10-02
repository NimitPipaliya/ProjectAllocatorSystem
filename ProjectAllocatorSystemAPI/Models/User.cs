using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProjectAllocatorSystemAPI.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(15)]
        public string LoginId { get; set; }
        [Required]
        [StringLength(1)]
        public string Gender { get; set; }
        [Required]
        [StringLength(50)]
        public string Email { get; set; }
        [Required]
        [StringLength(12)]
        public string Phone { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        [Required]
        public int SecurityQuestionId { get; set; }
        [Required]
        [StringLength(15)]
        public string Answer { get; set; }

        public int UserRoleId { get; set; }
        public SecurityQuestion SecurityQuestion { get; set; }
        public UserRole UserRole { get; set; }
    }
}
