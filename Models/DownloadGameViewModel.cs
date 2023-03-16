using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CVGS.Models
{
    public class DownloadGameViewModel
    {
        public int OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public int GameID { get; set; }
        public string GameName { get; set; }
        public int MemberID { get; set; }
        public bool OrderStatus { get; set; }
        public System.DateTime OrderDate { get; set; }
        public string ImageURL { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }

    }
}