using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardApp.WebApp.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "admin-role-id", "admin-user-id" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-user-id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Bio", "ConcurrencyStamp", "CreatedAt", "DisplayName", "Email", "EmailConfirmed", "LastLoginAt", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfileImageUrl", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "admin-user-id", 0, null, "FIXED-CONCURRENCY-STAMP", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "관리자", "admin@southmw.com", true, null, false, null, "ADMIN@SOUTHMW.COM", "ADMIN@SOUTHMW.COM", "AQAAAAIAAYagAAAAEJB3z7LqYzJqD6xH0Q0R8QZ5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Zg==", null, false, null, "FIXED-SECURITY-STAMP-ADMIN", false, "admin@southmw.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "admin-role-id", "admin-user-id" });
        }
    }
}
