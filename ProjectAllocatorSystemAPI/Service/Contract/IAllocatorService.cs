using ProjectAllocatorSystemAPI.Dtos;
using ProjectAllocatorSystemAPI.Models;

namespace ProjectAllocatorSystemAPI.Service.Contract
{
    public interface IAllocatorService
    {
        ServiceResponse<EmployeeDto> GetEmployeeById(int id);
        ServiceResponse<IEnumerable<EmployeeDto>> GetPaginatedEmployees(int page, int pageSize, string? search, string sortOrder, string? sortBy);
        ServiceResponse<string> AddAllocation(Allocation allocation);
    }
}
