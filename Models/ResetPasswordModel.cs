using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CVGS.Models
{

    //[NotMapped]
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "Current password required", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

  
      
        [StringLength(20)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "Password must have 8 characters,1 letter, 1 number, and 1 special character")]
        [Required(ErrorMessage = "New password required", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }



        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "New password and confirm password does not match")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string ResetCode { get; set; }
    }
}