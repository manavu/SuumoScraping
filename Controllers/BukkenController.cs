namespace SuumoScraping.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using LinqKit;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using SuumoScraping.Extensions;
    using SuumoScraping.Models;
    using SuumoScraping.ViewModels;

    public class BukkenController : Controller
    {
        private readonly IScrapingContextFactory _scrapingContextFactory;

        private readonly ILogger<SuumoDataProvider> _logger;

        private readonly BukkenService _bukkenService;

        public BukkenController(
            IScrapingContextFactory scrapingContextFactory,
            ILogger<SuumoDataProvider> logger,
            BukkenService bukkenService
        )
        {
            _scrapingContextFactory = scrapingContextFactory;
            _logger = logger;
            _bukkenService = bukkenService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Filter()
        {
            var filter = this.TempData.Get<FilterForm>("FilterForm") ?? new FilterForm();

            return this.View(filter);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Filter(FilterForm model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction("Filter");
            }

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
                    var _importedDateTo = model.ImportedDateTo.Value.AddDays(1);
                    bukkens = bukkens.Where(m => m.ImportedAt <= _importedDateTo);
                }

                var data = bukkens
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
                    .ToList();

                this.TempData.Put("BukkenInfos", data);
            }

            this.TempData.Put("FilterForm", model);

            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult List()
        {
            var filter = this.TempData.Get<FilterForm>("FilterForm") ?? new FilterForm();

            var data =
                this.TempData.Get<IList<BukkenInfo>>("BukkenInfos") ?? new List<BukkenInfo>();

            this.TempData.Put("BukkenInfos", data);
            this.TempData.Put("FilterForm", filter);

            return View(data);
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            using (var db = _scrapingContextFactory.Create())
            {
                var model = db
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
                    .Single();

                return View(model);
            }
        }
    }
}
