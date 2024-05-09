﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Zup;

#nullable disable

namespace Zup.Migrations
{
    [DbContext(typeof(ZupDbContext))]
    [Migration("20240509063101_AddedTags")]
    partial class AddedTags
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.4");

            modelBuilder.Entity("Zup.Entities.tbl_Tag", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Zup.Entities.tbl_TaskEntry", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("EndedOn")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("StartedOn")
                        .HasColumnType("TEXT");

                    b.Property<string>("Task")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("TaskEntries");
                });

            modelBuilder.Entity("Zup.Entities.tbl_TaskEntryNote", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.Property<string>("RTF")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("TaskID")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("TaskID");

                    b.ToTable("TaskEntryNotes");
                });

            modelBuilder.Entity("Zup.Entities.tbl_TaskEntryTag", b =>
                {
                    b.Property<Guid>("TaskID")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("TagID")
                        .HasColumnType("TEXT");

                    b.HasKey("TaskID", "TagID");

                    b.HasIndex("TagID");

                    b.ToTable("TaskEntryTags");
                });

            modelBuilder.Entity("Zup.Entities.tbl_TaskEntryNote", b =>
                {
                    b.HasOne("Zup.Entities.tbl_TaskEntry", null)
                        .WithMany()
                        .HasForeignKey("TaskID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Zup.Entities.tbl_TaskEntryTag", b =>
                {
                    b.HasOne("Zup.Entities.tbl_Tag", null)
                        .WithMany()
                        .HasForeignKey("TagID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Zup.Entities.tbl_TaskEntry", null)
                        .WithMany()
                        .HasForeignKey("TaskID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
