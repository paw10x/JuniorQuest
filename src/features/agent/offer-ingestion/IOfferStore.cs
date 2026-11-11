namespace JuniorQuest.OfferIngestion;

// Implement with Firebase once the central database is configured.
public interface IOfferStore
{
    Task UpsertManyAsync(
        IReadOnlyList<JobOffer> offers, CancellationToken cancellationToken = default);
}
