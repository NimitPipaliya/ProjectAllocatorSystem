using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProjectAllocatorSystemAPI.Dtos;
using ProjectAllocatorSystemAPI.Models;
using System.Diagnostics.CodeAnalysis;

namespace ProjectAllocatorSystemAPI.Data
{
    [ExcludeFromCodeCoverage]
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
         : base(options)
        {
        }

        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Skill> Skills { get; set; }
        public virtual DbSet<Allocation> Allocations { get; set; }
        public virtual DbSet<Training> Trainings { get; set; }
        public virtual DbSet<InternalProject> InternalProjects { get; set; }
        public virtual DbSet<JobRole> JobRoles { get; set; }
        public virtual DbSet<AllocationType> AllocationTypes { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<SecurityQuestion> SecurityQuestions { get; set; }
        public virtual DbSet<EmployeeSkills> EmployeeSkills { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
             .HasOne(d => d.JobRole)
             .WithMany(p => p.Employees)
             .HasForeignKey(d => d.JobRoleId);

            modelBuilder.Entity<EmployeeSkills>()
                .HasKey(s => new { s.EmpId, s.SId });

            modelBuilder.Entity<EmployeeSkills>()
             .HasOne(d => d.Employees)
             .WithMany(p => p.EmployeeSkills)
             .HasForeignKey(d => d.EmpId)
             .OnDelete(DeleteBehavior.NoAction);    

            modelBuilder.Entity<EmployeeSkills>()
             .HasOne(d => d.Skill)
             .WithMany(p => p.EmployeeSkills)
             .HasForeignKey(d => d.SId)
             .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Allocations)
                .WithOne(e => e.Employee)
                .HasForeignKey(e => e.EmployeeId).IsRequired();

            modelBuilder.Entity<Allocation>()
                .HasOne(e => e.Training)
                .WithMany().HasForeignKey(e => e.TrainingId);

            modelBuilder.Entity<Allocation>()
                .HasOne(e => e.InternalProject)
                .WithMany().HasForeignKey(e => e.InternalProjectId);

            modelBuilder.Entity<Allocation>()
                .HasOne(e => e.AllocationType)
                .WithMany(e => e.Allocations)
                .HasForeignKey(e => e.TypeId);

            modelBuilder.Entity<Employee>()
                .HasOne(d => d.Allocationtype)
                .WithMany(p => p.Employees)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasOne(u => u.SecurityQuestion)
                .WithMany()
                .HasForeignKey(u => u.SecurityQuestionId)
                .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasOne(u => u.UserRole)
                .WithMany()
                .HasForeignKey(u => u.UserRoleId)
                .OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<SPDto>().HasNoKey().ToView(null);

        }
        public virtual IQueryable<SPDto> GetEmployeeData(string startDate, string? enddate)
        {

            //var startdateParam = new SqlParameter("@StartDate", startDate);
            //var enddateParam = new SqlParameter("@EndDate", enddate);
            SqlParameter[] parameters = new SqlParameter[2];
            parameters[0] = new SqlParameter("@StartDate", startDate);

            // Check if enddate is null and set the SqlParameter accordingly
            if (enddate == null)
            {
                parameters[1] = new SqlParameter("@EndDate", DBNull.Value); // DBNull.Value represents SQL NULL
            }
            else
            {
                parameters[1] = new SqlParameter("@EndDate", enddate);
            }



            return Set<SPDto>().FromSqlRaw("dbo.GetEmployeeData @StartDate,@EndDate", parameters);
        }
        public EntityState GetEntryState<TEntity>(TEntity entity) where TEntity : class
        {
            return Entry(entity).State;
        }
        public void SetEntryState<TEntity>(TEntity entity, EntityState entityState) where TEntity : class
        {
            Entry(entity).State = entityState;
        }
    }
}
