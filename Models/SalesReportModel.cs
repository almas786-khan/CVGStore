using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CVGS.Models
{
    public class SalesReportModel
    {
        public string GameName { get; set; }
        public double GamePrice { get; set; }
        public int Purchases { get; set; }
        [DataType(DataType.Date)]
        public System.DateTime OrderDate { get; set; }

        public double TotalAmount { get; set; }
        public double Total { get; set; }
    }
}