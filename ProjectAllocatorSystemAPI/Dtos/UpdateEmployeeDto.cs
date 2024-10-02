using ProjectAllocatorSystemAPI.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectAllocatorSystemAPI.Dtos
{
    public class UpdateEmployeeDto
    {
        [Required]
        public int EmployeeId { get; set; }
        [Required(ErrorMessage = "Employee name is required.")]
        [StringLength(50)]
        [DisplayName("Employee Name")]
        public string EmployeeName { get; set; }
        [Required(ErrorMessage = "Email address is required.")]
        [StringLength(50)]
        [EmailAddress]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Invalid email format.")]
        [DisplayName("Email Address")]
        public string EmailId { get; set; }
        [Required(ErrorMessage = "Bench start date is required.")]
        public DateTime BenchStartDate { get; set; }
        public DateTime? BenchEndDate { get; set; }
        [Required(ErrorMessage = "Job Role is required.")]
        [DisplayName("Job Role")]
        public int JobRoleId { get; set; }
        //public JobRole JobRole { get; set; }
        public int TypeId { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime StartDate { get; set; }
        public List<string> Skills { get; set; }
    }
}
