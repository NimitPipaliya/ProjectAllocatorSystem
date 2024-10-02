using Microsoft.EntityFrameworkCore;
using ProjectAllocatorSystemAPI.Data.Contract;
using ProjectAllocatorSystemAPI.Models;
using System.Diagnostics.CodeAnalysis;

namespace ProjectAllocatorSystemAPI.Data.Implementation
{
    public class AllocatorRepository:IAllocatorRepository
    {
        private readonly IAppDbContext _appDbContext;

        public AllocatorRepository(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public IEnumerable<Employee> GetPaginatedEmployees(int page, int pageSize, string? search, string sortOrder, string? sortBy)
        {
            try
            {
                int skip = (page - 1) * pageSize;
                IQueryable<Employee> employees = _appDbContext.Employees.Include(c => c.JobRole).Include(c => c.Allocationtype);
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
        public Employee? GetEmployeeById(int id)
        {
            try
            {
                var employee = _appDbContext.Employees.Include(c => c.JobRole).Include(c => c.Allocationtype)
                .FirstOrDefault(c => c.EmployeeId == id);
                return employee;
            }
            catch
            {
                throw new Exception();
            }
        }

        public bool InsertAllocation(Allocation allocation)
        {
            try
            {
                var result = false;
                if (allocation != null)
                {
                    _appDbContext.Allocations.Add(allocation);
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
