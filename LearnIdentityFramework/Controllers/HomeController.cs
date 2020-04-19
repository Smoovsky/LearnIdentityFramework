using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnIdentityFramework.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Secret()
        {
            return View();
        }

        public async Task<IActionResult> Authenticate()
        {
            var customClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Shaun"),
                new Claim(ClaimTypes.Email, "abc@abc.com"),
                new Claim("CustomClaim", "CustomVal")
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
    }
}