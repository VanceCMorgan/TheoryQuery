//By Vance Morgan 000284251
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Capstone2019.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Capstone2019.Controllers
{
    /*
    Hanldes the authorization actions for this application
    */
    public class AuthController : Controller
    {
        // Authentication provider to use with google
        private readonly IAuthenticationSchemeProvider authenticationSchemeProvider;
        // database context
        private readonly ApplicationDbContext _context;
        //constructor
        public AuthController(IAuthenticationSchemeProvider _authenticationSchemeProvider, ApplicationDbContext context)
        {
            this.authenticationSchemeProvider = _authenticationSchemeProvider;
            _context = context;
        }

        /*
        Handles user login view
        */
        public async Task<IActionResult> Login(string returnUrl)
        {
            var allSchemeProvider = (await authenticationSchemeProvider.GetAllSchemesAsync())
                .Select(n => n.DisplayName).Where(n => !String.IsNullOrEmpty(n));

            return View(allSchemeProvider);
        }
        /*
        Sign in user
        */
        public IActionResult SignIn(String provider)
        {
            
            var fName = User.Identity.Name.Split(' ')[0];
            var user = _context.Users.Where(u => u.First_Name == fName).First();
            //determine if admin
            if (user.Is_Admin)
            {
                return RedirectToAction("LandingPage", "Admin");
            }
            else
            {
                return RedirectToAction("LandingPage", "Profile");
            }
        }

        /*
        Sign out user
        */
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index","Home");
        }
    }
}