using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace github_dashboard.Controllers;

// This controller will handle the login and logout processes
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpGet("login")]
    public IActionResult Login(string returnUrl = "/")
    {
        // This initiates the GitHub login process by challenging the GitHub authentication scheme.
        // The user will be redirected to GitHub to authorize the application.
        return Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, "GitHub");
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        // This signs the user out of the cookie authentication scheme.
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        // Redirect the user back to the homepage after logging out.
        return Redirect("/");
    }
}
