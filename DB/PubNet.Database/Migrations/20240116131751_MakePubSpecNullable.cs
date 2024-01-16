using Microsoft.EntityFrameworkCore.Migrations;
using PubNet.Database.Models;

#nullable disable

namespace PubNet.Database.Migrations
{
    /// <inheritdoc />
    public partial class MakePubSpecNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<PubSpec>(
                name: "PubSpec",
                table: "PackageVersions",
                type: "json",
                nullable: true,
                oldClrType: typeof(PubSpec),
                oldType: "json");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<PubSpec>(
                name: "PubSpec",
                table: "PackageVersions",
                type: "json",
                nullable: false,
                oldClrType: typeof(PubSpec),
                oldType: "json",
                oldNullable: true);
        }
    }
}
