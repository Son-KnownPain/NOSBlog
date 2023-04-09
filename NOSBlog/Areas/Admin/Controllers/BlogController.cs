using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NOSBlog.Auths;
using NOSBlog.Filters;
using NOSBlog.Models;


namespace NOSBlog.Areas.Admin.Controllers
{
    [AdminAuthorization]
    public class BlogController : Controller
    {
        NOSBlogEntities context = new NOSBlogEntities();

        // GET: Admin/Blog
        public ActionResult Index()
        {
            List<BlogViewModel> listBlog = (from b in context.blogs
                                            join u in context.users on b.user_id equals u.id
                                            orderby b.created_at descending
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
                                            }).ToList();
            ViewBag.blogs = listBlog;
            return View();
        }

        // GET /Admin/Blog/Delete
        [HttpGet]
        public ActionResult Delete(int? blogId)
        {
            if (blogId == null) return RedirectToAction("Index");
            blog blogToRemove = context.blogs.FirstOrDefault(blog => blog.id == blogId);
            if (blogToRemove == null) return RedirectToAction("Index");

            // Remove relationship record of blog
            List<Int32> listCmtIDInBlog = context.comments.Where(cmt => cmt.blog_id == blogToRemove.id)
                .Select(cmt => cmt.id).ToList();
            // 1. Remove user_like_comments records
            context.user_like_comments.RemoveRange(context.user_like_comments.Where(rd => listCmtIDInBlog.Contains(rd.comment_id)));
            // 2. Remove user_like_blogs records
            context.user_like_blogs.RemoveRange(context.user_like_blogs.Where(cmt => cmt.blog_id == blogToRemove.id));
            // 3. Remove comments of blog
            context.comments.Where(cmt => cmt.blog_id == blogToRemove.id).ToList()
                .ForEach(cmt => context.comments.Remove(cmt));
            // Remove thumbnail of blog
            String uploadsFolderPath = Server.MapPath("~/Uploads");
            if (System.IO.File.Exists(uploadsFolderPath + "/" + blogToRemove.thumbnail))
            {
                System.IO.File.Delete(uploadsFolderPath + "/" + blogToRemove.thumbnail);
            }
            // Remove record in database
            context.blogs.Remove(blogToRemove);
            context.SaveChanges();

            TempData["RemoveBlog"] = "Successfully remove your blog";

            return RedirectToAction("Index");
        }
    }
}