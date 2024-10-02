using Microsoft.EntityFrameworkCore;
using ProjectAllocatorSystemAPI.Data.Contract;
using ProjectAllocatorSystemAPI.Models;
using System.Diagnostics.CodeAnalysis;

namespace ProjectAllocatorSystemAPI.Data.Implementation
{
    public class ManagerRepository:IManagerRepository
    {
        private readonly IAppDbContext _appDbContext;
        public ManagerRepository(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            try
            {
                List<Employee> employees = _appDbContext.Employees.Where(e => e.TypeId == 1).ToList();
                return employees;
            }
            catch
            {
                throw new Exception();
            }
        }

        public IEnumerable<Employee> GetPaginatedEmployees(int page, int pageSize, string? search, string sortOrder, string? sortBy)
        {
            try
            {
                int skip = (page - 1) * pageSize;
                IQueryable<Employee> employees = _appDbContext.Employees.Include(c => c.JobRole).Where(e => e.TypeId == 1);
                if (!string.IsNullOrEmpty(search))
                {
                    employees = employees.Where(c => c.EmployeeName.Contains(search));
                }
                switch (sortBy?.ToLower())
                {
                    case "name":
                        employees = sortOrder.ToLower() == "desc" ? employees.OrderByDescending(b => b.EmployeeName) : employees.OrderBy(b => b.EmployeeName);
                        break;
                    case "jobrole":
                        employees = sortOrder.ToLower() == "desc" ? employees.OrderByDescending(b => b.JobRole.JobRoleName) : employees.OrderBy(b => b.JobRole.JobRoleName);
                        break;
                    case "startdate":
                        employees = sortOrder.ToLower() == "desc" ? employees.OrderByDescending(b => b.BenchStartDate) : employees.OrderBy(b => b.BenchStartDate);
                        break;
                    case "enddate":
                        employees = sortOrder.ToLower() == "desc" ? employees.OrderByDescending(b => b.BenchEndDate) : employees.OrderBy(b => b.BenchEndDate);
                        break;
                    default:
                        employees = employees.OrderBy(b => b.EmployeeName);
                        break;
                }
                return employees
                .Skip(skip)
                    .Take(pageSize)
                    .ToList();
            }
            catch
            {
                throw new Exception();
            }
        }
        public int TotalEmployees(string? search)
        {
            try
            {
                IQueryable<Employee> employees = _appDbContext.Employees.Where(c=>c.TypeId == 1);

                if (!string.IsNullOrEmpty(search))
                {
                    employees = employees.Where(c => c.EmployeeName.Contains(search));
                }
                return employees.Count();
            }
            catch
            {
                throw new Exception();
            }
        }

        public Allocation? GetAllocationByEmployeeById(int id)
        {
            try
            {
                var employee = _appDbContext.Allocations.Include(c=>c.Employee).Include(c=> c.Training).Include(c=>c.InternalProject).OrderBy(c=>c.AllocationId).Where(c=>c.AllocationType.TypeId==1).LastOrDefault(c=>c.EmployeeId==id);
                return employee;
            }
            catch
            {
                throw new Exception();
            }
        }

        public bool UpdateEmployee(Employee employee)
        {
            try
            {
                var result = false;
                if (employee != null)
                {
                    _appDbContext.Employees.Update(employee);
                    _appDbContext.SaveChanges();
                    result = true;
                }
                return result;
            }
            catch
            {
                throw new Exception();
            }
        }
    }
}
