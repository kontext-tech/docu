﻿// <auto-generated />
using System;
using Kontext.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Kontext.Docu.Web.Portals.Migrations
{
    [DbContext(typeof(ContextBlogDbContext))]
    partial class ContextBlogDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846");

            modelBuilder.Entity("Kontext.Data.Models.Blog", b =>
                {
                    b.Property<int>("BlogId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("BlogGroupId");

                    b.Property<int?>("CommentCount");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateModified");

                    b.Property<int?>("FileCount");

                    b.Property<bool?>("IsActive");

                    b.Property<string>("LanguageCode")
                        .HasMaxLength(16);

                    b.Property<string>("News");

                    b.Property<int?>("PingTrackCount");

                    b.Property<int?>("PostCount");

                    b.Property<string>("SecondaryCss");

                    b.Property<string>("SkinCssFile")
                        .HasMaxLength(128);

                    b.Property<string>("SubTitle")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.Property<string>("Tag")
                        .HasMaxLength(128);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<string>("TrackingCode");

                    b.Property<string>("UniqueName")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<Guid?>("UserId");

                    b.HasKey("BlogId");

                    b.HasIndex("LanguageCode");

                    b.HasIndex("UniqueName")
                        .IsUnique();

                    b.ToTable("Blogs","context");
                });

            modelBuilder.Entity("Kontext.Data.Models.BlogCategory", b =>
                {
                    b.Property<int>("BlogCategoryId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<int>("BlogId");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateModified");

                    b.Property<string>("Description")
                        .HasMaxLength(2048);

                    b.Property<string>("LanguageCode")
                        .HasMaxLength(16);

                    b.Property<string>("Tag")
                        .HasMaxLength(128);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.Property<string>("UniqueName")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.HasKey("BlogCategoryId");

                    b.HasIndex("BlogId");

                    b.HasIndex("LanguageCode");

                    b.HasIndex("UniqueName")
                        .IsUnique();

                    b.ToTable("BlogCategories","context");
                });

            modelBuilder.Entity("Kontext.Data.Models.BlogMediaObject", b =>
                {
                    b.Property<int>("MediaObjectId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BlogId");

                    b.Property<int?>("BlogPostId");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateDeleted");

                    b.Property<DateTime>("DateModified");

                    b.Property<DateTime?>("DatePublished");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("LanguageCode");

                    b.Property<string>("Name")
                        .HasMaxLength(128);

                    b.Property<string>("Tag")
                        .HasMaxLength(128);

                    b.Property<string>("TypeName")
                        .HasMaxLength(32);

                    b.Property<string>("Url")
                        .HasMaxLength(256);

                    b.HasKey("MediaObjectId");

                    b.HasIndex("BlogId");

                    b.HasIndex("BlogPostId");

                    b.HasIndex("LanguageCode");

                    b.ToTable("BlogMediaObjects","context");
                });

            modelBuilder.Entity("Kontext.Data.Models.BlogPost", b =>
                {
                    b.Property<int>("BlogPostId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author")
                        .HasMaxLength(64);

                    b.Property<int?>("BlogId")
                        .IsRequired();

                    b.Property<int?>("CommentCount");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateDeleted");

                    b.Property<DateTime>("DateModified");

                    b.Property<DateTime?>("DatePublished");

                    b.Property<string>("Description")
                        .HasMaxLength(2048);

                    b.Property<string>("Email")
                        .HasMaxLength(64);

                    b.Property<string>("IpAddress");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("KeyWords")
                        .HasMaxLength(128);

                    b.Property<string>("LanguageCode")
                        .HasMaxLength(16);

                    b.Property<string>("Tag")
                        .HasMaxLength(128);

                    b.Property<string>("Text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.Property<string>("UniqueName")
                        .HasMaxLength(128);

                    b.Property<int?>("ViewCount");

                    b.HasKey("BlogPostId");

                    b.HasIndex("BlogId");

                    b.HasIndex("LanguageCode");

                    b.HasIndex("UniqueName")
                        .IsUnique();

                    b.ToTable("BlogPosts","context");
                });

            modelBuilder.Entity("Kontext.Data.Models.BlogPostCategory", b =>
                {
                    b.Property<int>("BlogCategoryId");

                    b.Property<int>("BlogPostId");

                    b.HasKey("BlogCategoryId", "BlogPostId");

                    b.HasIndex("BlogPostId");

                    b.ToTable("BlogPostCategories","context");
                });

            modelBuilder.Entity("Kontext.Data.Models.BlogPostComment", b =>
                {
                    b.Property<int>("BlogPostCommentId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Approved");

                    b.Property<string>("Author")
                        .HasMaxLength(64);

                    b.Property<int?>("BlogId");

                    b.Property<int?>("BlogPostId")
                        .IsRequired();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateDeleted");

                    b.Property<DateTime>("DateModified");

                    b.Property<string>("Email")
                        .HasMaxLength(64);

                    b.Property<string>("IpAddress")
                        .HasMaxLength(64);

                    b.Property<bool>("IsBlogUser");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("LanguageCode")
                        .HasMaxLength(16);

                    b.Property<int?>("ReplyToBlogPostCommentId");

                    b.Property<string>("Tag")
                        .HasMaxLength(128);

                    b.Property<string>("Text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.Property<string>("UserAgent")
                        .HasMaxLength(150);

                    b.Property<Guid?>("UserId");

                    b.HasKey("BlogPostCommentId");

                    b.HasIndex("BlogId");

                    b.HasIndex("BlogPostId");

                    b.HasIndex("LanguageCode");

                    b.HasIndex("ReplyToBlogPostCommentId");

                    b.ToTable("BlogPostComments","context");
                });

            modelBuilder.Entity("Kontext.Data.Models.BlogPostTag", b =>
                {
                    b.Property<int>("BlogPostId");

                    b.Property<int>("TagId");

                    b.HasKey("BlogPostId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("BlogPostTags","context");
                });

            modelBuilder.Entity("Kontext.Data.Models.Language", b =>
                {
                    b.Property<string>("LanguageCode")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(16);

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.Property<string>("Tag")
                        .HasMaxLength(128);

                    b.HasKey("LanguageCode");

                    b.ToTable("Languages","context");
                });

            modelBuilder.Entity("Kontext.Data.Models.Tag", b =>
                {
                    b.Property<int>("TagId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateDeleted");

                    b.Property<DateTime>("DateModified");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("LanguageCode")
                        .HasMaxLength(16);

                    b.Property<int>("NTile");

                    b.Property<string>("TagName")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.HasKey("TagId");

                    b.HasIndex("LanguageCode");

                    b.HasIndex("TagName")
                        .IsUnique();

                    b.ToTable("Tags","context");
                });

            modelBuilder.Entity("Kontext.Data.Models.Blog", b =>
                {
                    b.HasOne("Kontext.Data.Models.Language", "Language")
                        .WithMany("Blogs")
                        .HasForeignKey("LanguageCode");
                });

            modelBuilder.Entity("Kontext.Data.Models.BlogCategory", b =>
                {
                    b.HasOne("Kontext.Data.Models.Blog", "Blog")
                        .WithMany("Categories")
                        .HasForeignKey("BlogId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Kontext.Data.Models.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageCode");
                });

            modelBuilder.Entity("Kontext.Data.Models.BlogMediaObject", b =>
                {
                    b.HasOne("Kontext.Data.Models.Blog", "Blog")
                        .WithMany("MediaObjects")
                        .HasForeignKey("BlogId");

                    b.HasOne("Kontext.Data.Models.BlogPost", "BlogPost")
                        .WithMany("MediaObjects")
                        .HasForeignKey("BlogPostId");

                    b.HasOne("Kontext.Data.Models.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageCode");
                });

            modelBuilder.Entity("Kontext.Data.Models.BlogPost", b =>
                {
                    b.HasOne("Kontext.Data.Models.Blog", "Blog")
                        .WithMany("Posts")
                        .HasForeignKey("BlogId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Kontext.Data.Models.Language", "Language")
                        .WithMany("BlogPosts")
                        .HasForeignKey("LanguageCode");
                });

            modelBuilder.Entity("Kontext.Data.Models.BlogPostCategory", b =>
                {
                    b.HasOne("Kontext.Data.Models.BlogCategory", "BlogCategory")
                        .WithMany("BlogPosts")
                        .HasForeignKey("BlogCategoryId");

                    b.HasOne("Kontext.Data.Models.BlogPost", "BlogPost")
                        .WithMany("BlogCategories")
                        .HasForeignKey("BlogPostId");
                });

            modelBuilder.Entity("Kontext.Data.Models.BlogPostComment", b =>
                {
                    b.HasOne("Kontext.Data.Models.Blog", "Blog")
                        .WithMany("Comments")
                        .HasForeignKey("BlogId");

                    b.HasOne("Kontext.Data.Models.BlogPost", "BlogPost")
                        .WithMany("Comments")
                        .HasForeignKey("BlogPostId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Kontext.Data.Models.Language", "Language")
                        .WithMany("Comments")
                        .HasForeignKey("LanguageCode");

                    b.HasOne("Kontext.Data.Models.BlogPostComment", "ReplyToBlogPostComment")
                        .WithMany("Comments")
                        .HasForeignKey("ReplyToBlogPostCommentId");
                });

            modelBuilder.Entity("Kontext.Data.Models.BlogPostTag", b =>
                {
                    b.HasOne("Kontext.Data.Models.BlogPost", "BlogPost")
                        .WithMany("Tags")
                        .HasForeignKey("BlogPostId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Kontext.Data.Models.Tag", "Tag")
                        .WithMany("BlogPosts")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Kontext.Data.Models.Tag", b =>
                {
                    b.HasOne("Kontext.Data.Models.Language", "Language")
                        .WithMany("Tags")
                        .HasForeignKey("LanguageCode");
                });
#pragma warning restore 612, 618
        }
    }
}
