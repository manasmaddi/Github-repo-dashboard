using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using github_dashboard.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Octokit;
using ILogger = Microsoft.Extensions.Logging.ILogger; 

namespace github_dashboard.Services;


public class GitHubService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMemoryCache _memoryCache;
     private readonly ILogger<GitHubService> _logger;

    public GitHubService(IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache /*, ILogger<GitHubService> logger */)
    {
        _httpContextAccessor = httpContextAccessor;
        _memoryCache = memoryCache;
         _logger = logger; //

    }

    /// <summary>
    /// Public method to orchestrate fetching repositories for the authenticated user.
    /// This method is the single entry point for the UI layer.
    /// </summary>
    public async Task<IEnumerable<Repository>> GetUserRepositoriesAsync()
    {
         _logger.LogInformation("Attempting to fetch user repositories.");

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User.Identity?.Name == null)
        {
            return Enumerable.Empty<Repository>();
        }

        // Use the user's name as part of the cache key to ensure data is user-specific.
        var cacheKey = $"repos_{httpContext.User.Identity.Name}";

        //  Try to get data from cache first.
        if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<Repository>? cachedRepos))
        {
             _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
            if (cachedRepos != null) return cachedRepos;
        }

         _logger.LogInformation("Cache miss for key: {CacheKey}. Fetching from API.", cacheKey);
        try
        {
            var accessToken = await GetUserAccessTokenAsync();
            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogWarning("User is authenticated but access token is missing.");
                return Enumerable.Empty<Repository>();
            }

            var githubClient = CreateGitHubClient(accessToken);

            var rawRepos = await FetchRepositoriesFromApiAsync(githubClient);

            _logger.LogInformation("Successfully fetched {RepoCount} repositories.", rawRepos.Count);
            var mappedRepos = MapToDomainModel(rawRepos).ToList();

            // Store the newly fetched data in the cache with an expiration time.
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5)); // Cache for 5 minutes

            _memoryCache.Set(cacheKey, mappedRepos, cacheEntryOptions);

            return mappedRepos;
        }
        catch (ApiException ex)
        {
             _logger.LogError(ex, "An error occurred while communicating with the GitHub API. Status: {StatusCode}", ex.StatusCode);
            return Enumerable.Empty<Repository>();
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "An unexpected error occurred in GetUserRepositoriesAsync.");
            return Enumerable.Empty<Repository>();
        }
    }

    /// <summary>
    /// Fetches the raw repository data from the GitHub API.
    /// </summary>
    private async Task<IReadOnlyList<Octokit.Repository>> FetchRepositoriesFromApiAsync(IGitHubClient client)
    {
        return await client.Repository.GetAllForCurrent();
    }

    /// <summary>
    /// Maps the detailed Octokit repository models to our simplified domain model.
    /// </summary>
    private IEnumerable<Repository> MapToDomainModel(IEnumerable<Octokit.Repository> octokitRepos)
    {
        return octokitRepos.Select(r => new Repository
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description ?? "No description.",
            Url = r.HtmlUrl,
            Stars = r.StargazersCount,
            Forks = r.ForksCount,
            OpenIssues = r.OpenIssuesCount,
            LastPush = r.PushedAt ?? r.UpdatedAt ?? DateTimeOffset.MinValue
        }).ToList();
    }

    /// <summary>
    /// Securely retrieves the user's GitHub access token from the authentication context.
    /// </summary>
    private async Task<string?> GetUserAccessTokenAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null || httpContext.User.Identity?.IsAuthenticated != true)
        {
             _logger.LogInformation("User is not authenticated.");
            return null;
        }

        
        return await httpContext.GetTokenAsync("access_token");
    }

    /// <summary>
    /// Creates and configures an authenticated GitHub API client.
    /// </summary>
    private IGitHubClient CreateGitHubClient(string token)
    {
        return new GitHubClient(new ProductHeaderValue("github-dashboard"))
        {
            Credentials = new Credentials(token)
        };
    }
}

