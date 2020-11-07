using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;

namespace esp2.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;    
        private readonly IUserClaimsPrincipalFactory<IdentityUser> _userClaimsPrincipalFactory;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IUserClaimsPrincipalFactory<IdentityUser> userClaimsPrincipalFactory,
            ILogger<HomeController> logger)    
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index()
        {
            return View();
        }


        [Authorize]
        public IActionResult Secret()   
        {
            return View();
        }

        [HttpGet]
       public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if(user != null)
            {
                var principal  = await _userClaimsPrincipalFactory.CreateAsync(user);
                await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme,principal);
                return RedirectToAction("Secret");


                //var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);
                //if(signInResult.Succeeded)
                //{
                //    return RedirectToAction("Secret");
                //}
            }

            return RedirectToAction("Register");
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if(user == null)
            {
                user = new IdentityUser 
                {
                    UserName = userName,
                };
                var result = await _userManager.CreateAsync(user,password);
                if(result .Succeeded)
                {
                    //var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
                    //await HttpContext.SignInAsync(principal);
                    //return RedirectToAction("Secret");
                    return RedirectToAction("Login");
                }
            }
            return RedirectToAction("Login");
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index");
        }
    }
}
