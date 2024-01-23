using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PubNet.Database.Context.Migrations
{
    /// <inheritdoc />
    public partial class Removeuniqueconstraintfromtokennames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tokens_IdentityId_Name",
                table: "Tokens");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_IdentityId",
                table: "Tokens",
                column: "IdentityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tokens_IdentityId",
                table: "Tokens");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_IdentityId_Name",
                table: "Tokens",
                columns: new[] { "IdentityId", "Name" },
                unique: true);
        }
    }
}
