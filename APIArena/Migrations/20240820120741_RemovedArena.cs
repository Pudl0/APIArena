using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIArena.Migrations
{
    /// <inheritdoc />
    public partial class RemovedArena : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Arenas_ArenaId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Players_Player2Id",
                table: "Sessions");

            migrationBuilder.DropTable(
                name: "Arenas");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_ArenaId",
                table: "Sessions");

            migrationBuilder.AlterColumn<Guid>(
                name: "Player2Id",
                table: "Sessions",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddColumn<int>(
                name: "Round",
                table: "Sessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ApiKeyId",
                table: "Players",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Players_Player2Id",
                table: "Sessions",
                column: "Player2Id",
                principalTable: "Players",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Players_Player2Id",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "Round",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "ApiKeyId",
                table: "Players");

            migrationBuilder.AlterColumn<Guid>(
                name: "Player2Id",
                table: "Sessions",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "Arenas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Height = table.Column<int>(type: "int", nullable: false),
                    Width = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arenas", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_ArenaId",
                table: "Sessions",
                column: "ArenaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Arenas_ArenaId",
                table: "Sessions",
                column: "ArenaId",
                principalTable: "Arenas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Players_Player2Id",
                table: "Sessions",
                column: "Player2Id",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
