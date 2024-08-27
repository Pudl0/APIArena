using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIArena.Migrations
{
    /// <inheritdoc />
    public partial class AddedApiKeyToPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ApiKeyId",
                table: "Players",
                type: "varbinary(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<int>(
                name: "XPos",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "YPos",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Players_ApiKeyId",
                table: "Players",
                column: "ApiKeyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_ApiKeys_ApiKeyId",
                table: "Players",
                column: "ApiKeyId",
                principalTable: "ApiKeys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_ApiKeys_ApiKeyId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_ApiKeyId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "ApiKeyId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "XPos",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "YPos",
                table: "Players");
        }
    }
}
