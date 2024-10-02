using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using ProjectAllocatorSystemMVC.Controllers;
using ProjectAllocatorSystemMVC.Infrastructure;
using ProjectAllocatorSystemMVC.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using Xunit.Sdk;
using ControllerContext = Microsoft.AspNetCore.Mvc.ControllerContext;
using ViewResult = Microsoft.AspNetCore.Mvc.ViewResult;

namespace ProjectAllocatorSystemMVCTests.Controllers
{
    public class AuthControllerTests : IDisposable
    {
        private readonly Mock<IHttpClientService> _httpClientService;
        private readonly Mock<IConfiguration> _configuration;
        private readonly Mock<IJwtTokenHandler> _tokenHandler;
        private readonly Mock<HttpContext> _httpContext;

        public AuthControllerTests()
        {
            _httpClientService = new Mock<IHttpClientService>();
            _configuration = new Mock<IConfiguration>();
            _tokenHandler = new Mock<IJwtTokenHandler>();
            _httpContext = new Mock<HttpContext>();
        }

        // LoginUser --------------------------------------------------

        [Fact]
        public void LoginUser_ReturnsViews()
        {
            // Arrange

            var target = new AuthController(_httpClientService.Object, _configuration.Object, _tokenHandler.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext.Object,
                },
            };
            //Act
            var result = target.LoginUser() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Login_ModelIsInvalid()
        {
            // Arrange
            var loginViewModel = new LoginViewModel
            { Password = "Password@123" };
            var target = new AuthController(_httpClientService.Object, _configuration.Object, _tokenHandler.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext.Object,
                },
            };
            target.ModelState.AddModelError("UserName", "Username is required");
            //Act
            var actual = target.LoginUser(loginViewModel) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(loginViewModel, actual.Model);
            Assert.False(target.ModelState.IsValid);
        }

