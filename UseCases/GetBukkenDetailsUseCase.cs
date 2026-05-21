namespace SuumoScraping.UseCases
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using SuumoScraping.Models;
    using SuumoScraping.ViewModels;

    public class GetBukkenDetailsUseCase
    {
        private readonly IScrapingContextFactory _scrapingContextFactory;

        public GetBukkenDetailsUseCase(IScrapingContextFactory scrapingContextFactory)
        {
            _scrapingContextFactory = scrapingContextFactory;
        }

        public async Task<BukkenInfo> ExecuteAsync(
            int id,
            CancellationToken cancellationToken = default
        )
        {
            using (var db = _scrapingContextFactory.Create())
            {
                return await db
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
                        Files = m.Files.Select(n => new FileInfo
                        {
                            Id = n.File.Id,
                            Title = n.Type,
                        }),
                        Prices = m.PriceChangesets.Select(n => new PriceInfo()
                        {
                            ChangedAt = n.ChangedAt,
                            Value = n.Text,
                        }),
                        ImportCount = m.ImportCount,
                    })
                    .SingleOrDefaultAsync(cancellationToken);
            }
        }
    }
}
