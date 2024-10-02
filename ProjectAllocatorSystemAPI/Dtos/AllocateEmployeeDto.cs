using System.ComponentModel.DataAnnotations;

namespace ProjectAllocatorSystemAPI.Dtos
{
    public class AllocateEmployeeDto
    {
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public int TypeId { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Details { get; set; }
    }
}
