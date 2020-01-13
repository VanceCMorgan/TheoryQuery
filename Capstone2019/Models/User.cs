using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone2019.Models
{
    /*
     * Represents a user of this application (admin or non-admin)
     */
    public class User
    {   
        [Key]
        public int User_ID { get; set; } //unique id for users
        [Required]
        [MinLength(2)]
        [DataType(DataType.Text)]
        public string User_Name { get; set; }
        [MinLength(2)]
        [DataType(DataType.Text)]
        public string First_Name { get; set; }
        [MinLength(2)]
        [DataType(DataType.Text)]
        public string Last_Name { get; set; }
        [Required]
        [MinLength(5)]
        [DataType(DataType.EmailAddress)]
        public string Email_Address { get; set; }
        public string Profile_Picture { get; set; }
        public DateTime Last_Sign_In { get; set; }
        public bool Is_Admin { get; set; }
    }
}
