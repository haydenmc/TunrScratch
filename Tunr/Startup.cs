using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tunr.Models;
using Tunr.Services;
using Tunr.Utilities;

namespace Tunr
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services
                .AddEntityFrameworkSqlServer()
                .AddDbContextPool<ApplicationDbContext>(options => 
                    options.UseSqlServer(Configuration.GetConnectionString("SqlServerConnectionString")));

            // Identity and authentication
            services
                .AddIdentity<TunrUser, TunrRole>(options => 
                {
                    // TODO: Store in config somewhere.
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 5;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.Expiration = TimeSpan.FromDays(7);
                options.LoginPath = "/User/Login";
                options.LogoutPath = "/User/Logout";
                options.AccessDeniedPath = "/User/AccessDenied";
                options.SlidingExpiration = true;
            });

            // Add Tunr services
            services.AddEntityFrameworkMusicMetadataStore();
            services
                .AddAzureBlobMusicFileStore(options =>
                {
                    options.StorageAccountConnectionString
                        = Configuration.GetConnectionString("AzureStorageConnectionString");
                });
            services.AddTagLibTagReaderService();
            services.AddFFMpegAudioInfoReaderService();
            services.AddMvc();

            // Increase size limits
            services.Configure<FormOptions>(options => {
                options.ValueLengthLimit = 1024 * 1024 * 128;
                options.MultipartBodyLengthLimit = 1024 * 1024 * 128; // In case of multipart
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Add auth
            app.UseAuthentication();

            // Add MVC / file serving
            app.UseStaticFiles();
            app
                .UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=App}/{action=Index}/{id?}"
                    );
                    routes.MapSpaFallbackRoute("spa-fallback", new { controller = "App", action = "Index" });
                });
        }
    }
}
