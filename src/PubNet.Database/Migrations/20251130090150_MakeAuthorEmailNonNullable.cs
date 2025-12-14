using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PubNet.Database.Migrations
{
	/// <inheritdoc />
	public partial class MakeAuthorEmailNonNullable : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				name: "Email",
				table: "Authors",
				type: "text",
				nullable: false,
				defaultValue: "",
				oldClrType: typeof(string),
				oldType: "text",
				oldNullable: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				name: "Email",
				table: "Authors",
				type: "text",
				nullable: true,
				oldClrType: typeof(string),
				oldType: "text");
		}
	}
}
