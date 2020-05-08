using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IdentityExample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using NETCore.MailKit.Core;

namespace IdentityExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailService _emailService;

        public HomeController(
            ILogger<HomeController> logger,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IEmailService emailService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Secret()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(
            string username,
            string password
        )
        {
            var user = _userManager.FindByNameAsync(username).Result;

            if (user != null)
            {
                var signInResult = _signInManager
                .PasswordSignInAsync(
                    username,
                    password,
                    false,
                    false
                ).Result;

                if (signInResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult VerifyEmail(string userId, string code)
        {
            var user = _userManager.FindByIdAsync(userId).Result;

            if (user == null) return BadRequest();

            var result = _userManager.ConfirmEmailAsync(user, code).Result;

            if (result.Succeeded) return View();

            return BadRequest();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(
            string username,
            string password
        )
        {
            var user = new IdentityUser()
            {
                UserName = username
            };

            var result = _userManager.CreateAsync(user, password).Result;

            if (result.Succeeded)
            {
                var code = _userManager
                    .GenerateEmailConfirmationTokenAsync(user)
                    .Result;

                var link = Url.Action(
                    nameof(VerifyEmail),
                    "Home",
                    new { userId = user.Id, code },
                    Request.Scheme,
                    Request.Host.ToString());

                _emailService.Send("baka@qq.com", "Email Verify", $"<a href=\"{link}\">Verify your email</a>", true);

                return RedirectToAction("EmailVerification");

                // var signInResult = _signInManager
                // .PasswordSignInAsync(
                //     username,
                //     password,
                //     false,
                //     false
                // ).Result;

                // if (signInResult.Succeeded)
                // {
                //     return RedirectToAction("Index");
                // }
            }

            return RedirectToAction("Index");
        }

        public IActionResult EmailVerification() => View();

        [HttpPost]
        public IActionResult LogOut()
        {
            _signInManager.SignOutAsync().Wait();

            return RedirectToAction("Index");
        }
    }
}
