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

namespace IdentityExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly SignInManager<IdentityUser> _signInManager;

        public HomeController(
            ILogger<HomeController> logger,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
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

        [HttpPost]
        public IActionResult LogOut()
        {
            _signInManager.SignOutAsync().Wait();

            return RedirectToAction("Index");
        }
    }
}
