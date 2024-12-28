﻿// <auto-generated />
using System;
using Hotel.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Hotel.Migrations
{
    [DbContext(typeof(HotelDbContext))]
    [Migration("20241228194835_CreateRoomsTable")]
    partial class CreateRoomsTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Hotel.Data.Room", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Floor")
                        .HasColumnType("int");

                    b.Property<int?>("ResidentId")
                        .HasColumnType("int");

                    b.Property<int>("RoomNumber")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("ResidentId");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("Hotel.Data.RoomStatistic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CurrentOccupancy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastCleaned")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("RoomId")
                        .HasColumnType("int");

                    b.Property<int>("TotalReservations")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoomId");

                    b.ToTable("RoomStatistics");
                });

            modelBuilder.Entity("Hotel.Data.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .HasColumnType("longtext");

                    b.Property<string>("FirebaseUid")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<string>("Role")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Hotel.Data.Room", b =>
                {
                    b.HasOne("Hotel.Data.User", "Resident")
                        .WithMany()
                        .HasForeignKey("ResidentId");

                    b.Navigation("Resident");
                });

            modelBuilder.Entity("Hotel.Data.RoomStatistic", b =>
                {
                    b.HasOne("Hotel.Data.Room", "Room")
                        .WithMany()
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Room");
                });
#pragma warning restore 612, 618
        }
    }
}
