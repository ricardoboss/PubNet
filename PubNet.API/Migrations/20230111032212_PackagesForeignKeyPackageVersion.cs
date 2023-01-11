using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PubNet.API.Migrations
{
    /// <inheritdoc />
    public partial class PackagesForeignKeyPackageVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackageVersion_Packages_PackageId",
                table: "PackageVersion");

            migrationBuilder.DropIndex(
                name: "IX_PackageVersion_PackageId",
                table: "PackageVersion");

            migrationBuilder.DropColumn(
                name: "PackageId",
                table: "PackageVersion");

            migrationBuilder.AddForeignKey(
                name: "FK_PackageVersion_Packages_Id",
                table: "PackageVersion",
                column: "Id",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackageVersion_Packages_Id",
                table: "PackageVersion");

            migrationBuilder.AddColumn<int>(
                name: "PackageId",
                table: "PackageVersion",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PackageVersion_PackageId",
                table: "PackageVersion",
                column: "PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_PackageVersion_Packages_PackageId",
                table: "PackageVersion",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id");
        }
    }
}
