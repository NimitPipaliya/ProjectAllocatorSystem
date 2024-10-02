using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAllocatorSystemAPI.Migrations
{
    public partial class JobRoleTableCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobRoles",
                columns: table => new
                {
                    JobRoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobRoleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobRoles", x => x.JobRoleId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_JobRoleId",
                table: "Employees",
                column: "JobRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_JobRoles_JobRoleId",
                table: "Employees",
                column: "JobRoleId",
                principalTable: "JobRoles",
                principalColumn: "JobRoleId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_JobRoles_JobRoleId",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "JobRoles");

            migrationBuilder.DropIndex(
                name: "IX_Employees_JobRoleId",
                table: "Employees");
        }
    }
}
