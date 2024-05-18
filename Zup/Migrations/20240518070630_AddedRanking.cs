using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zup.Migrations
{
    /// <inheritdoc />
    public partial class AddedRanking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Rank",
                table: "TaskEntries",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rank",
                table: "TaskEntries");
        }
    }
}
