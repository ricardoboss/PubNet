using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PubNet.API.Migrations
{
    /// <inheritdoc />
    public partial class PackageVersionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_PackageVersion_LatestId",
                table: "Packages");

            migrationBuilder.DropForeignKey(
                name: "FK_PackageVersion_Packages_PackageId",
                table: "PackageVersion");

            migrationBuilder.DropForeignKey(
                name: "FK_PackageVersionAnalyses_PackageVersion_VersionId",
                table: "PackageVersionAnalyses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PackageVersion",
                table: "PackageVersion");

            migrationBuilder.RenameTable(
                name: "PackageVersion",
                newName: "PackageVersions");

            migrationBuilder.RenameIndex(
                name: "IX_PackageVersion_Version",
                table: "PackageVersions",
                newName: "IX_PackageVersions_Version");

            migrationBuilder.RenameIndex(
                name: "IX_PackageVersion_PublishedAtUtc",
                table: "PackageVersions",
                newName: "IX_PackageVersions_PublishedAtUtc");

            migrationBuilder.RenameIndex(
                name: "IX_PackageVersion_PackageId",
                table: "PackageVersions",
                newName: "IX_PackageVersions_PackageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PackageVersions",
                table: "PackageVersions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_PackageVersions_LatestId",
                table: "Packages",
                column: "LatestId",
                principalTable: "PackageVersions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PackageVersionAnalyses_PackageVersions_VersionId",
                table: "PackageVersionAnalyses",
                column: "VersionId",
                principalTable: "PackageVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PackageVersions_Packages_PackageId",
                table: "PackageVersions",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_PackageVersions_LatestId",
                table: "Packages");

            migrationBuilder.DropForeignKey(
                name: "FK_PackageVersionAnalyses_PackageVersions_VersionId",
                table: "PackageVersionAnalyses");

            migrationBuilder.DropForeignKey(
                name: "FK_PackageVersions_Packages_PackageId",
                table: "PackageVersions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PackageVersions",
                table: "PackageVersions");

            migrationBuilder.RenameTable(
                name: "PackageVersions",
                newName: "PackageVersion");

            migrationBuilder.RenameIndex(
                name: "IX_PackageVersions_Version",
                table: "PackageVersion",
                newName: "IX_PackageVersion_Version");

            migrationBuilder.RenameIndex(
                name: "IX_PackageVersions_PublishedAtUtc",
                table: "PackageVersion",
                newName: "IX_PackageVersion_PublishedAtUtc");

            migrationBuilder.RenameIndex(
                name: "IX_PackageVersions_PackageId",
                table: "PackageVersion",
                newName: "IX_PackageVersion_PackageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PackageVersion",
                table: "PackageVersion",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_PackageVersion_LatestId",
                table: "Packages",
                column: "LatestId",
                principalTable: "PackageVersion",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PackageVersion_Packages_PackageId",
                table: "PackageVersion",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PackageVersionAnalyses_PackageVersion_VersionId",
                table: "PackageVersionAnalyses",
                column: "VersionId",
                principalTable: "PackageVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
