using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CVGS.Models
{
    public class ReviewViewModel
    {
        public int ReviewID { get; set; }
        public int MemberID { get; set; }
        public string Name { get; set; }
        public int GameID { get; set; }

        [Display(Name ="Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime ReviewDate { get; set; }
        public bool Status { get; set; }
        [Required(ErrorMessage = "Review is required")]
        [DataType(DataType.MultilineText)]

        [Display(Name="Review")]
        public string Review1 { get; set; }

        public virtual Game Game { get; set; }
        public virtual User User { get; set; }
    }
}