using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DateApp.Migrations
{
    /// <inheritdoc />
    public partial class AddGridCellIdToAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a377d037-7c14-4736-a38a-98af7d82b96d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "adfce66d-1c02-465d-a028-ab145b0bf26e");

            migrationBuilder.AddColumn<string>(
                name: "GridCellId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "33574930-4ad0-4b74-8e9f-4d146384badd");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3f8778fd-382b-406b-9355-8381fd350002");

            migrationBuilder.DropColumn(
                name: "GridCellId",
                table: "AspNetUsers");

        }
    }
}
