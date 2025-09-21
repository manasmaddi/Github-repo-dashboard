using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace github_dashboard.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    [HttpGet("/login")]
    public IActionResult Login(string returnUrl = "/")
    {
        // This initiates the GitHub OAuth flow. The scheme name "GitHub" must match
        // the one used in Program.cs
        return Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, "GitHub");
    }

    [HttpGet("/logout")]
    public async Task<IActionResult> Logout()
    {
        // This signs the user out of the cookie authentication scheme.
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        // Redirect back to the home page after logout.
        return LocalRedirect("/");
    }
}

