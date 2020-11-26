using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Controllers
{
    public class AuthController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public SignInManager<IdentityUser> _signInManager;
        public UserManager<IdentityUser> _userManager;

        public AuthController(
            ILogger<HomeController> logger,
            SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpGet]
        public IActionResult Register(string returnUrl)
        {
            return View(new RegisterViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel loginModel)
        {
            var result = _signInManager
            .PasswordSignInAsync(
                loginModel.UserName,
                loginModel.Password,
                false,
                false
            ).Result;

            if (result.Succeeded)
            {
                return Redirect(loginModel.ReturnUrl);
            }

            return View(loginModel);
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel registerModel)
        {
            if(!ModelState.IsValid)
            {
                return View(registerModel);
            }

            var user = new IdentityUser(
                registerModel.UserName);

            var result = _userManager
                .CreateAsync(user, registerModel.Password)
                .Result;

            if (result.Succeeded)
            {
                _signInManager
                .SignInAsync(user, false)
                .Wait();

                return Redirect(registerModel.ReturnUrl);
            }

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
    }
}
