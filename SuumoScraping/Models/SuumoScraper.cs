namespace SuumoScraping.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SuumoScraping.Extensions;

    public class SuumoScraper
    {
        private readonly DateTime _importedDate;

        private readonly ISuumoDataProvider _provider;

        private readonly IScrapingContextFactory _scrapingContextFactory;

        public SuumoScraper(ISuumoDataProvider provider, IScrapingContextFactory scrapingContextFactory)
        {
            this._importedDate = DateTime.Now.Date;
            this._provider = provider;
            this._scrapingContextFactory = scrapingContextFactory;
        }

        public void Execute()
        {
            var data1 = new List<(string, string)>();

            // 新宿
            // ParseRootPage("http://suumo.jp/ms/chuko/tokyo/sc_shinjuku/", detailPages);
            // var bukken2 = this._provider.GetBukkenDetail("https://suumo.jp" + "/ms/chuko/tokyo/sc_setagaya/nc_94305250/");

            // 都心部
            this._provider.GetAreaBukkenList("http://suumo.jp/ms/chuko/tokyo/sa_other_01/", data1);

            // 23区東部
            this._provider.GetAreaBukkenList("http://suumo.jp/ms/chuko/tokyo/sa_other_02/", data1);

            // 23区北部
            this._provider.GetAreaBukkenList("http://suumo.jp/ms/chuko/tokyo/sa_other_03/", data1);

            // 23区西部
            this._provider.GetAreaBukkenList("http://suumo.jp/ms/chuko/tokyo/sa_other_04/", data1);

            // 23区南部
            this._provider.GetAreaBukkenList("http://suumo.jp/ms/chuko/tokyo/sa_other_05/", data1);

            var count = 0;

            // 詳細ページを読み込む
            foreach (var data in data1)
            {
                using var db = this._scrapingContextFactory.Create();

                {
                    var msg = string.Format("count:{0}", count++);
                    System.Diagnostics.Debug.WriteLine(msg);
                }

                if (db.Bukkens.Any(m => m.ImportedDate == _importedDate && m.DetailUrl == data.Item2))
                {
                    continue;
                }

                try
                {
                    var src = this._provider.GetBukkenDetail("https://suumo.jp" + data.Item2);

                    // src.GetOrDefault("");

                    var company = new Company();
                    company.Name = src.GetOrDefault("企業名");
                    company.Address = src.GetOrDefault("企業住所");
                    company.TakkenLicense = src.GetOrDefault("宅建");
                    company.TransactionAspect = src.GetOrDefault("取引態様");
                   
                    var bukken = new Bukken();
                    bukken.Price = src.GetOrDefault("価格");
                    bukken.Price1 = src.GetOrDefault("価格最小", "0").ToDigit();
                    bukken.Price2 = src.GetOrDefault("価格最大", "0").ToDigit();
                    bukken.Access = src.GetOrDefault("交通1");
                    bukken.Access2 = src.GetOrDefault("交通2");
                    bukken.Access3 = src.GetOrDefault("交通3");
                    bukken.Direction = src.GetOrDefault("向き");
                    bukken.Balcony = src.GetOrDefault("バルコニー", "-");
                    bukken.BuiltYears = src.GetOrDefault("完成時期(築年月)");
                    bukken.Floor = src.GetOrDefault("所在階");
                    bukken.ManagementFee = src.GetOrDefault("管理費");
                    bukken.RepairingDeposit = src.GetOrDefault("修繕積立金");
                    bukken.RepairingFund = src.GetOrDefault("修繕積立基金");
                    bukken.Company = company;
                    bukken.Layout = src.GetOrDefault("間取り");
                    bukken.MoveInTime = src.GetOrDefault("入居時期");
                    bukken.FloorArea = src.GetOrDefault("専有面積");
                    bukken.FloorArea1 = Convert.ToDecimal(src.GetOrDefault("専有面積(㎡)", "0"));
                    bukken.FloorTubo = Convert.ToDecimal(src.GetOrDefault("専有面積(坪)", "0"));
                    bukken.FloorAreaMeasuringMethod = src.GetOrDefault("専有面積(計測方法)");
                    bukken.Address = src.GetOrDefault("所在地");
                    bukken.Restriction = src.GetOrDefault("その他制限事項");
                    bukken.RightsStyle = src.GetOrDefault("敷地の権利形態");
                    bukken.UseDistrict = src.GetOrDefault("用途地域");
                    bukken.Structure = src.GetOrDefault("構造・階建て");
                    bukken.Title = src.GetOrDefault("タイトル");

                    bukken.ImportedDate = _importedDate;
                    bukken.DetailUrl = data.Item2;

                    for (var i = 1; i < 255; i++)
                    {
                        if (!src.Any(m => m.Key.Contains($"画像{i}")))
                        {
                            break;
                        }

                        var imageUrl = src.Single(m => m.Key == $"画像{i}-URL").Value;
                        var imageAlt = src.Single(m => m.Key == $"画像{i}-概要").Value;

                        // URLのファイルがあればそれを使う
                        var file = db.Files.FirstOrDefault(m => m.Url == imageUrl);
                        if (file == null)
                        {
                            var fileData = this._provider.GetFileData(imageUrl);
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
                    System.Diagnostics.Debug.WriteLine(msg);

                    continue;
                }
            }
        }
    }
}
