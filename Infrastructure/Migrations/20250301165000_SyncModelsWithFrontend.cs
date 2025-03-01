using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelsWithFrontend : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Stops");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Users",
                newName: "ProfilePictureUrl");

            migrationBuilder.AddColumn<string>(
                name: "BannerPictureUrl",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<List<string>>(
                name: "ImageUrls",
                table: "Stops",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFocalPoint",
                table: "Stops",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLikedByAuthor",
                table: "Comments",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BannerPictureUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ImageUrls",
                table: "Stops");

            migrationBuilder.DropColumn(
                name: "IsFocalPoint",
                table: "Stops");

            migrationBuilder.DropColumn(
                name: "IsLikedByAuthor",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "ProfilePictureUrl",
                table: "Users",
                newName: "ImageUrl");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Stops",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
