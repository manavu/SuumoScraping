﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SuumoScraping.Models;

#nullable disable

namespace SuumoScraping.Migrations
{
    [DbContext(typeof(ScrapingContext))]
    partial class ScrapingContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.HasCharSet(modelBuilder, "utf8", DelegationModes.ApplyToDatabases);

            modelBuilder.Entity("SuumoScraping.Models.Bukken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Access")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Access2")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Access3")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Balcony")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("BuiltYears")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<string>("DetailUrl")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("Direction")
                        .HasMaxLength(5)
                        .HasColumnType("varchar(5)");

                    b.Property<string>("Floor")
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<string>("FloorArea")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<decimal>("FloorArea1")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("FloorAreaMeasuringMethod")
                        .HasColumnType("longtext");

                    b.Property<decimal?>("FloorTubo")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("FullText_Id")
                        .HasColumnType("int");

                    b.Property<DateTime>("ImportedDate")
                        .HasColumnType("date");

                    b.Property<string>("Layout")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("ManagementFee")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("MoveInTime")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Price")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<decimal>("Price1")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("Price2")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("RepairingDeposit")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("RepairingFund")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Restriction")
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)");

                    b.Property<string>("RightsStyle")
                        .HasMaxLength(300)
                        .HasColumnType("varchar(300)");

                    b.Property<string>("Structure")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("UseDistrict")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("FullText_Id");

                    b.HasIndex(new[] { "DetailUrl", "ImportedDate" }, "IX_Bukkens_DetailUrl_ImportedDate");

                    b.HasIndex(new[] { "ImportedDate", "DetailUrl" }, "IX_Bukkens_ImportedDate_DetailUrl");

                    b.ToTable("bukkens");
                });

            modelBuilder.Entity("SuumoScraping.Models.BukkenFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("Bukken_Id")
                        .HasColumnType("int");

                    b.Property<int?>("File_Id")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("Bukken_Id");

                    b.HasIndex("File_Id");

                    b.ToTable("bukkenfiles");
                });

            modelBuilder.Entity("SuumoScraping.Models.BukkenFulltext", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AccessBigram")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("AddressBigram")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("bukkenfulltexts");
                });

            modelBuilder.Entity("SuumoScraping.Models.File", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<byte[]>("FileData")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nchar(64)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "Url" }, "IX_Files_Url");

                    b.ToTable("files");
                });

            modelBuilder.Entity("SuumoScraping.Models.NewBukken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Access1")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Access2")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Access3")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Balcony")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<DateTime?>("BuiltYears")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime");

                    b.Property<string>("DetailUrl")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("Direction")
                        .HasMaxLength(5)
                        .HasColumnType("varchar(5)");

                    b.Property<string>("Floor")
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<string>("FloorArea")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<decimal>("FloorArea1")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("FloorAreaMeasuringMethod")
                        .HasColumnType("longtext");

                    b.Property<decimal?>("FloorTubo")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("ImportCount")
                        .HasColumnType("int");

                    b.Property<DateTime>("ImportedAt")
                        .HasColumnType("datetime");

                    b.Property<string>("Layout")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("ManagementFee")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("MoveInTime")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("RepairingDeposit")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("RepairingFund")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Restriction")
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)");

                    b.Property<string>("RightsStyle")
                        .HasMaxLength(300)
                        .HasColumnType("varchar(300)");

                    b.Property<string>("Structure")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("UseDistrict")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "DetailUrl" }, "IX_Bukkens_DetailUrl")
                        .IsUnique();

                    b.ToTable("newbukkens");
                });

            modelBuilder.Entity("SuumoScraping.Models.NewBukkenFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("File_Id")
                        .HasColumnType("int");

                    b.Property<int?>("NewBukken_Id")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("File_Id");

                    b.HasIndex("NewBukken_Id");

                    b.ToTable("newbukkenfiles");
                });

            modelBuilder.Entity("SuumoScraping.Models.Price", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("ChangedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<decimal?>("Max")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Min")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("NewBukken_Id")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("NewBukken_Id");

                    b.ToTable("prices");
                });

            modelBuilder.Entity("SuumoScraping.Models.Bukken", b =>
                {
                    b.HasOne("SuumoScraping.Models.BukkenFulltext", "FullText")
                        .WithMany()
                        .HasForeignKey("FullText_Id");

                    b.OwnsOne("SuumoScraping.Models.Company", "Company", b1 =>
                        {
                            b1.Property<int>("BukkenId")
                                .HasColumnType("int");

                            b1.Property<string>("Address")
                                .HasMaxLength(100)
                                .HasColumnType("varchar(100)");

                            b1.Property<string>("Name")
                                .HasMaxLength(100)
                                .HasColumnType("varchar(100)");

                            b1.Property<string>("TakkenLicense")
                                .HasMaxLength(80)
                                .HasColumnType("varchar(80)");

                            b1.Property<string>("TransactionAspect")
                                .HasMaxLength(80)
                                .HasColumnType("varchar(80)");

                            b1.HasKey("BukkenId");

                            b1.ToTable("bukkens");

                            b1.WithOwner()
                                .HasForeignKey("BukkenId");
                        });

                    b.Navigation("Company");

                    b.Navigation("FullText");
                });

            modelBuilder.Entity("SuumoScraping.Models.BukkenFile", b =>
                {
                    b.HasOne("SuumoScraping.Models.Bukken", "Bukken")
                        .WithMany("Files")
                        .HasForeignKey("Bukken_Id");

                    b.HasOne("SuumoScraping.Models.File", "File")
                        .WithMany("BukkenFiles")
                        .HasForeignKey("File_Id");

                    b.Navigation("Bukken");

                    b.Navigation("File");
                });

            modelBuilder.Entity("SuumoScraping.Models.NewBukken", b =>
                {
                    b.OwnsOne("SuumoScraping.Models.Company", "Company", b1 =>
                        {
                            b1.Property<int>("NewBukkenId")
                                .HasColumnType("int");

                            b1.Property<string>("Address")
                                .HasMaxLength(100)
                                .HasColumnType("varchar(100)");

                            b1.Property<string>("Name")
                                .HasMaxLength(100)
                                .HasColumnType("varchar(100)");

                            b1.Property<string>("TakkenLicense")
                                .HasMaxLength(80)
                                .HasColumnType("varchar(80)");

                            b1.Property<string>("TransactionAspect")
                                .HasMaxLength(80)
                                .HasColumnType("varchar(80)");

                            b1.HasKey("NewBukkenId");

                            b1.ToTable("newbukkens");

                            b1.WithOwner()
                                .HasForeignKey("NewBukkenId");
                        });

                    b.Navigation("Company");
                });

            modelBuilder.Entity("SuumoScraping.Models.NewBukkenFile", b =>
                {
                    b.HasOne("SuumoScraping.Models.File", "File")
                        .WithMany()
                        .HasForeignKey("File_Id");

                    b.HasOne("SuumoScraping.Models.NewBukken", "NewBukken")
                        .WithMany("Files")
                        .HasForeignKey("NewBukken_Id");

                    b.Navigation("File");

                    b.Navigation("NewBukken");
                });

            modelBuilder.Entity("SuumoScraping.Models.Price", b =>
                {
                    b.HasOne("SuumoScraping.Models.NewBukken", "NewBukken")
                        .WithMany("PriceChangesets")
                        .HasForeignKey("NewBukken_Id");

                    b.Navigation("NewBukken");
                });

            modelBuilder.Entity("SuumoScraping.Models.Bukken", b =>
                {
                    b.Navigation("Files");
                });

            modelBuilder.Entity("SuumoScraping.Models.File", b =>
                {
                    b.Navigation("BukkenFiles");
                });

            modelBuilder.Entity("SuumoScraping.Models.NewBukken", b =>
                {
                    b.Navigation("Files");

                    b.Navigation("PriceChangesets");
                });
#pragma warning restore 612, 618
        }
    }
}
