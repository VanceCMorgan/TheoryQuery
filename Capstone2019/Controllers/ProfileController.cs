//By Vance Morgan 000284251
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Capstone2019.Models;
using Microsoft.AspNetCore.Http;

namespace Capstone2019.Controllers
{
    /*
     * Handles the actions related to user profile and creation
     */
    public class ProfileController : Controller
    {
        //Db context
        private readonly ApplicationDbContext _context;
        //curently signed in user
        private static User _user;
        //controller costructor
        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Profile
        /*
         * Returns the profile index view with users specific information
         */
        public IActionResult Index(string error)
        {
            if (error != null)
            {
                ViewBag.Error = error;
            }
            
            if (User.Identity.IsAuthenticated)
            {
                var fName = User.Identity.Name.Split(' ')[0];
                _user = _context.Users.Where(u => u.First_Name == fName).First();
                //join compositions and comp tags to get all comp data
                ViewBag.Compositions = _context.Compositions.Where(c => c.Owner_ID == _user.User_ID)
               .Join(
                   _context.CompTags,
                   composition => composition.Composition_ID,
                   comptag => comptag.Comp_ID,
                   (composition, comptag) => new CompositionWithTag
                   {
                       Title = composition.Title,
                       Date_Created = composition.Date_Created,
                       Tags = comptag.Tags
                   }
               ).ToList();

                ViewBag.Image = _user.Profile_Picture;
                if (!_user.Is_Admin)
                {
                    return View(_context.Users.Where(u => u.First_Name == fName).First());
                }
                else
                {
                    //if adim go to admin profile
                    return RedirectToAction("Profile","Admin");
                }
            }
            else
            {
                // if not authenticated retun to sugn in oage
                return RedirectToAction("Index", "Home");
            }
        }
        /*
         * Called once the user is has been authenticated and the app has pulled user email and picture
         * from the google api, this action sets this info for the user and redirects back to the landing page
         */
        public async Task<IActionResult> AfterApi(string email,string pic)
        {
            var fName = User.Identity.Name.Split(' ')[0];
            _user = _context.Users.Where(u => u.First_Name == fName).FirstOrDefault();
            _user.Email_Address = email;
            _user.Profile_Picture = pic;
            _user.Last_Sign_In = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ssss"));
            _context.Users.Update(_user);
            ViewBag.image = pic;
            ViewBag.isAdmin = _user.Profile_Picture;
            await _context.SaveChangesAsync();
            if (_user.Is_Admin)
            {
                return RedirectToAction("LandingPage", "Admin");
            }
            else
            {
                return RedirectToAction("LandingPage", "Profile");
            }
        }

        /*
         * Called whe user updates info in their profile
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("User_ID,User_Name,First_Name,Last_Name,Email_Address,Last_Sign_In,Profile_Picture,Is_Admin")] User user)
        {
            //attempt to update user information, if error occurs return message string to the view
            try
            {
                User foundUser = user;
                var dt = _user.Last_Sign_In.ToString();
                foundUser.Last_Sign_In = Convert.ToDateTime(dt.Substring(0, dt.Length - 2)); //remove the 'AM' or 'PM' from string

                if (ModelState.IsValid && foundUser.First_Name != null)
                {
                    try
                    {
                        _context.Update(foundUser);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                    }

                    return RedirectToAction("Index", "Profile");
                }
                else
                {
                    return RedirectToAction("Index", "Profile", new { error = "Error Updating Profile" });
                }
            }catch(Exception e)
            {
                return RedirectToAction("Index", "Profile", new { error = "Error Updating Profile" });
            }
            
        }
        /*
         * called directly after user logins in,
         * checks to see if user exits
         * if so
         *      update user last sign in and update page info
         * else 
         *      create user and updat view info
         */
        public async Task<IActionResult> LandingPage()
        {
            bool adminFlag = false;
            if (User.Identity.IsAuthenticated)
            {
                
                var fName = User.Identity.Name.Split(' ')[0];
                var lName = User.Identity.Name.Split(' ')[1];
                

                //check user in db 
                User foundUser = new User();
                //if in db
                if (_context.Users.Any(u => u.First_Name == fName))
                {
                    foundUser = _context.Users.Where(u => u.First_Name == fName).First();
                    foundUser.Last_Sign_In = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ssss"));
                    ViewBag.image = foundUser.Profile_Picture;
                    ViewBag.isAdmin = foundUser.Is_Admin;
                    if (foundUser.Is_Admin)
                    {
                        adminFlag = true;
                    }
                }
                //if new
                else
                {
                    ViewBag.Error = "Not found";
                    User newUser = new User
                    {
                        First_Name = fName,
                        Email_Address = "get@fromapi.com",
                        Last_Name = lName,
                        User_Name = User.Identity.Name,
                        Last_Sign_In = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ssss")),
                        Is_Admin = false
                    };

                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();

                    var findAgain = _context.Users.Where(u => u.First_Name == fName).First();
                    ViewBag.result = _context.Compositions.Where(c => c.Owner_ID == findAgain.User_ID).ToList();

                    ViewBag.image = findAgain.Profile_Picture;
                    ViewBag.isAdmin = findAgain.Is_Admin;
                    if(newUser.Is_Admin)
                    {
                        adminFlag =true;
                    }
                }
                if (adminFlag)
                {
                    return RedirectToAction("LandingPage","Admin");
                }

            }
            return View();///
        }
        //Redirects user to the songmaker view
        public IActionResult Songmaker()
        {
            return RedirectToAction("Songmaker", "Songmaker");
        }
        //return true if a user with specified id exists in db
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.User_ID == id);
        }
    }
}
