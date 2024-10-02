using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAllocatorSystemAPI.Migrations
{
    public partial class TablesCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SkillId",
                table: "Employees");

            migrationBuilder.CreateTable(
                name: "AllocationTypes",
                columns: table => new
                {
                    TypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllocationTypes", x => x.TypeId);
                });

            migrationBuilder.CreateTable(
                name: "InternalProjects",
                columns: table => new
                {
                    InternalProjectId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternalProjects", x => x.InternalProjectId);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    SkillId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SkillName = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.SkillId);
                });

            migrationBuilder.CreateTable(
                name: "Trainings",
                columns: table => new
                {
                    TrainingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trainings", x => x.TrainingId);
                });

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

            migrationBuilder.CreateTable(
                name: "Allocations",
                columns: table => new
                {
                    AllocationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrainingId = table.Column<int>(type: "int", nullable: false),
                    InternalProjectId = table.Column<int>(type: "int", nullable: false),
                    TypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allocations", x => x.AllocationId);
                    table.ForeignKey(
                        name: "FK_Allocations_AllocationTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "AllocationTypes",
                        principalColumn: "TypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Allocations_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Allocations_InternalProjects_InternalProjectId",
                        column: x => x.InternalProjectId,
                        principalTable: "InternalProjects",
                        principalColumn: "InternalProjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Allocations_Trainings_TrainingId",
                        column: x => x.TrainingId,
                        principalTable: "Trainings",
                        principalColumn: "TrainingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Allocations_EmployeeId",
                table: "Allocations",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Allocations_InternalProjectId",
                table: "Allocations",
                column: "InternalProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Allocations_TrainingId",
                table: "Allocations",
                column: "TrainingId");

            migrationBuilder.CreateIndex(
                name: "IX_Allocations_TypeId",
                table: "Allocations",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSkill_SkillsSkillId",
                table: "EmployeeSkill",
                column: "SkillsSkillId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Allocations");

            migrationBuilder.DropTable(
                name: "EmployeeSkill");

            migrationBuilder.DropTable(
                name: "AllocationTypes");

            migrationBuilder.DropTable(
                name: "InternalProjects");

            migrationBuilder.DropTable(
                name: "Trainings");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.AddColumn<int>(
                name: "SkillId",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
