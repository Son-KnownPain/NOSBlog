using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NOSBlog.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        // GET: Admin/User/Create
        public ActionResult Create()
        {
            return View();
        }
    }
}