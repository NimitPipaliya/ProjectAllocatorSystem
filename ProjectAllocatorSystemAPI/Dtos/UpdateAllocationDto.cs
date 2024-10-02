namespace ProjectAllocatorSystemAPI.Dtos
{
    public class UpdateAllocationDto
    {
        public int EmployeeId { get; set; }
        public int TypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
