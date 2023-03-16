using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using CVGS.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

namespace CVGS.Controllers
{
    public class ProfileController : Controller
    {
        cvgsEntities2 _context = new cvgsEntities2();
        // GET: Profile
        public ActionResult Index()
        {
            string name = User.Identity.GetUserName();
            User user = _context.Users.FirstOrDefault(x => x.UserName == name);
            if (user.UserFirstName != null || user.UserLastName!= null || user.Birthdate != null || user.Gender!= null)
            {
                ViewBag.message = "Profile Exists";
            }
            ProfileViewModel user1= new ProfileViewModel();
            user1.UserID= user.UserID;
            return View(user1);
        }
        [HttpGet]
        public ActionResult Create()
        { 
            return View();
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = " UserLastName, UserFirstName,Gender, Birthdate")] ProfileViewModel mymodel, string IsEmail)
        {
            DateTime date = (DateTime)mymodel.Birthdate;
            date = date.AddYears(18);
            try
            {
                if (!(date <= DateTime.Now))
                {
                    ModelState.AddModelError("Birthdate", "You should be 18 years old");
                }
                //else
                //{
                    if (ModelState.IsValid)
                    {
                        string name = User.Identity.GetUserName();
                        User user = _context.Users.FirstOrDefault(x => x.UserName == name);
                        user.UserLastName = mymodel.UserLastName;
                        user.UserFirstName = mymodel.UserFirstName;
                        user.Gender = mymodel.Gender;
                        user.Birthdate = mymodel.Birthdate;
                        if (IsEmail == "True" || IsEmail == "true")
                        {
                            user.IsEmail = true;
                        }
                        else
                        {
                            user.IsEmail = false;
                        }
                        _context.Users.AddOrUpdate(user);
                        _context.SaveChanges();
                        ViewBag.result = "Profile Created.";
                        return View();
                    }
               // }
               
            }
          

            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(mymodel);
        }

        [HttpGet]
        public ActionResult Modify()
        {
            string name = User.Identity.GetUserName();
            User user = _context.Users.FirstOrDefault(x => x.UserName == name);
            ProfileViewModel user1 = new ProfileViewModel();
            if (user.UserFirstName != null || user.UserLastName != null || user.Birthdate != null || user.Gender != null)
            {
                ViewBag.message = "Profile Exists";
                user1.UserID = user.UserID;
                user1.UserFirstName = user.UserFirstName;
                user1.UserLastName = user.UserLastName;
                user1.Birthdate = user.Birthdate;
                user1.Gender = user.Gender;
                user1.IsEmail = user.IsEmail;

            }
            else
            {
                user1.UserID = user.UserID;
            }

           
           
            return View(user1);
            //return View(user);
        }

        [HttpPost]
        public ActionResult Modify(ProfileViewModel mymodel, string IsEmail)
        {
            string name = User.Identity.GetUserName();
            User user = _context.Users.FirstOrDefault(x => x.UserName == name);
            if (mymodel.Birthdate != null)
            {
                DateTime date = (DateTime)mymodel.Birthdate;
                date = date.AddYears(18);

                if (!(date <= DateTime.Now))
                {
                    ModelState.AddModelError("Birthdate", "You should be 18 years old");
                }
            }
            
                
                if (ModelState.IsValid)
                {


                    user.UserFirstName = mymodel.UserFirstName;
                    user.UserLastName = mymodel.UserLastName;
                    user.Gender = mymodel.Gender;
                    user.Birthdate = mymodel.Birthdate;
                    if (IsEmail == "True" || IsEmail == "true")
                    {
                        user.IsEmail = true;
                    }
                    else
                    {
                        user.IsEmail = false;
                    }
                    _context.Users.AddOrUpdate(user);
                    _context.SaveChanges();
                    ViewBag.result = "Modified successfully.";

                }

            
           
            return View(mymodel);
        }
    }
}