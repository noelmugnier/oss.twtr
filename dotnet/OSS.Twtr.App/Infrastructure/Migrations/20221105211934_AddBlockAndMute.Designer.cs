﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OSS.Twtr.App.Infrastructure;

#nullable disable

namespace OSS.Twtr.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20221105211934_AddBlockAndMute")]
    partial class AddBlockAndMute
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("OSS.Twtr.App.Domain.Entities.Author", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("MemberSince")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("OSS.Twtr.App.Domain.Entities.Block", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserIdToBlock")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("BlockedOn")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("UserId", "UserIdToBlock");

                    b.ToTable("BlockedUsers", (string)null);
                });

            modelBuilder.Entity("OSS.Twtr.App.Domain.Entities.Bookmark", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TweetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("BookmarkedOn")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("UserId", "TweetId");

                    b.ToTable("Bookmarks", (string)null);
                });

            modelBuilder.Entity("OSS.Twtr.App.Domain.Entities.Like", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TweetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("LikedOn")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("UserId", "TweetId");

                    b.ToTable("Likes", (string)null);
                });

            modelBuilder.Entity("OSS.Twtr.App.Domain.Entities.Mute", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserIdToMute")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("MutedOn")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("UserId", "UserIdToMute");

                    b.ToTable("MutedUsers", (string)null);
                });

            modelBuilder.Entity("OSS.Twtr.App.Domain.Entities.Subscription", b =>
                {
                    b.Property<Guid>("FollowerUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SubscribedToUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("SubscribedOn")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("FollowerUserId", "SubscribedToUserId");

                    b.ToTable("Subscriptions", (string)null);
                });

            modelBuilder.Entity("OSS.Twtr.App.Domain.Entities.Tweet", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Kind")
                        .HasColumnType("int");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("PostedOn")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("ReferenceTweetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ThreadId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ReferenceTweetId");

                    b.ToTable("Tweets", (string)null);
                });

            modelBuilder.Entity("OSS.Twtr.App.Domain.Entities.Tweet", b =>
                {
                    b.HasOne("OSS.Twtr.App.Domain.Entities.Author", null)
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OSS.Twtr.App.Domain.Entities.Tweet", null)
                        .WithMany()
                        .HasForeignKey("ReferenceTweetId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}