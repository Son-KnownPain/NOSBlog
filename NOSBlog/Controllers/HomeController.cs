using NOSBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NOSBlog.Models;

namespace NOSBlog.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            NOSBlogEntities context = new NOSBlogEntities();
            List<BlogViewModel> listBlog = (from b in context.blogs
                                   join u in context.users on b.user_id equals u.id
                                   orderby b.created_at descending
                                   select new BlogViewModel
                                   {
                                       id = b.id,
                                       title = b.title,
                                       summary = b.summary,
                                       thumbnail = b.thumbnail,
                                       content = b.content,
                                       like_count = b.like_count,
                                       comment_count = b.comment_count,
                                       created_at = b.created_at,
                                       updated_at = b.updated_at,
                                       fullname = u.last_name + " " + u.first_name,
                                       blue_tick = u.blue_tick,
                                       avatar = u.avatar,
                                   }).Take(10).ToList();
            ViewBag.blogs = listBlog;
            // Top users
            List<user> listTopUsers = context.users.
                OrderByDescending(user => user.collection_points).Take(5).ToList();
            ViewBag.topUsers = listTopUsers;
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