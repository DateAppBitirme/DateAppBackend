using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DateApp.Migrations
{
    /// <inheritdoc />
    public partial class Update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComplaintAndRequests_AspNetUsers_UserId",
                table: "ComplaintAndRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_ComplaintAndRequests_AspNetUsers_UserId",
                table: "ComplaintAndRequests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComplaintAndRequests_AspNetUsers_UserId",
                table: "ComplaintAndRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_ComplaintAndRequests_AspNetUsers_UserId",
                table: "ComplaintAndRequests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
