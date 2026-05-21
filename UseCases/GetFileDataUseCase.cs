namespace SuumoScraping.UseCases
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using SuumoScraping.Models;

    public class GetFileDataUseCase
    {
        private readonly IScrapingContextFactory _scrapingContextFactory;

        public GetFileDataUseCase(IScrapingContextFactory scrapingContextFactory)
        {
            _scrapingContextFactory = scrapingContextFactory;
        }

        public async Task<(byte[] FileData, string ContentType)?> ExecuteAsync(
            int id,
            CancellationToken cancellationToken = default
        )
        {
            using (var db = _scrapingContextFactory.Create())
            {
                var file = await db
                    .Files.Where(m => m.Id == id)
                    .Select(m => new { m.FileData, m.ContentType })
                    .SingleOrDefaultAsync(cancellationToken);

                if (file == null)
                {
                    return null;
                }

                return (file.FileData, file.ContentType);
            }
        }
    }
}
