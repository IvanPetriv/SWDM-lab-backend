using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCore.Migrations
{
    /// <inheritdoc />
    public partial class coursefilenormalization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "MediaMaterials");

            migrationBuilder.DropColumn(
                name: "FileContent",
                table: "MediaMaterials");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "MediaMaterials");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "MediaMaterials");

            migrationBuilder.DropColumn(
                name: "FileType",
                table: "MediaMaterials");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseFile",
                table: "MediaMaterials",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "CourseFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    FileType = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    FileContent = table.Column<byte[]>(type: "bytea", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseFiles", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseFiles");

            migrationBuilder.DropColumn(
                name: "CourseFile",
                table: "MediaMaterials");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "MediaMaterials",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<byte[]>(
                name: "FileContent",
                table: "MediaMaterials",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "MediaMaterials",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "MediaMaterials",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "MediaMaterials",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
