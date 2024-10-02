using System.ComponentModel.DataAnnotations;

namespace ProjectAllocatorSystemAPI.Dtos
{
    public class AllocationTypeDto
    {
        [Key]
        public int TypeId { get; set; }
        [Required]
        public string Type { get; set; }
    }
}
