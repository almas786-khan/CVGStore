using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CVGS.Models
{
    public class AddressViewModel
    {
        public User Users { get; set; }
        public IEnumerable<Country> Countries { get; set; }
        public IEnumerable<Province> Provinces { get; set; }
        public Address MailingAddress { get; set; }
        public Address ShippingAddress { get; set; }
    }
}