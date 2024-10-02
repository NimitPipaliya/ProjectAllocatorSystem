using Microsoft.EntityFrameworkCore;
using ProjectAllocatorSystemAPI.Data.Contract;
using ProjectAllocatorSystemAPI.Dtos;
using ProjectAllocatorSystemAPI.Models;
using System.Diagnostics.CodeAnalysis;

namespace ProjectAllocatorSystemAPI.Data.Implementation
{
    public class AdminRepository:IAdminRepository
    {
        private readonly IAppDbContext _appDbContext;

        public AdminRepository(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        
        public Employee? GetEmployeeById(int id)
        {
            try
            {
                var employee = _appDbContext.Employees.Include(c => c.JobRole).Include(c => c.Allocationtype).Include(s=>s.EmployeeSkills).ThenInclude(s=>s.Skill)
                .FirstOrDefault(c => c.EmployeeId == id);
                return employee;
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
                IQueryable<Employee> employees = _appDbContext.Employees.Include(c => c.JobRole).Include(c => c.Allocationtype).Include(s => s.EmployeeSkills).ThenInclude(s => s.Skill);
                if (!string.IsNullOrEmpty(search))
                {
                    employees = employees.Where(c => c.EmployeeName.Contains(search));
                }
                switch (sortBy)
                {
                    case "name":
                        employees = sortOrder.ToLower() == "desc" ? employees.OrderByDescending(b => b.EmployeeName) : employees.OrderBy(b => b.EmployeeName);
                        break;
                    case "jobRole":
                        employees = sortOrder.ToLower() == "desc" ? employees.OrderByDescending(b => b.JobRole.JobRoleName) : employees.OrderBy(b => b.JobRole.JobRoleName);
                        break;
                    //case "startDate":
                    //    employees = sortOrder.ToLower() == "desc" ? employees.OrderByDescending(b => b.BenchStartDate) : employees.OrderBy(b => b.BenchStartDate);
                    //    break;
                    //case "endDate":
                    //    employees = sortOrder.ToLower() == "desc" ? employees.OrderByDescending(b => b.BenchEndDate) : employees.OrderBy(b => b.BenchEndDate);
                    //    break;
                    case "allocationStatus":
                        employees = sortOrder.ToLower() == "desc" ? employees.OrderByDescending(b => b.Allocationtype.Type) : employees.OrderBy(b => b.Allocationtype.Type);
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

        public IEnumerable<Employee> GetAllEmployeees()
        {
            try
            {
                IEnumerable<Employee> employees = _appDbContext.Employees.Include(c => c.JobRole).Include(c => c.Allocationtype).Include(s => s.EmployeeSkills).ThenInclude(s => s.Skill).ToList();
                return employees;
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
                IQueryable<Employee> employees = _appDbContext.Employees;

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
        public bool Add(Employee employee)
        {
            try
            {
                var result = false;
                if (employee != null)
                {
                    _appDbContext.Employees.Add(employee);
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
        public bool Update(Employee employee)
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
        public bool Delete(int id)
        {
            try
            {
                var result = false;
                var employee = _appDbContext.Employees.Find(id);
                if (employee != null)
                {
                    var existingskills = _appDbContext.EmployeeSkills.Where(s => s.EmpId == employee.EmployeeId).ToList();
                    _appDbContext.EmployeeSkills.RemoveRange(existingskills);
                    _appDbContext.Employees.Remove(employee);
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

        public bool EmployeeNameExists(string name)
        {
            try
            {
                var employees = _appDbContext.Employees.FirstOrDefault(c => c.EmployeeName.ToLower() == name.ToLower());
                if (employees != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                throw new Exception();
            }
        }
        public bool EmployeeEmailExists(string emailId)
        {
            try
            {
                var employees = _appDbContext.Employees.FirstOrDefault(c => c.EmailId.ToLower() == emailId.ToLower());
                if (employees != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                throw new Exception();
            }
        }
        public bool EmployeeNameExists(int id, string name)
        {
            try
            {
                var employee = _appDbContext.Employees.FirstOrDefault(c => c.EmployeeName.ToLower() == name.ToLower() && c.EmployeeId != id);
                if (employee != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                throw new Exception();
            }
        }
        public bool EmployeeEmailExists(int id, string emailId)
        {
            try
            {
                var employee = _appDbContext.Employees.FirstOrDefault(c => c.EmailId.ToLower() == emailId.ToLower() && c.EmployeeId != id);
                if (employee != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                throw new Exception();
            }
        }
        public void AddSkills(Employee employee, List<string> skillName)
        {
            try
            {
                foreach (var skill in skillName)
                {
                    var existingSkill = _appDbContext.Skills.FirstOrDefault(s => s.SkillName == skill);
                    if (existingSkill == null)
                    {
                        existingSkill = new Skill { SkillName = skill };
                        _appDbContext.Skills.Add(existingSkill);
                        _appDbContext.SaveChanges();
                    }
                    if(employee.EmployeeSkills == null)
                    {
                        employee.EmployeeSkills = new List<EmployeeSkills>();
                    }
                    employee.EmployeeSkills.Add(new EmployeeSkills
                    {
                        Employees = employee,
                        Skill = existingSkill
                    });
                }
                _appDbContext.SaveChanges();
            }
            catch
            {
                throw new Exception();
            }
        }

        public void UpdateSkills(Employee employee,List<string> skillNames)
        {
            try
            {
                var existingskills = _appDbContext.EmployeeSkills.Where(s => s.EmpId == employee.EmployeeId).ToList();
                _appDbContext.EmployeeSkills.RemoveRange(existingskills);

                AddSkills(employee, skillNames);
            }
            catch
            {
                throw new Exception();
            }
        }

        public IEnumerable<Employee> GetEmployeesByDateRangeAndType(DateTime startDate, DateTime? endDate, int typeId)
        {
            try
            { 
                if (!endDate.HasValue)
                {
                return _appDbContext.Employees
                .Include(c => c.JobRole)
                .Include(c => c.Allocationtype)
                .Include(c => c.Allocations)
                .Where(c => c.BenchStartDate >= startDate && c.TypeId == typeId)
                .ToList();

            }
                return _appDbContext.Employees
                    .Include(c => c.JobRole)
                    .Include(c => c.Allocationtype)
                    .Include(c => c.Allocations)
                    .Where(c => c.BenchStartDate >= startDate && c.BenchEndDate <= endDate && c.TypeId == typeId)
                    .ToList();
            }
            catch
            {
                throw new Exception();
            }
        }

        public IEnumerable<Employee> GetEmployeesByJobRoleAndType(int jobRoleId, int typeId)
        {
            try
            {
                return _appDbContext.Employees
                .Include(c => c.JobRole)
                .Include(c => c.Allocationtype)
                .Where(c => c.JobRoleId == jobRoleId && c.TypeId == typeId)
                .ToList();
            }
            catch
            {
                throw new Exception();
            }
        }

        public IEnumerable<JobRole> GetAllJobroles()
        {
            try
            {
                IEnumerable<JobRole> employees = _appDbContext.JobRoles.ToList();
                return employees;
            }
            catch
            {
                throw new Exception();
            }
        }

        public IEnumerable<SPDto> GetEmployeeData(string startDate, string? enddate)
        {
            try
            {
                var result = _appDbContext.GetEmployeeData(startDate, enddate);
                return result;
            }
            catch
            {
                throw new Exception();
            }
        }

    }
}
