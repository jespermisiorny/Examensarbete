﻿// <auto-generated />
using System;
using Examensarbete.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Examensarbete.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240528070847_OnDeleteBehaviourChangeOnOrderData")]
    partial class OnDeleteBehaviourChangeOnOrderData
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.19")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Examensarbete.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Examensarbete.Models.Filter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Filters");
                });

            modelBuilder.Entity("Examensarbete.Models.FilterCategory", b =>
                {
                    b.Property<int>("FilterId")
                        .HasColumnType("int");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.HasKey("FilterId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("FilterCategories");
                });

            modelBuilder.Entity("Examensarbete.Models.Material", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("DefaultProductionProcess")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("EFEoLIncineration")
                        .HasColumnType("decimal(18, 5)");

                    b.Property<decimal>("EFEoLRecycling")
                        .HasColumnType("decimal(18, 5)");

                    b.Property<decimal>("EFMaterialNew")
                        .HasColumnType("decimal(18, 5)");

                    b.Property<decimal>("EFMaterialRecycled")
                        .HasColumnType("decimal(18, 5)");

                    b.Property<decimal>("EFProductionProcess")
                        .HasColumnType("decimal(18, 5)");

                    b.Property<decimal>("RecycledContentInMaterial")
                        .HasColumnType("decimal(18, 5)");

                    b.Property<decimal>("RecyclingRateAtEoL")
                        .HasColumnType("decimal(18, 5)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Materials");
                });

            modelBuilder.Entity("Examensarbete.Models.OrderData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Account")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ArticleNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("ConfirmedNetAmount")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<int?>("ConfirmedQuantity")
                        .HasColumnType("int");

                    b.Property<string>("CostCenter")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("ItemDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("date");

                    b.Property<string>("OrderGroup")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Organization")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("Price")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.Property<string>("SubArea")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SupplierName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UnitType")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("OrderData");
                });

            modelBuilder.Entity("Examensarbete.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ArticleNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsInactive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsIncomplete")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PackagingMaterialId")
                        .HasColumnType("int");

                    b.Property<decimal>("PricePerUnit")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<double>("RecyclingRateAtEoL")
                        .HasColumnType("float");

                    b.Property<double>("WeightPerUnit")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("PackagingMaterialId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Examensarbete.Models.ProductCategory", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.HasKey("ProductId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("ProductCategories");
                });

            modelBuilder.Entity("Examensarbete.Models.ProductMaterial", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("MaterialId")
                        .HasColumnType("int");

                    b.Property<int>("Percentage")
                        .HasColumnType("int");

                    b.HasKey("ProductId", "MaterialId");

                    b.HasIndex("MaterialId");

                    b.ToTable("ProductMaterials");
                });

            modelBuilder.Entity("Examensarbete.Models.Category", b =>
                {
                    b.HasOne("Examensarbete.Models.Category", "ParentCategory")
                        .WithMany("SubCategories")
                        .HasForeignKey("ParentId");

                    b.Navigation("ParentCategory");
                });

            modelBuilder.Entity("Examensarbete.Models.FilterCategory", b =>
                {
                    b.HasOne("Examensarbete.Models.Category", "Category")
                        .WithMany("FilterCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Examensarbete.Models.Filter", "Filter")
                        .WithMany("FilterCategories")
                        .HasForeignKey("FilterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Filter");
                });

            modelBuilder.Entity("Examensarbete.Models.OrderData", b =>
                {
                    b.HasOne("Examensarbete.Models.Product", "Product")
                        .WithMany("OrderDatas")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Examensarbete.Models.Product", b =>
                {
                    b.HasOne("Examensarbete.Models.Material", "PackagingMaterial")
                        .WithMany()
                        .HasForeignKey("PackagingMaterialId");

                    b.Navigation("PackagingMaterial");
                });

            modelBuilder.Entity("Examensarbete.Models.ProductCategory", b =>
                {
                    b.HasOne("Examensarbete.Models.Category", "Category")
                        .WithMany("ProductCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Examensarbete.Models.Product", "Product")
                        .WithMany("ProductCategories")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Examensarbete.Models.ProductMaterial", b =>
                {
                    b.HasOne("Examensarbete.Models.Material", "Material")
                        .WithMany("ProductMaterials")
                        .HasForeignKey("MaterialId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Examensarbete.Models.Product", "Product")
                        .WithMany("ProductMaterials")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Material");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Examensarbete.Models.Category", b =>
                {
                    b.Navigation("FilterCategories");

                    b.Navigation("ProductCategories");

                    b.Navigation("SubCategories");
                });

            modelBuilder.Entity("Examensarbete.Models.Filter", b =>
                {
                    b.Navigation("FilterCategories");
                });

            modelBuilder.Entity("Examensarbete.Models.Material", b =>
                {
                    b.Navigation("ProductMaterials");
                });

            modelBuilder.Entity("Examensarbete.Models.Product", b =>
                {
                    b.Navigation("OrderDatas");

                    b.Navigation("ProductCategories");

                    b.Navigation("ProductMaterials");
                });
#pragma warning restore 612, 618
        }
    }
}
