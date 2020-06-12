using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OAuthClient.Models;

namespace OAuthClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(
            ILogger<HomeController> logger,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            var token = HttpContext.GetTokenAsync("access_token").Result; // stored in cookie

            var claims = HttpContext.User.Claims; // oauth claims persisted by configuring behavior of OnTickCreated

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public IActionResult ServerSecretTest()
        {
            var serverRes = AccessTokenRefreshWrapper(() => SecuredGet("https://localhost:5001/secret/Secret")).Result;

            var apiRes = AccessTokenRefreshWrapper(() => SecuredGet("https://localhost:5004/secret/index")).Result;

            // var token = HttpContext.GetTokenAsync("access_token").Result;

            // var serverClient = _httpClientFactory.CreateClient();

            // serverClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            // var response = serverClient.GetAsync("https://localhost:5001/secret/Secret").Result;

            // RefreshAccessToken().Wait();

            // token = HttpContext.GetTokenAsync("access_token").Result;

            // var apiClient = _httpClientFactory.CreateClient();

            // apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            // var apiResponse = apiClient.GetAsync("https://localhost:5004/secret/index").Result;

            return Ok();
        }

        private async Task<HttpResponseMessage> SecuredGet(
            string url
        )
        {
            var token = HttpContext.GetTokenAsync("access_token").Result;

            var client = _httpClientFactory.CreateClient();

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            return await client.GetAsync(url);
        }

        public async Task<HttpResponseMessage> AccessTokenRefreshWrapper(
            Func<Task<HttpResponseMessage>> inputRequest
        )
        {
            var intiRes = await inputRequest();

            if (intiRes.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshAccessToken();
                intiRes = await inputRequest();
            }

            return intiRes;
        }

        public async Task RefreshAccessToken()
        {
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token"); // returned from initial login

            var refreshTokenClient = _httpClientFactory.CreateClient();

            var data = new Dictionary<string, string>()
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refreshToken
            };

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://localhost:5001/oauth/token")
            {
                Content = new FormUrlEncodedContent(data)
            };

            var basicCredentials = "username:password"; // fake one to satisfy documentation
            var encodedCred = Convert.ToBase64String(
                        System.Text.Encoding.UTF8
                        .GetBytes(basicCredentials));

            request.Headers.Add("Authorization", $"Basic {encodedCred}");

            var res = refreshTokenClient.SendAsync(request).Result;

            var resDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(res.Content.ReadAsStringAsync().Result);

            var newAccessToken = resDic["access_token"];
            var newRefreshToken = resDic["refresh_token"];

            var authInfo = await HttpContext.AuthenticateAsync("CookieScheme");

            authInfo
            .Properties
            .UpdateTokenValue("access_token", newAccessToken);

            authInfo
            .Properties
            .UpdateTokenValue("refresh_token", newRefreshToken);

            await HttpContext
            .SignInAsync(
                "CookieScheme",
                authInfo.Principal,
                authInfo.Properties);
        }
    }
}
