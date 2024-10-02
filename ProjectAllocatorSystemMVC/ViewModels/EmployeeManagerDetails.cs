using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ProjectAllocatorSystemMVC.ViewModels
{
    public class EmployeeManagerDetails
    {
        public int AllocationId { get; set; }

        public int EmployeeId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Details { get; set; }
        public int? TrainingId { get; set; }
        public int? InternalProjectId { get; set; }
        [Required]
        public int TypeId { get; set; }
        public EmployeeManagerViewModel? Employee { get; set; }
        //public AllocationTypeDto? AllocationType { get; set; }
        public TrainingViewModel? Training { get; set; }
        public InternalProjectViewModel? InternalProject { get; set; }
    }
}
