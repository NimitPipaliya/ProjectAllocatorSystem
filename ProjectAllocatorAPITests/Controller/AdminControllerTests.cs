using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Moq;
using ProjectAllocatorSystemAPI.Controllers;
using ProjectAllocatorSystemAPI.Dtos;
using ProjectAllocatorSystemAPI.Models;
using ProjectAllocatorSystemAPI.Service.Contract;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAllocatorAPITests.Controller
{
    public class AdminControllerTests:IDisposable
    {
        private readonly Mock<IAdminService> _mockEmployeeService;
        private readonly AdminController _employeeController;

        public AdminControllerTests()
        {
            // Arrange: Create a mock for IEmployeeService
            _mockEmployeeService = new Mock<IAdminService>();

            // Arrange: Create an instance of the controller with the mock service
            _employeeController = new AdminController(_mockEmployeeService.Object);
        }
        [Fact]
        public void GetAllEmployees_ReturnsOkWithEmployees_WhenEmployeesExist()
        {
            // Arrange
            var expectedEmployees = new List<EmployeeDto>
            {
                new EmployeeDto { EmployeeId = 1, EmployeeName = "John Doe" },
                new EmployeeDto { EmployeeId = 2, EmployeeName = "Jane Smith" }
                // Add more sample employees as needed
            };

            _mockEmployeeService.Setup(c => c.GetAllEmployees())
                .Returns(new ServiceResponse<IEnumerable<EmployeeDto>>
                {
                    Success = true,
                    Data = expectedEmployees
                });

            // Act
            var result = _employeeController.GetAllEmployees() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Value);
            _mockEmployeeService.Verify(c => c.GetAllEmployees(), Times.Once);
        }
        [Fact]
         public void GetAllEmployees_ThrowsException()
        {
            //Arrange
            var target = new AdminController(_mockEmployeeService.Object);
            _mockEmployeeService.Setup(c => c.GetAllEmployees()).Throws(new Exception());

            //Act

            var exception = Assert.Throws<Exception>(() => target.GetAllEmployees());

            //Assert
           

            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
            _mockEmployeeService.Verify(c => c.GetAllEmployees(), Times.Once);

        }

        [Fact]
        public void GetAllEmployees_ReturnsNotFound_WhenNoEmployeesExist()
        {
            // Arrange
            var emptyResponse = new ServiceResponse<IEnumerable<EmployeeDto>>
            {
                Success = false,
                Data = new List<EmployeeDto>(),
            };

            _mockEmployeeService.Setup(c => c.GetAllEmployees()).Returns(emptyResponse);

            // Act
            var result = _employeeController.GetAllEmployees() as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
            Assert.NotNull(result.Value);
            Assert.Equal(emptyResponse, result.Value);
            _mockEmployeeService.Verify(c => c.GetAllEmployees(), Times.Once);
        }
        

        [Fact]
        public void GetEmployeeById_ReturnsBadRequest_WithInvalidId()
        {
            // Arrange
            var invalidId = -1; // Example invalid employee ID

            // Act
            var result = _employeeController.GetEmployeeById(invalidId) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.NotNull(result.Value);
            Assert.Equal("Please enter valid data.", result.Value);
            // Add more assertions based on your specific requirements
        }
        [Fact]
        public void GetEmployeeById_ReturnsOk()
        {

            var Id = 1;
            var Employee = new Employee
            {
                EmployeeId = Id,
                EmployeeName = "Employee 1"
            };

            var response = new ServiceResponse<EmployeeDto>
            {
                Success = true,
                Data = new EmployeeDto
                {
                    EmployeeId = Id,
                    EmployeeName = Employee.EmployeeName
                }
            };
            var target = new AdminController(_mockEmployeeService.Object);
            _mockEmployeeService.Setup(c => c.GetEmployeeById(Id)).Returns(response);

            //Act
            var actual = target.GetEmployeeById(Id) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            _mockEmployeeService.Verify(c => c.GetEmployeeById(Id), Times.Once);
        }

        [Fact]
        public void GetEmployeeById_ReturnsNotFound()
        {

            var Id = 1;
            var Employee = new Employee
            {
                EmployeeId = Id,
                EmployeeName = "Employee 1"
            };

            var response = new ServiceResponse<EmployeeDto>
            {
                Success = false,
                Data = new EmployeeDto
                {
                    EmployeeId = Id,
                    EmployeeName = Employee.EmployeeName
                }
            };
            var target = new AdminController(_mockEmployeeService.Object);
            _mockEmployeeService.Setup(c => c.GetEmployeeById(Id)).Returns(response);

            //Act
            var actual = target.GetEmployeeById(Id) as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            _mockEmployeeService.Verify(c => c.GetEmployeeById(Id), Times.Once);
        }
        [Fact]
        public void GetAllEmployeeById_ThrowsException()
        {
            //Arrange
            var target = new AdminController(_mockEmployeeService.Object);
            _mockEmployeeService.Setup(c => c.GetEmployeeById(1)).Throws(new Exception());

            //Act

            var exception = Assert.Throws<Exception>(() => target.GetEmployeeById(1));

            //Assert


            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
            _mockEmployeeService.Verify(c => c.GetEmployeeById(1), Times.Once);

        }
        [Fact]
        public void GetPaginatedEmployees_ReturnsOkWithEmployees_WhenLetterIsNull_SearchIsNull()
        {
            //Arrange
            var Employees = new List<Employee>
            {
               new Employee{EmployeeId=1,EmployeeName="Employee 1"},
                 new Employee{EmployeeId=2,EmployeeName="Employee 2"},
             };

            int page = 1;
            int pageSize = 2;
            string sortOrder = "asc";
            string sortBy = "";
            var response = new ServiceResponse<IEnumerable<EmployeeDto>>
            {
                Success = true,
                //Data = Employees.Select(c => new EmployeeDto { Id = c.Id, FirstName = c.FirstName, EmployeeNumber = c.EmployeeNumber }) // Convert to EmployeeDto
            };

            var mockEmployeeService = new Mock<IAdminService>();
            var target = new AdminController(mockEmployeeService.Object);
            mockEmployeeService.Setup(c => c.GetPaginatedEmployees(page, pageSize, null, sortOrder,sortBy)).Returns(response);

            //Act
            var actual = target.GetPaginatedEmployees(null, sortBy,page, pageSize) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockEmployeeService.Verify(c => c.GetPaginatedEmployees(page, pageSize, null, sortOrder,sortBy), Times.Once);
        }
        [Fact]
        public void GetPaginatedEmployees_ReturnsOkWithEmployees_WhenLetterIsNull_SearchIsNotNull()
        {
            //Arrange
            var Employees = new List<Employee>
            {
               new Employee{EmployeeId=1,EmployeeName="Employee 1"},
                 new Employee{EmployeeId=2,EmployeeName = "Employee 2"},
             };

            int page = 1;
            int pageSize = 2;
            string sortOrder = "asc";
            string search = "tac";
            string sortBy = "";
            var response = new ServiceResponse<IEnumerable<EmployeeDto>>
            {
                Success = true,
                // Data = Employees.Select(c => new EmployeeDto { Id = c.Id, FirstName = c.FirstName, EmployeeNumber = c.EmployeeNumber }) // Convert to EmployeeDto
            };
            var target = new AdminController(_mockEmployeeService.Object);
            _mockEmployeeService.Setup(c => c.GetPaginatedEmployees(page, pageSize, search, sortOrder,sortBy)).Returns(response);

            //Act
            var actual = target.GetPaginatedEmployees(search, sortBy,page, pageSize, sortOrder) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            _mockEmployeeService.Verify(c => c.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy), Times.Once);
        }

        [Fact]
        public void GetPaginatedEmployees_ReturnsOkWithEmployees_WhenLetterIsNotNull_SearchIsNull()
        {
            //Arrange
            var Employees = new List<Employee>
            {
               new Employee{EmployeeId=1,EmployeeName="Employee 1"},
                 new Employee{EmployeeId=2,EmployeeName="Employee 2"},
             };

            var letter = 'd';
            int page = 1;
            int pageSize = 2;
            string sortOrder = "asc";
            string sortBy = "";

            var response = new ServiceResponse<IEnumerable<EmployeeDto>>
            {
                Success = true,
                //Data = Employees.Select(c => new EmployeeDto { Id = c.Id, FirstName = c.FirstName, EmployeeNumber = c.EmployeeNumber }) // Convert to EmployeeDto
            };
            var target = new AdminController(_mockEmployeeService.Object);
            _mockEmployeeService.Setup(c => c.GetPaginatedEmployees(page, pageSize, null, sortOrder,sortBy)).Returns(response);

            //Act
            var actual = target.GetPaginatedEmployees(null,sortBy, page, pageSize, sortOrder) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            _mockEmployeeService.Verify(c => c.GetPaginatedEmployees(page, pageSize, null, sortOrder, sortBy), Times.Once);
        }
        [Fact]
        public void GetPaginatedEmployees_ReturnsOkWithEmployees_WhenLetterIsNotNull_SearchIsNotNull()
        {
            //Arrange
            var Employees = new List<Employee>
            {
               new Employee{EmployeeId=1,EmployeeName="Employee 1"},
                 new Employee{EmployeeId=2,EmployeeName="Employee 2"},
             };

            var letter = 'd';
            int page = 1;
            int pageSize = 2;
            string sortOrder = "asc";
            string search = "dev";
            string sortBy = "";
            var response = new ServiceResponse<IEnumerable<EmployeeDto>>
            {
                Success = true,
                //Data = Employees.Select(c => new EmployeeDto { Id = c.Id, FirstName = c.FirstName, EmployeeNumber = c.EmployeeNumber }) // Convert to EmployeeDto
            };
            var target = new AdminController(_mockEmployeeService.Object);
            _mockEmployeeService.Setup(c => c.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy)).Returns(response);

            //Act
            var actual = target.GetPaginatedEmployees(search, sortBy,page, pageSize, sortOrder) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            _mockEmployeeService.Verify(c => c.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy), Times.Once);
        }

        [Fact]
        public void GetPaginatedEmployees_ReturnsNotFound_WhenLetterIsNull_SearchIsNull()
        {
            //Arrange
            var Employees = new List<Employee>
            {
               new Employee{EmployeeId=1,EmployeeName = "Employee 1"},
                 new Employee{EmployeeId=2,EmployeeName = "Employee 2"},
             };

            int page = 1;
            int pageSize = 2;
            string sortOrder = "asc";
            string sortBy = "";
            var response = new ServiceResponse<IEnumerable<EmployeeDto>>
            {
                Success = false,
                //Data = Employees.Select(c => new EmployeeDto { Id = c.Id, FirstName = c.FirstName, EmployeeNumber = c.EmployeeNumber }) // Convert to EmployeeDto
            };
            var target = new AdminController(_mockEmployeeService.Object);
            _mockEmployeeService.Setup(c => c.GetPaginatedEmployees(page, pageSize, null, sortOrder,sortBy)).Returns(response);

            //Act
            var actual = target.GetPaginatedEmployees(null, sortBy, page, pageSize, sortOrder) as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            _mockEmployeeService.Verify(c => c.GetPaginatedEmployees(page, pageSize, null, sortOrder, sortBy), Times.Once);
        }
        [Fact]
        public void GetPaginatedEmployees_ReturnsNotFound_WhenLetterIsNull_SearchIsNotNull()
        {
            //Arrange
            var Employees = new List<Employee>
            {
               new Employee{EmployeeId=1,EmployeeName = "Employee 1"},
                 new Employee{EmployeeId=2,EmployeeName = "Employee 2"},
             };

            int page = 1;
            int pageSize = 2;
            string sortOrder = "asc";
            string search = "dev";
            string sortBy = "";
            var response = new ServiceResponse<IEnumerable<EmployeeDto>>
            {
                Success = false,
                //Data = Employees.Select(c => new EmployeeDto { Id = c.Id, FirstName = c.FirstName, EmployeeNumber = c.EmployeeNumber }) // Convert to EmployeeDto
            };
            var target = new AdminController(_mockEmployeeService.Object);
            _mockEmployeeService.Setup(c => c.GetPaginatedEmployees(page, pageSize, search, sortOrder,sortBy)).Returns(response);

            //Act
            var actual = target.GetPaginatedEmployees(search, sortBy,page, pageSize, sortOrder) as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            _mockEmployeeService.Verify(c => c.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy), Times.Once);
        }

        [Fact]
        public void GetPaginatedEmployees_ReturnsNotFound_WhenLetterIsNotNull_SearchIsNull()
        {
            //Arrange
            var Employees = new List<Employee>
            {
               new Employee{EmployeeId=1,EmployeeName="Employee 1"},
                 new Employee{EmployeeId=2,EmployeeName = "Employee 2"},
             };

            var letter = 'd';
            int page = 1;
            int pageSize = 2;
            string sortOrder = "asc";
            string sortBy = "";

            var response = new ServiceResponse<IEnumerable<EmployeeDto>>
            {
                Success = false,
                //Data = Employees.Select(c => new EmployeeDto { Id = c.Id, FirstName = c.FirstName, EmployeeNumber = c.EmployeeNumber }) // Convert to EmployeeDto
            };
            var target = new AdminController(_mockEmployeeService.Object);
            _mockEmployeeService.Setup(c => c.GetPaginatedEmployees(page, pageSize, null, sortOrder, sortBy)).Returns(response);

            //Act
            var actual = target.GetPaginatedEmployees(null, sortBy,page, pageSize, sortOrder) as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            _mockEmployeeService.Verify(c => c.GetPaginatedEmployees(page, pageSize, null, sortOrder, sortBy), Times.Once);
        }
        [Fact]
        public void GetPaginatedEmployees_ReturnsNotFound_WhenLetterIsNotNull_SearchIsNotNull()
        {
            //Arrange
            var Employees = new List<Employee>
            {
               new Employee{EmployeeId = 1, EmployeeName = "Employee 1"},
                 new Employee{EmployeeId=2,EmployeeName="Employee 2"},
             };

            var letter = 'd';
            int page = 1;
            int pageSize = 2;
            string sortOrder = "asc";
            string search = "dev";
            string sortBy = "";

            var response = new ServiceResponse<IEnumerable<EmployeeDto>>
            {
                Success = false,
                // Data = Employees.Select(c => new EmployeeDto { Id = c.Id, FirstName = c.FirstName, EmployeeNumber = c.EmployeeNumber }) // Convert to EmployeeDto
            };
            var target = new AdminController(_mockEmployeeService.Object);
            _mockEmployeeService.Setup(c => c.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy)).Returns(response);

            //Act
            var actual = target.GetPaginatedEmployees(search, sortBy,page, pageSize, sortOrder) as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            _mockEmployeeService.Verify(c => c.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy), Times.Once);
        }

        [Fact]
        public void GetPaginatedEmployees_ThrowsException()
        {
            //Arrange
            int page = 1;
            int pageSize = 2;
            string sortOrder = "asc";
            string search = "dev";
            string sortBy = "";

            var target = new AdminController(_mockEmployeeService.Object);
            _mockEmployeeService.Setup(c => c.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy)).Throws(new Exception());

            //Act

            var exception = Assert.Throws<Exception>(() => target.GetPaginatedEmployees(search, sortBy,page,pageSize,sortOrder));

            //Assert


            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
            _mockEmployeeService.Verify(c => c.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy), Times.Once);

        }
        [Fact]
        public void AddContact_ReturnsOk_WhenContactIsAddedSuccessfully()
        {
            var fixture = new Fixture();
            var addContactDto = fixture.Create<AddEmployeeDto>();
            var response = new ServiceResponse<string>
            {
                Success = true,
            };
            var target = new AdminController(_mockEmployeeService.Object);
            _mockEmployeeService.Setup(c => c.AddEmployee(It.IsAny<AddEmployeeDto>())).Returns(response);

            //Act

            var actual = target.AddEmployee(addContactDto) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            _mockEmployeeService.Verify(c => c.AddEmployee(It.IsAny<AddEmployeeDto>()), Times.Once);

        }

        [Fact]
        public void AddContact_ReturnsBadRequest_WhenContactIsNotAdded()
        {
            var fixture = new Fixture();
            var addContactDto = fixture.Create<AddEmployeeDto>();
            var response = new ServiceResponse<string>
            {
                Success = false,
            };
            var target = new AdminController(_mockEmployeeService.Object);
            _mockEmployeeService.Setup(c => c.AddEmployee(It.IsAny<AddEmployeeDto>())).Returns(response);

            //Act

            var actual = target.AddEmployee(addContactDto) as BadRequestObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            _mockEmployeeService.Verify(c => c.AddEmployee(It.IsAny<AddEmployeeDto>()), Times.Once);

        }

        [Fact]
        public void AddContact_ThrowsException()
        {
            //Arrange
            var fixture = new Fixture();
            var addContactDto = fixture.Create<AddEmployeeDto>();
            var response = new ServiceResponse<string>
            {
                Success = false,
            };

            var target = new AdminController(_mockEmployeeService.Object);
            _mockEmployeeService.Setup(c => c.AddEmployee(It.IsAny<AddEmployeeDto>())).Throws(new Exception());

            //Ac

            var exception = Assert.Throws<Exception>(() => target.AddEmployee(It.IsAny<AddEmployeeDto>()));

            //Assert


            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
            _mockEmployeeService.Verify(c => c.AddEmployee(It.IsAny<AddEmployeeDto>()), Times.Once);

        }
        [Fact]
        public void Edit_ReturnsOk_WhenContactIsUpdatesSuccessfully()
        {
            var fixture = new Fixture();
            var updateContactDto = fixture.Create<UpdateEmployeeDto>();
            var response = new ServiceResponse<string>
            {
                Success = true,
            };
            var target = new AdminController(_mockEmployeeService.Object);
            _mockEmployeeService.Setup(c => c.ModifyEmployee(It.IsAny<UpdateEmployeeDto>())).Returns(response);

            //Act

            var actual = target.UpdateEmployee(updateContactDto) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            _mockEmployeeService.Verify(c => c.ModifyEmployee(It.IsAny<UpdateEmployeeDto>()), Times.Once);

        }

        [Fact]
        public void Edit_ReturnsBadRequest_WhenContactIsNotUpdated()
        {
            var fixture = new Fixture();
            var updateContactDto = fixture.Create<UpdateEmployeeDto>();
            var response = new ServiceResponse<string>
            {
                Success = false,
            };
            var target = new AdminController(_mockEmployeeService.Object);
            _mockEmployeeService.Setup(c => c.ModifyEmployee(It.IsAny<UpdateEmployeeDto>())).Returns(response);

            //Act

            var actual = target.UpdateEmployee(updateContactDto) as BadRequestObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            _mockEmployeeService.Verify(c => c.ModifyEmployee(It.IsAny<UpdateEmployeeDto>()), Times.Once);

        }
        [Fact]
        public void UpdateEmployee_ThrowsException()
        {
            //Arrange
            var fixture = new Fixture();
            var updateContactDto = fixture.Create<UpdateEmployeeDto>();
            var response = new ServiceResponse<string>
            {
                Success = false,
            };

            var target = new AdminController(_mockEmployeeService.Object);
            _mockEmployeeService.Setup(c => c.ModifyEmployee(It.IsAny<UpdateEmployeeDto>())).Throws(new Exception());

            //Ac

            var exception = Assert.Throws<Exception>(() => target.UpdateEmployee(It.IsAny<UpdateEmployeeDto>()));

            //Assert


            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
            _mockEmployeeService.Verify(c => c.ModifyEmployee(It.IsAny<UpdateEmployeeDto>()), Times.Once);

        }
        [Fact]
        public void DeleteConfirmed_ReturnsOkResponse_WhenContactDeletedSuccessfully()
        {

            var contactId = 1;
            var response = new ServiceResponse<string>
            {
                Success = true,
            };
            var target = new AdminController(_mockEmployeeService.Object);
            _mockEmployeeService.Setup(c => c.RemoveEmployee(contactId)).Returns(response);

            //Act

            var actual = target.DeleteEmployee(contactId) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            _mockEmployeeService.Verify(c => c.RemoveEmployee(contactId), Times.Once);
        }

        [Fact]
        public void DeleteConfirmed_ReturnsBadRequest_WhenContactNotDeleted()
        {

            var contactId = 1;
            var response = new ServiceResponse<string>
            {
                Success = false,
            };
            var target = new AdminController(_mockEmployeeService.Object);
            _mockEmployeeService.Setup(c => c.RemoveEmployee(contactId)).Returns(response);

            //Act

            var actual = target.DeleteEmployee(contactId) as BadRequestObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            _mockEmployeeService.Verify(c => c.RemoveEmployee(contactId), Times.Once);
        }

        [Fact]
        public void DeleteEmployee_ThrowsException()
        {
            //Arrange
            var target = new AdminController(_mockEmployeeService.Object);
            _mockEmployeeService.Setup(c => c.RemoveEmployee(1)).Throws(new Exception());

            //Act

            var exception = Assert.Throws<Exception>(() => target.DeleteEmployee(1));

            //Assert


            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
            _mockEmployeeService.Verify(c => c.RemoveEmployee(1), Times.Once);

        }

        [Fact]
        public void GetAllJobRoles_ValidData_ReturnsOkResult()
        {
            // Arrange
            var mockJobRoles = new[]
            {
            new JobRoleDto { JobRoleId = 1, JobRoleName = "Developer" },
            new JobRoleDto { JobRoleId = 2, JobRoleName = "Designer" }
        };

            // Set up the mock service behavior
            _mockEmployeeService.Setup(service => service.GetAllJobRoles())
                .Returns(new ServiceResponse<IEnumerable<JobRoleDto>>
                {
                    Success = true,
                    Data = mockJobRoles
                });

            // Act
            var result = _employeeController.GetAllJobRoles();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
        }

        [Fact]
        public void GetAllJobRoles_NoData_ReturnsNotFoundResult()
        {
            // Arrange
            // Set up the mock service behavior for no job roles found
            _mockEmployeeService.Setup(service => service.GetAllJobRoles())
                .Returns(new ServiceResponse<IEnumerable<JobRoleDto>>
                {
                    Success = false,
                    Message = "No record found!"
                });

            // Act
            var result = _employeeController.GetAllJobRoles();

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = (NotFoundObjectResult)result;
        }

        [Fact]
        public void GetAllJobRoles_ThrowsException()
        {
            //Arrange
            var target = new AdminController(_mockEmployeeService.Object);
            _mockEmployeeService.Setup(c => c.GetAllJobRoles()).Throws(new Exception());

            //Act

            var exception = Assert.Throws<Exception>(() => target.GetAllJobRoles());

            //Assert


            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
            _mockEmployeeService.Verify(c => c.GetAllJobRoles(), Times.Once);

        }


        [Fact]
        public void GetEmployeesByJobRoleAndType_NoData_ReturnsNotFoundResult()
        {
            // Arrange
            // Set up the mock service behavior for no employees found
            _mockEmployeeService.Setup(service => service.GetEmployeesByJobRoleAndType(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ServiceResponse<IEnumerable<EmployeeDto>>
                {
                    Success = false,
                    Message = "No records found"
                });

            // Act
            var result = _employeeController.GetEmployeesByJobRoleAndType(789, 101);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = (NotFoundObjectResult)result;
        }
        [Fact]
        public void GetEmployeesByJobRoleAndType_Data_ReturnsNotFoundResult()
        {
            // Arrange
            // Set up the mock service behavior for no employees found
            _mockEmployeeService.Setup(service => service.GetEmployeesByJobRoleAndType(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ServiceResponse<IEnumerable<EmployeeDto>>
                {
                    Success = true,
                });

            // Act
            var result = _employeeController.GetEmployeesByJobRoleAndType(789, 101);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var notFoundResult = (OkObjectResult)result;
        }

        [Fact]
        public void GetEmployeesByJobRoleAndType_ThrowsException()
        {
            //Arrange
            var target = new AdminController(_mockEmployeeService.Object);
            _mockEmployeeService.Setup(c => c.GetEmployeesByJobRoleAndType(1,1)).Throws(new Exception());

            //Act

            var exception = Assert.Throws<Exception>(() => target.GetEmployeesByJobRoleAndType(1,1));

            //Assert


            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
            _mockEmployeeService.Verify(c => c.GetEmployeesByJobRoleAndType(1,1), Times.Once);

        }
        [Fact]
        public void GetEmployeeData_Returns_OkObjectResult_When_ServiceReturnsSuccess()
        {
            // Arrange
            var mockAdminService = new Mock<IAdminService>();

            var startDate = "2024-01-01";
            string endDate = null;

            var mockSPDtos = new List<SPDto>
        {
            new SPDto { EmployeeId = 1, EmployeeName = "John Doe" },
            new SPDto { EmployeeId = 2, EmployeeName = "Jane Smith" },
        };

            var successResponse = new ServiceResponse<IEnumerable<SPDto>>
            {
                Success = true,
                Data = mockSPDtos
            };

            mockAdminService.Setup(s => s.GetEmployeeData(startDate, endDate))
                            .Returns(successResponse);

            var controller = new AdminController(mockAdminService.Object);

            // Act
            var result = controller.GetEmployeeData(startDate, endDate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseData = Assert.IsAssignableFrom<ServiceResponse<IEnumerable<SPDto>>>(okResult.Value);

            Assert.True(responseData.Success);
            Assert.Equal(mockSPDtos, responseData.Data);
        }

        [Fact]
        public void GetEmployeeData_ThrowsException()
        {
            //Arrange
            var target = new AdminController(_mockEmployeeService.Object);
            _mockEmployeeService.Setup(c => c.GetEmployeeData("01-01-2003", "01-01-2030")).Throws(new Exception());

            //Act

            var exception = Assert.Throws<Exception>(() => target.GetEmployeeData("01-01-2003", "01-01-2030"));

            //Assert


            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
            _mockEmployeeService.Verify(c => c.GetEmployeeData("01-01-2003", "01-01-2030"), Times.Once);

        }
        [Fact]
        public void UpdateEmployees_Returns_OkObjectResult_When_ServiceReturnsSuccess()
        {
            // Arrange
            var mockAdminService = new Mock<IAdminService>();

            var updateEmployeeDto = new UpdateAllocationDto
            {
                EmployeeId = 1,
            };

            var successResponse = new ServiceResponse<string>
            {
                Success = true,
                Data = "Employee allocation updated successfully."
            };

            mockAdminService.Setup(s => s.UpdateEmployee(updateEmployeeDto))
                            .Returns(successResponse);

            var controller = new AdminController(mockAdminService.Object);

            // Act
            var result = controller.UpdateEmployees(updateEmployeeDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseData = Assert.IsAssignableFrom<ServiceResponse<string>>(okResult.Value);

            Assert.True(responseData.Success);
            Assert.Equal("Employee allocation updated successfully.", responseData.Data);
        }

        [Fact]
        public void UpdateEmployees_ThrowsException()
        {
            //Arrange
            var fixture = new Fixture();
            var updateContactDto = fixture.Create<UpdateAllocationDto>();
            var response = new ServiceResponse<string>
            {
                Success = false,
            };

            var target = new AdminController(_mockEmployeeService.Object);
            _mockEmployeeService.Setup(c => c.UpdateEmployee(It.IsAny<UpdateAllocationDto>())).Throws(new Exception());

            //Ac

            var exception = Assert.Throws<Exception>(() => target.UpdateEmployees(It.IsAny<UpdateAllocationDto>()));

            //Assert


            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
            _mockEmployeeService.Verify(c => c.UpdateEmployee(It.IsAny<UpdateAllocationDto>()), Times.Once);

        }
        [Fact]
        public void GetEmployeeData_ReturnsNotFound_WhenResponseIsNotSuccessful()
        {
            // Arrange
            var mockAdminService = new Mock<IAdminService>();
            var errorResponse = new ServiceResponse<IEnumerable<SPDto>>
            {
                Success = false
            };
            mockAdminService.Setup(service => service.GetEmployeeData(It.IsAny<string>(), It.IsAny<string>()))
                            .Returns(errorResponse);

            var controller = new AdminController(mockAdminService.Object);

            // Act
            var result = controller.GetEmployeeData("2024-01-01", "2024-12-31");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var serviceResponse = Assert.IsType<ServiceResponse<IEnumerable<SPDto>>>(notFoundResult.Value);
            Assert.False(serviceResponse.Success);
        }

        [Fact]
        public void UpdateEmployees_Returns_BadRequestObjectResult_When_ServiceReturnsFailure()
        {
            // Arrange
            var mockAdminService = new Mock<IAdminService>();

            var updateEmployeeDto = new UpdateAllocationDto
            {
                EmployeeId = 1
            };

            var failureResponse = new ServiceResponse<string>
            {
                Success = false,
                Message = "Failed to update employee allocation."
            };

            mockAdminService.Setup(s => s.UpdateEmployee(updateEmployeeDto))
                            .Returns(failureResponse);

            var controller = new AdminController(mockAdminService.Object);

            // Act
            var result = controller.UpdateEmployees(updateEmployeeDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseData = Assert.IsAssignableFrom<ServiceResponse<string>>(badRequestResult.Value);

            Assert.False(responseData.Success);
            Assert.Equal("Failed to update employee allocation.", responseData.Message);
        }
        [Fact]
        public void GetTotalCountOfContacts_ReturnsOkResult_WithTotalCount()
        {
            // Arrange
            string search = "John";
            int expectedCount = 10; // Example expected count

            var mockAdminService = new Mock<IAdminService>();
            mockAdminService.Setup(service => service.TotalEmployees(search))
                            .Returns(new ServiceResponse<int> { Success = true, Data = expectedCount });

            var controller = new AdminController(mockAdminService.Object);

            // Act
            var result = controller.GetTotalCountOfContacts(search);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ServiceResponse<int>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(expectedCount, response.Data);
        }

        [Fact]
        public void GetTotalCountOfContacts_ReturnsNotFoundResult_WhenServiceFails()
        {
            // Arrange
            string search = "InvalidSearch";

            var mockAdminService = new Mock<IAdminService>();
            mockAdminService.Setup(service => service.TotalEmployees(search))
                            .Returns(new ServiceResponse<int> { Success = false, Message = "Error: No data found" });

            var controller = new AdminController(mockAdminService.Object);

            // Act
            var result = controller.GetTotalCountOfContacts(search);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ServiceResponse<int>>(notFoundResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Error: No data found", response.Message);
        }
        [Fact]
        public void GetTotalCountOfContacts_ThrowsException()
        {
            //Arrange
           
            string search = "dev";
           

            var target = new AdminController(_mockEmployeeService.Object);
            _mockEmployeeService.Setup(c => c.TotalEmployees(search)).Throws(new Exception());

            //Act

            var exception = Assert.Throws<Exception>(() => target.GetTotalCountOfContacts(search));

            //Assert


            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
            _mockEmployeeService.Verify(c => c.TotalEmployees(search), Times.Once);

        }

        public void Dispose()
        {
            _mockEmployeeService.VerifyAll();
        }

    }
}
