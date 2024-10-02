using Fare;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProjectAllocatorSystemAPI.Data;
using ProjectAllocatorSystemAPI.Data.Contract;
using ProjectAllocatorSystemAPI.Data.Implementation;
using ProjectAllocatorSystemAPI.Dtos;
using ProjectAllocatorSystemAPI.Models;
using ProjectAllocatorSystemAPI.Service.Contract;
using ProjectAllocatorSystemAPI.Service.Implementation;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAllocatorAPITests.Repository
{
    public class AdminRepositoryTests:IDisposable
    {
        private readonly Mock<IAppDbContext> _mockAppDbContext;
        private readonly AdminRepository _employeeRepository;

        public AdminRepositoryTests()
        {
            // Arrange: Create a mock for IAppDbContext
            _mockAppDbContext = new Mock<IAppDbContext>();

            // Arrange: Create an instance of the repository with the mock context
            _employeeRepository = new AdminRepository(_mockAppDbContext.Object);
        }
        
        [Fact]
        public void GetPaginatedEmployee_ReturnsCorrectEmployee_WhenEmployeeExists_SearchIsNull()
        {
            string sortOrder = "asc";
            string sortBy = "";
            var contacts = new List<Employee>
              {
                  new Employee{EmployeeId=1, EmployeeName="Employee 1"},
                  new Employee{EmployeeId=2, EmployeeName="Employee 2"},
                  new Employee{EmployeeId = 3, EmployeeName = "Employee 3"},

              }.AsQueryable();
            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Expression).Returns(contacts.Expression);
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Provider).Returns(contacts.Provider);

            _mockAppDbContext.Setup(c => c.Employees).Returns(mockDbSet.Object);
            var target = new AdminRepository(_mockAppDbContext.Object);
            //Act
            var actual = target.GetPaginatedEmployees(1, 2, null, sortOrder, sortBy);
            //Assert
            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count());
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Expression, Times.Once);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Provider, Times.Exactly(5));
            _mockAppDbContext.Verify(c => c.Employees, Times.Once);
        }

        [Fact]
        public void GetPaginatedContacts_ReturnsCorrectContacts_WhenContactsExists_SearchIsNotNull()
        {
            string sortOrder = "asc";
            string search = "e";
            string sortBy = "";
            var contacts = new List<Employee>
              {
                  new Employee{EmployeeId=1, EmployeeName="Employee 1"},
                  new Employee{EmployeeId = 2, EmployeeName = "Employee 2"},
                  new Employee{EmployeeId=3, EmployeeName
                  ="Employee 3"},

              }.AsQueryable();
            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Provider).Returns(contacts.Provider);
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Expression).Returns(contacts.Expression);

            _mockAppDbContext.Setup(c => c.Employees).Returns(mockDbSet.Object);
            var target = new AdminRepository(_mockAppDbContext.Object);
            //Act
            var actual = target.GetPaginatedEmployees(1, 2, search, sortOrder, sortBy);
            //Assert
            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count());
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Provider, Times.Exactly(5));
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Expression, Times.Once);
            _mockAppDbContext.Verify(c => c.Employees, Times.Once);
        }

        [Fact]
        public void GetPaginatedEmployee_ReturnsEmptyList_WhenNoEmployeeExists_SearchIsNull()
        {
            string sortOrder = "desc";
            string sortBy = "";
            var contacts = new List<Employee>().AsQueryable();
            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Expression).Returns(contacts.Expression);
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Provider).Returns(contacts.Provider);

            _mockAppDbContext.Setup(c => c.Employees).Returns(mockDbSet.Object);
            var target = new AdminRepository(_mockAppDbContext.Object);
            //Act
            var actual = target.GetPaginatedEmployees(1, 2, null, sortOrder, sortBy);
            //Assert
            Assert.NotNull(actual);
            Assert.Empty(actual);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Expression, Times.Once);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Provider, Times.Exactly(5));
            _mockAppDbContext.Verify(c => c.Employees, Times.Once);
        }

        [Fact]
        public void GetPaginatedEmployee_ReturnsEmptyList_WhenNoEmployeeExists_SearchIsNotNull()
        {
            string search = "con";
            string sortOrder = "asc";
            string sortBy = "";
            var contacts = new List<Employee>().AsQueryable();
            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Expression).Returns(contacts.Expression);
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Provider).Returns(contacts.Provider);

            _mockAppDbContext.Setup(c => c.Employees).Returns(mockDbSet.Object);
            var target = new AdminRepository(_mockAppDbContext.Object);
            //Act
            var actual = target.GetPaginatedEmployees(1, 2, search, sortOrder, sortBy);
            //Assert
            Assert.NotNull(actual);
            Assert.Empty(actual);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Expression, Times.Once);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Provider, Times.Exactly(5));
            _mockAppDbContext.Verify(c => c.Employees, Times.Once);
        }
        [Fact]
        public void GetPaginatedEmployees_SortsByName_Ascending()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { EmployeeId = 1, EmployeeName = "John" },
                new Employee { EmployeeId = 2, EmployeeName = "Alice" },
                new Employee { EmployeeId = 3, EmployeeName = "Bob" }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Expression).Returns(employees.Expression);
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Provider).Returns(employees.Provider);

            _mockAppDbContext.Setup(c => c.Employees).Returns(mockDbSet.Object);

            // Act
            var actual = _employeeRepository.GetPaginatedEmployees(1, 2, null, "asc", "name");

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count());
            Assert.Equal("Alice", actual.First().EmployeeName);
            Assert.Equal("Bob", actual.Skip(1).First().EmployeeName);
        }
        [Fact]
        public void GetPaginatedEmployees_SortsByJobRole_Descending()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { EmployeeId = 1, EmployeeName = "John", JobRole = new JobRole { JobRoleName = "Developer" } },
                new Employee { EmployeeId = 2, EmployeeName = "Alice", JobRole = new JobRole { JobRoleName = "Manager" } },
                new Employee { EmployeeId = 3, EmployeeName = "Bob", JobRole = new JobRole { JobRoleName = "Analyst" } }
            }.AsQueryable();

            // Set up the mock DbSet
            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Expression).Returns(employees.Expression);
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Provider).Returns(employees.Provider);

            _mockAppDbContext.Setup(c => c.Employees).Returns(mockDbSet.Object);

            // Act
            var actual = _employeeRepository.GetPaginatedEmployees(1, 2, null, "desc", "jobRole");

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count());
            Assert.Equal("Manager", actual.First().JobRole.JobRoleName);
        }
        [Fact]
        public void GetPaginatedEmployees_SortsByAllocation_Descending()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { EmployeeId = 1, EmployeeName = "John", Allocationtype = new AllocationType { Type = "Developer" } },
                new Employee { EmployeeId = 2, EmployeeName = "Alice",Allocationtype = new AllocationType { Type = "Manager" } },
                new Employee { EmployeeId = 3, EmployeeName = "Bob", Allocationtype = new AllocationType { Type = "Allocator" } }
            }.AsQueryable();

            // Set up the mock DbSet
            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Expression).Returns(employees.Expression);
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Provider).Returns(employees.Provider);

            _mockAppDbContext.Setup(c => c.Employees).Returns(mockDbSet.Object);

            // Act
            var actual = _employeeRepository.GetPaginatedEmployees(1, 2, null, "desc", "allocationStatus");

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count());
        }
        


        [Fact]
        public void GetPaginatedEmployees_SortsByStartDate_Descending()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { EmployeeId = 1, EmployeeName = "John", JobRole = new JobRole { JobRoleName = "Developer" } },
                new Employee { EmployeeId = 2, EmployeeName = "Alice", JobRole = new JobRole { JobRoleName = "Manager" } },
                new Employee { EmployeeId = 3, EmployeeName = "Bob", JobRole = new JobRole { JobRoleName = "Analyst" } }
            }.AsQueryable();

            // Set up the mock DbSet
            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Expression).Returns(employees.Expression);
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Provider).Returns(employees.Provider);

            _mockAppDbContext.Setup(c => c.Employees).Returns(mockDbSet.Object);

            // Act
            var actual = _employeeRepository.GetPaginatedEmployees(1, 2, null, "desc", "startDate");

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count());
        }
        [Fact]
        public void GetPaginatedEmployees_SortsByEndDate_Descending()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { EmployeeId = 1, EmployeeName = "John", JobRole = new JobRole { JobRoleName = "Developer" } },
                new Employee { EmployeeId = 2, EmployeeName = "Alice", JobRole = new JobRole { JobRoleName = "Manager" } },
                new Employee { EmployeeId = 3, EmployeeName = "Bob", JobRole = new JobRole { JobRoleName = "Analyst" } }
            }.AsQueryable();

            // Set up the mock DbSet
            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Expression).Returns(employees.Expression);
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Provider).Returns(employees.Provider);

            _mockAppDbContext.Setup(c => c.Employees).Returns(mockDbSet.Object);

            // Act
            var actual = _employeeRepository.GetPaginatedEmployees(1, 2, null, "desc", "endDate");

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count());
        }
        [Fact]
        public void GetEmployee_WhenEmployeeIsNull()
        {
            //Arrange
            var id = 1;
            var employees = new List<Employee>().AsQueryable();
            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Provider).Returns(employees.Provider);
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Expression).Returns(employees.Expression);
            _mockAppDbContext.SetupGet(c => c.Employees).Returns(mockDbSet.Object);
            var target = new AdminRepository(_mockAppDbContext.Object);
            //Act
            var actual = target.GetEmployeeById(id);
            //Assert
            Assert.Null(actual);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Provider, Times.Exactly(5));
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Expression, Times.Once);
            _mockAppDbContext.VerifyGet(c => c.Employees, Times.Once);

        }
        [Fact]
        public void GetEmployee_WhenEmployeesIsNotNull()
        {
            //Arrange
            var id = 1;
            var employees = new List<Employee>()
            {
              new Employee { EmployeeId = 1, EmployeeName = "Employee 1" },
                new Employee { EmployeeId = 2,  EmployeeName = "Employee 2" },
            }.AsQueryable();
            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Provider).Returns(employees.Provider);
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Expression).Returns(employees.Expression);
            _mockAppDbContext.SetupGet(c => c.Employees).Returns(mockDbSet.Object);
            var target = new AdminRepository(_mockAppDbContext.Object);
            //Act
            var actual = target.GetEmployeeById(id);
            //Assert
            Assert.NotNull(actual);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Provider, Times.Exactly(5));
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Expression, Times.Once);
            _mockAppDbContext.VerifyGet(c => c.Employees, Times.Once);
        }
        [Fact]
        public void GetAllEmployees_ReturnsEmployeesWithAssociations()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee
                {
                    EmployeeId = 1,
                    EmployeeName = "John",
                    JobRoleId = 101,
                    JobRole = new JobRole { JobRoleId = 101, JobRoleName = "Developer" },
                    Allocationtype = new AllocationType { TypeId = 201, Type = "Full-time" },
                    EmployeeSkills = new List<EmployeeSkills>
                    {
                        new EmployeeSkills { SId = 301, Skill = new Skill { Id = 401, SkillName = "C#" } },
                        new EmployeeSkills { SId = 302, Skill = new Skill { Id = 402, SkillName = "ASP.NET" } }
                    }
                },
                // Add more employees with different associations
            }.AsQueryable();

            // Set up the mock DbSet
            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Provider).Returns(employees.Provider);
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.GetEnumerator()).Returns(employees.GetEnumerator());


            _mockAppDbContext.Setup(c => c.Employees).Returns(mockDbSet.Object);
            _mockAppDbContext.SetupGet(c => c.Employees).Returns(mockDbSet.Object);
            // Act
            var actual = _employeeRepository.GetAllEmployeees();

            // Assert
            Assert.NotNull(actual);
            Assert.Single(actual); // Adjust based on your actual data
            var firstEmployee = actual.First();
            Assert.Equal("John", firstEmployee.EmployeeName);
            Assert.NotNull(firstEmployee.JobRole);
            Assert.NotNull(firstEmployee.Allocationtype);
            Assert.NotNull(firstEmployee.EmployeeSkills);
            Assert.Equal(2, firstEmployee.EmployeeSkills.Count); // Adjust based on your actual data
            // Add more assertions as needed for other associations
        }
        [Fact]
        public void UpdateEmployee_ReturnsTrue()
        {
            //Arrange
            var mockDbSet = new Mock<DbSet<Employee>>();
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.SetupGet(c => c.Employees).Returns(mockDbSet.Object);
            mockAppDbContext.Setup(c => c.SaveChanges()).Returns(1);
            var target = new AdminRepository(mockAppDbContext.Object);
            var employee = new Employee
            {
                EmployeeId = 1,
                EmployeeName = "E1"
            };


            //Act
            var actual = target.UpdateEmployee(employee);

            //Assert
            Assert.True(actual);
            mockDbSet.Verify(c => c.Update(employee), Times.Once);
            mockAppDbContext.Verify(c => c.SaveChanges(), Times.Once);
        }
        [Fact]
        public void UpdateEmployeet_ReturnsFalse()
        {
            //Arrange
            Employee employee = null;
            var mockAbContext = new Mock<IAppDbContext>();
            var target = new AdminRepository(mockAbContext.Object);

            //Act
            var actual = target.UpdateEmployee(employee);
            //Assert
            Assert.False(actual);
        }
        [Fact]
        public void UpdateEmployeeT_ReturnsTrue()
        {
            //Arrange
            var mockDbSet = new Mock<DbSet<Employee>>();
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.SetupGet(c => c.Employees).Returns(mockDbSet.Object);
            mockAppDbContext.Setup(c => c.SaveChanges()).Returns(1);
            var target = new AdminRepository(mockAppDbContext.Object);
            var employee = new Employee
            {
                EmployeeId = 1,
                EmployeeName = "E1"
            };


            //Act
            var actual = target.Update(employee);

            //Assert
            Assert.True(actual);
            mockDbSet.Verify(c => c.Update(employee), Times.Once);
            mockAppDbContext.Verify(c => c.SaveChanges(), Times.Once);
        }
        [Fact]
        public void UpdateEmployee_ReturnsFalse()
        {
            //Arrange
            Employee employee = null;
            var mockAbContext = new Mock<IAppDbContext>();
            var target = new AdminRepository(mockAbContext.Object);

            //Act
            var actual = target.Update(employee);
            //Assert
            Assert.False(actual);
        }
        [Fact]
        public void Add_AddsEmployeeToDatabase()
        {
            // Arrange
            var employeeToAdd = new Employee
            {
                EmployeeId = 101,
                EmployeeName = "Alice",
            };

            var mockDbSet = new Mock<DbSet<Employee>>();
            _mockAppDbContext.Setup(c => c.Employees).Returns(mockDbSet.Object);

            // Act
            var result = _employeeRepository.Add(employeeToAdd);

            // Assert
            mockDbSet.Verify(c => c.Add(It.IsAny<Employee>()), Times.Once);
            _mockAppDbContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void EmployeeNameExists_ReturnsTrue_WhenEmployeeExists()
        {
            // Arrange
            var employeesData = new List<Employee>
        {
            new Employee { EmployeeId = 1, EmployeeName = "Alice" },
            new Employee { EmployeeId = 2, EmployeeName = "Bob" }
        }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(m => m.Provider).Returns(employeesData.Provider);
            mockDbSet.As<IQueryable<Employee>>().Setup(m => m.Expression).Returns(employeesData.Expression);
            
            _mockAppDbContext.Setup(c => c.Employees).Returns(mockDbSet.Object);

            var testName = "Alice";

            // Act
            var result = _employeeRepository.EmployeeNameExists(testName);

            // Assert
            Assert.True(result);
            mockDbSet.As<IQueryable<Employee>>().Verify(m => m.Provider);
            mockDbSet.As<IQueryable<Employee>>().Verify(m => m.Expression);
        }

        [Fact]
        public void EmployeeNameExists_ReturnsFalse_WhenEmployeeDoesNotExist()
        {
            // Arrange
            var employeesData = new List<Employee>
        {
            new Employee { EmployeeId = 1, EmployeeName = "Alice" },
            new Employee { EmployeeId = 2, EmployeeName = "Bob" }
        }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(m => m.Provider).Returns(employeesData.Provider);
            mockDbSet.As<IQueryable<Employee>>().Setup(m => m.Expression).Returns(employeesData.Expression);

            _mockAppDbContext.Setup(c => c.Employees).Returns(mockDbSet.Object);

            var testName = "Charlie";

            // Act
            var result = _employeeRepository.EmployeeNameExists(testName);

            // Assert
            Assert.False(result);
            mockDbSet.As<IQueryable<Employee>>().Verify(m => m.Provider);
            mockDbSet.As<IQueryable<Employee>>().Verify(m => m.Expression);
        }
        [Fact]
        public void EmployeeNameExistsWithEmailID_ReturnsTrue_WhenEmployeeExists()
        {
            // Arrange
            var employeesData = new List<Employee>
        {
            new Employee { EmployeeId = 1, EmployeeName = "Alice" ,EmailId="abc@gmail.com"},
            new Employee { EmployeeId = 2, EmployeeName = "Bob" ,EmailId="xyz@gmail.com"}
        }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(m => m.Provider).Returns(employeesData.Provider);
            mockDbSet.As<IQueryable<Employee>>().Setup(m => m.Expression).Returns(employeesData.Expression);

            _mockAppDbContext.Setup(c => c.Employees).Returns(mockDbSet.Object);

            var email = "abc@gmail.com";

            // Act
            var result = _employeeRepository.EmployeeEmailExists(email);

            // Assert
            Assert.True(result);
            mockDbSet.As<IQueryable<Employee>>().Verify(m => m.Provider);
            mockDbSet.As<IQueryable<Employee>>().Verify(m => m.Expression);
        }
        [Fact]
        public void EmployeeNameExistsWithEmailID_ReturnsTrue_WhenEmployeeNotExists()
        {
            // Arrange
            var employeesData = new List<Employee>
        {
            new Employee { EmployeeId = 1, EmployeeName = "Alice" ,EmailId="abc@gmail.com"},
            new Employee { EmployeeId = 2, EmployeeName = "Bob" ,EmailId="xyz@gmail.com"}
        }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(m => m.Provider).Returns(employeesData.Provider);
            mockDbSet.As<IQueryable<Employee>>().Setup(m => m.Expression).Returns(employeesData.Expression);

            _mockAppDbContext.Setup(c => c.Employees).Returns(mockDbSet.Object);

            var email = "pqr@gmail.com";

            // Act
            var result = _employeeRepository.EmployeeEmailExists(email);

            // Assert
            Assert.False(result);
            mockDbSet.As<IQueryable<Employee>>().Verify(m => m.Provider);
            mockDbSet.As<IQueryable<Employee>>().Verify(m => m.Expression);
        }
        [Fact]
        public void EmployeeNameExists_ReturnsTrue_WhenEmployeeWithNameExistsAndDifferentId()
        {
            // Arrange
            var employeesData = new List<Employee>().AsQueryable();

            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(m => m.Provider).Returns(employeesData.Provider);
            mockDbSet.As<IQueryable<Employee>>().Setup(m => m.Expression).Returns(employeesData.Expression);
            
            _mockAppDbContext.Setup(c => c.Employees).Returns(mockDbSet.Object);

            var testId = 3;
            var testName = "Alice";

            // Act
            var result = _employeeRepository.EmployeeNameExists(testId, testName);

            // Assert
            Assert.False(result);
            mockDbSet.As<IQueryable<Employee>>().Verify(m => m.Provider);
            mockDbSet.As<IQueryable<Employee>>().Verify(m => m.Expression);
        }
        [Fact]
        public void EmployeeEmailExists_ReturnsTruee_WhenEmployeeWithEmailExistsAndDifferentId()
        {
            // Arrange
            var employeesData = new List<Employee>
        {
            new Employee { EmployeeId = 1, EmailId = "alice@example.com" },
            new Employee { EmployeeId = 2, EmailId = "bob@example.com" }
        }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(m => m.Provider).Returns(employeesData.Provider);
            mockDbSet.As<IQueryable<Employee>>().Setup(m => m.Expression).Returns(employeesData.Expression);

            _mockAppDbContext.Setup(c => c.Employees).Returns(mockDbSet.Object);

            var testId = 3;
            var testEmail = "alice@example.com";

            // Act
            var result = _employeeRepository.EmployeeEmailExists(testId, testEmail);

            // Assert
            Assert.True(result);
            
        }
        [Fact]
        public void EmployeeNameExistss_ReturnsTruee_WhenEmployeeWithEmailExistsAndDifferentId()
        {
            // Arrange
            var employeesData = new List<Employee>
        {
            new Employee { EmployeeId = 1, EmailId = "alice@example.com",EmployeeName="qwer" },
            new Employee { EmployeeId = 2, EmailId = "bob@example.com",EmployeeName="asdf"}
        }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(m => m.Provider).Returns(employeesData.Provider);
            mockDbSet.As<IQueryable<Employee>>().Setup(m => m.Expression).Returns(employeesData.Expression);

            _mockAppDbContext.Setup(c => c.Employees).Returns(mockDbSet.Object);

            var testId = 3;
            var testName = "qwer";

            // Act
            var result = _employeeRepository.EmployeeNameExists(testId, testName);

            // Assert
            Assert.True(result);

        }
        [Fact]
        public void EmployeeEmailExists_ReturnsTrue_WhenEmployeeWithEmailExistsAndDifferentId()
        {
            // Arrange
            var employeesData = new List<Employee>
        {
            new Employee { EmployeeId = 1, EmailId = "alice@example.com" },
            new Employee { EmployeeId = 2, EmailId = "bob@example.com" }
        }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(m => m.Provider).Returns(employeesData.Provider);
            mockDbSet.As<IQueryable<Employee>>().Setup(m => m.Expression).Returns(employeesData.Expression);
           
            _mockAppDbContext.Setup(c => c.Employees).Returns(mockDbSet.Object);

            var testId = 1;
            var testEmail = "alicef@example.com";

            // Act
            var result = _employeeRepository.EmployeeEmailExists(testId, testEmail);

            // Assert
            Assert.False(result);
            mockDbSet.As<IQueryable<Employee>>().Verify(m => m.Provider);
            mockDbSet.As<IQueryable<Employee>>().Verify(m => m.Expression);
        }
        [Fact]
        public void EmployeeEmailExistss_ReturnsTrue_WhenEmployeeWithEmailExistsAndDifferentId()
        {
            // Arrange
            var employeesData = new List<Employee>
        {
            new Employee { EmployeeId = 1, EmailId = "alice@example.com",EmployeeName="werew" },
            new Employee { EmployeeId = 2, EmailId = "bob@example.com" ,EmployeeName="wertyui"}
        }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(m => m.Provider).Returns(employeesData.Provider);
            mockDbSet.As<IQueryable<Employee>>().Setup(m => m.Expression).Returns(employeesData.Expression);
            
            _mockAppDbContext.Setup(c => c.Employees).Returns(mockDbSet.Object);

            var testId = 1;
            var testEmail = "alicee@example.com";

            // Act
            var result = _employeeRepository.EmployeeEmailExists(testId, testEmail);

            // Assert
            Assert.False(result);
            mockDbSet.As<IQueryable<Employee>>().Verify(m => m.Provider);
            mockDbSet.As<IQueryable<Employee>>().Verify(m => m.Expression);
        }
        [Fact]
        public void AddSkills_AddsNewSkillsToEmployee()
        {
            // Arrange
            var employee = new Employee { EmployeeId = 1, EmployeeSkills = new List<EmployeeSkills>() };
            var skillNames = new List<string> { "C#", "JavaScript" };

            var mockDbSetSkills = new Mock<DbSet<Skill>>();
            var existingSkills = new List<Skill>
        {
            new Skill { Id = 1, SkillName = "C#" },
            new Skill { Id = 2, SkillName = "JavaScript" }
        }.AsQueryable();

            mockDbSetSkills.As<IQueryable<Skill>>().Setup(m => m.Provider).Returns(existingSkills.Provider);
            mockDbSetSkills.As<IQueryable<Skill>>().Setup(m => m.Expression).Returns(existingSkills.Expression);

            _mockAppDbContext.Setup(c => c.Skills).Returns(mockDbSetSkills.Object);

            var mockDbSetEmployeeSkills = new Mock<DbSet<EmployeeSkills>>();

            // Act
            _employeeRepository.AddSkills(employee, skillNames);

            // Assert
            mockDbSetSkills.As<IQueryable<Skill>>().Verify(m => m.Provider);
            mockDbSetSkills.As<IQueryable<Skill>>().Verify(m => m.Expression);
        }
        [Fact]
        public void AddSkills_WhenEmployeeSkillsIsNull()
        {
            // Arrange
            var employee = new Employee { EmployeeId = 1};
            var skillNames = new List<string> { "C#", "JavaScript" };

            var mockDbSetSkills = new Mock<DbSet<Skill>>();
            var existingSkills = new List<Skill>
        {
            new Skill { Id = 1, SkillName = "C#" },
            new Skill { Id = 2, SkillName = "JavaScript" }
        }.AsQueryable();

            mockDbSetSkills.As<IQueryable<Skill>>().Setup(m => m.Provider).Returns(existingSkills.Provider);
            mockDbSetSkills.As<IQueryable<Skill>>().Setup(m => m.Expression).Returns(existingSkills.Expression);

            _mockAppDbContext.Setup(c => c.Skills).Returns(mockDbSetSkills.Object);

            var mockDbSetEmployeeSkills = new Mock<DbSet<EmployeeSkills>>();

            // Act
            _employeeRepository.AddSkills(employee, skillNames);

            // Assert
            mockDbSetSkills.As<IQueryable<Skill>>().Verify(m => m.Provider);
            mockDbSetSkills.As<IQueryable<Skill>>().Verify(m => m.Expression);
        }

        [Fact]
        public void AddSkills_AddsExistingSkillsToEmployee()
        {
            // Arrange
            var employee = new Employee { EmployeeId = 1, EmployeeSkills = new List<EmployeeSkills>() };
            var skillNames = new List<string> { "C#", "JavaScript" };

            var mockDbSetSkills = new Mock<DbSet<Skill>>();
            var existingSkills = new List<Skill>
        {
            new Skill { Id = 1, SkillName = "C#" },
            new Skill { Id = 2, SkillName = "JavaScript" }
        }.AsQueryable();

            mockDbSetSkills.As<IQueryable<Skill>>().Setup(m => m.Provider).Returns(existingSkills.Provider);
            mockDbSetSkills.As<IQueryable<Skill>>().Setup(m => m.Expression).Returns(existingSkills.Expression);

            _mockAppDbContext.Setup(c => c.Skills).Returns(mockDbSetSkills.Object);

            var mockDbSetEmployeeSkills = new Mock<DbSet<EmployeeSkills>>();

            // Act
            _employeeRepository.AddSkills(employee, skillNames);

            // Assert

            mockDbSetSkills.As<IQueryable<Skill>>().Verify(m => m.Provider);
            mockDbSetSkills.As<IQueryable<Skill>>().Verify(m => m.Expression);
        }
        [Fact]
        public void AddSkills_AddsNewSkill_WhenExistingSkillIsNull()
        {
            // Arrange
            var employee = new Employee { EmployeeId = 1, EmployeeSkills = new List<EmployeeSkills>() };
            var skillName = "C#";

            var mockDbSetSkills = new Mock<DbSet<Skill>>();
            var existingSkills = new List<Skill>().AsQueryable();

            mockDbSetSkills.As<IQueryable<Skill>>().Setup(m => m.Provider).Returns(existingSkills.Provider);
            mockDbSetSkills.As<IQueryable<Skill>>().Setup(m => m.Expression).Returns(existingSkills.Expression);

            _mockAppDbContext.Setup(c => c.Skills).Returns(mockDbSetSkills.Object);

            var mockDbSetEmployeeSkills = new Mock<DbSet<EmployeeSkills>>();

            // Act
            _employeeRepository.AddSkills(employee, new List<string> { skillName });

            // Assert
            mockDbSetSkills.As<IQueryable<Skill>>().Verify(m => m.Provider);
            mockDbSetSkills.As<IQueryable<Skill>>().Verify(m => m.Expression);
        }
        [Fact]
        public void TotalEmployees_ReturnsCount_WhenEmployeesExistWhenSearchIsNull()
        {
            var contacts = new List<Employee> {
                new Employee {EmployeeId = 1,EmployeeName="Employee 1"},
                new Employee {EmployeeId = 2,EmployeeName = "Employee 2"}
            }.AsQueryable();
            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Provider).Returns(contacts.Provider);
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Expression).Returns(contacts.Expression);
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Employees).Returns(mockDbSet.Object);
            var target = new AdminRepository(mockAppDbContext.Object);

            //Act
            var actual = target.TotalEmployees(null);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(contacts.Count(), actual);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.Verify(c => c.Employees, Times.Once);

        }

        [Fact]
        public void TotalEmployee_ReturnsCount_WhenEmployeeExistWhenSearchIsNotNull()
        {
            string search = "e";
            var contacts = new List<Employee> {
                new Employee {EmployeeId = 1, EmployeeName = "Employee 1"},
                new Employee {EmployeeId = 2,EmployeeName="Employee 2"}
            }.AsQueryable();
            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Provider).Returns(contacts.Provider);
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Expression).Returns(contacts.Expression);
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Employees).Returns(mockDbSet.Object);
            var target = new AdminRepository(mockAppDbContext.Object);

            //Act
            var actual = target.TotalEmployees(search);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(contacts.Count(), actual);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.Verify(c => c.Employees, Times.Once);

        }

        [Fact]
        public void TotalEmployee_ReturnsCountZero_WhenNoEmployeeExistWhenSearchIsNull()
        {
            var contacts = new List<Employee>
            {
            }.AsQueryable();
            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Provider).Returns(contacts.Provider);
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Expression).Returns(contacts.Expression);
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Employees).Returns(mockDbSet.Object);
            var target = new AdminRepository(mockAppDbContext.Object);

            //Act
            var actual = target.TotalEmployees(null);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(contacts.Count(), actual);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.Verify(c => c.Employees, Times.Once);

        }

        [Fact]
        public void TotalEmployee_ReturnsCountZero_WhenNoEmployeesExistWhenSearchIsNotNull()
        {
            var contacts = new List<Employee>
            {

            }.AsQueryable();
            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Provider).Returns(contacts.Provider);
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Expression).Returns(contacts.Expression);
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Employees).Returns(mockDbSet.Object);
            var target = new AdminRepository(mockAppDbContext.Object);
            string search = "abc";
            //Act
            var actual = target.TotalEmployees(search);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(contacts.Count(), actual);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.Verify(c => c.Employees, Times.Once);

        }
        
        [Fact]
        public void GetAllJoRoles_ReturnsJobRoles_WhenJobRolesExist()
        {
            // Arrange
            var jobRoles = new List<JobRole>
            {
                new JobRole
                {
                       JobRoleId = 1,
                       JobRoleName= "Developer"
                },
                new JobRole {
                        JobRoleId = 2,
                        JobRoleName = "Tester"
                }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<JobRole>>();
            mockDbSet.As<IQueryable<JobRole>>().Setup(m => m.Provider).Returns(jobRoles.Provider);
            mockDbSet.As<IQueryable<JobRole>>().Setup(m => m.Expression).Returns(jobRoles.Expression);
            mockDbSet.As<IQueryable<JobRole>>().Setup(m => m.ElementType).Returns(jobRoles.ElementType);
            mockDbSet.As<IQueryable<JobRole>>().Setup(m => m.GetEnumerator()).Returns(jobRoles.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.JobRoles).Returns(mockDbSet.Object);

            var target = new AdminRepository(mockDbContext.Object);

            // Act
            var actual = target.GetAllJobroles();

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(jobRoles.Count(), actual.Count());
            mockDbContext.Verify(c => c.JobRoles, Times.Once);

        }

        [Fact]
        public void GetEmployeeData_ReturnsEmployees_WhenemployeesExist()
        {
            // Arrange
            var emps = new List<SPDto>
            {
                new SPDto
                {
                       EmployeeId = 1, EmployeeName = "test1"
                },
                new SPDto {
                        EmployeeId = 2, EmployeeName = "test2"
                }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<SPDto>>();

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.GetEmployeeData(It.IsAny<string>(), It.IsAny<string>())).Returns(mockDbSet.Object);

            var target = new AdminRepository(mockDbContext.Object);

            // Act
            var actual = target.GetEmployeeData(It.IsAny<string>(), It.IsAny<string>());

            // Assert
            Assert.NotNull(actual);
            mockDbContext.Verify(c => c.GetEmployeeData(It.IsAny<string>(), It.IsAny<string>()),Times.Once);

        }


        [Fact]
        public void GetEmployeesByJobRoleAndType_ReturnsFilteredEmployees()
        {
            // Arrange
            var jobRoleId = 123;
            var typeId = 456;

            // Create mock employees with different job roles and types
            var employees = new List<Employee>
            {
                new Employee { EmployeeId = 1, JobRoleId = jobRoleId, TypeId = typeId },
                new Employee { EmployeeId = 2, JobRoleId = 789, TypeId = typeId }, // Different job role
                new Employee { EmployeeId = 3, JobRoleId = jobRoleId, TypeId = 101 }, // Different type
                new Employee { EmployeeId = 4, JobRoleId = 999, TypeId = 888 } // Different job role and type
            }.AsQueryable();
            // Set up the mock DbSet
            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Expression).Returns(employees.Expression);
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Provider).Returns(employees.Provider);
            _mockAppDbContext.Setup(c => c.Employees
                ).Returns(mockDbSet.Object);
            // Act
            var filteredEmployees = _employeeRepository.GetEmployeesByJobRoleAndType(jobRoleId, typeId);

            // Assert
            Assert.NotNull(filteredEmployees);
            Assert.Equal(1, filteredEmployees.Count()); // Only one employee with the specified job role and type
            Assert.Equal(1, filteredEmployees.First().EmployeeId); // Ensure it's the correct employee
        }

        [Fact]
        public void GetEmployeesByDateRangeAndType_ReturnsEmployeesWithEndDate()
        {
            // Arrange
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(3);
            var typeId = 456;

            // Create mock employees with different job roles and types
            var employees = new List<Employee>
            {
                new Employee { EmployeeId = 1, JobRoleId = 1, TypeId = typeId },
                new Employee { EmployeeId = 2, JobRoleId = 2, TypeId = typeId }, // Different job role
                new Employee { EmployeeId = 3, JobRoleId = 3, TypeId = 101 }, // Different type
                new Employee { EmployeeId = 4, JobRoleId = 4, TypeId = 888 } // Different job role and type
            }.AsQueryable();
            // Set up the mock DbSet
            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Expression).Returns(employees.Expression);
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Provider).Returns(employees.Provider);
            _mockAppDbContext.Setup(c => c.Employees
                ).Returns(mockDbSet.Object);
            // Act
            var filteredEmployees = _employeeRepository.GetEmployeesByDateRangeAndType(startDate, endDate, typeId);
            // Assert
            Assert.NotNull(filteredEmployees);

        }


        [Fact]
        public void GetEmployeesByDateRangeAndType_ReturnsEmployeesWithOutEndDate()
        {
            // Arrange
            var startDate = DateTime.Now;
            var typeId = 456;

            // Create mock employees with different job roles and types
            var employees = new List<Employee>
            {
                new Employee { EmployeeId = 1, JobRoleId = 1, TypeId = typeId },
                new Employee { EmployeeId = 2, JobRoleId = 2, TypeId = typeId }, // Different job role
                new Employee { EmployeeId = 3, JobRoleId = 3, TypeId = 101 }, // Different type
                new Employee { EmployeeId = 4, JobRoleId = 4, TypeId = 888 } // Different job role and type
            }.AsQueryable();
            // Set up the mock DbSet
            var mockDbSet = new Mock<DbSet<Employee>>();
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Expression).Returns(employees.Expression);
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Provider).Returns(employees.Provider);
            _mockAppDbContext.Setup(c => c.Employees
                ).Returns(mockDbSet.Object);
            // Act
            var filteredEmployees = _employeeRepository.GetEmployeesByDateRangeAndType(startDate, null, typeId);
            // Assert
            Assert.NotNull(filteredEmployees);

        }

        [Fact]
        public void DeleteContact_ReturnsTrue()
        {
            // Arrange
            var id = 1;
            var emp = new Employee
            {
                EmployeeId = 1 ,
                EmployeeName = "test"
            };
            var empskils = new EmployeeSkills
            {
                EmpId = 1,
                SId = 1
            };
            var mockAppDbContext = new Mock<IAppDbContext>();
            var emps = new List<Employee>() { emp };
            var emps1 = new List<EmployeeSkills>() { new EmployeeSkills {    EmpId = 1,
                SId = 1},
            new EmployeeSkills{   EmpId = 1,
                SId = 2 }
            }.AsQueryable();
            var mockDbSet = new Mock<DbSet<Employee>>();
            
            mockDbSet.Setup(p => p.Find(id)).Returns<object[]>(ids => emps.Find(c => c.EmployeeId == (int)ids[0]));

            var mockDbSet1 = new Mock<DbSet<EmployeeSkills>>();
            mockDbSet1.As<IQueryable<EmployeeSkills>>().Setup(c => c.Provider).Returns(emps1.Provider);
            mockDbSet1.As<IQueryable<EmployeeSkills>>().Setup(c => c.Expression).Returns(emps1.Expression);
            mockDbSet1.As<IQueryable<EmployeeSkills>>().Setup(c => c.ElementType).Returns(emps1.ElementType);



            mockAppDbContext.Setup(x => x.RemoveRange(empskils));
            mockAppDbContext.Setup(x => x.Remove(emp));
            //mockDbSet.As<IQueryable<Employee>>().Setup(c => c.GetEnumerator()).Returns(emps1.GetEnumerator());
            //mockDbSet1.Setup(c => c.RemoveRange)
            mockAppDbContext.SetupGet(c => c.Employees).Returns(mockDbSet.Object);
            mockAppDbContext.SetupGet(c => c.EmployeeSkills).Returns(mockDbSet1.Object);
            mockAppDbContext.Setup(x => x.SaveChanges()).Returns(1);
            var target = new AdminRepository(mockAppDbContext.Object);

            // Act
            var actual = target.Delete(id);

            // Assert
            Assert.True(actual);
            mockDbSet.Verify(p => p.Find(id), Times.Once);
            mockAppDbContext.VerifyGet(c => c.Employees, Times.Exactly(2));
            mockAppDbContext.Verify(x => x.SaveChanges(), Times.Once);
            mockDbSet.Verify(c => c.Remove(emp), Times.Once);
        }

        [Fact]
        public void UpdateSkills_AddSkills_ReturnsTrue()
        {
            //arrange
            var emp = new Employee { EmployeeId = 1, EmployeeName = "test", EmployeeSkills = new List<EmployeeSkills>() };
            var empskils = new EmployeeSkills
            {
                EmpId = 1,
                SId = 1
            };
            var emps1 = new List<EmployeeSkills>() { new EmployeeSkills {    EmpId = 1,
                SId = 1},
            new EmployeeSkills{   EmpId = 1,
                SId = 2 }
            }.AsQueryable();

            var empSkills = new List<string>()
            {
                "mvc","angular"
            };
            
            var mockAppDbContext = new Mock<IAppDbContext>();

            var mockDbSet1 = new Mock<DbSet<EmployeeSkills>>();
            mockDbSet1.As<IQueryable<EmployeeSkills>>().Setup(c => c.Provider).Returns(emps1.Provider);
            mockDbSet1.As<IQueryable<EmployeeSkills>>().Setup(c => c.Expression).Returns(emps1.Expression);
            mockDbSet1.As<IQueryable<EmployeeSkills>>().Setup(c => c.ElementType).Returns(emps1.ElementType);

            mockAppDbContext.Setup(x => x.RemoveRange(empskils));
            mockAppDbContext.SetupGet(c => c.EmployeeSkills).Returns(mockDbSet1.Object);
            var target = new AdminRepository(mockAppDbContext.Object);
            //-------------------------------------------
            var existingSkills = new List<Skill>
            {
                new Skill { Id = 1, SkillName = "mvc" },
                new Skill { Id = 2, SkillName = "angular" }
            }.AsQueryable();

            var mockDbSetSkills = new Mock<DbSet<Skill>>();
            mockDbSetSkills.As<IQueryable<Skill>>().Setup(m => m.Provider).Returns(existingSkills.Provider);
            mockDbSetSkills.As<IQueryable<Skill>>().Setup(m => m.Expression).Returns(existingSkills.Expression);
            mockDbSetSkills.As<IQueryable<Skill>>().Setup(m => m.ElementType).Returns(existingSkills.ElementType);
            mockDbSetSkills.As<IQueryable<Skill>>().Setup(m => m.GetEnumerator()).Returns(existingSkills.GetEnumerator());

            mockAppDbContext.SetupGet(c => c.Skills).Returns(mockDbSetSkills.Object);
            
            // Act
            target.UpdateSkills(emp, empSkills);

            // Assert
            mockDbSetSkills.As<IQueryable<Skill>>().Verify(m => m.Provider);
            mockDbSetSkills.As<IQueryable<Skill>>().Verify(m => m.Expression); 
            mockDbSet1.As<IQueryable<EmployeeSkills>>().Verify(c => c.Provider);
            mockDbSet1.As<IQueryable<EmployeeSkills>>().Verify(c => c.Expression);
        }

        [Fact]
        public void GetEmployeeById_ReturnsException_WhenRetrivalFails()
        {
            //Arrange
            var users = new List<Employee>
            {
            }.AsQueryable();
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.Employees).Throws(new Exception());
            var target = new AdminRepository(mockAbContext.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.GetEmployeeById(1));

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }

        [Fact]
        public void GetPaginatedEmployees_ReturnsException_WhenRetrivalFails()
        {
            //Arrange
            var users = new List<Employee>
            {
            }.AsQueryable();
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.Employees).Throws(new Exception());
            var target = new AdminRepository(mockAbContext.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.GetPaginatedEmployees(1, 4, null, "asc", null));

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }

        [Fact]
        public void UpdateEmployee_ReturnsException_WhenFails()
        {
            //Arrange
            var users = new List<Employee>
            {
            }.AsQueryable();
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.Users).Throws(new Exception());
            var target = new AdminRepository(mockAbContext.Object);
            mockAbContext.Setup(m => m.SaveChanges()).Throws(new Exception());

            var emp = new Employee()
            {
                EmployeeId = 1,
                EmployeeName = "Test",
                JobRoleId = 1,
            };
            // Act
            var exception = Assert.Throws<Exception>(() => target.UpdateEmployee(emp));

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }

        [Fact]
        public void GetAllEmployee_ReturnsException_WhenRetrivalFails()
        {
            //Arrange
            var users = new List<Employee>
            {
            }.AsQueryable();
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.Employees).Throws(new Exception());
            var target = new AdminRepository(mockAbContext.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.GetAllEmployeees());

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }

        [Fact]
        public void TotalEmployees_ReturnsException_WhenRetrivalFails()
        {
            //Arrange
            var users = new List<Employee>
            {
            }.AsQueryable();
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.Employees).Throws(new Exception());
            var target = new AdminRepository(mockAbContext.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.TotalEmployees(null));

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }

        [Fact]
        public void Add_ReturnsException_WhenFails()
        {
            //Arrange
            var users = new List<Employee>
            {
            }.AsQueryable();
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.Employees).Throws(new Exception());
            var target = new AdminRepository(mockAbContext.Object);
            mockAbContext.Setup(m => m.SaveChanges()).Throws(new Exception());

            var emp = new Employee()
            {
                EmployeeId = 1,
                EmployeeName = "Test",
                JobRoleId = 1,
            };
            // Act
            var exception = Assert.Throws<Exception>(() => target.Add(emp));

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }

        [Fact]
        public void UpdateEmployee_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var users = new List<Employee>
            {
            }.AsQueryable();
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.Users).Throws(new Exception());
            var target = new AdminRepository(mockAbContext.Object);
            mockAbContext.Setup(m => m.SaveChanges()).Throws(new Exception());

            var emp = new Employee()
            {
                EmployeeId = 1,
                EmployeeName = "Test",
                JobRoleId = 1,
            };
            // Act
            var exception = Assert.Throws<Exception>(() => target.Update(emp));

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }

        [Fact]
        public void Delete_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var users = new List<Employee>
            {
            }.AsQueryable();
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.Employees).Throws(new Exception());
            var target = new AdminRepository(mockAbContext.Object);
            mockAbContext.Setup(m => m.SaveChanges()).Throws(new Exception());

            var emp = new Employee()
            {
                EmployeeId = 1,
                EmployeeName = "Test",
                JobRoleId = 1,
            };
            // Act
            var exception = Assert.Throws<Exception>(() => target.Delete(1));

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }

        [Fact]
        public void EmployeeNameExists_ReturnsException_WhenRetrivalFails()
        {
            //Arrange
            var users = new List<Employee>
            {
            }.AsQueryable();
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.Employees).Throws(new Exception());
            var target = new AdminRepository(mockAbContext.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.EmployeeNameExists("test"));

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }
        
        [Fact]
        public void EmployeeNameExists2_ReturnsException_WhenRetrivalFails()
        {
            //Arrange
            var users = new List<Employee>
            {
            }.AsQueryable();
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.Employees).Throws(new Exception());
            var target = new AdminRepository(mockAbContext.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.EmployeeNameExists(1,"test"));

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }
        
        [Fact]
        public void EmployeeEmailExists_ReturnsException_WhenRetrivalFails()
        {
            //Arrange
            var users = new List<Employee>
            {
            }.AsQueryable();
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.Employees).Throws(new Exception());
            var target = new AdminRepository(mockAbContext.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.EmployeeEmailExists("test@user.com"));

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }

        [Fact]
        public void EmployeeEmailExists2_ReturnsException_WhenRetrivalFails()
        {
            //Arrange
            var users = new List<Employee>
            {
            }.AsQueryable();
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.Employees).Throws(new Exception());
            var target = new AdminRepository(mockAbContext.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.EmployeeEmailExists(1,"test@user.com"));

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }

        [Fact]
        public void AddSkills_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var users = new List<Employee>
            {
            }.AsQueryable();
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.Users).Throws(new Exception());
            var target = new AdminRepository(mockAbContext.Object);
            mockAbContext.Setup(m => m.SaveChanges()).Throws(new Exception());

            var emp = new Employee()
            {
                EmployeeId = 1,
                EmployeeName = "Test",
                JobRoleId = 1,
            };

            var skillList = new List<string>() { "skill1","skill2"};

            // Act
            var exception = Assert.Throws<Exception>(() => target.AddSkills(emp,skillList));

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }

        [Fact]
        public void UpdateSkills_ReturnsException_WhenEmployeeRetrivalFails()
        {
            //Arrange
            var users = new List<Employee>
            {
            }.AsQueryable();
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.EmployeeSkills).Throws(new Exception());
            var target = new AdminRepository(mockAbContext.Object);
            mockAbContext.Setup(m => m.SaveChanges()).Throws(new Exception());

            var emp = new Employee()
            {
                EmployeeId = 1,
                EmployeeName = "Test",
                JobRoleId = 1,
            };
            var skillList = new List<string>() { "skill1", "skill2" };

            // Act
            var exception = Assert.Throws<Exception>(() => target.UpdateSkills(emp,skillList));

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }

        [Fact]
        public void GetEmployeesByDateRangeAndType_ReturnsException_WhenRetrivalFails()
        {
            //Arrange
            var users = new List<Employee>
            {
            }.AsQueryable();
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.Employees).Throws(new Exception());
            var target = new AdminRepository(mockAbContext.Object);
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(3);

            // Act
            var exception = Assert.Throws<Exception>(() => target.GetEmployeesByDateRangeAndType(startDate,endDate,1));

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }

        [Fact]
        public void GetEmployeesByJobRoleAndType_ReturnsException_WhenRetrivalFails()
        {
            //Arrange
            var users = new List<Employee>
            {
            }.AsQueryable();
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.Employees).Throws(new Exception());
            var target = new AdminRepository(mockAbContext.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.GetEmployeesByJobRoleAndType(1, 1));

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }

        [Fact]
        public void GetAllJobroles_ReturnsException_WhenRetrivalFails()
        {
            //Arrange
            var users = new List<Employee>
            {
            }.AsQueryable();
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.JobRoles).Throws(new Exception());
            var target = new AdminRepository(mockAbContext.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.GetAllJobroles());

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }

        [Fact]
        public void GetEmployeeData_ReturnsException_WhenRetrivalFails()
        {
            //Arrange
            var users = new List<Employee>
            {
            }.AsQueryable();
            var mockAbContext = new Mock<IAppDbContext>();
            var target = new AdminRepository(mockAbContext.Object);
            var startDate = DateTime.Now.ToString();
            mockAbContext.Setup(m => m.GetEmployeeData(It.IsAny<string>(), It.IsAny<string>()))
                     .Throws(new Exception());
            // Act
            var exception = Assert.Throws<Exception>(() => target.GetEmployeeData(startDate,null));

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }

        public void Dispose()
        {
            _mockAppDbContext.VerifyAll();
        }
    }
}
