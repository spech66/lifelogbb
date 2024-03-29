﻿using LifelogBb.Models.Account;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Westwind.AspNetCore.Security;
using Microsoft.AspNetCore.Authorization;

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
                    Configuration["Authentication:JwtToken:SigningKey"],
                    Configuration["Authentication:JwtToken:Issuer"],
                    Configuration["Authentication:JwtToken:Audience"],
                    TimeSpan.FromMinutes(double.Parse(Configuration["Authentication:JwtToken:TokenTimeoutMinutes"])),
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
