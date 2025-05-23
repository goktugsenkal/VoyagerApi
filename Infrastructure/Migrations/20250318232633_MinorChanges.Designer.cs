﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20250318232633_MinorChanges")]
    partial class MinorChanges
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Core.Entities.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsLikedByAuthor")
                        .HasColumnType("boolean");

                    b.Property<int>("LikeCount")
                        .HasColumnType("integer");

                    b.Property<Guid>("VoyageId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("VoyagerUserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("VoyageId");

                    b.HasIndex("VoyagerUserId");

                    b.ToTable("Comments", (string)null);
                });

            modelBuilder.Entity("Core.Entities.Like", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("CommentId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("LikeType")
                        .HasColumnType("integer");

                    b.Property<Guid?>("VoyageId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("VoyagerUserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CommentId");

                    b.HasIndex("VoyagerUserId");

                    b.HasIndex("VoyageId", "CommentId", "VoyagerUserId")
                        .IsUnique()
                        .HasDatabaseName("IX_Likes_UniqueUserLike");

                    b.ToTable("Likes", (string)null);
                });

            modelBuilder.Entity("Core.Entities.Stop", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("ArrivalTimeToNextStop")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("DistanceToNextStop")
                        .HasColumnType("integer");

                    b.Property<int>("ImageCount")
                        .HasColumnType("integer");

                    b.PrimitiveCollection<List<string>>("ImageUrls")
                        .HasColumnType("text[]");

                    b.Property<bool>("IsFocalPoint")
                        .HasColumnType("boolean");

                    b.Property<double>("Latitude")
                        .HasColumnType("double precision");

                    b.Property<double>("Longitude")
                        .HasColumnType("double precision");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("OrderIndex")
                        .HasColumnType("integer");

                    b.Property<int>("TransportationTypeToNextStop")
                        .HasColumnType("integer");

                    b.Property<Guid>("VoyageId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("VoyageId");

                    b.HasIndex("IsFocalPoint", "Latitude", "Longitude");

                    b.ToTable("Stops");
                });

            modelBuilder.Entity("Core.Entities.Voyage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("ActualPrice")
                        .HasColumnType("integer");

                    b.Property<int>("CommentCount")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Currency")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("ExpectedPrice")
                        .HasColumnType("integer");

                    b.PrimitiveCollection<List<string>>("ImageUrls")
                        .IsRequired()
                        .HasColumnType("text[]");

                    b.Property<bool>("IsArchived")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("boolean");

                    b.Property<int>("LikeCount")
                        .HasColumnType("integer");

                    b.Property<string>("LocationName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("StopCount")
                        .HasColumnType("integer");

                    b.Property<string>("ThumbnailUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("VoyagerUserId")
                        .HasColumnType("uuid");

                    b.Property<string>("VoyagerUsername")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("VoyagerUserId");

                    b.ToTable("Voyages");
                });

            modelBuilder.Entity("Core.Entities.VoyagerUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("BannerPictureUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Bio")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("CompletedPlanCount")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("FollowerCount")
                        .HasColumnType("integer");

                    b.Property<int>("FollowingCount")
                        .HasColumnType("integer");

                    b.Property<int>("InspiredCount")
                        .HasColumnType("integer");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("LikeCount")
                        .HasColumnType("integer");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NormalizedUsername")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<int>("PlanCount")
                        .HasColumnType("integer");

                    b.Property<string>("ProfilePictureUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("text");

                    b.Property<DateTime?>("RefreshTokenExpiryTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Core.Entities.Comment", b =>
                {
                    b.HasOne("Core.Entities.Voyage", "Voyage")
                        .WithMany("Comments")
                        .HasForeignKey("VoyageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.Entities.VoyagerUser", "VoyagerUser")
                        .WithMany()
                        .HasForeignKey("VoyagerUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Voyage");

                    b.Navigation("VoyagerUser");
                });

            modelBuilder.Entity("Core.Entities.Like", b =>
                {
                    b.HasOne("Core.Entities.Comment", "Comment")
                        .WithMany("Likes")
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Core.Entities.Voyage", "Voyage")
                        .WithMany("Likes")
                        .HasForeignKey("VoyageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Core.Entities.VoyagerUser", "VoyagerUser")
                        .WithMany()
                        .HasForeignKey("VoyagerUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Comment");

                    b.Navigation("Voyage");

                    b.Navigation("VoyagerUser");
                });

            modelBuilder.Entity("Core.Entities.Stop", b =>
                {
                    b.HasOne("Core.Entities.Voyage", "Voyage")
                        .WithMany("Stops")
                        .HasForeignKey("VoyageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Voyage");
                });

            modelBuilder.Entity("Core.Entities.Voyage", b =>
                {
                    b.HasOne("Core.Entities.VoyagerUser", "User")
                        .WithMany("Voyages")
                        .HasForeignKey("VoyagerUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Core.Entities.Comment", b =>
                {
                    b.Navigation("Likes");
                });

            modelBuilder.Entity("Core.Entities.Voyage", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Likes");

                    b.Navigation("Stops");
                });

            modelBuilder.Entity("Core.Entities.VoyagerUser", b =>
                {
                    b.Navigation("Voyages");
                });
#pragma warning restore 612, 618
        }
    }
}
