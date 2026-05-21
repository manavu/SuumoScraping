namespace SuumoScraping.Infrastructure.Scraping
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ISuumoHtmlFetcher : IDisposable
    {
        Task<string> GetHtmlStringAsync(string url, CancellationToken cancellationToken = default);
        Task<byte[]> GetFileDataAsync(string url, CancellationToken cancellationToken = default);
    }
}
