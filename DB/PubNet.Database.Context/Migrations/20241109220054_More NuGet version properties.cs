using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PubNet.Database.Context.Migrations
{
    /// <inheritdoc />
    public partial class MoreNuGetversionproperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Authors",
                table: "NugetPackageVersions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Copyright",
                table: "NugetPackageVersions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DependencyGroups",
                table: "NugetPackageVersions",
                type: "json",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "NugetPackageVersions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IconFile",
                table: "NugetPackageVersions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IconUrl",
                table: "NugetPackageVersions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NuspecId",
                table: "NugetPackageVersions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NuspecVersion",
                table: "NugetPackageVersions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectUrl",
                table: "NugetPackageVersions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReadmeFile",
                table: "NugetPackageVersions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepositoryMetadata",
                table: "NugetPackageVersions",
                type: "json",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "NugetPackageVersions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "NugetPackageVersions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Authors",
                table: "NugetPackageVersions");

            migrationBuilder.DropColumn(
                name: "Copyright",
                table: "NugetPackageVersions");

            migrationBuilder.DropColumn(
                name: "DependencyGroups",
                table: "NugetPackageVersions");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "NugetPackageVersions");

            migrationBuilder.DropColumn(
                name: "IconFile",
                table: "NugetPackageVersions");

            migrationBuilder.DropColumn(
                name: "IconUrl",
                table: "NugetPackageVersions");

            migrationBuilder.DropColumn(
                name: "NuspecId",
                table: "NugetPackageVersions");

            migrationBuilder.DropColumn(
                name: "NuspecVersion",
                table: "NugetPackageVersions");

            migrationBuilder.DropColumn(
                name: "ProjectUrl",
                table: "NugetPackageVersions");

            migrationBuilder.DropColumn(
                name: "ReadmeFile",
                table: "NugetPackageVersions");

            migrationBuilder.DropColumn(
                name: "RepositoryMetadata",
                table: "NugetPackageVersions");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "NugetPackageVersions");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "NugetPackageVersions");
        }
    }
}
