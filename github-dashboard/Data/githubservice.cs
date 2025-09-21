namespace github_dashboard.Data;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Octokit;

public class GitHubService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMemoryCache _memoryCache;

    
    public GitHubService(IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache)
    {
        _httpContextAccessor = httpContextAccessor;
        _memoryCache = memoryCache;
    }

    public async Task<IEnumerable<Repository>> GetUserRepositoriesAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User.Identity?.Name == null) return Enumerable.Empty<Repository>();

        var cacheKey = $"repos_{httpContext.User.Identity.Name}";

        if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<Repository>? cachedRepos))
        {
            if (cachedRepos != null) return cachedRepos;
        }

        try
        {
            var accessToken = await GetUserAccessTokenAsync();
            if (string.IsNullOrEmpty(accessToken)) return Enumerable.Empty<Repository>();

            var githubClient = CreateGitHubClient(accessToken);
            var rawRepos = await githubClient.Repository.GetAllForCurrent();

            var mappedRepos = rawRepos.Select(r => new Repository
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description ?? "No description.",
                Url = r.HtmlUrl,
                Stars = r.StargazersCount,
                Forks = r.ForksCount,
                OpenIssues = r.OpenIssuesCount,

                LastPush = r.PushedAt ?? r.UpdatedAt
            }).ToList();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            _memoryCache.Set(cacheKey, mappedRepos, cacheEntryOptions);
            return mappedRepos;
        }
        catch (ApiException)
        {
            // Log the exception here in a real app
            return Enumerable.Empty<Repository>();
        }
    }

    public async Task<RepositoryDetail?> GetRepositoryDetailsAsync(long repositoryId)
    {
        var accessToken = await GetUserAccessTokenAsync();
        if (string.IsNullOrEmpty(accessToken)) return null;

        var githubClient = CreateGitHubClient(accessToken);

        try
        {
            var repo = await githubClient.Repository.Get(repositoryId);
            var commits = await githubClient.Repository.Commit.GetAll(repositoryId, new ApiOptions { PageSize = 10 });
            var contributors = await githubClient.Repository.GetAllContributors(repositoryId);

            var mappedCommits = commits.Select(c => new CommitInfo(
                c.Sha.Substring(0, 7),
                c.Commit.Message,
                c.Commit.Author.Name,
                c.Commit.Author.Date
            )).ToList();

            var mappedContributors = contributors.Select(c => new ContributorInfo(
                c.Login,
                c.AvatarUrl,
                c.HtmlUrl,
                c.Contributions
            )).ToList();

            return new RepositoryDetail(
                repo.Id,
                repo.Name,
                repo.Description ?? "No description.",
                repo.HtmlUrl,
                mappedCommits,
                mappedContributors
            );
        }
        catch (NotFoundException)
        {
            // This happens if the repo ID is invalid or not accessible
            return null;
        }
    }

    private async Task<string?> GetUserAccessTokenAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null || httpContext.User.Identity?.IsAuthenticated != true)
        {
            return null;
        }
        return await httpContext.GetTokenAsync("access_token");
    }

    private IGitHubClient CreateGitHubClient(string token)
    {
        return new GitHubClient(new ProductHeaderValue("github-dashboard"))
        {
            Credentials = new Credentials(token)
        };
    }
}
