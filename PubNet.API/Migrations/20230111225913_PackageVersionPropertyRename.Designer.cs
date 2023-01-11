﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PubNet.API.Contexts;

#nullable disable

namespace PubNet.API.Migrations
{
    [DbContext(typeof(PubNetContext))]
    [Migration("20230111225913_PackageVersionPropertyRename")]
    partial class PackageVersionPropertyRename
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.2");

            modelBuilder.Entity("PubNet.API.Models.Author", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("Inactive")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("RegisteredAtUtc")
                        .HasColumnType("TEXT");

                    b.Property<string>("Website")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("PubNet.API.Models.AuthorToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("ExpiresAtUtc")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("OwnerId")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Value")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.HasIndex("Value")
                        .IsUnique();

                    b.ToTable("Tokens");
                });

            modelBuilder.Entity("PubNet.API.Models.Package", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AuthorId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDiscontinued")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("LatestId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ReplacedBy")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("LatestId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Packages");
                });

            modelBuilder.Entity("PubNet.API.Models.PackageVersion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ArchiveSha256")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasAnnotation("Relational:JsonPropertyName", "archive_sha256");

                    b.Property<string>("ArchiveUrl")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasAnnotation("Relational:JsonPropertyName", "archive_url");

                    b.Property<int?>("PackageId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Pubspec")
                        .IsRequired()
                        .HasColumnType("json");

                    b.Property<bool>("Retracted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("PackageId");

                    b.ToTable("PackageVersion");
                });

            modelBuilder.Entity("PubNet.API.Models.PendingArchive", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ArchivePath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("UploadedAtUtc")
                        .HasColumnType("TEXT");

                    b.Property<int>("UploaderId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UploaderId");

                    b.ToTable("PendingArchives");
                });

            modelBuilder.Entity("PubNet.API.Models.AuthorToken", b =>
                {
                    b.HasOne("PubNet.API.Models.Author", "Owner")
                        .WithMany("Tokens")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("PubNet.API.Models.Package", b =>
                {
                    b.HasOne("PubNet.API.Models.Author", "Author")
                        .WithMany("Packages")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PubNet.API.Models.PackageVersion", "Latest")
                        .WithMany()
                        .HasForeignKey("LatestId");

                    b.Navigation("Author");

                    b.Navigation("Latest");
                });

            modelBuilder.Entity("PubNet.API.Models.PackageVersion", b =>
                {
                    b.HasOne("PubNet.API.Models.Package", null)
                        .WithMany("Versions")
                        .HasForeignKey("PackageId");
                });

            modelBuilder.Entity("PubNet.API.Models.PendingArchive", b =>
                {
                    b.HasOne("PubNet.API.Models.AuthorToken", "Uploader")
                        .WithMany()
                        .HasForeignKey("UploaderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Uploader");
                });

            modelBuilder.Entity("PubNet.API.Models.Author", b =>
                {
                    b.Navigation("Packages");

                    b.Navigation("Tokens");
                });

            modelBuilder.Entity("PubNet.API.Models.Package", b =>
                {
                    b.Navigation("Versions");
                });
#pragma warning restore 612, 618
        }
    }
}
