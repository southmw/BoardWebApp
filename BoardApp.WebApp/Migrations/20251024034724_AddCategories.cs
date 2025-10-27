using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardApp.WebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Boards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsPinned = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            // 초기 카테고리 데이터 삽입
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Name", "Description", "Color", "DisplayOrder", "IsPinned", "IsActive" },
                values: new object[,]
                {
                    { "공지사항", "중요한 공지사항", "#ef476f", 1, true, true },
                    { "자유게시판", "자유롭게 소통하는 공간", "#667eea", 2, false, true },
                    { "질문답변", "궁금한 점을 물어보세요", "#4cc9f0", 3, false, true },
                    { "정보공유", "유용한 정보를 공유하는 공간", "#06d6a0", 4, false, true }
                });

            // 기존 Board 데이터에 기본 카테고리(자유게시판, Id=2) 할당
            migrationBuilder.Sql("UPDATE Boards SET CategoryId = 2 WHERE CategoryId = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Boards_CategoryId",
                table: "Boards",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Boards_Categories_CategoryId",
                table: "Boards",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boards_Categories_CategoryId",
                table: "Boards");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Boards_CategoryId",
                table: "Boards");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Boards");
        }
    }
}
