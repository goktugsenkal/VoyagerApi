using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureVoyageDeleteBehaviors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Voyages_Users_VoyagerUserId",
                table: "Voyages");

            migrationBuilder.AddForeignKey(
                name: "FK_Voyages_Users_VoyagerUserId",
                table: "Voyages",
                column: "VoyagerUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Voyages_Users_VoyagerUserId",
                table: "Voyages");

            migrationBuilder.AddForeignKey(
                name: "FK_Voyages_Users_VoyagerUserId",
                table: "Voyages",
                column: "VoyagerUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
