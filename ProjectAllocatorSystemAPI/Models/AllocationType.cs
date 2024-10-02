using System.ComponentModel.DataAnnotations;

namespace ProjectAllocatorSystemAPI.Models
{
    public class AllocationType
    {
        [Key]
        public int TypeId { get; set; }
        [Required]
        public string Type { get; set; }
        public ICollection<Allocation> Allocations { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}
