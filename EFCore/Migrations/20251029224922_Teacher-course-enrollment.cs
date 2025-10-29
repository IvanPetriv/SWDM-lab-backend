using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCore.Migrations
{
    /// <inheritdoc />
    public partial class Teachercourseenrollment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_User_TeacherId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_User_StudentId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Courses_TeacherId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "Enrollments",
                newName: "UserId");

            migrationBuilder.AddColumn<Guid>(
                name: "FileId",
                table: "MediaMaterials",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TeacherId",
                table: "Enrollments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MediaMaterials_FileId",
                table: "MediaMaterials",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_TeacherId",
                table: "Enrollments",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_User_TeacherId",
                table: "Enrollments",
                column: "TeacherId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_User_UserId",
                table: "Enrollments",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaMaterials_CourseFiles_FileId",
                table: "MediaMaterials",
                column: "FileId",
                principalTable: "CourseFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_User_TeacherId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_User_UserId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaMaterials_CourseFiles_FileId",
                table: "MediaMaterials");

            migrationBuilder.DropIndex(
                name: "IX_MediaMaterials_FileId",
                table: "MediaMaterials");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_TeacherId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "MediaMaterials");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Enrollments");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Enrollments",
                newName: "StudentId");

            migrationBuilder.AddColumn<Guid>(
                name: "TeacherId",
                table: "Courses",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_TeacherId",
                table: "Courses",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_User_TeacherId",
                table: "Courses",
                column: "TeacherId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_User_StudentId",
                table: "Enrollments",
                column: "StudentId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
