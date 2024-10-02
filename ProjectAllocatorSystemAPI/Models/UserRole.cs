using System.ComponentModel.DataAnnotations;

namespace ProjectAllocatorSystemAPI.Models
{
    public class UserRole
    {
        [Key]
        public int UserRoleId { get; set; }
        public string UserRoleName { get; set; }
    }
}
