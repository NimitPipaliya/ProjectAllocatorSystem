using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProjectAllocatorSystemAPI.Data;
using ProjectAllocatorSystemAPI.Data.Contract;
using ProjectAllocatorSystemAPI.Data.Implementation;
using ProjectAllocatorSystemAPI.Dtos;
using ProjectAllocatorSystemAPI.Models;
using ProjectAllocatorSystemAPI.Service.Implementation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAllocatorAPITests.Service
{

    public class AdminServiceTests {

        [Fact]
        public void GetPaginatedEmployees_ReturnsEmployeeDtos_WhenEmployeesExist()
        {
            var page = 1;
            var pageSize = 10;
            var search = "NonExistentEmployee";
            var sortOrder = "asc";
            var sortBy = "EmployeeName";
            // Arrange
            var mockAdminRepository = new Mock<IAdminRepository>();
            var employees = new List<Employee>
        {
            new Employee
            {
                EmployeeId = 1,
                EmployeeName = "John Doe",
                EmailId = "john.doe@example.com",
                JobRoleId = 1,
                JobRole = new JobRole { JobRoleId = 1, JobRoleName = "Developer" },
                BenchStartDate = DateTime.Now,
                BenchEndDate = null,
                TypeId = 1,
                Allocationtype = new AllocationType { TypeId = 1, Type = "Full-Time" }
            }
        };
            mockAdminRepository.Setup(repo => repo.GetPaginatedEmployees(1, 10, null, "asc", "employeeName"))
                              .Returns(employees);


            mockAdminRepository.Setup(repo => repo.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy)).Returns(employees);
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(mockAdminRepository.Object, _mockAllocatorRepository.Object);

            // Act
            var response = employeeService.GetPaginatedEmployees(1, 10, null, "asc", "employeeName");

            // Assert
            Assert.True(response.Success);
            Assert.NotNull(response.Data);
            Assert.NotEmpty(response.Data);

            var employeeDto = response.Data.First(); // Assuming response.Data is IEnumerable<EmployeeDto>
            Assert.Equal(1, employeeDto.EmployeeId);
            Assert.Equal("John Doe", employeeDto.EmployeeName);
            Assert.Equal("john.doe@example.com", employeeDto.EmailId);
            Assert.Equal(1, employeeDto.JobRoleId);
            Assert.NotNull(employeeDto.JobRole);
            Assert.Equal("Developer", employeeDto.JobRole.JobRoleName);
            Assert.Null(employeeDto.BenchEndDate);
            Assert.Equal(1, employeeDto.TypeId);
        }


        [Fact]
        public void GetPaginatedEmployees_ReturnsNoRecordsFound_WhenNoEmployeesExist()
        {
            // Arrange
            var page = 1;
            var pageSize = 10;
            var search = "NonExistentEmployee";
            var sortOrder = "asc";
            var sortBy = "EmployeeName";

            var emptyEmployees = new List<Employee>(); // Empty list of employees

            var mockAdminRepository = new Mock<IAdminRepository>();
            mockAdminRepository.Setup(repo => repo.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy)).Returns(emptyEmployees);
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(mockAdminRepository.Object, _mockAllocatorRepository.Object);

            // Act
            var result = employeeService.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("No records found", result.Message);
        }
        [Fact]
        public void GetAllEmployees_ReturnsEmployees_WhenEmployeesExist()
        {
            // Arrange
            var employees = new List<Employee>
        {
            new Employee { EmployeeId = 1, EmployeeName = "Alice", EmailId = "alice@example.com", JobRoleId = 1, BenchStartDate = DateTime.Now, BenchEndDate = null, JobRole = new JobRole { JobRoleId = 1, JobRoleName = "Developer" }, TypeId = 1, Allocationtype = new AllocationType { TypeId = 1, Type = "Full Time" } }
            // Add more employees as needed
        };

            var mockAdminRepository = new Mock<IAdminRepository>();
            mockAdminRepository.Setup(repo => repo.GetAllEmployeees()).Returns(employees);
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(mockAdminRepository.Object, _mockAllocatorRepository.Object);

            // Act
            var result = employeeService.GetAllEmployees();

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data);

            var employeeDto = result.Data.First(); // Assuming at least one employee is returned
            Assert.Equal(1, employeeDto.EmployeeId); // Validate EmployeeId mapping
            Assert.Equal("Alice", employeeDto.EmployeeName); // Validate EmployeeName mapping
            Assert.Equal("alice@example.com", employeeDto.EmailId); // Validate EmailId mapping
            Assert.Equal(1, employeeDto.JobRoleId); // Validate JobRoleId mapping
            Assert.Equal("Developer", employeeDto.JobRole.JobRoleName); // Validate JobRoleName mapping
            Assert.Null(employeeDto.BenchEndDate); // Validate BenchEndDate mapping
            Assert.Equal(1, employeeDto.TypeId); // Validate TypeId mapping

        }

        [Fact]
        public void GetAllEmployees_ReturnsNoRecordsFound_WhenNoEmployeesExist()
        {
            // Arrange
            var emptyEmployees = new List<Employee>(); // Empty list of employees

            var mockAdminRepository = new Mock<IAdminRepository>();
            mockAdminRepository.Setup(repo => repo.GetAllEmployeees()).Returns(emptyEmployees);
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(mockAdminRepository.Object, _mockAllocatorRepository.Object);

            // Act
            var result = employeeService.GetAllEmployees();

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("No record found!", result.Message);
        }
        [Fact]
        public void GetEmployeeById_ReturnsEmployeeDto_WhenEmployeeExists()
        {
            // Arrange
            int employeeId = 1;
            var mockAdminRepository = new Mock<IAdminRepository>();
            var existingEmployee = new Employee
            {
                EmployeeId = employeeId,
                EmployeeName = "John Doe",
                EmailId = "john.doe@example.com",
                JobRoleId = 1,
                JobRole = new JobRole { JobRoleId = 1, JobRoleName = "Developer" },
                BenchStartDate = DateTime.Now,
                BenchEndDate = null,
                TypeId = 1,
                Allocationtype = new AllocationType { TypeId = 1, Type = "Full-Time" },
                EmployeeSkills = new List<EmployeeSkills>
            {
                new EmployeeSkills { Skill = new Skill { Id = 1, SkillName = "C#" } },
                new EmployeeSkills { Skill = new Skill { Id = 2, SkillName = "JavaScript" } }
            }
            };
            mockAdminRepository.Setup(repo => repo.GetEmployeeById(employeeId))
                              .Returns(existingEmployee);

            mockAdminRepository.Setup(repo => repo.GetEmployeeById(employeeId)).Returns(existingEmployee);
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(mockAdminRepository.Object, _mockAllocatorRepository.Object);

            // Act
            var response = employeeService.GetEmployeeById(employeeId);

            // Assert
            Assert.True(response.Success);
            Assert.NotNull(response.Data);

            var employeeDto = response.Data;
            Assert.Equal(employeeId, employeeDto.EmployeeId);
            Assert.Equal("John Doe", employeeDto.EmployeeName);
            Assert.Equal("john.doe@example.com", employeeDto.EmailId);
            Assert.Equal(1, employeeDto.JobRoleId);
            Assert.NotNull(employeeDto.JobRole);
            Assert.Equal("Developer", employeeDto.JobRole.JobRoleName);
            Assert.Null(employeeDto.BenchEndDate);
            Assert.Equal(1, employeeDto.TypeId);
            Assert.Contains("C#", employeeDto.Skills);
            Assert.Contains("JavaScript", employeeDto.Skills);
        }

        [Fact]
        public void GetEmployeeById_ReturnsNotFound_WhenEmployeeDoesNotExist()
        {
            // Arrange
            int nonExistingEmployeeId = 999; // Assume this ID does not exist in the repository

            var mockAdminRepository = new Mock<IAdminRepository>();
            mockAdminRepository.Setup(repo => repo.GetEmployeeById(nonExistingEmployeeId)).Returns((Employee)null);
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(mockAdminRepository.Object, _mockAllocatorRepository.Object);

            // Act
            var result = employeeService.GetEmployeeById(nonExistingEmployeeId);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Something went wrong,try after sometime", result.Message);
        }
        [Fact]
        public void TotalEmployees_ReturnsTotalCount_WhenSearchIsNotNullOrEmpty()
        {
            // Arrange
            string searchKeyword = "Alice";
            int expectedTotal = 5; // Sample total count

            var mockAdminRepository = new Mock<IAdminRepository>();
            mockAdminRepository.Setup(repo => repo.TotalEmployees(searchKeyword)).Returns(expectedTotal);
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(mockAdminRepository.Object, _mockAllocatorRepository.Object);

            // Act
            var result = employeeService.TotalEmployees(searchKeyword);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(expectedTotal, result.Data);
        }

        [Fact]
        public void TotalEmployees_ReturnsZero_WhenSearchIsEmptyOrNull()
        {
            // Arrange
            string searchKeyword = null; // or string.Empty
            int expectedTotal = 0; // Expected total count when no search keyword provided

            var mockAdminRepository = new Mock<IAdminRepository>();
            mockAdminRepository.Setup(repo => repo.TotalEmployees(searchKeyword)).Returns(expectedTotal);
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(mockAdminRepository.Object, _mockAllocatorRepository.Object);

            // Act
            var result = employeeService.TotalEmployees(searchKeyword);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(expectedTotal, result.Data);
        }
        [Fact]
        public void AddEmployee_SuccessfullyAddsEmployee_WhenValid()
        {
            // Arrange
            var employeeDto = new AddEmployeeDto
            {
                EmployeeName = "Alice",
                EmailId = "alice@example.com",
                BenchStartDate = DateTime.Now.AddDays(1), // Future date
                BenchEndDate = DateTime.Now.AddDays(10), // Future date after BenchStartDate
                JobRoleId = 1,
                Skills = new List<string> { "Skill1", "Skill2" }
            };
            var allocationDto = new Allocation
            {
                EmployeeId = 1,
                StartDate = DateTime.Parse("2024 - 02 - 02"),
                EndDate = DateTime.Parse("2024-02-02"),
                Details = "",
                TypeId = 1,
                TrainingId = 1,
                InternalProjectId = 1
            };


            var mockAdminRepository = new Mock<IAdminRepository>();
            mockAdminRepository.Setup(repo => repo.EmployeeNameExists(employeeDto.EmployeeName)).Returns(false); // Does not exist
            mockAdminRepository.Setup(repo => repo.EmployeeEmailExists(employeeDto.EmailId)).Returns(false); // Does not exist
            mockAdminRepository.Setup(repo => repo.Add(It.IsAny<Employee>())).Returns(true); // Simulate successful addition
                                                                                             // Optionally mock AddSkills if needed
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            _mockAllocatorRepository.Setup(repo => repo.InsertAllocation(allocationDto)).Returns(true);
            var employeeService = new AdminService(mockAdminRepository.Object, _mockAllocatorRepository.Object);

            // Act
            var result = employeeService.AddEmployee(employeeDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Something went wrong. Please try later", result.Message);

            // Additional assertions if needed
            mockAdminRepository.Verify(repo => repo.EmployeeNameExists(employeeDto.EmployeeName), Times.Once);
            mockAdminRepository.Verify(repo => repo.EmployeeEmailExists(employeeDto.EmailId), Times.Once);
            mockAdminRepository.Verify(repo => repo.Add(It.IsAny<Employee>()), Times.Once);
        }

        [Fact]
        public void AddEmployee_FailsToAddEmployee_WhenNameExists()
        {
            // Arrange
            var employeeDto = new AddEmployeeDto
            {
                EmployeeName = "Alice",
                EmailId = "alice@example.com",
                BenchStartDate = DateTime.Now.AddDays(1), // Future date
                BenchEndDate = DateTime.Now.AddDays(10), // Future date after BenchStartDate
                JobRoleId = 1,
                Skills = new List<string> { "Skill1", "Skill2" }
            };
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var mockAdminRepository = new Mock<IAdminRepository>();
            mockAdminRepository.Setup(repo => repo.EmployeeNameExists(employeeDto.EmployeeName)).Returns(true); // Name already exists

            var employeeService = new AdminService(mockAdminRepository.Object, _mockAllocatorRepository.Object);

            // Act
            var result = employeeService.AddEmployee(employeeDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Employee name already exists", result.Message);
            mockAdminRepository.Verify(repo => repo.EmployeeNameExists(employeeDto.EmployeeName), Times.Once);
            mockAdminRepository.Verify(repo => repo.EmployeeEmailExists(employeeDto.EmailId), Times.Never);
            mockAdminRepository.Verify(repo => repo.Add(It.IsAny<Employee>()), Times.Never);
        }
        [Fact]
        public void AddEmployee_ReturnsSuccess_WhenValidEmployeeAdded()
        {
            // Arrange
            var employeeDto = new AddEmployeeDto
            {
                EmployeeName = "John Doe",
                EmailId = "john.doe@example.com",
                BenchStartDate = DateTime.Now.AddDays(1),
                BenchEndDate = DateTime.Now.AddDays(10),
                JobRoleId = 101
            };
            var allocationDto = new Allocation
            {
                EmployeeId = 1,
                StartDate = DateTime.Parse("2024 - 02 - 02"),
                EndDate = DateTime.Parse("2024-02-02"),
                Details = "",
                TypeId = 1,
                TrainingId = 1,
                InternalProjectId = 1
            };
            var adminRepositoryMock = new Mock<IAdminRepository>();
            adminRepositoryMock.Setup(repo => repo.EmployeeNameExists(employeeDto.EmployeeName)).Returns(false);
            adminRepositoryMock.Setup(repo => repo.EmployeeEmailExists(employeeDto.EmailId)).Returns(false);
            adminRepositoryMock.Setup(repo => repo.Add(It.IsAny<Employee>())).Returns(true);
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            _mockAllocatorRepository.Setup(repo => repo.InsertAllocation(It.IsAny<Allocation>())).Returns(true);
            var employeeService = new AdminService(adminRepositoryMock.Object, _mockAllocatorRepository.Object);

            // Act
            var result = employeeService.AddEmployee(employeeDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Employee Added Successfully", result.Message);
            adminRepositoryMock.Verify(repo => repo.EmployeeNameExists(employeeDto.EmployeeName),Times.Once);
            adminRepositoryMock.Verify(repo => repo.EmployeeEmailExists(employeeDto.EmailId), Times.Once);
            adminRepositoryMock.Verify(repo => repo.Add(It.IsAny<Employee>()), Times.Once);
            _mockAllocatorRepository.Verify(repo => repo.InsertAllocation(It.IsAny<Allocation>()),Times.Once);
        }

        [Fact]
        public void AddEmployee_ReturnsError_WhenEmployeeNameExists()
        {
            // Arrange
            var existingEmployeeName = "John Doe";
            var employeeDto = new AddEmployeeDto { EmployeeName = existingEmployeeName };

            var adminRepositoryMock = new Mock<IAdminRepository>();
            adminRepositoryMock.Setup(repo => repo.EmployeeNameExists(existingEmployeeName)).Returns(true);
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(adminRepositoryMock.Object, _mockAllocatorRepository.Object);

            // Act
            var result = employeeService.AddEmployee(employeeDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Employee name already exists", result.Message);
        }
        [Fact]
        public void AddEmployee_ReturnsError_WhenEmailExists()
        {
            // Arrange
            var existingEmail = "john.doe@example.com";
            var employeeDto = new AddEmployeeDto { EmailId = existingEmail };

            var adminRepositoryMock = new Mock<IAdminRepository>();
            adminRepositoryMock.Setup(repo => repo.EmployeeEmailExists(existingEmail)).Returns(true);
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(adminRepositoryMock.Object, _mockAllocatorRepository.Object);

            // Act
            var result = employeeService.AddEmployee(employeeDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Email address already exists", result.Message);
        }
        [Fact]
        public void AddEmployee_ReturnsError_WhenBenchStartDateIsPast()
        {
            // Arrange
            var pastBenchStartDate = DateTime.Now.AddDays(-1); // A past date
            var employeeDto = new AddEmployeeDto { BenchStartDate = pastBenchStartDate };

            var adminRepositoryMock = new Mock<IAdminRepository>();
            // No need to set up any other methods for this specific scenario
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(adminRepositoryMock.Object, _mockAllocatorRepository.Object);

            // Act
            var result = employeeService.AddEmployee(employeeDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Bench start date cannot be past date.", result.Message);
        }
        [Fact]
        public void AddEmployee_ReturnsError_WhenBenchEndDateIsLessThanBenchStartDate()
        {
            // Arrange
            var pastBenchStartDate = DateTime.Now.AddDays(5);
            var futureBenchEndDate = DateTime.Now.AddDays(-2); // Bench end date earlier than start date
            var employeeDto = new AddEmployeeDto
            {
                BenchStartDate = pastBenchStartDate,
                BenchEndDate = futureBenchEndDate
            };

            var adminRepositoryMock = new Mock<IAdminRepository>();
            // No need to set up any other methods for this specific scenario
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(adminRepositoryMock.Object, _mockAllocatorRepository.Object);

            // Act
            var result = employeeService.AddEmployee(employeeDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Bench end date cannot be less that bench start date.", result.Message);
        }
        [Fact]
        public void AddEmployee_ReturnsError_WhenAddFails()
        {
            // Arrange
            var employeeDto = new AddEmployeeDto
            {
                EmployeeName = "John Doe",
                EmailId = "john.doe@example.com",
                BenchStartDate = DateTime.Now.AddDays(1),
                BenchEndDate = DateTime.Now.AddDays(10),
                JobRoleId = 101
            };

            var adminRepositoryMock = new Mock<IAdminRepository>();
            adminRepositoryMock.Setup(repo => repo.EmployeeNameExists(employeeDto.EmployeeName)).Returns(false);
            adminRepositoryMock.Setup(repo => repo.EmployeeEmailExists(employeeDto.EmailId)).Returns(false);
            adminRepositoryMock.Setup(repo => repo.Add(It.IsAny<Employee>())).Returns(false); // Simulate add failure
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(adminRepositoryMock.Object, _mockAllocatorRepository.Object);

            // Act
            var result = employeeService.AddEmployee(employeeDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Something went wrong. Please try later", result.Message);
        }
        [Fact]
        public void ModifyEmployee_ReturnsSuccess_WhenEmployeeUpdated()
        {
            // Arrange
            var existingEmployeeId = 42; // A valid employee ID
            var employeeDto = new UpdateEmployeeDto
            {
                EmployeeId = existingEmployeeId,
                EmployeeName = "Updated John Doe",
                EmailId = "updated.john.doe@example.com",
                BenchStartDate = DateTime.Now.AddDays(1),
                BenchEndDate = DateTime.Now.AddDays(10),
                JobRoleId = 101
            };

            var existingEmployee = new Employee
            {
                EmployeeId = existingEmployeeId,
                EmployeeName = "John Doe",
                EmailId = "john.doe@example.com",
                BenchStartDate = DateTime.Now.AddDays(2),
                BenchEndDate = DateTime.Now.AddDays(12),
                JobRoleId = 101,
                TypeId = 1
            };

            var adminRepositoryMock = new Mock<IAdminRepository>();
            adminRepositoryMock.Setup(repo => repo.GetEmployeeById(existingEmployeeId)).Returns(existingEmployee);
            adminRepositoryMock.Setup(repo => repo.Update(existingEmployee)).Returns(true);
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(adminRepositoryMock.Object, _mockAllocatorRepository.Object);

            // Act
            var result = employeeService.ModifyEmployee(employeeDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Employee updated successfully.", result.Message);
        }

        [Fact]
        public void ModifyEmployee_ReturnsError_WhenEmployeeNotFound()
        {
            // Arrange
            var nonExistentEmployeeId = 999; // An ID that doesn't exist in the sample data
            var employeeDto = new UpdateEmployeeDto { EmployeeId = nonExistentEmployeeId };

            var adminRepositoryMock = new Mock<IAdminRepository>();
            adminRepositoryMock.Setup(repo => repo.GetEmployeeById(nonExistentEmployeeId)).Returns((Employee)null);
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(adminRepositoryMock.Object, _mockAllocatorRepository.Object);

            // Act
            var result = employeeService.ModifyEmployee(employeeDto);

            // Assert
            Assert.False(result.Success);
        }
        [Fact]
        public void ModifyEmployee_ReturnsError_WhenBenchEndDateIsLessThanBenchStartDate()
        {
            // Arrange
            var pastBenchStartDate = DateTime.Now.AddDays(5);
            var futureBenchEndDate = DateTime.Now.AddDays(-2); // Bench end date earlier than start date
            var employeeDto = new UpdateEmployeeDto
            {
                BenchStartDate = pastBenchStartDate,
                BenchEndDate = futureBenchEndDate
            };

            var adminRepositoryMock = new Mock<IAdminRepository>();
            // No need to set up any other methods for this specific scenario
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(adminRepositoryMock.Object, _mockAllocatorRepository.Object);

            // Act
            var result = employeeService.ModifyEmployee(employeeDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Bench end date cannot be less that bench start date.", result.Message);
        }
        [Fact]
        public void ModifyEmployee_ReturnsError_WhenUpdateFails()
        {
            // Arrange
            var existingEmployeeId = 42; // A valid employee ID
            var employeeDto = new UpdateEmployeeDto
            {
                EmployeeId = existingEmployeeId,
                EmployeeName = "Updated John Doe",
                EmailId = "updated.john.doe@example.com",
                BenchStartDate = DateTime.Now.AddDays(1),
                BenchEndDate = DateTime.Now.AddDays(10),
                JobRoleId = 101
            };

            var existingEmployee = new Employee
            {
                EmployeeId = existingEmployeeId,
                EmployeeName = "John Doe",
                EmailId = "john.doe@example.com",
                BenchStartDate = DateTime.Now.AddDays(2),
                BenchEndDate = DateTime.Now.AddDays(12),
                JobRoleId = 101,
                TypeId = 1
            };

            var adminRepositoryMock = new Mock<IAdminRepository>();
            adminRepositoryMock.Setup(repo => repo.GetEmployeeById(existingEmployeeId)).Returns(existingEmployee);
            adminRepositoryMock.Setup(repo => repo.Update(existingEmployee)).Returns(false); // Simulate update failure
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(adminRepositoryMock.Object, _mockAllocatorRepository.Object);

            // Act
            var result = employeeService.ModifyEmployee(employeeDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Something went wrong,try after sometime", result.Message);
        }

        [Fact]
        public void ModifyEmployee_ReturnsError_WhenNameAlreadyExists()
        {
            //Arrange
            var dto = new UpdateEmployeeDto()
            {
                EmployeeId = 1,
                EmployeeName = "test1"
            };
            var adminRepositoryMock = new Mock<IAdminRepository>();
            adminRepositoryMock.Setup(c => c.EmployeeNameExists(It.IsAny<int>(), It.IsAny<string>())).Returns(true);
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(adminRepositoryMock.Object, _mockAllocatorRepository.Object);

            // Act
            var result = employeeService.ModifyEmployee(dto);

            // Assert
            adminRepositoryMock.Verify(c => c.EmployeeNameExists(It.IsAny<int>(), It.IsAny<string>()),Times.Once);
            Assert.False(result.Success);
            Assert.Equal("Employee name already exists.", result.Message);
        }

        [Fact]
        public void ModifyEmployee_ReturnsError_WhenEmailAlreadyExists()
        {
            //Arrange
            var dto = new UpdateEmployeeDto()
            {
                EmployeeId = 1,
                EmployeeName = "test1",
                EmailId = "user@test.com"
            };
            var adminRepositoryMock = new Mock<IAdminRepository>();
            adminRepositoryMock.Setup(c => c.EmployeeEmailExists(It.IsAny<int>(), It.IsAny<string>())).Returns(true);
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(adminRepositoryMock.Object, _mockAllocatorRepository.Object);

            // Act
            var result = employeeService.ModifyEmployee(dto);

            // Assert
            adminRepositoryMock.Verify(c => c.EmployeeEmailExists(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            Assert.False(result.Success);
            Assert.Equal("Email address already exists.", result.Message);
        }

        [Fact]
        public void RemoveEmployee_ReturnsSuccess_WhenEmployeeDeleted()
        {
            // Arrange
            var existingEmployeeId = 42; // A valid employee ID

            var adminRepositoryMock = new Mock<IAdminRepository>();
            adminRepositoryMock.Setup(repo => repo.Delete(existingEmployeeId)).Returns(true);
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(adminRepositoryMock.Object, _mockAllocatorRepository.Object);

            // Act
            var result = employeeService.RemoveEmployee(existingEmployeeId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Employee deleted successfully", result.Message);
        }

        [Fact]
        public void RemoveEmployee_ReturnsError_WhenEmployeeDeletionFails()
        {
            // Arrange
            var nonExistentEmployeeId = 999; // An ID that doesn't exist in the sample data

            var adminRepositoryMock = new Mock<IAdminRepository>();
            adminRepositoryMock.Setup(repo => repo.Delete(nonExistentEmployeeId)).Returns(false);
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(adminRepositoryMock.Object, _mockAllocatorRepository.Object);

            // Act
            var result = employeeService.RemoveEmployee(nonExistentEmployeeId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Something went wrong", result.Message);
        }

        [Fact]
        public void UpdateEmployee_ReturnsError_WhenDtoIsNull()
        {
            // Arrange
            var adminRepositoryMock = new Mock<IAdminRepository>();
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(adminRepositoryMock.Object, _mockAllocatorRepository.Object);

            // Act
            var result = employeeService.UpdateEmployee(null);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Something went wrong. Please try after sometime.", result.Message);
        }

        [Fact]
        public void UpdateEmployee_ReturnsError_WhenNoExistingEmployeeFound()
        {
            // Arrange
            var dto = new UpdateAllocationDto() { EmployeeId = 1, StartDate = DateTime.Now, TypeId = 1 };
            var adminRepositoryMock = new Mock<IAdminRepository>();
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(adminRepositoryMock.Object, _mockAllocatorRepository.Object);

            adminRepositoryMock.Setup(c => c.GetEmployeeById(-1)).Returns<Employee>(null);

            // Act
            var result = employeeService.UpdateEmployee(dto);

            // Assert
            Assert.False(result.Success);
        }

        [Fact]
        public void UpdateEmployee_ReturnsError_WhenTypeIdIs1AndUpdateFails()
        {
            // Arrange
            var dto = new UpdateAllocationDto() { EmployeeId = 1, StartDate = DateTime.Now, TypeId = 1 };
            var employee = new Employee()
            {
                EmployeeId = 1,
                TypeId = 1,
                BenchEndDate = DateTime.Now,
                BenchStartDate = DateTime.Now.AddDays(2)
            };
            var adminRepositoryMock = new Mock<IAdminRepository>();
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(adminRepositoryMock.Object, _mockAllocatorRepository.Object);

            adminRepositoryMock.Setup(c => c.GetEmployeeById(1)).Returns(employee);
            adminRepositoryMock.Setup(c => c.UpdateEmployee(employee)).Returns(false);


            // Act
            var result = employeeService.UpdateEmployee(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Something went wrong. Please try after sometime.", result.Message);
        }

        [Fact]
        public void UpdateEmployee_ReturnsError_WhenTypeIdIs2AndUpdateSucceseds()
        {
            // Arrange
            var dto = new UpdateAllocationDto() { EmployeeId = 1, StartDate = DateTime.Now, TypeId = 2 };
            var employee = new Employee()
            {
                EmployeeId = 1,
                TypeId = 2,
                BenchEndDate = DateTime.Now,
                BenchStartDate = DateTime.Now.AddDays(2)
            };
            var adminRepositoryMock = new Mock<IAdminRepository>();
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(adminRepositoryMock.Object, _mockAllocatorRepository.Object);

            adminRepositoryMock.Setup(c => c.GetEmployeeById(1)).Returns(employee);
            adminRepositoryMock.Setup(c => c.UpdateEmployee(employee)).Returns(true);


            // Act
            var result = employeeService.UpdateEmployee(dto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Employee updated successfully.", result.Message);
        }

        [Fact]
        public void GetEmployeesByDateRangeAndType_ReturnsSuccessWithData()
        {
            // Arrange
            var startDate = DateTime.Now.AddDays(-7);
            var endDate = DateTime.Now;
            var typeId = 1;

            var employeesData = new List<Employee>
            {
                new Employee
                {
                    EmployeeId = 1,
                    EmployeeName = "John Doe",
                    EmailId = "john.doe@example.com",
                    JobRoleId = 1,
                    JobRole = new JobRole {JobRoleId = 1 , JobRoleName = "developer"},
                    Allocationtype = new AllocationType { Type = "Full-Time" },
                    BenchStartDate = DateTime.Now.AddDays(-5),
                    BenchEndDate = DateTime.Now.AddDays(10),
                    TypeId = 1
                },
            };

            var adminRepositoryMock = new Mock<IAdminRepository>();
            adminRepositoryMock.Setup(repo => repo.GetEmployeesByDateRangeAndType(startDate, endDate, typeId))
                .Returns(employeesData);
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(adminRepositoryMock.Object, _mockAllocatorRepository.Object);

            // Act
            var result = employeeService.GetEmployeesByDateRangeAndType(startDate, endDate, typeId);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public void GetEmployeesByDateRangeAndType_ReturnsNoRecordsFound()
        {
            // Arrange
            var startDate = DateTime.Now.AddDays(-7);
            var endDate = DateTime.Now;
            var typeId = 2; // Assuming TypeId 2 for this test

            var emptyEmployeesData = new List<Employee>();

            var adminRepositoryMock = new Mock<IAdminRepository>();
            adminRepositoryMock.Setup(repo => repo.GetEmployeesByDateRangeAndType(startDate, endDate, typeId))
                .Returns(emptyEmployeesData);
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(adminRepositoryMock.Object, _mockAllocatorRepository.Object);

            // Act
            var result = employeeService.GetEmployeesByDateRangeAndType(startDate, endDate, typeId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("No records found", result.Message);
        }
        
        [Fact]
        public void GetEmployeesByJobRoleAndType_ValidInput_ReturnsEmployees()
        {
            // Arrange
            var jobRoleId = 123;
            var typeId = 456;

            // Create mock employee data
            var mockEmployees = new List<Employee>
        {
            new Employee { EmployeeId = 1, EmployeeName = "John Doe", JobRoleId = jobRoleId, TypeId = typeId },
            // Add more mock employees as needed
        };

            // Set up the mock repository behavior
            var adminRepositoryMock = new Mock<IAdminRepository>();
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(adminRepositoryMock.Object, _mockAllocatorRepository.Object);
            // Act
            var response = employeeService.GetEmployeesByJobRoleAndType(jobRoleId, typeId);

            // Assert
            Assert.False(response.Success);
        }
        

        [Fact]
        public void GetEmployeesByJobRoleAndType_NoEmployees_ReturnsEmptyList()
        {
            // Arrange
            var jobRoleId = 789;
            var typeId = 101;
            var adminRepositoryMock = new Mock<IAdminRepository>();
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(adminRepositoryMock.Object, _mockAllocatorRepository.Object);
            // Set up the mock repository behavior to return an empty list
            adminRepositoryMock.Setup(repo => repo.GetEmployeesByJobRoleAndType(jobRoleId, typeId))
                .Returns(new List<Employee>());

            // Act
            var response = employeeService.GetEmployeesByJobRoleAndType(jobRoleId, typeId);

            // Assert
            Assert.False(response.Success);
            Assert.Equal("No records found", response.Message);
        }

        
       
        [Fact]
        public void GetEmployeesByJobRoleAndType_Successful()
        {
            // Arrange
            var jobRoleId = 1;
            var typeId = 2;

            // Sample data for testing
            var employees = new List<Employee>
        {
            new Employee
            {
                EmployeeId = 1,
                EmployeeName = "John Doe",
                EmailId = "john.doe@example.com",
                JobRoleId = jobRoleId,
                JobRole = new JobRole { JobRoleId = jobRoleId, JobRoleName = "Developer" },
                BenchEndDate = null,
                TypeId = typeId
            },
            // Add more sample employees as needed
        };

            // Mock the repository directly inside the test method
            var adminRepositoryMock = new Mock<IAdminRepository>();
            adminRepositoryMock.Setup(repo => repo.GetEmployeesByJobRoleAndType(jobRoleId, typeId))
                                .Returns(employees);
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(adminRepositoryMock.Object, _mockAllocatorRepository.Object);

            // Act
            var result = employeeService.GetEmployeesByJobRoleAndType(jobRoleId, typeId);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(employees.Count, result.Data.Count());

            // Additional assertions can be added to check specific properties of EmployeeDto
            // For example:
            Assert.Equal(employees[0].EmployeeId, result.Data.First().EmployeeId);
            Assert.Equal(employees[0].EmployeeName, result.Data.First().EmployeeName);

            // Verify that the repository method was called with correct parameters
            adminRepositoryMock.Verify(repo => repo.GetEmployeesByJobRoleAndType(jobRoleId, typeId), Times.Once);
        }

        

        [Fact]
        public void GetAllJobRoles_ReturnsError_WhenNoJobRolesFound()
        {
            //Arrange
            List<JobRole> jobRoles = new List<JobRole>();
            var adminRepositoryMock = new Mock<IAdminRepository>();
            adminRepositoryMock.Setup(c => c.GetAllJobroles()).Returns(jobRoles);

            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(adminRepositoryMock.Object, _mockAllocatorRepository.Object);

            //Act
            var actual = employeeService.GetAllJobRoles();

            //Assert
            Assert.Equal("No record found!", actual.Message);
            Assert.False(actual.Success);

        }

        [Fact]
        public void GetAllJobRoles_ReturnsData_WhenJobRolesAreFound()
        {
            //Arrange
            List<JobRole> jobRoles = new List<JobRole>()
            {
                new JobRole{JobRoleId=1,JobRoleName="Developer"},
                new JobRole{JobRoleId=2,JobRoleName="Tester"},
            };
            var adminRepositoryMock = new Mock<IAdminRepository>();
            adminRepositoryMock.Setup(c => c.GetAllJobroles()).Returns(jobRoles);

            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(adminRepositoryMock.Object, _mockAllocatorRepository.Object);

            //Act
            var actual = employeeService.GetAllJobRoles();

            //Assert
            Assert.True(actual.Success);
            Assert.Equal(jobRoles.Count, actual.Data.Count());
        }

        [Fact]
        public void GetEmployeeData_ReturnsError_WhenEmployeesNotFound()
        {
            //Arrange
            var startDate = DateTime.Now.ToString();
            var endDate = DateTime.Now.AddDays(2).ToString();
            List<SPDto> dto = new List<SPDto>();
            var adminRepositoryMock = new Mock<IAdminRepository>();
            adminRepositoryMock.Setup(c => c.GetEmployeeData(startDate, endDate)).Returns(dto);

            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(adminRepositoryMock.Object, _mockAllocatorRepository.Object);

            //Act
            var actual = employeeService.GetEmployeeData(startDate,endDate);

            //Assert
            Assert.Equal("No record found", actual.Message);
            Assert.False(actual.Success);
        }

        [Fact]
        public void GetEmployeeData_ReturnsEmployees_WhenEmployeesFound()
        {
            //Arrange
            var startDate = DateTime.Now.ToString();
            var endDate = DateTime.Now.AddDays(2).ToString();
            List<SPDto> dto = new List<SPDto>()
            {
                new SPDto{EmployeeId=1,EmployeeName="Test1"},
                new SPDto{EmployeeId=2,EmployeeName="Test2"},
            };
            var adminRepositoryMock = new Mock<IAdminRepository>();
            adminRepositoryMock.Setup(c => c.GetEmployeeData(startDate, endDate)).Returns(dto);

            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var employeeService = new AdminService(adminRepositoryMock.Object, _mockAllocatorRepository.Object);

            //Act
            var actual = employeeService.GetEmployeeData(startDate, endDate);

            //Assert
            Assert.True(actual.Success);
            Assert.Equal(dto.Count, actual.Data.Count());
        }
        [Fact]
        public void GetEmployeeData_ValidDatesWithData_ReturnsSuccess()
        {
            // Arrange
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var mockAdminRepository = new Mock<IAdminRepository>();
            string startDate = "2024-01-01";
            string endDate = "2024-01-31"; // Assuming endDate is within range
            var _employeeService = new AdminService(mockAdminRepository.Object, _mockAllocatorRepository.Object);

            // Act
            var result = _employeeService.GetEmployeeData(startDate, endDate);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
        }
        [Fact]
        public void GetEmployeeData_ValidDatesNoData_ReturnsFailureWithMessage()
        {
            // Arrange
            var mockEmployees = new List<SPDto>
        {
            new SPDto
            {
                EmployeeId = 1,
                EmployeeName = "John Doe"
            }
            // Add more mock employees as needed
        };
            string startDate = "2024-01-01";
            string endDate = "2024-01-31"; // Assuming endDate is within range but no data exists
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var mockAdminRepository = new Mock<IAdminRepository>();
            var _employeeService = new AdminService(mockAdminRepository.Object, _mockAllocatorRepository.Object);
            mockAdminRepository.Setup(r => r.GetEmployeeData(startDate, endDate))
                              .Returns(mockEmployees);
            // Act
            var result = _employeeService.GetEmployeeData(startDate, endDate);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public void GetPaginatedEmployee_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var users = new List<EmployeeDto>
            {
            }.AsQueryable();
            var email = "email@example.com";
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var mockAdminRepository = new Mock<IAdminRepository>();
            mockAdminRepository.Setup(c => c.GetAllEmployeees()).Throws(new Exception());
            var target = new AdminService(mockAdminRepository.Object, _mockAllocatorRepository.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.GetAllEmployees());
            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }
        [Fact]
        public void GetPaginatedEmployees_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var users = new List<EmployeeDto>
            {
            }.AsQueryable();
            var page = 1;
            var pageSize = 10;
            var search = "NonExistentEmployee";
            var sortOrder = "asc";
            var sortBy = "EmployeeName";
            var email = "email@example.com";
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var mockAdminRepository = new Mock<IAdminRepository>();
            mockAdminRepository.Setup(c => c.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy)).Throws(new Exception());
            var target = new AdminService(mockAdminRepository.Object, _mockAllocatorRepository.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy));
            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }
        [Fact]
        public void GetEmployeeById_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var users = new List<EmployeeDto>
            {
            }.AsQueryable();
            var id = 1;
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var mockAdminRepository = new Mock<IAdminRepository>();
            mockAdminRepository.Setup(c => c.GetEmployeeById(id)).Throws(new Exception());
            var target = new AdminService(mockAdminRepository.Object, _mockAllocatorRepository.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.GetEmployeeById(id));
            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }
        [Fact]
        public void TotalEmployees_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var users = new List<EmployeeDto>
            {
            }.AsQueryable();
            var search = "";
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var mockAdminRepository = new Mock<IAdminRepository>();
            mockAdminRepository.Setup(c => c.TotalEmployees(search)).Throws(new Exception());
            var target = new AdminService(mockAdminRepository.Object, _mockAllocatorRepository.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.TotalEmployees(search));
            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }
        [Fact]
        public void AddEmployee_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var employeeDto = new AddEmployeeDto
            {
                EmployeeName = "Alice",
                EmailId = "alice@example.com",
                BenchStartDate = DateTime.Now.AddDays(1), // Future date
                BenchEndDate = DateTime.Now.AddDays(10), // Future date after BenchStartDate
                JobRoleId = 1,
                Skills = new List<string> { "Skill1", "Skill2" }
            };
            var search = "";
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var mockAdminRepository = new Mock<IAdminRepository>();
            mockAdminRepository.Setup(repo => repo.EmployeeNameExists(employeeDto.EmployeeName)).Throws(new Exception()); // Does not exist
            mockAdminRepository.Setup(repo => repo.EmployeeEmailExists(employeeDto.EmailId)).Throws(new Exception()); // Does not exist
            mockAdminRepository.Setup(repo => repo.Add(It.IsAny<Employee>())).Throws(new Exception()); var target = new AdminService(mockAdminRepository.Object, _mockAllocatorRepository.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.AddEmployee(employeeDto));
            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }
        [Fact]
        public void ModifyEmployee_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var employeeDto = new UpdateEmployeeDto
            {
                EmployeeId = 1,
                EmployeeName = "Alice",
                EmailId = "alice@example.com",
                BenchStartDate = DateTime.Now.AddDays(1), // Future date
                BenchEndDate = DateTime.Now.AddDays(10), // Future date after BenchStartDate
                JobRoleId = 1,
                Skills = new List<string> { "Skill1", "Skill2" }
            };
            var search = "";
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var mockAdminRepository = new Mock<IAdminRepository>();
            mockAdminRepository.Setup(repo => repo.EmployeeNameExists(employeeDto.EmployeeName)).Throws(new Exception()); // Does not exist
            mockAdminRepository.Setup(repo => repo.EmployeeEmailExists(employeeDto.EmailId)).Throws(new Exception()); // Does not exist
            mockAdminRepository.Setup(repo => repo.GetEmployeeById(employeeDto.EmployeeId)).Throws(new Exception()); // Does not exist

            mockAdminRepository.Setup(repo => repo.Update(It.IsAny<Employee>())).Throws(new Exception()); 
            var target = new AdminService(mockAdminRepository.Object, _mockAllocatorRepository.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.ModifyEmployee(employeeDto));
            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }

        [Fact]
        public void RemoveEmployee_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var users = new List<EmployeeDto>
            {
            }.AsQueryable();
            var id = 1;
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var mockAdminRepository = new Mock<IAdminRepository>();
            mockAdminRepository.Setup(c => c.Delete(id)).Throws(new Exception());
            var target = new AdminService(mockAdminRepository.Object, _mockAllocatorRepository.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.RemoveEmployee(id));
            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }
        [Fact]
        public void UpdateEmployee_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var employeeDto = new UpdateAllocationDto
            {
                EmployeeId = 1,
                EndDate = DateTime.Now.AddDays(1),
                StartDate = DateTime.Now.AddDays(1),
                TypeId = 1,
            };
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var mockAdminRepository = new Mock<IAdminRepository>();
            mockAdminRepository.Setup(repo => repo.GetEmployeeById(employeeDto.EmployeeId)).Throws(new Exception()); // Does not exist
            mockAdminRepository.Setup(repo => repo.UpdateEmployee(It.IsAny<Employee>())).Throws(new Exception()); var target = new AdminService(mockAdminRepository.Object, _mockAllocatorRepository.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.UpdateEmployee(employeeDto));
            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }
        [Fact]
        public void GetEmployeesByDateRangeAndType_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var users = new List<EmployeeDto>
            {
            }.AsQueryable();
            var startDate = DateTime.Parse("03-29-2024");
            var endDate = DateTime.Parse("03-29-2024");
            var typeId = 1;
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var mockAdminRepository = new Mock<IAdminRepository>();
            mockAdminRepository.Setup(c => c.GetEmployeesByDateRangeAndType(startDate,endDate,typeId)).Throws(new Exception());
            var target = new AdminService(mockAdminRepository.Object, _mockAllocatorRepository.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.GetEmployeesByDateRangeAndType(startDate,endDate,typeId));
            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }
        [Fact]
        public void GetEmployeesByJobRoleAndType_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var users = new List<EmployeeDto>
            {
            }.AsQueryable();
            var jobroleId = 1;
            var typeId = 1;
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var mockAdminRepository = new Mock<IAdminRepository>();
            mockAdminRepository.Setup(c => c.GetEmployeesByJobRoleAndType(jobroleId, typeId)).Throws(new Exception());
            var target = new AdminService(mockAdminRepository.Object, _mockAllocatorRepository.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.GetEmployeesByJobRoleAndType(jobroleId, typeId));
            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }
        [Fact]
        public void GetAllJobRoles_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var users = new List<EmployeeDto>
            {
            }.AsQueryable();
            var jobroleId = 1;
            var typeId = 1;
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var mockAdminRepository = new Mock<IAdminRepository>();
            mockAdminRepository.Setup(c => c.GetAllJobroles()).Throws(new Exception());
            var target = new AdminService(mockAdminRepository.Object, _mockAllocatorRepository.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.GetAllJobRoles());
            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }
        [Fact]
        public void GetEmployeeData_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var users = new List<EmployeeDto>
            {
            }.AsQueryable();
            var startDate = "03-29-2024";
            var endDate = "03-29-2024";
            var typeId = 1;
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            var mockAdminRepository = new Mock<IAdminRepository>();
            mockAdminRepository.Setup(c => c.GetEmployeeData(startDate, endDate)).Throws(new Exception());
            var target = new AdminService(mockAdminRepository.Object, _mockAllocatorRepository.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.GetEmployeeData(startDate, endDate));
            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }

    }
}
