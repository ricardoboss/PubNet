using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PubNet.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IsDiscontinued = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReplacedBy = table.Column<string>(type: "TEXT", nullable: true),
                    LatestId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PackageVersion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Version = table.Column<string>(type: "TEXT", nullable: false),
                    Retracted = table.Column<bool>(type: "INTEGER", nullable: false),
                    ArchiveUrl = table.Column<string>(name: "Archive_Url", type: "TEXT", nullable: false),
                    ArchiveSha256 = table.Column<string>(name: "Archive_Sha256", type: "TEXT", nullable: false),
                    Pubspec = table.Column<string>(type: "json", nullable: false),
                    PackageId = table.Column<int>(type: "INTEGER", nullable: true)
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
                name: "IX_Packages_LatestId",
                table: "Packages",
                column: "LatestId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageVersion_PackageId",
                table: "PackageVersion",
                column: "PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_PackageVersion_LatestId",
                table: "Packages",
                column: "LatestId",
                principalTable: "PackageVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_PackageVersion_LatestId",
                table: "Packages");

            migrationBuilder.DropTable(
                name: "PackageVersion");

            migrationBuilder.DropTable(
                name: "Packages");
        }
    }
}
