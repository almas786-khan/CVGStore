using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CVGS.Models
{
    public class CategoryPreferenceViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool IsChecked { get; set; }
    }
}