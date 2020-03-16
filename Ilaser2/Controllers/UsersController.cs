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
            ViewBag.User_Type = new SelectList(db.User_Type, "Type_Id", "Type_Name", userModel.TypeId);
            return View();
        }

        [HttpPost]
        public ActionResult Register(User userModel)
        {
            ViewBag.User_Type = new SelectList(db.User_Type, "Type_Id", "Type_Name", userModel.TypeId);


            if (db.Users.Any(x => x.Email == userModel.Email))
            {
                ViewBag.DuplicationMessage = "Sorry This User Already existed";
                return View("Register", userModel);
            }

            db.Users.Add(userModel);
            db.SaveChanges();
            ModelState.Clear();
            ViewBag.Message = "Your registeration is successfully done";

            return View("Register", new User());
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
                return RedirectToAction("Index", "Products");
            }
            else
            {
                Session["ad_Id"] = ad.User_Id;
                Session["ad_username"] = ad.User_FirstName;
                Session["User_Type"] = ad.TypeId;
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
    }
}
