using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ilaser2.Models;

using Microsoft.AspNet.Identity;
using System.IO;

namespace Ilaser2.Controllers
{
    public class UsersController : Controller
    {
        private ILaserEntities db = new ILaserEntities();

        public ActionResult Home()
        {
            return View(db.Products.ToList());

        }

        // GET: User
        public ActionResult Register(int id = 0)
        {
            User userModel = new User();
            ViewBag.TypeId = new SelectList(db.User_Type, "Type_Id", "Type_Name", userModel.TypeId);
            return View();
        }

        [HttpPost]
        public ActionResult Register(User userModel, HttpPostedFileBase upload)
        {
            ViewBag.TypeId = new SelectList(db.User_Type, "Type_Id", "Type_Name", userModel.TypeId);


            if (db.Users.Any(x => x.Email == userModel.Email))
            {
                ViewBag.DuplicationMessage = "Sorry This User Already existed";
                return View("Register", userModel);
            }
            else 
            {
                try
                {
                    string path = Path.Combine(Server.MapPath("~/Uploads"), upload.FileName);
                    upload.SaveAs(path);
                    userModel.UserPhoto = upload.FileName;

                    if (upload.FileName == null)
                    {
                        ViewBag.picture = "Please Select Profile Picture";
                    }
                    db.Users.Add(userModel);
                    if(userModel.Password!=userModel.ConfirmPassword)
                    {
                        ViewBag.NotMatch = "The Passowrds do not match each other";
                    }
                    db.SaveChanges();

                    ModelState.Clear();
                    ViewBag.Message = "Your registeration is successfully done";
                    Session["ProfilePicture"] = userModel.UserPhoto;
                    return View("Register", new User());
                }
               catch
                {
                    ViewBag.Picture = "Please Select a Profile picture";
                    return View("Register", new User());
                }
            }
           
        }

        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Login(User avm)
        {
            User ad = db.Users.Where(x => (x.Email == avm.Email) && (x.Password == avm.Password)).SingleOrDefault();
            if (ad == null)
            {

                ViewBag.Error = "Invalid UserName or Password";
                return View("Login", avm);
            }
            else if (ad.TypeId == 2)
            {
                Session["ad_Id"] = ad.User_Id;
                Session["ad_username"] = ad.User_FirstName;
                Session["User_Type"] = ad.TypeId;
                Session["UserPhoto"] = ad.UserPhoto;
                return RedirectToAction("Index", "Products");
            }
            else
            {
                Session["ad_Id"] = ad.User_Id;
                Session["ad_username"] = ad.User_FirstName;
                Session["User_Type"] = ad.TypeId;
                Session["UserPhoto"] = ad.UserPhoto;
                return RedirectToAction("Index", "Products");
            }

        }
        


        public ActionResult Logout()
        {

            int ad_id = (int)Session["ad_Id"];
            Session.Abandon();
            // AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Users");
        }

        public ActionResult EditProfile(int? id)
        {
            id= Convert.ToInt32(Session["ad_Id"]);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "User_Id", "User_FirstName");
            return View(user);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(User user, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string path = Path.Combine(Server.MapPath("~/Uploads"), upload.FileName);
                    upload.SaveAs(path);
                    user.UserPhoto = upload.FileName;
                    user.User_Id = Convert.ToInt32(Session["ad_Id"]);
                    user.TypeId = Convert.ToInt32(Session["User_Type"]);
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    return View("EditProfile", new User());

                }
                catch
                {
                    user.UserPhoto = Session["UserPhoto"].ToString();
                    user.User_Id = Convert.ToInt32(Session["ad_Id"]);
                    user.TypeId = Convert.ToInt32(Session["User_Type"]);
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    return View("EditProfile", new User());
                }

            }
            // ViewBag.UserId = new SelectList(db.Users, "User_Id", "User_FirstName", product.UserId);
            return View("EditProfile",new User());
        }

        public ActionResult Details(int? id)
        {
            id = Convert.ToInt32(Session["ProductPublisher"]);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
          
            return View(users);
        }

    }
}
