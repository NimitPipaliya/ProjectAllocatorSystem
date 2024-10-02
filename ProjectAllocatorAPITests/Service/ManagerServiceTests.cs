using Moq;
using ProjectAllocatorSystemAPI.Data.Contract;
using ProjectAllocatorSystemAPI.Dtos;
using ProjectAllocatorSystemAPI.Models;
using ProjectAllocatorSystemAPI.Service.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAllocatorAPITests.Service
{
    public class ManagerServiceTests
    {
        [Fact]
        public void GetPaginatedEmployees_ReturnsEmployees_WhenEmployeesExist()
        {
            // Arrange
            var page = 1;
            var pageSize = 10;
            var search = "Alice";
            var sortOrder = "asc";
            var sortBy = "EmployeeName";

            var employees = new List<Employee>
        {
            new Employee { EmployeeId = 1, EmployeeName = "Alice", EmailId = "alice@example.com", JobRoleId = 1, BenchStartDate = DateTime.Now, BenchEndDate = null, JobRole = new JobRole { JobRoleId = 1, JobRoleName = "Developer" }, TypeId = 1 /* Sample TypeId */ }
            // Add more employees as needed
        };

            var mockAdminRepository = new Mock<IManagerRepository>();
            mockAdminRepository.Setup(repo => repo.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy)).Returns(employees);

            var employeeService = new ManagerService(mockAdminRepository.Object);

            // Act
            var result = employeeService.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy);

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
        public void GetPaginatedEmployees_ReturnsNoRecordsFound_WhenNoEmployeesExist()
        {
            // Arrange
            var page = 1;
            var pageSize = 10;
            var search = "NonExistentEmployee";
            var sortOrder = "asc";
            var sortBy = "EmployeeName";

            var emptyEmployees = new List<Employee>(); // Empty list of employees

            var mockAdminRepository = new Mock<IManagerRepository>();
            mockAdminRepository.Setup(repo => repo.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy)).Returns(emptyEmployees);

            var employeeService = new ManagerService(mockAdminRepository.Object);

            // Act
            var result = employeeService.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("No records found", result.Message);
        }
        [Fact]
        public void TotalEmployees_ReturnsTotalCount_WhenSearchIsNotNullOrEmpty()
        {
            // Arrange
            string searchKeyword = "Alice";
            int expectedTotal = 5; // Sample total count

            var mockAdminRepository = new Mock<IManagerRepository>();
            mockAdminRepository.Setup(repo => repo.TotalEmployees(searchKeyword)).Returns(expectedTotal);

            var employeeService = new ManagerService(mockAdminRepository.Object);

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

            var mockAdminRepository = new Mock<IManagerRepository>();
            mockAdminRepository.Setup(repo => repo.TotalEmployees(searchKeyword)).Returns(expectedTotal);

            var employeeService = new ManagerService(mockAdminRepository.Object);

            // Act
            var result = employeeService.TotalEmployees(searchKeyword);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(expectedTotal, result.Data);
        }
        [Fact]
        public void GetAllocationByEmpId_ReturnsAllocationDto_WhenEmployeeExists()
        {
            // Arrange
            int employeeId = 1;
            var mockManagerRepository = new Mock<IManagerRepository>();
            var existingEmployee = new Allocation
            {
                AllocationId = 1,
                EmployeeId = employeeId,
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(30),
                Details = "Allocation details",
                TrainingId = 1,
                Training = new Training { TrainingId = 1, Name = "Training 1", Description = "Training description" },
                InternalProjectId = 1,
                InternalProject = new InternalProject { InternalProjectId = 1, Name = "Project 1", Description = "Project description" },
                TypeId = 1,
                Employee = new Employee { EmployeeId = employeeId ,EmployeeName = "test"},
            };
            
            mockManagerRepository.Setup(repo => repo.GetAllocationByEmployeeById(employeeId))
                                 .Returns(existingEmployee);

            var employeeService = new ManagerService(mockManagerRepository.Object);

            // Act
            var response = employeeService.GetAllocationByEmpId(employeeId);

            // Assert
            Assert.True(response.Success);
            Assert.NotNull(response.Data);

            var allocationDto = response.Data;
            Assert.Equal(existingEmployee.AllocationId, allocationDto.AllocationId);
            Assert.Equal(existingEmployee.EmployeeId, allocationDto.EmployeeId);
            Assert.Equal(existingEmployee.StartDate, allocationDto.StartDate);
            Assert.Equal(existingEmployee.EndDate, allocationDto.EndDate);
            Assert.Equal(existingEmployee.Details, allocationDto.Details);
            Assert.Equal(existingEmployee.TrainingId, allocationDto.TrainingId);
            Assert.Equal(existingEmployee.Training.Name, allocationDto.Training.Name);
            Assert.Equal(existingEmployee.Training.Description, allocationDto.Training.Description);
            Assert.Equal(existingEmployee.InternalProjectId, allocationDto.InternalProjectId);
            Assert.Equal(existingEmployee.InternalProject.Name, allocationDto.InternalProject.Name);
            Assert.Equal(existingEmployee.InternalProject.Description, allocationDto.InternalProject.Description);
            Assert.Equal(existingEmployee.TypeId, allocationDto.TypeId);
        }

        [Fact]
        public void GetAllocationByEmpId_ReturnsError_WhenEmployeeNotFound()
        {
            // Arrange
            var nonExistentEmployeeId = 999; // An ID that doesn't exist in the sample data

            var managerRepositoryMock = new Mock<IManagerRepository>();
            managerRepositoryMock.Setup(repo => repo.GetAllocationByEmployeeById(nonExistentEmployeeId))
                .Returns((Allocation)null);

            var allocationService = new ManagerService(managerRepositoryMock.Object);

            // Act
            var result = allocationService.GetAllocationByEmpId(nonExistentEmployeeId);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Something went wrong,try after sometime", result.Message);
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
            var mockAdminRepository = new Mock<IManagerRepository>();
            mockAdminRepository.Setup(c => c.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy)).Throws(new Exception());
            var target = new ManagerService(mockAdminRepository.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy));
            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }
        [Fact]
        public void TotalEmployees_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var search = "NonExistentEmployee";
            var mockAdminRepository = new Mock<IManagerRepository>();
            mockAdminRepository.Setup(c => c.TotalEmployees(search)).Throws(new Exception());
            var target = new ManagerService(mockAdminRepository.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.TotalEmployees(search));
            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }
        [Fact]
        public void GetAllocationByEmpId_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var id = 1;
            var mockAdminRepository = new Mock<IManagerRepository>();
            mockAdminRepository.Setup(c => c.GetAllocationByEmployeeById(id)).Throws(new Exception());
            var target = new ManagerService(mockAdminRepository.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.GetAllocationByEmpId(id));
            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }
    }
}
