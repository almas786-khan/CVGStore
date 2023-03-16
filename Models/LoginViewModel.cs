using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CVGS.Models
{
    public class LoginViewModel
    {
        public int UserID { get; set; }
      
        [Remote("IsUserExists", "Register", ErrorMessage = "User Name not found")]
        [Required(ErrorMessage = "Error please enter a Unique User name.")]
        [StringLength(20)]
        public string UserName { get; set; }

       
        [Required(ErrorMessage = "Error Please enter a password.")]
        [DataType(DataType.Password)]
        [StringLength(20)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "Password must have 8 characters,1 letter, 1 number, and 1 special character")]
        [Display(Name ="Password")]
        public string UserPassword { get; set; }
    }
}