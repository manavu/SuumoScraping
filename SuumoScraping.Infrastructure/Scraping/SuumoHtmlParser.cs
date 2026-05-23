namespace SuumoScraping.Infrastructure.Scraping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using HtmlAgilityPack;
    using Microsoft.Extensions.Logging;
    using SuumoScraping.Domain.Exceptions;
    using SuumoScraping.Domain.Extensions;
    using SuumoScraping.Domain.Gateways;
    using SuumoScraping.Domain.Models;

    public class SuumoHtmlParser : ISuumoHtmlParser
    {
        private readonly ILogger<SuumoHtmlParser> _logger;

        public SuumoHtmlParser(ILogger<SuumoHtmlParser> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public AreaPageResult ParseAreaPage(string url, string htmlString)
        {
            if (string.IsNullOrEmpty(htmlString))
            {
                return new AreaPageResult(new List<ScrapedBukkenSummary>(), null);
            }

            var doc = new HtmlDocument();
            doc.OptionAutoCloseOnEnd = false;
            doc.OptionCheckSyntax = false;
            doc.OptionFixNestedTags = true;
            doc.LoadHtml(htmlString);

            var bukkens = new List<ScrapedBukkenSummary>();

            // 物件情報の塊のルート
            var nodes = doc.DocumentNode.SelectNodes("//div[@class='property_unit-content']");
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    var headerNode = node.SelectSingleNode(".//div[1]/h2[1]/a[1]");
                    if (headerNode != null)
                    {
                        var hrefAttr = headerNode.Attributes.SingleOrDefault(m => m.Name == "href");
                        if (hrefAttr != null)
                        {
                            var uri = new Uri("https://suumo.jp" + hrefAttr.Value);
                            var title = headerNode.InnerText;
                            var detailUrl = uri.AbsolutePath;

                            bukkens.Add(new ScrapedBukkenSummary(title, detailUrl));
                        }
                    }
                }
            }

            // ページ切り替えのノードを取得する
            string nextPageUrl = null;
            var paginationNodes = doc.DocumentNode.SelectNodes("//p[@class='pagination-parts']");
            if (paginationNodes != null)
            {
                foreach (var node in paginationNodes)
                {
                    var aNode = node.SelectSingleNode(".//a");
                    if (aNode != null && aNode.InnerText.Trim() == "次へ")
                    {
                        var nextPageAttr = aNode.Attributes.SingleOrDefault(m => m.Name == "href");
                        if (nextPageAttr != null)
                        {
                            nextPageUrl = "https://suumo.jp" + nextPageAttr.Value;
                            break;
                        }
                    }
                }
            }

            return new AreaPageResult(bukkens, nextPageUrl);
        }

        public ScrapedBukkenDetail ParseBukkenDetail(
            string url,
            string bukkengaiyoHtml,
            string bukkenTokuchoHtml
        )
        {
            if (string.IsNullOrEmpty(bukkengaiyoHtml))
            {
                throw new ArgumentException("物件概要のHTMLが空です。", nameof(bukkengaiyoHtml));
            }

            var rawValues = new Dictionary<string, string>();

            // 1. 物件概要ページのパース
            var docGaiyo = new HtmlDocument();
            docGaiyo.OptionAutoCloseOnEnd = false;
            docGaiyo.OptionCheckSyntax = false;
            docGaiyo.OptionFixNestedTags = true;
            docGaiyo.LoadHtml(bukkengaiyoHtml);

            var trNodes = docGaiyo.DocumentNode.SelectNodes(
                "//table[@summary='表' and position()=1]/tbody[1]/tr"
            );
            if (trNodes == null)
            {
                throw new SuumoParseException(
                    "物件概要テーブルのノード取得に失敗しました。HTML構造が変更された可能性があります。",
                    url,
                    "bukkengaiyo_table",
                    bukkengaiyoHtml
                );
            }

            var nodePacks = new List<Tuple<HtmlNode, HtmlNode>>();
            foreach (var trNode in trNodes)
            {
                var thNodes = trNode.SelectNodes(".//th");
                var tdNodes = trNode.SelectNodes(".//td");
                if (thNodes != null && tdNodes != null)
                {
                    for (var i = 0; i < thNodes.Count; i++)
                    {
                        nodePacks.Add(new Tuple<HtmlNode, HtmlNode>(thNodes[i], tdNodes[i]));
                    }
                }
            }

            foreach (var nodePack in nodePacks)
            {
                var thText = nodePack.Item1.InnerText;
                if (thText == "会社概要")
                {
                    try
                    {
                        var values_ = nodePack
                            .Item2.SelectSingleNode(".//div/p")
                            ?.ChildNodes?.Where(m =>
                                m.Name == "#text" && !string.IsNullOrWhiteSpace(m.InnerText)
                            )
                            ?.Select(m => m.InnerText.Trim())
                            ?.ToList();

                        if (values_ != null)
                        {
                            var length = values_.Count;
                            if (length >= 2)
                            {
                                rawValues["取引態様"] = values_[0];
                                rawValues["宅建"] = values_[1];
                                rawValues["企業住所"] = values_[length - 1];
                                rawValues["企業名"] = values_[length - 2];
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        this._logger.LogWarning(
                            e,
                            "会社概要のパース中にエラーが発生しました: {Url}. メッセージ: {Message}",
                            url,
                            e.Message
                        );
                    }

                    continue;
                }

                if (nodePack.Item1.SelectSingleNode(".//div") == null)
                {
                    continue;
                }

                var item = nodePack.Item1.SelectSingleNode(".//div").InnerText;
                var values = nodePack
                    .Item2.ChildNodes.Where(m => m.Name == "#text")
                    .Select(m => m.InnerText.Trim());

                switch (item)
                {
                    case "価格":
                        var priceVal = values.FirstOrDefault();
                        rawValues["価格"] = priceVal;
                        if (!string.IsNullOrEmpty(priceVal))
                        {
                            try
                            {
                                var prices = priceVal.Split('～');
                                if (prices.Length >= 1)
                                    rawValues["価格最小"] = prices[0];
                                if (prices.Length >= 2)
                                    rawValues["価格最大"] = prices[1];
                            }
                            catch (Exception e)
                            {
                                this._logger.LogWarning(
                                    e,
                                    "価格のパース中にエラーが発生しました: {Url}. 値: {Val}. メッセージ: {Message}",
                                    url,
                                    priceVal,
                                    e.Message
                                );
                            }
                        }

                        break;

                    case "専有面積":
                        var areaVal = string.Join(string.Empty, values);
                        rawValues["専有面積"] = areaVal;
                        try
                        {
                            var m1 = Regex.Match(areaVal, @"[0-9\.]+m");
                            var m2 = Regex.Match(areaVal, @"[0-9\.]*坪");
                            var m3 = Regex.Match(areaVal, @"壁芯|登記");

                            if (m1.Success)
                                rawValues["専任面積(㎡)"] = m1.Value.Replace("m", string.Empty); // 後方互換のため
                            if (m1.Success)
                                rawValues["専有面積(㎡)"] = m1.Value.Replace("m", string.Empty);
                            if (m2.Success)
                                rawValues["専任面積(坪)"] = m2.Value.Replace("坪", string.Empty);
                            if (m2.Success)
                                rawValues["専有面積(坪)"] = m2.Value.Replace("坪", string.Empty);
                            if (m3.Success)
                                rawValues["専有面積(計測方法)"] = m3.Value;
                        }
                        catch (Exception e)
                        {
                            this._logger.LogWarning(
                                e,
                                "専有面積のパース中にエラーが発生しました: {Url}. 値: {Val}. メッセージ: {Message}",
                                url,
                                areaVal,
                                e.Message
                            );
                        }

                        break;

                    case "交通":
                        var tmp = values.Where(m => !string.IsNullOrWhiteSpace(m)).ToArray();
                        if (tmp.Length >= 1)
                            rawValues["交通1"] = tmp[0];
                        if (tmp.Length >= 2)
                            rawValues["交通2"] = tmp[1];
                        if (tmp.Length >= 3)
                            rawValues["交通3"] = tmp[2];
                        break;

                    default:
                        rawValues[item] = values.FirstOrDefault();
                        break;
                }
            }

            // 2. 物件特徴ページのパース（画像やタイトル）
            var title = string.Empty;
            var images = new List<ScrapedImage>();

            if (!string.IsNullOrEmpty(bukkenTokuchoHtml))
            {
                var docTokucho = new HtmlDocument();
                docTokucho.OptionAutoCloseOnEnd = false;
                docTokucho.OptionCheckSyntax = false;
                docTokucho.OptionFixNestedTags = true;
                docTokucho.LoadHtml(bukkenTokuchoHtml);

                var titleNode = docTokucho
                    .DocumentNode.SelectNodes("//table[@summary='表']/tbody[1]/tr[1]/td")
                    ?.FirstOrDefault();

                if (titleNode != null)
                {
                    var titleValues = titleNode
                        .ChildNodes.Where(m => m.Name == "#text")
                        .Select(m => m.InnerText.Trim());
                    title = titleValues.FirstOrDefault() ?? string.Empty;
                }

                var mainNode = docTokucho.DocumentNode.SelectSingleNode(
                    "//div[@id='mainContents']"
                );
                var imageNodes = mainNode?.SelectNodes(".//a[@class='jscNyroModal nyroModal']");

                if (imageNodes != null)
                {
                    foreach (var imageNode in imageNodes)
                    {
                        var imageTag = imageNode.SelectSingleNode(".//img");
                        if (imageTag != null)
                        {
                            var imageUrl = imageTag
                                .Attributes.SingleOrDefault(m => m.Name == "rel")
                                ?.Value;
                            if (string.IsNullOrEmpty(imageUrl))
                            {
                                var srcAttr = imageTag.Attributes.SingleOrDefault(m =>
                                    m.Name == "src"
                                );
                                if (srcAttr != null)
                                    imageUrl = srcAttr.Value;
                            }

                            var imageAlt = imageTag
                                .Attributes.SingleOrDefault(m => m.Name == "alt")
                                ?.Value;
                            imageAlt = string.IsNullOrEmpty(imageAlt) ? "-" : imageAlt;

                            if (imageUrl != null && imageUrl.Contains(".jpg"))
                            {
                                // 画像サイズ調整とエスケープ解除
                                imageUrl = Regex.Replace(imageUrl, @"(w=[0-9]+)", "w=452");
                                imageUrl = Regex.Replace(imageUrl, @"(h=[0-9]+)", "h=339");
                                imageUrl = imageUrl.Replace("&amp;", "&");

                                images.Add(new ScrapedImage(imageUrl, imageAlt));
                            }
                        }
                    }
                }
            }

            // 3. 型安全な DTO の組み立てと値のクレンジング
            var company = new ScrapedCompany(
                GetOrDefault(rawValues, "企業名"),
                GetOrDefault(rawValues, "企業住所"),
                GetOrDefault(rawValues, "宅建"),
                GetOrDefault(rawValues, "取引態様")
            );

            var accesses = new List<string>();
            if (rawValues.ContainsKey("交通1"))
                accesses.Add(rawValues["交通1"]);
            if (rawValues.ContainsKey("交通2"))
                accesses.Add(rawValues["交通2"]);
            if (rawValues.ContainsKey("交通3"))
                accesses.Add(rawValues["交通3"]);

            var priceRaw = GetOrDefault(rawValues, "価格");
            var priceMin = GetOrDefault(rawValues, "価格最小", "0").ToDigit();
            var priceMax = rawValues.ContainsKey("価格最大")
                ? (decimal?)rawValues["価格最大"].ToDigit()
                : null;

            var floorAreaRaw = GetOrDefault(rawValues, "専有面積");
            var floorAreaSqm = Convert.ToDecimal(GetOrDefault(rawValues, "専有面積(㎡)", "0"));
            var floorTubo = rawValues.ContainsKey("専有面積(坪)")
                ? (decimal?)Convert.ToDecimal(rawValues["専有面積(坪)"])
                : null;

            return new ScrapedBukkenDetail(
                title,
                GetOrDefault(rawValues, "所在地"),
                GetOrDefault(rawValues, "間取り"),
                floorAreaRaw,
                floorAreaSqm,
                floorTubo,
                GetOrDefault(rawValues, "専有面積(計測方法)"),
                priceRaw,
                priceMin,
                priceMax,
                accesses,
                GetOrDefault(rawValues, "バルコニー", "-"),
                GetOrDefault(rawValues, "完成時期(築年月)"),
                GetOrDefault(rawValues, "管理費"),
                GetOrDefault(rawValues, "修繕積立金"),
                GetOrDefault(rawValues, "修繕積立基金"),
                GetOrDefault(rawValues, "所在階"),
                GetOrDefault(rawValues, "向き"),
                GetOrDefault(rawValues, "用途地域"),
                GetOrDefault(rawValues, "構造・階建て"),
                GetOrDefault(rawValues, "敷地の権利形態"),
                GetOrDefault(rawValues, "入居時期"),
                GetOrDefault(rawValues, "その他制限事項"),
                company,
                images
            );
        }

        private static string GetOrDefault(
            Dictionary<string, string> dict,
            string key,
            string defaultValue = ""
        )
        {
            return dict.TryGetValue(key, out var val) ? val : defaultValue;
        }
    }
}
