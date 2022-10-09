using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LifelogBb.Models.Account;

namespace LifelogBb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration Configuration;

        public AccountController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Password")] LoginModel loginModel)
        {
            // TODO Add documentation hint to secure password instead of using appsettings.json file
            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-6.0
            // https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows
            var configPassword = Configuration["Account:Password"];

            if (BCrypt.Net.BCrypt.Verify(loginModel.Password, configPassword))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Default user"),
                    new Claim(ClaimTypes.Role, "Administrator"),
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    RedirectUri = "/Index",
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                return RedirectToAction(actionName: "Index", controllerName: "Home");
            }

            // Message = "Invalid password.";

            return View(loginModel);
        }
    }
}
