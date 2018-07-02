using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Kontext.Docu.Web.Portals.Migrations
{
    public partial class InitDocuContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "context");

            migrationBuilder.CreateTable(
                name: "Languages",
                schema: "context",
                columns: table => new
                {
                    LanguageCode = table.Column<string>(maxLength: 16, nullable: false),
                    DisplayName = table.Column<string>(maxLength: 32, nullable: false),
                    Tag = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.LanguageCode);
                });

            migrationBuilder.CreateTable(
                name: "Blogs",
                schema: "context",
                columns: table => new
                {
                    BlogId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<Guid>(nullable: true),
                    BlogGroupId = table.Column<Guid>(nullable: true),
                    Title = table.Column<string>(maxLength: 128, nullable: false),
                    SubTitle = table.Column<string>(maxLength: 256, nullable: false),
                    UniqueName = table.Column<string>(maxLength: 128, nullable: false),
                    IsActive = table.Column<bool>(nullable: true),
                    LanguageCode = table.Column<string>(maxLength: 16, nullable: true),
                    SkinCssFile = table.Column<string>(maxLength: 128, nullable: true),
                    SecondaryCss = table.Column<string>(nullable: true),
                    PostCount = table.Column<int>(nullable: true),
                    CommentCount = table.Column<int>(nullable: true),
                    FileCount = table.Column<int>(nullable: true),
                    PingTrackCount = table.Column<int>(nullable: true),
                    News = table.Column<string>(nullable: true),
                    TrackingCode = table.Column<string>(nullable: true),
                    Tag = table.Column<string>(maxLength: 128, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blogs", x => x.BlogId);
                    table.ForeignKey(
                        name: "FK_Blogs_Languages_LanguageCode",
                        column: x => x.LanguageCode,
                        principalSchema: "context",
                        principalTable: "Languages",
                        principalColumn: "LanguageCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                schema: "context",
                columns: table => new
                {
                    TagId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TagName = table.Column<string>(maxLength: 64, nullable: false),
                    LanguageCode = table.Column<string>(maxLength: 16, nullable: true),
                    NTile = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateDeleted = table.Column<DateTime>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.TagId);
                    table.ForeignKey(
                        name: "FK_Tags_Languages_LanguageCode",
                        column: x => x.LanguageCode,
                        principalSchema: "context",
                        principalTable: "Languages",
                        principalColumn: "LanguageCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BlogCategories",
                schema: "context",
                columns: table => new
                {
                    BlogCategoryId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UniqueName = table.Column<string>(maxLength: 128, nullable: false),
                    Title = table.Column<string>(maxLength: 256, nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(maxLength: 2048, nullable: true),
                    LanguageCode = table.Column<string>(maxLength: 16, nullable: true),
                    Tag = table.Column<string>(maxLength: 128, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    BlogId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogCategories", x => x.BlogCategoryId);
                    table.ForeignKey(
                        name: "FK_BlogCategories_Blogs_BlogId",
                        column: x => x.BlogId,
                        principalSchema: "context",
                        principalTable: "Blogs",
                        principalColumn: "BlogId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogCategories_Languages_LanguageCode",
                        column: x => x.LanguageCode,
                        principalSchema: "context",
                        principalTable: "Languages",
                        principalColumn: "LanguageCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BlogPosts",
                schema: "context",
                columns: table => new
                {
                    BlogPostId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(maxLength: 256, nullable: false),
                    Author = table.Column<string>(maxLength: 64, nullable: true),
                    Email = table.Column<string>(maxLength: 64, nullable: true),
                    LanguageCode = table.Column<string>(maxLength: 16, nullable: true),
                    Description = table.Column<string>(maxLength: 2048, nullable: true),
                    KeyWords = table.Column<string>(maxLength: 128, nullable: true),
                    Text = table.Column<string>(nullable: true),
                    ViewCount = table.Column<int>(nullable: true),
                    CommentCount = table.Column<int>(nullable: true),
                    UniqueName = table.Column<string>(maxLength: 128, nullable: true),
                    IpAddress = table.Column<string>(nullable: true),
                    Tag = table.Column<string>(maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateDeleted = table.Column<DateTime>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    DatePublished = table.Column<DateTime>(nullable: true),
                    BlogId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPosts", x => x.BlogPostId);
                    table.ForeignKey(
                        name: "FK_BlogPosts_Blogs_BlogId",
                        column: x => x.BlogId,
                        principalSchema: "context",
                        principalTable: "Blogs",
                        principalColumn: "BlogId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogPosts_Languages_LanguageCode",
                        column: x => x.LanguageCode,
                        principalSchema: "context",
                        principalTable: "Languages",
                        principalColumn: "LanguageCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BlogMediaObjects",
                schema: "context",
                columns: table => new
                {
                    MediaObjectId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TypeName = table.Column<string>(maxLength: 32, nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: true),
                    Url = table.Column<string>(maxLength: 256, nullable: true),
                    BlogPostId = table.Column<int>(nullable: true),
                    BlogId = table.Column<int>(nullable: true),
                    Tag = table.Column<string>(maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateDeleted = table.Column<DateTime>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    DatePublished = table.Column<DateTime>(nullable: true),
                    LanguageCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogMediaObjects", x => x.MediaObjectId);
                    table.ForeignKey(
                        name: "FK_BlogMediaObjects_Blogs_BlogId",
                        column: x => x.BlogId,
                        principalSchema: "context",
                        principalTable: "Blogs",
                        principalColumn: "BlogId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BlogMediaObjects_BlogPosts_BlogPostId",
                        column: x => x.BlogPostId,
                        principalSchema: "context",
                        principalTable: "BlogPosts",
                        principalColumn: "BlogPostId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BlogMediaObjects_Languages_LanguageCode",
                        column: x => x.LanguageCode,
                        principalSchema: "context",
                        principalTable: "Languages",
                        principalColumn: "LanguageCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BlogPostCategories",
                schema: "context",
                columns: table => new
                {
                    BlogPostId = table.Column<int>(nullable: false),
                    BlogCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPostCategories", x => new { x.BlogCategoryId, x.BlogPostId });
                    table.ForeignKey(
                        name: "FK_BlogPostCategories_BlogCategories_BlogCategoryId",
                        column: x => x.BlogCategoryId,
                        principalSchema: "context",
                        principalTable: "BlogCategories",
                        principalColumn: "BlogCategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BlogPostCategories_BlogPosts_BlogPostId",
                        column: x => x.BlogPostId,
                        principalSchema: "context",
                        principalTable: "BlogPosts",
                        principalColumn: "BlogPostId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BlogPostComments",
                schema: "context",
                columns: table => new
                {
                    BlogPostCommentId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Approved = table.Column<bool>(nullable: false),
                    Title = table.Column<string>(maxLength: 256, nullable: false),
                    Author = table.Column<string>(maxLength: 64, nullable: true),
                    IsBlogUser = table.Column<bool>(nullable: false),
                    UserId = table.Column<Guid>(nullable: true),
                    LanguageCode = table.Column<string>(maxLength: 16, nullable: true),
                    Email = table.Column<string>(maxLength: 64, nullable: true),
                    Text = table.Column<string>(nullable: true),
                    IpAddress = table.Column<string>(maxLength: 64, nullable: true),
                    UserAgent = table.Column<string>(maxLength: 150, nullable: true),
                    Tag = table.Column<string>(maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateDeleted = table.Column<DateTime>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    BlogId = table.Column<int>(nullable: true),
                    BlogPostId = table.Column<int>(nullable: false),
                    ReplyToBlogPostCommentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPostComments", x => x.BlogPostCommentId);
                    table.ForeignKey(
                        name: "FK_BlogPostComments_Blogs_BlogId",
                        column: x => x.BlogId,
                        principalSchema: "context",
                        principalTable: "Blogs",
                        principalColumn: "BlogId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BlogPostComments_BlogPosts_BlogPostId",
                        column: x => x.BlogPostId,
                        principalSchema: "context",
                        principalTable: "BlogPosts",
                        principalColumn: "BlogPostId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogPostComments_Languages_LanguageCode",
                        column: x => x.LanguageCode,
                        principalSchema: "context",
                        principalTable: "Languages",
                        principalColumn: "LanguageCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BlogPostComments_BlogPostComments_ReplyToBlogPostCommentId",
                        column: x => x.ReplyToBlogPostCommentId,
                        principalSchema: "context",
                        principalTable: "BlogPostComments",
                        principalColumn: "BlogPostCommentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BlogPostTags",
                schema: "context",
                columns: table => new
                {
                    BlogPostId = table.Column<int>(nullable: false),
                    TagId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPostTags", x => new { x.BlogPostId, x.TagId });
                    table.ForeignKey(
                        name: "FK_BlogPostTags_BlogPosts_BlogPostId",
                        column: x => x.BlogPostId,
                        principalSchema: "context",
                        principalTable: "BlogPosts",
                        principalColumn: "BlogPostId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogPostTags_Tags_TagId",
                        column: x => x.TagId,
                        principalSchema: "context",
                        principalTable: "Tags",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogCategories_BlogId",
                schema: "context",
                table: "BlogCategories",
                column: "BlogId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogCategories_LanguageCode",
                schema: "context",
                table: "BlogCategories",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_BlogCategories_UniqueName",
                schema: "context",
                table: "BlogCategories",
                column: "UniqueName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogMediaObjects_BlogId",
                schema: "context",
                table: "BlogMediaObjects",
                column: "BlogId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogMediaObjects_BlogPostId",
                schema: "context",
                table: "BlogMediaObjects",
                column: "BlogPostId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogMediaObjects_LanguageCode",
                schema: "context",
                table: "BlogMediaObjects",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostCategories_BlogPostId",
                schema: "context",
                table: "BlogPostCategories",
                column: "BlogPostId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostComments_BlogId",
                schema: "context",
                table: "BlogPostComments",
                column: "BlogId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostComments_BlogPostId",
                schema: "context",
                table: "BlogPostComments",
                column: "BlogPostId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostComments_LanguageCode",
                schema: "context",
                table: "BlogPostComments",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostComments_ReplyToBlogPostCommentId",
                schema: "context",
                table: "BlogPostComments",
                column: "ReplyToBlogPostCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_BlogId",
                schema: "context",
                table: "BlogPosts",
                column: "BlogId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_LanguageCode",
                schema: "context",
                table: "BlogPosts",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_UniqueName",
                schema: "context",
                table: "BlogPosts",
                column: "UniqueName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostTags_TagId",
                schema: "context",
                table: "BlogPostTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_LanguageCode",
                schema: "context",
                table: "Blogs",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_UniqueName",
                schema: "context",
                table: "Blogs",
                column: "UniqueName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_LanguageCode",
                schema: "context",
                table: "Tags",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TagName",
                schema: "context",
                table: "Tags",
                column: "TagName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogMediaObjects",
                schema: "context");

            migrationBuilder.DropTable(
                name: "BlogPostCategories",
                schema: "context");

            migrationBuilder.DropTable(
                name: "BlogPostComments",
                schema: "context");

            migrationBuilder.DropTable(
                name: "BlogPostTags",
                schema: "context");

            migrationBuilder.DropTable(
                name: "BlogCategories",
                schema: "context");

            migrationBuilder.DropTable(
                name: "BlogPosts",
                schema: "context");

            migrationBuilder.DropTable(
                name: "Tags",
                schema: "context");

            migrationBuilder.DropTable(
                name: "Blogs",
                schema: "context");

            migrationBuilder.DropTable(
                name: "Languages",
                schema: "context");
        }
    }
}
