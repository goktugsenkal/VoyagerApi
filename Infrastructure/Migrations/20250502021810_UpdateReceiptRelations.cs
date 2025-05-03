using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReceiptRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessageDeliveredReceipts_Users_UserId",
                table: "ChatMessageDeliveredReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessageReadReceipts_Users_UserId",
                table: "ChatMessageReadReceipts");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessageDeliveredReceipts_ChatUsers_UserId",
                table: "ChatMessageDeliveredReceipts",
                column: "UserId",
                principalTable: "ChatUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessageReadReceipts_ChatUsers_UserId",
                table: "ChatMessageReadReceipts",
                column: "UserId",
                principalTable: "ChatUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessageDeliveredReceipts_ChatUsers_UserId",
                table: "ChatMessageDeliveredReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessageReadReceipts_ChatUsers_UserId",
                table: "ChatMessageReadReceipts");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessageDeliveredReceipts_Users_UserId",
                table: "ChatMessageDeliveredReceipts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessageReadReceipts_Users_UserId",
                table: "ChatMessageReadReceipts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
