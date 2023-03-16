using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web.Mvc;
using CVGS.Models;
using Microsoft.AspNet.Identity;

namespace CVGS.Controllers
{
    public class MemberController : Controller
    {
        cvgsEntities2 _context = new cvgsEntities2();
        GameModel game = new GameModel();

        // GET: Member
        [Authorize]
        [HttpGet]
        public ActionResult Index(string Search)
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
            CheckGameID();
            CheckPurchasedGame();
            return View(_context.Games.Where(x => x.GameName.Contains(Search) ||Search==null).ToList());

        }
       

        [HttpPost]
        public ActionResult Index()
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
            string gID = Request["gID"];
            int GameID = Convert.ToInt32(gID);


            TempData["addWishList"] = "Added to Wish List successfully.";
            if (!ModelState.IsValid || ModelState.IsValid)
            {
                using (cvgsEntities2 db = new cvgsEntities2())
                {
                    var gameId = db.Games.Where(a => a.GameID == GameID).FirstOrDefault().GameID;

                    var userId = db.Users.FirstOrDefault(x => x.UserName == System.Web.HttpContext.Current.User.Identity.Name).UserID;
                    bool findWishList = _context.Wishlists.Any(x => (x.GameID == GameID && x.MemberID == userId));
                    if (findWishList)
                    {
                        var wishListId = db.Wishlists.Where(a => a.GameID == GameID && a.MemberID == userId).FirstOrDefault().WishlistID;

                        if (gameId != null)
                        {

                            Wishlist models = db.Wishlists.Find(wishListId);
                            db.Wishlists.Remove(models);
                            db.SaveChanges();

                            TempData["removeWishList"] = "Removed from WishList";
                            ViewBag.removeWishList = TempData["removeWishList"];
                            CheckGameID();
                            CheckPurchasedGame();
                            return View(_context.Games.ToList());
                        }

                    }

                    else
                    {
                        if (gameId != null)
                        {
                            //var wishListId = db.Wishlists.Where(a => a.GameID == GameID && a.MemberID == userId).FirstOrDefault().WishlistID;
                            Wishlist model = new Wishlist();
                            model.GameID = gameId;
                            model.MemberID = userId;
                            db.Wishlists.Add(model);
                            db.SaveChanges();
                            TempData["addWishList"] = "Added to Wish List successfully.";
                            ViewBag.addWishList = TempData["addWishList"];
                            CheckGameID();
                            CheckPurchasedGame();
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
        public ActionResult Details(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game gameDetail = _context.Games.Find(id);
            if (gameDetail == null)
            {
                return HttpNotFound();
            }
            string name = User.Identity.GetUserName();
            User user = _context.Users.FirstOrDefault(x => x.UserName == name);
            Rating rating = new Rating();
            bool rateExist = _context.Ratings.Any(x => x.GameID == id && x.MemberID == user.UserID);
            if (rateExist)
            {
                ViewBag.message = "Edit Rating";
            }
            return View(gameDetail);
        }
        

        [HttpGet]
        public ActionResult Rate(int id)
        {
            var list = new List<string>() { "1", "2", "3", "4", "5" };
            ViewBag.list = list;
            Rating rating = new Rating();
            rating.GameID = id;
            return View(rating);
        }

        [HttpPost]
        public ActionResult Rate(Rating rat, int id)
        {
            var list = new List<string>() { "1", "2", "3", "4", "5" };
            ViewBag.list = list;
            try
            {
                if (ModelState.IsValid)
                {
                    string name = User.Identity.GetUserName();
                    User user = _context.Users.FirstOrDefault(x => x.UserName == name);
                    Rating rating = new Rating();
                   bool rateExist= _context.Ratings.Any(x => x.GameID == id && x.MemberID == user.UserID);
                    // ratingID = _context.Ratings.FirstOrDefault(x => x.GameID == id && x.MemberID == user.UserID).RatingID;

                    if (!rateExist)
                    {
                        rating.Rating1 = rat.Rating1;
                        rating.MemberID = user.UserID;
                        rating.GameID = id;
                        rating.RatingDate = DateTime.Now;
                        _context.Ratings.Add(rating);

                        _context.SaveChanges();
                        var query = from ratings in _context.Ratings
                                    where ratings.GameID == id
                                    select ratings.Rating1;
                        var averageRating = query.Average();
                        var averageRatingRound = Math.Round(averageRating, 2);
                        var gameToUpdate = _context.Games.Find(id);
                       

                            gameToUpdate.GameOverallRating= averageRatingRound;
                        _context.Games.AddOrUpdate(gameToUpdate);
                         //   Add(gameToUpdate);
                       _context.SaveChanges();
                        ViewBag.result = "You gave "+ rat.Rating1+" rating to this game. Overall rating for this game is " + averageRatingRound;
                        ViewBag.message = "Rating done";


                    }
                    else
                    {
                        var query1 = (from ratings in _context.Ratings
                                    where ratings.GameID == id && ratings.MemberID == user.UserID
                                    select ratings.RatingID).Single();
                        int RatingID = Convert.ToInt32(query1);
                        var ratingToUpdate = _context.Ratings.SingleOrDefault(x=>x.RatingID==RatingID);
                          
                      
                        ratingToUpdate.Rating1 = rat.Rating1;
                        ratingToUpdate.RatingDate = DateTime.Now;
                        _context.Ratings.AddOrUpdate(ratingToUpdate);
                        _context.SaveChanges();

                        var query = from ratings in _context.Ratings
                                    where ratings.GameID == id
                                    select ratings.Rating1;

                        var averageRating = query.Average();
                        var averageRatingRound = Math.Round(averageRating, 2);
                        var gameToUpdate = _context.Games.Find(id);
                        gameToUpdate.GameOverallRating = averageRatingRound;
                        _context.Games.AddOrUpdate(gameToUpdate);
                        
                        _context.SaveChanges();
                        ViewBag.result = "Your Edited Rating is  " + rat.Rating1 + " to this game. Overall rating for this game is " + averageRatingRound;
                        ViewBag.message = "Rating done";
                        

                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
            Rating rating1 = new Rating();
            rating1.GameID = id;
            return View(rating1);
           // return View();
        }
        public void CheckPurchasedGame()
        {
            var ownedGameList = new ArrayList();
            using (cvgsEntities2 db = new cvgsEntities2())
            {
                var userId = db.Users.FirstOrDefault(x => x.UserName == System.Web.HttpContext.Current.User.Identity.Name).UserID;

                foreach (var order in db.Orders)
                {
                    if (order.MemberID == userId)
                    {
                        foreach (var game in db.OrderDetails)
                        {
                            if (game.OrderID == order.OrderID)
                            {
                                ownedGameList.Add(game.GameID);
                            }
                        }
                    }
                }
            }
            TempData["OwnedGames"] = ownedGameList.ToArray(typeof(object));
        }

        public void CheckGameID()
        {

            var arrList = new ArrayList();
            using (cvgsEntities2 db = new cvgsEntities2())
            {
                var userId = db.Users.FirstOrDefault(x => x.UserName == System.Web.HttpContext.Current.User.Identity.Name).UserID;

                foreach (var item in db.Wishlists)
                {

                    if (item.MemberID == userId)
                    {
                        if (item.GameID != null)
                        {

                            arrList.Add(item.GameID);

                        }
                    }
                    else
                    {
                        arrList.Add("False");
                    }

                }
            }
            TempData["CheckGameIdTrue"] = arrList.ToArray(typeof(object));
        }

        [HttpGet, Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel changePasswordModel)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                using (cvgsEntities2 db = new cvgsEntities2())
                {
                    var userId = db.Users.FirstOrDefault(x => x.UserName == System.Web.HttpContext.Current.User.Identity.Name).UserID;
                    //var user = db.Users.Where(a => a.UserID == changePasswordModel.UserID).FirstOrDefault();
                    var user = db.Users.Where(a => a.UserID == userId && a.UserPassword == changePasswordModel.CurrentPassword).FirstOrDefault();
                    if (user != null)
                    {
                        user.UserPassword = changePasswordModel.NewPassword;
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                        message = "New password updated successfully.";

                    }
                    else
                    {
                        message = "Password not Updated!";
                    }
                }
            }

            ViewBag.Message = message;
            return View();
        }
       
    }
}