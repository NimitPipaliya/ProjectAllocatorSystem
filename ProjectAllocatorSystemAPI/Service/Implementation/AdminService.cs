using Microsoft.IdentityModel.Tokens;
using ProjectAllocatorSystemAPI.Data;
using ProjectAllocatorSystemAPI.Data.Contract;
using ProjectAllocatorSystemAPI.Data.Implementation;
using ProjectAllocatorSystemAPI.Dtos;
using ProjectAllocatorSystemAPI.Models;
using ProjectAllocatorSystemAPI.Service.Contract;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace ProjectAllocatorSystemAPI.Service.Implementation
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IAllocatorRepository _allocatorRepository;
        public AdminService(IAdminRepository adminRepository,IAllocatorRepository allocatorRepository)
        {
            _adminRepository = adminRepository;
            _allocatorRepository = allocatorRepository;
        }
        public ServiceResponse<IEnumerable<EmployeeDto>> GetAllEmployees()
        {
            var response = new ServiceResponse<IEnumerable<EmployeeDto>>();
            try
            {
                var employees = _adminRepository.GetAllEmployeees();
                if (employees != null && employees.Any())
                {
                    List<EmployeeDto> employeeDtos = new List<EmployeeDto>();
                    foreach (var employee in employees)
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
                            TypeId = employee.TypeId,
                            allocation = new AllocationType
                            {
                                TypeId = employee.TypeId,
                                Type = employee.Allocationtype.Type
                            }
                        });

                    }
                    response.Success = true;
                    response.Data = employeeDtos;
                }
                else
                {
                    response.Success = false;
                    response.Message = "No record found!";
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
            return response;
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
                            TypeId = employee.TypeId,
                            allocation = new AllocationType
                            {
                                TypeId = employee.TypeId,
                                Type = employee.Allocationtype.Type
                            }

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
                        TypeId = existingEmployee.TypeId,
                        allocation = new AllocationType
                        {
                            TypeId = existingEmployee.TypeId,
                            Type = existingEmployee.Allocationtype.Type
                        },

                        Skills = existingEmployee.EmployeeSkills.Select(s => s.Skill.SkillName).ToList()

                    };
                    response.Success = true;
                    response.Data = employee;
                    response.Data.TypeId = employee.TypeId;
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
        public ServiceResponse<int> TotalEmployees(string? search)
        {
            var response = new ServiceResponse<int>();
            try
            {
                int totalPositions = _adminRepository.TotalEmployees(search);

                response.Success = true;
                response.Data = totalPositions;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
            return response;
        }
        public ServiceResponse<string> AddEmployee(AddEmployeeDto employeeDto)
        {
            var response = new ServiceResponse<string>();
            try
            {
                if (_adminRepository.EmployeeNameExists(employeeDto.EmployeeName))
                {
                    response.Success = false;
                    response.Message = "Employee name already exists";
                    return response;
                }
                if (_adminRepository.EmployeeEmailExists(employeeDto.EmailId))
                {
                    response.Success = false;
                    response.Message = "Email address already exists";
                    return response;
                }
                if (employeeDto.BenchStartDate < DateTime.Now.Date)
                {
                    response.Success = false;
                    response.Message = "Bench start date cannot be past date.";
                    return response;
                }
                if (employeeDto.BenchEndDate < employeeDto.BenchStartDate)
                {
                    response.Success = false;
                    response.Message = "Bench end date cannot be less that bench start date.";
                    return response;
                }
                var employee = new Employee()

                {
                    EmployeeName = employeeDto.EmployeeName,
                    EmailId = employeeDto.EmailId,
                    BenchStartDate = employeeDto.BenchStartDate,
                    BenchEndDate = employeeDto.BenchEndDate,
                    JobRoleId = employeeDto.JobRoleId,
                    TypeId = 1,
                };

                _adminRepository.AddSkills(employee, employeeDto.Skills);

                _adminRepository.Add(employee);

                Allocation allocation = new Allocation
                {
                    EmployeeId = employee.EmployeeId,
                    StartDate = employee.BenchStartDate,
                    EndDate = employee.BenchEndDate,
                    Details = "new employee",
                    TypeId = 1,
                    TrainingId = 1,
                    InternalProjectId = 1


                };

                var result = _allocatorRepository.InsertAllocation(allocation);


                if (result)
                {
                    response.Success = true;
                    response.Message = "Employee Added Successfully";
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
        public ServiceResponse<string> ModifyEmployee(UpdateEmployeeDto employeeDto)
        {
            var response = new ServiceResponse<string>();
            try
            {
                var message = string.Empty;
                if (_adminRepository.EmployeeNameExists(employeeDto.EmployeeId, employeeDto.EmployeeName))
                {
                    response.Success = false;
                    response.Message = "Employee name already exists.";
                    return response;

                }
                if (_adminRepository.EmployeeEmailExists(employeeDto.EmployeeId, employeeDto.EmailId))
                {
                    response.Success = false;
                    response.Message = "Email address already exists.";
                    return response;

                }
                if (employeeDto.BenchEndDate < employeeDto.BenchStartDate)
                {
                    response.Success = false;
                    response.Message = "Bench end date cannot be less that bench start date.";
                    return response;
                }
                var existingEmployee = _adminRepository.GetEmployeeById(employeeDto.EmployeeId);
                var result = false;
                if (existingEmployee != null)
                {
                    existingEmployee.EmployeeName = employeeDto.EmployeeName;
                    existingEmployee.EmailId = employeeDto.EmailId;
                    existingEmployee.BenchStartDate = employeeDto.BenchStartDate;
                    existingEmployee.BenchEndDate = employeeDto.BenchEndDate;
                    existingEmployee.JobRoleId = employeeDto.JobRoleId;
                    existingEmployee.TypeId = employeeDto.TypeId;

                    _adminRepository.UpdateSkills(existingEmployee, employeeDto.Skills);
                    result = _adminRepository.Update(existingEmployee);
                }
                if (result)
                {
                    response.Success = true;
                    response.Message = "Employee updated successfully.";
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
        public ServiceResponse<string> RemoveEmployee(int id)
        {
            var response = new ServiceResponse<string>();
            try
            {
                var result = _adminRepository.Delete(id);

                if (result)
                {
                    response.Success = true;
                    response.Message = "Employee deleted successfully";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Something went wrong";
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }

            return response;
        }

        public ServiceResponse<string> UpdateEmployee(UpdateAllocationDto updateEmployeeDto)
        {
            var response = new ServiceResponse<string>();
            try
            {

                if (updateEmployeeDto == null)
                {
                    response.Success = false;
                    response.Message = "Something went wrong. Please try after sometime.";
                    return response;
                }
                var employee = _adminRepository.GetEmployeeById(updateEmployeeDto.EmployeeId);

                if (employee != null)
                {
                    if (updateEmployeeDto.TypeId == 1)
                    {
                        employee.TypeId = updateEmployeeDto.TypeId;
                        employee.BenchStartDate = updateEmployeeDto.StartDate;
                        employee.BenchEndDate = updateEmployeeDto.EndDate;
                    }
                    if (updateEmployeeDto.TypeId == 2)
                    {
                        employee.TypeId = updateEmployeeDto.TypeId;
                        employee.BenchEndDate = (DateTime)updateEmployeeDto.StartDate;
                    }


                    var result = _adminRepository.UpdateEmployee(employee);
                    response.Success = result;
                    response.Message = result ? "Employee updated successfully." : "Something went wrong. Please try after sometime.";
                    return response;
                }
                response.Success = false;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
            return response;
        }

        public ServiceResponse<IEnumerable<EmployeeDto>> GetEmployeesByDateRangeAndType(DateTime startDate, DateTime? endDate, int typeId)
        {
            var response = new ServiceResponse<IEnumerable<EmployeeDto>>();

            try
            {
                var employees = _adminRepository.GetEmployeesByDateRangeAndType(startDate, endDate, typeId);

                if (employees != null && employees.Any())
                {
                    List<EmployeeDto> employeeDtos = new List<EmployeeDto>();

                    foreach (var employee in employees)
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
                            AllocationType = new AllocationTypeDto
                            {
                                Type = employee.Allocationtype.Type,
                            },


                            BenchStartDate = employee.BenchStartDate,
                            BenchEndDate = employee.BenchEndDate,
                            TypeId = employee.TypeId
                        });
                    }

                    response.Success = true;
                    response.Data = employeeDtos;
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

        public ServiceResponse<IEnumerable<EmployeeDto>> GetEmployeesByJobRoleAndType(int jobRoleId, int typeId)
        {
            var response = new ServiceResponse<IEnumerable<EmployeeDto>>();

            try
            {
                var employees = _adminRepository.GetEmployeesByJobRoleAndType(jobRoleId, typeId);

                if (employees != null && employees.Any())
                {
                    List<EmployeeDto> employeeDtos = new List<EmployeeDto>();

                    foreach (var employee in employees)
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
                            TypeId = employee.TypeId,
                           
                           

                        });
                    }

                    response.Success = true;
                    response.Data = employeeDtos;
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
        
        public ServiceResponse<IEnumerable<JobRoleDto>> GetAllJobRoles()
        {
            var response = new ServiceResponse<IEnumerable<JobRoleDto>>();
            try
            {
                var employees = _adminRepository.GetAllJobroles();
                if (employees != null && employees.Any())
                {
                    List<JobRoleDto> employeeDtos = new List<JobRoleDto>();
                    foreach (var employee in employees)
                    {
                        employeeDtos.Add(new JobRoleDto()
                        {
                            JobRoleId = employee.JobRoleId,
                            JobRoleName = employee.JobRoleName,

                        });
                    }
                    response.Success = true;
                    response.Data = employeeDtos;
                }
                else
                {
                    response.Success = false;
                    response.Message = "No record found!";
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
                // Log the exception if needed
            }
            return response;
        }

        public ServiceResponse<IEnumerable<SPDto>> GetEmployeeData(string startDate, string? enddate)
        {
            var response = new ServiceResponse<IEnumerable<SPDto>>();
            try
            {
                var contacts = _adminRepository.GetEmployeeData(startDate, enddate);

                if (contacts != null && contacts.Any())
                {
                    response.Data = contacts;
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    response.Message = "No record found";
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
