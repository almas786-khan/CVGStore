using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CVGS.Models
{
    public class Games
    {
        public int GameID { get; set; }
        public string GameName { get; set; }
        public string GameDescription { get; set; }
        public double GamePrice { get; set; }
        public Nullable<double> GameOverallRating { get; set; }
        public string ImageFile { get; set; }
    }
}