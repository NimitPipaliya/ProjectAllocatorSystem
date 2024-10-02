using ProjectAllocatorSystemAPI.Dtos;
using ProjectAllocatorSystemAPI.Models;

namespace ProjectAllocatorSystemAPI.Data.Contract
{
    public interface IAdminRepository
    {
        IEnumerable<Employee> GetAllEmployeees();
        Employee? GetEmployeeById(int id);
        IEnumerable<Employee> GetPaginatedEmployees(int page, int pageSize, string? search, string sortOrder, string? sortBy);
        int TotalEmployees(string? search);
        //IEnumerable<Skill> GetAllEmployeeSkills(string? search);
        public bool Add(Employee employee);
        public bool Update(Employee employee);
        public bool Delete(int id);
        public bool EmployeeNameExists(string name);
        public bool EmployeeEmailExists(string emailId);
        public bool EmployeeNameExists(int id, string name);
        public bool EmployeeEmailExists(int id, string emailId);
        //IEnumerable<Skill> GetAllEmployeeSkills(string? search);
        void AddSkills(Employee employee, List<string> skillName);
        void UpdateSkills(Employee employee, List<string> skillNames);

        public IEnumerable<Employee> GetEmployeesByDateRangeAndType(DateTime startDate, DateTime? endDate, int typeId);

        public IEnumerable<Employee> GetEmployeesByJobRoleAndType(int jobRoleId, int typeId);

        IEnumerable<JobRole> GetAllJobroles();
        bool UpdateEmployee(Employee employee);

        IEnumerable<SPDto> GetEmployeeData(string startDate, string? enddate);
    }
}
