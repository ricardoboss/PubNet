using Microsoft.EntityFrameworkCore.Migrations;
using PubNet.Database.Models;

#nullable disable

namespace PubNet.Database.Migrations
{
	/// <inheritdoc />
	public partial class AddReadmeToAnalysis : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<PubSpec>(
				name: "PubSpec",
				table: "PackageVersions",
				type: "json",
				nullable: false,
				oldClrType: typeof(PubSpec),
				oldType: "json",
				oldNullable: true);

			migrationBuilder.AddColumn<bool>(
				name: "ReadmeFound",
				table: "PackageVersionAnalyses",
				type: "boolean",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "ReadmeText",
				table: "PackageVersionAnalyses",
				type: "text",
				nullable: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "ReadmeFound",
				table: "PackageVersionAnalyses");

			migrationBuilder.DropColumn(
				name: "ReadmeText",
				table: "PackageVersionAnalyses");

			migrationBuilder.AlterColumn<PubSpec>(
				name: "PubSpec",
				table: "PackageVersions",
				type: "json",
				nullable: true,
				oldClrType: typeof(PubSpec),
				oldType: "json");
		}
	}
}
