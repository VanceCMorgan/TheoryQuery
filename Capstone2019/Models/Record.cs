using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone2019.Models
{
    /*
     * Represents a scale,chord or mode that can be used to create a composition
     */
    public class Record
    {
        [Key]
        public int Record_ID { get; set; }// unique id for records
        [Required]
        [MinLength(2)]
        [DataType(DataType.Text)]
        public string Record_Name { get; set; } //name of this record
        [Required]
        public char Type { get; set; } // whart type is the record (c,s,m)
        [Required]
        public string Root { get; set; }// what is the first note of the record (c,c#,d, etc.)
        [Required]
        public int Length { get; set; }// how many notes are in this record
        [Required]
        public bool Is_Fav { get; set; }// not used
        [Required]
        [MinLength(2)]
        [DataType(DataType.Text)]
        public string Notes_And_Chords { get; set; } //notes and chords in this record

    }
}
