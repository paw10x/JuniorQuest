namespace JuniorQuest.OfferIngestion;

public enum WorkplaceType { OnSite, Hybrid, Remote }
public enum EmploymentType { Internship, Permanent, Temporary }
public enum WorkSchedule { FullTime, PartTime }
public enum ContractType { EmploymentContract, B2B, MandateContract, SpecificTaskContract, Other }
public enum SeniorityLevel { Intern, Junior, Mid, Senior, Lead, Principal, Manager, Director, Executive }
public enum OfferStatus { Unknown, Open, Closed }
public enum CompensationBasis { Gross, Net }

public sealed record Compensation(
    string? Currency,
    string? Interval,
    decimal? Minimum,
    decimal? Maximum)
{
    public CompensationBasis? Basis { get; init; }
}

public sealed record JobOffer(
    string Id,
    string Title,
    string Company,
    string PostingUrl,
    string Description,
    DateTimeOffset FirstSeenAt,
    DateTimeOffset LastSeenAt)
{
    public string? ExternalId { get; init; }
    public string? ApplicationUrl { get; init; }
    public IReadOnlyList<string> Locations { get; init; } = [];
    public WorkplaceType? WorkplaceType { get; init; }
    public EmploymentType? EmploymentType { get; init; }
    public WorkSchedule? WorkSchedule { get; init; }
    public IReadOnlyList<ContractType> ContractTypes { get; init; } = [];
    public SeniorityLevel? SeniorityLevel { get; init; }
    public OfferStatus Status { get; init; } = OfferStatus.Unknown;
    public string? Department { get; init; }
    public string? PostingLanguage { get; init; }
    public IReadOnlyList<string> RequiredLanguages { get; init; } = [];
    public IReadOnlyList<string> Requirements { get; init; } = [];
    public IReadOnlyList<string> RequiredSkills { get; init; } = [];
    public IReadOnlyList<string> PreferredSkills { get; init; } = [];
    public Compensation? Compensation { get; init; }
    public DateTimeOffset? PublishedAt { get; init; }
    public DateTimeOffset? SourceUpdatedAt { get; init; }
    public DateTimeOffset? ApplicationDeadline { get; init; }
}
