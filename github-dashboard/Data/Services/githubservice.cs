using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using github_dashboard.Data;
using Microsoft.AspNetCore.Authentication;
using Octokit;

namespace github_dashboard.Services;

public class GitHubService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GitHubService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IEnumerable<Repository>> GetRepositoriesAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null || !httpContext.User.Identity.IsAuthenticated)
        {
            return Enumerable.Empty<Repository>();
        }

        // 1. Get the access token saved during login
        var accessToken = await httpContext.GetTokenAsync("access_token");

        if (string.IsNullOrEmpty(accessToken))
        {
            return Enumerable.Empty<Repository>();
        }

        // 2. Create an authenticated Octokit client
        var github = new GitHubClient(new ProductHeaderValue("github-dashboard"))
        {
            Credentials = new Credentials(accessToken)
        };

        // 3. Fetch all repositories for the current user
        var octokitRepos = await github.Repository.GetAllForCurrent();

        // 4. Map the data to our simplified Repository model
        return octokitRepos.Select(r => new Repository
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
    }
}
