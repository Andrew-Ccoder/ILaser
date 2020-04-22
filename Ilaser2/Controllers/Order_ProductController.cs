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
    public class Order_ProductController : Controller
    {
        private ILaserEntities db = new ILaserEntities();

        // GET: Order_Product
        public ActionResult Index()
        {
            int UserId = Convert.ToInt32(Session["ad_Id"]);
            var order_Product = db.Order_Product.Where(o => o.User_Id== UserId);
            return View(order_Product.ToList());
        }

        // GET: Order_Product/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order_Product order_Product = db.Order_Product.Find(id);
            if (order_Product == null)
            {
                return HttpNotFound();
            }
            Session["OrderedProductId"] = order_Product.Product_Id;
            return View(order_Product);
        }

        // GET: Order_Product/Create
        public ActionResult Create()
        {
            //ViewBag.Product_Id = new SelectList(db.Products, "Product_Id", "Name");
            //ViewBag.User_Id = new SelectList(db.Users, "User_Id", "User_FirstName");
            return View();
        }

        // POST: Order_Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Order_Product order_Product,Product product)
        {

            if (ModelState.IsValid)
            {
                if((order_Product.Qty <= Convert.ToInt32(Session["ProductQty"]))&& (order_Product.Qty>0))
                {
                    order_Product.User_Id = Convert.ToInt32(Session["ad_Id"]);
                    order_Product.Product_Id = Convert.ToInt32(Session["ProductId"]);
                    order_Product.Total_Price = (order_Product.Qty * Convert.ToInt32(Session["ProductPrice"])).ToString();
                    db.Order_Product.Add(order_Product);
                    db.SaveChanges();
                    Session["OrderedQty"] = order_Product.Qty;
                    product.Name = Session["ProductName"].ToString();
                    product.UserId = Convert.ToInt32(Session["ProductOwner"]);
                    product.Product_Id = Convert.ToInt32(Session["ProductId"]);
                    product.QTY = Convert.ToInt32(Session["ProductQty"]) - Convert.ToInt32(Session["OrderedQty"]);
                    product.Price = (Session["ProductPrice"]).ToString();
                    product.Description = Session["ProductDescription"].ToString();
                    product.ProductMainPicture = Session["ProductMainPhoto"].ToString();
                    db.Entry(product).State = EntityState.Modified;
                    if (product.QTY == 0)
                    {
                        product.IsDeleted = true;
                    }
                    db.SaveChanges();
                   
                    return RedirectToAction("Index");

                }
                else if (order_Product.Qty <= 0)
                {
                    ViewBag.MinusMessage = " Your Qty Must be Positive number";
                    return View(order_Product);
                }
                else 
                {
                    ViewBag.PQTY = Session["ProductQty"];
                    ViewBag.ErrorMessage = " The Qty in Stock is not enought.The Maximum Qty you should order not more than";
                    return View(order_Product);
                }
               
            }
     
           

           
            //ViewBag.Product_Id = new SelectList(db.Products, "Product_Id", "Name", order_Product.Product_Id);
            //ViewBag.User_Id = new SelectList(db.Users, "User_Id", "User_FirstName", order_Product.User_Id);
            return View(order_Product);
        }

        // GET: Order_Product/Edit/5
        public ActionResult Edit(int? id)
        {
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order_Product order_Product = db.Order_Product.Find(id);
            if (order_Product == null)
            {
                return HttpNotFound();
            }
            ViewBag.Product_Id = new SelectList(db.Products, "Product_Id", "Name", order_Product.Product_Id);
            ViewBag.User_Id = new SelectList(db.Users, "User_Id", "User_FirstName", order_Product.User_Id);
            return View(order_Product);
        }

        // POST: Order_Product/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Order_Product order_Product,Product product)
        {
            //var OrderedProduct = db.Products.Where(a => a.Product_Id == order_Product.Product_Id);

            //Session["OrderedProductId"] = OrderedProduct;
            if (ModelState.IsValid)
            {
                if ((order_Product.Qty <= Convert.ToInt32(Session["ProductQty"])) && (order_Product.Qty > 0))
                {
                    order_Product.User_Id = Convert.ToInt32(Session["ad_Id"]);
                    order_Product.Product_Id = Convert.ToInt32(Session["OrderedProductId"]);
                    order_Product.Total_Price = (order_Product.Qty * Convert.ToInt32(Session["ProductPrice"])).ToString();
                    db.Entry(order_Product).State = EntityState.Modified;
                    db.SaveChanges();
                    Session["OrderedQty"] = order_Product.Qty;
                    product.Name = Session["ProductName"].ToString();
                    product.UserId = Convert.ToInt32(Session["ProductOwner"]);
                    product.Product_Id = Convert.ToInt32(Session["ProductId"]);
                    product.QTY = Convert.ToInt32(Session["ProductQty"]) - Convert.ToInt32(Session["OrderedQty"]);
                    product.Price = (Session["ProductPrice"]).ToString();
                    product.Description = Session["ProductDescription"].ToString();
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else if (order_Product.Qty <= 0)
                {
                    ViewBag.MinusMessage = " Your Qty Must be Positive number";
                    return View(order_Product);
                }
                else
                {
                    ViewBag.PQTY = Session["ProductQty"];
                    ViewBag.ErrorMessage = " The Qty in Stock is not enought.The Maximum Qty you should order not more than";
                    return View(order_Product);
                }
            }
            //ViewBag.Product_Id = new SelectList(db.Products, "Product_Id", "Name", order_Product.Product_Id);
            //ViewBag.User_Id = new SelectList(db.Users, "User_Id", "User_FirstName", order_Product.User_Id);
            return View(order_Product);
        }

        // GET: Order_Product/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order_Product order_Product = db.Order_Product.Find(id);
            if (order_Product == null)
            {
                return HttpNotFound();
            }
            return View(order_Product);
        }

        // POST: Order_Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order_Product order_Product = db.Order_Product.Find(id);
            db.Order_Product.Remove(order_Product);
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
