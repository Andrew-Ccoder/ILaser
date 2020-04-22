using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ilaser2.Models;

namespace Ilaser2.Controllers
{
    public class RatingsController : Controller
    {
        private ILaserEntities db = new ILaserEntities();

        // GET: Ratings
        public ActionResult Index()
        {
            var ratings = db.Ratings.Include(r => r.Product).Include(r => r.User);
            return View(ratings.ToList());
        }

        // GET: Ratings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rating rating = db.Ratings.Find(id);
            if (rating == null)
            {
                return HttpNotFound();
            }
            return View(rating);
        }

        // GET: Ratings/Create
        public ActionResult Create()
        {
            //ViewBag.Product_Id = new SelectList(db.Products, "Product_Id", "Name");
            //ViewBag.User_Id = new SelectList(db.Users, "User_Id", "User_FirstName");
            return View();
        }

        // POST: Ratings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Rating rating)
        {
            
            if (ModelState.IsValid)
            {
                rating.User_Id = Convert.ToInt32(Session["ad_id"]);
                rating.Product_Id = Convert.ToInt32(Session["ProductId"]);
                rating.Rate_Range = Convert.ToInt32(Request.Form["range"]);
                //rating.Rating_Avg = Convert.ToInt32((from rate in db.Ratings
                //                                     where rate.Product_Id== rating.Product_Id
                //                                     select rate.Rate_Range).Average();
                db.Ratings.Add(rating);
                db.SaveChanges();
                Session["RatesAvg"] = rating.Rating_Avg;
                return RedirectToAction("Index","Products");
            }

            //ViewBag.Product_Id = new SelectList(db.Products, "Product_Id", "Name", rating.Product_Id);
            //ViewBag.User_Id = new SelectList(db.Users, "User_Id", "User_FirstName", rating.User_Id);
            return View(rating);
        }

        // GET: Ratings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rating rating = db.Ratings.Find(id);
            if (rating == null)
            {
                return HttpNotFound();
            }
            ViewBag.Product_Id = new SelectList(db.Products, "Product_Id", "Name", rating.Product_Id);
            ViewBag.User_Id = new SelectList(db.Users, "User_Id", "User_FirstName", rating.User_Id);
            return View(rating);
        }

        // POST: Ratings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Rate_Id,Rate_Range,User_Id,Product_Id")] Rating rating)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rating).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Product_Id = new SelectList(db.Products, "Product_Id", "Name", rating.Product_Id);
            ViewBag.User_Id = new SelectList(db.Users, "User_Id", "User_FirstName", rating.User_Id);
            return View(rating);
        }

        // GET: Ratings/Delete/5
        public ActionResult Delete(int? id)
        {
            id = Convert.ToInt32(Session["RatingId"]);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rating rating = db.Ratings.Find(id);
            id = Convert.ToInt32(Session["RatingId"]);
            if (rating == null)
            {
                return HttpNotFound();
            }
            return View(rating);
        }

        // POST: Ratings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Rating rating)
        {
            rating.Rate_Id= Convert.ToInt32(Session["RatingId"]);
            rating.Rate_Range= Convert.ToInt32(Request.Form["range"]);
            rating.Product_Id = Convert.ToInt32(Session["ProductId"]);
            rating.User_Id = Convert.ToInt32(Session["ad_Id"]);
            db.Entry(rating).State = EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("Index", "Products");
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
