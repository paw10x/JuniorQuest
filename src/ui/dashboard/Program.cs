using System.Globalization;
using JuniorQuest.Database;

if (args.Length == 1 && args[0] == "--check-database")
{
    try
    {
        string? emulator = Environment.GetEnvironmentVariable("FIREBASE_DATACONNECT_EMULATOR_HOST");
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        var store = new FirebaseOfferStore(http,
            emulator: emulator is null ? null : new Uri($"http://{emulator}"),
            getAccessToken: emulator is not null ? null : _ => Task.FromResult(
                Environment.GetEnvironmentVariable("GOOGLE_OAUTH_ACCESS_TOKEN")
                ?? throw new InvalidOperationException("Set GOOGLE_OAUTH_ACCESS_TOKEN to a Google Cloud access token, or set FIREBASE_DATACONNECT_EMULATOR_HOST=127.0.0.1:9399.")));
        var offers = await store.ListAsync(limit: 1);
        Console.WriteLine($"Firebase SQL Connect is reachable. {offers.Count} offer(s) returned (limit 1).");
        return 0;
    }
    catch (Exception exception)
    {
        Console.Error.WriteLine($"Database check failed: {exception.Message}");
        return 1;
    }
}

if (args.Length == 1 && args[0] is "-h" or "--help")
{
    Console.WriteLine("Usage: Dashboard [--demo | --check-database]");
    Console.WriteLine("  --demo  Show sample application status updates.");
    Console.WriteLine("  --check-database  Check the Firebase SQL Connect connection.");
    return 0;
}

bool demo = args.Length == 1 && args[0] == "--demo";
if (args.Length > 0 && !demo)
{
    Console.Error.WriteLine("Usage: Dashboard [--demo | --check-database]");
    return 2;
}

string[] actions =
[
    "Browse matched offers",
    "Update your profile",
    "Tailor a CV for an offer",
    "Track your applications"
];

StatusUpdate[] updates = [];
if (demo)
{
    Console.WriteLine("Demo mode | Sample data only");
    Console.WriteLine();
    DateTimeOffset now = DateTimeOffset.UtcNow;
    updates =
    [
        new("Example Labs", "Software Engineering Intern", "Application sent", now.AddHours(-1)),
        new("Example Systems", "Infrastructure Intern", "Interview invitation", now.AddDays(-1))
    ];
}

ShowDashboard(actions, updates);
while (true)
{
    Console.Write("\nChoose an action: ");
    string? choice = Console.ReadLine()?.Trim();
    if (choice is null)
    {
        Console.WriteLine();
        return 0;
    }

    if (choice == "0")
        return 0;

    if (int.TryParse(choice, NumberStyles.None, CultureInfo.InvariantCulture, out int number)
        && number >= 1 && number <= actions.Length)
    {
        Console.WriteLine($"{actions[number - 1]} will be available once the feature is connected.");
    }
    else
    {
        Console.WriteLine($"Enter a number from 0 to {actions.Length}.");
    }
}

static void ShowDashboard(IReadOnlyList<string> actions, IEnumerable<StatusUpdate> updates)
{
    const int recentUpdateLimit = 5;

    Console.WriteLine("JuniorQuest | Dashboard");
    Console.WriteLine();
    Console.WriteLine("Your actions");
    for (int index = 0; index < actions.Count; index++)
        Console.WriteLine($"  {index + 1}. {actions[index]}");

    Console.WriteLine("  0. Exit");
    Console.WriteLine();
    Console.WriteLine("Recent application status updates");

    StatusUpdate[] recent = updates.OrderByDescending(update => update.UpdatedAt)
        .Take(recentUpdateLimit)
        .ToArray();

    if (recent.Length == 0)
        Console.WriteLine("  No application status updates yet.");

    foreach (StatusUpdate update in recent)
    {
        string timestamp = update.UpdatedAt.ToLocalTime()
            .ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        Console.WriteLine($"  {timestamp} | {update.Status} | {update.Company} — {update.Role}");
    }
}

internal sealed record StatusUpdate(string Company, string Role, string Status, DateTimeOffset UpdatedAt);
