using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaTypeSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageCount",
                table: "Stops");

            migrationBuilder.RenameColumn(
                name: "ImageUrls",
                table: "Voyages",
                newName: "MediaKeys");

            migrationBuilder.RenameColumn(
                name: "ImageUrls",
                table: "Stops",
                newName: "MediaKeys");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MediaKeys",
                table: "Voyages",
                newName: "ImageUrls");

            migrationBuilder.RenameColumn(
                name: "MediaKeys",
                table: "Stops",
                newName: "ImageUrls");

            migrationBuilder.AddColumn<short>(
                name: "ImageCount",
                table: "Stops",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }
    }
}
