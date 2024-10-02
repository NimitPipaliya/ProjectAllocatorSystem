using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAllocatorSystemAPI.Migrations
{
    public partial class AddNewEmployeeskillTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeSkill");

            migrationBuilder.RenameColumn(
                name: "SkillId",
                table: "Skills",
                newName: "Id");

            migrationBuilder.AddColumn<int>(
                name: "SId",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SkillId",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EmployeeSkills",
                columns: table => new
                {
                    EmpId = table.Column<int>(type: "int", nullable: false),
                    SId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSkills", x => new { x.EmpId, x.SId });
                    table.ForeignKey(
                        name: "FK_EmployeeSkills_Employees_EmpId",
                        column: x => x.EmpId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_EmployeeSkills_Skills_SId",
                        column: x => x.SId,
                        principalTable: "Skills",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_SkillId",
                table: "Employees",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSkills_SId",
                table: "EmployeeSkills",
                column: "SId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Skills_SkillId",
                table: "Employees",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Skills_SkillId",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "EmployeeSkills");

            migrationBuilder.DropIndex(
                name: "IX_Employees_SkillId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "SId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "SkillId",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Skills",
                newName: "SkillId");

            migrationBuilder.CreateTable(
                name: "EmployeeSkill",
                columns: table => new
                {
                    EmployeesEmployeeId = table.Column<int>(type: "int", nullable: false),
                    SkillsSkillId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSkill", x => new { x.EmployeesEmployeeId, x.SkillsSkillId });
                    table.ForeignKey(
                        name: "FK_EmployeeSkill_Employees_EmployeesEmployeeId",
                        column: x => x.EmployeesEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeSkill_Skills_SkillsSkillId",
                        column: x => x.SkillsSkillId,
                        principalTable: "Skills",
                        principalColumn: "SkillId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSkill_SkillsSkillId",
                table: "EmployeeSkill",
                column: "SkillsSkillId");
        }
    }
}
