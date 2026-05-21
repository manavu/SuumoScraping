namespace SuumoScraping.Domain.Gateways
{
    using System.Collections.Generic;
    using SuumoScraping.Domain.Models;

    public record AreaPageResult(
        IReadOnlyList<ScrapedBukkenSummary> Bukkens,
        string NextPageUrl
    );

    public interface ISuumoGateway
    {
        // 1ページ分の物件一覧を取得（再帰を廃止し、呼び出し側でループやウェイトを制御可能にする）
        AreaPageResult GetAreaPage(string url);

        // 物件詳細を取得
        ScrapedBukkenDetail GetBukkenDetail(string detailUrl);

        // 画像などのバイナリデータを取得
        byte[] GetFileData(string url);
    }
}
