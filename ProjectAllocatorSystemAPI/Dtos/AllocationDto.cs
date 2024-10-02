using ProjectAllocatorSystemAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace ProjectAllocatorSystemAPI.Dtos
{
    public class AllocationDto
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
        public EmployeeDto? Employee { get; set; }
        public AllocationTypeDto? AllocationType { get; set; }
        public TrainingDto? Training { get; set; }
        public InternalProjectDto? InternalProject { get; set; }
    }
}
