using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ProjectAllocatorSystemAPI.Models
{
    public class Allocation
    {
        [Key]
        public int AllocationId {  get; set; }
        [Required]
        public int EmployeeId {  get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Required]
        public string Details {  get; set; }
        public int? TrainingId { get; set; } 
        public int? InternalProjectId { get; set; } 
        [Required]
        public int TypeId {  get; set; }
        public Employee Employee {  get; set; }
        public AllocationType AllocationType { get; set; }
        public Training Training {  get; set; }
        public InternalProject InternalProject {  get; set; }


    }
}
