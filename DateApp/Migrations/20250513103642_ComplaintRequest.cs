using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DateApp.Migrations
{
    /// <inheritdoc />
    public partial class ComplaintRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8ff59ea1-4d73-49bd-b52d-64dec1c5850d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b7883e6c-aa77-4d83-aa7e-7e71bca1c2dc");

            migrationBuilder.CreateTable(
                name: "ComplaintAndRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ComplaintTypeId = table.Column<int>(type: "int", nullable: true),
                    RequestTypeId = table.Column<int>(type: "int", nullable: true),
                    ComplaintType = table.Column<int>(type: "int", nullable: true),
                    RequestType = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplaintAndRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComplaintAndRequests_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "81e78ab5-833e-4771-b910-4ad00a957997", null, "admin", "ADMIN" },
                    { "86951f11-bea3-4f41-8507-13e5c73ed3ca", null, "user", "USER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintAndRequests_UserId",
                table: "ComplaintAndRequests",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComplaintAndRequests");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "81e78ab5-833e-4771-b910-4ad00a957997");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "86951f11-bea3-4f41-8507-13e5c73ed3ca");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "8ff59ea1-4d73-49bd-b52d-64dec1c5850d", null, "admin", "ADMIN" },
                    { "b7883e6c-aa77-4d83-aa7e-7e71bca1c2dc", null, "user", "USER" }
                });
        }
    }
}
