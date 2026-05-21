namespace SuumoScraping.Infrastructure.Scraping
{
    using System;
    using SuumoScraping.Domain.Gateways;
    using SuumoScraping.Domain.Models;

    public class SuumoGateway : ISuumoGateway
    {
        private readonly ISuumoHtmlFetcher _fetcher;
        private readonly ISuumoHtmlParser _parser;

        public SuumoGateway(ISuumoHtmlFetcher fetcher, ISuumoHtmlParser parser)
        {
            this._fetcher = fetcher ?? throw new ArgumentNullException(nameof(fetcher));
            this._parser = parser ?? throw new ArgumentNullException(nameof(parser));
        }

        public AreaPageResult GetAreaPage(string url)
        {
            var html = this._fetcher.GetHtmlString(url);
            return this._parser.ParseAreaPage(html);
        }

        public ScrapedBukkenDetail GetBukkenDetail(string detailUrl)
        {
            // 物件概要ページ
            var gaiyoUrl = detailUrl.EndsWith("/") ? detailUrl + "bukkengaiyo/?fmlg=t001" : detailUrl + "/bukkengaiyo/?fmlg=t001";
            var gaiyoHtml = this._fetcher.GetHtmlString(gaiyoUrl);

            // 物件特徴ページ
            var tokuchoHtml = this._fetcher.GetHtmlString(detailUrl);

            return this._parser.ParseBukkenDetail(gaiyoHtml, tokuchoHtml);
        }

        public byte[] GetFileData(string url)
        {
            return this._fetcher.GetFileData(url);
        }
    }
}
