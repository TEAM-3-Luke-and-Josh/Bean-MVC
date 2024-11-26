﻿// <auto-generated />
using System;
using BeanScene.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MVC_Full.Migrations
{
    [DbContext(typeof(BeanSceneContext))]
    [Migration("20241124020820_OrderingSystemTable")]
    partial class OrderingSystemTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BeanScene.Models.Guest", b =>
                {
                    b.Property<int>("GuestID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GuestID"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("GuestID");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("PhoneNumber")
                        .IsUnique();

                    b.ToTable("Guests");
                });

            modelBuilder.Entity("BeanScene.Models.ItemOption", b =>
                {
                    b.Property<int>("OptionID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OptionID"));

                    b.Property<string>("Description")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int>("ItemID")
                        .HasColumnType("int");

                    b.Property<int>("MenuItemItemID")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<decimal>("PriceModifier")
                        .HasColumnType("decimal(10, 2)");

                    b.HasKey("OptionID");

                    b.HasIndex("MenuItemItemID");

                    b.ToTable("ItemOptions");
                });

            modelBuilder.Entity("BeanScene.Models.MenuAvailability", b =>
                {
                    b.Property<int>("ItemID")
                        .HasColumnType("int");

                    b.Property<string>("SittingType")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("bit");

                    b.Property<int>("MenuItemItemID")
                        .HasColumnType("int");

                    b.HasKey("ItemID", "SittingType");

                    b.HasIndex("MenuItemItemID");

                    b.ToTable("MenuAvailability");
                });

            modelBuilder.Entity("BeanScene.Models.MenuCategory", b =>
                {
                    b.Property<int>("CategoryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryID"));

                    b.Property<string>("Description")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("CategoryID");

                    b.ToTable("MenuCategories");
                });

            modelBuilder.Entity("BeanScene.Models.MenuItem", b =>
                {
                    b.Property<int>("ItemID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ItemID"));

                    b.Property<int>("CategoryID")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("PrepTime")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(10, 2)");

                    b.HasKey("ItemID");

                    b.HasIndex("CategoryID");

                    b.ToTable("MenuItems");
                });

            modelBuilder.Entity("BeanScene.Models.Order", b =>
                {
                    b.Property<int>("OrderID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderID"));

                    b.Property<string>("OrderStatus")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasDefaultValue("Pending");

                    b.Property<DateTime>("OrderTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ReservationID")
                        .HasColumnType("int");

                    b.Property<string>("SpecialRequests")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("TableID")
                        .HasMaxLength(4)
                        .HasColumnType("nvarchar(4)");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("decimal(10, 2)");

                    b.HasKey("OrderID");

                    b.HasIndex("ReservationID");

                    b.HasIndex("TableID");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("BeanScene.Models.OrderItem", b =>
                {
                    b.Property<int>("OrderItemID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderItemID"));

                    b.Property<int>("ItemID")
                        .HasColumnType("int");

                    b.Property<string>("ItemStatus")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasDefaultValue("Pending");

                    b.Property<int>("MenuItemItemID")
                        .HasColumnType("int");

                    b.Property<int>("OrderID")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("SpecialInstructions")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<decimal>("Subtotal")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(10, 2)");

                    b.HasKey("OrderItemID");

                    b.HasIndex("MenuItemItemID");

                    b.HasIndex("OrderID");

                    b.ToTable("OrderItems");
                });

            modelBuilder.Entity("BeanScene.Models.Reservation", b =>
                {
                    b.Property<int>("ReservationID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ReservationID"));

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("GuestID")
                        .HasColumnType("int");

                    b.Property<string>("Notes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NumberOfGuests")
                        .HasColumnType("int");

                    b.Property<string>("ReservationStatus")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasDefaultValue("Pending");

                    b.Property<int>("SittingID")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.HasKey("ReservationID");

                    b.HasIndex("GuestID");

                    b.HasIndex("SittingID");

                    b.ToTable("Reservations");
                });

            modelBuilder.Entity("BeanScene.Models.Sitting", b =>
                {
                    b.Property<int>("SittingID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SittingID"));

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<bool>("ClosedForReservations")
                        .HasColumnType("bit");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("SittingType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.HasKey("SittingID");

                    b.ToTable("Sittings");
                });

            modelBuilder.Entity("BeanScene.Models.Table", b =>
                {
                    b.Property<string>("TableID")
                        .HasMaxLength(4)
                        .HasColumnType("nvarchar(4)");

                    b.Property<string>("Area")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<int?>("OrderID")
                        .HasColumnType("int");

                    b.HasKey("TableID");

                    b.HasIndex("OrderID");

                    b.ToTable("Tables");
                });

            modelBuilder.Entity("BeanScene.Models.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserID"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("UserID");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ItemOptionOrderItem", b =>
                {
                    b.Property<int>("OrderItemsOrderItemID")
                        .HasColumnType("int");

                    b.Property<int>("SelectedOptionsOptionID")
                        .HasColumnType("int");

                    b.HasKey("OrderItemsOrderItemID", "SelectedOptionsOptionID");

                    b.HasIndex("SelectedOptionsOptionID");

                    b.ToTable("OrderItemOptions", (string)null);
                });

            modelBuilder.Entity("ReservationTable", b =>
                {
                    b.Property<int>("ReservationsReservationID")
                        .HasColumnType("int");

                    b.Property<string>("TablesTableID")
                        .HasColumnType("nvarchar(4)");

                    b.HasKey("ReservationsReservationID", "TablesTableID");

                    b.HasIndex("TablesTableID");

                    b.ToTable("ReservationTables", (string)null);
                });

            modelBuilder.Entity("BeanScene.Models.ItemOption", b =>
                {
                    b.HasOne("BeanScene.Models.MenuItem", "MenuItem")
                        .WithMany("Options")
                        .HasForeignKey("MenuItemItemID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("MenuItem");
                });

            modelBuilder.Entity("BeanScene.Models.MenuAvailability", b =>
                {
                    b.HasOne("BeanScene.Models.MenuItem", "MenuItem")
                        .WithMany("Availability")
                        .HasForeignKey("MenuItemItemID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MenuItem");
                });

            modelBuilder.Entity("BeanScene.Models.MenuItem", b =>
                {
                    b.HasOne("BeanScene.Models.MenuCategory", "Category")
                        .WithMany("MenuItems")
                        .HasForeignKey("CategoryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("BeanScene.Models.Order", b =>
                {
                    b.HasOne("BeanScene.Models.Reservation", "Reservation")
                        .WithMany()
                        .HasForeignKey("ReservationID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("BeanScene.Models.Table", "Table")
                        .WithMany()
                        .HasForeignKey("TableID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Reservation");

                    b.Navigation("Table");
                });

            modelBuilder.Entity("BeanScene.Models.OrderItem", b =>
                {
                    b.HasOne("BeanScene.Models.MenuItem", "MenuItem")
                        .WithMany("OrderItems")
                        .HasForeignKey("MenuItemItemID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("BeanScene.Models.Order", "Order")
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MenuItem");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("BeanScene.Models.Reservation", b =>
                {
                    b.HasOne("BeanScene.Models.Guest", "Guest")
                        .WithMany("Reservations")
                        .HasForeignKey("GuestID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("BeanScene.Models.Sitting", "Sitting")
                        .WithMany("Reservations")
                        .HasForeignKey("SittingID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Guest");

                    b.Navigation("Sitting");
                });

            modelBuilder.Entity("BeanScene.Models.Table", b =>
                {
                    b.HasOne("BeanScene.Models.Order", null)
                        .WithMany("AssignedTables")
                        .HasForeignKey("OrderID");
                });

            modelBuilder.Entity("ItemOptionOrderItem", b =>
                {
                    b.HasOne("BeanScene.Models.OrderItem", null)
                        .WithMany()
                        .HasForeignKey("OrderItemsOrderItemID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BeanScene.Models.ItemOption", null)
                        .WithMany()
                        .HasForeignKey("SelectedOptionsOptionID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ReservationTable", b =>
                {
                    b.HasOne("BeanScene.Models.Reservation", null)
                        .WithMany()
                        .HasForeignKey("ReservationsReservationID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BeanScene.Models.Table", null)
                        .WithMany()
                        .HasForeignKey("TablesTableID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BeanScene.Models.Guest", b =>
                {
                    b.Navigation("Reservations");
                });

            modelBuilder.Entity("BeanScene.Models.MenuCategory", b =>
                {
                    b.Navigation("MenuItems");
                });

            modelBuilder.Entity("BeanScene.Models.MenuItem", b =>
                {
                    b.Navigation("Availability");

                    b.Navigation("Options");

                    b.Navigation("OrderItems");
                });

            modelBuilder.Entity("BeanScene.Models.Order", b =>
                {
                    b.Navigation("AssignedTables");

                    b.Navigation("OrderItems");
                });

            modelBuilder.Entity("BeanScene.Models.Sitting", b =>
                {
                    b.Navigation("Reservations");
                });
#pragma warning restore 612, 618
        }
    }
}