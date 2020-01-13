using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Capstone2019.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Capstone2019.Controllers
{
    /*
     * Handles actions on the homepage
     */
    public class HomeController : Controller
    {
        //Authentication provider to assist with google login
        private readonly IAuthenticationSchemeProvider authenticationSchemeProvider;
        //constructor
        public HomeController(IAuthenticationSchemeProvider _authenticationSchemeProvider)
        {
            this.authenticationSchemeProvider = _authenticationSchemeProvider;
        }
        //display login button is user is not authenticated else return landingpage
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                var allSchemeProvider = (await authenticationSchemeProvider.GetAllSchemesAsync())
                     .Select(n => n.DisplayName).Where(n => !String.IsNullOrEmpty(n));

                return View(allSchemeProvider);
            }
            else
            {
                return RedirectToAction("LandingPage","Profile");
            }
        }
        /*
         * Display the Privacy information
         */
        public IActionResult Privacy()
        {
            return View();
        }
        /*
         * Handles errors by returning the string to the view
         */
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //attempts to sing the user in using google
        public IActionResult SignIn(String provider)
        {
            var toReturn = Challenge(new AuthenticationProperties { RedirectUri = "/" }, provider);
            return toReturn;
        }
        /*
         * Signs the user out
         */
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
    

