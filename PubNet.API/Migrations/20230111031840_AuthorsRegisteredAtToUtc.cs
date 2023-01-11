using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PubNet.API.Migrations
{
    /// <inheritdoc />
    public partial class AuthorsRegisteredAtToUtc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegisteredAt",
                table: "Authors",
                newName: "RegisteredAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegisteredAtUtc",
                table: "Authors",
                newName: "RegisteredAt");
        }
    }
}
