namespace SuumoScraping.Domain.Exceptions
{
    using System;

    public class SuumoScrapingException : Exception
    {
        public string Url { get; }

        public SuumoScrapingException(string message, string url, Exception innerException = null)
            : base($"{message} (URL: {url})", innerException)
        {
            this.Url = url;
        }
    }

    public class SuumoFetchException : SuumoScrapingException
    {
        public int? HttpStatusCode { get; }

        public SuumoFetchException(
            string message,
            string url,
            int? statusCode = null,
            Exception innerException = null
        )
            : base(message, url, innerException)
        {
            this.HttpStatusCode = statusCode;
        }
    }

    public class SuumoParseException : SuumoScrapingException
    {
        public string ElementName { get; }

        public string RawHtml { get; }

        public SuumoParseException(
            string message,
            string url,
            string elementName,
            string rawHtml = null,
            Exception innerException = null
        )
            : base($"{message} [Failed Item: {elementName}]", url, innerException)
        {
            this.ElementName = elementName;
            this.RawHtml = rawHtml;
        }
    }
}
