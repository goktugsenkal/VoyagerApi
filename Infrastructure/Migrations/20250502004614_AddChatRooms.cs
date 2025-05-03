using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChatRooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatRoomParticipants_ChatUser_UserId",
                table: "ChatRoomParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatUser_Users_Id",
                table: "ChatUser");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_ChatUser_SenderId",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatUser",
                table: "ChatUser");

            migrationBuilder.RenameTable(
                name: "ChatUser",
                newName: "ChatUsers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatUsers",
                table: "ChatUsers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRoomParticipants_ChatUsers_UserId",
                table: "ChatRoomParticipants",
                column: "UserId",
                principalTable: "ChatUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUsers_Users_Id",
                table: "ChatUsers",
                column: "Id",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_ChatUsers_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "ChatUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatRoomParticipants_ChatUsers_UserId",
                table: "ChatRoomParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatUsers_Users_Id",
                table: "ChatUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_ChatUsers_SenderId",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatUsers",
                table: "ChatUsers");

            migrationBuilder.RenameTable(
                name: "ChatUsers",
                newName: "ChatUser");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatUser",
                table: "ChatUser",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRoomParticipants_ChatUser_UserId",
                table: "ChatRoomParticipants",
                column: "UserId",
                principalTable: "ChatUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUser_Users_Id",
                table: "ChatUser",
                column: "Id",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_ChatUser_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "ChatUser",
                principalColumn: "Id");
        }
    }
}
