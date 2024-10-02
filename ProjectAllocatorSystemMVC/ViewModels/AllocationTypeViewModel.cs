using System.ComponentModel.DataAnnotations;

namespace ProjectAllocatorSystemMVC.ViewModels
{
    public class AllocationTypeViewModel
    {
        [Key]
        public int TypeId { get; set; }
        [Required]
        public string Type { get; set; }
    }
}
