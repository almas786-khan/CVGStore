using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CVGS.Models;
using Microsoft.Ajax.Utilities;
using CaptchaMvc.HtmlHelpers;
using System.Web.Services.Description;

namespace CVGS.Controllers
{
    public class RegisterController : Controller
    {
       cvgsEntities2 _context = new cvgsEntities2();
        // GET: Member
        public ActionResult Index()
        {
            ViewBag.Message = "Registration Successfull";

            return View();
        }
  

        [HttpGet]
        public ActionResult Register()
        {
            RegisterUserModel user = new RegisterUserModel();
            return View(user);
        }
        [HttpPost]
        public ActionResult Register(RegisterUserModel model)
        {
            try
            {
              
                //cvgsEntities _context = new cvgsEntities();
                if (this.IsCaptchaValid("Capctha is  valid"))
                {
                    if (ModelState.IsValid)
                    {
                        User user = new User();
                        user.UserName = model.UserName;
                        user.UserEmail = model.UserEmail;
                        user.UserPassword = model.UserPassword;
                        user.UserType = model.UserType;
                        _context.Users.Add(user);
                        try
                        {
                            _context.SaveChanges();
                        }
                        catch (System.Data.Entity.Validation.DbEntityValidationException e)
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                foreach (var validationError in eve.ValidationErrors)
                                {
                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                                }
                            }
                        }
                       


                        return RedirectToAction("Login", "Login");
                    }
                    else
                    {

                        return View();
                    }
                }
                else
                {
                    ViewBag.ErrMessage = "Error: captcha is not valid.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                Response.Write("Property: " + ex.Message);
                return View();
            }

        }

        //newly added code
        public JsonResult IsUserExists(string UserName)
        {

            //check if any of the UserName matches the UserName specified in the Parameter using the ANY extension method.  
            return Json(_context.Users.Any(x => x.UserName == UserName), JsonRequestBehavior.AllowGet);
        }

    }
}