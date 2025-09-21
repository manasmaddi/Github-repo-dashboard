namespace github_dashboard.Data;


public record RepositoryDetail(
    long Id,
    string Name,
    string Description,
    string Url,
    List<CommitInfo> RecentCommits,
    List<ContributorInfo> Contributors
);

