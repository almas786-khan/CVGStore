using CVGS.Models;
using Microsoft.AspNet.Identity;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Platform = CVGS.Models.Platform;

namespace CVGS.Controllers
{
    public class PreferenceController : Controller
    {
        // GET: Preference
        cvgsEntities2 db = new cvgsEntities2();
        public ActionResult Index()
        {
            string name = User.Identity.GetUserName();
            User user = db.Users.FirstOrDefault(x => x.UserName == name);
            int id =user.UserID;
            return RedirectToAction("ViewPreference" , new { id = id });
           // return View();
        }
        public ActionResult ViewPreference(int id)
        {
            

            PreferenceViewModel mymodel = new PreferenceViewModel();
            mymodel.Users = db.Users.Find(id);
            //platform
            List<PlatformPreference> platformPreferences = db.PlatformPreferences.Where(x => x.UserId == id).ToList();
            List < Platform > platforms = db.Platforms.ToList();
            mymodel.PlatformPreferences = platformPreferences.Select(x => new PlatformPreferenceViewModel
            {
                PlatformId = x.PlatformId,
                PlatformName = x.Platform.PlatformName,
                IsChecked = platformPreferences.Any(y => y.Platform.Id == x.Id)
            }).ToList();
            //category
            List<CategoryPreference> categoryPreferences = db.CategoryPreferences.Where(x => x.UserId == id).ToList();
            List<Category> categories = db.Categories.ToList();
            mymodel.CategoryPreferences = categoryPreferences.Select(x => new CategoryPreferenceViewModel
            {
                CategoryId = x.CategoryId,
                CategoryName = x.Category.CategoryName,
                IsChecked = categoryPreferences.Any(y => y.Category.Id == x.Id)
            }).ToList();
            return View(mymodel);
        }
        [HttpGet]
        public ActionResult ModifyPreference(int id)
        {
            PreferenceViewModel mymodel = new PreferenceViewModel();
            mymodel.Users = db.Users.Find(id);
            //platform
            List<PlatformPreference> platformPreferences = db.PlatformPreferences.Where(x => x.UserId == id).ToList();
            List<Platform> platforms = db.Platforms.ToList();
            mymodel.PlatformPreferences = platforms.Select(x => new PlatformPreferenceViewModel
            {
                PlatformId = x.Id,
                PlatformName = x.PlatformName,
                IsChecked = platformPreferences.Any(y => y.Platform.Id == x.Id)
            }).ToList();
            //category
            List<CategoryPreference> categoryPreferences = db.CategoryPreferences.Where(x => x.UserId == id).ToList();
            List<Category> categories = db.Categories.ToList();
            mymodel.CategoryPreferences = categories.Select(x => new CategoryPreferenceViewModel
            {
                CategoryId = x.Id,
                CategoryName = x.CategoryName,
                IsChecked = categoryPreferences.Any(y => y.Category.Id == x.Id)
            }).ToList();
            return View(mymodel);
        }

        [HttpPost]
        public ActionResult ModifyPreference(PreferenceViewModel mymodel, int id)
        {
            //platform
            List<PlatformPreference> platformPreferences = db.PlatformPreferences.Where(x => x.UserId == id).ToList();

            List<PlatformPreference> platformPreferencesToDelete = platformPreferences.Where(x => mymodel.PlatformPreferences.Any(y => y.PlatformId == x.PlatformId && y.IsChecked == false)).ToList();
            platformPreferencesToDelete.ForEach(x => db.PlatformPreferences.Remove(x));

            List<PlatformPreferenceViewModel> platformPreferencesToAdd = mymodel.PlatformPreferences.Where(x => x.IsChecked == true && !(platformPreferences.Any(y => y.PlatformId == x.PlatformId))).ToList();
            List<PlatformPreference> DbEntriesp = platformPreferencesToAdd.Select(x => new PlatformPreference
            {
                UserId = id,
                PlatformId = x.PlatformId
            }).ToList();
            DbEntriesp.ForEach(x => db.PlatformPreferences.Add(x));
            //cateogry
            List<CategoryPreference> categoryPreferences = db.CategoryPreferences.Where(x => x.UserId == id).ToList();

            List<CategoryPreference> categoryPreferencesToDelete = categoryPreferences.Where(x => mymodel.CategoryPreferences.Any(y => y.CategoryId == x.CategoryId && y.IsChecked == false)).ToList();
            categoryPreferencesToDelete.ForEach(x => db.CategoryPreferences.Remove(x));

            List<CategoryPreferenceViewModel> categoryPreferencesToAdd = mymodel.CategoryPreferences.Where(x => x.IsChecked == true && !(categoryPreferences.Any(y => y.CategoryId == x.CategoryId))).ToList();
            List<CategoryPreference> DbEntriesc = categoryPreferencesToAdd.Select(x => new CategoryPreference
            {
                UserId = id,
                CategoryId = x.CategoryId
            }).ToList();
            DbEntriesc.ForEach(x => db.CategoryPreferences.Add(x));
            try
            {
                db.SaveChanges();
                //return View(categories);
            }
            catch (DataException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return RedirectToAction("ViewPreference", new { id = mymodel.Users.UserID });
        }
    }
}