namespace SuumoScraping.Infrastructure.Scraping
{
    using SuumoScraping.Domain.Gateways;
    using SuumoScraping.Domain.Models;

    public interface ISuumoHtmlParser
    {
        AreaPageResult ParseAreaPage(string url, string htmlString);
        ScrapedBukkenDetail ParseBukkenDetail(string url, string bukkengaiyoHtml, string bukkenTokuchoHtml);
    }
}
