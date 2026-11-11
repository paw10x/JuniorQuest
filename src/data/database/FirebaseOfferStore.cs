using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using JuniorQuest.OfferIngestion;

namespace JuniorQuest.Database;

// Use only in trusted agent processes. IAM access tokens must never ship to end users.
public sealed class FirebaseOfferStore : IOfferStore
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient http;
    private readonly Uri connector;
    private readonly Func<CancellationToken, Task<string>>? getAccessToken;

    public FirebaseOfferStore(HttpClient http, string projectId = "juniorquest",
        string location = "europe-central2", string serviceId = "juniorquest-service",
        string connectorId = "offers", Uri? emulator = null,
        Func<CancellationToken, Task<string>>? getAccessToken = null)
    {
        this.http = http;
        this.getAccessToken = getAccessToken;
        foreach (string segment in new[] { projectId, location, serviceId, connectorId })
            if (string.IsNullOrWhiteSpace(segment) || segment.Any(c => !char.IsAsciiLetterOrDigit(c) && c != '-'))
                throw new ArgumentException("Firebase resource IDs must contain only letters, numbers and hyphens.");
        if (emulator is not null && (!emulator.IsAbsoluteUri || !emulator.IsLoopback))
            throw new ArgumentException("The emulator must use a loopback URL.", nameof(emulator));
        if (emulator is null && getAccessToken is null)
            throw new ArgumentException("Supply a Google OAuth access-token provider for cloud access.");
        var origin = emulator ?? new Uri("https://firebasedataconnect.googleapis.com");
        connector = new Uri(origin, $"/v1/projects/{projectId}/locations/{location}/services/{serviceId}/connectors/{connectorId}");
    }

    public async Task UpsertManyAsync(IReadOnlyList<JobOffer> offers, CancellationToken cancellationToken = default)
    {
        // Each offer is atomic; successful earlier offers remain saved if a later request fails.
        foreach (JobOffer offer in offers)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(offer.Id);
            if (!Uri.TryCreate(offer.PostingUrl, UriKind.Absolute, out var url) ||
                url.Scheme is not ("https" or "http"))
                throw new ArgumentException("An offer must have an absolute HTTP(S) posting URL.");
            await ExecuteAsync("executeMutation", "UpsertOffer", offer, cancellationToken);
        }
    }

    public async Task<JobOffer?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var data = await ExecuteAsync("executeQuery", "GetOffer", new { id }, cancellationToken);
        return data.GetProperty("jobOffer").Deserialize<JobOffer>(Json);
    }

    public async Task<JobOffer?> FindByUrlAsync(string postingUrl, CancellationToken cancellationToken = default)
    {
        var data = await ExecuteAsync("executeQuery", "FindOfferByUrl", new { postingUrl }, cancellationToken);
        return data.GetProperty("jobOffers").Deserialize<JobOffer[]>(Json)!.FirstOrDefault();
    }

    public async Task<IReadOnlyList<JobOffer>> ListAsync(int limit = 100, int offset = 0,
        CancellationToken cancellationToken = default)
    {
        if (limit is < 1 or > 1000 || offset < 0)
            throw new ArgumentOutOfRangeException(nameof(limit), "Use a limit of 1–1000 and a nonnegative offset.");
        var data = await ExecuteAsync("executeQuery", "ListOffers", new { limit, offset }, cancellationToken);
        return data.GetProperty("jobOffers").Deserialize<JobOffer[]>(Json)!;
    }

    private async Task<JsonElement> ExecuteAsync(string method, string operationName, object variables,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{connector}:{method}")
        {
            Content = JsonContent.Create(new { operationName, variables }, options: Json)
        };
        if (getAccessToken is not null)
        {
            string token = await getAccessToken(cancellationToken);
            ArgumentException.ThrowIfNullOrWhiteSpace(token);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        using var response = await http.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        using var document = await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
        var root = document.RootElement;
        if (root.TryGetProperty("errors", out var errors) && errors.GetArrayLength() > 0)
            throw new InvalidOperationException($"Firebase operation {operationName} failed: {errors}");
        return root.GetProperty("data").Clone();
    }
}
