using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone2019.Models
{
    /*
     * Represents a list of tags (labels) for a given record
     */
    public class RecTag
    {
        [Key]
        public int RecTag_ID { get; set; }//unique id for rectags
        public int Rec_ID { get; set; }//the record_id of the associated record
        [Required]
        public int Owner_ID { get; set; }//user_id of the owner of this tag
        public string Tags { get; set; }//tags associated with this record
    }
}
