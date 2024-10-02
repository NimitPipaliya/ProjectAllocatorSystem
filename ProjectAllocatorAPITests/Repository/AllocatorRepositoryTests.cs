using Microsoft.EntityFrameworkCore;
using Moq;
using ProjectAllocatorSystemAPI.Data;
using ProjectAllocatorSystemAPI.Data.Implementation;
using ProjectAllocatorSystemAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAllocatorAPITests.Repository
{
    public class AllocatorRepositoryTests:IDisposable
    {
        private readonly Mock<IAppDbContext> _mockAppDbContext;
        private readonly AllocatorRepository _employeeRepository;

        public AllocatorRepositoryTests()
        {
            // Arrange: Create a mock for IAppDbContext
            _mockAppDbContext = new Mock<IAppDbContext>();

            // Arrange: Create an instance of the repository with the mock context
            _employeeRepository = new AllocatorRepository(_mockAppDbContext.Object);
        }
        [Fact]
        public void InsertEmployee_ValidEmployee_ReturnsTrue()
        {
            // Arrange
            var employee = new Allocation
            {

                TypeId = 1,
                EmployeeId = 3,
                Details = "qwert",
                // Add other properties as needed
            };

            var mockDbSet = new Mock<DbSet<Allocation>>();
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Allocations).Returns(mockDbSet.Object);

            var repository = new AllocatorRepository(mockDbContext.Object);

            // Act
            var result = repository.InsertAllocation(employee);

            // Assert
            Assert.True(result); // Ensure method returns true on successful insert

            // Verify that Add and SaveChanges were called
            mockDbSet.Verify(m => m.Add(employee), Times.Once);
            mockDbContext.Verify(m => m.SaveChanges(), Times.Once);
        }
        [Fact]
        public void InsertEmployee_NullEmployee_ReturnsFalse()
        {
            // Arrange
            Allocation employee = null;

            var mockDbSet = new Mock<DbSet<Allocation>>();
            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Allocations).Returns(mockDbSet.Object);

            var repository = new AllocatorRepository(mockDbContext.Object);

            // Act
            var result = repository.InsertAllocation(employee);

            // Assert
            Assert.False(result); // Ensure method returns false when employee is null

            // Verify that Add and SaveChanges were not called
            mockDbSet.Verify(m => m.Add(It.IsAny<Allocation>()), Times.Never);
            mockDbContext.Verify(m => m.SaveChanges(), Times.Never);
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
            var target = new AllocatorRepository(_mockAppDbContext.Object);
            //Act
            var actual = target.GetPaginatedEmployees(1, 2, null, sortOrder,sortBy);
            //Assert
            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count());
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Expression, Times.Once);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Provider, Times.Exactly(3));
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
            var target = new AllocatorRepository(_mockAppDbContext.Object);
            //Act
            var actual = target.GetPaginatedEmployees(1, 2, search, sortOrder, sortBy);
            //Assert
            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count());
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Provider, Times.Exactly(3));
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
            var target = new AllocatorRepository(_mockAppDbContext.Object);
            //Act
            var actual = target.GetPaginatedEmployees(1, 2, null, sortOrder,sortBy);
            //Assert
            Assert.NotNull(actual);
            Assert.Empty(actual);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Expression, Times.Once);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Provider, Times.Exactly(3));
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
            var target = new AllocatorRepository(_mockAppDbContext.Object);
            //Act
            var actual = target.GetPaginatedEmployees(1, 2, search, sortOrder, sortBy);
            //Assert
            Assert.NotNull(actual);
            Assert.Empty(actual);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Expression, Times.Once);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Provider, Times.Exactly(3));
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
            var target = new AllocatorRepository(_mockAppDbContext.Object);
            //Act
            var actual = target.GetEmployeeById(id);
            //Assert
            Assert.Null(actual);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Provider, Times.Exactly(3));
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
            var target = new AllocatorRepository(_mockAppDbContext.Object);
            //Act
            var actual = target.GetEmployeeById(id);
            //Assert
            Assert.NotNull(actual);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Provider, Times.Exactly(3));
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Expression, Times.Once);
            _mockAppDbContext.VerifyGet(c => c.Employees, Times.Once);

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
            var target = new AllocatorRepository(mockAbContext.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.GetPaginatedEmployees(1, 4, null, "asc", null));

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
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
            var target = new AllocatorRepository(mockAbContext.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.GetEmployeeById(1));

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }

        [Fact]
        [Trait("Auth", "AuthRepositoryTests")]
        public void InsertAllocation_ReturnsException_WhenUpdateFails()
        {
            //Arrange
            var users = new List<UserRole>
            {
            }.AsQueryable();
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.Users).Throws(new Exception());
            var target = new AllocatorRepository(mockAbContext.Object);
            var allocation = new Allocation()
            {
                AllocationId = 1,
                TrainingId = 1,
            };

            // Act
            var exception = Assert.Throws<Exception>(() => target.InsertAllocation(allocation));

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }

        public void Dispose()
        {
            _mockAppDbContext.VerifyAll();
        }
    }
}
