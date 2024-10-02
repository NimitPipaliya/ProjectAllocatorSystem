using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ProjectAllocatorSystemMVC.ViewModels
{
    public class ManagerAllocationViewModel
    {
        [Required(ErrorMessage = "Employee ID is required.")]
        public int EmployeeId { get; set; }
        [Required(ErrorMessage = "Type ID is required.")]
        public int TypeId { get; set; }
        [Required(ErrorMessage = "Start Date is required.")]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }
        [Required(ErrorMessage = "Details are required.")]
        public string Details { get; set; }
        public int? TrainingId { get; set; }
        public int? InternalProjectId { get; set; }

        public int type { get; set; }
        public List<SelectListItem> Types { get; set; }
        public List<SelectListItem> TrainingOptions { get; set; }
        public ManagerAllocationViewModel()
        {
            Types = new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "Training" },
            new SelectListItem { Value = "2", Text = "Internal Project" }
        };

            TrainingOptions = new List<SelectListItem>();
        }
    }
}
