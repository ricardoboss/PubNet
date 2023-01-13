using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PubNet.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Website = table.Column<string>(type: "text", nullable: true),
                    Inactive = table.Column<bool>(type: "boolean", nullable: false),
                    RegisteredAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<byte[]>(type: "bytea", nullable: false),
                    ExpiresAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    OwnerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tokens_Authors_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PendingArchives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ArchivePath = table.Column<string>(type: "text", nullable: false),
                    UploaderId = table.Column<int>(type: "integer", nullable: false),
                    UploadedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingArchives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PendingArchives_Tokens_UploaderId",
                        column: x => x.UploaderId,
                        principalTable: "Tokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsDiscontinued = table.Column<bool>(type: "boolean", nullable: false),
                    ReplacedBy = table.Column<string>(type: "text", nullable: true),
                    LatestId = table.Column<int>(type: "integer", nullable: true),
                    AuthorId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Packages_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PackageVersion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Version = table.Column<string>(type: "text", nullable: false),
                    Retracted = table.Column<bool>(type: "boolean", nullable: false),
                    ArchiveUrl = table.Column<string>(type: "text", nullable: false),
                    ArchiveSha256 = table.Column<string>(type: "text", nullable: false),
                    PublishedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Pubspec = table.Column<string>(type: "json", nullable: false),
                    PackageId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackageVersion_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Authors_Email",
                table: "Authors",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Authors_Username",
                table: "Authors",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_AuthorId",
                table: "Packages",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_LatestId",
                table: "Packages",
                column: "LatestId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_Name",
                table: "Packages",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PackageVersion_PackageId",
                table: "PackageVersion",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageVersion_PublishedAtUtc",
                table: "PackageVersion",
                column: "PublishedAtUtc",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_PackageVersion_Version",
                table: "PackageVersion",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_PendingArchives_UploaderId",
                table: "PendingArchives",
                column: "UploaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_Name",
                table: "Tokens",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_Name_OwnerId",
                table: "Tokens",
                columns: new[] { "Name", "OwnerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_OwnerId",
                table: "Tokens",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_PackageVersion_LatestId",
                table: "Packages",
                column: "LatestId",
                principalTable: "PackageVersion",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Authors_AuthorId",
                table: "Packages");

            migrationBuilder.DropForeignKey(
                name: "FK_Packages_PackageVersion_LatestId",
                table: "Packages");

            migrationBuilder.DropTable(
                name: "PendingArchives");

            migrationBuilder.DropTable(
                name: "Tokens");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "PackageVersion");

            migrationBuilder.DropTable(
                name: "Packages");
        }
    }
}
