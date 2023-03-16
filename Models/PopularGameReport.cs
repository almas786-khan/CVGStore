using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CVGS.Models
{
    public class PopularGameReport
    {
        public int GameID { get; set; }
        public string GameName { get; set; }

        public Nullable<double> GameOverallRating { get; set; }

        public int purchased;
    }
}