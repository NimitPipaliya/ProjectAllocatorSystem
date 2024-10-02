namespace ProjectAllocatorSystemMVC.ViewModels
{
    public class AllocatorListViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmailId { get; set; }
        public int JobRoleId { get; set; }
        public JobRoleViewModel JobRole {  get; set; }
        public AllocationTypeViewModel AllocationType { get; set; }
        public int TypeId { get; set; }
    }
}
