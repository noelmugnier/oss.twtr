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
    [Migration("20221107231011_AddTrendings")]
    partial class AddTrendings
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

                    b.Property<Guid?>("PinnedTweetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PinnedTweetId")
                        .IsUnique()
                        .HasFilter("[PinnedTweetId] IS NOT NULL");

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

                    b.HasIndex("UserIdToBlock");

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

                    b.HasIndex("TweetId");

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

                    b.HasIndex("TweetId");

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

                    b.HasIndex("UserIdToMute");

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

            modelBuilder.Entity("OSS.Twtr.App.Domain.Entities.Token", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("Kind")
                        .HasColumnType("int");

                    b.Property<Guid>("TweetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedOn");

                    b.HasIndex("TweetId");

                    b.ToTable("Tokens", (string)null);
                });

            modelBuilder.Entity("OSS.Twtr.App.Domain.Entities.Trending", b =>
                {
                    b.Property<DateTime>("AnalyzedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("TweetCount")
                        .HasColumnType("int");

                    b.HasKey("AnalyzedOn", "Name");

                    b.ToTable("Trendings", (string)null);
                });

            modelBuilder.Entity("OSS.Twtr.App.Domain.Entities.Tweet", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AllowedReplies")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

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

            modelBuilder.Entity("OSS.Twtr.App.Domain.Entities.Author", b =>
                {
                    b.HasOne("OSS.Twtr.App.Domain.Entities.Tweet", null)
                        .WithOne()
                        .HasForeignKey("OSS.Twtr.App.Domain.Entities.Author", "PinnedTweetId");
                });

            modelBuilder.Entity("OSS.Twtr.App.Domain.Entities.Block", b =>
                {
                    b.HasOne("OSS.Twtr.App.Domain.Entities.Author", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("OSS.Twtr.App.Domain.Entities.Author", null)
                        .WithMany()
                        .HasForeignKey("UserIdToBlock")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OSS.Twtr.App.Domain.Entities.Bookmark", b =>
                {
                    b.HasOne("OSS.Twtr.App.Domain.Entities.Tweet", null)
                        .WithMany()
                        .HasForeignKey("TweetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OSS.Twtr.App.Domain.Entities.Author", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("OSS.Twtr.App.Domain.Entities.Like", b =>
                {
                    b.HasOne("OSS.Twtr.App.Domain.Entities.Tweet", null)
                        .WithMany()
                        .HasForeignKey("TweetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OSS.Twtr.App.Domain.Entities.Author", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("OSS.Twtr.App.Domain.Entities.Mute", b =>
                {
                    b.HasOne("OSS.Twtr.App.Domain.Entities.Author", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("OSS.Twtr.App.Domain.Entities.Author", null)
                        .WithMany()
                        .HasForeignKey("UserIdToMute")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OSS.Twtr.App.Domain.Entities.Token", b =>
                {
                    b.HasOne("OSS.Twtr.App.Domain.Entities.Tweet", null)
                        .WithMany()
                        .HasForeignKey("TweetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OSS.Twtr.App.Domain.Entities.Tweet", b =>
                {
                    b.HasOne("OSS.Twtr.App.Domain.Entities.Author", null)
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OSS.Twtr.App.Domain.Entities.Tweet", "ReferenceTweet")
                        .WithMany()
                        .HasForeignKey("ReferenceTweetId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("ReferenceTweet");
                });
#pragma warning restore 612, 618
        }
    }
}
