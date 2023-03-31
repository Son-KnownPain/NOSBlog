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
        NOSBlogEntities context = new NOSBlogEntities();

        public ActionResult Index(int? page)
        {
            int currentPage = (page ?? 1) < 1 ? 1 : page ?? 1;
            int perPage = 5;
            int offsetNumber = (currentPage - 1 >= 0 ? currentPage - 1 : 0) * 5;
            NOSBlogEntities context = new NOSBlogEntities();
            List<BlogViewModel> listBlog = (from b in context.blogs
                                   join u in context.users on b.user_id equals u.id
                                   orderby b.id descending
                                   select new BlogViewModel
                                   {
                                       id = b.id,
                                       user_id = u.id,
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
                                   }).Skip(offsetNumber).Take(perPage).ToList();
            Dictionary<string, int> paginationInfo = new Dictionary<string, int>();
            paginationInfo.Add("currentPage", currentPage);
            int maxPage = (int) Math.Ceiling((decimal) context.blogs.Count() / perPage);
            paginationInfo.Add("maxPage", maxPage);
            ViewBag.pagination = paginationInfo;
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

        // GET /Home/Search
        public ActionResult Search()
        {
            return View();
        }

        // POST /Home/Search
        [HttpPost]
        public ActionResult Search(string[] includes, string searchValue)
        {
            foreach (var include in includes)
            {
                switch(include)
                {
                    case "blogs":
                        ViewBag.blogs = context.blogs.Where(b => b.title.Contains(searchValue)).ToList();
                        break;
                    case "users":
                        ViewBag.users = context.users.Where(u => (u.last_name + " " + u.first_name).Contains(searchValue)).ToList();
                        break;
                    case "items":
                        ViewBag.items = context.items.Where(i => i.name.Contains(searchValue)).ToList();
                        break;
                    default:
                        break;
                }
            }
            return View();
        }
    }
}