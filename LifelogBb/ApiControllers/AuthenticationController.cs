using LifelogBb.Models.Account;
using LifelogBb.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Westwind.AspNetCore.Security;

namespace LifelogBb.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public AuthenticationController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpPost]
        [AllowAnonymous]
        // Not an MCP tool. The MCP connection is already authenticated, so exposing a password taking
        // login tool there has no purpose.
        // [ValidateAntiForgeryToken] // No validation as we use this from Swagger/API as well
        public IActionResult Authenticate([FromBody] LoginModel loginModel)
        {
            if (loginModel == null)
            {
                return BadRequest();
            }

            var configPassword = Configuration["Account:Password"];

            if (BCrypt.Net.BCrypt.Verify(loginModel.Password, configPassword))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Default user"),
                    new Claim(ClaimTypes.Role, "Administrator"),
                };

                var token = JwtHelper.GetJwtToken(
                    "Default user",
                    Configuration.GetRequired("Authentication:JwtToken:SigningKey"),
                    Configuration.GetRequired("Authentication:JwtToken:Issuer"),
                    Configuration.GetRequired("Authentication:JwtToken:Audience"),
                    TimeSpan.FromMinutes(Configuration.GetRequiredDouble("Authentication:JwtToken:TokenTimeoutMinutes")),
                    claims.ToArray()
                );

                // return the token to API client
                return new JsonResult(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expires = token.ValidTo,
                    displayName = "Default user"
                });
            }

            return BadRequest();
        }
    }
}
