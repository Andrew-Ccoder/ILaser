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
    public class ProductsController : Controller
    {
        private ILaserEntities db = new ILaserEntities();

        // GET: Products
 
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.User);
            return View(products.ToList());
        }
        public ActionResult GetProductsByPublisher()
        {
            int UserId = Convert.ToInt32(Session["ad_Id"]);
            var products = db.Products.Where(p => p.UserId == UserId).ToList();
            return View(products);
        }
        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            Session["ProductId"] = product.Product_Id;
            Session["ProductPrice"] = product.Price;
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
           // ViewBag.UserId = new SelectList(db.Users, "User_Id", "User_FirstName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                product.UserId = Convert.ToInt32(Session["ad_Id"]);
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
           // ViewBag.UserId = new SelectList(db.Users, "User_Id", "User_FirstName", product.UserId);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
           
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "User_Id", "User_FirstName", product.UserId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                product.UserId = Convert.ToInt32(Session["ad_Id"]);
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
           // ViewBag.UserId = new SelectList(db.Users, "User_Id", "User_FirstName", product.UserId);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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

        public ActionResult GetOrdersByWorker()
        {

            var UserID = Convert.ToInt32(Session["ad_id"]);

            var OrderedProducts = from order in db.Order_Product
                                  join pro in db.Products
                                  on order.Product_Id equals pro.Product_Id
                                  where pro.UserId == UserID
                                  select order;

            var grouped = from j in OrderedProducts
                          group j by j.Product.Name
                        into gr
                          select new OrderedProductViewModel
                          {
                              ProductName = gr.Key,
                              Items = gr

                          };

            return View(grouped.ToList());
        }


        public ActionResult GetCommentsByPublisher()
        {

            var UserID = Convert.ToInt32(Session["ad_id"]);

            var CommentedProduct = from comment in db.Comments
                                  join pro in db.Products
                                  on comment.Prouduct_Id equals pro.Product_Id
                                  where pro.UserId == UserID
                                  select comment;

            var grouped = from j in CommentedProduct
                          group j by j.Product.Name
                          into gr
                          select new CommentedProductsViewModel
                          {
                              ProductName = gr.Key,
                              Items = gr

                          };

            return View(grouped.ToList());
        }

        public ActionResult GetRatingsByPublisher()
        {

            var UserID = Convert.ToInt32(Session["ad_id"]);

            var RatedProduct = from Rate in db.Ratings
                                   join pro in db.Products
                                   on Rate.Product_Id equals pro.Product_Id
                                   where pro.UserId == UserID
                                   select Rate;

            var grouped = from j in RatedProduct
                          group j by j.Product.Name
                          into gr
                          select new RatedProductsViewModel
                          {
                              ProductName = gr.Key,
                              Items = gr

                          };

            return View(grouped.ToList());
        }


       
    }

  
}
