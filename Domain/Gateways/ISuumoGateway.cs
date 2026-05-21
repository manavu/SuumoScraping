namespace SuumoScraping.Domain.Gateways
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using SuumoScraping.Domain.Models;

    public record AreaPageResult(IReadOnlyList<ScrapedBukkenSummary> Bukkens, string NextPageUrl);

    public interface ISuumoGateway
    {
        // 1ページ分の物件一覧を取得（再帰を廃止し、呼び出し側でループやウェイトを制御可能にする）
        Task<AreaPageResult> GetAreaPageAsync(
            string url,
            CancellationToken cancellationToken = default
        );

        // 物件詳細を取得
        Task<ScrapedBukkenDetail> GetBukkenDetailAsync(
            string detailUrl,
            CancellationToken cancellationToken = default
        );

        // 画像などのバイナリデータを取得
        Task<byte[]> GetFileDataAsync(string url, CancellationToken cancellationToken = default);
    }
}
