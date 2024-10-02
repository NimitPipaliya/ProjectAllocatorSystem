using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ProjectAllocatorSystemMVC.ViewModels
{
    public class ManagerListViewModel
    {
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
        [Required]
        [DisplayName("Bench Start Date")]
        public DateTime BenchStartDate { get; set; }
        public DateTime? BenchEndDate { get; set; }
        [Required(ErrorMessage = "Job Role is required.")]
        [DisplayName("Job Role")]
        public int JobRoleId { get; set; }
        public JobRoleViewModel JobRole { get; set; }
        public int TypeId { get; set; }
        //public AllocationType allocation { get; set; }
        public List<string> Skills { get; set; }
        //public AllocationTypeDto AllocationType { get; set; }
        public AllocationViewModelManager allocations { get; set; }
    }
}
