namespace JuniorQuest.OfferIngestion;

// Implement with the web-search agent once its tools and configuration are chosen.
public interface IOfferDiscovery
{
    Task<IReadOnlyList<JobOffer>> FindAsync(CancellationToken cancellationToken = default);
}
