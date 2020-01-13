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
    Handles the users composition view
    */
    public class CompositionsController : Controller
    {
        //database context
        private readonly ApplicationDbContext _context;
        //the signed in user
        private static User _user;
        //constructor
        public CompositionsController(ApplicationDbContext context)
        {
            _context = context;
        }
        /*
        return the view with all of the signed in users compositions
        error is a string representing an error message to diplay
        */
        public IActionResult Compositions(string error)
        {
            if(error != null){
                ViewBag.Error = error;
            }
            if (User.Identity.IsAuthenticated)
            {

                var fName = User.Identity.Name.Split(' ')[0];
                _user = _context.Users.Where(u => u.First_Name == fName).First();
                ViewBag.image = _user.Profile_Picture;
                ViewBag.isAdmin = _user.Is_Admin;
                var toReturn = _context.Compositions.Where(c=>c.Owner_ID == _user.User_ID)
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
                return View(toReturn);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        /*
        Deletes a composition from the compositions page
        filename represents the file to be deleted
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFile(string FileName)
        {
            try
            { 
                Composition toDel = _context.Compositions.Where(c => c.Title == FileName).First();
            
                _context.Remove(toDel);
                await _context.SaveChangesAsync();
            }
            catch
            {
                return RedirectToAction("Compositions", "Compositions", new { error = "Could not find file. Try again."});
            }
            return RedirectToAction("Compositions", "Compositions");
        }
        //Opens a composition in the sonmgaker page
        public IActionResult OpenFile(string FileName)
        {
            return RedirectToAction("OpenComp","Songmaker", new { compName = FileName });
        }

        }
}