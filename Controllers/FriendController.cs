using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CVGS.Models;
using Microsoft.AspNet.Identity;
using static System.Net.Mime.MediaTypeNames;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace CVGS.Controllers
{
    public class FriendController : Controller
    {
        // GET: Friend
        cvgsEntities2 _context = new cvgsEntities2();
        public ActionResult Index()
        {
            List<friendViewModel> list = new List<friendViewModel>();
            string name = User.Identity.GetUserName();
            CVGS.Models.User user1 = _context.Users.FirstOrDefault(x => x.UserName == name);

            var result = from friend in _context.Friends
                         join use in _context.Users on friend.Member2ID equals use.UserID
                         where friend.Member1ID == user1.UserID && friend.IsApprove == true
                         select new friendViewModel
                         {
                             Member1ID = user1.UserID,
                             Member1Name = name,
                             Member2ID = friend.Member2ID,
                             Member2Name = use.UserName
                         };
            list = result.ToList();

            return View(list);
        }
        [HttpPost]
        public ActionResult WishList(string id)
        { 
            return View();
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            List<WishListView> list = new List<WishListView>();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CVGS.Models.User user = _context.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var query = from wishlist in _context.Wishlists
                        join game in _context.Games on wishlist.GameID equals game.GameID
                        where wishlist.MemberID == id
                        select new WishListView
                        {
                            MemberID = wishlist.MemberID,
                            MemberName = user.UserName,
                            GameID = wishlist.GameID,
                            GameName =  game.GameName,
                            ImageURL=game.ImageURL,
                        };
            list = query.ToList();
            if (list.Count == 0)
            {
                ViewBag.result = "Nothing in WishList";
            }
            return View(list);
        }
        public void CheckFriendID()
        {
            var arrFriendListForRequest = new ArrayList();
            var arrFriendListForApprove = new ArrayList();
            var arrFriendListForFriendEachOther = new ArrayList();
            using (cvgsEntities2 db = new cvgsEntities2())
            {
                var userId = db.Users.FirstOrDefault(x => x.UserName == System.Web.HttpContext.Current.User.Identity.Name).UserID;
                //bool checkFriendsEachOther = db.Friends.Any(a => a.FriendId == userId && a.IsApprove == 1);
                foreach (var item in db.Friends)
                {

                    if (item.Member1ID == userId && item.IsApprove==false)
                    {
                        if (item.Member1ID == userId && item.RequestSent == true)// request sent
                        {
                            //for add friend button
                            arrFriendListForRequest.Add(item.Member2ID + "r" + 0);
                        }
                        else if (item.Member1ID == userId && item.RequestSent == false)//request received
                        {
                            //for request sent button
                            arrFriendListForRequest.Add(item.Member2ID + "r" + 1);

                        }

                    }
                    if (item.Member1ID == userId && item.IsApprove == true) // if they are already friends they can unfriend
                    {
                        arrFriendListForFriendEachOther.Add(item.Member2ID + "e" + 3);
                    }

                }


            }
            TempData["CheckRequest"] = arrFriendListForRequest.ToArray(typeof(object));
            TempData["CheckApprove"] = arrFriendListForApprove.ToArray(typeof(object));
            TempData["CheckFriendEachOther"] = arrFriendListForFriendEachOther.ToArray(typeof(object));
        }
       
        [HttpGet]
        public ActionResult AddFriend(string Search) {
            CheckFriendID();
            string name = User.Identity.GetUserName();
            CVGS.Models.User user = _context.Users.FirstOrDefault(x => x.UserName == name);
            //_context.Friends.Where(a => a.Member1ID == user.UserID && a.IsApprove == false);

            if (Search == null)
            {
                return View(_context.Users.Where(x => (x.UserName != name && x.UserType == "Member")).ToList());
            }
            bool findFriend = _context.Users.Any(x => ((x.UserName.Contains(Search) && x.UserType == "Member") || x.UserName != name));
            if (findFriend)
            {
                return View(_context.Users.Where(x => (x.UserName.Contains(Search) && x.UserType == "Member" && x.UserName != name)).ToList());


            }

            return View();
            
        }

        [HttpPost]
        public ActionResult AddFriend(string btnSubmits, friendViewModel model) {
            string fID = Request["fID"];
            int FriendID = Convert.ToInt32(fID);
            string name = User.Identity.GetUserName();
            CVGS.Models.User user = _context.Users.FirstOrDefault(x => x.UserName == name);

            bool findFriend = _context.Friends.Any(x => (x.Member1ID == user.UserID && x.Member2ID == FriendID && x.IsApprove == true) || (x.Member1ID == FriendID && x.Member2ID == user.UserID && x.IsApprove == true));
            bool findFriend1 = _context.Friends.Any(x => (x.Member1ID == user.UserID && x.Member2ID == FriendID && x.IsApprove == false) || (x.Member1ID == FriendID && x.Member2ID == user.UserID && x.IsApprove == false));

            if (btnSubmits == "Friend Request Received")
            {
                return RedirectToAction("FriendRequest");
            }
            if (btnSubmits == "Add Friend")
            {
                Friend friend = new Friend();
                friend.Member1ID = user.UserID;
                friend.Member2ID = FriendID;
                friend.RequestSent = true;
                friend.IsApprove = false;
                _context.Friends.Add(friend);
                _context.SaveChanges();
                Friend friend1 = new Friend();
                friend1.Member1ID = FriendID;
                friend1.Member2ID = user.UserID;
                friend1.RequestSent = false;
                friend1.IsApprove = false;
                _context.Friends.Add(friend1);
                _context.SaveChanges();
                CheckFriendID();
                TempData["friendAdded"] = "Friend request sent successfully.";
                ViewBag.friendAdded = TempData["friendAdded"];
                return View(_context.Users.Where(x => (x.UserName != name && x.UserType == "Member")).ToList());

            }
            if (btnSubmits == "Friend Request Sent")
            {
                TempData["RequestSent"] = "Friend Request Already sent. Waiting for approval";
                ViewBag.RequestSent = TempData["RequestSent"];
                CheckFriendID();
                return View(_context.Users.Where(x => (x.UserName != name && x.UserType == "Member")).ToList());

            }
            if (btnSubmits == "UnFriend")
            {
                if (findFriend)
                {
                    var friendId1 = _context.Friends.Where(a => a.Member1ID == user.UserID && a.Member2ID == FriendID).FirstOrDefault().FriendID;
                    var friendId2 = _context.Friends.Where(a => a.Member2ID == user.UserID && a.Member1ID == FriendID).FirstOrDefault().FriendID;
                    if (user.UserID != null)
                    {

                        Friend models = _context.Friends.SingleOrDefault(x => x.FriendID == friendId1);
                        _context.Friends.Remove(models);
                        _context.SaveChanges();
                        Friend models1 = _context.Friends.SingleOrDefault(x => x.FriendID == friendId2);
                        _context.Friends.Remove(models1);
                        _context.SaveChanges();

                        TempData["removedFriend"] = "Removed from Friendlist";
                        ViewBag.removedFriend = TempData["removedFriend"];
                        CheckFriendID();
                        //return View(_context.Users.ToList());
                        return View(_context.Users.Where(x => (x.UserName != name && x.UserType == "Member")).ToList());

                    }
                }

            }
            return View();
        }
        
        public ActionResult FriendRequest()
        {
            string name = User.Identity.GetUserName();
            CVGS.Models.User user = _context.Users.FirstOrDefault(x => x.UserName == name);
            List<friendViewModel> list = new List<friendViewModel>();
            bool friendRequestUpdate = _context.Friends.Any(a => a.RequestSent == false && a.Member1ID == user.UserID);
            if (friendRequestUpdate)
            {
                var friendRequestUpdate1 = _context.Friends.Where(a => a.RequestSent == false && a.Member1ID == user.UserID);
                var result = from friend in _context.Friends
                             join use in _context.Users on friend.Member2ID equals use.UserID
                             where friend.Member1ID == user.UserID && friend.RequestSent == false && friend.IsApprove == false
                             select new friendViewModel
                             {
                                 Member1ID = user.UserID,
                                 Member1Name = name,
                                 Member2ID = friend.Member2ID,
                                 Member2Name = use.UserName,
                                 RequestSent = friend.RequestSent,
                                 IsApprove=friend.IsApprove
                                 
                             };
                list = result.ToList();

                
            }
            
            return View(list);

        }
        [HttpPost]
        public ActionResult Evaluate(string submit)
        {
            var Member2Id = Request["gID"];
            int Member2ID = Convert.ToInt32(Member2Id);
            string name = User.Identity.GetUserName();
            CVGS.Models.User user = _context.Users.FirstOrDefault(x => x.UserName == name);
            var reviewToUpdate1 = _context.Friends.SingleOrDefault(x => x.Member2ID == Member2ID && x.Member1ID==user.UserID && x.RequestSent==false && x.IsApprove==false);
            var reviewToUpdate2 = _context.Friends.SingleOrDefault(x => x.Member1ID == Member2ID && x.Member2ID == user.UserID && x.RequestSent == true && x.IsApprove == false);
            if (submit == "Accept")
            {
                reviewToUpdate1.IsApprove = true;
                _context.Friends.AddOrUpdate(reviewToUpdate1);
                _context.SaveChanges();
                reviewToUpdate2.IsApprove = true;
                _context.Friends.AddOrUpdate(reviewToUpdate2);
                _context.SaveChanges();
                TempData["message"] = "Evaluation Done!!";
                CheckFriendID();
            }
            if (submit == "Reject")
            {
                //Review models = _context.Reviews.Find(reviewId);
                _context.Friends.Remove(reviewToUpdate1);
                _context.SaveChanges();
                _context.Friends.Remove(reviewToUpdate2);
                _context.SaveChanges();
                TempData["message"] = "Evaluation Done!!";
                CheckFriendID();

            }
            return RedirectToAction("Index");

        }

    }
}