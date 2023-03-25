using NOSBlog.Auths;
using NOSBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NOSBlog.Areas.Admin.Controllers
{
    public class CommentController : Controller
    {
        NOSBlogEntities context = new NOSBlogEntities();

        // GET: Admin/Comment
        public ActionResult Index()
        {
            if (!UserLogin.IsAdmin) return Redirect("/");
            ViewBag.comments = (from c in context.comments
                                join u in context.users on c.user_id equals u.id
                                orderby c.id descending
                                select new CommentViewModel
                                {
                                    comment_id = c.id,
                                    user_id = u.id,
                                    content = c.content,
                                    created_at = c.created_at,
                                    like_count = c.like_count,
                                    fullname = u.last_name + " " + u.first_name,
                                    blue_tick = u.blue_tick,
                                    avatar = u.avatar,
                                }).ToList();
            return View();
        }

        // GET /Admin/Comment/Delete
        [HttpGet]
        public ActionResult Delete(int? commentId)
        {
            if (!UserLogin.IsAdmin) return Redirect("/");
            comment commentToDelete = context.comments.FirstOrDefault(c => c.id == commentId);
            if (commentToDelete != null)
            {
                blog blogComment = context.blogs.FirstOrDefault(blog => blog.id == commentToDelete.blog_id);

                if (blogComment == null)
                {
                    return RedirectToAction("Index");
                }

                context.user_like_comments.RemoveRange(context.user_like_comments.Where(rd => rd.comment_id == commentId));
                blogComment.comment_count -= 1;

                context.comments.Remove(commentToDelete);

                context.SaveChanges();

                TempData["Success"] = "Successfully to delete comment";
            }

            return RedirectToAction("Index");
        }
    }
}