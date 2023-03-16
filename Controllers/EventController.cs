using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CVGS.Models;
using Microsoft.AspNet.Identity;

namespace CVGS.Controllers
{
    public class EventController : Controller
    {
        cvgsEntities2 _context = new cvgsEntities2();

        // GET: Event
        [HttpGet]
        public ActionResult Index()
        {
            var listodData = _context.Events.ToList();
            var userId = _context.Users.FirstOrDefault(x => x.UserName == System.Web.HttpContext.Current.User.Identity.Name);
            string userType=userId.UserType;
            if (userType =="Member")
            {
                ViewBag.message = "Member";
                CheckGameID();
            }
            return View(listodData);

        }
        [HttpPost]
        public ActionResult Index(string id)
        {
            string eID = Request["eID"];
            int EventID = Convert.ToInt32(eID);
            var listodData = _context.Events.ToList();
            string name = User.Identity.GetUserName();
            CVGS.Models.User user = _context.Users.FirstOrDefault(x => x.UserName == name);
            if (user.UserType =="Member")
            {
                ViewBag.message = "Member";
                CheckGameID();
            }
            try
            {
                if (ModelState.IsValid)
                {
                    bool findEvent = _context.EventRegistrations.Any(x => (x.MemberID == user.UserID && x.EventID == EventID));
                    if (findEvent)
                    {
                        var eventRegistrationId = _context.EventRegistrations.Where(a => a.MemberID == user.UserID && a.EventID == EventID).FirstOrDefault().EventRegistrationID;
                        EventRegistration model = _context.EventRegistrations.Find(eventRegistrationId);
                        _context.EventRegistrations.Remove(model);
                        _context.SaveChanges();
                        TempData["removeWishList"] = "Registration cancelled";
                        ViewBag.removeWishList = TempData["removeWishList"];
                        CheckGameID();
                        
                        return View(listodData);
                    }
                    else
                    {
                        EventRegistration eventRegister = new EventRegistration();
                        eventRegister.MemberID = user.UserID;
                        eventRegister.EventID = EventID;
                        _context.EventRegistrations.Add(eventRegister);
                        _context.SaveChanges();

                        TempData["AddEvent"] = "Event Registration is successful.";
                        CheckGameID();
                        ViewBag.AddEvent = TempData["AddEvent"];
                        
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
            return View(listodData);


        }
        public void CheckGameID()
        {

            var arrList = new ArrayList();
            using (cvgsEntities2 db = new cvgsEntities2())
            {
                var userId = db.Users.FirstOrDefault(x => x.UserName == System.Web.HttpContext.Current.User.Identity.Name).UserID;

                foreach (var item in db.EventRegistrations)
                {

                    if (item.MemberID == userId)
                    {
                        if (item.EventID != null)
                        {

                            arrList.Add(item.EventID);

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

       
        [HttpGet]
        public ActionResult AddEvent()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddEvent([Bind(Include = " EventName, EventDescription,EventDate, ImageURL, ImageFile")] Event model, HttpPostedFileBase ImageFile)
        {
            var fileExt = "";
            if (ImageFile != null)
            {
                fileExt = System.IO.Path.GetExtension(ImageFile.FileName).Substring(1);
            }

            var supportedTypes = new[] { "jpg", "jpeg", "png" };
            try
            {
                if (model.EventDate < DateTime.Now)
                {
                    ModelState.AddModelError("EventDate", "Please provide future event date");
                }
                if (ImageFile == null)
                {
                    ViewBag.result = "Image is required, so upload an image";
                }
                else if (ImageFile != null && ((ImageFile.ContentLength / 1024) < 10 || (ImageFile.ContentLength / 1024 > 300)))
                {
                    ViewBag.result = "file size should be in range of 10kb and 300kb";
                }
                else if (!supportedTypes.Contains(fileExt))
                {
                    ViewBag.result = "File Extension Is InValid - Only Upload jpg/png/jpeg/gif File";
                }

                else
                {
                    if (ModelState.IsValid)
                    {

                        string filename = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                        string extension = Path.GetExtension(model.ImageFile.FileName);
                        filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                        model.ImageURL = "~/Image/" + filename;
                        filename = Path.Combine(Server.MapPath("~/Image/"), filename);
                        model.ImageFile.SaveAs(filename);
                        _context.Events.Add(model);
                        _context.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View();
        }


        [HttpGet]
        public ActionResult EditEvent(int id)
        {
            var data = _context.Events.Where(x => x.EventID == id).FirstOrDefault();
            return View(data);
        }

       
        [HttpPost, ActionName("EditEvent")]
        public ActionResult EditEvent(Event model, int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var eventToUpdate = _context.Events.Find(id);
            if (TryUpdateModel(eventToUpdate, "",
               new string[] { "EventName", "EventDescription","EventDate", "ImageURL" }))
            {
                try
                {
                    _context.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(eventToUpdate);
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event eventToUpdate = _context.Events.Find(id);
            if (eventToUpdate == null)
            {
                return HttpNotFound();
            }
            return View(eventToUpdate);
        }

        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            Event event1 = _context.Events.Find(id);
            if (event1 == null)
            {
                return HttpNotFound();
            }
            return View(event1);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                Event eventToUpdate = _context.Events.Find(id);
                _context.Events.Remove(eventToUpdate);
                _context.SaveChanges();

            }
            catch (DataException/* dex */)
            { 
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
                            }
            return RedirectToAction("Index");
        }
      
        public ActionResult Report()
        {
            ViewBag.Message = "Reports can be viewed and downloaded from here.";

            return View();
        }


    }
}
    
