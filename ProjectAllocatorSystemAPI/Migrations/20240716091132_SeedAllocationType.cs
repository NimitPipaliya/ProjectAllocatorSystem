using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAllocatorSystemAPI.Migrations
{
    public partial class SeedAllocationType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
              table: "AllocationTypes",
              column: "Type",
              values: new object[]
              {
                    "Bench",
                    "Customer Project",
              });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
               table: "AllocationTypes",
               keyColumn: "TypeId",
               keyValues: new object[]
               {
                    1, 2
               });
        }
    }
}
