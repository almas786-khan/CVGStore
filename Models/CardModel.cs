using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CVGS.Models
{
    public class CardModel
    {
        public int CreditCardID { get; set; }

        public int MemberID { get; set; }
        [Display(Name ="Credit Card Number")]
        [Remote("IsCardNumberExists", "CreditCard", ErrorMessage = "Credit Card Number already in use")]
        [Required(ErrorMessage = "Error Please enter a credit card number")]
        [RegularExpression(@"[0-9]{16}$", ErrorMessage = "Card Number must be 16 DIGITS")]
        public string CardNumber { get; set; }

        [Display(Name = "Credit Card Holder Name")]
        [Required(ErrorMessage = "Error Please enter a name for card holder")]
        public string CardHolderName { get; set; }

        [Required(ErrorMessage = "Error Please enter a the CVC")]
        //  [RegularExpression(@"[0-9]{3}$", ErrorMessage = "CVC must be 3 digits")]
        [RegularExpression(@"[0-9]{3}$", ErrorMessage ="CVC must be 3 digits")]
        public string CVC { get; set; }
        [Required(ErrorMessage ="Error: Please enter an expiry date")]
        [DataType(DataType.DateTime)]
        public DateTime? Expiry { get; set; }
    }
}