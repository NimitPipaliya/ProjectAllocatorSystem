using Moq;
using ProjectAllocatorSystemAPI.Data.Contract;
using ProjectAllocatorSystemAPI.Data.Implementation;
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
    public class AllocatorServiceTests
    {
        [Fact]
        public void AddAllocation_Successfully_Adds_New_Employee()
        {
            // Arrange
            var mockRepository = new Mock<IAllocatorRepository>();
            var service = new AllocatorService(mockRepository.Object);

            var newEmployee = new Allocation
            {
                TypeId = 1,
                EmployeeId = 3,
                Details = "qwer",
                // Add other properties as needed for a valid new employee
            };
            mockRepository.Setup(x => x.InsertAllocation(It.IsAny<Allocation>()))
                          .Returns(true); // Simulate successful insertion

            // Act
            var result = service.AddAllocation(newEmployee);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Employee set on bench successfully", result.Message);
        }

        
        [Fact]
        public void AddAllocation_Returns_Failure_Response_When_Employee_Already_Exists()
        {
            // Arrange
            var mockRepository = new Mock<IAllocatorRepository>();
            var service = new AllocatorService(mockRepository.Object);

            var existingEmployee = new Allocation
            {

                TypeId = 1,
                EmployeeId = 3,
                Details = "qwert",
                // Add other properties as needed for an existing employee
            };
            var result = service.AddAllocation(existingEmployee);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Something went wrong. Please try later", result.Message);
        }

        [Fact]
        public void AddAllocation_Returns_Failure_Response_When_Insertion_Fails()
        {
            // Arrange
            var mockRepository = new Mock<IAllocatorRepository>();
            var service = new AllocatorService(mockRepository.Object);

            var newEmployee = new Allocation
            {

                TypeId = 1,
                EmployeeId = 3,
                Details = "qwert",
                // Add other properties as needed for a new employee
            };
            mockRepository.Setup(x => x.InsertAllocation(It.IsAny<Allocation>()))
                          .Returns(false); // Simulate insertion failure

            // Act
            var result = service.AddAllocation(newEmployee);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Something went wrong. Please try later", result.Message);
        }

        [Fact]
        public void AddAllocation_Returns_Failure_Response_For_Invalid_Email_Format()
        {
            // Arrange
            var mockRepository = new Mock<IAllocatorRepository>();
            var service = new AllocatorService(mockRepository.Object);

            var newEmployee = new Allocation
            {

                TypeId = 1,
                EmployeeId = 3,
                Details = "qwert",
                // Add other properties as needed
            };

            // Act
            var result = service.AddAllocation(newEmployee);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Something went wrong. Please try later", result.Message);
        }
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

            var mockAdminRepository = new Mock<IAllocatorRepository>();
            mockAdminRepository.Setup(repo => repo.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy)).Returns(employees);

            var employeeService = new AllocatorService(mockAdminRepository.Object);

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

            var mockAdminRepository = new Mock<IAllocatorRepository>();
            mockAdminRepository.Setup(repo => repo.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy)).Returns(emptyEmployees);

            var employeeService = new AllocatorService(mockAdminRepository.Object);

            // Act
            var result = employeeService.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("No records found", result.Message);
        }
        [Fact]
        public void GetEmployeeById_ReturnsEmployeeDto_WhenEmployeeExists()
        {
            // Arrange
            var employeeId = 1;

            var existingEmployee = new Employee
            {
                EmployeeId = 1,
                EmployeeName = "Alice",
                EmailId = "alice@example.com",
                JobRoleId = 1,
                BenchStartDate = DateTime.Now,
                BenchEndDate = null,
                JobRole = new JobRole { JobRoleId = 1, JobRoleName = "Developer" },
                TypeId = 1 /* Sample TypeId */
            };

            var mockAdminRepository = new Mock<IAllocatorRepository>();
            mockAdminRepository.Setup(repo => repo.GetEmployeeById(employeeId)).Returns(existingEmployee);

            var employeeService = new AllocatorService(mockAdminRepository.Object);

            // Act
            var result = employeeService.GetEmployeeById(employeeId);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);

            var employeeDto = result.Data;
            Assert.Equal(1, employeeDto.EmployeeId); // Validate EmployeeId mapping
            Assert.Equal("Alice", employeeDto.EmployeeName); // Validate EmployeeName mapping
            Assert.Equal("alice@example.com", employeeDto.EmailId); // Validate EmailId mapping
            Assert.Equal(1, employeeDto.JobRoleId); // Validate JobRoleId mapping
            Assert.Equal("Developer", employeeDto.JobRole.JobRoleName); // Validate JobRoleName mapping
            Assert.Null(employeeDto.BenchEndDate); // Validate BenchEndDate mapping
            Assert.Equal(1, employeeDto.TypeId); // Validate TypeId mapping
        }
        [Fact]
        public void GetEmployeeById_ReturnsFailureResponse_WhenEmployeeDoesNotExist()
        {
            // Arrange
            var nonExistingEmployeeId = 999;

            var mockAdminRepository = new Mock<IAllocatorRepository>();
            mockAdminRepository.Setup(repo => repo.GetEmployeeById(nonExistingEmployeeId)).Returns(() => null);

            var employeeService = new AllocatorService(mockAdminRepository.Object);

            // Act
            var result = employeeService.GetEmployeeById(nonExistingEmployeeId);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Something went wrong,try after sometime", result.Message);
        }
        [Fact]
        public void AddAllocationn_Successfully_Adds_New_Employee()
        {
            // Arrange
            var mockRepository = new Mock<IAllocatorRepository>();
            var service = new AllocatorService(mockRepository.Object);

            var newEmployee = new Allocation
            {
                EndDate = DateTime.Parse("2024-02-02"),
                StartDate = DateTime.Parse("2024-05-05"),
                TypeId = 1,
                EmployeeId = 3,
                Details = "qwer",
                // Add other properties as needed for a valid new employee
            };
            mockRepository.Setup(x => x.InsertAllocation(It.IsAny<Allocation>()))
                          .Returns(true); // Simulate successful insertion

            // Act
            var result = service.AddAllocation(newEmployee);

            // Assert
            Assert.False(result.Success);
        }

        [Fact]
        public void AddAllocationn_Successfully_Adds_New_EmployeeTypeID2()
        {
            // Arrange
            var mockRepository = new Mock<IAllocatorRepository>();
            var service = new AllocatorService(mockRepository.Object);

            var newEmployee = new Allocation
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(2),
                TypeId = 2,
                EmployeeId = 3,
                Details = "qwer",
                // Add other properties as needed for a valid new employee
            };
            mockRepository.Setup(x => x.InsertAllocation(It.IsAny<Allocation>()))
                          .Returns(true); // Simulate successful insertion

            // Act
            var result = service.AddAllocation(newEmployee);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Employee allocated to project successfully", result.Message);
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
            _mockAllocatorRepository.Setup(c => c.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy)).Throws(new Exception());
            var target = new AllocatorService(_mockAllocatorRepository.Object);

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
            _mockAllocatorRepository.Setup(c => c.GetEmployeeById(id)).Throws(new Exception());
            var target = new AllocatorService(_mockAllocatorRepository.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.GetEmployeeById(id));
            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }
        [Fact]
        public void AddAllocatio_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var newEmployee = new Allocation
            {
                EndDate = DateTime.Parse("07-20-2024"),
                StartDate = DateTime.Parse("07-25-2020"),
                TypeId = 1,
                EmployeeId = 3,
                Details = "qwer",
            };
            var _mockAllocatorRepository = new Mock<IAllocatorRepository>();
            _mockAllocatorRepository.Setup(x => x.InsertAllocation(It.IsAny<Allocation>()))
                          .Throws(new Exception());
            var target = new AllocatorService(_mockAllocatorRepository.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.AddAllocation(newEmployee));
            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }

    }
}
