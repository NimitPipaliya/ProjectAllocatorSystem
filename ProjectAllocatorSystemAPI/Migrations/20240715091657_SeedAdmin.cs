using Microsoft.EntityFrameworkCore.Migrations;
using System.Text;

#nullable disable

namespace ProjectAllocatorSystemAPI.Migrations
{
    public partial class SeedAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            byte[] passwordSalt;
            byte[] passwordHash;

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Admin@123"));
            }

            migrationBuilder.InsertData(
              table: "Users",
              columns: new[] { "Name", "LoginId", "Gender", "Email", "Phone", "PasswordHash", "PasswordSalt", "SecurityQuestionId", "Answer", "UserRoleId" },
              values: new object[]
              {
                    "Admin",
                    "admin",
                    "M",
                    "admin@gmail.com",
                    "9999999999",
                    passwordHash,
                    passwordSalt,
                    1,
                    "Black",
                    3
              });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
               table: "Users",
               keyColumn: "UserId",
               keyValues: new object[]
               {
                    1
               });
        }
    }
}
