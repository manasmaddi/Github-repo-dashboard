namespace github_dashboard.Data;

/// <summary>
/// Represents essential information about a single commit.
/// Using a record with a primary constructor is the modern C# standard for immutable data models.
/// </summary>
public record CommitInfo(
    string Sha,
    string Message,
    string Author,
    DateTimeOffset Timestamp
);

