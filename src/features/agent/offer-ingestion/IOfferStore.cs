namespace JuniorQuest.OfferIngestion;

// Firebase SQL Connect implementation lives in src/data/database.
public interface IOfferStore
{
    Task UpsertManyAsync(
        IReadOnlyList<JobOffer> offers, CancellationToken cancellationToken = default);
}
