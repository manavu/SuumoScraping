namespace SuumoScraping.Infrastructure.Persistence
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using SuumoScraping.Domain.Gateways;

    public class ScrapingContextFactory : IScrapingContextFactory
    {
        private readonly IConfiguration _configuration;

        public ScrapingContextFactory() { }

        public ScrapingContextFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IScrapingContext Create()
        {
            var connectionString =
                "server=db;database=scrapingdb;port=3306;uid=docker;password=docker;characterset=utf8;";

            var optionsBuilder = new DbContextOptionsBuilder<ScrapingContext>();

            optionsBuilder.UseMySQL(connectionString).LogTo(Console.WriteLine, LogLevel.Warning);

            return new ScrapingContext(optionsBuilder.Options);
        }
    }
}
