using Microsoft.AspNetCore.Mvc;
using Moq;
using ProjectAllocatorSystemAPI.Controllers;
using ProjectAllocatorSystemAPI.Dtos;
using ProjectAllocatorSystemAPI.Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAllocatorAPITests.Controller
{
    public class ManagerControllerTests:IDisposable
    {
        private readonly Mock<IManagerService> _managerServiceMock;
        private readonly ManagerController _employeeController;

        public ManagerControllerTests()
        {
            _managerServiceMock = new Mock<IManagerService>();
            _employeeController = new ManagerController(_managerServiceMock.Object);
        }
        public void Dispose()
        {
            // Clean up any disposable resources if needed
            // e.g., _managerServiceMock.Dispose();
        }
        [Fact]
        public void GetPaginatedEmployees_ReturnsOkResult_WithValidData()
        {
            // Arrange
            var searchQuery = "John";
            var sortByField = "LastName";
            var sortOrder = "asc";
            var page = 2;
            var pageSize = 10;

            var expectedEmployees = new List<EmployeeDto>
            {
                new EmployeeDto { EmployeeId = 1, EmployeeName = "John"},
                new EmployeeDto { EmployeeId = 2, EmployeeName = "John"}
                // Add more sample employees as needed
            };

            _managerServiceMock.Setup(s => s.GetPaginatedEmployees(page, pageSize, searchQuery, sortOrder, sortByField))
                .Returns(new ServiceResponse<IEnumerable<EmployeeDto>>
                {
                    Success = true,
                    Data = expectedEmployees
                });

            // Act
            var result = _employeeController.GetPaginatedEmployees(searchQuery, sortByField, page, pageSize, sortOrder);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            _managerServiceMock.Verify(
    s => s.GetPaginatedEmployees(page, pageSize, searchQuery, sortOrder, sortByField),
    Times.Once);
        }
        [Fact]
        public void GetPaginatedEmployees_ReturnsNotFound_WhenServiceFails()
        {
            // Arrange
            var searchQuery = "John";
            var sortByField = "LastName";
            var sortOrder = "asc";
            var page = 2;
            var pageSize = 10;

            // Simulate a failed service response
            _managerServiceMock.Setup(s => s.GetPaginatedEmployees(page, pageSize, searchQuery, sortOrder, sortByField))
                .Returns(new ServiceResponse<IEnumerable<EmployeeDto>>
                {
                    Success = false,
                });

            // Act
            var result = _employeeController.GetPaginatedEmployees(searchQuery, sortByField, page, pageSize, sortOrder);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var actualResponse = Assert.IsType<ServiceResponse<IEnumerable<EmployeeDto>>>(notFoundResult.Value);
            Assert.False(actualResponse.Success);
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

            var target = new ManagerController(_managerServiceMock.Object);
            _managerServiceMock.Setup(c => c.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy)).Throws(new Exception());

            //Act

            var exception = Assert.Throws<Exception>(() => target.GetPaginatedEmployees(search, sortBy, page, pageSize, sortOrder));

            //Assert


            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
            _managerServiceMock.Verify(c => c.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy), Times.Once);

        }
        [Fact]
        public void GetEmployeeById_ReturnsOk_WhenEmployeeFound()
        {
            // Arrange
            var validId = 42; // A valid employee ID
            var expectedEmployee = new AllocationDto { EmployeeId = validId, Details = "John"};

            _managerServiceMock.Setup(s => s.GetAllocationByEmpId(validId))
                .Returns(new ServiceResponse<AllocationDto>
                {
                    Success = true,
                    Data = expectedEmployee
                });

            // Act
            var result = _employeeController.GetEmployeeById(validId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public void GetEmployeeById_ReturnsBadRequest_WhenIdIsInvalid()
        {
            // Arrange
            var invalidId = -1; // An invalid ID (less than or equal to 0)

            // Act
            var result = _employeeController.GetEmployeeById(invalidId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Please enter valid data.", badRequestResult.Value);
        }

        [Fact]
        public void GetEmployeeById_ThrowsException()
        {
            //Arrange
           

            var target = new ManagerController(_managerServiceMock.Object);
            _managerServiceMock.Setup(c => c.GetAllocationByEmpId(1)).Throws(new Exception());

            //Act

            var exception = Assert.Throws<Exception>(() => target.GetEmployeeById(1));

            //Assert


            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
            _managerServiceMock.Verify(c => c.GetAllocationByEmpId(1), Times.Once);

        }
        [Fact]
        public void GetEmployeeCounts_ReturnsOkResponse()
        {
            //Arrange

            var response = new ServiceResponse<int>
            {
                Success = true,
                Data = 2
            };

            var mockManagerService = new Mock<IManagerService>();
            var target = new ManagerController(mockManagerService.Object);
            mockManagerService.Setup(c => c.TotalEmployees(null)).Returns(response);

            //Act
            var actual = target.GetTotalCountOfContacts(null) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockManagerService.Verify(c => c.TotalEmployees(null), Times.Once);
        }

        [Fact]
        public void GetEmployeesCounts_ReturnsNotFoundResponse()
        {
            //Arrange

            var response = new ServiceResponse<int>
            {
                Success = false,
            };

            var mockManagerService = new Mock<IManagerService>();
            var target = new ManagerController(mockManagerService.Object);
            mockManagerService.Setup(c => c.TotalEmployees(null)).Returns(response);

            //Act
            var actual = target.GetTotalCountOfContacts(null) as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockManagerService.Verify(c => c.TotalEmployees(null), Times.Once);
        }

        [Fact]
        public void GetTotalCountOfContacts_ThrowsException()
        {
            //Arrange


            var target = new ManagerController(_managerServiceMock.Object);
            _managerServiceMock.Setup(c => c.TotalEmployees("abc")).Throws(new Exception());

            //Act

            var exception = Assert.Throws<Exception>(() => target.GetTotalCountOfContacts("abc"));

            //Assert


            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
            _managerServiceMock.Verify(c => c.TotalEmployees("abc"), Times.Once);

        }
    }
}
