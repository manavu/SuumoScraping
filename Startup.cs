namespace SuumoScraping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.HttpsPolicy;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using SuumoScraping.Domain.Gateways;
    using SuumoScraping.Infrastructure.Scraping;
    using SuumoScraping.Models;
    using SuumoScraping.UseCases;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString =
                "server=db;database=scrapingdb;port=3306;uid=docker;password=docker;characterset=utf8;";
            services.AddScoped<IScrapingContextFactory, ScrapingContextFactory>();
            services.AddScoped<ISuumoHtmlFetcher, SuumoHtmlFetcher>();
            services.AddScoped<ISuumoHtmlParser, SuumoHtmlParser>();
            services.AddScoped<ISuumoGateway, SuumoGateway>();
            services.AddScoped<SuumoScraper>();

            // ユースケースの登録
            services.AddScoped<GetFilteredBukkensUseCase>();
            services.AddScoped<GetBukkenDetailsUseCase>();
            services.AddScoped<GetFileDataUseCase>();
            services.AddScoped<GetFloorPlansUseCase>();
            services.AddScoped<SyncBukkensUseCase>();

            // Use MySQL provider from Oracle
            services.AddDbContext<ScrapingContext>(dbContextOptions =>
                dbContextOptions
                    .UseMySQL(connectionString)
                    .LogTo(Console.WriteLine, LogLevel.Information)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
            );

            // add memory cache
            services.AddDistributedMemoryCache();

            // add session
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(15);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = "SessionCookie";
            });

            services.AddControllersWithViews().AddSessionStateTempDataProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );
            });
        }
    }
}
