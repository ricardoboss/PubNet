using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PubNet.Database.Migrations
{
	/// <inheritdoc />
	public partial class LimitArchiveSha256Column : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				name: "ArchiveSha256",
				table: "PackageVersions",
				type: "character varying(64)",
				maxLength: 64,
				nullable: false,
				oldClrType: typeof(string),
				oldType: "text");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				name: "ArchiveSha256",
				table: "PackageVersions",
				type: "text",
				nullable: false,
				oldClrType: typeof(string),
				oldType: "character varying(64)",
				oldMaxLength: 64);
		}
	}
}
