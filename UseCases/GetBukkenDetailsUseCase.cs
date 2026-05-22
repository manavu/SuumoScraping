namespace SuumoScraping.UseCases
{
    using System.Threading;
    using System.Threading.Tasks;
    using SuumoScraping.Domain.Gateways;
    using SuumoScraping.ViewModels;

    public class GetBukkenDetailsUseCase
    {
        private readonly IScrapingContextFactory _scrapingContextFactory;

        public GetBukkenDetailsUseCase(IScrapingContextFactory scrapingContextFactory)
        {
            this._scrapingContextFactory = scrapingContextFactory;
        }

        public async Task<BukkenInfo> ExecuteAsync(
            int id,
            CancellationToken cancellationToken = default
        )
        {
            using (var db = this._scrapingContextFactory.Create())
            {
                return await db.GetBukkenDetailsAsync(id, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
