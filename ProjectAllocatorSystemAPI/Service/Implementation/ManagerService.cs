using ProjectAllocatorSystemAPI.Data.Contract;
using ProjectAllocatorSystemAPI.Data.Implementation;
using ProjectAllocatorSystemAPI.Dtos;
using ProjectAllocatorSystemAPI.Service.Contract;

namespace ProjectAllocatorSystemAPI.Service.Implementation
{
    public class ManagerService : IManagerService
    {
        private readonly IManagerRepository _managerRepository;
        public ManagerService(IManagerRepository managerRepository)
        {
            _managerRepository = managerRepository;
        }

        public ServiceResponse<IEnumerable<EmployeeDto>> GetPaginatedEmployees(int page, int pageSize, string? search, string sortOrder, string? sortBy)
        {
            var response = new ServiceResponse<IEnumerable<EmployeeDto>>();
            try
            {
                var employees = _managerRepository.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy);

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
        public ServiceResponse<int> TotalEmployees(string? search)
        {
            var response = new ServiceResponse<int>();
            try
            {
                int totalPositions = _managerRepository.TotalEmployees(search);

                response.Success = true;
                response.Data = totalPositions;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
            return response;
        }

        public ServiceResponse<AllocationDto> GetAllocationByEmpId(int id)
        {
            var response = new ServiceResponse<AllocationDto>();
            try
            {
                var existingEmployee = _managerRepository.GetAllocationByEmployeeById(id);
                if (existingEmployee != null)
                {
                    var employee = new AllocationDto()
                    {

                        AllocationId = existingEmployee.AllocationId,
                        EmployeeId = existingEmployee.EmployeeId,
                        StartDate = existingEmployee.StartDate,
                        EndDate = existingEmployee.EndDate,
                        Details = existingEmployee.Details,
                        TrainingId = existingEmployee.TrainingId,
                        Training = new TrainingDto
                        {
                            TrainingId = existingEmployee.TrainingId,
                            Name = existingEmployee.Training.Name,
                            Description = existingEmployee.Training.Description,
                        },
                        InternalProjectId = existingEmployee.InternalProjectId,
                        InternalProject = new InternalProjectDto
                        {
                            InternalProjectId = existingEmployee.InternalProjectId,
                            Name = existingEmployee.InternalProject.Name,
                            Description = existingEmployee.InternalProject.Description
                        },
                        TypeId = existingEmployee.TypeId,
                        Employee = new EmployeeDto
                        {
                            EmployeeId = existingEmployee.EmployeeId,
                            EmployeeName = existingEmployee.Employee.EmployeeName
                        }


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

       


    }
}
