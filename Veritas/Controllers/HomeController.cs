﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Veritas.Models;
using Veritas.Services;

namespace Veritas.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        VeritasServices services = new VeritasServices();
        portal_s_websure user = new portal_s_websure();

        [CheckAuthorization]
        public ActionResult Index()
        {
            //string username = (string)System.Web.HttpContext.Current.Session["username"];
            //if (username != null)
            //{
            //    return View();
            //}

            //return RedirectToAction("Login", "Home");
            return View();
        }

        public ActionResult Login()
        {
            

            //return RedirectToAction("Home", "Login");

            return View();
        }

        [CheckAuthorization]
        public ActionResult Profile()
        {
           
            string username = (string)System.Web.HttpContext.Current.Session["username"];
            user = (from u in db.portal_s_websure where u.USERNAME == username select u).FirstOrDefault();
            TempData.Keep("profile");
            TempData["profile"] = user;
            return View();
        }

        public async Task<ActionResult> UpdateProfile([FromBody] portal_s_websure user)
        {
            var data = db.portal_s_websure.Find(user.USERID);

            //insert the username 
            data.FIRSTNAME = user.FIRSTNAME;

            db.Entry(data).State = EntityState.Modified;
            try
            {
                await db.SaveChangesAsync();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                string msg = ex.Message;
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }

        }

        public async Task<ActionResult> UpdateProfileImage(HttpPostedFileBase upload)
        {
            decimal OpId = (decimal) System.Web.HttpContext.Current.Session["userId"];

            string targetPath = @"~/App_Data/uploads";

            if (upload != null && upload.ContentLength > 0)
            {
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }

                string fileName = Path.GetFileName(upload.FileName);
                string filePath = Path.Combine(Server.MapPath("~/Uploads"), fileName);

                try
                {
                    upload.SaveAs(filePath);

                    //lets insert the image into the database
                    var userDate = (from u in db.ProfileImages where u.UserId == OpId select u).FirstOrDefault();
                    
                    if(userDate != null){
                        userDate.ImagePath= upload.FileName;
                        db.Entry(userDate).State = EntityState.Modified;
                    }
                    else
                    {
                        //this user is just uploading the picture
                        var userImage = new ProfileImage
                        {
                            UserId = OpId,
                            ImagePath = upload.FileName
                        };
                        db.ProfileImages.Add(userImage);
                        await db.SaveChangesAsync();
                    }
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception)
                {

                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);

                }



            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);

        }

        public async Task<ActionResult> auth(string email, string password)
        {
            var user = (from u in db.portal_s_websure where u.EMAIL == email && u.PASSWORD == password select u).FirstOrDefault();

            if (user != null)
            {
                System.Web.HttpContext.Current.Session.Add("username", user.USERNAME);
                System.Web.HttpContext.Current.Session.Add("userId", user.USERID);
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Login", "Home");
        }

        public ActionResult Complaint()
        {
            ViewBag.Message = "Your Complaint";
            return View();
        }

        public ActionResult ContactForm()
        {
            ViewBag.Message = "Your Contact Form";
            return View();
        }

        public ActionResult ReferralForm()
        {
            ViewBag.Message = "Your Referral from";
            return View();
        }

    }





}
