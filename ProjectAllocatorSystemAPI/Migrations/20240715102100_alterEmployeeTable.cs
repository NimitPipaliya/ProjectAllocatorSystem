using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAllocatorSystemAPI.Migrations
{
    public partial class alterEmployeeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_TypeId",
                table: "Employees",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_AllocationTypes_TypeId",
                table: "Employees",
                column: "TypeId",
                principalTable: "AllocationTypes",
                principalColumn: "TypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_AllocationTypes_TypeId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_TypeId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Employees");
        }
    }
}
