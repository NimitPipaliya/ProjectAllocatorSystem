using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace ProjectAllocatorSystemAPI.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
        [Required]
        public string EmployeeName { get; set; }
        [Required]
        public string EmailId { get; set; }
        [Required]
        public DateTime BenchStartDate { get; set; }
        public DateTime? BenchEndDate { get; set; }
        [Required]
        public int JobRoleId { get; set; }
        public JobRole JobRole { get; set; }
       // public int? SId { get; set; }
        public Skill? Skill { get; set; }
        public ICollection<Allocation> Allocations { get; set; }
        public int TypeId { get; set; }
        public AllocationType Allocationtype { get; set; }
        public ICollection<EmployeeSkills> EmployeeSkills {  get; set; }


    }
}
