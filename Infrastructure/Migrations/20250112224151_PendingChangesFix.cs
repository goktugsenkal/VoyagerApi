using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PendingChangesFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Like_Voyages_VoyageId",
                table: "Like");

            migrationBuilder.AlterColumn<Guid>(
                name: "VoyageId",
                table: "Like",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "CommentId",
                table: "Like",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LikeType",
                table: "Like",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Like_CommentId",
                table: "Like",
                column: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Like_Comment_CommentId",
                table: "Like",
                column: "CommentId",
                principalTable: "Comment",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Like_Voyages_VoyageId",
                table: "Like",
                column: "VoyageId",
                principalTable: "Voyages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Like_Comment_CommentId",
                table: "Like");

            migrationBuilder.DropForeignKey(
                name: "FK_Like_Voyages_VoyageId",
                table: "Like");

            migrationBuilder.DropIndex(
                name: "IX_Like_CommentId",
                table: "Like");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "Like");

            migrationBuilder.DropColumn(
                name: "LikeType",
                table: "Like");

            migrationBuilder.AlterColumn<Guid>(
                name: "VoyageId",
                table: "Like",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Like_Voyages_VoyageId",
                table: "Like",
                column: "VoyageId",
                principalTable: "Voyages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
