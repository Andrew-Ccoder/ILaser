using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ilaser2.Models;
using System.IO;

namespace Ilaser2.Controllers
{
    public class User_PhotoController : Controller
    {
        private ILaserEntities db = new ILaserEntities();

        // GET: User_Photo
        public ActionResult Index()
        {
            var myphoto = Convert.ToInt32(Session["ad_id"]);
            var user_Photo = db.User_Photo.Where(u => u.User_Id== myphoto);
            return View(user_Photo.ToList());
        }

        // GET: User_Photo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User_Photo user_Photo = db.User_Photo.Find(id);
            if (user_Photo == null)
            {
                return HttpNotFound();
            }
            return View(user_Photo);
        }

        // GET: User_Photo/Create
        public ActionResult Create()
        {
            ViewBag.User_Id = new SelectList(db.Users, "User_Id", "User_FirstName");
            return View();
        }

        // POST: User_Photo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User_Photo user_Photo, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                string path = Path.Combine(Server.MapPath("~/Uploads"), upload.FileName);
                upload.SaveAs(path);
                user_Photo.Photo_URL = upload.FileName;
                user_Photo.User_Id = Convert.ToInt32(Session["ad_id"]);
                db.User_Photo.Add(user_Photo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.User_Id = new SelectList(db.Users, "User_Id", "User_FirstName", user_Photo.User_Id);
            return View(user_Photo);
        }

        // GET: User_Photo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User_Photo user_Photo = db.User_Photo.Find(id);
            if (user_Photo == null)
            {
                return HttpNotFound();
            }
            ViewBag.User_Id = new SelectList(db.Users, "User_Id", "User_FirstName", user_Photo.User_Id);
            return View(user_Photo);
        }

        // POST: User_Photo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Photo_Id,Photo_URL,User_Id")] User_Photo user_Photo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user_Photo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.User_Id = new SelectList(db.Users, "User_Id", "User_FirstName", user_Photo.User_Id);
            return View(user_Photo);
        }

        // GET: User_Photo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User_Photo user_Photo = db.User_Photo.Find(id);
            if (user_Photo == null)
            {
                return HttpNotFound();
            }
            return View(user_Photo);
        }

        // POST: User_Photo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User_Photo user_Photo = db.User_Photo.Find(id);
            db.User_Photo.Remove(user_Photo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
