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

builder.Services.AddScoped<GitHubService>();

// --- AUTHENTICATION SETUP ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "GitHub"; 
})
    .AddCookie() // Adds cookie-based auth
    .AddGitHub("GitHub", options => 
    {
        options.ClientId = builder.Configuration["GitHub:ClientId"]!;
        options.ClientSecret = builder.Configuration["GitHub:ClientSecret"]!;
        options.Scope.Add("repo");

 
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


app.UseAuthentication(); 
app.UseAuthorization(); 


app.MapControllers(); 
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

