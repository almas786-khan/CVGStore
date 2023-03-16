using CVGS.Models;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using User = CVGS.Models.User;

namespace CVGS.Controllers
{
    public class DownloadController : Controller
    {
        cvgsEntities2 _context = new cvgsEntities2();
        // GET: Download
        public ActionResult Index()
        {
            string name = User.Identity.GetUserName();
            User user = _context.Users.FirstOrDefault(x => x.UserName == name);
            List<DownloadGameViewModel> list = new List<DownloadGameViewModel>();
            var result = (from order in _context.Orders
                         join orderd in _context.OrderDetails on order.OrderID equals orderd.OrderID
                         join game in _context.Games on orderd.GameID equals game.GameID
                         where order.MemberID == user.UserID 
                         select new  DownloadGameViewModel
                         {
                             GameID = game.GameID,
                             GameName = game.GameName,
                             OrderDate = order.OrderDate,
                             ImageURL = game.ImageURL
                         }).Distinct();
            list = result.ToList();

            return View(list);
            
        }
        [HttpPost]
        public FileResult DownloadGame(int id)
        {
            string gameName = "";
           
            if (id != 0)
            {
                gameName=_context.Games.SingleOrDefault(a => a.GameID == id).GameName;
            }
           
            string path = Server.MapPath($"~/Image/{gameName}.txt");
          
            using (StreamWriter sw = System.IO.File.CreateText(path))
            {
                sw.WriteLine(gameName);
                sw.WriteLine("Your Game file is downloaded. Enjoy !!");
            }
           
            var FileVirtualPath = path;
            return File(FileVirtualPath, "application/force- download", Path.GetFileName(FileVirtualPath));
        }
    }
}