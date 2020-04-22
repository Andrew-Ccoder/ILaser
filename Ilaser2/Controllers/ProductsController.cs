using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ilaser2.Models;
using System.Dynamic;
using System.IO;

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
        public ActionResult IndexTrail()
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
        public ActionResult GetDeletedProducts()
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
            Session["ProductOwner"] = product.UserId;
            Session["ProductQty"] = product.QTY;
            Session["ProductName"] = product.Name;
            Session["ProductId"] = product.Product_Id;
            Session["ProductPrice"] = product.Price;
            Session["ProductDescription"] = product.Description;
            Session["ProductPicture"] = product.ProductMainPicture;
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.Cat_Id = new SelectList(db.Categories, "CategoryId", "CategoryName");

            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase upload)
        {
            ViewBag.Cat_Id = new SelectList(db.Categories, "CategoryId", "CategoryName", product.Cat_Id);
            if (ModelState.IsValid)
            {
                string path = Path.Combine(Server.MapPath("~/Uploads"), upload.FileName);
                upload.SaveAs(path);
                product.ProductMainPicture = upload.FileName;
                product.UserId = Convert.ToInt32(Session["ad_Id"]);
                db.Products.Add(product);
                db.SaveChanges();
                return View();
            }
            Session["ProductMainPicture"] = product.ProductMainPicture;
            ViewBag.Cat_Id = new SelectList(db.Categories, "CategoryId", "CategoryName", product.Cat_Id);

            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            id = Convert.ToInt32(Session["ProductId"]);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
           
            if (product == null)
            {
                return HttpNotFound();
            }

            ViewBag.Cat_Id = new SelectList(db.Categories, "CategoryId", "CategoryName", product.Cat_Id);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase upload)
        {
            ViewBag.Cat_Id = new SelectList(db.Categories, "CategoryId", "CategoryName", product.Cat_Id);

        
            if (ModelState.IsValid)
            {

                try
                {
                    string path = Path.Combine(Server.MapPath("~/Uploads"), upload.FileName);

                    upload.SaveAs(path);
                    product.ProductMainPicture = upload.FileName;
                    
                    product.Name = Session["ProductName"].ToString();
                    product.UserId = Convert.ToInt32(Session["ad_Id"]);
                    product.Product_Id = Convert.ToInt32(Session["ProductId"]);
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");

                }

                catch
                {
                   
                    product.ProductMainPicture = Session["ProductPicture"].ToString();
                    product.Name = Session["ProductName"].ToString();
                    product.UserId = Convert.ToInt32(Session["ad_Id"]);
                    product.Product_Id = Convert.ToInt32(Session["ProductId"]);
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }




            }
           // ViewBag.UserId = new SelectList(db.Users, "User_Id", "User_FirstName", product.UserId);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            id= Convert.ToInt32(Session["ProductPhoto"]);
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
        public ActionResult DeleteConfirmed(Product product)
        {
            if (ModelState.IsValid)
            {


                product.Product_Id = Convert.ToInt32(Session["ProductPhoto"]);
                product.UserId = Convert.ToInt32(Session["ProductUser"]);
                product.QTY = Convert.ToInt32(Session["ProductQty"]);
                product.Price = (Session["ProductPrice"]).ToString();
                product.Name = Session["ProductName"].ToString();
                product.Description = Session["productDescription"].ToString();
                product.ProductMainPicture = Session["ProductMainPhoto"].ToString();
                product.IsDeleted = true;
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View(product);
            }
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

        public ActionResult Getphotosbyproduct()
        {
            var UserId = Convert.ToInt32(Session["ad-id"]);
            var ProductID = Convert.ToInt32(Session["ProductId"]);

            var productphotos = from photo in db.Photos
                                join pro in db.Products
                                on photo.Product_Id equals pro.Product_Id
                                where pro.Product_Id == ProductID
                                select photo;

            var grouped= from p in productphotos
                         group p by p.Product.Name
                         into gr
                         select new ProductPhotosViewModel
                         {
                             Productphoto = gr.Key,
                             Items = gr

                         };

            return View(grouped.ToList());
        }
        public ActionResult GetCommentsByProduct()
        {

            var ProductId = Convert.ToInt32(Session["ProductId"]);

            var CommentedProduct = from comment in db.Comments
                                   join pro in db.Products
                                   on comment.Prouduct_Id equals pro.Product_Id
                                   where pro.Product_Id == ProductId
                                   select comment;

            var grouped = from j in CommentedProduct
                          group j by j.Product.Name
                          into gr
                          select new ProductCommentesViewModel
                          {
                              ProductName = gr.Key,
                              Items = gr

                          };

            return View(grouped.ToList());
        }
        public ActionResult GetRatingsByProduct()
        {

            var ProductId = Convert.ToInt32(Session["ProductId"]);

            var RatedProduct = from Rate in db.Ratings
                               join pro in db.Products
                               on Rate.Product_Id equals pro.Product_Id
                               where pro.Product_Id == ProductId
                               select Rate;

            var grouped = from j in RatedProduct
                          group j by j.Product.Name
                          into gr
                          select new ProductRatesViewModel
                          {
                              ProductName = gr.Key,
                              Items = gr

                          };
           
            return View(grouped.ToList());
        }
        
            public ActionResult comment()
        {
            return PartialView("~/Views/Products/GetCommentsByProduct.cshtml", CommentsByProduct());

        }
        public ActionResult rating()
        {
            return PartialView("~/Views/Products/_ProductRatings.cshtml",RatingsByProduct());

        }
        public ActionResult photos()
        {
            return PartialView("~/Views/Products/Getphotosbyproduct.cshtml", photosbyproduct());

        }
        public IEnumerable<ProductRatesViewModel> RatingsByProduct()
        {

            var ProductId = Convert.ToInt32(Session["ProductId"]);

            var RatedProduct = from Rate in db.Ratings
                               join pro in db.Products
                               on Rate.Product_Id equals pro.Product_Id
                               where pro.Product_Id == ProductId
                               select Rate;

            var grouped = from j in RatedProduct
                          group j by j.Product.Name
                          into gr
                          select new ProductRatesViewModel
                          {
                              ProductName = gr.Key,
                              Items = gr

                          };

            return grouped.ToList();
        }
        public IEnumerable<ProductPhotosViewModel> photosbyproduct()
        {
            var UserId = Convert.ToInt32(Session["ad-id"]);
            var ProductID = Convert.ToInt32(Session["ProductId"]);

            var productphotos = from photo in db.Photos
                                join pro in db.Products
                                on photo.Product_Id equals pro.Product_Id
                                where pro.Product_Id == ProductID
                                select photo;

            var grouped = from p in productphotos
                          group p by p.Product.Name
                         into gr
                          select new ProductPhotosViewModel
                          {
                              Productphoto = gr.Key,
                              Items = gr

                          };

            return grouped.ToList();
        }

        public IEnumerable<ProductCommentesViewModel> CommentsByProduct()
        {

            var ProductId = Convert.ToInt32(Session["ProductId"]);

            var CommentedProduct = from comment in db.Comments
                                   join pro in db.Products
                                   on comment.Prouduct_Id equals pro.Product_Id
                                   where pro.Product_Id == ProductId
                                   select comment;

            var grouped = from j in CommentedProduct
                          group j by j.Product.Name
                          into gr
                          select new ProductCommentesViewModel
                          {
                              ProductName = gr.Key,
                              Items = gr

                          };

            return grouped.ToList();
        }

        public ActionResult Restore(int? id)
        {
            id = Convert.ToInt32(Session["ProductPhoto"]);
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
        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public ActionResult RestoreConfirmed(Product product)
        {
            if (ModelState.IsValid)
            {


                product.Product_Id = Convert.ToInt32(Session["ProductPhoto"]);
                product.UserId = Convert.ToInt32(Session["ProductUser"]);
                product.QTY = Convert.ToInt32(Session["ProductQty"]);
                product.Price = (Session["ProductPrice"]).ToString();
                product.Name = Session["ProductName"].ToString();
                product.Description = Session["productDescription"].ToString();
                product.ProductMainPicture = Session["ProductPicture"].ToString();
                product.IsDeleted = false;
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View(product);
            }
        }

        [HttpPost]
        public ActionResult Search(string searchName)
        {

            var result = db.Products.Where(a => a.Name.Contains(searchName)
             || a.Description.Contains(searchName)
             || a.Price.Contains(searchName)
             || a.Category.CategoryName.Contains(searchName)
             || a.Category.CategoryDiscription.Contains(searchName)).ToList();

            return View(result);
        }
    }




}
