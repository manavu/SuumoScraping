namespace SuumoScraping.UseCases
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using SuumoScraping.Domain.Gateways;
    using SuumoScraping.ViewModels;

    public class GetFloorPlansUseCase
    {
        private readonly IScrapingContextFactory _scrapingContextFactory;

        public GetFloorPlansUseCase(IScrapingContextFactory scrapingContextFactory)
        {
            this._scrapingContextFactory = scrapingContextFactory;
        }

        public async Task<IList<FloorPlanInfo>> ExecuteAsync(
            CancellationToken cancellationToken = default
        )
        {
            using (var db = this._scrapingContextFactory.Create())
            {
                var query = db
                    .NewBukkens.SelectMany(m => m.Files)
                    .Where(m => m.Type == "間取り図")
                    .Select(m => new FloorPlanInfo()
                    {
                        FileId = m.File.Id,
                        BukkenId = m.NewBukken.Id,
                        FloorArea = m.NewBukken.FloorArea,
                    })
                    .Take(1000);

                var list = await db.ToListAsync(query, cancellationToken).ConfigureAwait(false);
                return list;
            }
        }
    }
}
