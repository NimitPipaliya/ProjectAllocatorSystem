using Microsoft.EntityFrameworkCore;
using Moq;
using ProjectAllocatorSystemAPI.Data;
using ProjectAllocatorSystemAPI.Data.Implementation;
using ProjectAllocatorSystemAPI.Models;

namespace ProjectAllocatorAPITests.Repository
{
    public class AuthRepositoryTests
    {
        [Fact]
        [Trait("Auth", "AuthRepositoryTests")]
        public void RegisterUser_ReturnsTrue_WhenUserIsNotNull()
        {
            // Arrange
            var mockDbContext = new Mock<IAppDbContext>();
            var mockDbSet = new Mock<DbSet<User>>();
            var user = new User
            {
                Name = "TestName",
                LoginId = "loginId",
                Email = "xyz@gmail.com",
                Phone = "9090909090"
            };
            mockDbContext.Setup(db => db.Users).Returns(mockDbSet.Object);

            var target = new AuthRepository(mockDbContext.Object);

            // Act
            var result = target.RegisterUser(user);

            // Assert
            Assert.True(result); // Expecting user registration to succeed
            mockDbSet.Verify(m => m.Add(user), Times.Once);
            mockDbContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Fact]
        [Trait("Auth", "AuthRepositoryTests")]
        public void RegisterUser_ReturnsFalse_WhenUserIsNull()
        {
            // Arrange
            var mockDbContext = new Mock<IAppDbContext>();
            var mockDbSet = new Mock<DbSet<User>>();
            User user = null;
            mockDbContext.Setup(db => db.Users).Returns(mockDbSet.Object);

            var target = new AuthRepository(mockDbContext.Object);

            // Act
            var result = target.RegisterUser(user);

            // Assert
            Assert.False(result);
            mockDbSet.Verify(m => m.Add(It.IsAny<User>()), Times.Never);
            mockDbContext.Verify(m => m.SaveChanges(), Times.Never);
        }

