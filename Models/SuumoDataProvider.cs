namespace SuumoScraping.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using HtmlAgilityPack;
    using Microsoft.Extensions.Logging;

    public interface ISuumoDataProvider : IDisposable
    {
        void GetAreaBukkenList(string url, IList<(string, string)> data);

        IDictionary<string, string> GetBukkenDetail(string url);

        byte[] GetFileData(string url);
    }

    public class LoggingDataProvider : ISuumoDataProvider
    {
        private readonly ISuumoDataProvider _inner;
        private readonly ILogger<SuumoDataProvider> _logger;

        public LoggingDataProvider(ISuumoDataProvider inner, ILogger<SuumoDataProvider> logger)
        {
            this._inner = inner;
            this._logger = logger;
        }

        ~LoggingDataProvider()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void GetAreaBukkenList(string url, IList<(string, string)> data)
        {
            var sw = new Stopwatch();
            sw.Start();
            this._inner.GetAreaBukkenList(url, data);
            sw.Stop();

            if (_logger != null)
            {
                var msg = string.Format("{0} {1}", sw.ElapsedMilliseconds, url);
                _logger.LogInformation(msg);
            }
        }

        public IDictionary<string, string> GetBukkenDetail(string url)
        {
            var sw = new Stopwatch();
            sw.Start();
            var ret = this._inner.GetBukkenDetail(url);
            sw.Stop();

            if (_logger != null)
            {
                var msg = string.Format("{0} {1}", sw.ElapsedMilliseconds, url);
                _logger.LogInformation(msg);
            }

            return ret;
        }

        public byte[] GetFileData(string url)
        {
            var sw = new Stopwatch();
            sw.Start();
            var ret = this._inner.GetFileData(url);
            sw.Stop();

            if (_logger != null)
            {
                var msg = string.Format("{0} {1}", sw.ElapsedMilliseconds, url);
                _logger.LogInformation(msg);
            }

            return ret;
        }

        private void Dispose(bool isDisposing)
        {
            // アンマネージリソース開放

            if (isDisposing)
            {
                // マネージリソース開放
                if (this._inner != null)
                {
                    this._inner.Dispose();
                }
            }
        }
    }

    public class SuumoDataProvider : ISuumoDataProvider
    {
        private readonly HttpClient _client;

        public SuumoDataProvider()
        {
            var handler = new HttpClientHandler();
            handler.AllowAutoRedirect = true;

            this._client = new HttpClient(handler);

            // ユーザーエージェント文字列をセット（オプション）
            this._client.DefaultRequestHeaders.Add(
                "User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36");
        }

        ~SuumoDataProvider()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void GetAreaBukkenList(string url, IList<(string, string)> data)
        {
            var htmlString = this.GetHtmlString(url);
            if (string.IsNullOrEmpty(htmlString))
            {
                return;
            }

            var doc = new HtmlDocument();

            doc.OptionAutoCloseOnEnd = false;  //最後に自動で閉じる（？）
            doc.OptionCheckSyntax = false;     //文法チェック。
            doc.OptionFixNestedTags = true;    //閉じタグが欠如している場合の処理

            doc.LoadHtml(htmlString);

            {
                // 物件情報の塊のルート
                var nodes = doc.DocumentNode.SelectNodes("//div[@class='property_unit-content']");

                foreach (var node in nodes)
                {
                    var headerNode = node.SelectSingleNode(".//div[1]/h2[1]/a[1]");

                    // 標準がコンセプトページになったので、余計なクエリを取り除く
                    var uri = new Uri("https://suumo.jp" + headerNode.Attributes.Single(m => m.Name == "href").Value);

                    var title = headerNode.InnerText;
                    var detailUrl = uri.AbsolutePath;

                    data.Add((title, detailUrl));
                }
            }

            {
                // ページ切り替えのノードを取得する
                var nodes = doc.DocumentNode.SelectNodes("//p[@class='pagination-parts']");

                foreach (var node in nodes.Where(m => m.HasChildNodes))
                {
                    var href = node.FirstChild;

                    if (href.InnerText == "次へ")
                    {
                        var nextPageUrl = node.FirstChild.Attributes.Single(m => m.Name == "href").Value;

                        // var rand = new Random();
                        // System.Threading.Thread.Sleep(1000 * (rand.Next() % 10));

                        this.GetAreaBukkenList("https://suumo.jp" + nextPageUrl, data);

                        return;
                    }
                }
            }
        }

        public IDictionary<string, string> GetBukkenDetail(string url)
        {
            var bukken = new Dictionary<string, string>();

            {
                // 物件概要ページ
                var data = this.GetHtmlString(url + "bukkengaiyo/?fmlg=t001");
                if (string.IsNullOrEmpty(data))
                {
                    var msg = string.Format("ParseDetailPage {0}", url);
                    throw new Exception(msg);
                }

                var doc = new HtmlDocument();

                doc.OptionAutoCloseOnEnd = false;  //最後に自動で閉じる（？）
                doc.OptionCheckSyntax = false;     //文法チェック。
                doc.OptionFixNestedTags = true;    //閉じタグが欠如している場合の処理

                doc.LoadHtml(data);

                //var trNodes1 = doc.DocumentNode.SelectNodes("//table[1]/tbody[1]/tr");
                var trNodes1 = doc.DocumentNode.SelectNodes("//table[@summary='表' and position()=1]/tbody[1]/tr");
                //var trNodes1 = doc.DocumentNode.SelectNodes("//table[@summary='表']/tbody[1]/tr");

                if (trNodes1 == null)
                {
                    var msg = string.Format("ParseDetailPage SelectNodes failed. {0}", url);
                    throw new Exception(msg);
                }

                var nodePacks = new List<Tuple<HtmlNode, HtmlNode>>();

                foreach (var trNode in trNodes1)
                {
                    var thNodes = trNode.SelectNodes(".//th");
                    var tdNodes = trNode.SelectNodes(".//td");

                    for (var i = 0; i < thNodes.Count; i++)
                    {
                        var nodePack = new Tuple<HtmlNode, HtmlNode>(thNodes[i], tdNodes[i]);

                        nodePacks.Add(nodePack);
                    }
                }

                foreach (var nodePack in nodePacks)
                {
                    if (nodePack.Item1.InnerText == "会社概要")
                    {
                        try
                        {
                            var values_ = nodePack.Item2.SelectSingleNode(".//div/p")
                                .ChildNodes
                                .Where(m => m.Name == "#text")
                                .Select(m => m.InnerText)
                                .ToList();

                            var length = values_.Count();

                            if (length >= 2)
                            {
                                bukken.Add("取引態様", values_[0]);  // 取引態様
                                bukken.Add("宅建", values_[1]);      // 宅建
                                bukken.Add("企業住所", values_[length - 1]);   // 住所
                                bukken.Add("企業名", values_[length - 2]);      // 企業名
                            }
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine("企業分解:" + url + " msg:" + e.Message);
                        }

                        continue;
                    }

                    if (nodePack.Item1.SelectSingleNode(".//div") == null)
                    {
                        continue;
                    }

                    var item = nodePack.Item1.SelectSingleNode(".//div").InnerText;

                    // タグ等を除くテキスト部分だけを抽出
                    var values = nodePack.Item2.ChildNodes
                        .Where(m => m.Name == "#text")
                        .Select(m => m.InnerText.Trim('\t', '\r', '\n'));

                    switch (item)
                    {
                        case "価格":
                            bukken.Add("価格", values.First());

                            try
                            {
                                var prices = values.First().Split('～');

                                if (prices.Length >= 1)
                                {
                                    bukken.Add("価格最小", prices[0]);
                                }
                                if (prices.Length >= 2)
                                {
                                    bukken.Add("価格最大", prices[1]);
                                }
                            }
                            catch (Exception e)
                            {
                                System.Diagnostics.Debug.WriteLine("価格分解:" + url + " msg:" + e.Message);
                            }

                            break;
                        case "専有面積":
                            bukken.Add("専有面積", string.Join("", values));

                            try
                            {
                                var text = string.Join("", values);

                                var m1 = Regex.Match(text, @"[0-9\.]+m");
                                var m2 = Regex.Match(text, @"[0-9\.]*坪");
                                var m3 = Regex.Match(text, @"壁芯|登記");

                                if (m1.Success)
                                {
                                    bukken.Add("専有面積(㎡)", m1.Value.Replace("m", ""));
                                }

                                if (m2.Success)
                                {
                                    bukken.Add("専有面積(坪)", m2.Value.Replace("坪", ""));
                                }

                                if (m3.Success)
                                {
                                    bukken.Add("専有面積(計測方法)", m3.Value);
                                }
                            }
                            catch (Exception e)
                            {
                                System.Diagnostics.Debug.WriteLine("面積分解:" + url + " msg:" + e.Message);
                            }

                            break;
                        case "交通":
                            {
                                // 空の要素があるので取り除く
                                var tmp = values.Where(m => !string.IsNullOrWhiteSpace(m)).ToArray();

                                if (tmp.Length >= 1)
                                {
                                    bukken.Add("交通1", tmp[0]);
                                }

                                if (tmp.Length >= 2)
                                {
                                    bukken.Add("交通2", tmp[1]);
                                }

                                if (tmp.Length >= 3)
                                {
                                    bukken.Add("交通3", tmp[2]);
                                }
                            }
                            break;
                        default:
                            bukken.Add(item, values.First());
                            break;
                    }
                }
            }

            // --

            {
                // 物件特徴ページ
                var data = this.GetHtmlString(url);
                if (string.IsNullOrEmpty(data))
                {
                    var msg = string.Format("ParseDetailPage {0}", url);
                    throw new Exception(msg);
                }

                var doc = new HtmlDocument();

                doc.OptionAutoCloseOnEnd = false;  //最後に自動で閉じる（？）
                doc.OptionCheckSyntax = false;     //文法チェック。
                doc.OptionFixNestedTags = true;    //閉じタグが欠如している場合の処理

                doc.LoadHtml(data);

                // 物件名が入っているノードを取得
                var titleNode = doc.DocumentNode
                    .SelectNodes("//table[@summary='表']/tbody[1]/tr[1]/td")
                    .FirstOrDefault();

                if (titleNode != null)
                {
                    var values = titleNode.ChildNodes
                        .Where(m => m.Name == "#text")
                        .Select(m => m.InnerText.Trim('\t', '\r', '\n'));

                    bukken.Add("タイトル", values.First());
                }

                // ----画像はここから

                // メインの要素を取得
                var mainNode = doc.DocumentNode.SelectSingleNode("//div[@id='mainContents']");

                // 画像の要素を列挙
                var imageNodes = mainNode.SelectNodes(".//a[@class='jscNyroModal nyroModal']");

                if (imageNodes != null)
                {
                    var i = 0;
                    foreach (var imageNode in imageNodes)
                    {
                        var imageTag = imageNode.SelectSingleNode(".//img");

                        // 画像のURL
                        var imageUrl = imageTag.Attributes.SingleOrDefault(m => m.Name == "rel")?.Value;
                        if (string.IsNullOrEmpty(imageUrl))
                        {
                            imageUrl = imageTag.Attributes.Single(m => m.Name == "src").Value;
                        }

                        // 画像の概要
                        var imageAlt = imageTag.Attributes.Single(m => m.Name == "alt")?.Value;

                        // 空文字はダメなので
                        imageAlt = string.IsNullOrEmpty(imageAlt) ? "-" : imageAlt;

                        // JPEGでない場合は続行しない
                        if (!imageUrl.Contains(".jpg"))
                        {
                            continue;
                        }

                        imageUrl = Regex.Replace(imageUrl, @"(w=[0-9]+)", "w=452");
                        imageUrl = Regex.Replace(imageUrl, @"(h=[0-9]+)", "h=339");
                        imageUrl = imageUrl.Replace("&amp;", "&");

                        bukken.Add($"画像{i + 1}-URL", imageUrl);
                        bukken.Add($"画像{i + 1}-概要", imageAlt);

                        i++;
                    }
                }
            }

            return bukken;
        }

        private string GetHtmlString(string url)
        {
            for (var i = 0; i < 10; i++)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine(url);
                    return this._client.GetStringAsync(url).Result;
                }
                catch (AggregateException e) when (e.InnerException is HttpRequestException)
                {
                    // HTTPの例外だった場合
                    var ie = e.InnerException;

                    var msg = ie.Message;
                    System.Diagnostics.Debug.WriteLine(msg);
                    if (msg.Contains("Internal Server Error"))
                    {
                        break;
                    }

                    System.Threading.Thread.Sleep(1000 * 5);
                }
                catch (Exception e)
                {
                    var msg = e.Message;
                    System.Diagnostics.Debug.WriteLine(msg);

                    System.Threading.Thread.Sleep(1000);
                }
            }

            return null;
        }

        public byte[] GetFileData(string url)
        {
            for (var i = 0; i < 10; i++)
            {
                try
                {
                    using (var imageData = this._client.GetStreamAsync(url).Result)
                    using (var ms = new MemoryStream())
                    {
                        imageData.CopyTo(ms);
                        ms.Seek(0, SeekOrigin.Begin);

                        var data = new byte[ms.Length];
                        ms.Read(data, 0, (int)ms.Length);

                        return data;
                    }
                }
                catch (AggregateException e) when (e.InnerException is HttpRequestException)
                {
                    // HTTPの例外だった場合
                    var ie = e.InnerException;

                    var msg = ie.Message;
                    System.Diagnostics.Debug.WriteLine(msg);
                    if (msg.Contains("Internal Server Error"))
                    {
                        break;
                    }

                    System.Threading.Thread.Sleep(1000 * 10);
                }
                catch (Exception e)
                {
                    var msg = e.Message;
                    System.Diagnostics.Debug.WriteLine(msg);

                    System.Threading.Thread.Sleep(1000);
                }
            }

            return null;
        }

        private void Dispose(bool isDisposing)
        {
            // アンマネージリソース開放

            if (isDisposing)
            {
                // マネージリソース開放
                if (this._client != null)
                {
                    this._client.Dispose();
                }
            }
        }
    }
}