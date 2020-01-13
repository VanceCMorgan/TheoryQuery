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
     * Handles songmkaer actions for a user
     */
    public class SongmakerController : Controller
    {
        //database for referance in this controller
        private readonly ApplicationDbContext _context;
        //current selected record
        public Record currentRec;
        //represents the position of the record gallery, 0 is first position
        public static int skipper = 0;
        //the id of the record selected
        public static int recordIndex = 0;
        public SongmakerController(ApplicationDbContext context)
        {
            _context = context;
        }
        /**
        Displays the user songmaker page with it relevant informaiton
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
                if (!user.Is_Admin)
                {
                    return View();
                }
                //redirect to admin songmaker 
                else
                {
                    return RedirectToAction("Songmaker", "Admin");
                }
            }
            else
            {
                return RedirectToAction("Index","Home");
            }
           
        }

        /*
        Selects a new record to be displayed based on theinput id
        it reresents the requested record id
        */
        public IActionResult SelectRec(int id)
        {
            var recArray = _context.Records.ToArray();
            var needed = _context.Records.Where(r=>r.Record_ID == id).First();
            var foundindex = Array.IndexOf(recArray,needed);
            recordIndex = foundindex;

            return RedirectToAction("Songmaker", "Songmaker");
        }
        //move gallery to the right
        public JsonResult ShuffleRight(string record)
        {
            var threeView = new String[] { "a", "b", "c" };
            if (skipper < 4)
            {
                skipper += 1;
            }
            if (_context.Records.Any(r => r.Record_Name.StartsWith(record))){

               var thisRec = _context.Records.Where(r => r.Record_Name.StartsWith(record)).First();
                threeView = thisRec.Notes_And_Chords.Split(',').Skip(skipper).Take(3).ToArray();
            }
            return Json(threeView);
        }
        //move gallery to the left
        public JsonResult ShuffleLeft(string record)
        {
            var threeView = new String[] { "a","b","c"};
            if (skipper != 0)
            {
                skipper -= 1;
            }
            if(_context.Records.Any(r => r.Record_Name.StartsWith(record))){

                var thisRec = _context.Records.Where(r => r.Record_Name.StartsWith(record)).First();
                threeView = thisRec.Notes_And_Chords.Split(',').Skip(skipper).Take(3).ToArray();
            }
            return Json(threeView);
        }
        //setup the songmaker view record display with initial rec
        public JsonResult Startup(string record)
        {
            String[] threeView = new String[3];
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
            return RedirectToAction("Songmaker", "Songmaker");
        }
        //Passes the input compName to the songmaker action to be opened
        public IActionResult OpenComp(string compName)
        {
            return RedirectToAction("Songmaker", "Songmaker", new { composition = compName});
        }
        //returns the search results for a composition by current user
        public JsonResult GetCompData(string SearchValue)
        {
            var fName = User.Identity.Name.Split(' ')[0];
            var user = _context.Users.Where(u => u.First_Name == fName).First();
            var compList = _context.Compositions.Where(c=>c.Owner_ID == user.User_ID).ToList();
            return Json(compList);
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
                    answer = rnd.Next(1, 3);
                    if (answer != 1)
                    {
                        answer = 5;
                    }
                }
                answer = rnd.Next(1, 5);
                //last position
            }
            else if (skipper == 4)
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
        /*
         * Saves a new composition for the signed in user,
         * gets the filename tags and notes from the user and creates both 
         * a new comp and a new related comp tag 
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveComposition(string FileName,string Tags,string comp)
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    string fName = User.Identity.Name.Split(' ')[0];
                    var thisUser = _context.Users.Where(u => u.First_Name == fName).FirstOrDefault();

                    Composition newComp = new Composition
                    {
                        Title = FileName,
                        Date_Created = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                        Last_Saved = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                        Owner_ID = thisUser.User_ID,
                        Notes = comp
                    };

                    _context.Compositions.Add(newComp);
                    await _context.SaveChangesAsync();

                    Composition thisComp = _context.Compositions.Where(c => c.Title == newComp.Title).FirstOrDefault();

                    CompTag newCompTag = new CompTag
                    {
                        Owner_ID = thisUser.User_ID,
                        Comp_ID = thisComp.Composition_ID,
                        Tags = Tags
                    };

                    _context.CompTags.Add(newCompTag);
                    await _context.SaveChangesAsync();
                }catch(Exception e)
                {
                    return RedirectToAction("Songmaker", "Songmaker");
                }
               

            }

            return RedirectToAction("Songmaker", "Songmaker");
        }
        /*
         * Adds new tags to record, if rec already had a tag, it is added
         */
        public async void AddTag(string tags,string record)
        {

            string fName = User.Identity.Name.Split(' ')[0];
            var user = _context.Users.Where(u => u.First_Name == fName).FirstOrDefault();
            var rec = _context.Records.Where(r=>r.Record_Name == record).FirstOrDefault();
            //if tag already exists, replace original tags
            if (_context.RecTags.Any(rt=>rt.Owner_ID == user.User_ID && rt.Rec_ID == rec.Record_ID))
            {
                var existingtags = _context.RecTags.Where(rt => rt.Owner_ID == user.User_ID && rt.Rec_ID == rec.Record_ID).First();
                existingtags.Tags = tags;
                _context.RecTags.Update(existingtags);
            }
            else
            {
                var newTags = new RecTag()
                {
                    Owner_ID = user.User_ID,
                    Rec_ID = rec.Record_ID,
                    Tags = tags
                };
                _context.RecTags.Add(newTags);
            }
            await _context.SaveChangesAsync();
        }
        /*
         * Adds the requested record to the currently signed in users favourites
         */
        public async Task<JsonResult> FavouriteRecord(string record)
        {
            
            string fName = User.Identity.Name.Split(' ')[0];
            var thisUser = _context.Users.Where(u => u.First_Name == fName).FirstOrDefault();
            var thisRec = _context.Records.Where(r => r.Record_Name == record).First();
            Favourite newFav = new Favourite
            {
                Owner_ID = thisUser.User_ID,
                Record_ID = thisRec.Record_ID
            };
            _context.Favourites.Add(newFav);
            await _context.SaveChangesAsync();
            return Json(newFav);
        }
    }
}