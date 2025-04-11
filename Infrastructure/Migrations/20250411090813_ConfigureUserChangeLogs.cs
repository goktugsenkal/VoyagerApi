using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureUserChangeLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserChangeLogs",
                newName: "VoyagerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserChangeLogs_VoyagerUserId",
                table: "UserChangeLogs",
                column: "VoyagerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserChangeLogs_Users_VoyagerUserId",
                table: "UserChangeLogs",
                column: "VoyagerUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserChangeLogs_Users_VoyagerUserId",
                table: "UserChangeLogs");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_UserChangeLogs_VoyagerUserId",
                table: "UserChangeLogs");

            migrationBuilder.RenameColumn(
                name: "VoyagerUserId",
                table: "UserChangeLogs",
                newName: "UserId");
        }
    }
}
