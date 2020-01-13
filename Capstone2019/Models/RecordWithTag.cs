using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone2019.Models
{
    /*
     * Only used for display and not saved in db.
     */
    public class RecordWithTag
    {
        public string Title { get; set; } //title of this record
        public string Root { get; set; }// what is the first note of the record (c,c#,d, etc.)
        public char Type { get; set; }// what type is the record (c,s,m)
        public string Tags { get; set; }//tags associated with this record
        public int Record_ID { get; set; }// unique id for this record
    }
}
