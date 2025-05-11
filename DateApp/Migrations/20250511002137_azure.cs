using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DateApp.Migrations
{
    /// <inheritdoc />
    public partial class azure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "066cd01c-6c0e-4ea9-af33-e4dedaf4da46");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "34c2c5cd-2352-4796-8272-c9b4a2ba9dc1");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1aff304c-4de7-4685-892d-e1bdcc2d55c6", null, "admin", "ADMIN" },
                    { "1dd64fab-26d0-4856-9df3-03f5be6390ea", null, "user", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1aff304c-4de7-4685-892d-e1bdcc2d55c6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1dd64fab-26d0-4856-9df3-03f5be6390ea");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "066cd01c-6c0e-4ea9-af33-e4dedaf4da46", null, "admin", "ADMIN" },
                    { "34c2c5cd-2352-4796-8272-c9b4a2ba9dc1", null, "user", "USER" }
                });
        }
    }
}
