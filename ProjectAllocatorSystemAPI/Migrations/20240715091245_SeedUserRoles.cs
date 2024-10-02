using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAllocatorSystemAPI.Migrations
{
    public partial class SeedUserRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
              table: "UserRoles",
              column: "UserRoleName",
              values: new object[]
              {
                    "Admin"
              });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
               table: "UserRoles",
               keyColumn: "UserRoleId",
               keyValues: new object[]
               {
                    3
               });
        }
    }
}
