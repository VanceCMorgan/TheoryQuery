using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone2019.Models
{
    /*
     * Represents a list of tags (labels) for a give composition
     */
    public class CompTag
    {
        [Key]
        public int CompTag_ID { get; set; } //unique id for comptags
        public int Comp_ID { get; set; } // composition id of related composition
        [Required]
        public int Owner_ID { get; set; }// userid of comp owner
        public string Tags { get; set; }// tags associated with this composition

    }
}
