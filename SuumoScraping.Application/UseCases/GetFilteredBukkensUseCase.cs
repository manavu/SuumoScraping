namespace SuumoScraping.Application.UseCases
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using SuumoScraping.Domain.Gateways;
    using SuumoScraping.Domain.Models;

    public class GetFilteredBukkensUseCase
    {
        private readonly IScrapingContextFactory _scrapingContextFactory;

        public GetFilteredBukkensUseCase(IScrapingContextFactory scrapingContextFactory)
        {
            this._scrapingContextFactory = scrapingContextFactory;
        }

        public async Task<IList<BukkenInfo>> ExecuteAsync(
            FilterForm model,
            CancellationToken cancellationToken = default
        )
        {
            using (var db = this._scrapingContextFactory.Create())
            {
                return await db.GetFilteredBukkensAsync(model, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}
