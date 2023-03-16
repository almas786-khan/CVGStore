using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CVGS.Models;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web.Helpers;
using System.Data;
using CaptchaMvc.HtmlHelpers;
using System.Web.Services.Description;

namespace CVGS.Controllers
{
    public class LoginController : Controller
    {
        cvgsEntities2 _context = new cvgsEntities2();
        // GET: Login

        public ActionResult Index() { 
        return View();
        }
        //private int logAttempt;
        // GET: Accounts
        public ActionResult Login()
        {
            LoginViewModel user = new LoginViewModel();
            return View(user);
        }

        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {

            bool userExist = _context.Users.Any(x => x.UserName == model.UserName && x.UserPassword == model.UserPassword);
            User user = _context.Users.FirstOrDefault(x => x.UserName == model.UserName && x.UserPassword == model.UserPassword);

            if (userExist)
            {
                    if (user.UserType == "Admin")
                    {
                        FormsAuthentication.SetAuthCookie(user.UserName, false);
                        return RedirectToAction("Index", "Admin");
                    }
                else
                {
                    FormsAuthentication.SetAuthCookie(user.UserName, false);
                    return RedirectToAction("Index", "Member");
                }
               
            }
            else
            {
                var logAttemptsValue = TempData["logAttempts"];
                if (TempData["logAttempts"] == null)
                {
                    TempData["logAttempts"] = 1;
                    ViewBag.LogAttempts = TempData["logAttempts"];
                    TempData.Keep("logAttempts");
                    ModelState.AddModelError("", "Username or Password is wrong");

                }
                else
                {
                    if (logAttemptsValue.ToString() == "3")
                    {
                        TempData["logAttempts"] = null;
                        return RedirectToAction("LoginAttempts", "Login");
                    }
                    else if (logAttemptsValue.ToString() != null)
                    {
                        TempData["logAttempts"] = int.Parse(logAttemptsValue.ToString()) + 1;
                        ViewBag.LogAttempts = TempData.Peek("logAttempts");
                        ModelState.AddModelError("", "Username or Password is wrong");

                    }

                }

            }

            return View();
        }


        [AllowAnonymous]
        public ActionResult LoginAttempts()
        {
            ViewBag.Message = "Login failed.";

            return View();
        }





        [HttpPost]
        public ActionResult Signup(User userInfo)
        {

            _context.Users.Add(userInfo);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            Session.Remove("Cart");
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode, string emailFor = "ConfirmAccount")
        {
            var verifyUrl = "/Login/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            //dotnetawesome@gmail.com
            var fromEmail = new MailAddress("bandhandyandr@gmail.com", "Dotnet Awesome");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "zbxp ernx lpie xpol"; // Replace with actual password

            string subject = "";
            string body = "";
            if (emailFor == "ConfirmAccount")
            {
                subject = "Your account is successfully created!";
                body = "<br/><br/>We are excited to tell you that your Dotnet Awesome account is" +
                    " successfully created. Please click on the below link to verify your account" +
                    " <br/><br/><a href=" + link + ">Confirm account link</a> ";

            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Hi,<br/>We got request for reset your account password." +
                    "<br/> Here is New Password: " + activationCode +
                    "<br/> Please click on the below link to reset your password" +
                    "<br/><br/><a href=" + link + ">Reset Password link</a>";
            }


            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)

            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

        public ActionResult ConfirmAccount()
        {
            ViewBag.Message = "Confirm your Account.";

            return View();
        }

        public ActionResult AfterRegistration()
        {
            ViewBag.Message = "Confirmed Registration link has been sent to your email.";
            return View();
        }

        //Part 3 - Forgot Password

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword( string EmailID)
        {
            //Verify Email ID
            //Generate Reset password link 
            //Send Email 
            string message = "";
            //bool status = false;

            using (cvgsEntities2 dc = new cvgsEntities2())
            {
                var account = dc.Users.Where(a => a.UserEmail == EmailID).FirstOrDefault();
                if (account != null)
                {
                    //Send email for reset password
                    string resetCode = Guid.NewGuid().ToString();
                    SendVerificationLinkEmail(account.UserEmail, resetCode, "ResetPassword");
                    account.ResetPasswordCode = resetCode;
                    //This line I have added here to avoid confirm password not match issue , as we had added a confirm password property 
                    //in our model class in part 1
                    dc.Configuration.ValidateOnSaveEnabled = false;
                    dc.SaveChanges();
                    message = "Reset password link has been sent to your email id.";
                }
                else
                {
                    message = "Account not found";
                }
            }
            ViewBag.Message = message;
            return View();
        }

        public ActionResult ResetPassword(string id)
        {
            //Verify the reset password link
            //Find account associated with this link
            //redirect to reset password page
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            using (cvgsEntities2 dc = new cvgsEntities2())
            {
                var user = dc.Users.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
                if (user != null)
                {
                    ResetPasswordModel model = new ResetPasswordModel();
                    model.ResetCode = id;
                    return View(model);
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                using (cvgsEntities2 dc = new cvgsEntities2())
                {
                    var user = dc.Users.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                    if (user != null)
                    {
                        //user.UserPassword = Crypto.Hash(model.NewPassword);
                        user.UserPassword = model.NewPassword;
                        user.ResetPasswordCode = "";
                        dc.Configuration.ValidateOnSaveEnabled = false;
                        dc.SaveChanges();
                        message = "New password updated successfully";

                    }
                }

            }
            else
            {
                message = "Something invalid";

            }
            ViewBag.Message = message;
            return View(model);

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
            string message = "";
            try
            {
                //cvgsEntities _context = new cvgsEntities();
                if (this.IsCaptchaValid("Capctha is  valid"))
                {
                    if (ModelState.IsValid)
                    {
                        User user = new User();
                        UserRole userRole = new UserRole();
                        user.UserName = model.UserName;
                        user.UserEmail = model.UserEmail;
                        user.UserPassword = model.UserPassword;
                        user.UserType = "Member";
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
                        userRole.UserID= user.UserID;
                        userRole.Role = user.UserType;
                        _context.UserRoles.Add(userRole);   
                        _context.SaveChanges();
                        SendVerificationLinkEmail(user.UserEmail, user.UserID.ToString(), "ConfirmAccount");
                        message = "Confirmed Registration link has been sent to your email.";
                        ViewBag.Message = message;


                          return RedirectToAction("AfterRegistration", "Login");
                       
                       
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
            return Json(!_context.Users.Any(x => x.UserName == UserName), JsonRequestBehavior.AllowGet);
        }

    }
}