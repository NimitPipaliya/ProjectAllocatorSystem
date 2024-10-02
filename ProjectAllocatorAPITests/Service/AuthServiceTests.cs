using Moq;
using ProjectAllocatorSystemAPI.Data.Contract;
using ProjectAllocatorSystemAPI.Dtos;
using ProjectAllocatorSystemAPI.Models;
using ProjectAllocatorSystemAPI.Service.Contract;
using ProjectAllocatorSystemAPI.Service.Implementation;

namespace ProjectAllocatorAPITests.Service
{
    public class AuthServiceTests
    {
        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void RegisterUserService_ReturnsPasswordWeak_WhenPasswordIsWeak()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();

            var authService = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            var registerDto = new RegisterUserDto
            {
                Password = "pass",
            };

            mockPasswordService.Setup(service => service.CheckPasswordStrength(registerDto.Password)).Returns("Password too weak");

            // Act
            var result = authService.RegisterUserService(registerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Password too weak", result.Message);
            mockPasswordService.Verify(service => service.CheckPasswordStrength(registerDto.Password), Times.Once);
        }

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void RegisterUserService_ReturnsUserAlreadyExists_WhenUserExists()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();

            var authService = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            var registerDto = new RegisterUserDto
            {
                LoginId = "existingUserLoginId",
                Email = "existingUser@example.com",
            };

            mockPasswordService.Setup(service => service.CheckPasswordStrength(registerDto.Password)).Returns(string.Empty);
            mockAuthRepository.Setup(repo => repo.UserExists(registerDto.LoginId, registerDto.Email)).Returns(true);

            // Act
            var result = authService.RegisterUserService(registerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("User already exist", result.Message);
            mockPasswordService.Verify(service => service.CheckPasswordStrength(registerDto.Password), Times.Once);
            mockAuthRepository.Verify(repo => repo.UserExists(registerDto.LoginId, registerDto.Email), Times.Once);
        }

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void RegisterUserService_ReturnsInvalidGender_WhenGenderIsInvalid()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();

            var authService = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            var registerDto = new RegisterUserDto
            {
                Gender = "unknown",
            };

            mockPasswordService.Setup(service => service.CheckPasswordStrength(registerDto.Password)).Returns(string.Empty);
            mockAuthRepository.Setup(repo => repo.UserExists(registerDto.LoginId, registerDto.Email)).Returns(false);

            // Act
            var result = authService.RegisterUserService(registerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Gender can be either M or F", result.Message);
            mockPasswordService.Verify(service => service.CheckPasswordStrength(registerDto.Password), Times.Once);
            mockAuthRepository.Verify(repo => repo.UserExists(registerDto.LoginId, registerDto.Email), Times.Once);
        }

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void RegisterUserService_ReturnsInvalidPhoneNumber_WhenPhoneNumberIsInvalid_DigitLessThan10()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();

            var authService = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            var registerDto = new RegisterUserDto
            {
                Gender = "M",
                Phone = "123",
            };

            mockPasswordService.Setup(service => service.CheckPasswordStrength(registerDto.Password)).Returns(string.Empty);
            mockAuthRepository.Setup(repo => repo.UserExists(registerDto.LoginId, registerDto.Email)).Returns(false);

            // Act
            var result = authService.RegisterUserService(registerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Enter valid phone number", result.Message);
            mockPasswordService.Verify(service => service.CheckPasswordStrength(registerDto.Password), Times.Once);
            mockAuthRepository.Verify(repo => repo.UserExists(registerDto.LoginId, registerDto.Email), Times.Once);
        }

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void RegisterUserService_ReturnsInvalidPhoneNumber_WhenPhoneNumberIsInvalid_DigitMoreThan12()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();

            var authService = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            var registerDto = new RegisterUserDto
            {
                Gender = "M",
                Phone = "12322233344445",
            };

