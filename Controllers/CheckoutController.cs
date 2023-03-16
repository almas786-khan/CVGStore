using CVGS.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CVGS.Controllers
{
    public class CheckoutController : Controller
    {

        cvgsEntities2 _context = new cvgsEntities2();
        // GET: Checkout
        public ActionResult Index()
        {
            var listodData = _context.Games.ToList();
            return View(listodData);

        }

        public ActionResult Payment()
        {
            List<Game> list;
            if (Session["Cart"] == null)
            {
                list = new List<Game>();
            }
            else
            {
                list = (List<Game>)Session["Cart"];
            }

            Session["Cart"] = list;

            var listodData = Session["Cart"];

            if (list.Count == 0)
            {
                TempData["Cart"] = "Cart is Empty";
                ViewBag.CartSize = TempData["Cart"];
            }
            return View(listodData);
        }

        [HttpPost]
        public ActionResult Add()
        {
            int GameID = Convert.ToInt32(Request["gID"]);
            List<Game> list;
            if (Session["Cart"] == null)
            {
                list = new List<Game>();
            }
            else
            {
                list = (List<Game>)Session["Cart"];
            }
            list.Add(_context.Games.Where(p => p.GameID == GameID).FirstOrDefault());
            Session["Cart"] = list;
            checkCartItem();
            return RedirectToAction("Index", "Member");
           // return RedirectToAction("Payment");
        }
        public void checkCartItem()
        {
            var arrSaveList = new ArrayList();
            int GameID = Convert.ToInt32(Request["gID"]);
            List<Game> checkList;
            checkList = (List<Game>)Session["Cart"];
            foreach (var item in checkList)
            {
                arrSaveList.Add (item.GameID);
            }
            TempData["CheckCartGameId"] = arrSaveList.ToArray(typeof(object));
        }
        [HttpPost]
        public ActionResult RemoveFromIndex()
        {
            int GameID = Convert.ToInt32(Request["gID"]);
            List<Game> list;
            if (Session["Cart"] == null)
            {
                list = new List<Game>();
            }
            else
            {
                list = (List<Game>)Session["Cart"];
            }
            var itemToRemove = list.Single(r => r.GameID == GameID);
            list.Remove(itemToRemove);
            Session["Cart"] = list;
            checkCartItem();
            return RedirectToAction("Index", "Member");
        }

        [HttpPost]
        public ActionResult Remove()
        {
            int GameID = Convert.ToInt32(Request["gID"]);
            List<Game> list;
            if (Session["Cart"] == null)
            {
                list = new List<Game>();
            }
            else
            {
                list = (List<Game>)Session["Cart"];
            }
            var itemToRemove = list.Single(r => r.GameID == GameID);
            list.Remove(itemToRemove);
            Session["Cart"] = list;
            checkCartItem();
            return RedirectToAction("Payment");
        }

        public ActionResult Details()
        {
            string name = User.Identity.GetUserName();
            User user = _context.Users.FirstOrDefault(x => x.UserName == name);
            PaymentViewModel viewModel = new PaymentViewModel();
            viewModel.Games = (IEnumerable<Game>)Session["Cart"];

            viewModel.CreditCards = _context.CreditCards.Where(x => x.MemberID == user.UserID).ToList();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ConfirmDetail()
        {
            if (Request.Form["CreditCardId"] == null)
            {
                TempData["CardInfo"] = "Add a card to checkout";
                return RedirectToAction("Index", "CreditCard");
            }
            double Tcost = Convert.ToDouble(Request["Total"]);
            string name = User.Identity.GetUserName();
            User user = _context.Users.FirstOrDefault(x => x.UserName == name);

            Order order = new Order();
            order.OrderDate = DateTime.Now;
            order.CreditCardID = Convert.ToInt32(Request.Form["CreditCardId"]);
            order.TotalAmount = Tcost;
            order.MemberID = user.UserID;
            order.OrderStatus = false;

            _context.Orders.Add(order);
            _context.SaveChanges();

            List<Game> list = (List<Game>)Session["Cart"];

            foreach (Game game in list)
            {
                OrderDetail orderDetail = new OrderDetail();
                orderDetail.OrderID = order.OrderID;
                orderDetail.GameID = game.GameID;
                _context.OrderDetails.Add(orderDetail);
                _context.SaveChanges();
            }

            Session.Remove("Cart");
            TempData["InfoMessage"] = "Thank you for shopping. You can now download your game. ";
            return RedirectToAction("Index", "Download");
        }
    }
}