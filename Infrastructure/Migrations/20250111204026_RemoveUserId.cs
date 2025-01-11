using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Voyages_Users_UserId",
                table: "Voyages");

            migrationBuilder.DropForeignKey(
                name: "FK_Voyages_Users_VoyagerUserId",
                table: "Voyages");

            migrationBuilder.DropIndex(
                name: "IX_Voyages_UserId",
                table: "Voyages");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Voyages");

            migrationBuilder.AlterColumn<Guid>(
                name: "VoyagerUserId",
                table: "Voyages",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Voyages_Users_VoyagerUserId",
                table: "Voyages",
                column: "VoyagerUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Voyages_Users_VoyagerUserId",
                table: "Voyages");

            migrationBuilder.AlterColumn<Guid>(
                name: "VoyagerUserId",
                table: "Voyages",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Voyages",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Voyages_UserId",
                table: "Voyages",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Voyages_Users_UserId",
                table: "Voyages",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Voyages_Users_VoyagerUserId",
                table: "Voyages",
                column: "VoyagerUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
