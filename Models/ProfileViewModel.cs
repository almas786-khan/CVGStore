using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace CVGS.Models
{
    public class ProfileViewModel
    {
        public int UserID { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Name should not exceed limit")]
        [Display(Name = "First Name")]
        public string UserFirstName { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "Name should not exceed limit")]
        [Display(Name = "Last Name")]
        public string UserLastName { get; set; }

        [Required]
        public string Gender { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> Birthdate { get; set; }

        [Required]
        [Display(Name = "Would you like to get promotional emails?")]
        public Nullable<bool> IsEmail { get; set; }
    }
}