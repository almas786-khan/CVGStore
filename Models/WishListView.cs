using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace CVGS.Models
{
    public class WishListView
    {
        public int WishlistID { get; set; }
        public int MemberID { get; set; }
        public string MemberName { get; set; }
        public int GameID { get; set; }
        public string GameName { get; set; }
        [DisplayName("")]
        public string ImageURL { get; set; }
        public virtual Game Game { get; set; }
        public virtual User User { get; set; }
    }
}