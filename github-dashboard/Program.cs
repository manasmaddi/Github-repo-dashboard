using github_dashboard.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

// Register the GitHubService (assuming it's in the Data namespace)
builder.Services.AddScoped<GitHubService>();

// --- AUTHENTICATION SETUP ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "GitHub"; // The default scheme for challenges is GitHub
})
    .AddCookie() // Adds cookie-based authentication
    .AddGitHub("GitHub", options => // Adds GitHub as an OAuth provider
    {
        options.ClientId = builder.Configuration["GitHub:ClientId"]!;
        options.ClientSecret = builder.Configuration["GitHub:ClientSecret"]!;
        options.Scope.Add("repo"); // Request permission to read repositories

        // This part is important: it saves the access token so we can use it later
        options.Events = new OAuthEvents
        {
            OnCreatingTicket = async context =>
            {
                var tokens = context.Properties.GetTokens().ToList();
                tokens.Add(new AuthenticationToken
                {
                    Name = "access_token",
                    Value = context.AccessToken!
                });
                context.Properties.StoreTokens(tokens);
            }
        };
    });
// --- END AUTHENTICATION SETUP ---

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// --- MIDDLEWARE ORDER IS CRITICAL ---
app.UseAuthentication(); // 1. Who are you?
app.UseAuthorization();  // 2. Are you allowed to be here?
// --- END MIDDLEWARE ---

app.MapControllers(); // Needed for the AuthController
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

