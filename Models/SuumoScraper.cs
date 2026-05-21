namespace SuumoScraping.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using SuumoScraping.Domain.Exceptions;
    using SuumoScraping.Domain.Gateways;
    using SuumoScraping.Domain.Models;

    public class SuumoScraper
    {
        private readonly DateTime _importedDate;
        private readonly ISuumoGateway _gateway;
        private readonly IScrapingContextFactory _scrapingContextFactory;
        private readonly ILogger<SuumoScraper> _logger;

        public SuumoScraper(
            ISuumoGateway gateway,
            IScrapingContextFactory scrapingContextFactory,
            ILogger<SuumoScraper> logger
        )
        {
            this._importedDate = DateTime.Now.Date;
            this._gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
            this._scrapingContextFactory =
                scrapingContextFactory
                ?? throw new ArgumentNullException(nameof(scrapingContextFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ExecuteAsync(CancellationToken ct = default)
        {
            var detailUrls = new List<string>();
            var targetAreas = new[]
            {
                "https://suumo.jp/ms/chuko/saitama/sc_toda/",
                "https://suumo.jp/ms/chuko/saitama/sc_saitamashiurawa/",
                "https://suumo.jp/ms/chuko/saitama/sc_saitamashiminami/",
            };

            foreach (var areaUrl in targetAreas)
            {
                var currentUrl = areaUrl;
                while (!string.IsNullOrEmpty(currentUrl))
                {
                    if (ct.IsCancellationRequested)
                    {
                        this._logger.LogInformation(
                            "スクレイピング処理がキャンセルされました（エリア巡回中）。"
                        );
                        break;
                    }

                    try
                    {
                        var result = await this
                            ._gateway.GetAreaPageAsync(currentUrl, ct)
                            .ConfigureAwait(false);
                        foreach (var bukkenSummary in result.Bukkens)
                        {
                            detailUrls.Add(bukkenSummary.DetailUrl);
                        }

                        currentUrl = result.NextPageUrl;
                    }
                    catch (SuumoScrapingException e)
                    {
                        this._logger.LogError(
                            e,
                            "エリア一覧の取得中にスクレイピング例外が発生しました。エリア: {AreaUrl}",
                            currentUrl
                        );
                        break;
                    }
                    catch (Exception e)
                    {
                        this._logger.LogError(
                            e,
                            "エリア一覧の取得中に予期せぬ例外が発生しました。エリア: {AreaUrl}",
                            currentUrl
                        );
                        break;
                    }

                    // クローラーマナーとしての適切なウェイト設定
                    await Task.Delay(1000, ct).ConfigureAwait(false);
                }
            }

            // 重複排除
            detailUrls = detailUrls.Distinct().ToList();
            this._logger.LogInformation(
                "巡回対象のユニーク物件詳細URL数: {Count}件",
                detailUrls.Count
            );

            // 詳細ページを読み込む
            foreach (var detailUrl in detailUrls)
            {
                if (ct.IsCancellationRequested)
                {
                    this._logger.LogInformation(
                        "スクレイピング処理がキャンセルされました（詳細取得中）。"
                    );
                    break;
                }

                using var db = this._scrapingContextFactory.Create();

                if (
                    db.Bukkens.Any(m => m.ImportedDate == _importedDate && m.DetailUrl == detailUrl)
                )
                {
                    this._logger.LogInformation(
                        "本日すでに取得済みの物件のためスキップします: {Url}",
                        detailUrl
                    );
                    continue;
                }

                try
                {
                    var fullDetailUrl = "https://suumo.jp" + detailUrl;
                    var src = await this
                        ._gateway.GetBukkenDetailAsync(fullDetailUrl, ct)
                        .ConfigureAwait(false);

                    var company = new Company();
                    company.Name = src.Company.Name;
                    company.Address = src.Company.Address;
                    company.TakkenLicense = src.Company.TakkenLicense;
                    company.TransactionAspect = src.Company.TransactionAspect;

                    var bukken = new Bukken();
                    bukken.Price = src.PriceRaw;
                    bukken.Price1 = src.PriceMin;
                    bukken.Price2 = src.PriceMax;
                    bukken.Access = src.Accesses.ElementAtOrDefault(0) ?? string.Empty;
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
                            var fileData = await this
                                ._gateway.GetFileDataAsync(imageUrl, ct)
                                .ConfigureAwait(false);
                            if (fileData == null || fileData.Length == 0)
                            {
                                this._logger.LogWarning(
                                    "画像のダウンロードに失敗したかデータが空のため、画像の紐付けをスキップします: {Url}",
                                    imageUrl
                                );
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

                    this._logger.LogInformation(
                        "物件データの取得・DB保存に成功しました: {Url} ({Title})",
                        detailUrl,
                        bukken.Title
                    );
                }
                catch (SuumoFetchException e)
                {
                    this._logger.LogError(
                        e,
                        "物件詳細のフェッチ（通信）中にエラーが発生しました。スキップして後続の処理を行います。URL: {Url}, HTTPステータス: {Status}",
                        detailUrl,
                        e.HttpStatusCode
                    );
                }
                catch (SuumoParseException e)
                {
                    this._logger.LogError(
                        e,
                        "物件詳細の解析（パース）中にエラーが発生しました。スキップして後続の処理を行います。URL: {Url}, 失敗箇所: {Element}",
                        detailUrl,
                        e.ElementName
                    );
                }
                catch (SuumoScrapingException e)
                {
                    this._logger.LogError(
                        e,
                        "物件詳細の取得中にスクレイピング例外が発生しました。スキップして後続の処理を行います。URL: {Url}",
                        detailUrl
                    );
                }
                catch (Exception e)
                {
                    this._logger.LogError(
                        e,
                        "物件詳細の取得・保存中に予期せぬ例外が発生しました。スキップして後続の処理を行います。URL: {Url}",
                        detailUrl
                    );
                }

                // 詳細巡回ごとの適切なウェイト
                await Task.Delay(1000, ct).ConfigureAwait(false);
            }
        }
    }
}
