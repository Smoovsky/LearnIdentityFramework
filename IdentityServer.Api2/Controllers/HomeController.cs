using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IdentityServer.Api2.Models;
using System.Net.Http;
using IdentityModel.Client;

namespace IdentityServer.Api2.Controllers
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
            // retrieve access token
            var serverClient = _httpClientFactory.CreateClient();

            var discoveryDocument =
                serverClient
                .GetDiscoveryDocumentAsync("https://localhost:6001/")
                .Result;

            var tokenRes = serverClient
            .RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest()
                {
                    Address = discoveryDocument.TokenEndpoint,
                    ClientId = "client1",
                    ClientSecret = "client1secret",
                    Scope = "ApiOne"
                }
            )
            .Result;

            var apiClient = _httpClientFactory
                            .CreateClient();

            apiClient.SetBearerToken(tokenRes.AccessToken);

            var apiOneResponse = apiClient
                        .GetAsync("https://localhost:7001/secret")
                        .Result;

            return Ok(new
            {
                access_token = tokenRes.AccessToken,
                message = apiOneResponse.Content.ReadAsStringAsync().Result
            });
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
    }
}