            mockPasswordService.Setup(service => service.CheckPasswordStrength(registerDto.Password)).Returns(string.Empty);
            mockAuthRepository.Setup(repo => repo.UserExists(registerDto.LoginId, registerDto.Email)).Returns(false);

            // Act
            var result = authService.RegisterUserService(registerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Enter valid phone number", result.Message);
            mockPasswordService.Verify(service => service.CheckPasswordStrength(registerDto.Password), Times.Once);
            mockAuthRepository.Verify(repo => repo.UserExists(registerDto.LoginId, registerDto.Email), Times.Once);
        }
        
        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void RegisterUserService_ReturnsSuccess_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();

            var authService = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            var registerDto = new RegisterUserDto
            {
                Password = "ValidPassword@123",
                LoginId = "newLoginId",
                Email = "newUser@example.com",
                Gender = "M",
                Phone = "1234567890",
            };

            mockPasswordService.Setup(service => service.CheckPasswordStrength(registerDto.Password)).Returns(string.Empty);
            mockAuthRepository.Setup(repo => repo.UserExists(registerDto.LoginId, registerDto.Email)).Returns(false);
            mockAuthRepository.Setup(repo => repo.RegisterUser(It.IsAny<User>())).Returns(true);

            // Act
            var result = authService.RegisterUserService(registerDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(string.Empty, result.Message);
            mockPasswordService.Verify(service => service.CheckPasswordStrength(registerDto.Password), Times.Once);
            mockAuthRepository.Verify(repo => repo.UserExists(registerDto.LoginId, registerDto.Email), Times.Once);
            mockAuthRepository.Verify(repo => repo.RegisterUser(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void RegisterUserService_ReturnsSomethingWentWrong_WhenRegistrationFails()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();

            var authService = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            var registerDto = new RegisterUserDto
            {
                Password = "ValidPassword@123",
                LoginId = "newLoginId",
                Email = "newUser@example.com",
                Gender = "M",
                Phone = "1234567890",
            };

            mockPasswordService.Setup(service => service.CheckPasswordStrength(registerDto.Password)).Returns(string.Empty);
            mockAuthRepository.Setup(repo => repo.UserExists(registerDto.LoginId, registerDto.Email)).Returns(false);
            mockAuthRepository.Setup(repo => repo.RegisterUser(It.IsAny<User>())).Returns(false);

            // Act
            var result = authService.RegisterUserService(registerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Something went wrong, please try after sometime", result.Message);
            mockPasswordService.Verify(service => service.CheckPasswordStrength(registerDto.Password), Times.Once);
            mockAuthRepository.Verify(repo => repo.UserExists(registerDto.LoginId, registerDto.Email), Times.Once);
            mockAuthRepository.Verify(repo => repo.RegisterUser(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void LoginUserService_ReturnsSomethingWentWrong_WhenLoginDtoIsNull()
        {
            //Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockConfiguration = new Mock<IPasswordService>();

            var target = new AuthService(mockAuthRepository.Object, mockConfiguration.Object);


            // Act
            var result = target.LoginUserService(null);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Something went wrong, please try after some time", result.Message);

        }

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void LoginUserService_ReturnsInvalidUsernameOrPassword_WhenUserIsNull()
        {
            //Arrange
            var loginDto = new LoginDto
            {
                Username = "username"
            };
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockConfiguration = new Mock<IPasswordService>();
            mockAuthRepository.Setup(repo => repo.ValidateUser(loginDto.Username)).Returns<User>(null);

            var target = new AuthService(mockAuthRepository.Object, mockConfiguration.Object);


            // Act
            var result = target.LoginUserService(loginDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid user login id or password", result.Message);
            mockAuthRepository.Verify(repo => repo.ValidateUser(loginDto.Username), Times.Once);


        }

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void LoginUserService_ReturnsInvalidUsernameOrPassword_WhenPasswordIsWrong()
        {
            //Arrange
            var loginDto = new LoginDto
            {
                Username = "username",
                Password = "password"
            };
            var user = new User
            {
                UserId = 1,
                LoginId = "username",
                Email = "abc@gmail.com"
            };
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockConfiguration = new Mock<IPasswordService>();
            mockAuthRepository.Setup(repo => repo.ValidateUser(loginDto.Username)).Returns(user);

            var target = new AuthService(mockAuthRepository.Object, mockConfiguration.Object);


            // Act
            var result = target.LoginUserService(loginDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid user login id or password", result.Message);
            mockAuthRepository.Verify(repo => repo.ValidateUser(loginDto.Username), Times.Once);


        }

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void LoginUserService_ReturnsResponse_WhenLoginIsSuccessful()
        {
            //Arrange
            var loginDto = new LoginDto
            {
                Username = "username",
                Password = "password"
            };
            var user = new User
            {
                UserId = 1,
                LoginId = "username",
                Email = "abc@gmail.com"
            };
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockConfiguration = new Mock<IPasswordService>();
            mockAuthRepository.Setup(repo => repo.ValidateUser(loginDto.Username)).Returns(user);
            mockConfiguration.Setup(repo => repo.VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt)).Returns(true);
            mockConfiguration.Setup(repo => repo.CreateToken(user)).Returns("");

            var target = new AuthService(mockAuthRepository.Object, mockConfiguration.Object);


            // Act
            var result = target.LoginUserService(loginDto);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            mockAuthRepository.Verify(repo => repo.ValidateUser(loginDto.Username), Times.Once);
            mockConfiguration.Verify(repo => repo.VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt), Times.Once);
            mockConfiguration.Verify(repo => repo.CreateToken(user), Times.Once);


        }

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void GetAllQuestions_ReturnsQuestions_WhenDataExists()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();
            var authService = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            var mockQuestions = new List<SecurityQuestion>
            {
                new SecurityQuestion { SecurityQuestionId = 1, SecurityQuestionName = "Question 1" },
                new SecurityQuestion { SecurityQuestionId = 2, SecurityQuestionName = "Question 2" }
            };

            mockAuthRepository.Setup(repo => repo.GetAllSecurityQuestions()).Returns(mockQuestions);

            // Act
            var result = authService.GetAllQuestions();

            // Assert
            Assert.True(result.Success);
            Assert.Equal(mockQuestions, result.Data);
            mockAuthRepository.Verify(repo => repo.GetAllSecurityQuestions(), Times.Once);
        }

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void GetAllQuestions_ReturnsDataDoesNotExist_WhenNoQuestions()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();
            var authService = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            mockAuthRepository.Setup(repo => repo.GetAllSecurityQuestions()).Returns(new List<SecurityQuestion>());

            // Act
            var result = authService.GetAllQuestions();

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Data does not exists.", result.Message);
            Assert.Null(result.Data);
            mockAuthRepository.Verify(repo => repo.GetAllSecurityQuestions(), Times.Once);
        }

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void GetAllQuestions_ReturnsDataDoesNotExist_WhenQuestionsIsNull()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();
            var authService = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            mockAuthRepository.Setup(repo => repo.GetAllSecurityQuestions()).Returns((IEnumerable<SecurityQuestion>)null);

            // Act
            var result = authService.GetAllQuestions();

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Data does not exists.", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void GetUserRole_ReturnsRoles_WhenDataExists()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();
            var authService = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            var mockQuestions = new List<UserRole>
            {
                new UserRole { UserRoleId = 1, UserRoleName = "Role 1" },
                new UserRole { UserRoleId = 2, UserRoleName = "Role 2" }
            };

            mockAuthRepository.Setup(repo => repo.GetUserRoles()).Returns(mockQuestions);

            // Act
            var result = authService.GetUserRole();

            // Assert
            Assert.True(result.Success);
            Assert.Equal(mockQuestions, result.Data);
            mockAuthRepository.Verify(repo => repo.GetUserRoles(), Times.Once);
        }

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void GetUserRole_ReturnsDataDoesNotExist_WhenNoRoles()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();
            var authService = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            mockAuthRepository.Setup(repo => repo.GetUserRoles()).Returns(new List<UserRole>());

            // Act
            var result = authService.GetUserRole();

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Data does not exists.", result.Message);
            Assert.Null(result.Data);
            mockAuthRepository.Verify(repo => repo.GetUserRoles(), Times.Once);
        }

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void GetUserRole_ReturnsDataDoesNotExist_WhenRoleIdIs3()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();
            var authService = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);
            var user = new List<UserRole>
            {new UserRole{
                UserRoleId = 3,
                UserRoleName = "abc" }
            };
            mockAuthRepository.Setup(repo => repo.GetUserRoles()).Returns(user);

            // Act
            var result = authService.GetUserRole();

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Data does not exists.", result.Message);
            Assert.Null(result.Data);
            mockAuthRepository.Verify(repo => repo.GetUserRoles(), Times.Once);
        }

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void GetUserRole_ReturnsDataDoesNotExist_WhenRoleIsNull()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();
            var authService = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);
            var user = new List<UserRole>();
            mockAuthRepository.Setup(repo => repo.GetUserRoles()).Returns(user);

