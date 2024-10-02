using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAllocatorSystemAPI.Migrations
{
    public partial class makeSkillIdNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Skills_SkillId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "SId",
                table: "Employees");

            migrationBuilder.AlterColumn<int>(
                name: "SkillId",
                table: "Employees",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Skills_SkillId",
                table: "Employees",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Skills_SkillId",
                table: "Employees");

            migrationBuilder.AlterColumn<int>(
                name: "SkillId",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SId",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Skills_SkillId",
                table: "Employees",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
