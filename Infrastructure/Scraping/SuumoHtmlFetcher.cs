namespace SuumoScraping.Infrastructure.Scraping
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using SuumoScraping.Domain.Exceptions;
    using SuumoScraping.Domain.Gateways;

    public class SuumoHtmlFetcher : ISuumoHtmlFetcher
    {
        private readonly HttpClient _client;
        private readonly ILogger<SuumoHtmlFetcher> _logger;

        public SuumoHtmlFetcher(ILogger<SuumoHtmlFetcher> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var handler = new HttpClientHandler();
            handler.AllowAutoRedirect = true;

            this._client = new HttpClient(handler);
            this._client.DefaultRequestHeaders.Add(
                "User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:85.0) Gecko/20100101 Firefox/85.0");
        }

        public async Task<string> GetHtmlStringAsync(string url, CancellationToken cancellationToken = default)
        {
            const int maxRetry = 10;
            for (var i = 0; i < maxRetry; i++)
            {
                try
                {
                    this._logger.LogInformation("HTML取得開始: {Url} (試行 {RetryCount}/{MaxRetry})", url, i + 1, maxRetry);
                    return await this._client.GetStringAsync(url, cancellationToken).ConfigureAwait(false);
                }
                catch (HttpRequestException e)
                {
                    this._logger.LogWarning(e, "HTML取得に失敗しました (HttpRequestException): {Url}. メッセージ: {Message}", url, e.Message);

                    if (e.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        throw new SuumoFetchException("Suumoサーバーで内部エラーが発生しました。", url, (int?)e.StatusCode, e);
                    }

                    if (i == maxRetry - 1)
                    {
                        throw new SuumoFetchException("リトライ上限に達したため、HTML取得処理を断念しました。", url, (int?)e.StatusCode, e);
                    }

                    await Task.Delay(5000, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    this._logger.LogWarning(e, "予期せぬエラーによりHTML取得に失敗しました: {Url}. メッセージ: {Message}", url, e.Message);

                    if (i == maxRetry - 1)
                    {
                        throw new SuumoFetchException("リトライ上限に達したため、予期せぬエラーによりHTML取得処理を断念しました。", url, null, e);
                    }

                    await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
                }
            }

            throw new SuumoFetchException("HTML取得処理が正常に終了しませんでした（不明な理由）。", url);
        }

        public async Task<byte[]> GetFileDataAsync(string url, CancellationToken cancellationToken = default)
        {
            const int maxRetry = 10;
            for (var i = 0; i < maxRetry; i++)
            {
                try
                {
                    this._logger.LogInformation("バイナリデータ取得開始: {Url} (試行 {RetryCount}/{MaxRetry})", url, i + 1, maxRetry);
                    
                    var response = await this._client.GetAsync(url, cancellationToken).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (HttpRequestException e)
                {
                    this._logger.LogWarning(e, "バイナリ取得に失敗しました (HttpRequestException): {Url}. メッセージ: {Message}", url, e.Message);

                    if (e.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        throw new SuumoFetchException("Suumoサーバーで内部エラーが発生しました（バイナリ取得）。", url, (int?)e.StatusCode, e);
                    }

                    if (i == maxRetry - 1)
                    {
                        throw new SuumoFetchException("リトライ上限に達したため、バイナリデータ取得処理を断念しました。", url, (int?)e.StatusCode, e);
                    }

                    await Task.Delay(10000, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    this._logger.LogWarning(e, "予期せぬエラーによりバイナリ取得に失敗しました: {Url}. メッセージ: {Message}", url, e.Message);

                    if (i == maxRetry - 1)
                    {
                        throw new SuumoFetchException("リトライ上限に達したため、予期せぬエラーによりバイナリデータ取得処理を断念しました。", url, null, e);
                    }

                    await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
                }
            }

            throw new SuumoFetchException("バイナリデータ取得処理が正常に終了しませんでした（不明な理由）。", url);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._client?.Dispose();
            }
        }
    }
}