            // Act
            var result = authService.GetUserRole();

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Data does not exists.", result.Message);
            Assert.Null(result.Data);
            mockAuthRepository.Verify(repo => repo.GetUserRoles(), Times.Once);
        }

        // ChangePassword

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void ChangePassword_ReturnsErrorMessage_WhenDtoIsNull()
        {
            //Arrange
            ChangePasswordDto dto = null;
            var response = new ServiceResponse<ChangePasswordDto>()
            {
                Data = dto,
                Success = false,
                Message = "Something went wrong, please try after some time."
            };

            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();
            var target = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            //Act
            var actual = target.ChangePassword(dto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
        }

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void ChangePassword_ReturnsErrorMessage_WhenExistingUserIsNull()
        {
            var changePasswordDto = new ChangePasswordDto()
            {
                LoginId = "Test",
                OldPassword = "Test@123",
                NewPassword = "NewTest@123",
                ConfirmNewPassword = "NewTest@123"
            };

            var response = new ServiceResponse<ChangePasswordDto>()
            {
                Data = changePasswordDto,
                Success = false,
                Message = "Something went wrong, please try after some time."
            };

            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();
            var target = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            mockAuthRepository.Setup(c => c.ValidateUser(changePasswordDto.LoginId)).Returns<User>(null);

            //Act
            var actual = target.ChangePassword(changePasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockAuthRepository.Verify(c => c.ValidateUser(changePasswordDto.LoginId), Times.Once);
        }

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void ChangePassword_ReturnsErrorMessage_WhenNewAndOldPasswordIsSame()
        {
            var changePasswordDto = new ChangePasswordDto()
            {
                LoginId = "Test",
                OldPassword = "NewTest@123",
                NewPassword = "NewTest@123",
                ConfirmNewPassword = "NewTest@123"
            };

            var response = new ServiceResponse<ChangePasswordDto>()
            {
                Data = changePasswordDto,
                Success = false,
                Message = "New password cannot be same as old password."
            };

            var user = new User()
            {
                UserId = 1,
                Name = "test",
                Phone = "6798765678",
                LoginId = changePasswordDto.LoginId,
            };

            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();
            var target = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            mockAuthRepository.Setup(c => c.ValidateUser(changePasswordDto.LoginId)).Returns(user);

            //Act
            var actual = target.ChangePassword(changePasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockAuthRepository.Verify(c => c.ValidateUser(changePasswordDto.LoginId), Times.Once);
        }

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void ChangePassword_ReturnsErrorMessage_WhenVerifyPasswordHashFails()
        {
            var changePasswordDto = new ChangePasswordDto()
            {
                LoginId = "Test",
                OldPassword = "Test@123",
                NewPassword = "NewTest@123",
                ConfirmNewPassword = "NewTest@123"
            };

            var response = new ServiceResponse<ChangePasswordDto>()
            {
                Data = changePasswordDto,
                Success = false,
                Message = "Old password is incorrect."
            };
            var user = new User()
            {
                UserId = 1,
                Name = "test",
                Phone = "6798765678",
                LoginId = changePasswordDto.LoginId,
            };

            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();
            var target = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            mockAuthRepository.Setup(c => c.ValidateUser(changePasswordDto.LoginId)).Returns(user);
            mockPasswordService.Setup(x => x.VerifyPasswordHash(changePasswordDto.OldPassword, It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(false);

            //Act
            var actual = target.ChangePassword(changePasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockAuthRepository.Verify(c => c.ValidateUser(changePasswordDto.LoginId), Times.Once);
            mockPasswordService.Verify(x => x.VerifyPasswordHash(changePasswordDto.OldPassword, It.IsAny<byte[]>(), It.IsAny<byte[]>()), Times.Once);
        }
        
        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void ChangePassword_ReturnsErrorMessage_WhenUpdationFails()
        {
            var changePasswordDto = new ChangePasswordDto()
            {
                LoginId = "Test",
                OldPassword = "Test@123",
                NewPassword = "NewTest@123",
                ConfirmNewPassword = "NewTest@123"
            };

            var response = new ServiceResponse<ChangePasswordDto>()
            {
                Data = changePasswordDto,
                Success = false,
                Message = "Something went wrong, please try after some time."
            };

            var user = new User()
            {
                UserId = 1,
                Name = "test",
                Phone = "6798765678",
                LoginId = changePasswordDto.LoginId,
            };

            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();
            var target = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            mockAuthRepository.Setup(c => c.ValidateUser(changePasswordDto.LoginId)).Returns(user);
            mockPasswordService.Setup(x => x.VerifyPasswordHash(changePasswordDto.OldPassword, It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(true);
            mockAuthRepository.Setup(p => p.UpdateUser(It.IsAny<User>())).Returns(false);

            //Act
            var actual = target.ChangePassword(changePasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockAuthRepository.Verify(c => c.ValidateUser(changePasswordDto.LoginId), Times.Once);
            mockPasswordService.Verify(x => x.VerifyPasswordHash(changePasswordDto.OldPassword, It.IsAny<byte[]>(), It.IsAny<byte[]>()), Times.Once);
            mockAuthRepository.Verify(p => p.UpdateUser(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void ChangePassword_ReturnsSuccessMessage_WhenUpdatedSuccessfully()
        {
            var changePasswordDto = new ChangePasswordDto()
            {
                LoginId = "Test",
                OldPassword = "Test@123",
                NewPassword = "NewTest@123",
                ConfirmNewPassword = "NewTest@123"
            };

            var response = new ServiceResponse<ChangePasswordDto>()
            {
                Data = changePasswordDto,
                Success = true,
                Message = "Successfully updated password. Signin again!"
            };

            var user = new User()
            {
                UserId = 1,
                Name = "test",
                Phone = "6798765678",
                LoginId = changePasswordDto.LoginId,
            };

            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();
            var target = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            mockAuthRepository.Setup(c => c.ValidateUser(changePasswordDto.LoginId)).Returns(user);
            mockPasswordService.Setup(x => x.VerifyPasswordHash(changePasswordDto.OldPassword, It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(true);
            mockAuthRepository.Setup(p => p.UpdateUser(It.IsAny<User>())).Returns(true);

            //Act
            var actual = target.ChangePassword(changePasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockAuthRepository.Verify(c => c.ValidateUser(changePasswordDto.LoginId), Times.Once);
            mockPasswordService.Verify(x => x.VerifyPasswordHash(changePasswordDto.OldPassword, It.IsAny<byte[]>(), It.IsAny<byte[]>()), Times.Once);
            mockAuthRepository.Verify(p => p.UpdateUser(It.IsAny<User>()), Times.Once);
        }


        // Reset password

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void ResetPassword_ReturnsSuccessMessage_WhenDtoIsNull()
        {
            //Arrange
            ResetPasswordDto resetPasswordDto = null;
            var response = new ServiceResponse<ResetPasswordDto>()
            {
                Data = resetPasswordDto,
                Success = false,
                Message = "Something went wrong, please try after some time."
            };

            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();
            var target = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            //Act
            var actual = target.ResetPassword(resetPasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
        }

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void ResetPassword_ReturnsErrorMessage_WhenExistingUerIsNull()
        {
            var resetPasswordDto = new ResetPasswordDto()
            {
                LoginId = "test",
                SecurityQuestionId = 1,
                Answer = "testAnswer",
                NewPassword = "Test@1234",
                ConfirmNewPassword = "Test@1234"
            };

            var response = new ServiceResponse<ResetPasswordDto>()
            {
                Data = resetPasswordDto,
                Success = false,
                Message = "Invalid loginId!"
            };

            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();
            var target = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            mockAuthRepository.Setup(c => c.ValidateUser(resetPasswordDto.LoginId)).Returns<User>(null);

            //Act
            var actual = target.ResetPassword(resetPasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockAuthRepository.Verify(c => c.ValidateUser(resetPasswordDto.LoginId), Times.Once);
        }

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void ResetPassword_ReturnsErrorMessage_WhenQuestionSelectedIsNotSame()
        {
            var resetPasswordDto = new ResetPasswordDto()
            {
                LoginId = "test",
                SecurityQuestionId = 2,
                Answer = "testAnswer",
                NewPassword = "Test@1234",
                ConfirmNewPassword = "Test@1234"
            };

            var response = new ServiceResponse<ResetPasswordDto>()
            {
                Data = resetPasswordDto,
                Success = false,
                Message = "User verification failed!"
            };
            var user = new User()
            {
                UserId = 1,
                Name = "test",
                Phone = "6798765678",
                SecurityQuestionId = 1,
                LoginId = resetPasswordDto.LoginId,
            };
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();
            var target = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            mockAuthRepository.Setup(c => c.ValidateUser(resetPasswordDto.LoginId)).Returns(user);

            //Act
            var actual = target.ResetPassword(resetPasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockAuthRepository.Verify(c => c.ValidateUser(resetPasswordDto.LoginId), Times.Once);
        }

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void ResetPassword_ReturnsErrorMessage_WhenAnswerVerificationFails()
        {
            var resetPasswordDto = new ResetPasswordDto()
            {
                LoginId = "test",
                SecurityQuestionId = 1,
                Answer = "testAnswer",
                NewPassword = "Test@1234",
                ConfirmNewPassword = "Test@1234"
            };

            var response = new ServiceResponse<ResetPasswordDto>()
            {
                Data = resetPasswordDto,
                Success = false,
                Message = "User verification failed!"
            };
            var user = new User()
            {
                UserId = 1,
                Name = "test",
                Phone = "6798765678",
                SecurityQuestionId = 1,
                Answer = "wrongAnswer",
                LoginId = resetPasswordDto.LoginId,
            };
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();
            var target = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            mockAuthRepository.Setup(c => c.ValidateUser(resetPasswordDto.LoginId)).Returns(user);

            //Act
            var actual = target.ResetPassword(resetPasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockAuthRepository.Verify(c => c.ValidateUser(resetPasswordDto.LoginId), Times.Once);
        }

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void ResetPassword_ReturnsErrorMessage_WhenPasswordStrengthIsWeak()
        {
            //Arrange
            var resetPasswordDto = new ResetPasswordDto()
            {
                LoginId = "test",
                SecurityQuestionId = 1,
                Answer = "testAnswer",
                NewPassword = "test",
                ConfirmNewPassword = "test"
            };

            var response = new ServiceResponse<ResetPasswordDto>()
            {
                Data = resetPasswordDto,
                Success = false,
                Message = "Mininum password length should be 8\r\nPassword should be alphanumeric\r\nPassword should contain special characters\r\n"
            };
            var user = new User()
            {
                UserId = 1,
                Name = "test",
                Phone = "6798765678",
                Answer = "testAnswer",
                SecurityQuestionId = 1,
                LoginId = resetPasswordDto.LoginId,
            };
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();
            var target = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            mockAuthRepository.Setup(c => c.ValidateUser(resetPasswordDto.LoginId)).Returns(user);
            mockPasswordService.Setup(x => x.CheckPasswordStrength(resetPasswordDto.NewPassword)).Returns(response.Message);
            //Act
            var actual = target.ResetPassword(resetPasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockAuthRepository.Verify(c => c.ValidateUser(resetPasswordDto.LoginId), Times.Once);
            mockPasswordService.Verify(x => x.CheckPasswordStrength(resetPasswordDto.NewPassword),Times.Once);

        }

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void ResetPassword_ReturnsErrorMessage_WhenPasswordUpdateFails()
        {
            //Arrange
            var resetPasswordDto = new ResetPasswordDto()
            {
                LoginId = "test",
                SecurityQuestionId = 1,
                Answer = "testAnswer",
                NewPassword = "Test@1234",
                ConfirmNewPassword = "Test@1234"
            };

            var response = new ServiceResponse<ResetPasswordDto>()
            {
                Data = resetPasswordDto,
                Success = false,
                Message = "Something went wrong, please try after some time."
            };

            var user = new User()
            {
                UserId = 1,
                Name = "test",
                Answer = "testAnswer",
                Phone = "6798765678",
                SecurityQuestionId = 1,
                LoginId = resetPasswordDto.LoginId,
            };

            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();
            var target = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            mockAuthRepository.Setup(c => c.ValidateUser(resetPasswordDto.LoginId)).Returns(user);
            mockPasswordService.Setup(x => x.CheckPasswordStrength(resetPasswordDto.NewPassword)).Returns<string>(null);
            mockAuthRepository.Setup(x => x.UpdateUser(user)).Returns(false);

            //Act
            var actual = target.ResetPassword(resetPasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockAuthRepository.Verify(c => c.ValidateUser(resetPasswordDto.LoginId), Times.Once);
            mockPasswordService.Verify(x => x.CheckPasswordStrength(resetPasswordDto.NewPassword), Times.Once);
            mockAuthRepository.Verify(x => x.UpdateUser(user), Times.Once);
        }

        [Fact]
        [Trait("Auth", "AuthServiceTests")]
        public void ResetPassword_ReturnsSuccess_WhenPasswordResetSuccessfully()
        {
            //Arrange
            var resetPasswordDto = new ResetPasswordDto()
            {
                LoginId = "test",
                SecurityQuestionId = 1,
                Answer = "testAnswer",
                NewPassword = "Test@1234",
                ConfirmNewPassword = "Test@1234"
            };

            var response = new ServiceResponse<ResetPasswordDto>()
            {
                Data = resetPasswordDto,
                Success = true,
                Message = "Successfully updated password."
            };

            var user = new User()
            {
                UserId = 1,
                Answer = "testAnswer",
                Name = "test",
                Phone = "6798765678",
                SecurityQuestionId = 1,
                LoginId = resetPasswordDto.LoginId,
            };

            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();
            var target = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            mockAuthRepository.Setup(c => c.ValidateUser(resetPasswordDto.LoginId)).Returns(user);
            mockAuthRepository.Setup(x => x.UpdateUser(user)).Returns(true);

            //Act
            var actual = target.ResetPassword(resetPasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockAuthRepository.Verify(c => c.ValidateUser(resetPasswordDto.LoginId), Times.Once);
            mockAuthRepository.Verify(x => x.UpdateUser(user), Times.Once);
        }
        [Fact]
        public void LoginUserService_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var loginDto = new LoginDto
            {
                Username = "username",
                Password = "password"
            };
            var _mockAllocatorRepository = new Mock<IPasswordService>();
            var mockAdminRepository = new Mock<IAuthRepository>();
            mockAdminRepository.Setup(c => c.ValidateUser(loginDto.Username)).Throws(new Exception());
            

            var target = new AuthService(mockAdminRepository.Object, _mockAllocatorRepository.Object);
            // Act
            var exception = Assert.Throws<Exception>(() => target.LoginUserService(loginDto));
            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }
        [Fact]
        public void RegisterUserService_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var registerDto = new RegisterUserDto
            {
                Gender = "M",
                Phone = "123",
            };
            var _mockAllocatorRepository = new Mock<IPasswordService>();
            var mockAdminRepository = new Mock<IAuthRepository>();
            _mockAllocatorRepository.Setup(service => service.CheckPasswordStrength(registerDto.Password)).Throws(new Exception());


            var target = new AuthService(mockAdminRepository.Object, _mockAllocatorRepository.Object);
            // Act
            var exception = Assert.Throws<Exception>(() => target.RegisterUserService(registerDto));
            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }
        [Fact]
        public void GetAllQuestions_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var _mockAllocatorRepository = new Mock<IPasswordService>();
            var mockAdminRepository = new Mock<IAuthRepository>();
            mockAdminRepository.Setup(service => service.GetAllSecurityQuestions()).Throws(new Exception());


            var target = new AuthService(mockAdminRepository.Object, _mockAllocatorRepository.Object);
            // Act
            var exception = Assert.Throws<Exception>(() => target.GetAllQuestions());
            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }
        [Fact]
        public void GetUserRole_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var _mockAllocatorRepository = new Mock<IPasswordService>();
            var mockAdminRepository = new Mock<IAuthRepository>();
            mockAdminRepository.Setup(service => service.GetUserRoles()).Throws(new Exception());


            var target = new AuthService(mockAdminRepository.Object, _mockAllocatorRepository.Object);
            // Act
            var exception = Assert.Throws<Exception>(() => target.GetUserRole());
            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }
        [Fact]
        public void changePassword_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var changePasswordDto = new ChangePasswordDto()
            {
                LoginId = "Test",
                OldPassword = "Test@123",
                NewPassword = "NewTest@123",
                ConfirmNewPassword = "NewTest@123"
            };
            var _mockAllocatorRepository = new Mock<IPasswordService>();
            var mockAdminRepository = new Mock<IAuthRepository>();
            mockAdminRepository.Setup(service => service.ValidateUser(changePasswordDto.LoginId)).Throws(new Exception());


            var target = new AuthService(mockAdminRepository.Object, _mockAllocatorRepository.Object);
            // Act
            var exception = Assert.Throws<Exception>(() => target.ChangePassword(changePasswordDto));
            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }
        [Fact]
        public void ResetPassword_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var changePasswordDto = new ResetPasswordDto()
            {
                LoginId = "Test",
                NewPassword = "NewTest@123",
                ConfirmNewPassword = "NewTest@123"
            };
            var _mockAllocatorRepository = new Mock<IPasswordService>();
            var mockAdminRepository = new Mock<IAuthRepository>();
            mockAdminRepository.Setup(service => service.ValidateUser(changePasswordDto.LoginId)).Throws(new Exception());


            var target = new AuthService(mockAdminRepository.Object, _mockAllocatorRepository.Object);
            // Act
            var exception = Assert.Throws<Exception>(() => target.ResetPassword(changePasswordDto));
            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }
    }
}
