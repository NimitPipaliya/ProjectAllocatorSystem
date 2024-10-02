namespace ProjectAllocatorSystemMVC.ViewModels
{
    public class spViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int typeId { get; set; }

        public DateTime BenchStartDate { get; set; }
        public DateTime? BenchEndDate { get; set; }
        public int? TrainingId { get; set; }
        public string? TrainingName { get; set; }
        public string? TrainingDescription { get; set; }

        public string? ProjectName { get; set; }
        public string? ProjectDescription { get; set; }
    }
}
