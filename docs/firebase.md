# Firebase SQL Connect

The shared offers database uses the existing `juniorquest` Firebase project,
`juniorquest-service` in `europe-central2`, and connector `offers`. The existing
Cloud SQL configuration is preserved. No browser app registration or Firebase
web API key is needed by the C# agents.

## Files

- `dataconnect/schema/schema.gql` maps the current `JobOffer` model to PostgreSQL.
- `dataconnect/offers/` defines lookup by ID/URL, paginated listing, and upsert.
- `src/data/database/FirebaseOfferStore.cs` implements `IOfferStore` over REST.

IDs must be stable across discovery runs. Posting URLs are also unique; look up
the URL before assigning an ID to an already known offer. Upsert is transactional
per offer and preserves the stored `DiscoveredAt`. A batch is not one transaction;
retrying the same IDs is safe. Canonicalize source URLs before saving them.
Classification values use the C# enum names as strings. Compensation is JSON,
preserving the current nullable structure and decimal amounts. Timestamps map to
PostgreSQL `timestamptz`. User profiles and compatibility scores are not stored here.

## Run locally

From the repository root, with Node.js and .NET 10 installed:

```sh
npx --yes firebase-tools@latest dataconnect:compile
npx --yes firebase-tools@latest emulators:start --only dataconnect
```

In a second terminal:

```sh
FIREBASE_DATACONNECT_EMULATOR_HOST=127.0.0.1:9399 dotnet run --project src/ui/dashboard -- --check-database
```

In Rider, add `--check-database` to the Dashboard run configuration's program
arguments and set `FIREBASE_DATACONNECT_EMULATOR_HOST=127.0.0.1:9399` in its environment
variables. Normal dashboard mode remains available without database credentials.

## Deploy and check the cloud connection

Sign in with an account that can manage this Firebase project. Confirm that its
SQL Connect service, billing and Cloud SQL instance are configured, then run:

```sh
npx --yes firebase-tools@latest login
npx --yes firebase-tools@latest dataconnect:compile
npx --yes firebase-tools@latest dataconnect:sql:diff --project juniorquest
npx --yes firebase-tools@latest deploy --only dataconnect --project juniorquest
```

Review the schema diff before proceeding with deployment. These commands may
provision billable Cloud SQL resources if the configured instance does not exist.

For a local cloud check, sign in to the Google Cloud CLI using an IAM identity
allowed to execute SQL Connect operations, then run:

```sh
gcloud auth login
export GOOGLE_OAUTH_ACCESS_TOKEN="$(gcloud auth print-access-token)"
dotnet run --project src/ui/dashboard -- --check-database
```

Unset `FIREBASE_DATACONNECT_EMULATOR_HOST` when checking the cloud. Access tokens
expire; regenerate the token for subsequent checks. Firebase CLI login and Google
Cloud OAuth login are separate. `firebase login` alone does not authenticate C#.

## Connect an agent

Reference the `Database` project and supply a reusable `HttpClient`:

```csharp
using JuniorQuest.Database;

var store = new FirebaseOfferStore(httpClient,
    emulator: new Uri("http://127.0.0.1:9399"));
await store.UpsertManyAsync(offers, cancellationToken);
var existing = await store.FindByUrlAsync(postingUrl, cancellationToken);
var page = await store.ListAsync(limit: 100, offset: 0,
    cancellationToken: cancellationToken);
```

For cloud agents, omit `emulator` and inject `getAccessToken`, a callback that
returns a current Google OAuth token scoped to
`https://www.googleapis.com/auth/cloud-platform`. Use Application Default
Credentials or workload identity in the eventual agent host so tokens refresh
automatically. The dashboard environment-token option is a development check,
not a production credential lifecycle implementation.

All connector operations use `NO_ACCESS`, blocking Firebase client access. Only
trusted Google Cloud IAM callers should execute them. Keep agent credentials
outside the repository and never distribute privileged credentials in a user app.
The matcher should receive query-only IAM permissions; the ingestion agent needs
query and mutation execution permissions.

REST reference:
https://firebase.google.com/docs/reference/sql-connect/rest/v1/projects.locations.services.connectors/executeQuery

## Validation status

The .NET solution builds with zero warnings and errors. HTTP adapter smoke checks
passed for endpoint construction, offer serialization, decimal/enum round trips,
and GraphQL error handling. The dashboard check reports missing credentials clearly.

Cloud deployment and a real database round trip must be checked in an environment
with Firebase network access. The initial setup environment could not download
the SQL Connect compiler/emulator, so GraphQL compilation remains unverified.
