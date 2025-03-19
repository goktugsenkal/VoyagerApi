using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MinorChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DistanceToNext",
                table: "Stops",
                newName: "OrderIndex");

            migrationBuilder.RenameColumn(
                name: "ArrivalTimeToNext",
                table: "Stops",
                newName: "ImageCount");

            migrationBuilder.AddColumn<string>(
                name: "VoyagerUsername",
                table: "Voyages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ArrivalTimeToNextStop",
                table: "Stops",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DistanceToNextStop",
                table: "Stops",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoyagerUsername",
                table: "Voyages");

            migrationBuilder.DropColumn(
                name: "ArrivalTimeToNextStop",
                table: "Stops");

            migrationBuilder.DropColumn(
                name: "DistanceToNextStop",
                table: "Stops");

            migrationBuilder.RenameColumn(
                name: "OrderIndex",
                table: "Stops",
                newName: "DistanceToNext");

            migrationBuilder.RenameColumn(
                name: "ImageCount",
                table: "Stops",
                newName: "ArrivalTimeToNext");
        }
    }
}
