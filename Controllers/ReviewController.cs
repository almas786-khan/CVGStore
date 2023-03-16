using CVGS.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace CVGS.Controllers
{

    public class ReviewController : Controller
    {
        cvgsEntities2 _context = new cvgsEntities2();
       
        // GET: Review
        [HttpGet]
        public ActionResult Index(int id)
        {
            if (TempData.ContainsKey("message"))
            {
                TempData["message"] = "Review submitted for Admin's approval";
                ViewBag.Message = TempData["message"];
            }
            string name = User.Identity.GetUserName();
            List<ReviewViewModel> list = new List<ReviewViewModel>();
            var result = from game in _context.Games
                         join review in _context.Reviews on game.GameID equals id
                         join use in _context.Users on review.MemberID equals use.UserID
                         where review.GameID == id && review.Status==true
                         select new ReviewViewModel
                         {
                             GameID = review.GameID,
                             Review1 = review.Review1,
                             ReviewDate = review.ReviewDate,
                             Name = use.UserName
                         };

            list = result.ToList();
            if (list.Count == 0)
            {
                ViewBag.result = "EmptyList";
                var query = from review in _context.Games
                            where review.GameID == id
                            select new ReviewViewModel
                            {
                                GameID = id,
                                Review1 = "",
                                ReviewDate = DateTime.Now,
                                Name=""
                };
               list = query.ToList();
            }
            return View(list);
        }

        [HttpGet]
        public ActionResult AddReview(int id)
        {
            List<ReviewViewModel> list = new List<ReviewViewModel>();
            string name = User.Identity.GetUserName();
            CVGS.Models.User user = _context.Users.FirstOrDefault(x => x.UserName == name);
            bool findReview = _context.Reviews.Any(x => x.GameID == id && x.MemberID == user.UserID && x.Status == true);
            bool findReviewInProgress = _context.Reviews.Any(x => x.GameID == id && x.MemberID == user.UserID && x.Status == false);
            if (findReviewInProgress)
            {
                ViewBag.message = "Review is in progress you can't edit now.";
            }
            if (findReview)
            {
                ViewBag.message = "Edit your previous review.";
                var review1 = _context.Reviews.Where(x => x.GameID == id && x.MemberID == user.UserID && x.Status == true);
                var result = from review in _context.Reviews
                             join use in _context.Users on review.MemberID equals use.UserID
                             where review.GameID == id && review.Status == true && review.MemberID == user.UserID
                             select new ReviewViewModel
                             {
                                 GameID = review.GameID,
                                 Review1 = review.Review1,
                                 ReviewDate = review.ReviewDate,
                                 Name = user.UserName
                             };
                list = result.ToList();
                return View(list);
            }
            if (!findReview && !findReviewInProgress)
            {
               
                var query = from review in _context.Games
                            where review.GameID == id
                            select new ReviewViewModel
                            {
                                GameID = id,
                                Review1 = "",
                                ReviewDate = DateTime.Now,
                                Name = ""
                            };
                list = query.ToList();
                return View(list);
            }
            return View(list);
        }
        [HttpPost]
        public ActionResult AddReview(IEnumerable<ReviewViewModel> model,int id)
        {
            
            string name = User.Identity.GetUserName();
            CVGS.Models.User user = _context.Users.FirstOrDefault(x => x.UserName == name);

            if (ModelState.IsValid)
            {

                //var userId = _context.Users.FirstOrDefault(x => x.UserName == System.Web.HttpContext.Current.User.Identity.Name).UserID;
                bool findReview = _context.Reviews.Any(x => x.GameID == id && x.MemberID == user.UserID && x.Status == true);
                //bool findReviewInProgress = _context.Reviews.Any(x => x.GameID == id && x.MemberID == user.UserID && x.Status == false);


                if (findReview)
                {
                    var reviewID = _context.Reviews.FirstOrDefault(x => x.GameID == id && x.MemberID == user.UserID && x.Status == true).ReviewID;

                    var reviewToUpdate = _context.Reviews.SingleOrDefault(m => m.ReviewID == reviewID);
                    reviewToUpdate.ReviewDate = DateTime.Now;
                    reviewToUpdate.Review1 = model.ToList().FirstOrDefault(x=>x.GameID==id).Review1;
                    reviewToUpdate.Status = false ;
                    _context.Reviews.AddOrUpdate(reviewToUpdate);
                    _context.SaveChanges();
                    TempData["message"] = "Review submitted for Admin's approval";

                }
                
                else
                {
                    Review review = new Review();
                    review.MemberID = user.UserID;
                    review.GameID = id;
                    review.ReviewDate = System.DateTime.Now;
                    review.Review1 = model.ToList().FirstOrDefault(x => x.GameID == id).Review1;
                    review.Status = false;
                    _context.Reviews.Add(review);
                    _context.SaveChanges();
                    TempData["message"] = "Review submitted for Admin's approval";
                }
            }
            
            return RedirectToAction("Index", new {id=id});
            //return View();
        }
        [HttpGet]
        public ActionResult Evaluate()
        {
            if (TempData.ContainsKey("message"))
            {
                TempData["message"] = "Evaluation Done!!";
                ViewBag.Message = TempData["message"];
            }
            List<ReviewViewModel> list = new List<ReviewViewModel>();
            var result = from review in _context.Reviews
                         //join review in _context.Reviews on game.GameID equals id
                         join use in _context.Users on review.MemberID equals use.UserID
                         where review.Status == false
                         select new ReviewViewModel
                         {
                             GameID = review.GameID,
                             Review1 = review.Review1,
                             ReviewDate = review.ReviewDate,
                             Name = use.UserName,
                             ReviewID=review.ReviewID
                         };

            list = result.ToList();
            return View(list);
        }
        [HttpPost]
        public ActionResult Evaluate(string submit)
        {
            var reviewID= Request["gID"];
            int reviewId =Convert.ToInt32(reviewID);
            var reviewToUpdate =_context.Reviews.SingleOrDefault(x=>x.ReviewID == reviewId);
            if (submit == "Approve")
            {
                reviewToUpdate.Status = true;
                _context.Reviews.AddOrUpdate(reviewToUpdate);
                _context.SaveChanges();
                TempData["message"] = "Evaluation Done!!";
            }
            if (submit=="Reject")
            {
                //Review models = _context.Reviews.Find(reviewId);
                _context.Reviews.Remove(reviewToUpdate);
                _context.SaveChanges();
                TempData["message"] = "Evaluation Done!!";

            }
            return RedirectToAction("Evaluate");
        }

    }
}