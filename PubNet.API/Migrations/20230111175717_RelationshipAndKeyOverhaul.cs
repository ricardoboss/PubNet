using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PubNet.API.Migrations
{
    /// <inheritdoc />
    public partial class RelationshipAndKeyOverhaul : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Authors_Id",
                table: "Packages");

            migrationBuilder.DropForeignKey(
                name: "FK_PackageVersion_Packages_Id",
                table: "PackageVersion");

            migrationBuilder.DropForeignKey(
                name: "FK_Tokens_Authors_Id",
                table: "Tokens");

            migrationBuilder.AddColumn<int>(
                name: "PackageId",
                table: "PackageVersion",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_OwnerId",
                table: "Tokens",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageVersion_PackageId",
                table: "PackageVersion",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_AuthorId",
                table: "Packages",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Authors_AuthorId",
                table: "Packages",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PackageVersion_Packages_PackageId",
                table: "PackageVersion",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tokens_Authors_OwnerId",
                table: "Tokens",
                column: "OwnerId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Authors_AuthorId",
                table: "Packages");

            migrationBuilder.DropForeignKey(
                name: "FK_PackageVersion_Packages_PackageId",
                table: "PackageVersion");

            migrationBuilder.DropForeignKey(
                name: "FK_Tokens_Authors_OwnerId",
                table: "Tokens");

            migrationBuilder.DropIndex(
                name: "IX_Tokens_OwnerId",
                table: "Tokens");

            migrationBuilder.DropIndex(
                name: "IX_PackageVersion_PackageId",
                table: "PackageVersion");

            migrationBuilder.DropIndex(
                name: "IX_Packages_AuthorId",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "PackageId",
                table: "PackageVersion");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Authors_Id",
                table: "Packages",
                column: "Id",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PackageVersion_Packages_Id",
                table: "PackageVersion",
                column: "Id",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tokens_Authors_Id",
                table: "Tokens",
                column: "Id",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
