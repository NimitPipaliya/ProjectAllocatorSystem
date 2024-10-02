using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAllocatorSystemAPI.Migrations
{
    public partial class seedInternalProjectTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
              table: "InternalProjects",
              columns: new[] { "Name", "Description" },
              values: new object[,]
              {
                    { "select", "des"},
                    { "Civica Employee Master","Mangement system for employees" },
                    { "Civica Shopping App" , "Management system for products"},
                    { "Civica Book Library" ,"Management system for library"},
                    { "Army Enrollment App" ,"Management system for army"},
              });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
               table: "InternalProjects",
               keyColumn: "InternalProjectId",
                keyValues: new object[]
                {
                    1, 2, 3 , 4, 5,
                });
        }
    }
}
