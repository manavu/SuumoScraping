namespace SuumoScraping.Models
{
    using System;
    using System.Data.Entity.Infrastructure;
    using Microsoft.Extensions.Configuration;

    public interface IScrapingContextFactory : IDbContextFactory<ScrapingContext>
    {
    }

    public class ScrapingContextFactory : IScrapingContextFactory
    {
        private readonly IConfiguration _configuration;

        public ScrapingContextFactory()
        {
            // update-database がこのコンストラクタを呼び出す
            // update-database -verbose -ConnectionProviderName "MySql.Data.MySqlClient" -ConnectionString "server=localhost;database=ScrapingDb;port=3306;characterset=utf8;uid=****;pwd=****;"
            // この場合はこのクラスは呼ばれない
        }

        public ScrapingContextFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ScrapingContext Create()
        {
            var conStr = "server=localhost;database=ScrapingDb;port=3306;uid=root;password=****;characterset=utf8;";

            if (_configuration != null)
            {
                conStr = this._configuration["ConnectionStrings:ScrapingDb"];
            }

            return new ScrapingContext(conStr);
        }
    }
}
