using CVGS.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace CVGS.Controllers
{
    public class WishListController : Controller
    {
        cvgsEntities2 _context = new cvgsEntities2();
        // GET: WishList
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Main()
        {
           
            return View(_context.Games.ToList());
        }
        [HttpPost]
        public ActionResult Main(Wishlist model)
        {

            string gID = Request["gID"];
            int GameID = Convert.ToInt32(gID);
            

            TempData["addWishList"] = "Added to Wish List successfully.";
            if (!ModelState.IsValid || ModelState.IsValid)
            {
                using (cvgsEntities2 db = new cvgsEntities2())
                {
                    var gameId = db.Games.Where(a => a.GameID == GameID).FirstOrDefault().GameID;

                    var userId = db.Users.FirstOrDefault(x => x.UserName == System.Web.HttpContext.Current.User.Identity.Name).UserID;
                    bool findFriend = _context.Wishlists.Any(x => (x.GameID == GameID && x.MemberID == userId));
                    if (findFriend)
                    {
                        var wishListId = db.Wishlists.Where(a => a.GameID == GameID && a.MemberID == userId).FirstOrDefault().WishlistID;

                        if (gameId != null)
                        {

                            Wishlist models = db.Wishlists.Find(wishListId);
                            db.Wishlists.Remove(models);
                            db.SaveChanges();

                            TempData["removeWishList"] = "Removed from WishList";
                            ViewBag.removeWishList = TempData["removeWishList"];
                           
                            return View(_context.Games.ToList());
                        }

                    }

                    else
                    {
                        if (gameId != null)
                        {
                            model.GameID = gameId;
                            model.MemberID = userId;
                            db.Wishlists.Add(model);
                            db.SaveChanges();
                            TempData["addWishList"] = "Added to Wish List successfully.";
                            ViewBag.addWishList = TempData["addWishList"];
                          
                            return View(_context.Games.ToList());

                        }
                        else
                        {
                            return HttpNotFound();
                        }
                       
                    }
                   

                }
            }

            return View();
        }
        public ActionResult WishList()
        {
            if (ModelState.IsValid)
            {
                using (cvgsEntities2 db = new cvgsEntities2())
                {
                    var currentUserId = db.Users.FirstOrDefault(x => x.UserName == System.Web.HttpContext.Current.User.Identity.Name).UserID;

                    var wishlistGameId = db.Wishlists.Where(a => a.MemberID == currentUserId && a.GameID == a.Game.GameID).ToString();

                    return View(_context.Wishlists.Where(a => a.MemberID == currentUserId && a.GameID == a.Game.GameID).ToList());

                }
            }
            return View();
        }
        
        
    }
}