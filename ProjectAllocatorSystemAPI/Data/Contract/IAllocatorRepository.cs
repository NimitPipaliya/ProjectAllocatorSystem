using ProjectAllocatorSystemAPI.Models;

namespace ProjectAllocatorSystemAPI.Data.Contract
{
    public interface IAllocatorRepository
    {
        IEnumerable<Employee> GetPaginatedEmployees(int page, int pageSize, string? search, string sortOrder, string? sortBy);
        Employee? GetEmployeeById(int id);
        bool InsertAllocation(Allocation allocation);
    }
}
