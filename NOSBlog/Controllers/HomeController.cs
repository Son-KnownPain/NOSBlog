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
            return View();
        }

        public ActionResult Policy()
        {
            return View();
        }

        public ActionResult Security()
        {
            return View();
        }

        public ActionResult AboutUs()
        {
            return View();
        }
    }
}