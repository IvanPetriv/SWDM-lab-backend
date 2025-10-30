using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCore.Migrations
{
    /// <inheritdoc />
    public partial class files_nav : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "CourseFiles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_CourseFiles_CourseId",
                table: "CourseFiles",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseFiles_Courses_CourseId",
                table: "CourseFiles",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseFiles_Courses_CourseId",
                table: "CourseFiles");

            migrationBuilder.DropIndex(
                name: "IX_CourseFiles_CourseId",
                table: "CourseFiles");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "CourseFiles");
        }
    }
}