        [Fact]
        [Trait("Auth", "AuthRepositoryTests")]
        public void UserExists_ReturnsTrue_WhenUserExists()
        {
            // Arrange
            var loginId = "existing_login_id";
            var email = "existing_email@example.com";
            var usersData = new List<User>
        {
            new User { LoginId = loginId, Email = email },

        }.AsQueryable();

            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(usersData.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(usersData.Expression);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(usersData.ElementType);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(usersData.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(db => db.Users).Returns(mockDbSet.Object);

            var target = new AuthRepository(mockDbContext.Object);

            // Act
            var result = target.UserExists(loginId, email);

            // Assert
            Assert.True(result);
        }

        [Fact]
        [Trait("Auth", "AuthRepositoryTests")]
        public void UserExists_ReturnsFalse_WhenUserDoesNotExist()
        {
            // Arrange
            var loginId = "non_existing_login_id";
            var email = "non_existing_email@example.com";
            var usersData = new List<User>
            { }.AsQueryable();

            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(usersData.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(usersData.Expression);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(usersData.ElementType);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(usersData.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(db => db.Users).Returns(mockDbSet.Object);

            var target = new AuthRepository(mockDbContext.Object);

            // Act
            var result = target.UserExists(loginId, email);

            // Assert
            Assert.False(result);
        }

        [Fact]
        [Trait("Auth", "AuthRepositoryTests")]
        public void GetAllSecurityQuestions_ReturnsQuestions_WhenQuestionsExist()
        {
            // Arrange
            var questions = new List<SecurityQuestion>
        {
            new SecurityQuestion
            {
                   SecurityQuestionId = 1,
                   SecurityQuestionName = "Question1"
            },
            new SecurityQuestion {
                    SecurityQuestionId = 2,
                    SecurityQuestionName = "Question2"
            }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<SecurityQuestion>>();
            mockDbSet.As<IQueryable<SecurityQuestion>>().Setup(m => m.Provider).Returns(questions.Provider);
            mockDbSet.As<IQueryable<SecurityQuestion>>().Setup(m => m.Expression).Returns(questions.Expression);
            mockDbSet.As<IQueryable<SecurityQuestion>>().Setup(m => m.ElementType).Returns(questions.ElementType);
            mockDbSet.As<IQueryable<SecurityQuestion>>().Setup(m => m.GetEnumerator()).Returns(questions.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.SecurityQuestions).Returns(mockDbSet.Object);

            var target = new AuthRepository(mockDbContext.Object);

            // Act
            var actual = target.GetAllSecurityQuestions();

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(questions.Count(), actual.Count());
            mockDbContext.Verify(c => c.SecurityQuestions, Times.Once);

        }

        [Fact]
        [Trait("Auth", "AuthRepositoryTests")]
        public void GetUserRoles_ReturnsRoles_WhenRolesExist()
        {
            // Arrange
            var roles = new List<UserRole>
            {
            new UserRole
            {
                   UserRoleId = 1,
                   UserRoleName = "Role1"
            },
            new UserRole {
                    UserRoleId = 2,
                    UserRoleName = "Role2"
            }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<UserRole>>();
            mockDbSet.As<IQueryable<UserRole>>().Setup(m => m.Provider).Returns(roles.Provider);
            mockDbSet.As<IQueryable<UserRole>>().Setup(m => m.Expression).Returns(roles.Expression);
            mockDbSet.As<IQueryable<UserRole>>().Setup(m => m.ElementType).Returns(roles.ElementType);
            mockDbSet.As<IQueryable<UserRole>>().Setup(m => m.GetEnumerator()).Returns(roles.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.UserRoles).Returns(mockDbSet.Object);

            var target = new AuthRepository(mockDbContext.Object);

            // Act
            var actual = target.GetUserRoles();

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(roles.Count(), actual.Count());
            mockDbContext.Verify(c => c.UserRoles, Times.Once);

        }

        [Fact]
        [Trait("Auth", "AuthRepositoryTests")]
        public void ValidateUser_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var username = "existing_username";
            var userData = new List<User>
            {
            new User { LoginId = username },
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(userData.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(userData.Expression);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(userData.ElementType);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(userData.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(db => db.Users).Returns(mockDbSet.Object);

            var target = new AuthRepository(mockDbContext.Object);

            // Act
            var result = target.ValidateUser(username);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(username, result.LoginId);
        }

        [Fact]
        [Trait("Auth", "AuthRepositoryTests")]
        public void ValidateUser_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            var username = "non_existing_username";
            var userData = new List<User>
            { }.AsQueryable();

            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(userData.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(userData.Expression);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(userData.ElementType);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(userData.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(db => db.Users).Returns(mockDbSet.Object);

            var target = new AuthRepository(mockDbContext.Object);

            // Act
            var result = target.ValidateUser(username);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        [Trait("Auth", "AuthRepositoryTests")]
        public void UpdateUser_ReturnsTrue()
        {
            var mockDbSet = new Mock<DbSet<User>>();
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.SetupGet(c => c.Users).Returns(mockDbSet.Object);
            mockAppDbContext.Setup(c => c.SaveChanges()).Returns(1);

            var target = new AuthRepository(mockAppDbContext.Object);

            var user = new User()
            {
                UserId = 1,
                Name = "TestFName",
                LoginId = "Test",
                Phone = "74736273462",
                Email = "user@test.com",
                SecurityQuestionId = 1,
                SecurityQuestion = new SecurityQuestion { SecurityQuestionId = 1,  SecurityQuestionName = "testQue" },
            };

            var actual = target.UpdateUser(user);

            // Assert
            Assert.True(actual);
            mockDbSet.Verify(p => p.Update(user), Times.Once);
            mockAppDbContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        [Trait("Auth", "AuthRepositoryTests")]
        public void UpdateUser_ReturnsFalse()
        {
            // Arrange
            var mockDbSet = new Mock<DbSet<User>>();
            var mockAppDbContext = new Mock<IAppDbContext>();

            var target = new AuthRepository(mockAppDbContext.Object);
            User user = null;

            //Act
            var actual = target.UpdateUser(user);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        [Trait("Auth", "AuthRepositoryTests")]
        public void ValidateUser_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var users = new List<Employee>
            {
            }.AsQueryable();
            var email = "email@example.com";
            var mockDbSet = new Mock<DbSet<Employee>>();
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.Employees).Throws(new Exception());
            var target = new AuthRepository(mockAbContext.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.ValidateUser("testusername"));

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }

        [Fact]
        [Trait("Auth", "AuthRepositoryTests")]
        public void UserExists_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var users = new List<User>
            {
            }.AsQueryable();
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.Users).Throws(new Exception());
            var target = new AuthRepository(mockAbContext.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.UserExists("testLoginId","user@test.com"));

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }

        [Fact]
        [Trait("Auth", "AuthRepositoryTests")]
        public void GetAllSecurityQuestions_ReturnsException_WhenRetrivalFails()
        {
            //Arrange
            var users = new List<User>
            {
            }.AsQueryable();
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.SecurityQuestions).Throws(new Exception());
            var target = new AuthRepository(mockAbContext.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.GetAllSecurityQuestions());

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }

        [Fact]
        [Trait("Auth", "AuthRepositoryTests")]
        public void GetUserRoles_ReturnsException_WhenRetrivalFails()
        {
            //Arrange
            var users = new List<UserRole>
            {
            }.AsQueryable();
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.UserRoles).Throws(new Exception());
            var target = new AuthRepository(mockAbContext.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.GetUserRoles());

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }

        [Fact]
        [Trait("Auth", "AuthRepositoryTests")]
        public void UpdateUser_ReturnsException_WhenUpdateFails()
        {
            //Arrange
            var users = new List<UserRole>
            {
            }.AsQueryable();
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.Users).Throws(new Exception());
            var target = new AuthRepository(mockAbContext.Object);
            var user = new User()
            {
                UserId = 1,
                Name = "Test",
                UserRoleId = 1,
            };

            // Act
            var exception = Assert.Throws<Exception>(() => target.UpdateUser(user));

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }

        [Fact]
        [Trait("Auth", "AuthRepositoryTests")]
        public void RegisterUser_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var users = new List<Employee>
            {
            }.AsQueryable();
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.Users).Throws(new Exception());
            var target = new AuthRepository(mockAbContext.Object);
            mockAbContext.Setup(m => m.SaveChanges()).Throws(new Exception());

            var user = new User()
            {
                UserId = 1,
                Name = "Test",
                UserRoleId = 1,
            };
            // Act
            var exception = Assert.Throws<Exception>(() => target.RegisterUser(user));

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }
    }
}
