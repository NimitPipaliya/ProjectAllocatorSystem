using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAllocatorSystemAPI.Migrations
{
    public partial class StoredProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR ALTER PROCEDURE GetEmployeeData (
    @StartDate DATE,
    @EndDate DATE
)
AS
BEGIN
     -- Select data from the employee table
    SELECT 
        e.employeeId, 
        e.EmployeeName, 
        e.typeId, 
        e.BenchStartDate, 
        e.BenchEndDate, 
        a.TrainingId,
		t.Name as TrainingName,
		t.Description as TrainingDescription,
		i.Name as ProjectName,
		i.Description as ProjectDescription
    FROM 
        Employees e
    LEFT JOIN 
        (
            SELECT 
                a1.employeeId, 
                a1.trainingId,
				a1.InternalProjectId
            FROM 
                Allocations a1
            INNER JOIN
                (
                    SELECT 
                        employeeId, 
                        MAX(allocationId) AS latestAllocationId
                    FROM 
                        Allocations
                    GROUP BY 
                        employeeId
                ) latestAllocation 
                ON a1.employeeId = latestAllocation.employeeId 
                AND a1.allocationId = latestAllocation.latestAllocationId
        ) a ON e.employeeId = a.employeeId
		LEFT join 
		Trainings t on a.TrainingId = t.TrainingId
		LEFT join
		InternalProjects i on a.InternalProjectId =i.InternalProjectId
    WHERE 
        e.typeId = 1
        AND (e.BenchStartDate >= @StartDate
		or e.BenchEndDate >= @StartDate)
        AND (@EndDate is null or e.BenchEndDate<=@EndDate);
END");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS GetEmployeeData");
        }
    }
}
