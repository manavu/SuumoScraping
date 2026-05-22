namespace SuumoScraping.UseCases
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using SuumoScraping.Domain.Gateways;

    public class GetFileDataUseCase
    {
        private readonly IScrapingContextFactory _scrapingContextFactory;

        public GetFileDataUseCase(IScrapingContextFactory scrapingContextFactory)
        {
            this._scrapingContextFactory = scrapingContextFactory;
        }

        public async Task<(byte[] FileData, string ContentType)?> ExecuteAsync(
            int id,
            CancellationToken cancellationToken = default
        )
        {
            using (var db = this._scrapingContextFactory.Create())
            {
                var query = db
                    .Files.Where(m => m.Id == id)
                    .Select(m => new { m.FileData, m.ContentType });
                var file = await db.SingleOrDefaultAsync(query, cancellationToken)
                    .ConfigureAwait(false);

                if (file == null)
                {
                    return null;
                }

                return (file.FileData, file.ContentType);
            }
        }
    }
}
