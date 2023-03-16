using CVGS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CVGS.Controllers
{
    public class OrderController : Controller
    {
        cvgsEntities2 _context = new cvgsEntities2();
   
        [HttpGet]
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                TempData["message"] = "Order is Processed!!";
                ViewBag.Message = TempData["message"];
            }
            List<OrderViewModel> list = new List<OrderViewModel>();
            var result = from user in _context.Users
                         join order in _context.Orders on user.UserID equals order.MemberID
                         join orderdetails in _context.OrderDetails on order.OrderID equals orderdetails.OrderID    
                         join game in _context.Games on orderdetails.GameID equals game.GameID
                         select new OrderViewModel
                         {
                             OrderID = order.OrderID,
                             MemberName = user.UserName,
                             OrderDate = order.OrderDate,
                             TotalAmount = order.TotalAmount,
                             MemberID = order.MemberID,
                             OrderStatus = order.OrderStatus,
                             GameID = orderdetails.GameID,
                             GameName = game.GameName,
                             OrderDetailID = orderdetails.OrderDetailID
                             
                             
                         };

            list = result.ToList();
            return View(list);
        }
        [HttpPost]
        public ActionResult Index(string submit)
        {
            var orderID = Request["oID"];
            int orderId = Convert.ToInt32(orderID);
            var orderToUpdate = _context.Orders.SingleOrDefault(x => x.OrderID == orderId);
            if (submit == "Received")
            {
                orderToUpdate.OrderStatus = true;
                _context.Orders.AddOrUpdate(orderToUpdate);
                _context.SaveChanges();
                TempData["message"] = "Order is Processed!!";
            }
            return RedirectToAction("Index");
        }
    }
}