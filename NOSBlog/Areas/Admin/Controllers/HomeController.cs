using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NOSBlog.Auths;
using NOSBlog.Filters;

namespace NOSBlog.Areas.Admin.Controllers
{
    [AdminAuthorization]
    public class HomeController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}