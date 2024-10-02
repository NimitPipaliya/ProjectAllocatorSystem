using Castle.Core.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Newtonsoft.Json;
using ProjectAllocatorSystemMVC.Controllers;
using ProjectAllocatorSystemMVC.Implementation;
using ProjectAllocatorSystemMVC.Infrastructure;
using ProjectAllocatorSystemMVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace ProjectAllocatorSystemMVCTests.Controllers
{
    public class ManagerControllerTests:IDisposable
    {
        private readonly Mock<IHttpClientService> _mockHttpClientService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<HttpContext> _mockHttpContext;
        private readonly ManagerController _controller;

        public ManagerControllerTests()
        {
            _mockHttpClientService = new Mock<IHttpClientService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockHttpContext = new Mock<HttpContext>();

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            _controller = new ManagerController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object
                }
            };
        }

        [Fact]
        [Trait("Manager","MVC")]
        public void AllocateTask_ReturnsView_WhenStatusCodeIsSuccess()
        {
            var id = 1;
            var viewModel = new ManagerAllocationViewModel {  };
            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<ManagerAllocationViewModel>
            {
                Data = viewModel,
                Success = true
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            _mockHttpClientService.Setup(c => c.GetHttpResponseMessage<ManagerAllocationViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var target = new ManagerController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object
                },

            };
            //Act
            var actual = target.AllocateTask(id) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
            _mockHttpClientService.Verify(c => c.GetHttpResponseMessage<ManagerAllocationViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]
        [Trait("Manager", "MVC")]
        public void AllocateTask_RedirectsToAction_WhenStatusCodeIsNotSuccess()
        {
            var id = 1;
            var viewModel = new ManagerAllocationViewModel { };
            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var expectedServiceResponse = new ServiceResponse<ManagerAllocationViewModel>
            {
                Data = viewModel,
                Success = true,
                Message = "null",
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            _mockHttpClientService.Setup(c => c.GetHttpResponseMessage<ManagerAllocationViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new ManagerController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.AllocateTask(id) as ViewResult;

            //Assert
            Assert.Null(actual);
           _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
            _mockHttpClientService.Verify(c => c.GetHttpResponseMessage<ManagerAllocationViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]
        [Trait("Manager", "MVC")]
        public void AllocateTask_ReturnsErrorMessage_WhenStatusCodeIsSuccess_ButServiceResponseNull()
        {
            var id = 1;
            var viewModel = new ManagerAllocationViewModel { };
            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<ManagerAllocationViewModel>
            {
                Data = viewModel,
                Success = false
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content =null
            };
            _mockHttpClientService.Setup(c => c.GetHttpResponseMessage<ManagerAllocationViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new ManagerController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object
                },

            };
            //Act
            var actual = target.AllocateTask(id) as RedirectToActionResult;

            //Assert
            Assert.Equal("Index", actual.ActionName);
           _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
            _mockHttpClientService.Verify(c => c.GetHttpResponseMessage<ManagerAllocationViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]
        [Trait("Manager", "MVC")]
        public void AllocateTask_RedirectsToAction_WhenStatusCodeIsNotSuccess_andErrorResponseisNull()
        {
            var id = 1;
            var viewModel = new ManagerAllocationViewModel { };
            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var expectedServiceResponse = new ServiceResponse<ManagerAllocationViewModel>
            {
                Data = viewModel,
                Success = true,
                Message = "null",
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = null,
            };
            _mockHttpClientService.Setup(c => c.GetHttpResponseMessage<ManagerAllocationViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new ManagerController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.AllocateTask(id) as RedirectToActionResult;

            //Assert
            Assert.Equal("Index",actual.ActionName);
           _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
            _mockHttpClientService.Verify(c => c.GetHttpResponseMessage<ManagerAllocationViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]
        [Trait("Manager", "MVC")]
        public void AllocateTask_ReturnsSuccessMessage_WhenDataAddedSuccessfully()
        {
            var viewModel = new ManagerAllocationViewModel
            {
                EmployeeId = 1,
                StartDate = DateTime.Now,
                EndDate=null,
                InternalProjectId = 1,
                TrainingId=2,
                TypeId=1,
                Details="asfcfvsafsaf"              
            };
            
           
            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var expectedServiceResponse = new ServiceResponse<ManagerAllocationViewModel>
            {
                Data = viewModel,
                Success = true,
                Message="Allocation Added Successfully."
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            _mockHttpClientService.Setup(c => c.PostHttpResponseMessage<ManagerAllocationViewModel>(It.IsAny<string>(),viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new ManagerController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.AllocateTask(viewModel) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("Allocation Added Successfully.", target.TempData["SuccessMessage"]);
            Assert.Equal("Index", actual.ActionName);
           _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
            _mockHttpClientService.Verify(c => c.PostHttpResponseMessage<ManagerAllocationViewModel>(It.IsAny<string>(),viewModel,It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]
        [Trait("Manager", "MVC")]
        public void AllocateTask_ReturnsErrorMessage_WhenDataNotAddedSuccessfully()
        {
            var viewModel = new ManagerAllocationViewModel
            {
                EmployeeId = 1,
                StartDate = DateTime.Now,
                EndDate = null,
                InternalProjectId = 1,
                TrainingId = 2,
                TypeId = 1,
                Details = "asfcfvsafsaf"
            };

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<ManagerAllocationViewModel>
            {
                Data = null,
                Success = false,
                Message = "Allocation Failed."
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            _mockHttpClientService.Setup(c => c.PostHttpResponseMessage<ManagerAllocationViewModel>(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new ManagerController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object
                },

            };
            //Act
            var actual = target.AllocateTask(viewModel) as ViewResult;

            //Assert
            Assert.Equal("Allocation Failed.", target.TempData["ErrorMessage"]);
           _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
            _mockHttpClientService.Verify(c => c.PostHttpResponseMessage<ManagerAllocationViewModel>(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]
        [Trait("Manager", "MVC")]
        public void AllocateTask_ReturnsErrorMessage_WhenDataNotAddedSuccessfully_WhenErrorResponseIsNull()
        {
            var viewModel = new ManagerAllocationViewModel
            {
                EmployeeId = 1,
                StartDate = DateTime.Now,
                EndDate = null,
                InternalProjectId = 1,
                TrainingId = 2,
                TypeId = 1,
                Details = "asfcfvsafsaf"
            };

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<ManagerAllocationViewModel>
            {
                Data = null,
                Success = false,
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = null
            };
            _mockHttpClientService.Setup(c => c.PostHttpResponseMessage<ManagerAllocationViewModel>(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new ManagerController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object
                },

            };
            //Act
            var actual = target.AllocateTask(viewModel) as ViewResult;

            //Assert
            Assert.Equal("Something went wrong. Please try after sometime", target.TempData["ErrorMessage"]);
           _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
            _mockHttpClientService.Verify(c => c.PostHttpResponseMessage<ManagerAllocationViewModel>(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]
        [Trait("Manager", "MVC")]
        public void AllocateTask_Handles_Exception_And_ReturnsHome()
        {
            // Arrange
            var expectedViewData = new ManagerAllocationViewModel();

            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);


            var error = "An unexpected error occured, Please try again later !!";

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            _mockHttpClientService
                .Setup(c => c.GetHttpResponseMessage<ManagerAllocationViewModel>(
                    It.IsAny<string>(), _mockHttpContext.Object.Request))
                .Throws(new Exception());

            var target = new ManagerController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };

            // Act
            var result = target.AllocateTask(1) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(error, target.TempData["ErrorMessage"]);
        }
        [Fact]
        [Trait("Manager", "MVC")]
        public void AllocateTaskPost_Handles_Exception_And_ReturnsHome()
        {
            // Arrange
            var expectedViewData = new ManagerAllocationViewModel();

            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var response = new ServiceResponse<ManagerAllocationViewModel>
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
               .Setup(c => c.PostHttpResponseMessage<ManagerAllocationViewModel>(
                   It.IsAny<string>(), expectedViewData, _mockHttpContext.Object.Request))
               .Throws(new Exception());

            var target = new ManagerController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };

            // Act
            var result = target.AllocateTask(expectedViewData) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(error, target.TempData["ErrorMessage"]);
        }
        [Fact]
        [Trait("Manager", "MVC")]
        public void EmployeeDetails_ReturnsView_WhenStatusCodeIsSuccess()
        {
            var id = 1;
            var viewModel = new EmployeeManagerDetails { };
            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<EmployeeManagerDetails>
            {
                Data = viewModel,
                Success = true
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            _mockHttpClientService.Setup(c => c.GetHttpResponseMessage<EmployeeManagerDetails>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var target = new ManagerController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object
                },

            };
            //Act
            var actual = target.EmployeeDetails(id) as ViewResult;

            //Assert
            Assert.NotNull(actual);
           _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
            _mockHttpClientService.Verify(c => c.GetHttpResponseMessage<EmployeeManagerDetails>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]
        [Trait("Manager", "MVC")]
        public void EmployeeDetails_RedirectsToAction_WhenStatusCodeIsNotSuccess()
        {
            var id = 1;
            var viewModel = new EmployeeManagerDetails { };
            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var expectedServiceResponse = new ServiceResponse<EmployeeManagerDetails>
            {
                Data = viewModel,
                Success = true,
                Message = "null",
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            _mockHttpClientService.Setup(c => c.GetHttpResponseMessage<EmployeeManagerDetails>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new ManagerController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.EmployeeDetails(id) as ViewResult;

            //Assert
            Assert.Null(actual);
           _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
            _mockHttpClientService.Verify(c => c.GetHttpResponseMessage<EmployeeManagerDetails>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]
        [Trait("Manager", "MVC")]
        public void EmployeeDetails_ReturnsErrorMessage_WhenStatusCodeIsSuccess_ButServiceResponseNull()
        {
            var id = 1;
            var viewModel = new EmployeeManagerDetails { };
            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<EmployeeManagerDetails>
            {
                Data = viewModel,
                Success = false
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = null
            };
            _mockHttpClientService.Setup(c => c.GetHttpResponseMessage<EmployeeManagerDetails>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new ManagerController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object
                },

            };
            //Act
            var actual = target.EmployeeDetails(id) as RedirectToActionResult;

            //Assert
            Assert.Equal("Index", actual.ActionName);
           _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
            _mockHttpClientService.Verify(c => c.GetHttpResponseMessage<EmployeeManagerDetails>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]
        [Trait("Manager", "MVC")]
        public void EmployeeDetails_RedirectsToAction_WhenStatusCodeIsNotSuccess_andErrorResponseisNull()
        {
            var id = 1;
            var viewModel = new EmployeeManagerDetails { };
            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var expectedServiceResponse = new ServiceResponse<EmployeeManagerDetails>
            {
                Data = viewModel,
                Success = true,
                Message = "null",
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = null,
            };
            _mockHttpClientService.Setup(c => c.GetHttpResponseMessage<EmployeeManagerDetails>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new ManagerController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.EmployeeDetails(id) as RedirectToActionResult;

            //Assert
            Assert.Equal("Index", actual.ActionName);
           _mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
            _mockHttpClientService.Verify(c => c.GetHttpResponseMessage<EmployeeManagerDetails>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]
        [Trait("Manager", "MVC")]
        public void EmployeeDetails_Handles_Exception_And_ReturnsHome()
        {
            // Arrange
            var expectedViewData = new EmployeeManagerDetails();

            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);


            var error = "An unexpected error occured, Please try again later !!";

            _mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            _mockHttpClientService
                .Setup(c => c.GetHttpResponseMessage<EmployeeManagerDetails>(
                    It.IsAny<string>(), _mockHttpContext.Object.Request))
                .Throws(new Exception());

            var target = new ManagerController(_mockHttpClientService.Object, _mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = _mockHttpContext.Object,
                },
            };

            // Act
            var result = target.EmployeeDetails(1) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(error, target.TempData["ErrorMessage"]);
        }

        [Fact]
        [Trait("Manager", "MVC")]
        public void Index_ReturnsView_WithEmployees()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var target = new ManagerController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };
            var expectedEmployees = new List<ManagerListViewModel>
            {
                new ManagerListViewModel
                {EmployeeId = 1,
                EmployeeName = "employee1"
                },
            new ManagerListViewModel{
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
            mockHttpClientService.Setup(x => x.ExecuteApiRequest<ServiceResponse<IEnumerable<ManagerListViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
               .Returns(new ServiceResponse<IEnumerable<ManagerListViewModel>>
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
        [Trait("Manager", "MVC")]
        public void Index_RedirectsToFirstPage_WhenPageGreaterThanTotalPages()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var target = new ManagerController(mockHttpClientService.Object, mockConfiguration.Object)
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
        [Trait("Manager", "MVC")]
        public void Index_ReturnsList_WhenSerchisNull()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var target = new ManagerController(mockHttpClientService.Object, mockConfiguration.Object)
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
        [Trait("Manager", "MVC")]
        public void Index_ReturnsList_WhenSerchisNotNullButSortByisNull()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var target = new ManagerController(mockHttpClientService.Object, mockConfiguration.Object)
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
        [Trait("Manager", "MVC")]
        public void Index_ReturnsList()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var target = new ManagerController(mockHttpClientService.Object, mockConfiguration.Object)
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
        [Trait("Manager", "MVC")]
        public void Index_ReturnsView_EmptyBooks_WhenResponseIsSuccess()
        {
            //Arrange
            string searchString = null;
            int page = 1;
            int pageSize = 6;
            string sort_dir = "asc";

            var expectedEmployees = new List<ManagerListViewModel>
            {
                new ManagerListViewModel
                {EmployeeId = 1,
                EmployeeName = "emp1"},
            new ManagerListViewModel{
               EmployeeId =2,
            EmployeeName = "emp2"},
            };

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var expectedServiceResponse = new ServiceResponse<IEnumerable<ManagerListViewModel>>
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
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<ManagerListViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), It.IsAny<Object>(), 60)).Returns(expectedServiceResponse);

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedCount);

            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new ManagerController(mockHttpClientService.Object, mockConfiguration.Object)
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
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<ManagerListViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }
        [Fact]
        [Trait("Manager", "MVC")]
        public void Index_ReturnsView_Countzero_WhenResponseIsSuccess()
        {
            //Arrange
            string searchString = null;
            int page = 1;
            int pageSize = 6;
            string sort_dir = "asc";
            var expectedEmployees = new List<ManagerListViewModel>();

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var expectedServiceResponse = new ServiceResponse<IEnumerable<ManagerListViewModel>>
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
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<ManagerListViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), It.IsAny<Object>(), 60)).Returns(expectedServiceResponse);

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedCount);

            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new ManagerController(mockHttpClientService.Object, mockConfiguration.Object)
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
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<ManagerListViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Never);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }
        [Fact]
        [Trait("Manager", "MVC")]
        public void Index_Handles_Exception_And_ReturnsHome()
        {
            // Arrange
            string startDate = null;
            string endDate = "2024-07-31";
            var expectedViewData = new List<ManagerListViewModel>();

            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);

            var expectedResponse = new ServiceResponse<IEnumerable<ManagerListViewModel>>
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
                .Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<ManagerListViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, _mockHttpContext.Object.Request, null, 60))
                .Throws(new Exception());

            var target = new ManagerController(_mockHttpClientService.Object, _mockConfiguration.Object)
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
        public void Dispose()
        {
            _mockHttpClientService.VerifyAll();
            _mockConfiguration.VerifyAll();
            _mockHttpContext.VerifyAll();
        }
    }
}
