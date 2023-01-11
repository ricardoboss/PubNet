using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PubNet.API.Migrations
{
    /// <inheritdoc />
    public partial class PackageVersionPropertyRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Archive_Url",
                table: "PackageVersion",
                newName: "ArchiveUrl");

            migrationBuilder.RenameColumn(
                name: "Archive_Sha256",
                table: "PackageVersion",
                newName: "ArchiveSha256");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ArchiveUrl",
                table: "PackageVersion",
                newName: "Archive_Url");

            migrationBuilder.RenameColumn(
                name: "ArchiveSha256",
                table: "PackageVersion",
                newName: "Archive_Sha256");
        }
    }
}
