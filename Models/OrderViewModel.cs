using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CVGS.Models
{
    public class OrderViewModel
    {
        public int OrderID { get; set; }
        [DataType(DataType.Date)]
        public System.DateTime OrderDate { get; set; }
        public double TotalAmount { get; set; }
        public int MemberID { get; set; }
        public string MemberName { get; set; }
        public bool OrderStatus { get; set; }
        public int OrderDetailID { get; set; }
        public int GameID { get; set; }
        public string GameName { get; set; }
    }
}