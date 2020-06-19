using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using SimpleAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SimpleAuth.Controllers
{
    public class SecurityLevelAuthAttribute : AuthorizeAttribute
    {
        public SecurityLevelAuthAttribute(
            int lv
        ) : base($"{DynamicPolicies.SecurityLevel}.{lv}")
        {
        }
    }

    public class HomeController : Controller
    {
        [SecurityLevelAuth(5)]
        public IActionResult SecurityLevel()
        {
            return Ok();
        }

        [SecurityLevelAuth(10)]
        public IActionResult SecurityLevelHigher()
        {
            return Ok();
        }

        private readonly IAuthorizationService _authorizationService;

        public HomeController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [Authorize(Policy = "Claim.DOB")]
        public async Task<IActionResult> Secret()
        {
            return View();
        }

        [Authorize(Roles = "Admin")] // legacy .net code, role is now abstracte as one claimtype
        public async Task<IActionResult> SecretRequireAdmin()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Manager")] // legacy .net code, role is now abstracte as one claimtype
        public async Task<IActionResult> SecretRequireAdminOrManageer()
        {
            return View();
        }

        [Authorize(Roles = "Manager")] // legacy .net code, role is now abstracte as one claimtype
        [Authorize(Roles = "Admin")] // legacy .net code, role is now abstracte as one claimtype
        public async Task<IActionResult> SecretRequireAdminAndManageer()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Authenticate()
        {
            var customClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Shaun"),
                new Claim(ClaimTypes.Email, "abc@abc.com"),
                new Claim("CustomClaim", "CustomVal"),
                new Claim(ClaimTypes.DateOfBirth, "2000-Jan-01"),
                new Claim(ClaimTypes.DateOfBirth, "2000-Jan-01"),
                new Claim(DynamicPolicies.SecurityLevel, "6"),
            };

            var officialClaims = new List<Claim>()
            {
                new Claim("License", "H123")
            };

            var customIdentity = new ClaimsIdentity(customClaims, "Basic");

            var officialIdentity = new ClaimsIdentity(officialClaims);

            var resultUserPrincipal = new ClaimsPrincipal(new[] { customIdentity, officialIdentity });

            await HttpContext.SignInAsync(resultUserPrincipal);

            return RedirectToAction("Index");
        }

        public IActionResult AuthorizationServiceSample()
        {
            var customPolicy = new AuthorizationPolicyBuilder() // default scheme
                                .RequireClaim("Sample")
                                .Build();

            var authResult = _authorizationService
                        .AuthorizeAsync(HttpContext.User, customPolicy)
                        .Result;

            if (authResult.Succeeded)
            {
                // do stuff
            }
            return View("Index");
        }

        public IActionResult AuthorizationServiceTest(
            [FromServices] IAuthorizationService authService
        )
        {
            var testPolicy = new AuthorizationPolicyBuilder("TestPolicy")
                            .RequireClaim("233")
                            .Build();

            var authResult = authService.AuthorizeAsync(User, testPolicy).Result;

            if (authResult.Succeeded)
            {
                // do stuff
            }

            return Ok();
        }
    }
}