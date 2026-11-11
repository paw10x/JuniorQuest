# Offer ingestion

This feature will use an agent to find job postings on the web, extract them into
`JobOffer` records, and save them in the central Firebase database. Offers are
shared across users; discovery does not depend on an individual user's profile.

## Current scope

- `JobOffer.cs`: the shared offer model, including posting URL, title,
  company, description, locations, requirements, optional compensation and timestamps.
- `IOfferDiscovery.cs`: the contract for the agent that discovers offers on the web.
- `IOfferStore.cs`: the contract for saving offers without inserting duplicates
  when the same offer ID is processed again.

This is a class library, with no runnable ingestion command yet. Web discovery
and Firebase persistence are not implemented. Firebase configuration and credentials
will be supplied separately; credentials must stay outside the repository.

## Next steps

After Firebase is configured, implement `IOfferStore` for the selected Firebase
database. Implement `IOfferDiscovery` with the agent's web tools, then connect
discovery to storage. Choose stable offer IDs and deduplication rules at that point.
`PostingUrl` preserves the original listing URL and identifies the source website.
`ExternalId` optionally preserves the posting ID assigned by that website.

Unavailable optional values remain null and unavailable lists remain empty.
Timestamps use `DateTimeOffset` (nullable when unknown). The Firebase adapter will
handle conversion to database timestamps and normalize values to UTC. Preserve
`FirstSeenAt` on updates and refresh `LastSeenAt` after successful verification.

Employment classification is split into `EmploymentType` (internship, permanent,
temporary), `WorkSchedule` (full-time, part-time), and `ContractTypes` (employment
contract, B2B, etc.). These fields, workplace type and seniority use enums; unknown
scalar classifications remain null. `Status` defaults to `Unknown` until verified.
An unsuccessful fetch alone does not establish that an offer is closed.

`Requirements` preserves extracted requirement text; `RequiredSkills` and
`PreferredSkills` contain normalized skill names for matching. `RequiredLanguages`
contains candidate language requirements, while `PostingLanguage` identifies the
language of the listing. Compensation includes an optional gross/net `Basis`.
`ApplicationDeadline` is populated only when supplied by the source. User profiles, match scores and
application status belong outside the central offer records.
