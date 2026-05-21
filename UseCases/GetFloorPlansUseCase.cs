namespace SuumoScraping.UseCases
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using SuumoScraping.Models;
    using SuumoScraping.ViewModels;

    public class GetFloorPlansUseCase
    {
        private readonly IScrapingContextFactory _scrapingContextFactory;

        public GetFloorPlansUseCase(IScrapingContextFactory scrapingContextFactory)
        {
            _scrapingContextFactory = scrapingContextFactory;
        }

        public async Task<IList<FloorPlanInfo>> ExecuteAsync(
            CancellationToken cancellationToken = default
        )
        {
            using (var db = _scrapingContextFactory.Create())
            {
                return await db
                    .NewBukkens.SelectMany(m => m.Files)
                    .Where(m => m.Type == "間取り図")
                    .Select(m => new FloorPlanInfo()
                    {
                        FileId = m.File.Id,
                        BukkenId = m.NewBukken.Id,
                        FloorArea = m.NewBukken.FloorArea,
                    })
                    .Take(1000)
                    .ToListAsync(cancellationToken);
            }
        }
    }
}
