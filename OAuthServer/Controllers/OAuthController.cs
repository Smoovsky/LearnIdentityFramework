using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OAuthServer.Models;

namespace OAuthServer.Controllers
{
    public class OAuthController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public OAuthController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Authorize(
            string response_type, // auth flow type
            string client_id,
            string redirect_uri,
            string scope, // what info is required
            string state // identifier to mark client's session
        )
        {
            var qb = new QueryBuilder();
            qb.Add("redirectUri", redirect_uri);
            qb.Add("state", state);

            return View(model: qb.ToString());
        }

        [HttpPost]
        public IActionResult Authorize(
            string userName,
            string redirect_uri,
            string state
        )
        {
            const string authCode = "dummpAuthCode";

            var qb = new QueryBuilder();
            qb.Add("code", authCode);
            qb.Add("state", state);

            return Redirect($"{redirect_uri}{qb}");
        }

        public IActionResult Token(
            string grant_type, // current flow
            string code,
            string redirect_uri,
            string client_id
        )
        {
            // mechanism to validate code

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

            var response = new
            {
                access_token = token,
                token_type = "Bearer",
                raw_claim = "oauthExample"
            };

            return Ok(response);
        }

        [Authorize]
        public IActionResult Validate()
        {
            if (HttpContext.Request
                .Query
                .TryGetValue("access_token", out var token))
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}
