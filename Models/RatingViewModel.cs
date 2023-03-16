using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CVGS.Models
{
    public class RatingViewModel
    {
        public int RatingID { get; set; }

        [Required]
        public double Rating { get; set; }
        public int MemberID { get; set; }
        public int GameID { get; set; }
        public string GameName { get; set; }
        public Nullable<System.DateTime> RatingDate { get; set; }

    }
}