        [Fact]
        public void Login_ReturnView_WhenBadRequest()
        {
            // Arrange
            var loginViewModel = new LoginViewModel
            { Password = "Password@123", Username = "loginid" };

            var errorMessage = "Error Occurs";
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = false,
                Message = errorMessage
            };
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            _httpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), loginViewModel, It.IsAny<HttpRequest>()))
               .Returns(expectedResponse);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AuthController(_httpClientService.Object, _configuration.Object, _tokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext.Object,
                },
            };

            //Act
            var actual = target.LoginUser(loginViewModel) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(errorMessage, target.TempData["ErrorMessage"]);
            _httpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), loginViewModel, It.IsAny<HttpRequest>()), Times.Once);
            Assert.True(target.ModelState.IsValid);
        }
        [Fact]
        public void Login_Handles_Exception_And_ReturnsHome()
        {
            // Arrange
            var loginViewModel = new LoginViewModel
            { Password = "Password@123", Username = "loginid" };

            var errorMessage = "Error Occurs";
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = false,
                Message = errorMessage
            };

            var error = "An unexpected error occured, Please try again later !!";
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            _httpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), loginViewModel, It.IsAny<HttpRequest>()))
               .Throws(new Exception());
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AuthController(_httpClientService.Object, _configuration.Object, _tokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext.Object,
                },
            };

            //Act
            var result = target.LoginUser(loginViewModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(error, target.TempData["ErrorMessage"]);
        }

        [Fact]
        public void Login_Success_RedirectToAction()
        {
            // Arrange
            var loginViewModel = new LoginViewModel { Username = "loginid", Password = "Password" };
            var mockToken = "mockToken";

            var mockResponseCookie = new Mock<IResponseCookies>();
            mockResponseCookie.Setup(c => c.Append("jwtToken", mockToken, It.IsAny<CookieOptions>()));

            var mockHttpResponse = new Mock<HttpResponse>();
            _httpContext.SetupGet(c => c.Response).Returns(mockHttpResponse.Object);
            mockHttpResponse.SetupGet(c => c.Cookies).Returns(mockResponseCookie.Object);

            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = true,
                Data = mockToken,
            };

            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };

            _httpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), loginViewModel, It.IsAny<HttpRequest>()))
                                 .Returns(expectedResponse);


            var claims = new Claim[]
            {
                
            };
            var jwtToken = new JwtSecurityToken(claims: claims);
            _tokenHandler.Setup(t => t.ReadJwtToken(mockToken)).Returns(jwtToken);


            var target = new AuthController(_httpClientService.Object, _configuration.Object, _tokenHandler.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext.Object,
                },
            };

            // Act
            var result = target.LoginUser(loginViewModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            mockResponseCookie.Verify(c => c.Append("jwtToken", mockToken, It.IsAny<CookieOptions>()), Times.Once);
            Assert.True(target.ModelState.IsValid);

            _httpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), loginViewModel, It.IsAny<HttpRequest>()), Times.Once);
        }

        [Fact]
        public void Login_RedirectToAction_WhenBadRequest_WhenResponseIsNull()
        {
            // Arrange
            var loginViewModel = new LoginViewModel
            { Password = "Password@123", Username = "loginid" };
            _configuration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var expectedResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(JsonConvert.SerializeObject(null))
            };
            _httpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), loginViewModel, It.IsAny<HttpRequest>()))
               .Returns(expectedResponse);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var mockToken = new Mock<IJwtTokenHandler>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AuthController(_httpClientService.Object, _configuration.Object, mockToken.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext.Object,
                },
            };
            //Act
            var actual = target.LoginUser(loginViewModel) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("Something went wrong, Please try after sometime.", target.TempData["ErrorMessage"]);
            _configuration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            _httpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), loginViewModel, It.IsAny<HttpRequest>()), Times.Once);
            Assert.True(target.ModelState.IsValid);
        }

        //Logout --------------------------------------------------

        [Fact]
        public void Logout_RedirectsToAction_WhenLogoutSuccessful()
        {
            // Arrange
            var mockHttpContext = new DefaultHttpContext();
            _configuration.Setup(c => c["EndPoint:CivicaApi"]).Returns("endPoint");

            var target = new AuthController(_httpClientService.Object, _configuration.Object, _tokenHandler.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext,
                }
            };

            // Act
            var actual = target.Logout() as RedirectToActionResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal("Home", actual.ControllerName);
            _configuration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
        }

        //RegisterSuccess  --------------------------------------------------

        [Fact]
        public void RegisterSuccess_ReturnsView()
        {
            // Arrange
            _configuration.Setup(c => c["EndPoint:CivicaApi"]).Returns("endPoint");

            var target = new AuthController(_httpClientService.Object, _configuration.Object, _tokenHandler.Object);

            // Act
            var actual = target.RegisterSuccess() as ViewResult;

            // Assert
            Assert.NotNull(actual);
            _configuration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
        }

        //RegisterUser --------------------------------------------------

        [Fact]
        public void RegisterGet_ReturnsView()
        {
            // Arrange
            var questions = new List<SecurityQuestionViewModel>
            {
                new SecurityQuestionViewModel()
                {
                    SecurityQuestionId = 1,
                    SecurityQuestionName = "Question 1",
                },
                new SecurityQuestionViewModel()
                {
                    SecurityQuestionId = 2,
                    SecurityQuestionName = "Question 2",
                },
            };

            var questionResponse = new ServiceResponse<IEnumerable<SecurityQuestionViewModel>>()
            {
                Data = questions,
                Success = true,
                Message = "",
            };

            var userRoles = new List<UserRoleViewModel>()
            {
                new UserRoleViewModel(){UserRoleId = 1,UserRoleName="Allocator"},
                new UserRoleViewModel(){UserRoleId = 2,UserRoleName="Manager"},
            };

            var userRoleResponse = new ServiceResponse<IEnumerable<UserRoleViewModel>>()
            {
                Data = userRoles,
                Success = true,
                Message = "",
            };
            var httpContext = new DefaultHttpContext();

            _httpClientService.Setup(o => o.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>
               (It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>())).Returns(questionResponse);

            _httpClientService.Setup(o => o.ExecuteApiRequest<ServiceResponse<IEnumerable<UserRoleViewModel >>>
               (It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>())).Returns(userRoleResponse);

            var target = new AuthController(_httpClientService.Object, _configuration.Object, _tokenHandler.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext,
                }
            };

            // Act
            var actual = target.RegisterUser() as ViewResult;

            // Assert
            Assert.NotNull(actual);
            _httpClientService.Verify(o => o.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>
              (It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>()),Times.Once);
            _httpClientService.Verify(o => o.ExecuteApiRequest<ServiceResponse<IEnumerable<UserRoleViewModel>>>
               (It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>()),Times.Once);
        }

        [Fact]
        public void RegisterPost_ReturnsView_WhenModelStateIsInvalid()
        {
            // Arrange
            var questions = new List<SecurityQuestionViewModel>
            {
                new SecurityQuestionViewModel()
                {
                    SecurityQuestionId = 1,
                    SecurityQuestionName = "Question 1",
                },
                new SecurityQuestionViewModel()
                {
                    SecurityQuestionId = 2,
                    SecurityQuestionName = "Question 2",
                },
            };

            var questionResponse = new ServiceResponse<IEnumerable<SecurityQuestionViewModel>>()
            {
                Data = questions,
                Success = true,
                Message = "",
            };

            var userRoles = new List<UserRoleViewModel>()
            {
                new UserRoleViewModel(){UserRoleId = 1,UserRoleName="Allocator"},
                new UserRoleViewModel(){UserRoleId = 2,UserRoleName="Manager"},
            };

            var userRoleResponse = new ServiceResponse<IEnumerable<UserRoleViewModel>>()
            {
                Data = userRoles,
                Success = true,
                Message = "",
            };
            var httpContext = new DefaultHttpContext();

            _httpClientService.Setup(o => o.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>
               (It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>())).Returns(questionResponse);

            _httpClientService.Setup(o => o.ExecuteApiRequest<ServiceResponse<IEnumerable<UserRoleViewModel>>>
               (It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>())).Returns(userRoleResponse);
            var viewModel = new RegisterViewModel()
            {
                LoginId = "temp",
            };


            var target = new AuthController(_httpClientService.Object, _configuration.Object, _tokenHandler.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext,
                }
            };

            target.ModelState.AddModelError("Password", "Password is required");

            // Act
            var actual = target.RegisterUser(viewModel) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(viewModel, actual.Model);
            Assert.False(target.ModelState.IsValid);
            _httpClientService.Verify(o => o.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>
              (It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>()), Times.Once);
            _httpClientService.Verify(o => o.ExecuteApiRequest<ServiceResponse<IEnumerable<UserRoleViewModel>>>
               (It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void RegisterPost_SetsErrorMessage_WhenErrorResponseIsNull()
        {
            // Arrange
            var errorMessage = "Something went wrong please try after some time.";
            var questions = new List<SecurityQuestionViewModel>
            {
                new SecurityQuestionViewModel()
                {
                    SecurityQuestionId = 1,
                    SecurityQuestionName = "Question 1",
                },
                new SecurityQuestionViewModel()
                {
                    SecurityQuestionId = 2,
                    SecurityQuestionName = "Question 2",
                },
            };

            var questionResponse = new ServiceResponse<IEnumerable<SecurityQuestionViewModel>>()
            {
                Data = questions,
                Success = true,
                Message = "",
            };

            var userRoles = new List<UserRoleViewModel>()
            {
                new UserRoleViewModel(){UserRoleId = 1,UserRoleName="Allocator"},
                new UserRoleViewModel(){UserRoleId = 2,UserRoleName="Manager"},
            };

            var userRoleResponse = new ServiceResponse<IEnumerable<UserRoleViewModel>>()
            {
                Data = userRoles,
                Success = true,
                Message = "",
            };

            _httpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
               .Returns(questionResponse);
            _httpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<UserRoleViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
               .Returns(userRoleResponse);

            var viewModel = new RegisterViewModel
            {
                LoginId = "temp",
                Password = "pass",
            };

            var httpContext = new DefaultHttpContext();
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);

            var expectedResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            _httpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedResponse);
            _configuration.Setup(c => c["EndPoint:CivicaApi"]).Returns("endPoint");
            var target = new AuthController(_httpClientService.Object, _configuration.Object, _tokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext.Object,
                }
            };

            // Act
            var actual = target.RegisterUser(viewModel) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal(errorMessage, target.TempData["ErrorMessage"]);
            _configuration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            _httpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);
            _httpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
            _httpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<UserRoleViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60),Times.Once);
        }

        [Fact]
        public void RegisterUser_ErrorResponseNotNull()
        {
            // Arrange
            var viewModel = new RegisterViewModel();
            var errorMessage = "Something went wrong please try after some time.";
            var questions = new List<SecurityQuestionViewModel>
            {
                new SecurityQuestionViewModel()
                {
                    SecurityQuestionId = 1,
                    SecurityQuestionName = "Question 1",
                },
                new SecurityQuestionViewModel()
                {
                    SecurityQuestionId = 2,
                    SecurityQuestionName = "Question 2",
                },
            };

            var questionResponse = new ServiceResponse<IEnumerable<SecurityQuestionViewModel>>()
            {
                Data = questions,
                Success = true,
                Message = "",
            };

            var userRoles = new List<UserRoleViewModel>()
            {
                new UserRoleViewModel(){UserRoleId = 1,UserRoleName="Allocator"},
                new UserRoleViewModel(){UserRoleId = 2,UserRoleName="Manager"},
            };

            var userRoleResponse = new ServiceResponse<IEnumerable<UserRoleViewModel>>()
            {
                Data = userRoles,
                Success = true,
                Message = "",
            };

            var mockHttpContext = new Mock<HttpContext>();
            _httpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
               .Returns(questionResponse);
            _httpClientService.Setup(o => o.ExecuteApiRequest<ServiceResponse<IEnumerable<UserRoleViewModel>>>
              (It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), It.IsAny<object>(), It.IsAny<int>())).Returns(userRoleResponse);
            var mockErrorResponse = new ServiceResponse<string>
            {
                Success = false,
                Message = "User already exists."
            };
            var mockFailureResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(mockErrorResponse))
            };
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("endPoint");
            var target = new AuthController(_httpClientService.Object, _configuration.Object, _tokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                }
            };
            _httpClientService.Setup(c => c.PostHttpResponseMessage(
                It.IsAny<string>(), It.IsAny<RegisterViewModel>(), It.IsAny<HttpRequest>()))
                .Returns(mockFailureResponse);

            // Act
            var result = target.RegisterUser(viewModel) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("User already exists.", target.TempData["ErrorMessage"]);
        }

        [Fact]
        public void RegisterUser_RedirectToRegisterSuccess_WhenUserSavedSuccessfully()
        {
            // Arrange
            var registerViewModel = new RegisterViewModel
            { Name = "firstname", Password = "Password@123", Email = "email@gmail.com", ConfirmPassword = "Password@123", Phone = "1234567890", LoginId = "loginid" };
            _configuration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var successMessage = "User saved successfully";
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = true,
                Message = successMessage
            };
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            _httpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), registerViewModel, It.IsAny<HttpRequest>()))
               .Returns(expectedResponse);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AuthController(_httpClientService.Object, _configuration.Object, _tokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext.Object,
                },
            };

            //Act
            var actual = target.RegisterUser(registerViewModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("RegisterSuccess", actual.ActionName);
            Assert.Equal(successMessage, target.TempData["SuccessMessage"]);
            _configuration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            _httpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), registerViewModel, It.IsAny<HttpRequest>()), Times.Once);
            Assert.True(target.ModelState.IsValid);
        }
        [Fact]
        public void Register_Handles_Exception_And_ReturnsHome()
        {
            // Arrange
            var registerViewModel = new RegisterViewModel
            { Name = "firstname", Password = "Password@123", Email = "email@gmail.com", ConfirmPassword = "Password@123", Phone = "1234567890", LoginId = "loginid" };
            _configuration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var successMessage = "User saved successfully";
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = true,
                Message = successMessage
            };

            var error = "An unexpected error occured, Please try again later !!";

            _httpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), registerViewModel, It.IsAny<HttpRequest>()))
               .Throws(new Exception());
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AuthController(_httpClientService.Object, _configuration.Object, _tokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext.Object,
                },
            };

            //Act
            var result = target.RegisterUser(registerViewModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(error, target.TempData["ErrorMessage"]);
        }

        // ChangePassword --------------------------------------------------

        [Fact]
        public void ChangePassword_ReturnsViews()
        {
            // Arrange

            var target = new AuthController(_httpClientService.Object, _configuration.Object, _tokenHandler.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext.Object,
                },
            };
            //Act
            var result = target.ChangePassword() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        // ChnagePassword (post) -------------------------------------------

        [Fact]
        public void ChangePassword_ModelIsInvalid()
        {
            // Arrange
            var changePasswordViewModel = new ChangePasswordViewModel
            { NewPassword = "Password@123" };
            _configuration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AuthController(_httpClientService.Object, _configuration.Object, _tokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext.Object,
                },
            };
            target.ModelState.AddModelError("Old password", "Old password is required.");

            //Act
            var actual = target.ChangePassword(changePasswordViewModel) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(changePasswordViewModel, actual.Model);
            _configuration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            Assert.False(target.ModelState.IsValid);
        }

        [Fact]
        public void ChangePAssword_RedirectToAction_WhenBadRequest()
        {
            // Arrange
            var changePasswordViewModel = new ChangePasswordViewModel
            {
                OldPassword = "Oldpassword@123",
                NewPassword = "Password@123",
                ConfirmNewPassword = "Password@123"
            };
            _configuration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var errorMessage = "Error Occurs";
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = false,
                Message = errorMessage
            };
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            _httpClientService.Setup(c => c.PutHttpResponseMessage(It.IsAny<string>(), changePasswordViewModel, It.IsAny<HttpRequest>()))
               .Returns(expectedResponse);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AuthController(_httpClientService.Object, _configuration.Object, _tokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext.Object,
                },
            };

            //Act
            var actual = target.ChangePassword(changePasswordViewModel) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(errorMessage, target.TempData["ErrorMessage"]);
            _configuration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            _httpClientService.Verify(c => c.PutHttpResponseMessage(It.IsAny<string>(), changePasswordViewModel, It.IsAny<HttpRequest>()), Times.Once);
            Assert.True(target.ModelState.IsValid);
        }

        [Fact]
        public void ChangePassword_Success_RedirectToAction()
        {
            // Arrange
            var changePasswordViewModel = new ChangePasswordViewModel
            {
                OldPassword = "Oldpassword@123",
                NewPassword = "Password@123",
                ConfirmNewPassword = "Password@123"
            };

            _configuration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = true,

            };
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            _httpClientService.Setup(c => c.PutHttpResponseMessage(It.IsAny<string>(), changePasswordViewModel, It.IsAny<HttpRequest>()))
             .Returns(expectedResponse);
            var mockResponseCookie = new Mock<IResponseCookies>();
            mockResponseCookie.Setup(c => c.Delete("jwtToken"));
            var mockHttpResponse = new Mock<HttpResponse>();
            _httpContext.SetupGet(c => c.Response).Returns(mockHttpResponse.Object);
            mockHttpResponse.SetupGet(c => c.Cookies).Returns(mockResponseCookie.Object);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AuthController(_httpClientService.Object, _configuration.Object, _tokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext.Object,
                },
            };

            //Act
            var actual = target.ChangePassword(changePasswordViewModel) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("LoginUser", actual.ActionName);
            Assert.Equal("Auth", actual.ControllerName);
            Assert.True(target.ModelState.IsValid);
            mockResponseCookie.Verify(c => c.Delete("jwtToken"), Times.Once);
            _httpClientService.Verify(c => c.PutHttpResponseMessage(It.IsAny<string>(), changePasswordViewModel, It.IsAny<HttpRequest>()),Times.Once);
            _httpContext.VerifyGet(c => c.Response,Times.Exactly(2));
            mockResponseCookie.Verify(c => c.Delete("jwtToken"),Times.Once);
            mockHttpResponse.VerifyGet(c => c.Cookies,Times.Exactly(2));
        }

        [Fact]
        public void ChangePassword_HandelException()
        {
            // Arrange
            var changePasswordViewModel = new ChangePasswordViewModel
            {
                OldPassword = "Oldpassword@123",
                NewPassword = "Password@123",
                ConfirmNewPassword = "Password@123"
            };
            var error = "An unexpected error occured, Please try again later !!";
            _configuration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = true,

            };
            _httpClientService.Setup(c => c.PutHttpResponseMessage(It.IsAny<string>(), changePasswordViewModel, It.IsAny<HttpRequest>()))
             .Throws(new Exception());
         
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AuthController(_httpClientService.Object, _configuration.Object, _tokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext.Object,
                },
            };

            //Act
            var result = target.ChangePassword(changePasswordViewModel) as RedirectToActionResult;

            //Assert
            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(error, target.TempData["ErrorMessage"]);

        }


        [Fact]
        public void ChangePassword_RedirectToAction_WhenBadRequest_WhenResponseIsNull()
        {
            // Arrange
            var changePasswordViewModel = new ChangePasswordViewModel
            {
                OldPassword = "Oldpassword@123",
                NewPassword = "Password@123",
                ConfirmNewPassword = "Password@123"
            };
            _configuration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var expectedResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(JsonConvert.SerializeObject(null))
            };
            _httpClientService.Setup(c => c.PutHttpResponseMessage(It.IsAny<string>(), changePasswordViewModel, It.IsAny<HttpRequest>()))
               .Returns(expectedResponse);

            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AuthController(_httpClientService.Object, _configuration.Object, _tokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext.Object,
                },
            };

            //Act
            var actual = target.ChangePassword(changePasswordViewModel) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("Something went wrong please try after some time.", target.TempData["ErrorMessage"]);
            _configuration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            _httpClientService.Verify(c => c.PutHttpResponseMessage(It.IsAny<string>(), changePasswordViewModel, It.IsAny<HttpRequest>()), Times.Once);
            Assert.True(target.ModelState.IsValid);
        }


        // ForgetPassword --------------------------------------------------

        [Fact]
        public void ForgetPassword_ReturnsViews()
        {
            // Arrange
            var questions = new List<SecurityQuestionViewModel>()
            {
                new SecurityQuestionViewModel{SecurityQuestionId = 1, SecurityQuestionName = "TestQue1"},
                new SecurityQuestionViewModel{SecurityQuestionId = 2, SecurityQuestionName = "TestQue2"},
            };

            var expectedResponse = new ServiceResponse<IEnumerable<SecurityQuestionViewModel>>
            {
                Success = true,
                Data = questions
            };

            _httpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
               .Returns(expectedResponse);
            
            var target = new AuthController(_httpClientService.Object, _configuration.Object, _tokenHandler.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext.Object,
                },
            };
            //Act
            var result = target.ForgetPassword() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        // ForgetPassword (post) -------------------------------------------

        [Fact]
        public void ForgetPassword_RedirectToAction_WhenBadRequest()
        {
            // Arrange
            var viewModel = new ForgetPasswordViewModel
            { LoginId = "test" };
            
            _configuration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var errorMessage = "Error Occurs";

            var questions = new List<SecurityQuestionViewModel>()
            {
                new SecurityQuestionViewModel{SecurityQuestionId = 1, SecurityQuestionName = "TestQue1"},
                new SecurityQuestionViewModel{SecurityQuestionId = 2, SecurityQuestionName = "TestQue2"},
            };

            var expectedQueResponse = new ServiceResponse<IEnumerable<SecurityQuestionViewModel>>
            {
                Success = true,
                Data = questions
            };

            _httpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
               .Returns(expectedQueResponse);

            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = false,
                Message = errorMessage
            };
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            _httpClientService.Setup(c => c.PutHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedResponse);

            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AuthController(_httpClientService.Object, _configuration.Object, _tokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext.Object,
                },
            };

            //Act
            var actual = target.ForgetPassword(viewModel) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(errorMessage, target.TempData["ErrorMessage"]);
            _configuration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            _httpClientService.Verify(c => c.PutHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);
            Assert.True(target.ModelState.IsValid);
        }

        [Fact]
        public void ForgetPassword_RedirectToAction_WhenBadRequest_WhenResponseIsNull()
        {
            //Arrange
            var viewModel = new ForgetPasswordViewModel
            { LoginId = "test" };
    
            _configuration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var questions = new List<SecurityQuestionViewModel>()
            {
                new SecurityQuestionViewModel{SecurityQuestionId = 1, SecurityQuestionName = "TestQue1"},
                new SecurityQuestionViewModel{SecurityQuestionId = 2, SecurityQuestionName = "TestQue2"},
            };

            var expectedQueResponse = new ServiceResponse<IEnumerable<SecurityQuestionViewModel>>
            {
                Success = true,
                Data = questions
            };

            _httpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
               .Returns(expectedQueResponse);

            var expectedResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(JsonConvert.SerializeObject(null))
            };


            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AuthController(_httpClientService.Object, _configuration.Object, _tokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext.Object,
                },
            };

            _httpClientService.Setup(c => c.PutHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedResponse);

            //Act
            var actual = target.ForgetPassword(viewModel) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("Something went wrong please try after some time.", target.TempData["ErrorMessage"]);
            _configuration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            _httpClientService.Verify(c => c.PutHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);
            Assert.True(target.ModelState.IsValid);
        }

        [Fact]
        public void ForgetPassword_Success_RedirectToAction()
        {
            //Arrange
            var viewModel = new ForgetPasswordViewModel
            { LoginId = "test" };
            _configuration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = true,
                Message = "Success"
            };
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            _httpClientService.Setup(c => c.PutHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedResponse);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            

            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AuthController(_httpClientService.Object, _configuration.Object, _tokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext.Object,
                },
            };

            //Act
            var actual = target.ForgetPassword(viewModel) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal("Home", actual.ControllerName);
            Assert.Equal(expectedServiceResponse.Message, tempData["SuccessMessage"]);
            Assert.True(target.ModelState.IsValid);

            _httpClientService.Verify(c => c.PutHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]
        public void ForgetPassword_HandlesException()
        {
            //Arrange
            var viewModel = new ForgetPasswordViewModel
            { LoginId = "test" };
            _configuration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = true,
                Message = "Success"
            };
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            var error = "An unexpected error occured, Please try again later !!";

            _httpClientService.Setup(c => c.PutHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Throws(new Exception());
            var mockTempDataProvider = new Mock<ITempDataProvider>();


            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AuthController(_httpClientService.Object, _configuration.Object, _tokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext.Object,
                },
            };

            //Act
            var result = target.ForgetPassword(viewModel) as RedirectToActionResult;

            //Assert
            Assert.NotNull(result);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(error, target.TempData["ErrorMessage"]);

        }

        public void Dispose()
        {
            _httpClientService.VerifyAll();
            _configuration.VerifyAll();
            _tokenHandler.VerifyAll();
            _httpContext.VerifyAll();
        }
    }
}
