using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Tunr.Models;
using Tunr.Services;
using Tunr.Utilities;

namespace Tunr
{
    public class Startup
    {
        const string TokenAudience = "TunrClient";
        const string TokenIssuer = "Tunr";
        private RsaSecurityKey key;
        private TokenAuthOptions tokenOptions;
        public IConfigurationRoot Configuration { get; }
        
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.default.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Replace this with some sort of loading from config / file.
            RSAParameters keyParams = RSAKeyUtilities.GetRandomKey();

            // Create the key, and a set of token options to record signing credentials 
            // using that key, along with the other parameters we will need in the 
            // token controlller.
            key = new RsaSecurityKey(keyParams);
            tokenOptions = new TokenAuthOptions()
            {
                Audience = TokenAudience,
                Issuer = TokenIssuer,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature)
            };

            // Save the token options into an instance so they're accessible to the controller.
            services.AddSingleton<TokenAuthOptions>(tokenOptions);

            // Enable the use of an [Authorize("Bearer")] attribute on methods and
            // classes to protect.
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });

            // Add framework services.
            services.AddEntityFrameworkSqlServer()
                .AddDbContext<ApplicationDbContext>(options => 
                    options.UseSqlServer(Configuration.GetConnectionString("SqlServerConnectionString")));
            services.AddIdentity<TunrUser, TunrRole>(options => 
                {
                    options.Cookies.ApplicationCookie.AutomaticChallenge = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 5; // TODO: Store in config somewhere.
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext, Guid>()
                .AddDefaultTokenProviders();
            services.AddSqlPageBlobLibraryStore(options => {
                options.StorageAccountConnectionString
                    = Configuration.GetConnectionString("AzureStorageConnectionString");
            });
            services.AddAzureBlobMusicFileStore(options => {
                options.StorageAccountConnectionString
                    = Configuration.GetConnectionString("AzureStorageConnectionString");
            });
            services.AddTagLibTagService();
            services.AddFFMpegAudioInfoService();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Apply migrations
            app.ApplicationServices.GetRequiredService<ApplicationDbContext>().Database.Migrate();

            // Add auth
            app.UseJwtBearerAuthentication(new JwtBearerOptions {
                TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = key,
                    ValidAudience = tokenOptions.Audience,
                    ValidIssuer = tokenOptions.Issuer,
                    ValidateLifetime = true,
                    RequireExpirationTime = false,
                    ClockSkew = TimeSpan.FromMinutes(0)
                }
            });

            // Add MVC / file serving
            app.UseStaticFiles();
            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=App}/{action=Index}/{id?}"
                );
                routes.MapSpaFallbackRoute("spa-fallback", new { controller = "App", action = "Index" });
            });
        }
    }
}
