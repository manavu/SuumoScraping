namespace SuumoScraping.Models
{
    using System;
    // using System.Data.Entity.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public interface IScrapingContextFactory // : IDbContextFactory<ScrapingContext>
    {
        public ScrapingContext Create();
    }

    public class ScrapingContextFactory : IScrapingContextFactory
    {
        private readonly IConfiguration _configuration;

        public ScrapingContextFactory()
        {
        }

        public ScrapingContextFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ScrapingContext Create()
        {
            var connectionString = "server=db;database=scrapingdb;port=3306;uid=docker;password=docker;characterset=utf8;";
            //var connectionString = "server=db;database=ScrapingDb2;port=3306;uid=root;password=root;characterset=utf8;";

            /*
            if (_configuration != null)
            {
                connectionString = this._configuration["ConnectionStrings:ScrapingDb"];
            }*/

            var optionsBuilder = new DbContextOptionsBuilder<ScrapingContext>();

            var serverVersion = new MySqlServerVersion(new Version(5, 7, 11));
            optionsBuilder.UseMySql(connectionString, serverVersion)
                // The following three options help with debugging, but should
                // be changed or removed for production.
                .LogTo(Console.WriteLine, LogLevel.Warning);
            // .EnableSensitiveDataLogging()
            // .EnableDetailedErrors();

            return new ScrapingContext(optionsBuilder.Options);
        }
    }
}
