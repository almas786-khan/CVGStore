using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CVGS.Models
{
    public class friendViewModel
    {
        public int FriendID { get; set; }
        public int Member1ID { get; set; }
        public string Member1Name { get; set; }

        public int Member2ID { get; set; }
        public string Member2Name { get; set; }
        public bool RequestSent { get; set; }

        public bool IsApprove { get; set; }
    }
}