using Microsoft.AspNetCore.Authentication.Cookies;
using github_dashboard.Data; // Using the namespace from your file

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers(); // Needed for the AuthController

// 1. Add Authentication services
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "GitHub";
})
    .AddCookie()
    .AddGitHub("GitHub", options =>
    {
        options.ClientId = builder.Configuration["GitHub:ClientId"];
        options.ClientSecret = builder.Configuration["GitHub:ClientSecret"];
        options.CallbackPath = "/signin-github";
        options.Scope.Add("repo");
        options.SaveTokens = true;
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

// 2. Add Authentication and Authorization middleware
// IMPORTANT: This must come AFTER UseRouting() and BEFORE MapBlazorHub()
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); // This maps our AuthController
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

