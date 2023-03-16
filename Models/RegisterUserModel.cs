using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CVGS.Models
{
    public class RegisterUserModel
    {

        public int UserID { get; set; }

        [Remote("IsUserExists", "Login", ErrorMessage = "User Name already in use")]
        [Required(ErrorMessage = "Error please enter a Unique User name.")]
        [StringLength(20)]
        // [Index("Ix_displayName", Order = 1, IsUnique = true)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Error Please enter a password.")]
        [DataType(DataType.Password)]
        [StringLength(20)]
        [Display(Name = "Password")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "Password must have 8 characters,1 letter, 1 number, and 1 special character")]
        public string UserPassword { get; set; }

       
        // public Nullable<UInt16> SomeField { get; set; }
      
        public string UserType { get; set; }


        [Required(ErrorMessage = "Error Please enter an email.")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        [Display(Name ="Email")]
        [StringLength(50)]
        public string UserEmail { get; set; }
    }
}