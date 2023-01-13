using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PubNet.API.Migrations
{
    /// <inheritdoc />
    public partial class PackageVersionAnalysisEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PackageVersionAnalyses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VersionId = table.Column<int>(type: "integer", nullable: false),
                    Formatted = table.Column<bool>(type: "boolean", nullable: true),
                    DocumentationLink = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageVersionAnalyses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackageVersionAnalyses_PackageVersion_VersionId",
                        column: x => x.VersionId,
                        principalTable: "PackageVersion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PackageVersionAnalyses_VersionId",
                table: "PackageVersionAnalyses",
                column: "VersionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PackageVersionAnalyses");
        }
    }
}
