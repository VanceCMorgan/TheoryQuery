using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone2019.Models
{
    /**
     * Represents a song (list of notes entered)
     */
    public class Composition
    {
        [Key]
        public int Composition_ID { get; set; } //unique id for compositions
        [Required]
        public int Owner_ID { get; set; } //user id of owner
        [Required]
        [MinLength(2)]
        [DataType(DataType.Text)]
        public string Title { get; set; } // name if this composition
        [Required]
        public DateTime Date_Created { get; set; } // when this was initially created

        public DateTime Last_Saved { get; set; }// last time saved
        [Required]
        [MinLength(2)]
        [DataType(DataType.Text)]
        public string Notes { get; set; } //notes/chords in the composition
    }
}
