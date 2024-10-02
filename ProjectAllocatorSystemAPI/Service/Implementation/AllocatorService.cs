using ProjectAllocatorSystemAPI.Data.Contract;
using ProjectAllocatorSystemAPI.Data.Implementation;
using ProjectAllocatorSystemAPI.Dtos;
using ProjectAllocatorSystemAPI.Models;
using ProjectAllocatorSystemAPI.Service.Contract;
using System.Diagnostics.CodeAnalysis;

namespace ProjectAllocatorSystemAPI.Service.Implementation
{
    public class AllocatorService:IAllocatorService
    {
        private readonly IAllocatorRepository _adminRepository;
        public AllocatorService(IAllocatorRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }
        public ServiceResponse<IEnumerable<EmployeeDto>> GetPaginatedEmployees(int page, int pageSize, string? search, string sortOrder, string? sortBy)
        {
            var response = new ServiceResponse<IEnumerable<EmployeeDto>>();
            try
            {
                var employees = _adminRepository.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy);

                if (employees != null && employees.Any())
                {
                    List<EmployeeDto> employeeDtos = new List<EmployeeDto>();
                    foreach (var employee in employees.ToList())
                    {
                        employeeDtos.Add(new EmployeeDto()
                        {
                            EmployeeId = employee.EmployeeId,
                            EmployeeName = employee.EmployeeName,
                            EmailId = employee.EmailId,
                            JobRoleId = employee.JobRoleId,
                            JobRole = new JobRoleDto
                            {
                                JobRoleId = employee.JobRoleId,
                                JobRoleName = employee.JobRole.JobRoleName,
                            },
                            BenchStartDate = employee.BenchStartDate,
                            BenchEndDate = employee.BenchEndDate,
                            TypeId = employee.TypeId
                        });
                    }


                    response.Data = employeeDtos;
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    response.Message = "No records found";
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }

            return response;
        }
        public ServiceResponse<EmployeeDto> GetEmployeeById(int id)
        {
            var response = new ServiceResponse<EmployeeDto>();
            try
            {
                var existingEmployee = _adminRepository.GetEmployeeById(id);
                if (existingEmployee != null)
                {
                    var employee = new EmployeeDto()
                    {
                        EmployeeId = existingEmployee.EmployeeId,
                        EmployeeName = existingEmployee.EmployeeName,
                        EmailId = existingEmployee.EmailId,
                        JobRoleId = existingEmployee.JobRoleId,
                        JobRole = new JobRoleDto
                        {
                            JobRoleId = existingEmployee.JobRoleId,
                            JobRoleName = existingEmployee.JobRole.JobRoleName,
                        },
                        BenchStartDate = existingEmployee.BenchStartDate,
                        BenchEndDate = existingEmployee.BenchEndDate,
                        TypeId = existingEmployee.TypeId
                    };
                    response.Success = true;
                    response.Data = employee;
                }

                else
                {
                    response.Success = false;
                    response.Message = "Something went wrong,try after sometime";
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
            return response;
        }
       

        public ServiceResponse<string> AddAllocation(Allocation allocation)
        {
            var response = new ServiceResponse<string>();
            try
            {


                if (allocation.EndDate < allocation.StartDate)
                {
                    response.Success = false;
                    response.Message = "End date cannot be past of start date.";
                    return response;
                }
                var result = _adminRepository.InsertAllocation(allocation);
                if (result && allocation.TypeId == 2)
                {
                    response.Success = true;
                    response.Message = "Employee allocated to project successfully";
                }
                else if (result && allocation.TypeId == 1)
                {
                    response.Success = true;
                    response.Message = "Employee set on bench successfully";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Something went wrong. Please try later";
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
            return response;
        }

    }
}
