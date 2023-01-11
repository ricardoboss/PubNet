using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PubNet.API.Migrations
{
    /// <inheritdoc />
    public partial class LatestPackageVersionNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_PackageVersion_LatestId",
                table: "Packages");

            migrationBuilder.AlterColumn<int>(
                name: "LatestId",
                table: "Packages",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

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
                name: "FK_Packages_PackageVersion_LatestId",
                table: "Packages");

            migrationBuilder.AlterColumn<int>(
                name: "LatestId",
                table: "Packages",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_PackageVersion_LatestId",
                table: "Packages",
                column: "LatestId",
                principalTable: "PackageVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
