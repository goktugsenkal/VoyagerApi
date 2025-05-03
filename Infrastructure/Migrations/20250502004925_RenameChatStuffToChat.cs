using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameChatStuffToChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageDeliveredReceipts_Messages_MessageId",
                table: "MessageDeliveredReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageDeliveredReceipts_Users_UserId",
                table: "MessageDeliveredReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageReadReceipts_Messages_MessageId",
                table: "MessageReadReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageReadReceipts_Users_UserId",
                table: "MessageReadReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_ChatRooms_ChatRoomId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_ChatUsers_SenderId",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Messages",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MessageReadReceipts",
                table: "MessageReadReceipts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MessageDeliveredReceipts",
                table: "MessageDeliveredReceipts");

            migrationBuilder.RenameTable(
                name: "Messages",
                newName: "ChatMessages");

            migrationBuilder.RenameTable(
                name: "MessageReadReceipts",
                newName: "ChatMessageReadReceipts");

            migrationBuilder.RenameTable(
                name: "MessageDeliveredReceipts",
                newName: "ChatMessageDeliveredReceipts");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_SenderId",
                table: "ChatMessages",
                newName: "IX_ChatMessages_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_MessageReadReceipts_UserId",
                table: "ChatMessageReadReceipts",
                newName: "IX_ChatMessageReadReceipts_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_MessageDeliveredReceipts_UserId",
                table: "ChatMessageDeliveredReceipts",
                newName: "IX_ChatMessageDeliveredReceipts_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatMessages",
                table: "ChatMessages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatMessageReadReceipts",
                table: "ChatMessageReadReceipts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatMessageDeliveredReceipts",
                table: "ChatMessageDeliveredReceipts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessageDeliveredReceipts_ChatMessages_MessageId",
                table: "ChatMessageDeliveredReceipts",
                column: "MessageId",
                principalTable: "ChatMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessageDeliveredReceipts_Users_UserId",
                table: "ChatMessageDeliveredReceipts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessageReadReceipts_ChatMessages_MessageId",
                table: "ChatMessageReadReceipts",
                column: "MessageId",
                principalTable: "ChatMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessageReadReceipts_Users_UserId",
                table: "ChatMessageReadReceipts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_ChatRooms_ChatRoomId",
                table: "ChatMessages",
                column: "ChatRoomId",
                principalTable: "ChatRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_ChatUsers_SenderId",
                table: "ChatMessages",
                column: "SenderId",
                principalTable: "ChatUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessageDeliveredReceipts_ChatMessages_MessageId",
                table: "ChatMessageDeliveredReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessageDeliveredReceipts_Users_UserId",
                table: "ChatMessageDeliveredReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessageReadReceipts_ChatMessages_MessageId",
                table: "ChatMessageReadReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessageReadReceipts_Users_UserId",
                table: "ChatMessageReadReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_ChatRooms_ChatRoomId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_ChatUsers_SenderId",
                table: "ChatMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatMessages",
                table: "ChatMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatMessageReadReceipts",
                table: "ChatMessageReadReceipts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatMessageDeliveredReceipts",
                table: "ChatMessageDeliveredReceipts");

            migrationBuilder.RenameTable(
                name: "ChatMessages",
                newName: "Messages");

            migrationBuilder.RenameTable(
                name: "ChatMessageReadReceipts",
                newName: "MessageReadReceipts");

            migrationBuilder.RenameTable(
                name: "ChatMessageDeliveredReceipts",
                newName: "MessageDeliveredReceipts");

            migrationBuilder.RenameIndex(
                name: "IX_ChatMessages_SenderId",
                table: "Messages",
                newName: "IX_Messages_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatMessageReadReceipts_UserId",
                table: "MessageReadReceipts",
                newName: "IX_MessageReadReceipts_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatMessageDeliveredReceipts_UserId",
                table: "MessageDeliveredReceipts",
                newName: "IX_MessageDeliveredReceipts_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Messages",
                table: "Messages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessageReadReceipts",
                table: "MessageReadReceipts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessageDeliveredReceipts",
                table: "MessageDeliveredReceipts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageDeliveredReceipts_Messages_MessageId",
                table: "MessageDeliveredReceipts",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageDeliveredReceipts_Users_UserId",
                table: "MessageDeliveredReceipts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageReadReceipts_Messages_MessageId",
                table: "MessageReadReceipts",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageReadReceipts_Users_UserId",
                table: "MessageReadReceipts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_ChatRooms_ChatRoomId",
                table: "Messages",
                column: "ChatRoomId",
                principalTable: "ChatRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_ChatUsers_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "ChatUsers",
                principalColumn: "Id");
        }
    }
}
