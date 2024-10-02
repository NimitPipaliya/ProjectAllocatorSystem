namespace ProjectAllocatorSystemAPI.Models
{
    public class JobRole
    {
        public int JobRoleId { get; set; }
        public string JobRoleName { get; set; } 
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
