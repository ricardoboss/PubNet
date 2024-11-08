using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PubNet.Database.Context.Migrations
{
    /// <inheritdoc />
    public partial class AddSpdxLicenseIdentifierCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SpdxLicenseIdentifier",
                table: "DartPackageVersionAnalyses",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpdxLicenseIdentifier",
                table: "DartPackageVersionAnalyses");
        }
    }
}
