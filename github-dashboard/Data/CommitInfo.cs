namespace github_dashboard.Data;

public record CommitInfo(
    string Sha,
    string Message,
    string Author,
    DateTimeOffset Timestamp
);

