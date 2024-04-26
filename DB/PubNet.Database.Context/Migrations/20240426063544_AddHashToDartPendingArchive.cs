using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PubNet.Database.Context.Migrations
{
    /// <inheritdoc />
    public partial class AddHashToDartPendingArchive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArchiveHash",
                table: "DartPendingArchives",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArchiveHash",
                table: "DartPendingArchives");
        }
    }
}
