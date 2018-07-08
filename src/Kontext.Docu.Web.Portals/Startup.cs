using AspNetCore.XmlRpc;
using AspNetCore.XmlRpc.MetaWeblog;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Kontext.Configuration;
using Kontext.Data;
using Kontext.Data.MasterDataModels.Extensions;
using Kontext.Data.Models;
using Kontext.Data.Models.ViewModels;
using Kontext.Logging;
using Kontext.Security;
using Kontext.Security.AuthorizationPolicies;
using Kontext.Security.Blog;
using Kontext.Services;
using Kontext.Services.Captcha;
using Kontext.Docu.Web.Portals.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.Text;

namespace Kontext.Docu.Web.Portals
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Configuration and options 
            services.AddOptions();
            services.Configure<SiteConfig>(Configuration.GetSection("ContextConfig"));
            services.Configure<BlogConfig>(Configuration.GetSection("ContextConfig:BlogConfig"));
            services.Configure<EmailConfig>(Configuration.GetSection("ContextConfig:EmailConfig"));
            services.Configure<EmailTemplatesConfig>(Configuration.GetSection("ContextConfig:Templates"));
            services.Configure<SecurityConfig>(Configuration.GetSection("ContextConfig:SecurityConfig"));
            services.Configure<DatabaseConfig>(Configuration.GetSection("ContextConfig:DatabaseConfig"));
            // Configure XmlRpc
            services.Configure<XmlRpcOptions>(Configuration.GetSection("XmlRpc"));
            services.AddSingleton(Configuration);

            // Add meta weblog
            services.AddMetaWeblog<MetaWeblogXmlRpcService, DefaultMetaWeblogEndpointProvider>();

            // Localization
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var enCulture = "en-AU";
                var supportedCultures = new[]
                {
                    new CultureInfo("zh"),
                    new CultureInfo(enCulture),
                    new CultureInfo("en-us"),
                    new CultureInfo("en-gb"),
                };

                options.DefaultRequestCulture = new RequestCulture(culture: enCulture, uiCulture: enCulture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                var cookieProvider = new CookieRequestCultureProvider { CookieName = Constants.CookieNameForRequestCulture };
                options.RequestCultureProviders.Insert(0, cookieProvider);
                options.RequestCultureProviders.Insert(1, new CustomRequestCultureProvider(async context =>
                {
                    // custom request culture logic
                    var requestLanguages = context.Request.Headers["Accept-Language"];
                    foreach (var lang in requestLanguages)
                    {
                        if (lang.Contains("zh"))
                        {
                            return new ProviderCultureResult("zh-CN", "zh");
                        }
                        else if (lang.Contains("en-US"))
                        {
                            return new ProviderCultureResult("en-US", "en");
                        }
                        else if (lang.Contains("en-GB"))
                        {
                            return new ProviderCultureResult("en-GB", "en");
                        }
                        else if (lang.Contains("en"))
                        {
                            return new ProviderCultureResult("en-AU", "en");
                        }
                    }
                    return new ProviderCultureResult(options.DefaultRequestCulture.UICulture.Name);
                }));

            });

            var sp = services.BuildServiceProvider();
            var dbConfig = sp.GetService<IOptions<DatabaseConfig>>().Value;
            // Replace connection string tokens
            var connStrCore = Configuration.GetConnectionString(Constants.KontextCoreConnectionName);
            var connStrDocu = Configuration.GetConnectionString(Constants.KontextDocuConnectionName);
            if (connStrCore.Contains(Constants.ContentRootPathToken))
                connStrCore = connStrCore.Replace(Constants.ContentRootPathToken, Env.ContentRootPath);
            if (connStrDocu.Contains(Constants.ContentRootPathToken))
                connStrDocu = connStrDocu.Replace(Constants.ContentRootPathToken, Env.ContentRootPath);

            // Add database context
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                if (dbConfig.CoreDbType == DatabaseType.SQLite)
                {
                    options.UseSqlite(connStrCore,
                        b => b.MigrationsAssembly("Kontext.Docu.Web.Portals"));
                }
                else
                {
                    options.UseSqlServer(connStrDocu,
                        b => b.MigrationsAssembly("Kontext.Docu.Web.Portals"));
                }
                // Register context data models.
                options.UseContextProjectShared();
            });

            // Context blog database context
            services.AddDbContext<ContextBlogDbContext>(options =>
            {
                if (dbConfig.DocuDbType == DatabaseType.SQLite)
                {
                    options.UseSqlite(connStrDocu,
                        b => b.MigrationsAssembly("Kontext.Docu.Web.Portals"));
                }
                else
                {
                    options.UseSqlServer(connStrDocu,
                        b => b.MigrationsAssembly("Kontext.Docu.Web.Portals"));
                }
                // Register context blog data models.
                options.UseContextBlogModels();
            });


            // Configure Idenity
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Configure Idenity options and password complexity
            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;

                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                options.SignIn.RequireConfirmedEmail = true;

                // User email settings to ensure unique email.
                options.User.RequireUniqueEmail = true;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 5;

            });

            // Application cookie
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = Constants.CookieNameForApplication;
            });

            // Add application services.
            services.AddTransient<IEmailSenderService, EmailSenderService>();
            services.AddSingleton<IConfigService, ConfigurationService>();
            services.AddScoped<IContextUnitOfWork, ContextUnitOfWork>();
            services.AddScoped<IContextBlogUnitOfWork, ContextBlogUnitOfWork>();
            services.AddScoped<IAccountManager, AccountManager>();
            services.AddSingleton(Env.ContentRootFileProvider);
            services.AddSingleton<IApplicationPermissionProvider, BlogApplicationPermissionProvider>();
            services.AddSingleton<ICacheManager, MemoryCacheManager>();

            // Database initializer 
            services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();

            // Initialize object mappers
            Mapper.Initialize(cfg =>
            {
                cfg.AddCollectionMappers();
                cfg.AddProfile<ContextBlogAutoMapperConfiguration>();

                cfg.CreateMap<ApplicationUser, UserViewModel>()
                .ForMember(d => d.Roles, map => map.Ignore())
                .ReverseMap();

                cfg.CreateMap<ApplicationRole, RoleViewModel>()
                .ForMember(d => d.Permissions, map => map.MapFrom(s => s.Claims))
                .ForMember(d => d.UsersCount, map => map.ResolveUsing(s => s.UserRoles?.Count ?? 0)).ReverseMap();
                cfg.CreateMap<RoleViewModel, ApplicationRole>()
                    .ForMember(d => d.Claims, map => map.Ignore())
                    .ForMember(d => d.UserRoles, map => map.Ignore());
                cfg.CreateMap<ApplicationRoleClaim, ClaimViewModel>()
                    .ForMember(d => d.Type, map => map.MapFrom(s => s.ClaimType))
                    .ForMember(d => d.Value, map => map.MapFrom(s => s.ClaimValue))
                    .ReverseMap();
                cfg.CreateMap<ApplicationPermission, PermissionViewModel>()
                    .ReverseMap();
                var permissionProvider = services.BuildServiceProvider().GetService<IApplicationPermissionProvider>();
                cfg.CreateMap<ApplicationRoleClaim, PermissionViewModel>()
                    .ConvertUsing(s => Mapper.Map<PermissionViewModel>(permissionProvider.GetPermissionByValue(s.ClaimValue)));
            });

            // Add antiforgery
            services.AddAntiforgery(options =>
            {
                //options.CookieName = Constants.CookieNameForAntiforgery;
                options.Cookie.Name = Constants.CookieNameForAntiforgery;
                options.HeaderName = "Kontext.AF.RequestVerificationToken";
            });

            // Add http context accessor 
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Localization
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });

            // Add authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = Constants.CookieNamePrefixForCookieAuth;
                });

            // Add authorizations
            services.AddAuthorization(options =>
            {
                /*Generic admin*/
                options.AddPolicy(ApplicationAuthorizationPolicies.GenericAdminPolicy, policy => policy.RequireClaim(ApplicationClaimTypes.Permission, DefaultApplicationPermissionProvider.GenericAdmin));

                options.AddPolicy(ApplicationAuthorizationPolicies.ManageEmailsPolicy, policy => policy.RequireClaim(ApplicationClaimTypes.Permission, DefaultApplicationPermissionProvider.ManageEmails));

                /*Idenity policies*/
                options.AddPolicy(ApplicationAuthorizationPolicies.ViewUserByUserIdPolicy, policy => policy.Requirements.Add(new ViewUserByIdRequirement()));
                options.AddPolicy(ApplicationAuthorizationPolicies.ViewUsersPolicy, policy => policy.RequireClaim(ApplicationClaimTypes.Permission, DefaultApplicationPermissionProvider.ViewUsers));
                options.AddPolicy(ApplicationAuthorizationPolicies.ManageUserByUserIdPolicy, policy => policy.Requirements.Add(new ManageUserByIdRequirement()));
                options.AddPolicy(ApplicationAuthorizationPolicies.ManageUsersPolicy, policy => policy.RequireClaim(ApplicationClaimTypes.Permission, DefaultApplicationPermissionProvider.ManageUsers));
                options.AddPolicy(ApplicationAuthorizationPolicies.ViewRoleByRoleNamePolicy, policy => policy.Requirements.Add(new ViewRoleByNameRequirement()));
                options.AddPolicy(ApplicationAuthorizationPolicies.ViewRolesPolicy, policy => policy.RequireClaim(ApplicationClaimTypes.Permission, DefaultApplicationPermissionProvider.ViewRoles));
                options.AddPolicy(ApplicationAuthorizationPolicies.AssignRolesPolicy, policy => policy.Requirements.Add(new AssignRolesRequirement()));
                options.AddPolicy(ApplicationAuthorizationPolicies.ManageRolesPolicy, policy => policy.RequireClaim(ApplicationClaimTypes.Permission, DefaultApplicationPermissionProvider.ManageRoles));

                /*Blog policies*/
                options.AddPolicy(BlogApplicationAuthorizationPolicies.AddBlogPolicy, policy => policy.RequireClaim(ApplicationClaimTypes.Permission, BlogApplicationPermissionProvider.ManageBlogs));
                options.AddPolicy(BlogApplicationAuthorizationPolicies.EditBlogPolicy, policy => policy.RequireClaim(ApplicationClaimTypes.Permission, BlogApplicationPermissionProvider.ManageBlogs));
                options.AddPolicy(BlogApplicationAuthorizationPolicies.ViewBlogPolicy, policy => policy.RequireClaim(ApplicationClaimTypes.Permission, BlogApplicationPermissionProvider.ViewBlogs));

                options.AddPolicy(BlogApplicationAuthorizationPolicies.AddBlogCategoryPolicy, policy => policy.RequireClaim(ApplicationClaimTypes.Permission, BlogApplicationPermissionProvider.ManageBlogCategories));
                options.AddPolicy(BlogApplicationAuthorizationPolicies.EditBlogCategoryPolicy, policy => policy.RequireClaim(ApplicationClaimTypes.Permission, BlogApplicationPermissionProvider.ManageBlogCategories));
                options.AddPolicy(BlogApplicationAuthorizationPolicies.ViewBlogCategoryPolicy, policy => policy.RequireClaim(ApplicationClaimTypes.Permission, BlogApplicationPermissionProvider.ViewBlogCategories));

                options.AddPolicy(BlogApplicationAuthorizationPolicies.AddBlogPostPolicy, policy => policy.RequireClaim(ApplicationClaimTypes.Permission, BlogApplicationPermissionProvider.ManageBlogPosts));
                options.AddPolicy(BlogApplicationAuthorizationPolicies.EditBlogPostPolicy, policy => policy.RequireClaim(ApplicationClaimTypes.Permission, BlogApplicationPermissionProvider.ManageBlogPosts));
                options.AddPolicy(BlogApplicationAuthorizationPolicies.ViewBlogPostPolicy, policy => policy.RequireClaim(ApplicationClaimTypes.Permission, BlogApplicationPermissionProvider.ViewBlogPosts));

                options.AddPolicy(BlogApplicationAuthorizationPolicies.AddBlogCommentPolicy, policy => policy.RequireClaim(ApplicationClaimTypes.Permission, BlogApplicationPermissionProvider.ManageBlogComments));
                options.AddPolicy(BlogApplicationAuthorizationPolicies.EditBlogCommentPolicy, policy => policy.RequireClaim(ApplicationClaimTypes.Permission, BlogApplicationPermissionProvider.ManageBlogComments));
                options.AddPolicy(BlogApplicationAuthorizationPolicies.ViewBlogCommentPolicy, policy => policy.RequireClaim(ApplicationClaimTypes.Permission, BlogApplicationPermissionProvider.ViewBlogComments));

                options.AddPolicy(BlogApplicationAuthorizationPolicies.ManageTagsPolicy, policy => policy.RequireClaim(ApplicationClaimTypes.Permission, BlogApplicationPermissionProvider.ManageTags));
            });

            // Add MVC
            services.AddMvc()
            // support annotation localization to use shared resource
            .AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                    factory.Create(typeof(ContextProjectSharedResource));
            }).AddViewLocalization();

            // Adds session
            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.Name = Constants.CookieNameForSession;
                options.IdleTimeout = TimeSpan.FromSeconds(300);
                options.Cookie.HttpOnly = true;
            });

            // Register code
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IDatabaseInitializer dbInitializer, IOptions<RequestLocalizationOptions> localizationOptions)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();

                try
                {
                    // Migrate database and populate seed records
                    dbInitializer.SeedAsync().Wait();
                }
                catch (Exception ex)
                {
                    loggerFactory.CreateLogger<Startup>().LogCritical(LoggingEvents.INIT_DATABASE_ERROR, ex, LoggingEvents.INIT_DATABASE_ERROR.Name);
                    //throw;
                }
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            // Localization
            app.UseRequestLocalization(localizationOptions.Value);

            // Session
            app.UseSession();

            // Captcha
            app.UseCaptcha();

            // Use XmlRpc middleware
            app.UseMetaWeblog();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areas_identityadmin",
                    template: "admin/identity/{controller=Home}/{action=Index}/{id?}", defaults: new { area = "IdentityAdminArea" }, constraints: new { area = "IdentityAdminArea" });
                routes.MapRoute(
                    name: "areas_blogadmin",
                    template: "admin/blogapp/{controller=Home}/{action=Index}/{id?}", defaults: new { area = "BlogAdminArea" }, constraints: new { area = "BlogAdminArea" });
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            });
        }
    }
}
