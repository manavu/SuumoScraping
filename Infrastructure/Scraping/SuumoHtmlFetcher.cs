namespace SuumoScraping.Infrastructure.Scraping
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading;

    public interface ISuumoHtmlFetcher : IDisposable
    {
        string GetHtmlString(string url);
        byte[] GetFileData(string url);
    }

    public class SuumoHtmlFetcher : ISuumoHtmlFetcher
    {
        private readonly HttpClient _client;

        public SuumoHtmlFetcher()
        {
            var handler = new HttpClientHandler();
            handler.AllowAutoRedirect = true;

            this._client = new HttpClient(handler);
            this._client.DefaultRequestHeaders.Add(
                "User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:85.0) Gecko/20100101 Firefox/85.0");
        }

        public string GetHtmlString(string url)
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
                    var ie = e.InnerException;
                    var msg = ie.Message;
                    System.Diagnostics.Debug.WriteLine(msg);

                    if (msg.Contains("Internal Server Error"))
                    {
                        break;
                    }

                    Thread.Sleep(1000 * 5);
                }
                catch (Exception e)
                {
                    var msg = e.Message;
                    System.Diagnostics.Debug.WriteLine(msg);
                    Thread.Sleep(1000);
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
                    var ie = e.InnerException;
                    var msg = ie.Message;
                    System.Diagnostics.Debug.WriteLine(msg);

                    if (msg.Contains("Internal Server Error"))
                    {
                        break;
                    }

                    Thread.Sleep(1000 * 10);
                }
                catch (Exception e)
                {
                    var msg = e.Message;
                    System.Diagnostics.Debug.WriteLine(msg);
                    Thread.Sleep(1000);
                }
            }

            return null;
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
