using AspNetCore.XmlRpc;
using AspNetCore.XmlRpc.MetaWeblog;
using AspNetCore.XmlRpc.MetaWeblog.Models;
using Kontext.Configuration;
using Kontext.Data;
using Kontext.Data.Models;
using Kontext.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kontext.Docu.Web.Portals.Services
{
    /// <summary>
    /// MetaWeblog XML-RPC implementation.
    /// </summary>
    public class MetaWeblogXmlRpcService : IMetaWeblogXmlRpcService
    {
        private readonly IConfigService configService;
        private readonly IContextBlogUnitOfWork unitOfWork;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<MetaWeblogXmlRpcService> logger;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IOptions<BlogConfig> blogOptions;
        private readonly IOptions<SiteConfig> siteOptions;
        private readonly IHostingEnvironment environment;

        public MetaWeblogXmlRpcService(IConfigService configService,
            IContextBlogUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<MetaWeblogXmlRpcService> logger,
            IHttpContextAccessor contextAccessor,
            IOptions<BlogConfig> blogOptions,
            IOptions<SiteConfig> siteOptions,
            IHostingEnvironment environment)
        {
            this.configService = configService;
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.contextAccessor = contextAccessor;
            this.blogOptions = blogOptions;
            this.siteOptions = siteOptions;
            this.environment = environment;
        }

        /// <summary>
        /// Validate user.
        /// </summary>
        /// <param name="blog"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private async Task<ApplicationUser> ValidateUserAsync(Blog blog, string username, string password)
        {
            var user = await ValidateUserAsync(username, password);
            if (!blog.UserId.HasValue)
            {
                throw new XmlRpcFaultException(0, "This blog is not configured under this user.");
            }

            if (!Guid.Parse(user.Id).Equals(blog.UserId.Value))
            {
                throw new XmlRpcFaultException(0, "This blog is not configured under this user.");
            }
            return user;
        }

        private async Task<ApplicationUser> ValidateUserAsync(string username, string password)
        {
            var user = await userManager.FindByNameAsync(username);
            var checkResult = await userManager.CheckPasswordAsync(user, password);
            if (!checkResult)
            {
                throw new XmlRpcFaultException(0, "Username and password doesn't match.");
            }
            return user;
        }

        private string ExtractAndEnsureUserLanguage(HttpRequest request)
        {
            var lang = blogOptions.Value.DefaultLanguageLocale;
            if (request.Headers["Accept-Language"].Count > 0)
            {
                lang = request.Headers["Accept-Language"][0];
            }

            var langEntry = unitOfWork.LanguageRepository.FindOneAsync(l => l.LanguageCode == lang).Result;
            if (langEntry == null)
            {
                unitOfWork.LanguageRepository.Add(new Language
                {
                    LanguageCode = lang,
                    DisplayName = $"Placeholder: {lang}",
                    Tag = lang.ToLower()
                });
                unitOfWork.SaveChanges();
            }
            return lang;
        }

        /// <summary>
        /// Add new post
        /// </summary>
        /// <param name="blogid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="post"></param>
        /// <param name="publish"></param>
        /// <returns></returns>
        [XmlRpcMethod("metaWeblog.newPost")]
        public string NewPost(string blogid, string username, string password, PostInfo post, bool publish)
        {
            var blog = unitOfWork.BlogRepository.QueryOneAsync(b => b.UniqueName == blogid, nameof(Blog.Categories)).Result;
            var user = ValidateUserAsync(blog, username, password).Result;
            var request = contextAccessor.HttpContext.Request;

            //Try to parse datetime
            DateTime minDT = DateTime.MinValue;
            if (post.DateCreated == minDT)
            {
                post.DateCreated = DateTime.Now;
            }

            var blogPost = new BlogPost
            {
                BlogId = blog.BlogId,
                Author = user.FullName ?? user.UserName,
                CommentCount = 0,
                DateCreated = post.DateCreated,
                DateModified = post.DateCreated,
                Description = post.Excerpt,
                Title = post.Title,
                Email = user.Email,
                IpAddress = contextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString(),
                ViewCount = 0,
                IsDeleted = false,
                Text = post.Description,
                LanguageCode = ExtractAndEnsureUserLanguage(request),
                Tag = !string.IsNullOrWhiteSpace(post.KeyWords) && post.KeyWords.Contains(Constants.LiteLogTag) ? Constants.LiteLogType : null
            };

            if (publish)
                blogPost.DatePublished = DateTime.Now;

            //Slug or unique name
            if (!string.IsNullOrEmpty(post.Slug))
            {
                blogPost.UniqueName = post.Slug;
            }
            else
            {
                blogPost.UniqueName = ToUrlSlug(post.Title);
                if (string.IsNullOrEmpty(blogPost.UniqueName))
                    blogPost.UniqueName = Guid.NewGuid().ToString("N");
            }

            try
            {
                // Add blog entry first
                unitOfWork.BlogPostRepository.Add(blogPost);
                unitOfWork.SaveChanges();

                //Add all the categories
                var cates = ExtractAndEnsureCategories(post.Categories, blog);

                foreach (var cate in cates)
                {
                    if (cate != null)
                    {
                        unitOfWork.BlogPostCategoryRespository.Add(new BlogPostCategory
                        {
                            BlogCategoryId = cate.BlogCategoryId,
                            BlogPostId = blogPost.BlogPostId
                        });
                    }
                }
                unitOfWork.SaveChanges();

                // KeyWords (Tags)
                if (!string.IsNullOrWhiteSpace(post.KeyWords))
                {
                    var tags = ExtractAndEnsureTags(post.KeyWords);

                    foreach (var tag in tags)
                    {
                        unitOfWork.BlogPostTagRepository.Add(new BlogPostTag
                        {
                            TagId = tag.TagId,
                            BlogPostId = blogPost.BlogPostId
                        });
                    }
                    unitOfWork.SaveChanges();
                }

                return blogPost.UniqueName.ToString();
            }
            catch (Exception ex)
            {
                throw new XmlRpcFaultException(0, ex.Message + " " + ex.StackTrace);
            }
        }

        private List<BlogCategory> ExtractAndEnsureCategories(string[] cateTitles, Blog blog)
        {
            var cates = new List<BlogCategory>();
            foreach (var title in cateTitles)
            {
                var cateEntry = unitOfWork.BlogCategoryRepository.FindOneAsync(e => e.Title == title && e.BlogId == blog.BlogId).Result;
                if (cateEntry == null)
                {
                    unitOfWork.BlogCategoryRepository.Add(new BlogCategory
                    {
                        LanguageCode = blogOptions.Value.DefaultLanguageLocale,
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        BlogId = blog.BlogId,
                        Active = true,
                        Description = $"Blog posts about {title}",
                        Title = title,
                        UniqueName = ToUrlSlug(title)
                    });
                    unitOfWork.SaveChanges();

                    cateEntry = unitOfWork.BlogCategoryRepository.FindOneAsync(e => e.Title == title && e.BlogId == blog.BlogId).Result;
                }
                cates.Add(cateEntry);
            }
            return cates;
        }

        private List<Tag> ExtractAndEnsureTags(string keyWords)
        {
            var tags = new List<Tag>();
            var tagNames = keyWords.Split(blogOptions.Value.KeyWordsSeperator, StringSplitOptions.RemoveEmptyEntries);
            foreach (var name in tagNames)
            {
                var tagEntry = unitOfWork.TagRepository.FindOneAsync(e => e.TagName == name).Result;
                if (tagEntry == null)
                {
                    unitOfWork.TagRepository.Add(new Tag
                    {
                        TagName = name,
                        IsDeleted = false,
                        LanguageCode = blogOptions.Value.DefaultLanguageLocale,
                        NTile = 0,
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now
                    });
                    unitOfWork.SaveChanges();

                    tagEntry = unitOfWork.TagRepository.FindOneAsync(e => e.TagName == name).Result;
                }
                tags.Add(tagEntry);
            }
            return tags;
        }

        private static string ToUrlSlug(string value)
        {
            //First to lower case
            value = value.ToLowerInvariant();
            //Remove all accents
            var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(value);
            value = Encoding.ASCII.GetString(bytes);
            //Replace spaces
            value = Regex.Replace(value, @"\s", "-", RegexOptions.Compiled);
            //Remove invalid chars
            value = Regex.Replace(value, @"[^\w\s\p{Pd}]", "", RegexOptions.Compiled);
            //Trim dashes from end
            value = value.Trim('-', '_');
            //Replace double occurences of - or \_
            value = Regex.Replace(value, @"([-_]){2,}", "$1", RegexOptions.Compiled);
            return value;
        }


        /// <summary>
        /// Edit post
        /// </summary>
        /// <param name="postid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="post"></param>
        /// <param name="publish"></param>
        /// <returns></returns>
        [XmlRpcMethod("metaWeblog.editPost")]
        public string EditPost(string postid, string username, string password, PostInfo post, bool publish)
        {
            try
            {
                var blogEntry = unitOfWork.BlogPostRepository.QueryOneAsync(e => e.UniqueName == postid, nameof(BlogPost.Blog), nameof(BlogPost.BlogCategories), nameof(BlogPost.Tags)).Result;

                var user = ValidateUserAsync(blogEntry.Blog, username, password).Result;

                // Updates
                blogEntry.DateModified = DateTime.Now;
                blogEntry.Description = post.Excerpt;
                blogEntry.Title = post.Title;
                blogEntry.Text = post.Description;
                blogEntry.Tag = !string.IsNullOrWhiteSpace(post.KeyWords) && post.KeyWords.Contains(Constants.LiteLogTag) ? Constants.LiteLogType : null;

                if (publish)
                {
                    blogEntry.DatePublished = DateTime.Now;
                }

                // Update categories
                blogEntry.BlogCategories.Clear();
                unitOfWork.SaveChanges();

                var cates = ExtractAndEnsureCategories(post.Categories, blogEntry.Blog);
                foreach (var cate in cates)
                {
                    if (cate != null)
                    {
                        unitOfWork.BlogPostCategoryRespository.Add(new BlogPostCategory
                        {
                            BlogCategoryId = cate.BlogCategoryId,
                            BlogPostId = blogEntry.BlogPostId
                        });
                    }
                }

                // Update tags
                if (!string.IsNullOrWhiteSpace(post.KeyWords))
                {
                    blogEntry.Tags.Clear();
                    unitOfWork.SaveChanges();
                    var tags = ExtractAndEnsureTags(post.KeyWords);

                    foreach (var tag in tags)
                    {
                        unitOfWork.BlogPostTagRepository.Add(new BlogPostTag
                        {
                            TagId = tag.TagId,
                            BlogPostId = blogEntry.BlogPostId
                        });
                    }
                }

                unitOfWork.SaveChanges();

                return blogEntry.UniqueName;
            }
            catch (Exception ex)
            {
                throw new XmlRpcFaultException(0, ex.Message + " " + ex.StackTrace);
            }
        }

        /// <summary>
        /// Get blog post information
        /// </summary>
        /// <param name="postid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [XmlRpcMethod("metaWeblog.getPost")]
        public PostInfo GetPost(string postid, string username, string password)
        {
            try
            {
                var post = (from p in unitOfWork.BlogPostRepository.Entities.Include(e => e.Blog)
                            .Include(e => e.Tags).ThenInclude(e => e.Tag)
                            .Include(e => e.BlogCategories).ThenInclude(e => e.BlogCategory)
                            where p.UniqueName == postid
                            select p).FirstOrDefault();

                var user = ValidateUserAsync(post.Blog, username, password);

                var postInfo = new PostInfo
                {
                    DateCreated = post.DateCreated,
                    Description = post.Text,
                    Title = post.Title,
                    Excerpt = post.Description,
                    Slug = post.UniqueName,
                    Categories = post.BlogCategories.Select(e => e.BlogCategory.Title).ToArray(),
                    KeyWords = post.Tags != null ? String.Join(";", post.Tags.Select(p => p.Tag.TagName).ToArray()) : null,
                    PostId = post.UniqueName
                };

                return postInfo;
            }
            catch (Exception ex)
            {
                throw new XmlRpcFaultException(0, ex.Message + " " + ex.StackTrace);
            }
        }

        /// <summary>
        /// Get categories
        /// </summary>
        /// <param name="blogid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [XmlRpcMethod("metaWeblog.getCategories")]
        public CategoryInfo[] GetCategories(string blogid, string username, string password)
        {
            try
            {
                var blog = unitOfWork.BlogRepository.QueryOneAsync(e => e.UniqueName == blogid, nameof(Blog.Categories)).Result;
                var user = ValidateUserAsync(blog, username, password).Result;
                return (
                    from c in blog.Categories
                    where c.Active == true
                    select new CategoryInfo { Title = c.Title, Description = c.Description }
                    ).ToArray();
            }
            catch (Exception ex)
            {
                throw new XmlRpcFaultException(0, ex.Message + " " + ex.StackTrace);
            }
        }

        /// <summary>
        /// Get recent posts
        /// </summary>
        /// <param name="blogid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="numberOfPosts"></param>
        /// <returns></returns>
        [XmlRpcMethod("metaWeblog.getRecentPosts")]
        public PostInfo[] GetRecentPosts(string blogid, string username, string password, int numberOfPosts)
        {
            try
            {
                var blog = (from b in unitOfWork.BlogRepository.Entities.Include(e => e.Posts).ThenInclude(e => e.BlogCategories).ThenInclude(e => e.BlogCategory)
                            where b.UniqueName == blogid
                            select b).FirstOrDefault();

                var user = ValidateUserAsync(blog, username, password).Result;

                return
                    (from post in blog.Posts
                     orderby post.DateModified descending
                     select new PostInfo
                     {
                         DateCreated = post.DateCreated,
                         Description = post.Text,
                         Title = post.Title,
                         Excerpt = post.Description,
                         Slug = post.UniqueName,
                         Categories = (
                         from c in post.BlogCategories
                         select c.BlogCategory.Title
                         ).ToArray(),
                         KeyWords = post.Tags != null ? String.Join(";", post.Tags.Select(p => p.Tag.TagName).ToArray()) : null,
                         PostId = post.UniqueName
                     }).Take(numberOfPosts).ToArray();
            }
            catch (Exception ex)
            {
                throw new XmlRpcFaultException(0, ex.Message + " " + ex.StackTrace);
            }
        }

        /// <summary>
        /// New media object
        /// </summary>
        /// <param name="blogid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="mediaObject"></param>
        /// <returns></returns>
        [XmlRpcMethod("metaWeblog.newMediaObject")]
        public MediaObjectInfo NewMediaObject(string blogid, string username, string password, MediaObject mediaObject)
        {
            try
            {
                var blog = unitOfWork.BlogRepository.FindOneAsync(e => e.UniqueName == blogid).Result;
                var user = ValidateUserAsync(blog, username, password);
                var imageFolder = siteOptions.Value.ImageFolderPath;

                if (!Directory.Exists(imageFolder))
                {
                    Directory.CreateDirectory(imageFolder);
                }
                var blogImageFolder = Path.Combine(environment.WebRootPath, imageFolder, blog.UniqueName);

                if (!Directory.Exists(blogImageFolder))
                {
                    Directory.CreateDirectory(blogImageFolder);
                }

                var fileName = mediaObject.Name.Replace('\\', '_').Replace('/', '_');
                var imagePath = Path.Combine(blogImageFolder, fileName);

                WriteBytesToFile(imagePath, mediaObject.Bits);

                var imageVirtualPath = Path.Combine(imageFolder, blog.UniqueName, fileName);
                var request = contextAccessor.HttpContext.Request;
                var uriBuilder = new UriBuilder
                {
                    Host = request.Host.Host,
                    Scheme = request.Scheme,
                    Path = imageVirtualPath
                };

                if (request.Host.Port.HasValue)
                    uriBuilder.Port = request.Host.Port.Value;

                var url = uriBuilder.ToString();

                unitOfWork.BlogMediaObjectRepository.Add(new BlogMediaObject
                {
                    Url = url,
                    BlogId = blog.BlogId,
                    Name = mediaObject.Name,
                    TypeName = mediaObject.TypeName,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    IsDeleted = false,
                    DatePublished = DateTime.Now
                });
                unitOfWork.SaveChanges();

                return new MediaObjectInfo { Url = url };

            }
            catch (Exception ex)
            {
                throw new XmlRpcFaultException(0, ex.Message + " " + ex.StackTrace);
            }

        }

        private static void WriteBytesToFile(string destinationFilePath, byte[] data)
        {
            if (String.IsNullOrEmpty(destinationFilePath))
            {
                throw new ArgumentNullException("destinationFilePath");
            }

            if (!IsValidFilePath(destinationFilePath))
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture,
                                                                  "Invalid file path.",
                                                                  destinationFilePath));
            }

            using (var stream = new FileStream(destinationFilePath, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(data);
                }
            }
        }

        private static bool IsValidFilePath(string filePath)
        {
            char[] invalidChars = Path.GetInvalidPathChars();
            return !invalidChars.Any(c => filePath.Contains(c));
        }

        [XmlRpcMethod("blogger.deletePost")]
        public bool DeletePost(string key, string postid, string username, string password, bool publish)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get users and blogs
        /// </summary>
        /// <param name="key"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [XmlRpcMethod("blogger.getUsersBlogs")]
        public BlogInfo[] GetUsersBlogs(string key, string username, string password)
        {
            try
            {
                var user = ValidateUserAsync(username, password).Result;
                if (user != null)
                {
                    var blogs = unitOfWork.BlogRepository.FindAsync(e => e.UserId == Guid.Parse(user.Id)).Result;
                    return (from b in blogs
                            where b.IsActive == true
                            select
                            new BlogInfo
                            {
                                Blogid = b.BlogId.ToString(),
                                BlogName = b.Title,
                                Url = siteOptions.Value.SiteDomain
                            }).ToArray();
                }
                throw new XmlRpcFaultException(0, $"Username {username} doesn't exist.");
            }
            catch (Exception ex)
            {
                throw new XmlRpcFaultException(0, ex.Message + " " + ex.StackTrace);
            }
        }

        /// <summary>
        /// Find user info
        /// </summary>
        /// <param name="key"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [XmlRpcMethod("blogger.getUserInfo")]
        public UserInfo GetUserInfo(string key, string username, string password)
        {
            try
            {
                var user = ValidateUserAsync(username, password).Result;
                if (user != null)
                {
                    return new UserInfo
                    {
                        Email = user.Email,
                        FirstName = user.UserName,
                        LastName = user.FullName ?? "",
                        NickName = user.FullName ?? user.UserName,
                        Url = siteOptions.Value.SiteDomain,
                        UserId = user.Id
                    };
                }
                throw new XmlRpcFaultException(0, $"Username {username} doesn't exist.");
            }
            catch (Exception ex)
            {
                throw new XmlRpcFaultException(0, ex.Message + " " + ex.StackTrace);
            }
        }
    }
}
