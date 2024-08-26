using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIArena.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_ApiKeys_ApiKeyId",
                table: "Players");

            migrationBuilder.AddColumn<int>(
                name: "Mode",
                table: "Sessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<byte[]>(
                name: "ApiKeyId",
                table: "Players",
                type: "varbinary(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(32)",
                oldMaxLength: 32);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_ApiKeys_ApiKeyId",
                table: "Players",
                column: "ApiKeyId",
                principalTable: "ApiKeys",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_ApiKeys_ApiKeyId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Mode",
                table: "Sessions");

            migrationBuilder.AlterColumn<byte[]>(
                name: "ApiKeyId",
                table: "Players",
                type: "varbinary(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "varbinary(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_ApiKeys_ApiKeyId",
                table: "Players",
                column: "ApiKeyId",
                principalTable: "ApiKeys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
