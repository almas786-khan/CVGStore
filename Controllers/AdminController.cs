using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CVGS.Models;

namespace CVGS.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        cvgsEntities2 _context = new cvgsEntities2();
        public ActionResult Index()
        {
            var listodData = _context.Games.ToList();
            return View(listodData);

        }
        [HttpGet]
        public ActionResult AddGame()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddGame([Bind(Include =" GameName, GameDescription,GamePrice, ImageURL, ImageFile")]Game model, HttpPostedFileBase ImageFile)
        {
            var fileExt="";
            if (ImageFile!=null)
            {
                 fileExt = System.IO.Path.GetExtension(ImageFile.FileName).Substring(1);
            }
           
            var supportedTypes = new[] {"jpg","jpeg","png" };
            try
            {
                if (ImageFile == null)
                {
                    ViewBag.result = "Image is required, so upload an image";
                }
                else if (ImageFile != null && ((ImageFile.ContentLength / 1024) < 10 || (ImageFile.ContentLength / 1024 > 400)))
                {
                    ViewBag.result = "File size should be in range of 10kb and 400kb";
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
                        _context.Games.Add(model);
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
            return View(model);
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            var data = _context.Games.Where(x => x.GameID == id).FirstOrDefault();
            return View(data);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult Edit(Game model, int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var gameToUpdate = _context.Games.Find(id);
            if (TryUpdateModel(gameToUpdate, "",
               new string[] { "GameName", "GameDescription", "GamePrice", "GameOverallRating","ImageURL" }))
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
            return View(gameToUpdate);
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game game = _context.Games.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }
            return View(game);
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
            Game game = _context.Games.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }
            return View(game);
        }


        [HttpPost]
        
        public ActionResult Delete(int id)
        {
            try
            {
                Game game = _context.Games.Find(id);
                _context.Games.Remove(game);
                _context.SaveChanges();
            }
            catch (DataException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }

        public ActionResult Event()
        {
           
            var controller = DependencyResolver.Current.GetService<EventController>();

            var result = controller.Index();
            return result;
        }

        public ActionResult Report()
        {
            var controller = DependencyResolver.Current.GetService<ReportController>();

            var result = controller.Index();
            return result;
        }
    }
}