﻿using System;
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
    public class PhotosController : Controller
    {
        private ILaserEntities db = new ILaserEntities();

        // GET: Photos
        public ActionResult Index()
        {
            var photos = db.Photos.Include(p => p.Product);
            return View(photos.ToList());
        }

        public ActionResult PhotosByProduct()
        {
            var productphoto = Convert.ToInt32(Session["ProductId"]);
            var photos = db.Photos.Where(p => p.Product_Id==productphoto);
            return View(photos.ToList());
        }

        // GET: Photos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = db.Photos.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        // GET: Photos/Create
        public ActionResult Create()
        {
          //  ViewBag.Product_Id = new SelectList(db.Products, "Product_Id", "Name");
            return View();
        }

        // POST: Photos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Photo photo, HttpPostedFileBase upload)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    photo.Product_Id = Convert.ToInt32(Session["ProductId"]);
                    string path = Path.Combine(Server.MapPath("~/Uploads"), upload.FileName);

                    upload.SaveAs(path);
                    photo.Photo_URL = upload.FileName;
                    db.Photos.Add(photo);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Products");
                }
            }
           catch
            {
                return RedirectToAction("Index", "Products");
            }

           // ViewBag.Product_Id = new SelectList(db.Products, "Product_Id", "Name", photo.Product_Id);
            return View(photo);
        }

        // GET: Photos/Edit/5
        public ActionResult Edit(int? id)
        {
            

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = db.Photos.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            ViewBag.Product_Id = new SelectList(db.Products, "Product_Id", "Name", photo.Product_Id);
            return View(photo);
        }

        // POST: Photos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Photo_Id,Photo_URL,Product_Id")] Photo photo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(photo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Product_Id = new SelectList(db.Products, "Product_Id", "Name", photo.Product_Id);
            return View(photo);
        }

        // GET: Photos/Delete/5
        public ActionResult Delete(int? id)
        {
            id = Convert.ToInt32(Session["PhotoId"]);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = db.Photos.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        // POST: Photos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Photo photo)
        {
           
            photo.Photo_Id = Convert.ToInt32(Session["PhotoId"]);
            photo.Photo_URL = (Session["photoURL"]).ToString();
            photo.Product_Id= Convert.ToInt32(Session["ProductPhoto"]);
            db.Entry(photo).State = EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("Index","Products");
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
