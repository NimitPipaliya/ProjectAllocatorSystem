using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAllocatorSystemAPI.Migrations
{
    public partial class seedSecurityQuestions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
              table: "SecurityQuestions",
              column: "SecurityQuestionName",
              values: new object[]
              {
                    "What is your favourite colour?",
                    "What is your Birth City?",
                    "What is your Favourite holiday destination?",
              });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
               table: "SecurityQuestions",
               keyColumn: "SecurityQuestionId",
               keyValues: new object[]
               {
                    1, 2, 3
               });
        }
    }
}
