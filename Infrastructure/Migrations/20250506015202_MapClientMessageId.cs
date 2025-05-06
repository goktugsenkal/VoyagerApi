using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MapClientMessageId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessageDeliveredReceipts_ChatMessages_MessageId",
                table: "ChatMessageDeliveredReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessageReadReceipts_ChatMessages_MessageId",
                table: "ChatMessageReadReceipts");

            migrationBuilder.RenameColumn(
                name: "MessageId",
                table: "ChatMessageReadReceipts",
                newName: "ClientMessageId");

            migrationBuilder.RenameColumn(
                name: "MessageId",
                table: "ChatMessageDeliveredReceipts",
                newName: "ClientMessageId");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_ChatMessages_ClientMessageId",
                table: "ChatMessages",
                column: "ClientMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessageDeliveredReceipts_ChatMessages_ClientMessageId",
                table: "ChatMessageDeliveredReceipts",
                column: "ClientMessageId",
                principalTable: "ChatMessages",
                principalColumn: "ClientMessageId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessageReadReceipts_ChatMessages_ClientMessageId",
                table: "ChatMessageReadReceipts",
                column: "ClientMessageId",
                principalTable: "ChatMessages",
                principalColumn: "ClientMessageId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessageDeliveredReceipts_ChatMessages_ClientMessageId",
                table: "ChatMessageDeliveredReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessageReadReceipts_ChatMessages_ClientMessageId",
                table: "ChatMessageReadReceipts");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_ChatMessages_ClientMessageId",
                table: "ChatMessages");

            migrationBuilder.RenameColumn(
                name: "ClientMessageId",
                table: "ChatMessageReadReceipts",
                newName: "MessageId");

            migrationBuilder.RenameColumn(
                name: "ClientMessageId",
                table: "ChatMessageDeliveredReceipts",
                newName: "MessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessageDeliveredReceipts_ChatMessages_MessageId",
                table: "ChatMessageDeliveredReceipts",
                column: "MessageId",
                principalTable: "ChatMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessageReadReceipts_ChatMessages_MessageId",
                table: "ChatMessageReadReceipts",
                column: "MessageId",
                principalTable: "ChatMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
