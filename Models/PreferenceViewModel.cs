using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CVGS.Models
{
    public class PreferenceViewModel
    {
        public User Users { get; set; }
        public List<PlatformPreferenceViewModel> PlatformPreferences { get; set; }
        public List<CategoryPreferenceViewModel> CategoryPreferences { get; set; }
    }
}