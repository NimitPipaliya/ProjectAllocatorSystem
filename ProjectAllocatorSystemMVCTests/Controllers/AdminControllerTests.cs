using Microsoft.AspNetCore.Hosting;
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
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace ProjectAllocatorSystemMVCTests.Controllers
{
    public class AdminControllerTests:IDisposable
    {
        private readonly Mock<IHttpClientService> _mockHttpClientService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<HttpContext> _mockHttpContext;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _mockHttpClientService = new Mock<IHttpClientService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockHttpContext = new Mock<HttpContext>();

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            _controller = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
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
            var target = new AdminController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };
            var expectedEmployees = new List<EmployeeViewModel>
            {
                new EmployeeViewModel
                {EmployeeId = 1,
                EmployeeName = "employee1"
                },
            new EmployeeViewModel{
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
            mockHttpClientService.Setup(x => x.ExecuteApiRequest<ServiceResponse<IEnumerable<EmployeeViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
               .Returns(new ServiceResponse<IEnumerable<EmployeeViewModel>>
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
            var target = new AdminController(mockHttpClientService.Object, mockConfiguration.Object)
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
            var target = new AdminController(mockHttpClientService.Object, mockConfiguration.Object)
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
            var target = new AdminController(mockHttpClientService.Object, mockConfiguration.Object)
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
            var target = new AdminController(mockHttpClientService.Object, mockConfiguration.Object)
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

            var expectedEmployees = new List<EmployeeViewModel>
            {
                new EmployeeViewModel
                {EmployeeId = 1,
                EmployeeName = "emp1"},
            new EmployeeViewModel{
               EmployeeId =2,
            EmployeeName = "emp2"},
            };

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var expectedServiceResponse = new ServiceResponse<IEnumerable<EmployeeViewModel>>
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
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<EmployeeViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), It.IsAny<Object>(), 60)).Returns(expectedServiceResponse);

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedCount);

            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new AdminController(mockHttpClientService.Object, mockConfiguration.Object)
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
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<EmployeeViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
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
            var expectedEmployees = new List<EmployeeViewModel>();

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var expectedServiceResponse = new ServiceResponse<IEnumerable<EmployeeViewModel>>
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
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<EmployeeViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), It.IsAny<Object>(), 60)).Returns(expectedServiceResponse);

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedCount);

            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new AdminController(mockHttpClientService.Object, mockConfiguration.Object)
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
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<EmployeeViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Never);
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

            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object
                },
            };

            // Act
            var result = target.Index(null,null,"asc",1,2) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(error, target.TempData["ErrorMessage"]);
        }
        [Fact]
        public void Details_WhenSuccessResponseIsOK_ReturnView_WhenDataIsReceived()
        {
            // Arrange
            var id = 1;
            var expectedContacts = new ServiceResponse<EmployeeViewModel>
            {
                Success = true,
                Data = new EmployeeViewModel { EmployeeId = 2, EmployeeName = "Fname 2", EmailId = "test@email.com" }
            };

            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedContacts))
            };
            _mockHttpClientService.Setup(c => c.GetHttpResponseMessage<EmployeeViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()))
                .Returns(expectedResponse);

            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };

            //Act
            var actual = target.Details(id) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
        }
        [Fact]
        public void Details_WhenSuccessResponseIsOK_WhenDataIsNullAndSuccessIsTrue()
        {
            // Arrange
            var id = 1;
            var errorMessage = "Data is not found";
            var expectedServiceResponse = new ServiceResponse<EmployeeViewModel>
            {
                Success = true,
                Message = errorMessage,
                Data = null
            };
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            _mockHttpClientService.Setup(c => c.GetHttpResponseMessage<EmployeeViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()))
                .Returns(expectedResponse);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };

            //Act
            var actual = target.Details(id) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal(errorMessage, target.TempData["ErrorMessage"]);
            _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
            _mockHttpClientService.Verify(c => c.GetHttpResponseMessage<EmployeeViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);
        }


        [Fact]
        public void Details_ReturnsRedirectWithErrorMessage_WhenServiceResponseIsNotSuccessful()
        {
            // Arrange
            int phoneId = 1;
            var errorMessage = "An error occurred while processing the request.";

            var expectedErrorResponseContent = new ServiceResponse<EmployeeViewModel>
            {
                Success = false,
                Message = errorMessage
            };

            var expectedErrorResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedErrorResponseContent))
            };
            var mockTempData = new TempDataDictionary(_mockHttpContext.Object, Mock.Of<ITempDataProvider>());
            _mockHttpClientService.Setup(c => c.GetHttpResponseMessage<EmployeeViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedErrorResponse);
            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object },
                TempData = mockTempData
            };

            // Act
            var result = target.Details(phoneId) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal(errorMessage, mockTempData["ErrorMessage"]);
            _mockHttpClientService.Verify(c => c.GetHttpResponseMessage<EmployeeViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);
        }


        [Fact]
        public void Details_ReturnsRedirectWithErrorMessage_WhenServiceResponseIsNull()
        {
            // Arrange
            int phoneId = 1;
            var errorMessage = "Something went wrong.Please try after sometime.";

            var expectedErrorResponse = new HttpResponseMessage(HttpStatusCode.NotFound);
            var mockTempData = new TempDataDictionary(_mockHttpContext.Object, Mock.Of<ITempDataProvider>());
            _mockHttpClientService.Setup(c => c.GetHttpResponseMessage<EmployeeViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedErrorResponse);
            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object },
                TempData = mockTempData
            };

            // Act
            var result = target.Details(phoneId) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal(errorMessage, mockTempData["ErrorMessage"]);
            _mockHttpClientService.Verify(c => c.GetHttpResponseMessage<EmployeeViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);
        }
        [Fact]
        public void Details_Handles_Exception_And_ReturnsHome()
        {
            // Arrange
            var expectedViewData = new List<EmployeeViewModel>();

            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);

            var expectedResponse = new ServiceResponse<IEnumerable<EmployeeViewModel>>
            {
                Success = true,
                Data = expectedViewData
            };
            var error = "An unexpected error occured, Please try again later !!";

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            _mockHttpClientService
                .Setup(c => c.GetHttpResponseMessage<EmployeeViewModel>(
                    It.IsAny<string>(), _mockHttpContext.Object.Request))
                .Throws(new Exception());

            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };

            // Act
            var result = target.Details(1) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(error, target.TempData["ErrorMessage"]);
        }
        [Fact]
        public void Edit_ReturnsView_WhenStatusCodeIsSuccess()
        {
            var id = 1;
            var viewModel = new UpdateEmployeeViewModel { EmployeeId = 1, EmployeeName = "Category 1" };
            var expectedServiceResponse = new ServiceResponse<UpdateEmployeeViewModel>
            {
                Data = viewModel,
                Success = true
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            _mockHttpClientService.Setup(c => c.GetHttpResponseMessage<UpdateEmployeeViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object
                },

            };
            //Act
            var actual = target.Edit(id) as ViewResult;

            //Assert
            var model = actual.Model as UpdateEmployeeViewModel;
            Assert.NotNull(model);
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
            _mockHttpClientService.Verify(c => c.GetHttpResponseMessage<UpdateEmployeeViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);


        }
        [Fact]
        public void Edit_ReturnsErrorMessage_WhenStatusCodeIsSuccess()
        {
            // Arrange
            var id = 1;



            var mockHttpContext = new Mock<HttpContext>();
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var errorMessage = "Failed to update";
            var expectedServiceResponse = new ServiceResponse<UpdateEmployeeViewModel>
            {
                Data = null,
                Success = false,
                Message = errorMessage
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            _mockHttpClientService.Setup(c => c.GetHttpResponseMessage<UpdateEmployeeViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);

            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            // Act
            var result = target.Edit(id) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal(errorMessage, target.TempData["ErrorMessage"]);
            _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
        }
        [Fact]
        public void Edit_ReturnsErrorMessage_WhenStatusCodeIsNotSuccess()
        {
            // Arrange
            var id = 1;
            var mockTempDataProvider = new Mock<ITempDataProvider>();


            var errorMessage = "Failed to update";
            var expectedServiceResponse = new ServiceResponse<UpdateEmployeeViewModel>
            {
                Data = null,
                Success = false,
                Message = errorMessage
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            _mockHttpClientService.Setup(c => c.GetHttpResponseMessage<UpdateEmployeeViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);

            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };

            // Act
            var result = target.Edit(id) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal(errorMessage, target.TempData["ErrorMessage"]);
            _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
        }
        [Fact]
        public void Edit_ReturnsErrorMessage_WhenErrorResponseIsNull()
        {
            // Arrange
            var id = 1;
            var mockTempDataProvider = new Mock<ITempDataProvider>();



            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            _mockHttpClientService.Setup(c => c.GetHttpResponseMessage<UpdateEmployeeViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);

            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };

            // Act
            var result = target.Edit(id) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Something went wrong please try after some time.", target.TempData["ErrorMessage"]);
            _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
        }
        [Fact]
        public void Edit_ContactSavedSuccessfully_RedirectToAction()
        {
            //Arrange
            var id = 1;
            var viewModel = new UpdateEmployeeViewModel { EmployeeId = id, EmployeeName = "F1" };
            var successMessage = "Product saved successfully";
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Message = successMessage
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            _mockHttpClientService.Setup(c => c.PutHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object
                },

            };
            //Act
            var actual = target.Edit(viewModel) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal(successMessage, target.TempData["successMessage"]);
            _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
            _mockHttpClientService.Verify(c => c.PutHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);

        }




        [Fact]
        public void Edit_ContactFailedToSaveServiceResponseNull_RedirectToAction()
        {
            //Arrange
            var id = 1;
            var viewModel = new UpdateEmployeeViewModel { EmployeeId = id, EmployeeName = "C1" };

            var successMessage = "Contact saved successfully";
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Message = successMessage
            };

            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            _mockHttpClientService.Setup(c => c.PutHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object
                },

            };
            //Act
            var actual = target.Edit(viewModel) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal(successMessage, target.TempData["successMessage"]);
            _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
            _mockHttpClientService.Verify(c => c.PutHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);

        }

        [Fact]
        public void Edit_ContactFailed_WhenModelStateIsInvalid()
        {
            //Arrange
            var viewModel = new UpdateEmployeeViewModel()
            {
                EmployeeId = 1,
                EmployeeName = "C1",

            };
            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                }
            };
            target.ModelState.AddModelError("LastName", "Last name is required.");

            //Act
            var actual = target.Edit(viewModel) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            Assert.False(target.ModelState.IsValid);

        }
        [Fact]
        public void Edit_ContactFailed_WhenModelStateIsInvalid_WhenStateCountryAreNull()
        {
            //Arrange
            var mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            var viewModel = new UpdateEmployeeViewModel()
            {
                EmployeeId = 1,
                EmployeeName = "C1",

            };
            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                }
            };
            target.ModelState.AddModelError("LastName", "Last name is required.");

            //Act
            var actual = target.Edit(viewModel) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            Assert.False(target.ModelState.IsValid);
        }

        [Fact]
        public void Edit_ContactFailedToSave_ReturnRedirectToActionResult()
        {
            //Arrange
            var viewModel = new UpdateEmployeeViewModel { EmployeeName = "C1" };

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var errorMessage = "";
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Message = errorMessage
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            _mockHttpClientService.Setup(c => c.PutHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object
                },

            };
            //Act
            var actual = target.Edit(viewModel) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal(errorMessage, target.TempData["errorMessage"]);
            _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
            _mockHttpClientService.Verify(c => c.PutHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);

        }

        [Fact]
        public void Edit_ReturnsSomethingWentWrong_ReturnRedirectToActionResult()
        {
            //Arrange
            var mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            var viewModel = new UpdateEmployeeViewModel { EmployeeName = "C1" };

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var errorMessage = "Something went wrong. Please try after sometime.";
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(null))
            };
            _mockHttpClientService.Setup(c => c.PutHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object
                },

            };
            //Act
            var actual = target.Edit(viewModel) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal(errorMessage, target.TempData["errorMessage"]);
            _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
            _mockHttpClientService.Verify(c => c.PutHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);


        }
        [Fact]
        public void Edit_Handles_Exception_And_ReturnsHome()
        {
            // Arrange
            var expectedViewData = new UpdateEmployeeViewModel();

            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);

        
            var error = "An unexpected error occured, Please try again later !!";

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            _mockHttpClientService
                .Setup(c => c.PutHttpResponseMessage<UpdateEmployeeViewModel>(
                    It.IsAny<string>(),expectedViewData,_mockHttpContext.Object.Request))
                .Throws(new Exception());

            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };

            // Act
            var result = target.Edit(expectedViewData) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(error, target.TempData["ErrorMessage"]);
        }
        [Fact]
        public void EditGet_Handles_Exception_And_ReturnsHome()
        {
            // Arrange
            var expectedViewData = new UpdateEmployeeViewModel();

            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);


            var error = "An unexpected error occured, Please try again later !!";

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            _mockHttpClientService
                .Setup(c => c.GetHttpResponseMessage<UpdateEmployeeViewModel>(
                    It.IsAny<string>(),_mockHttpContext.Object.Request))
                .Throws(new Exception());

            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };

            // Act
            var result = target.Edit(1) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(error, target.TempData["ErrorMessage"]);
        }
        [Fact]
        public void AddEmployee_Returns_View_With_Correct_ViewModel_And_SkillsSuggestions()
        {
            // Arrange
            var controller = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object);

            // Act
            var result = controller.AddEmployee() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<AddEmployeeViewModel>(result.Model);

            var viewModel = result.Model as AddEmployeeViewModel;
            Assert.NotNull(viewModel);
        }
        [Fact]
        public void Create_ReturnsView()
        {
            // Arrange
            var mockHttpRequest = new Mock<HttpRequest>();
            var _mockHttpContext=new Mock<HttpContext>();
            _mockHttpContext.Setup(c => c.Request).Returns(mockHttpRequest.Object);
            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };
            //Act
            var actual = target.AddEmployee() as ViewResult;

            //Assert
            Assert.NotNull(actual);
            _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
        }
        [Fact]
        public void Create_ReturnsView_WithNullData()
        {

            //Arrange
            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };
            //Act
            var actual = target.AddEmployee() as ViewResult;

            //Assert
            Assert.NotNull(actual);
            _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
        }
        [Fact]
        public void Create_RedirectToActionResult_WhenContactSavedSuccessfully()
        {
            //Arrange
            var viewModel = new AddEmployeeViewModel { EmployeeName = "FirstName 1" };

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("FakeEndPoint");
            var successMessage = "Contact Saved Successfully";
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Message = successMessage,
            };
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            _mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedResponse);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object); ;
            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object) {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                }
            };

            //Act
            var actual = target.AddEmployee(viewModel) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal("Contact Saved Successfully", target.TempData["successMessage"]);
            _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
            _mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);
        }
        [Fact]
        public void Create_ReturnsViewResultWithErrorMessage_WhenResponseIsNotSuccess()
        {
            //Arrange
            var viewModel = new AddEmployeeViewModel { EmployeeName = "FirstName 1" };
            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("FakeEndPoint");
            var errorMessage = "Error Occured";
            var expectedErrorResponse = new ServiceResponse<string>
            {
                Message = errorMessage,
            };
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedErrorResponse))
            };

            _mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedResponse);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                }
            };

            //Act
            var actual = target.AddEmployee(viewModel) as ViewResult;

            //Assert
            Assert.True(target.ModelState.IsValid);

            Assert.Equal(errorMessage, target.TempData["errorMessage"]);
            _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
            _mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);
        }
        [Fact]
        public void Create_ReturnsViewResultWithErrorMessage_WhenErrorResponseIsNull()
        {
            //Arrange
            var viewModel = new AddEmployeeViewModel { EmployeeName = "FirstName 1" };

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("FakeEndPoint");
            var errorMessage = "Something went wrong try after some time";
            var expectedErrorResponse = new ServiceResponse<string>
            { };
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(null))
            };
            var mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedResponse);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                }
            };

            //Act
            var actual = target.AddEmployee(viewModel) as ViewResult;

            //Assert
            Assert.True(target.ModelState.IsValid);
            _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
            _mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);
        }
        [Fact]
        public void Create_ReturnsRedirectToActionResult_WhenResponseIsNotSuccess()
        {
            //Arrange

            var viewModel = new AddEmployeeViewModel { EmployeeName = "FirstName 1" };
            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("FakeEndPoint");
            var errorMessage = "Something went wrong try after some time";
            var expectedErrorResponse = new ServiceResponse<string>
            {
                Message = errorMessage,
            };
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedErrorResponse))
            };
            _mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedResponse);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                }
            };

            //Act
            var actual = target.AddEmployee(viewModel) as ViewResult;

            //Assert
            Assert.True(target.ModelState.IsValid);
            Assert.Equal(errorMessage, target.TempData["ErrorMessage"]);
            _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
            _mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);
        }
        [Fact]
        public void Create_ReturnsViewResult_WithContactList_WhenModelStateIsInvalid()
        {
            //Arrange

            var viewModel = new AddEmployeeViewModel { EmployeeName = "firstname" };
            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("FakeEndPoint");
            var mockHttpRequest = new Mock<HttpRequest>();
            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                }
            };
            target.ModelState.AddModelError("LastName", "Last name is required.");

            //Act
            var actual = target.AddEmployee(viewModel) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(viewModel, actual.Model);
            Assert.False(target.ModelState.IsValid);
            _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2)); }
        [Fact]
        public void Create_ReturnsViewResult_WithEmptyCountryandStateList_WhenModelStateIsInvalid()
        {
            //Arrange
            var viewModel = new AddEmployeeViewModel { EmployeeName = "firstname" };
            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("FakeEndPoint");
            var mockHttpRequest = new Mock<HttpRequest>();
            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                }
            };
            target.ModelState.AddModelError("LastName", "Last name is required.");

            //Act
            var actual = target.AddEmployee(viewModel) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(viewModel, actual.Model);
            Assert.False(target.ModelState.IsValid);
            _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2)); }

        [Fact]
        public void Create_RedirectToActionResult_WhenContactSavedSuccessfully_WhenFileISNotNull()
        {
            //Arrange

            var viewModel = new AddEmployeeViewModel { EmployeeName = "FirstName 1" };
            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("FakeEndPoint");
            var successMessage = "Contact Saved Successfully";
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Message = successMessage,
            };

            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            _mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedResponse);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                }
            };

            //Act
            var actual = target.AddEmployee(viewModel) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal("Contact Saved Successfully", target.TempData["successMessage"]);
            _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
            _mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);
        }
        [Fact]
        public void AddEmployee_Returns_View_With_Data_When_ServiceResponse_Is_Successful()
        {
            //Arrange

            var viewModel = new AddEmployeeViewModel { EmployeeName = "FirstName 1" };
            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("FakeEndPoint");
            var errorMessage = "Something went wrong try after some time";
            var expectedErrorResponse = new ServiceResponse<AddEmployeeViewModel>
            {
                Success = true,
                Data = viewModel,
                Message = "Employee added successfully."
            };

            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedErrorResponse))
            };
            _mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedResponse);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                }
            };

            //Act
            var actual = target.AddEmployee(viewModel) as ViewResult;

            //Assert
            Assert.True(target.ModelState.IsValid);
            _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
            _mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);
        }
        [Fact]
        public void AddEmployee_Handles_Exception_And_ReturnsHome()
        {
            // Arrange
            var expectedViewData = new AddEmployeeViewModel();

            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);


            var error = "An unexpected error occured, Please try again later !!";

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            _mockHttpClientService
                .Setup(c => c.PostHttpResponseMessage<AddEmployeeViewModel>(
                    It.IsAny<string>(), expectedViewData, _mockHttpContext.Object.Request))
                .Throws(new Exception());

            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };

            // Act
            var result = target.AddEmployee(expectedViewData) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(error, target.TempData["ErrorMessage"]);
        }
        [Fact]
        public void GetEmployeesByJobRoleAndType_ReturnsEmptyView_WhenJobRoleIdIsNull()
        {
            // Arrange
            int? jobRoleId = null;
            var expectedViewData = new List<EmployeeViewModel>();
            var jobRoles = new List<JobRoleViewModel>
        {
            new JobRoleViewModel { JobRoleId =1, JobRoleName = "C1"},
            new JobRoleViewModel { JobRoleId =2, JobRoleName = "C2"},
         };


            var expectedResponse = new ServiceResponse<IEnumerable<EmployeeViewModel>>
            {
                Success = true,
                Data = expectedViewData
            };

            var expectedResponseJobRoles = new ServiceResponse<IEnumerable<JobRoleViewModel>>
            {
                Success = true,
                Data = jobRoles
            };


            var _mockHttpClientService = new Mock<IHttpClientService>();
            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");


            _mockHttpClientService
              .Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<EmployeeViewModel>>>(It.IsAny<string>(), HttpMethod.Get, _mockHttpContext.Object.Request, null, 60))
              .Returns(expectedResponse);
            _mockHttpClientService
            .Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<JobRoleViewModel>>>(It.IsAny<string>(), HttpMethod.Get, _mockHttpContext.Object.Request, null, 60))
            .Returns(expectedResponseJobRoles);
            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };

            // Act
            var result = target.GetEmployeesByJobRoleAndType(jobRoleId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            _mockHttpClientService
             .Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<JobRoleViewModel>>>(It.IsAny<string>(), HttpMethod.Get, _mockHttpContext.Object.Request, null, 60), Times.Once);
        }
        [Fact]
        public void GetEmployeesByJobRoleAndType_Returns_View_With_Data_When_JobRoleId_Is_Not_Null_And_Successful_API_Call()
        {
            // Arrange
            int? jobRoleId = 1;
            var expectedViewData = new List<EmployeeViewModel>
            {
                new EmployeeViewModel { EmployeeId = 1, EmployeeName = "John Doe" },
                new EmployeeViewModel { EmployeeId = 2, EmployeeName = "Jane Smith" }
                // Add more mock data as needed
            };
            var jobRoles = new List<JobRoleViewModel>
            {
                new JobRoleViewModel { JobRoleId = 1, JobRoleName = "C1" },
                new JobRoleViewModel { JobRoleId = 2, JobRoleName = "C2" }
                // Add more mock job roles as needed
            };

            var expectedResponse = new ServiceResponse<IEnumerable<EmployeeViewModel>>
            {
                Success = true,
                Data = expectedViewData
            };

            var expectedResponseJobRoles = new ServiceResponse<IEnumerable<JobRoleViewModel>>
            {
                Success = true,
                Data = jobRoles
            };

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            _mockHttpClientService
                .Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<EmployeeViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, _mockHttpContext.Object.Request, null, 60))
                .Returns(expectedResponse);

            _mockHttpClientService
                .Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<JobRoleViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, _mockHttpContext.Object.Request, null, 60))
                .Returns(expectedResponseJobRoles);

            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };

            // Act
            var result = target.GetEmployeesByJobRoleAndType(jobRoleId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<IEnumerable<EmployeeViewModel>>(result.Model);
            Assert.Equal(expectedViewData, result.Model);
        }
        [Fact]
        public void GetEmployeesByJobRoleAndType_Returns_Empty_View_When_JobRoleId_Is_Not_Null_And_API_Call_Fails()
        {
            // Arrange
            int? jobRoleId = 1;
            var jobRoles = new List<JobRoleViewModel>
            {
                new JobRoleViewModel { JobRoleId = 1, JobRoleName = "C1" },
                new JobRoleViewModel { JobRoleId = 2, JobRoleName = "C2" }
                // Add more mock job roles as needed
            };

            var expectedResponseJobRoles = new ServiceResponse<IEnumerable<JobRoleViewModel>>
            {
                Success = true,
                Data = jobRoles
            };

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            _mockHttpClientService
                .Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<EmployeeViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, _mockHttpContext.Object.Request, null, 60))
                .Returns(new ServiceResponse<IEnumerable<EmployeeViewModel>>
                {
                    Success = false,
                    Message = "Failed to fetch employees by job role."
                });

            _mockHttpClientService
                .Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<JobRoleViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, _mockHttpContext.Object.Request, null, 60))
                .Returns(expectedResponseJobRoles);

            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };

            // Act
            var result = target.GetEmployeesByJobRoleAndType(jobRoleId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
            Assert.Empty(result.Model as IEnumerable<EmployeeViewModel>);
        }
        [Fact]
        public void GetEmployeesByJobRoleAndType_Handles_Exception_And_Returns_Empty_View()
        {
            // Arrange
            int? jobRoleId = 1; // Example jobRoleId
            var expectedViewData = new List<EmployeeViewModel>
            {
                new EmployeeViewModel { EmployeeId = 1, EmployeeName = "John Doe" },
                new EmployeeViewModel { EmployeeId = 2, EmployeeName = "Jane Smith" }
                // Add more mock data as needed
            };
            var jobRoles = new List<JobRoleViewModel>
            {
                new JobRoleViewModel { JobRoleId = 1, JobRoleName = "John Doe" },
                new JobRoleViewModel { JobRoleId = 2, JobRoleName = "Jane Smith" }
                // Add more mock data as needed
            };
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);

            var expectedResponse = new ServiceResponse<IEnumerable<EmployeeViewModel>>
            {
                Success = true,
                Data = expectedViewData
            };
            var expectedResponseJobRoles = new ServiceResponse<IEnumerable<JobRoleViewModel>>
            {
                Success = true,
                Data = jobRoles
            };
            var error = "An unexpected error occured, Please try again later !!";

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            _mockHttpClientService
                .Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<EmployeeViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, _mockHttpContext.Object.Request, null, 60))
                .Throws(new Exception());
            _mockHttpClientService
                .Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<JobRoleViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, _mockHttpContext.Object.Request, null, 60))
                .Returns(expectedResponseJobRoles);

            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData=tempData,
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };

            // Act
            var result = target.GetEmployeesByJobRoleAndType(jobRoleId) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(error, target.TempData["ErrorMessage"]);
        }
        [Fact]
        public void GetEmployeeData_ReturnsEmptyView_WhenStartDateIsNull()
        {
            // Arrange
            string startDate = "2024-07-25";
            string endDate = "2024-07-31";
            var expectedViewData = new List<spViewModel>
            {
                new spViewModel { EmployeeId = 1, EmployeeName = "John Doe" },
                new spViewModel { EmployeeId = 2, EmployeeName = "Jane Smith" }
                // Add more mock data as needed
            };
            var jobRoles = new List<JobRoleViewModel>
        {
            new JobRoleViewModel { JobRoleId =1, JobRoleName = "C1"},
            new JobRoleViewModel { JobRoleId =2, JobRoleName = "C2"},
         };


            var expectedResponse = new ServiceResponse<IEnumerable<spViewModel>>
            {
                Success = true,
                Data = expectedViewData
            };

            var expectedResponseJobRoles = new ServiceResponse<IEnumerable<JobRoleViewModel>>
            {
                Success = true,
                Data = jobRoles
            };



            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");


            _mockHttpClientService
              .Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<spViewModel>>>(It.IsAny<string>(), HttpMethod.Get, _mockHttpContext.Object.Request, null, 60))
              .Returns(expectedResponse);
            _mockHttpClientService
            .Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<JobRoleViewModel>>>(It.IsAny<string>(), HttpMethod.Get, _mockHttpContext.Object.Request, null, 60))
            .Returns(expectedResponseJobRoles);
            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };

            // Act
            var result = target.GetEmployeeData(startDate,endDate) as ViewResult;

            // Assert
            Assert.NotNull(result);
            _mockHttpClientService
             .Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<JobRoleViewModel>>>(It.IsAny<string>(), HttpMethod.Get, _mockHttpContext.Object.Request, null, 60), Times.Once);
        }
        [Fact]
        public void GetEmployeeData_Returns_View_With_Data_When_StartDate_Is_Not_Null_And_Successful_API_Call()
        {
            // Arrange
            string startDate = "2024-07-25";
            string endDate = "2024-07-31";
            var expectedViewData = new List<spViewModel>
            {
                new spViewModel { EmployeeId = 1, EmployeeName = "John Doe" },
                new spViewModel { EmployeeId = 2, EmployeeName = "Jane Smith" }
                // Add more mock data as needed
            };
            
            var jobRoles = new List<JobRoleViewModel>
            {
                new JobRoleViewModel { JobRoleId = 1, JobRoleName = "C1" },
                new JobRoleViewModel { JobRoleId = 2, JobRoleName = "C2" }
                // Add more mock job roles as needed
            };

            var expectedResponse = new ServiceResponse<IEnumerable<spViewModel>>
            {
                Success = true,
                Data = expectedViewData
            };

            var expectedResponseJobRoles = new ServiceResponse<IEnumerable<JobRoleViewModel>>
            {
                Success = true,
                Data = jobRoles
            };

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            _mockHttpClientService
                .Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<spViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, _mockHttpContext.Object.Request, null, 60))
                .Returns(expectedResponse);

            _mockHttpClientService
                .Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<JobRoleViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, _mockHttpContext.Object.Request, null, 60))
                .Returns(expectedResponseJobRoles);

            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };

            // Act
            var result = target.GetEmployeeData(startDate, endDate) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<IEnumerable<spViewModel>>(result.Model);
            Assert.Equal(expectedViewData, result.Model);
        }
        [Fact]
        public void GetEmployeeData_Returns_Empty_View_When_startDate_Is_Not_Null_And_API_Call_Fails()
        {
            // Arrange
            string startDate = "2024-07-25";
            string endDate = "2024-07-31";
            var expectedViewData = new List<spViewModel>
            {
                new spViewModel { EmployeeId = 1, EmployeeName = "John Doe" },
                new spViewModel { EmployeeId = 2, EmployeeName = "Jane Smith" }
                // Add more mock data as needed
            };
            var jobRoles = new List<JobRoleViewModel>
            {
                new JobRoleViewModel { JobRoleId = 1, JobRoleName = "C1" },
                new JobRoleViewModel { JobRoleId = 2, JobRoleName = "C2" }
                // Add more mock job roles as needed
            };

            var expectedResponseJobRoles = new ServiceResponse<IEnumerable<JobRoleViewModel>>
            {
                Success = true,
                Data = jobRoles
            };

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            _mockHttpClientService
                .Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<spViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, _mockHttpContext.Object.Request, null, 60))
                .Returns(new ServiceResponse<IEnumerable<spViewModel>>
                {
                    Success = false,
                    Message = "Failed to fetch employees by job role."
                });

            _mockHttpClientService
                .Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<JobRoleViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, _mockHttpContext.Object.Request, null, 60))
                .Returns(expectedResponseJobRoles);

            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };

            // Act
            var result = target.GetEmployeeData(startDate,endDate) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
            Assert.Empty(result.Model as IEnumerable<spViewModel>);
        }
        [Fact]
        public void GetEmployeeData_Returns_View_With_Data_When_StartDate_Is_Null_And_Successful_API_Call()
        {
            // Arrange
            string startDate = null;
            string endDate = "2024-07-31";
            var expectedViewData = new List<spViewModel>
            {
                new spViewModel { EmployeeId = 1, EmployeeName = "John Doe" },
                new spViewModel { EmployeeId = 2, EmployeeName = "Jane Smith" }
                // Add more mock data as needed
            };

            var jobRoles = new List<JobRoleViewModel>
            {
                new JobRoleViewModel { JobRoleId = 1, JobRoleName = "C1" },
                new JobRoleViewModel { JobRoleId = 2, JobRoleName = "C2" }
                // Add more mock job roles as needed
            };

            var expectedResponse = new ServiceResponse<IEnumerable<spViewModel>>
            {
                Success = true,
                Data = expectedViewData
            };

            var expectedResponseJobRoles = new ServiceResponse<IEnumerable<JobRoleViewModel>>
            {
                Success = true,
                Data = jobRoles
            };

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            _mockHttpClientService
                .Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<spViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, _mockHttpContext.Object.Request, null, 60))
                .Returns(expectedResponse);

            _mockHttpClientService
                .Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<JobRoleViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, _mockHttpContext.Object.Request, null, 60))
                .Returns(expectedResponseJobRoles);

            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };

            // Act
            var result = target.GetEmployeeData(startDate, endDate) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<IEnumerable<spViewModel>>(result.Model);
            Assert.Equal(expectedViewData, result.Model);
        }
        [Fact]
        public void GetEmployeeData_Returns_Empty_View_When_startDate_Is_Null_And_API_Call_Fails()
        {
            // Arrange
            string startDate = null;
            string endDate = "2024-07-31";
            var expectedViewData = new List<spViewModel>
            {
                new spViewModel { EmployeeId = 1, EmployeeName = "John Doe" },
                new spViewModel { EmployeeId = 2, EmployeeName = "Jane Smith" }
                // Add more mock data as needed
            };
            var jobRoles = new List<JobRoleViewModel>
            {
                new JobRoleViewModel { JobRoleId = 1, JobRoleName = "C1" },
                new JobRoleViewModel { JobRoleId = 2, JobRoleName = "C2" }
                // Add more mock job roles as needed
            };

            var expectedResponseJobRoles = new ServiceResponse<IEnumerable<JobRoleViewModel>>
            {
                Success = true,
                Data = jobRoles
            };

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            _mockHttpClientService
                .Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<spViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, _mockHttpContext.Object.Request, null, 60))
                .Returns(new ServiceResponse<IEnumerable<spViewModel>>
                {
                    Success = false,
                    Message = "Failed to fetch employees by job role."
                });

            _mockHttpClientService
                .Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<JobRoleViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, _mockHttpContext.Object.Request, null, 60))
                .Returns(expectedResponseJobRoles);

            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };

            // Act
            var result = target.GetEmployeeData(startDate, endDate) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
            Assert.Empty(result.Model as IEnumerable<spViewModel>);
        }
        [Fact]
        public void GetEmployeeData_Handles_Exception_And_Returns_Empty_View()
        {
            // Arrange
            string startDate = null;
            string endDate = "2024-07-31";
            var expectedViewData = new List<spViewModel>();
            var jobRoles = new List<JobRoleViewModel>
            {
                new JobRoleViewModel { JobRoleId = 1, JobRoleName = "John Doe" },
                new JobRoleViewModel { JobRoleId = 2, JobRoleName = "Jane Smith" }
                // Add more mock data as needed
            };
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);

            var expectedResponse = new ServiceResponse<IEnumerable<spViewModel>>
            {
                Success = true,
                Data = expectedViewData
            };
            var expectedResponseJobRoles = new ServiceResponse<IEnumerable<JobRoleViewModel>>
            {
                Success = true,
                Data = jobRoles
            };
            var error = "An unexpected error occured, Please try again later !!";

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            _mockHttpClientService
                .Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<spViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, _mockHttpContext.Object.Request, null, 60))
                .Throws(new Exception());
            _mockHttpClientService
                .Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<JobRoleViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, _mockHttpContext.Object.Request, null, 60))
                .Returns(expectedResponseJobRoles);

            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };

            // Act
            var result = target.GetEmployeeData(startDate,endDate) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(error, target.TempData["ErrorMessage"]);
        }
        [Fact]
        public void Delete_Returns_RedirectToIndex_On_Success()
        {
            // Arrange
            int id = 1; // Example id
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var expectedResponse = new ServiceResponse<string>
            {
                Success = true,
                Message = "Employee deleted successfully."
            };
            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            _mockHttpClientService
                           .Setup(c => c.ExecuteApiRequest<ServiceResponse<string>>(
                    It.IsAny<string>(), HttpMethod.Delete, _mockHttpContext.Object.Request, null, 60))
                .Returns(expectedResponse);

            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object
                }
            };

            // Act
            var result = target.Delete(id) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Employee deleted successfully.", target.TempData["SuccessMessage"]);
        }
        [Fact]
        public void Delete_Returns_RedirectToIndex_On_Failed()
        {
            // Arrange
            int id = 1; // Example id
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var expectedResponse = new ServiceResponse<string>
            {
                Success = false,
                Message = "Something went wrong."
            };
            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            _mockHttpClientService
                           .Setup(c => c.ExecuteApiRequest<ServiceResponse<string>>(
                    It.IsAny<string>(), HttpMethod.Delete, _mockHttpContext.Object.Request, null, 60))
                .Returns(expectedResponse);

            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object
                }
            };

            // Act
            var result = target.Delete(id) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Something went wrong.", target.TempData["ErrorMessage"]);
        }
        [Fact]
        public void Delete_Handles_Exception_And_ReturnsHome()
        {
            // Arrange
            var expectedViewData = new List<EmployeeViewModel>();

            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var error = "An unexpected error occured, Please try again later !!";

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            _mockHttpClientService
              .Setup(c => c.ExecuteApiRequest<ServiceResponse<string>>(
                  It.IsAny<string>(), HttpMethod.Delete, _mockHttpContext.Object.Request, null, 60)).Throws(new Exception());


            var target = new AdminController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };

            // Act
            var result = target.Delete(1) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(error, target.TempData["ErrorMessage"]);
        }
        public void Dispose()
        {
            _mockHttpContext.VerifyAll();
            _mockConfiguration.VerifyAll();
            _mockHttpClientService.VerifyAll();
        }
    }
}


  