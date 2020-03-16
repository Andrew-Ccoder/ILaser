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
    public class CommentsController : Controller
    {
        private ILaserEntities db = new ILaserEntities();

        // GET: Comments
        public ActionResult Index()
        {
            var comments = db.Comments.Include(c => c.Product).Include(c => c.User);
            return View(comments.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id,Comment comments)
        {
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            Session["commentedproduct"] = comment.Prouduct_Id;
            
            return View(comment);
        }

        // GET: Comments/Create
        public ActionResult Create()
        {
            //ViewBag.Prouduct_Id = new SelectList(db.Products, "Product_Id", "Name");
            //ViewBag.User_Id = new SelectList(db.Users, "User_Id", "User_FirstName");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.User_Id = Convert.ToInt32(Session["ad_id"]);
                comment.Prouduct_Id = Convert.ToInt32(Session["ProductId"]);
                comment.Time = DateTime.Now;
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.Prouduct_Id = new SelectList(db.Products, "Product_Id", "Name", comment.Prouduct_Id);
            //ViewBag.User_Id = new SelectList(db.Users, "User_Id", "User_FirstName", comment.User_Id);
            return View(comment);
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            //ViewBag.Prouduct_Id = new SelectList(db.Products, "Product_Id", "Name", comment.Prouduct_Id);
            //ViewBag.User_Id = new SelectList(db.Users, "User_Id", "User_FirstName", comment.User_Id);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.User_Id = Convert.ToInt32(Session["ad_id"]);
                comment.Prouduct_Id = Convert.ToInt32(Session["commentedproduct"]);
                comment.Time = DateTime.Now;
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.Prouduct_Id = new SelectList(db.Products, "Product_Id", "Name", comment.Prouduct_Id);
            //ViewBag.User_Id = new SelectList(db.Users, "User_Id", "User_FirstName", comment.User_Id);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
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
