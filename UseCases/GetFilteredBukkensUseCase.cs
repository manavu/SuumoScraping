namespace SuumoScraping.UseCases
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using LinqKit;
    using Microsoft.EntityFrameworkCore;
    using SuumoScraping.Models;
    using SuumoScraping.ViewModels;

    public class GetFilteredBukkensUseCase
    {
        private readonly IScrapingContextFactory _scrapingContextFactory;

        public GetFilteredBukkensUseCase(IScrapingContextFactory scrapingContextFactory)
        {
            _scrapingContextFactory = scrapingContextFactory;
        }

        public async Task<IList<BukkenInfo>> ExecuteAsync(
            FilterForm model,
            CancellationToken cancellationToken = default
        )
        {
            using (var db = _scrapingContextFactory.Create())
            {
                var bukkens = db.NewBukkens.AsQueryable();

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
                        m.PriceChangesets.OrderByDescending(n => n.ChangedAt).First().Min
                        >= minPrice
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
        }
    }
}
