using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLikesAndComments_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Users_VoyagerUserId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Voyages_VoyageId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Like_Comment_CommentId",
                table: "Like");

            migrationBuilder.DropForeignKey(
                name: "FK_Like_Users_VoyagerUserId",
                table: "Like");

            migrationBuilder.DropForeignKey(
                name: "FK_Like_Voyages_VoyageId",
                table: "Like");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Like",
                table: "Like");

            migrationBuilder.DropIndex(
                name: "IX_Like_VoyageId_VoyagerUserId",
                table: "Like");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.RenameTable(
                name: "Like",
                newName: "Likes");

            migrationBuilder.RenameTable(
                name: "Comment",
                newName: "Comments");

            migrationBuilder.RenameIndex(
                name: "IX_Like_VoyagerUserId",
                table: "Likes",
                newName: "IX_Likes_VoyagerUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Like_CommentId",
                table: "Likes",
                newName: "IX_Likes_CommentId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_VoyagerUserId",
                table: "Comments",
                newName: "IX_Comments_VoyagerUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_VoyageId",
                table: "Comments",
                newName: "IX_Comments_VoyageId");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Comments",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Likes",
                table: "Likes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UniqueUserLike",
                table: "Likes",
                columns: new[] { "VoyageId", "CommentId", "VoyagerUserId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Users_VoyagerUserId",
                table: "Comments",
                column: "VoyagerUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Voyages_VoyageId",
                table: "Comments",
                column: "VoyageId",
                principalTable: "Voyages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Comments_CommentId",
                table: "Likes",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Users_VoyagerUserId",
                table: "Likes",
                column: "VoyagerUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Voyages_VoyageId",
                table: "Likes",
                column: "VoyageId",
                principalTable: "Voyages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Users_VoyagerUserId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Voyages_VoyageId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Comments_CommentId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Users_VoyagerUserId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Voyages_VoyageId",
                table: "Likes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Likes",
                table: "Likes");

            migrationBuilder.DropIndex(
                name: "IX_Likes_UniqueUserLike",
                table: "Likes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.RenameTable(
                name: "Likes",
                newName: "Like");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "Comment");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_VoyagerUserId",
                table: "Like",
                newName: "IX_Like_VoyagerUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_CommentId",
                table: "Like",
                newName: "IX_Like_CommentId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_VoyagerUserId",
                table: "Comment",
                newName: "IX_Comment_VoyagerUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_VoyageId",
                table: "Comment",
                newName: "IX_Comment_VoyageId");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Comment",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Like",
                table: "Like",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Like_VoyageId_VoyagerUserId",
                table: "Like",
                columns: new[] { "VoyageId", "VoyagerUserId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Users_VoyagerUserId",
                table: "Comment",
                column: "VoyagerUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Voyages_VoyageId",
                table: "Comment",
                column: "VoyageId",
                principalTable: "Voyages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Like_Comment_CommentId",
                table: "Like",
                column: "CommentId",
                principalTable: "Comment",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Like_Users_VoyagerUserId",
                table: "Like",
                column: "VoyagerUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Like_Voyages_VoyageId",
                table: "Like",
                column: "VoyageId",
                principalTable: "Voyages",
                principalColumn: "Id");
        }
    }
}
