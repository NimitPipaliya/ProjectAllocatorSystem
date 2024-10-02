using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAllocatorSystemAPI.Migrations
{
    public partial class SeedUserRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
              table: "UserRoles",
              column: "UserRoleName",
              values: new object[]
              {
                    "Allocator",
                    "Manager",
              });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
               table: "UserRoles",
               keyColumn: "UserRoleId",
               keyValues: new object[]
               {
                    1, 2
               });
        }
    }
}
