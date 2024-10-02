using ProjectAllocatorSystemAPI.Dtos;

namespace ProjectAllocatorSystemAPI.Service.Contract
{
    public interface IAdminService
    {
        public ServiceResponse<IEnumerable<EmployeeDto>> GetAllEmployees();
        public ServiceResponse<IEnumerable<EmployeeDto>> GetPaginatedEmployees(int page, int pageSize, string? search, string sortOrder, string? sortBy);
        public ServiceResponse<EmployeeDto> GetEmployeeById(int id);
        public ServiceResponse<int> TotalEmployees(string? search);
        //ServiceResponse<IEnumerable<SkillDto>> GetEmployeeSkills(string? search);
        ServiceResponse<string> AddEmployee(AddEmployeeDto employeeDto);
        ServiceResponse<string> ModifyEmployee(UpdateEmployeeDto employeeDto);
        ServiceResponse<string> RemoveEmployee(int id);

        public ServiceResponse<IEnumerable<EmployeeDto>> GetEmployeesByDateRangeAndType(DateTime startDate, DateTime? endDate, int typeId);

        public ServiceResponse<IEnumerable<EmployeeDto>> GetEmployeesByJobRoleAndType(int jobRoleId, int typeId);

        ServiceResponse<IEnumerable<JobRoleDto>> GetAllJobRoles();
        //ServiceResponse<IEnumerable<SkillDto>> GetEmployeeSkills(string? search);
        ServiceResponse<string> UpdateEmployee(UpdateAllocationDto updateEmployeeDto);

        ServiceResponse<IEnumerable<SPDto>> GetEmployeeData(string startDate, string? enddate);

    }
}
