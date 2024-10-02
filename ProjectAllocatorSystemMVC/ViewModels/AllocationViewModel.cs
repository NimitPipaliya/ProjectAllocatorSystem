using System.ComponentModel.DataAnnotations;

namespace ProjectAllocatorSystemMVC.ViewModels
{
    public class AllocationViewModel
    {
        [Required]
        public int AllocationId { get; set; }
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Required]
        public string Details { get; set; }
        public int? TrainingId { get; set; }
        public int? InternalProjectId { get; set; }
        [Required]
        public int TypeId { get; set; }
        public EmployeeViewModel? Employee { get; set; }
        public AllocationTypeViewModel? AllocationType { get; set; }
        public TrainingViewModel? Training { get; set; }
        public InternalProjectViewModel? InternalProject { get; set; }
        //public TrainingViewModel? Training { get; set; }
        //public InternalProjectDto? InternalProject { get; set; }
    }
}
