namespace SuumoScraping.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using SuumoScraping.Domain.Gateways;
    using SuumoScraping.Domain.Models;

    public class SuumoScraper
    {
        private readonly DateTime _importedDate;
        private readonly ISuumoGateway _gateway;
        private readonly IScrapingContextFactory _scrapingContextFactory;

        public SuumoScraper(ISuumoGateway gateway, IScrapingContextFactory scrapingContextFactory)
        {
            this._importedDate = DateTime.Now.Date;
            this._gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
            this._scrapingContextFactory = scrapingContextFactory ?? throw new ArgumentNullException(nameof(scrapingContextFactory));
        }

        public void Execute(CancellationToken ct = default)
        {
            var detailUrls = new List<string>();
            var targetAreas = new[]
            {
                "https://suumo.jp/ms/chuko/saitama/sc_toda/",
                "https://suumo.jp/ms/chuko/saitama/sc_saitamashiurawa/",
                "https://suumo.jp/ms/chuko/saitama/sc_saitamashiminami/"
            };

            foreach (var areaUrl in targetAreas)
            {
                var currentUrl = areaUrl;
                while (!string.IsNullOrEmpty(currentUrl))
                {
                    if (ct.IsCancellationRequested)
                    {
                        Console.WriteLine("Scraping cancelled.");
                        break;
                    }

                    try
                    {
                        var result = this._gateway.GetAreaPage(currentUrl);
                        foreach (var bukkenSummary in result.Bukkens)
                        {
                            detailUrls.Add(bukkenSummary.DetailUrl);
                        }
                        currentUrl = result.NextPageUrl;
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine($"エリア取得エラー: {currentUrl} msg: {e.Message}");
                        break;
                    }

                    // クローラーマナーとしての適切なウェイト設定
                    Thread.Sleep(1000);
                }
            }

            // 重複排除
            detailUrls = detailUrls.Distinct().ToList();

            // 詳細ページを読み込む
            foreach (var detailUrl in detailUrls)
            {
                if (ct.IsCancellationRequested)
                {
                    Console.WriteLine("Scraping cancelled.");
                    break;
                }

                using var db = this._scrapingContextFactory.Create();

                if (db.Bukkens.Any(m => m.ImportedDate == _importedDate && m.DetailUrl == detailUrl))
                {
                    continue;
                }

                try
                {
                    var src = this._gateway.GetBukkenDetail("https://suumo.jp" + detailUrl);

                    var company = new Company();
                    company.Name = src.Company.Name;
                    company.Address = src.Company.Address;
                    company.TakkenLicense = src.Company.TakkenLicense;
                    company.TransactionAspect = src.Company.TransactionAspect;

                    var bukken = new Bukken();
                    bukken.Price = src.PriceRaw;
                    bukken.Price1 = src.PriceMin;
                    bukken.Price2 = src.PriceMax;
                    bukken.Access = src.Accesses.ElementAtOrDefault(0) ?? "";
                    bukken.Access2 = src.Accesses.ElementAtOrDefault(1);
                    bukken.Access3 = src.Accesses.ElementAtOrDefault(2);
                    bukken.Direction = src.Direction;
                    bukken.Balcony = src.Balcony;
                    bukken.BuiltYears = src.BuiltYears;
                    bukken.Floor = src.Floor;
                    bukken.ManagementFee = src.ManagementFee;
                    bukken.RepairingDeposit = src.RepairingDeposit;
                    bukken.RepairingFund = src.RepairingFund;
                    bukken.Company = company;
                    bukken.Layout = src.Layout;
                    bukken.MoveInTime = src.MoveInTime;
                    bukken.FloorArea = src.FloorAreaRaw;
                    bukken.FloorArea1 = src.FloorAreaSqm;
                    bukken.FloorTubo = src.FloorTubo;
                    bukken.FloorAreaMeasuringMethod = src.FloorAreaMeasuringMethod;
                    bukken.Address = src.Address;
                    bukken.Restriction = src.Restriction;
                    bukken.RightsStyle = src.RightsStyle;
                    bukken.UseDistrict = src.UseDistrict;
                    bukken.Structure = src.Structure;
                    bukken.Title = src.Title;

                    bukken.ImportedDate = _importedDate;
                    bukken.DetailUrl = detailUrl;

                    foreach (var image in src.Images)
                    {
                        var imageUrl = image.Url;
                        var imageAlt = image.Alt;

                        // URLのファイルがあればそれを使う
                        var file = db.Files.FirstOrDefault(m => m.Url == imageUrl);
                        if (file == null)
                        {
                            var fileData = this._gateway.GetFileData(imageUrl);
                            if (fileData == null)
                            {
                                continue;
                            }

                            file = new File(fileData, "image/jpeg", imageUrl);
                        }

                        var bukkenFile = new BukkenFile();
                        bukkenFile.File = file;
                        bukkenFile.Type = imageAlt;
                        bukken.Files.Add(bukkenFile);
                    }

                    db.Bukkens.Add(bukken);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    var msg = e.Message;
                    System.Diagnostics.Debug.WriteLine($"物件詳細の取得・保存エラー ({detailUrl}): {msg}");
                    continue;
                }
            }
        }
    }
}
