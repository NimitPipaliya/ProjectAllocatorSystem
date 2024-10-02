using ProjectAllocatorSystemAPI.Models;

namespace ProjectAllocatorSystemAPI.Data.Contract
{
    public interface IManagerRepository
    {
         IEnumerable<Employee> GetAllEmployee();
        public IEnumerable<Employee> GetPaginatedEmployees(int page, int pageSize, string? search, string sortOrder, string? sortBy);
        public int TotalEmployees(string? search);
        Allocation? GetAllocationByEmployeeById(int id);
        public bool UpdateEmployee(Employee employee);
    }
}
