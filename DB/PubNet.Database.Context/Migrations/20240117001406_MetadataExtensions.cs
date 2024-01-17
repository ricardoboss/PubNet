using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PubNet.Database.Context.Migrations
{
    /// <inheritdoc />
    public partial class MetadataExtensions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Identities",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "bytea");

            migrationBuilder.AddColumn<Guid>(
                name: "AnalysisId",
                table: "DartPackageVersions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "RegisteredAt",
                table: "Authors",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_DartPackageVersions_AnalysisId",
                table: "DartPackageVersions",
                column: "AnalysisId");

            migrationBuilder.AddForeignKey(
                name: "FK_DartPackageVersions_DartPackageVersionAnalyses_AnalysisId",
                table: "DartPackageVersions",
                column: "AnalysisId",
                principalTable: "DartPackageVersionAnalyses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DartPackageVersions_DartPackageVersionAnalyses_AnalysisId",
                table: "DartPackageVersions");

            migrationBuilder.DropIndex(
                name: "IX_DartPackageVersions_AnalysisId",
                table: "DartPackageVersions");

            migrationBuilder.DropColumn(
                name: "AnalysisId",
                table: "DartPackageVersions");

            migrationBuilder.DropColumn(
                name: "RegisteredAt",
                table: "Authors");

            migrationBuilder.AlterColumn<byte[]>(
                name: "PasswordHash",
                table: "Identities",
                type: "bytea",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);
        }
    }
}
