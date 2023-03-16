using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CVGS.Models
{
    public class ChangePasswordModel
    {
        public int UserID { get; set; }
        [Required(ErrorMessage = "Current password is required", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password:")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New password is required", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password:")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Confirm New password is required", AllowEmptyStrings = false)]
        [Compare("NewPassword", ErrorMessage = "New password and confirm password does not match")]
        [Display(Name = "Confirm New Password:")]
        public string ConfirmNewPassword { get; set; }
    }
}