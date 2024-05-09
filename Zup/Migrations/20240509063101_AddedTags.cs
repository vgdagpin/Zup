using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zup.Migrations
{
    /// <inheritdoc />
    public partial class AddedTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TaskEntryTags",
                columns: table => new
                {
                    TaskID = table.Column<Guid>(type: "TEXT", nullable: false),
                    TagID = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskEntryTags", x => new { x.TaskID, x.TagID });
                    table.ForeignKey(
                        name: "FK_TaskEntryTags_Tags_TagID",
                        column: x => x.TagID,
                        principalTable: "Tags",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskEntryTags_TaskEntries_TaskID",
                        column: x => x.TaskID,
                        principalTable: "TaskEntries",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskEntryNotes_TaskID",
                table: "TaskEntryNotes",
                column: "TaskID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskEntryTags_TagID",
                table: "TaskEntryTags",
                column: "TagID");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskEntryNotes_TaskEntries_TaskID",
                table: "TaskEntryNotes",
                column: "TaskID",
                principalTable: "TaskEntries",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskEntryNotes_TaskEntries_TaskID",
                table: "TaskEntryNotes");

            migrationBuilder.DropTable(
                name: "TaskEntryTags");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_TaskEntryNotes_TaskID",
                table: "TaskEntryNotes");
        }
    }
}
