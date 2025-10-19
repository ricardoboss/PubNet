using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PubNet.Database.Context.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNuGet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackageArchives_NugetPackageVersions_PackageVersionId",
                table: "PackageArchives");

            migrationBuilder.DropTable(
                name: "NugetPackageVersions");

            migrationBuilder.DropTable(
                name: "NugetPackages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NugetPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    LatestVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NugetPackages", x => x.Id);
                    table.UniqueConstraint("AK_NugetPackages_LatestVersionId", x => x.LatestVersionId);
                    table.ForeignKey(
                        name: "FK_NugetPackages_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NugetPackageVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PackageId = table.Column<Guid>(type: "uuid", nullable: false),
                    PublishedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NugetPackageVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NugetPackageVersions_NugetPackages_Id",
                        column: x => x.Id,
                        principalTable: "NugetPackages",
                        principalColumn: "LatestVersionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NugetPackageVersions_NugetPackages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "NugetPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NugetPackages_AuthorId_Name",
                table: "NugetPackages",
                columns: new[] { "AuthorId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NugetPackageVersions_PackageId_Version",
                table: "NugetPackageVersions",
                columns: new[] { "PackageId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NugetPackageVersions_PublishedAt",
                table: "NugetPackageVersions",
                column: "PublishedAt",
                descending: new bool[0]);

            migrationBuilder.AddForeignKey(
                name: "FK_PackageArchives_NugetPackageVersions_PackageVersionId",
                table: "PackageArchives",
                column: "PackageVersionId",
                principalTable: "NugetPackageVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
