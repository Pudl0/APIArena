﻿// <auto-generated />
using System;
using APIArena.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace APIArena.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("APIArena.Models.ApiKey", b =>
                {
                    b.Property<byte[]>("Id")
                        .HasMaxLength(32)
                        .HasColumnType("varbinary(32)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("varchar(32)");

                    b.Property<byte[]>("Salt")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("varbinary(16)");

                    b.Property<string>("Scopes")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("ApiKeys");
                });

            modelBuilder.Entity("APIArena.Models.Player", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("Experience")
                        .HasColumnType("int");

                    b.Property<int>("Gold")
                        .HasColumnType("int");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("APIArena.Models.Session", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("ArenaId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("Player1Id")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("Player2Id")
                        .HasColumnType("char(36)");

                    b.Property<int>("Round")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Player1Id");

                    b.HasIndex("Player2Id");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("APIArena.Models.Session", b =>
                {
                    b.HasOne("APIArena.Models.Player", "Player1")
                        .WithMany()
                        .HasForeignKey("Player1Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("APIArena.Models.Player", "Player2")
                        .WithMany()
                        .HasForeignKey("Player2Id");

                    b.Navigation("Player1");

                    b.Navigation("Player2");
                });
#pragma warning restore 612, 618
        }
    }
}
