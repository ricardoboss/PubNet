using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PubNet.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPackageVersionAnalysisToPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PackageVersionAnalyses_VersionId",
                table: "PackageVersionAnalyses");

            migrationBuilder.AddColumn<int>(
                name: "AnalysisId",
                table: "PackageVersions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentationLocalPath",
                table: "PackageVersionAnalyses",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PackageVersionAnalyses_VersionId",
                table: "PackageVersionAnalyses",
                column: "VersionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PackageVersionAnalyses_VersionId",
                table: "PackageVersionAnalyses");

            migrationBuilder.DropColumn(
                name: "AnalysisId",
                table: "PackageVersions");

            migrationBuilder.DropColumn(
                name: "DocumentationLocalPath",
                table: "PackageVersionAnalyses");

            migrationBuilder.CreateIndex(
                name: "IX_PackageVersionAnalyses_VersionId",
                table: "PackageVersionAnalyses",
                column: "VersionId");
        }
    }
}
