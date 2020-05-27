using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OAuthServer.Models;

namespace OAuthServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // [Authorize(AuthenticationSchemes = "OAuth")] // working
        [Authorize] // not working
        public IActionResult Secret()
        {
            return Ok();
        }

        public IActionResult Decode(string jwt)
        {
            var bytes = Convert.FromBase64String(jwt);

            return Ok(System.Text.Encoding.UTF8.GetString(bytes));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Authenticate()
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "sampleId"),
                new Claim("SampleClaimType", "sampleValue")
            };

            var signingCred = new SigningCredentials(
                new SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(Constants.Secret)),
                    SecurityAlgorithms.HmacSha256
            );

            var jwt = new JwtSecurityToken(
                Constants.Issuer,
                Constants.Audiance,
                claims,
                notBefore: DateTime.Now, // can be one of the claims, check JwtRegisteredClaimNames.Nbf out. Declares when the JWT starts to be valid
                expires: DateTime.Now.AddDays(1),
                signingCredentials: signingCred
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return Ok(new { token });
        }

        [Authorize]
        public IActionResult ValidateToken()
        {
            if (HttpContext.Request.Query.TryGetValue("jwt", out var token))
            {
                return Ok();
            }


            return BadRequest();
        }
    }
}
