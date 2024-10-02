using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAllocatorSystemAPI.Migrations
{
    public partial class SeedJobRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
              table: "JobRoles",
              column: "JobRoleName",
              values: new object[]
              {
                    "Developer",
                    "Tester",
                    "Business Analyst",
              });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
               table: "JobRoles",
               keyColumn: "JobRoleId",
               keyValues: new object[]
               {
                    1, 2, 3
               });
        }
    }
}
