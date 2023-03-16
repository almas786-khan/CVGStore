using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CVGS.Models
{
    public class PlatformPreferenceViewModel
    {
        public int PlatformId { get; set; }
        public string PlatformName { get; set; }
        public bool IsChecked { get; set; }
    }
}