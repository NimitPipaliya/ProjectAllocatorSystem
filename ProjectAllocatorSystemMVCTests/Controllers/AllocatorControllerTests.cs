using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using ProjectAllocatorSystemMVC.Controllers;
using ProjectAllocatorSystemMVC.Infrastructure;
using ProjectAllocatorSystemMVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAllocatorSystemMVCTests.Controllers
{
    public class AllocatorControllerTests
    {
        private readonly Mock<IHttpClientService> _mockHttpClientService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<HttpContext> _mockHttpContext;
        private readonly AllocatorController _controller;

        public AllocatorControllerTests()
        {
            _mockHttpClientService = new Mock<IHttpClientService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockHttpContext = new Mock<HttpContext>();

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            _controller = new AllocatorController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object
                }
            };
        }
        [Fact]
        [Trait("Allocator", "MVC")]
        public void Index_ReturnsView_WithEmployees()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var target = new AllocatorController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };
            var expectedEmployees = new List<AllocatorListViewModel>
            {
                new AllocatorListViewModel
                {EmployeeId = 1,
                EmployeeName = "employee1"
                },
            new AllocatorListViewModel{
                EmployeeId = 2,
                EmployeeName = "employee2"
               },
            };

            mockHttpClientService.Setup(x => x.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(new ServiceResponse<int>
                {
                    Success = true,
                    Data = expectedEmployees.Count
                });
            mockHttpClientService.Setup(x => x.ExecuteApiRequest<ServiceResponse<IEnumerable<AllocatorListViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
               .Returns(new ServiceResponse<IEnumerable<AllocatorListViewModel>>
               {
                   Success = true,
                   Data = expectedEmployees
               });

            // Act
            var result = target.Index("employee", "name", "asc", 1, 2) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedEmployees, result.Model);
        }
        [Fact]
        [Trait("Allocator", "MVC")]
        public void Index_RedirectsToFirstPage_WhenPageGreaterThanTotalPages()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var target = new AllocatorController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };
            var search = "employee";
            var page = 3;
            var pageSize = 10;
            var expectedRedirectToAction = new RedirectToActionResult("Index", null, new { search, page = 1, pageSize });

            mockHttpClientService.Setup(x => x.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(new ServiceResponse<int>
                {
                    Success = true,
                    Data = 20
                });

            // Act
            var result = target.Index(search, "name", "asc", page, pageSize) as RedirectToActionResult;

            // Assert
            Assert.Equal(expectedRedirectToAction.ActionName, result.ActionName);
            Assert.Equal(expectedRedirectToAction.ControllerName, result.ControllerName);
        }
        [Fact]
        [Trait("Allocator", "MVC")]
        public void Index_ReturnsList_WhenSerchisNull()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var target = new AllocatorController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };
            var page = 3;
            var pageSize = 10;
            var expectedRedirectToAction = new RedirectToActionResult("Index", null, new { page = 1, pageSize });

            mockHttpClientService.Setup(x => x.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(new ServiceResponse<int>
                {
                    Success = true,
                    Data = 20
                });

            // Act
            var result = target.Index(null, "name", "asc", page, pageSize) as RedirectToActionResult;

            // Assert
            Assert.Equal(expectedRedirectToAction.ActionName, result.ActionName);
            Assert.Equal(expectedRedirectToAction.ControllerName, result.ControllerName);
        }
        [Fact]
        [Trait("Allocator", "MVC")]
        public void Index_ReturnsList_WhenSerchisNotNullButSortByisNull()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var target = new AllocatorController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };
            var page = 3;
            var pageSize = 10;
            var expectedRedirectToAction = new RedirectToActionResult("Index", null, new { page = 1, pageSize });

            mockHttpClientService.Setup(x => x.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(new ServiceResponse<int>
                {
                    Success = true,
                    Data = 20
                });

            // Act
            var result = target.Index("employee", null, "asc", page, pageSize) as RedirectToActionResult;

            // Assert
            Assert.Equal(expectedRedirectToAction.ActionName, result.ActionName);
            Assert.Equal(expectedRedirectToAction.ControllerName, result.ControllerName);
        }
        [Fact]
        [Trait("Allocator", "MVC")]
        public void Index_ReturnsList()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var target = new AllocatorController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };
            var page = 3;
            var pageSize = 10;
            var expectedRedirectToAction = new RedirectToActionResult("Index", null, new { page = 1, pageSize });

            mockHttpClientService.Setup(x => x.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(new ServiceResponse<int>
                {
                    Success = true,
                    Data = 20
                });

            // Act
            var result = target.Index(null, null, "asc", page, pageSize) as RedirectToActionResult;

            // Assert
            Assert.Equal(expectedRedirectToAction.ActionName, result.ActionName);
            Assert.Equal(expectedRedirectToAction.ControllerName, result.ControllerName);
        }

        [Fact]
        [Trait("Allocator", "MVC")]
        public void Index_ReturnsView_EmptyBooks_WhenResponseIsSuccess()
        {
            //Arrange
            string searchString = null;
            int page = 1;
            int pageSize = 6;
            string sort_dir = "asc";

            var expectedEmployees = new List<AllocatorListViewModel>
            {
                new AllocatorListViewModel
                {EmployeeId = 1,
                EmployeeName = "emp1"},
            new AllocatorListViewModel{
               EmployeeId =2,
            EmployeeName = "emp2"},
            };

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var expectedServiceResponse = new ServiceResponse<IEnumerable<AllocatorListViewModel>>
            {
                Data = null,
                Message = "",
                Success = false
            };

            var expectedCount = new ServiceResponse<int>()
            {
                Data = expectedEmployees.Count(),
                Success = true
            };

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<AllocatorListViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), It.IsAny<Object>(), 60)).Returns(expectedServiceResponse);

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedCount);

            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new AllocatorController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },
            };

            //Act
            var actual = target.Index(searchString, null, sort_dir, page, pageSize) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<AllocatorListViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }
        [Fact]
        [Trait("Allocator", "MVC")]
        public void Index_ReturnsView_Countzero_WhenResponseIsSuccess()
        {
            //Arrange
            string searchString = null;
            int page = 1;
            int pageSize = 6;
            string sort_dir = "asc";
            var expectedEmployees = new List<AllocatorListViewModel>();

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var expectedServiceResponse = new ServiceResponse<IEnumerable<AllocatorListViewModel>>
            {
                Data = null,
                Message = "",
                Success = false
            };

            var expectedCount = new ServiceResponse<int>()
            {
                Data = 0,
                Success = true
            };

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<AllocatorListViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), It.IsAny<Object>(), 60)).Returns(expectedServiceResponse);

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedCount);

            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new AllocatorController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },
            };

            //Act
            var actual = target.Index(searchString, null, sort_dir, page, pageSize) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<AllocatorListViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Never);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }
        [Fact]
        public void Index_Handles_Exception_And_ReturnsHome()
        {
            // Arrange
            string startDate = null;
            string endDate = "2024-07-31";
            var expectedViewData = new List<EmployeeViewModel>();

            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);

            var expectedResponse = new ServiceResponse<IEnumerable<EmployeeViewModel>>
            {
                Success = true,
                Data = expectedViewData
            };
            var expectedResponse1 = new ServiceResponse<int>
            {
                Success = true,
                Data = 2,

            };
            var error = "An unexpected error occured, Please try again later !!";

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            _mockHttpClientService
                .Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(
                    It.IsAny<string>(), HttpMethod.Get, _mockHttpContext.Object.Request, null, 60)).Returns(expectedResponse1);


            _mockHttpClientService
                .Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<EmployeeViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, _mockHttpContext.Object.Request, null, 60))
                .Throws(new Exception());

            var target = new AllocatorController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };

            // Act
            var result = target.Index(null, null, "asc", 1, 2) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(error, target.TempData["ErrorMessage"]);
        }

        [Fact]
        [Trait("Allocator", "MVC")]
        public void AddAllocation_ReturnsView_WhenStatusCodeIsSuccess()
        {
            var id = 1;
            var viewModel = new AddAllocationViewModel { };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var expectedServiceResponse = new ServiceResponse<AddAllocationViewModel>
            {
                Data = viewModel,
                Success = true
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var mockHttpContext = new Mock<HttpContext>();
            var target = new AllocatorController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.AddAllocation(id) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]
        [Trait("Allocator", "MVC")]
        public void AddAllocation_RedirectsToAction_WhenStatusCodeIsNotSuccess()
        {
            var id = 1;
            var viewModel = new AddAllocationViewModel { };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new Mock<HttpContext>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<AddAllocationViewModel>
            {
                Data = viewModel,
                Success = true,
                Message = "null",
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new AllocatorController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.AddAllocation(id) as ViewResult;

            //Assert
            Assert.Null(actual);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]
        [Trait("Allocator", "MVC")]
        public void AddAllocation_ReturnsErrorMessage_WhenStatusCodeIsSuccess_ButServiceResponseNull()
        {
            var id = 1;
            var viewModel = new AddAllocationViewModel { };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new Mock<HttpContext>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<AddAllocationViewModel>
            {
                Data = viewModel,
                Success = false
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = null
            };
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new AllocatorController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.AddAllocation(id) as RedirectToActionResult;

            //Assert
            Assert.Equal("Index", actual.ActionName);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]
        [Trait("Allocator", "MVC")]
        public void AddAllocation_RedirectsToAction_WhenStatusCodeIsNotSuccess_andErrorResponseisNull()
        {
            var id = 1;
            var viewModel = new AddAllocationViewModel { };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new Mock<HttpContext>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<AddAllocationViewModel>
            {
                Data = viewModel,
                Success = true,
                Message = "null",
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = null,
            };
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new AllocatorController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.AddAllocation(id) as RedirectToActionResult;

            //Assert
            Assert.Equal("Index", actual.ActionName);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]
        [Trait("Allocator", "MVC")]
        public void AddAllocation_Handles_Exception_And_ReturnsHome()
        {
            // Arrange
            var expectedViewData = new AddAllocationViewModel();

            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);


            var error = "An unexpected error occured, Please try again later !!";

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            _mockHttpClientService
                .Setup(c => c.GetHttpResponseMessage<AddAllocationViewModel>(
                    It.IsAny<string>(), _mockHttpContext.Object.Request))
                .Throws(new Exception());

            var target = new AllocatorController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };

            // Act
            var result = target.AddAllocation(1) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(error, target.TempData["ErrorMessage"]);
        }
        [Fact]
        [Trait("Allocator", "MVC")]
        public void AddAllocationPostUpdateEmployee_Handles_Exception_And_ReturnsHome()
        {
            // Arrange
            var expectedViewData = new AddAllocationViewModel();

            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var response = new ServiceResponse<AddAllocationViewModel>
            {
                Success = true,
                Data = expectedViewData
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(response))
            };
            var error = "An unexpected error occured, Please try again later !!";

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            _mockHttpClientService
                .Setup(c => c.PostHttpResponseMessage<AddAllocationViewModel>(
                    It.IsAny<string>(),expectedViewData,_mockHttpContext.Object.Request))
                .Returns(expectedReponse);

            _mockHttpClientService
               .Setup(c => c.PutHttpResponseMessage<AddAllocationViewModel>(
                   It.IsAny<string>(), expectedViewData, _mockHttpContext.Object.Request))
               .Throws(new Exception());

            var target = new AllocatorController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };

            // Act
            var result = target.AddAllocation(expectedViewData) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(error, target.TempData["ErrorMessage"]);
        }
        [Fact]
        [Trait("Allocator", "MVC")]
        public void AddAllocationPost_Handles_Exception_And_ReturnsHome()
        {
            // Arrange
            var expectedViewData = new AddAllocationViewModel();

            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var response = new ServiceResponse<AddAllocationViewModel>
            {
                Success = true,
                Data = expectedViewData
            };
            var error = "An unexpected error occured, Please try again later !!";

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            _mockHttpClientService
                .Setup(c => c.PostHttpResponseMessage<AddAllocationViewModel>(
                    It.IsAny<string>(), expectedViewData, _mockHttpContext.Object.Request))
               .Throws(new Exception());

  

            var target = new AllocatorController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };

            // Act
            var result = target.AddAllocation(expectedViewData) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(error, target.TempData["ErrorMessage"]);
        }
        [Fact]
        [Trait("Allocator", "MVC")]
        public void SetBenchForm_ReturnsView_WhenStatusCodeIsSuccess()
        {
            var id = 1;
            var viewModel = new AddAllocationViewModel { };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var expectedServiceResponse = new ServiceResponse<AddAllocationViewModel>
            {
                Data = viewModel,
                Success = true
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var mockHttpContext = new Mock<HttpContext>();
            var target = new AllocatorController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.SetBenchForm(id) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]
        [Trait("Allocator", "MVC")]
        public void SetBenchForm_RedirectsToAction_WhenStatusCodeIsNotSuccess()
        {
            var id = 1;
            var viewModel = new AddAllocationViewModel { };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new Mock<HttpContext>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<AddAllocationViewModel>
            {
                Data = viewModel,
                Success = true,
                Message = "null",
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new AllocatorController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.SetBenchForm(id) as ViewResult;

            //Assert
            Assert.Null(actual);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]
        [Trait("Allocator", "MVC")]
        public void SetBenchForm_ReturnsErrorMessage_WhenStatusCodeIsSuccess_ButServiceResponseNull()
        {
            var id = 1;
            var viewModel = new AddAllocationViewModel { };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new Mock<HttpContext>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<AddAllocationViewModel>
            {
                Data = viewModel,
                Success = false
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = null
            };
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new AllocatorController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.SetBenchForm(id) as RedirectToActionResult;

            //Assert
            Assert.Equal("Index", actual.ActionName);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]
        [Trait("Allocator", "MVC")]
        public void SetBenchForm_RedirectsToAction_WhenStatusCodeIsNotSuccess_andErrorResponseisNull()
        {
            var id = 1;
            var viewModel = new AddAllocationViewModel { };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new Mock<HttpContext>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<AddAllocationViewModel>
            {
                Data = viewModel,
                Success = true,
                Message = "null",
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = null,
            };
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new AllocatorController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.SetBenchForm(id) as RedirectToActionResult;

            //Assert
            Assert.Equal("Index", actual.ActionName);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }

        [Fact]
        [Trait("Allocator", "MVC")]
        public void SetBenchForm_Handles_Exception_And_ReturnsHome()
        {
            // Arrange
            var expectedViewData = new AddAllocationViewModel();

            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);


            var error = "An unexpected error occured, Please try again later !!";

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            _mockHttpClientService
                .Setup(c => c.GetHttpResponseMessage<AddAllocationViewModel>(
                    It.IsAny<string>(), _mockHttpContext.Object.Request))
                .Throws(new Exception());

            var target = new AllocatorController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };

            // Act
            var result = target.SetBenchForm(1) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(error, target.TempData["ErrorMessage"]);
        }
        [Fact]
        [Trait("Allocator", "MVC")]
        public void AddAllocation_ReturnsErrorMessage_WhenDataNotAddedSuccessfully()
        {
            var viewModel = new AddAllocationViewModel
            {
                EmployeeId = 1,
                StartDate = DateTime.Now,
                EndDate = null,
                Details = "asfcfvsafsaf"
            };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new Mock<HttpContext>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<AddAllocationViewModel>
            {
                Data = null,
                Success = false,
                Message = "Allocation Failed."
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            mockHttpClientService.Setup(c => c.PostHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new AllocatorController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.AddAllocation(viewModel) as ViewResult;

            //Assert
            Assert.Equal("Allocation Failed.", target.TempData["ErrorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.PostHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]
        [Trait("Allocator", "MVC")]
        public void AddAllocation_ReturnsErrorMessage_WhenDataNotAddedSuccessfully_WhenErrorResponseIsNull()
        {
            var viewModel = new AddAllocationViewModel
            {
                EmployeeId = 1,
                StartDate = DateTime.Now,
                EndDate = null,
                Details = "asfcfvsafsaf"
            };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new Mock<HttpContext>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<AddAllocationViewModel>
            {
                Data = null,
                Success = false,
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = null
            };
            mockHttpClientService.Setup(c => c.PostHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new AllocatorController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.AddAllocation(viewModel) as ViewResult;

            //Assert
            Assert.Equal("Something went wrong, please try after sometime.", target.TempData["ErrorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.PostHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);

        }


        [Fact]
        [Trait("Allocator", "MVC")]
        public void AddAllocation_ReturnsOkResponse_WhenDataAddedSuccessfully()
        {
            //Arrange
            var viewModel = new AddAllocationViewModel
            {
                EmployeeId = 1,
                StartDate = DateTime.Now,
                EndDate = null,
                TypeId = 1
            };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new Mock<HttpContext>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<AddAllocationViewModel>
            {
                Data = viewModel,
                Success = true,
                Message = "Employee allocated successfully"
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            var expectedServiceResponse1 = new ServiceResponse<AddAllocationViewModel>
            {
                Data = viewModel,
                Success = true,
                Message = "Employee updated successfully"
            };
            var expectedReponse1 = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse1))
            };
            mockHttpClientService.Setup(c => c.PostHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse);
            mockHttpClientService.Setup(c => c.PutHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse1);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new AllocatorController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.AddAllocation(viewModel) as ViewResult;

            //Assert
            Assert.Equal(null, target.TempData["ErrorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.PostHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);
            mockHttpClientService.Verify(c => c.PutHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);
        }

        [Fact]
        [Trait("Allocator", "MVC")]
        public void AddAllocation_ReturnsErrorMessage_WhenUpdateFails()
        {
            //Arrange
            var viewModel = new AddAllocationViewModel
            {
                EmployeeId = 1,
                StartDate = DateTime.Now,
                EndDate = null,
                TypeId = 1
            };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new Mock<HttpContext>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<AddAllocationViewModel>
            {
                Data = viewModel,
                Success = true,
                Message = "Employee allocated successfully"
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            var expectedServiceResponse1 = new ServiceResponse<AddAllocationViewModel>
            {
                Data = null,
                Success = false,
                Message = "Something went wrong"
            };
            var expectedReponse1 = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse1))
            };
            mockHttpClientService.Setup(c => c.PostHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse);
            mockHttpClientService.Setup(c => c.PutHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse1);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new AllocatorController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.AddAllocation(viewModel) as ViewResult;

            //Assert
            Assert.Equal("Something went wrong", target.TempData["ErrorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.PostHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);
            mockHttpClientService.Verify(c => c.PutHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);
        }

        [Fact]
        [Trait("Allocator", "MVC")]
        public void AddAllocation_ReturnsErrorMessage_WhenUpdateFailsWithSomethingWentWrong()
        {
            //Arrange
            var viewModel = new AddAllocationViewModel
            {
                EmployeeId = 1,
                StartDate = DateTime.Now,
                EndDate = null,
                TypeId = 1
            };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new Mock<HttpContext>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<AddAllocationViewModel>
            {
                Data = viewModel,
                Success = true,
                Message = "Employee allocated successfully"
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            var expectedServiceResponse1 = new ServiceResponse<AddAllocationViewModel>
            {
                Data = null,
                Success = false,
                
            };
            var expectedReponse1 = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = null
            };
            mockHttpClientService.Setup(c => c.PostHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse);
            mockHttpClientService.Setup(c => c.PutHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse1);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new AllocatorController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.AddAllocation(viewModel) as ViewResult;

            //Assert
            Assert.Equal("Something went wrong, please try after sometime.", target.TempData["ErrorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.PostHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);
            mockHttpClientService.Verify(c => c.PutHttpResponseMessage<AddAllocationViewModel>(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);
        }
        
    }
}
