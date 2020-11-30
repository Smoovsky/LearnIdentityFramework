using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IdentityServer.MvcClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using IdentityModel.Client;

namespace IdentityServer.MvcClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(
            ILogger<HomeController> logger,
            IHttpClientFactory fac)
        {
            _logger = logger;
            _httpClientFactory = fac;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            var idToken = HttpContext.GetTokenAsync("id_token").Result;
            var accessToken = HttpContext.GetTokenAsync("access_token").Result;
            var refressToken = HttpContext.GetTokenAsync("refresh_token").Result;

            var idTokenContent = new JwtSecurityTokenHandler().ReadJwtToken(idToken);
            var accessTokenContent = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            // var idTokenContent = new JwtSecurityTokenHandler().ReadJwtToken(idToken);

            var user = HttpContext.User;

            var result = GetSecret(accessToken);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public string GetSecret(string accessToken)
        {
            var client = _httpClientFactory.CreateClient();

            // client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            client.SetBearerToken(accessToken);

            var res = client
                .GetAsync("https://localhost:7001/secret") //api1
                .Result;

            var content = res
                .Content
                .ReadAsStringAsync()
                .Result;

            RefreshAccessToken().Wait();

            return content;
        }

        public async Task RefreshAccessToken()
        {
            var serverClient = _httpClientFactory
                .CreateClient();

            var doc = await serverClient
                .GetDiscoveryDocumentAsync(
                    "https://localhost:6001");

            var refreshToken = await HttpContext.GetTokenAsync("refresh_token"); // returned from initial login

            var refreshTokenClient = _httpClientFactory.CreateClient();

            var tokenResponse = await refreshTokenClient.RequestRefreshTokenAsync(
                new RefreshTokenRequest
                {
                    Address = doc.TokenEndpoint,
                    RefreshToken = refreshToken,
                    ClientId = "clientMvc",
                    ClientSecret = "clientMvcSecret"
                }
            );

            var authInfo = await HttpContext
                .AuthenticateAsync("Cookie");

            authInfo
            .Properties
            .UpdateTokenValue("access_token", tokenResponse.AccessToken);
            authInfo
            .Properties.UpdateTokenValue("id_token", tokenResponse.IdentityToken);

            authInfo
            .Properties
            .UpdateTokenValue("refresh_token", tokenResponse.RefreshToken);

            await HttpContext
            .SignInAsync(
                "Cookie",
                authInfo.Principal,
                authInfo.Properties);
        }
    }
}
