//By Vance Morgan 000284251
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Capstone2019.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Capstone2019.Controllers
{
/**
Handles the majority oof admin user role actions
*/
    public class AdminController : Controller
    {
        //database for referance in this controller
        private readonly ApplicationDbContext _context;
        //current authorized user
        private static User _user;
        //current selected record
        public Record currentRec;
        //represents the position of the record gallery, 0 is first position
        public static int skipper = 0;
        //the id of the record selected
        public static int recordIndex = 0;
    
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        /**
        Displays the admin index view with relevant information
        error canbe set to anyerror stirng that should be displayed
        */
        public IActionResult Index(string error)
        {
            //display error
            if (error != null)
            {
                ViewBag.Error = error;
            }
            //ensure authenticated
            if (User.Identity.IsAuthenticated)
            {
                var fName = User.Identity.Name.Split(' ')[0];
                _user = _context.Users.Where(u => u.First_Name == fName).First();
                //determine if acctually admin
                if (_user.Is_Admin)
                {
                    ViewBag.image = _user.Profile_Picture;
                    ViewBag.isAdmin = _user.Is_Admin;
                    return View();
                }
                //if not admin redirect to user landing page
                else
                {
                    return RedirectToAction("LandingPage", "Profile");
                }
            }
            //if not authenticated redirect to user landing page
            return RedirectToAction("LandingPage","Profile");
        }

        /**
        Displays the admin songmaker page with it relevant informaiton
        composition represents a saved composition to be displayed (loaded)
        */
        public IActionResult Songmaker(string composition)
        {
            //ensure authenticated
            if (User.Identity.IsAuthenticated)
            {
                //get user
                var fName = User.Identity.Name.Split(' ')[0];
                var user = _context.Users.Where(u => u.First_Name == fName).First();
                //set user image and is admin property
                ViewBag.image = user.Profile_Picture;
                ViewBag.isAdmin = user.Is_Admin;
                //set comp if loading
                if (composition != null)
                {
                    ViewBag.compositionContents = _context.Compositions.Where(c => c.Title == composition).First().Notes.Split(',');
                }
                //set current record details
                currentRec = _context.Records.ToArray()[recordIndex];
                ViewBag.recTitle = currentRec.Record_Name;
                ViewBag.record = currentRec.Notes_And_Chords.Split(',').Skip(skipper).Take(3).ToArray();
                //check if admin
                if (user.Is_Admin)
                {
                    return View();
                }
                //redirect to user songmaker if not admin
                else
                {
                    return RedirectToAction("Songmaker","Songmaker");
                }
            }
            
            //redirect to user songmaker if not authenticated
            else
            {
                return RedirectToAction("Songmaker", "Songmaker");
            }
        }

        /*
        Selects a new record to be displayed based on theinput id
        it reresents the requested record id
        */
        public IActionResult SelectRec(int id)
        {
            var recArray = _context.Records.ToArray();
            var needed = _context.Records.Where(r => r.Record_ID == id).First();
            var foundindex = Array.IndexOf(recArray, needed);
            recordIndex = foundindex;

            return RedirectToAction("Songmaker", "Admin");
        }
        //move gallery to the right
        public JsonResult ShuffleRight(string record)
        {
            var threeView = new String[] { };
            if (skipper < 4)
            {
                skipper += 1;
            }
            if (_context.Records.Any(r => r.Record_Name.StartsWith(record)))
            {

                var thisRec = _context.Records.Where(r => r.Record_Name.StartsWith(record)).First();
                threeView = thisRec.Notes_And_Chords.Split(',').Skip(skipper).Take(3).ToArray();
            }
            return Json(threeView);
        }
        //move gallery to the left
        public JsonResult ShuffleLeft(string record)
        {
            var threeView = new String[] { };
            if (skipper != 0)
            {
                skipper -= 1;
            }
            if (_context.Records.Any(r => r.Record_Name.StartsWith(record)))
            {

                var thisRec = _context.Records.Where(r => r.Record_Name.StartsWith(record)).First();
                threeView = thisRec.Notes_And_Chords.Split(',').Skip(skipper).Take(3).ToArray();
            }
            return Json(threeView);
        }
        //setup the songmaker view record display with initial rec
        public JsonResult Startup(string record)
        {
            String[] threeView = new String[3]; //the three displayed notes
            if (_context.Records.Any(r => r.Record_Name.StartsWith(record)))
            {
                var thisRec = _context.Records.Where(r => r.Record_Name.StartsWith(record)).First();
                threeView = thisRec.Notes_And_Chords.Split(',').Skip(0).Take(3).ToArray();
            }
            return Json(threeView);
        }
        //Moves to next record
        public IActionResult NextRec()
        {
            recordIndex += 1;
            return RedirectToAction("Songmaker", "Admin");
        }
        /*
        Handles posts on the admin profile page, it binds the input values to a new user,
        finds that user and updates its values accodingly
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("User_ID,User_Name,First_Name,Last_Name,Email_Address,Last_Sign_In,Profile_Picture,Is_Admin")] User user)
        {
            if (User.Identity.IsAuthenticated)
            {
                User foundUser = user;

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(foundUser);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                    }
                }
                else
                {
                    return RedirectToAction("Profile", "Admin", new { Error = "Unable to updat information try again." });
                }
                //display view again
                return RedirectToAction("Profile", "Admin");

            }
            else
            {
                return RedirectToAction("Index","Home");
            }


        }
        /*
        Displays the Admin profile page with all relevant information
        */
        public IActionResult Profile(string Error)
        {
            if (Error != null)
            {
                ViewBag.Error = Error;
            }
            if (User.Identity.IsAuthenticated)
            {
                var fName = User.Identity.Name.Split(' ')[0];
                _user = _context.Users.Where(u => u.First_Name == fName).First();
                if (_user.Is_Admin)
                {
                    ViewBag.image = _user.Profile_Picture;
                    ViewBag.Users = _context.Users.ToList();
                    ViewBag.isAdmin = _user.Is_Admin;
                    return View(_user);
                }
                //if not admin go to profile page
                else
                {
                    return RedirectToAction("Index","Profile");
                }
            }
            //if not auuthenticated go back to login page
            return RedirectToAction("Index", "Home"); ;
        }
        /*
        Displays admin landing page
        */
        public IActionResult LandingPage()
        {
            if (User.Identity.IsAuthenticated)
            {
                var fName = User.Identity.Name.Split(' ')[0];
                _user = _context.Users.Where(u => u.First_Name == fName).First();
                ViewBag.isAdmin = _user.Is_Admin;
                ViewBag.image = _user.Profile_Picture;
                //check if admin
                if (_user.Is_Admin)
                {
                    return View();
                }
                //else go to user profile
                else
                {
                    return RedirectToAction("LandingPage", "Profile");
                }
            }
            else
            {
                return RedirectToAction("Index","Home");
            }
            
        }
        /*
        Creates a new user from admin page
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser([Bind("User_Name,First_Name,Last_Name,Email_Address,Is_Admin")] User user)
        {
            user.Last_Sign_In = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ssss"));
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                //return error if any issues with add
                return RedirectToAction("Index", "Admin", new { error = "Error adding user" });
            }
        }
        /*
        Handles admin existing user actions,
         1 = promote user
         2 = demote user
         3 = delete user
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExistingUser([Bind("User_Name")] User user, int choice)
        {
            if (user.User_Name != null)
            {
                User foundUser = _context.Users.Where(m => m.User_Name == user.User_Name).FirstOrDefault();
                if (foundUser != null)
                {
                    switch (choice)
                    {
                        case 1:
                            foundUser.Is_Admin = true;
                            _context.Update(foundUser);
                            await _context.SaveChangesAsync();
                            break;
                        case 2:
                            foundUser.Is_Admin = false;
                            _context.Update(foundUser);
                            await _context.SaveChangesAsync();
                            break;
                        case 4:
                            var comps = _context.Compositions.Where(c=>c.Owner_ID == foundUser.User_ID).ToList();
                            foreach(var c in comps)
                            {
                                _context.Compositions.Remove(c);
                            }
                            var favs = _context.Favourites.Where(c => c.Owner_ID == foundUser.User_ID).ToList();
                            foreach (var f in favs)
                            {
                                _context.Favourites.Remove(f);
                            }
                            var recTags = _context.RecTags.Where(c => c.Owner_ID == foundUser.User_ID).ToList();
                            foreach (var rt in recTags)
                            {
                                _context.RecTags.Remove(rt);
                            }
                            var compTags = _context.CompTags.Where(c => c.Owner_ID == foundUser.User_ID).ToList();
                            foreach (var ct in compTags)
                            {
                                _context.CompTags.Remove(ct);
                            }
                            await _context.SaveChangesAsync();
                            _context.Users.Remove(foundUser);
                            await _context.SaveChangesAsync();
                            break;
                        default:
                            throw new Exception("Error with selection");
                    }
                }
                else
                {
                    //return error if isues dinding user
                    return RedirectToAction("Index", "Admin", new { error = "Could not find that User"});
                }
            }
            return RedirectToAction("Index", "Admin");
        }

        /*
        Handles addition of records from admin page
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRecord([Bind("Record_Name,Root,Type,Notes_And_Chords")] Record record,string Tags)
        {
            if (record.Record_Name != null && record.Notes_And_Chords != null)
            {
                record.Length = record.Notes_And_Chords.Split(',').Count(); //calculated field based on length of notes and chords
                if (ModelState.IsValid)
                {
                    _context.Add(record);
                    await _context.SaveChangesAsync();

                }

                var fName = User.Identity.Name.Split(' ')[0];
                _user = _context.Users.Where(u => u.First_Name == fName).First();
                var foundRec = _context.Records.Find(record.Record_ID);
                RecTag rT = new RecTag
                {
                    Owner_ID = _user.User_ID,
                    Rec_ID = foundRec.Record_ID,
                    Tags = Tags
                };

                _context.Add(rT);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Admin");
            }
            else
            {
                //return error idf issues with add
                return RedirectToAction("Index", "Admin", new { error = "Invalid Record Details" });
            }

        }
        /*
        Handles record search from admin songmaker page,
        search by = field to search by (id,name,type,root)
        search value = value to search

        returns an object of the records that match this search value => recordList
        and the tags relateed to these records => TagList
        */
        public JsonResult GetSearchingData(string SearchBy, string SearchValue)
        {
            List<Record> RecordList = new List<Record>();
            if (SearchBy == "ID")
            {
                try
                {
                    int Id = Convert.ToInt32(SearchValue);
                    RecordList = _context.Records.Where(x => x.Record_ID == Id || SearchValue == null).ToList();
                }
                catch (FormatException)
                {
                    Console.WriteLine("{0} Is Not A ID ", SearchValue);
                }
                var IdList = RecordList.Select(r => r.Record_ID).ToList();
                List<RecTag> TagsList = _context.RecTags.Where(rt => IdList.Contains(rt.Rec_ID)).ToList();
                List<object> answer = new List<object>
                {
                    RecordList,
                    TagsList
                };
                return Json(answer);
            }
            else if (SearchBy == "Type")
            {
                RecordList = _context.Records.Where(x => x.Type == SearchValue[0] || SearchValue == null).ToList();
                var IdList = RecordList.Select(r => r.Record_ID).ToList();
                List<RecTag> TagsList = _context.RecTags.Where(rt => IdList.Contains(rt.Rec_ID)).ToList();
                List<object> answer = new List<object>
                {
                    RecordList,
                    TagsList
                };
                return Json(answer);
            }
            else if (SearchBy == "Root")
            {
                RecordList = _context.Records.Where(x => x.Root.StartsWith(SearchValue) || SearchValue == null).ToList();
                var IdList = RecordList.Select(r => r.Record_ID).ToList();
                List<RecTag> TagsList = _context.RecTags.Where(rt => IdList.Contains(rt.Rec_ID)).ToList();
                List<object> answer = new List<object>
                {
                    RecordList,
                    TagsList
                };
                return Json(answer);
            }
            else
            {
                RecordList = _context.Records.Where(x => x.Record_Name.StartsWith(SearchValue) || SearchValue == null).ToList();
                var IdList = RecordList.Select(r => r.Record_ID).ToList();
                List<RecTag> TagsList = _context.RecTags.Where(rt => IdList.Contains(rt.Rec_ID)).ToList();
                List<object> answer = new List<object>
                {
                    RecordList,
                    TagsList
                };
                return Json(answer);
            }
        }
        //plays the entire record that has record_name matchin record
        public JsonResult PlayRecord(string record)
        {
            var rec = _context.Records.Where(r=>r.Record_Name == record).First();
            var NAC = rec.Notes_And_Chords.Split(',');
            return Json(NAC);
        }
        //Sends suggestions to the songmaker by determining the 
        //current gallery position and the current composition state
        public JsonResult Suggestion(string[] comp)
        {
          
            Random rnd = new Random();
            int answer = 2;
            //first view position
            if (skipper == 0)
            {
                //if first note is empty
                if (comp[0] == "")
                {
                    //33% chance of suggesting 5th 66% chance of suggesting root
                    answer = rnd.Next(1,3);
                    if (answer!= 1)
                    {
                        answer = 5;
                    }
                }
                answer = rnd.Next(1,5);
            //last position
            }else if (skipper == 4)
            {
                //if first note is empty
                if (comp[0] == "")
                {
                    //33% chance of suggesting 5th 66% chance of suggesting root
                    answer = rnd.Next(1, 3);
                    if (answer != 1)
                    {
                        answer = 5;
                    }
                }
                answer = rnd.Next(0, 4);
            }
            //somewhere in middle
            else
            {
                //if first note is empty
                if (comp[0] == "")
                {
                    //33% chance of suggesting 5th 66% chance of suggesting root
                    answer = rnd.Next(1, 3);
                    if (answer != 1)
                    {
                        answer = 5;
                    }
                }
                answer = rnd.Next(0, 5);
            }

            return Json(answer);
        }
    }
}
