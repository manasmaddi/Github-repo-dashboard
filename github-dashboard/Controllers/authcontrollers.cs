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
        
       return Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, "GitHub");
    }

    [HttpGet("/logout")]
    public async Task<IActionResult> Logout()
    {
        // This signs the user 
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return LocalRedirect("/");
    }
}

