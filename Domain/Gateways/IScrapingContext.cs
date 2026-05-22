namespace SuumoScraping.Domain.Gateways
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using SuumoScraping.Domain.Models;
    using SuumoScraping.ViewModels;

    public interface IScrapingContext : IDisposable, IAsyncDisposable
    {
        IQueryable<NewBukken> NewBukkens { get; }

        IQueryable<Bukken> Bukkens { get; }

        IQueryable<File> Files { get; }

        IQueryable<BukkenFile> BukkenFiles { get; }

        void AddBukken(Bukken bukken);

        void AddNewBukken(NewBukken newBukken);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        void SetCommandTimeout(int timeout);

        Task<IDisposable> BeginTransactionAsync(CancellationToken cancellationToken = default);

        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

        Task<List<T>> ToListAsync<T>(
            IQueryable<T> query,
            CancellationToken cancellationToken = default
        );

        Task<T> SingleOrDefaultAsync<T>(
            IQueryable<T> query,
            CancellationToken cancellationToken = default
        );

        Task<T> FirstOrDefaultAsync<T>(
            IQueryable<T> query,
            CancellationToken cancellationToken = default
        );

        Task<bool> AnyAsync<T>(IQueryable<T> query, CancellationToken cancellationToken = default);

        Task<IList<BukkenInfo>> GetFilteredBukkensAsync(
            FilterForm model,
            CancellationToken cancellationToken
        );

        Task<BukkenInfo> GetBukkenDetailsAsync(int id, CancellationToken cancellationToken);

        Task<List<Bukken>> GetBukkensWithFilesAndFullTextAsync(
            string url,
            CancellationToken cancellationToken
        );

        Task<NewBukken> GetNewBukkenWithPricesAndFilesAsync(
            string url,
            CancellationToken cancellationToken
        );
    }
}
