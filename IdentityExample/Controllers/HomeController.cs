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

        public HomeController(
            ILogger<HomeController> logger,
            UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
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

        public IActionResult Register(
            string username,
            string password
        )
        {
            return View();
        }

        public IActionResult Login(
            string username,
            string password
        )
        {
            return RedirectToAction("Index");
        }

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

            if(result.Succeeded)
            {
                // EP2 20:00
            }

            return RedirectToAction("Index");
        }
    }
}
