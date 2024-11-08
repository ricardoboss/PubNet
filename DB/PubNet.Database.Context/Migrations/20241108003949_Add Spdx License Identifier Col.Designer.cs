﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PubNet.Database.Context;

#nullable disable

namespace PubNet.Database.Context.Migrations
{
    [DbContext(typeof(PubNetContext))]
    [Migration("20241108003949_Add Spdx License Identifier Col")]
    partial class AddSpdxLicenseIdentifierCol
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PubNet.Database.Entities.Auth.Identity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.Property<int>("Role")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.HasKey("Id");

                    b.HasIndex("AuthorId")
                        .IsUnique();

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Identities");
                });

            modelBuilder.Entity("PubNet.Database.Entities.Auth.Token", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Browser")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<DateTimeOffset>("CreatedAtUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DeviceType")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<DateTimeOffset>("ExpiresAtUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("IdentityId")
                        .HasColumnType("uuid");

                    b.Property<string>("IpAddress")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Platform")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<DateTimeOffset?>("RevokedAtUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<List<string>>("Scopes")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("text[]");

                    b.Property<string>("UserAgent")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("Id");

                    b.HasIndex("IdentityId");

                    b.HasIndex("Value")
                        .IsUnique();

                    b.ToTable("Tokens");
                });

            modelBuilder.Entity("PubNet.Database.Entities.Author", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("RegisteredAtUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("PubNet.Database.Entities.Dart.DartPackage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDiscontinued")
                        .HasColumnType("boolean");

                    b.Property<Guid?>("LatestVersionId")
                        .IsRequired()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("ReplacedBy")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId", "Name")
                        .IsUnique();

                    b.ToTable("DartPackages");
                });

            modelBuilder.Entity("PubNet.Database.Entities.Dart.DartPackageVersion", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("AnalysisId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("DartPackageVersionAnalysisPackageVersionId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("PackageId")
                        .HasColumnType("uuid");

                    b.Property<string>("PubSpec")
                        .IsRequired()
                        .HasColumnType("json");

                    b.Property<DateTimeOffset>("PublishedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("Retracted")
                        .HasColumnType("boolean");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AnalysisId");

                    b.HasIndex("DartPackageVersionAnalysisPackageVersionId")
                        .IsUnique();

                    b.HasIndex("PublishedAt")
                        .IsDescending();

                    b.HasIndex("PackageId", "Version")
                        .IsUnique();

                    b.ToTable("DartPackageVersions");
                });

            modelBuilder.Entity("PubNet.Database.Entities.Dart.DartPackageVersionAnalysis", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset?>("CompletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool?>("DocumentationGenerated")
                        .HasColumnType("boolean");

                    b.Property<bool?>("Formatted")
                        .HasColumnType("boolean");

                    b.Property<Guid>("PackageVersionId")
                        .HasColumnType("uuid");

                    b.Property<bool?>("ReadmeFound")
                        .HasColumnType("boolean");

                    b.Property<string>("ReadmeText")
                        .HasMaxLength(10000)
                        .HasColumnType("character varying(10000)");

                    b.Property<string>("SpdxLicenseIdentifier")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("DartPackageVersionAnalyses");
                });

            modelBuilder.Entity("PubNet.Database.Entities.Dart.DartPendingArchive", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ArchiveHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ArchivePath")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<DateTimeOffset>("UploadedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UploaderId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UploaderId");

                    b.ToTable("DartPendingArchives");
                });

            modelBuilder.Entity("PubNet.Database.Entities.Nuget.NugetPackage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("LatestVersionId")
                        .IsRequired()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId", "Name")
                        .IsUnique();

                    b.ToTable("NugetPackages");
                });

            modelBuilder.Entity("PubNet.Database.Entities.Nuget.NugetPackageVersion", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("PackageId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("PublishedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("PublishedAt")
                        .IsDescending();

                    b.HasIndex("PackageId", "Version")
                        .IsUnique();

                    b.ToTable("NugetPackageVersions");
                });

            modelBuilder.Entity("PubNet.Database.Entities.Packages.PackageArchive", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ArchiveSha256")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ArchiveType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ArchiveUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PackageTypeDiscriminator")
                        .IsRequired()
                        .HasMaxLength(21)
                        .HasColumnType("character varying(21)");

                    b.Property<Guid>("PackageVersionId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("PackageVersionId", "PackageTypeDiscriminator")
                        .IsUnique();

                    b.ToTable("PackageArchives", (string)null);

                    b.HasDiscriminator<string>("PackageTypeDiscriminator").HasValue("PackageArchive");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("PubNet.Database.Entities.Dart.DartPackageVersionArchive", b =>
                {
                    b.HasBaseType("PubNet.Database.Entities.Packages.PackageArchive");

                    b.HasDiscriminator().HasValue("dart");
                });

            modelBuilder.Entity("PubNet.Database.Entities.Nuget.NugetPackageVersionArchive", b =>
                {
                    b.HasBaseType("PubNet.Database.Entities.Packages.PackageArchive");

                    b.HasDiscriminator().HasValue("nuget");
                });

            modelBuilder.Entity("PubNet.Database.Entities.Auth.Identity", b =>
                {
                    b.HasOne("PubNet.Database.Entities.Author", "Author")
                        .WithOne("Identity")
                        .HasForeignKey("PubNet.Database.Entities.Auth.Identity", "AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("PubNet.Database.Entities.Auth.Token", b =>
                {
                    b.HasOne("PubNet.Database.Entities.Auth.Identity", "Identity")
                        .WithMany("Tokens")
                        .HasForeignKey("IdentityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Identity");
                });

            modelBuilder.Entity("PubNet.Database.Entities.Dart.DartPackage", b =>
                {
                    b.HasOne("PubNet.Database.Entities.Author", "Author")
                        .WithMany("DartPackages")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("PubNet.Database.Entities.Dart.DartPackageVersion", b =>
                {
                    b.HasOne("PubNet.Database.Entities.Dart.DartPackageVersionAnalysis", "Analysis")
                        .WithMany()
                        .HasForeignKey("AnalysisId");

                    b.HasOne("PubNet.Database.Entities.Dart.DartPackageVersionAnalysis", null)
                        .WithOne("PackageVersion")
                        .HasForeignKey("PubNet.Database.Entities.Dart.DartPackageVersion", "DartPackageVersionAnalysisPackageVersionId")
                        .HasPrincipalKey("PubNet.Database.Entities.Dart.DartPackageVersionAnalysis", "PackageVersionId");

                    b.HasOne("PubNet.Database.Entities.Dart.DartPackage", null)
                        .WithOne("LatestVersion")
                        .HasForeignKey("PubNet.Database.Entities.Dart.DartPackageVersion", "Id")
                        .HasPrincipalKey("PubNet.Database.Entities.Dart.DartPackage", "LatestVersionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PubNet.Database.Entities.Dart.DartPackage", "Package")
                        .WithMany("Versions")
                        .HasForeignKey("PackageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Analysis");

                    b.Navigation("Package");
                });

            modelBuilder.Entity("PubNet.Database.Entities.Dart.DartPendingArchive", b =>
                {
                    b.HasOne("PubNet.Database.Entities.Author", "Uploader")
                        .WithMany()
                        .HasForeignKey("UploaderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Uploader");
                });

            modelBuilder.Entity("PubNet.Database.Entities.Nuget.NugetPackage", b =>
                {
                    b.HasOne("PubNet.Database.Entities.Author", "Author")
                        .WithMany("NugetPackages")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("PubNet.Database.Entities.Nuget.NugetPackageVersion", b =>
                {
                    b.HasOne("PubNet.Database.Entities.Nuget.NugetPackage", null)
                        .WithOne("LatestVersion")
                        .HasForeignKey("PubNet.Database.Entities.Nuget.NugetPackageVersion", "Id")
                        .HasPrincipalKey("PubNet.Database.Entities.Nuget.NugetPackage", "LatestVersionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PubNet.Database.Entities.Nuget.NugetPackage", "Package")
                        .WithMany("Versions")
                        .HasForeignKey("PackageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Package");
                });

            modelBuilder.Entity("PubNet.Database.Entities.Dart.DartPackageVersionArchive", b =>
                {
                    b.HasOne("PubNet.Database.Entities.Dart.DartPackageVersion", "PackageVersion")
                        .WithMany()
                        .HasForeignKey("PackageVersionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PackageVersion");
                });

            modelBuilder.Entity("PubNet.Database.Entities.Nuget.NugetPackageVersionArchive", b =>
                {
                    b.HasOne("PubNet.Database.Entities.Nuget.NugetPackageVersion", "PackageVersion")
                        .WithMany()
                        .HasForeignKey("PackageVersionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PackageVersion");
                });

            modelBuilder.Entity("PubNet.Database.Entities.Auth.Identity", b =>
                {
                    b.Navigation("Tokens");
                });

            modelBuilder.Entity("PubNet.Database.Entities.Author", b =>
                {
                    b.Navigation("DartPackages");

                    b.Navigation("Identity");

                    b.Navigation("NugetPackages");
                });

            modelBuilder.Entity("PubNet.Database.Entities.Dart.DartPackage", b =>
                {
                    b.Navigation("LatestVersion");

                    b.Navigation("Versions");
                });

            modelBuilder.Entity("PubNet.Database.Entities.Dart.DartPackageVersionAnalysis", b =>
                {
                    b.Navigation("PackageVersion")
                        .IsRequired();
                });

            modelBuilder.Entity("PubNet.Database.Entities.Nuget.NugetPackage", b =>
                {
                    b.Navigation("LatestVersion");

                    b.Navigation("Versions");
                });
#pragma warning restore 612, 618
        }
    }
}
