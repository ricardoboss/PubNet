using System;
using Microsoft.EntityFrameworkCore.Migrations;
using PubNet.Database.Entities.Dart;

#nullable disable

namespace PubNet.Database.Context.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RegisteredAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DartPackageVersionAnalyses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PackageVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Formatted = table.Column<bool>(type: "boolean", nullable: true),
                    DocumentationGenerated = table.Column<bool>(type: "boolean", nullable: true),
                    ReadmeFound = table.Column<bool>(type: "boolean", nullable: true),
                    ReadmeText = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DartPackageVersionAnalyses", x => x.Id);
                    table.UniqueConstraint("AK_DartPackageVersionAnalyses_PackageVersionId", x => x.PackageVersionId);
                });

            migrationBuilder.CreateTable(
                name: "DartPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDiscontinued = table.Column<bool>(type: "boolean", nullable: false),
                    ReplacedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LatestVersionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DartPackages", x => x.Id);
                    table.UniqueConstraint("AK_DartPackages_LatestVersionId", x => x.LatestVersionId);
                    table.ForeignKey(
                        name: "FK_DartPackages_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DartPendingArchives",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ArchivePath = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    UploaderId = table.Column<Guid>(type: "uuid", nullable: false),
                    UploadedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DartPendingArchives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DartPendingArchives_Authors_UploaderId",
                        column: x => x.UploaderId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Identities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Identities_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NugetPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LatestVersionId = table.Column<Guid>(type: "uuid", nullable: false)
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
                name: "DartPackageVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Retracted = table.Column<bool>(type: "boolean", nullable: false),
                    PubSpec = table.Column<PubSpec>(type: "json", nullable: false),
                    AnalysisId = table.Column<Guid>(type: "uuid", nullable: true),
                    DartPackageVersionAnalysisPackageVersionId = table.Column<Guid>(type: "uuid", nullable: true),
                    PackageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: false),
                    PublishedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DartPackageVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DartPackageVersions_DartPackageVersionAnalyses_AnalysisId",
                        column: x => x.AnalysisId,
                        principalTable: "DartPackageVersionAnalyses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DartPackageVersions_DartPackageVersionAnalyses_DartPackageV~",
                        column: x => x.DartPackageVersionAnalysisPackageVersionId,
                        principalTable: "DartPackageVersionAnalyses",
                        principalColumn: "PackageVersionId");
                    table.ForeignKey(
                        name: "FK_DartPackageVersions_DartPackages_Id",
                        column: x => x.Id,
                        principalTable: "DartPackages",
                        principalColumn: "LatestVersionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DartPackageVersions_DartPackages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "DartPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdentityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    UserAgent = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Scopes = table.Column<string[]>(type: "text[]", maxLength: 2000, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiresAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RevokedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tokens_Identities_IdentityId",
                        column: x => x.IdentityId,
                        principalTable: "Identities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NugetPackageVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PackageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: false),
                    PublishedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "PackageArchives",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PackageVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    PackageTypeDiscriminator = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: false),
                    ArchiveUrl = table.Column<string>(type: "text", nullable: false),
                    ArchiveType = table.Column<string>(type: "text", nullable: false),
                    ArchiveSha256 = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageArchives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackageArchives_DartPackageVersions_PackageVersionId",
                        column: x => x.PackageVersionId,
                        principalTable: "DartPackageVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PackageArchives_NugetPackageVersions_PackageVersionId",
                        column: x => x.PackageVersionId,
                        principalTable: "NugetPackageVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Authors_UserName",
                table: "Authors",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DartPackages_AuthorId_Name",
                table: "DartPackages",
                columns: new[] { "AuthorId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DartPackageVersions_AnalysisId",
                table: "DartPackageVersions",
                column: "AnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_DartPackageVersions_DartPackageVersionAnalysisPackageVersio~",
                table: "DartPackageVersions",
                column: "DartPackageVersionAnalysisPackageVersionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DartPackageVersions_PackageId_Version",
                table: "DartPackageVersions",
                columns: new[] { "PackageId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DartPackageVersions_PublishedAt",
                table: "DartPackageVersions",
                column: "PublishedAt",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_DartPendingArchives_UploaderId",
                table: "DartPendingArchives",
                column: "UploaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Identities_AuthorId",
                table: "Identities",
                column: "AuthorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Identities_Email",
                table: "Identities",
                column: "Email",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_PackageArchives_PackageVersionId_PackageTypeDiscriminator",
                table: "PackageArchives",
                columns: new[] { "PackageVersionId", "PackageTypeDiscriminator" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_IdentityId_Name",
                table: "Tokens",
                columns: new[] { "IdentityId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_Value",
                table: "Tokens",
                column: "Value",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DartPendingArchives");

            migrationBuilder.DropTable(
                name: "PackageArchives");

            migrationBuilder.DropTable(
                name: "Tokens");

            migrationBuilder.DropTable(
                name: "DartPackageVersions");

            migrationBuilder.DropTable(
                name: "NugetPackageVersions");

            migrationBuilder.DropTable(
                name: "Identities");

            migrationBuilder.DropTable(
                name: "DartPackageVersionAnalyses");

            migrationBuilder.DropTable(
                name: "DartPackages");

            migrationBuilder.DropTable(
                name: "NugetPackages");

            migrationBuilder.DropTable(
                name: "Authors");
        }
    }
}
