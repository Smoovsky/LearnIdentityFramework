using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SimpleAuth.Controllers
{
    public class HomeController : Controller
    {
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

        public async Task<IActionResult> Authenticate()
        {
            var customClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Shaun"),
                new Claim(ClaimTypes.Email, "abc@abc.com"),
                new Claim("CustomClaim", "CustomVal"),
                new Claim(ClaimTypes.DateOfBirth, "2000-Jan-01"),
                new Claim(ClaimTypes.DateOfBirth, "2000-Jan-01"),
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