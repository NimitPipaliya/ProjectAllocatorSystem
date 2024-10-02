using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAllocatorSystemAPI.Migrations
{
    public partial class seedTrainingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
              table: "Trainings",
              columns: new[] { "Name", "Description" },
              values: new object[,]
              {
                    { "select", "des"},
                    { "Horizon Training","Induction programme" },
                    { "Angular Training" , "Training for JSE"},
                    { "Dot Net Training" ,"Training for JSE"},
                    { "Testing Training" ,"Training for JTA"},
              });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
               table: "Trainings",
               keyColumn: "TrainingId",
                keyValues: new object[]
                {
                    1, 2, 3 , 4, 5, 
                });
        }
    }
}
