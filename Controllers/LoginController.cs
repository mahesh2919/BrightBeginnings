using CrudAppWithImagesInMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CrudAppWithImagesInMVC.Controllers
{
    public class LoginController : Controller
    {
        ExampleDBEntities db =new ExampleDBEntities();
        // GET: Login
        public ActionResult Index()
        {
           
                return View();
           
            
        }
        [HttpPost]
        public ActionResult Index(user u)
        {
            var user = db.users.Where(model => model.username == u.username && model.password == u.password).FirstOrDefault();
            if(user != null)
            {
                Session["UserId"] = u.id.ToString();
                Session["Username"] = u.username.ToString();
                TempData["LoginMessage"] = "<script>alert('Login Sucessfully!!')</script>";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage= "<script>alert('User Name or Password are incorrect!')</script>";
                return View();
            }
            
        }
        public ActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Signup(user u)
        {
            u.IsActive = true;
            if(ModelState.IsValid==true)
            {
                db.users.Add(u);
               int a = db.SaveChanges();
                if(a>0)
                {
                    TempData["InsertMessage"] = "<script>alert('User Registered')</script>";
                    ModelState.Clear();
                    return RedirectToAction("Index","Login");
                }
                else
                {
                    TempData["InsertMessage"] = "<script>alert('User Not Registered')</script>";
                }
            }


            return View();
        }
    }
}