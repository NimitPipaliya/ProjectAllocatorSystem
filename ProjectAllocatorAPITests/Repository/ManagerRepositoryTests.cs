using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
    public class ManagerRepositoryTests:IDisposable
    {
        private readonly Mock<IAppDbContext> _dbContextMock;

        public ManagerRepositoryTests()
        {
            // Set up your mock DbContext
            _dbContextMock = new Mock<IAppDbContext>();
            //_employeeRepository = new ManagerRepository(_dbContextMock.Object);
        }
        [Fact]
        public void GetAllEmployeees_ReturnsEmployeesWithIncludes()
        {
            // Arrange
            var employeesData = new List<Employee>
            {
                new Employee { EmployeeId = 1, EmployeeName = "John" },
                new Employee { EmployeeId = 2, EmployeeName = "Jane" }
            }.AsQueryable();
            var mockDbSet = new Mock<DbSet<Employee>>();
            var dbContextMock = new Mock<IAppDbContext>();
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Provider).Returns(employeesData.Provider);
            mockDbSet.As<IQueryable<Employee>>().Setup(c => c.Expression).Returns(employeesData.Expression);
            dbContextMock.Setup(c => c.Employees).Returns(mockDbSet.Object);

            var employeeRepository = new ManagerRepository(dbContextMock.Object);

            // Act
            var result = employeeRepository.GetAllEmployee();

            // Assert
            var employeesWithIncludes = result.ToList();
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Provider);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Expression);
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

            _dbContextMock.Setup(c => c.Employees).Returns(mockDbSet.Object);
            var target = new ManagerRepository(_dbContextMock.Object);
            //Act
            var actual = target.GetPaginatedEmployees(1, 2, null, sortOrder, sortBy);
            //Assert
            Assert.NotNull(actual);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Expression, Times.Exactly(1));
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Provider, Times.Exactly(2));
            _dbContextMock.Verify(c => c.Employees, Times.Once);
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

            _dbContextMock.Setup(c => c.Employees).Returns(mockDbSet.Object);
            var target = new ManagerRepository(_dbContextMock.Object);
            //Act
            var actual = target.GetPaginatedEmployees(1, 2, search, sortOrder, sortBy);
            //Assert
            Assert.NotNull(actual);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Provider, Times.Exactly(2));
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Expression, Times.Once);
            _dbContextMock.Verify(c => c.Employees, Times.Once);
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

            _dbContextMock.Setup(c => c.Employees).Returns(mockDbSet.Object);
            var target = new ManagerRepository(_dbContextMock.Object);
            //Act
            var actual = target.GetPaginatedEmployees(1, 2, null, sortOrder, sortBy);
            //Assert
            Assert.NotNull(actual);
            Assert.Empty(actual);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Expression, Times.Once);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Provider, Times.Exactly(2));
            _dbContextMock.Verify(c => c.Employees, Times.Once);
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

            _dbContextMock.Setup(c => c.Employees).Returns(mockDbSet.Object);
            var target = new ManagerRepository(_dbContextMock.Object);
            //Act
            var actual = target.GetPaginatedEmployees(1, 2, search, sortOrder, sortBy);
            //Assert
            Assert.NotNull(actual);
            Assert.Empty(actual);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Expression, Times.Once);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Provider, Times.Exactly(2));
            _dbContextMock.Verify(c => c.Employees, Times.Once);
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

            _dbContextMock.Setup(c => c.Employees).Returns(mockDbSet.Object);
            var employeeRepository = new ManagerRepository(_dbContextMock.Object);
            // Act
            var actual = employeeRepository.GetPaginatedEmployees(1, 2, null, "asc", "name");

            // Assert
            Assert.NotNull(actual);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Expression);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Provider);
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

            _dbContextMock.Setup(c => c.Employees).Returns(mockDbSet.Object);
            var employeeRepository = new ManagerRepository(_dbContextMock.Object);
            // Act
            var actual = employeeRepository.GetPaginatedEmployees(1, 2, null, "desc", "jobrole");

            // Assert
            Assert.NotNull(actual);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Expression);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Provider);
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

            _dbContextMock.Setup(c => c.Employees).Returns(mockDbSet.Object);
            var employeeRepository = new ManagerRepository(_dbContextMock.Object);
            // Act
            var actual = employeeRepository.GetPaginatedEmployees(1, 2, null, "desc", "startDate");

            // Assert
            Assert.NotNull(actual);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Expression);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Provider);
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

            _dbContextMock.Setup(c => c.Employees).Returns(mockDbSet.Object);
            var employeeRepository = new ManagerRepository(_dbContextMock.Object);
            // Act
            var actual = employeeRepository.GetPaginatedEmployees(1, 2, null, "desc", "endDate");

            // Assert
            Assert.NotNull(actual);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Expression);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Provider);
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
            var target = new ManagerRepository(mockAppDbContext.Object);

            //Act
            var actual = target.TotalEmployees(null);

            //Assert
            Assert.NotNull(actual);
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
            var target = new ManagerRepository(mockAppDbContext.Object);

            //Act
            var actual = target.TotalEmployees(search);

            //Assert
            Assert.NotNull(actual);
            mockDbSet.As<IQueryable<Employee>>().Verify(c => c.Provider, Times.Exactly(1));
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
            var target = new ManagerRepository(mockAppDbContext.Object);

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
            var target = new ManagerRepository(mockAppDbContext.Object);
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
        public void UpdateEmployee_ReturnsTrue()
        {
            //Arrange
            var mockDbSet = new Mock<DbSet<Employee>>();
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.SetupGet(c => c.Employees).Returns(mockDbSet.Object);
            mockAppDbContext.Setup(c => c.SaveChanges()).Returns(1);
            var target = new ManagerRepository(mockAppDbContext.Object);
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
            var target = new ManagerRepository(mockAbContext.Object);

            //Act
            var actual = target.UpdateEmployee(employee);
            //Assert
            Assert.False(actual);
        }
        
        [Fact]
        public void GetAllocationByEmployeeById_retunsEmployee()
        {
            //Arrange
            var mockAppDbContext = new Mock<IAppDbContext>();
            var emps1 = new List<Allocation>()
            {
                new Allocation
                {
                   EmployeeId = 1,
                   TypeId = 1,
                   AllocationType = new AllocationType
                    {
                        TypeId = 2
                    },
                },
                new Allocation
                {
                    EmployeeId = 2,
                    TypeId = 2 ,
                    AllocationType = new AllocationType
                    {
                        TypeId = 2
                    },
                }
            }.AsQueryable();
            var mockDbSet1 = new Mock<DbSet<Allocation>>();
            mockDbSet1.As<IQueryable<Allocation>>().Setup(c => c.Provider).Returns(emps1.Provider);
            mockDbSet1.As<IQueryable<Allocation>>().Setup(c => c.Expression).Returns(emps1.Expression);
            mockDbSet1.As<IQueryable<Allocation>>().Setup(c => c.ElementType).Returns(emps1.ElementType);

            mockAppDbContext.SetupGet(c => c.Allocations).Returns(mockDbSet1.Object);

            var target = new ManagerRepository(mockAppDbContext.Object);

            //Act
            var actual = target.GetAllocationByEmployeeById(1);

            //Assert
            mockDbSet1.As<IQueryable<Allocation>>().Verify(m => m.Provider);
            mockDbSet1.As<IQueryable<Allocation>>().Verify(m => m.Expression);
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
            var target = new ManagerRepository(mockAbContext.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.GetAllEmployee());

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
            var target = new ManagerRepository(mockAbContext.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.GetPaginatedEmployees(1,4,null,"asc",null));

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
            var target = new ManagerRepository(mockAbContext.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.TotalEmployees(null));

            // Assert
            Assert.Equal("Exception of type 'System.Exception' was thrown.", exception.Message);
        }

        [Fact]
        public void GetAllocationByEmployeeById_ReturnsException_WhenRetrivalFails()
        {
            //Arrange
            var users = new List<Employee>
            {
            }.AsQueryable();
            var mockAbContext = new Mock<IAppDbContext>();
            mockAbContext.SetupGet(c => c.Allocations).Throws(new Exception());
            var target = new ManagerRepository(mockAbContext.Object);

            // Act
            var exception = Assert.Throws<Exception>(() => target.GetAllocationByEmployeeById(1));

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
            var target = new ManagerRepository(mockAbContext.Object);
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

        public void Dispose()
        {
            _dbContextMock.VerifyAll();
        }

    }
}
