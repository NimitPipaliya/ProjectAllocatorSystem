using System.ComponentModel.DataAnnotations;

namespace ProjectAllocatorSystemMVC.ViewModels
{
    public class AddAllocationViewModel
    {
        [Required]
        public int EmployeeId { get; set; }
        [Required(ErrorMessage = "Start date is required")]
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; } = null;

        [Required(ErrorMessage ="Details are required")]
        [MinLength(10, ErrorMessage = "Details must be 10 characters long")]
        public string Details { get; set; }
        public int? TrainingId { get; set; } = 1;
        public int? InternalProjectId { get; set; } = 1;

        [Required]
        public int TypeId { get; set; }
    }
}
