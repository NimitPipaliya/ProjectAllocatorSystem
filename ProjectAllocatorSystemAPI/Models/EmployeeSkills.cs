namespace ProjectAllocatorSystemAPI.Models
{
    public class EmployeeSkills
    {
        public int EmpId { get; set; }
        public Employee Employees { get; set; }

        public int SId { get; set; }
        public Skill Skill { get; set; }
    }
}
