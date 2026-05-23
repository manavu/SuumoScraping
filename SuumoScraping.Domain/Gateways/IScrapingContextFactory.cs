namespace SuumoScraping.Domain.Gateways
{
    public interface IScrapingContextFactory
    {
        IScrapingContext Create();
    }
}
