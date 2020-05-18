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
    public class SecretController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public SecretController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        public string Secret()
        {
            return "server secret";
        }
    }
}
