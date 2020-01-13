//By Vance Morgan 000284251
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Capstone2019.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Capstone2019.Controllers
{
    /*
    Handles the user favourites actions
    */
    public class FavouritesController : Controller
    {
        //the database context
        private readonly ApplicationDbContext _context;
        //the currently signed in user
        private static User _user;
        //constructor
        public FavouritesController(ApplicationDbContext context)
        {
            _context = context;
        }
        /*
        Displays the users favourites with tags
        */
        public IActionResult Favourites(string error)
        {
            if(error != null){
                ViewBag.Error= error;
            }
            if (User.Identity.IsAuthenticated)
            {

                var fName = User.Identity.Name.Split(' ')[0];
                _user = _context.Users.Where(u => u.First_Name == fName).First();
                ViewBag.image = _user.Profile_Picture;
                ViewBag.isAdmin = _user.Is_Admin;
                //join favourites,records and rectags
                var favs = _context.Favourites.Where(f => f.Owner_ID == _user.User_ID).ToList()
                     .Join(
                     _context.Records,
                    favourite => favourite.Record_ID,
                    record => record.Record_ID,
                    (favourite, record) => new RecordWithTag
                    {
                        Title = record.Record_Name,
                        Root = record.Root,
                        Type = record.Type,
                        Tags = "Tags",
                        Record_ID = record.Record_ID
                    }
                    ).Join(
                    _context.RecTags,
                    rwt=>rwt.Record_ID,
                    rectag=>rectag.Rec_ID,
                    (rwt, rectag)=> new RecordWithTag
                    {
                        Title = rwt.Title,
                        Root = rwt.Root,
                        Type = rwt.Type,
                        Tags = rectag.Tags,
                        Record_ID = rwt.Record_ID
                    }
                    ).ToList();


                return View(favs);
            }
            else
            {
                return RedirectToAction("Index", "Profile");
            }
        
        }
        /*
        Unfavourites a record
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> removeFavourite(string FileName)
        {
            try
            {
                var fName = User.Identity.Name.Split(' ')[0];
                _user = _context.Users.Where(u => u.First_Name == fName).First();
                var associatedRecord = _context.Records.Where(r => r.Record_Name == FileName).First();
                var toRemove = _context.Favourites.Where(f => f.Owner_ID == _user.User_ID && f.Record_ID == associatedRecord.Record_ID).ToList();
                foreach (var rem in toRemove)
                {
                    _context.Favourites.Remove(rem);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Favourites", "Favourites");
            }
            catch
            {
                return RedirectToAction("Favourites", "Favourites", new {error = "Could not find file. Try again."});
            }
        }
    }
}