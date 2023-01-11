﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PubNet.API.Contexts;

#nullable disable

namespace PubNet.API.Migrations
{
    [DbContext(typeof(PubNetContext))]
    partial class PackagesContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

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

                    b.Property<int>("LatestId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ReplacedBy")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

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

                    b.Property<string>("Archive_Sha256")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Archive_Url")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Pubspec")
                        .IsRequired()
                        .HasColumnType("json");

                    b.Property<bool>("Retracted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("PackageVersion");
                });

            modelBuilder.Entity("PubNet.API.Models.AuthorToken", b =>
                {
                    b.HasOne("PubNet.API.Models.Author", "Owner")
                        .WithMany("Tokens")
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("PubNet.API.Models.Package", b =>
                {
                    b.HasOne("PubNet.API.Models.Author", "Author")
                        .WithMany("Packages")
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PubNet.API.Models.PackageVersion", "Latest")
                        .WithMany()
                        .HasForeignKey("LatestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Latest");
                });

            modelBuilder.Entity("PubNet.API.Models.PackageVersion", b =>
                {
                    b.HasOne("PubNet.API.Models.Package", null)
                        .WithMany("Versions")
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
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
