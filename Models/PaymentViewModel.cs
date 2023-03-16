using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CVGS.Models
{
    public class PaymentViewModel
    {
        public IEnumerable<Game> Games { get; set; }
        public IEnumerable<CreditCard> CreditCards { get; set; }
    }
}