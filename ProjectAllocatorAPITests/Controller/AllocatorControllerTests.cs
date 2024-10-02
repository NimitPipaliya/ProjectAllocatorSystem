using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProjectAllocatorSystemAPI.Controllers;
using ProjectAllocatorSystemAPI.Dtos;
using ProjectAllocatorSystemAPI.Models;
using ProjectAllocatorSystemAPI.Service.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAllocatorAPITests.Controller
{
    public class AllocatorControllerTests:IDisposable
    {
        private readonly Mock<IAllocatorService> _adminServiceMock;
        private readonly AllocatorController _employeeController;

        public AllocatorControllerTests()
        {
            // Arrange: Create a mock for IAdminService
            _adminServiceMock = new Mock<IAllocatorService>();

            // Arrange: Create an instance of the controller with the mock service
            _employeeController = new AllocatorController(_adminServiceMock.Object);
        }
        [Fact]
        public void AddAllocation_ReturnsOk_WhenContactIsAddedSuccessfully()
        {
            var fixture = new Fixture();
            var AddAllocationDto = fixture.Create<AddEmployeeAllocationDto>();
            var response = new ServiceResponse<string>
            {
                Success = true,
            };
            var mockContactService = new Mock<IAllocatorService>();
            var target = new AllocatorController(mockContactService.Object);
            mockContactService.Setup(c => c.AddAllocation(It.IsAny<Allocation>())).Returns(response);

            //Act

            var actual = target.AddEmployee(AddAllocationDto) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockContactService.Verify(c => c.AddAllocation(It.IsAny<Allocation>()), Times.Once);

        }

        [Fact]
        public void AddAllocation_ReturnsBadRequest_WhenContactIsNotAdded()
        {
            var fixture = new Fixture();
            var AddAllocationDto = fixture.Create<AddEmployeeAllocationDto>();
            var response = new ServiceResponse<string>
            {
                Success = false,
            };
            var mockContactService = new Mock<IAllocatorService>();
            var target = new AllocatorController(mockContactService.Object);
            mockContactService.Setup(c => c.AddAllocation(It.IsAny<Allocation>())).Returns(response);

            //Act

            var actual = target.AddEmployee(AddAllocationDto) as BadRequestObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockContactService.Verify(c => c.AddAllocation(It.IsAny<Allocation>()), Times.Once);

        }

        //[Fact]
        //public void AddContact_ThrowsException()
        //{
        //    //Arrange
        //    var fixture = new Fixture();
        //    var addContactDto = fixture.Create<AddEmployeeAllocationDto>();
        //    var allocation = new Allocation() { };
        //    var response = new ServiceResponse<string>
        //    {
        //        Success = false,
        //    };
        //    var mockController = new Mock<InterviewerController>
        //    {
        //        CallBase = true // Ensures that methods not explicitly set up will still call the base implementation
        //    };

        //    var target = new AllocatorController(_adminServiceMock.Object);
        //    _adminServiceMock.Setup(c => c.AddAllocation(allocation)).Throws(new Exception());

        //    //Ac

        //    var exception = Assert.Throws<Exception>(() => target.AddEmployee(It.IsAny<AddEmployeeAllocationDto>()));

        //    //Assert


        //    Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
          

        //}
        [Fact]
        public void GetPaginatedEmployees_ReturnsOkResult_WithValidData()
        {
            // Arrange
            var search = "John"; // Example search term
            var sortBy = "Name"; // Example sorting field
            var page = 1;
            var pageSize = 4;
            var sortOrder = "asc";

            var expectedEmployees = new List<EmployeeDto>
            {
                new EmployeeDto { EmployeeId = 1, EmployeeName = "John Doe" },
                new EmployeeDto { EmployeeId = 2, EmployeeName = "Jane Smith" }
                // Add more sample employees as needed
            };

            // Set up the mock service to return the expected data
            _adminServiceMock.Setup(s => s.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy))
                .Returns(new ServiceResponse<IEnumerable<EmployeeDto>>
                {
                    Success = true,
                    Data = expectedEmployees
                });

            // Act
            var result = _employeeController.GetPaginatedEmployees(search, sortBy, page, pageSize, sortOrder);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Add more assertions based on your specific requirements
        }
        [Fact]
        public void GetPaginatedEmployees_ReturnsNotFoundResult_OnFailure()
        {
            // Arrange
            var search = "InvalidSearchTerm";
            var sortBy = "Name";
            var page = 1;
            var pageSize = 4;
            var sortOrder = "asc";
            _adminServiceMock.Setup(s => s.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy))
                .Returns(new ServiceResponse<IEnumerable<EmployeeDto>>
                {
                    Success = false
                });

            // Act
            var result = _employeeController.GetPaginatedEmployees(search, sortBy, page, pageSize, sortOrder);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            
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

            var target = new AllocatorController(_adminServiceMock.Object);
            _adminServiceMock.Setup(c => c.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy)).Throws(new Exception());

            //Act

            var exception = Assert.Throws<Exception>(() => target.GetPaginatedEmployees(search, sortBy, page, pageSize, sortOrder));

            //Assert


            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
            _adminServiceMock.Verify(c => c.GetPaginatedEmployees(page, pageSize, search, sortOrder, sortBy), Times.Once);

        }
        public void Dispose()
        {
            _adminServiceMock.VerifyAll();
        }
    }
}
