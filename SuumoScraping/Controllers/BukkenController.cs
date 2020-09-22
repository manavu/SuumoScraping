namespace SuumoScraping.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using LinqKit;
    using SuumoScraping.Models;
    using SuumoScraping.ViewModels;
    using SuumoScraping.Extensions;

    public class BukkenController : Controller
    {
        private readonly IScrapingContextFactory _scrapingContextFactory;

        public BukkenController(IScrapingContextFactory scrapingContextFactory)
        {
            _scrapingContextFactory = scrapingContextFactory;
        }

        public ActionResult Test()
        {
            using (var db = _scrapingContextFactory.Create())
            {
                db.Database.CommandTimeout = 0;

                var urls = from bukken in db.Bukkens
                           group bukken.DetailUrl by bukken.DetailUrl into g
                           select g.Key;

                /*
                Parallel.ForEach(urls.ToList(), (url) =>
                {
                    System.Diagnostics.Debug.WriteLine(url);
                    AddNewBukken(url);
                });*/

                foreach (var url in urls.ToList())
                {
                    System.Diagnostics.Debug.WriteLine(url);
                    AddNewBukken(url);
                }
            }

            return this.Content("aa");
        }

        private void AddNewBukken(string url)
        {
            using (var db = _scrapingContextFactory.Create())
            using (var tx = db.Database.BeginTransaction())
            {
                db.Database.CommandTimeout = 0;

                var bukkens = db.Bukkens
                    .Include("Files")
                    .Include("FullText")
                    .Where(m => m.DetailUrl == url)
                    .OrderBy(m => m.ImportedDate)
                    .ToList();

                var newBukken = db.NewBukkens
                    .Include("PriceChangesets")
                    .SingleOrDefault(m => m.DetailUrl == url);

                if (newBukken == null)
                {
                    newBukken = new NewBukken();
                    newBukken.CreatedAt = bukkens.Min(m => m.ImportedDate);
                    db.NewBukkens.Add(newBukken);
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

                    newBukken.Company = bukken.Company;
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

                    var currentPrice = newBukken.PriceChangesets
                        .OrderByDescending(m => m.ChangedAt)
                        .FirstOrDefault();

                    if (currentPrice == null || currentPrice.Min != bukken.Price1 || currentPrice.Max != bukken.Price2)
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

                        var newBukkenFile = new NewBukkenFile()
                        {
                            File = bukkenFile.File,
                            Type = bukkenFile.Type,
                        };

                        newBukken.Files.Add(newBukkenFile);
                    }

                    newBukken.ImportCount++;

                    while (bukken.Files.Any())
                    {
                        var bukkenFile = bukken.Files.First();
                        db.BukkenFiles.Remove(bukkenFile);
                    }

                    if (bukken.FullText != null)
                    {
                        db.Set<BukkenFulltext>().Remove(bukken.FullText);
                    }

                    db.Bukkens.Remove(bukken);
                }

                try
                {
                    db.SaveChanges();
                    tx.Commit();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException e)
                {
                    var msg = e.Message;
                    System.Diagnostics.Debug.WriteLine(msg);
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException e)
                {
                    var msg = e.Message;
                    System.Diagnostics.Debug.WriteLine(msg);
                }
            }
        }

        public ActionResult Test2()
        {
            using (var db = _scrapingContextFactory.Create())
            {
                db.Database.CommandTimeout = 0;

                var bukkenIds = db.Bukkens
                    .Where(m => m.FullText == null)
                    .Select(m => m.Id)
                    .ToList();

                System.Diagnostics.Debug.WriteLine("count" + bukkenIds.Count.ToString());

                Parallel.ForEach(bukkenIds, (bukkenId) =>
                {
                    System.Diagnostics.Debug.WriteLine(bukkenId.ToString());

                    using (var db2 = _scrapingContextFactory.Create())
                    {
                        var src = db2.Bukkens.Single(m => m.Id == bukkenId);

                        var tmp = new System.Text.StringBuilder();

                        if (!string.IsNullOrEmpty(src.Access))
                        {
                            tmp.Append(src.Access);
                        }

                        if (!string.IsNullOrEmpty(src.Access2))
                        {
                            tmp.AppendFormat("、{0}", src.Access2);
                        }

                        if (!string.IsNullOrEmpty(src.Access3))
                        {
                            tmp.AppendFormat("、{0}", src.Access3);
                        }

                        src.FullText = new BukkenFulltext();

                        src.FullText.AccessBigram = string.Join(" ", tmp.ToString().ToNGram(2));

                        src.FullText.AddressBigram = string.Join(" ", src.Address.ToNGram(2));

                        db2.SaveChanges();
                    }
                });

                /*
                foreach (var bukkenId in bukkenIds)
                {
                    System.Diagnostics.Debug.WriteLine(bukkenId.ToString());

                    using (var db2 = new ScrapingContext())
                    {
                        var src = db2.Bukkens.Single(m => m.Id == bukkenId);

                        var tmp = new System.Text.StringBuilder();

                        if (!string.IsNullOrEmpty(src.Access))
                        {
                            tmp.Append(src.Access);
                        }

                        if (!string.IsNullOrEmpty(src.Access2))
                        {
                            tmp.AppendFormat("、{0}", src.Access2);
                        }

                        if (!string.IsNullOrEmpty(src.Access3))
                        {
                            tmp.AppendFormat("、{0}", src.Access3);
                        }

                        src.FullText = new BukkenFulltext();

                        src.FullText.AccessBigram = string.Join(" ", tmp.ToString().ToNGram(2));

                        src.FullText.AddressBigram = string.Join(" ", src.Address.ToNGram(2));

                        db2.SaveChanges();
                    }
                }*/
            }

            return this.Content("aa");
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

            // grid-column=ImportedDate&grid-dir=0
            using (var db = _scrapingContextFactory.Create())
            {
                db.Database.CommandTimeout = 0;

                var bukkens = db.NewBukkens.AsQueryable();

                // タイトルでフィルタ
                if (!string.IsNullOrEmpty(model.Title))
                {
                    bukkens = bukkens.Where(m => m.Title.Contains(model.Title));
                }

                // 住所でフィルタ
                if (!string.IsNullOrEmpty(model.Address))
                {
                    var builder = PredicateBuilder.New<NewBukken>(true);

                    foreach (var address in model.Address.Split(' ', '　'))
                    {
                        builder = builder.Or(m => m.Address.Contains(address));
                    }

                    bukkens = bukkens.AsExpandable().Where(builder);
                }

                // 交通でフィルタ
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
                    bukkens = bukkens
                        .Where(m => m.PriceChangesets.OrderByDescending(n => n.ChangedAt).First().Min >= minPrice);
                }

                if (model.MaxPrice.HasValue)
                {
                    var maxPrice = model.MaxPrice.Value * 10000m;
                    bukkens = bukkens
                        .Where(m => m.PriceChangesets.OrderByDescending(n => n.ChangedAt).FirstOrDefault().Min <= maxPrice);
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
                        Price = m.PriceChangesets
                            .OrderByDescending(n => n.ChangedAt)
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

            var data = this.TempData.Get<IList<BukkenInfo>>("BukkenInfos") ?? new List<BukkenInfo>();

            this.TempData.Put("BukkenInfos", data);
            this.TempData.Put("FilterForm", filter);

            return View(data);
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            using (var db = _scrapingContextFactory.Create())
            {
                var model = db.NewBukkens
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
                        Price = m.PriceChangesets
                            .OrderByDescending(n => n.ChangedAt)
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
                            Title = n.Type
                        }),
                        ImportCount = m.ImportCount,
                    })
                    .Single();

                return View(model);
            }
        }

        [HttpGet]
        public ActionResult Import()
        {
            using (var provider = new LoggingDataProvider(new SuumoDataProvider()))
            {
                var service = new SuumoScraper(provider, _scrapingContextFactory);

                service.Execute();
            }

            return RedirectToAction("Filter");
        }
    }
}