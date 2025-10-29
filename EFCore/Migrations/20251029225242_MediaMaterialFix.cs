using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCore.Migrations
{
    /// <inheritdoc />
    public partial class MediaMaterialFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaMaterials_CourseFiles_FileId",
                table: "MediaMaterials");

            migrationBuilder.DropColumn(
                name: "CourseFile",
                table: "MediaMaterials");

            migrationBuilder.RenameColumn(
                name: "FileId",
                table: "MediaMaterials",
                newName: "CourseFileId");

            migrationBuilder.RenameIndex(
                name: "IX_MediaMaterials_FileId",
                table: "MediaMaterials",
                newName: "IX_MediaMaterials_CourseFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaMaterials_CourseFiles_CourseFileId",
                table: "MediaMaterials",
                column: "CourseFileId",
                principalTable: "CourseFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaMaterials_CourseFiles_CourseFileId",
                table: "MediaMaterials");

            migrationBuilder.RenameColumn(
                name: "CourseFileId",
                table: "MediaMaterials",
                newName: "FileId");

            migrationBuilder.RenameIndex(
                name: "IX_MediaMaterials_CourseFileId",
                table: "MediaMaterials",
                newName: "IX_MediaMaterials_FileId");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseFile",
                table: "MediaMaterials",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_MediaMaterials_CourseFiles_FileId",
                table: "MediaMaterials",
                column: "FileId",
                principalTable: "CourseFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
