using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateChatUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "ChatMessages",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.Sql(
                $"""
                     ALTER TABLE "ChatMessages"
                     ALTER COLUMN "ClientMessageId"
                     TYPE uuid
                     USING "ClientMessageId"::uuid;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "ChatMessages",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            // Revert ClientMessageId from uuid → text
            migrationBuilder.Sql($"""
                                    ALTER TABLE "ChatMessages"
                                    ALTER COLUMN "ClientMessageId"
                                    TYPE text
                                    USING "ClientMessageId"::text;
                                  """);
        }
    }
}
