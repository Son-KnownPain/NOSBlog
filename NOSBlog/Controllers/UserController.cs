using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NOSBlog.Controllers
{
    public class UserController : Controller
    {
        //GET User/Login
        public ActionResult Login()
        {
            return View();
        }

        // GET User/Register
        public ActionResult Register()
        {
            return View();
        }

        // GET: User/Store
        [HttpPost]
        public ActionResult Store(NOSBlog.Models.user userModel)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/User/Register.cshtml");
            } else
            {
                return Json(new
                {
                    success = true
                });
            }
        }
    }
}