using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PubNet.Database.Context.Migrations
{
    /// <inheritdoc />
    public partial class AddedRoletoIdentities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Identities",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Identities");
        }
    }
}
