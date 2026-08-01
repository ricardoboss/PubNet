using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PubNet.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSettingsAndAuthorRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Authors",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Key = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Key);
                });

            // existing installations have no admin yet; promote the oldest account instead of
            // sending them through onboarding again (which only triggers when no admin exists).
            migrationBuilder.Sql("""
                UPDATE "Authors"
                SET "Role" = 1337
                WHERE "RegisteredAtUtc" = (
                    SELECT MIN("RegisteredAtUtc")
                    FROM "Authors"
                );
                """);

            // an instance which already has accounts was set up before onboarding existed, so record it as
            // completed. Promoting an admin is not enough on its own: that admin can delete their own account,
            // which would leave neither an admin nor a marker and re-open onboarding to anonymous callers.
            // The key has to match OnboardingService.CompletedAtSettingKey.
            migrationBuilder.Sql("""
                INSERT INTO "Settings" ("Key", "Value", "UpdatedAtUtc")
                SELECT
                    'Onboarding:CompletedAt',
                    to_char(now() AT TIME ZONE 'utc', 'YYYY-MM-DD"T"HH24:MI:SS.US"Z"'),
                    now()
                WHERE EXISTS (SELECT 1 FROM "Authors");
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Authors");
        }
    }
}
