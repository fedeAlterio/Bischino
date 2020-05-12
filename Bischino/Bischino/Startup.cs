using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Bischino.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Bischino.Base.Security;
using Bischino.Base.Service;
using Bischino.Model;
using Bischino.Settings;
using Bischino.Skribble;

namespace Bischino
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }
        private CollectionServiceFactory DBServiceFactory { get; set; }

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(option => option.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Latest);

            var dbSettings = Configuration.GetSection("DBConfig").Get<DatabaseSettings>();
            var jwtSettings = Configuration.GetSection("Jwt").Get<JwtSettings>();

            AddAuthService(services, jwtSettings);
            DBServiceFactory = new CollectionServiceFactory(dbSettings);
            DBServiceFactory.AddDBServices(services);


            var gameHandler = new GameHandler();
            services.AddSingleton<IGameHandler>(gameHandler);

            var skribbleHandler = new SkribbleHandler();
            services.AddSingleton<ISkribbleHandler>(skribbleHandler);
        }

        private void AddAuthService(IServiceCollection services, JwtSettings jwtSettings)
        {
            Jwt.Initialize(jwtSettings);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = Jwt.SymmetricSecurityKey
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDefaultFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { Controller = "Home", action = "Index", id = 0 });
            });
        }
    }
}
