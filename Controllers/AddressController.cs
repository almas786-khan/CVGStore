using CVGS.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.Data;

namespace CVGS.Controllers
{
    public class AddressController : Controller
    {

        cvgsEntities2 _context = new cvgsEntities2();
        // GET: Address
        [HttpGet]
        public ActionResult Index()
        {
            string name = User.Identity.GetUserName();
            User user = _context.Users.FirstOrDefault(x => x.UserName == name);
            bool findAddress= _context.Addresses.Any(x => x.MemberID == user.UserID);
            if (findAddress)
            {
                ViewBag.message = "Address Exists";
            }
            Address address = new Address();
            return View(address);
        }


        [HttpGet]
        public ActionResult ModifyAddress()
        {
            string name = User.Identity.GetUserName();
            CVGS.Models.User user = _context.Users.FirstOrDefault(x => x.UserName == name);
            AddressViewModel mymodel = new AddressViewModel();
            mymodel.Users = _context.Users.Find(user.UserID);
            mymodel.MailingAddress = _context.Addresses.Where(a => a.User.UserID == user.UserID && a.IsMailingAddress == true).FirstOrDefault();
            try
            {
                if (mymodel.MailingAddress == null)
                {
                    var defaultAddress = new Address
                    {
                        MemberID = user.UserID,
                        ProvinceID = 27,
                        CountryID = 2,
                        IsMailingAddress = true
                    };
                    mymodel.MailingAddress = defaultAddress;
                }
                mymodel.ShippingAddress = _context.Addresses.Where(a => a.User.UserID == user.UserID && a.IsMailingAddress == false).FirstOrDefault();
                if (mymodel.ShippingAddress == null)
                {
                    var defaultAddress = new Address
                    {
                        MemberID = user.UserID,
                        ProvinceID = 27,
                        CountryID = 2,
                        IsMailingAddress = false
                    };
                    mymodel.ShippingAddress = defaultAddress;
                }
                mymodel.Countries = _context.Countries.ToList();
                mymodel.Provinces = _context.Provinces.ToList();
            }
            catch (DataException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(mymodel);
        }
        private void updateAddress(AddressViewModel mymodel, User user, bool isMailingAddress, bool? sameAddress)
        {
            bool same = sameAddress ?? false;
            Address address = _context.Addresses.Where(a => (a.IsMailingAddress == isMailingAddress) && (a.MemberID == user.UserID)).FirstOrDefault();
            if (address != null)
            {
                address.UnitAndStreet = isMailingAddress || same ? mymodel.MailingAddress.UnitAndStreet : mymodel.ShippingAddress.UnitAndStreet;
                address.City = isMailingAddress || same ? mymodel.MailingAddress.City : mymodel.ShippingAddress.City;
                address.ProvinceID = isMailingAddress || same ? mymodel.MailingAddress.ProvinceID : mymodel.ShippingAddress.ProvinceID;
                address.CountryID = isMailingAddress || same ? mymodel.MailingAddress.CountryID : mymodel.ShippingAddress.CountryID;
                address.PostalCode = isMailingAddress || same ? mymodel.MailingAddress.PostalCode : mymodel.ShippingAddress.PostalCode;
                address.IsSame = (bool)sameAddress;
            }
            else
            {
                var newAddress = new Address
                {
                    MemberID = user.UserID,
                    UnitAndStreet = isMailingAddress || same ? mymodel.MailingAddress.UnitAndStreet : mymodel.ShippingAddress.UnitAndStreet,
                    City = isMailingAddress || same ? mymodel.MailingAddress.City : mymodel.ShippingAddress.City,
                    ProvinceID = isMailingAddress || same ? mymodel.MailingAddress.ProvinceID : mymodel.ShippingAddress.ProvinceID,
                    CountryID = isMailingAddress || same ? mymodel.MailingAddress.CountryID : mymodel.ShippingAddress.CountryID,
                    PostalCode = isMailingAddress || same ? mymodel.MailingAddress.PostalCode : mymodel.ShippingAddress.PostalCode,
                    IsMailingAddress = isMailingAddress,
                    IsSame = (bool)sameAddress
                };
                _context.Addresses.Add(newAddress);

            }

        }
        [HttpPost]
        public ActionResult ModifyAddress(AddressViewModel mymodel)
        {
            string name = User.Identity.GetUserName();
            User user = _context.Users.FirstOrDefault(x => x.UserName == name);
           // User user = _context.Users.Find(id);
            if (ModelState.IsValid)
            {
                bool ?v = mymodel.ShippingAddress.IsSame;
                // Update Mailing Address
                this.updateAddress(mymodel, user, true, mymodel.ShippingAddress.IsSame);

                // Update shipping address
                this.updateAddress(mymodel, user, false, mymodel.ShippingAddress.IsSame);

                try
                {
                    _context.SaveChanges();
                    //return View(categories);
                }
                catch (DataException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return RedirectToAction("ModifyAddress", new { id = mymodel.Users.UserID });
        }
        public List<string> CountryList()
        {
            List<string> CultureList = new List<string>();
            CultureInfo[] getCultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo getCulture in getCultureInfo)
            {
                RegionInfo GetRegionInfo = new RegionInfo(getCulture.LCID);
                if (!(CultureList.Contains(GetRegionInfo.EnglishName)))
                {
                    CultureList.Add(GetRegionInfo.EnglishName);
                }
            }
            CultureList.Sort();
            return CultureList;
        }
    }
}