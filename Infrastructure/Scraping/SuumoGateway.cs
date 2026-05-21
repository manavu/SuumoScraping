namespace SuumoScraping.Infrastructure.Scraping
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using SuumoScraping.Domain.Exceptions;
    using SuumoScraping.Domain.Gateways;
    using SuumoScraping.Domain.Models;

    public class SuumoGateway : ISuumoGateway
    {
        private readonly ISuumoHtmlFetcher _fetcher;
        private readonly ISuumoHtmlParser _parser;
        private readonly ILogger<SuumoGateway> _logger;

        public SuumoGateway(
            ISuumoHtmlFetcher fetcher,
            ISuumoHtmlParser parser,
            ILogger<SuumoGateway> logger
        )
        {
            this._fetcher = fetcher ?? throw new ArgumentNullException(nameof(fetcher));
            this._parser = parser ?? throw new ArgumentNullException(nameof(parser));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AreaPageResult> GetAreaPageAsync(
            string url,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                this._logger.LogInformation("エリア一覧取得処理開始: {Url}", url);
                var html = await this
                    ._fetcher.GetHtmlStringAsync(url, cancellationToken)
                    .ConfigureAwait(false);

                if (string.IsNullOrEmpty(html))
                {
                    throw new SuumoParseException("取得したHTMLが空です。", url, "area_page_html");
                }

                var result = this._parser.ParseAreaPage(url, html);
                this._logger.LogInformation(
                    "エリア一覧取得処理成功: {Url}. 取得件数: {Count}件",
                    url,
                    result.Bukkens.Count
                );
                return result;
            }
            catch (SuumoScrapingException)
            {
                // ドメイン定義済みの例外はそのまま上位に伝播
                throw;
            }
            catch (Exception e)
            {
                this._logger.LogError(
                    e,
                    "エリア一覧取得処理中に予期せぬエラーが発生しました: {Url}",
                    url
                );
                throw new SuumoScrapingException("エリア一覧の取得に失敗しました。", url, e);
            }
        }

        public async Task<ScrapedBukkenDetail> GetBukkenDetailAsync(
            string detailUrl,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                this._logger.LogInformation("物件詳細取得処理開始: {Url}", detailUrl);

                // 物件概要ページ
                var gaiyoUrl = detailUrl.EndsWith("/")
                    ? detailUrl + "bukkengaiyo/?fmlg=t001"
                    : detailUrl + "/bukkengaiyo/?fmlg=t001";
                this._logger.LogInformation("物件概要フェッチ開始: {Url}", gaiyoUrl);
                var gaiyoHtml = await this
                    ._fetcher.GetHtmlStringAsync(gaiyoUrl, cancellationToken)
                    .ConfigureAwait(false);

                if (string.IsNullOrEmpty(gaiyoHtml))
                {
                    throw new SuumoParseException(
                        "取得した物件概要HTMLが空です。",
                        gaiyoUrl,
                        "bukkengaiyo_html"
                    );
                }

                // 物件特徴ページ
                this._logger.LogInformation("物件特徴フェッチ開始: {Url}", detailUrl);
                var tokuchoHtml = await this
                    ._fetcher.GetHtmlStringAsync(detailUrl, cancellationToken)
                    .ConfigureAwait(false);

                var detail = this._parser.ParseBukkenDetail(detailUrl, gaiyoHtml, tokuchoHtml);
                this._logger.LogInformation("物件詳細取得・解析成功: {Url}", detailUrl);
                return detail;
            }
            catch (SuumoScrapingException)
            {
                throw;
            }
            catch (Exception e)
            {
                this._logger.LogError(
                    e,
                    "物件詳細取得・解析中に予期せぬエラーが発生しました: {Url}",
                    detailUrl
                );
                throw new SuumoScrapingException("物件詳細の取得に失敗しました。", detailUrl, e);
            }
        }

        public async Task<byte[]> GetFileDataAsync(
            string url,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                this._logger.LogInformation("画像データ等のファイルダウンロード開始: {Url}", url);
                var data = await this
                    ._fetcher.GetFileDataAsync(url, cancellationToken)
                    .ConfigureAwait(false);
                this._logger.LogInformation(
                    "ファイルダウンロード成功: {Url}. サイズ: {Size}bytes",
                    url,
                    data?.Length ?? 0
                );
                return data;
            }
            catch (SuumoScrapingException)
            {
                throw;
            }
            catch (Exception e)
            {
                this._logger.LogError(
                    e,
                    "ファイルダウンロード中に予期せぬエラーが発生しました: {Url}",
                    url
                );
                throw new SuumoScrapingException("ファイルのダウンロードに失敗しました。", url, e);
            }
        }
    }
}
