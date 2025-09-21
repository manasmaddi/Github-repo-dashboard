namespace github_dashboard.Data;


public record ContributorInfo(
    string Login,
    string AvatarUrl,
    string HtmlUrl,
    int Contributions
);

