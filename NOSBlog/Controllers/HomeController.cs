using NOSBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NOSBlog.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            NOSBlogEntities context = new NOSBlogEntities();
            var allUsers = context.users.ToList();
            ViewBag.UsersCount = allUsers.Count;
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult AboutUs()
        {
            return View();
        }
    }
}