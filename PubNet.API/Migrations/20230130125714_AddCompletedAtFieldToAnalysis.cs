using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PubNet.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCompletedAtFieldToAnalysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentationLocalPath",
                table: "PackageVersionAnalyses");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CompletedAtUtc",
                table: "PackageVersionAnalyses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedAtUtc",
                table: "PackageVersionAnalyses");

            migrationBuilder.AddColumn<string>(
                name: "DocumentationLocalPath",
                table: "PackageVersionAnalyses",
                type: "text",
                nullable: true);
        }
    }
}
