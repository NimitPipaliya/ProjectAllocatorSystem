using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProjectAllocatorSystemAPI.Controllers;
using ProjectAllocatorSystemAPI.Dtos;
using ProjectAllocatorSystemAPI.Models;
using ProjectAllocatorSystemAPI.Service.Contract;
using System.Net;

namespace ProjectAllocatorAPITests.Controller
{
    public class AuthControllerTests
    {
        [Theory]
        [Trait("Auth", "AuthControllerTests")]
        [InlineData("User already exists.")]
        [InlineData("Something went wrong, please try after sometime.")]
        [InlineData("Mininum password length should be 8")]
        [InlineData("Password should be apphanumeric")]
        [InlineData("Password should contain special characters")]
        public void Register_ReturnsBadRequest_WhenRegistrationFails(string message)
        {
            // Arrange
            var registerDto = new RegisterUserDto();
            var mockAuthService = new Mock<IAuthService>();
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = false,
                Message = message

            };
            mockAuthService.Setup(service => service.RegisterUserService(registerDto))
                           .Returns(expectedServiceResponse);

            var target = new AuthController(mockAuthService.Object);

            // Act
            var actual = target.AddUser(registerDto) as ObjectResult;

            // Assert
            Assert.NotNull(actual);
            Assert.NotNull((ServiceResponse<string>)actual.Value);
            Assert.Equal(message, ((ServiceResponse<string>)actual.Value).Message);
            Assert.False(((ServiceResponse<string>)actual.Value).Success);
            Assert.Equal((int)HttpStatusCode.BadRequest, actual.StatusCode);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actual);
            Assert.IsType<ServiceResponse<string>>(badRequestResult.Value);
            Assert.False(((ServiceResponse<string>)badRequestResult.Value).Success);
            mockAuthService.Verify(service => service.RegisterUserService(registerDto), Times.Once);
        }
        
        [Fact]
        [Trait("Auth", "AuthControllerTests")]
        public void Register_ReturnsOk_WhenRegistrationSuccess()
        {
            // Arrange
            var registerDto = new RegisterUserDto();
            var mockAuthService = new Mock<IAuthService>();
            var message = "User Added Successfully";
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = true,
                Message = message

            };
            mockAuthService.Setup(service => service.RegisterUserService(registerDto))
                           .Returns(expectedServiceResponse);

            var target = new AuthController(mockAuthService.Object);

            // Act
            var actual = target.AddUser(registerDto) as OkObjectResult;

            // Assert
            Assert.NotNull(actual);
            Assert.NotNull((ServiceResponse<string>)actual.Value);
            Assert.Equal(message, ((ServiceResponse<string>)actual.Value).Message);
            Assert.True(((ServiceResponse<string>)actual.Value).Success);
            Assert.Equal((int)HttpStatusCode.OK, actual.StatusCode);
            var okResult = Assert.IsType<OkObjectResult>(actual);
            Assert.IsType<ServiceResponse<string>>(okResult.Value);
            Assert.True(((ServiceResponse<string>)okResult.Value).Success);
            mockAuthService.Verify(service => service.RegisterUserService(registerDto), Times.Once);
        }

        [Fact]
        [Trait("Auth", "AuthControllerTests")]
        public void GetAllSecurityQuestions_ReturnsOk_WhenQuestionsExists()
        {
            //Arrange

            var questions = new List<SecurityQuestion>
             {
            new SecurityQuestion {SecurityQuestionId=1,SecurityQuestionName="Question 1"},
            new SecurityQuestion{ SecurityQuestionId = 2, SecurityQuestionName = "Question 2"},
            };
            var response = new ServiceResponse<IEnumerable<SecurityQuestion>>
            {
                Success = true,
            };

            var mockAuthService = new Mock<IAuthService>();
            var target = new AuthController(mockAuthService.Object);
            mockAuthService.Setup(c => c.GetAllQuestions()).Returns(response);

            //Act
            var actual = target.GetAllSecurityQuestions() as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockAuthService.Verify(c => c.GetAllQuestions(), Times.Once);
        }


        [Fact]
        [Trait("Auth", "AuthControllerTests")]
        public void GetAllSecurityQuestions_ReturnsNotFound_WhenQuestionsDoesnotExist()
        {
            //Arrange
            var response = new ServiceResponse<IEnumerable<SecurityQuestion>>
            {
                Success = false,
                Data = Enumerable.Empty<SecurityQuestion>()
            };
            var mockAuthService = new Mock<IAuthService>();
            var target = new AuthController(mockAuthService.Object);
            mockAuthService.Setup(c => c.GetAllQuestions()).Returns(response);

            //Act
            var actual = target.GetAllSecurityQuestions() as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockAuthService.Verify(c => c.GetAllQuestions(), Times.Once);
        }

        [Fact]
        [Trait("Auth", "AuthControllerTests")]
        public void GetAllSecurityQuestions_ThrowsException()
        {
            //Arrange
            var mockAuthService = new Mock<IAuthService>();
            var target = new AuthController(mockAuthService.Object);
            mockAuthService.Setup(c => c.GetAllQuestions()).Throws(new Exception());

            //Act
            var exception = Assert.Throws<Exception>(() => target.GetAllSecurityQuestions());

            //Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
            mockAuthService.Verify(c => c.GetAllQuestions(), Times.Once);

        }

        [Fact]
        [Trait("Auth", "AuthControllerTests")]
        public void GetUserRole_ReturnsOk_WhenRolessExists()
        {
            //Arrange

            var roles = new List<UserRole>
             {
            new UserRole {UserRoleId=1,UserRoleName="Question 1"},
            new UserRole { UserRoleId = 2, UserRoleName = "Question 2" },
            };
            var response = new ServiceResponse<IEnumerable<UserRole>>
            {
                Success = true,
            };

            var mockAuthService = new Mock<IAuthService>();
            var target = new AuthController(mockAuthService.Object);
            mockAuthService.Setup(c => c.GetUserRole()).Returns(response);

            //Act
            var actual = target.GetUserRoles() as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockAuthService.Verify(c => c.GetUserRole(), Times.Once);
        }


        [Fact]
        [Trait("Auth", "AuthControllerTests")]
        public void GetUserRoles_ReturnsNotFound_WhenRolesDoesnotExist()
        {
            //Arrange
            var response = new ServiceResponse<IEnumerable<UserRole>>
            {
                Success = false,
                Data = Enumerable.Empty<UserRole>()
            };
            var mockAuthService = new Mock<IAuthService>();
            var target = new AuthController(mockAuthService.Object);
            mockAuthService.Setup(c => c.GetUserRole()).Returns(response);

            //Act
            var actual = target.GetUserRoles() as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockAuthService.Verify(c => c.GetUserRole(), Times.Once);
        }  
        
        [Fact]
        [Trait("Auth", "AuthControllerTests")]
        public void GetUserRoles_ThrowsException()
        {
            //Arrange
            var mockAuthService = new Mock<IAuthService>();
            var target = new AuthController(mockAuthService.Object);
            mockAuthService.Setup(c => c.GetUserRole()).Throws(new Exception());

            //Act
            var exception = Assert.Throws<Exception>(() => target.GetUserRoles());

            //Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
            mockAuthService.Verify(c => c.GetUserRole(), Times.Once);

        }

        [Theory]
        [Trait("Auth", "AuthControllerTests")]
        [InlineData("Invalid username or password!")]
        [InlineData("Something went wrong, please try after sometime.")]
        public void Login_ReturnsBadRequest_WhenLoginFails(string message)
        {
            // Arrange
            var loginDto = new LoginDto();
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = false,
                Message = message

            };
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(service => service.LoginUserService(loginDto))
                           .Returns(expectedServiceResponse);

            var target = new AuthController(mockAuthService.Object);

            // Act
            var actual = target.Login(loginDto) as ObjectResult;

            // Assert
            Assert.NotNull(actual);
            Assert.NotNull((ServiceResponse<string>)actual.Value);
            Assert.Equal(message, ((ServiceResponse<string>)actual.Value).Message);
            Assert.False(((ServiceResponse<string>)actual.Value).Success);
            Assert.Equal((int)HttpStatusCode.BadRequest, actual.StatusCode);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actual);
            Assert.IsType<ServiceResponse<string>>(badRequestResult.Value);
            Assert.False(((ServiceResponse<string>)badRequestResult.Value).Success);
            mockAuthService.Verify(service => service.LoginUserService(loginDto), Times.Once);
        }
        [Fact]
        public void Login_ReturnsOk_WhenLoginSucceeds()
        {
            // Arrange
            var loginDto = new LoginDto { Username = "username", Password = "password" };
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = true,
                Message = string.Empty

            };
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(service => service.LoginUserService(loginDto))
                           .Returns(expectedServiceResponse);

            var target = new AuthController(mockAuthService.Object);

            // Act
            var actual = target.Login(loginDto) as ObjectResult;

            // Assert
            Assert.NotNull(actual);
            Assert.NotNull((ServiceResponse<string>)actual.Value);
            Assert.Equal(string.Empty, ((ServiceResponse<string>)actual.Value).Message);
            Assert.True(((ServiceResponse<string>)actual.Value).Success);
            var okResult = Assert.IsType<OkObjectResult>(actual);
            Assert.IsType<ServiceResponse<string>>(okResult.Value);
            Assert.True(((ServiceResponse<string>)okResult.Value).Success);
            mockAuthService.Verify(service => service.LoginUserService(loginDto), Times.Once);
        }

        [Fact]
        public void Login_ThrowsException()
        {
            //Arrange
            var mockAuthService = new Mock<IAuthService>();
            var fixture = new Fixture();
            var addContactDto = fixture.Create<LoginDto>();
            var response = new ServiceResponse<string>
            {
                Success = false,
            };

            var target = new AuthController(mockAuthService.Object);
            mockAuthService.Setup(c => c.LoginUserService(It.IsAny<LoginDto>())).Throws(new Exception());

            //Ac

            var exception = Assert.Throws<Exception>(() => target.Login(It.IsAny<LoginDto>()));

            //Assert


            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
            mockAuthService.Verify(c => c.LoginUserService(It.IsAny<LoginDto>()), Times.Once);
        }


            // ChangePassword

            [Fact]
        [Trait("Auth", "AuthControllerTests")]
        public void ChangePassword_ReturnsOkResponse_WhenPasswordChangedSucessfully()
        {
            // Arrange
            var changePassword = new ChangePasswordDto() { LoginId = "abc", OldPassword = "Password@123", NewPassword = "Password@1234", ConfirmNewPassword = "Password@1234" };

            var response = new ServiceResponse<string>()
            {
                Data = { },
                Success = true,
                Message = "",
            };

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(c => c.ChangePassword(changePassword)).Returns(response);
            var target = new AuthController(mockAuthService.Object);

            // Act
            var actual = target.ChangePassword(changePassword) as OkObjectResult;

            // Assert
            Assert.NotNull(actual);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            Assert.Equal((int)HttpStatusCode.OK, actual.StatusCode);
            mockAuthService.Verify(c => c.ChangePassword(It.IsAny<ChangePasswordDto>()), Times.Once);
        }

        [Fact]
        [Trait("Auth", "AuthControllerTests")]

        public void ChangePassword_ReturnsBadRequest_WhenPasswordChangeFails()
        {
            // Arrange
            var changePassword = new ChangePasswordDto() { LoginId = "abc", OldPassword = "Password@123", NewPassword = "Password@1234", ConfirmNewPassword = "Password@1234" };

            var response = new ServiceResponse<string>()
            {
                Data = { },
                Success = false,
                Message = "",
            };

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(c => c.ChangePassword(changePassword)).Returns(response);
            var target = new AuthController(mockAuthService.Object);

            // Act
            var actual = target.ChangePassword(changePassword) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(actual);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            Assert.Equal((int)HttpStatusCode.BadRequest, actual.StatusCode);
            mockAuthService.Verify(c => c.ChangePassword(It.IsAny<ChangePasswordDto>()), Times.Once);
        }

        // ResetPassword
        [Fact]
        public void ChangePassword_ThrowsException()
        {
            //Arrange
            var mockAuthService = new Mock<IAuthService>();
            var fixture = new Fixture();
            var addContactDto = fixture.Create<ChangePasswordDto>();
            var response = new ServiceResponse<string>
            {
                Success = false,
            };

            var target = new AuthController(mockAuthService.Object);
            mockAuthService.Setup(c => c.ChangePassword(It.IsAny<ChangePasswordDto>())).Throws(new Exception());

            //Ac

            var exception = Assert.Throws<Exception>(() => target.ChangePassword(It.IsAny<ChangePasswordDto>()));

            //Assert


            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
            mockAuthService.Verify(c => c.ChangePassword(It.IsAny<ChangePasswordDto>()), Times.Once);
        }

        [Fact]
        [Trait("Auth", "AuthControllerTests")]
        public void ResetPassword_ReturnsOkResponse_WhenPasswordResetSuccessfully()
        {
            //Arrnge
            var resetPassword = new ResetPasswordDto()
            {
                LoginId = "test",
                SecurityQuestionId = 1,
                Answer = "testAnswer",
                NewPassword = "Test@1234",
                ConfirmNewPassword = "Test@1234"
            };

            var response = new ServiceResponse<string>()
            {
                Data = { },
                Success = true,
                Message = "",
            };

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(c => c.ResetPassword(resetPassword)).Returns(response);
            var target = new AuthController(mockAuthService.Object);

            //Act
            var actual = target.ResetPassword(resetPassword) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            Assert.Equal((int)HttpStatusCode.OK, actual.StatusCode);
            mockAuthService.Verify(c => c.ResetPassword(It.IsAny<ResetPasswordDto>()), Times.Once);
        }

        [Fact]
        [Trait("Auth", "AuthControllerTests")]
        public void ResetPassword_ReturnsBAdRequest_WhenPasswordResetFails()
        {
            //Arrnge
            var resetPassword = new ResetPasswordDto()
            {
                LoginId = "test",
                SecurityQuestionId = 1,
                Answer = "testAnswer",
                NewPassword = "Test@1234",
                ConfirmNewPassword = "Test@1234"
            };

            var response = new ServiceResponse<string>()
            {
                Data = { },
                Success = false,
                Message = "",
            };

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(c => c.ResetPassword(resetPassword)).Returns(response);
            var target = new AuthController(mockAuthService.Object);

            // Act
            var actual = target.ResetPassword(resetPassword) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(actual);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            Assert.Equal((int)HttpStatusCode.BadRequest, actual.StatusCode);
            mockAuthService.Verify(c => c.ResetPassword(It.IsAny<ResetPasswordDto>()), Times.Once);
        }

        [Fact]
        public void ResetPassword_ThrowsException()
        {
            //Arrange
            var mockAuthService = new Mock<IAuthService>();
            var fixture = new Fixture();
            var addContactDto = fixture.Create<ResetPasswordDto>();
            var response = new ServiceResponse<string>
            {
                Success = false,
            };

            var target = new AuthController(mockAuthService.Object);
            mockAuthService.Setup(c => c.ResetPassword(It.IsAny<ResetPasswordDto>())).Throws(new Exception());

            //Ac

            var exception = Assert.Throws<Exception>(() => target.ResetPassword(It.IsAny<ResetPasswordDto>()));

            //Assert


            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
            mockAuthService.Verify(c => c.ResetPassword(It.IsAny<ResetPasswordDto>()), Times.Once);
        }

    }
}
