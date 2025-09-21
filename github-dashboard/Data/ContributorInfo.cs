namespace github_dashboard.Data;

/// <summary>
/// Represents a repository contributor.
/// Using a record with a primary constructor is the modern C# standard for immutable data models.
/// </summary>
public record ContributorInfo(
    string Login,
    string AvatarUrl,
    string HtmlUrl,
    int Contributions
);

