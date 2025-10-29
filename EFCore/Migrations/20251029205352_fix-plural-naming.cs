using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCore.Migrations
{
    /// <inheritdoc />
    public partial class fixpluralnaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaMaterial_Courses_CourseId",
                table: "MediaMaterial");

            migrationBuilder.DropForeignKey(
                name: "FK_TextMaterial_Courses_CourseId",
                table: "TextMaterial");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TextMaterial",
                table: "TextMaterial");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MediaMaterial",
                table: "MediaMaterial");

            migrationBuilder.RenameTable(
                name: "TextMaterial",
                newName: "TextMaterials");

            migrationBuilder.RenameTable(
                name: "MediaMaterial",
                newName: "MediaMaterials");

            migrationBuilder.RenameIndex(
                name: "IX_TextMaterial_CourseId",
                table: "TextMaterials",
                newName: "IX_TextMaterials_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_MediaMaterial_CourseId",
                table: "MediaMaterials",
                newName: "IX_MediaMaterials_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TextMaterials",
                table: "TextMaterials",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MediaMaterials",
                table: "MediaMaterials",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaMaterials_Courses_CourseId",
                table: "MediaMaterials",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TextMaterials_Courses_CourseId",
                table: "TextMaterials",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaMaterials_Courses_CourseId",
                table: "MediaMaterials");

            migrationBuilder.DropForeignKey(
                name: "FK_TextMaterials_Courses_CourseId",
                table: "TextMaterials");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TextMaterials",
                table: "TextMaterials");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MediaMaterials",
                table: "MediaMaterials");

            migrationBuilder.RenameTable(
                name: "TextMaterials",
                newName: "TextMaterial");

            migrationBuilder.RenameTable(
                name: "MediaMaterials",
                newName: "MediaMaterial");

            migrationBuilder.RenameIndex(
                name: "IX_TextMaterials_CourseId",
                table: "TextMaterial",
                newName: "IX_TextMaterial_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_MediaMaterials_CourseId",
                table: "MediaMaterial",
                newName: "IX_MediaMaterial_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TextMaterial",
                table: "TextMaterial",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MediaMaterial",
                table: "MediaMaterial",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaMaterial_Courses_CourseId",
                table: "MediaMaterial",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TextMaterial_Courses_CourseId",
                table: "TextMaterial",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
