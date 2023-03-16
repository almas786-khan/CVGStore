using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CVGS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Ajax.Utilities;
using CaptchaMvc.HtmlHelpers;
using System.Web.Services.Description;

namespace CVGS.Controllers
{
    public class CreditCardController : Controller
    {
            // GET: Card
            cvgsEntities2 _context = new cvgsEntities2();

            public ActionResult Index()
            {
                string name = User.Identity.GetUserName();
                User user = _context.Users.FirstOrDefault(x => x.UserName == name);
                var listofData = _context.CreditCards.Where(x => x.MemberID == user.UserID).ToList();
                return View(listofData);
            }

            [HttpGet]
            public ActionResult AddCard()
            {
                CardModel cardModel = new CardModel();
                return View(cardModel);
            }

            [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCard(CardModel cardModel)
            {
                try
                {
                    if (cardModel.Expiry < DateTime.Now)
                    {
                        ModelState.AddModelError("Expiry", "Your card is expired");
                    }
                    if (ModelState.IsValid)
                    {
                        CreditCard card = new CreditCard();
                        string name = User.Identity.GetUserName();
                        User user = _context.Users.FirstOrDefault(x => x.UserName == name);
                        card.MemberID = user.UserID;
                        card.CardNumber = cardModel.CardNumber;
                        card.CardHolderName = cardModel.CardHolderName;
                        card.CVC = cardModel.CVC;
                        //card.Expiry = cardModel.Expiry;
                        _context.CreditCards.Add(card);
                        _context.SaveChanges();
                        return RedirectToAction("Index", "CreditCard");
                    }
                    return View();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    return View();
                }

            }

            public JsonResult IsCardNumberExists(string CardNumber)
            {
                //check if any of the UserName matches the UserName specified in the Parameter using the ANY extension method.  
                return Json(!_context.CreditCards.Any(x => x.CardNumber == CardNumber), JsonRequestBehavior.AllowGet);
            }
        }
    }
