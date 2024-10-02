using ProjectAllocatorSystemAPI.Dtos;

namespace ProjectAllocatorSystemAPI.Service.Contract
{
    public interface IManagerService
    {
        ServiceResponse<IEnumerable<EmployeeDto>> GetPaginatedEmployees(int page, int pageSize, string? search, string sortOrder, string? sortBy);
        ServiceResponse<int> TotalEmployees(string? search);

        ServiceResponse<AllocationDto> GetAllocationByEmpId(int id);
    }
}
