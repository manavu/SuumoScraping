namespace SuumoScraping.Infrastructure.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using LinqKit;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using Microsoft.Extensions.Logging;
    using SuumoScraping.Domain.Gateways;
    using SuumoScraping.Domain.Models;

    public partial class ScrapingContext : DbContext, IScrapingContext
    {
        private IDbContextTransaction _currentTransaction;

        public ScrapingContext(DbContextOptions<ScrapingContext> options)
            : base(options)
        {
            // 必須のコンストラクタ。
        }

        IQueryable<NewBukken> IScrapingContext.NewBukkens => this.NewBukkens;

        IQueryable<Bukken> IScrapingContext.Bukkens => this.Bukkens;

        IQueryable<File> IScrapingContext.Files => this.Files;

        IQueryable<BukkenFile> IScrapingContext.BukkenFiles => this.BukkenFiles;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Company Value Object configuration (Owned entity)
            modelBuilder.Entity<Bukken>().OwnsOne(b => b.Company);
            modelBuilder.Entity<NewBukken>().OwnsOne(b => b.Company);

            // Indexes
            modelBuilder.Entity<File>().HasIndex(f => f.Url, "IX_Files_Url");

            modelBuilder
                .Entity<Bukken>()
                .HasIndex(
                    b => new { b.DetailUrl, b.ImportedDate },
                    "IX_Bukkens_DetailUrl_ImportedDate"
                );
            modelBuilder
                .Entity<Bukken>()
                .HasIndex(
                    b => new { b.ImportedDate, b.DetailUrl },
                    "IX_Bukkens_ImportedDate_DetailUrl"
                );

            modelBuilder
                .Entity<NewBukken>()
                .HasIndex(b => b.DetailUrl, "IX_Bukkens_DetailUrl")
                .IsUnique();
        }

        public virtual DbSet<NewBukken> NewBukkens { get; set; }

        public virtual DbSet<Bukken> Bukkens { get; set; }

        public virtual DbSet<File> Files { get; set; }

        public virtual DbSet<BukkenFile> BukkenFiles { get; set; }

        public void AddBukken(Bukken bukken)
        {
            this.Bukkens.Add(bukken);
        }

        public void AddNewBukken(NewBukken newBukken)
        {
            this.NewBukkens.Add(newBukken);
        }

        public void SetCommandTimeout(int timeout)
        {
            this.Database.SetCommandTimeout(timeout);
        }

        public async Task<IDisposable> BeginTransactionAsync(
            CancellationToken cancellationToken = default
        )
        {
            this._currentTransaction = await this.Database.BeginTransactionAsync(cancellationToken);
            return this._currentTransaction;
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (this._currentTransaction != null)
            {
                await this._currentTransaction.CommitAsync(cancellationToken);
                await this._currentTransaction.DisposeAsync();
                this._currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (this._currentTransaction != null)
            {
                await this._currentTransaction.RollbackAsync(cancellationToken);
                await this._currentTransaction.DisposeAsync();
                this._currentTransaction = null;
            }
        }

        public Task<List<T>> ToListAsync<T>(
            IQueryable<T> query,
            CancellationToken cancellationToken = default
        )
        {
            return query.ToListAsync(cancellationToken);
        }

        public Task<T> SingleOrDefaultAsync<T>(
            IQueryable<T> query,
            CancellationToken cancellationToken = default
        )
        {
            return query.SingleOrDefaultAsync(cancellationToken);
        }

        public Task<T> FirstOrDefaultAsync<T>(
            IQueryable<T> query,
            CancellationToken cancellationToken = default
        )
        {
            return query.FirstOrDefaultAsync(cancellationToken);
        }

        public Task<bool> AnyAsync<T>(
            IQueryable<T> query,
            CancellationToken cancellationToken = default
        )
        {
            return query.AnyAsync(cancellationToken);
        }

        public async Task<IList<BukkenInfo>> GetFilteredBukkensAsync(
            FilterForm model,
            CancellationToken cancellationToken
        )
        {
            var bukkens = this.NewBukkens.AsQueryable();

            if (!string.IsNullOrEmpty(model.Title))
            {
                bukkens = bukkens.Where(m => m.Title.Contains(model.Title));
            }

            if (!string.IsNullOrEmpty(model.Address))
            {
                var builder = PredicateBuilder.New<NewBukken>(true);

                foreach (var address in model.Address.Split(' ', '　'))
                {
                    builder = builder.Or(m => m.Address.Contains(address));
                }

                bukkens = bukkens.AsExpandable().Where(builder);
            }

            if (!string.IsNullOrEmpty(model.Access))
            {
                var builder = PredicateBuilder.New<NewBukken>(true);

                foreach (var access in model.Access.Split(' ', '　'))
                {
                    builder = builder.Or(m => m.Access1.Contains(access));
                    builder = builder.Or(m => m.Access2.Contains(access));
                    builder = builder.Or(m => m.Access3.Contains(access));
                }

                bukkens = bukkens.AsExpandable().Where(builder);
            }

            if (model.MinPrice.HasValue)
            {
                var minPrice = model.MinPrice.Value * 10000m;
                bukkens = bukkens.Where(m =>
                    m.PriceChangesets.OrderByDescending(n => n.ChangedAt).First().Min >= minPrice
                );
            }

            if (model.MaxPrice.HasValue)
            {
                var maxPrice = model.MaxPrice.Value * 10000m;
                bukkens = bukkens.Where(m =>
                    m.PriceChangesets.OrderByDescending(n => n.ChangedAt).FirstOrDefault().Min
                    <= maxPrice
                );
            }

            if (model.MinArea.HasValue)
            {
                bukkens = bukkens.Where(m => m.FloorArea1 >= model.MinArea);
            }

            if (model.MaxArea.HasValue)
            {
                bukkens = bukkens.Where(m => m.FloorArea1 <= model.MaxArea);
            }

            if (model.ImportedDateFrom.HasValue)
            {
                bukkens = bukkens.Where(m => m.ImportedAt >= model.ImportedDateFrom);
            }

            if (model.ImportedDateTo.HasValue)
            {
                var importedDateTo = model.ImportedDateTo.Value.AddDays(1);
                bukkens = bukkens.Where(m => m.ImportedAt <= importedDateTo);
            }

            return await bukkens
                .Select(m => new BukkenInfo
                {
                    Id = m.Id,
                    Access1 = m.Access1,
                    Address = m.Address,
                    BuiltYears = m.BuiltYears,
                    Direction = m.Direction,
                    FloorArea = m.FloorArea,
                    Layout = m.Layout,
                    Price = m
                        .PriceChangesets.OrderByDescending(n => n.ChangedAt)
                        .FirstOrDefault()
                        .Text,
                    Title = m.Title,
                    ImportedDate = m.ImportedAt,
                    ImportCount = m.ImportCount,
                })
                .OrderByDescending(m => m.Id)
                .Take(2000)
                .ToListAsync(cancellationToken);
        }

        public async Task<BukkenInfo> GetBukkenDetailsAsync(
            int id,
            CancellationToken cancellationToken
        )
        {
            return await this
                .NewBukkens.Include(m => m.PriceChangesets)
                .Include(m => m.Files)
                .Where(m => m.Id == id)
                .Select(m => new BukkenInfo
                {
                    Id = m.Id,
                    Access1 = m.Access1,
                    Access2 = m.Access2,
                    Access3 = m.Access3,
                    Address = m.Address,
                    BuiltYears = m.BuiltYears,
                    Direction = m.Direction,
                    Floor = m.Floor,
                    Layout = m.Layout,
                    Price = m
                        .PriceChangesets.OrderByDescending(n => n.ChangedAt)
                        .FirstOrDefault()
                        .Text,
                    Title = m.Title,
                    FloorArea = m.FloorArea,
                    ManagementFee = m.ManagementFee,
                    RepairingDeposit = m.RepairingDeposit,
                    RepairingFund = m.RepairingFund,
                    Balcony = m.Balcony,
                    DetailUrl = m.DetailUrl,
                    ImportedDate = m.ImportedAt,
                    MoveInTime = m.MoveInTime,
                    RightsStyle = m.RightsStyle,
                    Structure = m.Structure,
                    UseDistrict = m.UseDistrict,
                    CompanyAddress = m.Company.Address,
                    CompanyName = m.Company.Name,
                    Files = m.Files.Select(n => new FileInfo { Id = n.File.Id, Title = n.Type }),
                    Prices = m.PriceChangesets.Select(n => new PriceInfo()
                    {
                        ChangedAt = n.ChangedAt,
                        Value = n.Text,
                    }),
                    ImportCount = m.ImportCount,
                })
                .SingleOrDefaultAsync(cancellationToken);
        }

        public Task<List<Bukken>> GetBukkensWithFilesAndFullTextAsync(
            string url,
            CancellationToken cancellationToken
        )
        {
            return this
                .Bukkens.Include(m => m.Files)
                    .ThenInclude(m => m.File)
                .Include(m => m.FullText)
                .Where(m => m.DetailUrl == url)
                .OrderBy(m => m.ImportedDate)
                .ToListAsync(cancellationToken);
        }

        public Task<NewBukken> GetNewBukkenWithPricesAndFilesAsync(
            string url,
            CancellationToken cancellationToken
        )
        {
            return this
                .NewBukkens.Include(m => m.PriceChangesets)
                .Include(m => m.Files)
                    .ThenInclude(m => m.File)
                .SingleOrDefaultAsync(m => m.DetailUrl == url, cancellationToken);
        }
    }
}
