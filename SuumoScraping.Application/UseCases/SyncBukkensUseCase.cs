namespace SuumoScraping.Application.UseCases
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using SuumoScraping.Domain.Gateways;
    using SuumoScraping.Domain.Models;

    public class SyncBukkensUseCase
    {
        private readonly IScrapingContextFactory _scrapingContextFactory;

        private readonly ILogger<SyncBukkensUseCase> _logger;

        public SyncBukkensUseCase(
            IScrapingContextFactory scrapingContextFactory,
            ILogger<SyncBukkensUseCase> logger
        )
        {
            _scrapingContextFactory = scrapingContextFactory;
            _logger = logger;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            using (var db = _scrapingContextFactory.Create())
            {
                var query =
                    from bukken in db.Bukkens
                    group bukken.DetailUrl by bukken.DetailUrl into g
                    select g.Key;
                var urls = await db.ToListAsync(query, cancellationToken);

                foreach (var url in urls)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        _logger.LogInformation("Sync cancelled.");
                        break;
                    }

                    _logger.LogInformation($"Syncing: {url}");
                    await this.SyncBukkenAsync(url, cancellationToken);
                }
            }
        }

        private async Task SyncBukkenAsync(string url, CancellationToken cancellationToken)
        {
            using (var db = _scrapingContextFactory.Create())
            using (var tx = await db.BeginTransactionAsync(cancellationToken))
            {
                db.SetCommandTimeout(0);

                var bukkens = await db.GetBukkensWithFilesAndFullTextAsync(url, cancellationToken);
                var newBukken = await db.GetNewBukkenWithPricesAndFilesAsync(
                    url,
                    cancellationToken
                );

                if (newBukken == null)
                {
                    newBukken = new NewBukken();
                    newBukken.CreatedAt = bukkens.Min(m => m.ImportedDate);
                    db.AddNewBukken(newBukken);
                }

                foreach (var bukken in bukkens)
                {
                    newBukken.DetailUrl = url;
                    newBukken.Access1 = bukken.Access;
                    newBukken.Access2 = bukken.Access2;
                    newBukken.Access3 = bukken.Access3;
                    newBukken.Address = bukken.Address;
                    newBukken.Balcony = bukken.Balcony;

                    if (!string.IsNullOrEmpty(bukken.BuiltYears))
                    {
                        var ret = DateTime.MinValue;

                        if (DateTime.TryParse(bukken.BuiltYears + "1日", out ret))
                        {
                            newBukken.BuiltYears = ret;
                        }
                    }

                    newBukken.Company.Name = bukken.Company.Name;
                    newBukken.Company.Address = bukken.Company.Address;
                    newBukken.Company.TakkenLicense = bukken.Company.TakkenLicense;
                    newBukken.Company.TransactionAspect = bukken.Company.TransactionAspect;

                    newBukken.Direction = bukken.Direction;
                    newBukken.Floor = bukken.Floor;
                    newBukken.FloorArea = bukken.FloorArea;
                    newBukken.FloorArea1 = bukken.FloorArea1;
                    newBukken.FloorAreaMeasuringMethod = bukken.FloorAreaMeasuringMethod;
                    newBukken.FloorTubo = bukken.FloorTubo;

                    newBukken.ImportedAt = bukken.ImportedDate;
                    newBukken.Layout = bukken.Layout;
                    newBukken.ManagementFee = bukken.ManagementFee;
                    newBukken.MoveInTime = bukken.MoveInTime;

                    newBukken.RepairingDeposit = bukken.RepairingDeposit;
                    newBukken.RepairingFund = bukken.RepairingFund;
                    newBukken.Restriction = bukken.Restriction;
                    newBukken.RightsStyle = bukken.RightsStyle;
                    newBukken.Structure = bukken.Structure;
                    newBukken.Title = bukken.Title;
                    newBukken.UseDistrict = bukken.UseDistrict;

                    bukken.Price2 = bukken.Price2 != 0 ? bukken.Price2 : null;

                    var currentPrice = newBukken
                        .PriceChangesets.OrderByDescending(m => m.ChangedAt)
                        .FirstOrDefault();

                    if (
                        currentPrice == null
                        || currentPrice.Min != bukken.Price1
                        || currentPrice.Max != bukken.Price2
                    )
                    {
                        var newPrice = new Price();
                        newPrice.Min = bukken.Price1;
                        newPrice.Max = bukken.Price2;
                        newPrice.Text = bukken.Price;
                        newPrice.ChangedAt = bukken.ImportedDate;

                        newBukken.PriceChangesets.Add(newPrice);
                    }

                    foreach (var bukkenFile in bukken.Files)
                    {
                        if (newBukken.Files.Any(m => m.File.Url == bukkenFile.File.Url))
                        {
                            continue;
                        }

                        var newBukkenFile = new NewBukkenFile();
                        newBukkenFile.File = bukkenFile.File;
                        newBukkenFile.Type = bukkenFile.Type;

                        newBukken.Files.Add(newBukkenFile);
                    }
                }

                newBukken.ImportCount = bukkens.Count;

                await db.SaveChangesAsync(cancellationToken);
                await db.CommitTransactionAsync(cancellationToken);
            }
        }
    }
}
