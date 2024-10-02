using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAllocatorSystemAPI.Migrations
{
    public partial class AlterColumnsInAllocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
             name: "TrainingId",
             table: "Allocations",
             type: "int",
             nullable: true,
             oldClrType: typeof(int),
             oldType: "int");

            migrationBuilder.AlterColumn<int>(
            name: "InternalProjectId",
            table: "Allocations",
            type: "int",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
            name: "TrainingId",
            table: "Allocations");

            migrationBuilder.DropColumn(
            name: "InternalProjectId",
            table: "Allocations");
        }
    }
}
