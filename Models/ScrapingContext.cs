namespace SuumoScraping.Models
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Console;

    public partial class ScrapingContext : DbContext
    {
        public ScrapingContext(DbContextOptions<ScrapingContext> options)
            : base(options)
        {
            // 必須のコンストラクタ。
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            /*
            // 以下、LoggerFactory の構築方法が v.2.x 以前と異なる。
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddDebug()
                    .AddFilter(category: DbLoggerCategory.Database.Command.Name, level: LogLevel.Information);
            });

            optionsBuilder.UseLoggerFactory(loggerFactory);
            */
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8", DelegationModes.ApplyToDatabases);
        }

        public virtual DbSet<NewBukken> NewBukkens { get; set; }

        public virtual DbSet<Bukken> Bukkens { get; set; }

        public virtual DbSet<File> Files { get; set; }

        public virtual DbSet<BukkenFile> BukkenFiles { get; set; }
    }
}
