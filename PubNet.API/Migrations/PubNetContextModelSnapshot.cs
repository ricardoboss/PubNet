﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PubNet.API.Contexts;
using PubNet.Models;

#nullable disable

namespace PubNet.API.Migrations
{
    [DbContext(typeof(PubNetContext))]
    partial class PubNetContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PubNet.API.Models.PackageVersionAnalysis", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("DocumentationLink")
                        .HasColumnType("text");

                    b.Property<bool?>("Formatted")
                        .HasColumnType("boolean");

                    b.Property<int>("VersionId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("VersionId");

                    b.ToTable("PackageVersionAnalyses");
                });

            modelBuilder.Entity("PubNet.API.Models.PendingArchive", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ArchivePath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("UploadedAtUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UploaderId")
                        .HasColumnType("integer");

                    b.Property<Guid>("Uuid")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UploaderId");

                    b.ToTable("PendingArchives");
                });

            modelBuilder.Entity("PubNet.Models.Author", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<bool>("Inactive")
                        .HasColumnType("boolean");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("text");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset>("RegisteredAtUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Website")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("PubNet.Models.Package", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("AuthorId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsDiscontinued")
                        .HasColumnType("boolean");

                    b.Property<int?>("LatestId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ReplacedBy")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("LatestId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Packages");
                });

            modelBuilder.Entity("PubNet.Models.PackageVersion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ArchiveSha256")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasAnnotation("Relational:JsonPropertyName", "archive_sha256");

                    b.Property<string>("ArchiveUrl")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasAnnotation("Relational:JsonPropertyName", "archive_url");

                    b.Property<int?>("PackageId")
                        .HasColumnType("integer");

                    b.Property<string>("PackageName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<PubSpec>("PubSpec")
                        .HasColumnType("json")
                        .HasAnnotation("Relational:JsonPropertyName", "pubspec");

                    b.Property<DateTimeOffset>("PublishedAtUtc")
                        .HasColumnType("timestamp with time zone")
                        .HasAnnotation("Relational:JsonPropertyName", "published");

                    b.Property<bool>("Retracted")
                        .HasColumnType("boolean");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("PackageId");

                    b.HasIndex("PublishedAtUtc")
                        .IsDescending();

                    b.HasIndex("Version");

                    b.ToTable("PackageVersions");
                });

            modelBuilder.Entity("PubNet.API.Models.PackageVersionAnalysis", b =>
                {
                    b.HasOne("PubNet.Models.PackageVersion", "Version")
                        .WithMany()
                        .HasForeignKey("VersionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Version");
                });

            modelBuilder.Entity("PubNet.API.Models.PendingArchive", b =>
                {
                    b.HasOne("PubNet.Models.Author", "Uploader")
                        .WithMany()
                        .HasForeignKey("UploaderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Uploader");
                });

            modelBuilder.Entity("PubNet.Models.Package", b =>
                {
                    b.HasOne("PubNet.Models.Author", "Author")
                        .WithMany("Packages")
                        .HasForeignKey("AuthorId");

                    b.HasOne("PubNet.Models.PackageVersion", "Latest")
                        .WithMany()
                        .HasForeignKey("LatestId");

                    b.Navigation("Author");

                    b.Navigation("Latest");
                });

            modelBuilder.Entity("PubNet.Models.PackageVersion", b =>
                {
                    b.HasOne("PubNet.Models.Package", null)
                        .WithMany("Versions")
                        .HasForeignKey("PackageId");
                });

            modelBuilder.Entity("PubNet.Models.Author", b =>
                {
                    b.Navigation("Packages");
                });

            modelBuilder.Entity("PubNet.Models.Package", b =>
                {
                    b.Navigation("Versions");
                });
#pragma warning restore 612, 618
        }
    }
}
