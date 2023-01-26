using CrudAppWithImagesInMVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CrudAppWithImagesInMVC.Controllers
{
    public class HomeController : Controller
    {
        ExampleDBEntities db = new ExampleDBEntities();
        private object students;

        // GET: Home
        //public ActionResult Index()
        //{
        //    var data = db.students.ToList();
        //    return View(data);
        //}

        [HttpGet]
        public ActionResult Index(string searchby, string search)
        {
            if (searchby == "Name")
            {
                var data = db.students.Where(model => (model.name.StartsWith(search) && model.IsActive == true)).ToList();
                return View(data);
            }
            else if (searchby == "Gender")
            {
                var data = db.students.Where(model => (model.gender == (search) && model.IsActive == true)).ToList();
                return View(data);
            }
            else
            {
                if (Session["Username"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                var data = db.students.Where(x => x.IsActive == true).ToList();
                return View(data);
            }
            }
        }

        public ActionResult Create()
        {
           return View();
        }
        [HttpPost]
        public ActionResult Create(student s)
        {
            s.IsActive = true;
            if(ModelState.IsValid==true)
            {
                string fileName = Path.GetFileNameWithoutExtension(s.ImageFile.FileName);
                string extension = Path.GetExtension(s.ImageFile.FileName);
                HttpPostedFileBase postedFile = s.ImageFile;
                int length = postedFile.ContentLength;
                if(extension.ToLower()==".jpg" || extension.ToLower() == ".png" || extension.ToLower() == ".jpeg")
                {
                    if(length<=1000000)
                    {
                        fileName = fileName + extension;
                        s.image_path = "~/images/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/images/"), fileName);
                        s.ImageFile.SaveAs(fileName);
                        db.students.Add(s);
                        int a = db.SaveChanges();

                        if(a>0)
                        {
                            TempData["InsertMessage"] = "<script>alert('Data Inserted')</script>";
                            ModelState.Clear();
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            TempData["InsertMessage"] = "<script>alert('Data not Inserted')</script>";
                        }
                    }
                    else
                    {
                        TempData["SizeMessage"] = "<script>alert('Image size should be less than 1 MB')</script>";
                    }
                }
                else
                {
                    TempData["ExtensionMessage"] = "<script>alert('Format Not Supported')</script>";
                }

            }
            return View();
        }

        public ActionResult Edit(int id)
        {
            var row=db.students.Where(model=>model.id==id).FirstOrDefault();
            Session["image"] = row.image_path;
            return View(row);
        }

        [HttpPost]
        public ActionResult Edit(student s)
        {
            s.IsActive = true;
            if (ModelState.IsValid == true)
            {
                if(s.ImageFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(s.ImageFile.FileName);
                    string extension = Path.GetExtension(s.ImageFile.FileName);
                    HttpPostedFileBase postedFile = s.ImageFile;
                    int length = postedFile.ContentLength;
                    if (extension.ToLower() == ".jpg" || extension.ToLower() == ".png" || extension.ToLower() == ".jpeg")
                    {
                        if (length <= 1000000)
                        {
                            fileName = fileName + extension;
                            s.image_path = "~/images/" + fileName;
                            fileName = Path.Combine(Server.MapPath("~/images/"), fileName);
                            s.ImageFile.SaveAs(fileName);
                            db.Entry(s).State=EntityState.Modified;
                            int a = db.SaveChanges();

                            if (a > 0)
                            {
                                string ImgePath = Request.MapPath(Session["image"].ToString());
                                if (System.IO.File.Exists(ImgePath))
                                {
                                    System.IO.File.Delete(ImgePath);
                                }
                                TempData["UpdateMessage"] = "<script>alert('Data Updated')</script>";
                                ModelState.Clear();
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                TempData["UpdateMessage"] = "<script>alert('Data not Updated')</script>";
                            }
                        }
                        else
                        {
                            TempData["SizeMessage"] = "<script>alert('Image size should be less than 1 MB')</script>";
                        }
                    }
                    else
                    {
                        TempData["ExtensionMessage"] = "<script>alert('Format Not Supported')</script>";
                    }
                }

                else
                {
                    s.image_path = Session["image"].ToString();
                    db.Entry(s).State = EntityState.Modified;
                    int a = db.SaveChanges();

                    if (a > 0)
                    {
                        TempData["UpdateMessage"] = "<script>alert('Data Updated')</script>";
                        ModelState.Clear();
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["UpdateMessage"] = "<script>alert('Data not Updated')</script>";
                    }
                }
            }
            return View();
        }

        public ActionResult Delete(int id)
        {
            var row = db.students.Where(model => model.id == id).FirstOrDefault();
            //if (id > 0)
            //{
            //    var row=db.students.Where(model => model.id == id).FirstOrDefault();
            //    row.IsActive = false;
            //    if (row != null)
            //    {

            //        db.Entry(row).State = EntityState.Deleted;
            //        int a = db.SaveChanges();
            //        if (a > 0)
            //        {
            //            TempData["DeleteMessage"] = "<script>alert('Data Deleted')</script>";
            //            string ImgePath = Request.MapPath(row.image_path.ToString());
            //            if (System.IO.File.Exists(ImgePath))
            //            {
            //                System.IO.File.Delete(ImgePath);
            //            }
            //        }
            //        else
            //        {
            //            TempData["DeleteMessage"] = "<script>alert('Data not Deleted')</script>";
            //        }

            //    }
            //}
            //return RedirectToAction("Index", "Home");
            return View(row);
        }
        [HttpPost]
        public ActionResult DeleteStudent(student s)
        {
            if (ModelState.IsValid == true)
            {
                s.IsActive = false;
                db.Entry(s).State = EntityState.Modified;
                try
                {
                    int a = db.SaveChanges();
                    if (a > 0)
                    {

                        TempData["DeleteMessage"] = "<script>alert('Data Deleted')</script>";
                        string ImgePath = Request.MapPath(s.image_path.ToString());
                        if (System.IO.File.Exists(ImgePath))
                        {
                            System.IO.File.Delete(ImgePath);
                        }
                        TempData["DeleteData"] = "Data Deleted";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["DeleteData"] = "Data Not Deleted";
                    }
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    //throw;
                }
            }
           return RedirectToAction("Index", "Home");
            //return View();
        }

        public ActionResult Details(int id)
        {
            var row = db.students.Where(model => model.id == id).FirstOrDefault();
            Session["image1"] = row.image_path.ToString();
            return View(row);
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }
    }
}