using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIArena.Migrations
{
    /// <inheritdoc />
    public partial class AddedMap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ArenaId",
                table: "Sessions",
                newName: "MapId");

            migrationBuilder.CreateTable(
                name: "Maps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Height = table.Column<int>(type: "int", nullable: false),
                    Width = table.Column<int>(type: "int", nullable: false),
                    SerializedTiles = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maps", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_MapId",
                table: "Sessions",
                column: "MapId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Maps_MapId",
                table: "Sessions",
                column: "MapId",
                principalTable: "Maps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Maps_MapId",
                table: "Sessions");

            migrationBuilder.DropTable(
                name: "Maps");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_MapId",
                table: "Sessions");

            migrationBuilder.RenameColumn(
                name: "MapId",
                table: "Sessions",
                newName: "ArenaId");
        }
    }
}
