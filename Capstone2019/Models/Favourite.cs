using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone2019.Models
{
    /*
     * Represents a record that the user wants to save for easy access later on
     */
    public class Favourite
    {
        [Key]
        public int Favourite_ID { get; set; } //unique id for favourites
        [Required]
        public int Record_ID { get; set; }// the record_id of the associated record
        public int Owner_ID { get; set; }// owner of this specific favourite
    }
}
