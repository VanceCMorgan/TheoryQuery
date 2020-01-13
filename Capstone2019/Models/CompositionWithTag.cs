using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone2019.Models
{
    /*
     * Only used for display and not saved in db.
     */
    public class CompositionWithTag
    {
        public string Title { get; set; } //title of comp            
        public DateTime Date_Created { get; set; }// date that the comp was created
        public string Tags { get; set; } // tags associated with this comp
    }
}
