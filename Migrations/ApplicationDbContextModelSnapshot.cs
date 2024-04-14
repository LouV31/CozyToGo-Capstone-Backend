﻿// <auto-generated />
using System;
using CozyToGo.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CozyToGo.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CozyToGo.Models.Address", b =>
                {
                    b.Property<int>("IdAddress")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdAddress"));

                    b.Property<string>("AddressName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IdUser")
                        .HasColumnType("int");

                    b.Property<bool>("IsPrimary")
                        .HasColumnType("bit");

                    b.Property<string>("StreetAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ZipCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdAddress");

                    b.HasIndex("IdUser");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("CozyToGo.Models.Dish", b =>
                {
                    b.Property<int>("IdDish")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdDish"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IdRestaurant")
                        .HasColumnType("int");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdDish");

                    b.HasIndex("IdRestaurant");

                    b.ToTable("Dishes");
                });

            modelBuilder.Entity("CozyToGo.Models.DishIngredient", b =>
                {
                    b.Property<int>("IdDishIngredient")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdDishIngredient"));

                    b.Property<int>("IdDish")
                        .HasColumnType("int");

                    b.Property<int>("IdIngredient")
                        .HasColumnType("int");

                    b.HasKey("IdDishIngredient");

                    b.HasIndex("IdDish");

                    b.HasIndex("IdIngredient");

                    b.ToTable("DishIngredients");
                });

            modelBuilder.Entity("CozyToGo.Models.Ingredient", b =>
                {
                    b.Property<int>("IdIngredient")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdIngredient"));

                    b.Property<int>("IdRestaurant")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("IdIngredient");

                    b.HasIndex("IdRestaurant");

                    b.ToTable("Ingredients");
                });

            modelBuilder.Entity("CozyToGo.Models.Order", b =>
                {
                    b.Property<int>("IdOrder")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdOrder"));

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DeliveryAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DeliveryDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdUser")
                        .HasColumnType("int");

                    b.Property<bool>("IsDelivered")
                        .HasColumnType("bit");

                    b.Property<string>("Notes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("Total")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("IdOrder");

                    b.HasIndex("IdUser");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("CozyToGo.Models.OrderDetail", b =>
                {
                    b.Property<int>("IdOrderDetail")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdOrderDetail"));

                    b.Property<int>("IdDish")
                        .HasColumnType("int");

                    b.Property<int>("IdOrder")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("IdOrderDetail");

                    b.HasIndex("IdDish");

                    b.HasIndex("IdOrder");

                    b.ToTable("OrderDetails");
                });

            modelBuilder.Entity("CozyToGo.Models.Restaurant", b =>
                {
                    b.Property<int>("IdRestaurant")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdRestaurant"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Category")
                        .HasColumnType("int");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClosingDay")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeSpan>("ClosingHours")
                        .HasColumnType("time");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<decimal>("Latitude")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Longitude")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeSpan>("OpeningHours")
                        .HasColumnType("time");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ZipCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdRestaurant");

                    b.ToTable("Restaurants");
                });

            modelBuilder.Entity("CozyToGo.Models.User", b =>
                {
                    b.Property<int>("IdUser")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdUser"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdUser");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CozyToGo.Models.Address", b =>
                {
                    b.HasOne("CozyToGo.Models.User", "User")
                        .WithMany("Addresses")
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("CozyToGo.Models.Dish", b =>
                {
                    b.HasOne("CozyToGo.Models.Restaurant", "Restaurant")
                        .WithMany("Dishes")
                        .HasForeignKey("IdRestaurant")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Restaurant");
                });

            modelBuilder.Entity("CozyToGo.Models.DishIngredient", b =>
                {
                    b.HasOne("CozyToGo.Models.Dish", "Dish")
                        .WithMany("DishIngredients")
                        .HasForeignKey("IdDish")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("CozyToGo.Models.Ingredient", "Ingredient")
                        .WithMany("DishIngredients")
                        .HasForeignKey("IdIngredient")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Dish");

                    b.Navigation("Ingredient");
                });

            modelBuilder.Entity("CozyToGo.Models.Ingredient", b =>
                {
                    b.HasOne("CozyToGo.Models.Restaurant", "Restaurant")
                        .WithMany("Ingredients")
                        .HasForeignKey("IdRestaurant")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Restaurant");
                });

            modelBuilder.Entity("CozyToGo.Models.Order", b =>
                {
                    b.HasOne("CozyToGo.Models.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("CozyToGo.Models.OrderDetail", b =>
                {
                    b.HasOne("CozyToGo.Models.Dish", "Dish")
                        .WithMany()
                        .HasForeignKey("IdDish")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CozyToGo.Models.Order", "Order")
                        .WithMany("OrderDetails")
                        .HasForeignKey("IdOrder")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Dish");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("CozyToGo.Models.Dish", b =>
                {
                    b.Navigation("DishIngredients");
                });

            modelBuilder.Entity("CozyToGo.Models.Ingredient", b =>
                {
                    b.Navigation("DishIngredients");
                });

            modelBuilder.Entity("CozyToGo.Models.Order", b =>
                {
                    b.Navigation("OrderDetails");
                });

            modelBuilder.Entity("CozyToGo.Models.Restaurant", b =>
                {
                    b.Navigation("Dishes");

                    b.Navigation("Ingredients");
                });

            modelBuilder.Entity("CozyToGo.Models.User", b =>
                {
                    b.Navigation("Addresses");

                    b.Navigation("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}
