using Microsoft.EntityFrameworkCore;
using ProjectAllocatorSystemAPI.Dtos;
using ProjectAllocatorSystemAPI.Models;

namespace ProjectAllocatorSystemAPI.Data
{
    public interface IAppDbContext : IDbContext
    {
         DbSet<Employee> Employees { get; set; }
         DbSet<Skill> Skills { get; set; }
         DbSet<Allocation> Allocations { get; set; }
         DbSet<Training> Trainings { get; set; }
         DbSet<InternalProject> InternalProjects { get; set; }

         DbSet<JobRole> JobRoles { get; set; }
         DbSet<AllocationType> AllocationTypes { get; set; }
         DbSet<User> Users { get; set; }
         DbSet<UserRole> UserRoles { get; set; }
         DbSet<SecurityQuestion> SecurityQuestions { get; set; }
         DbSet<EmployeeSkills> EmployeeSkills { get; set; }
        IQueryable<SPDto> GetEmployeeData(string startDate, string enddate);
    }
}
